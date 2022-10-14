
using Simulator.Boids;
using Simulator.Boids.Energy;
using Unity.Entities;

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
    }
}