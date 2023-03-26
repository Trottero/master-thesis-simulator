using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Simulator.Boids
{
    public class SchoolAuthoring : MonoBehaviour
    {
        public float CageSize = 10f;
        public int SwarmSize = 100;
    }

    public struct SchoolComponentData : IComponentData
    {
        public float CageSize;
        public int SwarmSize;
    }

    public class SchoolAuthoringBaker : Baker<SchoolAuthoring>
    {
        public override void Bake(SchoolAuthoring authoring)
        {
            var e = GetEntity(authoring, TransformUsageFlags.None);
            AddComponent(e, new SchoolComponentData
            {
                CageSize = authoring.CageSize,
                SwarmSize = authoring.SwarmSize
            });
        }
    }
}