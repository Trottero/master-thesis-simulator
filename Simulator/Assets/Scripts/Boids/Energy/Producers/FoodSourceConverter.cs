using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Simulator.Boids.Energy.Producers
{
    public class FoodSourceConverter : MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new FoodSourceComponent
            {
                EnergyLevel = 10f,
                RegenarationRate = 0.1f
            });
        }
    }
}
