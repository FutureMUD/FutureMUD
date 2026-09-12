using System;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Magic;
using MudSharp.Magic.Capabilities;
using MudSharp.Magic.Vancian;
using MudSharp.RPG.Checks;

#nullable enable
namespace MudSharp_Unit_Tests;

[TestClass]
public class VancianCapabilityTests
{
	internal static MudSharp.Models.MagicCapability Model(VancianTestFixture f)
	{
		var rule = f.Rules[0]; var allowance = f.Allowances[0];
		return new() { Id = 20, Name = "Wizard", CapabilityModel = "vancian", MagicSchoolId = 1, PowerLevel = 1,
			Definition = new XElement("Definition",new XElement("ConcentrationTrait",1),new XElement("ConcentrationCapabilityExpression","1"),
				new XElement("ConcentrationDifficultyExpression","5"),new XElement("Regenerators"),
				new XElement("Vancian",new XAttribute("version",1),new XAttribute("basePower",(int)SpellPower.Standard),new XAttribute("step",1),new XAttribute("outcome",(int)Outcome.Pass),
					new XAttribute("loadouts",10),new XAttribute("recovery","PreparationAction"),new XAttribute("prepareSeconds",5),new XAttribute("sleepSeconds",10),new XAttribute("intervalSeconds",0),
					f.Policies.Select(x => new XElement("Policy",new XAttribute("name",x.Key),new XAttribute("id",x.Value))),
					new XElement("Repertoire",new XAttribute("key",rule.Key),new XAttribute("alias",rule.Alias),new XAttribute("name",rule.Name),new XAttribute("order",0),new XAttribute("source",rule.Source),
						new XAttribute("min",0),new XAttribute("max",6),new XAttribute("candidate",rule.CandidateProgId),new XAttribute("limit",rule.SelectionLimitProgId),new XAttribute("bookPolicy",rule.BookPolicy)),
					new XElement("Allowance",new XAttribute("key",allowance.Key),new XAttribute("alias",allowance.Alias),new XAttribute("name",allowance.Name),new XAttribute("order",0),new XAttribute("mode",allowance.Mode),
						new XAttribute("level",allowance.SlotLevel!.Value),new XAttribute("version",1),new XAttribute("min",0),new XAttribute("max",1),new XAttribute("count",allowance.SlotCountProgId),new XAttribute("eligibility",0),
						new XElement("Rule",rule.Key)))).ToString() };
	}
	[TestMethod]
	public void RegisteredCapabilityRoundTripsClonesFreshKeysAndKeepsLegacyLoader()
	{
		var f = new VancianTestFixture(); var model = Model(f);
		var capability = (VancianMagicCapability)MagicCapabilityFactory.LoadCapability(model,f.World.Object);
		Assert.IsTrue(MagicCapabilityFactory.BuilderLoaders.ContainsKey("vancian")); Assert.AreEqual(0,capability.ConfigurationErrors().Count);
		Assert.IsTrue(capability.BuildingCommand(f.Actor.Object,new StringStack("repertoire known name Renamed Choices")));
		Assert.IsTrue(capability.BuildingCommand(f.Actor.Object,new StringStack("repertoire known order 8")));
		model.Definition = capability.SaveToXml(); var restored = new VancianMagicCapability(model,f.World.Object);
		Assert.AreEqual(f.Rules[0].Key,restored.Repertoires[0].Key); Assert.AreEqual(8,restored.Repertoires[0].SortOrder);
		var clone = new VancianMagicCapability(capability.CloneModel("Other Wizard"),f.World.Object);
		Assert.AreNotEqual(capability.Repertoires[0].Key,clone.Repertoires[0].Key); Assert.AreNotEqual(capability.Allowances[0].Key,clone.Allowances[0].Key);
		Assert.AreEqual(clone.Repertoires[0].Key,clone.Allowances[0].RepertoireKeys.Single()); Assert.AreEqual(0,clone.ConfigurationErrors().Count);
		var legacy = XElement.Parse(model.Definition); legacy.Element("Vancian")!.Remove(); model.Definition = legacy.ToString(); model.CapabilityModel = "skilllevel";
		Assert.IsInstanceOfType(MagicCapabilityFactory.LoadCapability(model,f.World.Object),typeof(SkillLevelBasedMagicCapability));
	}
	[TestMethod]
	public void UnsupportedBookModesBrokenLinksAndBadSchemaDisableWithoutOverwriting()
	{
		var f = new VancianTestFixture(); f.Rules[0] = f.Rules[0] with { Source = VancianRepertoireSource.Spellbook };
		f.Allowances[0] = f.Allowances[0] with { Mode = VancianAllowanceMode.Spontaneous };
		var model = Model(f); var capability = new VancianMagicCapability(model,f.World.Object);
		Assert.IsTrue(capability.ConfigurationErrors().Any(x => x.Contains("requires Selected")));
		var xml = XElement.Parse(model.Definition); xml.Descendants("Rule").Single().Value = Guid.NewGuid().ToString(); model.Definition = xml.ToString();
		Assert.IsTrue(new VancianMagicCapability(model,f.World.Object).ConfigurationErrors().Any(x => x.Contains("missing repertoire")));
		xml.Element("Vancian")!.SetAttributeValue("version",77); model.Definition = xml.ToString(); capability = new(model,f.World.Object);
		Assert.IsTrue(capability.ConfigurationErrors().Any(x => x.Contains("version"))); Assert.ThrowsException<InvalidOperationException>(() => capability.SaveToXml());
		model.Definition = "<broken"; capability = new(model,f.World.Object); Assert.IsTrue(capability.ConfigurationErrors().Count > 0);
		Assert.AreEqual(0,capability.InherentPowers(f.Actor.Object).Count()); Assert.ThrowsException<InvalidOperationException>(() => capability.SaveToXml());
	}
	[DataTestMethod]
	[DataRow(double.NaN)] [DataRow(double.PositiveInfinity)] [DataRow(-1.0)] [DataRow(2147483648.0)]
	public void InvalidProgressionNeverGrantsCapacity(double value)
	{
		var f = new VancianTestFixture(); var prog = f.Prog("count",_ => value);
		Assert.ThrowsException<InvalidOperationException>(() => VancianPolicy.Number(prog.Object,f.Actor.Object));
		prog.SetupGet(x => x.ReturnType).Returns(ProgVariableTypes.Text); Assert.IsFalse(VancianPolicy.ValidSignature(prog.Object,"count"));
	}
	[TestMethod]
	public void RecursiveCandidatePolicyFailsClosedAndCanRecoverAfterEditing()
	{
		var f = new VancianTestFixture(); var recurse = true;
		var prog = f.Prog("candidates",_ => !recurse || f.Service.Candidates(f.Actor.Object,f.Capability.Object,f.Rules[0].Key).Count > 0);
		f.Rules[0] = f.Rules[0] with { CandidateProgId = prog.Object.Id };
		Assert.AreEqual(0,f.Service.Candidates(f.Actor.Object,f.Capability.Object,f.Rules[0].Key).Count);
		recurse = false; Assert.AreEqual(3,f.Service.Candidates(f.Actor.Object,f.Capability.Object,f.Rules[0].Key).Count);
	}
	[TestMethod]
	public void EveryPolicyContractRejectsWrongReturnAndParameterTypes()
	{
		var f = new VancianTestFixture();
		foreach (var (name, signature) in VancianPolicy.Signatures)
		{
			var prog = f.Prog(name, _ => null);
			Assert.IsTrue(VancianPolicy.ValidSignature(prog.Object, name), name);
			prog.SetupGet(x => x.ReturnType).Returns(ProgVariableTypes.Text);
			Assert.IsFalse(VancianPolicy.ValidSignature(prog.Object, name), name);
			prog.SetupGet(x => x.ReturnType).Returns(signature.Return);
			prog.SetupGet(x => x.Parameters).Returns([ProgVariableTypes.Text]);
			Assert.IsFalse(VancianPolicy.ValidSignature(prog.Object, name), name);
		}
	}
}
