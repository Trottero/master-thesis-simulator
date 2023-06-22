using Simulator.Boids;
using Simulator.Boids.Energy;
using Simulator.Boids.Lifecycle;
using Simulator.Configuration.Components;
using Simulator.Framework;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace Boids.Energy
{
    [UpdateInGroup(typeof(FrameworkFixedSystemGroup))]
    public partial class ReproductionSystem : SystemBase
    {
        private EntityQuery _shouldReproduceQuery;
        private GlobalConfigurationComponent _configurationComponent;

        protected override void OnCreate()
        {
            base.OnCreate();
            _shouldReproduceQuery = GetEntityQuery(ComponentType.ReadOnly<ShouldReproduceComponent>());
            RequireForUpdate<GlobalConfigurationComponent>();
        }

        protected override void OnStartRunning()
        {
            base.OnStartRunning();
            _configurationComponent = SystemAPI.GetSingleton<GlobalConfigurationComponent>();
        }

        protected override void OnUpdate()
        {
            // Do not update system if disabled.
            if (!_configurationComponent.ReproductionConfiguration.ReproductionEnabled)
            {
                return;
            }

            var reproductionConfig = _configurationComponent.ReproductionConfiguration;

            // Mark entities with energy level above threshold for reproduction
            var ecb = new EntityCommandBuffer(Allocator.TempJob);
            Entities.WithAll<BoidComponent, EnergyComponent>().WithNone<ShouldReproduceComponent>().WithoutBurst().ForEach((Entity e, in EnergyComponent energy) =>
            {
                // Check if said entity has enough energy
                if (energy.Weight <= (decimal)reproductionConfig.MinWeightForReproduction) return;
                
                ecb.AddComponent<ShouldReproduceComponent>(e);
                ecb.SetComponent(e, new EnergyComponent
                {
                    Weight = energy.Weight - (decimal)reproductionConfig.ReproductionWeightLoss
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
            var proto = BoidSpawningHelper.SpawnPrototype(EntityManager, (decimal)reproductionConfig.OffspringWeight);
            
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