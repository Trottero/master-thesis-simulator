using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections;
using Simulator.Curves;
using Simulator.Boids.Energy.Producers;

namespace Simulator.Boids
{
    [BurstCompile]
    public partial struct ComputeOptimalDirectionJob : IJobEntity
    {
        [ReadOnly] public NativeArray<BoidProperties> OtherBoids;
        [ReadOnly] public NativeArray<LocalToWorld> FoodSources;
        [ReadOnly] public NativeArray<FoodSourceComponent> FoodSourceInformation;

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
                stayInCube = math.normalizesafe(-localToWorld.Position);
            }

            float3 foodsource = math.normalizesafe(GetClosestFoodSource(localToWorld.Position) - localToWorld.Position, float3.zero);

            boid.optimalDirection = math.normalizesafe(
                config.AlignmentWeight * alignment +
                config.CohesionWeight * cohesion +
                config.SeperationWeight * seperation +
                config.StayInCubeWeight * stayInCube +
                config.FoodSourceWeight * foodsource,
            math.normalizesafe(localToWorld.Forward));
        }

        float3 GetClosestFoodSource(float3 boidPosition)
        {
            float3 closestFoodSource = float3.zero;
            float closestDistance = float.MaxValue;
            for (int i = 0; i < FoodSources.Length; i++)
            {
                var effectivePosition = FoodSourceInformation[i].EffectivePosition(boidPosition, FoodSources[i].Position);
                var distance = math.distance(boidPosition, effectivePosition) / FoodSourceInformation[i].EnergyLevel;
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestFoodSource = effectivePosition;
                }
            }

            return closestFoodSource;
        }
    }
}