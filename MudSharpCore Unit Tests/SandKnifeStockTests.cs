#nullable enable

using System;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Climate;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.Magic;
using MudSharp.Magic.SpellEffects;

namespace MudSharp_Unit_Tests;

[TestClass]
public class SandKnifeStockTests
{
	[ClassInitialize]
	public static void Initialise(TestContext _) => FutureProgTestBootstrap.EnsureInitialised();

	[DataTestMethod]
	[DataRow("Desert", false, 0L, true)]
	[DataRow("Earth Plane", false, 0L, true)]
	[DataRow("Silt", false, 0L, true)]
	[DataRow("Shallows", false, 0L, true)]
	[DataRow("Salt Flats", false, 0L, true)]
	[DataRow("Field", true, 0L, true)]
	[DataRow("Field", false, 3L, true)]
	[DataRow("Field", false, 4L, false)]
	[DataRow("Field", false, 0L, false)]
	[DataRow("Water Plane", false, 0L, false)]
	[DataRow("Water Plane", true, 0L, true)]
	[DataRow("Shadow Plane", false, 0L, false)]
	[DataRow("Shadow Plane", true, 0L, true)]
	[DataRow("Inside", true, 3L, false)]
	[DataRow("City", true, 3L, false)]
	public void EnvironmentProg_TerrainFlagAndExplicitWeatherMapping_EvaluatesNativeVariables(string terrainName, bool stormFlag, long weatherId, bool allowed)
	{
		Assert.AreEqual(allowed, Environment(terrainName,stormFlag,weatherId,2,true));
	}

	[TestMethod]
	public void EnvironmentProg_LargeStormTagAndMissingMapping_PreservesIdentityAndRefusesMissing()
	{
		Assert.IsTrue(Environment("Field",true,0,(long)int.MaxValue+7,true));
		Assert.IsFalse(Environment("Field",true,0,(long)int.MaxValue+7,false));
	}

	private static bool Environment(string terrainName, bool stormFlag, long weatherId, long tagId, bool includeTag)
	{
		var world = new Mock<IFuturemud>();
		var tagged = new Mock<ITag>(); tagged.SetupGet(x => x.Id).Returns(tagId); tagged.SetupGet(x => x.GetObject).Returns(tagged.Object); tagged.SetupGet(x => x.Type).Returns(ProgVariableTypes.Tag);
		var tag = tagged.Object; var tags = new All<ITag>(); if(includeTag) tags.Add(tag); world.SetupGet(x => x.Tags).Returns(tags);
		var terrain = new Mock<ITerrain>(); terrain.SetupGet(x => x.GetObject).Returns(terrain.Object); terrain.SetupGet(x => x.Type).Returns(ProgVariableTypes.Terrain);
		terrain.Setup(x => x.GetProperty("name")).Returns(new TextVariable(terrainName));
		var room = new Mock<IRoom>(); room.SetupGet(x => x.GetObject).Returns(room.Object); room.SetupGet(x => x.Type).Returns(ProgVariableTypes.Location);
		room.Setup(x => x.GetProperty("terrain")).Returns(terrain.Object); room.Setup(x => x.IsA(tag)).Returns(stormFlag);
		var weather = new Mock<IWeatherEvent>(); weather.SetupGet(x => x.GetObject).Returns(weather.Object); weather.SetupGet(x => x.Type).Returns(ProgVariableTypes.WeatherEvent);
		weather.Setup(x => x.GetProperty("id")).Returns(new NumberVariable(weatherId));
		room.Setup(x => x.GetProperty("weather")).Returns(weatherId == 0 ? new NullVariable(ProgVariableTypes.WeatherEvent) : weather.Object);
		var caster = new Mock<ICharacter>(); caster.SetupGet(x => x.GetObject).Returns(caster.Object); caster.SetupGet(x => x.Type).Returns(ProgVariableTypes.Character);
		caster.Setup(x => x.GetProperty("location")).Returns(room.Object);
		var prog = new MudSharp.FutureProg.FutureProg(world.Object, "sand_test_environment", ProgVariableTypes.Boolean,
			[new Tuple<ProgVariableTypes, string>(ProgVariableTypes.Character, "caster")], ArmageddonSandKnifeStock.EligibilitySource(tagId,3));
		Assert.IsTrue(prog.Compile(), prog.CompileError);
		return prog.ExecuteBool(caster.Object);
	}

	[TestMethod]
	public void Definition_MonOnlyCreationAndIndependentDuration_RoundTripsIntoNativePolicy()
	{
		var knives = Enumerable.Range(10,6).Select(x => (long)x).ToArray(); var staffs = Enumerable.Range(30,8).Select(x => (long)x).ToArray();
		var f = new MagicCastingFixture();
		var definition = ArmageddonSandKnifeStock.Definition(10,1,knives,staffs,6,9);
		var model = (MudSharp.Models.MagicSpell)typeof(MagicSpell).GetMethod("SnapshotModel", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(f.Spell,[])!;
		model.Definition = definition.ToString();
		var loaded = new MagicSpell(model,f.World.Object); var effect = (CreateItemEffect)loaded.SpellEffects.Single();
		var lifecycle = effect.SaveToXml().Element("Lifecycle")!;
		Assert.AreEqual(ArmageddonSandKnifeStock.Key, loaded.StockIdentity);
		Assert.AreEqual(ArmageddonSandKnifeStock.MinimumEnergy, loaded.GradeProfile!.Efficiency!.MinimumCost);
		Assert.AreEqual("sand-knife", lifecycle.Element("Family")!.Value);
		Assert.IsNull(lifecycle.Element("LifetimeMultiplierProg"), "Sand does not inherit a shadow-plane lifetime reduction.");
		Assert.AreEqual("7", lifecycle.Element("PermanentOutput")!.Attribute("grade")!.Value);
		var action = definition.Element("Plan")!.Element("Phase")!.Element("Action")!;
		Assert.AreEqual("6", action.Attribute("tag")!.Value); Assert.AreEqual("7", action.Attribute("grade")!.Value);
		CollectionAssert.AreEqual(staffs, lifecycle.Element("GradeOutputs")!.Elements("Output").Single(x => x.Attribute("grade")!.Value=="7").Elements("Prototype").Select(x=>long.Parse(x.Value)).ToArray());
		Assert.AreEqual(5.0, new ControlledSpellEfficiency(ArmageddonSandKnifeStock.MinimumEnergy,1).MinimumCost);
	}
}
