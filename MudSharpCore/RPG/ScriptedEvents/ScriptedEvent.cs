using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Handlers;
using Org.BouncyCastle.Asn1.Pkcs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.RPG.ScriptedEvents;
#nullable enable
internal class ScriptedEvent : SaveableItem, IScriptedEvent
{
	public override string FrameworkItemType => "ScriptedEvent";

	private Models.ScriptedEvent CloneEventForAutoAssign(ICharacter? character)
	{
		using (new FMDB())
		{
			var dbitem = new Models.ScriptedEvent
			{
				Name = Name,
				CharacterId = character?.Id,
				IsTemplate = false,
				IsReady = true,
				IsFinished = false,
				EarliestDate = EarliestDate,
				CharacterFilterProgId = null
			};
			FMDB.Context.ScriptedEvents.Add(dbitem);

			foreach (var question in MultipleChoiceQuestions)
			{
				var dbquestion = new Models.ScriptedEventMultipleChoiceQuestion
				{
					ScriptedEvent = dbitem,
					Question = question.Question,
				};
				dbitem.MultipleChoiceQuestions.Add(dbquestion);

				foreach (var answer in question.Answers)
				{
					dbquestion.Answers.Add(new Models.ScriptedEventMultipleChoiceQuestionAnswer
					{
						ScriptedEventMultipleChoiceQuestion = dbquestion,
						DescriptionAfterChoice = answer.DescriptionAfterChoice,
						DescriptionBeforeChoice = answer.DescriptionBeforeChoice,
						AnswerFilterProgId = answer.AnswerFilterProg?.Id,
						AfterChoiceProgId = answer.AfterChoiceProg?.Id
					});
				}
			}

			foreach (var question in FreeTextQuestions)
			{
				dbitem.FreeTextQuestions.Add(new Models.ScriptedEventFreeTextQuestion { ScriptedEvent = dbitem, Question = question.Question });
			}

			return dbitem;
		}
		}

	protected ScriptedEvent(ScriptedEvent rhs, ICharacter? character)
	{
		Gameworld = rhs.Gameworld;
		_name = rhs.Name;
		_characterId = character?.Id;
		_character = character;
		IsTemplate = rhs.IsTemplate && character is null;
		IsReady = false;
		IsFinished = false;
		EarliestDate = DateTime.UtcNow;
		CharacterFilterProg = rhs.CharacterFilterProg;

		using (new FMDB())
		{
			var dbitem = new Models.ScriptedEvent
			{
				Name = Name,
				CharacterId = _characterId,
				IsTemplate = IsTemplate,
				IsReady = IsReady,
				IsFinished= IsFinished,
				EarliestDate = EarliestDate,
				CharacterFilterProgId = CharacterFilterProg?.Id
			};
			FMDB.Context.ScriptedEvents.Add(dbitem);

			foreach (var question in rhs.MultipleChoiceQuestions)
			{
				var dbquestion = new Models.ScriptedEventMultipleChoiceQuestion
				{
					ScriptedEvent = dbitem,
					Question = question.Question,
				};
				dbitem.MultipleChoiceQuestions.Add(dbquestion);

				foreach (var answer in question.Answers)
				{
					dbquestion.Answers.Add(new Models.ScriptedEventMultipleChoiceQuestionAnswer
					{
						ScriptedEventMultipleChoiceQuestion = dbquestion,
						DescriptionAfterChoice = answer.DescriptionAfterChoice,
						DescriptionBeforeChoice = answer.DescriptionBeforeChoice,
						AnswerFilterProgId = answer.AnswerFilterProg?.Id,
						AfterChoiceProgId = answer.AfterChoiceProg?.Id
					});
				}
			}

			foreach (var question in rhs.FreeTextQuestions)
			{
				dbitem.FreeTextQuestions.Add(new Models.ScriptedEventFreeTextQuestion { ScriptedEvent = dbitem, Question = question.Question });
			}

			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
			foreach (var dbquestion in dbitem.MultipleChoiceQuestions)
			{
				_multipleChoiceQuestions.Add(new ScriptedEventMultipleChoiceQuestion(dbquestion, Gameworld));
			}
			foreach (var dbquestion in dbitem.FreeTextQuestions)
			{
				_freeTextQuestions.Add(new ScriptedEventFreeTextQuestion(dbquestion, Gameworld));
			}
		}
	}

	public ScriptedEvent(Models.ScriptedEvent dbitem, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = dbitem.Id;
		_name = dbitem.Name;
		_characterId = dbitem.CharacterId;
		CharacterFilterProg = Gameworld.FutureProgs.Get(dbitem.CharacterFilterProgId ?? 0L);
		IsReady = dbitem.IsReady;
		IsFinished = dbitem.IsFinished;
		IsTemplate = dbitem.IsTemplate;
		EarliestDate = dbitem.EarliestDate;

		foreach (var question in dbitem.FreeTextQuestions)
		{
			_freeTextQuestions.Add(new ScriptedEventFreeTextQuestion(question, Gameworld));
		}

		foreach (var question in dbitem.MultipleChoiceQuestions)
		{
			_multipleChoiceQuestions.Add(new ScriptedEventMultipleChoiceQuestion(question, Gameworld));
		}
	}

	public ScriptedEvent(string name, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_name = name;
		IsReady = false;
		IsTemplate = false;
		IsFinished = false;
		EarliestDate = DateTime.UtcNow;

		using (new FMDB())
		{
			var dbitem = new Models.ScriptedEvent
			{
				Name = name,
				IsFinished = false,
				IsReady = false,
				IsTemplate = false,
				EarliestDate = EarliestDate
			};
			FMDB.Context.ScriptedEvents.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	private long? _characterId;
	private ICharacter? _character;
	public ICharacter? Character
	{
		get
		{
			if (_character is null && _characterId is not null)
			{
				_character = Gameworld.TryGetCharacter(_characterId.Value, true);
			}

			return _character;
		}
	}

	public IFutureProg? CharacterFilterProg { get; private set; }
	public bool IsReady { get; private set; }
	public bool IsFinished { get; private set; }
	public bool IsTemplate { get; private set; }
	public DateTime EarliestDate { get; private set; }

	private readonly List<IScriptedEventFreeTextQuestion> _freeTextQuestions = new();
	public IEnumerable<IScriptedEventFreeTextQuestion> FreeTextQuestions => _freeTextQuestions;

	private readonly List<IScriptedEventMultipleChoiceQuestion> _multipleChoiceQuestions = new();
	public IEnumerable<IScriptedEventMultipleChoiceQuestion> MultipleChoiceQuestions => _multipleChoiceQuestions;

	public IScriptedEvent Assign(ICharacter actor)
	{
		if (!IsTemplate)
		{
			_character = actor;
			_characterId = actor.Id;
			Changed = true;
			return this;
		}

		var item = new ScriptedEvent(this, actor);
		Gameworld.Add(item);
		return item;
	}

	public void SetFinished()
	{
		IsFinished = true;
		using (new FMDB())
		{
			var sb = new StringBuilder();
			sb.AppendLine($"{Character!.PersonalName.GetName(MudSharp.Character.Name.NameStyle.FullName).ColourName()} had the {Name.ColourValue()} offline event.");
			var i = 1;
			foreach (var multi in _multipleChoiceQuestions)
			{
				sb.AppendLine();
				sb.AppendLine($"Question #{i++.ToString("N0", Character)}".GetLineWithTitle(Character.InnerLineFormatLength, Character.Account.UseUnicode, Telnet.Cyan, Telnet.BoldWhite));
				sb.AppendLine();
				sb.AppendLine(multi.Question.SubstituteANSIColour().Wrap(Character.InnerLineFormatLength));
				sb.AppendLine();
				sb.AppendLine($"Answered".GetLineWithTitle(Character.InnerLineFormatLength, Character.Account.UseUnicode, Telnet.BoldCyan, Telnet.BoldWhite));
				sb.AppendLine();
				sb.AppendLine(multi.ChosenAnswer!.DescriptionBeforeChoice.SubstituteANSIColour().Wrap(Character.InnerLineFormatLength));
				sb.AppendLine();
				sb.AppendLine($"Result".GetLineWithTitle(Character.InnerLineFormatLength, Character.Account.UseUnicode, Telnet.BoldCyan, Telnet.BoldWhite));
				sb.AppendLine();
				sb.AppendLine(multi.ChosenAnswer!.DescriptionAfterChoice.SubstituteANSIColour().Wrap(Character.InnerLineFormatLength));
			}

			foreach (var text in _freeTextQuestions)
			{
				sb.AppendLine();
				sb.AppendLine($"Question #{i++.ToString("N0", Character)}".GetLineWithTitle(Character.InnerLineFormatLength, Character.Account.UseUnicode, Telnet.Cyan, Telnet.BoldWhite));
				sb.AppendLine();
				sb.AppendLine(text.Question.SubstituteANSIColour().Wrap(Character.InnerLineFormatLength));
				sb.AppendLine();
				sb.AppendLine($"Answered".GetLineWithTitle(Character.InnerLineFormatLength, Character.Account.UseUnicode, Telnet.BoldCyan, Telnet.BoldWhite));
				sb.AppendLine();
				sb.AppendLine(text.Answer.SubstituteANSIColour().Wrap(Character.InnerLineFormatLength));
			}

			var dbnote = new Models.AccountNote
			{
				AccountId = Character!.Account.Id,
				CharacterId = Character.Id,
				AuthorId = Character.Account.Id,
				IsJournalEntry = true,
				TimeStamp = DateTime.UtcNow,
				Subject = $"[Event] {Name}",
				InGameTimeStamp = (Character.Location?.DateTime() ?? Gameworld.Cells.First().DateTime()).GetDateTimeString(),
				Text = sb.ToString()
			};
			FMDB.Context.AccountNotes.Add(dbnote);
			FMDB.Context.SaveChanges();
		}
		Changed = true;
	}

	public IScriptedEvent Clone()
	{
		var item = new ScriptedEvent(this, null);
		Gameworld.Add(item);
		return item;
	}

	public void Delete()
	{
		Gameworld.SaveManager.Abort(this);
		if (_id != 0)
		{
			using (new FMDB())
			{
				Gameworld.SaveManager.Flush();
				var dbitem = FMDB.Context.ScriptedEvents.Find(Id);
				if (dbitem != null)
				{
					FMDB.Context.ScriptedEvents.Remove(dbitem);
					FMDB.Context.SaveChanges();
				}
			}
		}
	}

	public const string BuildingHelpText = @"You can use the following options with this command:

	#3name <name>#0 - gives a name to this event
	#3earliest <date>#0 - sets the earliest time this event can fire
	#3character <name|id>#0 - sets this event as assigned to a character
	#3ready#0 - toggles this event being ready to be fire
	#3template#0 - toggles this event being a template for other events
	#3filter <prog>#0 - sets a prog as a filter for auto assigning
	#3autoassign#0 - automatically assigned clones of this event to all matching PCs
	#3addfree#0 - drops you into an editor to enter the text of a new free text question
	#3addmulti#0 - drops you into an editor to enter the text of a new multiple choice question
	#3remfree <##>#0 - removes a free text question
	#3remmulti <##>#0 - removes a multiple choice question
	#3free <##>#0 - shows detailed information about a free text question
	#3free <##> question#0 - edits the question text
	#3multi <##>#0 - shows detailed information about a multi choice question
	#3multi <##> question#0 - edits the question text
	#3multi <##> question#0 - edit the question text
	#3multi <##> addanswer#0 - adds a new answer
	#3multi <##> removeanswer <##>#0 - removes an answer
	#3multi <##> answer <##>#0 - shows detailed information about an answer
	#3multi <##> answer <##> before#0 - edits the before text of an answer
	#3multi <##> answer <##> after#0 - edits the after text of an answer
	#3multi <##> answer <##> filter <prog>#0 - edits the filter prog of an answer
	#3multi <##> answer <##> choice <prog>#0 - edits the on choice prog of an answer";

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "name":
				return BuildingCommandName(actor, command);
			case "template":
				return BuildingCommandTemplate(actor);
			case "earliest":
			case "early":
			case "date":
			case "earliestdate":
			case "eventdate":
				return BuildingCommandEarliestDate(actor, command);
			case "ready":
			case "isready":
				return BuildingCommandIsReady(actor);
			case "prog":
			case "filter":
			case "filterprog":
			case "progfilter":
				return BuildingCommandFilterProg(actor, command);
			case "character":
			case "char":
			case "person":
			case "who":
				return BuildingCommandCharacter(actor, command);
			case "assign":
			case "autoassign":
				return BuildingCommandAutoAssign(actor);
			case "addfree":
				return BuildingCommandAddFreeQuestion(actor, command);
			case "addmulti":
				return BuildingCommandAddMultiQuestion(actor, command);
			case "remfree":
			case "delfree":
			case "deletefree":
			case "removefree":
				return BuildingCommandRemoveFreeTextQuestion(actor, command);
			case "remmulti":
			case "delmulti":
			case "removemulti":
			case "deletemulti":
				return BuildingCommandRemoveMultiQuestion(actor, command);
			case "text":
				return BuildingCommandText(actor, command);
			case "multi":
				return BuildingCommandMulti(actor, command);
			default:
				actor.OutputHandler.Send(BuildingHelpText.SubstituteANSIColour());
				break;
		}

		return false;
	}

	private bool BuildingCommandMulti(ICharacter actor, StringStack command)
	{
		if (IsReady)
		{
			actor.OutputHandler.Send("Ready scripted events cannot have their questions edited.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which multiple choice question do you want to edit?");
			return false;
		}

		if (!int.TryParse(command.PopSpeech(), out var index) || index < 1 || index > _multipleChoiceQuestions.Count)
		{
			actor.OutputHandler.Send($"You must enter a valid multiple choice text question number between {1.ToString("N0", actor).ColourValue()} and {_multipleChoiceQuestions.Count.ToString("N0", actor).ColourValue()}.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send(_multipleChoiceQuestions.ElementAt(index - 1).Show(actor));
			return true;
		}

		_multipleChoiceQuestions.ElementAt(index - 1).BuildingCommand(actor, command);
		return true;
	}

	private bool BuildingCommandText(ICharacter actor, StringStack command)
	{
		if (IsReady)
		{
			actor.OutputHandler.Send("Ready scripted events cannot have their questions edited.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which text question do you want to edit?");
			return false;
		}

		if (!int.TryParse(command.PopSpeech(), out var index) || index < 1 || index > _freeTextQuestions.Count)
		{
			actor.OutputHandler.Send($"You must enter a valid free text question number between {1.ToString("N0", actor).ColourValue()} and {_freeTextQuestions.Count.ToString("N0", actor).ColourValue()}.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send(_freeTextQuestions.ElementAt(index - 1).Show(actor));
			return true;
		}
		_freeTextQuestions.ElementAt(index - 1).BuildingCommand(actor, command);
		return true;
	}

	private bool BuildingCommandRemoveMultiQuestion(ICharacter actor, StringStack command)
	{
		if (IsReady)
		{
			actor.OutputHandler.Send("Ready scripted events cannot have their questions edited.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which multiple choice question do you want to remove?");
			return false;
		}

		if (!int.TryParse(command.PopSpeech(), out var index) || index < 1 || index > _multipleChoiceQuestions.Count)
		{
			actor.OutputHandler.Send($"You must enter a valid multiple choice text question number between {1.ToString("N0", actor).ColourValue()} and {_multipleChoiceQuestions.Count.ToString("N0", actor).ColourValue()}.");
			return false;
		}

		var question = _multipleChoiceQuestions.ElementAt(index - 1);
		_multipleChoiceQuestions.Remove(question);
		question.Delete();
		actor.OutputHandler.Send($"You delete the {index.ToOrdinal().ColourValue()} multiple choice question.");
		return true;
	}

	private bool BuildingCommandAddFreeQuestion(ICharacter actor, StringStack command)
	{
		if (IsReady)
		{
			actor.OutputHandler.Send("Ready scripted events cannot have their questions edited.");
			return false;
		}

		actor.OutputHandler.Send("Enter the text that will be shown to the player in the editor below.");
		actor.EditorMode(AddFreeQuestionPost, AddFreeQuestionCancel, 1.0, suppliedArguments: new object[] { actor });
		return true;
	}

	private void AddFreeQuestionCancel(IOutputHandler handler, object[] args)
	{
		handler.Send("You decide not to create a new question.");
	}

	private void AddFreeQuestionPost(string text, IOutputHandler handler, object[] args)
	{
		var newQuestion = new ScriptedEventFreeTextQuestion(this, text);
		_freeTextQuestions.Add(newQuestion);
		handler.Send($"You add a new free text question at position {_freeTextQuestions.Count.ToString("N0", ((ICharacter)args[0])).ColourValue()}.");
	}

	private bool BuildingCommandAddMultiQuestion(ICharacter actor, StringStack command)
	{
		if (IsReady)
		{
			actor.OutputHandler.Send("Ready scripted events cannot have their questions edited.");
			return false;
		}

		actor.OutputHandler.Send("Enter the text that will be shown to the player in the editor below.");
		actor.EditorMode(AddMultiQuestionPost, AddMultiQuestionCancel, 1.0, suppliedArguments: new object[] { actor });
		return true;
	}

	private void AddMultiQuestionCancel(IOutputHandler handler, object[] args)
	{
		handler.Send("You decide not to create a new question.");
	}

	private void AddMultiQuestionPost(string text, IOutputHandler handler, object[] args)
	{
		var newQuestion = new ScriptedEventMultipleChoiceQuestion(this, text);
		_multipleChoiceQuestions.Add(newQuestion);
		handler.Send($"You add a new multiple choice question at position {_multipleChoiceQuestions.Count.ToString("N0", ((ICharacter)args[0])).ColourValue()}.");
	}

	private bool BuildingCommandRemoveFreeTextQuestion(ICharacter actor, StringStack command)
	{
		if (IsReady)
		{
			actor.OutputHandler.Send("Ready scripted events cannot have their questions edited.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which free text question do you want to remove?");
			return false;
		}

		if (!int.TryParse(command.PopSpeech(), out var index) || index < 1 || index > _freeTextQuestions.Count)
		{
			actor.OutputHandler.Send($"You must enter a valid free text text question number between {1.ToString("N0", actor).ColourValue()} and {_freeTextQuestions.Count.ToString("N0", actor).ColourValue()}.");
			return false;
		}

		var question = _freeTextQuestions.ElementAt(index - 1);
		_freeTextQuestions.Remove(question);
		question.Delete();
		actor.OutputHandler.Send($"You delete the {index.ToOrdinal().ColourValue()} free text question.");
		return true;
	}

	private bool BuildingCommandAutoAssign(ICharacter actor)
	{
		if (!IsTemplate)
		{
			actor.OutputHandler.Send("Only templates can be auto assigned.");
			return false;
		}

		if (CharacterFilterProg is null)
		{
			actor.OutputHandler.Send("You must set a valid filter prog before auto assigning this scripted event template.");
			return false;
		}

		if (!FreeTextQuestions.Any() && !MultipleChoiceQuestions.Any())
		{
			actor.OutputHandler.Send("You must have at least one free text or multiple choice question before auto assigning this template.");
			return false;
		}

		// Make sure all the offline characters are loaded
		var loadedPCs = new List<ICharacter>();
		var onlinePCIDs = actor.Gameworld.Characters.Select(x => x.Id).ToHashSet();
		using (new FMDB())
		{
			var PCsToLoad =
				FMDB.Context.Characters.Where(
						x => !x.NpcsCharacter.Any() && x.Guest == null && !onlinePCIDs.Contains(x.Id) &&
							 (x.Status == (int)CharacterStatus.Active ||
							  x.Status == (int)CharacterStatus.Retired))
					.OrderBy(x => x.Id);
			var i = 0;
			while (true)
			{
				var any = false;
				foreach (var pc in PCsToLoad.Skip(i++ * 10).Take(10).ToList())
				{
					any = true;
					var character = actor.Gameworld.TryGetCharacter(pc.Id, true);
					character.Register(new NonPlayerOutputHandler());
					loadedPCs.Add(character);
					actor.Gameworld.Add(character, false);
				}

				if (!any)
				{
					break;
				}
			}
		}

		var targets = actor.Gameworld.Characters.Where(x => CharacterFilterProg.Execute<bool?>(x) == true).ToList();
		if (!targets.Any())
		{
			actor.OutputHandler.Send("The character filter prog did not return any valid targets.");
			return false;
		}

		var sb = new StringBuilder();
		sb.AppendLine($"The following characters will have this scripted event assigned:");
		sb.AppendLine();
		sb.AppendLine(StringUtilities.GetTextTable(
			from ch in targets select new List<string>
			{
				ch.Id.ToString("N0", actor),
				ch.PersonalName.GetName(MudSharp.Character.Name.NameStyle.FullWithNickname),
				ch.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreCanSee | PerceiveIgnoreFlags.IgnoreSelf | PerceiveIgnoreFlags.IgnoreLoadThings)
			},
			new List<string>
			{
				"Id",
				"Name",
				"Description"
			},
			actor,
			Telnet.Green
		));
		sb.AppendLine($"Are you sure you want to proceed?");
		sb.AppendLine(Accept.StandardAcceptPhrasing);

		actor.OutputHandler.Send(sb.ToString());
		actor.AddEffect(new Accept(actor, new GenericProposal 
		{ 
			AcceptAction = text => {
				using (new FMDB())
				{
					var events = new List<Models.ScriptedEvent>();
					foreach (var ch in targets)
					{
						events.Add(CloneEventForAutoAssign(ch));
					}
					FMDB.Context.SaveChanges();
					foreach (var newEvent in events)
					{
						Gameworld.Add(new ScriptedEvent(newEvent, Gameworld));
					}
				}

				actor.OutputHandler.Send($"Successfully assigned the scripted event to {targets.Count.ToString("N0", actor).ColourValue()} {"character".Pluralise(targets.Count != 1)}.");
			},
			RejectAction = text => {
				actor.OutputHandler.Send("You decide not to apply the scripted event to any characters.");
			},
			ExpireAction = () => {
				actor.OutputHandler.Send("You decide not to apply the scripted event to any characters.");
			},
			DescriptionString = "Applying a scripted event to characters"
		}), TimeSpan.FromSeconds(120));
		return true;
	}

	private bool BuildingCommandCharacter(ICharacter actor, StringStack command)
	{
		if (IsTemplate)
		{
			actor.OutputHandler.Send("Templates cannot be assigned to characters. Clone the scripted event and assign that to someone instead.");
			return false;
		}

		if (IsFinished)
		{
			actor.OutputHandler.Send("You can't reassign finished scripted events.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which character do you want to assign this scripted event to?");
			return false;
		}

		ICharacter? target;
		if (long.TryParse(command.SafeRemainingArgument, out var id))
		{
			target = Gameworld.TryGetCharacter(id, true);
			if (target is null)
			{
				actor.OutputHandler.Send("There is no such character with that id.");
				return false;
			}
		}
		else
		{
			target = Gameworld.TryPlayerCharacterByName(command.SafeRemainingArgument);
			if (target is null)
			{
				actor.OutputHandler.Send("There is no such character by that name.");
				return false;
			}
		}

		if (target.Status.In(Accounts.CharacterStatus.Suspended, Accounts.CharacterStatus.Deceased))
		{
			actor.OutputHandler.Send("You can't assign scripted events to suspended or deceased characters.");
			return false;
		}

		_character = target;
		_characterId = target.Id;
		Changed = true;
		actor.OutputHandler.Send($"This scripted event is now assigned to {target.PersonalName.GetName(MudSharp.Character.Name.NameStyle.FullWithNickname).ColourName()} ({target.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreCanSee | PerceiveIgnoreFlags.IgnoreSelf)}).");
		return true;
	}

	private bool BuildingCommandFilterProg(ICharacter actor, StringStack command)
	{
		if (!IsTemplate)
		{
			actor.OutputHandler.Send("Only template scripted events may have a filter prog assigned.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog should be used to automatically assign this scripted event to characters?");
			return false;
		}

		var prog = new FutureProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument, FutureProgVariableTypes.Boolean, new List<FutureProgVariableTypes> { FutureProgVariableTypes.Character }).LookupProg();
		if (prog is null)
		{
			return false;
		}

		CharacterFilterProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This scripted event template will now use the {prog.MXPClickableFunctionName()} prog to assign to characters.");
		return true;
	}

	private bool BuildingCommandIsReady(ICharacter actor)
	{
		if (IsReady)
		{
			if (IsFinished && !IsTemplate)
			{
				actor.OutputHandler.Send("You cannot unready a finished non-template scripted event.");
				return false;
			}

			IsReady = false;
			Changed = true;
			actor.OutputHandler.Send("This scripted event is no longer ready.");
			return true;
		}

		if (IsTemplate)
		{
			actor.OutputHandler.Send("You cannot make templates ready. Clone a scripted event and assign it to a character.");
			return false;
		}

		if (Character is null)
		{
			actor.OutputHandler.Send("You must first assign the scripted event to a character.");
			return false;
		}

		if (!FreeTextQuestions.Any() && !MultipleChoiceQuestions.Any())
		{
			actor.OutputHandler.Send("You must have at least one question per scripted event.");
			return false;
		}

		if (MultipleChoiceQuestions.Any(x => !x.Answers.Any()))
		{
			actor.OutputHandler.Send("All multiple choice questions need to have at least one answer.");
			return false;
		}

		IsReady = true;
		Changed = true;
		actor.OutputHandler.Send("This scripted event is now ready and will be triggered when appropriate.");
		return true;
	}

	private bool BuildingCommandEarliestDate(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What date should this scripted event wait for before executing?");
			return false;
		}

		if (!DateTime.TryParse(command.SafeRemainingArgument, out var dt))
		{
			actor.OutputHandler.Send("That is not a valid date.");
			return false;
		}

		var now = DateTime.UtcNow;
		dt = dt.ToUniversalTime();
		if (dt < now)
		{
			dt = now;
		}

		EarliestDate = dt;
		Changed = true;
		actor.OutputHandler.Send($"This scripted event will no fire no earlier than {(dt == now ? "right now".ColourValue() : dt.GetLocalDateString(actor, true).ColourValue())}.");
		return true;
	}

	private bool BuildingCommandTemplate(ICharacter actor)
	{
		if (IsTemplate)
		{
			IsTemplate = false;
			Changed = true;
			actor.OutputHandler.Send($"This scripted event is no longer a template.");
			return true;
		}

		if (Character is not null)
		{
			var newTemplate = new ScriptedEvent(this, null);
			newTemplate.IsTemplate = true;
			Gameworld.Add(newTemplate);
			actor.RemoveAllEffects<BuilderEditingEffect<IScriptedEvent>>();
			actor.AddEffect(new BuilderEditingEffect<IScriptedEvent>(actor) { EditingItem = newTemplate });
			actor.OutputHandler.Send($"You create a new scripted event template from the scripted event you were editing, with new ID #{newTemplate.Id.ToString("N0", actor)}. You are now editing the new scripted event.");
			return true;
		}

		IsTemplate = true;
		Changed = true;
		actor.OutputHandler.Send($"This scripted event is now a template.");
		return true;
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give to this scripted event?");
			return false;
		}

		_name = command.SafeRemainingArgument.TitleCase();
		Changed = true;
		actor.OutputHandler.Send($"You rename this scripted event to {Name.ColourName()}.");
		return true;
	}

	public string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Scripted Event #{Id.ToString("N0", actor)} - {Name}".GetLineWithTitle(actor, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine($"Is Template: {IsTemplate.ToColouredString()}");
		sb.AppendLine($"Ready For Use: {IsReady.ToColouredString()}");
		sb.AppendLine($"Is Finished: {IsFinished.ToColouredString()}");
		sb.AppendLine($"Earliest Date: {EarliestDate.GetLocalDateString(actor, true).ColourValue()}");
		sb.AppendLine($"Character Filter Prog: {CharacterFilterProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		if (!IsTemplate)
		{
			if (Character is null)
			{
				sb.AppendLine($"Assigned Character: {"None".ColourError()}");
			}
			else
			{
				sb.AppendLine($"Assigned Character: {Character.PersonalName.GetName(MudSharp.Character.Name.NameStyle.FullName).ColourName()} ({Character.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreCanSee | PerceiveIgnoreFlags.IgnoreSelf)}) [#{Character.Id.ToString("N0", actor)}]");
			}			
		}

		sb.AppendLine();
		sb.AppendLine("Multiple Choice Questions".GetLineWithTitle(actor, Telnet.Cyan, Telnet.BoldWhite));
		
		if (MultipleChoiceQuestions.Any())
		{
			var i = 1;
			foreach (var question in MultipleChoiceQuestions)
			{
				sb.AppendLine();
				sb.AppendLine($"Question #{i++.ToString("N0", actor)}".GetLineWithTitle(actor, Telnet.BoldCyan, Telnet.BoldWhite));
				sb.AppendLine();
				sb.AppendLine($"{question.Question.SubstituteANSIColour().Wrap(actor.InnerLineFormatLength)}");
				sb.AppendLine();
				sb.AppendLine(StringUtilities.GetTextTable(
					from item in question.Answers
					select new List<string>
					{
						item.Id.ToString("N0", actor),
						item.AnswerFilterProg?.MXPClickableFunctionName() ?? "",
						item.AfterChoiceProg?.MXPClickableFunctionName() ?? "",
						item.DescriptionBeforeChoice,
						(question.ChosenAnswer == item).ToColouredString()
					},
					new List<string>
					{
						"Id",
						"Filter",
						"Prog",
						"Before Desc",
						"Chosen?"
					},
					actor,
					Telnet.BoldCyan,
					3
				));
			}
		}
		else
		{
			sb.AppendLine();
			sb.AppendLine("\tNone");
		}
		sb.AppendLine();
		sb.AppendLine("Free Text Questions".GetLineWithTitle(actor, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine();
		if (FreeTextQuestions.Any())
		{
			var i = 1;
			foreach (var question in FreeTextQuestions)
			{
				sb.AppendLine();
				sb.AppendLine($"Question #{i++.ToString("N0", actor)}".GetLineWithTitle(actor, Telnet.BoldCyan, Telnet.BoldWhite));
				sb.AppendLine();
				sb.AppendLine($"{question.Question.SubstituteANSIColour().Wrap(actor.InnerLineFormatLength)}");
				if (!IsTemplate)
				{
					sb.AppendLine();
					if (!string.IsNullOrEmpty(question.Answer))
					{
						sb.AppendLine("Answer");
						sb.AppendLine();
						sb.AppendLine($"{question.Answer.SubstituteANSIColour().Wrap(actor.InnerLineFormatLength)}");
					}
					else
					{
						sb.AppendLine($"Not Yet Answered".ColourCommand());
					}
				}
			}
		}
		else
		{
			sb.AppendLine("\tNone");
		}
		return sb.ToString();
	}

	public override void Save()
	{
		var dbitem = FMDB.Context.ScriptedEvents.Find(Id);
		if (dbitem is null)
		{
			throw new ApplicationException($"ScriptedEvent {Id:N0} Named \"{Name}\" could not find itself in the database during save.");
		}
		dbitem.Name = Name;
		dbitem.IsReady = IsReady;
		dbitem.IsFinished = IsFinished;
		dbitem.IsTemplate = IsTemplate;
		dbitem.CharacterId = _characterId;
		dbitem.CharacterFilterProgId = CharacterFilterProg?.Id;
		dbitem.EarliestDate = EarliestDate;
		Changed = false;
	}
}
