#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
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
using RuntimeProg = MudSharp.FutureProg.FutureProg;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CultureToolkitStartingValueTests
{
	[TestMethod]
	public void WelshEnglishNobilityExecutesEditableMaxBasesAndOriginalBoostDeltaExactlyOnce()
	{
		FutureProgTestBootstrap.EnsureInitialised();
		using var context = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString(), x => x.EnableNullChecks(false)).Options);
		var original = new MudSharp.Models.FutureProg
		{
			FunctionName = "FixtureOriginalStart", FunctionText = "return 30 + @boosts * 12", ReturnType = (long)ProgVariableTypes.Number
		};
		var parameters = new[] { (ProgVariableTypes.Toon, "ch"), (ProgVariableTypes.Trait, "trait"), (ProgVariableTypes.Number, "boosts") };
		for (var i = 0; i < parameters.Length; i++) original.FutureProgsParameters.Add(new MudSharp.Models.FutureProgsParameter
		{
			ParameterIndex = i, ParameterName = parameters[i].Item2, ParameterType = (long)parameters[i].Item1
		});
		context.FutureProgs.Add(original);
		context.SaveChanges();
		var catalogue = new CultureToolkitCatalogue();
		var whole = catalogue.Compose("medieval");
		var cultureRow = whole.Cultures.Single(x => CultureToolkitCatalogue.Text(x, "key") == "culture.english-nobility");
		var ethnicityRow = whole.Ethnicities.Single(x => CultureToolkitCatalogue.Text(x, "key") == "ethnicity.welsh");
		var pack = whole with { Cultures = [cultureRow], Ethnicities = [ethnicityRow], Groups = [] };
		var culture = new MudSharp.Models.Culture { Name = "English Nobility", SkillStartingValueProgId = original.Id };
		var ethnicity = new MudSharp.Models.Ethnicity { Name = "Welsh" };
		context.Cultures.Add(culture);
		context.Ethnicities.Add(ethnicity);
		var languages = new[] { "welsh", "english.middle", "french.anglonorman", "latin" }.ToDictionary(x => x,
			x => new MudSharp.Models.TraitDefinition { Name = x });
		context.TraitDefinitions.AddRange(languages.Values);
		context.SaveChanges();
		var result = CultureToolkitStartingProgs.Upsert(context, catalogue, pack,
			new Dictionary<string, MudSharp.Models.Culture> { ["culture.english-nobility"] = culture },
			new Dictionary<string, MudSharp.Models.Ethnicity> { ["ethnicity.welsh"] = ethnicity }, languages, null, []);
		var world = new Mock<IFuturemud>();
		var progs = new All<IFutureProg>();
		var traits = new All<ITraitDefinition>();
		world.SetupGet(x => x.FutureProgs).Returns(progs);
		world.SetupGet(x => x.Traits).Returns(traits);
		world.SetupGet(x => x.SaveManager).Returns(Mock.Of<MudSharp.Framework.Save.ISaveManager>());
		var runtimeTraits = languages.ToDictionary(x => x.Key, x =>
		{
			var trait = new Mock<ITraitDefinition>();
			trait.SetupGet(y => y.Id).Returns(x.Value.Id);
			trait.SetupGet(y => y.Type).Returns(ProgVariableTypes.Trait);
			trait.SetupGet(y => y.GetObject).Returns(trait.Object);
			trait.Setup(y => y.GetProperty("id")).Returns(new NumberVariable(x.Value.Id));
			traits.Add(trait.Object);
			return trait.Object;
		});
		foreach (var model in context.FutureProgs.Include(x => x.FutureProgsParameters)) progs.Add(new RuntimeProg(model, world.Object));
		foreach (var prog in progs) Assert.IsTrue(prog.Compile(), prog.CompileError);
		var runtimeCulture = new Mock<ICulture>();
		runtimeCulture.Setup(x => x.GetProperty("id")).Returns(new NumberVariable(culture.Id));
		var runtimeEthnicity = new Mock<IEthnicity>();
		runtimeEthnicity.Setup(x => x.GetProperty("id")).Returns(new NumberVariable(ethnicity.Id));
		var ch = new Mock<IChargen>();
		ch.SetupGet(x => x.Type).Returns(ProgVariableTypes.Chargen);
		ch.SetupGet(x => x.GetObject).Returns(ch.Object);
		ch.Setup(x => x.GetProperty("culture")).Returns(runtimeCulture.Object);
		ch.Setup(x => x.GetProperty("ethnicity")).Returns(runtimeEthnicity.Object);
		var wrapper = progs.Get(result.OriginalToWrapperProgIds[original.Id]);
		foreach (var expected in new Dictionary<string, double> { ["welsh"] = 200, ["english.middle"] = 180, ["french.anglonorman"] = 150, ["latin"] = 50 })
		{
			Assert.AreEqual(expected.Value, wrapper.ExecuteDouble(ch.Object, runtimeTraits[expected.Key], 0), expected.Key);
			Assert.AreEqual(expected.Value + 24, wrapper.ExecuteDouble(ch.Object, runtimeTraits[expected.Key], 2), expected.Key);
		}
		var unrelated = new Mock<ITraitDefinition>();
		unrelated.SetupGet(x => x.Type).Returns(ProgVariableTypes.Trait);
		unrelated.SetupGet(x => x.GetObject).Returns(unrelated.Object);
		unrelated.Setup(x => x.GetProperty("id")).Returns(new NumberVariable(9999));
		Assert.AreEqual(54.0, wrapper.ExecuteDouble(ch.Object, unrelated.Object, 2));
		var awarded = progs.Get(result.FixedSkillsProgId).ExecuteCollection<ITraitDefinition>(ch.Object);
		CollectionAssert.AreEquivalent(runtimeTraits.Values.Select(x => x.Id).ToArray(), awarded.Select(x => x.Id).ToArray());
		var editableBase = progs.Get(result.NativeBaseProgId);
		editableBase.FunctionText = "return 240";
		Assert.IsTrue(editableBase.Compile());
		Assert.AreEqual(240.0, wrapper.ExecuteDouble(ch.Object, runtimeTraits["welsh"], 0));
		Assert.AreEqual(180.0, wrapper.ExecuteDouble(ch.Object, runtimeTraits["french.anglonorman"], 0));
	}
}
