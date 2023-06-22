using Simulator.Configuration.Components;
using Unity.Entities;
using UnityEngine;

namespace Simulator.Configuration.Authoring
{
    public class ReproductionConfigurationComponentAuthoring : MonoBehaviour
    {
        public double MinWeightForReproduction;
        public double ReproductionWeightLoss;
        public double OffspringWeight;
        public bool ReproductionEnabled;

    }
    public class ReproductionConfigurationComponentBaker : Baker<ReproductionConfigurationComponentAuthoring>
    {
        public override void Bake(ReproductionConfigurationComponentAuthoring authoring)
        {
            var configuration = new ReproductionConfigurationComponent
            {
                MinWeightForReproduction = authoring.MinWeightForReproduction,
                ReproductionWeightLoss = authoring.ReproductionWeightLoss,
                OffspringWeight = authoring.OffspringWeight,
                ReproductionEnabled = authoring.ReproductionEnabled
            };
            
            var entity = GetEntity(authoring, TransformUsageFlags.None);
            AddComponent(entity, configuration);
        }
    }
}