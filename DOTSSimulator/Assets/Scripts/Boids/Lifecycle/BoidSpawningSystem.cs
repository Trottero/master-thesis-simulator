using System.Collections;
using System.Collections.Generic;
using Simulator.Boids.Lifecycle;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace Simulator.Boids.Lifecycle
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial class BoidSpawningSystem : SystemBase
    {
        private EntityQuery _schoolComponentDataQuery;

        protected override void OnCreate()
        {
            _schoolComponentDataQuery = GetEntityQuery(ComponentType.ReadWrite<SchoolComponentData>());

            RequireForUpdate(_schoolComponentDataQuery);
        }

        protected override void OnUpdate()
        {
            if (BoidController.Instance == null)
            {
                return;
            }

            var _schoolEntity = _schoolComponentDataQuery.GetSingletonEntity();
            var _schoolComponentData = EntityManager.GetComponentData<SchoolComponentData>(_schoolEntity);

            var ecb = new EntityCommandBuffer(Allocator.TempJob);
            var proto = BoidSpawningHelper.SpawnPrototype(EntityManager);
            var _spawnBoidsJob = new SpawnBoidsJob
            {
                Prototype = proto,
                Ecb = ecb.AsParallelWriter(),
                EntityCount = _schoolComponentData.SwarmSize,
                CageSize = _schoolComponentData.CageSize
            };

            var _spawnBoidsJobHandle = _spawnBoidsJob.Schedule(_schoolComponentData.SwarmSize, 32, Dependency);
            _spawnBoidsJobHandle.Complete();

            ecb.Playback(EntityManager);
            ecb.Dispose();

            EntityManager.DestroyEntity(proto);
            EntityManager.DestroyEntity(_schoolEntity);
        }
    }
}