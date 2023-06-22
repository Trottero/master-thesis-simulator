using Unity.Entities;

namespace Simulator.Configuration.Components
{
    public struct SimulationFrameworkConfigurationComponent : IComponentData
    {
        public float UpdatesPerSecond;
        public float UpdateInterval => 1f / UpdatesPerSecond;
        public float EffectiveFixedSystemTimestep => UpdateInterval * (1f / MaxSimulationSpeed);
        public float MaxSimulationSpeed;
    }
}