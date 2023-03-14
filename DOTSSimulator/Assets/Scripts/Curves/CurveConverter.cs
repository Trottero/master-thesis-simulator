using System.Collections;
using System.Collections.Generic;
using Simulator.Boids;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Simulator.Curves
{
    public class CurveConverter : MonoBehaviour
    {
        public int NumberOfSamples = 16;
        public AnimationCurve separationCurve;
        public AnimationCurve cohesionCurve;
        public AnimationCurve alignmentCurve;
        public AnimationCurve energyCurve;

    }

    public class CurveBaker : Baker<CurveConverter>
    {
        public override void Bake(CurveConverter authoring)
        {
            ConvertCurve<SeparationCurveReference>(authoring.separationCurve, authoring.NumberOfSamples);
            ConvertCurve<AlignmentCurveReference>(authoring.alignmentCurve, authoring.NumberOfSamples);
            ConvertCurve<CohesionCurveReference>(authoring.cohesionCurve, authoring.NumberOfSamples);
            ConvertCurve<EnergyCurveReference>(authoring.energyCurve, authoring.NumberOfSamples);
        }

        private void ConvertCurve<T>(AnimationCurve curve, int numberOfSamples) where T : unmanaged, ICurveReference
        {
            var blobBuilder = new BlobBuilder(Allocator.Temp);
            ref var sampledCurve = ref blobBuilder.ConstructRoot<CurveStruct>();
            var sampledCurveArray = blobBuilder.Allocate(ref sampledCurve.SampledPoints, numberOfSamples);
            sampledCurve.NumberOfSamples = numberOfSamples;

            for (var i = 0; i < numberOfSamples; i++)
            {
                var samplePoint = (float)i / (float)(numberOfSamples - 1);
                var sampleValue = curve.Evaluate(samplePoint);
                sampledCurveArray[i] = sampleValue;
            }

            var blobAssetReference = blobBuilder.CreateBlobAssetReference<CurveStruct>(Allocator.Persistent);

            var curveReference = new T { CurveReference = blobAssetReference };
            AddComponent(curveReference);

            blobBuilder.Dispose();
        }
    }
}