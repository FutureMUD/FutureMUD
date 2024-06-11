using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using MudSharp.Body.Traits;
using MudSharp.Body.Traits.Subtypes;
using MudSharp.Character;
using MudSharp.Economy.Payment;
using MudSharp.Editor;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;

namespace MudSharp.CharacterCreation.Screens;

public class AttributeOrdererScreenStoryboard : ChargenScreenStoryboard
{
	private AttributeOrdererScreenStoryboard()
	{
	}

	public AttributeOrdererScreenStoryboard(IFuturemud gameworld, Models.ChargenScreenStoryboard dbitem)
		: base(dbitem, gameworld)
	{
		var definition = XElement.Parse(dbitem.StageDefinition);
		Blurb = definition.Element("Blurb").Value;
	}

	protected AttributeOrdererScreenStoryboard(IFuturemud gameworld, IChargenScreenStoryboard storyboard) : base(
		gameworld, storyboard)
	{
		switch (storyboard)
		{
			case AttributePointBuyScreenStoryboard pbs:
				Blurb = pbs.Blurb;
				break;
		}

		SaveAfterTypeChange();
	}

	protected override string StoryboardName => "AttributeOrderer";

	public string Blurb { get; protected set; }

	public override ChargenStage Stage => ChargenStage.SelectAttributes;

	public override string HelpText => $@"{BaseHelpText}
	#3blurb#0 - drops you into an editor to change the blurb";

	#region Overrides of ChargenScreenStoryboard

	/// <inheritdoc />
	protected override string SaveDefinition()
	{
		return new XElement("Definition",
			new XElement("Blurb", new XCData(Blurb))
		).ToString();
	}

	#endregion

	public static void RegisterFactory()
	{
		ChargenStoryboard.RegisterFactory(ChargenStage.SelectAttributes,
			new ChargenScreenStoryboardFactory("AttributeOrderer",
				(game, dbitem) => new AttributeOrdererScreenStoryboard(game, dbitem),
				(game, other) => new AttributeOrdererScreenStoryboard(game, other)),
			"AttributeOrderer",
			"Attributes rolled, select order only",
			((ChargenScreenStoryboard)Activator.CreateInstance(MethodBase.GetCurrentMethod().DeclaringType, true))
			.HelpText);
	}

	public override string Show(ICharacter voyeur)
	{
		var sb = new StringBuilder();
		sb.Append(ShowHeader(voyeur));
		sb.AppendLine();
		sb.AppendLine(
			"This screen allows the player to choose the order of their attributes only, then randomly rolls and assigns them in that order. It is the classic RPI experience, a.k.a Shadows of Isildur or Armageddon."
				.Wrap(voyeur.InnerLineFormatLength).ColourCommand());
		sb.AppendLine();
		sb.AppendLine("Blurb".GetLineWithTitle(voyeur, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine(Blurb.Wrap(voyeur.InnerLineFormatLength).SubstituteANSIColour());
		return sb.ToString();
	}

	public override IChargenScreen GetScreen(IChargen chargen)
	{
		return new AttributeOrdererScreen(chargen, this);
	}

	internal class AttributeOrdererScreen : ChargenScreen
	{
		private readonly List<IAttributeDefinition> _attributes;
		protected AttributeOrdererScreenStoryboard Storyboard;

		internal AttributeOrdererScreen(IChargen chargen, AttributeOrdererScreenStoryboard storyboard)
			: base(chargen, storyboard)
		{
			Storyboard = storyboard;
			_attributes = Chargen.SelectedRace.Attributes.Where(x => x.TraitType == TraitType.Attribute).ToList();
		}

		public override ChargenStage AssociatedStage => ChargenStage.SelectAttributes;

		public override string Display()
		{
			if (!ShownChargenAdvice && HasChargenAdvice())
			{
				return DisplayChargenAdvice();
			}

			return
				string.Format(
					"Attribute Selection".Colour(Telnet.Cyan) +
					"\n\n{0}\n\n{1}\n\nSelect a desired order for the listed attributes, e.g. {2}",
					Storyboard.Blurb.SubstituteANSIColour()
					          .Wrap(Chargen.Account.InnerLineFormatLength),
					/*Chargen.SelectedRace.Attributes.Select(
					        x =>
					                $"{x.Name.Colour(Telnet.Green)} ({x.Alias.Colour(Telnet.Green)}): {x.ChargenBlurb}")
					    .ListToString(conjunction: "", separator: "\n"),*/
					_attributes.Select(
						           x =>
							           $"{x.Name.Colour(Telnet.Green)}" +
							           (x.Name.ToLower().Equals(x.Alias.ToLower())
								           ? ": "
								           : " (" + x.Alias.Colour(Telnet.Green) + "): ") + x.ChargenBlurb)
					           .ListToString(conjunction: "", separator: "\n"),
					_attributes.Shuffle()
					           .Select(x => x.Alias)
					           .ListToString(separator: " ", conjunction: "")
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

			var split = command.Split(' ');
			if (split.Length != _attributes.Count())
			{
				return "You must select each and every attribute in the order.";
			}

			var selectedAttributes = new List<IAttributeDefinition>();
			foreach (var entry in split)
			{
				var attribute =
					_attributes.FirstOrDefault(x => x.Alias.Equals(entry, StringComparison.InvariantCultureIgnoreCase));
				if (attribute == null)
				{
					return "There is no such attribute as \'" + entry + "\'.";
				}

				if (selectedAttributes.Contains(attribute))
				{
					return "You cannot select " + attribute.Name + " more than once in the sequence.";
				}

				selectedAttributes.Add(attribute);
			}

			var stats = RollRandomStats(split.Length, Chargen.SelectedRace.AttributeTotalCap,
				Chargen.SelectedRace.IndividualAttributeCap, Chargen.SelectedRace.DiceExpression);
			Chargen.SelectedAttributes = selectedAttributes.Select(x =>
				TraitFactory.LoadAttribute(x, null, stats[selectedAttributes.IndexOf(x)] +
				                                    Convert.ToDouble(Chargen.SelectedRace.AttributeBonusProg.Execute(x, Chargen))
				)).ToList<ITrait>();
			State = ChargenScreenState.Complete;
			return "\n";
		}

		private List<int> RollRandomStats(int numberOfStats, int totalCap, int individualCap, string diceExpression)
		{
			var results = new List<int>();
			for (var i = 0; i < numberOfStats; i++)
			{
				results.Add(Dice.Roll(diceExpression));
			}

			var difference = totalCap - results.Sum();
			if (difference < 0)
			{
				for (var i = 0; i > difference; i--)
				{
					var whichStat = Constants.Random.Next(0, results.Count);
					results[whichStat] = results[whichStat] - 1;
				}
			}
			else if (difference > 0)
			{
				for (var i = 0; i < difference; i++)
				{
					var whichStat = Constants.Random.Next(0, results.Count);
					if (results[whichStat] == individualCap)
					{
						i--;
						continue;
					}

					results[whichStat] = results[whichStat] + 1;
				}
			}

			while (results.Any(x => x > individualCap))
			{
				var whichStat = results.FindIndex(x => x > individualCap);
				results[whichStat] = results[whichStat] - 1;
				whichStat = Constants.Random.Next(0, results.Count);
				results[whichStat] = results[whichStat] + 1;
			}

			results.Sort();
			results.Reverse();
			return results;
		}
	}

	#region Building Commands

	/// <inheritdoc />
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "blurb":
				return BuildingCommandBlurb(actor, command);
		}

		return BuildingCommandFallback(actor, command.GetUndo());
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