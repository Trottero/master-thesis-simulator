using Unity.Entities;

namespace Simulator.Configuration.Components
{
    public struct RegisteredPrefabsComponent : IComponentData
    {
        public Entity FoodSourcePrefab;
    }
}