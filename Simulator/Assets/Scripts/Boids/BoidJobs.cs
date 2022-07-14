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
        [ReadOnly] public BoidsConfiguration config;
        [ReadOnly] public float DeltaTime;
        void Execute(ref LocalToWorld localToWorld, in BoidComponent boid)
        {
            var ql = quaternion.LookRotation(boid.optimalDirection, math.up());

            localToWorld.Value = float4x4.TRS(
                localToWorld.Position + math.normalizesafe(localToWorld.Forward) * DeltaTime * config.Speed,
                ql,
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
            in LocalToWorld localToWorld,
            in SeparationCurveReference seperationCurve,
            in AlignmentCurveReference alignmentCurve,
            in CohesionCurveReference cohesionCurve
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
                var distance = math.distance(localToWorld.Position, OtherBoids[i].Position);
                if (distance < config.PerceptionRange)
                {
                    var distanceNormalized = distance / config.PerceptionRange;
                    boidsInRange++;

                    // calculate seperation
                    seperation -= (OtherBoids[i].Position - localToWorld.Position) * seperationCurve.Evaluate(distanceNormalized);
                    // Calculate cohesion
                    cohesion += (OtherBoids[i].Position - localToWorld.Position) * cohesionCurve.Evaluate(distanceNormalized);
                    // calculate alignment
                    alignment += OtherBoids[i].Direction * alignmentCurve.Evaluate(distanceNormalized);
                }
            }

            // We now have the total world space and direction of all boids in range

            // Calculate seperation
            seperation = math.normalizesafe(seperation, float3.zero);
            // Calculate cohesion
            cohesion = math.normalizesafe(cohesion, float3.zero);
            // Calculate alignment
            alignment = math.normalizesafe(alignment, float3.zero);

            float3 stayInCube = float3.zero;
            if (math.distance(localToWorld.Position, float3.zero) > 8)
            {
                stayInCube = math.normalizesafe(-localToWorld.Position) * 1f;
            }

            boid.optimalDirection = math.normalizesafe(
                config.AlignmentWeight * alignment + config.CohesionWeight * cohesion + config.SeperationWeight * seperation + stayInCube,
                math.normalizesafe(localToWorld.Forward));
        }
    }
}