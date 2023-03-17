using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;
using Unity.Mathematics;

namespace Simulator.Boids.Energy.Producers
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