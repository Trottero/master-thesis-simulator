using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
namespace Simulator.Boids.Energy.Producers
{
    public struct FoodSourceComponent : IComponentData
    {
        public float EnergyLevel;
        public float RegenarationRate;
        public float MaxEnergyLevel;
        public float FeedingRadius;
        public float EffectiveSize => EnergyLevel == 0 ? 0 : (EnergyLevel / 10f);

        public float EffectiveDistance(float3 position, float3 foodpos)
        {
            return math.distance(position, EffectivePosition(position, foodpos));
        }

        public float3 EffectivePosition(float3 position, float3 foodpos)
        {
            // Boid above food source
            if ((position.y - foodpos.y) < 0)
            {
                return foodpos;
            }

            // Above upperbound
            if (position.y > foodpos.y + EffectiveSize)
            {
                return new float3(foodpos.x, foodpos.y + EffectiveSize, foodpos.z);
            }

            // Use y for position calculation
            return new float3(foodpos.x, position.y, foodpos.z);
        }
    }
}
