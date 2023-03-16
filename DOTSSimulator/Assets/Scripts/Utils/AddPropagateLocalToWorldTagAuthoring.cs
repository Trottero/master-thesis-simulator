using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Utils
{
    public class AddPropagateLocalToWorldTagAuthoring : MonoBehaviour
    {
    }

    public class AddPropagateLocalToWorldTagBaker : Baker<AddPropagateLocalToWorldTagAuthoring>
    {
        public override void Bake(AddPropagateLocalToWorldTagAuthoring authoring)
        {
            AddComponent<PropagateLocalToWorld>();
        }
    }
}