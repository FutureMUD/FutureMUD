using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.RPG.ScriptedEvents;
#nullable enable
public interface IScriptedEvent : IFrameworkItem, ISaveable, IEditableItem
{
	ICharacter? Character { get; }
	IFutureProg? CharacterFilterProg { get; }
	bool IsReady { get; }
	bool IsFinished { get; }
	bool IsTemplate { get; }
	DateTime EarliestDate { get; }

	IEnumerable<IScriptedEventFreeTextQuestion> FreeTextQuestions { get; }
	IEnumerable<IScriptedEventMultipleChoiceQuestion> MultipleChoiceQuestions { get; }

	IScriptedEvent Assign(ICharacter actor);
	IScriptedEvent Clone();
}

public interface IScriptedEventFreeTextQuestion : IFrameworkItem, ISaveable, IEditableItem
{
	string Question { get; }
	string Answer { get; }
}

public interface IScriptedEventMultipleChoiceQuestion : IFrameworkItem, ISaveable, IEditableItem
{
	string Question { get; }
	IScriptedEventMultipleChoiceQuestionAnswer? ChosenAnswer { get; }
	IEnumerable<IScriptedEventMultipleChoiceQuestionAnswer> Answers { get; }
}

public interface IScriptedEventMultipleChoiceQuestionAnswer : IFrameworkItem, ISaveable, IEditableItem
{
	string DescriptionBeforeChoice { get; }
	string DescriptionAfterChoice { get; }
	IFutureProg? AnswerFilterProg { get; }
	IFutureProg? AfterChoiceProg { get; }
}

