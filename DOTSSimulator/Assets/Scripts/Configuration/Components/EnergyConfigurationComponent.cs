using Unity.Entities;

namespace Simulator.Configuration.Components
{
    [System.Serializable]
    public struct EnergyConfigurationComponent: IComponentData
    {
        public double InitialEnergyLevel;
        public double ConsumptionRate;
        public double FeedingRate;
    }
}