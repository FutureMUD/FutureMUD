using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Editor;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;

namespace MudSharp.CharacterCreation.Screens;

internal class WelcomeScreenStoryboard : ChargenScreenStoryboard
{
	private readonly List<string> _blurbs = new();

	private WelcomeScreenStoryboard()
	{
	}

	public WelcomeScreenStoryboard(IFuturemud gameworld, Models.ChargenScreenStoryboard dbitem) : base(dbitem,
		gameworld)
	{
		var definition = XElement.Parse(dbitem.StageDefinition);
		foreach (var item in definition.Elements("Blurb"))
		{
			_blurbs.Add(item.Value);
		}
	}

	protected override string StoryboardName => "WelcomeScreen";

	public override ChargenStage Stage => ChargenStage.Welcome;
	public IEnumerable<string> Blurbs => _blurbs;

	public override string HelpText => $@"{BaseHelpText}
	#3blurb add#0 - drops into an editor to create a new blurb screen
	#3blurb edit <##>#0 - drops into an editor to rewrite a blurb screen
	#3blurb reorder <old##> <new##>#0 - changes the order of an existing blurb
	#3blurb remove <##>#0 - removes a blurb from the welcome screen";

	#region Overrides of ChargenScreenStoryboard

	/// <inheritdoc />
	protected override string SaveDefinition()
	{
		return new XElement("Definition",
			from blurb in _blurbs
			select new XElement("Blurb", new XCData(blurb))
		).ToString();
	}

	#endregion

	public override IChargenScreen GetScreen(IChargen chargen)
	{
		return new WelcomeScreen(chargen, this);
	}

	public static void RegisterFactory()
	{
		ChargenStoryboard.RegisterFactory(ChargenStage.Welcome,
			new ChargenScreenStoryboardFactory("WelcomeScreen",
				(gameworld, definition) => new WelcomeScreenStoryboard(gameworld, definition)),
			"WelcomeScreen",
			"Introduction screen for chargen",
			((ChargenScreenStoryboard)Activator.CreateInstance(MethodBase.GetCurrentMethod().DeclaringType, true))
			.HelpText);
	}

	public override string Show(ICharacter voyeur)
	{
		var sb = new StringBuilder();
		sb.Append(ShowHeader(voyeur));
		sb.AppendLine();
		sb.AppendLine(
			"This screen is the first screen that people see when creating a new character. It is an opportunity for you to provide links to website resources, introduce your setting and provide general advice."
				.Wrap(voyeur.InnerLineFormatLength).ColourCommand());
		sb.AppendLine();
		sb.AppendLine("Blurbs".GetLineWithTitle(voyeur, Telnet.Cyan, Telnet.BoldWhite));
		var i = 1;
		foreach (var blurb in _blurbs)
		{
			sb.AppendLine();
			sb.AppendLine($"#{i++.ToString("N0", voyeur)}".Colour(Telnet.BoldYellow));
			sb.AppendLine();
			sb.AppendLine(blurb.Wrap(voyeur.InnerLineFormatLength).SubstituteANSIColour());
		}

		return sb.ToString();
	}

	internal class WelcomeScreen : ChargenScreen
	{
		private IEnumerator<string> _enumerator;
		protected WelcomeScreenStoryboard Storyboard;

		internal WelcomeScreen(IChargen chargen, WelcomeScreenStoryboard storyboard) : base(chargen, storyboard)
		{
			Storyboard = storyboard;
			_enumerator = Storyboard.Blurbs.GetEnumerator();
			if (!_enumerator.MoveNext())
			{
				State = ChargenScreenState.Complete;
			}
		}

		public override ChargenStage AssociatedStage => ChargenStage.Welcome;

		public override string Display()
		{
			return
				string.Format(Account,
					"{0}\n\n{1}\n\nEnter {2} to continue, {3} to begin again, or {4} to skip the introduction.",
					"Introduction and Welcome".Colour(Telnet.Cyan),
					_enumerator.Current.SubstituteANSIColour().Wrap(Account.InnerLineFormatLength),
					"continue".Colour(Telnet.Yellow),
					"reset".Colour(Telnet.Yellow),
					"skip".Colour(Telnet.Yellow)
				);
		}

		public override string HandleCommand(string command)
		{
			if (command.Equals("reset", StringComparison.InvariantCultureIgnoreCase))
			{
				_enumerator = Storyboard.Blurbs.GetEnumerator();
				_enumerator.MoveNext();
				return Display();
			}

			if (command.Equals("skip", StringComparison.InvariantCultureIgnoreCase))
			{
				State = ChargenScreenState.Complete;
				return "You elect to skip the welcome introduction.";
			}

			if (!string.IsNullOrEmpty(command) &&
			    "continue".StartsWith(command, StringComparison.InvariantCultureIgnoreCase))
			{
				if (_enumerator.MoveNext())
				{
					return Display();
				}

				State = ChargenScreenState.Complete;
				return "You finish the welcome introduction.\n";
			}

			return Display();
		}
	}

	#region Overrides of ChargenScreenStoryboard

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
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must either #3add#0, #3edit#0, #3reorder#0 or #3remove#0 a blurb.".SubstituteANSIColour());
			return false;
		}

		switch (command.PopForSwitch())
		{
			case "add":
			case "new":
			case "create":
				return BuildingCommandBlurbAdd(actor);
			case "reorder":
			case "order":
				return BuildingCommandBlurbReorder(actor, command);
			case "edit":
			case "change":
				return BuildingCommandBlurbEdit(actor, command);
			case "remove":
			case "delete":
			case "rem":
			case "del":
				return BuildingCommandBlurbRemove(actor, command);
			default:
				actor.OutputHandler.Send("You must either #3add#0, #3edit#0, #3reorder#0 or #3remove#0 a blurb.".SubstituteANSIColour());
				return false;
		}
	}

	private bool BuildingCommandBlurbRemove(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !int.TryParse(command.PopSpeech(), out var index1) || index1 <= 0)
		{
			actor.OutputHandler.Send(
				"You must enter a valid positive integer for the index of the blurb you'd like to remove.");
			return false;
		}

		if (index1 >= _blurbs.Count)
		{
			actor.OutputHandler.Send(
				$"The welcome screen only has {_blurbs.Count.ToString("N0", actor).ColourValue()} {"blurb".Pluralise(_blurbs.Count != 1)}.");
			return false;
		}

		actor.OutputHandler.Send(
			$"Are you sure you want to delete the {index1.ToOrdinal()} blurb?\n\n{_blurbs[index1 - 1].Wrap(actor.InnerLineFormatLength).SubstituteANSIColour()}\n\nThis action is irreversible.\n{Accept.StandardAcceptPhrasing}");
		actor.AddEffect(new Accept(actor, new GenericProposal
		{
			DescriptionString = "Deleting a blurb from the welcome chargen screen",
			AcceptAction = text =>
			{
				try
				{
					actor.OutputHandler.Send(
						$"You delete the {index1.ToOrdinal()} blurb:\n\n{_blurbs[index1 - 1].Wrap(actor.InnerLineFormatLength).SubstituteANSIColour()}");
					_blurbs.RemoveAt(index1 - 1);
					Changed = true;
				}
				catch
				{
					actor.OutputHandler.Send("The blurb in question had already been removed.");
				}
			},
			RejectAction = text => { actor.OutputHandler.Send("You decide not to delete the blurb."); },
			ExpireAction = () => { actor.OutputHandler.Send("You decide not to delete the blurb."); },
			Keywords = new List<string>()
		}));
		return true;
	}

	private bool BuildingCommandBlurbEdit(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !int.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send("You must specify the number of the blurb that you want to edit.");
			return false;
		}

		if (value < 1)
		{
			actor.OutputHandler.Send("You must specify a valid positive integer as the blurb position.");
			return false;
		}

		if (value > _blurbs.Count)
		{
			actor.OutputHandler.Send(
				$"There are only {_blurbs.Count.ToString("N0", actor).ColourValue()} {"blurb".Pluralise(_blurbs.Count != 1)}, so {value.ToString("N0", actor).ColourValue()} is not a valid position.");
			return false;
		}

		actor.OutputHandler.Send(
			$"Now editing the {value.ToOrdinal().ColourValue()} blurb text, replacing:\n\n{_blurbs[value - 1].Wrap(actor.InnerLineFormatLength).SubstituteANSIColour()}\n\nEnter your text in the editor below:\n");
		actor.EditorMode(PostEditBlurb, CancelEditBlurb, 1.0, _blurbs[value - 1], EditorOptions.None,
			new object[] { actor.Account.InnerLineFormatLength, value - 1, actor });
		return true;
	}

	private void CancelEditBlurb(IOutputHandler handler, object[] args)
	{
		var index = (int)args[1];
		handler.Send(
			$"You decide not to change the {(index + 1).ToOrdinal().ColourValue()} blurb for the welcome screen.");
	}

	private void PostEditBlurb(string text, IOutputHandler handler, object[] args)
	{
		var index = (int)args[1];
		if (_blurbs.Count <= index)
		{
			var ch = (ICharacter)args[2];
			ch.AddEffect(new StoredEditorText(ch, text));
			handler.Send(
				"Blurbs were deleted while you were editing. The text you entered has been saved as a *recall text if you want to recycle.");
			return;
		}

		_blurbs[index] = text;
		Changed = true;
		handler.Send(
			$"You set the {(index + 1).ToOrdinal().ColourValue()} blurb to the following:\n\n{text.Wrap((int)args[0]).SubstituteANSIColour()}");
	}

	private bool BuildingCommandBlurbReorder(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !int.TryParse(command.PopSpeech(), out var index1) || index1 <= 0)
		{
			actor.OutputHandler.Send(
				"You must enter a valid positive integer for the index of the first item you'd like to swap the order of.");
			return false;
		}

		if (command.IsFinished || !int.TryParse(command.PopSpeech(), out var index2) || index2 <= 0)
		{
			actor.OutputHandler.Send(
				"You must enter a valid positive integer for the index of the second item you'd like to swap the order of.");
			return false;
		}

		if (index1 == index2)
		{
			actor.OutputHandler.Send("You can't reorder a blurb to be at the same position as it started.");
			return false;
		}

		if (_blurbs.Count >= index1)
		{
			actor.OutputHandler.Send(
				$"The value {index1.ToString("N0", actor).ColourValue()} for the first index is higher than the number of items ({_blurbs.Count.ToString("N0", actor).ColourValue()}).");
			return false;
		}

		if (_blurbs.Count >= index2)
		{
			actor.OutputHandler.Send(
				$"The value {index2.ToString("N0", actor).ColourValue()} for the second index is higher than the number of items ({_blurbs.Count.ToString("N0", actor).ColourValue()}).");
			return false;
		}

		_blurbs.Swap(index1 - 1, index2 - 1);
		actor.OutputHandler.Send(
			$"You swap the order of the {index1.ToOrdinal().ColourValue()} and {index2.ToOrdinal().ColourValue()} blurbs.");
		Changed = true;
		return true;
	}

	private bool BuildingCommandBlurbAdd(ICharacter actor)
	{
		actor.EditorMode(PostBlurb, CancelBlurb, 1.0, null, EditorOptions.None,
			new object[] { actor.Account.InnerLineFormatLength });
		return true;
	}

	private void CancelBlurb(IOutputHandler handler, object[] args)
	{
		handler.Send("You decide not to add a new blurb to the welcome screen.");
	}

	private void PostBlurb(string text, IOutputHandler handler, object[] args)
	{
		_blurbs.Add(text);
		Changed = true;
		handler.Send($"You add the following new blurb screen:\n\n{text.Wrap((int)args[0]).SubstituteANSIColour()}");
	}

	#endregion
}