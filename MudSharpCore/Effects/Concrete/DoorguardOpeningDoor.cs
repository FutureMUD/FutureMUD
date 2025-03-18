using System.Xml.Linq;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;

namespace MudSharp.Effects.Concrete;

public class DoorguardOpeningDoor : Effect, IDoorguardOpeningDoorEffect
{
	public DoorguardOpeningDoor(IPerceivable owner, ICellExit exit)
		: base(owner)
	{
		Exit = exit;
	}

	public DoorguardOpeningDoor(IPerceivable owner, XElement element) : base(owner)
	{
		Exit = Gameworld.ExitManager.GetExitByID(long.Parse(element.Element("Exit").Value))?.CellExitFor(owner.Location);
	}

	#region Overrides of Effect

	/// <inheritdoc />
	protected override XElement SaveDefinition()
	{
		return new XElement("Definition", new XElement("Exit", Exit?.Exit.Id ?? 0));
	}

	/// <inheritdoc />
	public override bool Applies(object target)
	{
		if (target is ICellExit ce)
		{
			return ce == Exit;
		}

		return base.Applies(target);
	}

	#endregion

	protected override string SpecificEffectType => "DoorguardOpeningDoor";

	public ICellExit Exit { get; }

	public override string Describe(IPerceiver voyeur)
	{
		return $"Doorguard is Opening a Door to {Exit?.OutboundDirectionDescription ?? "somewhere"}";
	}

	public static void InitialiseEffectType()
	{
		RegisterFactory("DoorguardOpeningDoor", (effect, owner) => new DoorguardOpeningDoor(owner, effect));
	}

	public override string ToString()
	{
		return "Doorguard Opening Door Effect";
	}
}