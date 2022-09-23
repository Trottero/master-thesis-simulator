using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;

namespace Simulator.Utils
{
    [BurstCompile]
    public partial struct CopyLocalToWorldJob : IJobEntity
    {
        [WriteOnly] public NativeArray<LocalToWorld> NativeArray;

        void Execute([EntityInQueryIndex] int entityInQueryIndex, in LocalToWorld transform)
        {
            NativeArray[entityInQueryIndex] = transform;
        }
    }
}