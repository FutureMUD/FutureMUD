using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using System;
using System.Collections.Generic;
using System.Linq;
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

		public string Question { get; private set; }
		public IScriptedEventMultipleChoiceQuestionAnswer ChosenAnswer { get; private set; }
		private readonly List<IScriptedEventMultipleChoiceQuestionAnswer> _answers = new();
		public IEnumerable<IScriptedEventMultipleChoiceQuestionAnswer> Answers => _answers;

		public bool BuildingCommand(ICharacter actor, StringStack command)
		{
			throw new NotImplementedException();
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
