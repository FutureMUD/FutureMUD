#nullable enable

using ExpressionEngine;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.GameItems;
using MudSharp.Health;
using MudSharp.Movement;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.SpellEffects;

internal static class PersistentSensoryCombatSpellEffectTemplateHelper
{
	public static readonly string[] PerceivableTriggerTypes = SpellTriggerFactory.MagicTriggerTypes
		.Where(x => IsPerceivableTarget(SpellTriggerFactory.BuilderInfoForType(x).TargetTypes))
		.ToArray();

	public static readonly string[] CharacterTriggerTypes = SpellTriggerFactory.MagicTriggerTypes
		.Where(x => IsCharacterTarget(SpellTriggerFactory.BuilderInfoForType(x).TargetTypes))
		.ToArray();

	public static bool IsPerceivableTarget(string types)
	{
		return types is "character" or "characters" or "item" or "items" or "perceivable" or "perceivables";
	}

	public static bool IsCharacterTarget(string types)
	{
		return types is "character" or "characters";
	}
}

public class BurningEffect : IMagicSpellEffectTemplate
{
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("burning", (root, spell) => new BurningEffect(root, spell));
		SpellEffectFactory.RegisterLoadTimeFactory("ignite", (root, spell) => new BurningEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("burning", BuilderFactory,
			"Applies recurring spell-owned burning damage",
			HelpText,
			false,
			true,
			PersistentSensoryCombatSpellEffectTemplateHelper.PerceivableTriggerTypes);
		SpellEffectFactory.RegisterBuilderFactory("ignite", BuilderFactory,
			"Alias for burning",
			HelpText,
			false,
			true,
			PersistentSensoryCombatSpellEffectTemplateHelper.PerceivableTriggerTypes);
	}

	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands,
		IMagicSpell spell)
	{
		return (new BurningEffect(new XElement("Effect",
			new XAttribute("type", "burning"),
			new XElement("DamageType", (int)DamageType.Burning),
			new XElement("DamageFormula", new XCData("power")),
			new XElement("PainFormula", new XCData("power")),
			new XElement("StunFormula", new XCData("power * 0.5")),
			new XElement("ThermalFormula", new XCData("0")),
			new XElement("TickSeconds", 10.0),
			new XElement("MinimumOxidation", 0.1),
			new XElement("SelfOxidising", false),
			new XElement("SDescAddendum", new XCData("(burning)")),
			new XElement("DescAddendum", new XCData("@ is wreathed in magical flames.")),
			new XElement("AddendumColour", "bold red")
		), spell), string.Empty);
	}

	private BurningEffect(XElement root, IMagicSpell spell)
	{
		Spell = spell;
		DamageType = (DamageType)int.Parse(root.Element("DamageType")?.Value ?? ((int)DamageType.Burning).ToString());
		DamageFormula = new Expression(root.Element("DamageFormula")?.Value ?? "power");
		PainFormula = new Expression(root.Element("PainFormula")?.Value ?? "power");
		StunFormula = new Expression(root.Element("StunFormula")?.Value ?? "power * 0.5");
		ThermalFormula = new Expression(root.Element("ThermalFormula")?.Value ?? "0");
		TickSeconds = MagicBuilderValidation.ClampFinite(
			MagicBuilderValidation.ParseFiniteOrDefault(root.Element("TickSeconds")?.Value, 10.0),
			1.0, MagicBuilderValidation.MaximumSpellTickSeconds, 10.0);
		MinimumOxidation = MagicBuilderValidation.ClampFinite(
			MagicBuilderValidation.ParseFiniteOrDefault(root.Element("MinimumOxidation")?.Value, 0.1),
			0.0, double.MaxValue, 0.1);
		SelfOxidising = bool.Parse(root.Element("SelfOxidising")?.Value ?? "false");
		SDescAddendum = root.Element("SDescAddendum")?.Value ?? "(burning)";
		DescAddendum = root.Element("DescAddendum")?.Value ?? "@ is wreathed in magical flames.";
		AddendumColour = Telnet.GetColour(root.Element("AddendumColour")?.Value ?? "bold red") ?? Telnet.BoldRed;
	}

	public IMagicSpell Spell { get; }
	public IFuturemud Gameworld => Spell.Gameworld;
	public DamageType DamageType { get; private set; }
	public Expression DamageFormula { get; private set; }
	public Expression PainFormula { get; private set; }
	public Expression StunFormula { get; private set; }
	public Expression ThermalFormula { get; private set; }
	public double TickSeconds { get; private set; }
	public double MinimumOxidation { get; private set; }
	public bool SelfOxidising { get; private set; }
	public string SDescAddendum { get; private set; }
	public string DescAddendum { get; private set; }
	public ANSIColour AddendumColour { get; private set; }
	public bool IsInstantaneous => false;
	public bool RequiresTarget => true;

	public XElement SaveToXml()
	{
		return new XElement("Effect",
			new XAttribute("type", "burning"),
			new XElement("DamageType", (int)DamageType),
			new XElement("DamageFormula", new XCData(DamageFormula.OriginalExpression)),
			new XElement("PainFormula", new XCData(PainFormula.OriginalExpression)),
			new XElement("StunFormula", new XCData(StunFormula.OriginalExpression)),
			new XElement("ThermalFormula", new XCData(ThermalFormula.OriginalExpression)),
			new XElement("TickSeconds", TickSeconds),
			new XElement("MinimumOxidation", MinimumOxidation),
			new XElement("SelfOxidising", SelfOxidising),
			new XElement("SDescAddendum", new XCData(SDescAddendum)),
			new XElement("DescAddendum", new XCData(DescAddendum)),
			new XElement("AddendumColour", AddendumColour.Name)
		);
	}

	public bool IsCompatibleWithTrigger(IMagicTrigger trigger)
	{
		return PersistentSensoryCombatSpellEffectTemplateHelper.IsPerceivableTarget(trigger.TargetTypes);
	}

	public IMagicSpellEffect? GetOrApplyEffect(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
	{
		if (target is not ICharacter and not IGameItem)
		{
			return null;
		}

		return new SpellBurningEffect(target, parent, null, DamageType,
			Evaluate(DamageFormula, outcome, power),
			Evaluate(PainFormula, outcome, power),
			Evaluate(StunFormula, outcome, power),
			Evaluate(ThermalFormula, outcome, power),
			TickSeconds,
			MinimumOxidation,
			SelfOxidising,
			SDescAddendum,
			DescAddendum,
			AddendumColour);
	}

	private static double Evaluate(Expression formula, OpposedOutcomeDegree outcome, SpellPower power)
	{
		formula.Parameters["power"] = (int)power;
		formula.Parameters["outcome"] = (int)outcome;
		return Math.Max(0.0, formula.EvaluateDouble());
	}

	public IMagicSpellEffectTemplate Clone()
	{
		return new BurningEffect(SaveToXml(), Spell);
	}

	public const string HelpText = @"You can use the following options with this effect:

	#3type <damage type>#0 - sets the damage type
	#3damage <formula>#0 - sets the per-tick damage formula
	#3pain <formula>#0 - sets the per-tick pain formula
	#3stun <formula>#0 - sets the per-tick stun formula
	#3thermal <formula>#0 - sets the per-tick thermal load formula
	#3tick <seconds>#0 - sets the tick interval
	#3oxygen <factor>#0 - sets the minimum atmospheric oxidation factor
	#3selfoxidising#0 - toggles whether the flame supplies its own oxidiser
	#3sdesc <text>#0 - sets the short-description addendum
	#3desc <text>#0 - sets the full-description addendum
	#3colour <colour>#0 - sets the addendum colour";

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "type":
				return BuildingCommandType(actor, command);
			case "damage":
			case "formula":
				return BuildingCommandFormula(actor, command, "damage");
			case "pain":
				return BuildingCommandFormula(actor, command, "pain");
			case "stun":
				return BuildingCommandFormula(actor, command, "stun");
			case "thermal":
				return BuildingCommandFormula(actor, command, "thermal");
			case "tick":
			case "seconds":
				return BuildingCommandTick(actor, command);
			case "oxygen":
			case "oxidation":
				return BuildingCommandOxygen(actor, command);
			case "selfoxidising":
			case "selfoxidizing":
				SelfOxidising = !SelfOxidising;
				Spell.Changed = true;
				actor.OutputHandler.Send($"This spell fire will {SelfOxidising.NowNoLonger()} supply its own oxidiser.");
				return true;
			case "sdesc":
				return BuildingCommandSDesc(actor, command);
			case "desc":
				return BuildingCommandDesc(actor, command);
			case "colour":
			case "color":
				return BuildingCommandColour(actor, command);
		}

		actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
		return false;
	}

	private bool BuildingCommandType(ICharacter actor, StringStack command)
	{
		if (!command.SafeRemainingArgument.TryParseEnum<DamageType>(out var type))
		{
			actor.OutputHandler.Send($"That is not a valid damage type. Valid types are {Enum.GetValues<DamageType>().ListToColouredString()}.");
			return false;
		}

		DamageType = type;
		Spell.Changed = true;
		actor.OutputHandler.Send($"The burning damage type is now {DamageType.DescribeEnum().ColourValue()}.");
		return true;
	}

	private bool BuildingCommandFormula(ICharacter actor, StringStack command, string which)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"What formula should this effect use for {which}?");
			return false;
		}

		var expression = new Expression(command.SafeRemainingArgument);
		if (expression.HasErrors())
		{
			actor.OutputHandler.Send(expression.Error);
			return false;
		}

		switch (which)
		{
			case "damage":
				DamageFormula = expression;
				break;
			case "pain":
				PainFormula = expression;
				break;
			case "stun":
				StunFormula = expression;
				break;
			case "thermal":
				ThermalFormula = expression;
				break;
		}

		Spell.Changed = true;
		actor.OutputHandler.Send($"The {which} formula is now {expression.OriginalExpression.ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandTick(ICharacter actor, StringStack command)
	{
		if (command.IsFinished ||
		    !MagicBuilderValidation.TryParseFiniteDoubleInRange(command.SafeRemainingArgument, 1.0,
			    MagicBuilderValidation.MaximumSpellTickSeconds, out var seconds))
		{
			actor.OutputHandler.Send($"You must enter a number of seconds between {1.0.ToString("N1", actor).ColourValue()} and {MagicBuilderValidation.MaximumSpellTickSeconds.ToString("N0", actor).ColourValue()}.");
			return false;
		}

		TickSeconds = seconds;
		Spell.Changed = true;
		actor.OutputHandler.Send($"The burning tick interval is now {TickSeconds.ToString("N2", actor).ColourValue()} seconds.");
		return true;
	}

	private bool BuildingCommandOxygen(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !MagicBuilderValidation.TryParseFiniteDouble(command.SafeRemainingArgument, out var factor) || factor < 0.0)
		{
			actor.OutputHandler.Send("You must enter an atmospheric oxidation factor of 0.0 or greater.");
			return false;
		}

		MinimumOxidation = factor;
		Spell.Changed = true;
		actor.OutputHandler.Send($"The required oxidation factor is now {MinimumOxidation.ToString("N2", actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandSDesc(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What short-description addendum should burning targets have?");
			return false;
		}

		SDescAddendum = command.SafeRemainingArgument;
		Spell.Changed = true;
		actor.OutputHandler.Send($"Burning targets will show the short-description addendum {SDescAddendum.Colour(AddendumColour)}.");
		return true;
	}

	private bool BuildingCommandDesc(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What description addendum should burning targets have?");
			return false;
		}

		DescAddendum = command.SafeRemainingArgument;
		Spell.Changed = true;
		actor.OutputHandler.Send($"Burning targets will show the description addendum {DescAddendum.Colour(AddendumColour)}.");
		return true;
	}

	private bool BuildingCommandColour(ICharacter actor, StringStack command)
	{
		var colour = Telnet.GetColour(command.SafeRemainingArgument);
		if (colour is null)
		{
			actor.OutputHandler.Send($"That is not a valid colour. The options are:\n\n{Telnet.GetColourOptions.Select(x => x.Colour(Telnet.GetColour(x))).ListToLines(true)}");
			return false;
		}

		AddendumColour = colour;
		Spell.Changed = true;
		actor.OutputHandler.Send($"Burning addendum text will now use {colour.Name.Colour(colour)}.");
		return true;
	}

	public string Show(ICharacter actor)
	{
		return SpellEffectPresentation.Describe(actor, "Burning",
			("Damage Type", DamageType.DescribeEnum().ColourValue()),
			("Damage", DamageFormula.OriginalExpression.ColourCommand()),
			("Pain", PainFormula.OriginalExpression.ColourCommand()),
			("Stun", StunFormula.OriginalExpression.ColourCommand()),
			("Thermal", ThermalFormula.OriginalExpression.ColourCommand()),
			("Tick", $"{TickSeconds.ToString("N2", actor)}s".ColourValue()),
			("Minimum Oxidation", MinimumOxidation.ToString("N2", actor).ColourValue()),
			("Self Oxidising", SelfOxidising.ToColouredString()),
			("Short Addendum", SDescAddendum.Colour(AddendumColour)),
			("Description Addendum", DescAddendum.Colour(AddendumColour)));
	}
}

public class TrackMarkEffect : IMagicSpellEffectTemplate
{
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("trackmark", (root, spell) => new TrackMarkEffect(root, spell));
		SpellEffectFactory.RegisterLoadTimeFactory("tracktrail", (root, spell) => new TrackMarkEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("trackmark", BuilderFactory,
			"Alters the target's future track intensity and track circumstances",
			HelpText,
			false,
			true,
			PersistentSensoryCombatSpellEffectTemplateHelper.CharacterTriggerTypes);
		SpellEffectFactory.RegisterBuilderFactory("tracktrail", BuilderFactory,
			"Alias for trackmark",
			HelpText,
			false,
			true,
			PersistentSensoryCombatSpellEffectTemplateHelper.CharacterTriggerTypes);
	}

	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands,
		IMagicSpell spell)
	{
		return (new TrackMarkEffect(new XElement("Effect",
			new XAttribute("type", "trackmark"),
			new XElement("VisualMultiplier", 2.0),
			new XElement("OlfactoryMultiplier", 1.0),
			new XElement("VisualBonus", 1.0),
			new XElement("OlfactoryBonus", 0.0),
			new XElement("MarkTracks", true),
			new XElement("SDescAddendum", new XCData("(leaving luminous tracks)")),
			new XElement("AddendumColour", "bold cyan")
		), spell), string.Empty);
	}

	private TrackMarkEffect(XElement root, IMagicSpell spell)
	{
		Spell = spell;
		VisualTrackIntensityMultiplier = Math.Max(0.0, double.Parse(root.Element("VisualMultiplier")?.Value ?? "2"));
		OlfactoryTrackIntensityMultiplier = Math.Max(0.0, double.Parse(root.Element("OlfactoryMultiplier")?.Value ?? "1"));
		VisualTrackIntensityBonus = Math.Max(0.0, double.Parse(root.Element("VisualBonus")?.Value ?? "1"));
		OlfactoryTrackIntensityBonus = Math.Max(0.0, double.Parse(root.Element("OlfactoryBonus")?.Value ?? "0"));
		MarkTracksMagical = bool.Parse(root.Element("MarkTracks")?.Value ?? "true");
		SDescAddendum = root.Element("SDescAddendum")?.Value ?? "(leaving luminous tracks)";
		AddendumColour = Telnet.GetColour(root.Element("AddendumColour")?.Value ?? "bold cyan") ?? Telnet.BoldCyan;
	}

	public IMagicSpell Spell { get; }
	public IFuturemud Gameworld => Spell.Gameworld;
	public double VisualTrackIntensityMultiplier { get; private set; }
	public double OlfactoryTrackIntensityMultiplier { get; private set; }
	public double VisualTrackIntensityBonus { get; private set; }
	public double OlfactoryTrackIntensityBonus { get; private set; }
	public bool MarkTracksMagical { get; private set; }
	public string SDescAddendum { get; private set; }
	public ANSIColour AddendumColour { get; private set; }
	public bool IsInstantaneous => false;
	public bool RequiresTarget => true;

	public XElement SaveToXml()
	{
		return new XElement("Effect",
			new XAttribute("type", "trackmark"),
			new XElement("VisualMultiplier", VisualTrackIntensityMultiplier),
			new XElement("OlfactoryMultiplier", OlfactoryTrackIntensityMultiplier),
			new XElement("VisualBonus", VisualTrackIntensityBonus),
			new XElement("OlfactoryBonus", OlfactoryTrackIntensityBonus),
			new XElement("MarkTracks", MarkTracksMagical),
			new XElement("SDescAddendum", new XCData(SDescAddendum)),
			new XElement("AddendumColour", AddendumColour.Name)
		);
	}

	public bool IsCompatibleWithTrigger(IMagicTrigger trigger)
	{
		return PersistentSensoryCombatSpellEffectTemplateHelper.IsCharacterTarget(trigger.TargetTypes);
	}

	public IMagicSpellEffect? GetOrApplyEffect(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
	{
		return target is ICharacter
			? new SpellTrackMarkEffect(target, parent, null,
				VisualTrackIntensityMultiplier,
				OlfactoryTrackIntensityMultiplier,
				VisualTrackIntensityBonus,
				OlfactoryTrackIntensityBonus,
				MarkTracksMagical ? TrackCircumstances.MagicallyMarked : TrackCircumstances.None,
				SDescAddendum,
				AddendumColour)
			: null;
	}

	public IMagicSpellEffectTemplate Clone()
	{
		return new TrackMarkEffect(SaveToXml(), Spell);
	}

	public const string HelpText = @"You can use the following options with this effect:

	#3visualmult <amount>#0 - sets the visual track intensity multiplier
	#3olfactorymult <amount>#0 - sets the olfactory track intensity multiplier
	#3visualbonus <amount>#0 - sets the flat visual track intensity bonus
	#3olfactorybonus <amount>#0 - sets the flat olfactory track intensity bonus
	#3marked#0 - toggles adding the magically-marked track circumstance
	#3sdesc <text>#0 - sets the short-description addendum
	#3colour <colour>#0 - sets the addendum colour";

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "visualmult":
			case "visualmultiplier":
				return BuildingCommandDouble(actor, command, "visual multiplier");
			case "olfactorymult":
			case "olfactorymultiplier":
			case "smellmult":
			case "smellmultiplier":
				return BuildingCommandDouble(actor, command, "olfactory multiplier");
			case "visualbonus":
				return BuildingCommandDouble(actor, command, "visual bonus");
			case "olfactorybonus":
			case "smellbonus":
				return BuildingCommandDouble(actor, command, "olfactory bonus");
			case "marked":
			case "magical":
				MarkTracksMagical = !MarkTracksMagical;
				Spell.Changed = true;
				actor.OutputHandler.Send($"Tracks made by this target will {MarkTracksMagical.NowNoLonger()} carry the magically-marked circumstance.");
				return true;
			case "sdesc":
				return BuildingCommandSDesc(actor, command);
			case "colour":
			case "color":
				return BuildingCommandColour(actor, command);
		}

		actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
		return false;
	}

	private bool BuildingCommandDouble(ICharacter actor, StringStack command, string field)
	{
		if (command.IsFinished || !double.TryParse(command.SafeRemainingArgument, out var value) || value < 0.0)
		{
			actor.OutputHandler.Send("You must enter a number of 0.0 or greater.");
			return false;
		}

		switch (field)
		{
			case "visual multiplier":
				VisualTrackIntensityMultiplier = value;
				break;
			case "olfactory multiplier":
				OlfactoryTrackIntensityMultiplier = value;
				break;
			case "visual bonus":
				VisualTrackIntensityBonus = value;
				break;
			case "olfactory bonus":
				OlfactoryTrackIntensityBonus = value;
				break;
		}

		Spell.Changed = true;
		actor.OutputHandler.Send($"The {field} is now {value.ToString("N2", actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandSDesc(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What short-description addendum should marked targets have?");
			return false;
		}

		SDescAddendum = command.SafeRemainingArgument;
		Spell.Changed = true;
		actor.OutputHandler.Send($"Marked targets will show the short-description addendum {SDescAddendum.Colour(AddendumColour)}.");
		return true;
	}

	private bool BuildingCommandColour(ICharacter actor, StringStack command)
	{
		var colour = Telnet.GetColour(command.SafeRemainingArgument);
		if (colour is null)
		{
			actor.OutputHandler.Send($"That is not a valid colour. The options are:\n\n{Telnet.GetColourOptions.Select(x => x.Colour(Telnet.GetColour(x))).ListToLines(true)}");
			return false;
		}

		AddendumColour = colour;
		Spell.Changed = true;
		actor.OutputHandler.Send($"Track mark addendum text will now use {colour.Name.Colour(colour)}.");
		return true;
	}

	public string Show(ICharacter actor)
	{
		return SpellEffectPresentation.Describe(actor, "Track Mark",
			("Visual Multiplier", VisualTrackIntensityMultiplier.ToString("N2", actor).ColourValue()),
			("Visual Bonus", VisualTrackIntensityBonus.ToString("N2", actor).ColourValue()),
			("Olfactory Multiplier", OlfactoryTrackIntensityMultiplier.ToString("N2", actor).ColourValue()),
			("Olfactory Bonus", OlfactoryTrackIntensityBonus.ToString("N2", actor).ColourValue()),
			("Magical Tracks", MarkTracksMagical.ToColouredString()),
			("Short Addendum", SDescAddendum.Colour(AddendumColour)));
	}
}
