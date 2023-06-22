using Simulator.Configuration.Components;
using Unity.Entities;
using UnityEngine;

namespace Simulator.Configuration.Authoring
{
    public class SchoolConfigurationComponentAuthoring : MonoBehaviour
    {
        public float CageSize = 10f;
        public int SwarmSize = 100;
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(CageSize, CageSize, CageSize));
        }
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