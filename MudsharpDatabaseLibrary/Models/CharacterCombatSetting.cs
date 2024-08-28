using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
	public partial class CharacterCombatSetting
	{
		public long Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public bool GlobalTemplate { get; set; }
		public long? AvailabilityProgId { get; set; }
		public long? CharacterOwnerId { get; set; }
		public double WeaponUsePercentage { get; set; }
		public double MagicUsePercentage { get; set; }
		public double PsychicUsePercentage { get; set; }
		public double NaturalWeaponPercentage { get; set; }
		public double AuxiliaryPercentage { get; set; }
		public bool PreferToFightArmed { get; set; }
		public bool PreferFavouriteWeapon { get; set; }
		public bool PreferShieldUse { get; set; }
		public string ClassificationsAllowed { get; set; }
		public long RequiredIntentions { get; set; }
		public long ForbiddenIntentions { get; set; }
		public long PreferredIntentions { get; set; }
		public bool AttackHelpless { get; set; }
		public bool AttackUnarmed { get; set; }
		public bool FallbackToUnarmedIfNoWeapon { get; set; }
		public bool AttackCriticallyInjured { get; set; }
		public bool SkirmishToOtherLocations { get; set; }
		public int PursuitMode { get; set; }
		public int DefaultPreferredDefenseType { get; set; }
		public int PreferredMeleeMode { get; set; }
		public int PreferredRangedMode { get; set; }
		public bool AutomaticallyMoveTowardsTarget { get; set; }
		public bool PreferNonContactClinchBreaking { get; set; }
		public int InventoryManagement { get; set; }
		public int MovementManagement { get; set; }
		public int RangedManagement { get; set; }
		public bool ManualPositionManagement { get; set; }
		public double MinimumStaminaToAttack { get; set; }
		public bool MoveToMeleeIfCannotEngageInRangedCombat { get; set; }
		public int PreferredWeaponSetup { get; set; }
		public double RequiredMinimumAim { get; set; }
		public string MeleeAttackOrderPreference { get; set; }
		public int GrappleResponse { get; set; }

		public virtual FutureProg AvailabilityProg { get; set; }
		public virtual Character CharacterOwner { get; set; }
	}
}
