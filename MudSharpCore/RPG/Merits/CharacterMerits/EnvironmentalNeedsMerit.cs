#nullable enable

using System;
using System.Collections.Generic;
using System.Globalization;
using CultureInfo = System.Globalization.CultureInfo;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Models;
using MudSharp.RPG.Merits.Interfaces;

namespace MudSharp.RPG.Merits.CharacterMerits;

/// <summary>Opt-in environmental contributions; ordinary needs and explicit fulfilment stay separate.</summary>
public sealed class EnvironmentalNeedsMerit : CharacterMeritBase, IEnvironmentalNeedsMerit
{
	public const string MeritTypeName = "Environmental Needs";
	private readonly Dictionary<string, string> _configurationWarnings = new(StringComparer.Ordinal);
	private readonly Dictionary<string, string> _invalidProgReferences = new(StringComparer.Ordinal);
	public long HungerProgId { get; private set; }
	public long ThirstProgId { get; private set; }
	public long WaterProgId { get; private set; }
	public double? MaximumHungerRate { get; private set; }
	public double? MaximumThirstRate { get; private set; }
	public IReadOnlyCollection<string> ConfigurationWarnings => _configurationWarnings.Values.ToArray();

	private EnvironmentalNeedsMerit() { }

	private EnvironmentalNeedsMerit(Merit merit, IFuturemud gameworld) : base(merit, gameworld)
	{
		var definition = XElement.Parse(merit.Definition);
		HungerProgId = ReadProgId(definition, "HungerProg");
		ThirstProgId = ReadProgId(definition, "ThirstProg");
		WaterProgId = ReadProgId(definition, "WaterProg");
		MaximumHungerRate = ReadCap(definition, "MaximumHungerRate");
		MaximumThirstRate = ReadCap(definition, "MaximumThirstRate");
		ResolveProg(HungerProgId, "HungerProg");
		ResolveProg(ThirstProgId, "ThirstProg");
		ResolveProg(WaterProgId, "WaterProg");
	}

	private EnvironmentalNeedsMerit(IFuturemud gameworld, string name)
		: base(gameworld, name, MeritTypeName, "@ have|has environmentally adapted needs")
	{
		MeritScope = MeritScope.Body;
		DoDatabaseInsert();
	}

	public static void RegisterMeritInitialiser()
	{
		MeritFactory.RegisterMeritInitialiser(MeritTypeName, (merit, world) => new EnvironmentalNeedsMerit(merit, world));
		MeritFactory.RegisterBuilderMeritInitialiser(MeritTypeName, (world, name) => new EnvironmentalNeedsMerit(world, name));
		MeritFactory.RegisterMeritHelp(MeritTypeName, "Adds configured environmental needs without changing explicit food or water fulfilment", new EnvironmentalNeedsMerit().HelpText);
	}

	private long ReadProgId(XElement definition, string field)
	{
		var text = (string?)definition.Element(field);
		if (text is null) return 0;
		if (long.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var id) && id >= 0) return id;
		_configurationWarnings[field] = $"{field} must contain a nonnegative numeric prog ID. The channel is inactive; set it to a number(character) prog or none.";
		_invalidProgReferences[field] = text;
		return 0;
	}

	private double? ReadCap(XElement definition, string field)
	{
		var text = (string?)definition.Element(field);
		if (text is null) return null;
		if (double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out var value) &&
			double.IsFinite(value) && value >= 0.0) return value;
		_configurationWarnings[field] = $"{field} must be finite and nonnegative. The cap is inactive; set a valid maximum or none.";
		return null;
	}

	private IFutureProg? ResolveProg(long id, string channel)
	{
		if (id == 0) return null;
		var prog = Gameworld.FutureProgs.Get(id);
		if (prog is null || prog.ReturnType != ProgVariableTypes.Number || !prog.MatchesParameters([ProgVariableTypes.Character]))
		{
			_configurationWarnings[channel] = $"{channel} references missing or incompatible prog #{id}. It must return number and accept character; this channel contributes zero.";
			return null;
		}
		_configurationWarnings.Remove(channel);
		return prog;
	}

	public EnvironmentalNeedsAdjustment GetNeedsAdjustment(ICharacter character)
	{
		var evaluated = new Dictionary<long, double>();
		var failed = new HashSet<long>();
		double Evaluate(long id, string channel)
		{
			if (id == 0) return 0.0;
			if (evaluated.TryGetValue(id, out var cached)) return cached;
			var prog = ResolveProg(id, channel);
			if (prog is null)
			{
				failed.Add(id);
				return evaluated[id] = 0.0;
			}
			try
			{
				// Null/failed execution must be distinguishable from a valid numeric zero.
				var value = prog.ExecuteDouble(double.NaN, character);
				if (double.IsFinite(value)) return evaluated[id] = value;
				_configurationWarnings[channel] = $"{channel} prog #{id} returned a non-finite value. It contributes zero; correct the prog's numeric result.";
			}
			catch (Exception ex) when (ex is ArithmeticException or InvalidCastException or FormatException or InvalidOperationException or ArgumentException)
			{
				_configurationWarnings[channel] = $"{channel} prog #{id} could not produce a numeric result ({ex.GetType().Name}). It contributes zero; correct the prog.";
			}
			failed.Add(id);
			return evaluated[id] = 0.0;
		}

		var hunger = Evaluate(HungerProgId, "HungerProg");
		var thirst = Evaluate(ThirstProgId, "ThirstProg");
		var water = Evaluate(WaterProgId, "WaterProg");
		return new EnvironmentalNeedsAdjustment(hunger, thirst, water,
			failed.Contains(HungerProgId) || _configurationWarnings.ContainsKey("HungerProg") ? null : MaximumHungerRate,
			failed.Contains(ThirstProgId) || _configurationWarnings.ContainsKey("ThirstProg") ? null : MaximumThirstRate);
	}

	protected override XElement SaveSubtypeDefinition(XElement root)
	{
		object Reference(string field, long id) => _invalidProgReferences.TryGetValue(field, out var text) ? text : id;
		root.Add(new XElement("HungerProg", Reference("HungerProg", HungerProgId)),
			new XElement("ThirstProg", Reference("ThirstProg", ThirstProgId)),
			new XElement("WaterProg", Reference("WaterProg", WaterProgId)));
		if (MaximumHungerRate is { } hunger) root.Add(new XElement("MaximumHungerRate", hunger.ToString("R", CultureInfo.InvariantCulture)));
		if (MaximumThirstRate is { } thirst) root.Add(new XElement("MaximumThirstRate", thirst.ToString("R", CultureInfo.InvariantCulture)));
		return root;
	}

	protected override void SubtypeShow(ICharacter actor, StringBuilder sb)
	{
		string DescribeProg(long id) => id == 0 ? "None (zero contribution)" :
			Gameworld.FutureProgs.Get(id)?.MXPClickableFunctionNameWithId() ?? $"Missing prog #{id}".ColourError();
		sb.AppendLine($"Hunger Rate Delta Prog: {DescribeProg(HungerProgId)}");
		sb.AppendLine($"Thirst Rate Delta Prog: {DescribeProg(ThirstProgId)}");
		sb.AppendLine($"Water Loss Prog (litres/game hour): {DescribeProg(WaterProgId)}");
		sb.AppendLine($"Maximum Hunger Rate: {(MaximumHungerRate?.ToString("N2", actor) ?? "Uncapped").ColourValue()}");
		sb.AppendLine($"Maximum Thirst Rate: {(MaximumThirstRate?.ToString("N2", actor) ?? "Uncapped").ColourValue()}");
		foreach (var warning in _configurationWarnings.Values) sb.AppendLine(warning.ColourError());
	}

	protected override string SubtypeHelp => $@"{base.SubtypeHelp}
	#3hungerprog <prog|none>#0 - sets the number(character) hunger-rate delta
	#3thirstprog <prog|none>#0 - sets the number(character) thirst-rate delta
	#3waterprog <prog|none>#0 - sets water-loss delta in litres per game hour
	#3maxhunger <number|none>#0 - caps the combined baseline/environment hunger rate
	#3maxthirst <number|none>#0 - caps the combined baseline/environment thirst rate

Environmental progs may return negative credits. Consumers clamp net losses at zero; this does not create food/water or change explicit sweat and drinking.";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		var option = command.PopForSwitch();
		switch (option)
		{
			case "hungerprog":
			case "thirstprog":
			case "waterprog":
				return BuildingCommandProg(actor, command, option);
			case "maxhunger":
			case "maxthirst":
				return BuildingCommandCap(actor, command, option);
			default:
				return base.BuildingCommand(actor, command.GetUndo());
		}
	}

	private bool BuildingCommandProg(ICharacter actor, StringStack command, string option)
	{
		var text = command.SafeRemainingArgument;
		long id;
		if (text.EqualTo("none")) id = 0;
		else
		{
			var prog = new ProgLookupFromBuilderInput(actor, text, ProgVariableTypes.Number, [ProgVariableTypes.Character]).LookupProg();
			if (prog is null) return false;
			id = prog.Id;
		}
		var channel = option switch { "hungerprog" => "HungerProg", "thirstprog" => "ThirstProg", _ => "WaterProg" };
		switch (option)
		{
			case "hungerprog": HungerProgId = id; break;
			case "thirstprog": ThirstProgId = id; break;
			default: WaterProgId = id; break;
		}
		_configurationWarnings.Remove(channel);
		_invalidProgReferences.Remove(channel);
		Changed = true;
		actor.OutputHandler.Send(id == 0 ? $"{channel} now contributes zero." : $"{channel} now uses prog #{id}. It must return a finite rate delta.");
		return true;
	}

	private bool BuildingCommandCap(ICharacter actor, StringStack command, string option)
	{
		var text = command.SafeRemainingArgument;
		double? cap = null;
		if (!text.EqualTo("none"))
		{
			if (!double.TryParse(text, NumberStyles.Float, actor.Account.Culture, out var value) || !double.IsFinite(value) || value < 0.0)
			{
				actor.OutputHandler.Send("Enter a finite nonnegative maximum rate, or none to remove the cap.");
				return false;
			}
			cap = value;
		}
		var channel = option == "maxhunger" ? "MaximumHungerRate" : "MaximumThirstRate";
		if (option == "maxhunger") MaximumHungerRate = cap;
		else MaximumThirstRate = cap;
		_configurationWarnings.Remove(channel);
		Changed = true;
		actor.OutputHandler.Send(cap is { } maximum ? $"{channel} is now {maximum.ToString("N2", actor).ColourValue()}." : $"{channel} is now uncapped.");
		return true;
	}
}
