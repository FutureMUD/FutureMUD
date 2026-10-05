#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.Magic;
using MudSharp.Magic.Casting;
using MudSharp.Magic.Vancian;
namespace MudSharp_Unit_Tests;
[TestClass]
public class CastingTargetFilterTests
{
	[DataTestMethod]
	[DataRow("character", "compile")][DataRow("item", "compile")]
	[DataRow("character", "missing")][DataRow("item", "missing")]
	[DataRow("character", "null")][DataRow("item", "null")]
	[DataRow("character", "failed")][DataRow("item", "failed")]
	[DataRow("character", "signature")][DataRow("item", "signature")]
	public void ConfiguredInvalidPolicies_RefuseBeforePaymentAndRetainReference(string type, string failure)
	{
		var f = new MagicCastingFixture(); var prog = Policy(72, failure);
		var progs = new List<IFutureProg> { f.World.Object.FutureProgs.Get(1), prog.Object };
		f.World.SetupGet(x => x.FutureProgs).Returns(MagicCastingFixture.Collection(() => progs));
		Assert.IsTrue(f.Spell.BuildingCommand(f.Actor.Object, new StringStack("trigger new " + type)));
		Assert.IsTrue(f.Spell.BuildingCommand(f.Actor.Object,new StringStack("grades scalar remove target 0")));
		((List<IMagicSpellEffectTemplate>)f.Spell.SpellEffects).Clear();
		SetFilter(f.Spell, type, 72);
		f.Actor.Setup(x => x.TargetActorOrCorpse("target", It.IsAny<PerceiveIgnoreFlags>())).Returns(f.Actor.Object);
		f.Actor.Setup(x => x.TargetItem("target")).Returns(Mock.Of<IGameItem>());
		f.Acquire(); if (failure == "missing") progs.Remove(prog.Object);
		if (failure is "missing" or "compile" or "signature") {
			Assert.IsFalse(f.Spell.ReadyForGame);
			StringAssert.Contains(f.Spell.WhyNotReadyForGame(f.Actor.Object), "Target filter");
		} else Assert.IsTrue(f.Spell.ReadyForGame);
		var balances = new Dictionary<IMagicResource,double>(f.Balances); var writes = f.Store.Writes;
		var result = f.Service.Cast(f.Intent() with { Targets = "target" });
		Assert.AreEqual(MagicCastingStatus.Refused,result.Status,result.Message);
		CollectionAssert.AreEquivalent(balances.Values.ToArray(),f.Balances.Values.ToArray());
		Assert.AreEqual(writes,f.Store.Writes);Assert.IsNull(result.OperationId);Assert.AreEqual(0,f.Rolls);
		if(failure is "null" or "failed")prog.Verify(x=>x.ExecuteWithStatus(out It.Ref<object>.IsAny,It.IsAny<object[]>()),Times.AtLeastOnce());
		var saved=f.Spell.Trigger.SaveToXml();Assert.AreEqual(72L,(long)saved.Element("TargetFilterProg")!);
		Assert.AreEqual(72L,(long)f.Spell.Trigger.Clone().SaveToXml().Element("TargetFilterProg")!);
	}
	[DataTestMethod][DataRow("character",0L)][DataRow("item",0L)][DataRow("character",72L)][DataRow("item",72L)]
	public void AbsentOrHealthyPolicies_PreserveNativeAndDeviceTargetResolution(string type,long id)
	{
		var f=new MagicCastingFixture();var prog=Policy(72,"healthy");
		f.World.SetupGet(x=>x.FutureProgs).Returns(MagicCastingFixture.Collection(()=>new[]{prog.Object}));
		SetFilter(f.Spell,type,id);
		f.Actor.Setup(x=>x.TargetActorOrCorpse("target",It.IsAny<PerceiveIgnoreFlags>())).Returns(f.Actor.Object);
		f.Actor.Setup(x=>x.TargetItem("target")).Returns(Mock.Of<IGameItem>());
		Assert.IsNotNull(SpellTargetCapture.Resolve(f.Actor.Object,f.Spell,SpellPower.Standard,new StringStack("target"),true));
		Assert.IsTrue(f.Spell.Trigger.BuildingCommand(f.Actor.Object,new StringStack("filterprog clear")));
		Assert.AreEqual(0L,(long)f.Spell.Trigger.SaveToXml().Element("TargetFilterProg")!);
		Assert.IsNotNull(SpellTargetCapture.Resolve(f.Actor.Object,f.Spell,SpellPower.Standard,new StringStack("target"),true));
	}
	internal static Mock<IFutureProg> Policy(long id,string failure)
	{
		var prog=new Mock<IFutureProg>();prog.SetupGet(x=>x.Id).Returns(id);
		prog.SetupGet(x=>x.ReturnType).Returns(ProgVariableTypes.Boolean);
		prog.SetupGet(x=>x.CompileError).Returns(failure=="compile"?"Injected compile failure":"");
		prog.Setup(x=>x.MatchesParameters(It.IsAny<IEnumerable<ProgVariableTypes>>())).Returns(failure!="signature");
		object result=failure=="null"?null!:true;
		prog.Setup(x=>x.ExecuteWithStatus(out result,It.IsAny<object[]>())).Returns(failure!="failed");return prog;
	}
	internal static void SetFilter(MagicSpell spell,string type,long id)
	{
		var root=new XElement("Trigger",new XAttribute("type",type),new XElement("MinimumPower",0),new XElement("MaximumPower",10),new XElement("TargetFilterProg",id),new XElement("CanTargetSelf",true));
		typeof(MagicSpell).GetProperty("Trigger")!.SetValue(spell,SpellTriggerFactory.LoadTrigger(root,spell));
	}
}
