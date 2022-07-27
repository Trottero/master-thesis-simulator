using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Physics.Extensions;
using Unity.Physics;

namespace Simulator.Boids
{
    [BurstCompile]
    public partial struct UpdateBoidLocationJob : IJobEntity
    {
        [ReadOnly] public BoidsConfiguration config;
        [ReadOnly] public float DeltaTime;
        void Execute(ref PhysicsVelocity physicsVelocity, ref LocalToWorld transform, in PhysicsMass physicsMass, in BoidComponent boid)
        {
            var maxRot = math.radians(config.RotationSpeed);
            var adjustedRotation = RotateTowards(math.normalizesafe(physicsVelocity.Linear, transform.Forward), boid.optimalDirection, maxRot, 0f);

            physicsVelocity.Linear += (adjustedRotation - physicsVelocity.Linear) * DeltaTime;

            // This should force the boid to rotate towards the direction it wants to go.
            var diff = boid.optimalDirection - transform.Forward;
            physicsVelocity.Angular = diff;
            // var ql = quaternion.LookRotation(adjustedRotation, math.up());

            // transform.Value = float4x4.TRS(
            // transform.Position + math.normalizesafe(transform.Forward) * DeltaTime * config.Speed,
            // ql,
            // new float3(1f));


            // transform.Value = float4x4.TRS(
            //                 transform.Position,
            //                 ql,
            //                 new float3(1f));


            // var force = boid.optimalDirection * 0.1f * DeltaTime;
            // physicsVelocity.ApplyLinearImpulse(physicsMass, force);

            // localToWorld.Value = float4x4.TRS(
            //     localToWorld.Position + math.normalizesafe(localToWorld.Forward) * DeltaTime * config.Speed,
            //     ql,
            //     new float3(1f));
        }

        public float3 RotateTowards(float3 current, float3 target, float maxRadsDelta, float maxMag)
        {
            float delta = math.acos(math.dot(current, target) / (math.length(current) * math.length(target)));
            float magDiff = math.length(target) - math.length(current);
            float sign = math.sign(magDiff);
            float maxMagDelta = math.min(maxMag, math.abs(magDiff));
            float diff = math.min(1f, maxRadsDelta / delta);

            return float3Slerp(current, target, diff) * (math.length(current) + maxMagDelta * sign);
        }

        public float3 float3Slerp(float3 current, float3 target, float scale)
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