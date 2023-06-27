using Unity.Entities;

namespace Simulator.Configuration.Components
{
    [System.Serializable]
    public struct FoodSourcesConfigurationComponent : IComponentData
    {
        public int NumberOfFoodSources;
        public double EnergyLevel;
        public double RegenerationRate;
        public double MaxEnergyLevel;
        public float FeedingRadius;
    }
}