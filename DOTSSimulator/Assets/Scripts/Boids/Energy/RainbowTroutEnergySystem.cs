using Simulator.Configuration;
using Simulator.Configuration.Components;
using Simulator.Framework;
using Simulator.Utils;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Simulator.Boids.Energy
{
    [UpdateInGroup(typeof(FrameworkFixedSystemGroup))]
    public partial class RainbowTroutEnergySystem : SystemBase
    {
        private GlobalConfigurationComponent _configurationComponent;
        private RainbowTroutEnergyConfigurationComponent _config;

        private EntityQuery _noEnergyQuery;

        protected override void OnCreate()
        {
            base.OnCreate();
            _noEnergyQuery = GetEntityQuery(typeof(NoEnergyComponent));
            RequireForUpdate<GlobalConfigurationComponent>();
        }

        protected override void OnStartRunning()
        {
            _configurationComponent = SystemAPI.GetSingleton<GlobalConfigurationComponent>();
            _config = _configurationComponent.RainbowTroutEnergyConfiguration;

            base.OnStartRunning();
        }

        protected override void OnUpdate()
        {
            if (_configurationComponent.EnergyConfiguration.EnergyEquation != EnergyEquationType.RainbowTrout)
            {
                return;
            }
            
            var dt = (decimal)_configurationComponent.SimulationFrameworkConfiguration.UpdateInterval;
            var cr = (decimal)_configurationComponent.EnergyConfiguration.ConsumptionRate;
            // Update energy level
            
            Entities.WithAll<BoidComponent, EnergyComponent>().WithoutBurst()
                .ForEach((ref EnergyComponent energy) => energy.Weight = getNextWeight(energy.Weight, dt))
                .Run();
            
            var ecb = new EntityCommandBuffer(Allocator.TempJob);
            // // Delete enties with energy level below 0
            Entities.WithAll<BoidComponent, EnergyComponent>().WithNone<NoEnergyComponent>().WithoutBurst().ForEach((Entity e, in EnergyComponent energy) =>
                {
                    if (energy.Weight < 0)
                    {
                        ecb.AddComponent<NoEnergyComponent>(e);
                    }
                }).Run();
            
            ecb.DestroyEntity(_noEnergyQuery);
            ecb.Playback(EntityManager);
            ecb.Dispose();

        }
        
        private decimal getNextWeight(decimal weight, decimal dt)
        {
            var W = weight;
            var Pred_E_i = (decimal)getPredatorDensity(weight, _config.Alpha1, _config.Beta1);

            var mean_prey_ED = 1f; // We fix this to one, our prey is extremely dense.
            var Ration_prey = 100f;
            var Cons = (Ration_prey / (float)W) * mean_prey_ED; // Ration prey would be the amount of food eaten per day (or in our case grams per timestep)
            var Cons_p1 = consumption(GetTemperature, (float)W, 1) * mean_prey_ED;
            var pvalue = Cons / Cons_p1;

            var Eg = egestion(Cons, GetTemperature, pvalue);
            var SpecDA = SpDynAct(Cons, Eg);
            var Ex = excretion(Cons, Eg, GetTemperature, pvalue);
            var Res = respiration(GetTemperature, weight) * _config.Oxycal;

            var G = Cons - (Res + Eg + Ex + SpecDA);
                
            var spawn = 0;
            var egain = (decimal)G * W;
            var SpawnE = spawn * W * Pred_E_i;
            var finalwt = Mathm.Power((egain - SpawnE + Pred_E_i * W) / (decimal)_config.Alpha1, 1 / (decimal)(_config.Beta1 + 1));
            // Full day
            var gain = finalwt - W;
            return W + gain / 24 / 60 / 60 * dt;
        }
        
        private float consumption(float temperature, float weight, float pvalue)
        {
            float Cf3T(float t)
            {
                // Temperature function equation 3 (Hanson et al. 1997; equation from Thornton and Lessem 1978)
                var L1 = Mathf.Exp(_config.CG1 * (t - _config.CQ));
                var KA = (_config.CK1 * L1) / (1 + _config.CK1 * (L1 - 1));
                var L2 = Mathf.Exp(_config.CG2 * (_config.CTL - t));
                var KB = (_config.CK4 * L2) / (1 + _config.CK4 * (L2 - 1));
                var ft = KA * KB;
                return ft;
            }
            
            var Cmax = _config.CA * Mathf.Pow(weight, _config.CB);
            var ft = Cf3T(temperature);
            return Cmax * pvalue * ft;
        }

        private float getPredatorDensity(decimal weight, float alpha1, float beta1)
        {
           return alpha1 * Mathf.Pow((float)weight, beta1);
        }

        private float SpDynAct(float consumption, float egestion)
        {
            return _config.SDA * (consumption - egestion);
        }
        
        private float excretion(float consumption, float egestion, float temperature, float pvalue)
        {
            return _config.UA * Mathf.Pow(temperature, _config.UB) * Mathf.Exp(_config.UG * pvalue) * (consumption - egestion);
        }
        private float egestion(float consumption, float temperature, float pvalue)
        {
            // Egestion model from Stewart et al. (1983)
            var PE = _config.FA * Mathf.Pow(temperature , _config.FB) * Mathf.Exp(_config.FG * pvalue);
            // var PFF = sum(globalout_Ind_Prey[i,] * globalout_Prey[i,]); // allows specification of indigestible prey, as proportions
            var PFF = 0f;
            var PF = ((PE - 0.1f) / 0.9f) * (1 - PFF) + PFF;
            var Eg = PF * consumption;
            return Eg;
        }
        
        private float respiration(float temperature, decimal weight)
        {
            var RY = Mathf.Log(_config.RQ) * (_config.RTM - _config.RTO + 2);
            var RZ = Mathf.Log(_config.RQ) * (_config.RTM - _config.RTO);
            var RX = Mathf.Pow(RZ, 2) * Mathf.Pow(1 + Mathf.Pow(1 + 40 / RY, 0.5f), 2) / 400;
            
            float Rf2T(float t)
            {
                if (t >= _config.RTM)
                {
                    return 0.000001f;
                }
                
                var V = (_config.RTM - t) / (_config.RTM - _config.RTO);
                var ft = Mathf.Pow(V, RX * Mathf.Exp(RX * (1 - V)));

                return ft < 0 ? 0.000001f : ft;
            }

            var Rmax = _config.RA * Mathf.Pow((float)weight, _config.RB);
            var ft = Rf2T(temperature);
            var R = Rmax * ft * _config.ACT;
            return R;
        }

        private float GetTemperature => 22.5f;
    }
}