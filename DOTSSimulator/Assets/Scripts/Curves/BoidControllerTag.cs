using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Simulator.Curves
{
    public struct BoidControllerTagComponent : IComponentData { }

    public class BoidControllerTag : MonoBehaviour
    {
    }

    public class BoidControllerTagBaker : Baker<BoidControllerTag>
    {
        public override void Bake(BoidControllerTag authoring)
        {
            AddComponent(new BoidControllerTagComponent());
        }
    }
}