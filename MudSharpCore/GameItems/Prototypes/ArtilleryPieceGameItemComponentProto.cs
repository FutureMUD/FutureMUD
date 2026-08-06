using MudSharp.Accounts;
using MudSharp.Combat;
using MudSharp.Form.Material;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Units;
using MudSharp.GameItems.Components;

#nullable enable

namespace MudSharp.GameItems.Prototypes;

/// <summary>
/// Builder configuration for a portable, crew-served artillery piece. The profile string is
/// intentionally data-owned, allowing the seeder to share it with ammunition and chambers.
/// </summary>
public class ArtilleryPieceGameItemComponentProto : GameItemComponentProto, IArtilleryPiecePrototype
{
	public const string SpongeTagName = "Artillery Sponge";
	public const string WaddingTagName = "Artillery Wadding";
	public const string RammerTagName = "Artillery Rammer";
	public const string VentToolTagName = "Artillery Vent Tool";
	public const string LinstockTagName = "Artillery Linstock";
	public const string FuseTagName = "Artillery Fuse";

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
	public double PowderMass { get; private set; } = 1500.0;
	public double PrimingPowderMass { get; private set; } = 25.0;
	public ISolid? GunpowderMaterial =>
		Gameworld.Materials.Get(Gameworld.GetStaticLong("GunpowderMaterialId")) ??
		Gameworld.Materials.GetByName(MusketGameItemComponentProto.GunpowderMaterialName) as ISolid;
	public ITag? SpongeTag => ResolveTag("SpongeTag", SpongeTagName);
	public ITag? WaddingTag => ResolveTag("WaddingTag", WaddingTagName);
	public ITag? RammerTag => ResolveTag("RammerTag", RammerTagName);
	public ITag? VentToolTag => ResolveTag("VentToolTag", VentToolTagName);
	public ITag? LinstockTag => ResolveTag("LinstockTag", LinstockTagName);
	public ITag? FuseTag => ResolveTag("FuseTag", FuseTagName);
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
		PowderMass = Math.Max(0.001, (double?)root.Element("PowderMass") ?? 1500.0);
		PrimingPowderMass = Math.Max(0.001, (double?)root.Element("PrimingPowderMass") ?? 25.0);
		_tagIds["SpongeTag"] = (long?)root.Element("SpongeTag") ?? 0;
		_tagIds["WaddingTag"] = (long?)root.Element("WaddingTag") ?? 0;
		_tagIds["RammerTag"] = (long?)root.Element("RammerTag") ?? 0;
		_tagIds["VentToolTag"] = (long?)root.Element("VentToolTag") ?? 0;
		_tagIds["LinstockTag"] = (long?)root.Element("LinstockTag") ?? 0;
		_tagIds["FuseTag"] = (long?)root.Element("FuseTag") ?? 0;
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
			new XElement("PowderMass", PowderMass),
			new XElement("PrimingPowderMass", PrimingPowderMass),
			new XElement("SpongeTag", SpongeTag?.Id ?? 0),
			new XElement("WaddingTag", WaddingTag?.Id ?? 0),
			new XElement("RammerTag", RammerTag?.Id ?? 0),
			new XElement("VentToolTag", VentToolTag?.Id ?? 0),
			new XElement("LinstockTag", LinstockTag?.Id ?? 0),
			new XElement("FuseTag", FuseTag?.Id ?? 0),
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

	private const string BuildingHelpText = @"You can use the following options:

	#3ranged <type>#0 - sets the artillery ranged weapon type
	#3profile <name>#0 - sets the ammunition and chamber compatibility profile
	#3mechanism <muzzleloading|removablechamber>#0 - sets the loading mechanism
	#3crew <minimum> [maximum]#0 - sets the required and maximum crew
	#3emplacement <true|false>#0 - controls whether the piece must be mounted or emplaced
	#3arc <traverse> <elevation> <depression>#0 - sets the firing arcs in degrees
	#3powder <mass>#0 - sets the physical gunpowder charge per shot
	#3primer <mass>#0 - sets the physical priming-powder charge
	#3spongetag|wadtag|rammertag|venttag|linstocktag|fusetag <tag>#0 - sets a physical tool or consumable tag";
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
			case "powder":
				return BuildingCommandMass(actor, command, false);
			case "primer":
				return BuildingCommandMass(actor, command, true);
			case "spongetag":
				return BuildingCommandTag(actor, command, "SpongeTag");
			case "wadtag":
				return BuildingCommandTag(actor, command, "WaddingTag");
			case "rammertag":
				return BuildingCommandTag(actor, command, "RammerTag");
			case "venttag":
				return BuildingCommandTag(actor, command, "VentToolTag");
			case "linstocktag":
				return BuildingCommandTag(actor, command, "LinstockTag");
			case "fusetag":
				return BuildingCommandTag(actor, command, "FuseTag");
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	private readonly Dictionary<string, long> _tagIds = new(StringComparer.OrdinalIgnoreCase);

	private ITag? ResolveTag(string key, string fallbackName)
	{
		return Gameworld.Tags.Get(_tagIds.GetValueOrDefault(key)) ?? Gameworld.Tags.GetByName(fallbackName);
	}

	private bool BuildingCommandTag(ICharacter actor, StringStack command, string key)
	{
		var text = command.SafeRemainingArgument;
		var tag = long.TryParse(text, out var id) ? Gameworld.Tags.Get(id) : Gameworld.Tags.GetByName(text);
		if (tag is null)
		{
			actor.Send("There is no such item tag.");
			return false;
		}
		_tagIds[key] = tag.Id;
		Changed = true;
		actor.Send($"The {key.ToLowerInvariant()} is now {tag.Name.ColourName()}.");
		return true;
	}

	private bool BuildingCommandMass(ICharacter actor, StringStack command, bool primer)
	{
		if (!Gameworld.UnitManager.TryGetBaseUnits(command.SafeRemainingArgument, UnitType.Mass, actor, out var mass) || mass <= 0.0)
		{
			actor.Send("You must specify a positive mass.");
			return false;
		}
		if (primer)
		{
			PrimingPowderMass = mass;
		}
		else
		{
			PowderMass = mass;
		}
		Changed = true;
		actor.Send($"The {(primer ? "primer" : "main charge")} now uses {Gameworld.UnitManager.DescribeExact(mass, UnitType.Mass, actor).ColourValue()} of gunpowder.");
		return true;
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
