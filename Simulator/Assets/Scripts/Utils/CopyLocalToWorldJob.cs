using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;

namespace Simulator.Utils
{
    [BurstCompile]
    public partial struct CopyLocalToWorldJob : IJobEntity
    {
        [WriteOnly] public NativeArray<LocalToWorld> LocalToWorlds;

        void Execute([EntityInQueryIndex] int entityInQueryIndex, in LocalToWorld transform)
        {
            LocalToWorlds[entityInQueryIndex] = transform;
        }
    }
}