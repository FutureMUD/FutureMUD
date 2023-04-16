using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Framework;
using MudSharp.Health;
using MudSharp.Models;

namespace MudSharp.Body.PartProtos
{
	public class BonyGrabbingWieldingBodypartProto : GrabbingWieldingBodypartProto, IBone
	{
		/// <inheritdoc />
		public BonyGrabbingWieldingBodypartProto(BodypartProto proto, IFuturemud game) : base(proto, game)
		{
		}

		/// <inheritdoc />
		protected BonyGrabbingWieldingBodypartProto(BonyGrabbingWieldingBodypartProto rhs, string newName) : base(rhs, newName)
		{
		}

		public override IBodypart Clone(string newName)
		{
			return new BonyGrabbingWieldingBodypartProto(this, newName);
		}

		#region Overrides of GrabbingWieldingBodypartProto

		/// <inheritdoc />
		public override BodypartTypeEnum BodypartType => BodypartTypeEnum.BonyGrabbingWielding;

		#endregion

		#region Implementation of IBone

		/// <inheritdoc />
		public bool CriticalBone => true;

		/// <inheritdoc />
		public bool CanBeImmobilised => true;

		/// <inheritdoc />
		public double BoneHealingModifier => 1.0;

		public double BoneEffectiveHealthModifier => Gameworld.GetStaticDouble("BonyPartEffectiveHitpointForBonebreakModifier");

		/// <inheritdoc />
		public IEnumerable<(IOrganProto Organ, BodypartInternalInfo Info)> CoveredOrgans => Enumerable.Empty<(IOrganProto Organ, BodypartInternalInfo Info)>();

		/// <inheritdoc />
		public (double OrdinaryDamage, double BoneDamage) ShouldBeBoneBreak(IDamage damage)
		{
			var boneDamage = Math.Max(0.0, (damage.DamageAmount -
			                 Gameworld.GetStaticDouble($"BonyPartBoneBreakLeeway{damage.DamageType.DescribeEnum()}")) *
			                 Gameworld.GetStaticDouble($"BonyPartBoneBreakDamage{damage.DamageType.DescribeEnum()}"));
			return (Gameworld.GetStaticDouble($"BonyPartBoneBreakDamage{damage.DamageType.DescribeEnum()}") * damage.DamageAmount, boneDamage);
		}

		#endregion
	}
}
