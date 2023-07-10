using Simulator.Configuration.Components;
using Unity.Entities;
using UnityEngine;

namespace Simulator.Configuration.Authoring
{
    public class RainbowTroutEnergyConfigurationComponentAuthoring : MonoBehaviour
    {
        public float Rtm;
        public float Rto;
        public float Ra;
        public float Rq;
        public float Rb;
        public float Act;
        public float Sda;
        public float Alpha1;
        public float Beta1;
        public float Oxycal;
        public float Ua;
        public float Ub;
        public float Ug;
        public float Cq;
        public float Ca;
        public float Cb;
        public float Ck1;
        public float Ck4;
        public float Ctl;
        public float Cto;
        public float Ctm;
        public float Fa;
        public float Fb;
        public float Fg;
    }
    
    public class RainbowTroutEnergyConfigurationComponentBaker : Baker<RainbowTroutEnergyConfigurationComponentAuthoring>
    {
        public override void Bake(RainbowTroutEnergyConfigurationComponentAuthoring authoring)
        {
            var entity = GetEntity(authoring, TransformUsageFlags.None);
            AddComponent(entity, new RainbowTroutEnergyConfigurationComponent
            {
                RTM = authoring.Rtm,
                RTO = authoring.Rto,
                RA = authoring.Ra,
                RQ = authoring.Rq,
                RB = authoring.Rb,
                ACT = authoring.Act,
                SDA = authoring.Sda,
                Alpha1 = authoring.Alpha1,
                Beta1 = authoring.Beta1,
                Oxycal = authoring.Oxycal,
                UA = authoring.Ua,
                UB = authoring.Ub,
                UG = authoring.Ug,
                CQ = authoring.Cq,
                CA = authoring.Ca,
                CB = authoring.Cb,
                CK1 = authoring.Ck1,
                CK4 = authoring.Ck4,
                CTL = authoring.Ctl,
                CTO = authoring.Cto,
                CTM = authoring.Ctm,
                FA = authoring.Fa,
                FB = authoring.Fb,
                FG = authoring.Fg
            });
        }
    }
}