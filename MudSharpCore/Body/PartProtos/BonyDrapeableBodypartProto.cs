using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.Health;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Body.PartProtos
{
	public class BonyDrapeableBodypartProto : DrapeableBodypartProto, IBone
	{
		public BonyDrapeableBodypartProto(BodypartProto proto, IFuturemud game) : base(proto, game)
		{
			DisplayOrder = proto.DisplayOrder ?? 0;
		}

		public BonyDrapeableBodypartProto(BonyDrapeableBodypartProto rhs, string newName) : base(rhs, newName)
		{
			DisplayOrder = rhs.DisplayOrder;
		}

		protected override void InternalSave(BodypartProto dbitem)
		{
			dbitem.DisplayOrder = DisplayOrder;
		}


		public override BodypartTypeEnum BodypartType => BodypartTypeEnum.BonyDrapeable;

		public override IBodypart Clone(string newName)
		{
			return new BonyDrapeableBodypartProto(this, newName);
		}

		public bool CriticalBone => true;
		public virtual bool CanBeImmobilised => true;
		public double BoneHealingModifier => 1.0;
		public double BoneEffectiveHealthModifier => Gameworld.GetStaticDouble("BonyPartEffectiveHitpointForBonebreakModifier");
		public IEnumerable<(IOrganProto Organ, BodypartInternalInfo Info)> CoveredOrgans => Enumerable.Empty<(IOrganProto Organ, BodypartInternalInfo Info)>();
		public (double OrdinaryDamage, double BoneDamage) ShouldBeBoneBreak(IDamage damage)
		{
			var boneDamage = Math.Max(0.0, (damage.DamageAmount -
			                                Gameworld.GetStaticDouble($"BonyPartBoneBreakLeeway{damage.DamageType.DescribeEnum()}")) *
			                               Gameworld.GetStaticDouble($"BonyPartBoneBreakDamage{damage.DamageType.DescribeEnum()}"));
			return (Gameworld.GetStaticDouble($"BonyPartBoneBreakDamage{damage.DamageType.DescribeEnum()}") * damage.DamageAmount, boneDamage);
		}
	}
}
