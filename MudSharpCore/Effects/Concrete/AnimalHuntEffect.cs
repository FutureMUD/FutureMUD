#nullable enable

using System.Globalization;
using MudSharp.Combat;
using MudSharp.Construction;
using MudSharp.NPC;
using MudSharp.NPC.AI;

namespace MudSharp.Effects.Concrete;

/// <summary>Durable intent and observations only. Loading never replays an attack, dose, or trap payload.</summary>
public sealed class AnimalHuntEffect : Effect, ICombatTacticEffect
{
	private bool _selectingMove;
	private AnimalHuntPhase _phase;
	public long AiId { get; }
	public long TargetId { get; }
	public long OriginCellId { get; }
	public long LastCellId { get; private set; }
	public long LastRaceId { get; private set; }
	public RoomLayer LastLayer { get; private set; }
	public DateTime LastSeen { get; private set; }
	public DateTime Deadline { get; }
	public Guid? TrapId { get; private set; }
	public bool VenomDelivered { get; private set; }
	public AnimalHuntPhase Phase { get => _phase; set { if (_phase == value) return; _phase = value; Changed = true; } }
	public AnimalAI? Ai => (Owner as INPC)?.AIs.OfType<AnimalAI>().FirstOrDefault(x => x.Id == AiId);
	public ICharacter? Target => Gameworld.TryGetCharacter(TargetId, true);

	public AnimalHuntEffect(ICharacter owner, AnimalAI ai, ICharacter target) : base(owner)
	{
		AiId = ai.Id;
		TargetId = target.Id;
		OriginCellId = owner.Location.Id;
		Deadline = RuntimeClock.UtcNow + ai.Hunting.PursuitTimeout;
		Observe(target);
	}

	private AnimalHuntEffect(XElement root, IPerceivable owner) : base(root, owner)
	{
		var xml = root.Element("Effect")!;
		AiId = long.Parse(xml.Element("Ai")!.Value);
		TargetId = long.Parse(xml.Element("Target")!.Value);
		OriginCellId = long.Parse(xml.Element("Origin")!.Value);
		LastCellId = long.Parse(xml.Element("LastCell")!.Value);
		LastRaceId = long.TryParse(xml.Element("LastRace")?.Value, out var race) ? race : 0;
		LastLayer = Enum.Parse<RoomLayer>(xml.Element("LastLayer")!.Value);
		LastSeen = DateTime.Parse(xml.Element("LastSeen")!.Value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
		Deadline = DateTime.Parse(xml.Element("Deadline")!.Value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
		_phase = Enum.Parse<AnimalHuntPhase>(xml.Element("Phase")!.Value);
		TrapId = Guid.TryParse(xml.Element("Trap")?.Value, out var trap) ? trap : null;
		VenomDelivered = bool.TryParse(xml.Element("VenomDelivered")?.Value, out var delivered) && delivered;
	}

	public void RecordVenomDelivery() { VenomDelivered = true; Changed = true; }

	public void FollowDetectedTrail(ICell destination)
	{
		LastCellId = destination.Id;
		Changed = true;
	}

	public void Observe(ICharacter target)
	{
		LastCellId = target.Location.Id;
		LastRaceId = target.Race.Id;
		LastLayer = target.RoomLayer;
		LastSeen = RuntimeClock.UtcNow;
		TrapId = target.EffectsOfType<TrapRestraintEffect>().FirstOrDefault()?.TrapInstanceId;
		Changed = true;
	}

	public bool TrySelectMove(ICharacter actor, out ICombatMove? move)
	{
		move = null;
		if (_selectingMove) return false;
		if (Ai is not { Hunting.Enabled: true } ai)
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

	public void MoveResolved(ICombatMove move, CombatMoveResult result) => Ai?.HuntMoveResolved(this, move, result);
	public static void InitialiseEffectType() => RegisterFactory("AnimalHunt", (xml, owner) => new AnimalHuntEffect(xml, owner));
	protected override string SpecificEffectType => "AnimalHunt";
	public override bool SavingEffect => true;
	public override void ExpireEffect()
	{
		Phase = AnimalHuntPhase.Abandoned;
		if (Owner is ICharacter { Combat: not null }) Owner.Reschedule(this, TimeSpan.FromSeconds(10));
		else Owner.RemoveEffect(this);
	}
	public override string Describe(IPerceiver voyeur) => $"Hunting target #{TargetId.ToString("N0", voyeur)}: {Phase.DescribeEnum()}, until {Deadline:O}.";
	protected override XElement SaveDefinition() => new("Effect", new XElement("Ai", AiId), new XElement("Target", TargetId),
		new XElement("Origin", OriginCellId), new XElement("LastCell", LastCellId), new XElement("LastRace", LastRaceId), new XElement("LastLayer", LastLayer),
		new XElement("LastSeen", LastSeen.ToString("O")), new XElement("Deadline", Deadline.ToString("O")),
		new XElement("Phase", Phase), new XElement("Trap", TrapId), new XElement("VenomDelivered", VenomDelivered));
}
