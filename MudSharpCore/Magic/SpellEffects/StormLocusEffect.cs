#nullable enable

using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Magic.Lifecycle;
using MudSharp.Planes;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.SpellEffects;

public sealed class StormLocusEffect : IMagicSpellEffectTemplate, IMagicSpellEffectPreparedSelection, IMagicSpellEffectOperation
{
	private readonly XElement _definition;
	private readonly Dictionary<IPerceivable, Selection> _selections = new(ReferenceEqualityComparer.Instance);
	public StormLocusEffect(XElement xml, IMagicSpell spell) { _definition = new(xml); Spell = spell; }
	public IMagicSpell Spell { get; }
	public IFuturemud Gameworld => Spell.Gameworld;
	public bool IsInstantaneous => false;
	public bool RequiresTarget => true;
	public XElement SaveToXml() => new(_definition);
	public IMagicSpellEffectTemplate Clone() => new StormLocusEffect(SaveToXml(), Spell);
	public bool IsCompatibleWithTrigger(IMagicTrigger trigger) => trigger is SpellTriggers.CastingTriggerRoom;
	public string? DefinitionError => (int?)_definition.Attribute("version") != 1 ||
		new[] { "WaterPlane", "VoidPlane" }.Any(x => Gameworld.Planes.Get((long?)_definition.Element(x) ?? 0) is null) ||
		Gameworld.Merits.Get((long?)_definition.Element("DefilerMerit") ?? 0) is null ? "Bind Storm Locus to exact native Water/Void planes and Defiler merit." : null;
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("stormlocus", (xml, spell) => new StormLocusEffect(xml, spell));
		SpellEffectFactory.RegisterBuilderFactory("stormlocus", (_, spell) => (new StormLocusEffect(new XElement("Effect", new XAttribute("type", "stormlocus"),
			new XAttribute("version", 1)), spell), ""), "Persistent bounded room electrical hazard", "water|void <plane>; defiler <merit>", false, true, ["room"]);
	}
	internal static bool RoomAllowed(ICharacter caster, IRoom room, IMagicSpell spell, XElement policy)
	{
		var planes = room.GetPlanarPresence().PresencePlaneIds;
		return room.Id > 0 && !room.Temporary && room.RouteDefinition is null && ReferenceEquals(room.Gameworld.Rooms.Get(room.Id), room) &&
			room.OutdoorsType(caster) == RoomOutdoorsType.Outdoors && !planes.Contains((long)policy.Element("WaterPlane")!) &&
			(!planes.Contains((long)policy.Element("VoidPlane")!) || caster.Merits.Any(x => x.Id == (long)policy.Element("DefilerMerit")!)) &&
			caster.CanInteractPlanar(room, PlanarInteractionKind.Magic) && MagicInterdictionHelper.GetInterdiction(caster, room, spell.School, false) is null;
	}
	private sealed record Selection(ICharacter Caster, IRoom Room, int Grade, SpellTravelSnapshot Physical, string Definition,
		ISpellRoomHazardJournal Journal, Func<bool> Raw) : IMagicSpellEffectPreparedSelectionRawToken
	{ public bool Attempted { get; set; } public bool IsCurrent => !Attempted && Physical.AtCapturedPositions && Raw(); }
	public IMagicSpellEffectPreparedSelectionToken CapturePreparedSelection(ICharacter caster, IPerceivable recipient)
	{
		if (_selections.TryGetValue(recipient, out var prior)) return prior;
		if (DefinitionError is { } error) throw new InvalidOperationException(error);
		if (recipient is not IRoom room || !ReferenceEquals(caster.Location, room) || Spell is not MagicSpell { InvocationGrade: >= 1 and <= 7 } spell ||
			Gameworld.SpellRoomHazards is not { } journal) throw new InvalidOperationException("Storm Locus requires the caster's room and a durable pulse journal.");
		var physical = new SpellTravelSnapshot(Gameworld, [caster], [room]); var definition = SaveToXml().ToString(SaveOptions.DisableFormatting);
		var merits = caster.Merits.ToArray();
		var token = new Selection(caster, room, spell.InvocationGrade.Value, physical, definition, journal,
			() => definition == SaveToXml().ToString(SaveOptions.DisableFormatting) && caster.Merits.SequenceEqual(merits, ReferenceEqualityComparer.Instance) &&
				!room.Effects.OfType<SpellStormLocusEffect>().Any() && ReferenceEquals(Gameworld.SpellRoomHazards, journal));
		if (!token.IsCurrent || !RoomAllowed(caster, room, Spell, _definition) || !token.IsCurrent)
			throw new InvalidOperationException("The outdoor room, native planes, wards or existing hazard refuse Storm Locus.");
		_selections.Add(recipient, token); return token;
	}
	public bool TryReusePreparedSelection(IMagicSpellEffectPreparedSelectionToken selection, ICharacter caster, IPerceivable recipient, out string? error)
	{
		error = "Storm Locus's exact caster, room or policy changed.";
		if (selection is not Selection token || !ReferenceEquals(token.Caster, caster) || !ReferenceEquals(token.Room, recipient) ||
			Spell is not MagicSpell native || native.InvocationGrade != token.Grade || !token.IsCurrent || !RoomAllowed(caster, token.Room, Spell, _definition) || !token.IsCurrent) return false;
		_selections[recipient] = token; error = null; return true;
	}
	public bool TryConfirmPreparedSelection(ICharacter caster, IPerceivable recipient, out string? error)
	{ error = "The room hazard was not prepared."; return _selections.TryGetValue(recipient, out var token) && TryReusePreparedSelection(token, caster, recipient, out error); }
	public MagicEffectOperation Apply(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome, SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] parameters)
	{
		if (target is null || !TryConfirmPreparedSelection(caster, target, out _)) return new(MagicEffectOperationStatus.Rejected, null);
		var token = _selections[target]; var clock = RuntimeClock.UtcNow;
		var now = new DateTime(clock.Ticks - clock.Ticks % 10, DateTimeKind.Utc);
		var anchor = new SpellReturnAnchor(CharacterInstanceIdentityComparer.IdentityId(caster), caster.InstanceId, caster.Body.Id,
			token.Room.Id, token.Room.CurrentOverlay.Id, token.Room.CurrentOverlay.Terrain.Id, caster.RoomLayer);
		SpellReturnAnchor.Parse(anchor.Save());
		var origin = new SpellLifecycleOrigin(Guid.NewGuid(), Spell.Id, token.Grade, anchor.CharacterId, SpellRoomHazardJournal.Family,
			SpellLifecycleMode.TemporaryCleanup, now, now.AddSeconds(30 * token.Grade), anchor.Save());
		token.Attempted = true; var life = token.Journal.Create(origin);
		if (life.Origin != origin || life.Entities.Count != 0 || !token.Physical.AtCapturedPositions || !token.Raw())
			throw new InvalidOperationException("Paid hazard activation changed after its durable journal; never replay.");
		return new(MagicEffectOperationStatus.Applied, new SpellStormLocusEffect(token.Room, parent, origin, SaveToXml()));
	}
	public IMagicSpellEffect? GetOrApplyEffect(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome, SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] parameters) => Apply(caster, target, outcome, power, parent, parameters).Effect;
	public string Show(ICharacter actor) => "One room hazard for 30 seconds per grade; durable 10-second pulses cause 2 electrical damage per grade to distinct eligible occupants, without training.";
	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		var option = command.PopForSwitch(); var element = option switch { "water" => "WaterPlane", "void" => "VoidPlane", "defiler" => "DefilerMerit", _ => null };
		if (element is null || !long.TryParse(command.PopSpeech(), out var id) || !command.IsFinished ||
			(element == "DefilerMerit" ? Gameworld.Merits.Get(id) is null : Gameworld.Planes.Get(id) is null)) return false;
		_definition.SetElementValue(element, id); Spell.Changed = true; actor.OutputHandler.Send("The native room hazard policy binding has been updated."); return true;
	}
}
