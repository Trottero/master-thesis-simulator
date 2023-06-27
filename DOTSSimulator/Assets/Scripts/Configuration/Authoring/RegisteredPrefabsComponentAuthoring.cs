using Simulator.Configuration.Components;
using Unity.Entities;
using UnityEngine;

namespace Simulator.Configuration.Authoring
{
    public class RegisteredPrefabsComponentAuthoring : MonoBehaviour
    {
        public GameObject FoodSourcePrefab;
    }
    
    public class RegisteredPrefabsComponentBaker : Baker<RegisteredPrefabsComponentAuthoring>
    {
        public override void Bake(RegisteredPrefabsComponentAuthoring authoring)
        {
            var entity = GetEntity(authoring, TransformUsageFlags.None);
            var prefabEntity = GetEntity(authoring.FoodSourcePrefab, TransformUsageFlags.Dynamic | TransformUsageFlags.NonUniformScale);
            
            AddComponent(entity, new RegisteredPrefabsComponent
            {
                FoodSourcePrefab = prefabEntity
            });
        }
    }
}