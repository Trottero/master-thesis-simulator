using Framework;
using Simulator.Utils;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Simulator.Curves;
using Unity.Physics;
using Simulator.Boids.Energy.Producers;
using Simulator.Boids.Energy;
using Simulator.Configuration;
using Unity.Burst;

namespace Simulator.Boids
{
    [UpdateInGroup(typeof(FrameworkFixedSystemGroup))]
    [BurstCompile]
    public partial class BoidsSystem : SystemBase
    {
        private EntityQuery _boidQuery;
        private EntityQuery _boidLocationQuery;
        private EntityQuery _boidEnergyQuery;
        private EntityQuery _boidDisplacementQuery;
        private EntityQuery _foodSourceQuery;
        private BoidController _controller;
        private Entity _gameControllerEntity;

        private SimulationConfigurationComponent _simulationConfiguration;

        protected override void OnCreate()
        {
            _boidQuery = GetEntityQuery(
                ComponentType.ReadWrite<BoidComponent>(),
                ComponentType.ReadOnly<LocalToWorld>(),
                ComponentType.ReadOnly<CohesionCurveReference>(),
                ComponentType.ReadOnly<AlignmentCurveReference>(),
                ComponentType.ReadOnly<SeparationCurveReference>());

            _boidLocationQuery = GetEntityQuery(
                ComponentType.ReadOnly<BoidComponent>(),
                ComponentType.ReadOnly<LocalToWorld>());

            _boidEnergyQuery = GetEntityQuery(
                ComponentType.ReadOnly<BoidComponent>(),
                ComponentType.ReadOnly<LocalToWorld>(),
                ComponentType.ReadWrite<EnergyComponent>());

            _boidDisplacementQuery = GetEntityQuery(
                ComponentType.ReadWrite<PhysicsVelocity>(),
                ComponentType.ReadOnly<PhysicsMass>(),
                ComponentType.ReadWrite<LocalToWorld>(),
                ComponentType.ReadOnly<BoidComponent>());

            _foodSourceQuery = GetEntityQuery(
                ComponentType.ReadWrite<FoodSourceComponent>(),
                ComponentType.ReadOnly<LocalToWorld>());

            RequireForUpdate<SimulationConfigurationComponent>();
            RequireForUpdate(_boidQuery);
        }

        protected override void OnStartRunning()
        {
            base.OnStartRunning();
            _simulationConfiguration = SystemAPI.GetSingleton<SimulationConfigurationComponent>();
        }

        protected override void OnUpdate()
        {
            if (!_controller)
            {
                _controller = BoidController.Instance;
                return;
            }


            // Store all of the boid positions into an array for future use
            var boidPositions = new NativeArray<BoidProperties>(_boidLocationQuery.CalculateEntityCount(), Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
            var copyBoidLocationsHandle = new CopyLocationsJob
            {
                BoidLocations = boidPositions
            }.ScheduleParallel(_boidLocationQuery, Dependency);

            // Store all of the food components in an array for future use.
            var foodSourceCount = _foodSourceQuery.CalculateEntityCount();
            var foodSourcePositions = new NativeArray<LocalToWorld>(foodSourceCount, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
            var foodSourceInformation = new NativeArray<FoodSourceComponent>(foodSourceCount, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
            var copyFoodSourceLocationsHandle = new CopyFoodSourceLocationsJob
            {
                Locations = foodSourcePositions,
                ProducerComponents = foodSourceInformation
            }.ScheduleParallel(_foodSourceQuery, Dependency);

            // Compute the optimal direction for the boids
            var optimalDirectionHandle = new ComputeOptimalDirectionJob
            {
                OtherBoids = boidPositions,
                Config = _controller.configuration.BoidConfig,
                FoodSources = foodSourcePositions,
                FoodSourceInformation = foodSourceInformation
            }.ScheduleParallel(_boidQuery, JobHandle.CombineDependencies(copyBoidLocationsHandle, copyFoodSourceLocationsHandle));

            // Use the previously computed array to update the boid locations
            var updateBoidLocationsHandle = new UpdateBoidLocationJob
            {
                Config = _controller.configuration.BoidConfig,
                SimulationConfig = _simulationConfiguration
            }.ScheduleParallel(_boidDisplacementQuery, optimalDirectionHandle);

            // Update the energy when close to things
            var updateFishEnergyJobHandler = new UpdateFishEnergyJob
            {
                FoodSourceInformation = foodSourceInformation,
                FoodSourceLocations = foodSourcePositions,
                EnergyConfig = _controller.configuration.EnergyConfig,
                SimulationConfig = _simulationConfiguration
            }.Schedule(_boidEnergyQuery, updateBoidLocationsHandle);

            // Update the food sources
            new UpdateFoodSourceEnergyJob
            {
                FoodSourceInformation = foodSourceInformation
            }.ScheduleParallel(_foodSourceQuery, updateFishEnergyJobHandler).Complete();

            boidPositions.Dispose(Dependency);
            foodSourcePositions.Dispose(Dependency);
            foodSourceInformation.Dispose(Dependency);
        }
    }
}