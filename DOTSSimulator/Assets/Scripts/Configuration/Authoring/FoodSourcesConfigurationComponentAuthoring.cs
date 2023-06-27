using Simulator.Configuration.Components;
using Unity.Entities;
using UnityEngine;

namespace Simulator.Configuration.Authoring
{
    public class FoodSourcesConfigurationComponentAuthoring : MonoBehaviour
    {
        public int NumberOfFoodSources;
        public double EnergyLevel;
        public double RegenerationRate;
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
                EnergyLevel = authoring.EnergyLevel,
                RegenerationRate = authoring.RegenerationRate,
                MaxEnergyLevel = authoring.MaxEnergyLevel,
                FeedingRadius = authoring.FeedingRadius
            });
        }
    }
}