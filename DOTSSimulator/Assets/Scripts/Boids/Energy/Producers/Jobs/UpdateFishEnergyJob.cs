using Simulator.Boids.Energy.Producers.Components;
using Simulator.Configuration.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine.Serialization;

namespace Simulator.Boids.Energy.Producers.Jobs
{
    [BurstCompile]
    public partial struct UpdateFishEnergyJob : IJobEntity
    {
        public NativeArray<FoodSourceComponent> FoodSourceInformation;
        [ReadOnly] public NativeArray<LocalToWorld> FoodSourceLocations;
        [ReadOnly] public EnergyConfigurationComponent EnergyConfig;
        [FormerlySerializedAs("SimulationConfig")] [ReadOnly] public SimulationFrameworkConfigurationComponent SimulationFrameworkConfig;

        void Execute(ref EnergyComponent boidEnergy, in LocalToWorld boidLocation)
        {
            for (var i = 0; i < FoodSourceInformation.Length; i++)
            {
                var foodSource = FoodSourceInformation[i];
                if (foodSource.EnergyLevel <= 5)
                {
                    continue;
                }

                var foodSourceLocation = FoodSourceLocations[i];

                var distance = foodSource.EffectiveDistance(boidLocation.Position, foodSourceLocation.Position);
                if (distance < 1f)
                {
                    // Assimilation rate is 1f
                    var consumed = (decimal)EnergyConfig.FeedingRate * (decimal)SimulationFrameworkConfig.UpdateInterval;
                    // Add energy to boid
                    boidEnergy.Weight += consumed;
                    // Remove energy from food source
                    foodSource.EnergyLevel -= consumed;
                }

                FoodSourceInformation[i] = foodSource;
            }
        }
    }
}