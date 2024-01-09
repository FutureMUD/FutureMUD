using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Editor;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;

namespace MudSharp.CharacterCreation.Screens;

public class NotePickerScreenStoryboard : ChargenScreenStoryboard
{
	private NotePickerScreenStoryboard()
	{
	}

	public NotePickerScreenStoryboard(IFuturemud gameworld, Models.ChargenScreenStoryboard dbitem)
		: base(dbitem, gameworld)
	{
		var definition = XElement.Parse(dbitem.StageDefinition);
		var notes = definition
		            .Elements("Note")
		            .Select(note => (
			            note.Attribute("Name").Value,
			            note.Element("Blurb")?.Value,
			            int.TryParse(note.Element("Prog")?.Value, out var value)
				            ? Gameworld.FutureProgs.Get(value)
				            : Gameworld.FutureProgs.FirstOrDefault(
					            x =>
						            x.FunctionName.Equals(note.Element("Prog").Value,
							            StringComparison.InvariantCultureIgnoreCase))
		            )).ToList();
		NotesBlurbs = notes;
	}

	protected override string StoryboardName => "NotePicker";

	public List<(string Name, string Blurb, IFutureProg Prog)> NotesBlurbs { get; protected set; }

	public override ChargenStage Stage => ChargenStage.SelectNotes;

	public override string HelpText => $@"{BaseHelpText}
	#3note add <name> <prog>#0 - drops you into an editor to add a new note type
	#3note remove <name>#0 - removes a note type
	#3note rename <name> <newname>#0 - renames a note type
	#3note blurb <name>#0 - drops you into an editor to edit an existing note type
	#3note prog <name> <prog>#0 - changes the prog controlling a note type";

	/// <inheritdoc />
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "note":
				switch (command.PopForSwitch())
				{
					case "add":
						return BuildingCommandNoteAdd(actor, command);
					case "remove":
					case "rem":
						return BuildingCommandNoteRemove(actor, command);
					case "rename":
					case "name":
						return BuildingCommandNoteRename(actor, command);
					case "blurb":
						return BuildingCommandNoteBlurb(actor, command);
					case "prog":
						return BuildingCommandNoteProg(actor, command);
					default:
						actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
						return false;
				}
		}

		return BuildingCommandFallback(actor, command.GetUndo());
	}

	private bool BuildingCommandNoteAdd(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What name should the new note entry have?");
			return false;
		}

		var name = command.PopSpeech().TitleCase();
		if (NotesBlurbs.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send(
				$"There is already a note entry with the name {name.ColourName()}. Names must be unique.");
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog should control whether this note entry is required?");
			return false;
		}

		var prog = new FutureProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			FutureProgVariableTypes.Boolean, new List<FutureProgVariableTypes>
			{
				FutureProgVariableTypes.Chargen
			}).LookupProg();
		if (prog is null)
		{
			return false;
		}

		actor.OutputHandler.Send("Enter your blurb for the new note below:\n");
		actor.EditorMode(PostAddNote, CancelAddNote, 1.0, null, EditorOptions.None, new object[]
		{
			actor.InnerLineFormatLength,
			name,
			prog
		});
		throw new NotImplementedException();
	}

	private void CancelAddNote(IOutputHandler handler, object[] args)
	{
		var which = (string)args[1];
		handler.Send($"You decide not to change the blurb note entry {which.ColourName()} for this chargen screen.");
	}

	private void PostAddNote(string text, IOutputHandler handler, object[] args)
	{
		var which = (string)args[1];
		if (NotesBlurbs.Any(x => x.Name.EqualTo(which)))
		{
			handler.Send(
				$"A note entry called {which.ColourName()} has been added. Names must be unique, so your entry has been cancelled.");
			return;
		}

		var prog = (IFutureProg)args[2];
		NotesBlurbs.Add((which, text, prog));
		Changed = true;
		handler.Send(
			$"You add note entry {which.ColourName()}, controlled by prog {prog.MXPClickableFunctionName()}, with the following blurb:\n\n{text.Wrap((int)args[0]).SubstituteANSIColour()}");
	}

	private bool BuildingCommandNoteRemove(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which note would you like to remove?");
			return false;
		}

		var name = command.SafeRemainingArgument;
		var note = NotesBlurbs.FirstOrDefault(x => x.Name.EqualTo(name));
		if (note == default)
		{
			actor.OutputHandler.Send("There is no such note.");
			return false;
		}

		actor.OutputHandler.Send(
			$"Are you sure you want to permanently delete the requirement for players to enter the {note.Name.ColourName()} blurb entry? This cannot be undone.\n{Accept.StandardAcceptPhrasing}");
		actor.AddEffect(new Accept(actor, new GenericProposal
		{
			DescriptionString = "Removing a note entry requirement",
			AcceptAction = text =>
			{
				NotesBlurbs.Remove(note);
				Changed = true;
				actor.OutputHandler.Send(
					$"You remove the requirement to enter the {note.Name.ColourName()} blurb entry in character creation.");
			},
			RejectAction =
				text => { actor.OutputHandler.Send("You decide not to delete the note entry requirement."); },
			ExpireAction = () => { actor.OutputHandler.Send("You decide not to delete the note entry requirement."); }
		}), TimeSpan.FromSeconds(120));
		return true;
	}

	private bool BuildingCommandNoteRename(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which note would you like to rename?");
			return false;
		}

		var name = command.PopSpeech();
		var note = NotesBlurbs.FirstOrDefault(x => x.Name.EqualTo(name));
		if (note == default)
		{
			actor.OutputHandler.Send("There is no such note.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Which new name would you like to give to the {note.Name.ColourName()} note entry?");
			return false;
		}

		var newName = command.SafeRemainingArgument.TitleCase();
		if (NotesBlurbs.Any(x => x.Name.EqualTo(newName)))
		{
			actor.OutputHandler.Send(
				$"There is already a note entry called {newName.ColourName()}. Names must be unique.");
			return false;
		}

		var oldName = note.Name;
		NotesBlurbs[NotesBlurbs.IndexOf(note)] = (newName, note.Blurb, note.Prog);
		Changed = true;
		actor.OutputHandler.Send($"You rename the note entry {oldName.ColourName()} to {newName.ColourName()}.");
		return true;
	}

	private bool BuildingCommandNoteBlurb(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which note would you like to change the blurb of?");
			return false;
		}

		var name = command.SafeRemainingArgument;
		var note = NotesBlurbs.FirstOrDefault(x => x.Name.EqualTo(name));
		if (note == default)
		{
			actor.OutputHandler.Send("There is no such note.");
			return false;
		}

		actor.OutputHandler.Send(
			$"Replacing the following text:\n\n{note.Blurb.SubstituteANSIColour().Wrap(actor.InnerLineFormatLength)}\n\nEnter your new blurb below:\n");
		actor.EditorMode(PostBlurb, CancelBlurb, 1.0, note.Blurb, EditorOptions.None,
			new object[] { actor.Account.InnerLineFormatLength, note.Name });
		return true;
	}

	private void CancelBlurb(IOutputHandler handler, object[] args)
	{
		var which = (string)args[1];
		handler.Send($"You decide not to change the blurb note entry {which.ColourName()} for this chargen screen.");
	}

	private void PostBlurb(string text, IOutputHandler handler, object[] args)
	{
		var which = (string)args[1];
		var note = NotesBlurbs.FirstOrDefault(x => x.Name.EqualTo(which));
		if (note == default)
		{
			handler.Send("The note you were editing has been renamed or deleted.");
			return;
		}

		NotesBlurbs[NotesBlurbs.IndexOf(note)] = (note.Name, text, note.Prog);
		Changed = true;
		handler.Send(
			$"You set the blurb for note entry {which.ColourName()} to the following:\n\n{text.Wrap((int)args[0]).SubstituteANSIColour()}");
	}

	private bool BuildingCommandNoteProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which note would you like to change the prog for?");
			return false;
		}

		var name = command.PopSpeech();
		var note = NotesBlurbs.FirstOrDefault(x => x.Name.EqualTo(name));
		if (note == default)
		{
			actor.OutputHandler.Send("There is no such note.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog should control the requirement to enter this note?");
			return false;
		}

		var prog = new FutureProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			FutureProgVariableTypes.Boolean, new List<FutureProgVariableTypes>
			{
				FutureProgVariableTypes.Chargen
			}).LookupProg();
		if (prog is null)
		{
			return false;
		}

		NotesBlurbs[NotesBlurbs.IndexOf(note)] = (note.Name, note.Blurb, prog);
		Changed = true;
		actor.OutputHandler.Send(
			$"The note entry {note.Name.ColourName()} will now use the {prog.MXPClickableFunctionName()} to control whether it needs to be entered.");
		return true;
	}

	#region Overrides of ChargenScreenStoryboard

	/// <inheritdoc />
	protected override string SaveDefinition()
	{
		return new XElement("Definition",
			from note in NotesBlurbs
			select new XElement("Note",
				new XAttribute("Name", note.Name),
				new XElement("Blurb", new XCData(note.Blurb)),
				new XElement("Prog", note.Prog?.Id ?? 0)
			)
		).ToString();
	}

	#endregion

	public static void RegisterFactory()
	{
		ChargenStoryboard.RegisterFactory(ChargenStage.SelectNotes,
			new ChargenScreenStoryboardFactory("NotePicker",
				(game, dbitem) => new NotePickerScreenStoryboard(game, dbitem)),
			"NotePicker",
			"Enter text notes about your character",
			((ChargenScreenStoryboard)Activator.CreateInstance(MethodBase.GetCurrentMethod().DeclaringType, true))
			.HelpText);
	}

	public override IChargenScreen GetScreen(IChargen chargen)
	{
		return new NotePickerScreen(chargen, this);
	}

	public override string Show(ICharacter voyeur)
	{
		var sb = new StringBuilder();
		sb.Append(ShowHeader(voyeur));
		sb.AppendLine();
		sb.AppendLine(
			"This screen can be configured to require people to enter free-text notes regarding topics such as background, personality, role-specific details etc."
				.Wrap(voyeur.InnerLineFormatLength).ColourCommand());
		sb.AppendLine();
		if (!NotesBlurbs.Any())
		{
			sb.AppendLine("\tThere are no note types configured.".ColourError());
		}
		else
		{
			foreach (var (name, blurb, prog) in NotesBlurbs)
			{
				sb.AppendLine();
				sb.AppendLine(name.GetLineWithTitle(voyeur, Telnet.Cyan, Telnet.BoldWhite));
				sb.AppendLine();
				sb.AppendLine($"Filter Prog: {prog.MXPClickableFunctionName() ?? "None".ColourError()}");
				sb.AppendLine();
				sb.AppendLine(blurb.Wrap(voyeur.InnerLineFormatLength).SubstituteANSIColour());
			}
		}

		return sb.ToString();
	}

	internal class NotePickerScreen : ChargenScreen
	{
		private readonly IEnumerator<(string Name, string Blurb, IFutureProg Prog)> Enumerator;
		private readonly List<Tuple<string, string>> SelectedNotes = new();
		private readonly NotePickerScreenStoryboard Storyboard;

		internal NotePickerScreen(IChargen chargen, NotePickerScreenStoryboard storyboard)
			: base(chargen, storyboard)
		{
			Storyboard = storyboard;
			Enumerator =
				Storyboard.NotesBlurbs.Where(x => (bool?)x.Prog?.Execute(Chargen) ?? true)
				          .ToList()
				          .GetEnumerator();
			if (!Enumerator.MoveNext())
			{
				State = ChargenScreenState.Complete;
			}
		}

		public override ChargenStage AssociatedStage => ChargenStage.SelectNotes;

		public override string Display()
		{
			if (Enumerator.Current == default)
			{
				return "";
			}

			Chargen.SetEditor(new EditorController(Chargen.Menu, null, PostNote, null, EditorOptions.DenyCancel));
			return
				string.Format(
					"{2} Entry\n\n{0}\n\nEnter your {1} in the editor below.\n{3}\n",
					Enumerator.Current.Blurb.SubstituteANSIColour().Wrap(Account.InnerLineFormatLength),
					Enumerator.Current.Name,
					Enumerator.Current.Name.Colour(Telnet.Cyan), 
					"You are now entering an editor, use @ on a blank line to exit and *help to see help.".Colour(Telnet.Yellow)
				);
		}

		public override string HandleCommand(string command)
		{
			return Display();
		}

		private void PostNote(string note, IOutputHandler handler, object[] arguments)
		{
			SelectedNotes.Add(Tuple.Create(Enumerator.Current.Item1, note));
			handler.Send(
				$"Your submitted {Enumerator.Current.Item1} is:\n\n{note.Wrap(Account.InnerLineFormatLength, "\t").NoWrap()}");

			if (!Enumerator.MoveNext())
			{
				State = ChargenScreenState.Complete;
				Chargen.SelectedNotes = SelectedNotes;
				return;
			}

			handler.Send(Display());
		}
	}
}