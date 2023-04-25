using Simulator.Boids.Energy;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Rendering;
using Unity.Transforms;
using Collider = Unity.Physics.Collider;
using UnityEngine.Rendering;
using UnityEngine;

namespace Simulator.Boids.Lifecycle
{
    public static class BoidSpawningHelper
    {
        public static PhysicsWorldIndex worldIndex = new PhysicsWorldIndex();
        public static Entity SpawnPrototype(EntityManager entityManager)
        {
            var boidController = BoidController.Instance;

            var archtype = entityManager.CreateArchetype(
                   typeof(BoidComponent),
                   typeof(LocalToWorld),
                   typeof(LocalTransform),
                   typeof(PhysicsCollider),
                   typeof(PhysicsVelocity),
                   typeof(PhysicsMass),
                   typeof(PhysicsDamping),
                   typeof(PhysicsGravityFactor),
                   typeof(EnergyComponent));

            var prototype = entityManager.CreateEntity(archtype);

            var renderMeshArray = new RenderMeshArray(new UnityEngine.Material[] { boidController.BoidMaterial }, new Mesh[] { boidController.BoidMesh });
            RenderMeshUtility.AddComponents(prototype, entityManager, new RenderMeshDescription(ShadowCastingMode.Off), renderMeshArray, MaterialMeshInfo.FromRenderMeshArrayIndices(0, 0));

            entityManager.SetComponentData(prototype, new EnergyComponent
            {
                EnergyLevel = boidController.configuration.EnergyConfig.InitialEnergyLevel,
            });

            SetPhysicsForPrototype(entityManager, prototype);
            Debug.Log("Spawned prototype");
            return prototype;
        }


        private static void SetPhysicsForPrototype(EntityManager entityManager, Entity proto)
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
                entityManager.SetComponentData(proto, PhysicsMass.CreateDynamic(colliderPtr->MassProperties, 0.1f));
            }

            entityManager.AddSharedComponentManaged(proto, worldIndex);
        }

        public static void SpawnBoids(EntityCommandBuffer ecb, Entity proto, int count, float maxRadius)
        {
            for (int i = 0; i < count; i++)
            {
                var rng = new System.Random(i);

                var e = ecb.Instantiate(proto);
                ecb.SetComponent(e,
                    LocalTransform.FromPositionRotation(
                        RandomPosition(rng, maxRadius),
                        RandomRotation(rng)));
            }
        }

        private static float3 RandomPosition(System.Random rng, float maxRadius)
        {
            return new float3((float)rng.NextDouble() * maxRadius - maxRadius / 2f,
                              (float)rng.NextDouble() * maxRadius - maxRadius / 2f,
                              (float)rng.NextDouble() * maxRadius - maxRadius / 2f);
        }

        private static quaternion RandomRotation(System.Random rng)
        {
            return quaternion.Euler((float)rng.NextDouble() * 360,
                                    (float)rng.NextDouble() * 360,
                                    (float)rng.NextDouble() * 360);
        }
    }
}