using UnityEngine;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Jobs;
using System;
using Unity.Physics;
using Collider = Unity.Physics.Collider;

namespace Simulator.Boids
{

    public class BoidController : MonoBehaviour
    {
        public float CageSize = 10f;
        public int SwarmSize = 100;

        [SerializeField] public Mesh BoidMesh;
        [SerializeField] public UnityEngine.Material BoidMaterial;
        [SerializeField] public BoidsConfigurationRef configuration;

        public static BoidController Instance;

        void Awake()
        {
            Instance = this;

            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var archtype = entityManager.CreateArchetype(
                typeof(BoidComponent),
                typeof(LocalToWorld),
                typeof(Rotation),
                typeof(Translation),
                typeof(PhysicsCollider),
                typeof(PhysicsVelocity),
                typeof(PhysicsMass),
                typeof(PhysicsDamping),
                typeof(PhysicsGravityFactor));

            var prototype = entityManager.CreateEntity(archtype);

            var rm = new RenderMeshDescription(BoidMesh, BoidMaterial);
            RenderMeshUtility.AddComponents(prototype, entityManager, rm);

            SetPhysicsForPrototype(entityManager, prototype);

            SpawnBoids(entityManager, prototype);

            entityManager.DestroyEntity(prototype);
        }

        private void SetPhysicsForPrototype(EntityManager entityManager, Entity proto)
        {
            entityManager.SetComponentData(proto, new PhysicsGravityFactor
            {
                Value = 0f
            });

            BlobAssetReference<Collider> spCollider = Unity.Physics.SphereCollider.Create(new SphereGeometry
            {
                Center = float3.zero,
                Radius = 0.07f
            });

            entityManager.SetComponentData(proto, new PhysicsCollider { Value = spCollider });

            unsafe
            {
                Collider* colliderPtr = (Collider*)spCollider.GetUnsafePtr();
                entityManager.SetComponentData(proto, PhysicsMass.CreateDynamic(colliderPtr->MassProperties, 1f));
            }

            entityManager.AddSharedComponentData(proto, new PhysicsWorldIndex());
        }

        private void SpawnBoids(EntityManager entityManager, Entity proto)
        {
            var ecb = new EntityCommandBuffer(Allocator.TempJob);

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

                Ecb.SetComponent(index, e, new Rotation
                {
                    Value = RandomRotation(rng)
                });

                Ecb.SetComponent(index, e, new Translation
                {
                    Value = RandomPosition(rng)
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