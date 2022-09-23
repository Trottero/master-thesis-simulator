using Simulator.Utils;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Simulator.Curves;
using Unity.Physics;
using Simulator.Boids.Energy.Producers;
using Simulator.Boids.Energy;

namespace Simulator.Boids
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial class BoidsSystem : SystemBase
    {
        EntityQuery boid_query;
        EntityQuery boid_location_query;
        EntityQuery boid_energy_query;
        EntityQuery boid_displacement_query;
        EntityQuery food_source_query;
        private BoidController controller;
        protected override void OnCreate()
        {
            boid_query = GetEntityQuery(
                ComponentType.ReadWrite<BoidComponent>(),
                ComponentType.ReadOnly<LocalToWorld>(),
                ComponentType.ReadOnly<CohesionCurveReference>(),
                ComponentType.ReadOnly<AlignmentCurveReference>(),
                ComponentType.ReadOnly<SeparationCurveReference>());

            boid_location_query = GetEntityQuery(
                ComponentType.ReadOnly<BoidComponent>(),
                ComponentType.ReadOnly<LocalToWorld>());

            boid_energy_query = GetEntityQuery(
                ComponentType.ReadOnly<BoidComponent>(),
                ComponentType.ReadOnly<LocalToWorld>(),
                ComponentType.ReadWrite<EnergyComponent>());

            boid_displacement_query = GetEntityQuery(
                ComponentType.ReadWrite<PhysicsVelocity>(),
                ComponentType.ReadOnly<PhysicsMass>(),
                ComponentType.ReadWrite<LocalToWorld>(),
                ComponentType.ReadOnly<BoidComponent>());

            food_source_query = GetEntityQuery(
                ComponentType.ReadWrite<FoodSourceComponent>(),
                ComponentType.ReadOnly<LocalToWorld>());
        }

        protected override void OnUpdate()
        {
            if (!controller)
            {
                controller = BoidController.Instance;
                return;
            }

            var boidCount = boid_location_query.CalculateEntityCount();

            NativeArray<BoidProperties> boidPositions = new NativeArray<BoidProperties>(boidCount, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
            var copyLocationsJob = new CopyLocationsJob
            {
                BoidLocations = boidPositions
            };
            var copyLocationsJobHandle = copyLocationsJob.ScheduleParallel(boid_location_query, Dependency);

            var foodSourceCount = food_source_query.CalculateEntityCount();
            NativeArray<LocalToWorld> foodSourcePositions = new NativeArray<LocalToWorld>(foodSourceCount, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
            NativeArray<FoodSourceComponent> foodSourceInformation = new NativeArray<FoodSourceComponent>(foodSourceCount, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
            var copyFoodSourceLocationsJob = new CopyFoodSourceLocationsJob
            {
                Locations = foodSourcePositions,
                ProducerComponents = foodSourceInformation
            };
            var copyFoodSourceLocationsJobHandle = copyFoodSourceLocationsJob.ScheduleParallel(food_source_query, copyLocationsJobHandle);

            var bj = new ComputeOptimalDirectionJob
            {
                OtherBoids = boidPositions,
                config = controller.configuration.BoidConfig,
                FoodSources = foodSourcePositions
            };
            var boidJobHandle = bj.ScheduleParallel(boid_query, copyFoodSourceLocationsJobHandle);

            var updateBoidJob = new UpdateBoidLocationJob
            {
                DeltaTime = Time.DeltaTime,
                config = controller.configuration.BoidConfig
            };
            var updateBoidJobHandle = updateBoidJob.ScheduleParallel(boid_displacement_query, boidJobHandle);
            updateBoidJobHandle.Complete();

            // Update the energy when close to things
            new UpdateFishEnergyJob
            {
                DeltaTime = Time.DeltaTime,
                FoodSourceInformation = foodSourceInformation,
                FoodSourceLocations = foodSourcePositions,
                EnergyConfig = controller.configuration.EnergyConfig
            }.Schedule(boid_energy_query, copyFoodSourceLocationsJobHandle).Complete();

            boidPositions.Dispose(Dependency);
            foodSourcePositions.Dispose(Dependency);
            foodSourceInformation.Dispose(Dependency);
        }
    }
}