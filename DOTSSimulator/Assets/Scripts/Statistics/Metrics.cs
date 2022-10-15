
using Simulator.Boids;
using Simulator.Boids.Energy;
using Simulator.Boids.Energy.Producers;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Simulator.Statistics
{
    public static class Metrics
    {
        public static readonly Statistic StatisticStep = new Statistic
        {
            Name = "Step",
            Init = (statisticSystem, statistic) =>
            {
                statistic.Query = statisticSystem.GetEntityQuery(typeof(StatisticComponentData));
            },
            Aggregator = (statisticSystem, statistic, entity) =>
            {
                var step = statisticSystem.GetComponent<StatisticComponentData>(entity);
                statistic.Value = step.Step;
            }
        };
        public static readonly Statistic AverageEnergy = new Statistic
        {
            Name = "AvgEnergy",
            Init = (statisticSystem, statistic) =>
            {
                statistic.Query = statisticSystem.GetEntityQuery(typeof(BoidComponent));

            },
            PreAggregator = (statisticSystem, statistic) =>
            {
                statistic.Value = 0f;
            },
            Aggregator = (StatisticSystem system, Statistic stat, Entity e) =>
            {
                stat.Value += system.GetComponent<EnergyComponent>(e).EnergyLevel;
            },
            PostAggregator = (StatisticSystem system, Statistic stat) =>
            {
                stat.Value /= stat.Query.CalculateEntityCount();
            }
        };

        public static readonly Statistic NumberOfBoids = new Statistic
        {
            Name = "NoBoids",
            Init = (statisticSystem, statistic) =>
            {
                statistic.Query = statisticSystem.GetEntityQuery(typeof(BoidComponent));
            },
            PostAggregator = (StatisticSystem system, Statistic stat) =>
            {
                stat.Value = stat.Query.CalculateEntityCount();
            }
        };

        public static readonly Statistic NumberOfFoodSources = new Statistic
        {
            Name = "NoFoodSources",
            Init = (statisticSystem, statistic) =>
            {
                statistic.Query = statisticSystem.GetEntityQuery(typeof(FoodSourceComponent));
            },
            PostAggregator = (StatisticSystem system, Statistic stat) =>
            {
                stat.Value = stat.Query.CalculateEntityCount();
            }
        };

        public static readonly Statistic TotalFoodAvailable = new Statistic
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
            Aggregator = (StatisticSystem system, Statistic stat, Entity e) =>
            {
                stat.Value += system.GetComponent<FoodSourceComponent>(e).EnergyLevel;
            }
        };

        // Computes the average deviation from the angle of the school of fish.
        // Andreas Huth and Christian Wissel 1992
        public static readonly Statistic Polarization = new Statistic
        {
            Name = "Polarization",
            Init = (statisticSystem, statistic) =>
            {
                statistic.Query = statisticSystem.GetEntityQuery(typeof(BoidComponent), typeof(Rotation));
            },
            PreAggregator = (statisticSystem, statistic) =>
            {
                statistic.Value = 0f;
                // Loop over all of the entities satisfying the query and take their rotation
                // and add it to the statistic bag.
                var avgangle = float3.zero;
                var rotations = statistic.Query.ToComponentDataArray<Rotation>(Allocator.TempJob);
                foreach (var rotation in rotations)
                {
                    avgangle += math.forward(rotation.Value);
                }

                statistic.StatisticBag["avgdirection"] = avgangle / (float)rotations.Length;

                rotations.Dispose();
            },
            Aggregator = (StatisticSystem system, Statistic stat, Entity e) =>
            {
                var rotation = system.GetComponent<Rotation>(e);
                var schoolangle = (float3)stat.StatisticBag["avgdirection"];
                var angle = math.dot(math.forward(rotation.Value), schoolangle);
                // normalize difference to 0..1 where 1 is 180 degrees
                stat.Value += (1f - angle) / 2f;
            },
            PostAggregator = (StatisticSystem system, Statistic stat) =>
            {
                stat.Value /= stat.Query.CalculateEntityCount();
            }
        };

        // Compute the distance to the average center of the school of fish.
        // Andreas Huth and Christian Wissel 1992
        public static readonly Statistic Expanse = new Statistic
        {
            Name = "Expanse",
            Init = (statisticSystem, statistic) =>
            {
                statistic.Query = statisticSystem.GetEntityQuery(typeof(BoidComponent), typeof(Translation));
            },
            PreAggregator = (statisticSystem, statistic) =>
            {
                statistic.Value = 0f;
                // Loop over all of the entities satisfying the query and take their translation
                // and add it to the statistic bag.
                var avgpos = float3.zero;
                var translations = statistic.Query.ToComponentDataArray<Translation>(Allocator.TempJob);
                foreach (var translation in translations)
                {
                    avgpos += translation.Value;
                }

                statistic.StatisticBag["avgpos"] = avgpos / (float)translations.Length;

                translations.Dispose();
            },
            Aggregator = (StatisticSystem system, Statistic stat, Entity e) =>
            {
                var translation = system.GetComponent<Translation>(e);
                var avgpos = (float3)stat.StatisticBag["avgpos"];
                var distance = math.distance(translation.Value, avgpos);
                stat.Value += distance;
            },
            PostAggregator = (StatisticSystem system, Statistic stat) =>
            {
                stat.Value /= stat.Query.CalculateEntityCount();
            }
        };
    }
}