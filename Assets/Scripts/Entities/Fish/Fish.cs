using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FishBowlGame.Entities
{
    public class Fish : MonoBehaviour
    {

        private GeneralFishSettings _generalFishSettings => _fishManager.GeneralFishSettings;
        private SwarmSettings _swarmSettings => _fishManager.SwarmSettings;
        private NavigationSettings _navigationSettings => _fishManager.NavigationSettings;
        private EnergySettings _energySettings => _fishManager.EnergySettings;

        private FishManager _fishManager;
        private Vector3[] _precomputedSpherePoints;
        private RaycastHit[] _hitsAllocated;
        private RaycastHit[] _internalHitsAllocated = new RaycastHit[3];
        private int _numberOfHits = 0;
        private Collider _selfCollider;
        private Rigidbody _selfRigidbody;

        private Vector3 _cohesion, _alignment, _separation, _evasion, _food, _optimalDirection;
        private int _hitCount = 0;
        public float EnergyLevel = 100;

        void Start()
        {
            resetVectors();

            _fishManager = FindObjectOfType<FishManager>();
            _selfRigidbody = GetComponent<Rigidbody>();
            _precomputedSpherePoints = GetPointsOnSphere(_generalFishSettings.PerceptionResolution);
            _hitsAllocated = new RaycastHit[_generalFishSettings.PerceptionResolution];

            _selfCollider = GetComponentInChildren<Collider>();
        }

        void resetVectors()
        {
            _cohesion = _alignment = _separation = _evasion = _food = _optimalDirection = Vector3.zero;
            _hitCount = 0;
        }

        void FixedUpdate()
        {
            // percept();

            var fishDirection = calculateDirectionVector();

            // Apply the movement
            var singleStep = _generalFishSettings.Speed * Time.fixedDeltaTime;
            var smoothedRotation = Vector3.RotateTowards(transform.forward, fishDirection, singleStep, 0f);
            _selfRigidbody.MoveRotation(Quaternion.LookRotation(smoothedRotation));
            _selfRigidbody.MovePosition(transform.position + transform.forward * _generalFishSettings.Speed * Time.fixedDeltaTime);
            // transform.rotation = Quaternion.LookRotation(smoothedRotation);
            // transform.position += transform.forward * _generalFishSettings.Speed * Time.deltaTime;
        }


        private void percept()
        {
            // Find all other fish in a sphere
            _numberOfHits = GoodSphereRaycastNonAlloc(transform.position, _generalFishSettings.PerceptionRange, _hitsAllocated, _internalHitsAllocated);
        }

        private Vector3 calculateDirectionVector()
        {
            resetVectors();
            for (int i = 0; i < _fishManager.Fish.Count; i++)
            {
                if (_fishManager.Fish[i] == this)
                {
                    continue;
                }

                var direction = _fishManager.Fish[i].transform.position - transform.position;
                if (direction.magnitude > _generalFishSettings.PerceptionRange)
                {
                    continue;
                }

                if (Vector3.Dot(transform.forward, direction.normalized) < _generalFishSettings.PerceptionDot)
                {
                    continue;
                }

                var scaledDistance = direction.magnitude / _generalFishSettings.PerceptionRange;
                _cohesion += _swarmSettings.CohesionCurve.Evaluate(scaledDistance) * direction.normalized;
                _alignment += _swarmSettings.AlignmentCurve.Evaluate(scaledDistance) * _fishManager.Fish[i].transform.forward;
                _separation += _swarmSettings.SeparationCurve.Evaluate(scaledDistance) * -direction.normalized;
                _hitCount++;
            }

            return (_cohesion.normalized * _swarmSettings.CohesionWeight
                + _alignment.normalized * _swarmSettings.AlignmentWeight
                + _separation * _swarmSettings.SeparationWeight
                + _evasion.normalized * _navigationSettings.EvasionWeight
                + _food.normalized * _navigationSettings.FoodWeight).normalized;
        }

        private void OnDrawGizmosSelected()
        {
            for (int i = 0; i < _fishManager.Fish.Count; i++)
            {
                if (_fishManager.Fish[i] == this)
                {
                    continue;
                }

                var direction = _fishManager.Fish[i].transform.position - transform.position;
                if (direction.magnitude > _generalFishSettings.PerceptionRange)
                {
                    continue;
                }

                if (Vector3.Dot(transform.forward, direction.normalized) < _generalFishSettings.PerceptionDot)
                {
                    continue;
                }

                Debug.DrawLine(transform.position, _fishManager.Fish[i].transform.position, Color.cyan);
            }

            Debug.DrawLine(transform.position, transform.position + _cohesion, Color.red);
            Debug.DrawLine(transform.position, transform.position + _alignment, Color.green);
            Debug.DrawLine(transform.position, transform.position + _separation, Color.blue);
        }

        private Vector3[] GetPointsOnSphere(int count)
        {
            var points = new Vector3[count];

            // Return all points on the sphere of perception
            for (int i = 0; i < points.Length; i++)
            {
                var phi = Math.Acos(1 - 2 * ((float)i + 0.5) / points.Length);
                var theta = Math.PI * (1 + Math.Sqrt(5)) * (float)i + 0.5; // Golden ratio
                points[i].x = (float)Math.Cos(theta) * (float)Math.Sin(phi);
                points[i].y = (float)Math.Sin(theta) * (float)Math.Sin(phi);
                points[i].z = (float)Math.Cos(phi);
            }

            return points;
        }

        private int GoodSphereRaycastNonAlloc(Vector3 origin, float radius, RaycastHit[] raycastHitBuffer, RaycastHit[] innerBuffer)
        {
            var totalHits = 0;
            var spherePointIndex = 0;
            while (totalHits < _generalFishSettings.PerceptionResolution && spherePointIndex < _generalFishSettings.PerceptionResolution)
            {
                var hits = Physics.RaycastNonAlloc(origin, _precomputedSpherePoints[spherePointIndex], innerBuffer, radius);
                for (int i = 0; i < hits; i++)
                {
                    if (totalHits + i >= _generalFishSettings.PerceptionResolution)
                    {
                        break;
                    }
                    raycastHitBuffer[totalHits + i] = innerBuffer[i];
                    totalHits++;
                }

                spherePointIndex++;
            }
            return totalHits;
        }

        public bool Consume(FoodItem foodItem)
        {
            if (EnergyLevel < 10)
            {
                EnergyLevel += foodItem.Energy;
                return true;
            }
            return false;
        }
    }
}