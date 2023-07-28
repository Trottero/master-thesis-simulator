using Simulator.Boids.Energy.Producers.Components;
using Simulator.Configuration.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Simulator.Boids.Energy.Jobs
{
    [BurstCompile]
    public partial struct RainbowTroutEnergyJob : IJobEntity
    {
        [ReadOnly] public EnergyConfigurationComponent EnergyConfig;
        [ReadOnly] public SimulationFrameworkConfigurationComponent SimulationFrameworkConfig;
        [ReadOnly] public RainbowTroutEnergyConfigurationComponent RainbowTroutEnergyConfig;

        [ReadOnly] public NativeArray<LocalToWorld> FoodSourceLocations;
        public NativeArray<FoodSourceComponent> FoodSourceInformation;


        void Execute(ref EnergyComponent fishEnergy, in LocalToWorld boidLocation)
        {
            const int secondsInDay = 24 * 60 * 60;
            var foodIndex = GetFoodIndex(boidLocation);
            var gain = getNextWeight(fishEnergy.Weight, (decimal)SimulationFrameworkConfig.UpdateInterval, foodIndex);
            fishEnergy.Weight = gain;
        }

        private int GetFoodIndex(LocalToWorld boidLocation)
        {
            for (int i = 0; i < FoodSourceInformation.Length; i++)
            {
                var foodSource = FoodSourceInformation[i];
                if (foodSource.EnergyLevel <= 5)
                {
                    continue;
                }
                
                var foodSourceLocation = FoodSourceLocations[i];
                var distance = foodSource.EffectiveDistance(boidLocation.Position, foodSourceLocation.Position);
                if (distance < foodSource.FeedingRadius)
                {
                    return i;
                }
            }
            return -1;
        }

        private decimal getNextWeight(decimal weight, decimal dt, int foodIndex)
        {
            var W = weight;
            var Pred_E_i = (decimal)getPredatorDensity(weight, RainbowTroutEnergyConfig.Alpha1,
                RainbowTroutEnergyConfig.Beta1);

            const int secondsInDay = 24 * 60 * 60;
            var mean_prey_ED = 3000f; // We fix this to one, our prey is extremely dense.
            var Ration_prey = 0f;

            var consumed = consumption(GetTemperature, (float)W, 1) * mean_prey_ED;
            
            if (foodIndex != -1)
            {
                // In range of a food source, consume from that instead - Magic scalar computed in previous experiments
                Ration_prey = (float)(EnergyConfig.FeedingRate * secondsInDay);
                var fc = FoodSourceInformation[foodIndex];
                fc.EnergyLevel -= (decimal)Ration_prey / secondsInDay * dt;
                FoodSourceInformation[foodIndex] = fc;
            }
            
            // Ration prey would be the amount of food eaten per day (or in our case grams per timestep)
            var consumptionMax = Ration_prey / (float)W * mean_prey_ED; 
            
            var pvalue = consumptionMax / consumed;

            var Eg = egestion(consumptionMax, GetTemperature, pvalue);
            var SpecDA = SpDynAct(consumptionMax, Eg);
            var Ex = excretion(consumptionMax, Eg, GetTemperature, pvalue);
            var Res = respiration(GetTemperature, weight) * RainbowTroutEnergyConfig.Oxycal;

            var G = consumptionMax - (Res + Eg + Ex + SpecDA);

            var spawn = 0;
            var egain = G * (float)W;
            var SpawnE = spawn * W * Pred_E_i;
            var finalwt = (decimal)CalculateAllomatricMass(egain, (float)SpawnE, (float)W); // Full day
            var gain = finalwt - W;
            return W + gain / secondsInDay * dt;
        }

        private float CalculateAllomatricMass(float egain, float spawnE, float W)
        {
            float finalwt;
            if (W < RainbowTroutEnergyConfig.Cutoff)
            {
                if (RainbowTroutEnergyConfig.Beta1 == 0)
                {
                    finalwt = (egain - spawnE + W * RainbowTroutEnergyConfig.Alpha1) / RainbowTroutEnergyConfig.Alpha1;
                }
                else
                {
                    var flagval = (RainbowTroutEnergyConfig.Alpha1 * RainbowTroutEnergyConfig.Alpha1 + 4 *
                        RainbowTroutEnergyConfig.Beta1 *
                        (W * (RainbowTroutEnergyConfig.Alpha1 + RainbowTroutEnergyConfig.Beta1 * W) + egain - spawnE));
                    if (flagval < 0)
                    {
                        Debug.LogError("Fish lost too much weight");
                    }

                    finalwt = (-RainbowTroutEnergyConfig.Alpha1 + math.sqrt(flagval)) /
                              (2 * RainbowTroutEnergyConfig.Beta1);
                }

                if (finalwt > RainbowTroutEnergyConfig.Cutoff)
                {
                    // Check if pushed over cutoff
                    var a1b1cut = RainbowTroutEnergyConfig.Alpha1 +
                                  RainbowTroutEnergyConfig.Beta1 * RainbowTroutEnergyConfig.Cutoff;
                    var egainco = RainbowTroutEnergyConfig.Cutoff * a1b1cut - W * a1b1cut;

                    if (RainbowTroutEnergyConfig.Beta2 == 0)
                    {
                        return (egain - spawnE - egainco + RainbowTroutEnergyConfig.Cutoff * a1b1cut) /
                               RainbowTroutEnergyConfig.Alpha2;
                    }

                    var flagval2 = RainbowTroutEnergyConfig.Alpha2 * RainbowTroutEnergyConfig.Alpha2 + 4 *
                        RainbowTroutEnergyConfig.Beta2 *
                        (egain - spawnE - egainco + RainbowTroutEnergyConfig.Cutoff * (RainbowTroutEnergyConfig.Alpha1 +
                            RainbowTroutEnergyConfig.Beta1 * RainbowTroutEnergyConfig.Cutoff));
                    if (flagval2 < 0)
                    {
                        Debug.LogError("Fish lost too much weight");
                    }

                    return (-RainbowTroutEnergyConfig.Alpha2 + math.sqrt(flagval2)) /
                           (2 * RainbowTroutEnergyConfig.Beta2);
                }

                return finalwt;
            }

            if (RainbowTroutEnergyConfig.Beta2 == 0)
            {
                return (egain - spawnE + W * RainbowTroutEnergyConfig.Alpha2) / RainbowTroutEnergyConfig.Alpha2;
            }

            var flagval3 = RainbowTroutEnergyConfig.Alpha2 * RainbowTroutEnergyConfig.Alpha2 + 4 *
                RainbowTroutEnergyConfig.Beta2 *
                (W * (RainbowTroutEnergyConfig.Alpha2 + RainbowTroutEnergyConfig.Beta2 * W) + egain - spawnE);

            if (flagval3 < 0)
            {
                Debug.LogError("Fish lost too much weight");
            }

            finalwt = (-RainbowTroutEnergyConfig.Alpha2 + math.sqrt(flagval3)) / (2 * RainbowTroutEnergyConfig.Beta2);
            if (finalwt >= RainbowTroutEnergyConfig.Cutoff)
            {
                return finalwt;
            }

            // Recalculate if below cutoff
            var elossCo = W * (RainbowTroutEnergyConfig.Alpha2 + RainbowTroutEnergyConfig.Beta2 * W) -
                          RainbowTroutEnergyConfig.Cutoff * (RainbowTroutEnergyConfig.Alpha1 +
                                                             RainbowTroutEnergyConfig.Beta1 *
                                                             RainbowTroutEnergyConfig.Cutoff);
            if (RainbowTroutEnergyConfig.Beta1 == 0)
            {
                return (egain - spawnE + elossCo + RainbowTroutEnergyConfig.Cutoff * RainbowTroutEnergyConfig.Alpha1) /
                       RainbowTroutEnergyConfig.Alpha1;
            }

            var flagval4 = RainbowTroutEnergyConfig.Alpha1 * RainbowTroutEnergyConfig.Alpha1 + 4 *
                RainbowTroutEnergyConfig.Beta1 *
                (egain - spawnE + elossCo + RainbowTroutEnergyConfig.Cutoff * (RainbowTroutEnergyConfig.Alpha1 +
                                                                               RainbowTroutEnergyConfig.Beta1 *
                                                                               RainbowTroutEnergyConfig.Cutoff));
            if (flagval4 < 0)
            {
                Debug.LogError("Fish lost too much weight");
            }

            return (-RainbowTroutEnergyConfig.Alpha1 + math.sqrt(flagval4)) / (2 * RainbowTroutEnergyConfig.Beta1);
        }

        private float Cf3T(float t)
        {
            // Temperature function equation 3 (Hanson et al. 1997; equation from Thornton and Lessem 1978)
            var L1 = math.exp(RainbowTroutEnergyConfig.CG1 * (t - RainbowTroutEnergyConfig.CQ));
            var KA = RainbowTroutEnergyConfig.CK1 * L1 / (1 + RainbowTroutEnergyConfig.CK1 * (L1 - 1));
            var L2 = math.exp(RainbowTroutEnergyConfig.CG2 * (RainbowTroutEnergyConfig.CTL - t));
            var KB = RainbowTroutEnergyConfig.CK4 * L2 / (1 + RainbowTroutEnergyConfig.CK4 * (L2 - 1));
            return KA * KB;
        }

        private float consumption(float temperature, float weight, float pvalue)
        {
            var cMax = RainbowTroutEnergyConfig.CA * math.pow(weight, RainbowTroutEnergyConfig.CB);
            var ft = Cf3T(temperature);
            return cMax * pvalue * ft;
        }

        private float getPredatorDensity(decimal weight, float alpha1, float beta1)
        {
            return alpha1 * math.pow((float)weight, beta1);
        }

        private float SpDynAct(float consumption, float egestion)
        {
            return RainbowTroutEnergyConfig.SDA * (consumption - egestion);
        }

        private float excretion(float consumption, float egestion, float temperature, float pvalue)
        {
            return RainbowTroutEnergyConfig.UA * math.pow(temperature, RainbowTroutEnergyConfig.UB) *
                   math.exp(RainbowTroutEnergyConfig.UG * pvalue) * (consumption - egestion);
        }

        private float egestion(float consumption, float temperature, float pvalue)
        {
            // Egestion model from Stewart et al. (1983)
            var PE = RainbowTroutEnergyConfig.FA * math.pow(temperature, RainbowTroutEnergyConfig.FB) *
                     math.exp(RainbowTroutEnergyConfig.FG * pvalue);
            // var PFF = sum(globalout_Ind_Prey[i,] * globalout_Prey[i,]); // allows specification of indigestible prey, as proportions
            var PFF = 0f;
            var PF = (PE - 0.1f) / 0.9f * (1 - PFF) + PFF;
            var Eg = PF * consumption;
            return Eg;
        }

        private float respiration(float temperature, decimal weight)
        {
            var RY = math.log(RainbowTroutEnergyConfig.RQ) *
                     (RainbowTroutEnergyConfig.RTM - RainbowTroutEnergyConfig.RTO + 2);
            var RZ = math.log(RainbowTroutEnergyConfig.RQ) *
                     (RainbowTroutEnergyConfig.RTM - RainbowTroutEnergyConfig.RTO);
            var RX = math.pow(RZ, 2) * math.pow(1 + math.sqrt(1 + 40 / RY), 2) / 400;

            var Rmax = RainbowTroutEnergyConfig.RA * math.pow((float)weight, RainbowTroutEnergyConfig.RB);
            var ft = Rf2T(temperature, RX);
            var R = Rmax * ft * RainbowTroutEnergyConfig.ACT;
            return R;
        }

        private float Rf2T(float t, float RX)
        {
            if (t >= RainbowTroutEnergyConfig.RTM)
            {
                return 0.000001f;
            }

            var V = (RainbowTroutEnergyConfig.RTM - t) /
                    (RainbowTroutEnergyConfig.RTM - RainbowTroutEnergyConfig.RTO);
            var ft = math.pow(V, RX) * math.exp(RX * (1 - V));

            return ft < 0 ? 0.000001f : ft;
        }

        private float GetTemperature => 5f;
    }
}