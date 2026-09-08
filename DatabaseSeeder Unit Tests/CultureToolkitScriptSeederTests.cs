#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using DatabaseSeeder.Seeders.CultureToolkit;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Models;
using MudSharp.Framework;
using MudSharp.FutureProg.Variables;
using MudSharp.FutureProg;
using ITraitDefinition = MudSharp.Body.Traits.ITraitDefinition;
using Moq;
using FutureProg = MudSharp.Models.FutureProg;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CultureToolkitScriptSeederTests
{
	[TestMethod]
	public void ScriptRerunPreservesBuilderMembershipDeletionModifiersAndKnowledgeAcquisition()
	{
		using var context = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString(), x => x.EnableNullChecks(false)).Options);
		var alwaysTrue = new FutureProg { FunctionName = "AlwaysTrue", FunctionText = "return true", ReturnType = (long)MudSharp.FutureProg.ProgVariableTypes.Boolean };
		context.Add(alwaysTrue);
		var english = new Language { Name = "English", LinkedTrait = new TraitDefinition { Name = "English" } };
		var builderLanguage = new Language { Name = "Builder language", LinkedTrait = new TraitDefinition { Name = "Builder language" } };
		context.AddRange(english, builderLanguage);
		context.SaveChanges();
		var catalogue = new CultureToolkitCatalogue();
		var pack = catalogue.Compose("earlymodern");
		var languages = new Dictionary<string, Language> { ["english.earlymodern"] = english };
		var conflicts = new List<string>();
		var scripts = CultureToolkitScriptSeeder.Upsert(context, catalogue, pack, new Dictionary<string, FuturemudDatabaseContext>(), languages, new Dictionary<string, Script>(), conflicts);
		var latin = scripts["latin"];
		Assert.AreEqual(1, scripts.Count);
		Assert.IsNotNull(context.ScriptsDesignedLanguages.Find(latin.Id, english.Id));
		Assert.AreEqual(6, latin.Knowledge.LearnableType);
		latin.DocumentLengthModifier = 1.75;
		latin.Knowledge.CanAcquireProgId = alwaysTrue.Id;
		context.Remove(context.ScriptsDesignedLanguages.Find(latin.Id, english.Id)!);
		context.Add(new ScriptsDesignedLanguage { ScriptId = latin.Id, LanguageId = builderLanguage.Id });
		context.SaveChanges();
		for (var i = 0; i < 2; i++) CultureToolkitScriptSeeder.Upsert(context, catalogue, pack,
			new Dictionary<string, FuturemudDatabaseContext>(), languages, new Dictionary<string, Script>(), conflicts);
		Assert.AreEqual(1, context.Scripts.Count());
		Assert.AreEqual(1.75, latin.DocumentLengthModifier);
		Assert.AreEqual(alwaysTrue.Id, latin.Knowledge.CanAcquireProgId);
		Assert.IsNull(context.ScriptsDesignedLanguages.Find(latin.Id, english.Id));
		Assert.IsNotNull(context.ScriptsDesignedLanguages.Find(latin.Id, builderLanguage.Id));
		Assert.IsTrue(conflicts.Any(x => x.Contains("builder edit")));
		var managedProg = context.FutureProgs.Single(x => x.FunctionName.StartsWith("CultureScript"));
		using var compiler = new OfflineProgCompilation(context.FutureProgs.Include(x => x.FutureProgsParameters).ToArray());
		var executable = compiler.Compile(managedProg.Id);
		var trait = new Mock<ITraitDefinition>();
		trait.SetupGet(x => x.Type).Returns(ProgVariableTypes.Trait);
		trait.SetupGet(x => x.GetObject).Returns(trait.Object);
		trait.Setup(x => x.GetProperty("id")).Returns(new NumberVariable(english.LinkedTraitId));
		Assert.IsFalse(executable.ExecuteBool(null, trait.Object), "Deleted English membership must not authorize acquisition.");
		trait.Setup(x => x.GetProperty("id")).Returns(new NumberVariable(builderLanguage.LinkedTraitId));
		Assert.IsTrue(executable.ExecuteBool(null, trait.Object), "Actual custom-language trait must authorize acquisition.");
		context.Remove(context.ScriptsDesignedLanguages.Find(latin.Id, builderLanguage.Id)!);
		context.SaveChanges();
		CultureToolkitScriptSeeder.Upsert(context, catalogue, pack, new Dictionary<string, FuturemudDatabaseContext>(), languages,
			new Dictionary<string, Script>(), conflicts);
		using var emptyCompiler = new OfflineProgCompilation(context.FutureProgs.Include(x => x.FutureProgsParameters).ToArray());
		Assert.IsFalse(emptyCompiler.Compile(managedProg.Id).ExecuteBool(null, trait.Object));
		Assert.AreEqual("return false", managedProg.FunctionText);
		managedProg.FunctionText = "return true";
		context.SaveChanges();
		CultureToolkitScriptSeeder.Upsert(context, catalogue, pack, new Dictionary<string, FuturemudDatabaseContext>(), languages,
			new Dictionary<string, Script>(), conflicts);
		Assert.AreEqual("return true", managedProg.FunctionText, "A custom stock-prog body is independent of the effective empty graph.");
		Assert.AreEqual(alwaysTrue.Id, latin.Knowledge.CanAcquireProgId);
	}
}
