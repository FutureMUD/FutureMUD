using MudSharp.Construction;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using System.Xml.Linq;

namespace MudSharp.Effects.Concrete.SpellEffects;

public class SpellRoomGravityEffect : MagicSpellEffectBase, IDescriptionAdditionEffect, IGravityOverrideEffect
{
	public static void InitialiseEffectType()
	{
		RegisterFactory("SpellRoomGravity", (effect, owner) => new SpellRoomGravityEffect(effect, owner));
	}

	public SpellRoomGravityEffect(IPerceivable owner, IMagicSpellEffectParent parent, IFutureProg prog, GravityModel gravityModel,
		string descAddendum, ANSIColour colour) : base(owner, parent, prog)
	{
		GravityModel = gravityModel;
		DescAddendum = descAddendum;
		AddendumColour = colour;
	}

	protected SpellRoomGravityEffect(XElement root, IPerceivable owner) : base(root, owner)
	{
		var tr = root.Element("Effect");
		GravityModel = (GravityModel)int.Parse(tr!.Element("GravityModel")!.Value);
		DescAddendum = tr.Element("DescAddendum")?.Value ?? string.Empty;
		AddendumColour = Telnet.GetColour(tr.Element("AddendumColour")?.Value ?? "cyan");
	}

	protected override XElement SaveDefinition()
	{
		return new XElement("Effect",
			new XElement("ApplicabilityProg", ApplicabilityProg?.Id ?? 0),
			new XElement("GravityModel", (int)GravityModel),
			new XElement("DescAddendum", new XCData(DescAddendum)),
			new XElement("AddendumColour", AddendumColour.Name)
		);
	}

	protected override string SpecificEffectType => "SpellRoomGravity";

	public GravityModel GravityModel { get; }

	public int Priority => 50;

	public string DescAddendum { get; set; }

	public ANSIColour AddendumColour { get; set; }

	public override string Describe(IPerceiver voyeur)
	{
		return $"Room Gravity - {GravityModel.DescribeColour()}";
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
		base.RemovalEffect();
		if (Owner is ICell cell)
		{
			cell.CheckFallExitStatus();
		}
	}

	public string GetAdditionalText(IPerceiver voyeur, bool colour)
	{
		if (string.IsNullOrEmpty(DescAddendum))
		{
			return string.Empty;
		}

		return colour ? DescAddendum.Colour(AddendumColour) : DescAddendum;
	}

	public bool PlayerSet => false;
}
