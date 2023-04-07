using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;

namespace MudSharp.Effects.Concrete;

public class IgnoreForce : Effect, IIgnoreForceEffect
{
	public IgnoreForce(IPerceivable owner)
		: base(owner)
	{
	}

	protected IgnoreForce(XElement effect, IPerceivable owner)
		: base(effect, owner)
	{
	}

	protected override string SpecificEffectType => "IgnoreForce";

	public override bool SavingEffect => true;

	public override string Describe(IPerceiver voyeur)
	{
		return "Character will not respond to FORCE command";
	}

	public static void InitialiseEffectType()
	{
		RegisterFactory("IgnoreForce", (effect, owner) => new IgnoreForce(effect, owner));
	}

	public override string ToString()
	{
		return "IgnoreForce Effect";
	}
}