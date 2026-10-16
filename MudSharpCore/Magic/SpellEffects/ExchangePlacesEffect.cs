#nullable enable

using MudSharp.Body;
using MudSharp.Body.Position;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Magic.SpellTriggers;
using MudSharp.NPC.AI;
using MudSharp.Planes;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.SpellEffects;

/// <summary>A single paid exchange of two frozen physical travellers linked by an active mind contact.</summary>
public sealed partial class ExchangePlacesEffect : IMagicSpellEffectTemplate, IMagicSpellEffectPreparedSelection,
	IMagicSpellEffectOperation
{
	private readonly XElement _definition;
	private readonly Dictionary<IPerceivable, Selection> _selections = new(ReferenceEqualityComparer.Instance);
	public ExchangePlacesEffect(XElement definition, IMagicSpell spell) { _definition = new(definition); Spell = spell; }
	public IMagicSpell Spell { get; }
	public IFuturemud Gameworld => Spell.Gameworld;
	public bool IsInstantaneous => true;
	public bool RequiresTarget => true;
	public XElement SaveToXml() => new(_definition);
	public IMagicSpellEffectTemplate Clone() => new ExchangePlacesEffect(SaveToXml(), Spell);
	public string? DefinitionError { get { try { Configuration(); return null; } catch (Exception ex) { return ex.Message; } } }
	public bool IsCompatibleWithTrigger(IMagicTrigger trigger) => trigger is CastingTriggerMindContactCharacter;
	public const string HelpText = "Use restricted <tag>, populated <tag>, consumedroot <tag>, or retainedrank <0-5> <tag>.";

	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("exchangeplaces", (xml, spell) => new ExchangePlacesEffect(xml, spell));
		SpellEffectFactory.RegisterBuilderFactory("exchangeplaces", (_, spell) =>
			(new ExchangePlacesEffect(new XElement("Effect", new XAttribute("type", "exchangeplaces"),
				new XAttribute("version", 1), new XElement("RestrictedRoomTag", 0), new XElement("PopulatedRoomTag", 0),
				new XElement("ConsumedRootTag", 0), new XElement("RetainedRanks", Enumerable.Range(0, 6)
					.Select(rank => new XElement("Rank", new XAttribute("number", rank), new XAttribute("tag", 0))))), spell), string.Empty),
			"Exchanges two exact mind-linked travellers", HelpText, true, true, ["mindcontactcharacter"]);
	}

	private (ITag Restricted, ITag Populated, ITag ConsumedRoot, ITag[] Retained) Configuration()
	{
		if ((string?)_definition.Attribute("version") != "1") throw new FormatException("Unsupported exchange configuration.");
		ITag Tag(long id) => id > 0 && Gameworld.Tags.Get(id) is { } tag ? tag :
			throw new FormatException("Exchange Places requires all of its explicit native tag bindings.");
		var ranks = _definition.Element("RetainedRanks")?.Elements("Rank").ToArray() ?? [];
		if (ranks.Length != 6 || !ranks.Select(x => (int?)x.Attribute("number")).SequenceEqual(Enumerable.Range(0, 6).Select(x => (int?)x)))
			throw new FormatException("Exchange Places requires consecutive retained ranks zero through five.");
		var tags = ranks.Select(x => Tag((long?)x.Attribute("tag") ?? 0)).ToArray();
		if (tags.Distinct().Count() != 6 || tags.Skip(1).Where((tag, index) => !tag.IsA(tags[index])).Any())
			throw new FormatException("Retained component rank tags must preserve their ascending ancestry.");
		return (Tag((long?)_definition.Element("RestrictedRoomTag") ?? 0),
			Tag((long?)_definition.Element("PopulatedRoomTag") ?? 0),
			Tag((long?)_definition.Element("ConsumedRootTag") ?? 0), tags);
	}

	private string? AdmissionError(ICharacter caster, ICharacter target, int grade, out IGameItem? retained)
	{
		retained = null;
		var config = Configuration();
		if (caster.Combat is not null || caster.PositionState?.Id is not (1 or 9 or 10 or 11 or 13 or 14 or 15 or 16 or 17 or 18 or 19 or 20))
			return "The caster must use the native minimum-standing posture mapping and be out of combat.";
		if (!CastingTriggerMindContactCharacter.IsCurrentContact(caster, target)) return "The exact active mind contact is no longer present.";
		if (caster.Location is not { } source || target.Location is not { } destination ||
			ReferenceEquals(source, destination) && caster.RoomLayer == target.RoomLayer)
			return "Exchange Places requires two different spatial positions.";
		if (target.IsAdministrator() || target.State.HasFlag(CharacterState.Dead) ||
			caster.RidingMount is not null || target.RidingMount is not null || caster.Riders.Any() || target.Riders.Any())
			return "Only the two living, unmounted physical travellers may exchange places.";
		if (!caster.SharesPlaneWith(target) || !source.SharesPlaneWith(destination)) return "The travellers must be on the same plane.";
		if (destination.OutdoorsType(target) != RoomOutdoorsType.Outdoors) return "The other traveller is beyond the open wind's grasp.";
		if (source.IsA(config.Restricted) || destination.IsA(config.Restricted) || destination.IsA(config.Populated))
			return "A configured room restriction prevents the exchange.";
		var error = SpellTravelEndpointPolicy.RelocationError(caster, caster, source, destination, Spell, caster.RoomLayer, target.RoomLayer) ??
			SpellTravelEndpointPolicy.RelocationError(caster, target, destination, source, Spell, target.RoomLayer, caster.RoomLayer);
		if (error is not null) return error;
		var rank = config.Retained[Math.Max(grade - 2, 0)];
		retained = RetainedCandidates(caster).Where(item => CurrentComponent(caster, item) && item.IsA(rank) && !item.IsA(config.ConsumedRoot))
			.Where(item => item.ContainedIn?.IsA(config.ConsumedRoot) != true)
			.OrderBy(item => item.Id).FirstOrDefault();
		if (retained is null) return "Carry the required Teleportation component separately from the consumed Alteration component.";
		return retained.ContainedIn is { } parent && !caster.Body.CanGet(retained, parent, 1,
			ItemCanGetIgnore.IgnoreInventoryPlans | ItemCanGetIgnore.IgnoreFreeHands) ? "The retained Teleportation component is not accessible." : null;
	}

	private IEnumerable<IGameItem> RetainedCandidates(ICharacter caster) => caster.Body.DirectItems.Concat(caster.Body.DirectItems
		.Where(parent => parent.GetItemType<IOpenable>()?.IsOpen != false)
		.SelectMany(parent => parent.GetItemType<IContainer>()?.Contents ?? []));
	private bool LiveItem(IGameItem item) => item.Id > 0 && !item.Deleted && !item.Destroyed &&
		ReferenceEquals(item.Gameworld, Gameworld) && ReferenceEquals(Gameworld.Items.Get(item.Id), item);
	private bool CurrentComponent(ICharacter caster, IGameItem item)
	{
		if (!LiveItem(item)) return false;
		if (item.ContainedIn is not { } parent) return ReferenceEquals(item.InInventoryOf, caster.Body) &&
			caster.Body.DirectItems.Any(x => ReferenceEquals(x, item));
		return LiveItem(parent) && parent.ContainedIn is null && ReferenceEquals(parent.InInventoryOf, caster.Body) &&
			caster.Body.DirectItems.Any(x => ReferenceEquals(x, parent)) && parent.GetItemType<IOpenable>()?.IsOpen != false &&
			parent.GetItemType<IContainer>()?.Contents.Any(x => ReferenceEquals(x, item)) == true;
	}

	private sealed record Frame(ICharacter Actor, IBody Body, long Id, long Instance, ICharacterIdentity? Identity, CharacterState State, IPositionState? Position,
		IRoom Room, RoomLayer Layer, IRoomOverlay Overlay, ITerrain Terrain, IZone Zone, IShard Shard)
	{
		public static Frame Capture(ICharacter actor) => new(actor, actor.Body, actor.Id, actor.InstanceId, actor.Identity, actor.State, actor.PositionState,
			actor.Location, actor.RoomLayer, actor.Location.CurrentOverlay, actor.Location.CurrentOverlay.Terrain,
			actor.Location.OwningZone, actor.Location.OwningZone.Shard);
		public bool PhysicalCurrent => Actor.Id == Id && Actor.InstanceId == Instance && ReferenceEquals(Actor.Identity, Identity) &&
			Actor.State == State && ReferenceEquals(Actor.PositionState, Position) &&
			ReferenceEquals(Actor.Body, Body) && ReferenceEquals(Body.Actor, Actor) &&
			Body.Id > 0 && Actor.Identity?.Instances.Any(x => ReferenceEquals(x, Actor)) == true &&
			Actor.Gameworld.Actors.Any(x => ReferenceEquals(x, Actor)) &&
			Actor.RidingMount is null && !Actor.Riders.Any();
		public bool EndpointCurrent => ReferenceEquals(Actor.Gameworld.Rooms.Get(Room.Id), Room) &&
			ReferenceEquals(Room.CurrentOverlay, Overlay) && ReferenceEquals(Overlay.Terrain, Terrain) &&
			ReferenceEquals(Room.OwningZone, Zone) && ReferenceEquals(Zone.Shard, Shard);
		public bool At(IRoom room, RoomLayer layer) => PhysicalCurrent && ReferenceEquals(Actor.Location, room) && Actor.RoomLayer == layer &&
			room.Characters.Count(x => ReferenceEquals(x, Actor)) == 1 &&
			room.OwningZone.Characters.Count(x => ReferenceEquals(x, Actor)) == 1 &&
			room.OwningZone.Shard.Characters.Count(x => ReferenceEquals(x, Actor)) == 1 &&
			(ReferenceEquals(room, Room) || !Room.Characters.Any(x => ReferenceEquals(x, Actor)));
		public bool AtOrigin => EndpointCurrent && At(Room, Layer);
	}

	private sealed record Selection(ICharacter Caster, ICharacter Target, Frame Source, Frame Destination,
		IGameItem Retained, int Grade, string Definition, Func<bool> PolicyCurrent, Func<bool> Check) : IMagicSpellEffectPreparedSelectionRawToken
	{
		public bool IsCurrent => !Attempted && Check();
		public bool Attempted { get; set; }
	}

	public IMagicSpellEffectPreparedSelectionToken CapturePreparedSelection(ICharacter caster, IPerceivable recipient)
	{
		if (_selections.TryGetValue(recipient, out var existing))
		{
			if (!TryConfirmPreparedSelection(caster, recipient, out var error)) throw new InvalidOperationException(error);
			return existing;
		}
		if (recipient is not ICharacter target || Spell is not MagicSpell { InvocationGrade: >= 1 and <= 7 } native)
			throw new InvalidOperationException("Exchange Places requires a selected-grade character invocation.");
		var grade = native.InvocationGrade.Value;
		var config = Configuration();
		if (caster.Body is null || target.Body is null || caster.Location is null || target.Location is null)
			throw new InvalidOperationException("The exchange needs two exact physical bodies and endpoints.");
		var retained = RetainedCandidates(caster).Where(item => CurrentComponent(caster, item) &&
			item.IsA(config.Retained[Math.Max(grade - 2, 0)]) && !item.IsA(config.ConsumedRoot) &&
			item.ContainedIn?.IsA(config.ConsumedRoot) != true).OrderBy(item => item.Id).FirstOrDefault();
		if (retained is null) throw new InvalidOperationException("Carry the required separate Teleportation component.");
		var source = Frame.Capture(caster); var destination = Frame.Capture(target);
		var policyCurrent = CapturePolicyInputs(caster, target, retained, config.Restricted, config.Populated, config.ConsumedRoot, config.Retained);
		var contacts = caster.EffectsOfType<ConnectMindEffect>().Where(x => ReferenceEquals(x.TargetCharacter, target)).ToArray();
		var contactPowers = contacts.Select(x => (Contact: x, Power: x.MindPower, Range: x.MindPower.PowerDistance)).ToArray();
		if (AdmissionError(caster, target, grade, out var admitted) is { } refusal) throw new InvalidOperationException(refusal);
		if (!ReferenceEquals(retained, admitted)) throw new InvalidOperationException("The retained component changed during admission.");
		var definition = SaveToXml().ToString(SaveOptions.DisableFormatting);
		var token = new Selection(caster, target, source, destination, retained!, grade, definition, policyCurrent, () =>
			source.AtOrigin && destination.AtOrigin && policyCurrent() && CurrentComponent(caster, retained!) &&
			contactPowers.All(x => ReferenceEquals(x.Contact.MindPower, x.Power) && x.Power.PowerDistance == x.Range) &&
			contacts.Any(x => caster.Effects.Any(y => ReferenceEquals(x, y)) && target.Effects.Any(y => ReferenceEquals(x.ChildEffect, y))) &&
			SaveToXml().ToString(SaveOptions.DisableFormatting) == definition);
		if (!token.IsCurrent) throw new InvalidOperationException("The exact travellers changed during exchange selection.");
		_selections.Add(recipient, token); return token;
	}

	public bool TryReusePreparedSelection(IMagicSpellEffectPreparedSelectionToken selection, ICharacter caster,
		IPerceivable recipient, out string? error)
	{
		error = "The prepared exchange travellers, contact, component or endpoints changed.";
		if (selection is not Selection token || !ReferenceEquals(token.Caster, caster) || !ReferenceEquals(token.Target, recipient) ||
			Spell is not MagicSpell native || native.InvocationGrade != token.Grade ||
			SaveToXml().ToString(SaveOptions.DisableFormatting) != token.Definition || !token.IsCurrent) return false;
		if (AdmissionError(caster, token.Target, token.Grade, out var retained) is { } refusal) { error = refusal; return false; }
		if (!ReferenceEquals(retained, token.Retained) || !token.IsCurrent) return false;
		_selections[recipient] = token; error = null; return true;
	}
	public bool TryConfirmPreparedSelection(ICharacter caster, IPerceivable recipient, out string? error)
	{
		error = "The exchange was not prepared.";
		return _selections.TryGetValue(recipient, out var token) && TryReusePreparedSelection(token, caster, recipient, out error);
	}

	public MagicEffectOperation Apply(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
	{
		if (target is null) return new(MagicEffectOperationStatus.Rejected, null);
		if (!TryConfirmPreparedSelection(caster, target, out var refusal))
		{
			caster.OutputHandler.Send($"The exchange could not proceed: {refusal}".ColourError());
			return new(MagicEffectOperationStatus.Rejected, null);
		}
		var token = _selections[target]; token.Attempted = true;
		var first = token.Source; var second = token.Destination;
		try
		{
			Move(first, second.Room, second.Layer, true);
			// Moving may legitimately sever an out-of-range mind contact. Do not select another contact or traveller here.
			if (first.At(second.Room, second.Layer) && second.AtOrigin && first.EndpointCurrent && second.EndpointCurrent &&
				SpellTravelEndpointPolicy.RelocationError(caster, token.Target, second.Room, first.Room, Spell, second.Layer, first.Layer) is null &&
				first.At(second.Room, second.Layer) && second.AtOrigin && token.PolicyCurrent())
				Move(second, first.Room, first.Layer, true);
			if (first.At(second.Room, second.Layer) && second.At(first.Room, first.Layer) && first.EndpointCurrent && second.EndpointCurrent && token.PolicyCurrent())
				return new(MagicEffectOperationStatus.Applied, null);
		}
		catch
		{
			// The native move may have raised after publishing location. Inspect and restore only our exact moved pair.
		}
		Restore(second, first);
		Restore(first, second);
		var restored = first.AtOrigin && second.AtOrigin &&
			(ReferenceEquals(first.Room, second.Room) ||
			 !first.Room.Characters.Any(x => ReferenceEquals(x, second.Actor)) &&
			 !second.Room.Characters.Any(x => ReferenceEquals(x, first.Actor)));
		if (!restored) throw new InvalidOperationException("The exact exchange pair could not be restored; reconcile the paid operation before another cast.");
		return new(MagicEffectOperationStatus.NoChange, null);

		void Restore(Frame traveller, Frame other)
		{
			// Leave callbacks can fail before Location changes. Repair that membership gap too,
			// but never retrieve a replaced body or a traveller moved to an unrelated location.
			if (!traveller.PhysicalCurrent || !traveller.EndpointCurrent || !other.EndpointCurrent || traveller.AtOrigin ||
				!ReferenceEquals(traveller.Actor.Location, traveller.Room) && !ReferenceEquals(traveller.Actor.Location, other.Room)) return;
			// Compensation has only the captured original destination. A revoked order
			// cannot authorize fresh travel, but must not prevent this exact pair's recovery.
			try { using var repair = CommandExecutionScope.EnterIndependent(); Move(traveller, traveller.Room, traveller.Layer, false, true); }
			catch { /* Independently compensate the other traveller, then classify actual membership. */ }
		}

		void Move(Frame traveller, IRoom room, RoomLayer layer, bool echo, bool compensating = false)
		{
			using var movement = CommandExecutionScope.EnterSoloDisplacement(traveller.Actor,
				() => first.PhysicalCurrent && second.PhysicalCurrent && first.EndpointCurrent && second.EndpointCurrent &&
					(compensating || token.PolicyCurrent()));
			traveller.Actor.Teleport(room, layer, false, echo,
				"@ vanishes in a cloud of grey dust.", "@ appears in a cloud of grey dust.", "Grey dust clears to reveal new surroundings.");
		}
	}
	public IMagicSpellEffect? GetOrApplyEffect(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters) =>
		Apply(caster, target, outcome, power, parent, additionalParameters).Effect;

	public string Show(ICharacter actor) => "Exchange two exact mind-linked travellers, with outdoor and typed room restrictions; retain Teleportation rank max(grade-2,0).";
	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		var option = command.PopForSwitch();
		var element = option switch { "restricted" => "RestrictedRoomTag", "populated" => "PopulatedRoomTag", "consumedroot" => "ConsumedRootTag", _ => null };
		var rank = -1;
		if (option == "retainedrank" && (!int.TryParse(command.PopSpeech(), out rank) || rank is < 0 or > 5)) return false;
		var tag = Gameworld.Tags.GetByIdOrName(command.SafeRemainingArgument);
		if (tag is null || element is null && rank < 0) { actor.OutputHandler.Send(HelpText); return false; }
		if (element is not null) _definition.SetElementValue(element, tag.Id);
		else _definition.Element("RetainedRanks")!.Elements("Rank").Single(x => (int)x.Attribute("number")! == rank).SetAttributeValue("tag", tag.Id);
		Spell.Changed = true; actor.OutputHandler.Send("The exchange binding is now " + tag.Name.ColourValue() + "."); return true;
	}
}
