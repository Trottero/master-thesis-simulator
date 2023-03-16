using Simulator.Configuration;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;


namespace Simulator.Boids.Energy.Producers
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial class ProducerSystem : SystemBase
    {
        private SimulationConfigurationComponent _simulationConfiguration;

        private EntityQuery _noPostTransformScaleQuery;

        protected override void OnCreate()
        {
            base.OnCreate();

            _noPostTransformScaleQuery = new EntityQueryBuilder(Allocator.Persistent).WithAll<FoodSourceComponent>()
                .WithNone<PostTransformScale>().Build(this);

            RequireForUpdate<SimulationConfigurationComponent>();
        }

        protected override void OnStartRunning()
        {
            base.OnStartRunning();
            _simulationConfiguration = SystemAPI.GetSingleton<SimulationConfigurationComponent>();
        }

        protected override void OnUpdate()
        {
            var dt = _simulationConfiguration.UpdateInterval;

            // Ensure that all entities that act as a food source have a PostTransformScale component
            // If they don't have it, add it.
            if (_noPostTransformScaleQuery.CalculateEntityCount() > 0)
            {
                var ecb = new EntityCommandBuffer(Allocator.TempJob);
                Entities.WithStoreEntityQueryInField(ref _noPostTransformScaleQuery).ForEach(
                        (ref Entity e, in FoodSourceComponent foodSourceComponent) =>
                        {
                            ecb.AddComponent<PostTransformScale>(e);
                            ecb.SetComponent(e, new PostTransformScale
                            {
                                Value = float3x3.Scale(1, foodSourceComponent.EffectiveSize, 1)
                            });
                        })
                    .Run();

                ecb.Playback(EntityManager);
                ecb.Dispose();
            }

            // Regenerate
            Entities.WithAll<FoodSourceComponent>().ForEach((ref FoodSourceComponent foodSource) =>
            {
                foodSource.EnergyLevel = math.clamp(foodSource.EnergyLevel + foodSource.RegenarationRate * dt, 0f,
                    foodSource.MaxEnergyLevel);
            }).ScheduleParallel();

            // Let scale reflect energy level
            Entities.WithAll<FoodSourceComponent, PostTransformScale>()
                .ForEach((ref PostTransformScale scale, in FoodSourceComponent foodSource) =>
                {
                    scale.Value = float3x3.Scale(new float3(1, foodSource.EffectiveSize, 1));
                }).ScheduleParallel();
        }
    }
}