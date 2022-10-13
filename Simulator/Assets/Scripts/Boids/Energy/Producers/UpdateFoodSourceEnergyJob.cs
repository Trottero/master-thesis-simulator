using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;
using Unity.Mathematics;

namespace Simulator.Boids.Energy.Producers
{
    public partial struct UpdateFoodSourceEnergyJob : IJobEntity
    {
        public NativeArray<FoodSourceComponent> FoodSourceInformation;
        void Execute([EntityInQueryIndex] int entityInQueryIndex, ref FoodSourceComponent fc)
        {
            fc = FoodSourceInformation[entityInQueryIndex];
        }
    }
}