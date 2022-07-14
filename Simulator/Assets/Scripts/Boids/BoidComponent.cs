using Unity.Entities;
using Unity.Mathematics;

namespace Simulator.Boids
{
    public struct BoidComponent : IComponentData
    {
        public float3 optimalDirection;
    }
}