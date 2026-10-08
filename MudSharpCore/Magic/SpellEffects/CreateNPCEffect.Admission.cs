#nullable enable
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Magic.Casting;
using MudSharp.Magic.Lifecycle;
using MudSharp.NPC.Templates;

namespace MudSharp.Magic.SpellEffects;

public sealed partial class CreateNPCEffect
{
	private readonly Dictionary<IPerceivable, NpcSelection> _preparedSelections = new(ReferenceEqualityComparer.Instance);

	public IMagicSpellEffectPreparedSelectionToken? CapturePreparedSelection(ICharacter caster, IPerceivable recipient)
	{
		if (LifecycleMode is null && _loadError is null) return null;
		if (_preparedSelections.TryGetValue(recipient, out var existing))
		{
			if (!TryConfirmPreparedSelection(caster, recipient, out var error)) throw new InvalidOperationException(error);
			return existing;
		}
		if (!ValidateBeforePayment(caster, recipient, out var knownSeconds, out var diagnostic)) throw new InvalidOperationException(diagnostic);
		var template = (SimpleNPCTemplate)NPCTemplate;
		var checks = new List<Func<bool>>();
		void Value<T>(Func<T> read) { var before = read(); checks.Add(() => EqualityComparer<T>.Default.Equals(before, read())); }
		void Reference<T>(Func<T> read) where T : class? { var before = read(); checks.Add(() => ReferenceEquals(before, read())); }
		void Room(IRoom room)
		{
			Reference(() => room.Gameworld);
			Reference(() => room.RouteDefinition);
			if (room.RouteDefinition is { } route)
			{
				Value(() => route.LengthMetres);
				Value(() => route.DefaultPositionMetres);
				Value(() => route.TopologyVersion);
			}
		}
		Reference(() => caster.Gameworld); Reference(() => caster.Body); Value(() => caster.InstanceId);
		Reference(() => caster.Location); Value(() => caster.RoomLayer); Value(() => caster.RoutePositionMetres);
		Reference(() => caster.Movement);
		Reference(() => Gameworld.SpellOwnedNpcs); Reference(() => Gameworld.NpcTemplates.Get(_npcPrototypeId));
		Reference(() => template.Gameworld); Value(() => template.Id); Value(() => template.Status); Value(() => template.RevisionNumber);
		Value(() => template.NativeCreationDefinition);
		Value(() => SaveToXml().ToString(SaveOptions.DisableFormatting));
		Room(caster.Location);
		if (recipient is IRoom targetRoom && !ReferenceEquals(targetRoom, caster.Location)) Room(targetRoom);
		// The admitted template is Simple: these ownership guards inspect native fields and
		// collections only, including referenced role/Combo merit changes, with no prog or RNG.
		checks.Add(() => NativeNpcCreationEligibility.TemplateError(template, Gameworld) is null);
		// Recheck the captured scalar after later confirmations without evaluating the formula again.
		if (knownSeconds is { } seconds) checks.Add(() => ValidLifetime(seconds, out _));
		var token = new NpcSelection(caster, recipient, template, () => checks.All(check => check()));
		_preparedSelections.Add(recipient, token);
		return token;
	}

	public bool TryReusePreparedSelection(IMagicSpellEffectPreparedSelectionToken selection,
		ICharacter caster, IPerceivable recipient, out string? error)
	{
		if (selection is not NpcSelection token || !ReferenceEquals(token.Caster, caster) ||
			!ReferenceEquals(token.Recipient, recipient) || !ReferenceEquals(token.Template, NPCTemplate) || !token.IsCurrent)
		{
			error = "The prepared NPC template, caster or spawn frame changed.";
			return false;
		}
		if (!ValidateBeforePayment(caster, recipient, out error)) return false;
		_preparedSelections[recipient] = token;
		return true;
	}

	public bool TryConfirmPreparedSelection(ICharacter caster, IPerceivable recipient, out string? error)
	{
		if (!_preparedSelections.TryGetValue(recipient, out var token) || !ReferenceEquals(token.Caster, caster) || !token.IsCurrent)
		{
			error = "The prepared NPC template, caster or spawn frame changed.";
			return false;
		}
		return ValidateBeforePayment(caster, recipient, out error);
	}

	private bool ValidateBeforePayment(ICharacter caster, IPerceivable recipient, out string? error)
		=> ValidateBeforePayment(caster, recipient, out _, out error);

	private bool ValidateBeforePayment(ICharacter caster, IPerceivable recipient, out double? knownSeconds, out string? error)
	{
		knownSeconds = null;
		error = DefinitionError;
		if (error is not null) return false;
		if (Spell is not MagicSpell { InvocationGrade: not null })
		{
			error = "Lifecycle NPC creation requires a configured selected-grade native casting invocation.";
			return false;
		}
		try
		{
			var effective = RouteSpatialService.Instance.GetEffectiveLocation(caster);
			var room = recipient as IRoom ?? effective.Room;
			var location = ReferenceEquals(room, effective.Room) ? effective :
				CharacterInstanceService.CreateDefaultSpawnLocation(room, RoomLayer.GroundLevel);
			if (!ReferenceEquals(caster.Gameworld, Gameworld) || room is null || !ReferenceEquals(room.Gameworld, Gameworld))
			{
				error = "The NPC caster and spawn room must belong to the casting gameworld.";
				return false;
			}
			if (!RouteSpatialService.Instance.TryValidateLocation(location, out error)) return false;
			if (LifecycleMode != SpellLifecycleMode.Permanent && LifetimeExpression is CastingExpression expression)
			{
				if (!expression.TryEvaluateFixedInputs(out knownSeconds, out error)) return false;
				if (knownSeconds is { } seconds && !ValidLifetime(seconds, out error)) return false;
			}
			return true;
		}
		catch (Exception ex)
		{
			error = "NPC admission could not be prepared: " + ex.Message;
			return false;
		}
	}

	private static bool ValidLifetime(double seconds, out string? error)
	{
		error = "The NPC lifetime must be finite, positive and representable as an absolute UTC deadline.";
		var now = RuntimeClock.UtcNow;
		if (!double.IsFinite(seconds) || seconds <= 0 || seconds > (DateTime.MaxValue - now).TotalSeconds)
			return false;
		try
		{
			if (TimeSpan.FromSeconds(seconds) <= TimeSpan.Zero) return false;
			now.AddSeconds(seconds);
		}
		catch (ArgumentException) { return false; }
		catch (OverflowException) { return false; }
		error = null;
		return true;
	}

	private sealed record NpcSelection(ICharacter Caster, IPerceivable Recipient, SimpleNPCTemplate Template,
		Func<bool> Check) : IMagicSpellEffectPreparedSelectionRawToken
	{
		public bool IsCurrent => Check();
	}
}
