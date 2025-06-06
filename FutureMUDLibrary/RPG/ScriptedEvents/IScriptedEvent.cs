﻿using MudSharp.Character;
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
	void Delete();
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
	void SetFinished();
}

public interface IScriptedEventFreeTextQuestion : IFrameworkItem, ISaveable, IEditableItem
{
	void Delete();
	string Question { get; }
	string Answer { get; }
	void SetAnswer(string answer);
}

public interface IScriptedEventMultipleChoiceQuestion : IFrameworkItem, ISaveable, IEditableItem
{
	void Delete();
	string Question { get; }
	IScriptedEventMultipleChoiceQuestionAnswer? ChosenAnswer { get; }
	IEnumerable<IScriptedEventMultipleChoiceQuestionAnswer> Answers { get; }
	void ChooseAnswer(IScriptedEventMultipleChoiceQuestionAnswer answer);
}

public interface IScriptedEventMultipleChoiceQuestionAnswer : IFrameworkItem, ISaveable, IEditableItem
{
	void Delete();
	string DescriptionBeforeChoice { get; }
	string DescriptionAfterChoice { get; }
	IFutureProg? AnswerFilterProg { get; }
	IFutureProg? AfterChoiceProg { get; }
}

