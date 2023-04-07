using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using System.Xml.Linq;

namespace MudSharp.Effects.Concrete;

public class DoorguardMode : Effect, IDoorguardModeEffect
{
	public DoorguardMode(IPerceivable owner)
		: base(owner)
	{
	}

	public DoorguardMode(XElement effect, IPerceivable owner)
		: base(effect, owner)
	{
	}

	protected override string SpecificEffectType => "DoorguardMode";

	public override bool SavingEffect => true;

	public override string Describe(IPerceiver voyeur)
	{
		return "Doorguard Mode";
	}

	public static void InitialiseEffectType()
	{
		RegisterFactory("DoorguardMode", (effect, owner) => new DoorguardMode(effect, owner));
	}

	public override string ToString()
	{
		return "Doorguard Mode Effect";
	}
}