using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

using Simulator.Boids;

namespace Simulator.Curves
{
    public partial class InitializeAccelerationCurveSystem : SystemBase
    {
        private Entity _gameControllerEntity;

        protected override void OnStartRunning()
        {
            _gameControllerEntity = GetSingletonEntity<BoidControllerTag>();
        }

        protected override void OnUpdate()
        {
            var ecb = new EntityCommandBuffer(Allocator.TempJob);
            var gameControllerEntity = _gameControllerEntity;
            Entities
                .WithAll<BoidComponent>()
                .WithNone<SeparationCurveReference, CohesionCurveReference, AlignmentCurveReference>()
                .ForEach((Entity e) =>
                {
                    ecb.AddComponent<SeparationCurveReference>(e);
                    ecb.AddComponent<CohesionCurveReference>(e);
                    ecb.AddComponent<AlignmentCurveReference>(e);

                    var separationCurveReference = GetComponent<SeparationCurveReference>(gameControllerEntity);
                    var cohesionCurveReference = GetComponent<CohesionCurveReference>(gameControllerEntity);
                    var alignmentCurveReference = GetComponent<AlignmentCurveReference>(gameControllerEntity);

                    ecb.SetComponent(e, separationCurveReference);
                    ecb.SetComponent(e, cohesionCurveReference);
                    ecb.SetComponent(e, alignmentCurveReference);

                }).Run();

            ecb.Playback(EntityManager);
            ecb.Dispose();
        }
    }
}