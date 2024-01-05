using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using MudSharp.Body.Traits;
using MudSharp.Body.Traits.Subtypes;
using MudSharp.Character;
using MudSharp.CharacterCreation.Resources;
using MudSharp.Editor;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Models;
using MudSharp.PerceptionEngine;
using NCalc;
using Expression = ExpressionEngine.Expression;

namespace MudSharp.CharacterCreation.Screens;

public class AttributePointBuyScreenStoryboard : ChargenScreenStoryboard
{
	private AttributePointBuyScreenStoryboard()
	{
	}

	protected AttributePointBuyScreenStoryboard(IFuturemud gameworld, Models.ChargenScreenStoryboard dbitem)
		: base(dbitem, gameworld)
	{
		var definition = XElement.Parse(dbitem.StageDefinition);
		Blurb = definition.Element("Blurb").Value;
		MaximumBoostsProg = long.TryParse(definition.Element("MaximumBoostsProg").Value, out var value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.GetByName(definition.Element("MaximumBoostsProg").Value);
		MaximumFreeBoostsProg = long.TryParse(definition.Element("MaximumFreeBoostsProg").Value, out value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.GetByName(definition.Element("MaximumFreeBoostsProg").Value);
		MaximumMinusesProg = long.TryParse(definition.Element("MaximumMinusesProg").Value, out value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.GetByName(definition.Element("MaximumMinusesProg").Value);
		FreeBoostsProg = long.TryParse(definition.Element("FreeBoostsProg").Value, out value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.GetByName(definition.Element("FreeBoostsProg").Value);
		AttributeBaseValueProg = long.TryParse(definition.Element("AttributeBaseValueProg").Value, out value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.GetByName(definition.Element("AttributeBaseValueProg").Value);
		BoostCostExpression = new Expression(definition.Element("BoostCostExpression").Value);
		BoostResource = Gameworld.ChargenResources.Get(long.Parse(definition.Element("BoostResource").Value));
		MaximumExtraBoosts = int.Parse(definition.Element("MaximumExtraBoosts").Value);
	}

	protected AttributePointBuyScreenStoryboard(IFuturemud gameworld, IChargenScreenStoryboard storyboard) : base(
		gameworld, storyboard)
	{
		BoostResource = Gameworld.ChargenResources.FirstOrDefault();
		MaximumExtraBoosts = 3;
		BoostCostExpression = new Expression("pow(2, max(0,boosts-1)) * 100", EvaluateOptions.IgnoreCase);
		switch (storyboard)
		{
			case AttributeOrdererScreenStoryboard aos:
				Blurb = aos.Blurb;
				break;
		}

		SaveAfterTypeChange();
	}

	protected override string StoryboardName => "AttributePointBuy";

	public string Blurb { get; protected set; }

	public IFutureProg MaximumBoostsProg { get; protected set; }
	public IFutureProg MaximumFreeBoostsProg { get; protected set; }

	public IFutureProg MaximumMinusesProg { get; protected set; }

	public IFutureProg FreeBoostsProg { get; protected set; }

	public IFutureProg AttributeBaseValueProg { get; protected set; }
	public Expression BoostCostExpression { get; protected set; }

	public IChargenResource BoostResource { get; protected set; }
	public int MaximumExtraBoosts { get; protected set; }

	public override ChargenStage Stage => ChargenStage.SelectAttributes;

	#region Overrides of ChargenScreenStoryboard

	/// <inheritdoc />
	protected override string SaveDefinition()
	{
		return new XElement("Definition",
			new XElement("Blurb", new XCData(Blurb)),
			new XElement("MaximumBoostsProg", MaximumBoostsProg?.Id ?? 0),
			new XElement("MaximumFreeBoostsProg", MaximumFreeBoostsProg?.Id ?? 0),
			new XElement("MaximumMinusesProg", MaximumMinusesProg?.Id ?? 0),
			new XElement("MaximumExtraBoosts", MaximumExtraBoosts),
			new XElement("FreeBoostsProg", FreeBoostsProg?.Id ?? 0),
			new XElement("AttributeBaseValueProg", AttributeBaseValueProg?.Id ?? 0),
			new XElement("BoostCostExpression", new XCData(BoostCostExpression.OriginalExpression)),
			new XElement("BoostResource", BoostResource?.Id ?? 0)
		).ToString();
	}

	#endregion

	public override string Show(ICharacter voyeur)
	{
		var sb = new StringBuilder();
		sb.Append(ShowHeader(voyeur));
		sb.AppendLine();
		sb.AppendLine(
			"This screen starts all attributes off at a base value and then lets the player spend a number of points to boost them to the level that they desire. There is no randomisation, it's all based on a point-buy."
				.Wrap(voyeur.InnerLineFormatLength).ColourCommand());
		sb.AppendLine();
		sb.AppendLine($"Attribute Base Value: {AttributeBaseValueProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"# Free Boosts: {FreeBoostsProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Maximum Boosts: {MaximumBoostsProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Maximum Free Boosts: {MaximumFreeBoostsProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Maximum Minuses: {MaximumMinusesProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Maximum Extra Boosts: {MaximumExtraBoosts.ToString("N0", voyeur).ColourValue()}");
		sb.AppendLine($"Boost Resource: {BoostResource.PluralName.ColourValue()}");
		sb.AppendLine($"Boost Cost Expression: {BoostCostExpression.OriginalExpression.ColourName()}");

		sb.AppendLine();
		sb.AppendLine("Blurb".GetLineWithTitle(voyeur, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine(Blurb.Wrap(voyeur.InnerLineFormatLength).SubstituteANSIColour());
		return sb.ToString();
	}

	public static void RegisterFactory()
	{
		ChargenStoryboard.RegisterFactory(ChargenStage.SelectAttributes,
			new ChargenScreenStoryboardFactory("AttributePointBuy",
				(game, dbitem) => new AttributePointBuyScreenStoryboard(game, dbitem),
				(game, other) => new AttributePointBuyScreenStoryboard(game, other)),
			"AttributePointBuy",
			"Assign points to attributes, no randomisation",
			((ChargenScreenStoryboard)Activator.CreateInstance(MethodBase.GetCurrentMethod().DeclaringType, true))
			.HelpText);
	}

	public override IChargenScreen GetScreen(IChargen chargen)
	{
		return new AttributePointBuyScreen(chargen, this);
	}

	public override IEnumerable<(IChargenResource Resource, int Cost)> ChargenCosts(IChargen chargen)
	{
		var (nonFreeBoosts, _) = EvaluateBoosts(chargen);
		if (nonFreeBoosts == 0)
		{
			return Enumerable.Empty<(IChargenResource, int)>();
		}

		BoostCostExpression.Parameters["boosts"] = nonFreeBoosts;
		return new[] { (BoostResource, (int)(double)BoostCostExpression.Evaluate()) };
	}

	public (int NonFreeBoosts, int RemainingFreeBoosts) EvaluateBoosts(IChargen chargen)
	{
		var totalNonFreeBoosts = 0;
		var remainingFreeBoosts = (int)FreeBoostsProg.ExecuteDouble(chargen);
		foreach (var attribute in chargen.SelectedAttributes)
		{
			var boosts =
				(int)Math.Round(
					attribute.RawValue - AttributeBaseValueProg.ExecuteDouble(chargen, attribute.Definition) -
					chargen.SelectedRace.AttributeBonusProg.ExecuteDouble(attribute.Definition), 0);
			var nonFreeBoosts = Math.Max(0,
				boosts - (int)(MaximumFreeBoostsProg?.ExecuteDouble(chargen, attribute.Definition) ?? 0.0));
			totalNonFreeBoosts += nonFreeBoosts;
			remainingFreeBoosts -= boosts - nonFreeBoosts;
		}

		if (remainingFreeBoosts < 0)
		{
			totalNonFreeBoosts += remainingFreeBoosts.Abs();
			remainingFreeBoosts = 0;
		}

		return (totalNonFreeBoosts, remainingFreeBoosts);
	}

	internal class AttributePointBuyScreen : ChargenScreen
	{
		private readonly Dictionary<IAttributeDefinition, double> _baseValues = new();
		private readonly Dictionary<IAttributeDefinition, int> _maximumBoosts = new();

		private readonly Dictionary<IAttributeDefinition, int> _maximumFreeBoosts = new();
		private readonly Dictionary<IAttributeDefinition, int> _maximumMinuses = new();
		private readonly Counter<IAttributeDefinition> _numberOfBoosts = new();
		private int _totalFreeBoosts;
		protected AttributePointBuyScreenStoryboard Storyboard;

		internal AttributePointBuyScreen(IChargen chargen, AttributePointBuyScreenStoryboard storyboard) : base(chargen,
			storyboard)
		{
			Storyboard = storyboard;
			Reset();
		}

		public override ChargenStage AssociatedStage => ChargenStage.SelectAttributes;

		public void Reset()
		{
			foreach (var attribute in Chargen.SelectedRace.Attributes.Where(x => x.TraitType == TraitType.Attribute))
			{
				_maximumFreeBoosts[attribute] = (int)Storyboard.MaximumFreeBoostsProg.ExecuteDouble(Chargen, attribute);
				_maximumBoosts[attribute] = (int)Storyboard.MaximumBoostsProg.ExecuteDouble(Chargen, attribute);
				_maximumMinuses[attribute] = -1 * (int)Storyboard.MaximumMinusesProg.ExecuteDouble(Chargen, attribute);
				_baseValues[attribute] = Storyboard.AttributeBaseValueProg.ExecuteDouble(Chargen, attribute);
				_numberOfBoosts[attribute] = 0;
			}

			_totalFreeBoosts = (int)Storyboard.FreeBoostsProg.ExecuteDouble(Chargen);
			UpdateChargenSelectedAttributes();
		}

		public void UpdateChargenSelectedAttributes()
		{
			var selectedAttributes = new List<ITrait>();
			foreach (var attribute in _baseValues.Keys)
			{
				selectedAttributes.Add(TraitFactory.LoadAttribute(attribute, null,
					_baseValues[attribute] + _numberOfBoosts[attribute] +
					Chargen.SelectedRace.AttributeBonusProg.ExecuteDouble(attribute)));
			}

			Chargen.SelectedAttributes = selectedAttributes;
		}

		public override string Display()
		{
			if (!ShownChargenAdvice && HasChargenAdvice())
			{
				return DisplayChargenAdvice();
			}

			var sb = new StringBuilder();
			sb.AppendLine("Attribute Selection".Colour(Telnet.Cyan));
			sb.AppendLine();
			sb.AppendLine(Storyboard.Blurb.Wrap(Account.InnerLineFormatLength));
			sb.AppendLine();
			foreach (var attribute in _baseValues.Keys.OrderBy(x => x.Name))
			{
				var current = _baseValues[attribute] + _numberOfBoosts[attribute] +
				              Chargen.SelectedRace.AttributeBonusProg.ExecuteDouble(attribute);
				var boostString = "";
				if (_numberOfBoosts[attribute] > 0)
				{
					boostString = new string('+', _numberOfBoosts[attribute]);
				}
				else if (_numberOfBoosts[attribute] < 0)
				{
					boostString = new string('-', _numberOfBoosts[attribute] * -1);
				}

				sb.AppendLine(
					$"\t{$"{attribute.Name.TitleCase()} [{boostString}]",-35} {attribute.Decorator.Decorate(current),-20} {$"Max +{_maximumFreeBoosts[attribute].ToString("N0", Chargen.Account)}({_maximumBoosts[attribute].ToString("N0", Chargen.Account)})/{_maximumMinuses[attribute].ToString("N0", Chargen.Account)}",-30}");
			}

			var (boosts, remaining) = Storyboard.EvaluateBoosts(Chargen);
			var maxBoosts = Storyboard.MaximumBoostsProg.ExecuteInt(Chargen);

			sb.AppendLine();
			sb.AppendLine(
				$"You have {remaining.ToString("N0", Chargen.Account).ColourValue()}/{_totalFreeBoosts.ToString("N0", Chargen.Account).ColourValue()} free boosts remaining.");
			if (boosts > 0)
			{
				var costs = Storyboard.ChargenCosts(Chargen);
				sb.AppendLine(
					$"You have made {boosts.ToString("N0", Chargen.Account).ColourValue()} extra boosts costing {costs.Select(x => $"{x.Cost.ToString("N0", Chargen.Account)} {x.Resource.Alias}".ColourValue()).First()}.");
			}

			sb.AppendLine();
			sb.AppendLine(
				$"Type the name of an attribute to apply a boost, prefixed by - to remove a boost, e.g. {Chargen.SelectedRace.Attributes.First().Name.ToLowerInvariant().Colour(Telnet.Green)} / -{Chargen.SelectedRace.Attributes.First().Name.ToLowerInvariant().Colour(Telnet.Red)}");
			sb.AppendLine(
				$"Type {"reset".ColourCommand()} to reset all choices, or {"done".ColourCommand()} when you are finished.");
			sb.AppendLine(
				$"You can also type {"help <attribute>".ColourCommand()} to see a description of each attribute.");
			return sb.ToString();
		}

		public override string HandleCommand(string command)
		{
			if (string.IsNullOrWhiteSpace(command))
			{
				return Display();
			}

			if (!ShownChargenAdvice && HasChargenAdvice())
			{
				return HandleCommandChargenAdvice(command);
			}

			if (command.Equals("help", StringComparison.InvariantCultureIgnoreCase) ||
			    command.StartsWith("help ", StringComparison.InvariantCultureIgnoreCase))
			{
				var ss = new StringStack(command.RemoveFirstWord());
				if (ss.IsFinished)
				{
					return "What attribute do you want to view the helpfile for?";
				}

				var argument = ss.SafeRemainingArgument;
				var attribute =
					Chargen.SelectedRace.Attributes.FirstOrDefault(
						x => x.Alias.Equals(argument, StringComparison.InvariantCultureIgnoreCase)) ??
					Chargen.SelectedRace.Attributes.FirstOrDefault(
						x => x.Name.StartsWith(argument, StringComparison.InvariantCultureIgnoreCase));
				;
				if (attribute == null)
				{
					return $"There is no such attribute as '{argument}' to view the helpfile for.";
				}

				var helpfile =
					Chargen.Gameworld.Helpfiles.FirstOrDefault(
						x => x.Name.Equals(attribute.Name, StringComparison.InvariantCultureIgnoreCase));
				if (helpfile == null)
				{
					return attribute.ChargenBlurb;
				}

				return helpfile.DisplayHelpFile(Chargen);
			}

			if (command.EqualTo("reset"))
			{
				Reset();
				return Display();
			}

			if (command.EqualTo("done"))
			{
				var (_, remaining) = Storyboard.EvaluateBoosts(Chargen);
				if (remaining > 0)
				{
					return
						$"You still have {remaining.ToString("N0", Chargen.Account).ColourValue()} free boosts that you haven't spent. If you still want to continue, type {"doneforce".ColourCommand()}.";
				}

				UpdateChargenSelectedAttributes();
				State = ChargenScreenState.Complete;
				return "\n";
			}

			if (command.EqualTo("doneforce"))
			{
				State = ChargenScreenState.Complete;
				return "\n";
			}

			var remove = false;
			if (command[0] == '-')
			{
				remove = true;
				command = command.Substring(1);
			}

			var which = _baseValues.Keys.FirstOrDefault(
				            x => x.Alias.Equals(command, StringComparison.InvariantCultureIgnoreCase)) ??
			            _baseValues.Keys.FirstOrDefault(
				            x => x.Name.StartsWith(command, StringComparison.InvariantCultureIgnoreCase));
			if (which == null)
			{
				return "That is not a valid attribute selection.";
			}

			if (remove)
			{
				if (_numberOfBoosts[which] <= _maximumMinuses[which])
				{
					return $"Your {which.Name.ColourValue()} attribute is already at its minimum value.";
				}

				_numberOfBoosts[which] -= 1;
				UpdateChargenSelectedAttributes();
				return Display();
			}

			if (_numberOfBoosts[which] >= _maximumBoosts[which])
			{
				return $"Your {which.Name.ColourValue()} attribute is already at its maximum value.";
			}

			var (nonFree, free) = Storyboard.EvaluateBoosts(Chargen);
			if (free <= 0 || _maximumFreeBoosts[which] >= _numberOfBoosts[which])
			{
				if (nonFree >= Storyboard.MaximumExtraBoosts)
				{
					return Storyboard.MaximumExtraBoosts > 0
						? "You have already made the maximum number of extra non-free boosts you can make."
						: "You don't have any boosts left to spend";
				}
			}

			_numberOfBoosts[which] += 1;
			UpdateChargenSelectedAttributes();
			return Display();
		}
	}

	#region Building Commands

	public override string HelpText => $@"{BaseHelpText}
	#3blurb#0 - drops you into an editor to change the blurb
	#3maxboosts <prog>#0 - sets the prog for maximum total boosts
	#3maxfree <prog>#0 - sets the prog for maximum free boosts per attribute
	#3maxminus <prog>#0 - sets the prog for maximum number of minuses allowed
	#3freeboosts <prog>#0 - sets the prog for total number of free boosts
	#3attribute <prog>#0 - sets the prog for base attribute value per attribute
	#3cost <expression>#0 - sets the cost of the each boost. See below for special parameters.
	#3maxextra <#>#0 - sets the maximum number of non-free boosts that can be made per attribute
	#3resource <which>#0 - sets the resource used to pay for boosts

The following parameters can be used in the cost expression:

	#6boosts#0 - the number of non-free boosts selected";

	/// <inheritdoc />
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "blurb":
				return BuildingCommandBlurb(actor, command);
			case "maxboosts":
				return BuildingCommandMaxBoosts(actor, command);
			case "maxfree":
				return BuildingCommandMaxFree(actor, command);
			case "maxminus":
				return BuildingCommandMaxMinus(actor, command);
			case "freeboosts":
				return BuildingCommandFreeBoosts(actor, command);
			case "attribute":
				return BuildingCommandAttribute(actor, command);
			case "cost":
				return BuildingCommandCost(actor, command);
			case "maxextra":
				return BuildingCommandMaxExtra(actor, command);
			case "resource":
				return BuildingCommandResource(actor, command);
			// TODO
		}

		return BuildingCommandFallback(actor, command.GetUndo());
	}

	private bool BuildingCommandResource(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which account resource should be used to pay for attribute boosts?");
			return false;
		}

		var resource = Gameworld.ChargenResources.GetByIdOrName(command.SafeRemainingArgument);
		if (resource is null)
		{
			actor.OutputHandler.Send("There is no such account resource.");
			return false;
		}

		BoostResource = resource;
		Changed = true;
		actor.OutputHandler.Send(
			$"Attribute boosts will now be paid for with the {resource.PluralName.ColourValue()} account resource.");
		return true;
	}

	private bool BuildingCommandMaxExtra(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"How many extra non-free boosts should players be able to make for each attribute?");
			return false;
		}

		if (!int.TryParse(command.SafeRemainingArgument, out var value) || value < 0)
		{
			actor.OutputHandler.Send("That is not a valid number.");
			return false;
		}

		MaximumExtraBoosts = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"Players will now be able to make as many as {value.ToString("N0", actor).ColourValue()} extra {"boost".Pluralise(value != 1)} for each attribute.");
		return true;
	}

	private bool BuildingCommandCost(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What should the expression for boost cost be?");
			return false;
		}

		var expression = new Expression(command.SafeRemainingArgument, EvaluateOptions.IgnoreCase);
		if (expression.HasErrors())
		{
			actor.OutputHandler.Send(expression.Error);
			return false;
		}

		BoostCostExpression = expression;
		Changed = true;
		actor.OutputHandler.Send(
			$"The expression for boost cost will now be {expression.OriginalExpression.ColourName()}.");
		return true;
	}

	private bool BuildingCommandAttribute(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog do you want to use?");
			return false;
		}

		var prog = new FutureProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			FutureProgVariableTypes.Number, new List<IEnumerable<FutureProgVariableTypes>>
			{
				new List<FutureProgVariableTypes>
				{
					FutureProgVariableTypes.Chargen,
					FutureProgVariableTypes.Trait
				},
				new List<FutureProgVariableTypes>
				{
					FutureProgVariableTypes.Chargen
				}
			}).LookupProg();
		if (prog is null)
		{
			return false;
		}

		AttributeBaseValueProg = prog;
		Changed = true;
		actor.OutputHandler.Send(
			$"The {prog.MXPClickableFunctionName()} prog will now be used to control default attribute value (pre-boosts).");
		return true;
	}

	private bool BuildingCommandFreeBoosts(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog do you want to use?");
			return false;
		}

		var prog = new FutureProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			FutureProgVariableTypes.Number, new List<IEnumerable<FutureProgVariableTypes>>
			{
				new List<FutureProgVariableTypes>
				{
					FutureProgVariableTypes.Chargen
				}
			}).LookupProg();
		if (prog is null)
		{
			return false;
		}

		FreeBoostsProg = prog;
		Changed = true;
		actor.OutputHandler.Send(
			$"The {prog.MXPClickableFunctionName()} prog will now be used to control the total number of free boosts to spend.");
		return true;
	}

	private bool BuildingCommandMaxMinus(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog do you want to use?");
			return false;
		}

		var prog = new FutureProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			FutureProgVariableTypes.Number, new List<IEnumerable<FutureProgVariableTypes>>
			{
				new List<FutureProgVariableTypes>
				{
					FutureProgVariableTypes.Chargen,
					FutureProgVariableTypes.Trait
				},
				new List<FutureProgVariableTypes>
				{
					FutureProgVariableTypes.Chargen
				}
			}).LookupProg();
		if (prog is null)
		{
			return false;
		}

		MaximumMinusesProg = prog;
		Changed = true;
		actor.OutputHandler.Send(
			$"The {prog.MXPClickableFunctionName()} prog will now be used to control the maximum minuses that can be applied to an attribute.");
		return true;
	}

	private bool BuildingCommandMaxFree(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog do you want to use?");
			return false;
		}

		var prog = new FutureProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			FutureProgVariableTypes.Number, new List<IEnumerable<FutureProgVariableTypes>>
			{
				new List<FutureProgVariableTypes>
				{
					FutureProgVariableTypes.Chargen,
					FutureProgVariableTypes.Trait
				},
				new List<FutureProgVariableTypes>
				{
					FutureProgVariableTypes.Chargen
				}
			}).LookupProg();
		if (prog is null)
		{
			return false;
		}

		MaximumFreeBoostsProg = prog;
		Changed = true;
		actor.OutputHandler.Send(
			$"The {prog.MXPClickableFunctionName()} prog will now be used to control maximum boosts per attribute.");
		return true;
	}

	private bool BuildingCommandMaxBoosts(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog do you want to use?");
			return false;
		}

		var prog = new FutureProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			FutureProgVariableTypes.Number, new List<IEnumerable<FutureProgVariableTypes>>
			{
				new List<FutureProgVariableTypes>
				{
					FutureProgVariableTypes.Chargen,
					FutureProgVariableTypes.Trait
				},
				new List<FutureProgVariableTypes>
				{
					FutureProgVariableTypes.Chargen
				}
			}).LookupProg();
		if (prog is null)
		{
			return false;
		}

		MaximumBoostsProg = prog;
		Changed = true;
		actor.OutputHandler.Send(
			$"The {prog.MXPClickableFunctionName()} prog will now be used to control the maximum total boosts per attribute.");
		return true;
	}

	private bool BuildingCommandBlurb(ICharacter actor, StringStack command)
	{
		actor.EditorMode(PostBlurb, CancelBlurb, 1.0, Blurb, EditorOptions.None,
			new object[] { actor.Account.InnerLineFormatLength });
		return true;
	}

	private void CancelBlurb(IOutputHandler handler, object[] args)
	{
		handler.Send("You decide not to change the blurb for this chargen screen.");
	}

	private void PostBlurb(string text, IOutputHandler handler, object[] args)
	{
		Blurb = text;
		Changed = true;
		handler.Send($"You set the blurb to the following:\n\n{text.Wrap((int)args[0]).SubstituteANSIColour()}");
	}

	#endregion
}