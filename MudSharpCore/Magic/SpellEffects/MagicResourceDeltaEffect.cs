#nullable enable
using ExpressionEngine;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Magic.Casting;
using MudSharp.Body.Traits;
using MudSharp.Magic;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.SpellEffects;

public class MagicResourceDeltaEffect : IMagicSpellEffectTemplate, IMagicSpellEffectOperation, IMagicSpellEffectPreparedSelection
{
	private string? _exactLoadError;
	private XElement? _invalidExactDefinition;
	private long _configuredResourceId;
	private bool _applyingExact;
	private readonly Dictionary<IPerceivable, ResourceSelection> _selections = new(ReferenceEqualityComparer.Instance);
	private sealed record ResourceSelection(IPerceivable Target, IHaveMagicResource Holder, IMagicResource Resource,
		long ResourceId, IFuturemud World, bool CastingReserve, ICharacter? Character, IBody? Body,
		long InstanceId, long BodyId, ICharacterIdentity? Identity, ICharacter? Primary,
		ICharacter? HolderCharacter, ICharacterIdentity? HolderIdentity, ICharacter? HolderPrimary)
		: IMagicSpellEffectPreparedSelectionRawToken
	{
		public bool IsCurrent => ReferenceEquals(Target.Gameworld, World) && Resource.Id == ResourceId &&
			ReferenceEquals(World.MagicResources.Get(ResourceId), Resource) &&
			(Character is null || MagicCastingService.IsConfiguredReserve(World, ResourceId) == CastingReserve &&
				Body?.Id == BodyId && ReferenceEquals(Character.Body, Body) && ReferenceEquals(Body?.Actor, Character) && Character.InstanceId == InstanceId &&
				ReferenceEquals(Character.Identity, Identity) && ReferenceEquals(Identity?.PrimaryInstance, Primary) &&
				ReferenceEquals(CastingReserve ? Primary ?? Character : Character, Holder)) &&
			(HolderCharacter is null || ReferenceEquals(HolderCharacter.Identity, HolderIdentity) &&
				ReferenceEquals(HolderIdentity?.PrimaryInstance, HolderPrimary) &&
				(!CastingReserve || ReferenceEquals(HolderPrimary ?? HolderCharacter, HolderCharacter)));
	}

	public const string HelpText = @"You can use the following options with this effect:

	#3resource <which>#0 - sets which magic resource is changed
	#3formula <expr>#0 - sets the amount delta formula
	#3exactdepletion true|false#0 - removes only the available amount from the exact native reserve, with no transfer or negative balance";

	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("magicresourcedelta",
			(root, spell) => new MagicResourceDeltaEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("magicresourcedelta", BuilderFactory,
			"Instantly adds or removes magic resource from a character, item or room",
			HelpText,
			true,
			true,
			StandaloneSpellEffectTemplateHelper.MagicResourceTriggerTypes);
	}

	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands,
		IMagicSpell spell)
	{
		return (new MagicResourceDeltaEffect(new XElement("Effect",
			new XAttribute("type", "magicresourcedelta"),
			new XElement("Resource", 0L),
			new XElement("Formula", new XCData("power*outcome"))
		), spell), string.Empty);
	}

	protected MagicResourceDeltaEffect(XElement root, IMagicSpell spell)
	{
		Spell = spell;
		LoadFromXml(root);
	}

	public IMagicSpell Spell { get; }
	public IFuturemud Gameworld => Spell.Gameworld;
	public IMagicResource? Resource { get; private set; }
	public ITraitExpression DeltaExpression { get; internal set; } = null!;
	public bool ExactResourceDepletion { get; private set; }
	public string? DefinitionError
	{
		get
		{
			if (_exactLoadError is not null) return _exactLoadError;
			if (!ExactResourceDepletion) return null;
			if (Resource is null || _configuredResourceId <= 0 || Resource.Id != _configuredResourceId)
				return "Exact resource depletion requires a resolved positive native resource identity.";
			if (DeltaExpression is null || DeltaExpression.HasErrors()) return "Exact resource depletion requires a valid finite nonpositive formula.";
			if (DeltaExpression is TraitExpression native && native.Parameters.Count == 0 &&
				!native.Formula.ParameterNames.Any() && !native.Formula.FunctionNames.Any() &&
				(!native.Formula.TryEvaluateDoubleWith(new Dictionary<string, object>(), out var result, out _) || result > 0))
				return "Exact resource depletion requires a finite nonpositive formula result.";
			return null;
		}
	}

	private void LoadFromXml(XElement root)
	{
		var resourceText = root.Element("Resource")?.Value ?? "0";
		Resource = long.TryParse(resourceText, out var value)
			? Gameworld.MagicResources.Get(value)
			: Gameworld.MagicResources.GetByIdOrName(resourceText);
		DeltaExpression = new TraitExpression(root.Element("Formula")?.Value ?? "0", Gameworld);
		_configuredResourceId = long.TryParse(resourceText, out var resourceId) ? resourceId : 0;
		if (root.Element("ExactResourceDepletion") is { } mode)
		{
			if (!bool.TryParse(mode.Value, out var exact)) _exactLoadError = "ExactResourceDepletion must be an explicit boolean.";
			ExactResourceDepletion = exact;
			if (exact && (_configuredResourceId <= 0 || Resource is null || Resource.Id != _configuredResourceId))
				_exactLoadError = "Exact resource depletion requires a resolved positive native resource identity.";
			if (_exactLoadError is not null) _invalidExactDefinition = new XElement(root);
		}
	}

	public XElement SaveToXml()
	{
		if (_invalidExactDefinition is not null) return new XElement(_invalidExactDefinition);
		return new XElement("Effect",
			new XAttribute("type", "magicresourcedelta"),
			new XElement("Resource", Resource?.Id ?? 0L),
			new XElement("Formula", new XCData(DeltaExpression.OriginalFormulaText)),
			ExactResourceDepletion ? new XElement("ExactResourceDepletion", true) : null
		);
	}

	public bool IsInstantaneous => true;
	public bool RequiresTarget => true;

	public bool IsCompatibleWithTrigger(IMagicTrigger types)
	{
		return StandaloneSpellEffectTemplateHelper.IsMagicResourceTarget(types.TargetTypes);
	}

	public IMagicSpellEffect? GetOrApplyEffect(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
	{
		if (DefinitionError is { } error) throw new InvalidOperationException(error);
		if (ExactResourceDepletion) { Apply(caster, target, outcome, power, parent, additionalParameters); return null; }
		if (target is not IHaveMagicResource targetResource || Resource is null)
		{
			return null;
		}

		var delta = DeltaExpression.EvaluateWith(caster, values: new (string, object)[]
		{
			("power", (int)power),
			("outcome", (int)outcome)
		});
		if (delta == 0.0)
		{
			return null;
		}

		targetResource.AddResource(Resource, delta);
		return null;
	}

	public IMagicSpellEffectTemplate Clone()
	{
		return new MagicResourceDeltaEffect(SaveToXml(), Spell);
	}

	public string Show(ICharacter actor)
	{
		return SpellEffectPresentation.Describe(actor, "Magic Resource Delta",
			("Resource", Resource?.Name.ColourValue() ?? "None".ColourError()),
			("Formula", DeltaExpression.OriginalFormulaText.ColourCommand()),
			("Exact Native Depletion", ExactResourceDepletion.ToColouredString()),
			("Validation", DefinitionError?.ColourError() ?? "Valid".ColourValue()));
	}

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "resource":
				return BuildingCommandResource(actor, command);
			case "formula":
				return BuildingCommandFormula(actor, command);
			case "exactdepletion":
				if (!bool.TryParse(command.SafeRemainingArgument, out var exact) || exact && Resource is null)
				{
					actor.OutputHandler.Send("Use true or false; exact depletion requires a resolved native resource."); return false;
				}
				var previous = ExactResourceDepletion; var loadError = _exactLoadError; var definition = _invalidExactDefinition;
				ExactResourceDepletion = exact; _exactLoadError = null; _invalidExactDefinition = null;
				if (DefinitionError is { } error)
				{
					ExactResourceDepletion = previous; _exactLoadError = loadError; _invalidExactDefinition = definition;
					actor.OutputHandler.Send(error.ColourError()); return false;
				}
				Spell.Changed = true;
				actor.OutputHandler.Send($"Exact native reserve depletion is now {exact.ToColouredString()}."); return true;
		}

		actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
		return false;
	}

	private bool BuildingCommandResource(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which magic resource should this effect change?");
			return false;
		}

		var resource = Gameworld.MagicResources.GetByIdOrName(command.SafeRemainingArgument);
		if (resource is null)
		{
			actor.OutputHandler.Send("There is no such magic resource.");
			return false;
		}

		Resource = resource; _configuredResourceId = resource.Id;
		_exactLoadError = null; _invalidExactDefinition = null;
		Spell.Changed = true;
		actor.OutputHandler.Send(
			$"This effect will now change the {resource.Name.ColourValue()} magic resource.");
		return true;
	}

	private bool BuildingCommandFormula(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What formula should determine the resource change?");
			return false;
		}

		var expression = new TraitExpression(command.SafeRemainingArgument, Gameworld);
		if (expression.HasErrors())
		{
			actor.OutputHandler.Send(expression.Error);
			return false;
		}

		var previous = DeltaExpression; DeltaExpression = expression;
		if (DefinitionError is { } error) { DeltaExpression = previous; actor.OutputHandler.Send(error.ColourError()); return false; }
		Spell.Changed = true;
		actor.OutputHandler.Send(
			$"This effect will now use the formula {expression.OriginalFormulaText.ColourCommand()}.");
		return true;
	}
	public IMagicSpellEffectPreparedSelectionToken? CapturePreparedSelection(ICharacter caster, IPerceivable recipient)
	{
		if (!ExactResourceDepletion || Resource is null || recipient is not IHaveMagicResource) return null;
		if (!_selections.TryGetValue(recipient, out var selected))
		{
			var character = recipient as ICharacter;
			var identity = character?.Identity; var primary = identity?.PrimaryInstance;
			var reserve = character is not null && MagicCastingService.IsConfiguredReserve(Gameworld, Resource.Id);
			var holder = character is not null && reserve ? primary ?? character : (IHaveMagicResource)recipient;
			var holderCharacter = holder as ICharacter; var holderIdentity = holderCharacter?.Identity;
			selected = new(recipient, holder, Resource, Resource.Id, Gameworld, reserve, character, character?.Body,
				character?.InstanceId ?? 0, character?.Body?.Id ?? 0, identity, primary, holderCharacter, holderIdentity, holderIdentity?.PrimaryInstance);
			_selections[recipient] = selected;
		}
		return selected;
	}
	public bool TryReusePreparedSelection(IMagicSpellEffectPreparedSelectionToken selection, ICharacter caster,
		IPerceivable recipient, out string? error)
	{
		error = null;
		if (!ExactResourceDepletion) return true;
		if (selection is not ResourceSelection selected || !ReferenceEquals(selected.Target, recipient) ||
			!ReferenceEquals(selected.Resource, Resource) || !selected.IsCurrent)
		{
			error = "The selected reserve recipient, physical body, resource definition or actual holder changed."; return false;
		}
		_selections[recipient] = selected; return true;
	}
	public bool TryConfirmPreparedSelection(ICharacter caster, IPerceivable recipient, out string? error)
	{
		error = DefinitionError;
		if (error is not null) return false;
		if (!ExactResourceDepletion) return true;
		if (!_selections.TryGetValue(recipient, out var selected) || !SelectionCurrent(selected) ||
			!TryReadBalance(selected, out var balance) || balance < 0 || !SelectionCurrent(selected))
			error = "The selected reserve recipient, physical body, definition, holder or balance is invalid or changed.";
		return error is null;
	}
	private bool SelectionCurrent(ResourceSelection selected) => ReferenceEquals(selected.Resource, Resource) && selected.IsCurrent;
	private static bool TryReadBalance(ResourceSelection selected, out double balance)
	{
		balance = selected.Holder.MagicResourceAmounts.TryGetValue(selected.Resource, out var value) ? value : 0.0;
		return double.IsFinite(balance) && balance >= 0;
	}
	public MagicEffectOperation Apply(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
	{
		if (DefinitionError is { } error) throw new InvalidOperationException(error);
		if (!ExactResourceDepletion)
		{
			GetOrApplyEffect(caster, target, outcome, power, parent, additionalParameters);
			return new(MagicEffectOperationStatus.Unknown, null);
		}
		if (target is null || target is not IHaveMagicResource) return new(MagicEffectOperationStatus.Rejected, null);
		var selected = (ResourceSelection?)CapturePreparedSelection(caster, target);
		if (selected is null || !SelectionCurrent(selected)) return new(MagicEffectOperationStatus.Rejected, null);
		if (_applyingExact) throw new InvalidOperationException("Exact reserve depletion cannot be re-entered during a native callback.");
		_applyingExact = true;
		try
		{
			var expression = DeltaExpression;
			var delta = expression.EvaluateWith(caster, values: [("power", (int)power), ("outcome", (int)outcome)]);
			if (!double.IsFinite(delta) || delta > 0) throw new InvalidOperationException("Exact reserve depletion requires a finite nonpositive evaluated amount.");
			if (!SelectionCurrent(selected) || !ReferenceEquals(expression, DeltaExpression) || !TryReadBalance(selected, out var before) || !SelectionCurrent(selected))
				return new(MagicEffectOperationStatus.Rejected, null);
			var amount = Math.Min(before, -delta);
			// Absence of this exact reserve is a no-op. Do not invent a key or substitute another balance.
			if (amount == 0) return new(MagicEffectOperationStatus.NoChange, null);
			if (!MagicResourceCapacity.TryGetCap(selected.Resource, selected.Holder, out var capacity, out _) ||
				!SelectionCurrent(selected) || !TryReadBalance(selected, out var admitted) || admitted != before ||
				selected.CastingReserve && before > capacity || !SelectionCurrent(selected))
				return new(MagicEffectOperationStatus.Rejected, null);
			var expected = before - amount;
			var admission = new MagicResourceCapacityAdmission([new(selected.Holder, selected.Resource, amount, capacity)]);
			if (!SelectionCurrent(selected) || !ReferenceEquals(expression, DeltaExpression)) return new(MagicEffectOperationStatus.Rejected, null);
			bool debited;
			// Capacity has already run its live callbacks and passed raw fences. The native debit
			// receives this one sampled capacity, preserving native holder/dirty-state behavior.
			using (admission.OpenScope()) debited = selected.Holder.UseResource(selected.Resource, amount);
			var validBalance = TryReadBalance(selected, out var after);
			if (!SelectionCurrent(selected) || !ReferenceEquals(expression, DeltaExpression) || !validBalance ||
				after != before && after != expected || !debited && after != before)
				throw new InvalidOperationException("Reserve state changed during a native debit callback; the paid write is uncertain.");
			return new(after < before ? MagicEffectOperationStatus.Applied : debited ? MagicEffectOperationStatus.NoChange : MagicEffectOperationStatus.Rejected, null);
		}
		finally { _applyingExact = false; }
	}

}
