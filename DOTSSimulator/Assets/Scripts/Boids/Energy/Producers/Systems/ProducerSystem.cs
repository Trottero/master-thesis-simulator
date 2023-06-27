using Simulator.Boids.Energy.Producers.Components;
using Simulator.Configuration.Components;
using Simulator.Framework;
using Simulator.Utils;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Simulator.Boids.Energy.Producers.Systems
{
    [UpdateInGroup(typeof(FrameworkFixedSystemGroup))]
    [UpdateAfter(typeof(BoidsSystem))]
    public partial class ProducerSystem : SystemBase
    {
        private SimulationFrameworkConfigurationComponent _simulationFrameworkConfiguration;

        protected override void OnCreate()
        {
            base.OnCreate();
            RequireForUpdate<GlobalConfigurationComponent>();
        }

        protected override void OnStartRunning()
        {
            base.OnStartRunning();
            _simulationFrameworkConfiguration = SystemAPI.GetSingleton<GlobalConfigurationComponent>().SimulationFrameworkConfiguration;
        }

        protected override void OnUpdate()
        {
            var dt = (decimal)_simulationFrameworkConfiguration.UpdateInterval;

            // Regenerate
            Entities.WithAll<FoodSourceComponent>().ForEach((ref FoodSourceComponent foodSource) =>
            {
                foodSource.EnergyLevel = Mathm.Clamp(foodSource.EnergyLevel + foodSource.RegenerationRate * dt, 0m,
                    foodSource.MaxEnergyLevel);
            }).Run();

            // Let scale reflect energy level
            Entities.WithAll<FoodSourceComponent, PostTransformMatrix>()
            .ForEach((ref PostTransformMatrix scale, in FoodSourceComponent foodSource) =>
            {
                scale.Value = float4x4.Scale(new float3(0.1f, foodSource.EffectiveSize, 0.1f));
            }).Run();
        }
    }
}