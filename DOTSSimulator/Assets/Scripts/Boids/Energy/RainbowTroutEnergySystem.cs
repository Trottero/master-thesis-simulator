using Simulator.Boids.Energy.Jobs;
using Simulator.Configuration;
using Simulator.Configuration.Components;
using Simulator.Framework;
using Simulator.Utils;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Simulator.Boids.Energy
{
    [UpdateInGroup(typeof(FrameworkFixedSystemGroup))]
    public partial class RainbowTroutEnergySystem : SystemBase
    {
        private GlobalConfigurationComponent _configurationComponent;
        private RainbowTroutEnergyConfigurationComponent _config;

        private EntityQuery _energyQuery;

        protected override void OnCreate()
        {
            base.OnCreate();
            _energyQuery = GetEntityQuery(typeof(EnergyComponent));
            RequireForUpdate<GlobalConfigurationComponent>();
        }

        protected override void OnStartRunning()
        {
            _configurationComponent = SystemAPI.GetSingleton<GlobalConfigurationComponent>();
            _config = _configurationComponent.RainbowTroutEnergyConfiguration;

            base.OnStartRunning();
        }

        protected override void OnUpdate()
        {
            if (_configurationComponent.EnergyConfiguration.EnergyEquation != EnergyEquationType.RainbowTrout)
            {
                return;
            }

            var dt = (decimal)_configurationComponent.SimulationFrameworkConfiguration.UpdateInterval;
            // Update energy level

            var updateEnergyJobHandle = new RainbowTroutEnergyJob
            {
                EnergyConfig = _configurationComponent.EnergyConfiguration,
                SimulationFrameworkConfig = _configurationComponent.SimulationFrameworkConfiguration,
                RainbowTroutEnergyConfig = _config,
            }.ScheduleParallel(_energyQuery, Dependency);
            
            updateEnergyJobHandle.Complete();

            Entities
                .WithDeferredPlaybackSystem<EndSimulationEntityCommandBufferSystem>()
                .ForEach((Entity entity, EntityCommandBuffer ecb, in EnergyComponent health) =>
                    {
                        if (health.Weight < 0)
                        {
                            ecb.DestroyEntity(entity);
                        }
                    }
                ).Run();
        }
    }
}