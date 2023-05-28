using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

namespace Simulator.Boids.Energy.Producers
{
    public class FoodSourceConverter : MonoBehaviour
    {
        public double InitialEnergyLevel = 100d;

        public double RegenerationRate = 2d;

        public double MaxEnergyLevel = 100d;
    }

    public class FoodSourceConverterBaker : Baker<FoodSourceConverter>
    {
        public override void Bake(FoodSourceConverter authoring)
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