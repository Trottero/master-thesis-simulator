using UnityEngine;
using Unity.Entities;
using Unity.Rendering;
using System;
using Unity.Physics;
using Collider = Unity.Physics.Collider;
using Simulator.Boids.Energy;
using Unity.Collections;
using Simulator.Boids.Lifecycle;

namespace Simulator.Boids
{

    public partial class BoidController : MonoBehaviour
    {
        public float CageSize = 10f;
        public int SwarmSize = 100;

        [SerializeField] public Mesh BoidMesh;
        [SerializeField] public UnityEngine.Material BoidMaterial;
        [SerializeField] public GlobalConfiguration configuration;

        public static BoidController Instance = null;

        void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            // Debug.Log("Starting BoidController");
            // var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            // Debug.Log("Attempting to spawn prototype");
            // var prototype = BoidSpawningHelper.SpawnPrototype(entityManager);

            // var ecb = new EntityCommandBuffer(Allocator.Persistent);
            // BoidSpawningHelper.SpawnBoids(ecb, prototype, SwarmSize, CageSize);

            // ecb.Playback(entityManager);
            // ecb.Dispose();
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(CageSize, CageSize, CageSize));
        }
    }


    [Serializable]
    public class GlobalConfiguration
    {
        public BoidsConfiguration BoidConfig;
        public EnergyConfiguration EnergyConfig;
        public ReproductionConfiguration ReproductionConfig;
    }

    [Serializable]
    public struct BoidsConfiguration
    {
        public float Speed;
        public float RotationSpeed;
        public float PerceptionRange;
        public float SeperationWeight;
        public float CohesionWeight;
        public float AlignmentWeight;
        public float StayInCubeWeight;
        public float FoodSourceWeight;
    }

    [Serializable]
    public struct ReproductionConfiguration
    {
        public float ReproductionThreshold;
        public float ReproductionCost;
    }

    [Serializable]
    public struct EnergyConfiguration
    {
        public float InitialEnergyLevel;
        public float ConsumptionRate;
        public float AssimilationRate;
    }
}