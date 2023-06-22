
using System;
using System.Collections.Generic;
using Simulator.Boids;
using Simulator.Boids.Energy;
using Simulator.Boids.Energy.Producers;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;

namespace Simulator.Statistics
{
    public static class Metrics
    {
        public static readonly Statistic StatisticStep = new()
        {
            Name = "Step",
            Init = (statisticSystem, statistic) =>
            {
                statistic.Query = statisticSystem.GetEntityQuery(typeof(StatisticComponentData));
            },
            Aggregator = (statisticSystem, statistic, entity) =>
            {
                var step = statisticSystem.EntityManager.GetComponentData<StatisticComponentData>(entity);
                statistic.Value = step.MetaStep;
            }
        };

        public static readonly Statistic TimeStamp = new()
        {
            Name = "TimeStamp",
            StatisticBag = new Dictionary<string, object>(),
            Init = (statisticSystem, statistic) =>
            {
                statistic.StatisticBag["StartTime"] = DateTime.UtcNow;
                statistic.Query = statisticSystem.GetEntityQuery(typeof(StatisticComponentData));
            },
            PreAggregator = (_, statistic) =>
            {
                statistic.Value = (float)(DateTime.UtcNow - (DateTime)statistic.StatisticBag["StartTime"]).TotalSeconds;
            }
        };
        
        public static readonly Statistic AverageEnergy = new()
        {
            Name = "AvgEnergy",
            Init = (statisticSystem, statistic) =>
            {
                statistic.Query = statisticSystem.GetEntityQuery(typeof(BoidComponent));

            },
            PreAggregator = (statisticSystem, statistic) =>
            {
                statistic.Value = 0f;
                statistic.StatisticBag["weight"] = 0m;
            },
            Aggregator = (system, stat, e) =>
            {
                stat.StatisticBag["weight"] = (decimal)stat.StatisticBag["weight"] + system.EntityManager.GetComponentData<EnergyComponent>(e).Weight;
            },
            PostAggregator = (_, stat) =>
            {
                var count = stat.Query.CalculateEntityCount();
                if (count > 0)
                {
                    stat.Value = (float)((decimal)stat.StatisticBag["weight"] / count);
                }
            }
        };

        public static readonly Statistic NumberOfBoids = new()
        {
            Name = "NoBoids",
            Init = (statisticSystem, statistic) =>
            {
                statistic.Query = statisticSystem.GetEntityQuery(typeof(BoidComponent));
            },
            PostAggregator = (_, stat) =>
            {
                stat.Value = stat.Query.CalculateEntityCount();
            }
        };

        public static readonly Statistic NumberOfFoodSources = new()
        {
            Name = "NoFoodSources",
            Init = (statisticSystem, statistic) =>
            {
                statistic.Query = statisticSystem.GetEntityQuery(typeof(FoodSourceComponent));
            },
            PostAggregator = (system, stat) =>
            {
                stat.Value = stat.Query.CalculateEntityCount();
            }
        };

        public static readonly Statistic TotalFoodAvailable = new()
        {
            Name = "TotalFoodAvailable",
            Init = (statisticSystem, statistic) =>
            {
                statistic.Query = statisticSystem.GetEntityQuery(typeof(FoodSourceComponent));
            },
            PreAggregator = (statisticSystem, statistic) =>
            {
                statistic.Value = 0f;
            },
            Aggregator = (system, stat, e) =>
            {
                stat.Value += (float)system.EntityManager.GetComponentData<FoodSourceComponent>(e).EnergyLevel;
            }
        };

        // Computes the average deviation from the angle of the school of fish.
        // Andreas Huth and Christian Wissel 1992
        public static readonly Statistic Polarization = new()
        {
            Name = "Polarization",
            Init = (statisticSystem, statistic) =>
            {
                statistic.Query = statisticSystem.GetEntityQuery(typeof(BoidComponent), typeof(LocalTransform));
            },
            PreAggregator = (statisticSystem, statistic) =>
            {
                statistic.Value = 0f;
                // Loop over all of the entities satisfying the query and take their rotation
                // and add it to the statistic bag.
                var avgangle = float3.zero;
                var rotations = statistic.Query.ToComponentDataArray<LocalTransform>(Allocator.TempJob);
                foreach (var rotation in rotations)
                {
                    avgangle += math.forward(rotation.Rotation);
                }

                statistic.StatisticBag["avgdirection"] = avgangle / rotations.Length;

                rotations.Dispose();
            },
            Aggregator = (system, stat, e) =>
            {
                var rotation = system.EntityManager.GetComponentData<LocalTransform>(e);
                var schoolangle = (float3)stat.StatisticBag["avgdirection"];
                var angle = math.dot(math.forward(rotation.Rotation), schoolangle);
                // normalize difference to 0..1 where 1 is 180 degrees
                stat.Value += (1f - angle) / 2f;
            },
            PostAggregator = (system, stat) =>
            {
                stat.Value /= stat.Query.CalculateEntityCount();
            }
        };

        // Compute the distance to the average center of the school of fish.
        // Andreas Huth and Christian Wissel 1992
        public static readonly Statistic Expanse = new()
        {
            Name = "Expanse",
            Init = (statisticSystem, statistic) =>
            {
                statistic.Query = statisticSystem.GetEntityQuery(typeof(BoidComponent), typeof(LocalTransform));
            },
            PreAggregator = (statisticSystem, statistic) =>
            {
                statistic.Value = 0f;
                // Loop over all of the entities satisfying the query and take their translation
                // and add it to the statistic bag.
                var avgpos = float3.zero;
                var translations = statistic.Query.ToComponentDataArray<LocalTransform>(Allocator.TempJob);
                foreach (var translation in translations)
                {
                    avgpos += translation.Position;
                }

                statistic.StatisticBag["avgpos"] = avgpos / translations.Length;

                translations.Dispose();
            },
            Aggregator = (system, stat, e) =>
            {
                var translation = system.EntityManager.GetComponentData<LocalTransform>(e);
                var avgpos = (float3)stat.StatisticBag["avgpos"];
                var distance = math.distance(translation.Position, avgpos);
                stat.Value += distance;
            },
            PostAggregator = (system, stat) =>
            {
                stat.Value /= stat.Query.CalculateEntityCount();
            }
        };
    }
}