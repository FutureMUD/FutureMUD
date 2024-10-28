using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.RPG.ScriptedEvents
{
	internal class ScriptedEventMultipleChoiceQuestionAnswer : SaveableItem, IScriptedEventMultipleChoiceQuestionAnswer
	{
		public ScriptedEventMultipleChoiceQuestionAnswer(Models.ScriptedEventMultipleChoiceQuestionAnswer dbitem, IFuturemud gameworld)
		{
			Gameworld= gameworld;
			_id = dbitem.Id;
			AfterChoiceProg = Gameworld.FutureProgs.Get(dbitem.AfterChoiceProgId ?? 0L);
			AnswerFilterProg = Gameworld.FutureProgs.Get(dbitem.AnswerFilterProgId ?? 0L);
			DescriptionAfterChoice = dbitem.DescriptionAfterChoice;
			DescriptionBeforeChoice = dbitem.DescriptionBeforeChoice;
		}

        public ScriptedEventMultipleChoiceQuestionAnswer(string before, string after, IScriptedEventMultipleChoiceQuestion question)
        {
			Gameworld = question.Gameworld;
			DescriptionAfterChoice = after;
			DescriptionBeforeChoice = before;
			AnswerFilterProg = Gameworld.AlwaysTrueProg;
			using (new FMDB())
			{
				var dbitem = new Models.ScriptedEventMultipleChoiceQuestionAnswer
				{
					ScriptedEventMultipleChoiceQuestionId = question.Id,
					AnswerFilterProgId = AnswerFilterProg?.Id,
					DescriptionAfterChoice = DescriptionAfterChoice,
					DescriptionBeforeChoice = DescriptionBeforeChoice,
				};
				FMDB.Context.ScriptedEventMultipleChoiceQuestionAnswers.Add(dbitem);
				FMDB.Context.SaveChanges();
				_id = dbitem.Id;
			}
        }

        public override void Save()
		{
			var dbitem = FMDB.Context.ScriptedEventMultipleChoiceQuestionAnswers.Find(Id);
			dbitem.DescriptionBeforeChoice = DescriptionBeforeChoice;
			dbitem.DescriptionAfterChoice = DescriptionAfterChoice;
			dbitem.AnswerFilterProgId = AnswerFilterProg?.Id;
			dbitem.AfterChoiceProgId = AfterChoiceProg?.Id;
			Changed = false;
		}

		public void Delete()
		{
			Gameworld.SaveManager.Abort(this);
			if (_id != 0)
			{
				using (new FMDB())
				{
					Gameworld.SaveManager.Flush();
					var dbitem = FMDB.Context.ScriptedEventMultipleChoiceQuestionAnswers.Find(Id);
					if (dbitem != null)
					{
						FMDB.Context.ScriptedEventMultipleChoiceQuestionAnswers.Remove(dbitem);
						FMDB.Context.SaveChanges();
					}
				}
			}
		}

		public override string FrameworkItemType => "ScriptedEventMultipleChoiceQuestionAnswer";
		public string DescriptionBeforeChoice { get; private set; }
		public string DescriptionAfterChoice { get; private set; }
		public IFutureProg AnswerFilterProg { get; private set; }
		public IFutureProg AfterChoiceProg { get; private set; }

		public bool BuildingCommand(ICharacter actor, StringStack command)
		{
			switch (command.PopForSwitch())
			{
				case "filter":
				case "filterprog":
					return BuildingCommandFilterProg(actor, command);
				case "onchoiceprog":
				case "choiceprog":
				case "choice":
				case "onchoice":
					return BuildingCommandOnChoiceProg(actor, command);
				case "before":
					return BuildingCommandBefore(actor);
				case "after":
					return BuildingCommandAfter(actor);
				default:
					actor.OutputHandler.Send(@"You can use the following options with this command:

	#3before#0 - edits the before text of an answer
	#3after#0 - edits the after text of an answer
	#3filter <prog>#0 - edits the filter prog of an answer
	#3choice <prog>#0 - edits the on choice prog of an answer".SubstituteANSIColour());
					return false;
			}
		}

		private bool BuildingCommandBefore(ICharacter actor)
		{
			actor.OutputHandler.Send($"Replacing:\n\n{DescriptionBeforeChoice.SubstituteANSIColour().Wrap(actor.InnerLineFormatLength)}\n\nEnter the text in the editor below:");
			actor.EditorMode(BeforePost, BeforeCancel, 1.0);
			return true;
		}

		private void BeforeCancel(IOutputHandler handler, object[] arg2)
		{
			handler.Send("You decide not to change the before text.");
		}

		private void BeforePost(string text, IOutputHandler handler, object[] arg3)
		{
			DescriptionBeforeChoice = text;
			Changed = true;
			handler.Send("You change the before text.");
		}

		private bool BuildingCommandAfter(ICharacter actor)
		{
			actor.OutputHandler.Send($"Replacing:\n\n{DescriptionAfterChoice.SubstituteANSIColour().Wrap(actor.InnerLineFormatLength)}\n\nEnter the text in the editor below:");
			actor.EditorMode(AfterPost, AfterCancel, 1.0);
			return true;
		}

		private void AfterCancel(IOutputHandler handler, object[] arg2)
		{
			handler.Send("You decide not to change the after text.");
		}

		private void AfterPost(string text, IOutputHandler handler, object[] arg3)
		{
			DescriptionAfterChoice = text;
			Changed = true;
			handler.Send("You change the after text.");
		}

		private bool BuildingCommandOnChoiceProg(ICharacter actor, StringStack command)
		{
			if (command.IsFinished)
			{
				actor.OutputHandler.Send("You must either specify a prog or #3none#0 to remove it.".SubstituteANSIColour());
				return false;
			}

			if (command.PeekSpeech().EqualToAny("none", "remove", "rem", "delete", "del"))
			{
				AfterChoiceProg = null;
				Changed = true;
				actor.OutputHandler.Send("This answer no longer executes any prog when selected.");
				return true;
			}

			var prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument, ProgVariableTypes.Void, new List<ProgVariableTypes> { ProgVariableTypes.Character }).LookupProg();
			if (prog is null)
			{
				return false;
			}

			AfterChoiceProg = prog;
			Changed = true;
			actor.OutputHandler.Send($"This answer will now execute the {prog.MXPClickableFunctionName()} prog when selected.");
			return true;
		}

		private bool BuildingCommandFilterProg(ICharacter actor, StringStack command)
		{
			if (command.IsFinished)
			{
				actor.OutputHandler.Send("You must specify a prog.");
				return false;
			}

			var prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument, ProgVariableTypes.Boolean, new List<ProgVariableTypes> { ProgVariableTypes.Character }).LookupProg();
			if (prog is null)
			{
				return false;
			}

			AnswerFilterProg = prog;
			Changed = true;
			actor.OutputHandler.Send($"This answer will now use the {prog.MXPClickableFunctionName()} prog to determine if it can be picked.");
			return true;
		}

		public string Show(ICharacter actor)
		{
			var sb = new StringBuilder();
			sb.AppendLine($"Answer #{Id.ToString("N0", actor)}".GetLineWithTitle(actor, Telnet.Blue, Telnet.BoldWhite));
			sb.AppendLine();
			sb.AppendLine($"Filter Prog: {AnswerFilterProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
			sb.AppendLine($"On Choice Prog: {AfterChoiceProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
			sb.AppendLine();
			sb.AppendLine($"Description Before Choice:");
			sb.AppendLine();
			sb.AppendLine(DescriptionBeforeChoice.SubstituteANSIColour().Wrap(actor.InnerLineFormatLength, "\t"));
			sb.AppendLine();
			sb.AppendLine($"Description After Choice:");
			sb.AppendLine();
			sb.AppendLine(DescriptionAfterChoice.SubstituteANSIColour().Wrap(actor.InnerLineFormatLength, "\t"));
			return sb.ToString();
		}
	}
}
