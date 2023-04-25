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

            // Regenerate
            Entities.WithAll<FoodSourceComponent>().ForEach((ref FoodSourceComponent foodSource) =>
            {
                foodSource.EnergyLevel = math.clamp(foodSource.EnergyLevel + foodSource.RegenarationRate * dt, 0f,
                    foodSource.MaxEnergyLevel);
            }).ScheduleParallel();

            // Let scale reflect energy level
            Entities.WithAll<FoodSourceComponent, PostTransformMatrix>()
                .ForEach((ref PostTransformMatrix scale, in FoodSourceComponent foodSource) =>
                {
                    scale.Value = float4x4.Scale(new float3(0.1f, foodSource.EffectiveSize, 0.1f));
                }).ScheduleParallel();
        }
    }
}