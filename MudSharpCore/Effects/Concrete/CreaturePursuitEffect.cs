#nullable enable

using System.Globalization;
using MudSharp.Combat;
using MudSharp.Construction;
using MudSharp.NPC;
using MudSharp.NPC.AI;

namespace MudSharp.Effects.Concrete;

/// <summary>Durable intent and observations only. Loading never replays an attack, dose, or trap payload.</summary>
public abstract class CreaturePursuitEffect : Effect, ICombatTacticEffect
{
	private bool _selectingMove;
	private AnimalHuntPhase _phase;
	public long AiId { get; }
	public long TargetId { get; }
	public long OriginRoomId { get; }
	public long LastRoomId { get; private set; }
	public long LastRaceId { get; private set; }
	public RoomLayer LastLayer { get; private set; }
	public double? LastRoutePosition { get; private set; }
	public DateTime LastSeen { get; private set; }
	public DateTime Deadline { get; }
	public Guid? TrapId { get; private set; }
	public bool VenomDelivered { get; private set; }
	public AnimalHuntPhase Phase { get => _phase; set { if (_phase == value) return; _phase = value; Changed = true; } }
	public CreatureAIBase? Ai => (Owner as INPC)?.AIs.OfType<CreatureAIBase>().FirstOrDefault(x => x.Id == AiId);
	public ICharacter? Target => Gameworld.TryGetCharacter(TargetId, true);

	protected CreaturePursuitEffect(ICharacter owner, CreatureAIBase ai, ICharacter target) : base(owner)
	{
		AiId = ai.Id;
		TargetId = target.Id;
		OriginRoomId = owner.Location.Id;
		Deadline = RuntimeClock.UtcNow + ai.Hunting.PursuitTimeout;
		Observe(target);
	}

	protected CreaturePursuitEffect(XElement root, IPerceivable owner) : base(root, owner)
	{
		var xml = root.Element("Effect")!;
		AiId = long.Parse(xml.Element("Ai")!.Value);
		TargetId = long.Parse(xml.Element("Target")!.Value);
		OriginRoomId = long.Parse(xml.Element("Origin")!.Value);
		LastRoomId = long.Parse(xml.Element("LastCell")!.Value);
		LastRaceId = long.TryParse(xml.Element("LastRace")?.Value, out var race) ? race : 0;
		LastLayer = Enum.Parse<RoomLayer>(xml.Element("LastLayer")!.Value);
		LastRoutePosition = double.TryParse(xml.Element("LastRoutePosition")?.Value, NumberStyles.Float,
			CultureInfo.InvariantCulture, out var position) && double.IsFinite(position) ? position : null;
		LastSeen = DateTime.Parse(xml.Element("LastSeen")!.Value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
		Deadline = DateTime.Parse(xml.Element("Deadline")!.Value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
		_phase = Enum.Parse<AnimalHuntPhase>(xml.Element("Phase")!.Value);
		TrapId = Guid.TryParse(xml.Element("Trap")?.Value, out var trap) ? trap : null;
		VenomDelivered = bool.TryParse(xml.Element("VenomDelivered")?.Value, out var delivered) && delivered;
	}

	public void RecordVenomDelivery() { VenomDelivered = true; Changed = true; }

	public void FollowDetectedTrail(IRoom destination)
	{
		LastRoomId = destination.Id;
		Changed = true;
	}

	public void Observe(ICharacter target)
	{
		LastRoomId = target.Location.Id;
		LastRaceId = target.Race.Id;
		LastLayer = target.RoomLayer;
		LastRoutePosition = RouteSpatialService.Instance.GetEffectiveLocation(target).RoutePositionMetres;
		LastSeen = RuntimeClock.UtcNow;
		TrapId = target.EffectsOfType<TrapRestraintEffect>().FirstOrDefault()?.TrapInstanceId;
		Changed = true;
	}

	public bool TrySelectMove(ICharacter actor, out ICombatMove? move)
	{
		move = null;
		if (_selectingMove) return false;
		if (Ai is not { } ai || !AllowsTactics)
		{
			Owner.RemoveEffect(this);
			return false;
		}
		try
		{
			_selectingMove = true;
			return ai.SelectHuntMove(actor, this, out move);
		}
		finally { _selectingMove = false; }
	}
	protected virtual bool AllowsTactics => Ai is { Hunting.Enabled: true };

	public void MoveResolved(ICombatMove move, CombatMoveResult result) => Ai?.HuntMoveResolved(this, move, result);
	public override bool SavingEffect => true;
	public override void ExpireEffect()
	{
		Phase = AnimalHuntPhase.Abandoned;
		if (Owner is ICharacter { Combat: not null }) Owner.Reschedule(this, TimeSpan.FromSeconds(10));
		else Owner.RemoveEffect(this);
	}
	public override string Describe(IPerceiver voyeur) => $"Hunting target #{TargetId.ToString("N0", voyeur)}: {Phase.DescribeEnum()}, until {Deadline:O}.";
	protected override XElement SaveDefinition() => new("Effect", new XElement("Ai", AiId), new XElement("Target", TargetId),
		new XElement("Origin", OriginRoomId), new XElement("LastCell", LastRoomId), new XElement("LastRace", LastRaceId), new XElement("LastLayer", LastLayer),
		new XElement("LastRoutePosition", LastRoutePosition),
		new XElement("LastSeen", LastSeen.ToString("O")), new XElement("Deadline", Deadline.ToString("O")),
		new XElement("Phase", Phase), new XElement("Trap", TrapId), new XElement("VenomDelivered", VenomDelivered));
}
