using Unity.Entities;

namespace Simulator.Configuration.Components
{
    public struct FoodSourcesConfigurationComponent : IComponentData
    {
        public int NumberOfFoodSources;
        public decimal EnergyLevel;
        public decimal RegenarationRate;
        public decimal MaxEnergyLevel;
        public float FeedingRadius;
    }
}