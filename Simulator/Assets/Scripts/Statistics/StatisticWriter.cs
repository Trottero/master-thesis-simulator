using System.IO;
using UnityEngine;

namespace Simulator.Statistics
{
    public class StatisticWriter
    {
        private readonly string _simulationId;
        private readonly string _dir;
        private readonly string _filepath;
        public StatisticWriter(string simulationId)
        {
            _simulationId = simulationId;
            _dir = Path.Combine(Application.persistentDataPath, "Simulation");
            Directory.CreateDirectory(_dir);

            _filepath = Path.Combine(_dir, $"{_simulationId}.csv");
            Debug.Log($"Writing statistics to {_filepath}");
            if (!File.Exists(_filepath))
            {
                Write("Step", "BoidCount", "AvgEnergy");
            }
        }

        public void Write(params object[] values)
        {
            var line = string.Join(",", values);
            File.AppendAllText(_filepath, line + "\n");
        }
    }
}