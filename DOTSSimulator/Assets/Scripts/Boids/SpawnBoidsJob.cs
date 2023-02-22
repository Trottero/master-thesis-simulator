using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Jobs;

namespace Simulator.Boids
{
    [BurstCompatible]
    public struct SpawnBoidsJob : IJobEntity
    {
        public Entity Prototype;
        public int EntityCount;
        public EntityCommandBuffer.ParallelWriter Ecb;
        public float CageSize;

        public void Execute([EntityInQueryIndex] int index)
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