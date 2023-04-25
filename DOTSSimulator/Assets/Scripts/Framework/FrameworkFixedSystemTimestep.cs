using Simulator.Configuration;
using Unity.Entities;

namespace Framework
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial class FrameworkFixedSystemTimestep: SystemBase
    {
        private SimulationConfigurationComponent _simulationConfiguration;

        protected override void OnStartRunning()
        {
            _simulationConfiguration = SystemAPI.GetSingleton<SimulationConfigurationComponent>();

            var system = World.GetOrCreateSystemManaged<FixedStepSimulationSystemGroup>();
            system.Timestep = _simulationConfiguration.EffectiveFixedSystemTimestep;
            
            base.OnStartRunning();
        }

        protected override void OnUpdate()
        {
        }
    }
}