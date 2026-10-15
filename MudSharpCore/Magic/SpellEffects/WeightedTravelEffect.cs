#nullable enable

using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects;
using MudSharp.GameItems;
using MudSharp.Magic.SpellTriggers;
using MudSharp.NPC.AI;
using MudSharp.Planes;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.SpellEffects;

/// <summary>Separate source selectors share only the bounded, configured pool and solo displacement machinery.</summary>
public abstract class WeightedTravelEffect : IMagicSpellEffectTemplate, IMagicSpellEffectPreparedSelection,
	IMagicSpellEffectInvocationInputs, IMagicSpellEffectOperation
{
	private readonly XElement _definition;
	private readonly Dictionary<IPerceivable, SpellTravelDestinationRequest> _requests = new(ReferenceEqualityComparer.Instance);
	private readonly Dictionary<IPerceivable, Selection> _selections = new(ReferenceEqualityComparer.Instance);
	protected WeightedTravelEffect(XElement xml, IMagicSpell spell, bool banish, string type)
	{ _definition = new(xml); Spell = spell; Banishes = banish; Type = type; }
	protected bool Banishes { get; }
	protected string Type { get; }
	public IMagicSpell Spell { get; }
	public IFuturemud Gameworld => Spell.Gameworld;
	public bool IsInstantaneous => true;
	public bool RequiresTarget => true;
	public XElement SaveToXml() => new(_definition);
	public abstract IMagicSpellEffectTemplate Clone();
	protected static XElement BuilderDefinition(string type) => new("Effect", new XAttribute("type", type), new XAttribute("version", 1),
		new[] { "RestrictedRoomTag", "LethalRoomTag", "UrbanPoolTag", "WildPoolTag", "ElementalPoolTag" }.Select(x => new XElement(x, 0)),
		new XElement("UrbanWeight", 10), new XElement("WildWeight", 90), new XElement("ElementalPlanes"));
	public bool IsCompatibleWithTrigger(IMagicTrigger trigger) => Banishes ? trigger is CastingTriggerBanishTraveller : trigger is CastingTriggerWaystepDestination;
	private sealed record Configuration(ITag Restricted, ITag Lethal, ITag Urban, ITag Wild, ITag Elemental,
		int UrbanWeight, int WildWeight, long[] ElementalPlanes);
	private Configuration ReadConfiguration()
	{
		if ((string?)_definition.Attribute("version") != "1" || (string?)_definition.Attribute("type") != Type) throw new FormatException("Unsupported weighted travel configuration.");
		ITag Tag(string name) => Gameworld.Tags.Get((long?)_definition.Element(name) ?? 0) ?? throw new FormatException("Bind the exact " + name + ".");
		var urban = (int?)_definition.Element("UrbanWeight") ?? 0; var wild = (int?)_definition.Element("WildWeight") ?? 0;
		var planes = _definition.Element("ElementalPlanes")?.Elements("Plane").Select(x => (long)x).ToArray() ?? [];
		if (urban is < 1 or > 10000 || wild is < 1 or > 10000 || planes.Length is < 1 or > 64 ||
			planes.Distinct().Count() != planes.Length || planes.Any(x => x <= 0 || Gameworld.Planes.Get(x) is null))
			throw new FormatException("Weighted travel needs bounded positive group weights and current native elemental planes.");
		return new(Tag("RestrictedRoomTag"), Tag("LethalRoomTag"), Tag("UrbanPoolTag"), Tag("WildPoolTag"), Tag("ElementalPoolTag"), urban, wild, planes);
	}
	public string? DefinitionError { get { try { ReadConfiguration(); return null; } catch (FormatException ex) { return ex.Message; } } }
	public bool TryBindInvocationInputs(ICharacter caster, IPerceivable recipient, IReadOnlyList<SpellAdditionalParameter> parameters, out string? error)
	{
		error = "The exact weighted-pool or rune destination input is missing.";
		if (parameters.Count != 1 || parameters[0].ParameterName != SpellTravelDestinationRequest.ParameterName ||
			parameters[0].Item is not SpellTravelDestinationRequest request || Banishes && request.Rune is not null) return false;
		_requests[recipient] = request; error = null; return true;
	}
	private sealed record Candidate(IRoom Room, int Group);
	private sealed record Selection(ICharacter Caster, ICharacter Mover, Configuration Config, bool Elemental, Candidate[] Candidates,
		SpellTravelSnapshot Physical, ResolvedSpellTravelAnchor? Rune, RoomLayer Layer, string Definition, Func<bool> Raw,
		Func<bool> RuneRaw) : IMagicSpellEffectPreparedSelectionRawToken
	{
		public bool Attempted { get; set; }
		public bool IsCurrent => !Attempted && Physical.AtCapturedPositions && Raw() && RuneRaw();
	}
	private static bool Tagged(IRoom room, ITag tag) => room.Tags.Any(x => x.IsA(tag));
	private Candidate[] TaggedPool(ICharacter caster, Configuration config, bool elemental)
	{
		var rows = Gameworld.Rooms.Select(room => new { Room = room,
			Urban = !elemental && Tagged(room, config.Urban), Wild = !elemental && Tagged(room, config.Wild),
			Elemental = elemental && ReferenceEquals(room.OwningZone, caster.Location.OwningZone) && Tagged(room, config.Elemental) })
			.Where(x => x.Urban || x.Wild || x.Elemental).Take(4097).ToArray();
		if (rows.Length > 4096 || rows.Any(x => x.Urban && x.Wild)) throw new InvalidOperationException("The mapped travel pool is oversized or has ambiguous urban/wilderness membership.");
		return rows.Select(x => new Candidate(x.Room, x.Elemental ? 2 : x.Urban ? 0 : 1)).OrderBy(x => x.Room.Id).ToArray();
	}
	private string? EndpointError(ICharacter caster, ICharacter mover, IRoom destination, RoomLayer layer, Configuration config)
	{
		if (Banishes && !ReferenceEquals(caster, mover) && (mover.IsAdministrator() || !ReferenceEquals(mover.Location, caster.Location) || mover.RoomLayer != caster.RoomLayer || !caster.CanSee(mover)))
			return "Banishment requires one exact visible local non-staff physical traveller.";
		if (ReferenceEquals(mover.Location, destination) || Tagged(mover.Location, config.Restricted) || Tagged(destination, config.Restricted) ||
			Tagged(destination, config.Lethal) || destination.OutdoorsType(mover) != RoomOutdoorsType.Outdoors ||
			!mover.Location.SharesPlaneWith(destination)) return "The destination is outside this source route's permitted outdoor pool.";
		return SpellTravelEndpointPolicy.RelocationError(caster, mover, mover.Location, destination, Spell, mover.RoomLayer, layer);
	}
	private Func<bool> CaptureRune(ResolvedSpellTravelAnchor? anchor)
	{
		if (anchor is null) return () => true;
		var item = anchor.Item; var owner = item.OwnershipReference; var location = item.Location; var carrier = item.InInventoryOf;
		var container = item.ContainedIn; var id = item.Id; var effects = item.Effects.ToArray();
		var parent = ((IMagicSpellEffect)anchor.Effect).ParentEffect; var children = parent.SpellEffects.ToArray();
		var rooms = item.TrueLocations.Select(x => x.Id).Order().ToArray();
		return () => item.Id == id && !item.Deleted && !item.Destroyed && ReferenceEquals(Gameworld.Items.Get(id), item) &&
			item.OwnershipReference == owner && ReferenceEquals(item.Location, location) && ReferenceEquals(item.InInventoryOf, carrier) &&
			ReferenceEquals(item.ContainedIn, container) && effects.SequenceEqual(item.Effects, ReferenceEqualityComparer.Instance) &&
			ReferenceEquals(((IMagicSpellEffect)anchor.Effect).ParentEffect, parent) && children.SequenceEqual(parent.SpellEffects, ReferenceEqualityComparer.Instance) &&
			rooms.SequenceEqual(item.TrueLocations.Select(x => x.Id).Order());
	}
	public IMagicSpellEffectPreparedSelectionToken CapturePreparedSelection(ICharacter caster, IPerceivable recipient)
	{
		if (_selections.TryGetValue(recipient, out var existing))
		{ if (!TryConfirmPreparedSelection(caster, recipient, out var stale)) throw new InvalidOperationException(stale); return existing; }
		if (recipient is not ICharacter mover || !Banishes && !ReferenceEquals(caster, mover) || !_requests.TryGetValue(recipient, out var request) ||
			Spell is not MagicSpell { InvocationGrade: >= 1 and <= 7 }) throw new InvalidOperationException("Weighted travel requires its exact typed selector and a selected grade.");
		var config = ReadConfiguration(); var origin = mover.Location; var layer = mover.RoomLayer;
		var endpoints = new SpellTravelSnapshot(Gameworld, [caster, mover], [caster.Location, origin]);
		ResolvedSpellTravelAnchor? rune = null;
		if (request.Rune is { } item && !SpellTravelAnchorResolver.TryResolve(caster, item, SpellTravelAnchorMode.AnchorRuneLiveRoom, out rune, out var refusal))
			throw new InvalidOperationException(refusal);
		var plane = origin.CurrentPlane() ?? throw new InvalidOperationException("The origin has no current native plane.");
		var elemental = config.ElementalPlanes.Contains(plane.Id);
		var mapped = rune is null ? TaggedPool(caster, config, elemental) : [new Candidate(rune.Destination, 3)];
		var physical = new SpellTravelSnapshot(Gameworld, [caster, mover], mapped.Select(x => x.Room).Concat([caster.Location, origin]));
		var runeRaw = CaptureRune(rune); var definition = SaveToXml().ToString(SaveOptions.DisableFormatting);
		var policyTags = new[] { config.Restricted, config.Lethal, config.Urban, config.Wild, config.Elemental };
		var planeRefs = config.ElementalPlanes.Select(x => Gameworld.Planes.Get(x)).ToArray();
		bool Raw() => endpoints.AtCapturedPositions && definition == SaveToXml().ToString(SaveOptions.DisableFormatting) &&
			policyTags.All(tag => ReferenceEquals(Gameworld.Tags.Get(tag.Id), tag)) &&
			planeRefs.All(plane => ReferenceEquals(Gameworld.Planes.Get(plane.Id), plane)) &&
			(rune is not null || SamePool(mapped, TaggedPool(caster, config, elemental)));
		var candidates = mapped.Where(x => EndpointError(caster, mover, x.Room, layer, config) is null).ToArray();
		if (candidates.Length == 0 || !Raw() || !physical.AtCapturedPositions || !runeRaw())
			throw new InvalidOperationException("No exact permitted destination remains in the mapped pool or rune route.");
		var token = new Selection(caster, mover, config, elemental, candidates, physical, rune, layer, definition, Raw, runeRaw);
		_selections.Add(recipient, token); return token;
	}
	private static bool SamePool(Candidate[] first, Candidate[] second) => first.Length == second.Length &&
		first.Where((entry, index) => !ReferenceEquals(entry.Room, second[index].Room) || entry.Group != second[index].Group).Any() == false;
	public bool TryReusePreparedSelection(IMagicSpellEffectPreparedSelectionToken selection, ICharacter caster, IPerceivable recipient, out string? error)
	{
		error = "The prepared weighted destination, exact rune or traveller changed.";
		if (selection is not Selection token || !ReferenceEquals(token.Caster, caster) || !ReferenceEquals(token.Mover, recipient) ||
			!_requests.TryGetValue(recipient, out var request) || !ReferenceEquals(request.Rune, token.Rune?.Item) ||
			SaveToXml().ToString(SaveOptions.DisableFormatting) != token.Definition || !token.IsCurrent ||
			token.Rune is { } rune && !rune.IsCurrentFor(caster) || token.Candidates.Any(x => EndpointError(caster, token.Mover, x.Room, token.Layer, token.Config) is not null) || !token.IsCurrent) return false;
		_selections[recipient] = token; error = null; return true;
	}
	public bool TryConfirmPreparedSelection(ICharacter caster, IPerceivable recipient, out string? error)
	{ error = "The weighted destination was not prepared."; return _selections.TryGetValue(recipient, out var token) && TryReusePreparedSelection(token, caster, recipient, out error); }
	private static IRoom Draw(Selection token)
	{
		if (token.Rune is { } rune) return rune.Destination;
		var groups = token.Candidates.GroupBy(x => x.Group).OrderBy(x => x.Key).ToArray();
		int Weight(int group) => group == 0 ? token.Config.UrbanWeight : group == 1 ? token.Config.WildWeight : 1;
		var pick = Constants.Random.Next(groups.Sum(x => Weight(x.Key))); var selected = groups[0];
		foreach (var group in groups) { pick -= Weight(group.Key); if (pick < 0) { selected = group; break; } }
		var zones = selected.GroupBy(x => x.Room.OwningZone).ToArray(); var zone = zones[Constants.Random.Next(zones.Length)].ToArray();
		return zone[Constants.Random.Next(zone.Length)].Room;
	}
	public MagicEffectOperation Apply(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome, SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] parameters)
	{
		if (target is null || !TryConfirmPreparedSelection(caster, target, out _)) return new(MagicEffectOperationStatus.Rejected, null);
		var token = _selections[target]; var destination = Draw(token); token.Attempted = true;
		var mover = token.Physical.Travellers.Single(x => ReferenceEquals(x.Actor, token.Mover));
		var others = token.Physical.Travellers.Where(x => !ReferenceEquals(x.Actor, token.Mover)).ToArray();
		bool Raw() => token.Physical.Current && token.RuneRaw() && others.All(x => x.At(x.Origin, x.Layer));
		bool Current() => Raw() && !Tagged(mover.Origin, token.Config.Restricted) && !Tagged(destination, token.Config.Restricted) &&
			!Tagged(destination, token.Config.Lethal) && destination.OutdoorsType(token.Mover) == RoomOutdoorsType.Outdoors &&
			mover.Origin.SharesPlaneWith(destination) && SpellTravelEndpointPolicy.InFlightError(caster, token.Mover, mover.Origin,
				destination, Spell, mover.Layer, token.Layer) is null && Raw();
		try
		{
			using var displacement = CommandExecutionScope.EnterSoloDisplacement(token.Mover, Current);
			token.Mover.Teleport(destination, token.Layer, false, true);
			if (Current() && mover.At(destination, token.Layer)) return new(MagicEffectOperationStatus.Applied, null);
		}
		catch { /* Independently restore only this selected body and its captured position. */ }
		if (mover.Current && ReferenceEquals(Gameworld.Rooms.Get(mover.Origin.Id), mover.Origin) &&
			(ReferenceEquals(token.Mover.Location, mover.Origin) || ReferenceEquals(token.Mover.Location, destination)))
		{
			try
			{
				using var independent = CommandExecutionScope.EnterIndependent();
				using var repair = CommandExecutionScope.EnterSoloDisplacement(token.Mover, () => mover.Current);
				token.Mover.Teleport(mover.Origin, mover.Layer, false, false);
			}
			catch { /* Actual exact membership determines the outcome. */ }
		}
		if (!mover.At(mover.Origin, mover.Layer) || !others.All(x => x.At(x.Origin, x.Layer)))
			throw new InvalidOperationException("Weighted travel has an uncertain native outcome; reconcile the paid operation without replay.");
		return new(MagicEffectOperationStatus.NoChange, null);
	}
	public IMagicSpellEffect? GetOrApplyEffect(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome, SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] parameters) => Apply(caster, target, outcome, power, parent, parameters).Effect;
	public string Show(ICharacter actor) => "Separate same-plane weighted destination pools and an exact owned live Anchor Rune route; no arbitrary room lookup.";
	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		var option = command.PopForSwitch();
		if (option.EqualToAny("urbanweight", "wildweight"))
		{
			if (!int.TryParse(command.PopSpeech(), out var weight) || weight is < 1 or > 10000 || !command.IsFinished) return false;
			_definition.SetElementValue(option.EqualTo("urbanweight") ? "UrbanWeight" : "WildWeight", weight); Spell.Changed = true;
			actor.OutputHandler.Send("The weighted travel group weight has been updated."); return true;
		}
		if (option.EqualTo("planes"))
		{
			var ids = new List<long>();
			while (!command.IsFinished)
			{
				if (!long.TryParse(command.PopSpeech(), out var id) || id <= 0 || Gameworld.Planes.Get(id) is null || ids.Contains(id) || ids.Count >= 64) return false;
				ids.Add(id);
			}
			if (ids.Count == 0) return false;
			_definition.Element("ElementalPlanes")!.ReplaceWith(new XElement("ElementalPlanes", ids.Select(x => new XElement("Plane", x))));
			Spell.Changed = true; actor.OutputHandler.Send("The native elemental plane bindings have been updated."); return true;
		}
		var element = option switch { "restricted" => "RestrictedRoomTag", "lethal" => "LethalRoomTag", "urban" => "UrbanPoolTag", "wild" => "WildPoolTag", "elemental" => "ElementalPoolTag", _ => null };
		if (element is null || Gameworld.Tags.GetByIdOrName(command.SafeRemainingArgument) is not { } tag) return false;
		_definition.SetElementValue(element, tag.Id); Spell.Changed = true; actor.OutputHandler.Send("The weighted travel binding has been updated."); return true;
	}
}

public sealed class WaystepEffect(XElement xml, IMagicSpell spell) : WeightedTravelEffect(xml, spell, false, "waystep")
{
	public override IMagicSpellEffectTemplate Clone() => new WaystepEffect(SaveToXml(), Spell);
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("waystep", (xml, spell) => new WaystepEffect(xml, spell));
		SpellEffectFactory.RegisterBuilderFactory("waystep", (_, spell) => (new WaystepEffect(BuilderDefinition("waystep"), spell), ""),
			"Moves the caster through a weighted pool or exact owned live rune", "restricted|lethal|urban|wild|elemental <tag>; urbanweight|wildweight <weight>; planes <ids>", true, true, ["waystepdestination"]);
	}
}

public sealed class BanishTravellerEffect(XElement xml, IMagicSpell spell) : WeightedTravelEffect(xml, spell, true, "banishtraveller")
{
	public override IMagicSpellEffectTemplate Clone() => new BanishTravellerEffect(SaveToXml(), Spell);
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("banishtraveller", (xml, spell) => new BanishTravellerEffect(xml, spell));
		SpellEffectFactory.RegisterBuilderFactory("banishtraveller", (_, spell) => (new BanishTravellerEffect(BuilderDefinition("banishtraveller"), spell), ""),
			"Banishes one resisted local traveller through a weighted pool", "restricted|lethal|urban|wild|elemental <tag>; urbanweight|wildweight <weight>; planes <ids>", true, true, ["banishtraveller"]);
	}
}
