using MudSharp.Effects.Interfaces;
using MudSharp.Framework;

namespace MudSharp.Effects.Concrete;

public class DoorguardOpeningDoor : Effect, IDoorguardOpeningDoorEffect
{
	public DoorguardOpeningDoor(IPerceivable owner)
		: base(owner)
	{
	}

	protected override string SpecificEffectType => "DoorguardOpeningDoor";

	public override string Describe(IPerceiver voyeur)
	{
		return "Doorguard is Opening a Door";
	}

	public static void InitialiseEffectType()
	{
		RegisterFactory("DoorguardOpeningDoor", (effect, owner) => new DoorguardOpeningDoor(owner));
	}

	public override string ToString()
	{
		return "Doorguard Opening Door Effect";
	}
}