#nullable enable
using MudSharp.Body.Traits;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Health;
using MudSharp.RPG.Checks;
using MudSharp.Body;

namespace MudSharp.Magic.SpellEffects;

public class PoisonEffect : CharacterSpellEffectTemplateBase, IMagicSpellEffectOperation, IMagicSpellEffectPreparedSelection
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
	public ITraitExpression GramsExpression { get; internal set; } = null!;
	public string GramsFormulaText { get; private set; } = "0";
	public string? OwnedFamily { get; private set; }
	private ConditionBodySelection? _selection;
	public string? DefinitionError => OwnedFamily is not null &&
		(string.IsNullOrWhiteSpace(OwnedFamily) || OwnedFamily.Length > 255 ||
		 GramsFormulaText.Length > 255 || Drug is null || Vector != DrugVector.Touched)
		? "Owned poison requires a named family, a native drug and declared contact exposure." : null;

	protected override string BuilderEffectType => "poison";
	protected override string ShowText =>
		$"Poison - {(Drug?.Name ?? "None")} via {Vector.Describe()} at {GramsFormulaText}";

	public override string Show(ICharacter actor)
	{
		return SpellEffectPresentation.Describe(actor, "Poison",
			("Drug", (Drug?.Name ?? "None").ColourValue()),
			("Vector", Vector.Describe().ColourValue()),
			("Grams", GramsFormulaText.ColourCommand()));
	}

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
		GramsFormulaText = root.Element("Formula")?.Value ?? "0";
		GramsExpression = new TraitExpression(GramsFormulaText, Gameworld);
		OwnedFamily = root.Element("OwnedFamily")?.Value;
	}

	protected override void SaveToXml(XElement root)
	{
		root.Add(new XElement("Drug", Drug?.Id ?? 0L));
		root.Add(new XElement("Vector", (int)Vector));
		root.Add(new XElement("Formula", new XCData(GramsFormulaText)));
		if (OwnedFamily is not null) root.Add(new XElement("OwnedFamily", OwnedFamily));
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
		GramsFormulaText = expression.OriginalFormulaText;
		Spell.Changed = true;
		actor.OutputHandler.Send($"This poison effect now uses the formula {GramsExpression.OriginalFormulaText.ColourCommand()}.");
		return true;
	}

	protected override IMagicSpellEffect? CreateEffect(ICharacter caster, ICharacter target,
		OpposedOutcomeDegree outcome, SpellPower power, IMagicSpellEffectParent parent,
		SpellAdditionalParameter[] additionalParameters)
	{
		if (DefinitionError is { } error) throw new InvalidOperationException(error);
		if (OwnedFamily is not null && (_selection is null || !_selection.IsCurrent(target)))
			throw new InvalidOperationException("The selected poison recipient or physical body changed before exposure.");
		if (Drug is null)
		{
			return null;
		}

		double grams = GramsExpression.EvaluateWith(caster, values: new (string, object)[]
		{
			("power", (int)power),
			("outcome", (int)outcome)
		});
		if (!double.IsFinite(grams) || grams <= 0.0)
		{
			return null;
		}

		return new SpellPoisonEffect(target, parent, Drug, Vector, grams, GramsFormulaText,
			ownedFamily: OwnedFamily);
	}

	public IMagicSpellEffectPreparedSelectionToken? CapturePreparedSelection(ICharacter caster, IPerceivable recipient) =>
		OwnedFamily is null || recipient is not ICharacter target ? null : _selection = new(target, target.Body);

	public bool TryReusePreparedSelection(IMagicSpellEffectPreparedSelectionToken selection, ICharacter caster,
		IPerceivable recipient, out string? error)
	{
		error = null;
		if (OwnedFamily is null) return true;
		if (selection is not ConditionBodySelection body || !body.IsCurrent(recipient))
		{ error = "The selected poison recipient or physical body changed."; return false; }
		_selection = body;
		return true;
	}

	public bool TryConfirmPreparedSelection(ICharacter caster, IPerceivable recipient, out string? error)
	{
		error = OwnedFamily is not null && (_selection is null || !_selection.IsCurrent(recipient))
			? "The selected poison recipient or physical body changed." : null;
		return error is null;
	}

	public MagicEffectOperation Apply(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
	{
		var child = GetOrApplyEffect(caster, target, outcome, power, parent, additionalParameters);
		return new(OwnedFamily is null ? MagicEffectOperationStatus.Unknown :
			child is null ? MagicEffectOperationStatus.NoChange : MagicEffectOperationStatus.Applied, child);
	}

	public override IMagicSpellEffectTemplate Clone() => new PoisonEffect(SaveToXml(), Spell);
}

public class RemovePoisonEffect : CharacterSpellEffectRemovalTemplateBase, IMagicSpellEffectOperation, IMagicSpellEffectPreparedSelection
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
	public string? OwnedFamily { get; private set; }
	private ConditionBodySelection? _selection;
	public string? DefinitionError => OwnedFamily is not null &&
		(string.IsNullOrWhiteSpace(OwnedFamily) || OwnedFamily.Length > 255 ||
		 FormulaText.Length > 255 || Drug is null || Vector != DrugVector.Touched)
		? "Owned poison cure requires a named family, a native drug and declared contact exposure." : null;

	protected override string BuilderEffectType => "removepoison";
	protected override string ShowText => $"Remove Poison - {(Drug?.Name ?? "None")} via {Vector.Describe()} at {FormulaText}";

	public override string Show(ICharacter actor)
	{
		return SpellEffectPresentation.Describe(actor, "Remove Poison",
			("Drug", (Drug?.Name ?? "None").ColourValue()),
			("Vector", Vector.Describe().ColourValue()),
			("Formula", FormulaText.ColourCommand()));
	}

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
		OwnedFamily = root.Element("OwnedFamily")?.Value;
	}

	protected override void SaveToXml(XElement root)
	{
		root.Add(new XElement("Drug", Drug?.Id ?? 0L));
		root.Add(new XElement("Vector", (int)Vector));
		root.Add(new XElement("Formula", new XCData(FormulaText)));
		if (OwnedFamily is not null) root.Add(new XElement("OwnedFamily", OwnedFamily));
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
		if (OwnedFamily is not null)
		{
			ApplyOwnedRemoval(target);
			return;
		}
		target.RemoveAllEffects<SpellPoisonEffect>(x =>
			x.Drug == Drug &&
			x.Vector == Vector &&
			x.GramsFormulaText.EqualTo(FormulaText), true);
	}

	private MagicEffectOperationStatus ApplyOwnedRemoval(ICharacter target)
	{
		if (DefinitionError is { } error) throw new InvalidOperationException(error);
		var selected = _selection ?? new ConditionBodySelection(target, target.Body);
		if (!selected.IsCurrent(target)) return MagicEffectOperationStatus.Rejected;
		var matches = target.EffectsOfType<SpellPoisonEffect>().Where(x => x.OwnedFamily == OwnedFamily &&
			x.Drug == Drug && x.Vector == Vector && x.GramsFormulaText.EqualTo(FormulaText)).ToArray();
		bool OwnedDose(DrugDosage dose) => dose.SpellOriginId.HasValue &&
			dose.SpellOriginFamily == OwnedFamily && dose.Drug == Drug &&
			dose.OriginalVector == Vector && dose.SpellOriginFormula.EqualTo(FormulaText);
		var hasDoses = selected.Body.ActiveDrugDosages.Any(OwnedDose) ||
			selected.Body.LatentDrugDosages.Any(OwnedDose);
		if (!selected.IsCurrent(target)) return MagicEffectOperationStatus.Rejected;
		if (matches.Length == 0 && !hasDoses) return MagicEffectOperationStatus.NoChange;
		foreach (var effect in matches)
		{
			if (!selected.IsCurrent(target)) throw new InvalidOperationException("Poison cure recipient changed after payment.");
			target.RemoveEffect(effect, true);
			if (!selected.IsCurrent(target)) throw new InvalidOperationException("Poison cure recipient changed during removal.");
		}
		selected.Body.RemoveDrugDosages(OwnedDose);
		if (!selected.IsCurrent(target)) throw new InvalidOperationException("Poison cure recipient changed during dose removal.");
		return MagicEffectOperationStatus.Applied;
	}

	public IMagicSpellEffectPreparedSelectionToken? CapturePreparedSelection(ICharacter caster, IPerceivable recipient) =>
		OwnedFamily is null || recipient is not ICharacter target ? null : _selection = new(target, target.Body);

	public bool TryReusePreparedSelection(IMagicSpellEffectPreparedSelectionToken selection, ICharacter caster,
		IPerceivable recipient, out string? error)
	{
		error = null;
		if (OwnedFamily is null) return true;
		if (selection is not ConditionBodySelection body || !body.IsCurrent(recipient))
		{ error = "The selected poison cure recipient or physical body changed."; return false; }
		_selection = body;
		return true;
	}

	public bool TryConfirmPreparedSelection(ICharacter caster, IPerceivable recipient, out string? error)
	{
		error = OwnedFamily is not null && (_selection is null || !_selection.IsCurrent(recipient))
			? "The selected poison cure recipient or physical body changed." : null;
		return error is null;
	}

	public MagicEffectOperation Apply(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
	{
		if (OwnedFamily is null)
		{
			GetOrApplyEffect(caster, target, outcome, power, parent, additionalParameters);
			return new(MagicEffectOperationStatus.Unknown, null);
		}
		return new(target is ICharacter character ? ApplyOwnedRemoval(character) : MagicEffectOperationStatus.Rejected, null);
	}

	public override IMagicSpellEffectTemplate Clone() => new RemovePoisonEffect(SaveToXml(), Spell);
}

public class DiseaseEffect : CharacterSpellEffectTemplateBase, IMagicSpellEffectOperation, IMagicSpellEffectPreparedSelection
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
	public ITraitExpression IntensityExpression { get; set; } = null!;
	public ITraitExpression VirulenceExpression { get; set; } = null!;
	public string? OwnedFamily { get; private set; }
	public DrugVector ExposureVector { get; private set; }
	private ConditionBodySelection? _selection;
	public string? DefinitionError => OwnedFamily is not null &&
		(string.IsNullOrWhiteSpace(OwnedFamily) || InfectionType != InfectionType.SownSickness ||
		 ExposureVector != DrugVector.Touched || VirulenceDifficulty != Difficulty.Normal)
		? "Owned disease requires a nonempty family, SownSickness, declared contact exposure and Normal native virulence difficulty." : null;

	protected override string BuilderEffectType => "disease";
	protected override string ShowText =>
		$"Disease - {InfectionType.Describe()} @ {VirulenceDifficulty.DescribeEnum()} intensity {IntensityExpression.OriginalFormulaText} virulence {VirulenceExpression.OriginalFormulaText}";

	public override string Show(ICharacter actor)
	{
		return SpellEffectPresentation.Describe(actor, "Disease",
			("Type", InfectionType.Describe().ColourValue()),
			("Virulence Difficulty", VirulenceDifficulty.DescribeEnum().ColourValue()),
			("Intensity", IntensityExpression.OriginalFormulaText.ColourCommand()),
			("Virulence", VirulenceExpression.OriginalFormulaText.ColourCommand()));
	}

	protected override void LoadFromXml(XElement root)
	{
		InfectionType = (InfectionType)int.Parse(root.Element("InfectionType")?.Value ?? ((int)InfectionType.Simple).ToString());
		VirulenceDifficulty = (Difficulty)int.Parse(root.Element("VirulenceDifficulty")?.Value ?? ((int)Difficulty.Normal).ToString());
		IntensityExpression = new TraitExpression(root.Element("IntensityFormula")?.Value ?? "0", Gameworld);
		VirulenceExpression = new TraitExpression(root.Element("VirulenceFormula")?.Value ?? "1", Gameworld);
		OwnedFamily = root.Element("OwnedFamily")?.Value;
		ExposureVector = (DrugVector)((int?)root.Element("ExposureVector") ?? 0);
	}

	protected override void SaveToXml(XElement root)
	{
		root.Add(new XElement("InfectionType", (int)InfectionType));
		root.Add(new XElement("VirulenceDifficulty", (int)VirulenceDifficulty));
		root.Add(new XElement("IntensityFormula", new XCData(IntensityExpression.OriginalFormulaText)));
		root.Add(new XElement("VirulenceFormula", new XCData(VirulenceExpression.OriginalFormulaText)));
		if (OwnedFamily is not null)
		{
			root.Add(new XElement("OwnedFamily", OwnedFamily));
			root.Add(new XElement("ExposureVector", (int)ExposureVector));
		}
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
		if (DefinitionError is { } error) throw new InvalidOperationException(error);
		if (OwnedFamily is not null && _selection is { } selected && !selected.IsCurrent(target))
			throw new InvalidOperationException("The selected disease recipient or physical body changed before application.");
		double intensity = IntensityExpression.EvaluateWith(caster, values: new (string, object)[]
		{
			("power", (int)power),
			("outcome", (int)outcome)
		});
		if (OwnedFamily is not null && (!double.IsFinite(intensity) || intensity is < 1 or > 7))
			throw new InvalidOperationException("Owned sickness requires selected grade intensity 1..7.");
		if (intensity <= 0.0)
		{
			return null;
		}

		double virulence = VirulenceExpression.EvaluateWith(caster, values: new (string, object)[]
		{
			("power", (int)power),
			("outcome", (int)outcome)
		});
		if (OwnedFamily is not null && (!double.IsFinite(intensity) || intensity is < 1 or > 7 ||
			!double.IsFinite(virulence) || virulence != 1))
			throw new InvalidOperationException("Owned sickness requires selected grade intensity 1..7 and native virulence multiplier one.");
		return new SpellDiseaseEffect(target, parent, InfectionType, VirulenceDifficulty, intensity,
			System.Math.Max(0.0, virulence), IntensityExpression.OriginalFormulaText,
			VirulenceExpression.OriginalFormulaText, ownedFamily: OwnedFamily, exposureVector: ExposureVector,
			ownedBody: _selection?.Body ?? target.Body);
	}

	public IMagicSpellEffectPreparedSelectionToken? CapturePreparedSelection(ICharacter caster, IPerceivable recipient) =>
		OwnedFamily is null || recipient is not ICharacter target ? null : _selection = new(target, target.Body);
	public bool TryReusePreparedSelection(IMagicSpellEffectPreparedSelectionToken selection, ICharacter caster,
		IPerceivable recipient, out string? error)
	{
		error = null;
		if (OwnedFamily is null) return true;
		if (selection is not ConditionBodySelection body || !body.IsCurrent(recipient))
		{ error = "The selected disease recipient or physical body changed."; return false; }
		_selection = body;
		return true;
	}
	public bool TryConfirmPreparedSelection(ICharacter caster, IPerceivable recipient, out string? error)
	{
		error = OwnedFamily is not null && (_selection is null || !_selection.IsCurrent(recipient))
			? "The selected disease recipient or physical body changed." : null;
		return error is null;
	}
	public MagicEffectOperation Apply(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
	{
		var child = GetOrApplyEffect(caster, target, outcome, power, parent, additionalParameters);
		return new(OwnedFamily is null ? MagicEffectOperationStatus.Unknown :
			child is null ? MagicEffectOperationStatus.NoChange : MagicEffectOperationStatus.Applied, child);
	}

	public override IMagicSpellEffectTemplate Clone() => new DiseaseEffect(SaveToXml(), Spell);
}

internal sealed record ConditionBodySelection(ICharacter Character, IBody Body) : IMagicSpellEffectPreparedSelectionToken
{
	public bool IsCurrent(IPerceivable recipient) => ReferenceEquals(Character, recipient) && ReferenceEquals(Character.Body, Body);
}

public class RemoveDiseaseEffect : CharacterSpellEffectRemovalTemplateBase, IMagicSpellEffectOperation, IMagicSpellEffectPreparedSelection
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
	public string? OwnedFamily { get; private set; }
	public DrugVector ExposureVector { get; private set; }
	private ConditionBodySelection? _selection;
	public string? DefinitionError => OwnedFamily is not null &&
		(string.IsNullOrWhiteSpace(OwnedFamily) || InfectionType != InfectionType.SownSickness || ExposureVector != DrugVector.Touched)
		? "Owned disease cure requires a nonempty family, SownSickness and declared contact exposure." : null;

	protected override string BuilderEffectType => "removedisease";
	protected override string ShowText =>
		$"Remove Disease - {InfectionType.Describe()} @ {VirulenceDifficulty.DescribeEnum()} intensity {IntensityFormulaText} virulence {VirulenceFormulaText}";

	public override string Show(ICharacter actor)
	{
		return SpellEffectPresentation.Describe(actor, "Remove Disease",
			("Type", InfectionType.Describe().ColourValue()),
			("Virulence Difficulty", VirulenceDifficulty.DescribeEnum().ColourValue()),
			("Intensity", IntensityFormulaText.ColourCommand()),
			("Virulence", VirulenceFormulaText.ColourCommand()));
	}

	protected override void LoadFromXml(XElement root)
	{
		InfectionType = (InfectionType)int.Parse(root.Element("InfectionType")?.Value ?? ((int)InfectionType.Simple).ToString());
		VirulenceDifficulty = (Difficulty)int.Parse(root.Element("VirulenceDifficulty")?.Value ?? ((int)Difficulty.Normal).ToString());
		IntensityFormulaText = root.Element("IntensityFormula")?.Value ?? "0";
		VirulenceFormulaText = root.Element("VirulenceFormula")?.Value ?? "1";
		OwnedFamily = root.Element("OwnedFamily")?.Value;
		ExposureVector = (DrugVector)((int?)root.Element("ExposureVector") ?? 0);
	}

	protected override void SaveToXml(XElement root)
	{
		root.Add(new XElement("InfectionType", (int)InfectionType));
		root.Add(new XElement("VirulenceDifficulty", (int)VirulenceDifficulty));
		root.Add(new XElement("IntensityFormula", new XCData(IntensityFormulaText)));
		root.Add(new XElement("VirulenceFormula", new XCData(VirulenceFormulaText)));
		if (OwnedFamily is not null)
		{
			root.Add(new XElement("OwnedFamily", OwnedFamily));
			root.Add(new XElement("ExposureVector", (int)ExposureVector));
		}
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
		if (OwnedFamily is not null)
		{
			ApplyOwnedRemoval(target);
			return;
		}
		target.RemoveAllEffects<SpellDiseaseEffect>(x =>
			x.InfectionType == InfectionType &&
			x.VirulenceDifficulty == VirulenceDifficulty &&
			x.IntensityFormulaText.EqualTo(IntensityFormulaText) &&
			x.VirulenceFormulaText.EqualTo(VirulenceFormulaText), true);
	}

	private MagicEffectOperationStatus ApplyOwnedRemoval(ICharacter target)
	{
		if (DefinitionError is { } error) throw new InvalidOperationException(error);
		var selected = _selection ?? new ConditionBodySelection(target, target.Body);
		if (!selected.IsCurrent(target)) return MagicEffectOperationStatus.Rejected;
		var matches = target.EffectsOfType<SpellDiseaseEffect>().Where(x => x.OwnedFamily == OwnedFamily &&
			x.ExposureVector == ExposureVector && x.InfectionType == InfectionType &&
			ReferenceEquals(x.OwnedBody, selected.Body)).ToArray();
		if (!selected.IsCurrent(target)) return MagicEffectOperationStatus.Rejected;
		if (matches.Length == 0) return MagicEffectOperationStatus.NoChange;
		foreach (var effect in matches)
		{
			if (!selected.IsCurrent(target)) throw new InvalidOperationException("Disease cure recipient changed after a paid removal.");
			target.RemoveEffect(effect, true);
			if (!selected.IsCurrent(target)) throw new InvalidOperationException("Disease cure recipient changed during a paid removal callback.");
		}
		return MagicEffectOperationStatus.Applied;
	}
	public IMagicSpellEffectPreparedSelectionToken? CapturePreparedSelection(ICharacter caster, IPerceivable recipient) =>
		OwnedFamily is null || recipient is not ICharacter target ? null : _selection = new(target, target.Body);
	public bool TryReusePreparedSelection(IMagicSpellEffectPreparedSelectionToken selection, ICharacter caster,
		IPerceivable recipient, out string? error)
	{
		error = null;
		if (OwnedFamily is null) return true;
		if (selection is not ConditionBodySelection body || !body.IsCurrent(recipient))
		{ error = "The selected disease cure recipient or physical body changed."; return false; }
		_selection = body;
		return true;
	}
	public bool TryConfirmPreparedSelection(ICharacter caster, IPerceivable recipient, out string? error)
	{
		error = OwnedFamily is not null && (_selection is null || !_selection.IsCurrent(recipient))
			? "The selected disease cure recipient or physical body changed." : null;
		return error is null;
	}
	public MagicEffectOperation Apply(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
	{
		if (OwnedFamily is null)
		{
			GetOrApplyEffect(caster, target, outcome, power, parent, additionalParameters);
			return new(MagicEffectOperationStatus.Unknown, null);
		}
		return new(target is ICharacter character ? ApplyOwnedRemoval(character) : MagicEffectOperationStatus.Rejected, null);
	}

	public override IMagicSpellEffectTemplate Clone() => new RemoveDiseaseEffect(SaveToXml(), Spell);
}
