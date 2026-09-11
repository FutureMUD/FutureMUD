using System.Globalization;
using MudSharp.Database;
using MudSharp.Framework.Save;
using MudSharp.Health;

#nullable enable
namespace MudSharp.Magic;

public sealed class MagicalSubstance : SaveableItem, IMagicalSubstance
{
	private readonly List<SubstanceBinding> _bindings = new();
	private readonly List<SubstanceEffectEntry> _entries = new();
	public override string FrameworkItemType => "MagicalSubstance";
	public DrugVector Vectors { get; private set; } = DrugVector.Ingested;
	public SpellPower Power { get; private set; } = SpellPower.Standard;
	public double ReferenceDose { get; private set; } = 1.0;
	public double ClearancePerTick { get; private set; } = 0.001;
	public IEnumerable<SubstanceBinding> Bindings => _bindings;
	public IEnumerable<SubstanceEffectEntry> Entries => _entries;
	public MagicalSubstance(Models.MagicalSubstance model, IFuturemud world)
	{
		Gameworld = world;
		_id = model.Id;
		_name = model.Name;
		var root = XElement.Parse(model.Definition);
		Vectors = (DrugVector)(int)root.Attribute("vectors")!;
		Power = (SpellPower)(int)root.Attribute("power")!;
		ReferenceDose = (double)root.Attribute("reference")!;
		ClearancePerTick = (double)root.Attribute("clearance")!;
		foreach (var binding in root.Elements("Binding"))
			_bindings.Add(new((SubstanceCarrier)(int)binding.Attribute("carrier")!, (long)binding.Attribute("id")!, (double)binding.Attribute("quantity")!));
		foreach (var entry in root.Elements("Entry")) _entries.Add(LoadEntry(entry));
	}
	public MagicalSubstance(string name, IFuturemud world, MagicalSubstance? source = null)
	{
		Gameworld = world;
		_name = name;
		if (source is not null)
		{
			Vectors = source.Vectors; Power = source.Power; ReferenceDose = source.ReferenceDose; ClearancePerTick = source.ClearancePerTick;
			_bindings.AddRange(source.Bindings);
			foreach (var entry in source.Entries)
			{
				var xml = SaveEntry(entry); xml.SetAttributeValue("key", Guid.NewGuid()); _entries.Add(LoadEntry(xml));
			}
		}
		using (new FMDB())
		{
			var model = new Models.MagicalSubstance { Name = name, Definition = Definition().ToString() };
			FMDB.Context.MagicalSubstances.Add(model); FMDB.Context.SaveChanges(); _id = model.Id;
		}
	}
	public static XElement SaveEntry(SubstanceEffectEntry x) => new("Entry",
		new XAttribute("key", x.Key), new XAttribute("spell", x.SpellId), new XAttribute("lifecycle", (int)x.Lifecycle),
		new XAttribute("pulse", (int)x.PulseMode), new XAttribute("stack", (int)x.Stacking), new XAttribute("scale", (int)x.Scaling),
		new XAttribute("minimum", x.MinimumDose), new XAttribute("maximum", x.MaximumDose), new XAttribute("duration", x.DurationSeconds),
		new XAttribute("durationcap", x.MaximumDurationSeconds), new XAttribute("interval", x.IntervalSeconds));
	public static SubstanceEffectEntry LoadEntry(XElement x) => new()
	{
		Key = Guid.Parse(x.Attribute("key")!.Value), SpellId = (long)x.Attribute("spell")!, Lifecycle = (SubstanceLifecycle)(int)x.Attribute("lifecycle")!,
		PulseMode = (SubstancePulseMode)(int)x.Attribute("pulse")!, Stacking = (SubstanceStacking)(int)x.Attribute("stack")!, Scaling = (SubstanceScaling)(int)x.Attribute("scale")!,
		MinimumDose = (double)x.Attribute("minimum")!, MaximumDose = (double)x.Attribute("maximum")!, DurationSeconds = (double)x.Attribute("duration")!,
		MaximumDurationSeconds = (double)x.Attribute("durationcap")!, IntervalSeconds = (double)x.Attribute("interval")!
	};
	private XElement Definition() => new("Substance", new XAttribute("vectors", (int)Vectors), new XAttribute("power", (int)Power),
		new XAttribute("reference", ReferenceDose), new XAttribute("clearance", ClearancePerTick),
		_bindings.Select(x => new XElement("Binding", new XAttribute("carrier", (int)x.Carrier), new XAttribute("id", x.Id), new XAttribute("quantity", x.QuantityPerUnit))), _entries.Select(SaveEntry));
	public override void Save()
	{
		var model = FMDB.Context.MagicalSubstances.Find(Id)!;
		model.Name = Name; model.Definition = Definition().ToString(); Changed = false;
	}
	public IEnumerable<string> ReadinessErrors
	{
		get
		{
			if (!Enum.IsDefined(Power) || Vectors == DrugVector.None || ((int)Vectors & ~15) != 0) yield return "Choose valid power and exposure vectors.";
			if (!_entries.Any()) yield return "No effect entries.";
			if (!SubstanceDose.IsPositive(ReferenceDose) || !SubstanceDose.IsPositive(ClearancePerTick)) yield return "Reference dose and clearance must be positive and finite.";
			foreach (var binding in _bindings)
			{
				if (!SubstanceDose.IsPositive(binding.QuantityPerUnit) || !Enum.IsDefined(binding.Carrier)) yield return "Bindings require a valid carrier and positive finite quantity.";
				var exists = binding.Carrier switch { SubstanceCarrier.Liquid => Gameworld.Liquids.Get(binding.Id) is not null,
					SubstanceCarrier.Gas => Gameworld.Gases.Get(binding.Id) is not null, _ => Gameworld.ItemProtos.Any(x => x.Id == binding.Id) };
				if (!exists) yield return $"Missing {binding.Carrier} #{binding.Id}.";
			}
			foreach (var entry in _entries)
				foreach (var error in SubstanceSpellResolver.Errors(Gameworld.MagicSpells.Get(entry.SpellId), entry)) yield return $"Entry {_entries.IndexOf(entry) + 1}: {error}";
		}
	}
	public const string Help = @"You can use the following options with this command:

	#3name <name>#0 - renames this magical substance
	#3vectors <ingested|injected|touched|inhaled> [...]#0 - sets the exposure routes
	#3power <0-10>#0 - sets the authored spell power (5 is Standard)
	#3reference <quantity>#0 - sets the quantity representing one full dose
	#3clearance <quantity>#0 - sets the quantity cleared every ten seconds
	#3bind <liquid|gas|item> <id> <quantity>#0 - sets quantity per engine fluid unit or whole item
	#3unbind <liquid|gas|item> <id>#0 - removes a carrier binding
	#3add <spell id> <activation|maintained|periodic>#0 - adds a payload entry
	#3remove <entry>#0 - removes a payload entry
	#3check#0 - reports missing references and incompatible payloads

The following options configure an existing payload entry:

	#3entry <number> stack <aggregate|replace|strongest|independent>#0 - sets the stacking policy
	#3entry <number> scale <duration|magnitude>#0 - sets how dose scales the payload
	#3entry <number> pulse <presence|timed>#0 - sets the periodic lifetime mode
	#3entry <number> minimum <dose>#0 - sets the minimum dose required in one action
	#3entry <number> maximum <dose>#0 - caps the effective dose
	#3entry <number> duration <seconds>#0 - sets the reference lifetime
	#3entry <number> durationcap <seconds>#0 - caps the accumulated lifetime
	#3entry <number> interval <seconds>#0 - sets the pulse interval (at least one second)

Author the payload spell effects with #3magic spell#0.";

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "name":
				return BuildingCommandName(actor, command);
			case "vectors":
				return BuildingCommandVectors(actor, command);
			case "power":
				return BuildingCommandPower(actor, command);
			case "reference":
				return BuildingCommandReference(actor, command);
			case "clearance":
				return BuildingCommandClearance(actor, command);
			case "bind":
				return BuildingCommandBind(actor, command);
			case "unbind":
				return BuildingCommandUnbind(actor, command);
			case "add":
				return BuildingCommandAdd(actor, command);
			case "remove":
				return BuildingCommandRemove(actor, command);
			case "entry":
				return BuildingCommandEntry(actor, command);
			case "check":
				return BuildingCommandCheck(actor);
			default:
				actor.OutputHandler.Send(Help.SubstituteANSIColour());
				return false;
		}
	}

	private bool BuildingCommandCheck(ICharacter actor)
	{
		var errors = ReadinessErrors.ToList();
		actor.OutputHandler.Send(errors.Count > 0 ? errors.Select(x => x.ColourError()).ListToLines() : "Ready for exposure.".ColourValue());
		return false;
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		var name = command.SafeRemainingArgument;
		if (string.IsNullOrWhiteSpace(name) || name.Length > 200 || Gameworld.MagicalSubstances.Any(x => x != this && x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send("Choose a unique name of 1 to 200 characters.");
			return false;
		}

		_name = name;
		Changed = true;
		actor.OutputHandler.Send($"This magical substance is now called {Name.ColourName()}.");
		return true;
	}

	private bool BuildingCommandVectors(ICharacter actor, StringStack command)
	{
		var vectors = DrugVector.None;
		while (!command.IsFinished)
		{
			if (!Enum.TryParse<DrugVector>(command.PopSpeech(), true, out var vector) || !Enum.IsDefined(vector) || vector == DrugVector.None)
			{
				actor.OutputHandler.Send("Choose exposure routes from ingested, injected, touched and inhaled.");
				return false;
			}

			vectors |= vector;
		}

		if (vectors == DrugVector.None)
		{
			actor.OutputHandler.Send("Which exposure routes should this magical substance use: ingested, injected, touched or inhaled?");
			return false;
		}

		Vectors = vectors;
		Changed = true;
		actor.OutputHandler.Send($"This magical substance now uses the {Vectors.DescribeEnum().ColourName()} exposure routes.");
		return true;
	}

	private bool BuildingCommandPower(ICharacter actor, StringStack command)
	{
		if (!int.TryParse(command.SafeRemainingArgument, out var power) || power is < 0 or > 10)
		{
			actor.OutputHandler.Send("What spell power should this magical substance use? Enter a number from 0 to 10 (5 is Standard).");
			return false;
		}

		Power = (SpellPower)power;
		Changed = true;
		actor.OutputHandler.Send($"This magical substance now uses {Power.DescribeEnum().ColourName()} spell power.");
		return true;
	}

	private bool BuildingCommandReference(ICharacter actor, StringStack command)
	{
		if (!Number(command.SafeRemainingArgument, actor, out var quantity))
		{
			actor.OutputHandler.Send("Enter a positive, finite quantity for one full dose.");
			return false;
		}

		ReferenceDose = quantity;
		Changed = true;
		actor.OutputHandler.Send($"The reference dose is now {quantity.ToString("N6", actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandClearance(ICharacter actor, StringStack command)
	{
		if (!Number(command.SafeRemainingArgument, actor, out var quantity))
		{
			actor.OutputHandler.Send("Enter a positive, finite quantity cleared every ten seconds.");
			return false;
		}

		ClearancePerTick = quantity;
		Changed = true;
		actor.OutputHandler.Send($"The clearance per ten seconds is now {quantity.ToString("N6", actor).ColourValue()}.");
		return true;
	}

	private static bool TryGetBinding(ICharacter actor, StringStack command, out SubstanceCarrier carrier, out long id)
	{
		id = 0;
		if (Enum.TryParse(command.PopSpeech(), true, out carrier) && Enum.IsDefined(carrier) &&
			long.TryParse(command.PopSpeech(), out id) && id > 0) return true;

		actor.OutputHandler.Send("Specify a carrier type (liquid, gas or item) followed by its positive ID.");
		return false;
	}

	private bool BuildingCommandBind(ICharacter actor, StringStack command)
	{
		if (!TryGetBinding(actor, command, out var carrier, out var id)) return false;
		if (!Number(command.SafeRemainingArgument, actor, out var quantity))
		{
			actor.OutputHandler.Send("Enter a positive, finite quantity per engine fluid unit or whole item.");
			return false;
		}

		_bindings.RemoveAll(x => x.Carrier == carrier && x.Id == id);
		_bindings.Add(new(carrier, id, quantity));
		Changed = true;
		actor.OutputHandler.Send($"This magical substance now binds to {carrier.DescribeEnum().ColourName()} #{id.ToString("N0", actor).ColourValue()} at {quantity.ToString("N6", actor).ColourValue()} quantity per unit.");
		return true;
	}

	private bool BuildingCommandUnbind(ICharacter actor, StringStack command)
	{
		if (!TryGetBinding(actor, command, out var carrier, out var id)) return false;
		_bindings.RemoveAll(x => x.Carrier == carrier && x.Id == id);
		Changed = true;
		actor.OutputHandler.Send($"This magical substance no longer binds to {carrier.DescribeEnum().ColourName()} #{id.ToString("N0", actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandAdd(ICharacter actor, StringStack command)
	{
		if (!long.TryParse(command.PopSpeech(), out var id) || Gameworld.MagicSpells.Get(id) is not { } spell)
		{
			actor.OutputHandler.Send("Which spell should be added? Specify an existing spell ID.");
			return false;
		}

		if (!Enum.TryParse<SubstanceLifecycle>(command.PopSpeech(), true, out var lifecycle) || !Enum.IsDefined(lifecycle))
		{
			actor.OutputHandler.Send("Choose an activation, maintained or periodic lifecycle for this entry.");
			return false;
		}

		_entries.Add(new() { SpellId = id, Lifecycle = lifecycle });
		Changed = true;
		actor.OutputHandler.Send($"Entry {_entries.Count.ToString("N0", actor).ColourValue()} now applies {spell.Name.ColourName()} with the {lifecycle.DescribeEnum().ColourName()} lifecycle.");
		return true;
	}

	private SubstanceEffectEntry? GetBuildingEntry(ICharacter actor, StringStack command)
	{
		if (int.TryParse(command.PopSpeech(), out var index) && index >= 1 && index <= _entries.Count) return _entries[index - 1];
		actor.OutputHandler.Send("Specify an entry number from this magical substance's show output.");
		return null;
	}

	private bool BuildingCommandRemove(ICharacter actor, StringStack command)
	{
		var entry = GetBuildingEntry(actor, command);
		if (entry is null) return false;
		var index = _entries.IndexOf(entry) + 1;
		_entries.Remove(entry);
		Changed = true;
		actor.OutputHandler.Send($"You remove entry {index.ToString("N0", actor).ColourValue()} from this magical substance.");
		return true;
	}

	private bool BuildingCommandEntry(ICharacter actor, StringStack command)
	{
		var entry = GetBuildingEntry(actor, command);
		if (entry is null) return false;
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "stack":
				return BuildingCommandEntryStack(actor, command, entry);
			case "scale":
				return BuildingCommandEntryScale(actor, command, entry);
			case "pulse":
				return BuildingCommandEntryPulse(actor, command, entry);
			case "minimum":
				return BuildingCommandEntryMinimum(actor, command, entry);
			case "maximum":
				return BuildingCommandEntryMaximum(actor, command, entry);
			case "duration":
				return BuildingCommandEntryDuration(actor, command, entry);
			case "durationcap":
				return BuildingCommandEntryDurationCap(actor, command, entry);
			case "interval":
				return BuildingCommandEntryInterval(actor, command, entry);
			default:
				actor.OutputHandler.Send(Help.SubstituteANSIColour());
				return false;
		}
	}

	private bool BuildingCommandEntryStack(ICharacter actor, StringStack command, SubstanceEffectEntry entry)
	{
		if (!Enum.TryParse<SubstanceStacking>(command.SafeRemainingArgument, true, out var value) || !Enum.IsDefined(value))
		{
			actor.OutputHandler.Send("Choose aggregate, replace, strongest or independent for this entry's stacking policy.");
			return false;
		}

		entry.Stacking = value;
		Changed = true;
		actor.OutputHandler.Send($"This entry's stacking policy is now {value.DescribeEnum().ColourName()}.");
		return true;
	}

	private bool BuildingCommandEntryScale(ICharacter actor, StringStack command, SubstanceEffectEntry entry)
	{
		if (!Enum.TryParse<SubstanceScaling>(command.SafeRemainingArgument, true, out var value) || !Enum.IsDefined(value))
		{
			actor.OutputHandler.Send("Choose duration or magnitude for this entry's dose scaling.");
			return false;
		}

		entry.Scaling = value;
		Changed = true;
		actor.OutputHandler.Send($"This entry's dose scaling is now {value.DescribeEnum().ColourName()}.");
		return true;
	}

	private bool BuildingCommandEntryPulse(ICharacter actor, StringStack command, SubstanceEffectEntry entry)
	{
		if (!Enum.TryParse<SubstancePulseMode>(command.SafeRemainingArgument, true, out var value) || !Enum.IsDefined(value))
		{
			actor.OutputHandler.Send("Choose presence or timed for this entry's pulse mode.");
			return false;
		}

		entry.PulseMode = value;
		Changed = true;
		actor.OutputHandler.Send($"This entry's pulse mode is now {value.DescribeEnum().ColourName()}.");
		return true;
	}

	private bool BuildingCommandEntryMinimum(ICharacter actor, StringStack command, SubstanceEffectEntry entry)
	{
		if (!double.TryParse(command.SafeRemainingArgument, NumberStyles.Float, actor, out var value) || !double.IsFinite(value) || value < 0)
		{
			actor.OutputHandler.Send("Enter a finite minimum dose that is zero or greater.");
			return false;
		}

		entry.MinimumDose = value;
		Changed = true;
		actor.OutputHandler.Send($"This entry's minimum dose is now {value.ToString("N3", actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandEntryMaximum(ICharacter actor, StringStack command, SubstanceEffectEntry entry)
	{
		if (!double.TryParse(command.SafeRemainingArgument, NumberStyles.Float, actor, out var value) || !double.IsFinite(value) || value <= 0)
		{
			actor.OutputHandler.Send("Enter a finite maximum dose that is greater than zero.");
			return false;
		}

		entry.MaximumDose = value;
		Changed = true;
		actor.OutputHandler.Send($"This entry's maximum dose is now {value.ToString("N3", actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandEntryDuration(ICharacter actor, StringStack command, SubstanceEffectEntry entry)
	{
		if (!double.TryParse(command.SafeRemainingArgument, NumberStyles.Float, actor, out var value) || !double.IsFinite(value) || value <= 0)
		{
			actor.OutputHandler.Send("Enter a finite reference duration seconds that is greater than zero.");
			return false;
		}

		entry.DurationSeconds = value;
		Changed = true;
		actor.OutputHandler.Send($"This entry's reference duration is now {value.ToString("N3", actor).ColourValue()} seconds.");
		return true;
	}

	private bool BuildingCommandEntryDurationCap(ICharacter actor, StringStack command, SubstanceEffectEntry entry)
	{
		if (!double.TryParse(command.SafeRemainingArgument, NumberStyles.Float, actor, out var value) || !double.IsFinite(value) || value <= 0)
		{
			actor.OutputHandler.Send("Enter a finite duration cap seconds that is greater than zero.");
			return false;
		}

		entry.MaximumDurationSeconds = value;
		Changed = true;
		actor.OutputHandler.Send($"This entry's duration cap is now {value.ToString("N3", actor).ColourValue()} seconds.");
		return true;
	}

	private bool BuildingCommandEntryInterval(ICharacter actor, StringStack command, SubstanceEffectEntry entry)
	{
		if (!double.TryParse(command.SafeRemainingArgument, NumberStyles.Float, actor, out var value) || !double.IsFinite(value) || value < 1)
		{
			actor.OutputHandler.Send("Enter a finite pulse interval seconds that is at least one.");
			return false;
		}

		entry.IntervalSeconds = value;
		Changed = true;
		actor.OutputHandler.Send($"This entry's pulse interval is now {value.ToString("N3", actor).ColourValue()} seconds.");
		return true;
	}

	private static bool Number(string text, ICharacter actor, out double value) =>
		double.TryParse(text, NumberStyles.Float, actor, out value) && SubstanceDose.IsPositive(value);

	public string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Magical Substance #{Id.ToString("N0", actor)} - {Name}".GetLineWithTitleInner(actor, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine($"Exposure Routes: {Vectors.DescribeEnum().ColourName()}");
		sb.AppendLine($"Spell Power: {Power.DescribeEnum().ColourName()}");
		sb.AppendLine($"Reference Dose: {ReferenceDose.ToString("N3", actor).ColourValue()}");
		sb.AppendLine($"Clearance per Ten Seconds: {ClearancePerTick.ToString("N6", actor).ColourValue()}");
		sb.AppendLine();
		sb.AppendLine("Carrier Bindings".GetLineWithTitleInner(actor, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine();
		if (_bindings.Count == 0) sb.AppendLine("None".ColourError());
		foreach (var binding in _bindings)
		{
			sb.AppendLine($"Carrier: {binding.Carrier.DescribeEnum().ColourName()} #{binding.Id.ToString("N0", actor).ColourValue()}");
			sb.AppendLine($"Quantity per Unit: {binding.QuantityPerUnit.ToString("N6", actor).ColourValue()}");
			sb.AppendLine();
		}

		if (_entries.Count == 0)
		{
			sb.AppendLine();
			sb.AppendLine("Payload Entries".GetLineWithTitleInner(actor, Telnet.Cyan, Telnet.BoldWhite));
			sb.AppendLine();
			sb.AppendLine("None".ColourError());
		}

		foreach (var (entry, index) in _entries.Select((entry, index) => (entry, index + 1)))
		{
			sb.AppendLine($"Payload Entry {index.ToString("N0", actor)}".GetLineWithTitleInner(actor, Telnet.Cyan, Telnet.BoldWhite));
			sb.AppendLine();
			sb.AppendLine($"Spell: {Gameworld.MagicSpells.Get(entry.SpellId)?.Name.ColourName() ?? "Missing".ColourError()} (#{entry.SpellId.ToString("N0", actor).ColourValue()})");
			sb.AppendLine($"Lifecycle: {entry.Lifecycle.DescribeEnum().ColourName()}");
			sb.AppendLine($"Stacking: {entry.Stacking.DescribeEnum().ColourName()}");
			sb.AppendLine($"Dose Scaling: {entry.Scaling.DescribeEnum().ColourName()}");
			sb.AppendLine($"Pulse Mode: {entry.PulseMode.DescribeEnum().ColourName()}");
			sb.AppendLine($"Minimum Dose: {entry.MinimumDose.ToString("N3", actor).ColourValue()}");
			sb.AppendLine($"Maximum Dose: {entry.MaximumDose.ToString("N3", actor).ColourValue()}");
			sb.AppendLine($"Reference Duration: {entry.DurationSeconds.ToString("N3", actor).ColourValue()} seconds");
			sb.AppendLine($"Duration Cap: {entry.MaximumDurationSeconds.ToString("N3", actor).ColourValue()} seconds");
			sb.AppendLine($"Pulse Interval: {entry.IntervalSeconds.ToString("N3", actor).ColourValue()} seconds");
			sb.AppendLine();
		}

		sb.AppendLine("Readiness".GetLineWithTitleInner(actor, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine();
		var errors = ReadinessErrors.ToList();
		sb.AppendLine($"Ready for Exposure: {(errors.Count == 0).ToColouredString()}");
		foreach (var error in errors) sb.AppendLine($"Building Error: {error.ColourError()}");
		return sb.ToString();
	}
}
