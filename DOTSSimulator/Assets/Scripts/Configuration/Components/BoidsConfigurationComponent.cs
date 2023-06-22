using Unity.Entities;

namespace Simulator.Configuration.Components
{
    public struct BoidsConfigurationComponent: IComponentData
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
}