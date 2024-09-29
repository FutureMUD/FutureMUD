using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Editor;
using MudSharp.Framework;
using MudSharp.Framework.Units;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;

namespace MudSharp.CharacterCreation.Screens;

internal class WeightPickerScreenStoryboard : ChargenScreenStoryboard
{
	private WeightPickerScreenStoryboard()
	{
	}

	public WeightPickerScreenStoryboard(IFuturemud gameworld, Models.ChargenScreenStoryboard dbitem)
		: base(dbitem, gameworld)
	{
		var definition = XElement.Parse(dbitem.StageDefinition);
		Blurb = definition.Element("Blurb").Value;
		MaximumWeightProg = long.TryParse(definition.Element("MaximumWeightProg").Value, out var value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.FirstOrDefault(
				x =>
					x.FunctionName.Equals(definition.Element("MaximumWeightProg").Value,
						StringComparison.InvariantCultureIgnoreCase));
		MinimumWeightProg = long.TryParse(definition.Element("MinimumWeightProg").Value, out value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.FirstOrDefault(
				x =>
					x.FunctionName.Equals(definition.Element("MinimumWeightProg").Value,
						StringComparison.InvariantCultureIgnoreCase));
	}

	protected override string StoryboardName => "WeightPicker";

	public IFutureProg MaximumWeightProg { get; protected set; }

	public IFutureProg MinimumWeightProg { get; protected set; }

	public override ChargenStage Stage => ChargenStage.SelectWeight;

	public string Blurb { get; protected set; }

	#region Overrides of ChargenScreenStoryboard

	/// <inheritdoc />
	protected override string SaveDefinition()
	{
		return new XElement("Definition",
			new XElement("Blurb", new XCData(Blurb)),
			new XElement("MaximumWeightProg", MaximumWeightProg?.Id ?? 0),
			new XElement("MinimumWeightProg", MinimumWeightProg?.Id ?? 0)
		).ToString();
	}

	#endregion

	public override string Show(ICharacter voyeur)
	{
		var sb = new StringBuilder();
		sb.Append(ShowHeader(voyeur));
		sb.AppendLine();
		sb.AppendLine("This screen allows people to enter their weight directly.".Wrap(voyeur.InnerLineFormatLength)
			.ColourCommand());
		sb.AppendLine();
		sb.AppendLine($"Minimum Weight Prog: {MinimumWeightProg.MXPClickableFunctionNameWithId()}");
		sb.AppendLine($"Maximum Weight Prog: {MaximumWeightProg.MXPClickableFunctionNameWithId()}");
		sb.AppendLine();
		sb.AppendLine("Blurb".GetLineWithTitle(voyeur, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine(Blurb.Wrap(voyeur.InnerLineFormatLength).SubstituteANSIColour());
		return sb.ToString();
	}

	public static void RegisterFactory()
	{
		ChargenStoryboard.RegisterFactory(ChargenStage.SelectWeight,
			new ChargenScreenStoryboardFactory("WeightPicker",
				(game, dbitem) => new WeightPickerScreenStoryboard(game, dbitem)),
			"WeightPicker",
			"Enter weight directly as a number",
			((ChargenScreenStoryboard)Activator.CreateInstance(MethodBase.GetCurrentMethod().DeclaringType, true))
			.HelpText);
	}

	public override IChargenScreen GetScreen(IChargen chargen)
	{
		return new WeightPickerScreen(chargen, this);
	}

	internal class WeightPickerScreen : ChargenScreen
	{
		protected WeightPickerScreenStoryboard Storyboard;

		internal WeightPickerScreen(IChargen chargen, WeightPickerScreenStoryboard storyboard)
			: base(chargen, storyboard)
		{
			Storyboard = storyboard;
		}

		public override ChargenStage AssociatedStage => ChargenStage.SelectWeight;

		public override string Display()
		{
			if (!ShownChargenAdvice && HasChargenAdvice())
			{
				return DisplayChargenAdvice();
			}

			return
				string.Format(
					"Weight Selection".Colour(Telnet.Cyan) +
					"\n\n{0}\n\nEnter your desired weight, between {1} and {2}.",
					Storyboard.Blurb.SubstituteANSIColour().Wrap(Account.InnerLineFormatLength),
					Storyboard.Gameworld.UnitManager.Describe(
						Convert.ToDouble(Storyboard.MinimumWeightProg.Execute(Chargen)),
						UnitType.Mass, Account).Colour(Telnet.Green),
					Storyboard.Gameworld.UnitManager.Describe(
						Convert.ToDouble(Storyboard.MaximumWeightProg.Execute(Chargen)),
						UnitType.Mass, Account).Colour(Telnet.Green)
				);
		}

		public override string HandleCommand(string command)
		{
			if (string.IsNullOrEmpty(command))
			{
				return Display();
			}

			if (!ShownChargenAdvice && HasChargenAdvice())
			{
				return HandleCommandChargenAdvice(command);
			}

			var weight = Storyboard.Gameworld.UnitManager.GetBaseUnits(command, UnitType.Mass, out var success);
			if (!success)
			{
				return "That is not a valid weight.";
			}

			var minimumWeight = Convert.ToDouble(Storyboard.MinimumWeightProg.Execute(Chargen));
			var maximumWeight = Convert.ToDouble(Storyboard.MaximumWeightProg.Execute(Chargen));

			if (weight < minimumWeight || weight > maximumWeight)
			{
				var corrected = false;
				if (Storyboard.Gameworld.UnitManager.Describe(weight, UnitType.Mass, Chargen.Account) ==
				    Storyboard.Gameworld.UnitManager.Describe(minimumWeight, UnitType.Mass, Chargen.Account))
				{
					weight = minimumWeight;
					corrected = true;
				}

				if (Storyboard.Gameworld.UnitManager.Describe(weight, UnitType.Mass, Chargen.Account) ==
				    Storyboard.Gameworld.UnitManager.Describe(maximumWeight, UnitType.Mass, Chargen.Account))
				{
					weight = maximumWeight;
					corrected = true;
				}

				if (!corrected)
				{
					return
						$"You must select a weight between {Storyboard.Gameworld.UnitManager.Describe(minimumWeight, UnitType.Mass, Account).Colour(Telnet.Green)} and {Storyboard.Gameworld.UnitManager.Describe(maximumWeight, UnitType.Mass, Account).Colour(Telnet.Green)}.";
				}
			}

			Chargen.SelectedWeight = weight;
			State = ChargenScreenState.Complete;
			return "You select " +
			       Storyboard.Gameworld.UnitManager.Describe(weight, UnitType.Mass, Account).Colour(Telnet.Green) +
			       " as your weight.\n";
		}
	}

	#region Building Commands

	public override string HelpText => $@"{BaseHelpText}
	#3max <prog>#0 - sets the prog for maximum weight
	#3min <prog>#0 - sets the prog for minimum weight";

	/// <inheritdoc />
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "blurb":
				return BuildingCommandBlurb(actor, command);
			case "max":
			case "maxweight":
				return BuildingCommandMax(actor, command);
			case "min":
			case "minweight":
				return BuildingCommandMin(actor, command);
		}

		return BuildingCommandFallback(actor, command.GetUndo());
	}

	private bool BuildingCommandMin(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"Which prog should control the minimum weight of characters in character creation?");
			return false;
		}

		var prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			FutureProgVariableTypes.Number, new List<FutureProgVariableTypes>
			{
				FutureProgVariableTypes.Chargen
			}).LookupProg();
		if (prog is null)
		{
			return false;
		}

		MinimumWeightProg = prog;
		Changed = true;
		actor.OutputHandler.Send(
			$"The prog {prog.MXPClickableFunctionName()} will now be used to control minimum weight in character creation.");
		return true;
	}

	private bool BuildingCommandMax(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"Which prog should control the maximum weight of characters in character creation?");
			return false;
		}

		var prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			FutureProgVariableTypes.Number, new List<FutureProgVariableTypes>
			{
				FutureProgVariableTypes.Chargen
			}).LookupProg();
		if (prog is null)
		{
			return false;
		}

		MaximumWeightProg = prog;
		Changed = true;
		actor.OutputHandler.Send(
			$"The prog {prog.MXPClickableFunctionName()} will now be used to control maximum weight in character creation.");
		return true;
	}

	private bool BuildingCommandBlurb(ICharacter actor, StringStack command)
	{
		actor.OutputHandler.Send(
			$"Replacing the following text:\n\n{Blurb.SubstituteANSIColour().Wrap(actor.InnerLineFormatLength)}\n\nEnter your new blurb below:\n");
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