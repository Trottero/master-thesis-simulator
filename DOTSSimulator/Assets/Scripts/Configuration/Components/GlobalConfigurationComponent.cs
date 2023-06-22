using Unity.Entities;

namespace Simulator.Configuration.Components
{
    public struct GlobalConfigurationComponent: IComponentData
    {
        public SimulationFrameworkConfigurationComponent SimulationFrameworkConfiguration;
        public BoidsConfigurationComponent BoidsConfiguration;
        public EnergyConfigurationComponent EnergyConfiguration;
        public ReproductionConfigurationComponent ReproductionConfiguration;
    }
}
