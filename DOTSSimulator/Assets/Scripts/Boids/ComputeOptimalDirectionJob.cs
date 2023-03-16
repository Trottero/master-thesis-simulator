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
            in SeparationCurveReference separationCurve,
            in AlignmentCurveReference alignmentCurve,
            in CohesionCurveReference cohesionCurve
        )
        {
            float3 separation = float3.zero;
            float3 cohesion = float3.zero;
            float3 alignment = float3.zero;

            // Current boid has position and direction
            for (int i = 0; i < OtherBoids.Length; i++)
            {
                // Calculate optimal direction here :)
                var distance = math.distance(localToWorld.Position, OtherBoids[i].Position);
                if (distance < config.PerceptionRange)
                {
                    var distanceNormalized = distance / config.PerceptionRange;
                    // calculate separation
                    separation -= (OtherBoids[i].Position - localToWorld.Position) *
                                  separationCurve.Evaluate(distanceNormalized);
                    // Calculate cohesion
                    cohesion += (OtherBoids[i].Position - localToWorld.Position) *
                                cohesionCurve.Evaluate(distanceNormalized);
                    // calculate alignment
                    alignment += OtherBoids[i].Direction * alignmentCurve.Evaluate(distanceNormalized);
                }
            }

            // We now have the total world space and direction of all boids in range

            // Calculate separation
            separation = math.normalizesafe(separation, float3.zero);
            // Calculate cohesion
            cohesion = math.normalizesafe(cohesion, float3.zero);
            // Calculate alignment
            alignment = math.normalizesafe(alignment, float3.zero);

            var stayInCube = float3.zero;
            if (math.distance(localToWorld.Position, float3.zero) > 8)
            {
                stayInCube = math.normalizesafe(-localToWorld.Position);
            }

            var foodSource = math.normalizesafe(GetClosestFoodSource(localToWorld.Position) - localToWorld.Position,
                float3.zero);


            boid.optimalDirection = math.normalizesafe(
                config.AlignmentWeight * alignment +
                config.CohesionWeight * cohesion +
                config.SeperationWeight * separation +
                config.StayInCubeWeight * stayInCube +
                config.FoodSourceWeight * foodSource,
                math.normalizesafe(localToWorld.Forward));
        }

        private float3 GetClosestFoodSource(float3 boidPosition)
        {
            var closestFoodSource = float3.zero;
            var closestDistance = float.MaxValue;
            for (var i = 0; i < FoodSources.Length; i++)
            {
                var effectivePosition =
                    FoodSourceInformation[i].EffectivePosition(boidPosition, FoodSources[i].Position);
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