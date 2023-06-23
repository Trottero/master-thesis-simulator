using Simulator.Configuration.Components;
using Unity.Entities;
using UnityEngine;

namespace Simulator.Configuration.Authoring
{
    public class FoodSourcesConfigurationComponentAuthoring : MonoBehaviour
    {
        public int NumberOfFoodSources;
        public double EnergyLevel;
        public double RegenarationRate;
        public double MaxEnergyLevel;
        public float FeedingRadius;
    }
    
    public class FoodSourcesConfigurationComponentBaker : Baker<FoodSourcesConfigurationComponentAuthoring>
    {
        public override void Bake(FoodSourcesConfigurationComponentAuthoring authoring)
        {
            var entity = GetEntity(authoring, TransformUsageFlags.None);
            
            AddComponent(entity, new FoodSourcesConfigurationComponent
            {
                NumberOfFoodSources = authoring.NumberOfFoodSources,
                EnergyLevel = (decimal)authoring.EnergyLevel,
                RegenarationRate = (decimal)authoring.RegenarationRate,
                MaxEnergyLevel = (decimal)authoring.MaxEnergyLevel,
                FeedingRadius = authoring.FeedingRadius
            });
        }
    }
}