using MudSharp.Accounts;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.RPG.Checks;

#nullable enable
namespace MudSharp.GameItems.Prototypes;

public sealed class ChargedMagicDeviceGameItemComponentProto : GameItemComponentProto, IChargedMagicDevicePrototype
{
	private string? _error;
	private readonly HashSet<long> _spells = [];
	private readonly SortedDictionary<int, int> _gradeCosts = [];
	public override string TypeDescription => "ChargedMagicDevice";
	public MagicDeviceKind Kind { get; private set; } = MagicDeviceKind.Wand;
	public MagicDeviceRole Role { get; private set; } = MagicDeviceRole.Charged;
	public MagicDeviceEligibility Eligibility { get; private set; } = MagicDeviceEligibility.Caster;
	public int Capacity { get; private set; } = 5;
	public int SecondsPerCharge { get; private set; } = 60;
	public long CapabilityId { get; private set; }
	public long UseProgId { get; private set; }
	public long CheckTraitId { get; private set; }
	public int MinimumUseGrade { get; private set; }
	public Difficulty CheckDifficulty { get; private set; } = Difficulty.Normal;
	public Outcome MinimumOutcome { get; private set; } = Outcome.MinorPass;
	public MagicDeviceMode Mode { get; private set; } = MagicDeviceMode.StoredCharges;
	public long ReservoirSpellId { get; private set; }
	public long ReservoirResourceId { get; private set; }
	public int MaximumEnergy { get; private set; }
	public int InitialEnergy { get; private set; }
	public bool AllowsWornUse { get; private set; }
	public MagicDeviceDelivery Delivery { get; private set; } = MagicDeviceDelivery.OtherCharacters;
	public bool RequiresOpenSky { get; private set; }
	public IReadOnlyDictionary<int, int> GradeCosts => _gradeCosts;
	public IReadOnlySet<long> Spells => _spells;
	internal string Configuration => SaveToXml();
	private IInventoryPlanTemplate? _plan;
	public IInventoryPlanTemplate ProductionPlan => _plan ??= new InventoryPlanTemplate(Gameworld, [new InventoryPlanPhaseTemplate(1, [])]);
	public ChargedMagicDeviceGameItemComponentProto(IFuturemud game, IAccount account) : base(game, account, "ChargedMagicDevice") { }
	public ChargedMagicDeviceGameItemComponentProto(Models.GameItemComponentProto model, IFuturemud game) : base(model, game) { }
	protected override void LoadFromXml(XElement root)
	{
		try
		{
			if ((int?)root.Attribute("version") != 1) throw new FormatException("Unsupported device configuration.");
			Kind = (MagicDeviceKind)(int)root.Element("Kind")!;
			Role = (MagicDeviceRole)(int)root.Element("Role")!;
			Eligibility = (MagicDeviceEligibility)(int)root.Element("Eligibility")!;
			Capacity = (int)root.Element("Capacity")!;
			SecondsPerCharge = (int)root.Element("Seconds")!;
			CapabilityId = (long?)root.Element("Capability") ?? 0;
			UseProgId = (long?)root.Element("UseProg") ?? 0;
			CheckTraitId = (long?)root.Element("CheckTrait") ?? 0;
			MinimumUseGrade = (int?)root.Element("MinimumUseGrade") ?? 0;
			CheckDifficulty = (Difficulty)((int?)root.Element("Difficulty") ?? (int)Difficulty.Normal);
			MinimumOutcome = (Outcome)((int?)root.Element("Outcome") ?? (int)Outcome.MinorPass);
			if (root.Element("Reservoir") is { } reservoir)
			{
				Mode = (MagicDeviceMode)((int?)reservoir.Attribute("mode") ?? 0);
				ReservoirSpellId = (long?)reservoir.Attribute("spell") ?? 0;
				ReservoirResourceId = (long?)reservoir.Attribute("resource") ?? 0;
				MaximumEnergy = (int?)reservoir.Attribute("maximum") ?? 0;
				InitialEnergy = (int?)reservoir.Attribute("initial") ?? 0;
				AllowsWornUse = (bool?)reservoir.Attribute("worn") ?? false;
				Delivery = (MagicDeviceDelivery)((int?)reservoir.Attribute("delivery") ?? 0);
				RequiresOpenSky = (bool?)reservoir.Attribute("openSky") ?? false;
				foreach (var cost in reservoir.Elements("GradeCost"))
					if (!_gradeCosts.TryAdd((int)cost.Attribute("grade")!, (int)cost.Attribute("cost")!))
						throw new FormatException("Duplicate reservoir grade cost.");
			}
			foreach (var spell in root.Elements("Spell")) _spells.Add((long)spell);
			_plan = root.Element("Plan") is { } plan ? new InventoryPlanTemplate(plan, Gameworld) : null;
		}
		catch (Exception ex) { _error = ex.Message; }
	}
	public IReadOnlyList<string> ConfigurationErrors()
	{
		var errors = new List<string>();
		if (_error is not null) errors.Add(_error);
		if (!Enum.IsDefined(Kind) || !Enum.IsDefined(Role) || !Enum.IsDefined(Eligibility)) errors.Add("Invalid device mode or eligibility.");
		if (!Enum.IsDefined(Mode) || !Enum.IsDefined(Delivery)) errors.Add("Invalid reservoir mode or delivery policy.");
		if (Capacity is < 1 or > 100 || SecondsPerCharge is < 1 or > 86400) errors.Add("Capacity must be 1-100; seconds per charge 1-86,400.");
		if (Mode == MagicDeviceMode.StoredCharges && (_spells.Count == 0 || _spells.Any(id => Gameworld.MagicSpells.Get(id) is null))) errors.Add("Explicitly allow at least one existing compatible spell for this carrier.");
		if (Mode == MagicDeviceMode.EnergyReservoir)
		{
			if (Role != MagicDeviceRole.Charged || CheckTraitId != 0) errors.Add("Energy reservoirs use a fixed charged payload and no additional item-control check.");
			if (ReservoirSpellId <= 0 || Gameworld.MagicSpells.Get(ReservoirSpellId) is null) errors.Add("Reservoir mode requires one existing fixed spell.");
			if (ReservoirResourceId <= 0 || Gameworld.MagicResources.Get(ReservoirResourceId) is null) errors.Add("Reservoir mode requires one existing energy resource.");
			if (MaximumEnergy <= 0 || InitialEnergy < 0 || InitialEnergy > MaximumEnergy) errors.Add("Reservoir energy must have a positive maximum and initial energy within that maximum.");
			if (_gradeCosts.Count == 0 || _gradeCosts.Any(x => x.Key is < 1 or > 7 || x.Value <= 0)) errors.Add("Configure positive integer energy costs for grades 1-7.");
		}
		if (Eligibility == MagicDeviceEligibility.MagicType && CapabilityId == 0 || CapabilityId != 0 && Gameworld.MagicCapabilities.Get(CapabilityId) is null) errors.Add("Select an existing capability for the magic type requirement.");
		if (CheckTraitId != 0 && Gameworld.Traits.Get(CheckTraitId) is null) errors.Add("Missing item-control check trait.");
		if (MinimumUseGrade is < 0 or > 7) errors.Add("Minimum acquired use grade must be 0-7; zero disables this additional requirement.");
		if (!Enum.IsDefined(CheckDifficulty) || MinimumOutcome is not (Outcome.MinorPass or Outcome.Pass or Outcome.MajorPass)) errors.Add("Invalid committed control check policy.");
		if (UseProgId != 0)
		{
			var prog = Gameworld.FutureProgs.Get(UseProgId);
			if (prog is null || prog.AcceptsAnyParameters || prog.ReturnType != ProgVariableTypes.Boolean ||
				!prog.Parameters.SequenceEqual(new[] { ProgVariableTypes.Character, ProgVariableTypes.Item }) || !string.IsNullOrEmpty(prog.CompileError))
				errors.Add("Usability requires a compiled boolean(character,item) prog.");
		}
		return errors.AsReadOnly();
	}
	public override bool CanSubmit() => ConfigurationErrors().Count == 0;
	public override string WhyCannotSubmit() => string.Join("\n", ConfigurationErrors());
	protected override string SaveToXml() => MagicDeviceDefinition.Serialize(Kind, Role, Eligibility, Capacity,
		SecondsPerCharge, CapabilityId, UseProgId, CheckTraitId, MinimumUseGrade, CheckDifficulty, MinimumOutcome,
		_spells, ProductionPlan.SaveToXml(), Mode, ReservoirSpellId, ReservoirResourceId, MaximumEnergy,
			InitialEnergy, AllowsWornUse, Delivery, _gradeCosts, RequiresOpenSky);
	public const string HelpText = @"Settings:
	kind wand|staff - carrier name; new wand/staff defaults are 5/10 charges
	role charged|focus|dual - explicit modes; depleted charged mode never casts personally
	eligibility anyone|caster|magictype|acquiredspell - current activation entitlement
	capability <capability|none> - exact magic type for magictype eligibility
	capacity <1-100>, seconds <1-86400> - capacity and production time per charge
	spell add|remove <spell> - explicit carrier-compatible payload whitelist
	usable <prog|none> - boolean(character,item), rechecked before commitment
	check <trait|none> [difficulty] [minorpass|pass|majorpass] - optional committed control check
	mingrade <0-7> - additional current acquired/control grade requirement; zero disables
	plan add <normal action>, plan remove <ordinal> - additional per-charge production supplies
	reservoir mode storedcharges|energyreservoir; reservoir spell <spell>; reservoir resource <resource>
	reservoir energy <maximum> <initial>; reservoir cost <grade 1-7> <positive integer>
	reservoir clearcosts; reservoir worn yes|no; reservoir delivery othercharacters|roomcorpses|nativearea
	reservoir opensky yes|no - optional room-wide spell location restriction
	checkconfig - validate references

Stored-charge instances start empty; reservoir instances start at their configured initial energy.
Use magicdevice show, charge, charged or focus for stored-charge devices. Stock eligibility is caster.
Production is acquired-route-only and pays spell and production supplies per charge.
Committed control failure spends one charge. Item use gives no spell proficiency or mastery.";
	public override string ShowBuildingHelp => HelpText;
	private static T DefinedEnum<T>(string text) where T : struct, Enum => Enum.TryParse<T>(text, true, out var value) && Enum.IsDefined(value)
		? value : throw new InvalidOperationException($"Use one of: {string.Join(", ", Enum.GetNames<T>())}.");
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		var setting = command.PopForSwitch();
		try
		{
			switch (setting)
			{
				case "checkconfig": actor.Send(string.Join("\n", ConfigurationErrors().DefaultIfEmpty("Device configuration is valid."))); return true;
				case "kind": Kind = DefinedEnum<MagicDeviceKind>(command.PopSpeech()); Capacity = Kind == MagicDeviceKind.Staff ? 10 : 5; break;
				case "role": Role = DefinedEnum<MagicDeviceRole>(command.PopSpeech()); break;
				case "eligibility": Eligibility = DefinedEnum<MagicDeviceEligibility>(command.PopSpeech()); break;
				case "capacity": var capacity = int.Parse(command.PopSpeech()); if (capacity is < 1 or > 100) throw new InvalidOperationException("Capacity must be 1-100."); Capacity = capacity; break;
				case "seconds": var seconds = int.Parse(command.PopSpeech()); if (seconds is < 1 or > 86400) throw new InvalidOperationException("Seconds must be 1-86,400."); SecondsPerCharge = seconds; break;
				case "mingrade": var minimumGrade = int.Parse(command.PopSpeech()); if (minimumGrade is < 0 or > 7) throw new InvalidOperationException("Minimum use grade must be 0-7."); MinimumUseGrade = minimumGrade; break;
			case "reservoir":
				var reservoirSetting = command.PopForSwitch();
				switch (reservoirSetting)
				{
					case "mode": Mode = DefinedEnum<MagicDeviceMode>(command.PopSpeech()); break;
					case "spell": ReservoirSpellId = Gameworld.MagicSpells.GetByIdOrName(command.SafeRemainingArgument)?.Id ?? throw new InvalidOperationException("No such spell."); break;
					case "resource": ReservoirResourceId = Gameworld.MagicResources.GetByIdOrName(command.SafeRemainingArgument)?.Id ?? throw new InvalidOperationException("No such magic resource."); break;
					case "energy": var max = int.Parse(command.PopSpeech()); var initial = int.Parse(command.PopSpeech()); if (max < 1 || initial < 0 || initial > max) throw new InvalidOperationException("Use reservoir energy <maximum> <initial>."); MaximumEnergy = max; InitialEnergy = initial; break;
					case "cost": var grade = int.Parse(command.PopSpeech()); var cost = int.Parse(command.PopSpeech()); if (grade is < 1 or > 7 || cost <= 0) throw new InvalidOperationException("Use reservoir cost <grade 1-7> <positive integer>."); _gradeCosts[grade] = cost; break;
					case "clearcosts": _gradeCosts.Clear(); break;
					case "worn": var worn = command.PopSpeech(); if (!worn.EqualTo("yes") && !worn.EqualTo("no")) throw new InvalidOperationException("Use reservoir worn yes|no."); AllowsWornUse = worn.EqualTo("yes"); break;
					case "delivery": Delivery = DefinedEnum<MagicDeviceDelivery>(command.SafeRemainingArgument); break;
					case "opensky": var sky = command.PopSpeech(); if (!sky.EqualTo("yes") && !sky.EqualTo("no")) throw new InvalidOperationException("Use reservoir opensky yes|no."); RequiresOpenSky = sky.EqualTo("yes"); break;
					default: throw new InvalidOperationException("Use reservoir mode|spell|resource|energy|cost|clearcosts|worn|delivery.");
				}
				break;
				case "capability":
					var capability = command.SafeRemainingArgument;
					CapabilityId = capability.EqualTo("none") ? 0 : Gameworld.MagicCapabilities.GetByIdOrName(capability)?.Id ?? throw new InvalidOperationException("No such capability."); break;
				case "usable":
					var prog = command.SafeRemainingArgument;
					UseProgId = prog.EqualTo("none") ? 0 : Gameworld.FutureProgs.GetByIdOrName(prog)?.Id ?? throw new InvalidOperationException("No such prog."); break;
				case "spell":
					var operation = command.PopForSwitch();
					var spell = Gameworld.MagicSpells.GetByIdOrName(command.SafeRemainingArgument) ?? throw new InvalidOperationException("No such spell.");
					if (operation == "add") _spells.Add(spell.Id); else if (operation == "remove") _spells.Remove(spell.Id); else throw new InvalidOperationException("Use spell add|remove <spell>."); break;
				case "check":
					var trait = command.PopSpeech();
					CheckTraitId = trait.EqualTo("none") ? 0 : Gameworld.Traits.GetByIdOrName(trait)?.Id ?? throw new InvalidOperationException("No such trait.");
					if (!command.IsFinished) CheckDifficulty = DefinedEnum<Difficulty>(command.PopSpeech());
					if (!command.IsFinished) MinimumOutcome = DefinedEnum<Outcome>(command.PopSpeech()); break;
				case "plan":
					var planOperation = command.PopForSwitch();
					if (planOperation == "add") { var action = InventoryPlanTemplate.ParseActionFromBuilderInput(actor, command); if (action is null) return false; ProductionPlan.Phases.First().AddAction(action); }
					else if (planOperation == "remove") ProductionPlan.Phases.First().RemoveAction(ProductionPlan.Phases.First().Actions.ElementAt(int.Parse(command.PopSpeech()) - 1));
					else throw new InvalidOperationException("Use plan add|remove."); break;
				default: return base.BuildingCommand(actor, command.GetUndo());
			}
			Changed = true; actor.Send("Device configuration updated."); return true;
		}
		catch (Exception ex) { actor.Send(ex.Message); return false; }
	}
	public override string ComponentDescriptionOLC(ICharacter actor) => $"{Kind}: {Role}, {Mode}, eligibility {Eligibility}, capacity {Capacity}, seconds/charge {SecondsPerCharge}, capability #{CapabilityId}, control trait #{CheckTraitId}; spells {string.Join(", ", _spells)}; reservoir spell #{ReservoirSpellId}, resource #{ReservoirResourceId}, energy {InitialEnergy}/{MaximumEnergy}, costs {string.Join(", ", _gradeCosts.Select(x => $"{x.Key}:{x.Value}"))}\n{WhyCannotSubmit()}";
	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter? loader = null, bool temporary = false) => new ChargedMagicDeviceGameItemComponent(this, parent, temporary);
	public override IGameItemComponent LoadComponent(Models.GameItemComponent model, IGameItem parent) => new ChargedMagicDeviceGameItemComponent(model, this, parent);
	public override IEditableRevisableItem CreateNewRevision(ICharacter actor) => CreateNewRevision(actor, (model, game) => new ChargedMagicDeviceGameItemComponentProto(model, game));
	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("chargedmagicdevice", true, (game, account) => new ChargedMagicDeviceGameItemComponentProto(game, account));
		manager.AddDatabaseLoader("ChargedMagicDevice", (model, game) => new ChargedMagicDeviceGameItemComponentProto(model, game));
		manager.AddTypeHelpInfo("ChargedMagicDevice", "Explicit charged wand/staff or focus with paid homogeneous payloads", HelpText);
	}
}
