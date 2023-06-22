using Simulator.Configuration.Components;
using Unity.Entities;
using UnityEngine;

namespace Simulator.Configuration.Authoring
{
    public class SchoolConfigurationComponentAuthoring : MonoBehaviour
    {
        public float CageSize = 10f;
        public int SwarmSize = 100;
    }
    
    public class SchoolAuthoringBaker : Baker<SchoolConfigurationComponentAuthoring>
    {
        public override void Bake(SchoolConfigurationComponentAuthoring configurationComponent)
        {
            var e = GetEntity(configurationComponent, TransformUsageFlags.None);
            AddComponent(e, new SchoolConfigurationComponent
            {
                CageSize = configurationComponent.CageSize,
                SwarmSize = configurationComponent.SwarmSize
            });
        }
    }
}