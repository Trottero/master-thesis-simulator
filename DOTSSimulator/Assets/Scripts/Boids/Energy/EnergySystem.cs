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
        private BoidController controller;
        private Entity _gameControllerEntity;
        private SimulationConfigurationComponent _simulationConfiguration;

        protected override void OnStartRunning()
        {
            base.OnStartRunning();
            _gameControllerEntity = GetSingletonEntity<BoidControllerTag>();
            _simulationConfiguration = GetComponent<SimulationConfigurationComponent>(_gameControllerEntity);

            var system = World.GetOrCreateSystem<FixedStepSimulationSystemGroup>();
            system.Timestep = _simulationConfiguration.EffectiveUpdatesPerSecond;
        }

        protected override void OnUpdate()
        {
            if (!controller)
            {
                controller = BoidController.Instance;
                return;
            }

            var dt = _simulationConfiguration.UpdateInterval;
            var cr = controller.configuration.EnergyConfig.ConsumptionRate;

            // Update energy level
            Entities.WithAll<BoidComponent, RenderMesh, EnergyComponent>().WithoutBurst().ForEach((ref EnergyComponent energy) =>
            {
                energy.EnergyLevel -= controller.configuration.EnergyConfig.ConsumptionRate * dt;
                // mesh.material.color = new Color(1f, 1f, 1f, energy.EnergyLevel);
            }).Run();

            var ecb = new EntityCommandBuffer(Allocator.TempJob);

            // // Delete enties with energy level below 0
            Entities.WithAll<BoidComponent, EnergyComponent>().WithNone<NoEnergyComponent>().WithoutBurst().ForEach((Entity e, in EnergyComponent energy) =>
                {
                    if (energy.EnergyLevel < 0)
                    {
                        ecb.AddComponent<NoEnergyComponent>(e);
                    }
                }).Run();

            ecb.DestroyEntitiesForEntityQuery(GetEntityQuery(typeof(NoEnergyComponent)));
            ecb.Playback(EntityManager);
            ecb.Dispose();

        }
    }
}