using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.RPG.Checks;
using System.Xml.Linq;

namespace MudSharp.Effects.Concrete;

public class GeneralActiveCheckBonus : Effect, ICheckBonusEffect
{
	public GeneralActiveCheckBonus(XElement effect, IPerceivable owner) : base(effect, owner)
	{
		CheckBonus = double.Parse(effect.Element("Bonus").Value);
	}

	public GeneralActiveCheckBonus(IPerceiver owner, double bonus, IFutureProg prog) : base(owner, prog)
	{
		CheckBonus = bonus;
	}

	protected override XElement SaveDefinition()
	{
		return new XElement("Bonus", CheckBonus);
	}

	public override bool SavingEffect => true;

	protected override string SpecificEffectType => "GeneralActiveCheckBonus";

	public bool AppliesToCheck(CheckType type)
	{
		return type.IsGeneralActivityCheck();
	}

	public double CheckBonus { get; set; }

	public override string Describe(IPerceiver voyeur)
	{
		return $"Receiving a bonus of {CheckBonus} to all general active checks.";
	}
}