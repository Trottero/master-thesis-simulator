using System.Collections;
using System.Collections.Generic;
using Simulator.Boids;
using Unity.Entities;
using UnityEngine;

namespace Simulator.Configuration
{
    public struct SimulationConfigurationComponent : IComponentData
    {
        public float UpdatesPerSecond;
        public float UpdateInterval => 1f / UpdatesPerSecond;
        public float EffectiveUpdatesPerSecond => UpdateInterval * (1f / MaxSimulationSpeed);
        public float MaxSimulationSpeed;
    }
}
