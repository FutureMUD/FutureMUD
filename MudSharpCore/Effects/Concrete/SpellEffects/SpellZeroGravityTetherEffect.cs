using MudSharp.Construction;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp.Effects.Concrete.SpellEffects;

public class SpellZeroGravityTetherEffect : MagicSpellEffectBase, IZeroGravityTetherEffect
{
	public static void InitialiseEffectType()
	{
		RegisterFactory("SpellZeroGravityTether", (effect, owner) => new SpellZeroGravityTetherEffect(effect, owner));
	}

	public SpellZeroGravityTetherEffect(IPerceivable owner, IMagicSpellEffectParent parent, IFutureProg prog, IPerceivable anchor, int maximumRooms) : base(owner, parent, prog)
	{
		Anchor = anchor;
		MaximumRooms = maximumRooms;
	}

	protected SpellZeroGravityTetherEffect(XElement root, IPerceivable owner) : base(root, owner)
	{
		var tr = root.Element("Effect");
		Anchor = Gameworld.GetPerceivable(tr!.Element("AnchorType")!.Value, long.Parse(tr.Element("AnchorId")!.Value));
		MaximumRooms = int.Parse(tr.Element("MaximumRooms")!.Value);
	}

	protected override XElement SaveDefinition()
	{
		return new XElement("Effect",
			new XElement("ApplicabilityProg", ApplicabilityProg?.Id ?? 0),
			new XElement("AnchorType", Anchor.FrameworkItemType),
			new XElement("AnchorId", Anchor.Id),
			new XElement("MaximumRooms", MaximumRooms)
		);
	}

	protected override string SpecificEffectType => "SpellZeroGravityTether";

	public IPerceivable Anchor { get; }

	public IGameItem PhysicalTether => null;

	public int MaximumRooms { get; }

	public override string Describe(IPerceiver voyeur)
	{
		return $"Magically tethered to {Anchor.HowSeen(voyeur, colour: false).ColourName()} with a maximum length of {MaximumRooms.ToString("N0", voyeur).ColourValue()} rooms.";
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
