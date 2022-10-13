using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Simulator.Boids.Energy.Producers
{
    public class FoodSourceConverter : MonoBehaviour, IConvertGameObjectToEntity
    {
        public float InitialEnergyLevel = 100f;
        public float RegenarationRate = 2f;
        public float MaxEnergyLevel => 100f;
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new FoodSourceComponent
            {
                EnergyLevel = InitialEnergyLevel,
                RegenarationRate = RegenarationRate,
                MaxEnergyLevel = MaxEnergyLevel
            });
        }
    }
}
