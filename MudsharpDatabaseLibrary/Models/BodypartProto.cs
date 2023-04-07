using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class BodypartProto
    {
        public BodypartProto()
        {
            BodiesSeveredParts = new HashSet<BodiesSeveredParts>();
            BodyProtos = new HashSet<BodyProto>();
            BodyProtosAdditionalBodyparts = new HashSet<BodyProtosAdditionalBodyparts>();
            BodypartGroupDescribersBodypartProtos = new HashSet<BodypartGroupDescribersBodypartProtos>();
            BodypartInternalInfosBodypartProto = new HashSet<BodypartInternalInfos>();
            BodypartInternalInfosInternalPart = new HashSet<BodypartInternalInfos>();
            BodypartProtoAlignmentHits = new HashSet<BodypartProtoAlignmentHits>();
            BodypartProtoBodypartProtoUpstreamChildNavigation = new HashSet<BodypartProtoBodypartProtoUpstream>();
            BodypartProtoBodypartProtoUpstreamParentNavigation = new HashSet<BodypartProtoBodypartProtoUpstream>();
            BodypartProtoOrientationHits = new HashSet<BodypartProtoOrientationHits>();
            BoneOrganCoveragesBone = new HashSet<BoneOrganCoverage>();
            BoneOrganCoveragesOrgan = new HashSet<BoneOrganCoverage>();
            ButcheryProductsBodypartProtos = new HashSet<ButcheryProductsBodypartProtos>();
            Infections = new HashSet<Infection>();
            InverseCountAs = new HashSet<BodypartProto>();
            Limbs = new HashSet<Limb>();
            LimbsBodypartProto = new HashSet<LimbBodypartProto>();
            LimbsSpinalParts = new HashSet<LimbsSpinalPart>();
            RacesAdditionalBodyparts = new HashSet<RacesAdditionalBodyparts>();
            RacesWeaponAttacks = new HashSet<RacesWeaponAttacks>();
        }

        public long Id { get; set; }
        public int BodypartType { get; set; }
        public long BodyId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long? CountAsId { get; set; }
        public long BodypartShapeId { get; set; }
        public int? DisplayOrder { get; set; }
        public int MaxLife { get; set; }
        public int SeveredThreshold { get; set; }
        public double PainModifier { get; set; }
        public double BleedModifier { get; set; }
        public int RelativeHitChance { get; set; }
        public int Location { get; set; }
        public int Alignment { get; set; }
        public bool? Unary { get; set; }
        public int? MaxSingleSize { get; set; }
        public int IsOrgan { get; set; }
        public double WeightLimit { get; set; }
        public bool IsCore { get; set; }
        public double StunModifier { get; set; }
        public double DamageModifier { get; set; }
        public long DefaultMaterialId { get; set; }
        public bool Significant { get; set; }
        public double RelativeInfectability { get; set; }
        public double HypoxiaDamagePerTick { get; set; }
        public bool IsVital { get; set; }
        public long? ArmourTypeId { get; set; }
        public double ImplantSpace { get; set; }
        public double ImplantSpaceOccupied { get; set; }
        public int Size { get; set; }

        public virtual ArmourType ArmourType { get; set; }
        public virtual BodyProto Body { get; set; }
        public virtual BodypartShape BodypartShape { get; set; }
        public virtual BodypartProto CountAs { get; set; }
        public virtual Material DefaultMaterial { get; set; }
        public virtual ICollection<BodiesSeveredParts> BodiesSeveredParts { get; set; }
        public virtual ICollection<BodyProto> BodyProtos { get; set; }
        public virtual ICollection<BodyProtosAdditionalBodyparts> BodyProtosAdditionalBodyparts { get; set; }
        public virtual ICollection<BodypartGroupDescribersBodypartProtos> BodypartGroupDescribersBodypartProtos { get; set; }
        public virtual ICollection<BodypartInternalInfos> BodypartInternalInfosBodypartProto { get; set; }
        public virtual ICollection<BodypartInternalInfos> BodypartInternalInfosInternalPart { get; set; }
        public virtual ICollection<BodypartProtoAlignmentHits> BodypartProtoAlignmentHits { get; set; }
        public virtual ICollection<BodypartProtoBodypartProtoUpstream> BodypartProtoBodypartProtoUpstreamChildNavigation { get; set; }
        public virtual ICollection<BodypartProtoBodypartProtoUpstream> BodypartProtoBodypartProtoUpstreamParentNavigation { get; set; }
        public virtual ICollection<BodypartProtoOrientationHits> BodypartProtoOrientationHits { get; set; }
        public virtual ICollection<BoneOrganCoverage> BoneOrganCoveragesBone { get; set; }
        public virtual ICollection<BoneOrganCoverage> BoneOrganCoveragesOrgan { get; set; }
        public virtual ICollection<ButcheryProductsBodypartProtos> ButcheryProductsBodypartProtos { get; set; }
        public virtual ICollection<Infection> Infections { get; set; }
        public virtual ICollection<BodypartProto> InverseCountAs { get; set; }
        public virtual ICollection<Limb> Limbs { get; set; }
        public virtual ICollection<LimbBodypartProto> LimbsBodypartProto { get; set; }
        public virtual ICollection<LimbsSpinalPart> LimbsSpinalParts { get; set; }
        public virtual ICollection<RacesAdditionalBodyparts> RacesAdditionalBodyparts { get; set; }
        public virtual ICollection<RacesWeaponAttacks> RacesWeaponAttacks { get; set; }
    }
}
