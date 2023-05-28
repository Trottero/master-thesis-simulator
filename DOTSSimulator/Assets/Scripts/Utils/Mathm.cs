using Unity.Burst;

namespace Simulator.Utils
{
    [BurstCompile]
    public static class Mathm
    {
        /// <summary>
        /// Returns the larger of two decimal numbers.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>Largest of a and b</returns>
        public static decimal Max(decimal a, decimal b) => a > b ? a : b;
        
        /// <summary>
        /// Returns the smaller of two decimal numbers.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>Smallest of a and b</returns>
        public static decimal Min(decimal a, decimal b) => a < b ? a : b;
        
        /// <summary>
        /// Clamps a value between a minimum and maximum value.
        /// </summary>
        /// <param name="value">Value to clamp</param>
        /// <param name="min">Min to clamp to</param>
        /// <param name="max">Max to clamp to</param>
        /// <returns><see cref="value"/> But within the range of <see cref="min"/> and <see cref="max"/> </returns>
        public static decimal Clamp(decimal value, decimal min, decimal max)
        {
            return Max(min, Min(max, value));
        }
    }
}