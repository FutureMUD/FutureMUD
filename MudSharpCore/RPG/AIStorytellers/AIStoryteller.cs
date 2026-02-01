using System;
using System.ClientModel.Primitives;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MudSharp.Character;
using MudSharp.Character.Name;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.Models;
using MudSharp.PerceptionEngine.Parsers;
using OpenAI;
using OpenAI.Chat;
using OpenAI.Responses;
using static Google.Protobuf.Reflection.SourceCodeInfo.Types;

#pragma warning disable OPENAI001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

namespace MudSharp.RPG.AIStorytellers;

public class AIStoryteller : SaveableItem, IAIStoryteller
{
	public override string FrameworkItemType => "AIStoryteller";

	public AIStoryteller(Models.AIStoryteller storyteller, IFuturemud gameworld)
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
	public bool SubscribeToRoomEvents { get; private set; }
	public bool SubscribeTo5mHeartbeat { get; private set; }
	public bool SubscribeToHourHeartbeat { get; private set; }
	public IFutureProg HeartbeatStatus5mProg { get; private set; }
	public IFutureProg HeartbeatStatus1hProg { get; private set; }
	public bool IsPaused { get; private set; }

	public ResponseReasoningEffortLevel ReasoningEffort { get; private set; }
	public IAIStorytellerSurveillanceStrategy SurveillanceStrategy { get; private set; }
	public IFutureProg CustomPlayerInformationProg { get; private set; }

	private readonly List<ICell> _subscribedCells = new();

	public void SubscribeEvents()
	{
		UnsubscribeEvents();
		if (SubscribeToHourHeartbeat)
		{
			Gameworld.HeartbeatManager.FuzzyHourHeartbeat += HeartbeatManager_FuzzyHourHeartbeat;
		}

		if (SubscribeTo5mHeartbeat)
		{
			Gameworld.HeartbeatManager.FuzzyFiveMinuteHeartbeat += HeartbeatManager_FiveMinuteHeartbeat;
		}

		if (SubscribeToRoomEvents)
		{
			var cells = SurveillanceStrategy.GetCells(Gameworld).ToList();
			_subscribedCells.Clear();
			_subscribedCells.AddRange(cells);
			foreach (var cell in cells)
			{
				cell.OnRoomEmoteEcho += Cell_OnRoomEcho;
			}
		}
	}

	private void AddUniversalToolsToResponseOptions(CreateResponseOptions options)
	{
		options.Tools.Add(ResponseTool.CreateFunctionTool(
			functionName: "CreateSituation",
			functionDescription: "Creates a new situation for the AI Storyteller to manage",
			functionParameters: BinaryData.FromBytes(
				"""
				{
				"type": "object",
				  "properties": {
					"Title": {
					  "type": "string",
					  "description": "The title of the situation"
					},
					"Description": {
					  "type": "string",
					  "description": "A description of the situation"
					}
				  },
				  "required": ["Title", "Description"]
				}
				"""u8.ToArray()
			),
			strictModeEnabled: true
			));
		options.Tools.Add(ResponseTool.CreateFunctionTool(
			functionName: "UpdateSituation",
			functionDescription: "Updates an existing situation with fresh details",
			functionParameters: BinaryData.FromBytes(
				"""
				{
				"type": "object",
				  "properties": {
					"Id": {
					  "type": "int32",
					  "description": "The id of the situation to update"
					},
					"Title": {
					  "type": "string",
					  "description": "The title of the situation"
					},
					"Description": {
					  "type": "string",
					  "description": "A description of the situation"
					}
				  },
				  "required": ["Id", "Title", "Description"]
				}
				"""u8.ToArray()
			),
			strictModeEnabled: true
			));
		options.Tools.Add(ResponseTool.CreateFunctionTool(
			functionName: "ResolveSituation",
			functionDescription: "Resolves a situation and sends it to the archive",
			functionParameters: BinaryData.FromBytes(
				"""
				{
				"type": "object",
				  "properties": {
					"Id": {
					  "type": "int32",
					  "description": "The id of the situation to resolve"
					},
					"Title": {
					  "type": "string",
					  "description": "The final title of the situation"
					},
					"Description": {
					  "type": "string",
					  "description": "A final description of the situation"
					}
				  },
				  "required": ["Id", "Title", "Description"]
				}
				"""u8.ToArray()
			),
			strictModeEnabled: true
			));
		options.Tools.Add(ResponseTool.CreateFunctionTool(
			functionName: "Noop",
			functionDescription: "Use this tool call when it is mandatory for you to make a tool call response but you don't actually want to do anything",
			functionParameters: null,
			strictModeEnabled: true
			));
		options.Tools.Add(ResponseTool.CreateFunctionTool(
			functionName: "OnlinePlayers",
			functionDescription: "Provides information about which players are online",
			functionParameters: null,
			strictModeEnabled: true
			));
		options.Tools.Add(ResponseTool.CreateFunctionTool(
			functionName: "PlayerInformation",
			functionDescription: "Retrieves detailed information about a specific player character",
			functionParameters: BinaryData.FromBytes(
				"""
				{
				"type": "object",
				  "properties": {
					"Id": {
					  "type": "long",
					  "description": "The id the player to retrieve information about"
					}
				  },
				  "required": ["Id"]
				}
				"""u8.ToArray()
			),
			strictModeEnabled: true
			));
		options.Tools.Add(ResponseTool.CreateFunctionTool(
			functionName: "CreateMemory",
			functionDescription: "Creates a memory or fact about a particular player character",
			functionParameters: BinaryData.FromBytes(
				"""
				{
				"type": "object",
				  "properties": {
					"Id": {
					  "type": "long",
					  "description": "The id of the character to create a memory about"
					},
					"Title": {
					  "type": "string",
					  "description": "The title of the memory"
					},
					"Details": {
					  "type": "string",
					  "description": "The details of the memory"
					}
				  },
				  "required": ["Id", "Title", "Details"]
				}
				"""u8.ToArray()
			),
			strictModeEnabled: true
			));
		options.Tools.Add(ResponseTool.CreateFunctionTool(
			functionName: "UpdateMemory",
			functionDescription: "Creates a memory or fact about a particular player character",
			functionParameters: BinaryData.FromBytes(
				"""
				{
				"type": "object",
				  "properties": {
					"Id": {
					  "type": "long",
					  "description": "The id of the memory to update"
					},
					"Title": {
					  "type": "string",
					  "description": "The title of the memory"
					},
					"Details": {
					  "type": "string",
					  "description": "The details of the memory"
					}
				  },
				  "required": ["Id", "Title", "Details"]
				}
				"""u8.ToArray()
			),
			strictModeEnabled: true
			));
		options.Tools.Add(ResponseTool.CreateFunctionTool(
			functionName: "Landmarks",
			functionDescription: "Returns a collection of all the important landmark locations",
			functionParameters: null,
			strictModeEnabled: true
			));
		options.Tools.Add(ResponseTool.CreateFunctionTool(
			functionName: "ShowLandmark",
			functionDescription: "Shows detailed information about a particular landmark",
			functionParameters: BinaryData.FromBytes(
				"""
				{
				"type": "object",
				  "properties": {
					"Id": {
					  "type": "string",
					  "description": "The name of the landmark to show information about"
					}
				  },
				  "required": ["Id"]
				}
				"""u8.ToArray()
			),
			strictModeEnabled: true
			));
	}

	private void PassHeartbeatEventToAIStoryteller(string heartbeatType)
	{
		var apiKey = Futuremud.Games.First().GetStaticConfiguration("GPT_Secret_Key");
		if (string.IsNullOrEmpty(apiKey))
		{
			return;
		}

		var sb = new StringBuilder();
		sb.AppendLine("Your attention subroutine has flagged something that has happened as relevant to you.");

		ResponsesClient client = new(Model, apiKey);
		List<ResponseItem> messages = [
				ResponseItem.CreateDeveloperMessageItem(SystemPrompt),
				ResponseItem.CreateUserMessageItem(sb.ToString()),
			];
		var options = new CreateResponseOptions(messages);
		options.ReasoningOptions.ReasoningEffortLevel = ReasoningEffort;

		AddUniversalToolsToResponseOptions(options);
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
		List<ResponseItem> messages = [
				ResponseItem.CreateDeveloperMessageItem(SystemPrompt),
				ResponseItem.CreateUserMessageItem(sb.ToString()),
			];
		var options = new CreateResponseOptions(messages);
		options.ReasoningOptions.ReasoningEffortLevel = ReasoningEffort;

		AddUniversalToolsToResponseOptions(options);

		var requiresAction = false;
		do
		{
			requiresAction = false;
			var task = Task.Run(async () =>
			{
				try
				{
					ResponseResult response = await client.CreateResponseAsync(options);
					messages.AddRange(response.OutputItems);
					foreach (ResponseItem outputItem in response.OutputItems)
					{
						if (outputItem is FunctionCallResponseItem functionCall)
						{
							using JsonDocument argumentsJson = JsonDocument.Parse(functionCall.FunctionArguments);
							switch (functionCall.FunctionName)
							{
								case "Noop":
									return;
								case "CreateSituation":
									{
										var title = argumentsJson.RootElement.GetProperty("Title").GetString();
										var description = argumentsJson.RootElement.GetProperty("Description").GetString();
										var situation = new AIStorytellers.AIStorytellerSituation(Gameworld, this, title, description);
										_situations.Add(situation);
										messages.Add(ResponseItem.CreateFunctionCallOutputItem(functionCall.Id, $"Created new situation '{situation.Name}' with Id {situation.Id}"));
										break;
									}
								case "UpdateSituation":
									{
										var id = argumentsJson.RootElement.GetProperty("Id").GetInt32();
										var situation = _situations.FirstOrDefault(x => x.Id == id);
										if (situation is null)
										{
											messages.Add(ResponseItem.CreateFunctionCallOutputItem(functionCall.Id, $"No situation with Id {id} found"));
											break;
										}
										var newTitle = argumentsJson.RootElement.GetProperty("Title").GetString();
										var newDescription = argumentsJson.RootElement.GetProperty("Description").GetString();
										situation.UpdateSituation(newTitle, newDescription);
										messages.Add(ResponseItem.CreateFunctionCallOutputItem(functionCall.Id, $"Updated situation '{situation.Name}' (Id {situation.Id})"));
										break;
									}
								case "ResolveSituation":
									{
										var id = argumentsJson.RootElement.GetProperty("Id").GetInt32();
										var situation = _situations.FirstOrDefault(x => x.Id == id);
										if (situation is null)
										{
											messages.Add(ResponseItem.CreateFunctionCallOutputItem(functionCall.Id, $"No situation with Id {id} found"));
											break;
										}
										var newTitle = argumentsJson.RootElement.GetProperty("Title").GetString();
										var newDescription = argumentsJson.RootElement.GetProperty("Description").GetString();
										situation.UpdateSituation(newTitle, newDescription);
										situation.Resolve();
										messages.Add(ResponseItem.CreateFunctionCallOutputItem(functionCall.Id, $"Resolved situation '{situation.Name}' (Id {situation.Id})"));
										break;
									}
								case "OnlinePlayers":
									var onlinePlayers = Gameworld.Characters.Where(x => x.IsPlayerCharacter).ToList();
									var sb = new StringBuilder();
									sb.Append("""{ "players": [""");
									foreach (var pc in onlinePlayers)
									{
										sb.AppendLine(
											$$"""
											{
												"Id" : {{pc.Id}},
												"Name" : "{{pc.PersonalName.GetName(NameStyle.FullName).EscapeForJson()}}",
												"Gender" : "{{pc.Gender.GenderClass().EscapeForJson()}}",
												"ShortDescription" : "{{pc.HowSeen(null, colour: false, flags: PerceiveIgnoreFlags.TrueDescription).EscapeForJson()}}",
												"LocationId" : {{pc.Location?.Id ?? 0:N0}},
												"LocationName" : "{{pc.Location?.HowSeen(null, colour: false).EscapeForJson() ?? "Unknown"}}",
												"NumberOfMemories" : {{_characterMemories.Count(x => x.Character.Id == pc.Id):N0}}
											}
											"""
										);
									}
									sb.Append("]}");
									messages.Add(ResponseItem.CreateFunctionCallOutputItem(functionCall.Id, sb.ToString()));
									break;
								case "PlayerInformation":
									{
										var id = argumentsJson.RootElement.GetProperty("Id").GetInt64();
										var pc = Gameworld.Characters.FirstOrDefault(x => x.IsPlayerCharacter && x.Id == id);
										if (pc is null)
										{
											messages.Add(ResponseItem.CreateFunctionCallOutputItem(functionCall.Id, $"No player character with Id {id} found"));
											break;
										}
										sb = new StringBuilder();
										sb.AppendLine("{");
										sb.AppendLine($$"""  "Id" : {{pc.Id}}, """);
										sb.AppendLine($$"""  "Name" : "{{pc.PersonalName.GetName(NameStyle.FullName).EscapeForJson()}}", """);
										sb.AppendLine($$"""  "Gender" : "{{pc.Gender.GenderClass()}}", """);
										sb.AppendLine($$"""  "Race" : "{{pc.Race.Name.EscapeForJson()}}", """);
										sb.AppendLine($$"""  "Ethnicity" : "{{pc.Ethnicity.Name.EscapeForJson()}}", """);
										sb.AppendLine($$"""  "Culture" : "{{pc.Culture.Name.EscapeForJson()}}", """);
										sb.AppendLine($$"""  "Age" : {{pc.AgeInYears:N0}}, """);
										sb.AppendLine($$"""  "AgeCategory" : "{{pc.AgeCategory.DescribeEnum(true).EscapeForJson()}}", """);
										sb.AppendLine($$"""  "Birthday" : "{{pc.Birthday.Display(TimeAndDate.Date.CalendarDisplayMode.Long).EscapeForJson()}}", """);
										sb.AppendLine($$"""  "ShortDescription" : "{{pc.HowSeen(null, colour: false, flags: PerceiveIgnoreFlags.TrueDescription).EscapeForJson()}}", """);
										sb.AppendLine($$"""  "FullDescription" : "{{pc.HowSeen(null, type: Form.Shape.DescriptionType.Full, colour: false, flags: PerceiveIgnoreFlags.TrueDescription).EscapeForJson()}}", """);
										sb.AppendLine($$"""  "LocationId" : {{pc.Location?.Id ?? 0:N0}}, """);
										sb.AppendLine($$"""  "LocationName" : "{{pc.Location?.HowSeen(null, colour: false).EscapeForJson() ?? "Unknown"}}", """);
										if (CustomPlayerInformationProg is not null)
										{
											var info = CustomPlayerInformationProg?.ExecuteDictionary<string>(pc);
											foreach (var kvp in info)
											{
												sb.AppendLine($$"""  "{{kvp.Key.EscapeForJson()}}" : "{{kvp.Value.EscapeForJson()}}", """);
											}
										}

										sb.AppendLine("""  "Memories" : [""");
										foreach (var item in _characterMemories.Where(x => x.Character.Id == pc.Id))
										{
											sb.AppendLine("    {");
											sb.AppendLine($$"""      "Id" : {{item.Id}}, """);
											sb.AppendLine($$"""      "Title" : "{{item.Name.EscapeForJson()}}", """);
											sb.AppendLine($$"""      "Details" : "{{item.MemoryText.EscapeForJson()}}", """);
											sb.AppendLine($$"""      "CreatedOn" : "{{item.CreatedOn.ToString("o")}}", """);
											sb.AppendLine("    },");
										}
										sb.AppendLine("""  ],""");
										sb.AppendLine("}");
										messages.Add(ResponseItem.CreateFunctionCallOutputItem(functionCall.Id, sb.ToString()));
										break;
									}
								case "CreateMemory":
									{
										var id = argumentsJson.RootElement.GetProperty("Id").GetInt64();
										var player = Gameworld.TryGetCharacter(id, true);
										if (player is null)
										{
											messages.Add(ResponseItem.CreateFunctionCallOutputItem(functionCall.Id, $"No player with Id {id} found"));
											break;
										}
										var newTitle = argumentsJson.RootElement.GetProperty("Title").GetString();
										var newDescription = argumentsJson.RootElement.GetProperty("Details").GetString();
										var newMemory = new AIStorytellerCharacterMemory(this, player, newTitle, newDescription);
										_characterMemories.Add(newMemory);
										messages.Add(ResponseItem.CreateFunctionCallOutputItem(functionCall.Id, $"Created memory '{newTitle}' for character {id:N0} with memory Id {newMemory.Id:N0})"));
										break;
									}
								case "UpdateMemory":
									{
										var memoryId = argumentsJson.RootElement.GetProperty("Id").GetInt64();
										var memory = _characterMemories.FirstOrDefault(x => x.Id == memoryId);
										if (memory is null)
										{
											messages.Add(ResponseItem.CreateFunctionCallOutputItem(functionCall.Id, $"No memory with Id {memoryId} found"));
											break;
										}
										var updatedTitle = argumentsJson.RootElement.GetProperty("Title").GetString();
										var updatedDetails = argumentsJson.RootElement.GetProperty("Details").GetString();
										memory.UpdateMemory(updatedTitle, updatedDetails);
										messages.Add(ResponseItem.CreateFunctionCallOutputItem(functionCall.Id, $"Updated memory '{updatedTitle}' (Id {memory.Id:N0})"));
										break;
									}
								case "ForgetMemory":
									{
										var memoryId = argumentsJson.RootElement.GetProperty("Id").GetInt64();
										var memory = _characterMemories.FirstOrDefault(x => x.Id == memoryId);
										if (memory is null)
										{
											messages.Add(ResponseItem.CreateFunctionCallOutputItem(functionCall.Id, $"No memory with Id {memoryId} found"));
											break;
										}

										memory.Forget();
										_characterMemories.Remove(memory);
										messages.Add(ResponseItem.CreateFunctionCallOutputItem(functionCall.Id, $"Forgot memory '{memory.MemoryTitle}' (Id {memory.Id:N0})"));
										break;
									}
								case "Landmarks":
									{
										var landmarks = Gameworld.Cells.SelectNotNull(x => x.EffectsOfType<LandmarkEffect>().FirstOrDefault()).ToList();
										sb = new StringBuilder();
										sb.Append("""{ "landmarks": [""");
										foreach (var item in landmarks)
										{
											var cell = (ICell)item.Owner;
											sb.AppendLine($$"""
												{
													"Id" : "{{item.Name.EscapeForJson()}}",
													"RoomId" : {{cell.Id:N0}},
													"RoomName" : "{{cell.HowSeen(null, colour: false).EscapeForJson()}}",
												}
												""");
										}
										sb.Append("""]}""");
										break;
									}
								case "ShowLandmark":
									{
										var landmarkId = argumentsJson.RootElement.GetProperty("Id").GetString();
										var landmark = Gameworld.Cells.SelectNotNull(x => x.EffectsOfType<LandmarkEffect>().FirstOrDefault())
											.FirstOrDefault(x => x.Name.EqualTo(landmarkId));
										if (landmark is null)
										{
											messages.Add(ResponseItem.CreateFunctionCallOutputItem(functionCall.Id, $"No landmark with Id {landmarkId} found"));
											break;
										}

										var cell = (ICell)landmark.Owner;

										sb = new StringBuilder();

										sb.AppendLine("{");
										sb.AppendLine($$"""
													"Id" : "{{landmark.Name.EscapeForJson()}}",
													"RoomId" : { { cell.Id:N0} },
													"RoomName" : "{{cell.HowSeen(null, colour: false).EscapeForJson()}}",
													"RoomDescription" : "{{cell.ProcessedFullDescription(null, PerceiveIgnoreFlags.TrueDescription, cell.CurrentOverlay).EscapeForJson()}}",
													"Details" : "{{landmark.LandmarkDescriptionTexts.Select(x => x.Text).ListToCommaSeparatedValues("\n").StripANSIColour().EscapeForJson()}}",
													"Occupants": 
													[
													""");
										foreach (var occupant in cell.Characters)
										{
											sb.AppendLine($$"""
														{
															"Id" : {{occupant.Id:N0}},
															"Name" : "{{occupant.PersonalName.GetName(NameStyle.FullName).EscapeForJson()}}",
															"Gender" : "{{occupant.Gender.GenderClass()}}",
															"ShortDescription" : "{{occupant.HowSeen(null, colour: false, flags: PerceiveIgnoreFlags.TrueDescription).EscapeForJson()}}"
										}
										""");
										}
										sb.AppendLine($$"""
													]
													""");
										sb.AppendLine("}");
										break;
									}

								default:
									{
										// Handle other unexpected calls.
										break;
									}
							}

							requiresAction = true;
							break;
						}
					}
				}
				catch (Exception e)
				{
					e.ToString().Prepend("#2GPT Error#0\n").WriteLineConsole();
					Futuremud.Games.First().DiscordConnection.NotifyAdmins($"**GPT Error**\n\n```\n{e.ToString()}```");
				}
			});
		}
		while (requiresAction);
	}

	private void Cell_OnRoomEcho(ICell location, RoomLayer? layer, PerceptionEngine.IEmoteOutput emote)
	{
		if (IsPaused)
		{
			return;
		}

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
	private void HeartbeatManager_FiveMinuteHeartbeat() => throw new NotImplementedException();
	private void HeartbeatManager_FuzzyHourHeartbeat()
	{

	}

	public void UnsubscribeEvents()
	{
		Gameworld.HeartbeatManager.FuzzyHourHeartbeat -= HeartbeatManager_FuzzyHourHeartbeat;
		foreach (var cell in _subscribedCells)
		{
			cell.OnRoomEmoteEcho -= Cell_OnRoomEcho;
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

	public void Pause() { IsPaused = true; Changed = true; }
	public void Unpause() { IsPaused = false; Changed = true; }

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
