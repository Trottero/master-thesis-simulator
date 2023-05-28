using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Simulator.Boids.Energy
{
    public struct EnergyComponent : IComponentData
    {
        public decimal Weight;
    }
}