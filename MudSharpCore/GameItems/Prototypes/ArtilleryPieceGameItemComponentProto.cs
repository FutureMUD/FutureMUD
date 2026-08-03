using MudSharp.Accounts;
using MudSharp.Combat;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;

#nullable enable

namespace MudSharp.GameItems.Prototypes;

/// <summary>
/// Builder configuration for a portable, crew-served artillery piece. The profile string is
/// intentionally data-owned, allowing the seeder to share it with ammunition and chambers.
/// </summary>
public class ArtilleryPieceGameItemComponentProto : GameItemComponentProto, IArtilleryPiecePrototype
{
	public override string TypeDescription => "Artillery Piece";
	public IRangedWeaponType? RangedWeaponType { get; private set; }
	public string ArtilleryProfile { get; private set; } = "general";
	public ArtilleryLoadingMechanism LoadingMechanism { get; private set; }
	public int MinimumCrew { get; private set; } = 1;
	public int MaximumCrew { get; private set; } = 4;
	public bool RequiresEmplacement { get; private set; } = true;
	public double MaximumTraverse { get; private set; } = 180.0;
	public double MaximumElevation { get; private set; } = 45.0;
	public double MaximumDepression { get; private set; } = 5.0;
	private readonly Dictionary<string, HashSet<ArtilleryCrewAction>> _crewRoles =
		new(StringComparer.InvariantCultureIgnoreCase);

	protected ArtilleryPieceGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "ArtilleryPiece")
	{
		RangedWeaponType = gameworld.RangedWeaponTypes.FirstOrDefault(x => x.RangedWeaponType == Combat.RangedWeaponType.Artillery);
		SeedDefaultCrewRoles();
	}

	protected ArtilleryPieceGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	protected override void LoadFromXml(XElement root)
	{
		RangedWeaponType = Gameworld.RangedWeaponTypes.Get((long?)root.Element("RangedWeaponType") ?? 0);
		ArtilleryProfile = root.Element("ArtilleryProfile")?.Value ?? "general";
		LoadingMechanism = root.Element("LoadingMechanism")?.Value.TryParseEnum<ArtilleryLoadingMechanism>(out var mechanism) == true
			? mechanism
			: ArtilleryLoadingMechanism.MuzzleLoading;
		MinimumCrew = Math.Max(1, (int?)root.Element("MinimumCrew") ?? 1);
		MaximumCrew = Math.Max(MinimumCrew, (int?)root.Element("MaximumCrew") ?? Math.Max(4, MinimumCrew));
		RequiresEmplacement = (bool?)root.Element("RequiresEmplacement") ?? true;
		MaximumTraverse = Math.Clamp((double?)root.Element("MaximumTraverse") ?? 180.0, 0.0, 360.0);
		MaximumElevation = Math.Clamp((double?)root.Element("MaximumElevation") ?? 45.0, 0.0, 90.0);
		MaximumDepression = Math.Clamp((double?)root.Element("MaximumDepression") ?? 5.0, 0.0, 90.0);
		_crewRoles.Clear();
		foreach (var role in root.Element("CrewRoles")?.Elements("Role") ?? [])
		{
			var name = role.Attribute("name")?.Value;
			if (string.IsNullOrWhiteSpace(name)) continue;
			var actions = role.Elements("Action")
				.Select(x => x.Value.TryParseEnum<ArtilleryCrewAction>(out var action) ? action : (ArtilleryCrewAction?)null)
				.Where(x => x.HasValue)
				.Select(x => x!.Value)
				.ToHashSet();
			if (actions.Count > 0) _crewRoles[name] = actions;
		}
		if (_crewRoles.Count == 0) SeedDefaultCrewRoles();
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("RangedWeaponType", RangedWeaponType?.Id ?? 0),
			new XElement("ArtilleryProfile", ArtilleryProfile),
			new XElement("LoadingMechanism", LoadingMechanism),
			new XElement("MinimumCrew", MinimumCrew),
			new XElement("MaximumCrew", MaximumCrew),
			new XElement("RequiresEmplacement", RequiresEmplacement),
			new XElement("MaximumTraverse", MaximumTraverse),
			new XElement("MaximumElevation", MaximumElevation),
			new XElement("MaximumDepression", MaximumDepression),
			new XElement("CrewRoles", _crewRoles.Select(x => new XElement("Role", new XAttribute("name", x.Key),
				x.Value.Order().Select(y => new XElement("Action", y)))))).ToString();
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter? loader = null, bool temporary = false)
	{
		return new ArtilleryPieceGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new ArtilleryPieceGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator, (proto, gameworld) => new ArtilleryPieceGameItemComponentProto(proto, gameworld));
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("artillery", true, (gameworld, account) => new ArtilleryPieceGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("artillerypiece", false, (gameworld, account) => new ArtilleryPieceGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("ArtilleryPiece", (proto, gameworld) => new ArtilleryPieceGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo("ArtilleryPiece", "Makes an item a portable, crew-served artillery piece", BuildingHelpText);
	}

	private const string BuildingHelpText = "Options: ranged <type>, profile <name>, mechanism <muzzleloading|removablechamber>, crew <minimum> [maximum], emplacement <true|false>, arc <traverse> <elevation> <depression>.";
	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "ranged":
				var text = command.SafeRemainingArgument;
				var type = Gameworld.RangedWeaponTypes.GetByName(text);
				if (type?.RangedWeaponType != Combat.RangedWeaponType.Artillery)
				{
					actor.Send("You must select an artillery ranged weapon type.");
					return false;
				}
				RangedWeaponType = type;
				Changed = true;
				return true;
			case "profile":
				ArtilleryProfile = command.SafeRemainingArgument.ToLowerInvariant();
				Changed = !string.IsNullOrWhiteSpace(ArtilleryProfile);
				return Changed;
			case "mechanism":
				if (!command.PopSpeech().TryParseEnum<ArtilleryLoadingMechanism>(out var mechanism)) return false;
				LoadingMechanism = mechanism;
				Changed = true;
				return true;
			case "crew":
				if (!int.TryParse(command.PopSpeech(), out var crew) || crew < 1 || crew > 16) return false;
				MinimumCrew = crew;
				MaximumCrew = command.IsFinished ? Math.Max(MaximumCrew, crew) :
					int.TryParse(command.PopSpeech(), out var maximum) && maximum >= crew && maximum <= 16 ? maximum : MaximumCrew;
				Changed = true;
				return true;
			case "arc":
				if (!double.TryParse(command.PopSpeech(), out var traverse) || !double.TryParse(command.PopSpeech(), out var elevation) ||
					!double.TryParse(command.PopSpeech(), out var depression)) return false;
				MaximumTraverse = Math.Clamp(traverse, 0.0, 360.0);
				MaximumElevation = Math.Clamp(elevation, 0.0, 90.0);
				MaximumDepression = Math.Clamp(depression, 0.0, 90.0);
				Changed = true;
				return true;
			case "emplacement":
				if (!bool.TryParse(command.PopSpeech(), out var emplacement)) return false;
				RequiresEmplacement = emplacement;
				Changed = true;
				return true;
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	public override bool CanSubmit() => RangedWeaponType is not null && !string.IsNullOrWhiteSpace(ArtilleryProfile) && base.CanSubmit();
	public override string WhyCannotSubmit() => RangedWeaponType is null ? "You must select an artillery ranged weapon type." : base.WhyCannotSubmit();
	public override string ComponentDescriptionOLC(ICharacter actor) =>
		$"{Name.ColourName()} ({LoadingMechanism.DescribeEnum()}, profile {ArtilleryProfile.ColourValue()}, crew {MinimumCrew.ToString(actor).ColourValue()}-{MaximumCrew.ToString(actor).ColourValue()})";

	public IReadOnlyCollection<ArtilleryCrewAction> AllowedActionsForRole(string role)
	{
		return _crewRoles.TryGetValue(role, out var actions)
			? actions
			: _crewRoles.TryGetValue("crew", out var defaultActions) ? defaultActions : [];
	}

	public bool HasCrewRole(string role) => _crewRoles.ContainsKey(role);
	public IEnumerable<string> CrewRoles => _crewRoles.Keys;

	private void SeedDefaultCrewRoles()
	{
		_crewRoles["captain"] = [ArtilleryCrewAction.Command, ArtilleryCrewAction.Aim, ArtilleryCrewAction.Fire];
		_crewRoles["loader"] = [ArtilleryCrewAction.LoadCharge, ArtilleryCrewAction.LoadWad, ArtilleryCrewAction.LoadProjectile, ArtilleryCrewAction.Ram];
		_crewRoles["primer"] = [ArtilleryCrewAction.Vent, ArtilleryCrewAction.Prime];
		_crewRoles["sponger"] = [ArtilleryCrewAction.Sponge];
		_crewRoles["crew"] = Enum.GetValues<ArtilleryCrewAction>().ToHashSet();
	}
}
