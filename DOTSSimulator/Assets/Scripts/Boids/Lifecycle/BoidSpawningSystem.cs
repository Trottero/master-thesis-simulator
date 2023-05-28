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

            var schoolEntity = _schoolComponentDataQuery.GetSingletonEntity();
            var schoolComponentData = EntityManager.GetComponentData<SchoolComponentData>(schoolEntity);

            var ecb = new EntityCommandBuffer(Allocator.TempJob);
            var proto = BoidSpawningHelper.SpawnPrototype(EntityManager, BoidController.Instance.configuration.EnergyConfig.InitialEnergyLevel);
            new SpawnBoidsJob
            {
                Prototype = proto,
                Ecb = ecb.AsParallelWriter(),
                CageSize = schoolComponentData.CageSize
            }.Schedule(schoolComponentData.SwarmSize, 32, Dependency).Complete();

            ecb.Playback(EntityManager);
            ecb.Dispose();

            EntityManager.DestroyEntity(proto);
            EntityManager.DestroyEntity(schoolEntity);
        }
    }
}