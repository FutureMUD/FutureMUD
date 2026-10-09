using MudSharp.Construction;
using MudSharp.GameItems;

namespace MudSharp.Effects.Concrete;

public class ZeroGravityTether : Effect, IZeroGravityTetherEffect
{
	private bool _eventsRegistered;
	public static void InitialiseEffectType()
	{
		RegisterFactory("ZeroGravityTether", (effect, owner) => new ZeroGravityTether(effect, owner));
	}

	public ZeroGravityTether(IPerceivable owner, IPerceivable anchor, int maximumRooms, IGameItem physicalTether = null, IFutureProg applicabilityProg = null) : base(owner, applicabilityProg)
	{
		Anchor = anchor;
		MaximumRooms = maximumRooms;
		PhysicalTether = physicalTether;
		LoadErrors = Anchor is null;
	}

	protected ZeroGravityTether(XElement effect, IPerceivable owner) : base(effect, owner)
	{
		var root = effect.Element("Effect");
		var anchorType = root!.Element("AnchorType")!.Value;
		var anchorId = long.Parse(root.Element("AnchorId")!.Value);
		Anchor = Gameworld.GetPerceivable(anchorType, anchorId);
		var physicalTetherId = long.Parse(root.Element("PhysicalTetherId")!.Value);
		if (physicalTetherId > 0)
		{
			PhysicalTether = Gameworld.GetPerceivable("GameItem", physicalTetherId) as IGameItem;
		}

		MaximumRooms = int.Parse(root.Element("MaximumRooms")!.Value);
		LoadErrors = Anchor is null || physicalTetherId > 0 && PhysicalTether is null;
	}

	public override void InitialEffect() => RegisterEvents();
	public override void Login() => RegisterEvents();
	public override void RemovalEffect() => UnregisterEvents();

	private void RegisterEvents()
	{
		if (_eventsRegistered || LoadErrors) return;
		Anchor.OnDeleted += TetherTargetDeleted;
		if (PhysicalTether is not null) PhysicalTether.OnDeleted += TetherTargetDeleted;
		Owner.OnQuit += OwnerQuit;
		_eventsRegistered = true;
	}

	private void UnregisterEvents()
	{
		if (!_eventsRegistered) return;
		Anchor.OnDeleted -= TetherTargetDeleted;
		if (PhysicalTether is not null) PhysicalTether.OnDeleted -= TetherTargetDeleted;
		Owner.OnQuit -= OwnerQuit;
		_eventsRegistered = false;
	}

	private void OwnerQuit(IPerceivable owner) => UnregisterEvents();

	private void TetherTargetDeleted(IPerceivable target)
	{
		LoadErrors = true;
		UnregisterEvents();
		Owner.RemoveEffect(this, true);
	}

	protected override XElement SaveDefinition()
	{
		return new XElement("Effect",
			new XElement("AnchorType", Anchor?.GetPersistedReferenceType() ?? "GameItem"),
			new XElement("AnchorId", Anchor?.Id ?? 0),
			new XElement("PhysicalTetherId", PhysicalTether?.Id ?? 0),
			new XElement("MaximumRooms", MaximumRooms)
		);
	}

	protected override string SpecificEffectType => "ZeroGravityTether";

	public override bool SavingEffect => !LoadErrors;

	public IPerceivable Anchor { get; }

	public IGameItem PhysicalTether { get; }

	public int MaximumRooms { get; }

	public override string Describe(IPerceiver voyeur)
	{
		if (LoadErrors || Anchor is null) return "The tether no longer has a valid anchor or tether item.";
		return $"Tethered to {Anchor.HowSeen(voyeur, colour: false).ColourName()} with a maximum length of {MaximumRooms.ToString("N0", voyeur).ColourValue()} rooms.";
	}

	public bool BlocksMovementTo(IRoom destination)
	{
		if (LoadErrors || Anchor is null) return false;
		var anchorLocation = Anchor as IRoom ?? Anchor.Location;
		if (anchorLocation is null)
		{
			return true;
		}

		if (destination == anchorLocation)
		{
			return false;
		}

		var visited = new HashSet<IRoom> { anchorLocation };
		var frontier = new Queue<(IRoom Room, int Distance)>();
		frontier.Enqueue((anchorLocation, 0));
		while (frontier.Count > 0)
		{
			var (room, distance) = frontier.Dequeue();
			if (distance >= MaximumRooms)
			{
				continue;
			}

			foreach (var exit in room.ExitsFor(null, true))
			{
				if (!visited.Add(exit.Destination))
				{
					continue;
				}

				var newDistance = distance + 1;
				if (exit.Destination == destination)
				{
					return newDistance > MaximumRooms;
				}

				frontier.Enqueue((exit.Destination, newDistance));
			}
		}

		return true;
	}
}
