using Simulator.Configuration;
using Simulator.Configuration.Components;
using Simulator.Framework;
using Unity.Collections;
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
            _noEnergyQuery = GetEntityQuery(typeof(NoEnergyComponent));
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
            Entities.WithAll<BoidComponent, EnergyComponent>().ForEach((ref EnergyComponent energy) => energy.Weight -= cr * dt).Run();

            var ecb = new EntityCommandBuffer(Allocator.TempJob);
            // // Delete enties with energy level below 0
            Entities.WithAll<BoidComponent, EnergyComponent>().WithNone<NoEnergyComponent>().WithoutBurst().ForEach((Entity e, in EnergyComponent energy) =>
                {
                    if (energy.Weight < 0)
                    {
                        ecb.AddComponent<NoEnergyComponent>(e);
                    }
                }).Run();

            ecb.DestroyEntity(_noEnergyQuery);
            ecb.Playback(EntityManager);
            ecb.Dispose();

        }
    }
}