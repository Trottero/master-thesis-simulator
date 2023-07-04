using System;
using System.Collections.Generic;
using System.Linq;
using Simulator.Boids.Lifecycle;
using Simulator.Configuration.Components;
using Simulator.Framework;
using Unity.Collections;
using Unity.Entities;

namespace Simulator.Statistics
{
    public class Statistic
    {
        public EntityQuery Query;
        public string Name;
        public float Value;
        public Dictionary<string, object> StatisticBag = new();
        public Action<StatisticSystem, Statistic> Init;
        public Action<StatisticSystem, Statistic> PreAggregator;
        public Action<StatisticSystem, Statistic, Entity> Aggregator;
        public Action<StatisticSystem, Statistic> PostAggregator;
    }

    [UpdateInGroup(typeof(FrameworkFixedSystemGroup))]
    [UpdateAfter(typeof(BoidSpawningSystem))]
    public partial class StatisticSystem : SystemBase
    {
        private SimulationFrameworkConfigurationComponent _simulationFrameworkConfiguration;

        private List<Statistic> _statistics = new()
        {
            Metrics.StatisticStep,
            Metrics.TimeStamp,
            Metrics.AverageEnergy,
            Metrics.AverageEnergy5thPercentile,
            Metrics.AverageEnergy95thPercentile,
            Metrics.NumberOfBoids,
            Metrics.NumberOfFoodSources,
            Metrics.TotalFoodAvailable,
            Metrics.Polarization,
            Metrics.Expanse
        };

        private Entity _statisticEntity;
        private StatisticWriter _statisticWriter;

        // Please close your eyes before looking at the next 3 lines of code
        public new EntityQuery GetEntityQuery(params ComponentType[] componentTypes)
        {
            return base.GetEntityQuery(componentTypes);
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            var id = DateTime.UtcNow.ToString("yyyy-MM-dd-HH-mm-ss");
            _statisticWriter = new StatisticWriter(id, _statistics.Select(s => s.Name).ToArray());

            RequireForUpdate<StatisticComponentData>();
            RequireForUpdate<GlobalConfigurationComponent>();
        }

        protected override void OnStartRunning()
        {
            base.OnStartRunning();

            _statisticEntity = SystemAPI.GetSingletonEntity<StatisticComponentData>();
            _simulationFrameworkConfiguration = SystemAPI.GetSingleton<GlobalConfigurationComponent>().SimulationFrameworkConfiguration;

            // Init all statistics
            foreach (var statistic in _statistics)
            {
                statistic.Init?.Invoke(this, statistic);
            }
        }

        protected override void OnUpdate()
        {
            var statistics = EntityManager.GetComponentData<StatisticComponentData>(_statisticEntity);
            if (statistics.Step != 0)
            {
                EntityManager.SetComponentData(_statisticEntity, new StatisticComponentData { Step = (statistics.Step + 1) % (long)(3600 * _simulationFrameworkConfiguration.UpdatesPerSecond), MetaStep = statistics.MetaStep});
                return;
            }
            
            // Pre-aggregator
            foreach (var statistic in _statistics)
            {
                statistic.PreAggregator?.Invoke(this, statistic);
            }
            
            // Aggregator
            foreach (var statistic in _statistics)
            {
                var query = statistic.Query;
                var aggregator = statistic.Aggregator;
                var entities = query.ToEntityArray(Allocator.TempJob);
                foreach (var entity in entities)
                {
                    aggregator?.Invoke(this, statistic, entity);
                }
                entities.Dispose();
            }

            // Post-aggregator
            foreach (var statistic in _statistics)
            {
                statistic.PostAggregator?.Invoke(this, statistic);
            }

            // Create list of all statistic values
            var values = new List<float>();
            foreach (var statistic in _statistics)
            {
                values.Add(statistic.Value);
            }

            _statisticWriter.Write(values.Select(x => x.ToString()).ToArray());
            
            EntityManager.SetComponentData(_statisticEntity, new StatisticComponentData { Step = (statistics.Step + 1) % (long)(3600 * _simulationFrameworkConfiguration.UpdatesPerSecond), MetaStep = statistics.MetaStep + 1});
        }
    }
}