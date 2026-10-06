#nullable enable
using System;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.Health;
using MudSharp.Magic;
using MudSharp.Magic.SpellEffects;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests;
[TestClass]
public class FiveUtilityStockTests
{
	[ClassInitialize]public static void Initialise(TestContext _)=>FutureProgTestBootstrap.EnsureInitialised();
	[DataTestMethod]
	[DataRow("sense","arm.spell.sense_enchantment",60,7.0,"detectmagick")]
	[DataRow("unravel","arm.spell.unravel_enchantment",60,7.0,"dispelmagic")]
	[DataRow("mend","arm.spell.mend_flesh",30,20.0,"heal")]
	[DataRow("draw","arm.spell.draw_water",30,7.0,"createliquid")]
	[DataRow("hover","arm.spell.hovering_light",30,7.0,"createitem")]
	public void StockDefinitions_ReloadEditableNativeProfilesAndSelectedAdapters(string stock,string key,int opening,double minimum,string adapter)
	{
		var f=new MagicCastingFixture();var model=(MudSharp.Models.MagicSpell)typeof(MagicSpell).GetMethod("SnapshotModel",BindingFlags.NonPublic|BindingFlags.Instance)!.Invoke(f.Spell,[])!;
		var xml=Definition(stock);model.Definition=xml.ToString();var spell=new MagicSpell(model,f.World.Object);
		Assert.AreEqual(key,spell.StockIdentity);Assert.AreEqual(opening,spell.GradeProfile!.OpeningSkill);Assert.AreEqual(minimum,spell.GradeProfile.Efficiency!.MinimumCost);
		Assert.AreEqual(7,spell.GradeProfile.Grades.Count);Assert.IsTrue(spell.GradeProfile.Practice!.Enabled);Assert.IsNull(spell.GradeProfile.Practice.MaximumGrade);
		Assert.AreEqual(adapter,(string)spell.SpellEffects.Single().SaveToXml().Attribute("type")!);
		var saved=(MudSharp.Models.MagicSpell)typeof(MagicSpell).GetMethod("SnapshotModel",BindingFlags.NonPublic|BindingFlags.Instance)!.Invoke(spell,[])!;
		var reloaded=new MagicSpell(saved,f.World.Object);Assert.AreEqual(spell.StockIdentity,reloaded.StockIdentity);Assert.AreEqual(minimum,reloaded.GradeProfile!.Efficiency!.MinimumCost);
		if(stock=="unravel"){var dispel=(DispelMagicEffect)reloaded.SpellEffects.Single();Assert.IsTrue(dispel.Contest);Assert.AreEqual(DispelCasterPolicy.AnyCaster,dispel.CasterPolicy);Assert.IsTrue(dispel.AllowHostile);Assert.AreEqual("any",dispel.EffectKey);}
		if(stock=="mend"){var heal=(HealEffect)reloaded.SpellEffects.Single();Assert.IsTrue(heal.HealWorstWoundsFirst&&heal.HealOverflow);Assert.AreEqual("2*grade*grade",heal.HealingAmount.OriginalFormulaText);}
		if(stock=="draw"){var liquid=(CreateLiquidEffect)reloaded.SpellEffects.Single();Assert.IsTrue(liquid.ContainerOnly);Assert.AreEqual("0.5*grade",liquid.LitresExpression!.OriginalFormulaText);Assert.AreEqual("2",(string)liquid.SaveToXml().Element("ContainerFill")!.Element("BonusPlane")!.Attribute("multiplier")!);}
		if(stock=="hover"){var item=(CreateItemEffect)reloaded.SpellEffects.Single();Assert.IsTrue(item.WornLight);Assert.IsNull(item.PermanentGrade);Assert.AreEqual("1800*grade",item.LifetimeExpression!.OriginalFormulaText);Assert.AreEqual(SpellLifecycleMode.TemporaryCleanup,item.LifecycleMode);}
	}
	private static XElement Definition(string stock)=>stock switch {
		"sense"=>ArmageddonSenseEnchantmentStock.Definition(10,1,1),"unravel"=>ArmageddonUnravelEnchantmentStock.Definition(10,1,0),
		"mend"=>ArmageddonMendFleshStock.Definition(10,1,1),"draw"=>ArmageddonDrawWaterStock.Definition(10,1,1,1,2),
		_=>ArmageddonHoveringLightStock.Definition(10,1,0,1)};

	[DataTestMethod][DataRow(1,2.0)][DataRow(7,98.0)]
	public void MendFlesh_SelectedGradeBindsNativeWorstFirstBudget(int grade,double budget)
	{
		var f=new MagicCastingFixture();var model=(MudSharp.Models.MagicSpell)typeof(MagicSpell).GetMethod("SnapshotModel",BindingFlags.NonPublic|BindingFlags.Instance)!.Invoke(f.Spell,[])!;
		model.Definition=Definition("mend").ToString();var spell=new MagicSpell(model,f.World.Object);var copy=spell.CastingCopy(f.Actor.Object,f.Traits[0],grade,SpellPower.ExtremelyWeak,Difficulty.Normal,7);
		var worse=new Mock<IWound>();worse.SetupProperty(x=>x.CurrentDamage,100);worse.Setup(x=>x.CanBeTreated(TreatmentType.Mend)).Returns(Difficulty.Normal);
		var lesser=new Mock<IWound>();lesser.SetupProperty(x=>x.CurrentDamage,3);lesser.Setup(x=>x.CanBeTreated(TreatmentType.Mend)).Returns(Difficulty.Normal);
		var impossible=new Mock<IWound>();impossible.SetupProperty(x=>x.CurrentDamage,200);impossible.Setup(x=>x.CanBeTreated(TreatmentType.Mend)).Returns(Difficulty.Impossible);
		f.Body.SetupGet(x=>x.Wounds).Returns([lesser.Object,impossible.Object,worse.Object]);
		var report=((IMagicSpellEffectOperation)copy.SpellEffects.Single()).Apply(f.Actor.Object,f.Actor.Object,default,default,null!,[]);
		Assert.AreEqual(MagicEffectOperationStatus.Applied,report.Status);Assert.AreEqual(100-budget,worse.Object.CurrentDamage);Assert.AreEqual(3,lesser.Object.CurrentDamage);Assert.AreEqual(200,impossible.Object.CurrentDamage);
	}
	[DataTestMethod]
	[DataRow("Silt",false,false)][DataRow("sIlT",false,false)][DataRow("Fire Plane",true,false)][DataRow("Desert",true,true)][DataRow("Water Plane",true,true)]
	public void StockTerrainProgs_CompileAndEvaluateNativeVariables(string name,bool sense,bool water)
	{
		var world=new Mock<IFuturemud>();var terrain=new Mock<ITerrain>();terrain.SetupGet(x=>x.Type).Returns(ProgVariableTypes.Terrain);terrain.SetupGet(x=>x.GetObject).Returns(terrain.Object);terrain.Setup(x=>x.GetProperty("name")).Returns(new TextVariable(name));
		var cell=new Mock<ICell>();cell.SetupGet(x=>x.Type).Returns(ProgVariableTypes.Location);cell.SetupGet(x=>x.GetObject).Returns(cell.Object);cell.Setup(x=>x.GetProperty("terrain")).Returns(terrain.Object);
		var actor=new Mock<ICharacter>();actor.SetupGet(x=>x.Type).Returns(ProgVariableTypes.Character);actor.SetupGet(x=>x.GetObject).Returns(actor.Object);actor.Setup(x=>x.GetProperty("location")).Returns(cell.Object);
		bool Evaluate(string source,ProgVariableTypes targetType){var prog=new MudSharp.FutureProg.FutureProg(world.Object,"fiveStockTerrainTest",ProgVariableTypes.Boolean,[Tuple.Create(targetType,"target"),Tuple.Create(ProgVariableTypes.Character,"caster")],source);Assert.IsTrue(prog.Compile(),prog.CompileError);return prog.ExecuteBool(null!,actor.Object);}
		Assert.AreEqual(sense,Evaluate(ArmageddonSenseEnchantmentStock.EligibilitySource,ProgVariableTypes.Character));Assert.AreEqual(water,Evaluate(ArmageddonDrawWaterStock.EligibilitySource,ProgVariableTypes.Item));
	}
}
