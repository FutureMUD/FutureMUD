using ExpressionEngine;
using MudSharp.RPG.Checks;

#nullable enable

namespace MudSharp.GameItems;

public class ConditionMaintenanceProfile
{
	public const string DefaultQualityPenaltyExpression =
		"if(condition>=0.20,0,if(condition>=0.16,-1,if(condition>=0.12,-2,if(condition>=0.08,-3,if(condition>=0.04,-4,-5)))))";

	public const string DefaultMeleeUseExpression = "0.00025";
	public const string DefaultRangedUseExpression = "0.0005";
	public const string DefaultShieldBlockUseExpression = "0.0005";
	public const string DefaultArmourUseExpression = "0.00025";
	public const string DefaultMeasurementUseExpression = "0.0001";

	public static readonly string DefaultRangedOrMeleeUseExpression =
		$"if(usekind=={(int)ItemConditionUseKind.RangedFire},{DefaultRangedUseExpression},{DefaultMeleeUseExpression})";

	public static readonly string DefaultShieldOrMeleeUseExpression =
		$"if(usekind=={(int)ItemConditionUseKind.ShieldBlock},{DefaultShieldBlockUseExpression},{DefaultMeleeUseExpression})";

	public ConditionMaintenanceProfile(string defaultUseExpression)
	{
		DefaultUseExpression = defaultUseExpression;
		ConditionUseExpression = new Expression(defaultUseExpression);
		ConditionQualityPenaltyExpression = new Expression(DefaultQualityPenaltyExpression);
	}

	public string DefaultUseExpression { get; }
	public bool ConditionDegradesOnUse { get; private set; }
	public Expression ConditionUseExpression { get; private set; }
	public Expression ConditionQualityPenaltyExpression { get; private set; }

	public void LoadFromXml(XElement root)
	{
		ConditionDegradesOnUse = bool.Parse(root.Element("ConditionDegradesOnUse")?.Value ?? "false");
		ConditionUseExpression =
			new Expression(root.Element("ConditionUseFormula")?.Value ?? DefaultUseExpression);
		ConditionQualityPenaltyExpression =
			new Expression(root.Element("ConditionQualityFormula")?.Value ?? DefaultQualityPenaltyExpression);
	}

	public IEnumerable<XElement> SaveToXml()
	{
		yield return new XElement("ConditionDegradesOnUse", ConditionDegradesOnUse);
		yield return new XElement("ConditionUseFormula", new XCData(ConditionUseExpression.OriginalExpression));
		yield return new XElement("ConditionQualityFormula",
			new XCData(ConditionQualityPenaltyExpression.OriginalExpression));
	}

	public void ResetToDefaults()
	{
		ConditionDegradesOnUse = false;
		ConditionUseExpression = new Expression(DefaultUseExpression);
		ConditionQualityPenaltyExpression = new Expression(DefaultQualityPenaltyExpression);
	}

	public int QualityPenaltyStages(IGameItem parent)
	{
		if (!ConditionDegradesOnUse)
		{
			return 0;
		}

		var result = ConditionQualityPenaltyExpression.EvaluateDoubleWith(GetParameters(parent,
			new ItemConditionUseContext(ItemConditionUseKind.MeleeAttack)));
		if (double.IsNaN(result) || double.IsInfinity(result))
		{
			return 0;
		}

		return Math.Min(0, (int)Math.Round(result, MidpointRounding.AwayFromZero));
	}

	public void UseCondition(IGameItem parent, ItemConditionUseContext context)
	{
		if (!ConditionDegradesOnUse)
		{
			return;
		}

		var loss = ConditionUseExpression.EvaluateDoubleWith(GetParameters(parent, context));
		if (double.IsNaN(loss) || double.IsInfinity(loss) || loss <= 0.0)
		{
			return;
		}

		parent.Condition = Math.Clamp(parent.Condition - loss, 0.0, 1.0);
	}

	public string Describe(ICharacter actor)
	{
		return $@"Condition Degrades: {ConditionDegradesOnUse.ToColouredString()}
Condition Use Formula: {ConditionUseExpression.OriginalExpression.ColourCommand()}
Condition Quality Formula: {ConditionQualityPenaltyExpression.OriginalExpression.ColourCommand()}";
	}

	public bool BuildingCommand(ICharacter actor, StringStack command, Action changed)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(ConditionBuildingHelp.SubstituteANSIColour());
			return false;
		}

		switch (command.PopForSwitch())
		{
			case "on":
			case "yes":
			case "true":
				ConditionDegradesOnUse = true;
				changed();
				actor.OutputHandler.Send("This component will now degrade its parent item's condition when used.");
				return true;
			case "off":
			case "no":
			case "false":
				ConditionDegradesOnUse = false;
				changed();
				actor.OutputHandler.Send("This component will no longer degrade its parent item's condition when used.");
				return true;
			case "use":
			case "usage":
			case "loss":
				return BuildingCommandUseFormula(actor, command, changed);
			case "quality":
			case "penalty":
				return BuildingCommandQualityFormula(actor, command, changed);
			case "defaults":
			case "default":
			case "reset":
				ResetToDefaults();
				changed();
				actor.OutputHandler.Send("This component's condition maintenance settings have been reset to their defaults.");
				return true;
			case "show":
				actor.OutputHandler.Send(Describe(actor));
				return false;
			default:
				actor.OutputHandler.Send(ConditionBuildingHelp.SubstituteANSIColour());
				return false;
		}
	}

	public const string ConditionBuildingHelp =
		@"Use #3condition on|off#0 to toggle condition degradation.
Use #3condition use <formula>#0 to set the condition loss per use.
Use #3condition quality <formula>#0 to set the quality stage penalty.
Use #3condition defaults#0 to reset the maintenance formulas.

Formula variables: condition, rawquality, basequality, usekind, outcome, degree, damage, absorbed, passed.";

	private bool BuildingCommandUseFormula(ICharacter actor, StringStack command, Action changed)
	{
		if (!TryParseExpression(actor, command.SafeRemainingArgument, out var expression))
		{
			return false;
		}

		ConditionUseExpression = expression!;
		changed();
		actor.OutputHandler.Send(
			$"This component's condition use formula is now {ConditionUseExpression.OriginalExpression.ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandQualityFormula(ICharacter actor, StringStack command, Action changed)
	{
		if (!TryParseExpression(actor, command.SafeRemainingArgument, out var expression))
		{
			return false;
		}

		ConditionQualityPenaltyExpression = expression!;
		changed();
		actor.OutputHandler.Send(
			$"This component's condition quality penalty formula is now {ConditionQualityPenaltyExpression.OriginalExpression.ColourCommand()}.");
		return true;
	}

	private static bool TryParseExpression(ICharacter actor, string text, out Expression? expression)
	{
		expression = null;
		if (string.IsNullOrWhiteSpace(text))
		{
			actor.OutputHandler.Send("You must specify an expression.");
			return false;
		}

		var candidate = new Expression(text);
		if (candidate.HasErrors())
		{
			actor.OutputHandler.Send(candidate.Error.ColourError());
			return false;
		}

		try
		{
			_ = candidate.EvaluateDoubleWith(SampleParameters);
		}
		catch (Exception ex)
		{
			actor.OutputHandler.Send($"There was an error evaluating that expression: {ex.Message.ColourError()}");
			return false;
		}

		expression = candidate;
		return true;
	}

	private static (string Name, object Value)[] SampleParameters =>
	[
		("condition", 0.5),
		("rawquality", (int)ItemQuality.Standard),
		("basequality", (int)ItemQuality.Standard),
		("usekind", (int)ItemConditionUseKind.MeleeAttack),
		("outcome", (int)Outcome.Pass),
		("degree", 1.0),
		("damage", 0.0),
		("absorbed", 0.0),
		("passed", 0.0)
	];

	private static (string Name, object Value)[] GetParameters(IGameItem parent, ItemConditionUseContext context)
	{
		return
		[
			("condition", parent.Condition),
			("rawquality", (int)parent.RawQuality),
			("basequality", (int)(parent.Skin?.Quality ?? parent.RawQuality)),
			("usekind", (int)context.UseKind),
			("outcome", (int)context.Outcome),
			("degree", context.Degree),
			("damage", context.Damage),
			("absorbed", context.Absorbed),
			("passed", context.Passed)
		];
	}
}
