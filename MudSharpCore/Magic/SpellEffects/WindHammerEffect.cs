using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Framework;
using MudSharp.Movement;
using MudSharp.NPC.AI;
using MudSharp.RPG.Checks;

#nullable enable

namespace MudSharp.Magic.SpellEffects;

/// <summary>A native strike and one selected exit step, linked by the strike's actual operation result.</summary>
public sealed class WindHammerEffect : IMagicSpellEffectTemplate, IMagicSpellEffectOperation,
	IMagicSpellEffectInvocationInputs, IMagicSpellEffectPreparedSelection
{
	private readonly DamageEffect _damage;
	private readonly ForcedPathMovementEffect _movement;
	private readonly Dictionary<IPerceivable, Selection> _selections = new(ReferenceEqualityComparer.Instance);
	private IRoomExit? _boundExit;

	private sealed record Selection(ICharacter Caster, ICharacter Victim, IRoomExit Exit,
		IMagicSpellEffectPreparedSelectionToken Damage, string Definition, Func<bool> ExitCurrent)
		: IMagicSpellEffectPreparedSelectionRawToken
	{
		public bool IsCurrent => ExitCurrent() && Damage is IMagicSpellEffectPreparedSelectionRawToken raw && raw.IsCurrent;
	}

	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("windhammer", (root, spell) => new WindHammerEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("windhammer", (_, spell) =>
			(new WindHammerEffect(DefaultDefinition(), spell), string.Empty),
			"Strikes a character, then forces one legal step through the selected exit only on an applied hit",
			HelpText, true, true, WindSpellEffectTemplateHelper.CharacterExitTriggerTypes);
	}

	private static XElement DefaultDefinition() => new("Effect", new XAttribute("type", "windhammer"),
		new XAttribute("schema", 1), new XElement("Damage", new XElement("Effect", new XAttribute("type", "damage"),
			new XElement("DamageType", (int)MudSharp.Health.DamageType.Shockwave),
			new XElement("DamageExpression", "5*grade"), new XElement("PainExpression", "0"),
			new XElement("StunExpression", "0"), new XElement("Bodypart", 0), new XElement("Limb", -1),
			new XElement("IdentitySafeDamage", new XAttribute("schema", 1), new XAttribute("version", 1)))));

	private WindHammerEffect(XElement root, IMagicSpell spell)
	{
		if ((string?)root.Attribute("type") != "windhammer" || (int?)root.Attribute("schema") != 1 ||
			root.Elements("Damage").Count() != 1 || root.Element("Damage")!.Elements().Count() != 1)
			throw new FormatException("Wind Hammer requires one schema-1 native damage child.");
		Spell = spell;
		_damage = SpellEffectFactory.LoadEffect(root.Element("Damage")!.Elements().Single(), spell) as DamageEffect ??
			throw new FormatException("Wind Hammer requires a native damage child.");
		_movement = SpellEffectFactory.LoadEffect(new XElement("Effect", new XAttribute("type", "forcedpathmovement"),
			new XElement("Steps", 1), new XElement("AllowFallExits", false)), spell) as ForcedPathMovementEffect ??
			throw new InvalidOperationException("The native one-step movement adapter is unavailable.");
	}

	public IMagicSpell Spell { get; }
	public IFuturemud Gameworld => Spell.Gameworld;
	public DamageEffect Damage => _damage;
	public bool IsInstantaneous => true;
	public bool RequiresTarget => true;
	public string? DefinitionError => !_damage.IdentitySafeDamage
		? "Wind Hammer requires frozen native damage identity and custody."
		: _damage.DefinitionError;

	public XElement SaveToXml() => new("Effect", new XAttribute("type", "windhammer"),
		new XAttribute("schema", 1), new XElement("Damage", _damage.SaveToXml()));

	public bool IsCompatibleWithTrigger(IMagicTrigger trigger) => trigger.TargetTypes == "character&exit";

	public bool TryBindInvocationInputs(ICharacter caster, IPerceivable recipient,
		IReadOnlyList<SpellAdditionalParameter> parameters, out string? error)
	{
		error = DefinitionError;
		if (error is not null) return false;
		if (recipient is not ICharacter victim || parameters.Count != 1 ||
			!parameters[0].ParameterName.EqualTo("exit") || parameters[0].Item is not IRoomExit exit ||
			_boundExit is not null && !ReferenceEquals(_boundExit, exit) ||
			!ReferenceEquals(caster.Location, victim.Location) || caster.RoomLayer != victim.RoomLayer ||
			!ReferenceEquals(exit.Origin, victim.Location) || exit.IsFallExit)
		{
			error = "Wind Hammer requires one exact local character and a non-fall exit from the target's room.";
			return false;
		}
		_boundExit = exit;
		error = null;
		return true;
	}

	public IMagicSpellEffectPreparedSelectionToken CapturePreparedSelection(ICharacter caster, IPerceivable recipient)
	{
		if (_selections.TryGetValue(recipient, out var existing))
		{
			if (!TryConfirmPreparedSelection(caster, recipient, out var reason)) throw new InvalidOperationException(reason);
			return existing;
		}
		if (_boundExit is not { } exit || recipient is not ICharacter victim ||
			!ReferenceEquals(caster.Location, victim.Location) || !ReferenceEquals(exit.Origin, victim.Location))
			throw new InvalidOperationException("Wind Hammer needs the exact bound character and local exit before payment.");
		var damage = _damage.CapturePreparedSelection(caster, recipient) ??
			throw new InvalidOperationException("Wind Hammer needs a frozen native damage selection.");
		var origin = exit.Origin;
		var destination = exit.Destination;
		var nativeExit = exit.Exit;
		var layer = victim.RoomLayer;
		var casterLayer = caster.RoomLayer;
		bool ExitCurrent() => ReferenceEquals(_boundExit, exit) && ReferenceEquals(exit.Exit, nativeExit) &&
			ReferenceEquals(exit.Origin, origin) && ReferenceEquals(exit.Destination, destination) &&
			ReferenceEquals(caster.Location, origin) && ReferenceEquals(victim.Location, origin) &&
			caster.RoomLayer == casterLayer && victim.RoomLayer == layer && !exit.IsFallExit;
		var selection = new Selection(caster, victim, exit, damage,
			SaveToXml().ToString(SaveOptions.DisableFormatting), ExitCurrent);
		_selections.Add(recipient, selection);
		if (!TryConfirmPreparedSelection(caster, recipient, out var error))
			throw new InvalidOperationException(error);
		return selection;
	}

	public bool TryReusePreparedSelection(IMagicSpellEffectPreparedSelectionToken selection,
		ICharacter caster, IPerceivable recipient, out string? error)
	{
		error = "The selected Wind Hammer character, exit or damage body changed.";
		if (selection is not Selection chosen || !ReferenceEquals(caster, chosen.Caster) ||
			!ReferenceEquals(recipient, chosen.Victim) || !ReferenceEquals(_boundExit, chosen.Exit) ||
			SaveToXml().ToString(SaveOptions.DisableFormatting) != chosen.Definition || !chosen.IsCurrent ||
			!_damage.TryReusePreparedSelection(chosen.Damage, caster, recipient, out _)) return false;
		_selections[recipient] = chosen;
		return TryConfirmPreparedSelection(caster, recipient, out error);
	}

	public bool TryConfirmPreparedSelection(ICharacter caster, IPerceivable recipient, out string? error)
	{
		error = "The selected Wind Hammer character, exit or damage body changed.";
		if (!_selections.TryGetValue(recipient, out var selection) ||
			!ReferenceEquals(caster, selection.Caster) || !selection.IsCurrent ||
			!_damage.TryConfirmPreparedSelection(caster, recipient, out _)) return false;
		var exit = selection.Exit;
		var victim = selection.Victim;
		if (!exit.WhichLayersExitAppears().Contains(victim.RoomLayer) ||
			!exit.Origin.ExitsFor(victim).Any(candidate => ReferenceEquals(candidate, exit)) ||
			exit.MovementTransition(victim).TransitionType == RoomMovementTransition.NoViableTransition ||
			!victim.CanMove(exit, CanMoveFlags.IgnoreWhetherExitCanBeCrossed |
				CanMoveFlags.IgnoreCancellableActionBlockers | CanMoveFlags.IgnoreSafeMovement) ||
			!victim.CanCross(exit).Success || !selection.IsCurrent ||
			!_damage.TryConfirmPreparedSelection(caster, recipient, out _))
		{
			error = "The selected exit is no longer a legal step for the exact damage recipient.";
			return false;
		}
		error = null;
		return true;
	}

	public MagicEffectOperation Apply(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
	{
		string? error = null;
		if (target is null || !_selections.TryGetValue(target, out var selection) ||
			additionalParameters.Length != 1 || !additionalParameters[0].ParameterName.EqualTo("exit") ||
			!ReferenceEquals(additionalParameters[0].Item, selection.Exit) ||
			!TryConfirmPreparedSelection(caster, target, out error))
			throw new InvalidOperationException(error ?? "Wind Hammer's exact prepared inputs changed before its hit.");
		var hit = _damage.Apply(caster, target, outcome, power, parent, additionalParameters);
		if (hit.Status != MagicEffectOperationStatus.Applied) return hit;
		bool StillSelected() => CommandExecutionScope.TryContinue() && selection.ExitCurrent() &&
			selection.Exit.Origin.ExitsFor(selection.Victim)
				.Any(candidate => ReferenceEquals(candidate, selection.Exit));
		if (StillSelected())
			_movement.ApplyGuardedOneStep(caster, target,
				[new SpellAdditionalParameter { ParameterName = "exit", Item = selection.Exit }], StillSelected);
		return hit;
	}

	public IMagicSpellEffect? GetOrApplyEffect(ICharacter caster, IPerceivable? target,
		OpposedOutcomeDegree outcome, SpellPower power, IMagicSpellEffectParent parent,
		SpellAdditionalParameter[] additionalParameters) =>
		Apply(caster, target, outcome, power, parent, additionalParameters).Effect;

	public IMagicSpellEffectTemplate Clone() => new WindHammerEffect(SaveToXml(), Spell);

	public string Show(ICharacter actor) => SpellEffectPresentation.Describe(actor, "Wind Hammer",
		("Damage", _damage.Show(actor)), ("Movement", "one admitted non-fall exit step after an applied hit"),
		("Validity", DefinitionError?.ColourError() ?? "valid".ColourValue()));

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || command.PopSpeech().EqualTo("identitysafe"))
		{
			actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
			return false;
		}
		return _damage.BuildingCommand(actor, command.GetUndo());
	}

	public const string HelpText = "Use the native damage type, formula, pain, stun, bodypart or limb options; the identity-safe hit and one-step movement policy are fixed.";
}
