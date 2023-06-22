using Unity.Entities;

namespace Simulator.Configuration.Components
{
    public struct FoodSourcesConfigurationComponent : IComponentData
    {
        public int NumberOfFoodSources;
        public Entity FoodSourcePrefab;
    }
}