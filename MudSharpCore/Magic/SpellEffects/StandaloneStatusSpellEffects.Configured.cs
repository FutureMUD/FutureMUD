#nullable enable
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Health;
using MudSharp.RPG.Checks;
using System.Xml.Linq;

namespace MudSharp.Magic.SpellEffects;

public class PoisonEffect : CharacterSpellEffectTemplateBase
{
	public const string HelpText = @"You can use the following options with this effect:
	#3drug <which>#0 - sets which drug is applied as the poison
	#3vector <which>#0 - sets how the poison enters the body
	#3formula <expr>#0 - sets the grams formula";

	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("poison", (root, spell) => new PoisonEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("poison", BuilderFactory,
			"Poisons the target with a configured drug payload",
			HelpText,
			false,
			true,
			StandaloneSpellEffectTemplateHelper.CharacterTriggerTypes);
	}

	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands, IMagicSpell spell)
		=> (new PoisonEffect(new XElement("Effect",
			new XAttribute("type", "poison"),
			new XElement("Drug", 0L),
			new XElement("Vector", (int)DrugVector.Injected),
			new XElement("Formula", new XCData("power*outcome"))
		), spell), string.Empty);

	protected PoisonEffect(XElement root, IMagicSpell spell) : base(root, spell) { }

	public IDrug? Drug { get; private set; }
	public DrugVector Vector { get; private set; }
	public ITraitExpression GramsExpression { get; private set; } = null!;

	protected override string BuilderEffectType => "poison";
	protected override string ShowText =>
		$"Poison - {(Drug?.Name ?? "None")} via {Vector.Describe()} at {GramsExpression.OriginalFormulaText}";

	protected override void LoadFromXml(XElement root)
	{
		XElement? drugElement = root.Element("Drug");
		if (drugElement != null)
		{
			Drug = long.TryParse(drugElement.Value, out long value)
				? Gameworld.Drugs.Get(value)
				: Gameworld.Drugs.GetByIdOrName(drugElement.Value);
		}

		Vector = (DrugVector)int.Parse(root.Element("Vector")?.Value ?? ((int)DrugVector.Injected).ToString());
		GramsExpression = new TraitExpression(root.Element("Formula")?.Value ?? "0", Gameworld);
	}

	protected override void SaveToXml(XElement root)
	{
		root.Add(new XElement("Drug", Drug?.Id ?? 0L));
		root.Add(new XElement("Vector", (int)Vector));
		root.Add(new XElement("Formula", new XCData(GramsExpression.OriginalFormulaText)));
	}

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "drug":
				return BuildingCommandDrug(actor, command);
			case "vector":
				return BuildingCommandVector(actor, command);
			case "formula":
				return BuildingCommandFormula(actor, command);
		}

		actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
		return false;
	}

	private bool BuildingCommandDrug(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which drug should this poison effect apply?");
			return false;
		}

		IDrug? drug = Gameworld.Drugs.GetByIdOrName(command.SafeRemainingArgument);
		if (drug is null)
		{
			actor.OutputHandler.Send("There is no such drug.");
			return false;
		}

		Drug = drug;
		Spell.Changed = true;
		actor.OutputHandler.Send($"This poison effect will now apply {Drug.Name.ColourValue()}.");
		return true;
	}

	private bool BuildingCommandVector(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !command.SafeRemainingArgument.TryParseEnum(out DrugVector vector) || vector == DrugVector.None)
		{
			actor.OutputHandler.Send($"You must specify a valid drug vector. Valid options are {System.Enum.GetValues<DrugVector>().ListToColouredString()}.");
			return false;
		}

		Vector = vector;
		Spell.Changed = true;
		actor.OutputHandler.Send($"This poison effect will now apply via {Vector.Describe().ColourValue()}.");
		return true;
	}

	private bool BuildingCommandFormula(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What grams formula should this poison use?");
			return false;
		}

		TraitExpression expression = new(command.SafeRemainingArgument, Gameworld);
		if (expression.HasErrors())
		{
			actor.OutputHandler.Send(expression.Error);
			return false;
		}

		GramsExpression = expression;
		Spell.Changed = true;
		actor.OutputHandler.Send($"This poison effect now uses the formula {GramsExpression.OriginalFormulaText.ColourCommand()}.");
		return true;
	}

	protected override IMagicSpellEffect? CreateEffect(ICharacter caster, ICharacter target,
		OpposedOutcomeDegree outcome, SpellPower power, IMagicSpellEffectParent parent,
		SpellAdditionalParameter[] additionalParameters)
	{
		if (Drug is null)
		{
			return null;
		}

		double grams = GramsExpression.EvaluateWith(caster, values: new (string, object)[]
		{
			("power", (int)power),
			("outcome", (int)outcome)
		});
		if (grams <= 0.0)
		{
			return null;
		}

		return new SpellPoisonEffect(target, parent, Drug, Vector, grams, GramsExpression.OriginalFormulaText);
	}

	public override IMagicSpellEffectTemplate Clone() => new PoisonEffect(SaveToXml(), Spell);
}

public class RemovePoisonEffect : CharacterSpellEffectRemovalTemplateBase
{
	public const string HelpText = @"You can use the following options with this effect:
	#3drug <which>#0 - sets which drug payload is removed
	#3vector <which>#0 - sets the vector that must match
	#3formula <expr>#0 - sets the grams formula text that must match";

	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("removepoison", (root, spell) => new RemovePoisonEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("removepoison", BuilderFactory,
			"Removes a matching magical poison payload from the target",
			HelpText,
			true,
			true,
			StandaloneSpellEffectTemplateHelper.CharacterTriggerTypes);
	}

	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands, IMagicSpell spell)
		=> (new RemovePoisonEffect(new XElement("Effect",
			new XAttribute("type", "removepoison"),
			new XElement("Drug", 0L),
			new XElement("Vector", (int)DrugVector.Injected),
			new XElement("Formula", new XCData("power*outcome"))
		), spell), string.Empty);

	protected RemovePoisonEffect(XElement root, IMagicSpell spell) : base(root, spell) { }

	public IDrug? Drug { get; private set; }
	public DrugVector Vector { get; private set; }
	public string FormulaText { get; private set; } = "0";

	protected override string BuilderEffectType => "removepoison";
	protected override string ShowText => $"Remove Poison - {(Drug?.Name ?? "None")} via {Vector.Describe()} at {FormulaText}";

	protected override void LoadFromXml(XElement root)
	{
		XElement? drugElement = root.Element("Drug");
		if (drugElement != null)
		{
			Drug = long.TryParse(drugElement.Value, out long value)
				? Gameworld.Drugs.Get(value)
				: Gameworld.Drugs.GetByIdOrName(drugElement.Value);
		}

		Vector = (DrugVector)int.Parse(root.Element("Vector")?.Value ?? ((int)DrugVector.Injected).ToString());
		FormulaText = root.Element("Formula")?.Value ?? "0";
	}

	protected override void SaveToXml(XElement root)
	{
		root.Add(new XElement("Drug", Drug?.Id ?? 0L));
		root.Add(new XElement("Vector", (int)Vector));
		root.Add(new XElement("Formula", new XCData(FormulaText)));
	}

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "drug":
				return BuildingCommandDrug(actor, command);
			case "vector":
				return BuildingCommandVector(actor, command);
			case "formula":
				return BuildingCommandFormula(actor, command);
		}

		actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
		return false;
	}

	private bool BuildingCommandDrug(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which drug should this remove-poison effect match?");
			return false;
		}

		IDrug? drug = Gameworld.Drugs.GetByIdOrName(command.SafeRemainingArgument);
		if (drug is null)
		{
			actor.OutputHandler.Send("There is no such drug.");
			return false;
		}

		Drug = drug;
		Spell.Changed = true;
		actor.OutputHandler.Send($"This remove-poison effect will now match {Drug.Name.ColourValue()}.");
		return true;
	}

	private bool BuildingCommandVector(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !command.SafeRemainingArgument.TryParseEnum(out DrugVector vector) || vector == DrugVector.None)
		{
			actor.OutputHandler.Send($"You must specify a valid drug vector. Valid options are {System.Enum.GetValues<DrugVector>().ListToColouredString()}.");
			return false;
		}

		Vector = vector;
		Spell.Changed = true;
		actor.OutputHandler.Send($"This remove-poison effect will now match {Vector.Describe().ColourValue()}.");
		return true;
	}

	private bool BuildingCommandFormula(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What grams formula text should this remove-poison effect match?");
			return false;
		}

		TraitExpression expression = new(command.SafeRemainingArgument, Gameworld);
		if (expression.HasErrors())
		{
			actor.OutputHandler.Send(expression.Error);
			return false;
		}

		FormulaText = expression.OriginalFormulaText;
		Spell.Changed = true;
		actor.OutputHandler.Send($"This remove-poison effect will now match the formula {FormulaText.ColourCommand()}.");
		return true;
	}

	protected override void RemoveEffects(ICharacter target)
	{
		target.RemoveAllEffects<SpellPoisonEffect>(x =>
			x.Drug == Drug &&
			x.Vector == Vector &&
			x.GramsFormulaText.EqualTo(FormulaText), true);
	}

	public override IMagicSpellEffectTemplate Clone() => new RemovePoisonEffect(SaveToXml(), Spell);
}

public class DiseaseEffect : CharacterSpellEffectTemplateBase
{
	public const string HelpText = @"You can use the following options with this effect:
	#3type <which>#0 - sets the infection type
	#3difficulty <which>#0 - sets the infection virulence difficulty
	#3intensity <expr>#0 - sets the infection intensity formula
	#3virulence <expr>#0 - sets the infection virulence multiplier formula";

	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("disease", (root, spell) => new DiseaseEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("disease", BuilderFactory,
			"Afflicts the target with a systemic disease",
			HelpText,
			false,
			true,
			StandaloneSpellEffectTemplateHelper.CharacterTriggerTypes);
	}

	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands, IMagicSpell spell)
		=> (new DiseaseEffect(new XElement("Effect",
			new XAttribute("type", "disease"),
			new XElement("InfectionType", (int)InfectionType.Simple),
			new XElement("VirulenceDifficulty", (int)Difficulty.Normal),
			new XElement("IntensityFormula", new XCData("power*outcome*0.001")),
			new XElement("VirulenceFormula", new XCData("1.0"))
		), spell), string.Empty);

	protected DiseaseEffect(XElement root, IMagicSpell spell) : base(root, spell) { }

	public InfectionType InfectionType { get; private set; }
	public Difficulty VirulenceDifficulty { get; private set; }
	public ITraitExpression IntensityExpression { get; private set; } = null!;
	public ITraitExpression VirulenceExpression { get; private set; } = null!;

	protected override string BuilderEffectType => "disease";
	protected override string ShowText =>
		$"Disease - {InfectionType.Describe()} @ {VirulenceDifficulty.DescribeEnum()} intensity {IntensityExpression.OriginalFormulaText} virulence {VirulenceExpression.OriginalFormulaText}";

	protected override void LoadFromXml(XElement root)
	{
		InfectionType = (InfectionType)int.Parse(root.Element("InfectionType")?.Value ?? ((int)InfectionType.Simple).ToString());
		VirulenceDifficulty = (Difficulty)int.Parse(root.Element("VirulenceDifficulty")?.Value ?? ((int)Difficulty.Normal).ToString());
		IntensityExpression = new TraitExpression(root.Element("IntensityFormula")?.Value ?? "0", Gameworld);
		VirulenceExpression = new TraitExpression(root.Element("VirulenceFormula")?.Value ?? "1", Gameworld);
	}

	protected override void SaveToXml(XElement root)
	{
		root.Add(new XElement("InfectionType", (int)InfectionType));
		root.Add(new XElement("VirulenceDifficulty", (int)VirulenceDifficulty));
		root.Add(new XElement("IntensityFormula", new XCData(IntensityExpression.OriginalFormulaText)));
		root.Add(new XElement("VirulenceFormula", new XCData(VirulenceExpression.OriginalFormulaText)));
	}

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "type":
				return BuildingCommandType(actor, command);
			case "difficulty":
				return BuildingCommandDifficulty(actor, command);
			case "intensity":
				return BuildingCommandIntensity(actor, command);
			case "virulence":
				return BuildingCommandVirulence(actor, command);
		}

		actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
		return false;
	}

	private bool BuildingCommandType(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !command.SafeRemainingArgument.TryParseEnum(out InfectionType value))
		{
			actor.OutputHandler.Send($"You must specify a valid infection type. Valid options are {System.Enum.GetValues<InfectionType>().ListToColouredString()}.");
			return false;
		}

		InfectionType = value;
		Spell.Changed = true;
		actor.OutputHandler.Send($"This disease effect will now apply {InfectionType.Describe().ColourValue()}.");
		return true;
	}

	private bool BuildingCommandDifficulty(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !command.SafeRemainingArgument.TryParseEnum(out Difficulty value))
		{
			actor.OutputHandler.Send($"You must specify a valid difficulty. Valid options are {System.Enum.GetValues<Difficulty>().ListToColouredString()}.");
			return false;
		}

		VirulenceDifficulty = value;
		Spell.Changed = true;
		actor.OutputHandler.Send($"This disease effect will now use {VirulenceDifficulty.DescribeColoured()} virulence difficulty.");
		return true;
	}

	private bool BuildingCommandIntensity(ICharacter actor, StringStack command)
	{
		return BuildingCommandFormula(actor, command, true);
	}

	private bool BuildingCommandVirulence(ICharacter actor, StringStack command)
	{
		return BuildingCommandFormula(actor, command, false);
	}

	private bool BuildingCommandFormula(ICharacter actor, StringStack command, bool intensity)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"What {(intensity ? "intensity" : "virulence")} formula should this disease use?");
			return false;
		}

		TraitExpression expression = new(command.SafeRemainingArgument, Gameworld);
		if (expression.HasErrors())
		{
			actor.OutputHandler.Send(expression.Error);
			return false;
		}

		if (intensity)
		{
			IntensityExpression = expression;
		}
		else
		{
			VirulenceExpression = expression;
		}

		Spell.Changed = true;
		actor.OutputHandler.Send($"This disease effect now uses the {(intensity ? "intensity" : "virulence")} formula {expression.OriginalFormulaText.ColourCommand()}.");
		return true;
	}

	protected override IMagicSpellEffect? CreateEffect(ICharacter caster, ICharacter target,
		OpposedOutcomeDegree outcome, SpellPower power, IMagicSpellEffectParent parent,
		SpellAdditionalParameter[] additionalParameters)
	{
		double intensity = IntensityExpression.EvaluateWith(caster, values: new (string, object)[]
		{
			("power", (int)power),
			("outcome", (int)outcome)
		});
		if (intensity <= 0.0)
		{
			return null;
		}

		double virulence = VirulenceExpression.EvaluateWith(caster, values: new (string, object)[]
		{
			("power", (int)power),
			("outcome", (int)outcome)
		});
		return new SpellDiseaseEffect(target, parent, InfectionType, VirulenceDifficulty, intensity,
			System.Math.Max(0.0, virulence), IntensityExpression.OriginalFormulaText,
			VirulenceExpression.OriginalFormulaText);
	}

	public override IMagicSpellEffectTemplate Clone() => new DiseaseEffect(SaveToXml(), Spell);
}

public class RemoveDiseaseEffect : CharacterSpellEffectRemovalTemplateBase
{
	public const string HelpText = @"You can use the following options with this effect:
	#3type <which>#0 - sets the infection type that must match
	#3difficulty <which>#0 - sets the virulence difficulty that must match
	#3intensity <expr>#0 - sets the intensity formula text that must match
	#3virulence <expr>#0 - sets the virulence formula text that must match";

	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("removedisease", (root, spell) => new RemoveDiseaseEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("removedisease", BuilderFactory,
			"Removes a matching spell-borne disease from the target",
			HelpText,
			true,
			true,
			StandaloneSpellEffectTemplateHelper.CharacterTriggerTypes);
	}

	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands, IMagicSpell spell)
		=> (new RemoveDiseaseEffect(new XElement("Effect",
			new XAttribute("type", "removedisease"),
			new XElement("InfectionType", (int)InfectionType.Simple),
			new XElement("VirulenceDifficulty", (int)Difficulty.Normal),
			new XElement("IntensityFormula", new XCData("power*outcome*0.001")),
			new XElement("VirulenceFormula", new XCData("1.0"))
		), spell), string.Empty);

	protected RemoveDiseaseEffect(XElement root, IMagicSpell spell) : base(root, spell) { }

	public InfectionType InfectionType { get; private set; }
	public Difficulty VirulenceDifficulty { get; private set; }
	public string IntensityFormulaText { get; private set; } = "0";
	public string VirulenceFormulaText { get; private set; } = "1";

	protected override string BuilderEffectType => "removedisease";
	protected override string ShowText =>
		$"Remove Disease - {InfectionType.Describe()} @ {VirulenceDifficulty.DescribeEnum()} intensity {IntensityFormulaText} virulence {VirulenceFormulaText}";

	protected override void LoadFromXml(XElement root)
	{
		InfectionType = (InfectionType)int.Parse(root.Element("InfectionType")?.Value ?? ((int)InfectionType.Simple).ToString());
		VirulenceDifficulty = (Difficulty)int.Parse(root.Element("VirulenceDifficulty")?.Value ?? ((int)Difficulty.Normal).ToString());
		IntensityFormulaText = root.Element("IntensityFormula")?.Value ?? "0";
		VirulenceFormulaText = root.Element("VirulenceFormula")?.Value ?? "1";
	}

	protected override void SaveToXml(XElement root)
	{
		root.Add(new XElement("InfectionType", (int)InfectionType));
		root.Add(new XElement("VirulenceDifficulty", (int)VirulenceDifficulty));
		root.Add(new XElement("IntensityFormula", new XCData(IntensityFormulaText)));
		root.Add(new XElement("VirulenceFormula", new XCData(VirulenceFormulaText)));
	}

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "type":
				return BuildingCommandType(actor, command);
			case "difficulty":
				return BuildingCommandDifficulty(actor, command);
			case "intensity":
				return BuildingCommandFormula(actor, command, true);
			case "virulence":
				return BuildingCommandFormula(actor, command, false);
		}

		actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
		return false;
	}

	private bool BuildingCommandType(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !command.SafeRemainingArgument.TryParseEnum(out InfectionType value))
		{
			actor.OutputHandler.Send($"You must specify a valid infection type. Valid options are {System.Enum.GetValues<InfectionType>().ListToColouredString()}.");
			return false;
		}

		InfectionType = value;
		Spell.Changed = true;
		actor.OutputHandler.Send($"This remove-disease effect will now match {InfectionType.Describe().ColourValue()}.");
		return true;
	}

	private bool BuildingCommandDifficulty(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !command.SafeRemainingArgument.TryParseEnum(out Difficulty value))
		{
			actor.OutputHandler.Send($"You must specify a valid difficulty. Valid options are {System.Enum.GetValues<Difficulty>().ListToColouredString()}.");
			return false;
		}

		VirulenceDifficulty = value;
		Spell.Changed = true;
		actor.OutputHandler.Send($"This remove-disease effect will now match {VirulenceDifficulty.DescribeColoured()} virulence difficulty.");
		return true;
	}

	private bool BuildingCommandFormula(ICharacter actor, StringStack command, bool intensity)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"What {(intensity ? "intensity" : "virulence")} formula text should this remove-disease effect match?");
			return false;
		}

		TraitExpression expression = new(command.SafeRemainingArgument, Gameworld);
		if (expression.HasErrors())
		{
			actor.OutputHandler.Send(expression.Error);
			return false;
		}

		if (intensity)
		{
			IntensityFormulaText = expression.OriginalFormulaText;
		}
		else
		{
			VirulenceFormulaText = expression.OriginalFormulaText;
		}

		Spell.Changed = true;
		actor.OutputHandler.Send($"This remove-disease effect will now match the {(intensity ? "intensity" : "virulence")} formula {expression.OriginalFormulaText.ColourCommand()}.");
		return true;
	}

	protected override void RemoveEffects(ICharacter target)
	{
		target.RemoveAllEffects<SpellDiseaseEffect>(x =>
			x.InfectionType == InfectionType &&
			x.VirulenceDifficulty == VirulenceDifficulty &&
			x.IntensityFormulaText.EqualTo(IntensityFormulaText) &&
			x.VirulenceFormulaText.EqualTo(VirulenceFormulaText), true);
	}

	public override IMagicSpellEffectTemplate Clone() => new RemoveDiseaseEffect(SaveToXml(), Spell);
}
