using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Simulator.Configuration
{

    public class ConfigurationConverter : MonoBehaviour, IConvertGameObjectToEntity
    {

        [Serializable]
        public class SimulationConfiguration
        {
            public float UpdatesPerSecond = 30;
            public float MaxSimulationSpeed = 2;
        }

        public SimulationConfiguration Config;
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var configuration = new SimulationConfigurationComponent
            {
                UpdatesPerSecond = Config.UpdatesPerSecond,
                MaxSimulationSpeed = Config.MaxSimulationSpeed
            };

            Debug.Log($"Current Configuration: UpdatesPerSecond: {Config.UpdatesPerSecond}, MaxSimulationSpeed: {Config.MaxSimulationSpeed}");

            dstManager.AddComponentData(entity, configuration);
        }
    }
}

