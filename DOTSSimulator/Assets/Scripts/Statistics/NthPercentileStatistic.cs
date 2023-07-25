using System;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Simulator.Statistics
{
    public class NthPercentileStatistic : Statistic
    {
        public NthPercentileStatistic(float percentile, Func<StatisticSystem, Entity, float> selector)
        {
            PostAggregator = (system, statistic) =>
            {
                var entities = statistic.Query.ToEntityArray(Allocator.TempJob);
                if (entities.Length == 0)
                {
                    statistic.Value = 0;
                    entities.Dispose();
                    return;
                }
                
                var values = new float[entities.Length];
                for (var i = 0; i < entities.Length; i++)
                {
                    values[i] = selector(system, entities[i]);
                }
                
                // Calculate index of percentile
                var index = Math.Clamp((int)Mathf.Round(percentile * entities.Length), 0, entities.Length - 1);
                
                // Sort values
                Array.Sort(values);
                
                // Set value
                statistic.Value = values[index];

                entities.Dispose();
            };
        }
        
    }
}