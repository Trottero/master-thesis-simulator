using Unity.Entities;
using Unity.Mathematics;

namespace Simulator.Boids.Energy.Producers.Components
{
    public struct FoodSourceComponent : IComponentData
    {
        public decimal EnergyLevel;
        public decimal RegenerationRate;
        public decimal MaxEnergyLevel;
        public float FeedingRadius;
        public float EffectiveSize => (float)EnergyLevel / 10f;

        public float EffectiveDistance(float3 position, float3 foodPosition)
        {
            return math.distance(position, EffectivePosition(position, foodPosition));
        }

        public float3 EffectivePosition(float3 fishPosition, float3 foodPosition)
        {
            // Check if boid is below food position
            if (fishPosition.y < foodPosition.y)
            {
                return foodPosition;
            }

            // Above upperbound
            if (fishPosition.y > foodPosition.y + EffectiveSize)
            {
                return new float3(foodPosition.x, foodPosition.y + EffectiveSize, foodPosition.z);
            }

            // Use boid y for effective position
            return new float3(foodPosition.x, fishPosition.y, foodPosition.z);
        }
    }
}