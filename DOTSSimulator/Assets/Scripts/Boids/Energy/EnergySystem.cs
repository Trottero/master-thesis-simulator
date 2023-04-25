using Simulator.Configuration;
using Simulator.Curves;
using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

namespace Simulator.Boids.Energy
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
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
            base.OnStartRunning();
            _simulationConfiguration = SystemAPI.GetSingleton<SimulationConfigurationComponent>();

            var system = World.GetOrCreateSystemManaged<FixedStepSimulationSystemGroup>();
            system.Timestep = _simulationConfiguration.EffectiveUpdatesPerSecond;
        }

        protected override void OnUpdate()
        {
            if (!_controller)
            {
                _controller = BoidController.Instance;
                return;
            }

            var dt = _simulationConfiguration.UpdateInterval;
            var cr = _controller.configuration.EnergyConfig.ConsumptionRate;

            // Update energy level
            Entities.WithAll<BoidComponent, EnergyComponent>()
                .ForEach((ref EnergyComponent energy) => energy.EnergyLevel -= cr * dt).Run();

            var ecb = new EntityCommandBuffer(Allocator.TempJob);
            // // Delete enties with energy level below 0
            Entities.WithAll<BoidComponent, EnergyComponent>().WithNone<NoEnergyComponent>().WithoutBurst().ForEach((Entity e, in EnergyComponent energy) =>
                {
                    if (energy.EnergyLevel < 0)
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