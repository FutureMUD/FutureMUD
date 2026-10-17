#nullable enable

using System.Globalization;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Autobuilder;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.GameItems;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.SpellEffects;

/// <summary>A paid room invocation with exactly one frozen, directly carried Arcane Mark.</summary>
public sealed class CreateGateEffect : IMagicSpellEffectTemplate, IMagicSpellEffectAdmission,
	IMagicSpellEffectPreparedSelection
{
	private readonly Dictionary<IPerceivable, Selection> _selections = new(ReferenceEqualityComparer.Instance);
	private readonly XElement _definition;
	public const string HelpText = "Use autobuilder <template>, lifetime <seconds per grade>, pulse <seconds>, or undeadmerit <merit>.";

	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("creategate", (xml, spell) => new CreateGateEffect(xml, spell));
		SpellEffectFactory.RegisterBuilderFactory("creategate", (_, spell) =>
			(new CreateGateEffect(new SpellGateConfiguration(0, 120, 0).SaveEffect(), spell), string.Empty),
			"Creates one marked Nilaz passage with occupied closure", HelpText, true, true,
			StandaloneSpellEffectTemplateHelper.RoomTriggerTypes);
	}

	public CreateGateEffect(XElement definition, IMagicSpell spell) { Spell = spell; _definition = new(definition); }
	public IMagicSpell Spell { get; }
	public IFuturemud Gameworld => Spell.Gameworld;
	public bool IsInstantaneous => true;
	public bool RequiresTarget => true;
	public XElement SaveToXml() => new(_definition);
	public IMagicSpellEffectTemplate Clone() => new CreateGateEffect(SaveToXml(), Spell);
	public bool IsCompatibleWithTrigger(IMagicTrigger trigger) =>
		StandaloneSpellEffectTemplateHelper.IsRoomTarget(trigger.TargetTypes);

	private SpellGateConfiguration Configuration()
	{
		var names = new[] { "AutobuilderTemplate", "SecondsPerGrade", "UndeadMerit", "SuffocationPulseSeconds" };
		if (_definition.Name != "Effect" || (string?)_definition.Attribute("type") != "creategate" ||
			(string?)_definition.Attribute("version") != "1" || _definition.Attributes().Count() != 2 ||
			_definition.Elements().Count() != names.Length ||
			!_definition.Elements().Select(x => x.Name.LocalName).Order().SequenceEqual(names.Order()) ||
			_definition.Elements().Any(x => x.Name.Namespace != XNamespace.None || x.HasAttributes || x.HasElements))
			throw new FormatException("Unsupported Nilaz gate configuration.");
		return new((long)_definition.Element("AutobuilderTemplate")!,
			(double)_definition.Element("SecondsPerGrade")!, (long)_definition.Element("UndeadMerit")!,
			(int)_definition.Element("SuffocationPulseSeconds")!);
	}

	public string? DefinitionError
	{
		get { try { Configuration(); return null; } catch (Exception ex) { return "Invalid Nilaz gate configuration: " + ex.Message; } }
	}

	private bool TryMark(ICharacter caster, out IGameItem? item, out SpellMagicTagEffect? mark,
		out MagicSpellParent? parent, out string? error)
	{
		item = null; mark = null; parent = null;
		var candidates = caster.Body.DirectItems.Select(x => (Item: x, Valid: SpellTravelAnchorResolver.TryResolve(
			caster, x, SpellTravelAnchorMode.ArcaneMarkedRoom, out var _, out var _)))
			.Where(x => x.Valid).Select(x => x.Item).Distinct<IGameItem>(ReferenceEqualityComparer.Instance).ToArray();
		if (candidates.Length != 1)
		{
			error = "Carry exactly one active Arcane Mark item to open a Nilaz passage."; return false;
		}
		item = candidates[0];
		if (!SpellTravelAnchorResolver.TryResolve(caster, item, SpellTravelAnchorMode.ArcaneMarkedRoom,
			out var anchor, out error) || anchor!.Effect is not SpellMagicTagEffect tag ||
			tag.ParentEffect is not MagicSpellParent head || head.Identity == Guid.Empty || tag.RemainingUses <= 0)
		{
			error = "The carried Arcane Mark no longer has exact saved-room authority."; return false;
		}
		mark = tag; parent = head; error = null; return true;
	}

	private bool Validate(ICharacter caster, IPerceivable recipient, out SpellGateConfiguration? config,
		out int grade, out IGameItem? item, out SpellMagicTagEffect? mark, out MagicSpellParent? parent,
		out string? error)
	{
		config = null; item = null; mark = null; parent = null; grade = 0; error = DefinitionError;
		if (error is not null) return false;
		if (recipient is not IRoom room || Spell is not MagicSpell { InvocationGrade: >= 1 and <= 7 } native ||
			Gameworld.SpellOwnedGates is null)
		{
			error = "Nilaz creation requires a selected-grade room invocation."; return false;
		}
		grade = native.InvocationGrade!.Value; config = Configuration();
		if (!TryMark(caster, out item, out mark, out parent, out error)) return false;
		error = Gameworld.SpellOwnedGates.AdmissionError(caster, room, item!, Spell, config, grade);
		return error is null;
	}

	public IMagicSpellEffectPreparedSelectionToken CapturePreparedSelection(ICharacter caster, IPerceivable recipient)
	{
		if (_selections.TryGetValue(recipient, out var existing))
		{
			if (!TryConfirmPreparedSelection(caster, recipient, out var stale)) throw new InvalidOperationException(stale);
			return existing;
		}
		if (!Validate(caster, recipient, out var config, out var grade, out var item, out var mark,
			out var parent, out var error)) throw new InvalidOperationException(error);
		var source = (IRoom)recipient;
		var template = Gameworld.AutobuilderRooms.Get(config!.AutobuilderTemplateId) as IAutobuilderRoomPresentation ??
			throw new InvalidOperationException("The owned Nilaz autobuilder is unavailable.");
		var destination = Gameworld.Rooms.Get(mark!.SavedRoomId) ??
			throw new InvalidOperationException("The marked room is unavailable.");
		var sourceOverlay = source.CurrentOverlay; var destinationOverlay = destination.CurrentOverlay;
		var body = caster.Body; var uses = mark.RemainingUses; var identity = parent!.Identity;
		var templateDefinition = template.PresentationDefinition;
		var definition = SaveToXml().ToString(SaveOptions.DisableFormatting);
		var selected = new Selection(caster, source, item!, mark, parent, config, grade, definition, () =>
			ReferenceEquals(caster.Location, source) && ReferenceEquals(caster.Body, body) &&
			ReferenceEquals(source.CurrentOverlay, sourceOverlay) &&
			ReferenceEquals(Gameworld.Rooms.Get(destination.Id), destination) &&
			ReferenceEquals(destination.CurrentOverlay, destinationOverlay) &&
			ReferenceEquals(Gameworld.AutobuilderRooms.Get(config.AutobuilderTemplateId), template) &&
			template.PresentationDefinition == templateDefinition &&
			body.DirectItems.Any(x => ReferenceEquals(x, item)) &&
			SpellTravelAnchorResolver.TryResolve(caster, item!, SpellTravelAnchorMode.ArcaneMarkedRoom,
				out var fresh, out _) && ReferenceEquals(fresh!.Effect, mark) &&
				fresh.Destination?.Id == destination.Id && mark.RemainingUses == uses &&
				ReferenceEquals(mark.ParentEffect, parent) && parent.Identity == identity &&
				SaveToXml().ToString(SaveOptions.DisableFormatting) == definition);
		_selections.Add(recipient, selected);
		return selected;
	}

	public bool TryReusePreparedSelection(IMagicSpellEffectPreparedSelectionToken selection, ICharacter caster,
		IPerceivable recipient, out string? error)
	{
		error = "The prepared Nilaz mark, endpoint or presentation changed.";
		if (selection is not Selection token || !ReferenceEquals(token.Caster, caster) ||
			!ReferenceEquals(token.Source, recipient) || !token.IsCurrent ||
			SaveToXml().ToString(SaveOptions.DisableFormatting) != token.Definition) return false;
		if (!Validate(caster, recipient, out _, out var grade, out var item, out var mark, out var parent, out error) ||
			grade != token.Grade || !ReferenceEquals(item, token.Item) || !ReferenceEquals(mark, token.Mark) ||
			parent?.Identity != token.Parent.Identity || !token.IsCurrent) return false;
		_selections[recipient] = token; return true;
	}

	public bool TryConfirmPreparedSelection(ICharacter caster, IPerceivable recipient, out string? error)
	{
		error = "The prepared Nilaz mark, endpoint or presentation changed.";
		return _selections.TryGetValue(recipient, out var token) && ReferenceEquals(token.Caster, caster) &&
			token.IsCurrent && Validate(caster, recipient, out _, out var grade, out var item,
				out var mark, out var parent, out error) && grade == token.Grade &&
			ReferenceEquals(item, token.Item) && ReferenceEquals(mark, token.Mark) &&
			parent?.Identity == token.Parent.Identity && token.IsCurrent;
	}

	public bool TryPrepareApplication(ICharacter caster, IPerceivable target, OpposedOutcomeDegree outcome,
		SpellPower power, TimeSpan resolvedDuration, out IMagicSpellEffectApplication? application, out string? error)
	{
		application = null;
		if (!TryConfirmPreparedSelection(caster, target, out error)) return false;
		var selected = _selections[target];
		application = new Creation(Guid.NewGuid(), this, selected); return true;
	}

	public IMagicSpellEffect? GetOrApplyEffect(ICharacter caster, IPerceivable? target,
		OpposedOutcomeDegree outcome, SpellPower power, IMagicSpellEffectParent parent,
		SpellAdditionalParameter[] additionalParameters)
	{
		if (target is null) throw new InvalidOperationException("Nilaz creation requires a room target.");
		if (!TryPrepareApplication(caster, target, outcome, power, TimeSpan.Zero,
			out var application, out var error)) throw new InvalidOperationException(error ?? "Nilaz admission failed.");
		return application!.Create(parent);
	}

	private sealed record Selection(ICharacter Caster, IRoom Source, IGameItem Item, SpellMagicTagEffect Mark,
		MagicSpellParent Parent, SpellGateConfiguration Configuration, int Grade, string Definition,
		Func<bool> Check) : IMagicSpellEffectPreparedSelectionRawToken
	{
		public bool IsCurrent => Check();
	}

	private sealed record Creation(Guid Id, CreateGateEffect Effect, Selection Selection) : IMagicSpellEffectApplicationOperation
	{
		public IMagicSpellEffect Create(IMagicSpellEffectParent parent) => Apply(parent).Effect!;
		public MagicEffectOperation Apply(IMagicSpellEffectParent parent)
		{
			if (!Effect.TryConfirmPreparedSelection(Selection.Caster, Selection.Source, out var error))
				throw new InvalidOperationException(error);
			var caster = Selection.Caster; var source = Selection.Source;
			if (!SpellTravelAnchorResolver.TryResolve(caster, Selection.Item, SpellTravelAnchorMode.ArcaneMarkedRoom,
				out var mark, out _)) throw new InvalidOperationException("The selected mark no longer resolves.");
			var destination = mark!.Destination;
			var c = Selection.Configuration; var now = RuntimeClock.UtcNow;
			var anchor = new SpellGateAnchor(source.Id, source.CurrentOverlay.Id, destination.Id,
				destination.CurrentOverlay.Id, Selection.Item.Id, Selection.Parent.Identity,
				c.AutobuilderTemplateId, c.UndeadMeritId, c.SuffocationPulseSeconds);
			var origin = new SpellLifecycleOrigin(Id, Effect.Spell.Id, Selection.Grade,
				CharacterInstanceIdentityComparer.IdentityId(caster), SpellGateAnchor.Family,
				SpellLifecycleMode.TemporaryCleanup, now, now.AddSeconds(c.SecondsPerGrade * Selection.Grade),
				anchor.Save());
			Effect.Gameworld.SpellOwnedGates!.CreatePrepared(caster, source, Selection.Item, Effect.Spell,
				c, origin, () => Selection.IsCurrent);
			return new(MagicEffectOperationStatus.Applied, null);
		}
	}

	public string Show(ICharacter actor)
	{
		if (DefinitionError is { } error) return error.ColourError();
		var c = Configuration();
		return $"Nilaz passage from autobuilder {c.AutobuilderTemplateId.ToString("N0", actor).ColourValue()}, " +
			$"{TimeSpan.FromSeconds(c.SecondsPerGrade).Describe(actor).ColourValue()} per grade, " +
			$"closed-room pulse {TimeSpan.FromSeconds(c.SuffocationPulseSeconds).Describe(actor).ColourValue()}, " +
			$"undead merit {c.UndeadMeritId.ToString("N0", actor).ColourValue()}.";
	}

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		var field = command.PopSpeech().ToLowerInvariant(); var value = command.SafeRemainingArgument;
		string? name = null; object? parsed = null;
		switch (field)
		{
			case "autobuilder":
				if (Gameworld.AutobuilderRooms.GetByIdOrName(value) is not IAutobuilderRoomPresentation template) break;
				name = "AutobuilderTemplate"; parsed = template.Id; break;
			case "undeadmerit":
				if (Gameworld.Merits.GetByIdOrName(value) is not { } merit) break;
				name = "UndeadMerit"; parsed = merit.Id; break;
			case "lifetime":
				if (!double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var lifetime) ||
					!double.IsFinite(lifetime) || lifetime <= 0) break;
				name = "SecondsPerGrade"; parsed = lifetime; break;
			case "pulse":
				if (!int.TryParse(value, out var seconds) || seconds is < 10 or > 3600) break;
				name = "SuffocationPulseSeconds"; parsed = seconds; break;
		}
		if (name is null) { actor.OutputHandler.Send(HelpText); return false; }
		_definition.SetElementValue(name, parsed); Spell.Changed = true;
		actor.OutputHandler.Send(Show(actor)); return true;
	}
}
