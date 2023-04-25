using Unity.Entities;

namespace Simulator.Configuration
{
    public struct SimulationConfigurationComponent : IComponentData
    {
        public float UpdatesPerSecond;
        public float UpdateInterval => 1f / UpdatesPerSecond;
        public float EffectiveFixedSystemTimestep => UpdateInterval * (1f / MaxSimulationSpeed);
        public float MaxSimulationSpeed;
    }
}
