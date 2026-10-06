using System;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body.Traits;
using MudSharp.Body.Traits.Subtypes;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Magic;
using MudSharp.Magic.Casting;
using MudSharp.Magic.Resources;

#nullable enable
namespace MudSharp_Unit_Tests;

[TestClass]
public class MagicCastingCapacityTests
{
	[DataTestMethod]
	[DataRow(double.NaN)]
	[DataRow(double.PositiveInfinity)]
	[DataRow(-1.0)]
	public void InvalidResourceCapacity_QuoteAndCastRefuseBeforePaymentOrProgress(double cap)
	{
		var f = new MagicCastingFixture(); f.Acquire();
		Mock.Get(f.Resources[1]).Setup(x => x.ResourceCap(It.IsAny<IHaveMagicResource>())).Returns(cap);
		var writes = f.Store.Writes;
		var quote = f.Service.Quote(f.Intent(1, false));
		Assert.IsFalse(quote.Allowed); StringAssert.Contains(quote.Reason, "capacity");
		Assert.AreEqual(MagicCastingStatus.Refused, f.Service.Cast(f.Intent(1, false)).Status);
		Assert.AreEqual(100.0, f.Balances[f.Resources[1]]); Assert.AreEqual(writes, f.Store.Writes);
		Assert.AreEqual(0, f.Rolls); Assert.AreEqual(0, f.Samples); Assert.AreEqual(0, f.SkillUses);
	}

	[TestMethod]
	public void ReducedMaximum_BalanceAboveCapCannotFundConfiguredCast_QuoteRemainsReadOnly()
	{
		var f = new MagicCastingFixture(); f.Acquire();
		Mock.Get(f.Resources[1]).Setup(x => x.ResourceCap(It.IsAny<IHaveMagicResource>())).Returns(7);
		var writes = f.Store.Writes;
		Assert.IsFalse(f.Service.Quote(f.Intent(2, false)).Allowed, "Ten energy cannot be spent from a seven-unit maximum.");
		Assert.AreEqual(100.0, f.Balances[f.Resources[1]]); Assert.AreEqual(writes, f.Store.Writes);
		Mock.Get(f.Resources[1]).Setup(x => x.ResourceCap(It.IsAny<IHaveMagicResource>())).Returns(100);
		Assert.IsTrue(f.Service.Quote(f.Intent(2, false)).Allowed);
		Assert.AreEqual(100.0, f.Balances[f.Resources[1]]); Assert.AreEqual(writes, f.Store.Writes);
	}

	[TestMethod]
	public void BalanceBelowMaximum_RaisingCapacityDoesNotFundCasting()
	{
		var f = new MagicCastingFixture(); f.Acquire(); f.Balances[f.Resources[1]] = 3;
		Assert.IsFalse(f.Service.Quote(f.Intent(1, false)).Allowed);
		Mock.Get(f.Resources[1]).Setup(x => x.ResourceCap(It.IsAny<IHaveMagicResource>())).Returns(1000);
		Assert.IsFalse(f.Service.Quote(f.Intent(1, false)).Allowed);
		Assert.AreEqual(3.0, f.Balances[f.Resources[1]]); Assert.AreEqual(0, f.Rolls); Assert.AreEqual(0, f.Samples);
	}

	[TestMethod]
	public void AttributeCapacity_ExplicitRawOrEffectiveBinding_ChangesMaximumWithoutSkillGrowthOrRefill()
	{
		var (f, resource, attribute) = AttributeFixture("variable*10");
		Assert.AreEqual(42.0, resource.ResourceCap(f.Actor.Object));
		f.Actor.Setup(x => x.TraitValue(attribute, TraitBonusContext.None)).Returns(() => f.Skills[9] + 2);
		Assert.IsTrue(resource.BuildingCommand(f.Actor.Object, new StringStack("capattribute 9 90 effective")));
		Assert.AreEqual(120.0, resource.ResourceCap(f.Actor.Object));
		Assert.IsTrue(resource.BuildingCommand(f.Actor.Object, new StringStack("capattribute 9 90 raw")));
		Assert.AreEqual(100.0, resource.ResourceCap(f.Actor.Object));
		f.Skills[1] = 999; Assert.AreEqual(100.0, resource.ResourceCap(f.Actor.Object));
		f.Skills[9] = 5; Assert.AreEqual(50.0, resource.ResourceCap(f.Actor.Object));
		Assert.AreEqual(100.0, f.Balances[resource]); Assert.AreEqual(0, f.Store.Writes);
		Assert.IsTrue(resource.BuildingCommand(f.Actor.Object, new StringStack("capattribute none")));
		Assert.AreEqual(42.0, resource.ResourceCap(f.Actor.Object));
	}

	[TestMethod]
	public void AttributeCapacity_SecondaryHolderUsesCanonicalBodyAttribute_XmlRoundTripRetainsSelection()
	{
		var (f, resource, _) = AttributeFixture("variable*10");
		Assert.IsTrue(resource.BuildingCommand(f.Actor.Object, new StringStack("capattribute 9 90 raw")));
		var primary = new Mock<ICharacterInstance>(); var secondary = new Mock<ICharacterInstance>(); var identity = new Mock<ICharacterIdentity>();
		identity.SetupGet(x => x.PrimaryInstance).Returns(primary.Object);
		primary.SetupGet(x => x.Identity).Returns(identity.Object); secondary.SetupGet(x => x.Identity).Returns(identity.Object);
		primary.SetupGet(x => x.Id).Returns(100); secondary.SetupGet(x => x.Id).Returns(500);
		primary.SetupGet(x => x.Gameworld).Returns(f.World.Object); secondary.SetupGet(x => x.Gameworld).Returns(f.World.Object);
		primary.Setup(x => x.HasTrait(It.IsAny<ITraitDefinition>())).Returns(true);
		primary.Setup(x => x.TraitRawValue(It.IsAny<ITraitDefinition>())).Returns(12);
		secondary.Setup(x => x.TraitRawValue(It.IsAny<ITraitDefinition>())).Returns(99);
		Assert.AreEqual(120.0, resource.ResourceCap(secondary.Object));
		var reload = new CapacityResource(resource.Model(), f.World.Object);
		Assert.AreEqual(resource.AttributeCapacity, reload.AttributeCapacity);
		Assert.AreEqual(120.0, reload.ResourceCap(secondary.Object));
		Assert.IsFalse(reload.BuildingCommand(f.Actor.Object, new StringStack("capattribute 1 90 raw")), "A native skill is not a capacity attribute.");
		Assert.AreEqual(resource.AttributeCapacity, reload.AttributeCapacity);
	}

	[DataTestMethod]
	[DataRow(4.0, 48.0)]
	[DataRow(11.5, 95.5)]
	[DataRow(19.0, 118.0)]
	public void AttributeCapacity_HistoricalShapeClampsToNativeAttributeBounds(double value, double expected)
	{
		var (f, resource, _) = AttributeFixture("Min(12*variable,Max(2*variable,100+3*(variable-13)))");
		f.Skills[9] = value;
		Assert.IsTrue(resource.BuildingCommand(f.Actor.Object, new StringStack("capattribute 9 90 raw")));
		Assert.AreEqual(expected, resource.ResourceCap(f.Actor.Object));
		Assert.AreEqual(100.0, f.Balances[resource]);
	}

	[DataTestMethod]
	[DataRow("missing-attribute")]
	[DataRow("missing-expression")]
	[DataRow("malformed-xml")]
	[DataRow("unsupported-version")]
	[DataRow("skill-attribute")]
	[DataRow("skill-parameter")]
	[DataRow("unknown-parameter")]
	[DataRow("random")]
	[DataRow("negative")]
	[DataRow("nonfinite")]
	public void AttributeCapacity_InvalidDefinitionsFailClosedAndNeverFallBackToLegacyCap(string invalid)
	{
		var formula = invalid switch { "skill-parameter" => "variable+skill", "unknown-parameter" => "variable+mastery",
			"random" => "variable+rand(1,10)", "negative" => "-variable", "nonfinite" => "1.0/0.0", _ => "variable*10" };
		var (f, resource, _) = AttributeFixture(formula, invalid == "skill-parameter");
		var xml = XElement.Parse(resource.Model().Definition);
		var attributeXml = new XElement("AttributeCapacity", new XAttribute("version", invalid == "unsupported-version" ? 2 : 1),
			new XAttribute("attribute", invalid == "missing-attribute" ? 999 : invalid == "skill-attribute" ? 1 : 9),
			new XAttribute("expression", invalid == "missing-expression" ? 999 : 90), new XAttribute("basis", "raw"));
		if (invalid == "malformed-xml") attributeXml.Attribute("attribute")!.Remove();
		xml.Add(attributeXml); var model = resource.Model(); model.Definition = xml.ToString();
		var reload = new CapacityResource(model, f.World.Object); f.Resources[1] = reload;
		f.Balances.Remove(resource); f.Balances[reload] = 100;
		Assert.IsTrue(double.IsNaN(reload.ResourceCap(f.Actor.Object)));
		Assert.IsFalse(MagicResourceCapacity.TryGetCap(reload, f.Actor.Object, out _, out var error)); Assert.IsFalse(string.IsNullOrEmpty(error));
		Assert.IsTrue(XElement.DeepEquals(attributeXml, XElement.Parse(reload.Model().Definition).Element("AttributeCapacity")));
		f.Acquire(); var writes = f.Store.Writes;
		var quote = f.Service.Quote(f.Intent(1, false));
		Assert.IsFalse(quote.Allowed); StringAssert.Contains(quote.Reason, "capacity");
		Assert.AreEqual(MagicCastingStatus.Refused, f.Service.Cast(f.Intent(1, false)).Status);
		Assert.AreEqual(100.0, f.Balances[reload]); Assert.AreEqual(writes, f.Store.Writes);
		Assert.IsTrue(reload.BuildingCommand(f.Actor.Object, new StringStack("capattribute none")));
		Assert.AreEqual(42.0, reload.ResourceCap(f.Actor.Object));
	}

	private static (MagicCastingFixture Fixture, CapacityResource Resource, IAttributeDefinition Attribute) AttributeFixture(string formula, bool skillParameter = false)
	{
		var f = new MagicCastingFixture(); var attribute = new Mock<IAttributeDefinition>();
		attribute.SetupGet(x => x.Id).Returns(9); attribute.SetupGet(x => x.Name).Returns("Explicit Fixture Attribute");
		attribute.SetupGet(x => x.TraitType).Returns(TraitType.Attribute); attribute.SetupGet(x => x.OwnerScope).Returns(TraitOwnerScope.Body);
		f.Traits.Add(attribute.Object); f.Skills[9] = 10;
		var expression = new MudSharp.Models.TraitExpression { Id = 90, Name = "Fixture Capacity", Expression = formula };
		if (skillParameter) expression.TraitExpressionParameters.Add(new MudSharp.Models.TraitExpressionParameters { Parameter = "skill", TraitDefinitionId = 1 });
		f.Expressions.Add(new TraitExpression(expression, f.World.Object));
		var legacy = new Mock<IFutureProg>(); legacy.SetupGet(x => x.Id).Returns(90); legacy.SetupGet(x => x.Name).Returns("Legacy Capacity");
		legacy.Setup(x => x.ExecuteDouble(It.IsAny<double>(), It.IsAny<object[]>())).Returns(42);
		var progs = f.World.Object.FutureProgs.Append(legacy.Object).ToArray();
		f.World.SetupGet(x => x.FutureProgs).Returns(MagicCastingFixture.Collection(() => progs));
		var resource = new CapacityResource(new MudSharp.Models.MagicResource { Id = 11, Name = "Explicit Capacity Reserve", ShortName = "cap",
			Definition = "<Definition><ResourceCapProg>90</ResourceCapProg></Definition>" }, f.World.Object);
		f.Balances.Remove(f.Resources[1]); f.Resources[1] = resource; f.Balances[resource] = 100;
		return (f, resource, attribute.Object);
	}

	private sealed class CapacityResource(MudSharp.Models.MagicResource model, IFuturemud world) : SimpleMagicResource(model, world)
	{
		public MudSharp.Models.MagicResource Model() => new() { Id = Id, Name = Name, ShortName = ShortName, Definition = SaveDefinition() };
	}
}
