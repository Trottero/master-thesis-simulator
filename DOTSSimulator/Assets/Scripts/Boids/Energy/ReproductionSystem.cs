using Simulator.Boids;
using Simulator.Boids.Energy;
using Simulator.Boids.Lifecycle;
using Simulator.Configuration;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace Boids.Energy
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial class ReproductionSystem : SystemBase
    {
        private BoidController _controller;
        private Entity _gameControllerEntity;
        private SimulationConfigurationComponent _simulationConfiguration;

        private EntityQuery _shouldReproduceQuery;

        protected override void OnCreate()
        {
            base.OnCreate();

            _shouldReproduceQuery = GetEntityQuery(ComponentType.ReadOnly<ShouldReproduceComponent>());
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

            var reproductionConfig = _controller.configuration.ReproductionConfig;

            // Mark entities with energy level above threshold for reproduction
            var ecb = new EntityCommandBuffer(Allocator.TempJob);
            Entities.WithAll<BoidComponent, EnergyComponent>().WithNone<ShouldReproduceComponent>().WithoutBurst().ForEach((Entity e, in EnergyComponent energy) =>
            {
                // Check if said entity has enough energy
                if (energy.EnergyLevel <= reproductionConfig.ReproductionThreshold) return;
                
                ecb.AddComponent<ShouldReproduceComponent>(e);
                ecb.SetComponent(e, new EnergyComponent
                {
                    EnergyLevel = energy.EnergyLevel - reproductionConfig.ReproductionCost
                });
            }).Run();

            ecb.Playback(EntityManager);
            ecb.Dispose();

            if (_shouldReproduceQuery.CalculateEntityCount() == 0)
            {
                return;
            }
            
            Debug.Log("Reproducing " + _shouldReproduceQuery.CalculateEntityCount() + " boids");

            // Reproduce
            ecb = new EntityCommandBuffer(Allocator.TempJob);
            
            // Spawn simple prototype
            var proto = BoidSpawningHelper.SpawnPrototype(EntityManager);
            
            new SpawnBoidsJob
            {
                Prototype = proto,
                Ecb = ecb.AsParallelWriter(),
                CageSize = 2f
            }.Schedule(_shouldReproduceQuery.CalculateEntityCount(), 32, Dependency).Complete();

            ecb.RemoveComponent<ShouldReproduceComponent>(_shouldReproduceQuery);

            ecb.DestroyEntity(proto);
            ecb.Playback(EntityManager);
            ecb.Dispose();
        }
    }
}