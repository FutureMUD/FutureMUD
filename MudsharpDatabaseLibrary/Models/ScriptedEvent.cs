using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Models
{
	public class ScriptedEvent
	{
		public ScriptedEvent()
		{
			MultipleChoiceQuestions = new HashSet<ScriptedEventMultipleChoiceQuestion>();
			FreeTextQuestions = new HashSet<ScriptedEventFreeTextQuestion>();
		}

		public long Id { get; set; }
		public string Name { get; set; }
		public long? CharacterId { get; set; }
		public long? CharacterFilterProgId { get; set; }
		public bool IsReady { get; set; }
		public DateTime EarliestDate { get; set; }
		public bool IsFinished { get; set; }
		public bool IsTemplate { get; set; }

		public virtual Character Character { get; set; }
		public virtual FutureProg CharacterFilterProg { get; set; }
		public virtual ICollection<ScriptedEventMultipleChoiceQuestion> MultipleChoiceQuestions { get; set; }
		public virtual ICollection<ScriptedEventFreeTextQuestion> FreeTextQuestions { get; set; }
	}

	public class ScriptedEventFreeTextQuestion
	{
		public long Id { get; set; }
		public long ScriptedEventId { get; set; }
		public string Question { get; set; }
		public string Answer { get; set; }
		public virtual ScriptedEvent ScriptedEvent { get; set; }
	}

	public class ScriptedEventMultipleChoiceQuestion
	{
		public ScriptedEventMultipleChoiceQuestion()
		{
			Answers = new HashSet<ScriptedEventMultipleChoiceQuestionAnswer>();
		}

		public long Id { get; set; }
		public long ScriptedEventId { get; set; }
		public string Question { get; set; }
		public long? ChosenAnswerId { get; set; }

		public virtual ScriptedEvent ScriptedEvent { get; set; }
		public virtual ScriptedEventMultipleChoiceQuestionAnswer ChosenAnswer { get; set; }
		public virtual ICollection<ScriptedEventMultipleChoiceQuestionAnswer> Answers { get; set; }
	}

	public class ScriptedEventMultipleChoiceQuestionAnswer
	{
		public long Id { get; set; }
		public long ScriptedEventMultipleChoiceQuestionId { get; set; }
		public string DescriptionBeforeChoice { get; set; }
		public string DescriptionAfterChoice { get; set; }
		public long? AnswerFilterProgId { get; set; }
		public long? AfterChoiceProgId { get; set; }

		public virtual ScriptedEventMultipleChoiceQuestion ScriptedEventMultipleChoiceQuestion { get; set; }
		public virtual FutureProg AnswerFilterProg { get; set; }
		public virtual FutureProg AfterChoiceProg { get; set; }
	}
}
