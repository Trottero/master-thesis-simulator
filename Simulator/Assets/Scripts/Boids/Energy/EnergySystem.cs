using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

namespace Simulator.Boids.Energy
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial class EnergySystem : SystemBase
    {
        private BoidController controller;

        protected override void OnUpdate()
        {
            if (!controller)
            {
                controller = BoidController.Instance;
                return;
            }

            var dt = Time.fixedDeltaTime;

            // Update energy level
            Entities.WithAll<BoidComponent, RenderMesh>().WithoutBurst().ForEach((ref EnergyComponent energy) =>
            {
                energy.EnergyLevel -= controller.configuration.EnergyConfig.ConsumptionRate * dt;
                // mesh.material.color = new Color(1f, 1f, 1f, energy.EnergyLevel);
            }).ScheduleParallel();

            var ecb = new EntityCommandBuffer(Allocator.TempJob);

            // Delete enties with energy level below 0
            Entities.WithAll<BoidComponent, EnergyComponent>().WithoutBurst().ForEach((Entity e, in EnergyComponent energy) =>
                {
                    if (energy.EnergyLevel < 0)
                    {
                        ecb.DestroyEntity(e);
                    }
                }).Run();

            ecb.Playback(EntityManager);
            ecb.Dispose();

            // ecb.DestroyEntitiesForEntityQuery(GetEntityQuery(typeof(EnergyComponent), typeof(RenderMesh)));
        }
    }
}