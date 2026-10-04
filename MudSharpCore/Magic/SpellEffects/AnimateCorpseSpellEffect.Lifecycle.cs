#nullable enable

using System.Xml.Linq;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework.Scheduling;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.SpellEffects;

public sealed partial class AnimateCorpseSpellEffect : IMagicSpellEffectAdmission
{
	private XElement? _invalidLifecycle;
	private string? _lifetimeFormula;
	private double? _preparedSeconds;
	public bool DurableLifecycle { get; private set; }
	public string LifecycleFamily { get; private set; } = "";
	public ITraitExpression? LifetimeExpression { get; internal set; }
	public string? DefinitionError => _invalidLifecycle is not null ? "Invalid corpse-animation lifecycle schema." : !DurableLifecycle ? null :
		Gameworld.SpellOwnedCorpseAnimations is null ? "Durable corpse animation is unavailable." :
		string.IsNullOrWhiteSpace(LifecycleFamily) || LifecycleFamily.Length > 128 ? "Set a corpse-animation family of at most 128 characters." :
		LifetimeExpression is null || LifetimeExpression.HasErrors() || LifetimeExpression.NonTraitParameters.Contains("outcome", StringComparer.OrdinalIgnoreCase)
			? "Set a valid lifetime in real seconds, determinable before the casting check." :
		_aiIds.Count == 0 || _aiIds.Any(x => Gameworld.AIs.Get(x)?.IsReadyToBeUsed != true) ? "Select available ready corpse-animation AIs." : null;

	private void LoadLifecycle(XElement? root)
	{
		if (root is null) return;
		if ((string?)root.Attribute("version") != "1" || (string?)root.Attribute("mode") != "TemporaryCleanup")
		{ _invalidLifecycle = new(root); return; }
		DurableLifecycle = true; LifecycleFamily = root.Element("Family")?.Value ?? "";
		_lifetimeFormula = root.Element("Seconds")?.Value;
		if (_lifetimeFormula is not null) LifetimeExpression = new TraitExpression(_lifetimeFormula, Gameworld);
	}

	private XElement? SaveLifecycle() => _invalidLifecycle is not null ? new(_invalidLifecycle) : !DurableLifecycle ? null :
		new("Lifecycle", new XAttribute("version", 1), new XAttribute("mode", "TemporaryCleanup"),
			new XElement("Family", LifecycleFamily), new XElement("Seconds", _lifetimeFormula));

	internal bool ValidateInvocation(ICharacter caster, IPerceivable target, out string? error)
	{
		error = DefinitionError;
		if (error is not null || !DurableLifecycle) return error is null;
		if (!Lifecycle.SpellOwnedCorpseAnimationService.CanPersistPresentation(SavePresentation(Guid.Empty)))
		{ error = "The durable corpse-animation echoes exceed the bounded lifecycle presentation size."; return false; }
		if (Spell is not MagicSpell { InvocationGrade: not null } native)
		{ error = "Durable corpse animation requires a selected-grade native invocation."; return false; }
		if (target is not IGameItem item || item.GetItemType<ICorpse>() is not { } corpse || corpse.OriginalCharacter is not { } owner)
		{ error = "Select an eligible corpse."; return false; }
		if (!_allowFinal && corpse.RepresentsFinalCharacterDeath || !_allowSkeletal && corpse.Decay == DecayState.Skeletal ||
			owner.IsPlayerCharacter && (!_allowPcs || owner.IsGuest) || !owner.IsPlayerCharacter && !_allowNpcs ||
			!_allowAdmins && DirectPossessionSecurity.HasProtectedStaffAuthority(owner))
		{ error = "This corpse does not satisfy the spell's configured eligibility."; return false; }
		error = Gameworld.SpellOwnedCorpseAnimations!.AdmissionError(item);
		if (error is not null) return false;
		if (_preparedSeconds is not null) return true;
		try
		{
			var seconds = LifetimeExpression!.Evaluate(caster, native.CastingTrait, TraitBonusContext.SpellDuration);
			if (!double.IsFinite(seconds) || seconds <= 0 || seconds > (DateTime.MaxValue - RuntimeClock.UtcNow).TotalSeconds || TimeSpan.FromSeconds(seconds) <= TimeSpan.Zero)
				error = "Corpse-animation lifetime must be finite, positive and representable as an absolute UTC deadline.";
			else _preparedSeconds = seconds;
		}
		catch (Exception ex) { error = "Corpse-animation lifetime could not be prepared: " + ex.Message; }
		return error is null;
	}

	public bool TryPrepareApplication(ICharacter caster, IPerceivable target, OpposedOutcomeDegree outcome,
		SpellPower power, TimeSpan resolvedDuration, out IMagicSpellEffectApplication? application, out string? error)
	{
		application = null;
		if (!ValidateInvocation(caster, target, out error)) return false;
		application = new PreparedAnimation(this, caster, target, outcome, power, Guid.NewGuid());
		return true;
	}

	private sealed record PreparedAnimation(AnimateCorpseSpellEffect Effect, ICharacter Caster, IPerceivable Target,
		OpposedOutcomeDegree Outcome, SpellPower Power, Guid Id) : IMagicSpellEffectApplication
	{
		public IMagicSpellEffect Create(IMagicSpellEffectParent parent)
		{
			if (!Effect.DurableLifecycle) return Effect.GetOrApplyEffect(Caster, Target, Outcome, Power, parent, [])!;
			if (!Effect.ValidateInvocation(Caster, Target, out var error)) throw new InvalidOperationException(error);
			var item = (IGameItem)Target; var corpse = item.GetItemType<ICorpse>(); var originalCell = item.Location.Id; var layer = item.RoomLayer;
			var native = (MagicSpell)Effect.Spell; var now = RuntimeClock.UtcNow;
			var ais = Effect._aiIds.Select(x => Effect.Gameworld.AIs.Get(x)!).ToArray();
			var origin = new SpellLifecycleOrigin(Id, native.Id, native.InvocationGrade!.Value, CharacterInstanceIdentityComparer.IdentityId(Caster),
				Effect.LifecycleFamily, SpellLifecycleMode.TemporaryCleanup, now, now.AddSeconds(Effect._preparedSeconds!.Value), Effect.SavePresentation(native.InvocationOriginId));
			var animated = Effect.Gameworld.SpellOwnedCorpseAnimations!.Create(item, Caster, ais, origin);
			animated.AddEffect(new CorpseAnimationDispelProxyEffect(animated, item.Id));
			var result = new SpellAnimatedCorpseEffect(item, parent, origin.CreatorId, Caster.InstanceId, item.Id,
				CharacterInstanceIdentityComparer.IdentityId(corpse.OriginalCharacter), corpse.OriginalBody.Id, animated.InstanceId,
				originalCell, (int)layer, native.Id, ais.Select(x => x.Id), CharacterInstancePersistencePolicy.DespawnOnReboot,
				Effect._roomEcho, Effect._collapseEcho, Effect._restoreEcho);
			result.BindOwnedLifecycle(Id);
			return result;
		}
	}

	private string SavePresentation(Guid? invocation) => new XElement("Presentation", new XAttribute("invocation", invocation?.ToString() ?? ""),
		new XElement("Target", _targetEcho), new XElement("Collapse", _collapseEcho), new XElement("Restore", _restoreEcho)).ToString(SaveOptions.DisableFormatting);

	private bool BuildingCommandLifecycle(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "lifecycle":
				var mode = command.PopSpeech();
				if (!mode.EqualTo("legacy") && !mode.EqualTo("durable")) { actor.OutputHandler.Send("Specify legacy or durable."); return false; }
				DurableLifecycle = mode.EqualTo("durable"); _invalidLifecycle = null; break;
			case "family":
				if (string.IsNullOrWhiteSpace(command.SafeRemainingArgument) || command.SafeRemainingArgument.Length > 128)
				{ actor.OutputHandler.Send("Specify a family of at most 128 characters."); return false; }
				LifecycleFamily = command.SafeRemainingArgument; break;
			case "lifetime":
				var expression = new TraitExpression(command.SafeRemainingArgument, Gameworld);
				if (expression.HasErrors() || expression.NonTraitParameters.Contains("outcome", StringComparer.OrdinalIgnoreCase))
				{ actor.OutputHandler.Send("Specify a valid real-seconds lifetime determinable before the casting check."); return false; }
				LifetimeExpression = expression; _lifetimeFormula = command.SafeRemainingArgument; _preparedSeconds = null; break;
		}
		Spell.Changed = true; actor.OutputHandler.Send("Corpse-animation lifecycle configuration updated."); return true;
	}
}
