using System.Collections.Generic;
using System.Linq;
using System.Text;
using MudSharp.Models;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.Magic;

namespace MudSharp.Combat;

public class CharacterCombatSettings : SaveableItem, ICharacterCombatSettings
{
	private readonly List<WeaponClassification> _classificationsAllowed = new();

	private CombatMoveIntentions _forbiddenIntentions;

	private CombatMoveIntentions _preferredIntentions;

	private CombatMoveIntentions _requiredIntentions;

	public CharacterCombatSettings(CharacterCombatSetting setting, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		Load(setting, gameworld);
	}

	public CharacterCombatSettings(ICharacter characterOwner, string name)
	{
		Gameworld = characterOwner.Gameworld;
		using (new FMDB())
		{
			var dbitem = new CharacterCombatSetting
			{
				Name = name,
				CharacterOwnerId = characterOwner?.Id,
				Description = "An undescribed manner of fighting",
				ClassificationsAllowed = "1 2 3 4 5 7",
				GlobalTemplate = false,
				WeaponUsePercentage = 1,
				PreferToFightArmed = true,
				PreferShieldUse = true,
				PreferNonContactClinchBreaking = true,
				ForbiddenIntentions =
					(long)(CombatMoveIntentions.Dirty | CombatMoveIntentions.Flashy | CombatMoveIntentions.Savage),
				InventoryManagement = (int)AutomaticInventorySettings.AutomaticButDontDiscard,
				PreferredMeleeMode = (int)CombatStrategyMode.StandardMelee,
				PreferredRangedMode = (int)CombatStrategyMode.StandardRange,
				MinimumStaminaToAttack = MinimumStaminaToAttack,
				MeleeAttackOrderPreference = "0 1 2 3 4",
				GrappleResponse = (int)GrappleResponse.Avoidance
			};
			FMDB.Context.CharacterCombatSettings.Add(dbitem);
			FMDB.Context.SaveChanges();
			Load(dbitem, characterOwner?.Gameworld);
		}
	}

	public CharacterCombatSettings(ICharacter characterOwner, CharacterCombatSettings settingToCopy, string newName)
	{
		Gameworld = characterOwner.Gameworld;
		using (new FMDB())
		{
			var dbitem = new CharacterCombatSetting
			{
				Name = newName,
				CharacterOwnerId = characterOwner?.Id,
				Description = settingToCopy.Description,
				AvailabilityProgId = settingToCopy.AvailabilityProg?.Id,
				ClassificationsAllowed = settingToCopy.ClassificationsAllowed.Select(x => ((int)x).ToString())
				                                      .ListToString(separator: " ", conjunction: ""),
				GlobalTemplate = false,
				WeaponUsePercentage = settingToCopy.WeaponUsePercentage,
				NaturalWeaponPercentage = settingToCopy.NaturalWeaponPercentage,
				MagicUsePercentage = settingToCopy.MagicUsePercentage,
				PsychicUsePercentage = settingToCopy.PsychicUsePercentage,
				AuxiliaryPercentage = settingToCopy.AuxiliaryPercentage,
				PreferToFightArmed = settingToCopy.PreferToFightArmed,
				PreferShieldUse = settingToCopy.PreferShieldUse,
				PreferFavouriteWeapon = settingToCopy.PreferFavouriteWeapon,
				AttackUnarmedOrHelpless = settingToCopy.AttackUnarmedOrHelpless,
				FallbackToUnarmedIfNoWeapon = settingToCopy.FallbackToUnarmedIfNoWeapon,
				AttackCriticallyInjured = settingToCopy.AttackCriticallyInjured,
				SkirmishToOtherLocations = settingToCopy.SkirmishToOtherLocations,
				PursuitMode = (int)settingToCopy.PursuitMode,
				DefaultPreferredDefenseType = (int)settingToCopy.DefaultPreferredDefenseType,
				PreferredMeleeMode = (int)settingToCopy.PreferredMeleeMode,
				PreferredRangedMode = (int)settingToCopy.PreferredRangedMode,
				ForbiddenIntentions = (long)settingToCopy.ForbiddenIntentions,
				PreferredIntentions = (long)settingToCopy.PreferredIntentions,
				RequiredIntentions = (long)settingToCopy.RequiredIntentions,
				AutomaticallyMoveTowardsTarget = settingToCopy.AutomaticallyMoveTowardsTarget,
				PreferNonContactClinchBreaking = settingToCopy.PreferNonContactClinchBreaking,
				InventoryManagement = (int)settingToCopy.InventoryManagement,
				MovementManagement = (int)settingToCopy.MovementManagement,
				RangedManagement = (int)settingToCopy.RangedManagement,
				ManualPositionManagement = settingToCopy.ManualPositionManagement,
				MinimumStaminaToAttack = settingToCopy.MinimumStaminaToAttack,
				MoveToMeleeIfCannotEngageInRangedCombat = settingToCopy.MoveToMeleeIfCannotEngageInRangedCombat,
				PreferredWeaponSetup = (int)settingToCopy.PreferredWeaponSetup,
				RequiredMinimumAim = settingToCopy.RequiredMinimumAim,
				MeleeAttackOrderPreference = settingToCopy.MeleeAttackOrderPreferences
				                                          .Select(x => ((int)x).ToString())
				                                          .ListToCommaSeparatedValues(" "),
				GrappleResponse = (int)settingToCopy.GrappleResponse
			};
			FMDB.Context.CharacterCombatSettings.Add(dbitem);
			FMDB.Context.SaveChanges();
			Load(dbitem, characterOwner?.Gameworld);
		}
	}

	public override string FrameworkItemType => "CharacterCombatSettings";

	public long? CharacterOwnerId { get; set; }

	/// <summary>
	///     The availability prog, if set, restricts access to a global character combat settings
	/// </summary>
	public IFutureProg AvailabilityProg { get; set; }

	/// <summary>
	///     If true, this Character Combat Settings is a global setting and cannot be modified by a player
	/// </summary>
	public bool GlobalTemplate { get; set; }

	public string Description { get; set; }

	public AttackHandednessOptions PreferredWeaponSetup { get; set; }

	public double MinimumStaminaToAttack { get; set; }

	public GrappleResponse GrappleResponse { get; set; }

	/// <summary>
	/// Controls the degree of automation in the inventory management in combat
	/// </summary>
	public AutomaticInventorySettings InventoryManagement { get; set; }

	/// <summary>
	/// Controls the degree of automation in the movement management in combat
	/// </summary>
	public AutomaticMovementSettings MovementManagement { get; set; }

	public AutomaticRangedSettings RangedManagement { get; set; }

	public bool ManualPositionManagement { get; set; }

	/// <summary>
	///     Percentage chance to choose a weapon move over other types of moves
	/// </summary>
	public double WeaponUsePercentage { get; set; }

	/// <summary>
	///     Percentage chance to choose a magic attack over other types of moves
	/// </summary>
	public double MagicUsePercentage { get; set; }

	/// <summary>
	///     Percentage chance to choose a psychic attack over other types of moves
	/// </summary>
	public double PsychicUsePercentage { get; set; }

	/// <summary>
	///     Percentage chance to choose a natural weapon attack over other types of moves
	/// </summary>
	public double NaturalWeaponPercentage { get; set; }

	/// <summary>
	///     Percentage chance to choose an auxillary over other types of moves
	/// </summary>
	public double AuxiliaryPercentage { get; set; }

	/// <summary>
	///     If true, this combatant prefers to fight armed (and so will draw/retrieve weapons)
	/// </summary>
	public bool PreferToFightArmed { get; set; }

	/// <summary>
	///     If true, this combatant will prefer to retrieve a lost weapon over drawing a new one
	/// </summary>
	public bool PreferFavouriteWeapon { get; set; }

	/// <summary>
	///     If true, this combatant will prefer to use a shield if they have one available
	/// </summary>
	public bool PreferShieldUse { get; set; }

	/// <summary>
	///     If true, this combatant will attack opponents who are unarmed or helpless
	/// </summary>
	public bool AttackUnarmedOrHelpless { get; set; }

	/// <summary>
	///     If true, this combatant will use unarmed attacks if they would otherwise have used an armed attack but don't have a
	///     weapon
	/// </summary>
	public bool FallbackToUnarmedIfNoWeapon { get; set; }

	public double RequiredMinimumAim { get; set; } = 0.3;

	/// <summary>
	///     If true, this combatant will attack opponents who have been critically injured
	/// </summary>
	public bool AttackCriticallyInjured { get; set; }

	public bool SkirmishToOtherLocations { get; set; }

	public bool AutomaticallyMoveTowardsTarget { get; set; }

	public bool PreferNonContactClinchBreaking { get; set; }

	public bool MoveToMeleeIfCannotEngageInRangedCombat { get; set; }

	public PursuitMode PursuitMode { get; set; }

	public DefenseType DefaultPreferredDefenseType { get; set; }

	public CombatStrategyMode PreferredMeleeMode { get; set; }

	public CombatStrategyMode PreferredRangedMode { get; set; }
	public IEnumerable<WeaponClassification> ClassificationsAllowed => _classificationsAllowed;

	public CombatMoveIntentions RequiredIntentions
	{
		get => _requiredIntentions;
		set
		{
			_requiredIntentions = value;
			Changed = true;
		}
	}

	public CombatMoveIntentions ForbiddenIntentions
	{
		get => _forbiddenIntentions;
		set
		{
			_forbiddenIntentions = value;
			Changed = true;
		}
	}

	public CombatMoveIntentions PreferredIntentions
	{
		get => _preferredIntentions;
		set
		{
			_preferredIntentions = value;
			Changed = true;
		}
	}

	// TODO - load from DB
	private readonly List<IMagicSchool> _forbiddenSchools = new();

	public List<IMagicSchool> ForbiddenSchools => _forbiddenSchools;

	public List<MeleeAttackOrderPreference> MeleeAttackOrderPreferences { get; } = new();

	private void Load(CharacterCombatSetting setting, IFuturemud gameworld)
	{
		_id = setting.Id;
		_name = setting.Name;
		Description = setting.Description;
		AvailabilityProg = gameworld?.FutureProgs.Get(setting.AvailabilityProgId ?? 0);
		GlobalTemplate = setting.GlobalTemplate;
		CharacterOwnerId = setting.CharacterOwnerId;
		WeaponUsePercentage = setting.WeaponUsePercentage;
		MagicUsePercentage = setting.MagicUsePercentage;
		PsychicUsePercentage = setting.PsychicUsePercentage;
		AuxiliaryPercentage = setting.AuxiliaryPercentage;
		NaturalWeaponPercentage = setting.NaturalWeaponPercentage;
		PreferFavouriteWeapon = setting.PreferFavouriteWeapon;
		PreferShieldUse = setting.PreferShieldUse;
		PreferToFightArmed = setting.PreferToFightArmed;
		AttackUnarmedOrHelpless = setting.AttackUnarmedOrHelpless;
		AttackCriticallyInjured = setting.AttackCriticallyInjured;
		FallbackToUnarmedIfNoWeapon = setting.FallbackToUnarmedIfNoWeapon;
		SkirmishToOtherLocations = setting.SkirmishToOtherLocations;
		AutomaticallyMoveTowardsTarget = setting.AutomaticallyMoveTowardsTarget;
		PreferNonContactClinchBreaking = setting.PreferNonContactClinchBreaking;
		PursuitMode = (PursuitMode)setting.PursuitMode;
		MoveToMeleeIfCannotEngageInRangedCombat = setting.MoveToMeleeIfCannotEngageInRangedCombat;
		DefaultPreferredDefenseType = (DefenseType)setting.DefaultPreferredDefenseType;
		PreferredMeleeMode = (CombatStrategyMode)setting.PreferredMeleeMode;
		if (!PreferredMeleeMode.IsMeleeStrategy())
		{
			PreferredMeleeMode = CombatStrategyMode.StandardMelee;
			Changed = true;
		}

		PreferredRangedMode = (CombatStrategyMode)setting.PreferredRangedMode;
		if (!PreferredRangedMode.IsRangedStrategy())
		{
			PreferredRangedMode = CombatStrategyMode.StandardRange;
			Changed = true;
		}

		_requiredIntentions = (CombatMoveIntentions)setting.RequiredIntentions;
		_forbiddenIntentions = (CombatMoveIntentions)setting.ForbiddenIntentions;
		_preferredIntentions = (CombatMoveIntentions)setting.PreferredIntentions;
		if (!string.IsNullOrEmpty(setting.ClassificationsAllowed))
		{
			_classificationsAllowed.AddRange(
				setting.ClassificationsAllowed.Split(' ')
				       .Select(x => (WeaponClassification)int.Parse(x))
				       .Where(x => x != WeaponClassification.None));
		}

		ManualPositionManagement = setting.ManualPositionManagement;
		MovementManagement = (AutomaticMovementSettings)setting.MovementManagement;
		InventoryManagement = (AutomaticInventorySettings)setting.InventoryManagement;
		RangedManagement = (AutomaticRangedSettings)setting.RangedManagement;
		MinimumStaminaToAttack = setting.MinimumStaminaToAttack;
		PreferredWeaponSetup = (AttackHandednessOptions)setting.PreferredWeaponSetup;
		RequiredMinimumAim = setting.RequiredMinimumAim;
		GrappleResponse = (GrappleResponse)setting.GrappleResponse;
		foreach (var value in setting.MeleeAttackOrderPreference.Split(' '))
		{
			MeleeAttackOrderPreferences.Add((MeleeAttackOrderPreference)int.Parse(value));
		}
	}

	public override void Save()
	{
		using (new FMDB())
		{
			var dbitem = FMDB.Context.CharacterCombatSettings.Find(Id);
			dbitem.Name = Name;
			dbitem.Description = Description;
			dbitem.AvailabilityProgId = AvailabilityProg?.Id;
			dbitem.GlobalTemplate = GlobalTemplate;
			dbitem.CharacterOwnerId = CharacterOwnerId;
			dbitem.WeaponUsePercentage = WeaponUsePercentage;
			dbitem.MagicUsePercentage = MagicUsePercentage;
			dbitem.PsychicUsePercentage = PsychicUsePercentage;
			dbitem.AuxiliaryPercentage = AuxiliaryPercentage;
			dbitem.NaturalWeaponPercentage = NaturalWeaponPercentage;
			dbitem.PreferFavouriteWeapon = PreferFavouriteWeapon;
			dbitem.PreferShieldUse = PreferShieldUse;
			dbitem.PreferToFightArmed = PreferToFightArmed;
			dbitem.AttackUnarmedOrHelpless = AttackUnarmedOrHelpless;
			dbitem.FallbackToUnarmedIfNoWeapon = FallbackToUnarmedIfNoWeapon;
			dbitem.AttackCriticallyInjured = AttackCriticallyInjured;
			dbitem.SkirmishToOtherLocations = SkirmishToOtherLocations;
			dbitem.PreferredMeleeMode = (int)PreferredMeleeMode;
			dbitem.PreferredRangedMode = (int)PreferredRangedMode;
			dbitem.DefaultPreferredDefenseType = (int)DefaultPreferredDefenseType;
			dbitem.PursuitMode = (int)PursuitMode;
			dbitem.RequiredIntentions = (long)RequiredIntentions;
			dbitem.ForbiddenIntentions = (long)ForbiddenIntentions;
			dbitem.PreferredIntentions = (long)PreferredIntentions;
			dbitem.AutomaticallyMoveTowardsTarget = AutomaticallyMoveTowardsTarget;
			dbitem.PreferNonContactClinchBreaking = PreferNonContactClinchBreaking;
			dbitem.InventoryManagement = (int)InventoryManagement;
			dbitem.MovementManagement = (int)MovementManagement;
			dbitem.RangedManagement = (int)RangedManagement;
			dbitem.ManualPositionManagement = ManualPositionManagement;
			dbitem.MinimumStaminaToAttack = MinimumStaminaToAttack;
			dbitem.MoveToMeleeIfCannotEngageInRangedCombat = MoveToMeleeIfCannotEngageInRangedCombat;
			dbitem.ClassificationsAllowed =
				ClassificationsAllowed.Select(x => ((int)x).ToString())
				                      .ListToString(separator: " ", conjunction: "");
			dbitem.PreferredWeaponSetup = (int)PreferredWeaponSetup;
			dbitem.RequiredMinimumAim = RequiredMinimumAim;
			dbitem.MeleeAttackOrderPreference = MeleeAttackOrderPreferences
			                                    .Select(x => ((int)x).ToString()).ListToCommaSeparatedValues(" ");
			dbitem.GrappleResponse = (int)GrappleResponse;
			FMDB.Context.SaveChanges();
			Changed = false;
		}
	}

	public bool AddClassification(WeaponClassification classification)
	{
		if (GlobalTemplate || _classificationsAllowed.Contains(classification))
		{
			return false;
		}

		_classificationsAllowed.Add(classification);
		Changed = true;
		return true;
	}

	public bool RemoveClassification(WeaponClassification classification)
	{
		if (GlobalTemplate || !_classificationsAllowed.Contains(classification))
		{
			return false;
		}

		_classificationsAllowed.Remove(classification);
		Changed = true;
		return true;
	}

	public bool ClearClassifications()
	{
		_classificationsAllowed.Clear();
		Changed = true;
		return true;
	}

	public bool CanUse(ICharacter who)
	{
		return
			who.Id == CharacterOwnerId ||
			(GlobalTemplate && AvailabilityProg?.Execute<bool?>(who) != false);
	}

	public string Show(ICharacter voyeur)
	{
		var sb = new StringBuilder();
		sb.AppendLineColumns((uint)voyeur.LineFormatLength, 2,
			$"{$"Combat Setting #{Id:N0}".Colour(Telnet.BoldYellow)} - {Name.Colour(Telnet.Cyan)}",
			$"Global: {(GlobalTemplate ? "Yes".Colour(Telnet.Green) : "No".Colour(Telnet.Red))}");

		sb.AppendLine($"Description: {Description}");
		if (voyeur.IsAdministrator())
		{
			sb.AppendLineColumns((uint)voyeur.LineFormatLength, 3,
				$"Global: {(GlobalTemplate ? "Yes".Colour(Telnet.Green) : "No".Colour(Telnet.Red))}",
				$"Owner: {CharacterOwnerId?.ToString("N0", voyeur).Colour(Telnet.Green) ?? "None"}",
				$"Availability Prog: {(AvailabilityProg != null ? string.Format(voyeur, "{0} (#{1:N0})".FluentTagMXP("send", $"href='show futureprog {AvailabilityProg.Id}'"), AvailabilityProg.FunctionName, AvailabilityProg.Id) : "None".Colour(Telnet.Red))}");
		}

		sb.AppendLine();
		sb.AppendLine("Auto-Inventory".GetLineWithTitle(voyeur.LineFormatLength, voyeur.Account.UseUnicode,
			Telnet.Cyan, Telnet.BoldYellow));
		sb.AppendLineColumns((uint)voyeur.LineFormatLength, 3,
			$"Inventory: {InventoryManagement.Describe().Colour(Telnet.Green)}",
			$"Combat Movement: {MovementManagement.Describe().Colour(Telnet.Green)}",
			$"Ranged Combat: {RangedManagement.Describe().Colour(Telnet.Green)}");
		sb.AppendLineColumns((uint)voyeur.LineFormatLength, 3,
			$"Prefer to Fight Armed: {(PreferToFightArmed ? "Yes".Colour(Telnet.Green) : "No".Colour(Telnet.Red))}",
			$"Prefer Favourite Weapon: {(PreferFavouriteWeapon ? "Yes".Colour(Telnet.Green) : "No".Colour(Telnet.Red))}",
			$"Prefer Shield Use: {(PreferShieldUse ? "Yes".Colour(Telnet.Green) : "No".Colour(Telnet.Red))}");
		sb.AppendLine(
			$"Classifications Allowed: {ClassificationsAllowed.Select(x => x.Describe().Colour(Telnet.Green)).ListToString()}");

		sb.AppendLine();
		sb.AppendLine("Auto-Attack Selection".GetLineWithTitle(voyeur.LineFormatLength, voyeur.Account.UseUnicode,
			Telnet.Cyan, Telnet.BoldYellow));
		sb.AppendLineColumns((uint)voyeur.LineFormatLength, 3,
			$"Preferred Setup: {PreferredWeaponSetup.Describe().Colour(Telnet.Green)}",
			$"Non-Contact Clinch Breakers: {(PreferNonContactClinchBreaking ? "Yes".Colour(Telnet.Green) : "No".Colour(Telnet.Red))}",
			$"Melee if Can't Range? {(MoveToMeleeIfCannotEngageInRangedCombat ? "Yes".Colour(Telnet.Green) : "No".Colour(Telnet.Red))}");
		sb.AppendLineColumns((uint)voyeur.LineFormatLength, 3,
			$"Attack Unarmed or Helpless: {(AttackUnarmedOrHelpless ? "Yes".Colour(Telnet.Green) : "No".Colour(Telnet.Red))}",
			$"Attack Critically Wounded: {(AttackCriticallyInjured ? "Yes".Colour(Telnet.Green) : "No".Colour(Telnet.Red))}",
			$"Unarmed if Weaponless: {(FallbackToUnarmedIfNoWeapon ? "Yes".Colour(Telnet.Green) : "No".Colour(Telnet.Red))}");
		sb.AppendLineColumns((uint)voyeur.LineFormatLength, 3,
			$"Minimum Aim: {RequiredMinimumAim.ToString("P0", voyeur).Colour(Telnet.Green)}",
			$"Minimum Stamina to Attack: {MinimumStaminaToAttack.ToString("N2", voyeur).Colour(Telnet.Green)}",
			$"Grapple Response: {GrappleResponse.DescribeEnum(true).ColourValue()}");
		sb.AppendLine($"Required Intentions: {RequiredIntentions.Describe().Colour(Telnet.Cyan)}");
		sb.AppendLine($"Forbidden Intentions: {ForbiddenIntentions.Describe().Colour(Telnet.Red)}");
		sb.AppendLine($"Preferred Intentions: {PreferredIntentions.Describe().Colour(Telnet.Green)}");
		sb.AppendLine(
			$"Balance: Weapon {$"{WeaponUsePercentage:P0}".Colour(Telnet.Green)} / Unarmed " +
			$"{NaturalWeaponPercentage:P0}".Colour(Telnet.Green) +
			$"{(AuxiliaryPercentage > 0 ? $"Auxiliary {AuxiliaryPercentage:P0}" : "")}" +
			$"{(MagicUsePercentage > 0 ? $" Magic {MagicUsePercentage:P0}" : "")}" +
			$"{(PsychicUsePercentage > 0 ? $" Psychic {PsychicUsePercentage:P0}" : "")}");
		sb.AppendLine(
			$"Melee Order: {MeleeAttackOrderPreferences.Select(x => x.DescribeEnum().Colour(Telnet.Yellow)).ListToString()}");

		sb.AppendLineColumns((uint)voyeur.LineFormatLength, 2,
			$"Melee Strategy: {PreferredMeleeMode.Describe().Colour(Telnet.Green)}",
			$"Range Strategy: {PreferredRangedMode.Describe().Colour(Telnet.Green)}");

		sb.AppendLine();
		sb.AppendLine("Auto-Movement".GetLineWithTitle(voyeur.LineFormatLength, voyeur.Account.UseUnicode,
			Telnet.Cyan, Telnet.BoldYellow));
		sb.AppendLine($"Skirmish to Other Rooms: {SkirmishToOtherLocations.ToString().Colour(Telnet.Green)}");
		sb.AppendLineColumns((uint)voyeur.LineFormatLength, 3,
			$"Pursuit Mode: {PursuitMode.Describe().Colour(Telnet.Green)}",
			$"Move Rooms To Target: {AutomaticallyMoveTowardsTarget.ToString().Colour(Telnet.Green)}",
			$"Combat Position: {(ManualPositionManagement ? "Manual" : "Automatic").Colour(Telnet.Green)}");

		return sb.ToString();
	}

	public void SetName(string name)
	{
		_name = name;
		Changed = true;
	}
}