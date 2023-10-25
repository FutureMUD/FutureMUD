using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MoreLinq;
using MudSharp.Character;
using MudSharp.Editor;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.ScriptedEvents;

namespace MudSharp.Menus;
#nullable enable
public class ScriptedEventMenu : CharacterLoginMenu
{
	public IScriptedEvent ScriptedEvent { get; }
	public bool FirstTime { get; }
	private bool _awaitingDone;

	private IScriptedEventFreeTextQuestion? _currentFreeTextQuestion;
	private IScriptedEventMultipleChoiceQuestion? _currentMultipleChoiceQuestion;

	public ScriptedEventMenu(ICharacter character, IScriptedEvent scriptedEvent, bool firstTime) : base(character)
	{
		ScriptedEvent = scriptedEvent;
		FirstTime = firstTime;
		if (ScriptedEvent.MultipleChoiceQuestions.Any())
		{
			_currentMultipleChoiceQuestion = ScriptedEvent.MultipleChoiceQuestions.First();
		}
		else if (ScriptedEvent.FreeTextQuestions.Any())
		{
			_currentFreeTextQuestion = ScriptedEvent.FreeTextQuestions.First();
		}
		else
		{
			throw new NotImplementedException("This should never happen");
		}
	}

	public bool DetermineNextQuestion()
	{
		if (_currentMultipleChoiceQuestion is not null)
		{
			_currentMultipleChoiceQuestion = ScriptedEvent.MultipleChoiceQuestions.SkipUntil(x => x == _currentMultipleChoiceQuestion).FirstOrDefault();
			if (_currentMultipleChoiceQuestion is not null)
			{
				return true;
			}
		}

		if (_currentFreeTextQuestion is null)
		{
			_currentFreeTextQuestion = ScriptedEvent.FreeTextQuestions.FirstOrDefault();
		}
		else
		{
			_currentFreeTextQuestion = ScriptedEvent.FreeTextQuestions.SkipUntil(x => x == _currentFreeTextQuestion).FirstOrDefault();
		}
		
		return _currentFreeTextQuestion is not null;
	}

	public string ShowMenuText()
	{
		var sb = new StringBuilder();
		sb.AppendLine();
		sb.AppendLine($"Offline Event - {ScriptedEvent.Name}".GetLineWithTitle(Character, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine();
		if (_currentMultipleChoiceQuestion is not null)
		{
			sb.AppendLine(_currentMultipleChoiceQuestion.Question.SubstituteANSIColour().Wrap(Character.InnerLineFormatLength));
			var i = 1;
			foreach (var answer in _currentMultipleChoiceQuestion.Answers.Where(x => x.AnswerFilterProg?.Execute<bool?>(Character) != false))
			{
				sb.AppendLine();
				sb.AppendLine($"Option #{i++.ToString("N0", Character)}".GetLineWithTitle(Character, Telnet.Cyan, Telnet.BoldWhite));
				sb.AppendLine();
				sb.AppendLine(answer.DescriptionBeforeChoice.SubstituteANSIColour().Wrap(Character.InnerLineFormatLength));
			}
			sb.AppendLine();
			sb.AppendLine($"Enter the number of the option you would like to choose: ".ColourCommand());
			return sb.ToString();
		}

		if (_currentFreeTextQuestion is not null)
		{
			sb.AppendLine(_currentFreeTextQuestion!.Question.SubstituteANSIColour().Wrap(Character.InnerLineFormatLength));
			sb.AppendLine();
			sb.AppendLine("Enter your answer in the text editor below:\nYou are now entering an editor, use @ on a blank line to exit and *help to see help.\n".ColourCommand());
			var original = Controller;
			_nextContext = new EditorController(this, null, PostFreeText, CancelFreeText, EditorOptions.None);
			return sb.ToString();
		}

		return string.Empty;
	}

	private void CancelFreeText(IOutputHandler handler, object[] args)
	{
		_nextContext = new LoggedInMenu(Character.Account, Gameworld);
	}

	private void PostFreeText(string text, IOutputHandler handler, object[] args)
	{
	_currentFreeTextQuestion!.SetAnswer(text);
		if (!DetermineNextQuestion())
		{
			FinishScriptedEvent();
			return;
		}

		OutputHandler.Send(ShowMenuText());
	}

	private void FinishScriptedEvent()
	{
		ScriptedEvent.SetFinished();
		LoseControl(Controller);
		DoLogin(FirstTime);
	}

	public override bool ExecuteCommand(string command)
	{
		if (_currentFreeTextQuestion is null && _currentMultipleChoiceQuestion is null)
		{
			OutputHandler.Send(ShowMenuText());
			return true;
		}

		if (_awaitingDone)
		{
			_awaitingDone = false;
			if (!DetermineNextQuestion())
			{
				FinishScriptedEvent();
				return true;
			}

			OutputHandler.Send(ShowMenuText());
			return true;
		}

		if (string.IsNullOrWhiteSpace(command))
		{
			OutputHandler.Send(ShowMenuText());
			return true;
		}

		if (command.EqualToAny("quit", "close", "stop", "back"))
		{
			_nextContext = new LoggedInMenu(Character.Account, Gameworld);
			return true;
		}

		if (!int.TryParse(command, out var index) || index < 1 || index > _currentMultipleChoiceQuestion!.Answers.Count())
		{
			OutputHandler.Send($"You must enter a valid number between {1.ToString("N0", Character).ColourValue()} and {_currentMultipleChoiceQuestion!.Answers.Count().ToString("N0", Character).ColourValue()}.");
			return true;
		}

		var answer = _currentMultipleChoiceQuestion.Answers.ElementAt(index - 1);
		_currentMultipleChoiceQuestion.ChooseAnswer(answer);
		answer.AfterChoiceProg?.Execute(Character);

		var sb = new StringBuilder();
		sb.AppendLine();
		sb.AppendLine($"Result".GetLineWithTitle(Character, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine(answer.DescriptionAfterChoice.SubstituteANSIColour().Wrap(Character.InnerLineFormatLength));
		sb.AppendLine();
		sb.AppendLine($"Type {"done".ColourCommand()} to continue.");
		OutputHandler.Send(sb.ToString());
		_awaitingDone = true;
		return true;
	}

	public override string Prompt => "> \n";

	public override void AssumeControl(IController controller)
	{
		base.AssumeControl(controller);
		OutputHandler.Send(ShowMenuText());
	}

	public override void SilentAssumeControl(IController controller)
	{
		base.SilentAssumeControl(controller);
		OutputHandler.Send(ShowMenuText());
	}
}
