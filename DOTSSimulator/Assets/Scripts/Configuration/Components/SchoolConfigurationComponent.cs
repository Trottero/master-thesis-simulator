using Unity.Entities;

namespace Simulator.Configuration.Components
{
    [System.Serializable]
    public struct SchoolConfigurationComponent : IComponentData
    {
        public float CageSize;
        public int SwarmSize;
    }
}