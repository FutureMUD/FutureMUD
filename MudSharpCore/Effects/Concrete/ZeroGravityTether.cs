using MudSharp.Construction;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp.Effects.Concrete;

public class ZeroGravityTether : Effect, IZeroGravityTetherEffect
{
	public static void InitialiseEffectType()
	{
		RegisterFactory("ZeroGravityTether", (effect, owner) => new ZeroGravityTether(effect, owner));
	}

	public ZeroGravityTether(IPerceivable owner, IPerceivable anchor, int maximumRooms, IGameItem physicalTether = null, IFutureProg applicabilityProg = null) : base(owner, applicabilityProg)
	{
		Anchor = anchor;
		MaximumRooms = maximumRooms;
		PhysicalTether = physicalTether;
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
	}

	protected override XElement SaveDefinition()
	{
		return new XElement("Effect",
			new XElement("AnchorType", Anchor.FrameworkItemType),
			new XElement("AnchorId", Anchor.Id),
			new XElement("PhysicalTetherId", PhysicalTether?.Id ?? 0),
			new XElement("MaximumRooms", MaximumRooms)
		);
	}

	protected override string SpecificEffectType => "ZeroGravityTether";

	public override bool SavingEffect => true;

	public IPerceivable Anchor { get; }

	public IGameItem PhysicalTether { get; }

	public int MaximumRooms { get; }

	public override string Describe(IPerceiver voyeur)
	{
		return $"Tethered to {Anchor.HowSeen(voyeur, colour: false).ColourName()} with a maximum length of {MaximumRooms.ToString("N0", voyeur).ColourValue()} rooms.";
	}

	public bool BlocksMovementTo(ICell destination)
	{
		var anchorLocation = Anchor as ICell ?? Anchor.Location;
		if (anchorLocation is null)
		{
			return true;
		}

		if (destination == anchorLocation)
		{
			return false;
		}

		var visited = new HashSet<ICell> { anchorLocation };
		var frontier = new Queue<(ICell Cell, int Distance)>();
		frontier.Enqueue((anchorLocation, 0));
		while (frontier.Count > 0)
		{
			var (cell, distance) = frontier.Dequeue();
			if (distance >= MaximumRooms)
			{
				continue;
			}

			foreach (var exit in cell.ExitsFor(null, true))
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
