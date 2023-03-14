using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Scripting;

namespace Simulator.Statistics
{
    // StatisticSystemGroup runs very infrequently
    // It is used to collect statistics about the simulation
    public class StatisticSystemGroup : ComponentSystemGroup
    {
        [Preserve]
        public StatisticSystemGroup()
        {
            RateManager = new RateUtils.FixedRateCatchUpManager(1f);
        }
    }
}