using Unity.Entities;
using UnityEngine;

namespace Simulator.Configuration
{
    public class SimulationConfigurationMono : MonoBehaviour
    {
        public float UpdatesPerSecond = 15;
        public float MaxSimulationSpeed = 16;
    }
    
    public struct SimulationConfigurationComponent : IComponentData
    {
        public float UpdatesPerSecond;
        public float UpdateInterval => 1f / UpdatesPerSecond;
        public float EffectiveFixedSystemTimestep => UpdateInterval * (1f / MaxSimulationSpeed);
        public float MaxSimulationSpeed;
    }

    public class SimulationConfigurationBaker : Baker<SimulationConfigurationMono>
    {
        public override void Bake(SimulationConfigurationMono authoring)
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