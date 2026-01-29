using System;
using System.ClientModel.Primitives;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.Models;
using OpenAI;
using OpenAI.Chat;
using OpenAI.Responses;

#pragma warning disable OPENAI001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

namespace MudSharp.RPG.AIStorytellers;

public class AIStoryteller : SaveableItem, IAIStoryteller
{
	public override string FrameworkItemType => "AIStoryteller";

	public AIStoryteller(MudSharp.Models.AIStoryteller storyteller, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		Id = storyteller.Id;
		_name = storyteller.Name;
		Description = storyteller.Description;
		Model = storyteller.Model;
		SystemPrompt = storyteller.SystemPrompt;
		AttentionAgentPrompt = storyteller.AttentionAgentPrompt;
		ReasoningEffort = (ResponseReasoningEffortLevel)storyteller.ReasoningEffort;
		SurveillanceStrategy = new AIStorytellerSurveillanceStrategy(gameworld, storyteller.SurveillanceStrategyDefinition);
	}

	public string Description { get; private set; }
	public string Model { get; private set; }
	public string SystemPrompt { get; private set; }
	public string AttentionAgentPrompt { get; private set; }
	public ResponseReasoningEffortLevel ReasoningEffort { get; private set; }
	public IAIStorytellerSurveillanceStrategy SurveillanceStrategy { get; private set; }

	private readonly List<ICell> _subscribedCells = new();

	public void SubscribeEvents()
	{
		UnsubscribeEvents();
		Gameworld.HeartbeatManager.FuzzyHourHeartbeat += HeartbeatManager_FuzzyHourHeartbeat;
		var cells = SurveillanceStrategy.GetCells(Gameworld).ToList();
		_subscribedCells.Clear();
		_subscribedCells.AddRange(cells);
		foreach (var cell in cells)
		{
			cell.OnRoomEcho += Cell_OnRoomEcho;
		}
	}

	private void PassSituationToAIStoryteller(ICell location, PerceptionEngine.IEmoteOutput emote, string echo, string attentionReason)
	{
		var apiKey = Futuremud.Games.First().GetStaticConfiguration("GPT_Secret_Key");
		if (string.IsNullOrEmpty(apiKey))
		{
			return;
		}

		var sb = new StringBuilder();
		sb.AppendLine("Your attention subroutine has flagged something that has happened as relevant to you.");
		sb.AppendLine($"The relevant thing is taking place at a location called {location.HowSeen(null)}.");
		if (emote is not null && emote.DefaultSource is ICharacter ch)
		{
			sb.AppendLine($"The character that originated the thing is called {ch.PersonalName.GetName(Character.Name.NameStyle.FullName)} and their description is {ch.HowSeen(ch, flags: PerceiveIgnoreFlags.TrueDescription)}.");
		}
		sb.AppendLine("The thing that has been flagged to your attention is the following:");
		sb.AppendLine(echo.StripANSIColour().StripMXP());
		sb.AppendLine($"The attention subroutine thought that this was relevant for the following reason:");
		sb.AppendLine(attentionReason?.IfNullOrWhiteSpace("No reason provided"));

		ResponsesClient client = new(Model, apiKey);
		var options = new CreateResponseOptions([
				ResponseItem.CreateDeveloperMessageItem(SystemPrompt),
				ResponseItem.CreateUserMessageItem(echo),
			]);
		options.ReasoningOptions.ReasoningEffortLevel = ReasoningEffort;
	}

	private void Cell_OnRoomEcho(ICell location, RoomLayer layer, PerceptionEngine.IEmoteOutput emote)
	{
		var apiKey = Futuremud.Games.First().GetStaticConfiguration("GPT_Secret_Key");
		if (string.IsNullOrEmpty(apiKey))
		{
			return;
		}

		var echo = emote.ParseFor(null);


		ResponsesClient client = new(Model, apiKey);
		var options = new CreateResponseOptions([
				ResponseItem.CreateDeveloperMessageItem(AttentionAgentPrompt),
				ResponseItem.CreateUserMessageItem(echo),
			]);
		options.ReasoningOptions.ReasoningEffortLevel = ResponseReasoningEffortLevel.Minimal;
		var task = Task.Run(async () =>
		{
			try
			{
				ResponseResult attention = await client.CreateResponseAsync(options);
				var ss = new StringStack(attention.GetOutputText());
				switch (ss.Pop().ToLowerInvariant())
				{
					case "interested":
						PassSituationToAIStoryteller(location, emote, echo, ss.SafeRemainingArgument);
						return;

				}
			}
			catch (Exception e)
			{
				e.ToString().Prepend("#2GPT Error#0\n").WriteLineConsole();
				Futuremud.Games.First().DiscordConnection.NotifyAdmins($"**GPT Error**\n\n```\n{e.ToString()}```");
			}
		});
	}

	private void HeartbeatManager_FuzzyHourHeartbeat() => throw new NotImplementedException();
	public void UnsubscribeEvents()
	{
		Gameworld.HeartbeatManager.FuzzyHourHeartbeat -= HeartbeatManager_FuzzyHourHeartbeat;
		foreach (var cell in _subscribedCells)
		{
			cell.OnRoomEcho -= Cell_OnRoomEcho;
		}
	}

	public void Delete()
	{
		Changed = false;
		UnsubscribeEvents();
		Gameworld.SaveManager.Abort(this);
		if (_id != 0)
		{
			using (new FMDB())
			{
				Gameworld.SaveManager.Flush();
				var dbitem = FMDB.Context.AIStorytellers.Find(Id);
				if (dbitem != null)
				{
					FMDB.Context.AIStorytellers.Remove(dbitem);
					FMDB.Context.SaveChanges();
				}
			}
		}
	}

	public void Pause() => throw new NotImplementedException();
	public void Unpause() => throw new NotImplementedException();

	private readonly List<IAIStorytellerCharacterMemory> _characterMemories = new();
	public IEnumerable<IAIStorytellerCharacterMemory> CharacterMemories => _characterMemories;

	private readonly List<IAIStorytellerSituation> _situations = new();
	public IEnumerable<IAIStorytellerSituation> Situations => _situations;

	public bool BuildingCommand(ICharacter actor, StringStack command) => throw new NotImplementedException();
	public string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"AI Storyteller #{Id} - {Name.Colour(Telnet.Cyan)}");
		sb.AppendLine($"Model: {Model.ColourValue()}");
		sb.AppendLine($"");
		sb.AppendLine("Description".GetLineWithTitleInner(actor, Telnet.Blue, Telnet.BoldWhite));
		sb.AppendLine($"");
		sb.AppendLine(Description.Wrap(actor.InnerLineFormatLength, "\t"));
		sb.AppendLine($"");
		sb.AppendLine("Surveillance Strategy".GetLineWithTitleInner(actor, Telnet.Blue, Telnet.BoldWhite));
		sb.AppendLine($"");
		sb.AppendLine(SurveillanceStrategy.Show(actor));
		sb.AppendLine($"");
		sb.AppendLine("Attention Prompt".GetLineWithTitleInner(actor, Telnet.Blue, Telnet.BoldWhite));
		sb.AppendLine($"");
		sb.AppendLine(AttentionAgentPrompt.Wrap(actor.InnerLineFormatLength, "\t"));
		sb.AppendLine($"");
		sb.AppendLine("System Prompt".GetLineWithTitleInner(actor, Telnet.Blue, Telnet.BoldWhite));
		sb.AppendLine($"");
		sb.AppendLine(SystemPrompt.Wrap(actor.InnerLineFormatLength, "\t"));
		sb.AppendLine($"");
		sb.AppendLine("Current Situations".GetLineWithTitleInner(actor, Telnet.Blue, Telnet.BoldWhite));
		sb.AppendLine($"");
		sb.AppendLine(StringUtilities.GetTextTable(
			from item in _situations
			select new List<string> {
				item.Id.ToStringN0(actor),
				item.Name,
				item.CreatedOn.GetLocalDateString(actor, true)
			},
			[
				"Id",
				"Title",
				"Created"
			],
			actor,
			Telnet.Green
		));

		return sb.ToString();
	}
	public override void Save()
	{
		Changed = false;
		var dbitem = FMDB.Context.AIStorytellers.Find(Id);
		if (dbitem is null)
		{
			return;	
		}

		dbitem.Name = Name;
		dbitem.Description = Description;
		dbitem.AttentionAgentPrompt = AttentionAgentPrompt;
		dbitem.SystemPrompt = SystemPrompt;
		dbitem.SurveillanceStrategyDefinition = SurveillanceStrategy.SaveDefinition();
	}
}
