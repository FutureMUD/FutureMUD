using MudSharp.Construction;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using System.Xml.Linq;

namespace MudSharp.Effects.Concrete;

public class OverrideGravity : Effect, IGravityOverrideEffect
{
	public static void InitialiseEffectType()
	{
		RegisterFactory("OverrideGravity", (effect, owner) => new OverrideGravity(effect, owner));
	}

	public OverrideGravity(IPerceivable owner, GravityModel gravityModel, IFutureProg applicabilityProg = null) : base(owner, applicabilityProg)
	{
		GravityModel = gravityModel;
	}

	protected OverrideGravity(XElement effect, IPerceivable owner) : base(effect, owner)
	{
		var root = effect.Element("Effect");
		GravityModel = (GravityModel)int.Parse(root!.Element("GravityModel")!.Value);
	}

	protected override XElement SaveDefinition()
	{
		return new XElement("Effect",
			new XElement("GravityModel", (int)GravityModel)
		);
	}

	protected override string SpecificEffectType => "OverrideGravity";

	public override bool SavingEffect => true;

	public GravityModel GravityModel { get; }

	public int Priority => 100;

	public override string Describe(IPerceiver voyeur)
	{
		return $"Overriding local gravity to {GravityModel.DescribeColour()}.";
	}

	public override void InitialEffect()
	{
		if (Owner is ICell cell)
		{
			cell.CheckFallExitStatus();
		}
	}

	public override void RemovalEffect()
	{
		if (Owner is ICell cell)
		{
			cell.CheckFallExitStatus();
		}
	}
}
