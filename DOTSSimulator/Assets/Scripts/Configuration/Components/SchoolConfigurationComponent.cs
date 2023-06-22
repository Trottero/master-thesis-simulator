using Unity.Entities;

namespace Simulator.Configuration.Components
{
    public struct SchoolConfigurationComponent : IComponentData
    {
        public float CageSize;
        public int SwarmSize;
    }
}