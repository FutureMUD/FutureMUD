using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;

namespace MudSharp.Effects.Concrete;

public class SneakSubtle : Effect, ISneakEffect, IScoreAddendumEffect
{
	public SneakSubtle(IPerceivable owner)
		: base(owner)
	{
	}

	protected SneakSubtle(XElement effect, IPerceivable owner)
		: base(effect, owner)
	{
	}

	public bool ShowInScore => true;
	public bool ShowInHealth => false;

	public string ScoreAddendum => "You are sneaking subtly when you move.";

	protected override string SpecificEffectType => "SneakSubtle";

	public override bool SavingEffect => true;

	public override string Describe(IPerceiver voyeur)
	{
		return "Sneaking Subtly";
	}

	public static void InitialiseEffectType()
	{
		RegisterFactory("SneakSubtle", (effect, owner) => new SneakSubtle(effect, owner));
	}

	public override string ToString()
	{
		return "Sneak Subtle Effect";
	}

	public bool Subtle => true;
}