using Simulator.Boids.Energy.Producers.Components;
using Simulator.Boids.Lifecycle;
using Simulator.Configuration.Components;
using Simulator.Framework;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Simulator.Boids.Energy.Producers.Systems
{
    [UpdateBefore(typeof(BoidSpawningSystem))]
    [UpdateInGroup(typeof(FrameworkFixedSystemGroup))]
    public partial class SpawnFoodSourcesSystem : SystemBase
    {
        private FoodSourcesConfigurationComponent _foodSourcesConfiguration;
        private SchoolConfigurationComponent _schoolConfiguration;
        private RegisteredPrefabsComponent _registeredPrefabs;

        protected override void OnCreate()
        {
            base.OnCreate();
            RequireForUpdate<GlobalConfigurationComponent>();
            RequireForUpdate<RegisteredPrefabsComponent>();
        }

        protected override void OnStartRunning()
        {
            base.OnStartRunning();
            var globalConfig = SystemAPI.GetSingleton<GlobalConfigurationComponent>();
            
            _foodSourcesConfiguration = globalConfig.FoodSourcesConfiguration;
            _schoolConfiguration = globalConfig.SchoolConfiguration;
            _registeredPrefabs = SystemAPI.GetSingleton<RegisteredPrefabsComponent>();

            var rng = new Random(1);
            for (var i = 0; i < _foodSourcesConfiguration.NumberOfFoodSources; i++)
            {
                
                var entity = EntityManager.Instantiate(_registeredPrefabs.FoodSourcePrefab);
                var position = new float3(rng.NextFloat(), -1, rng.NextFloat()) * _schoolConfiguration.CageSize / 2;
                Debug.Log("Spawned food source at " + position);
                EntityManager.SetComponentData(entity, new LocalTransform
                {
                    Position = position,
                    Rotation = quaternion.identity,
                    Scale = 1
                });
                
                Debug.Log("Spawned food source at " + position);

                EntityManager.SetComponentData(entity, new FoodSourceComponent
                {
                    EnergyLevel = (decimal)_foodSourcesConfiguration.EnergyLevel,
                    RegenerationRate = (decimal)_foodSourcesConfiguration.RegenerationRate,
                    MaxEnergyLevel = (decimal)_foodSourcesConfiguration.MaxEnergyLevel,
                    FeedingRadius = _foodSourcesConfiguration.FeedingRadius
                });
            }
        }

        protected override void OnUpdate()
        {
        }
    }
}