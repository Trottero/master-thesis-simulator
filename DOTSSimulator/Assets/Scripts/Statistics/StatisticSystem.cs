using System;
using System.Collections.Generic;
using System.Linq;
using Simulator.Configuration;
using Simulator.Curves;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Simulator.Statistics
{

    public class Statistic
    {
        public EntityQuery Query;
        public string Name;
        public float Value;
        public Dictionary<string, object> StatisticBag = new Dictionary<string, object>();
        public Action<StatisticSystem, Statistic> Init;
        public Action<StatisticSystem, Statistic> PreAggregator;
        public Action<StatisticSystem, Statistic, Entity> Aggregator;
        public Action<StatisticSystem, Statistic> PostAggregator;
    }

    [UpdateInGroup(typeof(StatisticSystemGroup))]
    public partial class StatisticSystem : SystemBase
    {
        private SimulationConfigurationComponent _simulationConfiguration;

        private List<Statistic> _statistics = new List<Statistic>()
        {
            Metrics.StatisticStep,
            Metrics.AverageEnergy,
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
            RequireForUpdate<SimulationConfigurationComponent>();
        }

        protected override void OnStartRunning()
        {
            base.OnStartRunning();

            _statisticEntity = SystemAPI.GetSingletonEntity<StatisticComponentData>();
            _simulationConfiguration = SystemAPI.GetSingleton<SimulationConfigurationComponent>();

            // Init all statistics
            foreach (var statistic in _statistics)
            {
                statistic.Init?.Invoke(this, statistic);
            }
        }

        protected override void OnUpdate()
        {
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

            var step = EntityManager.GetComponentData<StatisticComponentData>(_statisticEntity).Step;
            EntityManager.SetComponentData(_statisticEntity, new StatisticComponentData { Step = step + (long)_simulationConfiguration.MaxSimulationSpeed });
        }
    }
}