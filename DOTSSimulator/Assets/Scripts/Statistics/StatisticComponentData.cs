using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Simulator.Statistics
{
    [GenerateAuthoringComponent]
    public struct StatisticComponentData : IComponentData
    {
        public long Step;
    }
}