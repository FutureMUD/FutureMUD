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
		public bool CanBeImmobilised { get; protected set; }
		public double BoneHealingModifier { get; protected set; }
		public IEnumerable<(IOrganProto Organ, BodypartInternalInfo Info)> CoveredOrgans => Enumerable.Empty<(IOrganProto Organ, BodypartInternalInfo Info)>();
		public bool ShouldBeBoneBreak(IDamage damage)
		{
			// TODO - how do we decide regular damage vs bone breaking damage?
			switch (damage.DamageType)
			{
				case DamageType.Slashing:
				case DamageType.Chopping:
				case DamageType.Crushing:
				case DamageType.Piercing:
				case DamageType.Ballistic:
				case DamageType.Shockwave:
				case DamageType.Bite:
				case DamageType.Claw:
				case DamageType.Shearing:
				case DamageType.ArmourPiercing:
				case DamageType.Wrenching:
				case DamageType.Shrapnel:
				case DamageType.Falling:
				case DamageType.Eldritch:
				case DamageType.Arcane:
					return true;
			}

			return false;
		}
	}
}
