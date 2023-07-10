using Unity.Entities;
using UnityEngine;

namespace Simulator.Configuration.Components
{
    [System.Serializable]
    public struct RainbowTroutEnergyConfigurationComponent : IComponentData
    {
        public float RTM; // lethal temp
        public float RTO; // optimal temp
        public float RA; // 1g fish at 0c no swimming
        public float RQ;
        public float RB; // Slope of mass function
        public float ACT; // Activity multiplier
        public float SDA; // Specific dynamic action

        public float Alpha1; // intercept for the allometric mass function for first size range
        public float Beta1; // slope of the allometric mass function for first size range
        public float Oxycal;

        public float UA; // Excretion
        public float UB; // coefficient of water temperature dependence of excretion
        public float UG; // coefficient for feeding level dependence (P-value) of excretion

        public float CQ; //water temperature dependent coefficient of consumption
        public float CA; // intercept for the allometric mass function
        public float CB; // slope for the allometric mass function
        public float CK1; // small fraction of the maximum rate
        public float CK4; // reduced fraction of the maximum rate
        public float CTL; // temperature at which dependence is some reduced fraction (CK4) of the max. rate

        public float CTO; // laboratory temperature preferendum
        public float CTM; // maximum water temperature above which consumption ceases
        public float CG1 => (1 / (CTO - CQ)) * Mathf.Log((0.98f * (1 - CK1)) / (CK1 * 0.02f));
        public float CG2 => (1 / (CTL - CTM)) * Mathf.Log((0.98f * (1 - CK4)) / (CK4 * 0.02f));

        public float FA; // Egestion
        public float FB; // coefficient of water temperature dependence of egestion
        public float FG; // coefficient for feeding level dependence (P-value) of egestion

        public void Print()
        {
            Debug.Log($"RTM: {RTM}, RTO: {RTO}, RA: {RA}, RQ: {RQ}, RB: {RB}, ACT: {ACT}, SDA: {SDA}, Alpha1: {Alpha1}, Beta1: {Beta1}, Oxycal: {Oxycal}, UA: {UA}, UB: {UB}, UG: {UG}, CQ: {CQ}, CA: {CA}, CB: {CB}, CK1: {CK1}, CK4: {CK4}, CTL: {CTL}, CTO: {CTO}, CTM: {CTM}, CG1: {CG1}, CG2: {CG2}, FA: {FA}, FB: {FB}, FG: {FG}");
        }
    }
}