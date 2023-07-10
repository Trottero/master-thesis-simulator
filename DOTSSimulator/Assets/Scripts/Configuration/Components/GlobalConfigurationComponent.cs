using Unity.Entities;

namespace Simulator.Configuration.Components
{
    [System.Serializable]
    public struct GlobalConfigurationComponent : IComponentData
    {
        public SimulationFrameworkConfigurationComponent SimulationFrameworkConfiguration;
        public BoidsConfigurationComponent BoidsConfiguration;
        public EnergyConfigurationComponent EnergyConfiguration;
        public ReproductionConfigurationComponent ReproductionConfiguration;
        public SchoolConfigurationComponent SchoolConfiguration;
        public FoodSourcesConfigurationComponent FoodSourcesConfiguration;
        public RainbowTroutEnergyConfigurationComponent RainbowTroutEnergyConfiguration;
    }
}
