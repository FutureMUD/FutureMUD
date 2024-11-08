using System.Collections.Generic;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.Magic;

namespace MudSharp.Combat
{
	
	public enum AutomaticInventorySettings {
		FullyAutomatic,
		FullyManual,
		RetrieveOnly,
		AutomaticButDontDiscard
	}

	public enum AutomaticMovementSettings {
		FullyAutomatic,
		FullyManual,
		SeekCoverOnly,
		KeepRange
	}

	public enum AutomaticRangedSettings {
		FullyAutomatic,
		FullyManual,
		ContinueFiringOnly
	}

	public enum GrappleResponse
	{
		Avoidance,
		Counter,
		Throw,
		Ignore
	}

	public enum MeleeAttackOrderPreference {
		Weapon,
		Implant,
		Prosthetic,
		Magic,
		Psychic
	}

	public interface ICharacterCombatSettings : ISaveable, IFrameworkItem
	{
		long? CharacterOwnerId { get; set; }

		/// <summary>
		///     The availability prog, if set, restricts access to a global character combat settings
		/// </summary>
		IFutureProg AvailabilityProg { get; set; }

		/// <summary>
		///     If true, this Character Combat Settings is a global setting and cannot be modified by a player
		/// </summary>
		bool GlobalTemplate { get; set; }

		string Description { get; set; }
		AttackHandednessOptions PreferredWeaponSetup { get; set; }
		double MinimumStaminaToAttack { get; set; }
		GrappleResponse GrappleResponse { get; set; }

		/// <summary>
		/// Controls the degree of automation in the inventory management in combat
		/// </summary>
		AutomaticInventorySettings InventoryManagement { get; set; }

		/// <summary>
		/// Controls the degree of automation in the movement management in combat
		/// </summary>
		AutomaticMovementSettings MovementManagement { get; set; }

		AutomaticRangedSettings RangedManagement { get; set; }
		bool ManualPositionManagement { get; set; }

		/// <summary>
		///     Percentage chance to choose a weapon move over other types of moves
		/// </summary>
		double WeaponUsePercentage { get; set; }

		/// <summary>
		///     Percentage chance to choose a magic attack over other types of moves
		/// </summary>
		double MagicUsePercentage { get; set; }

		/// <summary>
		///     Percentage chance to choose a psychic attack over other types of moves
		/// </summary>
		double PsychicUsePercentage { get; set; }

		/// <summary>
		///     Percentage chance to choose a natural weapon attack over other types of moves
		/// </summary>
		double NaturalWeaponPercentage { get; set; }

		/// <summary>
		///     Percentage chance to choose an auxillary over other types of moves
		/// </summary>
		double AuxiliaryPercentage { get; set; }

		/// <summary>
		///     If true, this combatant prefers to fight armed (and so will draw/retrieve weapons)
		/// </summary>
		bool PreferToFightArmed { get; set; }

		/// <summary>
		///     If true, this combatant will prefer to retrieve a lost weapon over drawing a new one
		/// </summary>
		bool PreferFavouriteWeapon { get; set; }

		/// <summary>
		///     If true, this combatant will prefer to use a shield if they have one available
		/// </summary>
		bool PreferShieldUse { get; set; }

		/// <summary>
		///     If true, this combatant will attack opponents who are unarmed or helpless
		/// </summary>
		bool AttackHelpless { get; set; }

		bool AttackDisarmed { get; set; }

		/// <summary>
		///     If true, this combatant will use unarmed attacks if they would otherwise have used an armed attack but don't have a
		///     weapon
		/// </summary>
		bool FallbackToUnarmedIfNoWeapon { get; set; }

		double RequiredMinimumAim { get; set; }

		/// <summary>
		///     If true, this combatant will attack opponents who have been critically injured
		/// </summary>
		bool AttackCriticallyInjured { get; set; }

		bool SkirmishToOtherLocations { get; set; }
		bool AutomaticallyMoveTowardsTarget { get; set; }
		bool PreferNonContactClinchBreaking { get; set; }
		bool MoveToMeleeIfCannotEngageInRangedCombat { get; set; }
		PursuitMode PursuitMode { get; set; }
		DefenseType DefaultPreferredDefenseType { get; set; }
		CombatStrategyMode PreferredMeleeMode { get; set; }
		CombatStrategyMode PreferredRangedMode { get; set; }
		IEnumerable<WeaponClassification> ClassificationsAllowed { get; }
		CombatMoveIntentions RequiredIntentions { get; set; }
		CombatMoveIntentions ForbiddenIntentions { get; set; }
		CombatMoveIntentions PreferredIntentions { get; set; }
		List<IMagicSchool> ForbiddenSchools { get; }
		List<MeleeAttackOrderPreference> MeleeAttackOrderPreferences { get; }
		bool AddClassification(WeaponClassification classification);
		bool RemoveClassification(WeaponClassification classification);
		bool ClearClassifications();
		bool CanUse(ICharacter who);
		string Show(ICharacter voyeur);
		void SetName(string name);
	}
}