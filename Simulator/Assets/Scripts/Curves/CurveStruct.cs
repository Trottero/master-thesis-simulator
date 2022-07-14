using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Simulator.Curves
{
    public struct CurveStruct
    {
        public BlobArray<float> SampledPoints;
        public int NumberOfSamples;

        public float GetValueAtTime(float time)
        {
            var approxSampleIndex = (NumberOfSamples - 1) * time;
            var sampleIndexBelow = (int)math.floor(approxSampleIndex);
            if (sampleIndexBelow >= NumberOfSamples - 1)
            {
                return SampledPoints[NumberOfSamples - 1];
            }
            var indexRemainder = approxSampleIndex - sampleIndexBelow;
            return math.lerp(SampledPoints[sampleIndexBelow], SampledPoints[sampleIndexBelow + 1], indexRemainder);
        }
    }

    public interface ICurveReference : IComponentData
    {
        // We dont store these as sharedcomponents, as that would mean
        // That they are managed objects which in turn hurts performance
        // As the shared component cause incompatibility with the burst compiler
        BlobAssetReference<CurveStruct> CurveReference { get; set; }
        float Evaluate(float time);
    }

    public struct SeperationCurveReference : ICurveReference
    {
        public BlobAssetReference<CurveStruct> CurveReference { get; set; }
        public readonly float Evaluate(float time) => CurveReference.Value.GetValueAtTime(time);
    }

    public struct AlignmentCurveReference : ICurveReference
    {
        public BlobAssetReference<CurveStruct> CurveReference { get; set; }
        public readonly float Evaluate(float time) => CurveReference.Value.GetValueAtTime(time);
    }

    public struct CohesionCurveReference : ICurveReference
    {
        public BlobAssetReference<CurveStruct> CurveReference { get; set; }
        public readonly float Evaluate(float time) => CurveReference.Value.GetValueAtTime(time);
    }
}