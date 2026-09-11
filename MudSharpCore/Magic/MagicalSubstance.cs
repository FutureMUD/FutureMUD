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
	public const string Help = @"Magical substance settings:
	#3name <name>#0; #3vectors <ingested injected touched inhaled>#0; #3power <0-10>#0
	#3reference <quantity>#0; #3clearance <quantity per ten seconds>#0
	#3bind <liquid|gas|item> <id> <quantity per engine fluid unit or whole item>#0
	#3unbind <liquid|gas|item> <id>#0
	#3add <spell id> <activation|maintained|periodic>#0; #3remove <entry>#0
	#3entry <number> <stack|scale|pulse> <value>#0
	#3entry <number> <minimum|maximum|duration|durationcap|interval> <number>#0
	Stack: aggregate, replace, strongest, independent. Scale: duration, magnitude. Pulse: presence, timed.
	#3check#0 reports readiness. Spell effects are authored through magic spell.";
	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		var verb = command.PopSpeech().ToLowerInvariant();
		if (verb == "check") { actor.Send(ReadinessErrors.Any() ? ReadinessErrors.ListToLines() : "Ready for exposure."); return false; }
		if (verb == "name" && !string.IsNullOrWhiteSpace(command.SafeRemainingArgument))
		{
			var name = command.SafeRemainingArgument;
			if (name.Length > 200 || Gameworld.MagicalSubstances.Any(x => x != this && x.Name.EqualTo(name))) { actor.Send("Choose a unique name of at most 200 characters."); return false; }
			_name = name;
		}
		else if (verb == "vectors")
		{
			var vectors = DrugVector.None;
			while (!command.IsFinished)
			{
				if (!Enum.TryParse<DrugVector>(command.PopSpeech(), true, out var vector) || !Enum.IsDefined(vector) || vector == DrugVector.None) return Invalid(actor);
				vectors |= vector;
			}
			if (vectors == DrugVector.None) return Invalid(actor);
			Vectors = vectors;
		}
		else if (verb == "power" && int.TryParse(command.SafeRemainingArgument, out var power) && power is >= 0 and <= 10) Power = (SpellPower)power;
		else if (verb is "reference" or "clearance" && Number(command.SafeRemainingArgument, actor, out var number))
		{ if (verb == "reference") ReferenceDose = number; else ClearancePerTick = number; }
		else if (verb is "bind" or "unbind")
		{
			if (!Enum.TryParse<SubstanceCarrier>(command.PopSpeech(), true, out var carrier) || !Enum.IsDefined(carrier) || !long.TryParse(command.PopSpeech(), out var id) || id <= 0) return Invalid(actor);
			var quantity = 0.0;
			if (verb == "bind" && !Number(command.SafeRemainingArgument, actor, out quantity)) return Invalid(actor);
			_bindings.RemoveAll(x => x.Carrier == carrier && x.Id == id);
			if (verb == "bind") _bindings.Add(new(carrier, id, quantity));
		}
		else if (verb == "add")
		{
			if (!long.TryParse(command.PopSpeech(), out var id) || Gameworld.MagicSpells.Get(id) is null || !Enum.TryParse<SubstanceLifecycle>(command.PopSpeech(), true, out var lifecycle) || !Enum.IsDefined(lifecycle)) return Invalid(actor);
			_entries.Add(new() { SpellId = id, Lifecycle = lifecycle });
		}
		else if (verb is "entry" or "remove")
		{
			if (!int.TryParse(command.PopSpeech(), out var index) || index < 1 || index > _entries.Count) return Invalid(actor);
			var entry = _entries[index - 1];
			if (verb == "remove") _entries.Remove(entry);
			else
			{
				var setting = command.PopSpeech().ToLowerInvariant(); var value = command.SafeRemainingArgument;
				if (setting == "stack" && Enum.TryParse<SubstanceStacking>(value, true, out var stack) && Enum.IsDefined(stack)) entry.Stacking = stack;
				else if (setting == "scale" && Enum.TryParse<SubstanceScaling>(value, true, out var scale) && Enum.IsDefined(scale)) entry.Scaling = scale;
				else if (setting == "pulse" && Enum.TryParse<SubstancePulseMode>(value, true, out var pulse) && Enum.IsDefined(pulse)) entry.PulseMode = pulse;
				else if (double.TryParse(value, NumberStyles.Float, actor, out var n) && double.IsFinite(n) && n >= 0)
				{
					switch (setting)
					{
						case "minimum": entry.MinimumDose = n; break;
						case "maximum" when n > 0: entry.MaximumDose = n; break;
						case "duration" when n > 0: entry.DurationSeconds = n; break;
						case "durationcap" when n > 0: entry.MaximumDurationSeconds = n; break;
						case "interval" when n >= 1: entry.IntervalSeconds = n; break;
						default: return Invalid(actor);
					}
				}
				else return Invalid(actor);
			}
		}
		else return Invalid(actor);
		Changed = true; actor.Send("Magical substance updated.".ColourValue()); return true;
	}
	private static bool Number(string text, ICharacter actor, out double value) => double.TryParse(text, NumberStyles.Float, actor, out value) && SubstanceDose.IsPositive(value);
	private static bool Invalid(ICharacter actor) { actor.Send(Help.SubstituteANSIColour()); return false; }
	public string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Magical Substance #{Id.ToString("N0", actor)}: {Name.ColourName()}");
		sb.AppendLine($"Vectors: {Vectors.DescribeEnum()}; Power: {Power.DescribeEnum()}; Reference: {ReferenceDose.ToString("N3", actor)}; Clearance/tick: {ClearancePerTick.ToString("N6", actor)}");
		foreach (var b in _bindings) sb.AppendLine($"{b.Carrier.DescribeEnum()} #{b.Id.ToString("N0", actor)}: {b.QuantityPerUnit.ToString("N6", actor)} quantity/unit");
		foreach (var e in _entries) sb.AppendLine($"{(_entries.IndexOf(e) + 1).ToString(actor)}. Spell #{e.SpellId.ToString(actor)} {e.Lifecycle.DescribeEnum()}, {e.Stacking.DescribeEnum()}, {e.Scaling.DescribeEnum()}, {e.PulseMode.DescribeEnum()}; dose {e.MinimumDose.ToString(actor)}..{e.MaximumDose.ToString(actor)}, duration {e.DurationSeconds.ToString(actor)}s (cap {e.MaximumDurationSeconds.ToString(actor)}s), interval {e.IntervalSeconds.ToString(actor)}s");
		sb.AppendLine(ReadinessErrors.Any() ? ReadinessErrors.ListToLines() : "Ready for exposure."); return sb.ToString();
	}
}
