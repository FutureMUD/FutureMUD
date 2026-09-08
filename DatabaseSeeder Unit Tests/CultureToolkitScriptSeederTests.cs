#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using DatabaseSeeder.Seeders.CultureToolkit;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Models;

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
	}
}
