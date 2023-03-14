using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;
using Unity.Mathematics;
using Simulator.Configuration;

namespace Simulator.Boids.Energy.Producers
{
    [BurstCompile]
    public partial struct UpdateFishEnergyJob : IJobEntity
    {
        public NativeArray<FoodSourceComponent> FoodSourceInformation;
        public NativeArray<LocalToWorld> FoodSourceLocations;
        public EnergyConfiguration EnergyConfig;
        public SimulationConfigurationComponent SimulationConfig;
        void Execute(ref EnergyComponent boidEnergy, in LocalToWorld boidLocation)
        {
            for (int i = 0; i < FoodSourceInformation.Length; i++)
            {
                var foodSource = FoodSourceInformation[i];
                if (foodSource.EnergyLevel <= 5)
                {
                    continue;
                }
                var foodSourceLocation = FoodSourceLocations[i];

                float distance = foodSource.EffectiveDistance(boidLocation.Position, foodSourceLocation.Position);
                if (distance < 1f)
                {
                    // Assimilation rate is 1f
                    float consumed = EnergyConfig.AssimilationRate * SimulationConfig.UpdateInterval;
                    // Add energy to boid
                    boidEnergy.EnergyLevel += consumed;
                    // Remove energy from food source
                    foodSource.EnergyLevel -= consumed;
                }
                FoodSourceInformation[i] = foodSource;
            }
        }
    }
}