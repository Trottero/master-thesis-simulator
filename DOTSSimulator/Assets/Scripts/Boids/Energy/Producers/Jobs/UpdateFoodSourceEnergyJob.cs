using Simulator.Boids.Energy.Producers.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Simulator.Boids.Energy.Producers.Jobs
{
    [BurstCompile]
    public partial struct UpdateFoodSourceEnergyJob : IJobEntity
    {
        [ReadOnly] public NativeArray<FoodSourceComponent> FoodSourceInformation;
        void Execute([EntityIndexInQuery] int entityInQueryIndex, ref FoodSourceComponent fc)
        {
            fc = FoodSourceInformation[entityInQueryIndex];
        }
    }
}