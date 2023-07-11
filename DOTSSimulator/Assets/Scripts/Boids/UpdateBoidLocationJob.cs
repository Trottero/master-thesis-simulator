using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Physics;
using Simulator.Configuration;
using Simulator.Configuration.Components;
using UnityEngine.Serialization;

namespace Simulator.Boids
{
    [BurstCompile]
    public partial struct UpdateBoidLocationJob : IJobEntity
    {
        [ReadOnly] public BoidsConfigurationComponent Config;
        [ReadOnly] public SimulationFrameworkConfigurationComponent SimulationFrameworkConfig;

        void Execute(ref PhysicsVelocity physicsVelocity, ref LocalToWorld transform, in BoidComponent boid)
        {
            var maxRot = math.radians(Config.RotationSpeed);
            var adjustedRotation = RotateTowards(math.normalizesafe(physicsVelocity.Linear, transform.Forward),
                boid.OptimalDirection, maxRot * SimulationFrameworkConfig.UpdateInterval, 0f);

            physicsVelocity.Linear = adjustedRotation * Config.Speed;

            // This should force the boid to rotate towards the direction it wants to go.
            var diff = boid.OptimalDirection - transform.Forward;
            physicsVelocity.Angular = diff;
        }

        private static float3 RotateTowards(float3 current, float3 target, float maxRadsDelta, float maxMag)
        {
            float delta = math.acos(math.dot(current, target) / (math.length(current) * math.length(target)));
            float magDiff = math.length(target) - math.length(current);
            float sign = math.sign(magDiff);
            float maxMagDelta = math.min(maxMag, math.abs(magDiff));
            float diff = math.min(1f, maxRadsDelta / delta);
            
            return Float3Slerp(current, target, diff) * (math.length(current) + maxMagDelta * sign);
        }

        private static float3 Float3Slerp(float3 current, float3 target, float scale)
        {
            if (scale == 0f)
            {
                return current;
            }

            if (scale == 1f)
            {
                return target;
            }

            return current + (target - current) * scale;
        }
    }
}