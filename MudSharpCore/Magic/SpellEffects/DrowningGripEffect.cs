#nullable enable

using System;
using System.Xml.Linq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Magic.Casting;
using MudSharp.NPC.AI;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.SpellEffects;

/// <summary>Applies a temporary native breathing interruption to an exact living body.</summary>
public sealed class DrowningGripEffect : CharacterSpellEffectTemplateBase, IMagicSpellEffectPreparedSelection
{
	private Selection? _selection;

	private sealed record Selection(ICharacter Caster, ICharacter Victim, IBody Body, IRoom Room,
		RoomLayer Layer, long CasterInstance, long VictimInstance) : IMagicSpellEffectPreparedSelectionRawToken
	{
		public bool IsCurrent => ReferenceEquals(Victim.Body, Body) && ReferenceEquals(Body.Actor, Victim) &&
			ReferenceEquals(Caster.Location, Room) && ReferenceEquals(Victim.Location, Room) &&
			Caster.RoomLayer == Layer && Victim.RoomLayer == Layer &&
			Caster.InstanceId == CasterInstance && Victim.InstanceId == VictimInstance &&
			CommandExecutionAuthority.IsCurrent(Caster, false) &&
			CommandExecutionAuthority.IsCurrent(Victim, false) &&
			Body.NeedsToBreathe && Body.CanBreathe;
	}

	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("drowninggrip", (root, spell) => new DrowningGripEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("drowninggrip", (_, spell) =>
			(new DrowningGripEffect(new XElement("Effect", new XAttribute("type", "drowninggrip")), spell), string.Empty),
			"Temporarily interrupts a character's native breathing", "No effect options; set the spell duration and resistance normally.",
			false, true, StandaloneSpellEffectTemplateHelper.CharacterTriggerTypes);
	}

	internal DrowningGripEffect(XElement root, IMagicSpell spell) : base(root, spell) { }

	protected override string BuilderEffectType => "drowninggrip";
	protected override string ShowText => "Drowning Grip";

	public IMagicSpellEffectPreparedSelectionToken CapturePreparedSelection(ICharacter caster, IPerceivable recipient)
	{
		if (recipient is not ICharacter victim || caster.Location is not IRoom room ||
			victim.Body is not IBody body || !ReferenceEquals(victim.Location, room))
			throw new InvalidOperationException("Drowning Grip requires an exact local breathing character.");
		var selection = new Selection(caster, victim, body, room, caster.RoomLayer, caster.InstanceId, victim.InstanceId);
		if (!selection.IsCurrent)
			throw new InvalidOperationException("Drowning Grip requires a current, breathing target in the caster's layer.");
		_selection = selection;
		return selection;
	}

	public bool TryReusePreparedSelection(IMagicSpellEffectPreparedSelectionToken selection, ICharacter caster,
		IPerceivable recipient, out string? error)
	{
		error = "The Drowning Grip target's body, location or breathing state changed.";
		if (selection is not Selection token || !ReferenceEquals(token.Caster, caster) ||
			!ReferenceEquals(token.Victim, recipient) || !token.IsCurrent) return false;
		_selection = token;
		error = null;
		return true;
	}

	public bool TryConfirmPreparedSelection(ICharacter caster, IPerceivable recipient, out string? error) =>
		TryReusePreparedSelection(_selection!, caster, recipient, out error);

	protected override IMagicSpellEffect CreateEffect(ICharacter caster, ICharacter target,
		OpposedOutcomeDegree outcome, SpellPower power, IMagicSpellEffectParent parent,
		SpellAdditionalParameter[] additionalParameters)
	{
		if (!TryConfirmPreparedSelection(caster, target, out var error))
			throw new InvalidOperationException(error);
		return new SpellDrowningGripEffect(target, parent);
	}

	public override IMagicSpellEffectTemplate Clone() => new DrowningGripEffect(SaveToXml(), Spell);
}
