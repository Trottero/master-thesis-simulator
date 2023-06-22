using Simulator.Configuration.Components;
using Unity.Entities;
using UnityEngine;

namespace Simulator.Configuration.Authoring
{
    public class SimulationFrameworkConfigurationAuthoring : MonoBehaviour
    {
        public float UpdatesPerSecond = 15;
        public float MaxSimulationSpeed = 16;

    }
    public class SimulationConfigurationBaker : Baker<SimulationFrameworkConfigurationAuthoring>
    {
        public override void Bake(SimulationFrameworkConfigurationAuthoring authoring)
        {
            var configuration = new SimulationFrameworkConfigurationComponent
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