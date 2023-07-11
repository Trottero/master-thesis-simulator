using Simulator.Configuration.Components;
using Unity.Entities;
using Unity.Physics.Systems;

namespace Simulator.Framework
{
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

            var fixedSimulationSystemGroup = World.GetOrCreateSystemManaged<FixedStepSimulationSystemGroup>();
            fixedSimulationSystemGroup.RateManager = new RateUtils.FixedRateSimpleManager(_simulationFrameworkConfiguration.UpdateInterval);
            
            var physicsSystemGroup = World.GetOrCreateSystemManaged<PhysicsSystemGroup>();
            physicsSystemGroup.RateManager = new RateUtils.FixedRateSimpleManager(_simulationFrameworkConfiguration.UpdateInterval);
            
            var frameworkFixedGroup = World.GetOrCreateSystemManaged<FrameworkFixedSystemGroup>();
            frameworkFixedGroup.RateManager = new RateUtils.FixedRateSimpleManager(_simulationFrameworkConfiguration.UpdateInterval);

            base.OnStartRunning();
        }

        protected override void OnUpdate()
        {
        }
    }
}