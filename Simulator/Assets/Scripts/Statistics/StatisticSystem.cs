using System;
using System.Collections;
using System.Collections.Generic;
using Simulator.Boids;
using Simulator.Boids.Energy;
using Simulator.Configuration;
using Simulator.Curves;
using Unity.Entities;
using UnityEngine;

namespace Simulator.Statistics
{
    [UpdateInGroup(typeof(StatisticSystemGroup))]
    public partial class StatisticSystem : SystemBase
    {
        private SimulationConfigurationComponent _simulationConfiguration;

        private Entity _statisticEntity;

        private EntityQuery _boidQuery;

        private StatisticWriter _statisticWriter;

        protected override void OnCreate()
        {
            base.OnCreate();
            _boidQuery = GetEntityQuery(typeof(BoidComponent));
            var id = DateTime.UtcNow.ToString("yyyy-MM-dd-HH-mm-ss");
            _statisticWriter = new StatisticWriter(id);
        }
        protected override void OnStartRunning()
        {
            base.OnStartRunning();
            var _gameControllerEntity = GetSingletonEntity<BoidControllerTag>();
            _simulationConfiguration = GetComponent<SimulationConfigurationComponent>(_gameControllerEntity);

            _statisticEntity = GetSingletonEntity<StatisticComponentData>();
        }

        protected override void OnUpdate()
        {
            var step = GetComponent<StatisticComponentData>(_statisticEntity).Step;

            var boidCount = _boidQuery.CalculateEntityCount();

            var avgenergy = 0f;
            Entities.WithAll<BoidComponent>().ForEach((in EnergyComponent energy) =>
            {
                avgenergy += energy.EnergyLevel;
            }).Run();

            avgenergy /= boidCount;

            _statisticWriter.Write(step * _simulationConfiguration.MaxSimulationSpeed, boidCount, avgenergy);

            SetComponent(_statisticEntity, new StatisticComponentData { Step = step + 1 });
        }
    }

}