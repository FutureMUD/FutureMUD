using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

public class AIStorytellerSeeder : IDatabaseSeeder
{
	private const string StorytellerName = "Example Narrative Steward";
	private const string FiveMinuteStatusProgName = "AIStorytellerStatusFiveMinute";
	private const string HourlyStatusProgName = "AIStorytellerStatusHourly";
	private const string SafeCustomToolProgName = "AIStorytellerToolRecordCue";

	public bool SafeToRunMoreThanOnce => true;

	public IEnumerable<(string Id, string Question,
		Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool> Filter,
		Func<string, FuturemudDatabaseContext, (bool Success, string error)> Validator)> SeederQuestions =>
	[
		(
			"install",
			"Do you want to install an AI Storyteller starter pack with example prompts, heartbeat progs, a safe custom tool prog, and sample reference documents?\n\nPlease answer #3yes#f or #3no#f: ",
			(_, _) => true,
			(answer, _) =>
			{
				if (answer.EqualToAny("yes", "y", "no", "n"))
				{
					return (true, string.Empty);
				}

				return (false, "Invalid answer");
			}
		)
	];

	public int SortOrder => 215;
	public string Name => "AI Storyteller Starter Pack";
	public string Tagline => "Adds one complete AI storyteller example package";
	public string FullDescription => @"This package installs a complete, operational AI Storyteller example with:

- one sample storyteller configured for heartbeat operation
- sample five-minute and hourly status progs
- one safe sample custom tool prog
- two sample reference documents (one global and one storyteller-restricted)

This package is safe to run multiple times and updates/reuses existing sample records.";

	public ShouldSeedResult ShouldSeedData(FuturemudDatabaseContext context)
	{
		if (!context.Accounts.Any())
		{
			return ShouldSeedResult.PrerequisitesNotMet;
		}

		var hasStoryteller = context.AIStorytellers.Any(x => x.Name == StorytellerName);
		var hasFiveMinuteProg = context.FutureProgs.Any(x => x.FunctionName == FiveMinuteStatusProgName);
		var hasHourlyProg = context.FutureProgs.Any(x => x.FunctionName == HourlyStatusProgName);
		var hasCustomToolProg = context.FutureProgs.Any(x => x.FunctionName == SafeCustomToolProgName);
		var hasPrimerDocument = context.AIStorytellerReferenceDocuments.Any(x => x.Name == "AI Storyteller Primer");
		var hasFactionDocument = context.AIStorytellerReferenceDocuments.Any(x => x.Name == "Harbour Ward Factions");

		if (hasStoryteller && hasFiveMinuteProg && hasHourlyProg && hasCustomToolProg && hasPrimerDocument &&
		    hasFactionDocument)
		{
			return ShouldSeedResult.MayAlreadyBeInstalled;
		}

		if (hasStoryteller || hasFiveMinuteProg || hasHourlyProg || hasCustomToolProg || hasPrimerDocument ||
		    hasFactionDocument)
		{
			return ShouldSeedResult.ExtraPackagesAvailable;
		}

		return ShouldSeedResult.ReadyToInstall;
	}

	public string SeedData(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> questionAnswers)
	{
		if (!questionAnswers.TryGetValue("install", out var answer) || !answer.EqualToAny("yes", "y"))
		{
			return "No changes were applied.";
		}

		context.Database.BeginTransaction();
		try
		{
			var fiveMinuteProg = EnsureProg(
				context,
				FiveMinuteStatusProgName,
				"AI Storyteller",
				"Heartbeat",
				ProgVariableTypes.Text,
				"Sample heartbeat status prog for AI storyteller five minute triggers.",
				"return \"Five-minute heartbeat check complete.\""
			);

			var hourlyProg = EnsureProg(
				context,
				HourlyStatusProgName,
				"AI Storyteller",
				"Heartbeat",
				ProgVariableTypes.Text,
				"Sample heartbeat status prog for AI storyteller hourly triggers.",
				"return \"Hourly heartbeat check complete. Consider reviewing unresolved situations.\""
			);

			var safeCustomToolProg = EnsureProg(
				context,
				SafeCustomToolProgName,
				"AI Storyteller",
				"Tools",
				ProgVariableTypes.Text,
				"A safe sample custom tool that only echoes back a short narrative cue.",
				"""return "Narrative cue captured: " + @Cue""",
				(ProgVariableTypes.Text, "Cue")
			);

			var storyteller = context.AIStorytellers.FirstOrDefault(x => x.Name == StorytellerName);
			if (storyteller is null)
			{
				storyteller = new AIStoryteller();
				context.AIStorytellers.Add(storyteller);
			}

			storyteller.Name = StorytellerName;
			storyteller.Description =
				"An example AI storyteller pack seeded by DatabaseSeeder for heartbeat-driven narrative management.";
			storyteller.Model = "gpt-5";
			storyteller.SystemPrompt = """
You are an AI Storyteller for a roleplay-intensive MUD.
You must be concise, world-aware, and action-oriented.
When uncertain, gather context with tools before taking action.
Keep responses focused on in-world narrative continuity.
""";
			storyteller.AttentionAgentPrompt = """
Reply with "interested" if the event materially affects ongoing narrative situations.
You may append a short reason after the word interested.
Reply with "ignore" otherwise.
""";
			storyteller.SurveillanceStrategyDefinition =
				"""<Definition><Zones /><IncludedCells /><ExcludedCells /></Definition>""";
			storyteller.ReasoningEffort = "2";
			storyteller.CustomToolCallsDefinition = BuildCustomToolDefinition(safeCustomToolProg.Id);
			storyteller.SubscribeToRoomEvents = false;
			storyteller.SubscribeToSpeechEvents = false;
			storyteller.SubscribeToCrimeEvents = false;
			storyteller.SubscribeToStateEvents = false;
			storyteller.SubscribeTo5mHeartbeat = true;
			storyteller.SubscribeTo10mHeartbeat = false;
			storyteller.SubscribeTo30mHeartbeat = false;
			storyteller.SubscribeToHourHeartbeat = true;
			storyteller.HeartbeatStatus5mProgId = fiveMinuteProg.Id;
			storyteller.HeartbeatStatus10mProgId = null;
			storyteller.HeartbeatStatus30mProgId = null;
			storyteller.HeartbeatStatus1hProgId = hourlyProg.Id;
			storyteller.IsPaused = false;

			context.SaveChanges();

			EnsureReferenceDocument(
				context,
				"AI Storyteller Primer",
				"General guidance for storyteller behaviour and priorities.",
				"Storyteller",
				"Guide",
				"storyteller,policy,operations,narrative",
				"""
This document describes baseline storyteller operations:
- Preserve continuity across unresolved situations.
- Prefer factual retrieval via tools before major actions.
- Keep side effects auditable and intentional.
""",
				string.Empty
			);

			EnsureReferenceDocument(
				context,
				"Harbour Ward Factions",
				"Faction notes scoped to the seeded storyteller.",
				"World",
				"FactionBrief",
				"harbour,ward,factions,politics",
				"""
Harbour Ward snapshot:
- Dock Syndicate controls freight disputes and labour contracts.
- Lantern Circle brokers intelligence and rumours.
- Tidewatch maintains formal law but negotiates with both groups.
""",
				storyteller.Id.ToString()
			);

			context.SaveChanges();
			context.Database.CommitTransaction();
			return "Installed or updated AI Storyteller starter pack successfully.";
		}
		catch (Exception e)
		{
			context.Database.RollbackTransaction();
			return $"Failed to install AI Storyteller starter pack: {e.Message}";
		}
	}

	private static FutureProg EnsureProg(FuturemudDatabaseContext context, string functionName, string category,
		string subcategory, ProgVariableTypes returnType, string comment, string text,
		params (ProgVariableTypes Type, string Name)[] parameters)
	{
		var prog = context.FutureProgs.FirstOrDefault(x => x.FunctionName == functionName);
		if (prog is null)
		{
			prog = new FutureProg
			{
				FunctionName = functionName,
				FunctionComment = comment,
				FunctionText = text,
				ReturnType = (long)returnType,
				Category = category,
				Subcategory = subcategory,
				Public = false,
				AcceptsAnyParameters = false,
				StaticType = (int)FutureProgStaticType.NotStatic
			};
			context.FutureProgs.Add(prog);
			context.SaveChanges();
		}

		prog.FunctionComment = comment;
		prog.FunctionText = text;
		prog.ReturnType = (long)returnType;
		prog.Category = category;
		prog.Subcategory = subcategory;
		prog.Public = false;
		prog.AcceptsAnyParameters = false;
		prog.StaticType = (int)FutureProgStaticType.NotStatic;

		var existingParameters = prog.FutureProgsParameters.ToList();
		foreach (var parameter in existingParameters)
		{
			context.FutureProgsParameters.Remove(parameter);
		}

		for (var index = 0; index < parameters.Length; index++)
		{
			var (type, name) = parameters[index];
			context.FutureProgsParameters.Add(new FutureProgsParameter
			{
				FutureProg = prog,
				ParameterIndex = index,
				ParameterType = (long)type,
				ParameterName = name
			});
		}

		context.SaveChanges();
		return prog;
	}

	private static void EnsureReferenceDocument(FuturemudDatabaseContext context, string name, string description,
		string folderName, string documentType, string keywords, string contents, string restrictedStorytellerIds)
	{
		var document = context.AIStorytellerReferenceDocuments.FirstOrDefault(x => x.Name == name);
		if (document is null)
		{
			document = new AIStorytellerReferenceDocument();
			context.AIStorytellerReferenceDocuments.Add(document);
		}

		document.Name = name;
		document.Description = description;
		document.FolderName = folderName;
		document.DocumentType = documentType;
		document.Keywords = keywords;
		document.DocumentContents = contents;
		document.RestrictedStorytellerIds = restrictedStorytellerIds;
	}

	private static string BuildCustomToolDefinition(long customToolProgId)
	{
		return new XElement("ToolCalls",
			new XElement("ToolCall",
				new XElement("Name", new XCData("RecordNarrativeCue")),
				new XElement("Description", new XCData("Records a short narrative cue and echoes it back.")),
				new XElement("Prog", customToolProgId),
				new XElement("IncludeWithEcho", false),
				new XElement("ParameterDescriptions",
					new XElement("Description", new XAttribute("name", "Cue"),
						new XCData("A concise narrative cue to preserve for continuity."))))
		).ToString();
	}
}
