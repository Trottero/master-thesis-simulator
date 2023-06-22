using System.Collections;
using System.Collections.Generic;
using Simulator.Boids.Lifecycle;
using Simulator.Configuration.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace Simulator.Boids.Lifecycle
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial class BoidSpawningSystem : SystemBase
    {
        private GlobalConfigurationComponent _configurationComponent;
        private Entity _schoolComponentEntity;
        private SchoolConfigurationComponent _schoolConfigurationComponent ;

        protected override void OnCreate()
        {
            RequireForUpdate<SchoolConfigurationComponent>();
            RequireForUpdate<GlobalConfigurationComponent>();
        }
        
        protected override void OnStartRunning()
        {
            _configurationComponent = SystemAPI.GetSingleton<GlobalConfigurationComponent>();
            _schoolComponentEntity = SystemAPI.GetSingletonEntity<SchoolConfigurationComponent>();
            _schoolConfigurationComponent = EntityManager.GetComponentData<SchoolConfigurationComponent>(_schoolComponentEntity);
        }

        protected override void OnUpdate()
        {
            var ecb = new EntityCommandBuffer(Allocator.TempJob);
            var proto = BoidSpawningHelper.SpawnPrototype(EntityManager, (decimal)_configurationComponent.EnergyConfiguration.InitialEnergyLevel);
            new SpawnBoidsJob
            {
                Prototype = proto,
                Ecb = ecb.AsParallelWriter(),
                CageSize = _schoolConfigurationComponent.CageSize
            }.Schedule(_schoolConfigurationComponent.SwarmSize, 32, Dependency).Complete();

            ecb.Playback(EntityManager);
            ecb.Dispose();

            EntityManager.DestroyEntity(proto);
            EntityManager.DestroyEntity(_schoolComponentEntity);
        }
    }
}