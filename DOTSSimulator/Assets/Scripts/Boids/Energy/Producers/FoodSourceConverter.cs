using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

namespace Simulator.Boids.Energy.Producers
{
    public class FoodSourceConverter : MonoBehaviour
    {
        public float InitialEnergyLevel = 100f;

        public float RegenerationRate = 2f;

        public float MaxEnergyLevel = 100f;
    }

    public class FoodSourceConverterBaker : Baker<FoodSourceConverter>
    {
        public override void Bake(FoodSourceConverter authoring)
        {
            var foodSource = new FoodSourceComponent
            {
                EnergyLevel = authoring.InitialEnergyLevel,
                RegenarationRate = authoring.RegenerationRate,
                MaxEnergyLevel = authoring.MaxEnergyLevel
            };

            AddComponent(foodSource);
            AddComponent(new PropagateLocalToWorld());
            AddComponent(new WorldToLocal_Tag());
        }
    }
}