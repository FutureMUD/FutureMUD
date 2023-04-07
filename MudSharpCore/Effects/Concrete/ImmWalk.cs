using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using System.Xml.Linq;

namespace MudSharp.Effects.Concrete;

public class Immwalk : Effect, IImmwalkEffect, IAdminEffect
{
	public Immwalk(IPerceivable owner)
		: base(owner)
	{
	}

	public Immwalk(XElement effect, IPerceivable owner)
		: base(effect, owner)
	{
	}

	protected override string SpecificEffectType => "Immwalk";

	public override bool SavingEffect => true;

	public override string Describe(IPerceiver voyeur)
	{
		return "Imm Walk";
	}

	public static void InitialiseEffectType()
	{
		RegisterFactory("Immwalk", (effect, owner) => new Immwalk(effect, owner));
	}

	public override string ToString()
	{
		return "Imm Walk Effect";
	}
}