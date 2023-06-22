using Simulator.Configuration.Components;
using Unity.Entities;
using UnityEngine;

namespace Simulator.Configuration.Authoring
{
    public class BoidsConfigurationComponentAuthoring : MonoBehaviour
    {
        public float Speed;
        public float RotationSpeed;
        public float PerceptionRange;
        public float SeparationWeight;
        public float CohesionWeight;
        public float AlignmentWeight;
        public float StayInCubeWeight;
        public float FoodSourceWeight;

    }
    
    public class BoidsConfigurationComponentBaker : Baker<BoidsConfigurationComponentAuthoring>
    {
        public override void Bake(BoidsConfigurationComponentAuthoring authoring)
        {
            var configuration = new BoidsConfigurationComponent
            {
                Speed = authoring.Speed,
                RotationSpeed = authoring.RotationSpeed,
                PerceptionRange = authoring.PerceptionRange,
                SeparationWeight = authoring.SeparationWeight,
                CohesionWeight = authoring.CohesionWeight,
                AlignmentWeight = authoring.AlignmentWeight,
                StayInCubeWeight = authoring.StayInCubeWeight,
                FoodSourceWeight = authoring.FoodSourceWeight
            };
            
            var entity = GetEntity(authoring, TransformUsageFlags.None);
            AddComponent(entity, configuration);
        }
    }
}