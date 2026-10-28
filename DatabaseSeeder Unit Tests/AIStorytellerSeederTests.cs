#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;

namespace DatabaseSeederUnitTests;

[TestClass]
public class AIStorytellerSeederTests
{
	private static FuturemudDatabaseContext CreateContext() => new(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
		.UseInMemoryDatabase(Guid.NewGuid().ToString())
		.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning)).Options);
	private static string Seed(FuturemudDatabaseContext context)
	{
		var result = new AIStorytellerSeeder().SeedData(context, new Dictionary<string, string> { ["install"] = "yes" });
		Assert.IsFalse(result.StartsWith("Failed", StringComparison.Ordinal), result);
		return result;
	}

	[TestMethod]
	public void FreshInstall_UsesRoleDefaultsAndRerunsWithoutDuplicates()
	{
		using var context = CreateContext();
		Seed(context); Seed(context);
		var storyteller = context.AIStorytellers.Single();
		Assert.AreEqual("gpt-6.1-terra", storyteller.Model);
		Assert.AreEqual("gpt-6-luna", storyteller.AttentionClassifierModel);
		Assert.AreEqual("none", storyteller.AttentionClassifierReasoningEffort);
		Assert.AreEqual("5", XElement.Parse(storyteller.CustomToolCallsDefinition).Element("ExecutionOptions")?.Element("BatchWindowSeconds")?.Value);
		Assert.AreEqual(3, context.FutureProgs.Count());
		Assert.AreEqual(12, context.AIStorytellerReferenceDocuments.Count());
		Assert.AreEqual(16, context.SeederManagedRecords.Count());
		Assert.IsFalse(context.AIStorytellerReferenceDocuments.AsEnumerable().Any(x => x.DocumentContents.Contains("BypassAttention")));
		Assert.IsFalse(context.AIStorytellerReferenceDocuments.AsEnumerable().Any(x => x.DocumentContents.Contains("End bypass") || x.DocumentContents.Contains("Attention focus guidance:")));
	}

	[TestMethod]
	public void ManagedRerun_PreservesRenamesPromptsPauseAndLinkedContent()
	{
		using var context = CreateContext();
		Seed(context);
		var storyteller = context.AIStorytellers.Single();
		var id = storyteller.Id;
		storyteller.Name = "My narrator";
		storyteller.SystemPrompt = "My private setting";
		storyteller.IsPaused = true;
		storyteller.SubscribeToSpeechEvents = true;
		storyteller.CustomToolCallsDefinition = "<ToolCalls><BuilderExtension /></ToolCalls>";
		var prog = context.FutureProgs.First();
		prog.FunctionName = "BuilderRenamedProg";
		prog.FunctionText = "return \"Builder content\"";
		var document = context.AIStorytellerReferenceDocuments.First();
		document.Name = "Builder renamed reference";
		document.DocumentContents = "My lore";
		document.RestrictedStorytellerIds = "999";
		context.SaveChanges();
		var report = Seed(context);
		Seed(context);
		Assert.AreEqual(id, context.AIStorytellers.Single().Id);
		Assert.AreEqual("My narrator", storyteller.Name);
		Assert.AreEqual("My private setting", storyteller.SystemPrompt);
		Assert.IsTrue(storyteller.IsPaused);
		Assert.IsTrue(storyteller.SubscribeToSpeechEvents);
		Assert.AreEqual("<ToolCalls><BuilderExtension /></ToolCalls>", storyteller.CustomToolCallsDefinition);
		Assert.AreEqual("return \"Builder content\"", prog.FunctionText);
		Assert.AreEqual("BuilderRenamedProg", prog.FunctionName);
		Assert.AreEqual("My lore", document.DocumentContents);
		Assert.AreEqual("999", document.RestrictedStorytellerIds);
		StringAssert.Contains(report, "preserved");
	}

	private static void MakeLegacy(FuturemudDatabaseContext context)
	{
		var s = context.AIStorytellers.Single();
		s.Model = "gpt-5";
		s.TimeModel = null!; s.AttentionClassifierModel = null!; s.TimeSystemPrompt = null!;
		s.TimeReasoningEffort = null!; s.AttentionClassifierReasoningEffort = null!;
		s.ReasoningEffort = "2";
		s.AttentionAgentPrompt = "Reply with \"interested\" if the event materially affects ongoing narrative situations.\nYou may append a short reason after the word interested.\nReply with \"ignore\" otherwise.";
		var xml = XElement.Parse(s.CustomToolCallsDefinition);
		xml.Element("ExecutionOptions")!.Remove();
		s.CustomToolCallsDefinition = xml.ToString();
		context.SeederManagedRecords.RemoveRange(context.SeederManagedRecords.Where(x => x.EntityType == "AIStoryteller"));
		context.SaveChanges();
	}

	[TestMethod]
	public void LegacyStock_WithDifferentLineEndings_IsAdoptedAndUpgraded()
	{
		using var context = CreateContext();
		Seed(context);
		MakeLegacy(context);
		var s = context.AIStorytellers.Single();
		s.SystemPrompt = s.SystemPrompt.Replace("\r\n", "\n").Replace("\n", "\r\n");
		s.AttentionAgentPrompt = s.AttentionAgentPrompt.Replace("\n", "\r\n");
		context.SaveChanges();
		var id = s.Id;
		Seed(context);
		Assert.AreEqual(id, s.Id);
		Assert.AreEqual("gpt-6.1-terra", s.Model);
		Assert.AreEqual("gpt-6-luna", s.AttentionClassifierModel);
		StringAssert.Contains(s.AttentionAgentPrompt, "JSON schema");
	}

	[TestMethod]
	public void UnownedCustomStoryteller_IsPreservedWithoutAdoption()
	{
		using var context = CreateContext();
		Seed(context); MakeLegacy(context);
		var s = context.AIStorytellers.Single();
		s.SystemPrompt = "Customized setting policy";
		context.SaveChanges();
		StringAssert.Contains(Seed(context), "no ownership inferred");
		Assert.AreEqual("gpt-5", s.Model);
		Assert.AreEqual("Customized setting policy", s.SystemPrompt);
		Assert.IsFalse(context.SeederManagedRecords.Any(x => x.EntityType == "AIStoryteller"));
	}

	[TestMethod]
	public void UnownedModifiedLinkedProg_PreventsLegacyStorytellerAdoption()
	{
		using var context = CreateContext();
		Seed(context); MakeLegacy(context);
		context.SeederManagedRecords.RemoveRange(context.SeederManagedRecords.Where(x => x.EntityType == "FutureProg"));
		context.FutureProgs.First().FunctionText = "return \"Custom\"";
		context.SaveChanges();
		Seed(context);
		Assert.AreEqual("gpt-5", context.AIStorytellers.Single().Model);
		Assert.IsFalse(context.SeederManagedRecords.Any(x => x.EntityType == "AIStoryteller"));
	}
	[TestMethod]
	public void LegacyReference_LineEndingsUpgradeAndManagedParametersRemainCustomized()
	{
		using var context = CreateContext();
		Seed(context);
		var document = context.AIStorytellerReferenceDocuments.Single(x => x.Name == "AI Storyteller Primer");
		document.DocumentContents = document.DocumentContents.Split("\n\nRuntime guidance:")[0].Replace("\n", "\r\n");
		context.SeederManagedRecords.RemoveRange(context.SeederManagedRecords.Where(x => x.EntityType == "AIStorytellerReferenceDocument" && x.LogicalId == document.Id));
		var parameter = context.FutureProgsParameters.Single();
		parameter.ParameterName = "BuilderCue";
		context.SaveChanges();
		Seed(context); Seed(context);
		StringAssert.Contains(document.DocumentContents, "Runtime guidance:");
		Assert.AreEqual("BuilderCue", context.FutureProgsParameters.Single().ParameterName);
		Assert.AreEqual(16, context.SeederManagedRecords.Count());
	}

}
