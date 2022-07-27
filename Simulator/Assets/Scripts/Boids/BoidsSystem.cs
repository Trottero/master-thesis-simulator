using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

using Simulator.Curves;
using Unity.Physics;

namespace Simulator.Boids
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial class BoidsSystem : SystemBase
    {
        EntityQuery boid_query;
        EntityQuery boid_location_query;
        EntityQuery boid_displacement_query;
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

            boid_displacement_query = GetEntityQuery(
                ComponentType.ReadWrite<PhysicsVelocity>(),
                ComponentType.ReadOnly<PhysicsMass>(),
                ComponentType.ReadWrite<LocalToWorld>(),
                ComponentType.ReadOnly<BoidComponent>());
        }

        protected override void OnUpdate()
        {
            if (!controller)
            {
                controller = BoidController.Instance;
            }

            if (!controller)
            {
                // Unitialized
                return;
            }

            var boidCount = boid_location_query.CalculateEntityCount();

            NativeArray<BoidProperties> boidPositions = new NativeArray<BoidProperties>(boidCount, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);

            var copyLocationsJob = new CopyLocationsJob
            {
                BoidLocations = boidPositions
            };

            var copyLocationsJobHandle = copyLocationsJob.ScheduleParallel(boid_location_query, Dependency);

            var bj = new ComputeOptimalDirectionJob
            {
                OtherBoids = boidPositions,
                config = controller.configuration.Values
            };
            var boidJobHandle = bj.ScheduleParallel(boid_query, copyLocationsJobHandle);

            var updateBoidJob = new UpdateBoidLocationJob
            {
                DeltaTime = Time.DeltaTime,
                config = controller.configuration.Values
            };

            var updateBoidJobHandle = updateBoidJob.ScheduleParallel(boid_displacement_query, boidJobHandle);
            updateBoidJobHandle.Complete();

            boidPositions.Dispose(Dependency);
        }
    }
}