using Unity.Jobs;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections;
using Simulator.Curves;

namespace Simulator.Boids
{

    public struct BoidProperties
    {
        public float3 Position;
        public float3 Direction;
    }

    [BurstCompile]
    public partial struct CopyLocationsJob : IJobEntity
    {
        [WriteOnly] public NativeArray<BoidProperties> BoidLocations;

        void Execute([EntityInQueryIndex] int entityInQueryIndex, in LocalToWorld localToWorld)
        {
            BoidLocations[entityInQueryIndex] = new BoidProperties
            {
                Position = localToWorld.Position,
                Direction = localToWorld.Forward
            };
        }
    }

    [BurstCompile]
    public partial struct UpdateBoidLocationJob : IJobEntity
    {
        [ReadOnly] public float DeltaTime;
        void Execute(ref LocalToWorld localToWorld, in BoidComponent boid)
        {
            localToWorld.Value = float4x4.TRS(
                localToWorld.Position + boid.optimalDirection * DeltaTime,
                quaternion.LookRotation(boid.optimalDirection, localToWorld.Up),
                new float3(10f));
        }
    }

    [BurstCompile]
    public partial struct ComputeOptimalDirectionJob : IJobEntity
    {
        [ReadOnly] public NativeArray<BoidProperties> OtherBoids;

        [ReadOnly] public BoidsConfiguration config;

        void Execute(
            ref BoidComponent boid,
            in LocalToWorld localToWorld
            // in SeperationCurveReference seperationCurve,
            // in AlignmentCurveReference alignmentCurve,
            // in CohesionCurveReference cohesionCurve
            )
        {
            float3 seperation = float3.zero;
            float3 cohesion = float3.zero;
            float3 alignment = float3.zero;

            int boidsInRange = 0;
            // Current boid has position and direction
            for (int i = 0; i < OtherBoids.Length; i++)
            {
                // Calculate optimal direction here :)
                if (math.distance(localToWorld.Position, OtherBoids[i].Position) < config.PerceptionRange)
                {
                    // var distanceNormalized = boidDistance / config.PerceptionRange;
                    boidsInRange++;

                    // calculate seperation
                    seperation -= OtherBoids[i].Position;
                    // Calculate cohesion
                    cohesion += OtherBoids[i].Position;
                    // calculate alignment
                    alignment += OtherBoids[i].Direction;
                }
            }

            // We now have the total world space and direction of all boids in range
            if (boidsInRange > 0)
            {
                seperation /= boidsInRange;
                cohesion /= boidsInRange;
                alignment /= boidsInRange;

                // Calculate seperation
                seperation = math.normalize(seperation - localToWorld.Position);
                // Calculate cohesion
                cohesion = math.normalize(cohesion - localToWorld.Position);
                // Calculate alignment
                alignment = math.normalize(alignment);
            }


            boid.optimalDirection = config.AlignmentWeight * alignment + config.CohesionWeight * cohesion + config.SeperationWeight * seperation;
        }
    }
}