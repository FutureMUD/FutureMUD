#nullable enable

using MudSharp.Body.Position.PositionStates;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;

namespace MudSharp.Effects.Concrete;

public sealed class RoomExitPolicyEffect : Effect, IRoomExitPolicyEffect
{
	public RoomExitPolicyEffect(IRoom owner, CardinalDirection direction, string message,
		long accessProgId = 0, bool requiresFlight = false, bool controlsOpening = false, long deniedAttemptProgId = 0) : base(owner)
	{
		Direction = direction;
		Message = message;
		AccessProgId = accessProgId;
		RequiresFlight = requiresFlight;
		ControlsOpening = controlsOpening;
		DeniedAttemptProgId = deniedAttemptProgId;
	}

	public RoomExitPolicyEffect(XElement root, IPerceivable owner) : base(root, owner)
	{
		var definition = root.Element("Effect")!;
		Direction = (CardinalDirection)(int)definition.Element("Direction")!;
		Message = (string?)definition.Element("Message") ?? "You cannot travel that way.";
		AccessProgId = (long?)definition.Element("AccessProg") ?? 0;
		RequiresFlight = (bool?)definition.Element("RequiresFlight") ?? false;
		ControlsOpening = (bool?)definition.Element("ControlsOpening") ?? false;
		DeniedAttemptProgId = (long?)definition.Element("DeniedAttemptProg") ?? 0;
	}

	public CardinalDirection Direction { get; }
	public long AccessProgId { get; }
	public bool RequiresFlight { get; }
	public bool ControlsOpening { get; }
	public long DeniedAttemptProgId { get; }
	public string Message { get; }
	protected override string SpecificEffectType => "RoomExitPolicy";
	public override bool SavingEffect => true;
	public override string Describe(IPerceiver voyeur) =>
		$"{Direction.DescribeEnum().ColourName()} access{(ControlsOpening ? " and opening" : "")}: {(RequiresFlight ? "flight required" : AccessProgId > 0 ? $"prog #{AccessProgId}" : "denied")}; denied attempt prog #{DeniedAttemptProgId}; {Message.ColourCommand()}";
	protected override XElement SaveDefinition() => new("Effect", new XElement("Direction", (int)Direction),
		new XElement("Message", Message), new XElement("AccessProg", AccessProgId), new XElement("RequiresFlight", RequiresFlight),
		new XElement("ControlsOpening", ControlsOpening), new XElement("DeniedAttemptProg", DeniedAttemptProgId));
	public static void InitialiseEffectType() =>
		RegisterFactory("RoomExitPolicy", (effect, owner) => new RoomExitPolicyEffect(effect, owner));

	public string? DenialReason(ICharacter? actor, IRoomExit exit)
	{
		if (exit.OutboundDirection != Direction || !Applies(actor)) return null;
		if (RequiresFlight && (actor?.IsAdministrator() == true || actor?.PositionState == PositionFlying.Instance)) return null;
		if (AccessProgId > 0 && actor is not null &&
		    Gameworld.FutureProgs.Get(AccessProgId)?.ExecuteBool(actor, exit.Origin, exit.Destination) == true) return null;
		return Message;
	}

	public void OnDeniedAttempt(ICharacter actor, IRoomExit exit)
	{
		if (DeniedAttemptProgId > 0 && ReferenceEquals(actor.Location, exit.Origin) && DenialReason(actor, exit) is not null)
			Gameworld.FutureProgs.Get(DeniedAttemptProgId)?.Execute(actor, exit.Origin, exit.Destination);
	}
}
