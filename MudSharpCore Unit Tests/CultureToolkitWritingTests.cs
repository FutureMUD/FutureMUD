#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using DatabaseSeeder.Seeders;
using DatabaseSeeder.Seeders.CultureToolkit;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body.Traits;
using MudSharp.Character.Heritage;
using MudSharp.CharacterCreation;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.RPG.Knowledge;
using RuntimeProg = MudSharp.FutureProg.FutureProg;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CultureToolkitWritingTests
{
	[TestMethod]
	public void OrthodoxCroatGetsOnlyNativeLatinTraditionAndRequiresLiteracyAndSelectedLanguage()
	{
		FutureProgTestBootstrap.EnsureInitialised();
		using var context = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString(), x => x.EnableNullChecks(false)).Options);
		var catalogue = new CultureToolkitCatalogue();
		var whole = catalogue.Compose("medieval");
		var pack = whole with { Groups = [] };
		var culture = new MudSharp.Models.Culture { Name = "Orthodox learned" };
		var ethnicity = new MudSharp.Models.Ethnicity { Name = "Croat" };
		var languages = new[] { "legacy:Serbo-Croatian", "literacy", "greek.medieval" }.ToDictionary(x => x, x => new MudSharp.Models.TraitDefinition { Name = x == "literacy" ? "Literacy" : x });
		var scripts = new[] { "latin", "cyrillic", "greek" }.ToDictionary(x => x, x => new MudSharp.Models.Knowledge { Name = x });
		context.AddRange(culture, ethnicity);
		context.AddRange(languages.Values);
		context.AddRange(scripts.Values);
		var acquireGreek = new MudSharp.Models.FutureProg { FunctionName = "CanPickGreekScriptKnowledge", FunctionText = "return true", ReturnType = (long)ProgVariableTypes.Boolean };
		acquireGreek.FutureProgsParameters.Add(new MudSharp.Models.FutureProgsParameter { ParameterIndex = 0, ParameterName = "ch", ParameterType = (long)ProgVariableTypes.Chargen });
		acquireGreek.FutureProgsParameters.Add(new MudSharp.Models.FutureProgsParameter { ParameterIndex = 1, ParameterName = "trait", ParameterType = (long)ProgVariableTypes.Trait });
		scripts["greek"].Type = "Script";
		scripts["greek"].CanAcquireProg = acquireGreek;
		context.Add(new MudSharp.Models.Script { Name = "Greek", Knowledge = scripts["greek"] });
		var target = new MudSharp.Models.FutureProg { FunctionName = "ChargenFreeKnowledges", FunctionText = "var knowledges as knowledge collection\nreturn @knowledges", ReturnType = (long)(ProgVariableTypes.Knowledge | ProgVariableTypes.Collection) };
		target.FutureProgsParameters.Add(new MudSharp.Models.FutureProgsParameter { ParameterName = "ch", ParameterType = (long)ProgVariableTypes.Chargen });
		context.Add(target);
		context.SaveChanges();
		ChargenFreeKnowledgeProgReconciler.Reconcile(context);
		context.SaveChanges();
		CultureToolkitKnowledgeIntegration.PreserveLegacyBlock(context, "medieval", ChargenFreeKnowledgeProgReconciler.CaptureVerifiedCultureBlock(context));
		var resolution = CultureToolkitWritingGrants.Upsert(context, catalogue, pack,
			new Dictionary<string, MudSharp.Models.Culture> { ["culture.orthodox-learned-communities"] = culture },
			new Dictionary<string, MudSharp.Models.Ethnicity> { ["ethnicity.croat"] = ethnicity },
			new Dictionary<string, IReadOnlyList<string>> { ["ethnicity.croat"] = ["legacy:Serbo-Croatian"] },
			languages, scripts, languages["literacy"], []);
		// Script reconciliation can replace the stock acquisition reference. Its previous
		// broad block must still be recognised and retained only outside toolkit scope.
		scripts["greek"].CanAcquireProg = null;
		scripts["greek"].CanAcquireProgId = null;
		context.SaveChanges();
		ChargenFreeKnowledgeProgReconciler.Reconcile(context);
		context.SaveChanges();
		var world = new Mock<IFuturemud>();
		var progs = new All<IFutureProg>();
		var knowledges = new All<IKnowledge>();
		world.SetupGet(x => x.FutureProgs).Returns(progs);
		world.SetupGet(x => x.Knowledges).Returns(knowledges);
		foreach (var item in scripts.Values)
		{
			var knowledge = new Mock<IKnowledge>();
			knowledge.SetupGet(x => x.Id).Returns(item.Id);
			knowledge.SetupGet(x => x.Name).Returns(item.Name);
			knowledge.SetupGet(x => x.Type).Returns(ProgVariableTypes.Knowledge);
			knowledge.SetupGet(x => x.GetObject).Returns(knowledge.Object);
			knowledges.Add(knowledge.Object);
		}
		foreach (var model in context.FutureProgs.Include(x => x.FutureProgsParameters)) progs.Add(new RuntimeProg(model, world.Object));
		foreach (var prog in progs) Assert.IsTrue(prog.Compile(), prog.CompileError);
		var ch = new Mock<IChargen>();
		ch.SetupGet(x => x.Type).Returns(ProgVariableTypes.Chargen);
		ch.SetupGet(x => x.GetObject).Returns(ch.Object);
		var runtimeCulture = new Mock<ICulture>();
		runtimeCulture.Setup(x => x.GetProperty("id")).Returns(new NumberVariable(culture.Id));
		var runtimeEthnicity = new Mock<IEthnicity>();
		runtimeEthnicity.Setup(x => x.GetProperty("id")).Returns(new NumberVariable(ethnicity.Id));
		ch.Setup(x => x.GetProperty("culture")).Returns(runtimeCulture.Object);
		ch.Setup(x => x.GetProperty("ethnicity")).Returns(runtimeEthnicity.Object);
		long[] Execute(params string[] selected)
		{
			var traits = selected.Select(key =>
			{
				var trait = new Mock<ITraitDefinition>();
				trait.SetupGet(x => x.Id).Returns(languages[key].Id);
				trait.SetupGet(x => x.Type).Returns(ProgVariableTypes.Trait);
				trait.SetupGet(x => x.GetObject).Returns(trait.Object);
				trait.Setup(x => x.GetProperty("id")).Returns(new NumberVariable(languages[key].Id));
				trait.Setup(x => x.GetProperty("name")).Returns(new TextVariable(languages[key].Name));
				return trait.Object;
			}).ToList();
			ch.Setup(x => x.GetProperty("skills")).Returns(new CollectionVariable(traits, ProgVariableTypes.Trait));
			return progs.Get(target.Id)!.ExecuteCollection<IKnowledge>(ch.Object).Select(x => x.Id).ToArray();
		}
		CollectionAssert.AreEqual(new[] { scripts["latin"].Id }, Execute("literacy", "legacy:Serbo-Croatian", "greek.medieval"));
		Assert.AreEqual(0, Execute("legacy:Serbo-Croatian").Length);
		Assert.AreEqual(0, Execute("literacy", "greek.medieval").Length);
		Assert.IsTrue(progs.Get(resolution.AppliesProgId)!.ExecuteBool(ch.Object));
		runtimeCulture.Setup(x => x.GetProperty("id")).Returns(new NumberVariable(999));
		runtimeEthnicity.Setup(x => x.GetProperty("id")).Returns(new NumberVariable(999));
		CollectionAssert.AreEqual(new[] { scripts["greek"].Id }, Execute("literacy", "greek.medieval"));
	}
}
