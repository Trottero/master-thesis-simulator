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
                var values = new float[entities.Length];
                for (var i = 0; i < entities.Length; i++)
                {
                    values[i] = selector(system, entities[i]);
                }
                
                // Calculate index of percentile
                var index = (int)Mathf.Round(percentile * entities.Length);
                
                // Sort values
                Array.Sort(values);
                
                // Set value
                statistic.Value = values[index];

                entities.Dispose();
            };
        }
        
    }
}