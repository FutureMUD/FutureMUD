using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using Org.BouncyCastle.Asn1.Pkcs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.RPG.ScriptedEvents;

internal class ScriptedEvent : SaveableItem, IScriptedEvent
{
	public override string FrameworkItemType => "ScriptedEvent";

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
				// _multipleChoiceQuestions.Add
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
			// Add
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

	public IScriptedEvent Clone()
	{
		var item = new ScriptedEvent(this, null);
		Gameworld.Add(item);
		return item;
	}

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant().CollapseString())
		{

		}

		return false;
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
			foreach (var question in MultipleChoiceQuestions)
			{
				sb.AppendLine();
				sb.AppendLine($"Question #{question.Id.ToString("N0", actor)}");
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
			foreach (var question in FreeTextQuestions)
			{
				sb.AppendLine();
				sb.AppendLine($"Question #{question.Id.ToString("N0", actor)}");
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
