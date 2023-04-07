using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using System.Xml.Linq;

namespace MudSharp.Effects.Concrete;

public class Sneak : Effect, ISneakEffect, IScoreAddendumEffect
{
	public Sneak(IPerceivable owner)
		: base(owner)
	{
	}

	protected Sneak(XElement effect, IPerceivable owner)
		: base(effect, owner)
	{
	}

	public bool ShowInScore => true;
	public bool ShowInHealth => false;

	public string ScoreAddendum => "You are sneaking when you move.";

	protected override string SpecificEffectType => "Sneak";

	public override bool SavingEffect => true;

	public override string Describe(IPerceiver voyeur)
	{
		return "Sneaking";
	}

	public static void InitialiseEffectType()
	{
		RegisterFactory("Sneak", (effect, owner) => new Sneak(effect, owner));
	}

	public override string ToString()
	{
		return "Sneak Effect";
	}

	public bool Subtle => false;
}