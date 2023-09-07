using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.Migrations;
using System;
using System.Text;

namespace MudSharp.RPG.ScriptedEvents;
#nullable enable
internal class ScriptedEventFreeTextQuestion : SaveableItem, IScriptedEventFreeTextQuestion
{
	public ScriptedEventFreeTextQuestion(Models.ScriptedEventFreeTextQuestion question, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = question.Id;
		_name = "Question";
		Question = question.Question;
		Answer = question.Answer;
	}

	public ScriptedEventFreeTextQuestion(IScriptedEvent scriptedEvent, string question)
	{
		Gameworld = scriptedEvent.Gameworld;
		_name = "Question";
		Question = question;
		Answer = "";
		using (new FMDB())
		{
			var dbitem = new Models.ScriptedEventFreeTextQuestion
			{
				Question = question,
				Answer = "",
				ScriptedEventId = scriptedEvent.Id,
			};
			FMDB.Context.ScriptedEventFreeTextQuestions.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	public override void Save()
	{
		var dbitem = FMDB.Context.ScriptedEventFreeTextQuestions.Find(Id);
		dbitem.Question = Question;
		dbitem.Answer = Answer;
		Changed = false;
	}

	public override string FrameworkItemType => "ScriptedEventFreeTextQuestion";
	public string Question { get; private set; }
	public string Answer { get; private set; }

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
		sb.AppendLine("Answer:");
		sb.AppendLine();
		if (!string.IsNullOrWhiteSpace(Answer))
		{
			sb.AppendLine(Answer.SubstituteANSIColour().Wrap(actor.InnerLineFormatLength, "\t"));
		}
		else
		{
			sb.AppendLine("\tNot Yet Answered".ColourError());
		}
		
		return sb.ToString();
	}
}
