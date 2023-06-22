using Simulator.Configuration.Components;
using Unity.Entities;
using UnityEngine;

namespace Simulator.Configuration.Authoring
{
    public class FoodSourcesConfigurationComponentAuthoring : MonoBehaviour
    {
        public int NumberOfFoodSources;
        public GameObject FoodSourcePrefab;
    }
    
    public class FoodSourcesConfigurationComponentBaker : Baker<FoodSourcesConfigurationComponentAuthoring>
    {
        public override void Bake(FoodSourcesConfigurationComponentAuthoring authoring)
        {
            var prefabEntity = GetEntity(authoring.FoodSourcePrefab, TransformUsageFlags.Dynamic);
            var entity = GetEntity(authoring, TransformUsageFlags.None);
            
            AddComponent(entity, new FoodSourcesConfigurationComponent
            {
                NumberOfFoodSources = authoring.NumberOfFoodSources,
                FoodSourcePrefab = prefabEntity
            });
        }
    }
}