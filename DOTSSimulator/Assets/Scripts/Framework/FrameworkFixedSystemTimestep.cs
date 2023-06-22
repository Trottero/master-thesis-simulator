using Simulator.Configuration.Components;
using Unity.Entities;
using Unity.Physics.Systems;

namespace Simulator.Framework
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial class FrameworkFixedSystemTimestep: SystemBase
    {
        private SimulationFrameworkConfigurationComponent _simulationFrameworkConfiguration;

        protected override void OnCreate()
        {
            base.OnCreate();
            RequireForUpdate<SimulationFrameworkConfigurationComponent>();
        }
        
        protected override void OnStartRunning()
        {
            _simulationFrameworkConfiguration = SystemAPI.GetSingleton<SimulationFrameworkConfigurationComponent>();

            var physicsSystemGroup = World.GetOrCreateSystemManaged<PhysicsSystemGroup>();
            physicsSystemGroup.RateManager =
                new RateUtils.FixedRateCatchUpManager(_simulationFrameworkConfiguration.EffectiveFixedSystemTimestep);
            
            var frameworkFixedGroup = World.GetOrCreateSystemManaged<FrameworkFixedSystemGroup>();
            frameworkFixedGroup.RateManager = new RateUtils.FixedRateCatchUpManager(_simulationFrameworkConfiguration.EffectiveFixedSystemTimestep);
            
            base.OnStartRunning();
        }

        protected override void OnUpdate()
        {
        }
    }
}