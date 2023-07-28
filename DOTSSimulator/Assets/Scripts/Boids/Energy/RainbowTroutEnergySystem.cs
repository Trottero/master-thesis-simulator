using Simulator.Boids.Energy.Jobs;
using Simulator.Boids.Energy.Producers.Components;
using Simulator.Boids.Energy.Producers.Jobs;
using Simulator.Configuration;
using Simulator.Configuration.Components;
using Simulator.Framework;
using Simulator.Utils;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Simulator.Boids.Energy
{
    [UpdateInGroup(typeof(FrameworkFixedSystemGroup))]
    public partial class RainbowTroutEnergySystem : SystemBase
    {
        private GlobalConfigurationComponent _configurationComponent;
        private RainbowTroutEnergyConfigurationComponent _config;

        private EntityQuery _energyQuery;
        private EntityQuery _foodSourceQuery;

        protected override void OnCreate()
        {
            base.OnCreate();
            _energyQuery = GetEntityQuery(typeof(EnergyComponent), typeof(LocalToWorld));
            _foodSourceQuery = GetEntityQuery(typeof(FoodSourceComponent), typeof(LocalToWorld));
            
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
            
            var foodSourceInformation = _foodSourceQuery.ToComponentDataArray<FoodSourceComponent>(Allocator.TempJob);
            var foodSourceLocations = _foodSourceQuery.ToComponentDataArray<LocalToWorld>(Allocator.TempJob);

            var updateEnergyJobHandle = new RainbowTroutEnergyJob
            {
                EnergyConfig = _configurationComponent.EnergyConfiguration,
                SimulationFrameworkConfig = _configurationComponent.SimulationFrameworkConfiguration,
                RainbowTroutEnergyConfig = _config,
                FoodSourceInformation = foodSourceInformation,
                FoodSourceLocations = foodSourceLocations
            }.Schedule(_energyQuery, Dependency);
            
            new UpdateFoodSourceEnergyJob
            {
                FoodSourceInformation = foodSourceInformation
            }.Schedule(_foodSourceQuery, updateEnergyJobHandle).Complete();

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
            
            foodSourceInformation.Dispose();
            foodSourceLocations.Dispose();
        }
    }
}