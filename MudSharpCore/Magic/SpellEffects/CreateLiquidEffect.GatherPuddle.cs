#nullable enable

using System.Xml.Linq;
using MudSharp.Body;
using MudSharp.Construction;
using MudSharp.Form.Material;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.SpellEffects;

public partial class CreateLiquidEffect : IMagicSpellEffectPreparedSelection
{
	private XElement? _gatherPuddlePolicy;
	private string? _gatherPuddleLoadError;
	private XElement? _unreadablePuddleDefinition;
	private readonly Dictionary<IPerceivable, PuddleSelection> _puddleSelections = new(ReferenceEqualityComparer.Instance);
	public bool GatherPuddleOnly => _gatherPuddlePolicy is not null;
	private void LoadGatherPuddle(XElement definition)
	{
		var markers = definition.Elements().Where(x => x.Name.LocalName == "GatherPuddle").ToArray();
		if (markers.Length == 0) return;
		_gatherPuddlePolicy = new(markers[0]);
		var policy = markers[0];
		if (markers.Length != 1 || policy.Name != "GatherPuddle" || policy.Attributes().Count() != 1 || (string?)policy.Attribute("version") != "1" ||
			policy.Elements().Count() != 1 || policy.Element("Litres") is not { HasElements: false, HasAttributes: false } litres ||
			litres.Value != "0.25*grade" || definition.Elements().Count() != 3 || definition.Element("LiquidId") is null || definition.Element("AmountFormula")?.Value != "0" || ContainerOnly)
		{
			_gatherPuddleLoadError = "Invalid explicit room-puddle policy; retain it for builder correction.";
			_unreadablePuddleDefinition = new(definition);
		}
	}
	private string? GatherPuddleDefinitionError() => _gatherPuddleLoadError ?? (!GatherPuddleOnly ? null :
		ContainerOnly ? "Room puddle creation cannot fill containers." : Liquid is null ? "Select the native clean-water liquid." : null);
	private sealed record PuddleSelection(ICharacter Caster, IRoom Room, IBody Body, RoomLayer Layer, double? Point, int Grade,
		ILiquid Liquid, double Conversion, double Capacity, double Amount, string Definition, Func<bool> Check, Func<bool> LiveCheck) : IMagicSpellEffectPreparedSelectionRawToken
	{
		public bool IsCurrent => Check();
	}
	private PuddleSelection PreparePuddle(ICharacter caster, IPerceivable recipient)
	{
		if (DefinitionError is { } error) throw new InvalidOperationException(error);
		if (!GatherPuddleOnly || recipient is not IRoom room || !ReferenceEquals(caster.Location, room) || !ReferenceEquals(room.Gameworld, Gameworld) ||
			!ReferenceEquals(caster.Gameworld, Gameworld) || room.Id <= 0 || !ReferenceEquals(Gameworld.Rooms.Get(room.Id), room) ||
			Spell is not MagicSpell { InvocationGrade: >= 1 and <= 7 } native)
			throw new InvalidOperationException("Gather Puddle requires the exact current native room and a selected external grade.");
		var body = caster.Body; var layer = caster.RoomLayer; var point = caster.RoutePositionMetres; var grade = native.InvocationGrade!.Value;
		var state = caster.State; var instance = caster.InstanceId; var identity = caster.Identity; var route = room.RouteDefinition; var routeLength = route?.LengthMetres;
		var liquid = Liquid; var definition = SaveToXml().ToString(SaveOptions.DisableFormatting);
		bool RawFrame() => ReferenceEquals(caster.Body, body) && ReferenceEquals(caster.Location, room) && ReferenceEquals(caster.Identity, identity) &&
			caster.InstanceId == instance && caster.State == state && caster.RoomLayer == layer && caster.RoutePositionMetres == point &&
			ReferenceEquals(caster.Gameworld, Gameworld) && ReferenceEquals(room.Gameworld, Gameworld) && ReferenceEquals(Gameworld.Rooms.Get(room.Id), room) &&
			ReferenceEquals(room.RouteDefinition, route) && route?.LengthMetres == routeLength && ReferenceEquals(Gameworld.Liquids.Get(_liquidId), liquid) &&
			native.InvocationGrade == grade && SaveToXml().ToString(SaveOptions.DisableFormatting) == definition;
		var conversion = Gameworld.UnitManager.BaseFluidToLitres;
		var capacity = Gameworld.GetStaticDouble("EnormousPoolLiquidQuantity");
		var enabled = Gameworld.GetStaticBool("PuddlesEnabled"); var swimming = room.IsSwimmingLayer(layer);
		var amount = 0.25 * grade / conversion;
		if (!enabled || swimming || route is null && point is not null || routeLength is { } length && (!double.IsFinite(length) || length <= 0) ||
			point is { } coordinate && (!double.IsFinite(coordinate) || coordinate < 0 || coordinate > routeLength) ||
			!double.IsFinite(conversion) || conversion <= 0 || !double.IsFinite(capacity) || capacity <= 0 || !double.IsFinite(amount) || amount <= 0 || !RawFrame())
			throw new InvalidOperationException("The native puddle route, amount, capacity or original physical frame is unavailable.");
		bool Current() => RawFrame() && Gameworld.UnitManager.BaseFluidToLitres == conversion && RawFrame();
		bool LiveCurrent() => Current() &&
			Gameworld.GetStaticDouble("EnormousPoolLiquidQuantity") == capacity && Gameworld.GetStaticBool("PuddlesEnabled") && !room.IsSwimmingLayer(layer) && RawFrame();
		var selection = new PuddleSelection(caster, room, body, layer, point, grade, liquid, conversion, capacity, Math.Min(amount, capacity), definition, Current, LiveCurrent);
		if (!selection.LiveCheck()) throw new InvalidOperationException("The room puddle frame changed during admission.");
		return selection;
	}
	private IMagicSpellEffectPreparedSelectionToken? CapturePuddleSelection(ICharacter caster, IPerceivable recipient)
	{
		if (!GatherPuddleOnly) return null;
		if (_puddleSelections.TryGetValue(recipient, out var existing))
		{
			if (!ReferenceEquals(existing.Caster, caster) || !MatchesPuddleReceiver(existing) || !existing.LiveCheck() || !MatchesPuddleReceiver(existing))
				throw new InvalidOperationException("The original room puddle selection changed; it cannot be captured again.");
			return existing;
		}
		var selection = PreparePuddle(caster, recipient); _puddleSelections[recipient] = selection; return selection;
	}
	private bool TryReusePuddleSelection(IMagicSpellEffectPreparedSelectionToken selection, ICharacter caster, IPerceivable recipient, out string? error)
	{
		error = "The original room puddle selection changed.";
		if (selection is not PuddleSelection token || !ReferenceEquals(token.Caster, caster) || !ReferenceEquals(token.Room, recipient) ||
			!MatchesPuddleReceiver(token) || !token.LiveCheck() || !MatchesPuddleReceiver(token)) return false;
		_puddleSelections[recipient] = token; error = null; return true;
	}
	private bool TryConfirmPuddleSelection(ICharacter caster, IPerceivable recipient, out string? error)
	{
		error = null;
		if (!GatherPuddleOnly) return true;
		if (_puddleSelections.TryGetValue(recipient, out var selection) && ReferenceEquals(selection.Caster, caster) &&
			MatchesPuddleReceiver(selection) && selection.LiveCheck() && MatchesPuddleReceiver(selection)) return true;
		error = "The original room puddle selection changed."; return false;
	}
	private bool MatchesPuddleReceiver(PuddleSelection selection) =>
		SaveToXml().ToString(SaveOptions.DisableFormatting) == selection.Definition && Spell is MagicSpell { InvocationGrade: { } grade } && grade == selection.Grade &&
		ReferenceEquals(Gameworld, selection.Room.Gameworld) && ReferenceEquals(Liquid, selection.Liquid) && selection.IsCurrent;
	private bool TryPreparePuddleApplication(ICharacter caster, IPerceivable recipient, out IMagicSpellEffectApplication? application, out string? error)
	{
		application = null; error = null;
		try
		{
			if (!_puddleSelections.TryGetValue(recipient, out var selection))
			{ selection = PreparePuddle(caster, recipient); _puddleSelections[recipient] = selection; }
			if (!ReferenceEquals(selection.Caster, caster) || !MatchesPuddleReceiver(selection) || !selection.LiveCheck() || !MatchesPuddleReceiver(selection))
				throw new InvalidOperationException("The original room puddle selection changed.");
			application = new PuddleCreation(selection); return true;
		}
		catch (InvalidOperationException exception) { error = exception.Message; return false; }
	}
	private sealed record PuddleCreation(PuddleSelection Selection) : IMagicSpellEffectApplicationOperation
	{
		private bool _attempted;
		public IMagicSpellEffect Create(IMagicSpellEffectParent parent) => Apply(parent).Effect!;
		public MagicEffectOperation Apply(IMagicSpellEffectParent parent)
		{
			if (_attempted) throw new InvalidOperationException("A room puddle application must never replay.");
			if (!Selection.LiveCheck()) throw new InvalidOperationException("The paid room puddle frame changed before native transfer.");
			_attempted = true;
			var mixture = new LiquidMixture(Selection.Liquid, Selection.Amount, Selection.Caster.Gameworld);
			if (!Selection.IsCurrent) throw new InvalidOperationException("The paid room puddle frame changed during mixture preparation.");
			Selection.Room.AddLiquidToSurface(mixture, Selection.Layer, Selection.Caster);
			if (!Selection.LiveCheck() || !mixture.IsEmpty)
				throw new InvalidOperationException("Native puddle transfer has an uncertain paid result; do not replay.");
			return new(MagicEffectOperationStatus.Applied, null);
		}
	}
}
