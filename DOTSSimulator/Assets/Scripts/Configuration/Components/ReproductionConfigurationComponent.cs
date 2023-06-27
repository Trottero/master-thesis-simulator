using Unity.Entities;

namespace Simulator.Configuration.Components
{
    [System.Serializable]
    public struct ReproductionConfigurationComponent: IComponentData
    {
        public double MinWeightForReproduction;
        public double ReproductionWeightLoss;
        public double OffspringWeight;
        public bool ReproductionEnabled;
    }
}