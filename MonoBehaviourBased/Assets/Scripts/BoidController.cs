using UnityEngine;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using Unity.Collections;
using Unity.Mathematics;

public class BoidController : MonoBehaviour
{
    public float CageSize = 10f;
    public int SwarmSize = 100;

    public GameObject BoidObject;
    public Material BoidMaterial;

    public static BoidController Instance;

    void Awake()
    {
        Instance = this;

        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var archtype = entityManager.CreateArchetype(
            typeof(BoidComponent),
            typeof(RenderMesh),
            typeof(RenderBounds),
            typeof(LocalToWorld));

        NativeArray<Entity> boidEntities = new NativeArray<Entity>(SwarmSize, Allocator.Temp);
        entityManager.CreateEntity(archtype, boidEntities);

        for (int i = 0; i < boidEntities.Length; i++)
        {
            entityManager.SetComponentData(boidEntities[i], new LocalToWorld
            {
                Value = float4x4.TRS(
                    RandomPosition(),
                    RandomRotation(),
                    new float3(1f, 1f, 1f))
            });

            entityManager.SetSharedComponentData(boidEntities[i], new RenderMesh
            {
                mesh = BoidObject.GetComponentInChildren<MeshFilter>().sharedMesh,
                material = BoidMaterial
            });

        }
        boidEntities.Dispose();
    }

    private float3 RandomPosition()
    {
        return new float3(UnityEngine.Random.Range(-CageSize / 2, CageSize / 2),
                          UnityEngine.Random.Range(-CageSize / 2, CageSize / 2),
                          UnityEngine.Random.Range(-CageSize / 2, CageSize / 2));
    }

    private quaternion RandomRotation()
    {
        return quaternion.Euler(UnityEngine.Random.Range(0, 360),
                                UnityEngine.Random.Range(0, 360),
                                UnityEngine.Random.Range(0, 360));
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(CageSize, CageSize, CageSize));
    }
}
