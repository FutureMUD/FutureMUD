#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using DatabaseSeeder.Seeders;
using DatabaseSeeder.Seeders.CultureToolkit;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.FutureProg;
using MudSharp.Models;
using FutureProg = MudSharp.Models.FutureProg;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CultureToolkitWritingGrantTests
{
	[DataTestMethod]
	[DataRow("antiquity")]
	[DataRow("darkages")]
	[DataRow("medieval")]
	[DataRow("renaissance")]
	[DataRow("earlymodern")]
	public void AuthoredWritingRulesCompileAndIntegrationReplacesBroadUnionAndPreservesEdits(string era)
	{
		using var context = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString(), x => x.EnableNullChecks(false)).Options);
		var catalogue = new CultureToolkitCatalogue();
		var pack = catalogue.Compose(era);
		var cultures = pack.Cultures.ToDictionary(x => CultureToolkitCatalogue.Text(x, "key"), x => new Culture { Name = CultureToolkitCatalogue.Text(x, "key") });
		var ethnicities = pack.Ethnicities.ToDictionary(x => CultureToolkitCatalogue.Text(x, "key"), x => new Ethnicity { Name = CultureToolkitCatalogue.Text(x, "key") });
		var languages = pack.Languages.ToDictionary(x => CultureToolkitCatalogue.Text(x, "key"), x => new TraitDefinition { Name = CultureToolkitCatalogue.Text(x, "key") });
		var scripts = catalogue.Document("data.script_policy.json").EnumerateArray().ToDictionary(x => CultureToolkitCatalogue.Text(x, "key"), x => new Knowledge { Name = CultureToolkitCatalogue.Text(x, "label") });
		var literacy = new TraitDefinition { Name = "Literacy" };
		context.AddRange(cultures.Values);
		context.AddRange(ethnicities.Values);
		context.AddRange(languages.Values);
		context.AddRange(scripts.Values);
		context.Add(literacy);
		var target = new FutureProg { FunctionName = "ChargenFreeKnowledges", FunctionText = "var knowledges as knowledge collection\n// Independent grants\nreturn @knowledges", ReturnType = (long)(ProgVariableTypes.Knowledge | ProgVariableTypes.Collection) };
		if (era == "antiquity") target.FunctionText = target.FunctionText.Replace("\n", "\r\n");
		target.FutureProgsParameters.Add(new FutureProgsParameter { ParameterName = "ch", ParameterType = (long)ProgVariableTypes.Chargen });
		context.Add(target);
		context.SaveChanges();
		var natives = catalogue.Document("data.ethnicity_language_defaults.json").GetProperty("defaults").EnumerateArray()
			.Where(x => ethnicities.ContainsKey(CultureToolkitCatalogue.Text(x, "ethnicity")))
			.ToDictionary(x => CultureToolkitCatalogue.Text(x, "ethnicity"), x => (IReadOnlyList<string>)CultureToolkitCatalogue.Strings(x.GetProperty("by_era").GetProperty(era)).ToArray());
		var conflicts = new List<string>();
		var resolution = CultureToolkitWritingGrants.Upsert(context, catalogue, pack, cultures, ethnicities, natives, languages, scripts, literacy, conflicts);
		Assert.IsTrue(resolution.KnowledgeProgId > 0);
		Assert.AreEqual(0, conflicts.Count);
		ChargenFreeKnowledgeProgReconciler.Reconcile(context);
		context.SaveChanges();
		using (var compiler = new MudSharp.Framework.OfflineProgCompilation(context.FutureProgs.Include(x => x.FutureProgsParameters).ToArray()))
			compiler.Compile(target.Id);
		StringAssert.Contains(target.FunctionText, "if (@CultureWritingPolicyApplies(@ch))");
		Assert.IsFalse(ChargenFreeKnowledgeProgReconciler.HasRepairableCultureDrift(context));
		var original = target.FunctionText;
		var rerun = ChargenFreeKnowledgeProgReconciler.Reconcile(context);
		Assert.IsFalse(rerun.Message.Contains("builder edit"), rerun.Message);
		Assert.AreEqual(original, target.FunctionText);
		target.FunctionText = target.FunctionText.Replace("// Independent grants", "// Builder outside block")
			.Replace("if (@CultureWritingPolicyApplies(@ch))", "if (false)");
		context.SaveChanges();
		var builder = target.FunctionText;
		var result = ChargenFreeKnowledgeProgReconciler.Reconcile(context);
		Assert.AreEqual(builder, target.FunctionText);
		StringAssert.Contains(result.Message, "builder edit");
	}
}
