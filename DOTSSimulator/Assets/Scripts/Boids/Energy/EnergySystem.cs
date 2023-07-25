using Simulator.Configuration;
using Simulator.Configuration.Components;
using Simulator.Framework;
using Unity.Entities;

namespace Simulator.Boids.Energy
{
    [UpdateInGroup(typeof(FrameworkFixedSystemGroup))]
    public partial class EnergySystem : SystemBase
    {
        private GlobalConfigurationComponent _configurationComponent;

        private EntityQuery _noEnergyQuery;

        protected override void OnCreate()
        {
            base.OnCreate();
            RequireForUpdate<GlobalConfigurationComponent>();
        }

        protected override void OnStartRunning()
        {
            _configurationComponent = SystemAPI.GetSingleton<GlobalConfigurationComponent>();
            base.OnStartRunning();
        }

        protected override void OnUpdate()
        {
            if (_configurationComponent.EnergyConfiguration.EnergyEquation != EnergyEquationType.Linear)
            {
                return;
            }

            var dt = (decimal)_configurationComponent.SimulationFrameworkConfiguration.UpdateInterval;
            var cr = (decimal)_configurationComponent.EnergyConfiguration.ConsumptionRate;
            // Update energy level
            Entities.WithAll<BoidComponent, EnergyComponent>().ForEach((ref EnergyComponent energy) => energy.Weight -= cr * dt).ScheduleParallel();

            Entities
                .WithDeferredPlaybackSystem<EndSimulationEntityCommandBufferSystem>()
                .ForEach((Entity entity, EntityCommandBuffer ecb, in EnergyComponent health) =>
                    {
                        if (health.Weight < 0)
                        {
                            ecb.DestroyEntity(entity);
                        }
                    }
                ).ScheduleParallel();
        }
    }
}