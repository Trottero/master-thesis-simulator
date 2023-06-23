using Simulator.Boids.Energy.Producers.Components;
using Simulator.Configuration.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Random = Unity.Mathematics.Random;

namespace Simulator.Boids.Energy.Producers.Systems
{
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
            _foodSourcesConfiguration = SystemAPI.GetSingleton<FoodSourcesConfigurationComponent>();
            _schoolConfiguration = SystemAPI.GetSingleton<SchoolConfigurationComponent>();
            _registeredPrefabs = SystemAPI.GetSingleton<RegisteredPrefabsComponent>();

            var rng = new Random(1);
            var ecb = new EntityCommandBuffer(Allocator.TempJob);
            for (var i = 0; i < _foodSourcesConfiguration.NumberOfFoodSources; i++)
            {
                var entity = ecb.Instantiate(_registeredPrefabs.FoodSourcePrefab);
                ecb.SetComponent(entity, new LocalTransform
                {
                    Position = new float3(rng.NextFloat(), -1, rng.NextFloat()) * _schoolConfiguration.CageSize / 2,
                    Rotation = quaternion.identity,
                    Scale = 1
                });

                ecb.SetComponent(entity, new FoodSourceComponent
                {
                    EnergyLevel = _foodSourcesConfiguration.EnergyLevel,
                    RegenarationRate = _foodSourcesConfiguration.RegenarationRate,
                    MaxEnergyLevel = _foodSourcesConfiguration.MaxEnergyLevel,
                    FeedingRadius = _foodSourcesConfiguration.FeedingRadius
                });
            }

            ecb.Playback(EntityManager);
            ecb.Dispose();
        }

        protected override void OnUpdate()
        {
        }
    }
}