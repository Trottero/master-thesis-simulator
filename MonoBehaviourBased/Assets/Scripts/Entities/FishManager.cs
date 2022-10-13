using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FishBowlGame.Entities
{
    [Serializable]
    public class SwarmSettings
    {
        public AnimationCurve CohesionCurve;
        public float CohesionWeight = 1f;
        public AnimationCurve AlignmentCurve;
        public float AlignmentWeight = 1f;
        public AnimationCurve SeparationCurve;
        public float SeparationWeight = 1f;
    }

    [Serializable]
    public class NavigationSettings
    {
        public AnimationCurve EvasionCurve;
        public float EvasionWeight = 100f;
        public float EvasionMultiplier = 5f;
        public AnimationCurve FoodCurve;
        public float FoodWeight = 8f;
        public float FoodMultiplier = 1f;
    }

    [Serializable]
    public class GeneralFishSettings
    {
        public float Speed = 1f;
        public float MaxRotationSpeed = 30f;
        public float PerceptionRange = 4f;
        public float PerceptionDot = -1;
        public int PerceptionResolution = 120;
    }

    [Serializable]
    public class EnergySettings
    {
        public float InitialEnergy = 50f;
        public float MaxEnergy = 100f;
        public float EnergyLossPerSecond = 1f;
    }

    public class FishManager : MonoBehaviour
    {
        [SerializeField]
        public GeneralFishSettings GeneralFishSettings;
        [SerializeField]
        public SwarmSettings SwarmSettings;
        [SerializeField]
        public NavigationSettings NavigationSettings;
        [SerializeField]
        public EnergySettings EnergySettings;

        public GameObject prefab;
        public int SwarmSize = 200;

        public List<Fish> Fish = new List<Fish>();

        // Start is called before the first frame update
        void Start()
        {
            Application.targetFrameRate = 120;

            var container = new GameObject("FishSwarm");

            // Spawn the prefab with random rotation and position within -12 to 12
            for (int i = 0; i < SwarmSize; i++)
            {
                var randomPos = new Vector3(Random.Range(-12f, 12f), Random.Range(-12f, 12f), Random.Range(-12f, 12f));
                GameObject go = Instantiate(prefab, randomPos, Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)), container.transform);
                var f = go.GetComponent<Fish>();
                Fish.Add(f);
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
