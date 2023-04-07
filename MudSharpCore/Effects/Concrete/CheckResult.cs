using System.Xml.Linq;
using MudSharp.Body.Traits;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.RPG.Checks;

namespace MudSharp.Effects.Concrete;

public class CheckResult : Effect, ICheckResultEffect
{
	public CheckResult(IPerceivable owner, CheckType check, Difficulty difficulty, Outcome outcome,
		ITraitDefinition trait = null, IFrameworkItem target = null, IFrameworkItem tool = null)
		: base(owner)
	{
		Check = check;
		Difficulty = difficulty;
		Outcome = outcome;
		Trait = trait;
		TargetID = target?.Id;
		TargetType = target != null ? target.FrameworkItemType : "None";
		ToolID = tool?.Id;
		ToolType = tool != null ? tool.FrameworkItemType : "None";
	}

	public CheckResult(XElement effect, IPerceivable owner)
		: base(effect, owner)
	{
		var definition = effect.Element("Effect");
		Check = (CheckType)int.Parse(definition.Attribute("Check").Value);
		Difficulty = (Difficulty)int.Parse(definition.Attribute("Difficulty").Value);
		Outcome = (Outcome)int.Parse(definition.Attribute("Outcome").Value);
		Trait = owner.Gameworld.Traits.Get(long.Parse(definition.Attribute("Trait").Value));
		TargetID = long.Parse(definition.Attribute("TargetId").Value);
		ToolID = long.Parse(definition.Attribute("ToolId").Value);
		TargetType = definition.Attribute("TargetType").Value;
		ToolType = definition.Attribute("ToolType").Value;
	}

	protected override string SpecificEffectType => "CheckResult";

	public CheckType Check { get; protected set; }
	public Difficulty Difficulty { get; protected set; }
	public long? TargetID { get; protected set; }
	public string TargetType { get; protected set; }
	public long? ToolID { get; protected set; }
	public string ToolType { get; protected set; }
	public Outcome Outcome { get; protected set; }
	public ITraitDefinition Trait { get; protected set; }

	public bool SameCheck(CheckType type, Difficulty difficulty, IFrameworkItem target, ITraitDefinition trait,
		IFrameworkItem tool)
	{
		return
			Check == type &&
			Difficulty == difficulty &&
			target.FrameworkItemEquals(TargetID, TargetType) &&
			tool.FrameworkItemEquals(TargetID, TargetType) &&
			Trait == trait
			;
	}

	public bool SameResult(ICheckResultEffect other)
	{
		return
			Check == other.Check &&
			Difficulty == other.Difficulty &&
			Trait == other.Trait &&
			TargetID == other.TargetID &&
			TargetType == other.TargetType &&
			ToolID == other.ToolID &&
			ToolType == other.ToolType;
	}

	public override void ExpireEffect()
	{
		Owner.RemoveEffect(this);
	}

	public override string Describe(IPerceiver voyeur)
	{
		return string.Format("Fixed Check Result of {0} for {1} ({5}){2}{3}{4}.",
			Outcome.Describe(),
			Check,
			TargetID.HasValue ? string.Format(voyeur, " +[{0} #{1:N0}]", TargetType, TargetID) : "",
			ToolID.HasValue ? string.Format(voyeur, " +[{0} #{1:N0}]", ToolType, ToolID) : "",
			Trait != null ? string.Format(voyeur, " +[{0} #{1:N0}]", Trait.Name, Trait.Id) : "",
			Difficulty.Describe()
		);
	}

	public override bool SavingEffect => true;

	public static void InitialiseEffectType()
	{
		RegisterFactory("CheckResult", (effect, owner) => new CheckResult(effect, owner));
	}

	public override string ToString()
	{
		return string.Format("Fixed Check Result of {0} for {1} ({5}){2}{3}{4}.",
			Outcome.Describe(),
			Check,
			TargetID.HasValue ? $" +[{TargetType} #{TargetID:N0}]" : "",
			ToolID.HasValue ? $" +[{ToolType} #{ToolID:N0}]" : "",
			Trait != null ? $" +[{Trait.Name} #{Trait.Id:N0}]" : "",
			Difficulty.Describe()
		);
	}

	protected override XElement SaveDefinition()
	{
		return
			new XElement("Effect", new XAttribute("Outcome", (int)Outcome), new XAttribute("Check", (int)Outcome),
				new XAttribute("Difficulty", (int)Difficulty),
				new XAttribute("Trait", Trait?.Id ?? 0), new XAttribute("TargetId", TargetID ?? 0),
				new XAttribute("TargetType", TargetType), new XAttribute("ToolId", ToolID ?? 0),
				new XAttribute("ToolType", TargetType));
	}
}