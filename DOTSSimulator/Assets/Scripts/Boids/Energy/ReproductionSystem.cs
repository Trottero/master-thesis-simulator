using Simulator.Configuration;
using Simulator.Curves;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Rendering;
using UnityEngine;

namespace Simulator.Boids.Energy
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial class ReproductionSystem : SystemBase
    {
        private BoidController controller;
        private Entity _gameControllerEntity;
        private SimulationConfigurationComponent _simulationConfiguration;

        private EntityQuery _shouldReproduceQuery;

        protected override void OnCreate()
        {
            _shouldReproduceQuery = GetEntityQuery(ComponentType.ReadOnly<ShouldReproduceComponent>());
        }

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
            // if (!controller)
            // {
            //     controller = BoidController.Instance;
            //     return;
            // }

            // var reproductionConfig = controller.configuration.ReproductionConfig;

            // // Mark entities with energy level above threshold for reproduction
            // var ecb = new EntityCommandBuffer(Allocator.TempJob);
            // Entities.WithAll<BoidComponent, EnergyComponent>().WithNone<ShouldReproduceComponent>().WithoutBurst().ForEach((Entity e, in EnergyComponent energy) =>
            //     {
            //         if (energy.EnergyLevel > reproductionConfig.ReproductionThreshold)
            //         {
            //             ecb.AddComponent<ShouldReproduceComponent>(e);
            //         }
            //     }).Run();

            // ecb.Playback(EntityManager);
            // ecb.Dispose();

            // Entities.WithAll<BoidComponent, EnergyComponent, ShouldReproduceComponent>().WithoutBurst().ForEach((ref EnergyComponent energy) =>
            //     {
            //         if (energy.EnergyLevel > reproductionConfig.ReproductionThreshold)
            //         {
            //             energy.EnergyLevel -= reproductionConfig.ReproductionCost;
            //         }
            //     }).Run();


            // if (_shouldReproduceQuery.CalculateEntityCount() != 0)
            // {
            //     Debug.Log("Reproducing " + _shouldReproduceQuery.CalculateEntityCount() + " boids");

            //     // Reproduce
            //     ecb = new EntityCommandBuffer(Allocator.TempJob);
            //     var prototype = BoidSpawningHelper.SpawnPrototype(EntityManager);
            //     BoidSpawningHelper.SpawnBoids(ecb, prototype, _shouldReproduceQuery.CalculateEntityCount(), 10f);

            //     ecb.RemoveComponentForEntityQuery<ShouldReproduceComponent>(_shouldReproduceQuery);

            //     ecb.Playback(EntityManager);
            //     ecb.Dispose();

            //     EntityManager.DestroyEntity(prototype);
            // }
        }
    }
}