using Simulator.Configuration;
using Simulator.Curves;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;


namespace Simulator.Boids.Energy.Producers
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial class ProducerSystem : SystemBase
    {

        private SimulationConfigurationComponent _simulationConfiguration;

        protected override void OnStartRunning()
        {
            base.OnStartRunning();
            var _gameControllerEntity = GetSingletonEntity<BoidControllerTag>();
            _simulationConfiguration = GetComponent<SimulationConfigurationComponent>(_gameControllerEntity);
        }

        protected override void OnUpdate()
        {
            var dt = _simulationConfiguration.UpdateInterval;

            // Regnerate
            Entities.WithAll<FoodSourceComponent>().ForEach((ref FoodSourceComponent foodSource) =>
            {
                foodSource.EnergyLevel = math.clamp(foodSource.EnergyLevel + foodSource.RegenarationRate * dt, 0f, foodSource.MaxEnergyLevel);
            }).ScheduleParallel();

            // Let scale reflect energy level
            Entities.WithAll<FoodSourceComponent, LocalToWorld>().ForEach((ref FoodSourceComponent foodSource, ref LocalToWorld scale) =>
            {
                scale.Value = Matrix4x4.TRS(scale.Position, scale.Rotation, new float3(1, foodSource.EffectiveSize, 1));
            }).ScheduleParallel();
        }

    }
}