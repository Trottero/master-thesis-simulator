using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;

namespace Simulator.Boids
{
    [BurstCompile]
    public partial struct CopyLocationsJob : IJobEntity
    {
        [WriteOnly] public NativeArray<BoidProperties> BoidLocations;

        void Execute([EntityInQueryIndex] int entityInQueryIndex, in LocalToWorld transform)
        {
            BoidLocations[entityInQueryIndex] = new BoidProperties
            {
                Position = transform.Position,
                Direction = transform.Forward
            };
        }
    }
}