using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.PerceptionEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.RPG.ScriptedEvents
{
	internal class ScriptedEventMultipleChoiceQuestion : SaveableItem, IScriptedEventMultipleChoiceQuestion
	{
		public ScriptedEventMultipleChoiceQuestion(Models.ScriptedEventMultipleChoiceQuestion question, IFuturemud gameworld)
		{
			Gameworld = gameworld;
			_id = question.Id;
			_name = "Question";
			Question = question.Question;
			foreach (var dbanswer in question.Answers)
			{
				var answer = new ScriptedEventMultipleChoiceQuestionAnswer(dbanswer, gameworld);
				_answers.Add(answer);
				if (question.ChosenAnswerId == answer.Id)
				{
					ChosenAnswer = answer;
				}
			}
		}

        public ScriptedEventMultipleChoiceQuestion(IScriptedEvent scriptedEvent, string question)
        {
			Gameworld = scriptedEvent.Gameworld;
			_name = "Question";
			Question = question;
			using (new FMDB())
			{
				var dbitem = new Models.ScriptedEventMultipleChoiceQuestion
				{
					Question = question,
					ScriptedEventId = scriptedEvent.Id,
				};
				FMDB.Context.ScriptedEventMultipleChoiceQuestions.Add(dbitem);
				FMDB.Context.SaveChanges();
				_id = dbitem.Id;
			}
		}

        public void Delete()
		{
			Gameworld.SaveManager.Abort(this);
			if (_id != 0)
			{
				using (new FMDB())
				{
					Gameworld.SaveManager.Flush();
					var dbitem = FMDB.Context.ScriptedEventMultipleChoiceQuestions.Find(Id);
					if (dbitem != null)
					{
						FMDB.Context.ScriptedEventMultipleChoiceQuestions.Remove(dbitem);
						FMDB.Context.SaveChanges();
					}
				}
			}
		}

		public string Question { get; private set; }
		public IScriptedEventMultipleChoiceQuestionAnswer ChosenAnswer { get; private set; }
		private readonly List<IScriptedEventMultipleChoiceQuestionAnswer> _answers = new();
		public IEnumerable<IScriptedEventMultipleChoiceQuestionAnswer> Answers => _answers;

		public bool BuildingCommand(ICharacter actor, StringStack command)
		{
			switch (command.PopSpeech().ToLowerInvariant().CollapseString())
			{
				case "question":
					return BuildingCommandQuestion(actor);
				case "addanswer":
					return BuildingCommandAddAnswer(actor, command);
				case "deleteanswer":
				case "removeanswer":
				case "delanswer":
				case "remanswer":
					return BuildingCommandRemoveAnswer(actor, command);
				case "answer":
					return BuildingCommandAnswer(actor, command);
				default:
					actor.OutputHandler.Send(@"You can use the following options with this subcommand:

	#3question#0 - edit the question text
	#3addanswer#0 - adds a new answer
	#3removeanswer <##>#0 - removes an answer
	#3answer <##>#0 - shows detailed information about an answer
	#3answer <##> before#0 - edits the before text of an answer
	#3answer <##> after#0 - edits the after text of an answer
	#3answer <##> filter <prog>#0 - edits the filter prog of an answer
	#3answer <##> choice <prog>#0 - edits the on choice prog of an answer".SubstituteANSIColour());
					return false;
			}
		}

		private bool BuildingCommandAnswer(ICharacter actor, StringStack command)
		{
			if (command.IsFinished)
			{
				actor.OutputHandler.Send("Which answer do you want to edit?");
				return false;
			}

			if (!int.TryParse(command.PopSpeech(), out var index) || index < 1 || index > _answers.Count)
			{
				actor.OutputHandler.Send($"You must enter a valid answer number between {1.ToString("N0", actor).ColourValue()} and {_answers.Count.ToString("N0", actor).ColourValue()}.");
				return false;
			}

			if (command.IsFinished)
			{
				_answers.ElementAt(index - 1).Show(actor);
				return true;
			}
			_answers.ElementAt(index - 1).BuildingCommand(actor, command);
			return true;
		}

		private bool BuildingCommandRemoveAnswer(ICharacter actor, StringStack command)
		{
			if (command.IsFinished)
			{
				actor.OutputHandler.Send("Which answer do you want to remove?");
				return false;
			}

			if (!int.TryParse(command.PopSpeech(), out var index) || index < 1 || index > _answers.Count)
			{
				actor.OutputHandler.Send($"You must enter a valid answer question number between {1.ToString("N0", actor).ColourValue()} and {_answers.Count.ToString("N0", actor).ColourValue()}.");
				return false;
			}

			var answer = _answers.ElementAt(index - 1);
			_answers.Remove(answer);
			answer.Delete();
			actor.OutputHandler.Send($"You delete the {index.ToOrdinal().ColourValue()} answer.");
			return true;
		}

		private bool BuildingCommandAddAnswer(ICharacter actor, StringStack command)
		{
			actor.OutputHandler.Send("Enter the text shown to the character before they have made their choice below:");
			actor.EditorMode(AddAnswerPost, AddAnswerCancel, 1.0, suppliedArguments: new[] { actor } );
			return true;
		}

		private void AddAnswerPost(string text, IOutputHandler handler, object[] args)
		{
			var actor = (ICharacter)args[0];
			actor.OutputHandler.Send("Enter the text shown to the character after they have locked in their choice below:");
			actor.EditorMode(AddAnswerPostSecond, AddAnswerCancel, 1.0, suppliedArguments: new object[] { actor, text });
		}

		private void AddAnswerPostSecond(string text, IOutputHandler handler, object[] args)
		{
			var actor = (ICharacter)args[0];
			var before = (string)args[1];
			var newAnswer = new ScriptedEventMultipleChoiceQuestionAnswer(before, text, this);
			_answers.Add(newAnswer);
			actor.OutputHandler.Send($"You create a new answer at position {_answers.Count.ToString("N0", actor).ColourValue()}.");
		}

		private void AddAnswerCancel(IOutputHandler handler, object[] arg2)
		{
			handler.Send("You decide not to create a new answer.");
		}

		private bool BuildingCommandQuestion(ICharacter actor)
		{
			actor.OutputHandler.Send($"Replacing:\n\n{Question.SubstituteANSIColour().Wrap(actor.InnerLineFormatLength)}\n\nEnter the question in the editor below:");
			actor.EditorMode(QuestionPost, QuestionCancel, 1.0);
			return true;
		}

		private void QuestionCancel(IOutputHandler handler, object[] arg2)
		{
			handler.Send("You decide not to change the question.");
		}

		private void QuestionPost(string text, IOutputHandler handler, object[] arg3)
		{
			Question = text;
			Changed = true;
			handler.Send("You change the question text.");
		}

		public string Show(ICharacter actor)
		{
			var sb = new StringBuilder();
			sb.AppendLine($"Question #{Id.ToString("N0", actor)}".GetLineWithTitle(actor, Telnet.Blue, Telnet.BoldWhite));
			sb.AppendLine();
			sb.AppendLine("Question:");
			sb.AppendLine();
			sb.AppendLine(Question.SubstituteANSIColour().Wrap(actor.InnerLineFormatLength, "\t"));
			sb.AppendLine();
			foreach (var answer in Answers)
			{
				sb.AppendLine($"Answer #{answer.Id.ToString("N0", actor)}".GetLineWithTitle(actor, Telnet.Blue, Telnet.BoldWhite));
				sb.AppendLine();
				sb.AppendLine($"Filter Prog: {answer.AnswerFilterProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
				sb.AppendLine($"On Choice Prog: {answer.AfterChoiceProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
				sb.AppendLine($"Is Chosen Answer: {(ChosenAnswer == answer).ToColouredString()}");
				sb.AppendLine();
				sb.AppendLine($"Description Before Choice:");
				sb.AppendLine();
				sb.AppendLine(answer.DescriptionBeforeChoice.SubstituteANSIColour().Wrap(actor.InnerLineFormatLength, "\t"));
				sb.AppendLine();
				sb.AppendLine($"Description After Choice:");
				sb.AppendLine();
				sb.AppendLine(answer.DescriptionAfterChoice.SubstituteANSIColour().Wrap(actor.InnerLineFormatLength, "\t"));
			}
			return sb.ToString();
		}

		public override void Save()
		{
			var dbitem = FMDB.Context.ScriptedEventMultipleChoiceQuestions.Find(Id);
			dbitem.Question = Question;
			dbitem.ChosenAnswerId = ChosenAnswer?.Id;
			Changed = false;
		}

		public override string FrameworkItemType => "ScriptedEventMultipleChoiceQuestion";
	}
}
