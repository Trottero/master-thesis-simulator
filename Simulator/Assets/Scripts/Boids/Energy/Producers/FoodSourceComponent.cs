using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
namespace Simulator.Boids.Energy.Producers
{
    public struct FoodSourceComponent : IComponentData
    {
        public float EnergyLevel;
        public float RegenarationRate;
        public float MaxEnergyLevel;
        public float FeedingRadius => 1f;
        public float EffectiveSize => EnergyLevel == 0 ? 0 : (EnergyLevel / 10f);
    }
}
