using Simulator.Configuration.Components;
using Simulator.Framework;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace Simulator.Boids.Lifecycle
{
    [UpdateInGroup(typeof(FrameworkFixedSystemGroup))]
    public partial class BoidSpawningSystem : SystemBase
    {
        private GlobalConfigurationComponent _configurationComponent;
        private SchoolConfigurationComponent _schoolConfigurationComponent;

        protected override void OnCreate()
        {
            RequireForUpdate<GlobalConfigurationComponent>();
        }
        
        protected override void OnStartRunning()
        {
            base.OnStartRunning();
            
            _configurationComponent = SystemAPI.GetSingleton<GlobalConfigurationComponent>();
            _schoolConfigurationComponent = _configurationComponent.SchoolConfiguration;
            
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
        }

        protected override void OnUpdate()
        {
        }
    }
}