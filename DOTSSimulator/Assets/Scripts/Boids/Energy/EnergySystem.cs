using Framework;
using Simulator.Configuration;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Simulator.Boids.Energy
{
    [UpdateInGroup(typeof(FrameworkFixedSystemGroup))]
    public partial class EnergySystem : SystemBase
    {
        private BoidController _controller;
        private SimulationConfigurationComponent _simulationConfiguration;

        private EntityQuery _noEnergyQuery;

        protected override void OnCreate()
        {
            base.OnCreate();
            _noEnergyQuery = GetEntityQuery(typeof(NoEnergyComponent));
            RequireForUpdate<SimulationConfigurationComponent>();

        }

        protected override void OnStartRunning()
        {
            _simulationConfiguration = SystemAPI.GetSingleton<SimulationConfigurationComponent>();
            base.OnStartRunning();
        }

        protected override void OnUpdate()
        {
            if (!_controller)
            {
                _controller = BoidController.Instance;
                return;
            }

            var dt = (decimal)_simulationConfiguration.UpdateInterval;
            var cr = (decimal)_controller.configuration.EnergyConfig.ConsumptionRate;
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