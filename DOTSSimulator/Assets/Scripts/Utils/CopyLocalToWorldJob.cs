using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;
using Simulator.Boids.Energy.Producers;
using Simulator.Boids.Energy.Producers.Components;

namespace Simulator.Utils
{
    [BurstCompile]
    public partial struct CopyFoodSourceLocationsJob : IJobEntity
    {
        [WriteOnly] public NativeArray<LocalToWorld> Locations;
        [WriteOnly] public NativeArray<FoodSourceComponent> ProducerComponents;

        void Execute([EntityIndexInQuery] int entityInQueryIndex, in LocalToWorld transform, in FoodSourceComponent producer)
        {
            Locations[entityInQueryIndex] = transform;
            ProducerComponents[entityInQueryIndex] = producer;
        }
    }
}