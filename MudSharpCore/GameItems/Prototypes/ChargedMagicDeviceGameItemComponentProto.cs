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
		if (Capacity is < 1 or > 100 || SecondsPerCharge is < 1 or > 86400) errors.Add("Capacity must be 1-100; seconds per charge 1-86,400.");
		if (_spells.Count == 0 || _spells.Any(id => Gameworld.MagicSpells.Get(id) is null)) errors.Add("Explicitly allow at least one existing compatible spell for this carrier.");
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
		_spells, ProductionPlan.SaveToXml());
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
	checkconfig - validate references

Instances start empty. Use magicdevice show, charge, charged or focus. Stock eligibility is caster.
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
	public override string ComponentDescriptionOLC(ICharacter actor) => $"{Kind}: {Role}, eligibility {Eligibility}, capacity {Capacity}, seconds/charge {SecondsPerCharge}, capability #{CapabilityId}, control trait #{CheckTraitId}; spells {string.Join(", ", _spells)}\n{WhyCannotSubmit()}";
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
