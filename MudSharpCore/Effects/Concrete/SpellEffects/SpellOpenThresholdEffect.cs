#nullable enable

using System.Globalization;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Magic;
using MudSharp.Magic.SpellEffects;
using MudSharp.Planes;

namespace MudSharp.Effects.Concrete.SpellEffects;

public sealed class SpellOpenThresholdEffect : SpellPortalEffect, IIndependentlyExpiringSpellEffect
{
	private readonly long _sourceOverlay, _destinationOverlay, _restricted, _lethal;
	private readonly RoomLayer _sourceLayer, _destinationLayer;
	public DateTime? ExpiryUtc { get; }
	public SpellOpenThresholdEffect(IRoom owner, IMagicSpellEffectParent parent, IRoom destination, RoomLayer sourceLayer, RoomLayer destinationLayer,
		long restricted, long lethal, DateTime expiry) : base(owner, parent, owner, destination, "enter", "threshold", "threshold",
		"a shimmering threshold", "a shimmering threshold", "through", "through", 1.0)
	{
		_sourceOverlay = owner.CurrentOverlay.Id; _destinationOverlay = destination.CurrentOverlay.Id; _restricted = restricted; _lethal = lethal;
		_sourceLayer = sourceLayer; _destinationLayer = destinationLayer; ExpiryUtc = expiry;
	}
	private SpellOpenThresholdEffect(XElement xml, IPerceivable owner) : base(xml, owner)
	{
		var root = xml.Element("Effect")!;
		_sourceOverlay = (long)root.Element("SourceOverlay")!; _destinationOverlay = (long)root.Element("DestinationOverlay")!;
		_restricted = (long)root.Element("RestrictedTag")!; _lethal = (long)root.Element("LethalTag")!;
		_sourceLayer = (RoomLayer)(int)root.Element("SourceLayer")!; _destinationLayer = (RoomLayer)(int)root.Element("DestinationLayer")!;
		ExpiryUtc = DateTime.Parse(root.Element("ExpiryUtc")!.Value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind).ToUniversalTime();
		if (PortalInstanceId == Guid.Empty || new[] { SourceRoomId, DestinationRoomId, _sourceOverlay, _destinationOverlay, _restricted, _lethal }.Any(x => x <= 0) ||
			SourceRoomId == DestinationRoomId || !Enum.IsDefined(_sourceLayer) || !Enum.IsDefined(_destinationLayer)) throw new FormatException("Invalid exact threshold endpoints.");
	}
	public new static void InitialiseEffectType() => RegisterFactory("SpellOpenThreshold", (xml, owner) => new SpellOpenThresholdEffect(xml, owner));
	private bool EndpointsCurrent => RuntimeClock.UtcNow < ExpiryUtc && Gameworld.Rooms.Get(SourceRoomId) is { } source && Gameworld.Rooms.Get(DestinationRoomId) is { } destination &&
		ReferenceEquals(source, Owner) && source.CurrentOverlay.Id == _sourceOverlay && destination.CurrentOverlay.Id == _destinationOverlay &&
		!source.Temporary && !destination.Temporary && source.RouteDefinition is null && destination.RouteDefinition is null && source.SharesPlaneWith(destination) &&
		Gameworld.Tags.Get(_restricted) is { } restricted && Gameworld.Tags.Get(_lethal) is { } lethal &&
		!source.IsA(restricted) && !destination.IsA(restricted) && !source.IsA(lethal) && !destination.IsA(lethal);
	public override void InitialEffect() { if (EndpointsCurrent) base.InitialEffect(); }
	public override void Login() { if (EndpointsCurrent) base.Login(); }
	private bool AllowsTraversal(ICharacter actor, IRoomExit exit) => EndpointsCurrent && ReferenceEquals(exit.Exit, RegisteredExit) &&
		ParentEffect is { } parent && ReferenceEquals(parent.Owner, Owner) && Owner.Effects.Any(x => ReferenceEquals(x, this)) &&
		Owner.Effects.Any(x => ReferenceEquals(x, parent)) && parent.SpellEffects.Any(x => ReferenceEquals(x, this)) &&
		(exit.Origin.Id == SourceRoomId && exit.Destination.Id == DestinationRoomId && actor.RoomLayer == _sourceLayer ||
		 exit.Origin.Id == DestinationRoomId && exit.Destination.Id == SourceRoomId && actor.RoomLayer == _destinationLayer) &&
		_sourceLayer == _destinationLayer &&
		SpellTravelEndpointPolicy.RelocationError(actor, actor, exit.Origin, exit.Destination, Spell, actor.RoomLayer, actor.RoomLayer) is null && EndpointsCurrent;
	protected override IExit CreatePortalExit(IRoom source, IRoom destination) => new ThresholdExit(this, source, destination);
	private sealed class ThresholdExit(SpellOpenThresholdEffect effect, IRoom source, IRoom destination)
		: TransientExit(effect.Gameworld, source, destination, "enter", "threshold", "threshold", "a shimmering threshold", "a shimmering threshold", "through", "through", 1.0,
			null, effect.Spell, effect, $"spell-threshold:{effect.PortalInstanceId:D}"), IAdmittedMagicPortalExit
	{
		public bool AllowsTraversal(ICharacter actor, IRoomExit exit) => effect.AllowsTraversal(actor, exit);
	}
	protected override string SpecificEffectType => "SpellOpenThreshold";
	protected override XElement SaveDefinition()
	{
		var xml = base.SaveDefinition(); xml.Add(new XElement("SourceOverlay", _sourceOverlay), new XElement("DestinationOverlay", _destinationOverlay),
			new XElement("RestrictedTag", _restricted), new XElement("LethalTag", _lethal), new XElement("SourceLayer", (int)_sourceLayer), new XElement("DestinationLayer", (int)_destinationLayer),
			new XElement("ExpiryUtc", ExpiryUtc!.Value.ToString("O", CultureInfo.InvariantCulture))); return xml;
	}
}
