using UnityEngine;
using System;

namespace Simulator.Boids
{

    public partial class BoidController : MonoBehaviour
    {
        public float CageSize = 10f;
        public int SwarmSize = 100;

        [SerializeField] public Mesh BoidMesh;
        [SerializeField] public UnityEngine.Material BoidMaterial;
        [SerializeField] public GlobalConfiguration configuration;

        public static BoidController Instance;

        private void Awake()
        {
            Instance = this;
        }

        private void OnDrawGizmos()
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