#nullable enable
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Moq;
using MudSharp.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Magic;
using MudSharp.Magic.SpellEffects;
using MudSharp.Magic.Lifecycle;
namespace MudSharp_Unit_Tests;

[TestClass]
public class ProvisionStockTests
{
	private sealed class FoodEffect(XElement root, IMagicSpell spell) : CreateItemEffect(root, spell);
	[DataTestMethod][DataRow("empty")][DataRow("missing")][DataRow("duplicate")][DataRow("version")][DataRow("fallback")]
	public void MalformedFoodProfiles_RetainAuthoredXmlAndRefuseClone(string failure)
	{
		var world = new Mock<IFuturemud> { DefaultValue = DefaultValue.Mock };
		var xml = ArmageddonSustainMealStock.Definition(10, 1, 1, 2, 3).Element("Effects")!.Element("Effect")!;
		var profiles = xml.Element("Lifecycle")!.Element("FoodProfiles")!;
		if (failure == "empty") profiles.RemoveNodes();
		if (failure == "missing") profiles.Element("Profile")!.Attribute("predicate")!.Remove();
		if (failure == "duplicate") profiles.Add(new XElement(profiles.Element("Profile")!));
		if (failure == "version") profiles.Attribute("version")!.Value = "99";
		if (failure == "fallback") profiles.Element("Profile")!.Attribute("predicate")!.Value = "72";
		var effect = new FoodEffect(xml, Mock.Of<IMagicSpell>(x => x.Gameworld == world.Object));
		Assert.IsNotNull(effect.DefinitionError); Assert.IsTrue(XNode.DeepEquals(profiles, effect.SaveToXml().Element("Lifecycle")!.Element("FoodProfiles")));
		Assert.IsNotNull(((CreateItemEffect)effect.Clone()).DefinitionError);
	}
	[DataTestMethod][DataRow(false)][DataRow(true)]
	public void EditableStockDefinitions_ReloadNativeProfilesAndSourceContracts(bool food)
	{
		var f = new MagicCastingFixture();
		var model = (MudSharp.Models.MagicSpell)typeof(MagicSpell).GetMethod("SnapshotModel", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(f.Spell, [])!;
		model.Definition = (food ? ArmageddonSustainMealStock.Definition(10, 1, 1) : ArmageddonDrawWineStock.Definition(10, 1, 1, 1, 2)).ToString();
		var spell = new MagicSpell(model, f.World.Object);
		Assert.AreEqual(food ? ArmageddonSustainMealStock.Key : ArmageddonDrawWineStock.Key, spell.StockIdentity);
		Assert.AreEqual(30, spell.GradeProfile!.OpeningSkill); Assert.AreEqual(7.0, spell.GradeProfile.Efficiency!.MinimumCost);
		Assert.IsTrue(spell.GradeProfile.Practice!.Enabled); Assert.IsNull(spell.GradeProfile.Practice.MaximumGrade);
		if (food) {
			var effect = (CreateItemEffect)spell.SpellEffects.Single();
			Assert.IsTrue(effect.CountByGrade); Assert.AreEqual(SpellLifecycleMode.TemporaryCleanup, effect.LifecycleMode);
			Assert.AreEqual("1350*grade", effect.LifetimeExpression!.OriginalFormulaText); Assert.IsNull(effect.PermanentGrade);
		} else {
			var effect = (CreateLiquidEffect)spell.SpellEffects.Single();
			Assert.IsTrue(effect.ContainerOnly); Assert.AreEqual("0.5*grade", effect.LitresExpression!.OriginalFormulaText);
			Assert.AreEqual("2", (string)effect.SaveToXml().Element("ContainerFill")!.Element("BonusPlane")!.Attribute("multiplier")!);
		}
		var saved = (MudSharp.Models.MagicSpell)typeof(MagicSpell).GetMethod("SnapshotModel", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(spell, [])!;
		Assert.AreEqual(spell.StockIdentity, new MagicSpell(saved, f.World.Object).StockIdentity);
	}
}
