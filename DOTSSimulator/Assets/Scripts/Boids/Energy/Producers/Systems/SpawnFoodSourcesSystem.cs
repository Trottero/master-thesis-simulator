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

        protected override void OnCreate()
        {
            base.OnCreate();
            RequireForUpdate<GlobalConfigurationComponent>();
        }

        protected override void OnStartRunning()
        {
            base.OnStartRunning();
            _foodSourcesConfiguration = SystemAPI.GetSingleton<FoodSourcesConfigurationComponent>();
            _schoolConfiguration = SystemAPI.GetSingleton<SchoolConfigurationComponent>();

            var rng = new Random(1);
            var ecb = new EntityCommandBuffer(Allocator.TempJob);
            for (var i = 0; i < _foodSourcesConfiguration.NumberOfFoodSources; i++)
            {
                var entity = ecb.Instantiate(_foodSourcesConfiguration.FoodSourcePrefab);
                ecb.SetComponent(entity, new LocalTransform
                {
                    Position = new float3(rng.NextFloat(), -1, rng.NextFloat()) * _schoolConfiguration.CageSize / 2,
                    Rotation = quaternion.identity,
                    Scale = 1
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