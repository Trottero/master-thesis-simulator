using UnityEngine;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Jobs;
using System;

namespace Simulator.Boids
{

    public class BoidController : MonoBehaviour
    {
        public float CageSize = 10f;
        public int SwarmSize = 100;

        [SerializeField] public Mesh BoidMesh;
        [SerializeField] public Material BoidMaterial;
        [SerializeField] public BoidsConfigurationRef configuration;

        public static BoidController Instance;

        void Awake()
        {
            Instance = this;

            EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var archtype = entityManager.CreateArchetype(
                typeof(BoidComponent),
                typeof(LocalToWorld));

            var ecb = new EntityCommandBuffer(Allocator.TempJob);

            var rm = new RenderMeshDescription(BoidMesh, BoidMaterial);

            var proto = entityManager.CreateEntity(archtype);

            RenderMeshUtility.AddComponents(proto, entityManager, rm);

            var spawnJob = new SpawnBoidsJob
            {
                CageSize = CageSize,
                Ecb = ecb.AsParallelWriter(),
                EntityCount = SwarmSize,
                Prototype = proto,
            };

            var spawnHandle = spawnJob.Schedule(SwarmSize, 128);
            spawnHandle.Complete();

            ecb.Playback(entityManager);
            ecb.Dispose();
            entityManager.DestroyEntity(proto);
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(CageSize, CageSize, CageSize));
        }

        [BurstCompatible]
        public struct SpawnBoidsJob : IJobParallelFor
        {
            public Entity Prototype;
            public int EntityCount;
            public EntityCommandBuffer.ParallelWriter Ecb;
            public float CageSize;

            public void Execute(int index)
            {
                var e = Ecb.Instantiate(index, Prototype);
                var rng = new System.Random(index);

                Ecb.SetComponent(index, e, new LocalToWorld
                {
                    Value = float4x4.TRS(
                       RandomPosition(rng),
                       RandomRotation(rng),
                       new float3(10f))
                });
            }

            private float3 RandomPosition(System.Random rng)
            {
                return new float3((float)rng.NextDouble() * CageSize - CageSize / 2f,
                                  (float)rng.NextDouble() * CageSize - CageSize / 2f,
                                  (float)rng.NextDouble() * CageSize - CageSize / 2f);
            }

            private quaternion RandomRotation(System.Random rng)
            {
                return quaternion.Euler((float)rng.NextDouble() * 360,
                                        (float)rng.NextDouble() * 360,
                                        (float)rng.NextDouble() * 360);
            }
        }
    }


    [Serializable]
    public class BoidsConfigurationRef
    {
        public BoidsConfiguration Values;

    }

    [Serializable]
    public struct BoidsConfiguration
    {
        public float Speed;
        public float RotationSpeed;
        public float PerceptionRange;
        public float SeperationWeight;
        public float CohesionWeight;
        public float AlignmentWeight;
    }
}