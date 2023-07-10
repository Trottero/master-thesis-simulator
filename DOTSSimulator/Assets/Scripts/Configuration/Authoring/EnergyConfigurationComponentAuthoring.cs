using Simulator.Configuration.Components;
using Unity.Entities;
using UnityEngine;

namespace Simulator.Configuration.Authoring
{
    public class EnergyConfigurationComponentAuthoring : MonoBehaviour
    {
        public double InitialEnergyLevel;
        public double ConsumptionRate;
        public double FeedingRate;
        public EnergyEquationType EnergyEquation;

    }
    public class EnergyConfigurationComponentBaker : Baker<EnergyConfigurationComponentAuthoring>
    {
        public override void Bake(EnergyConfigurationComponentAuthoring authoring)
        {
            var configuration = new EnergyConfigurationComponent
            {
                InitialEnergyLevel = authoring.InitialEnergyLevel,
                ConsumptionRate = authoring.ConsumptionRate,
                FeedingRate = authoring.FeedingRate,
                EnergyEquation = authoring.EnergyEquation
            };
            
            var entity = GetEntity(authoring, TransformUsageFlags.None);
            AddComponent(entity, configuration);
        }
    }
}