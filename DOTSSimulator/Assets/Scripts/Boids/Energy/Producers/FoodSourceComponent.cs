using Unity.Entities;
using Unity.Mathematics;

namespace Simulator.Boids.Energy.Producers
{
    public struct FoodSourceComponent : IComponentData
    {
        public decimal EnergyLevel;
        public decimal RegenarationRate;
        public decimal MaxEnergyLevel;
        public float FeedingRadius;
        public float EffectiveSize => EnergyLevel == 0 ? 0 : ((float)EnergyLevel / 10f);

        public float EffectiveDistance(float3 position, float3 foodPosition)
        {
            return math.distance(position, EffectivePosition(position, foodPosition));
        }

        public float3 EffectivePosition(float3 position, float3 foodPosition)
        {
            // Check if boid is below food position
            if (position.y < foodPosition.y)
            {
                return foodPosition;
            }

            // Above upperbound
            if (position.y > foodPosition.y + EffectiveSize)
            {
                return new float3(foodPosition.x, foodPosition.y + EffectiveSize, foodPosition.z);
            }

            // Use boid y for effective position
            return new float3(foodPosition.x, position.y, foodPosition.z);
        }
    }
}