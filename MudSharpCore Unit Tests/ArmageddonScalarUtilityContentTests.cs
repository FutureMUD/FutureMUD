#nullable enable
using System;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body.Traits;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Character;
using MudSharp.Magic;
using MudSharp.Magic.SpellEffects;
using MudSharp.Planes;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonScalarUtilityContentTests
{
	[DataTestMethod]
	[DataRow(1, SpellPower.ExtremelyStrong)]
	[DataRow(7, SpellPower.ExtremelyWeak)]
	[DataRow(3, SpellPower.VeryStrong)]
	public void CoolingShade_NativeChildrenAndColdReloadRespectSelectedGrade(int grade, SpellPower power)
	{
		var f = new MagicCastingFixture();
		var content = ArmageddonScalarUtilityContent.Create()[0].Content;
		var spell = Load(f, content);
		var original = spell.SpellEffects.Select(x => x.SaveToXml()).ToArray();
		var copy = spell.CastingCopy(f.Actor.Object, f.Traits[0], grade, power, Difficulty.Normal);
		Assert.AreEqual(60.0 * grade, copy.EffectDurationExpression.Evaluate(f.Actor.Object));
		Assert.AreEqual(0, copy.GradeConfigurationErrors().Count);
		var parent = new MagicSpellParent(f.Actor.Object.Location, copy, f.Actor.Object, power);
		var children = copy.SpellEffects.Select(x => x.GetOrApplyEffect(f.Actor.Object, f.Actor.Object.Location,
			OpposedOutcomeDegree.None, power, parent, [])).ToArray();
		Assert.IsInstanceOfType(children[0], typeof(SpellRoomLightEffect));
		Assert.IsInstanceOfType(children[1], typeof(SpellRoomTemperatureEffect));
		Assert.AreEqual(-10.0 * grade, ((IAreaLightEffect)children[0]!).AddedLight, 1e-10);
		Assert.AreEqual(-0.5 * grade, ((IAffectEnvironmentalTemperature)children[1]!).TemperatureDelta, 1e-10);
		MagicSpellParent.InitialiseEffectType(); SpellRoomLightEffect.InitialiseEffectType(); SpellRoomTemperatureEffect.InitialiseEffectType();
		foreach (var child in children) parent.AddSpellEffect(child!);
		var saved = parent.SaveToXml(new() { [parent] = TimeSpan.FromSeconds(60 * grade) });
		var cold = saved.Element("Effect")!.Element("Children")!.Elements().Last();
		Assert.AreEqual("SpellRoomTemperature", cold.Element("Type")!.Value);
		var loaded = (IAffectEnvironmentalTemperature)Effect.LoadEffect(cold, f.Actor.Object.Location);
		Assert.AreEqual(-0.5 * grade, loaded.TemperatureDelta, 1e-10);
		var loadedParent = (MagicSpellParent)Effect.LoadEffect(saved, f.Actor.Object.Location);
		Assert.AreEqual(2, loadedParent.SpellEffects.Count());
		Assert.AreEqual(-0.5 * grade, ((IAffectEnvironmentalTemperature)loadedParent.SpellEffects.Last()).TemperatureDelta, 1e-10);
		Assert.AreEqual(60000L * grade, (long)saved.Element("Remaining")!);
		foreach (var pair in spell.SpellEffects.Zip(original))
		{
			Assert.IsTrue(XNode.DeepEquals(pair.First.SaveToXml(), pair.Second));
			Assert.IsTrue(XNode.DeepEquals(pair.First.Clone().SaveToXml(), pair.Second));
		}
	}

	[TestMethod]
	public void CoolingShade_SourcePriceAndUnboundLegacyFieldsRemainExact()
	{
		var f=new MagicCastingFixture();var content=ArmageddonScalarUtilityContent.Create().Single().Content;
		Assert.AreEqual(20.0,content.MinimumEnergy);Assert.AreEqual("20*grade",content.CostRow(1).Expression);
		var row=Load(f,content).SnapshotModel();var xml=XElement.Parse(row.Definition);
		xml.Element("ControlledPower")!.Element("ScalarBindings")!.RemoveNodes();row.Definition=xml.ToString();
		var copy=new MagicSpell(row,f.World.Object).CastingCopy(f.Actor.Object,f.Traits[0],7,SpellPower.ExtremelyStrong,Difficulty.Normal);
		Assert.AreEqual(-10.0,((RoomLightEffect)copy.SpellEffects.First()).AddedLuxPerPower);
		Assert.AreEqual(-0.5,((RoomTemperatureEffect)copy.SpellEffects.Last()).TemperatureDeltaPerPower);
	}

	[DataTestMethod]
	[DataRow(0,"1/0")] [DataRow(1,"sqrt(-1)")]
	public void CoolingShade_InvalidScalarEvaluationRefusesBeforePayment(int index,string formula)
	{
		var f=new MagicCastingFixture();f.Acquire();var content=ArmageddonScalarUtilityContent.Create().Single().Content;
		var row=Load(f,content).SnapshotModel();var xml=XElement.Parse(row.Definition);
		xml.Element("ControlledPower")!.Element("ScalarBindings")!.Elements().ElementAt(index).Value=formula;
		row.Definition=xml.ToString();f.Spells[0]=new MagicSpell(row,f.World.Object);
		var result=f.Service.Cast(f.Intent(1,false) with {Targets="here"});
		Assert.AreEqual(MagicCastingStatus.Refused,result.Status,result.Message);StringAssert.Contains(result.Message,"target["+index+"]");
		Assert.AreEqual(100.0,f.Balances[f.Resources[1]]);Assert.AreEqual(0,f.Rolls);Assert.AreEqual(0,f.Store.Operations.Count);
	}

	private static MagicSpell Load(MagicCastingFixture f,ArmageddonUtilitySpellContent content)
	{
		FutureProgTestBootstrap.EnsureInitialised();
		Mock.Get(f.Actor.Object.Location).SetupGet(x => x.Gameworld).Returns(f.World.Object);
		Mock.Get(f.World.Object.LightModel).Setup(x => x.GetIlluminationDescription(It.IsAny<double>())).Returns("dim");
		f.Expressions.Clear();var cost=content.CostRow(1);cost.Id=1;var duration=content.DurationRow(1);duration.Id=2;
		f.Expressions.Add(new TraitExpression(cost,f.World.Object));f.Expressions.Add(new TraitExpression(duration,f.World.Object));
		var row=content.SpellRow(1,1,1);row.Id=1;row.EffectDurationExpressionId=2;row.Definition=content.BuildDefinition(10,1,0).ToString();
		return new MagicSpell(row,f.World.Object);
	}
}
