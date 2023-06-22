using Simulator.Boids.Energy.Producers.Components;
using Unity.Entities;
using UnityEngine;

namespace Simulator.Boids.Energy.Producers.Authoring
{
    public class FoodSourceComponentAuthoring : MonoBehaviour
    {
        public double InitialEnergyLevel = 100d;

        public double RegenerationRate = 2d;

        public double MaxEnergyLevel = 100d;
    }

    public class FoodSourceConverterBaker : Baker<FoodSourceComponentAuthoring>
    {
        public override void Bake(FoodSourceComponentAuthoring authoring)
        {
            var foodSource = new FoodSourceComponent
            {
                EnergyLevel = (decimal)authoring.InitialEnergyLevel,
                RegenarationRate = (decimal)authoring.RegenerationRate,
                MaxEnergyLevel = (decimal)authoring.MaxEnergyLevel
            };

            var entity = GetEntity(authoring, TransformUsageFlags.NonUniformScale | TransformUsageFlags.Dynamic);
            AddComponent(entity, foodSource);
        }
    }
}