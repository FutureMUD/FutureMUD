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

public class HeightPickerScreenStoryboard : ChargenScreenStoryboard
{
	private HeightPickerScreenStoryboard()
	{
	}

	public HeightPickerScreenStoryboard(IFuturemud gameworld, Models.ChargenScreenStoryboard dbitem)
		: base(dbitem, gameworld)
	{
		var definition = XElement.Parse(dbitem.StageDefinition);
		Blurb = definition.Element("Blurb").Value;
		MaximumHeightProg = long.TryParse(definition.Element("MaximumHeightProg").Value, out var value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.FirstOrDefault(
				x =>
					x.FunctionName.Equals(definition.Element("MaximumHeightProg").Value,
						StringComparison.InvariantCultureIgnoreCase));
		MinimumHeightProg = long.TryParse(definition.Element("MinimumHeightProg").Value, out value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.FirstOrDefault(
				x =>
					x.FunctionName.Equals(definition.Element("MinimumHeightProg").Value,
						StringComparison.InvariantCultureIgnoreCase));
	}

	protected override string StoryboardName => "HeightPicker";

	public string Blurb { get; protected set; }

	public IFutureProg MaximumHeightProg { get; protected set; }

	public IFutureProg MinimumHeightProg { get; protected set; }

	public override ChargenStage Stage => ChargenStage.SelectHeight;

	#region Overrides of ChargenScreenStoryboard

	/// <inheritdoc />
	protected override string SaveDefinition()
	{
		return new XElement("Definition",
			new XElement("Blurb", new XCData(Blurb)),
			new XElement("MaximumHeightProg", MaximumHeightProg?.Id ?? 0),
			new XElement("MinimumHeightProg", MinimumHeightProg?.Id ?? 0)
		).ToString();
	}

	#endregion

	public override string Show(ICharacter voyeur)
	{
		var sb = new StringBuilder();
		sb.Append(ShowHeader(voyeur));
		sb.AppendLine();
		sb.AppendLine("This screen allows people to enter their height directly.".Wrap(voyeur.InnerLineFormatLength)
			.ColourCommand());
		sb.AppendLine();
		sb.AppendLine($"Minimum Height Prog: {MinimumHeightProg.MXPClickableFunctionNameWithId()}");
		sb.AppendLine($"Maximum Height Prog: {MaximumHeightProg.MXPClickableFunctionNameWithId()}");
		sb.AppendLine();
		sb.AppendLine("Blurb".GetLineWithTitle(voyeur, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine(Blurb.Wrap(voyeur.InnerLineFormatLength).SubstituteANSIColour());
		return sb.ToString();
	}

	public static void RegisterFactory()
	{
		ChargenStoryboard.RegisterFactory(ChargenStage.SelectHeight,
			new ChargenScreenStoryboardFactory("HeightPicker",
				(game, dbitem) => new HeightPickerScreenStoryboard(game, dbitem)),
			"HeightPicker",
			"Enter a height directly as a measure",
			((ChargenScreenStoryboard)Activator.CreateInstance(MethodBase.GetCurrentMethod().DeclaringType, true))
			.HelpText);
	}

	public override IChargenScreen GetScreen(IChargen chargen)
	{
		return new HeightPickerScreen(chargen, this);
	}

	internal class HeightPickerScreen : ChargenScreen
	{
		protected HeightPickerScreenStoryboard Storyboard;

		internal HeightPickerScreen(IChargen chargen, HeightPickerScreenStoryboard storyboard)
			: base(chargen, storyboard)
		{
			Storyboard = storyboard;
		}

		public override ChargenStage AssociatedStage => ChargenStage.SelectHeight;

		public override string Display()
		{
			if (!ShownChargenAdvice && HasChargenAdvice())
			{
				return DisplayChargenAdvice();
			}

			return
				string.Format(
					"Height Selection".Colour(Telnet.Cyan) +
					"\n\n{0}\n\nEnter your desired height, between {1} and {2}.",
					Storyboard.Blurb.SubstituteANSIColour().Wrap(Account.InnerLineFormatLength),
					Storyboard.Gameworld.UnitManager.Describe(
						Convert.ToDouble(Storyboard.MinimumHeightProg.Execute(Chargen)),
						UnitType.Length, Chargen.Account).Colour(Telnet.Green),
					Storyboard.Gameworld.UnitManager.Describe(
						Convert.ToDouble(Storyboard.MaximumHeightProg.Execute(Chargen)),
						UnitType.Length, Chargen.Account).Colour(Telnet.Green)
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

			var height = Storyboard.Gameworld.UnitManager.GetBaseUnits(command, UnitType.Length, out var success);
			if (!success)
			{
				return "That is not a valid height.";
			}

			var minimumHeight = Convert.ToDouble(Storyboard.MinimumHeightProg.Execute(Chargen));
			var maximumHeight = Convert.ToDouble(Storyboard.MaximumHeightProg.Execute(Chargen));

			if (height < minimumHeight || height > maximumHeight)
			{
				var corrected = false;
				if (Storyboard.Gameworld.UnitManager.Describe(height, UnitType.Length, Chargen.Account) ==
				    Storyboard.Gameworld.UnitManager.Describe(minimumHeight, UnitType.Length, Chargen.Account))
				{
					height = minimumHeight;
					corrected = true;
				}

				if (Storyboard.Gameworld.UnitManager.Describe(height, UnitType.Length, Chargen.Account) ==
				    Storyboard.Gameworld.UnitManager.Describe(maximumHeight, UnitType.Length, Chargen.Account))
				{
					height = maximumHeight;
					corrected = true;
				}

				if (!corrected)
				{
					return
						$"You must select a height between {Storyboard.Gameworld.UnitManager.Describe(minimumHeight, UnitType.Length, Chargen.Account).Colour(Telnet.Green)} and {Storyboard.Gameworld.UnitManager.Describe(maximumHeight, UnitType.Length, Chargen.Account).Colour(Telnet.Green)}.";
				}
			}

			Chargen.SelectedHeight = height;
			State = ChargenScreenState.Complete;
			return "You select " +
			       Storyboard.Gameworld.UnitManager.Describe(height, UnitType.Length, Chargen.Account)
			                 .Colour(Telnet.Green) + " as your height.\n";
		}
	}

	#region Building Commands

	public override string HelpText => $@"{BaseHelpText}
	#3max <prog>#0 - sets the prog for maximum height
	#3min <prog>#0 - sets the prog for minimum height";

	/// <inheritdoc />
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "blurb":
				return BuildingCommandBlurb(actor, command);
			case "max":
			case "maxheight":
				return BuildingCommandMax(actor, command);
			case "min":
			case "minheight":
				return BuildingCommandMin(actor, command);
		}

		return BuildingCommandFallback(actor, command.GetUndo());
	}

	private bool BuildingCommandMin(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"Which prog should control the minimum height of characters in character creation?");
			return false;
		}

		var prog = new FutureProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			FutureProgVariableTypes.Number, new List<FutureProgVariableTypes>
			{
				FutureProgVariableTypes.Chargen
			}).LookupProg();
		if (prog is null)
		{
			return false;
		}

		MinimumHeightProg = prog;
		Changed = true;
		actor.OutputHandler.Send(
			$"The prog {prog.MXPClickableFunctionName()} will now be used to control minimum height in character creation.");
		return true;
	}

	private bool BuildingCommandMax(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"Which prog should control the maximum height of characters in character creation?");
			return false;
		}

		var prog = new FutureProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			FutureProgVariableTypes.Number, new List<FutureProgVariableTypes>
			{
				FutureProgVariableTypes.Chargen
			}).LookupProg();
		if (prog is null)
		{
			return false;
		}

		MaximumHeightProg = prog;
		Changed = true;
		actor.OutputHandler.Send(
			$"The prog {prog.MXPClickableFunctionName()} will now be used to control maximum height in character creation.");
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