using Simulator.Configuration;
using Unity.Entities;
using Unity.Physics.Systems;

namespace Framework
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial class FrameworkFixedSystemTimestep: SystemBase
    {
        private SimulationConfigurationComponent _simulationConfiguration;

        protected override void OnStartRunning()
        {
            _simulationConfiguration = SystemAPI.GetSingleton<SimulationConfigurationComponent>();

            var physicsSystemGroup = World.GetOrCreateSystemManaged<PhysicsSystemGroup>();
            physicsSystemGroup.RateManager =
                new RateUtils.FixedRateCatchUpManager(_simulationConfiguration.EffectiveFixedSystemTimestep);
            
            var frameworkFixedGroup = World.GetOrCreateSystemManaged<FrameworkFixedSystemGroup>();
            frameworkFixedGroup.RateManager = new RateUtils.FixedRateCatchUpManager(_simulationConfiguration.EffectiveFixedSystemTimestep);
            
            base.OnStartRunning();
        }

        protected override void OnUpdate()
        {
        }
    }
}