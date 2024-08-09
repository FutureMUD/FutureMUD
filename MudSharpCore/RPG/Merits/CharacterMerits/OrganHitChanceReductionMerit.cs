using MudSharp.Body;
using MudSharp.Framework;
using MudSharp.Health;
using MudSharp.RPG.Merits.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Body.PartProtos;
using MudSharp.Character;
using NCalc;
using Expression = ExpressionEngine.Expression;

namespace MudSharp.RPG.Merits.CharacterMerits;

public class OrganHitChanceReductionMerit : CharacterMeritBase, IOrganHitReductionMerit
{
	public static void RegisterMeritInitialiser()
	{
		MeritFactory.RegisterMeritInitialiser("Organ Hit Reduction",
			(merit, gameworld) => new OrganHitChanceReductionMerit(merit, gameworld));
		MeritFactory.RegisterBuilderMeritInitialiser("Organ Hit Reduction", (gameworld, name) => new OrganHitChanceReductionMerit(gameworld, name));
		MeritFactory.RegisterMeritHelp("Organ Hit Reduction", "Alters the chance for organs to be hit by attacks", new OrganHitChanceReductionMerit().HelpText);
	}

	protected OrganHitChanceReductionMerit(Models.Merit merit, IFuturemud gameworld) : base(merit, gameworld)
	{
		var root = XElement.Parse(merit.Definition);
		MinimumWoundSeverity =
			(WoundSeverity)int.Parse(root.Attribute("minseverity")?.Value ?? ((int)WoundSeverity.None).ToString());
		MaximumWoundSeverity =
			(WoundSeverity)int.Parse(root.Attribute("maxseverity")?.Value ??
			                         ((int)WoundSeverity.Horrifying).ToString());
		HitChanceExpression = new Expression(root.Element("Chance")?.Value ?? "0.0");
		foreach (var item in root.Element("Organs")?.Elements("Organ") ?? Enumerable.Empty<XElement>())
		{
			_organs.Add((BodypartTypeEnum)int.Parse(item.Value));
		}
	}

	protected OrganHitChanceReductionMerit(){}

	protected OrganHitChanceReductionMerit(IFuturemud gameworld, string name) : base(gameworld, name, "Organ Hit Reduction", "@ have|has an altered chance for &0's organs to be hit")
	{
		MinimumWoundSeverity = WoundSeverity.None;
		MaximumWoundSeverity = WoundSeverity.Horrifying;
		HitChanceExpression = new Expression("chance*0.75");
		DoDatabaseInsert();
	}

	/// <inheritdoc />
	protected override XElement SaveSubtypeDefinition(XElement root)
	{
		root.Add(new XAttribute("minseverity", (int)MinimumWoundSeverity));
		root.Add(new XAttribute("maxseverity", (int)MinimumWoundSeverity));
		root.Add(new XElement("Chance", new XCData(HitChanceExpression.OriginalExpression)));
		root.Add(new XElement("Organs",
			from item in _organs
			select new XElement("Organ", (int)item)
		));
		return root;
	}

	/// <inheritdoc />
	protected override void SubtypeShow(ICharacter actor, StringBuilder sb)
	{
		sb.AppendLine($"Minimum Severity: {MinimumWoundSeverity.DescribeEnum().ColourValue()}");
		sb.AppendLine($"Maximum Severity: {MaximumWoundSeverity.DescribeEnum().ColourValue()}");
		sb.AppendLine($"Hit Chance Formula: {HitChanceExpression.OriginalExpression.ColourCommand()}");
		sb.AppendLine($"Organs: {(_organs.Count == 0 ? "All".ColourValue() : _organs.Select(x => x.DescribeEnum().ColourValue()).ListToString())}");
	}

	/// <inheritdoc />
	protected override string SubtypeHelp => @$"{base.SubtypeHelp}
	#3min <severity>#0 - sets the minimum severity at which this applies
	#3max <severity>#0 - sets the maximum severity at which this applies
	#3chance <formula>#0 - sets the formula for chance. Base chance is variable #6chance#0
	#3organ <type>#0 - toggles an organ type being affected
	#3organ all#0 - resets this to applying to all organs";

	/// <inheritdoc />
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "min":
				return BuildingCommandMin(actor, command);
			case "max":
				return BuildingCommandMax(actor, command);
			case "chance":
				return BuildingCommandChance(actor, command);
			case "organ":
				return BuildingCommandOrgan(actor, command);
		}
		return base.BuildingCommand(actor, command.GetUndo());
	}

	private bool BuildingCommandOrgan(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must either specify an organ type or specify #3all#0 to remove all specific organs.".SubstituteANSIColour());
			return false;
		}

		if (command.PeekSpeech().EqualTo("all"))
		{
			_organs.Clear();
			Changed = true;
			actor.OutputHandler.Send($"This merit will now apply to all organs.");
			return true;
		}

		if (!command.SafeRemainingArgument.TryParseEnum<BodypartTypeEnum>(out var value) || !value.IsOrgan())
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid organ type. The valid types are {Enum.GetValues<BodypartTypeEnum>().Where(x => x.IsOrgan()).Select(x => x.DescribeEnum().ColourValue()).ListToString()}.");
			return false;
		}

		Changed = true;
		if (_organs.Remove(value))
		{
			actor.OutputHandler.Send($"This merit will no longer apply to organs of type {value.DescribeEnum().ColourValue()}.");
			return true;
		}

		_organs.Add(value);
		actor.OutputHandler.Send($"This merit will now apply to organs of type {value.DescribeEnum().ColourValue()}.");
		return true;
	}

	private bool BuildingCommandChance(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What should be the formula for the chance for organs to be hit?");
			return false;
		}

		var formula = new Expression(command.SafeRemainingArgument);
		if (formula.HasErrors())
		{
			actor.OutputHandler.Send(formula.Error);
			return false;
		}

		Changed = true;
		HitChanceExpression = formula;
		actor.OutputHandler.Send($"The formula for organ hit chance is now {command.SafeRemainingArgument.ColourCommand()}");
		return true;
	}

	private bool BuildingCommandMax(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"Which wound severity should be the maximum at which this merit applies?\nValid options are {Enum.GetValues<WoundSeverity>().Select(x => x.DescribeEnum().ColourValue()).ListToString()}.");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParseEnum(out WoundSeverity severity))
		{
			actor.OutputHandler.Send($"That is not a valid severity.\nValid options are {Enum.GetValues<WoundSeverity>().Select(x => x.DescribeEnum().ColourValue()).ListToString()}.");
			return false;
		}

		MaximumWoundSeverity = severity;
		Changed = true;
		actor.OutputHandler.Send($"This merit now applies only when the wound is at most a severity of {severity.DescribeEnum().ColourValue()}.");
		return true;
	}

	private bool BuildingCommandMin(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"Which wound severity should be the minimum at which this merit applies?\nValid options are {Enum.GetValues<WoundSeverity>().Select(x => x.DescribeEnum().ColourValue()).ListToString()}.");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParseEnum(out WoundSeverity severity))
		{
			actor.OutputHandler.Send($"That is not a valid severity.\nValid options are {Enum.GetValues<WoundSeverity>().Select(x => x.DescribeEnum().ColourValue()).ListToString()}.");
			return false;
		}

		MinimumWoundSeverity = severity;
		Changed = true;
		actor.OutputHandler.Send($"This merit now applies only when the wound is at least a severity of {severity.DescribeEnum().ColourValue()}.");
		return true;
	}

	public WoundSeverity MinimumWoundSeverity { get; set; }
	public WoundSeverity MaximumWoundSeverity { get; set; }
	public Expression HitChanceExpression { get; set; }
	private readonly List<BodypartTypeEnum> _organs = new();

	public bool MissesOrgan(KeyValuePair<IOrganProto, BodypartInternalInfo> organInfo, IDamage damage,
		WoundSeverity severity)
	{
		if (_organs.Count > 0 && !_organs.Contains(organInfo.Key.BodypartType))
		{
			return false;
		}

		if (severity < MinimumWoundSeverity)
		{
			return false;
		}

		if (severity > MaximumWoundSeverity)
		{
			return false;
		}

		HitChanceExpression.Parameters["chance"] = organInfo.Value.HitChance;
		if (RandomUtilities.Random(0, 100) <= (double)HitChanceExpression.Evaluate())
		{
			return true;
		}

		return false;
	}
}