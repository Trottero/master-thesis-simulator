using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Simulator.Statistics
{
    public struct StatisticComponentData : IComponentData
    {
        public long Step;
    }

    public class StatisticComponentDataAuthoring : MonoBehaviour
    {
        public long Step;
    }

    public class StatisticComponentDataBaker : Baker<StatisticComponentDataAuthoring>
    {
        public override void Bake(StatisticComponentDataAuthoring authoring)
        {
            var statistic = new StatisticComponentData
            {
                Step = authoring.Step
            };

            AddComponent(statistic);
        }
    }
}