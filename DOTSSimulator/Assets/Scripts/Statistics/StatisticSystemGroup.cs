using Unity.Entities;

namespace Simulator.Statistics
{
    // StatisticSystemGroup runs very infrequently
    // It is used to collect statistics about the simulation
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class StatisticSystemGroup : ComponentSystemGroup
    {
        public StatisticSystemGroup()
        {
            RateManager = new RateUtils.FixedRateCatchUpManager(1f);
        }
    }
}