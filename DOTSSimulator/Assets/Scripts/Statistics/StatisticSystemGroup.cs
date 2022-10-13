using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Simulator.Statistics
{
    // StatisticSystemGroup runs very infrequently
    // It is used to collect statistics about the simulation
    public class StatisticSystemGroup : FixedStepSimulationSystemGroup
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            Timestep = 1f;
        }
    }
}