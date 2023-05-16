using Unity.Entities;
using Unity.Physics.Systems;

namespace Framework
{
    [UpdateInGroup(typeof (FixedStepSimulationSystemGroup))]
    [UpdateBefore(typeof(PhysicsSystemGroup))]
    public partial class FrameworkFixedSystemGroup : ComponentSystemGroup
    {
        
    }
}