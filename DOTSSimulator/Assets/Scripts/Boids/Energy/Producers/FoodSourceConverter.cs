using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Simulator.Boids.Energy.Producers
{
    public class FoodSourceConverter : MonoBehaviour
    {
        public float InitialEnergyLevel = 100f;
        public float RegenarationRate = 2f;
        public float MaxEnergyLevel => 100f;
    }

    public class FoodSourceConverterBaker : Baker<FoodSourceConverter>
    {
        public override void Bake(FoodSourceConverter authoring)
        {
            var foodSource = new FoodSourceComponent
            {
                EnergyLevel = authoring.InitialEnergyLevel,
                RegenarationRate = authoring.RegenarationRate,
                MaxEnergyLevel = authoring.MaxEnergyLevel
            };

            AddComponent(foodSource);
            AddComponent(new PostTransformScale { Value = float3x3.Scale(new float3(1, foodSource.EffectiveSize, 1)) });
        }
    }
}