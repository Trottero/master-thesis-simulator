using Unity.Entities;
using UnityEngine;

namespace Simulator.Configuration
{
    public class SimulationConfiguration : MonoBehaviour
    {
        public float UpdatesPerSecond = 30;
        public float MaxSimulationSpeed = 8;
    }

    public class ConfigurationBaker : Baker<SimulationConfiguration>
    {
        public override void Bake(SimulationConfiguration authoring)
        {
            var configuration = new SimulationConfigurationComponent
            {
                UpdatesPerSecond = authoring.UpdatesPerSecond,
                MaxSimulationSpeed = authoring.MaxSimulationSpeed
            };

            Debug.Log(
                $"Current Configuration: UpdatesPerSecond: {authoring.UpdatesPerSecond}, MaxSimulationSpeed: {authoring.MaxSimulationSpeed}");
            var entity = GetEntity(authoring, TransformUsageFlags.None);
            AddComponent(entity, configuration);
        }
    }
}