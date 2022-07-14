using System.Collections;
using System.Collections.Generic;
using Simulator.Boids;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Simulator.Curves
{
    public class CurveConverter : MonoBehaviour, IConvertGameObjectToEntity
    {
        public int NumberOfSamples = 16;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var controller = GetComponent<BoidController>();
            var seperationCurve = controller.configuration.SeperationCurve;
            var cohesionCurve = controller.configuration.CohesionCurve;
            var alignmentCurve = controller.configuration.AlignmentCurve;

            ConvertCurve<SeperationCurveReference>(entity, dstManager, seperationCurve);
            ConvertCurve<AlignmentCurveReference>(entity, dstManager, alignmentCurve);
            ConvertCurve<CohesionCurveReference>(entity, dstManager, cohesionCurve);
        }

        private void ConvertCurve<T>(Entity entity, EntityManager dstManager, AnimationCurve curve) where T : struct, ICurveReference
        {
            var blobBuilder = new BlobBuilder(Allocator.Temp);
            ref var sampledCurve = ref blobBuilder.ConstructRoot<CurveStruct>();
            var sampledCurveArray = blobBuilder.Allocate(ref sampledCurve.SampledPoints, NumberOfSamples);
            sampledCurve.NumberOfSamples = NumberOfSamples;

            for (var i = 0; i < NumberOfSamples; i++)
            {
                var samplePoint = (float)i / (float)(NumberOfSamples - 1);
                var sampleValue = curve.Evaluate(samplePoint);
                sampledCurveArray[i] = sampleValue;
            }

            var blobAssetReference = blobBuilder.CreateBlobAssetReference<CurveStruct>(Allocator.Persistent);

            var curveReference = new T { CurveReference = blobAssetReference };
            dstManager.AddComponentData(entity, curveReference);
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}