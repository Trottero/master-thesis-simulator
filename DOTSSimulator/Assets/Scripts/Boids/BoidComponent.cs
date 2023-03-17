using Unity.Entities;
using Unity.Mathematics;

namespace Simulator.Boids
{
    public struct BoidComponent : IComponentData
    {
        // The optimal direction of the current boid (target position)
        public float3 OptimalDirection;
    }
}