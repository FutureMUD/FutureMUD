using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.CharacterCreation.Resources;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using MudSharp.Models;

namespace MudSharp.CharacterCreation;

public abstract class ChargenScreenStoryboard : SaveableItem, IChargenScreenStoryboard
{
	protected ChargenScreenStoryboard(Models.ChargenScreenStoryboard storyboard, IFuturemud gameworld) : base()
	{
		_id = storyboard.Id;
		_name = StoryboardName;
		Gameworld = gameworld;
	}

	protected ChargenScreenStoryboard()
	{
	}

	protected ChargenScreenStoryboard(IFuturemud gameworld, IChargenScreenStoryboard storyboard)
	{
		Gameworld = gameworld;
		_id = storyboard.Id;
		_name = StoryboardName;
	}

	protected void SaveAfterTypeChange()
	{
		using (new FMDB())
		{
			var dbitem = FMDB.Context.ChargenScreenStoryboards.Find(Id);
			dbitem.ChargenType = StoryboardName;
			dbitem.StageDefinition = SaveDefinition();
			FMDB.Context.SaveChanges();
		}
	}

	#region Overrides of FrameworkItem

	/// <inheritdoc />
	public sealed override string FrameworkItemType => "ChargenScreenStoryboard";

	#endregion

	protected abstract string StoryboardName { get; }

	public abstract class ChargenScreen : IChargenScreen
	{
		protected IChargenScreenStoryboard _storyboard;
		protected IChargen Chargen;

		protected ChargenScreen(IChargen chargen, IChargenScreenStoryboard storyboard)
		{
			_storyboard = storyboard;
			Chargen = chargen;
		}

		protected IAccount Account => Chargen.Account;

		#region IChargenScreen Members

		public virtual IChargenScreen NextScreen
			=>
				_storyboard.Gameworld.ChargenStoryboard.StageScreenMap[
					_storyboard.Gameworld.ChargenStoryboard.DefaultNextStage[AssociatedStage]].GetScreen(Chargen);

		public abstract ChargenStage AssociatedStage { get; }

		public abstract string Display();

		public ChargenScreenState State { get; protected set; }

		public abstract string HandleCommand(string command);

		#endregion IChargenScreen Members

		protected bool HasChargenAdvice()
		{
			return Chargen.SelectedRace?.ChargenAdvices.Any(x =>
				       x.TargetStage == AssociatedStage && x.ShouldShowAdviceProg?.ExecuteBool(Chargen) != false) ==
			       true ||
			       Chargen.SelectedEthnicity?.ChargenAdvices.Any(x =>
				       x.TargetStage == AssociatedStage && x.ShouldShowAdviceProg?.ExecuteBool(Chargen) != false) ==
			       true ||
			       Chargen.SelectedCulture?.ChargenAdvices.Any(x =>
				       x.TargetStage == AssociatedStage && x.ShouldShowAdviceProg?.ExecuteBool(Chargen) != false) ==
			       true ||
			       Chargen.SelectedRoles.Any(x => x.ChargenAdvices.Any(y =>
				       y.TargetStage == AssociatedStage && y.ShouldShowAdviceProg?.ExecuteBool(Chargen) != false));
		}

		protected bool ShownChargenAdvice;

		protected string HandleCommandChargenAdvice(string command)
		{
			if (command.EqualTo("continue"))
			{
				ShownChargenAdvice = true;
				return Display();
			}

			return DisplayChargenAdvice();
		}

		protected string DisplayChargenAdvice()
		{
			var sb = new StringBuilder(
				$"You have Chargen Advice for the {AssociatedStage.Describe()} Stage:\n".Colour(Telnet.BoldWhite));
			foreach (var advice in Chargen.SelectedRace?.ChargenAdvices.Where(x =>
				         x.TargetStage == AssociatedStage &&
				         x.ShouldShowAdviceProg?.ExecuteBool(Chargen) != false) ?? Enumerable.Empty<ChargenAdvice>())
			{
				sb.AppendLine();
				sb.AppendLine(advice.AdviceTitle.GetLineWithTitle(Chargen.Account.LineFormatLength,
					Chargen.Account.UseUnicode, Telnet.Cyan, Telnet.BoldYellow));
				sb.AppendLine();
				sb.AppendLine(advice.AdviceText.SubstituteANSIColour());
				sb.AppendLine();
				sb.AppendLine("End".GetLineWithTitle(Chargen.Account.LineFormatLength, Chargen.Account.UseUnicode,
					Telnet.Cyan, Telnet.BoldYellow));
			}

			foreach (var advice in Chargen.SelectedEthnicity?.ChargenAdvices.Where(x =>
				         x.TargetStage == AssociatedStage &&
				         x.ShouldShowAdviceProg?.ExecuteBool(Chargen) != false) ?? Enumerable.Empty<ChargenAdvice>())
			{
				sb.AppendLine();
				sb.AppendLine(advice.AdviceTitle.GetLineWithTitle(Chargen.Account.LineFormatLength,
					Chargen.Account.UseUnicode, Telnet.Cyan, Telnet.BoldYellow));
				sb.AppendLine();
				sb.AppendLine(advice.AdviceText.SubstituteANSIColour());
				sb.AppendLine();
				sb.AppendLine("End".GetLineWithTitle(Chargen.Account.LineFormatLength, Chargen.Account.UseUnicode,
					Telnet.Cyan, Telnet.BoldYellow));
			}

			foreach (var advice in Chargen.SelectedCulture?.ChargenAdvices.Where(x =>
				         x.TargetStage == AssociatedStage &&
				         x.ShouldShowAdviceProg?.ExecuteBool(Chargen) != false) ?? Enumerable.Empty<ChargenAdvice>())
			{
				sb.AppendLine();
				sb.AppendLine(advice.AdviceTitle.GetLineWithTitle(Chargen.Account.LineFormatLength,
					Chargen.Account.UseUnicode, Telnet.Cyan, Telnet.BoldYellow));
				sb.AppendLine();
				sb.AppendLine(advice.AdviceText.SubstituteANSIColour());
				sb.AppendLine();
				sb.AppendLine("End".GetLineWithTitle(Chargen.Account.LineFormatLength, Chargen.Account.UseUnicode,
					Telnet.Cyan, Telnet.BoldYellow));
			}

			foreach (var advice in Chargen.SelectedRoles.SelectMany(x => x.ChargenAdvices).Distinct().Where(x =>
				         x.TargetStage == AssociatedStage && x.ShouldShowAdviceProg?.ExecuteBool(Chargen) != false))
			{
				sb.AppendLine();
				sb.AppendLine(advice.AdviceTitle.GetLineWithTitle(Chargen.Account.LineFormatLength,
					Chargen.Account.UseUnicode, Telnet.Cyan, Telnet.BoldYellow));
				sb.AppendLine();
				sb.AppendLine(advice.AdviceText.SubstituteANSIColour());
				sb.AppendLine();
				sb.AppendLine("End".GetLineWithTitle(Chargen.Account.LineFormatLength, Chargen.Account.UseUnicode,
					Telnet.Cyan, Telnet.BoldYellow));
			}

			sb.AppendLine();
			sb.AppendLine(
				$"You may type {"continue".ColourCommand()} to leave this screen and continue the current stage.");

			return sb.ToString();
		}
	}

	#region IChargenScreenStoryboard Members

	public abstract ChargenStage Stage { get; }

	public abstract IChargenScreen GetScreen(IChargen chargen);

	public virtual IEnumerable<(IChargenResource Resource, int Cost)> ChargenCosts(IChargen chargen)
	{
		return Enumerable.Empty<(IChargenResource Resource, int Cost)>();
	}

	public virtual string Show(ICharacter voyeur)
	{
		return "This storyboard type has not been set up with a show display yet.";
	}

	protected string ShowHeader(ICharacter voyeur)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"{Name.SplitCamelCase()} Storyboard".GetLineWithTitle(voyeur, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine($"Stage: {Stage.DescribeEnum().ColourName()}");
		sb.AppendLine(
			$"Dependencies: {Gameworld.ChargenStoryboard.StageDependencies[Stage].Select(x => x.DescribeEnum().ColourName()).DefaultIfEmpty("None".ColourError()).ListToString()}");
		var order = Gameworld.ChargenStoryboard.OrderOf(Stage);
		if (order == 0)
		{
			sb.AppendLine($"Previous Screen: {"None".ColourError()}");
		}
		else
		{
			sb.AppendLine(
				$"Previous Screen: {Gameworld.ChargenStoryboard.DefaultOrder.ElementAt(order - 1).DescribeEnum().ColourName()}");
		}

		if (order == Gameworld.ChargenStoryboard.DefaultOrder.Count - 1)
		{
			sb.AppendLine($"Next Screen: {"None".ColourError()}");
		}
		else
		{
			sb.AppendLine(
				$"Next Screen: {Gameworld.ChargenStoryboard.DefaultOrder.ElementAt(order + 1).DescribeEnum().ColourName()}");
		}

		return sb.ToString();
	}

	protected static string BaseHelpText = @"You can use the following options with this screen:

	#3dependency <stage>#0 - sets a dependency of this stage on another stage";


	public abstract string HelpText { get; }

	public virtual bool BuildingCommand(ICharacter actor, StringStack command)
	{
		actor.OutputHandler.Send("This storyboard type has not been set up with any building commands yet.");
		return false;
	}

	protected bool BuildingCommandFallback(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "dep":
			case "dependency":
			case "depends":
			case "depend":
				return BuildingCommandDependency(actor, command);
			default:
				actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
				return false;
		}
	}

	protected bool BuildingCommandDependency(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Which other chargen stage do you want to toggle this one as being dependent on?\nThe valid choices are {Enum.GetValues<ChargenStage>().Select(x => x.DescribeEnum().ColourName()).ListToString()}.");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParseEnum(out ChargenStage stage))
		{
			actor.OutputHandler.Send(
				$"That is not a valid chargen stage.\nThe valid choices are {Enum.GetValues<ChargenStage>().Select(x => x.DescribeEnum().ColourName()).ListToString()}.");
			return false;
		}

		if (stage.In(
			    ChargenStage.ConfirmQuit,
			    ChargenStage.Menu,
			    ChargenStage.None,
			    ChargenStage.Submit
		    ))
		{
			actor.OutputHandler.Send($"The {stage.DescribeEnum().ColourName()} stage cannot be used as a dependency.");
			return false;
		}

		if (Stage == stage)
		{
			actor.OutputHandler.Send("You cannot make a stage depend on itself.");
			return false;
		}

		if (Gameworld.ChargenStoryboard.OrderOf(stage) > Gameworld.ChargenStoryboard.OrderOf(Stage))
		{
			actor.OutputHandler.Send(
				$"The {stage.DescribeEnum().ColourName()} currently occurs after the {Name.ColourName()} stage. It cannot depend on a stage that happens after it in the order.");
			return false;
		}

		if (Gameworld.ChargenStoryboard.StageDependencies[Stage].Contains(stage))
		{
			Gameworld.ChargenStoryboard.RemoveDependency(Stage, stage);
			actor.OutputHandler.Send(
				$"The {Name.ColourName()} stage will no longer depend on the {stage.DescribeEnum().ColourName()} stage.");
		}
		else
		{
			Gameworld.ChargenStoryboard.AddDependency(Stage, stage);
			actor.OutputHandler.Send(
				$"The {Name.ColourName()} stage will now depend on the {stage.DescribeEnum().ColourName()} stage.");
		}

		Changed = true;
		return true;
	}

	#endregion IChargenScreenStoryboard Members

	#region Overrides of SaveableItem

	/// <inheritdoc />
	public sealed override void Save()
	{
		var dbitem = FMDB.Context.ChargenScreenStoryboards.Find(Id);
		dbitem.Order = Gameworld.ChargenStoryboard.OrderOf(Stage) * 10;
		dbitem.NextStage = (int)Gameworld.ChargenStoryboard.DefaultNextStage[Stage];
		FMDB.Context.ChargenScreenStoryboardDependentStages.RemoveRange(dbitem.DependentStages);
		foreach (var dependency in Gameworld.ChargenStoryboard.StageDependencies[Stage])
		{
			dbitem.DependentStages.Add(new ChargenScreenStoryboardDependentStage
				{ Owner = dbitem, Dependency = (int)dependency });
		}

		dbitem.StageDefinition = SaveDefinition();

		Changed = false;
	}

	protected abstract string SaveDefinition();

	#endregion
}