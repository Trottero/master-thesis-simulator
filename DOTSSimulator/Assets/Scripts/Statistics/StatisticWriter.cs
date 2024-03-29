using System.IO;
using UnityEngine;

namespace Simulator.Statistics
{
    public class StatisticWriter
    {
        private readonly string _simulationId;
        private readonly string _dir;
        private readonly string _filepath;
        public StatisticWriter(string simulationId, string[] headers)
        {
            _simulationId = simulationId;
            _dir = Path.Combine(Application.dataPath, "SimulationResults");
            Directory.CreateDirectory(_dir);

            _filepath = Path.Combine(_dir, $"{_simulationId}.csv");
            Debug.Log($"Writing statistics to {_filepath}");
            if (!File.Exists(_filepath))
            {
                Write(headers);
            }
        }

        public void Write(string[] values)
        {
            var line = string.Join(",", values);
            File.AppendAllText(_filepath, line + "\n");
        }
    }
}