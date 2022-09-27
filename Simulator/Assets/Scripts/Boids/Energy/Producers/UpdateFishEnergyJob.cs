using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;
using Unity.Mathematics;

namespace Simulator.Boids.Energy.Producers
{
    public partial struct UpdateFishEnergyJob : IJobEntity
    {
        public NativeArray<FoodSourceComponent> FoodSourceInformation;
        public NativeArray<LocalToWorld> FoodSourceLocations;
        public EnergyConfiguration EnergyConfig;
        public float DeltaTime;

        void Execute(ref EnergyComponent boidEnergy, in LocalToWorld boidLocation)
        {
            for (int i = 0; i < FoodSourceInformation.Length; i++)
            {
                var foodSource = FoodSourceInformation[i];
                if (foodSource.EnergyLevel <= 5)
                {
                    continue;
                }
                var foodSourceLocation = FoodSourceLocations[i];
                float distance = 0;
                if (boidLocation.Position.y >= foodSourceLocation.Position.y && boidLocation.Position.y <= foodSourceLocation.Position.y + foodSource.EffectiveSize)
                {
                    // Ignore y for distance calculation
                    distance = math.distance(new float3(boidLocation.Position.x, 0, boidLocation.Position.z), new float3(foodSourceLocation.Position.x, 0, foodSourceLocation.Position.z));
                }
                if (boidLocation.Position.y > foodSourceLocation.Position.y + foodSource.EffectiveSize)
                {
                    // Larger than top
                    distance = math.distance(boidLocation.Position, new float3(foodSourceLocation.Position.x, foodSourceLocation.Position.y + foodSource.EffectiveSize, foodSourceLocation.Position.z));
                }
                if (boidLocation.Position.y < foodSourceLocation.Position.y)
                {
                    // Smaller than bottom
                    distance = math.distance(boidLocation.Position, foodSourceLocation.Position);
                }

                if (distance < 1f)
                {
                    // Assimilation rate is 1f
                    float consumed = EnergyConfig.AssimilationRate * DeltaTime;
                    // Add energy to boid
                    boidEnergy.EnergyLevel += consumed;
                    // Remove energy from food source
                    foodSource.EnergyLevel -= consumed;
                }
                FoodSourceInformation[i] = foodSource;
            }
        }
    }
}