using MudSharp.Character;
using MudSharp.Character.Name;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Concrete;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;
using MudSharp.TimeAndDate;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Time;
using OpenAI.Responses;
using System;
using System.ClientModel.Primitives;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

#pragma warning disable OPENAI001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

namespace MudSharp.RPG.AIStorytellers;

public partial class AIStoryteller
{
    private void AddUniversalToolsToResponseOptions(CreateResponseOptions options, StorytellerToolProfile toolProfile)
    {
        bool includeExtendedWorldTools = toolProfile == StorytellerToolProfile.Full;

        AddFunctionTool(
            options,
            "CreateSituation",
            "Creates a new situation for the AI storyteller to manage.",
            """
			{
			  "type": "object",
			  "properties": {
			    "Title": { "type": "string", "description": "The title of the situation." },
			    "Description": { "type": "string", "description": "The detailed text of the situation." },
			    "CharacterId": { "type": "integer", "description": "Optional character scope id. Use either CharacterId or RoomId, not both." },
			    "RoomId": { "type": "integer", "description": "Optional room scope id. Use either CharacterId or RoomId, not both." }
			  },
			  "required": ["Title", "Description"]
			}
			""");

        AddFunctionTool(
            options,
            "UpdateSituation",
            "Updates an existing situation.",
            """
			{
			  "type": "object",
			  "properties": {
			    "Id": { "type": "integer", "description": "The id of the situation to update." },
			    "Title": { "type": "string", "description": "The updated title." },
			    "Description": { "type": "string", "description": "The updated detailed text." },
			    "CharacterId": { "type": "integer", "description": "Optional character scope id. If supplied, updates scope. Use either CharacterId or RoomId." },
			    "RoomId": { "type": "integer", "description": "Optional room scope id. If supplied, updates scope. Use either CharacterId or RoomId." }
			  },
			  "required": ["Id", "Title", "Description"]
			}
			""");

        AddFunctionTool(
            options,
            "ResolveSituation",
            "Resolves an existing situation.",
            """
			{
			  "type": "object",
			  "properties": {
			    "Id": { "type": "integer", "description": "The id of the situation to resolve." },
			    "Title": { "type": "string", "description": "The final title." },
			    "Description": { "type": "string", "description": "The final detailed text." },
			    "CharacterId": { "type": "integer", "description": "Optional character scope id. If supplied, updates scope before resolving. Use either CharacterId or RoomId." },
			    "RoomId": { "type": "integer", "description": "Optional room scope id. If supplied, updates scope before resolving. Use either CharacterId or RoomId." }
			  },
			  "required": ["Id", "Title", "Description"]
			}
			""");

        AddFunctionTool(
            options,
            "ShowSituation",
            "Shows full details of a specific situation by id.",
            """
			{
			  "type": "object",
			  "properties": {
			    "Id": { "type": "integer", "description": "The id of the situation to retrieve." }
			  },
			  "required": ["Id"]
			}
			""");

        AddFunctionTool(options, "Noop",
            "Use this when no side-effect is required but a tool response is needed.", null);
        AddFunctionTool(options, "OnlinePlayers",
            "Returns summary information about online player characters.", null);

        AddFunctionTool(
            options,
            "PlayerInformation",
            "Returns detailed information about a specific player character.",
            """
			{
			  "type": "object",
			  "properties": {
			    "Id": { "type": "integer", "description": "The id of the player character." }
			  },
			  "required": ["Id"]
			}
			""");

        AddFunctionTool(
            options,
            "ListMemoriesForPlayer",
            "Returns a list of memories about a specific player character.",
            """
			{
			  "type": "object",
			  "properties": {
			    "Id": { "type": "integer", "description": "The id of the player character." }
			  },
			  "required": ["Id"]
			}
			""");

        AddFunctionTool(
            options,
            "ShowMemory",
            "Shows the full detail of a specific character memory.",
            """
			{
			  "type": "object",
			  "properties": {
			    "Id": { "type": "integer", "description": "The id of the memory to retrieve. Usually known through use of the ListMemoriesForPlayer tool" }
			  },
			  "required": ["Id"]
			}
			""");

        AddFunctionTool(
            options,
            "CreateMemory",
            "Creates a new memory about a player character.",
            """
			{
			  "type": "object",
			  "properties": {
			    "Id": { "type": "integer", "description": "The id of the target character." },
			    "Title": { "type": "string", "description": "The memory title." },
			    "Details": { "type": "string", "description": "The memory details." }
			  },
			  "required": ["Id", "Title", "Details"]
			}
			""");

        AddFunctionTool(
            options,
            "UpdateMemory",
            "Updates a memory.",
            """
			{
			  "type": "object",
			  "properties": {
			    "Id": { "type": "integer", "description": "The id of the memory." },
			    "Title": { "type": "string", "description": "The memory title." },
			    "Details": { "type": "string", "description": "The memory details." }
			  },
			  "required": ["Id", "Title", "Details"]
			}
			""");

        AddFunctionTool(
            options,
            "ForgetMemory",
            "Forgets (deletes) a memory.",
            """
			{
			  "type": "object",
			  "properties": {
			    "Id": { "type": "integer", "description": "The id of the memory to forget." }
			  },
			  "required": ["Id"]
			}
			""");

        if (includeExtendedWorldTools)
        {
            AddFunctionTool(options, "Landmarks",
                "Returns summary information about available landmarks.", null);
            AddFunctionTool(
                options,
                "ShowLandmark",
                "Returns detailed information about a specific landmark.",
                """
				{
				  "type": "object",
				  "properties": {
				    "Id": { "type": "string", "description": "The name of the landmark." }
				  },
				  "required": ["Id"]
				}
				""");
        }

        AddFunctionTool(
            options,
            "SearchReferenceDocuments",
            "Searches AI storyteller reference documents.",
            """
			{
			  "type": "object",
			  "properties": {
			    "Query": { "type": "string", "description": "Search text. Leave blank to list all." }
			  },
			  "required": []
			}
			""");

        AddFunctionTool(
            options,
            "ShowReferenceDocument",
            "Returns a specific reference document in detail by id.",
            """
			{
			  "type": "object",
			  "properties": {
			    "Id": { "type": "integer", "description": "The id of the reference document." }
			  },
			  "required": ["Id"]
			}
			""");

        if (includeExtendedWorldTools)
        {
            AddFunctionTool(
                options,
                "PathBetweenRooms",
                "Returns a list of movement commands to path between two rooms.",
                """
				{
				  "type": "object",
				  "properties": {
				    "OriginRoomId": { "type": "integer", "description": "The id of the origin room." },
				    "DestinationRoomId": { "type": "integer", "description": "The id of the destination room." },
				    "PathSearchFunction": { "type": "string", "description": "Path search mode. Valid values include RespectClosedDoors, IncludeUnlockedDoors, IncludeFireableDoors, IgnorePresenceOfDoors, PathIgnoreDoors, PathRespectClosedDoors and PathIncludeUnlockedDoors." }
				  },
				  "required": ["OriginRoomId", "DestinationRoomId", "PathSearchFunction"]
				}
				""");

            AddFunctionTool(
                options,
                "PathFromCharacterToRoom",
                "Returns a list of movement commands to path from a character's current room to a destination room.",
                """
				{
				  "type": "object",
				  "properties": {
				    "OriginCharacterId": { "type": "integer", "description": "The id of the origin character." },
				    "DestinationRoomId": { "type": "integer", "description": "The id of the destination room." },
				    "PathSearchFunction": { "type": "string", "description": "Path search mode. Valid values include IncludeUnlockableDoors, PathIncludeUnlockableDoors, PathIgnoreDoors, PathRespectClosedDoors, PathIncludeUnlockedDoors, RespectClosedDoors, IncludeUnlockedDoors, IncludeFireableDoors and IgnorePresenceOfDoors." }
				  },
				  "required": ["OriginCharacterId", "DestinationRoomId", "PathSearchFunction"]
				}
				""");

            AddFunctionTool(
                options,
                "PathBetweenCharacters",
                "Returns a list of movement commands to path between two characters using their current rooms.",
                """
				{
				  "type": "object",
				  "properties": {
				    "OriginCharacterId": { "type": "integer", "description": "The id of the origin character." },
				    "DestinationCharacterId": { "type": "integer", "description": "The id of the destination character." },
				    "PathSearchFunction": { "type": "string", "description": "Path search mode. Valid values include IncludeUnlockableDoors, PathIncludeUnlockableDoors, PathIgnoreDoors, PathRespectClosedDoors, PathIncludeUnlockedDoors, RespectClosedDoors, IncludeUnlockedDoors, IncludeFireableDoors and IgnorePresenceOfDoors." }
				  },
				  "required": ["OriginCharacterId", "DestinationCharacterId", "PathSearchFunction"]
				}
				""");

            AddFunctionTool(options, "RecentCharacterPlans",
                "Returns plans for online characters who recently updated plans (90 day window semantics).", null);

            AddFunctionTool(
                options,
                "CharacterPlans",
                "Returns plans for a specific character by id.",
                """
				{
				  "type": "object",
				  "properties": {
				    "Id": { "type": "integer", "description": "The id of the character." }
				  },
				"required": ["Id"]
				}
				""");
        }

        AddFunctionTool(options, "CurrentDateTime",
            "Returns the current in-game date, time and timezone for a default monitored context when only one calendar/clock/timezone is in active use.",
            null);

        AddFunctionTool(
            options,
            "DateTimeForTarget",
            "Returns the current in-game date and time for a specific character or room (or defaults when only one context exists).",
            """
			{
			  "type": "object",
			  "properties": {
			    "CharacterId": { "type": "integer", "description": "Optional character id to resolve date and time for." },
			    "RoomId": { "type": "integer", "description": "Optional room id to resolve date and time for." }
			  },
			  "required": []
			}
			""");

        if (includeExtendedWorldTools)
        {
            AddFunctionTool(
                options,
                "CalendarDefinition",
                "Returns detailed definition information for a calendar, including months and intercalary months.",
                """
				{
				  "type": "object",
				  "properties": {
				    "Id": { "type": "string", "description": "The calendar id, alias or name." },
				    "Year": { "type": "integer", "description": "Optional year number to expand year-specific month data." }
				  },
				  "required": ["Id"]
				}
				""");
        }
    }

    private void AddFunctionTool(CreateResponseOptions options, string functionName, string functionDescription,
        string functionParametersJson)
    {
        options.Tools.Add(ResponseTool.CreateFunctionTool(
            functionName: functionName,
            functionDescription: functionDescription,
            functionParameters: BinaryData.FromString(
                NormalizeFunctionToolSchema(functionParametersJson)),
            strictModeEnabled: true
        ));
    }

    private const string EmptyStrictSchema = """
		{
		  "type": "object",
		  "properties": {},
		  "required": [],
		  "additionalProperties": false
		}
		""";

    internal static string NormalizeFunctionToolSchema(string functionParametersJson)
    {
        if (string.IsNullOrWhiteSpace(functionParametersJson))
        {
            return EmptyStrictSchema;
        }

        using JsonDocument document = JsonDocument.Parse(functionParametersJson);
        using MemoryStream stream = new();
        using (Utf8JsonWriter writer = new(stream))
        {
            WriteStrictSchemaElement(document.RootElement, writer);
        }

        return Encoding.UTF8.GetString(stream.ToArray());
    }

    private static void WriteStrictSchemaElement(JsonElement element, Utf8JsonWriter writer,
        bool forceNullable = false)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                {
                    List<JsonProperty> schemaProperties = element.EnumerateObject().ToList();
                    bool isObjectSchema = schemaProperties.Any(x =>
                    x.NameEquals("properties") ||
                    (x.NameEquals("type") && JsonTypeRepresentsObject(x.Value)));
                    HashSet<string> requiredPropertyNames = isObjectSchema
                        ? GetRequiredSchemaPropertyNames(schemaProperties)
                        : [];
                    List<string> declaredPropertyNames = isObjectSchema
                        ? GetDeclaredSchemaPropertyNames(schemaProperties)
                        : [];
                    bool typeWritten = false;

                    writer.WriteStartObject();
                    foreach (JsonProperty property in schemaProperties)
                    {
                        if (isObjectSchema && property.NameEquals("additionalProperties"))
                        {
                            continue;
                        }

                        if (isObjectSchema && property.NameEquals("required"))
                        {
                            continue;
                        }

                        if (forceNullable && property.NameEquals("type"))
                        {
                            writer.WritePropertyName("type");
                            WriteTypeWithNull(property.Value, writer);
                            typeWritten = true;
                            continue;
                        }

                        if (isObjectSchema && property.NameEquals("properties") &&
                            property.Value.ValueKind == JsonValueKind.Object)
                        {
                            writer.WritePropertyName("properties");
                            writer.WriteStartObject();
                            foreach (JsonProperty declaredProperty in property.Value.EnumerateObject())
                            {
                                writer.WritePropertyName(declaredProperty.Name);
                                WriteStrictSchemaElement(
                                    declaredProperty.Value,
                                    writer,
                                    forceNullable: !requiredPropertyNames.Contains(declaredProperty.Name));
                            }

                            writer.WriteEndObject();
                            continue;
                        }

                        writer.WritePropertyName(property.Name);
                        WriteStrictSchemaElement(property.Value, writer);
                    }

                    if (forceNullable && !typeWritten && isObjectSchema)
                    {
                        writer.WritePropertyName("type");
                        writer.WriteStartArray();
                        writer.WriteStringValue("object");
                        writer.WriteStringValue("null");
                        writer.WriteEndArray();
                    }

                    if (isObjectSchema)
                    {
                        writer.WritePropertyName("required");
                        writer.WriteStartArray();
                        foreach (string propertyName in declaredPropertyNames)
                        {
                            writer.WriteStringValue(propertyName);
                        }

                        writer.WriteEndArray();
                        writer.WriteBoolean("additionalProperties", false);
                    }

                    writer.WriteEndObject();
                    return;
                }
            case JsonValueKind.Array:
                writer.WriteStartArray();
                foreach (JsonElement item in element.EnumerateArray())
                {
                    WriteStrictSchemaElement(item, writer);
                }

                writer.WriteEndArray();
                return;
            default:
                element.WriteTo(writer);
                return;
        }
    }

    private static List<string> GetDeclaredSchemaPropertyNames(IEnumerable<JsonProperty> schemaProperties)
    {
        JsonProperty propertiesProperty = schemaProperties.FirstOrDefault(x => x.NameEquals("properties"));
        if (propertiesProperty.Value.ValueKind != JsonValueKind.Object)
        {
            return [];
        }

        return propertiesProperty.Value.EnumerateObject().Select(x => x.Name).ToList();
    }

    private static HashSet<string> GetRequiredSchemaPropertyNames(IEnumerable<JsonProperty> schemaProperties)
    {
        JsonProperty requiredProperty = schemaProperties.FirstOrDefault(x => x.NameEquals("required"));
        if (requiredProperty.Value.ValueKind != JsonValueKind.Array)
        {
            return [];
        }

        return requiredProperty.Value
            .EnumerateArray()
            .Where(x => x.ValueKind == JsonValueKind.String)
            .Select(x => x.GetString())
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Cast<string>()
            .ToHashSet(StringComparer.Ordinal);
    }

    private static void WriteTypeWithNull(JsonElement typeElement, Utf8JsonWriter writer)
    {
        switch (typeElement.ValueKind)
        {
            case JsonValueKind.String:
                {
                    string schemaType = typeElement.GetString() ?? string.Empty;
                    writer.WriteStartArray();
                    if (!string.IsNullOrWhiteSpace(schemaType))
                    {
                        writer.WriteStringValue(schemaType);
                    }

                    if (!schemaType.EqualTo("null"))
                    {
                        writer.WriteStringValue("null");
                    }

                    writer.WriteEndArray();
                    return;
                }
            case JsonValueKind.Array:
                {
                    bool hasNull = false;
                    writer.WriteStartArray();
                    foreach (JsonElement typeOption in typeElement.EnumerateArray())
                    {
                        if (typeOption.ValueKind == JsonValueKind.String && typeOption.GetString().EqualTo("null"))
                        {
                            hasNull = true;
                        }

                        typeOption.WriteTo(writer);
                    }

                    if (!hasNull)
                    {
                        writer.WriteStringValue("null");
                    }

                    writer.WriteEndArray();
                    return;
                }
            default:
                typeElement.WriteTo(writer);
                return;
        }
    }

    private static bool JsonTypeRepresentsObject(JsonElement typeElement)
    {
        switch (typeElement.ValueKind)
        {
            case JsonValueKind.String:
                return string.Equals(typeElement.GetString(), "object", StringComparison.OrdinalIgnoreCase);
            case JsonValueKind.Array:
                return typeElement.EnumerateArray().Any(x =>
                    x.ValueKind == JsonValueKind.String &&
                    string.Equals(x.GetString(), "object", StringComparison.OrdinalIgnoreCase));
            default:
                return false;
        }
    }

    private void AddCustomToolCallsToResponseOptions(CreateResponseOptions options, bool includeEchoes)
    {
        AddCustomToolCallsToResponseOptions(options, CustomToolCalls);
        if (includeEchoes)
        {
            AddCustomToolCallsToResponseOptions(options, CustomToolCallsEchoOnly);
        }
    }

    private void AddCustomToolCallsToResponseOptions(CreateResponseOptions options,
        IEnumerable<AIStorytellerCustomToolCall> toolCalls)
    {
        foreach (AIStorytellerCustomToolCall toolCall in toolCalls
                     .Where(x => x.IsValid)
                     .OrderBy(x => x.Name, StringComparer.OrdinalIgnoreCase))
        {
            string schema = BuildCustomToolCallSchema(toolCall);
            AddFunctionTool(options, toolCall.Name, toolCall.Description, schema);
        }
    }


    internal readonly record struct ToolExecutionResult(string OutputJson, bool MalformedJson);
    internal Action<string>? ErrorLoggerOverride { get; set; }
    internal Action<Exception>? ExceptionLoggerOverride { get; set; }
    private const string MalformedToolCallFeedbackMessage =
        "One or more tool calls used malformed JSON. Retry with valid JSON arguments that exactly match the declared tool schemas.";

    internal void ConfigureToolLoopResponseOptions(CreateResponseOptions options, bool includeEchoTools,
        bool requireToolCall, StorytellerToolProfile toolProfile)
    {
        options.ReasoningOptions ??= new();
        options.ReasoningOptions.ReasoningEffortLevel = ReasoningEffort;
        options.MaxOutputTokenCount = MaxStorytellerOutputTokens;
        options.ParallelToolCallsEnabled = true;
        options.ToolChoice = requireToolCall
            ? ResponseToolChoice.CreateRequiredChoice()
            : ResponseToolChoice.CreateAutoChoice();
        AddUniversalToolsToResponseOptions(options, toolProfile);
        AddCustomToolCallsToResponseOptions(options, includeEchoTools);
    }

    internal static bool IsNoopOnlyFunctionCallBatch(IEnumerable<string?> functionNames)
    {
        List<string> names = functionNames.Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
        return names.Any() && names.All(x => x!.EqualTo("Noop"));
    }

    private void ExecuteToolCall(ResponsesClient client, List<ResponseItem> messages, bool includeEchoTools,
        StorytellerToolProfile toolProfile)
    {
        DateTime started = DateTime.UtcNow;
        int malformedRetries = 0;
        int missingToolCallRetries = 0;
        bool hasObservedToolCall = false;

        for (int depth = 0; depth < MaxToolCallDepth; depth++)
        {
            if (DateTime.UtcNow - started > MaxToolExecutionDuration)
            {
                LogStorytellerError("Tool execution duration exceeded safety budget.");
                return;
            }

            try
            {
                CreateResponseOptions options = new(messages);
                bool requireToolCall = !hasObservedToolCall;
                ConfigureToolLoopResponseOptions(options, includeEchoTools, requireToolCall, toolProfile);
                DebugAIMessaging("Engine -> Storyteller Continuation Request",
                    $"Round {depth + 1:N0}/{MaxToolCallDepth:N0}, Include Echo Tools: {includeEchoTools}, Tool Profile: {toolProfile}, Require Tool Call: {requireToolCall}, Context Messages: {messages.Count:N0}");

                ResponseResult response = client.CreateResponseAsync(options).GetAwaiter().GetResult().Value;
                DebugResponseUsage("Storyteller -> Engine Usage", response);
                DebugAIMessaging("Storyteller -> Engine Response", response.GetOutputText());
                List<FunctionCallResponseItem> functionCalls = response.OutputItems.OfType<FunctionCallResponseItem>().ToList();
                if (functionCalls.Any())
                {
                    messages.AddRange(response.OutputItems);
                    DebugAIMessaging("Storyteller -> Engine Tool Requests",
                        string.Join(
                            "\n\n",
                            functionCalls.Select(x =>
                                $"""
Function: {x.FunctionName}
Call Id: {x.CallId.IfNullOrWhiteSpace(x.Id).IfNullOrWhiteSpace("(none)")}
Arguments:
{x.FunctionArguments.ToString().IfNullOrWhiteSpace("{}")}
""")));
                }
                if (!functionCalls.Any())
                {
                    if (!hasObservedToolCall)
                    {
                        missingToolCallRetries++;
                        if (missingToolCallRetries > MaxMissingToolCallRetries)
                        {
                            LogStorytellerError(
                                $"Storyteller failed to produce a required tool call after {MaxMissingToolCallRetries:N0} retries.");
                            return;
                        }

                        messages.Add(ResponseItem.CreateUserMessageItem(MissingToolCallFeedbackMessage));
                        DebugAIMessaging("Engine -> Storyteller Retry Feedback",
                            $"""
Missing tool-call retry {missingToolCallRetries:N0}/{MaxMissingToolCallRetries:N0}
{MissingToolCallFeedbackMessage}
""");
                        continue;
                    }

                    DebugAIMessaging("Storyteller Tool Loop Complete",
                        $"Round {depth + 1:N0} returned no function calls.");
                    return;
                }

                hasObservedToolCall = true;
                missingToolCallRetries = 0;
                bool noopOnlyBatch = IsNoopOnlyFunctionCallBatch(functionCalls.Select(x => x.FunctionName));
                (bool shouldContinue, int retries) = ProcessFunctionCallBatch(
                    functionCalls.Select(x => (
                        x.CallId.IfNullOrWhiteSpace(x.Id),
                        x.FunctionName,
                        x.FunctionArguments.ToString())),
                    includeEchoTools,
                    messages,
                    malformedRetries);
                malformedRetries = retries;
                if (!shouldContinue)
                {
                    return;
                }

                if (noopOnlyBatch)
                {
                    DebugAIMessaging("Storyteller Tool Loop Complete",
                        $"Round {depth + 1:N0} completed via Noop tool call.");
                    return;
                }
            }
            catch (Exception e)
            {
                LogStorytellerException(e);
                return;
            }
        }

        LogStorytellerError($"Tool call depth exceeded safety budget of {MaxToolCallDepth:N0} rounds.");
    }

    private void LogStorytellerError(string message)
    {
        ErrorLoggerOverride?.Invoke(message);
        DebugAIMessaging("Storyteller Error", message);
        string formattedMessage = $"Storyteller {Id:N0}: {message}";
        formattedMessage.Prepend("#2GPT Error#0\n").WriteLineConsole();
        try
        {
            Futuremud.Games.FirstOrDefault()?.DiscordConnection?.NotifyAdmins(
                $"**GPT Error**\n\n```\n{formattedMessage}```");
        }
        catch
        {
            // Best-effort logging only.
        }
    }

    private void LogStorytellerException(Exception e)
    {
        ExceptionLoggerOverride?.Invoke(e);
        DebugAIMessaging("Storyteller Exception", e.ToString());
        e.ToString().Prepend("#2GPT Error#0\n").WriteLineConsole();
        try
        {
            Futuremud.Games.FirstOrDefault()?.DiscordConnection?.NotifyAdmins($"**GPT Error**\n\n```\n{e}```");
        }
        catch
        {
            // Best-effort logging only.
        }
    }

    internal (bool Continue, int MalformedRetries) ProcessFunctionCallBatch(
        IEnumerable<(string CallId, string FunctionName, string? ArgumentsJson)> functionCalls,
        bool includeEchoTools,
        List<ResponseItem> messages,
        int malformedRetries)
    {
        bool malformedThisRound = false;
        foreach ((string CallId, string FunctionName, string ArgumentsJson) functionCall in functionCalls)
        {
            string callId = string.IsNullOrWhiteSpace(functionCall.CallId)
                ? Guid.NewGuid().ToString("N")
                : functionCall.CallId;
            DebugAIMessaging("Engine Executing Tool Call",
                $"""
Function: {functionCall.FunctionName}
Call Id: {callId}
Arguments:
{functionCall.ArgumentsJson.IfNullOrWhiteSpace("{}")}
""");
            ToolExecutionResult result = ExecuteFunctionCall(functionCall.FunctionName, functionCall.ArgumentsJson, includeEchoTools);
            malformedThisRound |= result.MalformedJson;
            messages.Add(ResponseItem.CreateFunctionCallOutputItem(callId, result.OutputJson));
            DebugAIMessaging("Engine -> Storyteller Tool Output",
                $"""
Function: {functionCall.FunctionName}
Call Id: {callId}
Malformed Json: {result.MalformedJson}
Output:
{result.OutputJson}
""");
        }

        if (!malformedThisRound)
        {
            return (true, 0);
        }

        malformedRetries++;
        if (malformedRetries > MaxMalformedToolCallRetries)
        {
            LogStorytellerError("Malformed tool-call JSON retry budget exceeded.");
            return (false, malformedRetries);
        }

        messages.Add(ResponseItem.CreateUserMessageItem(MalformedToolCallFeedbackMessage));
        DebugAIMessaging("Engine -> Storyteller Retry Feedback",
            $"""
Malformed JSON retry {malformedRetries:N0}/{MaxMalformedToolCallRetries:N0}
{MalformedToolCallFeedbackMessage}
""");
        return (true, malformedRetries);
    }

    internal ToolExecutionResult ExecuteFunctionCall(string functionName, string? argumentsText, bool includeEchoTools)
    {
        JsonDocument? argumentsJson = null;
        try
        {
            if (string.IsNullOrWhiteSpace(argumentsText))
            {
                argumentsText = "{}";
            }

            argumentsJson = JsonDocument.Parse(argumentsText);
            JsonElement arguments = argumentsJson.RootElement;
            return functionName switch
            {
                "Noop" => SuccessResult(new Dictionary<string, object>
                {
                    ["message"] = "No action taken."
                }),
                "CreateSituation" => HandleCreateSituation(arguments),
                "UpdateSituation" => HandleUpdateSituation(arguments),
                "ResolveSituation" => HandleResolveSituation(arguments),
                "ShowSituation" => HandleShowSituation(arguments),
                "OnlinePlayers" => HandleOnlinePlayers(),
                "PlayerInformation" => HandlePlayerInformation(arguments),
                "CreateMemory" => HandleCreateMemory(arguments),
                "UpdateMemory" => HandleUpdateMemory(arguments),
                "ShowMemory" => HandleShowMemory(arguments),
                "ListMemoriesForPlayer" => HandleListMemoriesForPlayer(arguments),
                "ForgetMemory" => HandleForgetMemory(arguments),
                "Landmarks" => HandleLandmarks(),
                "ShowLandmark" => HandleShowLandmark(arguments),
                "SearchReferenceDocuments" => HandleSearchReferenceDocuments(arguments),
                "ShowReferenceDocument" => HandleShowReferenceDocument(arguments),
                "PathBetweenRooms" => HandlePathBetweenRooms(arguments),
                "PathFromCharacterToRoom" => HandlePathFromCharacterToRoom(arguments),
                "PathBetweenCharacters" => HandlePathBetweenCharacters(arguments),
                "RecentCharacterPlans" => HandleRecentCharacterPlans(),
                "CharacterPlans" => HandleCharacterPlans(arguments),
                "CurrentDateTime" => HandleCurrentDateTime(),
                "DateTimeForTarget" => HandleDateTimeForTarget(arguments),
                "CalendarDefinition" => HandleCalendarDefinition(arguments),
                _ => HandleCustomFunctionCall(functionName, arguments, includeEchoTools)
            };
        }
        catch (JsonException e)
        {
            return ErrorResult($"Malformed tool-call JSON: {e.Message}", malformedJson: true);
        }
        catch (Exception e)
        {
            return ErrorResult($"Tool handler error: {e.Message}");
        }
        finally
        {
            argumentsJson?.Dispose();
        }
    }

    private ToolExecutionResult SuccessResult(object payload)
    {
        return new ToolExecutionResult(JsonSerializer.Serialize(new Dictionary<string, object>
        {
            ["ok"] = true,
            ["result"] = ConvertToToolOutputValue(payload)
        }), false);
    }

    private static ToolExecutionResult ErrorResult(string error, bool malformedJson = false)
    {
        return new ToolExecutionResult(JsonSerializer.Serialize(new Dictionary<string, object>
        {
            ["ok"] = false,
            ["error"] = error
        }), malformedJson);
    }

    private bool TryResolveSituationScope(JsonElement arguments, out bool scopeSpecified, out long? scopeCharacterId,
        out long? scopeRoomId, out string error)
    {
        scopeSpecified = false;
        scopeCharacterId = null;
        scopeRoomId = null;
        error = string.Empty;

        bool hasCharacterScope = TryGetOptionalLong(arguments, "CharacterId", out long parsedCharacterId);
        bool hasRoomScope = TryGetOptionalLong(arguments, "RoomId", out long parsedRoomId);
        if (hasCharacterScope && parsedCharacterId <= 0)
        {
            error = "CharacterId must be a positive integer.";
            return false;
        }

        if (hasRoomScope && parsedRoomId <= 0)
        {
            error = "RoomId must be a positive integer.";
            return false;
        }

        if (hasCharacterScope && hasRoomScope)
        {
            error = "Specify either CharacterId or RoomId, but not both.";
            return false;
        }

        if (hasCharacterScope)
        {
            ICharacter character = Gameworld.TryGetCharacter(parsedCharacterId, true);
            if (character is null)
            {
                error = $"No character with id {parsedCharacterId:N0} exists.";
                return false;
            }

            scopeCharacterId = character.Id;
            scopeSpecified = true;
            return true;
        }

        if (hasRoomScope)
        {
            ICell room = Gameworld.Cells.Get(parsedRoomId);
            if (room is null)
            {
                error = $"No room with id {parsedRoomId:N0} exists.";
                return false;
            }

            scopeRoomId = room.Id;
            scopeSpecified = true;
        }

        return true;
    }

    private static void AddSituationScopeToPayload(IDictionary<string, object> payload, IAIStorytellerSituation situation)
    {
        payload["CharacterId"] = situation.ScopeCharacterId;
        payload["RoomId"] = situation.ScopeRoomId;
    }


    private ToolExecutionResult HandleCreateSituation(JsonElement arguments)
    {
        if (!TryGetRequiredString(arguments, "Title", out string title, out string error))
        {
            return ErrorResult(error);
        }

        if (!TryGetRequiredString(arguments, "Description", out string description, out error))
        {
            return ErrorResult(error);
        }

        if (!TryResolveSituationScope(arguments, out _, out long? scopeCharacterId, out long? scopeRoomId, out error))
        {
            return ErrorResult(error);
        }

        AIStorytellerSituation situation = new(Gameworld, this, title, description, scopeCharacterId, scopeRoomId);
        _situations.Add(situation);
        Dictionary<string, object> payload = new()
        {
            ["Id"] = situation.Id,
            ["Title"] = situation.Name,
            ["CreatedOn"] = situation.CreatedOn.ToString("O"),
            ["Resolved"] = situation.IsResolved
        };
        AddSituationScopeToPayload(payload, situation);
        return SuccessResult(payload);
    }

    private ToolExecutionResult HandleUpdateSituation(JsonElement arguments)
    {
        if (!TryGetRequiredLong(arguments, "Id", out long id, out string error))
        {
            return ErrorResult(error);
        }

        if (!TryGetRequiredString(arguments, "Title", out string title, out error))
        {
            return ErrorResult(error);
        }

        if (!TryGetRequiredString(arguments, "Description", out string description, out error))
        {
            return ErrorResult(error);
        }

        IAIStorytellerSituation situation = _situations.FirstOrDefault(x => x.Id == id);
        if (situation is null)
        {
            return ErrorResult($"No situation with id {id:N0} exists.");
        }

        if (!TryResolveSituationScope(arguments, out bool scopeSpecified, out long? scopeCharacterId, out long? scopeRoomId,
                out error))
        {
            return ErrorResult(error);
        }

        situation.UpdateSituation(title, description);
        if (scopeSpecified)
        {
            situation.SetScope(scopeCharacterId, scopeRoomId);
        }

        Dictionary<string, object> payload = new()
        {
            ["Id"] = situation.Id,
            ["Title"] = situation.Name,
            ["Resolved"] = situation.IsResolved
        };
        AddSituationScopeToPayload(payload, situation);
        return SuccessResult(payload);
    }

    private ToolExecutionResult HandleResolveSituation(JsonElement arguments)
    {
        if (!TryGetRequiredLong(arguments, "Id", out long id, out string error))
        {
            return ErrorResult(error);
        }

        if (!TryGetRequiredString(arguments, "Title", out string title, out error))
        {
            return ErrorResult(error);
        }

        if (!TryGetRequiredString(arguments, "Description", out string description, out error))
        {
            return ErrorResult(error);
        }

        IAIStorytellerSituation situation = _situations.FirstOrDefault(x => x.Id == id);
        if (situation is null)
        {
            return ErrorResult($"No situation with id {id:N0} exists.");
        }

        if (!TryResolveSituationScope(arguments, out bool scopeSpecified, out long? scopeCharacterId, out long? scopeRoomId,
                out error))
        {
            return ErrorResult(error);
        }

        situation.UpdateSituation(title, description);
        if (scopeSpecified)
        {
            situation.SetScope(scopeCharacterId, scopeRoomId);
        }

        situation.Resolve();
        Dictionary<string, object> payload = new()
        {
            ["Id"] = situation.Id,
            ["Title"] = situation.Name,
            ["Resolved"] = situation.IsResolved
        };
        AddSituationScopeToPayload(payload, situation);
        return SuccessResult(payload);
    }

    private ToolExecutionResult HandleShowSituation(JsonElement arguments)
    {
        if (!TryGetRequiredLong(arguments, "Id", out long id, out string error))
        {
            return ErrorResult(error);
        }

        IAIStorytellerSituation situation = _situations.FirstOrDefault(x => x.Id == id);
        if (situation is null)
        {
            return ErrorResult($"No situation with id {id:N0} exists.");
        }

        Dictionary<string, object> payload = new()
        {
            ["Id"] = situation.Id,
            ["Title"] = situation.Name,
            ["Description"] = situation.SituationText,
            ["CreatedOn"] = situation.CreatedOn.ToString("O"),
            ["Resolved"] = situation.IsResolved
        };
        AddSituationScopeToPayload(payload, situation);
        return SuccessResult(payload);
    }

    private ToolExecutionResult HandleOnlinePlayers()
    {
        List<Dictionary<string, object>> players = Gameworld.Characters
            .Where(x => x.IsPlayerCharacter)
            .Select(x => new Dictionary<string, object>
            {
                ["Id"] = x.Id,
                ["Name"] = x.PersonalName.GetName(NameStyle.FullName),
                ["Gender"] = x.Gender.GenderClass(),
                ["ShortDescription"] = x.HowSeen(null, colour: false, flags: PerceiveIgnoreFlags.TrueDescription),
                ["LocationId"] = x.Location?.Id ?? 0L,
                ["LocationName"] = x.Location?.HowSeen(null, colour: false) ?? "Unknown",
                ["NumberOfMemories"] = _characterMemories.Count(y => y.Character.Id == x.Id)
            })
            .ToList();

        return SuccessResult(new Dictionary<string, object>
        {
            ["Players"] = players
        });
    }

    private ToolExecutionResult HandleShowMemory(JsonElement arguments)
    {
        if (!TryGetRequiredLong(arguments, "Id", out long id, out string error))
        {
            return ErrorResult(error);
        }

        IAIStorytellerCharacterMemory memory = _characterMemories.FirstOrDefault(x => x.Id == id);
        if (memory is null)
        {
            return ErrorResult($"No memory with id {id:N0} exists.");
        }

        return SuccessResult(new Dictionary<string, object>
        {
            ["Id"] = memory.Id,
            ["Title"] = memory.MemoryTitle,
            ["Description"] = memory.MemoryText,
            ["CreatedOn"] = memory.CreatedOn.ToString("O")
        });
    }

    private ToolExecutionResult HandleListMemoriesForPlayer(JsonElement arguments)
    {
        if (!TryGetRequiredLong(arguments, "Id", out long id, out string error))
        {
            return ErrorResult(error);
        }

        ICharacter pc = Gameworld.TryGetCharacter(id, true);
        if (pc is null)
        {
            return ErrorResult($"No player character with id {id:N0} exists.");
        }

        Dictionary<string, object> result = new();
        result["Memories"] = _characterMemories
            .Where(x => x.Character.Id == pc.Id)
            .Select(x => new Dictionary<string, object>
            {
                ["Id"] = x.Id,
                ["Title"] = x.MemoryTitle,
                ["Details"] = x.MemoryText,
                ["CreatedOn"] = x.CreatedOn.ToString("O")
            })
            .ToList();

        return SuccessResult(result);
    }

    private ToolExecutionResult HandlePlayerInformation(JsonElement arguments)
    {
        if (!TryGetRequiredLong(arguments, "Id", out long id, out string error))
        {
            return ErrorResult(error);
        }

        ICharacter pc = Gameworld.TryGetCharacter(id, true);
        if (pc is null)
        {
            return ErrorResult($"No player character with id {id:N0} exists.");
        }

        Dictionary<string, object> result = new()
        {
            ["Id"] = pc.Id,
            ["Name"] = pc.PersonalName.GetName(NameStyle.FullName),
            ["Gender"] = pc.Gender.GenderClass(),
            ["Race"] = pc.Race.Name,
            ["Ethnicity"] = pc.Ethnicity.Name,
            ["Culture"] = pc.Culture.Name,
            ["Age"] = pc.AgeInYears,
            ["AgeCategory"] = pc.AgeCategory.DescribeEnum(true),
            ["Birthday"] = pc.Birthday.Display(TimeAndDate.Date.CalendarDisplayMode.Long),
            ["ShortDescription"] = pc.HowSeen(null, colour: false, flags: PerceiveIgnoreFlags.TrueDescription),
            ["FullDescription"] =
                pc.HowSeen(null, type: Form.Shape.DescriptionType.Full, colour: false,
                    flags: PerceiveIgnoreFlags.TrueDescription),
            ["LocationId"] = pc.Location?.Id ?? 0L,
            ["LocationName"] = pc.Location?.HowSeen(null, colour: false) ?? "Unknown"
        };

        if (CustomPlayerInformationProg is not null)
        {
            try
            {
                IReadOnlyDictionary<string, string> customInfo = CustomPlayerInformationProg.ExecuteDictionary<string>(pc);
                foreach ((string key, string value) in customInfo)
                {
                    result[key] = value;
                }
            }
            catch (Exception e)
            {
                LogStorytellerError(
                    $"Custom player information prog failed for storyteller {Id:N0}: {e.Message}");
            }
        }

        result["Memories"] = _characterMemories
            .Where(x => x.Character.Id == pc.Id)
            .Select(x => new Dictionary<string, object>
            {
                ["Id"] = x.Id,
                ["Title"] = x.MemoryTitle,
                ["Details"] = x.MemoryText,
                ["CreatedOn"] = x.CreatedOn.ToString("O")
            })
            .ToList();

        return SuccessResult(result);
    }

    private ToolExecutionResult HandleCreateMemory(JsonElement arguments)
    {
        if (!TryGetRequiredLong(arguments, "Id", out long id, out string error))
        {
            return ErrorResult(error);
        }

        if (!TryGetRequiredString(arguments, "Title", out string title, out error))
        {
            return ErrorResult(error);
        }

        if (!TryGetRequiredString(arguments, "Details", out string details, out error))
        {
            return ErrorResult(error);
        }

        ICharacter player = Gameworld.TryGetCharacter(id, true);
        if (player is null)
        {
            return ErrorResult($"No character with id {id:N0} exists.");
        }

        AIStorytellerCharacterMemory memory = new(this, player, title, details);
        _characterMemories.Add(memory);
        return SuccessResult(new Dictionary<string, object>
        {
            ["Id"] = memory.Id,
            ["CharacterId"] = player.Id,
            ["Title"] = memory.MemoryTitle
        });
    }

    private ToolExecutionResult HandleUpdateMemory(JsonElement arguments)
    {
        if (!TryGetRequiredLong(arguments, "Id", out long id, out string error))
        {
            return ErrorResult(error);
        }

        if (!TryGetRequiredString(arguments, "Title", out string title, out error))
        {
            return ErrorResult(error);
        }

        if (!TryGetRequiredString(arguments, "Details", out string details, out error))
        {
            return ErrorResult(error);
        }

        IAIStorytellerCharacterMemory memory = _characterMemories.FirstOrDefault(x => x.Id == id);
        if (memory is null)
        {
            return ErrorResult($"No memory with id {id:N0} exists.");
        }

        memory.UpdateMemory(title, details);
        return SuccessResult(new Dictionary<string, object>
        {
            ["Id"] = memory.Id,
            ["Title"] = memory.MemoryTitle
        });
    }

    private ToolExecutionResult HandleForgetMemory(JsonElement arguments)
    {
        if (!TryGetRequiredLong(arguments, "Id", out long id, out string error))
        {
            return ErrorResult(error);
        }

        IAIStorytellerCharacterMemory memory = _characterMemories.FirstOrDefault(x => x.Id == id);
        if (memory is null)
        {
            return ErrorResult($"No memory with id {id:N0} exists.");
        }

        memory.Forget();
        _characterMemories.Remove(memory);
        return SuccessResult(new Dictionary<string, object>
        {
            ["Id"] = id,
            ["Forgot"] = true
        });
    }

    private ToolExecutionResult HandleLandmarks()
    {
        List<Dictionary<string, object>> landmarks = Gameworld.Cells
            .SelectNotNull(x => x.EffectsOfType<LandmarkEffect>().FirstOrDefault())
            .Select(x =>
            {
                ICell cell = (ICell)x.Owner;
                return new Dictionary<string, object>
                {
                    ["Id"] = x.Name,
                    ["RoomId"] = cell.Id,
                    ["RoomName"] = cell.HowSeen(null, colour: false)
                };
            })
            .ToList();

        return SuccessResult(new Dictionary<string, object>
        {
            ["Landmarks"] = landmarks
        });
    }

    private ToolExecutionResult HandleShowLandmark(JsonElement arguments)
    {
        if (!TryGetRequiredString(arguments, "Id", out string landmarkId, out string error))
        {
            return ErrorResult(error);
        }

        LandmarkEffect landmark = Gameworld.Cells
            .SelectNotNull(x => x.EffectsOfType<LandmarkEffect>().FirstOrDefault())
            .FirstOrDefault(x => x.Name.EqualTo(landmarkId));
        if (landmark is null)
        {
            return ErrorResult($"No landmark with id '{landmarkId}' exists.");
        }

        ICell cell = (ICell)landmark.Owner;
        List<Dictionary<string, object>> occupants = cell.Characters.Select(x => new Dictionary<string, object>
        {
            ["Id"] = x.Id,
            ["Name"] = x.PersonalName.GetName(NameStyle.FullName),
            ["Gender"] = x.Gender.GenderClass(),
            ["ShortDescription"] = x.HowSeen(null, colour: false, flags: PerceiveIgnoreFlags.TrueDescription)
        }).ToList();

        return SuccessResult(new Dictionary<string, object>
        {
            ["Id"] = landmark.Name,
            ["RoomId"] = cell.Id,
            ["RoomName"] = cell.HowSeen(null, colour: false),
            ["RoomDescription"] =
                cell.ProcessedFullDescription(null, PerceiveIgnoreFlags.TrueDescription, cell.CurrentOverlay),
            ["Details"] = landmark.LandmarkDescriptionTexts.Select(x => x.Text).ToList(),
            ["Occupants"] = occupants
        });
    }

    private ToolExecutionResult HandleSearchReferenceDocuments(JsonElement arguments)
    {
        string query = TryGetOptionalString(arguments, "Query") ?? string.Empty;
        List<Dictionary<string, object>> documents = Gameworld.AIStorytellerReferenceDocuments
            .Where(IsReferenceDocumentVisibleToStoryteller)
            .Where(x => x.ReturnForSearch(query))
            .Select(x => new Dictionary<string, object>
            {
                ["Id"] = x.Id,
                ["Name"] = x.Name,
                ["Folder"] = (x as AIStorytellerReferenceDocument)?.FolderName ?? string.Empty,
                ["Type"] = (x as AIStorytellerReferenceDocument)?.DocumentType ?? string.Empty,
                ["Keywords"] = (x as AIStorytellerReferenceDocument)?.Keywords ?? string.Empty,
                ["Description"] = (x as AIStorytellerReferenceDocument)?.Description ?? string.Empty
            })
            .ToList();

        return SuccessResult(new Dictionary<string, object>
        {
            ["Query"] = query,
            ["Documents"] = documents
        });
    }

    private ToolExecutionResult HandleShowReferenceDocument(JsonElement arguments)
    {
        if (!TryGetRequiredLong(arguments, "Id", out long id, out string error))
        {
            return ErrorResult(error);
        }

        IAIStorytellerReferenceDocument document = Gameworld.AIStorytellerReferenceDocuments.Get(id);
        if (document is null || !IsReferenceDocumentVisibleToStoryteller(document))
        {
            return ErrorResult($"No visible reference document with id {id:N0} exists.");
        }

        AIStorytellerReferenceDocument concrete = document as AIStorytellerReferenceDocument;
        return SuccessResult(new Dictionary<string, object>
        {
            ["Id"] = document.Id,
            ["Name"] = document.Name,
            ["Folder"] = concrete?.FolderName ?? string.Empty,
            ["Type"] = concrete?.DocumentType ?? string.Empty,
            ["Keywords"] = concrete?.Keywords ?? string.Empty,
            ["Description"] = concrete?.Description ?? string.Empty,
            ["Contents"] = concrete?.DocumentContents ?? string.Empty
        });
    }

    private static bool TryResolvePathSearchFunction(string pathSearchFunction, ICharacter? contextCharacter,
        out Func<ICellExit, bool> function, out string resolvedFunctionName, out string error)
    {
        function = PathSearch.IgnorePresenceOfDoors;
        resolvedFunctionName = pathSearchFunction;
        error = string.Empty;

        switch ((pathSearchFunction ?? string.Empty).Trim().ToLowerInvariant())
        {
            case "respectcloseddoors":
                function = PathSearch.RespectClosedDoors;
                resolvedFunctionName = nameof(PathSearch.RespectClosedDoors);
                return true;
            case "pathrespectcloseddoors":
                if (contextCharacter is null)
                {
                    function = PathSearch.RespectClosedDoors;
                    resolvedFunctionName = nameof(PathSearch.RespectClosedDoors);
                }
                else
                {
                    function = PathSearch.PathRespectClosedDoors(contextCharacter);
                    resolvedFunctionName = nameof(PathSearch.PathRespectClosedDoors);
                }

                return true;
            case "includeunlockeddoors":
                function = PathSearch.IncludeUnlockedDoors;
                resolvedFunctionName = nameof(PathSearch.IncludeUnlockedDoors);
                return true;
            case "pathincludeunlockeddoors":
                if (contextCharacter is null)
                {
                    function = PathSearch.IncludeUnlockedDoors;
                    resolvedFunctionName = nameof(PathSearch.IncludeUnlockedDoors);
                }
                else
                {
                    function = PathSearch.PathIncludeUnlockedDoors(contextCharacter);
                    resolvedFunctionName = nameof(PathSearch.PathIncludeUnlockedDoors);
                }

                return true;
            case "includefireabledoors":
                function = PathSearch.IncludeFireableDoors;
                resolvedFunctionName = nameof(PathSearch.IncludeFireableDoors);
                return true;
            case "ignorepresenceofdoors":
                function = PathSearch.IgnorePresenceOfDoors;
                resolvedFunctionName = nameof(PathSearch.IgnorePresenceOfDoors);
                return true;
            case "ignoredoors":
            case "pathignoredoors":
                if (contextCharacter is null)
                {
                    function = PathSearch.IgnorePresenceOfDoors;
                    resolvedFunctionName = nameof(PathSearch.IgnorePresenceOfDoors);
                }
                else
                {
                    function = PathSearch.PathIgnoreDoors(contextCharacter);
                    resolvedFunctionName = nameof(PathSearch.PathIgnoreDoors);
                }

                return true;
            case "includeunlockabledoors":
            case "pathincludeunlockabledoors":
                if (contextCharacter is null)
                {
                    error =
                        $"PathSearchFunction '{pathSearchFunction}' requires a character context. Use one of RespectClosedDoors, IncludeUnlockedDoors, IncludeFireableDoors or IgnorePresenceOfDoors.";
                    return false;
                }

                function = PathSearch.PathIncludeUnlockableDoors(contextCharacter);
                resolvedFunctionName = nameof(PathSearch.PathIncludeUnlockableDoors);
                return true;
        }

        error =
            $"Unknown PathSearchFunction '{pathSearchFunction}'. Valid values are RespectClosedDoors, IncludeUnlockedDoors, IncludeFireableDoors, IgnorePresenceOfDoors, PathIgnoreDoors, PathRespectClosedDoors, PathIncludeUnlockedDoors and PathIncludeUnlockableDoors.";
        return false;
    }

    private static List<string> ConvertPathToDirectionCommands(IEnumerable<ICellExit> path)
    {
        return path.Select(x =>
            x is NonCardinalCellExit nonCardinalExit
                ? $"{nonCardinalExit.Verb} {nonCardinalExit.PrimaryKeyword}".ToLowerInvariant()
                : x.OutboundDirection.DescribeBrief()).ToList();
    }

    private ToolExecutionResult HandlePathBetweenRooms(JsonElement arguments)
    {
        if (!TryGetRequiredLong(arguments, "OriginRoomId", out long originRoomId, out string error))
        {
            return ErrorResult(error);
        }

        if (!TryGetRequiredLong(arguments, "DestinationRoomId", out long destinationRoomId, out error))
        {
            return ErrorResult(error);
        }

        if (!TryGetRequiredString(arguments, "PathSearchFunction", out string pathSearchFunction, out error))
        {
            return ErrorResult(error);
        }

        ICell origin = Gameworld.Cells.Get(originRoomId);
        if (origin is null)
        {
            return ErrorResult($"No room with id {originRoomId:N0} exists.");
        }

        ICell destination = Gameworld.Cells.Get(destinationRoomId);
        if (destination is null)
        {
            return ErrorResult($"No room with id {destinationRoomId:N0} exists.");
        }

        if (!TryResolvePathSearchFunction(pathSearchFunction, null, out Func<ICellExit, bool> pathFunction, out string resolvedPathFunction,
                out error))
        {
            return ErrorResult(error);
        }

        List<ICellExit> path = origin.PathBetween(destination, 50, pathFunction).ToList();
        bool sameRoom = origin == destination;

        return SuccessResult(new Dictionary<string, object>
        {
            ["OriginRoomId"] = origin.Id,
            ["DestinationRoomId"] = destination.Id,
            ["PathSearchFunction"] = resolvedPathFunction,
            ["HasPath"] = sameRoom || path.Any(),
            ["Directions"] = ConvertPathToDirectionCommands(path)
        });
    }

    private ToolExecutionResult HandlePathFromCharacterToRoom(JsonElement arguments)
    {
        if (!TryGetRequiredLong(arguments, "OriginCharacterId", out long originCharacterId, out string error))
        {
            return ErrorResult(error);
        }

        if (!TryGetRequiredLong(arguments, "DestinationRoomId", out long destinationRoomId, out error))
        {
            return ErrorResult(error);
        }

        if (!TryGetRequiredString(arguments, "PathSearchFunction", out string pathSearchFunction, out error))
        {
            return ErrorResult(error);
        }

        ICharacter originCharacter = Gameworld.TryGetCharacter(originCharacterId, true);
        if (originCharacter is null)
        {
            return ErrorResult($"No character with id {originCharacterId:N0} exists.");
        }

        if (originCharacter.Location is null)
        {
            return ErrorResult($"Character {originCharacterId:N0} has no location.");
        }

        ICell destination = Gameworld.Cells.Get(destinationRoomId);
        if (destination is null)
        {
            return ErrorResult($"No room with id {destinationRoomId:N0} exists.");
        }

        if (!TryResolvePathSearchFunction(pathSearchFunction, originCharacter, out Func<ICellExit, bool> pathFunction,
                out string resolvedPathFunction, out error))
        {
            return ErrorResult(error);
        }

        List<ICellExit> path = originCharacter.PathBetween(destination, 50, pathFunction).ToList();
        bool sameRoom = originCharacter.Location == destination;

        return SuccessResult(new Dictionary<string, object>
        {
            ["OriginCharacterId"] = originCharacter.Id,
            ["OriginRoomId"] = originCharacter.Location.Id,
            ["DestinationRoomId"] = destination.Id,
            ["PathSearchFunction"] = resolvedPathFunction,
            ["HasPath"] = sameRoom || path.Any(),
            ["Directions"] = ConvertPathToDirectionCommands(path)
        });
    }

    private ToolExecutionResult HandlePathBetweenCharacters(JsonElement arguments)
    {
        if (!TryGetRequiredLong(arguments, "OriginCharacterId", out long originCharacterId, out string error))
        {
            return ErrorResult(error);
        }

        if (!TryGetRequiredLong(arguments, "DestinationCharacterId", out long destinationCharacterId, out error))
        {
            return ErrorResult(error);
        }

        if (!TryGetRequiredString(arguments, "PathSearchFunction", out string pathSearchFunction, out error))
        {
            return ErrorResult(error);
        }

        ICharacter originCharacter = Gameworld.TryGetCharacter(originCharacterId, true);
        if (originCharacter is null)
        {
            return ErrorResult($"No character with id {originCharacterId:N0} exists.");
        }

        if (originCharacter.Location is null)
        {
            return ErrorResult($"Character {originCharacterId:N0} has no location.");
        }

        ICharacter destinationCharacter = Gameworld.TryGetCharacter(destinationCharacterId, true);
        if (destinationCharacter is null)
        {
            return ErrorResult($"No character with id {destinationCharacterId:N0} exists.");
        }

        if (destinationCharacter.Location is null)
        {
            return ErrorResult($"Character {destinationCharacterId:N0} has no location.");
        }

        if (!TryResolvePathSearchFunction(pathSearchFunction, originCharacter, out Func<ICellExit, bool> pathFunction,
                out string resolvedPathFunction, out error))
        {
            return ErrorResult(error);
        }

        List<ICellExit> path = originCharacter.PathBetween(destinationCharacter, 50, pathFunction).ToList();
        bool sameRoom = originCharacter.Location == destinationCharacter.Location;

        return SuccessResult(new Dictionary<string, object>
        {
            ["OriginCharacterId"] = originCharacter.Id,
            ["OriginRoomId"] = originCharacter.Location.Id,
            ["DestinationCharacterId"] = destinationCharacter.Id,
            ["DestinationRoomId"] = destinationCharacter.Location.Id,
            ["PathSearchFunction"] = resolvedPathFunction,
            ["HasPath"] = sameRoom || path.Any(),
            ["Directions"] = ConvertPathToDirectionCommands(path)
        });
    }

    private ToolExecutionResult HandleRecentCharacterPlans()
    {
        List<Dictionary<string, object>> plans = Gameworld.Characters
            .Where(x => x.AffectedBy<RecentlyUpdatedPlan>())
            .Select(x => (Character: x, Effect: x.EffectsOfType<RecentlyUpdatedPlan>().FirstOrDefault()))
            .Where(x => x.Effect is not null)
            .Select(x =>
            {
                TimeSpan updatedAgo = x.Character.ScheduledDuration(x.Effect);
                TimeSpan windowRemaining = TimeSpan.FromDays(90) - updatedAgo;
                return (x.Character, UpdatedAgo: updatedAgo, WindowRemaining: windowRemaining);
            })
            .OrderBy(x => x.WindowRemaining)
            .Select(x => new Dictionary<string, object>
            {
                ["Id"] = x.Character.Id,
                ["Name"] = x.Character.PersonalName.GetName(NameStyle.FullName),
                ["ShortDescription"] =
                    x.Character.HowSeen(null, colour: false, flags: PerceiveIgnoreFlags.TrueDescription),
                ["ShortTermPlan"] = x.Character.ShortTermPlan ?? string.Empty,
                ["LongTermPlan"] = x.Character.LongTermPlan ?? string.Empty,
                ["UpdatedAgo"] = x.UpdatedAgo.ToString("c"),
                ["WindowRemaining"] = x.WindowRemaining.ToString("c")
            })
            .ToList();

        return SuccessResult(new Dictionary<string, object>
        {
            ["WindowDays"] = 90,
            ["Plans"] = plans
        });
    }

    private ToolExecutionResult HandleCharacterPlans(JsonElement arguments)
    {
        if (!TryGetRequiredLong(arguments, "Id", out long id, out string error))
        {
            return ErrorResult(error);
        }

        ICharacter target = Gameworld.TryGetCharacter(id, true);
        if (target is null)
        {
            return ErrorResult($"No character with id {id:N0} exists.");
        }

        RecentlyUpdatedPlan recentPlanEffect = target.EffectsOfType<RecentlyUpdatedPlan>().FirstOrDefault();
        TimeSpan updatedAgo = recentPlanEffect is not null ? target.ScheduledDuration(recentPlanEffect) : TimeSpan.Zero;
        TimeSpan windowRemaining = recentPlanEffect is not null ? TimeSpan.FromDays(90) - updatedAgo : TimeSpan.Zero;

        return SuccessResult(new Dictionary<string, object?>
        {
            ["Id"] = target.Id,
            ["Name"] = target.PersonalName.GetName(NameStyle.FullName),
            ["ShortDescription"] = target.HowSeen(null, colour: false, flags: PerceiveIgnoreFlags.TrueDescription),
            ["ShortTermPlan"] = target.ShortTermPlan ?? string.Empty,
            ["LongTermPlan"] = target.LongTermPlan ?? string.Empty,
            ["RecentlyUpdated"] = recentPlanEffect is not null,
            ["UpdatedAgo"] = recentPlanEffect is not null ? updatedAgo.ToString("c") : null,
            ["WindowRemaining"] = recentPlanEffect is not null ? windowRemaining.ToString("c") : null
        });
    }

    private List<ICell> GetStorytellerMonitoredCells()
    {
        List<ICell> monitoredCells = SurveillanceStrategy.GetCells(Gameworld)
            .Distinct()
            .ToList();
        return monitoredCells.Any() ? monitoredCells : Gameworld.Cells.ToList();
    }

    private List<(ICalendar Calendar, IClock Clock, IMudTimeZone TimeZone, ICell? ContextCell)> GetDateTimeContexts()
    {
        return GetStorytellerMonitoredCells()
            .SelectMany(cell =>
                cell.Calendars.Select(calendar =>
                    (Calendar: calendar, Clock: calendar.FeedClock, TimeZone: cell.TimeZone(calendar.FeedClock), ContextCell: cell)))
            .GroupBy(x => (x.Calendar.Id, x.Clock.Id, x.TimeZone.Id))
            .Select(x => x.First())
            .ToList();
    }

    private static Dictionary<string, object?> BuildDateTimeResult(ILocation location, ICalendar calendar)
    {
        IClock clock = calendar.FeedClock;
        IMudTimeZone timeZone = location.TimeZone(clock);
        MudDate date = location.Date(calendar);
        MudTime time = location.Time(clock);

        return new Dictionary<string, object?>
        {
            ["CalendarId"] = calendar.Id,
            ["CalendarName"] = calendar.FullName,
            ["ClockId"] = clock.Id,
            ["ClockName"] = clock.Name,
            ["TimeZoneId"] = timeZone.Id,
            ["TimeZoneName"] = timeZone.Name,
            ["DateLong"] = calendar.DisplayDate(date, CalendarDisplayMode.Long),
            ["DateShort"] = calendar.DisplayDate(date, CalendarDisplayMode.Short),
            ["TimeLong"] = clock.DisplayTime(time, TimeDisplayTypes.Long),
            ["TimeShort"] = clock.DisplayTime(time, TimeDisplayTypes.Short),
            ["TimeVague"] = clock.DisplayTime(time, TimeDisplayTypes.Vague),
            ["DateTime"] = $"{clock.DisplayTime(time, TimeDisplayTypes.Long)} on {calendar.DisplayDate(date, CalendarDisplayMode.Long)}"
        };
    }

    private ToolExecutionResult HandleCurrentDateTime()
    {
        List<(ICalendar Calendar, IClock Clock, IMudTimeZone TimeZone, ICell ContextCell)> contexts = GetDateTimeContexts();
        if (!contexts.Any())
        {
            return ErrorResult("There are no monitored rooms to evaluate date and time for.");
        }

        if (contexts.Count > 1)
        {
            return ErrorResult(
                "Multiple calendar/clock/timezone contexts are in use. Use DateTimeForTarget with CharacterId or RoomId.");
        }

        (ICalendar Calendar, IClock Clock, IMudTimeZone TimeZone, ICell ContextCell) context = contexts[0];
        if (context.ContextCell is null)
        {
            return ErrorResult("Unable to resolve a monitored room context for current date and time.");
        }

        return SuccessResult(BuildDateTimeResult(context.ContextCell, context.Calendar));
    }

    private ToolExecutionResult HandleDateTimeForTarget(JsonElement arguments)
    {
        bool hasCharacter = TryGetOptionalLong(arguments, "CharacterId", out long characterId);
        bool hasRoom = TryGetOptionalLong(arguments, "RoomId", out long roomId);
        if (hasCharacter && hasRoom)
        {
            return ErrorResult("Specify either CharacterId or RoomId, but not both.");
        }

        if (!hasCharacter && !hasRoom)
        {
            return HandleCurrentDateTime();
        }

        if (hasCharacter)
        {
            ICharacter character = Gameworld.TryGetCharacter(characterId, true);
            if (character is null)
            {
                return ErrorResult($"No character with id {characterId:N0} exists.");
            }

            if (character.Location is null)
            {
                return ErrorResult($"Character {characterId:N0} has no location.");
            }

            ICalendar calendar = character.Location.Calendars.FirstOrDefault();
            if (calendar is null)
            {
                return ErrorResult($"Character {characterId:N0} location has no calendar.");
            }

            Dictionary<string, object> result = BuildDateTimeResult(character.Location, calendar);
            result["CharacterId"] = character.Id;
            result["CharacterName"] = character.PersonalName.GetName(NameStyle.FullName);
            result["RoomId"] = character.Location.Id;
            result["RoomName"] = character.Location.HowSeen(null, colour: false);
            return SuccessResult(result);
        }

        ICell room = Gameworld.Cells.Get(roomId);
        if (room is null)
        {
            return ErrorResult($"No room with id {roomId:N0} exists.");
        }

        ICalendar roomCalendar = room.Calendars.FirstOrDefault();
        if (roomCalendar is null)
        {
            return ErrorResult($"Room {roomId:N0} has no calendar.");
        }

        Dictionary<string, object> roomResult = BuildDateTimeResult(room, roomCalendar);
        roomResult["RoomId"] = room.Id;
        roomResult["RoomName"] = room.HowSeen(null, colour: false);
        return SuccessResult(roomResult);
    }

    private ToolExecutionResult HandleCalendarDefinition(JsonElement arguments)
    {
        if (!TryGetRequiredString(arguments, "Id", out string calendarId, out string error))
        {
            return ErrorResult(error);
        }

        ICalendar calendar = Gameworld.Calendars.GetByIdOrNames(calendarId);
        if (calendar is null)
        {
            return ErrorResult($"No calendar identified by '{calendarId}' exists.");
        }

        bool hasYear = TryGetOptionalInt(arguments, "Year", out int yearNumber);
        Dictionary<string, object> result = new()
        {
            ["Id"] = calendar.Id,
            ["Name"] = calendar.FullName,
            ["ShortName"] = calendar.ShortName,
            ["Alias"] = calendar.Alias,
            ["Description"] = calendar.Description,
            ["AncientEpoch"] = calendar.AncientEraLongString,
            ["ModernEpoch"] = calendar.ModernEraLongString,
            ["Weekdays"] = calendar.Weekdays.ToList(),
            ["DaysInNormalYear"] =
                calendar.Months.Sum(x => x.NormalDays) +
                calendar.Intercalaries.Where(x => x.Rule.DivisibleBy == 1).Sum(x => x.Month.NormalDays)
        };

        if (hasYear)
        {
            Year year = calendar.CreateYear(yearNumber);
            result["Year"] = year.YearName;
            result["DaysInYear"] = year.Months.Sum(x => x.Days);
            result["Months"] = year.Months.Select(month => new Dictionary<string, object?>
            {
                ["Order"] = month.NominalOrder,
                ["Name"] = month.FullName,
                ["ShortName"] = month.ShortName,
                ["Days"] = month.Days,
                ["SpecialDays"] = month.DayNames
                    .Select(x => new Dictionary<string, object?>
                    {
                        ["Day"] = x.Key,
                        ["Name"] = x.Value.FullName
                    })
                    .ToList()
            }).ToList();
        }
        else
        {
            result["Months"] = calendar.Months.Select(month => new Dictionary<string, object?>
            {
                ["Order"] = month.NominalOrder,
                ["Name"] = month.FullName,
                ["ShortName"] = month.ShortName,
                ["Days"] = month.NormalDays,
                ["SpecialDays"] = month.SpecialDayNames
                    .Select(x => new Dictionary<string, object?>
                    {
                        ["Day"] = x.Key,
                        ["Name"] = x.Value.FullName
                    })
                    .ToList()
            }).ToList();

            result["IntercalaryMonths"] = calendar.Intercalaries.Select(intercalary => new Dictionary<string, object?>
            {
                ["Name"] = intercalary.Month.FullName,
                ["ShortName"] = intercalary.Month.ShortName,
                ["Days"] = intercalary.Month.NormalDays,
                ["After"] =
                    calendar.Months.FirstOrDefault(x => x.NominalOrder == intercalary.Month.NominalOrder - 1)?.FullName ??
                    "None",
                ["SpecialDays"] = intercalary.Month.SpecialDayNames
                    .Select(x => new Dictionary<string, object?>
                    {
                        ["Day"] = x.Key,
                        ["Name"] = x.Value.FullName
                    })
                    .ToList(),
                ["Rule"] = intercalary.Rule.ToString()
            }).ToList();
        }

        return SuccessResult(result);
    }

    private static bool TryGetOptionalLong(JsonElement arguments, string propertyName, out long value)
    {
        value = 0;
        if (!arguments.TryGetProperty(propertyName, out JsonElement element))
        {
            return false;
        }

        if (element.ValueKind == JsonValueKind.Number && element.TryGetInt64(out value))
        {
            return true;
        }

        if (element.ValueKind == JsonValueKind.String && long.TryParse(element.GetString(), out value))
        {
            return true;
        }

        return false;
    }

    private static bool TryGetOptionalInt(JsonElement arguments, string propertyName, out int value)
    {
        value = 0;
        if (!arguments.TryGetProperty(propertyName, out JsonElement element))
        {
            return false;
        }

        if (element.ValueKind == JsonValueKind.Number && element.TryGetInt32(out value))
        {
            return true;
        }

        if (element.ValueKind == JsonValueKind.String && int.TryParse(element.GetString(), out value))
        {
            return true;
        }

        return false;
    }

    private ToolExecutionResult HandleCustomFunctionCall(string functionName, JsonElement arguments,
        bool includeEchoTools)
    {
        AIStorytellerCustomToolCall toolCall = CustomToolCalls.FirstOrDefault(x => x.Name.EqualTo(functionName));
        if (toolCall is null && includeEchoTools)
        {
            toolCall = CustomToolCallsEchoOnly.FirstOrDefault(x => x.Name.EqualTo(functionName));
        }

        if (toolCall is null)
        {
            return ErrorResult($"Unknown function '{functionName}'.");
        }

        if (toolCall.Prog is null)
        {
            return ErrorResult($"Custom tool '{functionName}' has no prog.");
        }

        if (!string.IsNullOrWhiteSpace(toolCall.Prog.CompileError))
        {
            return ErrorResult(
                $"Custom tool '{functionName}' is invalid because its prog does not compile.");
        }

        List<object> progArguments = new();
        foreach ((ProgVariableTypes parameterType, string parameterName) in toolCall.Prog.NamedParameters)
        {
            if (!arguments.TryGetProperty(parameterName, out JsonElement argumentValue))
            {
                return ErrorResult($"Missing required custom-tool parameter '{parameterName}'.");
            }

            if (!TryConvertJsonArgument(argumentValue, parameterType, out object convertedValue, out string error))
            {
                return ErrorResult(error);
            }

            progArguments.Add(convertedValue);
        }

        object result = toolCall.Prog.ExecuteWithRecursionProtection(progArguments.ToArray());
        return SuccessResult(new Dictionary<string, object>
        {
            ["Function"] = functionName,
            ["ReturnType"] = toolCall.Prog.ReturnType.Describe(),
            ["Result"] = ConvertToToolOutputValue(result)
        });
    }


    private object? ConvertToToolOutputValue(object? value)
    {
        switch (value)
        {
            case null:
                return null;
            case string or bool or byte or sbyte or short or ushort or int or uint or long or ulong or float or
                double or decimal:
                return value;
            case DateTime dateTime:
                return dateTime.ToString("O");
            case TimeSpan timeSpan:
                return timeSpan.ToString("c");
            case MudDateTime mudDateTime:
                return mudDateTime.ToString();
            case IDictionary dictionary:
                {
                    Dictionary<string, object> result = new();
                    foreach (DictionaryEntry entry in dictionary)
                    {
                        result[entry.Key?.ToString() ?? string.Empty] = ConvertToToolOutputValue(entry.Value);
                    }

                    return result;
                }
            case IEnumerable enumerable when value is not string:
                {
                    List<object> list = new();
                    foreach (object item in enumerable)
                    {
                        list.Add(ConvertToToolOutputValue(item));
                    }

                    return list;
                }
            case IPerceivable perceivable:
                return new Dictionary<string, object?>
                {
                    ["Id"] = perceivable.Id,
                    ["Name"] = perceivable.Name,
                    ["Type"] = perceivable.FrameworkItemType
                };
            case IFrameworkItem frameworkItem:
                return new Dictionary<string, object?>
                {
                    ["Id"] = frameworkItem.Id,
                    ["Name"] = frameworkItem.Name,
                    ["Type"] = frameworkItem.FrameworkItemType
                };
            default:
                return value.ToString() ?? string.Empty;
        }
    }


}
