using System.Globalization;
using MudSharp.Accounts;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.Magic;

#nullable enable
namespace MudSharp.GameItems.Prototypes;

/// <summary>Shared editing/persistence for four independently attachable content components.</summary>
public abstract class ConditionalMagicItemGameItemComponentProto : GameItemComponentProto
{
	public ConditionalMagicItemPolicy Policy { get; private set; } = new();
	protected abstract ConditionalMagicItemKind Kind { get; }
	protected ConditionalMagicItemGameItemComponentProto(IFuturemud world, IAccount account, string type) : base(world, account, type)
	{
		Policy = new() { Kind = Kind, EligibilityProgId = world.AlwaysFalseProg.Id };
		Changed = true;
	}
	protected ConditionalMagicItemGameItemComponentProto(MudSharp.Models.GameItemComponentProto model, IFuturemud world) : base(model, world) { }
	protected override void LoadFromXml(XElement root) => Policy = ConditionalMagicItemPolicy.Load(root);
	protected override string SaveToXml() => Policy.Save().ToString();
	public override bool CanSubmit() => Errors().Count == 0;
	public override string WhyCannotSubmit() => string.Join("\n", Errors());
	private IReadOnlyList<string> Errors()
	{
		var errors = Policy.Errors().ToList();
		if (Policy.Kind != Kind) errors.Add("The policy kind does not match this component.");
		foreach (var id in new[] { Policy.EligibilityProgId, Policy.ControllerProgId, Policy.TerritoryProgId, Policy.EscortProgId, Policy.ProtectedTargetProgId, Policy.IncompatibleWearerProgId }.Where(x => x != 0))
		{
			var prog = Gameworld.FutureProgs.Get(id);
			if (prog is null || !prog.ReturnType.CompatibleWith(ProgVariableTypes.Boolean) || !prog.MatchesParameters([ProgVariableTypes.Character])) errors.Add($"Prog #{id} must be boolean(character).");
		}
		if (Policy.CapabilityId > 0 && Gameworld.MagicCapabilities.Get(Policy.CapabilityId) is not IMagicCastingCapability) errors.Add("Choose a configured casting capability.");
		if (Policy.ResourceId > 0 && Gameworld.MagicResources.Get(Policy.ResourceId) is null) errors.Add("Missing resource.");
		foreach (var choice in Policy.Choices) if (Gameworld.MagicSpells.Get(choice.SpellId) is null) errors.Add($"Missing spell #{choice.SpellId}.");
		foreach (var id in Policy.IssuedPrototypeIds.Append(Policy.RequiredRegaliaPrototypeId).Where(x => x > 0)) if (Gameworld.ItemProtos.Get(id) is null) errors.Add($"Missing item prototype #{id}.");
		return errors;
	}
	public override string ComponentDescriptionOLC(ICharacter actor) =>
		$"{TypeDescription}: eligibility #{Policy.EligibilityProgId}, controller #{Policy.ControllerProgId}, territory #{Policy.TerritoryProgId}, escort #{Policy.EscortProgId}, protected target #{Policy.ProtectedTargetProgId}, incompatible wearer #{Policy.IncompatibleWearerProgId}.\nGroup {Policy.BindingGroup}; range {Policy.Range}; regalia #{Policy.RequiredRegaliaPrototypeId}; worn {Policy.RequireWorn}; wear locations {string.Join(", ", Policy.WearLocations)}.\nSelection capability #{Policy.CapabilityId}, limit {Policy.SelectionLimit}, replacement {Policy.AllowReplacement}: {string.Join(", ", Policy.Choices.Select(x => $"{x.SpellId} ({x.Category})"))}.\nResource #{Policy.ResourceId}, charge capacity {Policy.Capacity}; issued items {string.Join(", ", Policy.IssuedPrototypeIds)}.\nPeriod {Policy.IntervalSeconds}s +/-{Policy.IntervalJitterSeconds}s; chance {Policy.ActivationChance}; outside {Policy.PunishOutsideTerritory}; removal blocked {Policy.PreventRemoval}; hostile spells intercepted {Policy.InterceptHostileSpells}; delay {Policy.ActionDelaySeconds}s.\nActivation: {Policy.Activation}\nRemoval: {Policy.Removal}\nPeriodic: {Policy.Periodic}\nIncompatible wear: {Policy.IncompatibleWear}\n{WhyCannotSubmit()}";

	public const string Help = "eligibility|controller|territory|escort|protected|incompatible <boolean(character) prog|none>; regalia <item prototype|none>; group <text>; range <SameRoom|SameZone|SameShard|World>; wearlocations <names separated by commas>; removallock|outside|hostiles|worn|replacement <true|false>; interval|jitter|chance|delay <number>; capability <id>; limit|capacity <integer>; resource <id>; choice <spell id> <category> (updates/adds); unchoice <spell id>; issue <item prototype id> (toggles); activation|removal|periodic|incompatiblewear <health> <stun> <stamina> <health floor> <collapse bool> <destroy bool> <message>; activationfractions|removalfractions|periodicfractions|incompatiblewearfractions <health fraction> <stun fraction> <stamina fraction>; interceptspells <IDs>; interceptmoods <comma-separated moods>; message|rechargemessage <text>.";
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		var field = command.PopForSwitch();
		var value = command.SafeRemainingArgument;
		try
		{
			long Id() => long.Parse(value, CultureInfo.InvariantCulture);
			double Number() { var n = double.Parse(value, NumberStyles.Float, actor); if (!double.IsFinite(n)) throw new FormatException(); return n; }
			long Prog()
			{
				if (value.EqualTo("none")) return 0;
				var p = Gameworld.FutureProgs.GetByIdOrName(value);
				if (p is null || !p.ReturnType.CompatibleWith(ProgVariableTypes.Boolean) || !p.MatchesParameters([ProgVariableTypes.Character])) throw new FormatException("Choose a boolean(character) prog.");
				return p.Id;
			}
			ItemConsequence Consequence()
			{
				double Pop() => double.Parse(command.PopSpeech(), NumberStyles.Float, actor);
				var consequence = new ItemConsequence(Pop(), Pop(), Pop(), Pop(), bool.Parse(command.PopSpeech()), bool.Parse(command.PopSpeech()), command.SafeRemainingArgument);
				if (!consequence.IsValid) throw new FormatException("Amounts must be finite and nonnegative.");
				return consequence;
			}
			var next = field switch
			{
				"eligibility" => Policy with { EligibilityProgId = Prog() }, "controller" => Policy with { ControllerProgId = Prog() },
				"territory" => Policy with { TerritoryProgId = Prog() }, "escort" => Policy with { EscortProgId = Prog() },
				"protected" => Policy with { ProtectedTargetProgId = Prog() }, "incompatible" => Policy with { IncompatibleWearerProgId = Prog() },
				"regalia" => Policy with { RequiredRegaliaPrototypeId = value.EqualTo("none") ? 0 : Id() },
				"group" => Policy with { BindingGroup = value }, "range" => Policy with { Range = Enum.Parse<BindingActivationRange>(value, true) },
				"wearlocations" => Policy with { WearLocations = value.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries) },
				"interceptspells" => Policy with { InterceptSpellIds = value.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToArray() },
				"interceptmoods" => Policy with { InterceptMoods = value.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries) },
				"removallock" => Policy with { PreventRemoval = bool.Parse(value) }, "outside" => Policy with { PunishOutsideTerritory = bool.Parse(value) },
				"hostiles" => Policy with { InterceptHostileSpells = bool.Parse(value) }, "worn" => Policy with { RequireWorn = bool.Parse(value) },
				"replacement" => Policy with { AllowReplacement = bool.Parse(value) }, "interval" => Policy with { IntervalSeconds = Number() },
				"jitter" => Policy with { IntervalJitterSeconds = Number() }, "chance" => Policy with { ActivationChance = Number() },
				"delay" => Policy with { ActionDelaySeconds = Number() }, "capability" => Policy with { CapabilityId = Id() },
				"limit" => Policy with { SelectionLimit = int.Parse(value) }, "capacity" => Policy with { Capacity = int.Parse(value) },
				"resource" => Policy with { ResourceId = Id() }, "message" => Policy with { ActivationMessage = value },
				"rechargemessage" => Policy with { RechargeMessage = value }, "activation" => Policy with { Activation = Consequence() },
				"removal" => Policy with { Removal = Consequence() }, "periodic" => Policy with { Periodic = Consequence() },
				"incompatiblewear" => Policy with { IncompatibleWear = Consequence() },
				_ => null
			};
			if (field == "choice")
			{
				var spell = Gameworld.MagicSpells.GetByIdOrName(command.PopSpeech()) ?? throw new FormatException("Choose a spell.");
				var category = command.SafeRemainingArgument;
				if (string.IsNullOrWhiteSpace(category)) throw new FormatException("Supply a category.");
				next = Policy with { Choices = Policy.Choices.Where(x => x.SpellId != spell.Id).Append(new(spell.Id, category)).ToArray() };
			}
			if (field.EndsWith("fractions", StringComparison.Ordinal))
			{
				var values = value.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(x => double.Parse(x, NumberStyles.Float, actor)).ToArray();
				if (values.Length != 3 || values.Any(x => !double.IsFinite(x) || x is < 0 or > 1)) throw new FormatException("Supply health, stun and stamina fractions, each from 0 to 1.");
				ItemConsequence Fractions(ItemConsequence c) => c with { HealthFraction = values[0], StunFraction = values[1], StaminaFraction = values[2] };
				next = field switch
				{
					"activationfractions" => Policy with { Activation = Fractions(Policy.Activation) },
					"removalfractions" => Policy with { Removal = Fractions(Policy.Removal) },
					"periodicfractions" => Policy with { Periodic = Fractions(Policy.Periodic) },
					"incompatiblewearfractions" => Policy with { IncompatibleWear = Fractions(Policy.IncompatibleWear) },
					_ => null
				};
			}
			if (field == "unchoice") next = Policy with { Choices = Policy.Choices.Where(x => x.SpellId != Id()).ToArray() };
			if (field == "issue") { var id = Id(); next = Policy with { IssuedPrototypeIds = Policy.IssuedPrototypeIds.Contains(id) ? Policy.IssuedPrototypeIds.Where(x => x != id).ToArray() : Policy.IssuedPrototypeIds.Append(id).ToArray() }; }
			if (next is null) return base.BuildingCommand(actor, command.GetUndo());
			Policy = next;
			Changed = true;
			actor.Send($"{field.ColourName()} updated. {WhyCannotSubmit()}");
			return true;
		}
		catch (Exception ex) when (ex is FormatException or OverflowException or ArgumentException)
		{
			actor.Send($"Invalid setting: {ex.Message}\n{Help}");
			return false;
		}
	}
}
