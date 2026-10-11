#nullable enable
using System;
using System.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body.Traits;
using MudSharp.CharacterCreation;
using MudSharp.CharacterCreation.Roles;
using MudSharp.Framework;
using MudSharp.Magic;
using MudSharp.Magic.Capabilities;
using MudSharp.Magic.Casting;
using MudSharp.RPG.Merits.Interfaces;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonStockProgressionRuntimeTests
{
	[DataTestMethod]
	[DataRow(ArmageddonProgressionMode.Classic, "earth")]
	[DataRow(ArmageddonProgressionMode.Classic, "sorcerer")]
	[DataRow(ArmageddonProgressionMode.Simple, "earth")]
	[DataRow(ArmageddonProgressionMode.Simple, "sorcerer")]
	public void ApprovedGraph_ClassEnrolmentNaturalEdgesAndRestart_RetainGradeOneAndRawCaps(ArmageddonProgressionMode mode, string tradition)
	{
		var (f, stock, cap, role) = Fixture(mode, tradition);
		var definition = stock.Capabilities.Single(x => x.Key == "arm.capability." + tradition);
		Assert.IsNull(f.Store.Enrolment(100, cap.CastingPolicy!.Identity));
		var results = f.Service.EnrolFromSelectedClass(f.Actor.Object, role.Object);
		Assert.AreEqual(1, results.Count);
		Assert.IsTrue(results[0].Result.Allowed, results[0].Result.Message);
		var roots = stock.Bindings.Where(x => x.CapabilityKey == definition.Key && x.Starting && !x.Support).Count();
		Assert.AreEqual(roots, f.Store.Acquired.Count);
		foreach (var acquired in f.Store.Acquired.Values) Assert.AreEqual(1, acquired.ControlledGrade);
		if (mode == ArmageddonProgressionMode.Simple)
		{
			Assert.AreEqual(2, f.Skills.Count);
			Assert.IsTrue(f.Skills.Values.All(x => x == 10));
			Assert.AreEqual(2, cap.CastingPolicy.Supports.Count);
			var main = cap.CastingPolicy.DefaultTraitId;
			Assert.AreEqual(100.0, f.Service.RawSkillImprovementCap(f.Actor.Object, main));
			f.Skills[main] = 100;
			f.Service.NotifyProgress(f.Actor.Object, traitId: main);
			Assert.AreEqual(definition.SpellKeys.Count - (tradition == "sorcerer" ? 1 : 0), f.Store.Acquired.Count);
			if (tradition == "sorcerer")
			{
				var craft = cap.CastingPolicy.Supports.Single(x => x.TraitId != main).TraitId;
				f.Skills[craft] = 80;
				f.Service.NotifyProgress(f.Actor.Object, traitId: craft);
			}
		}
		else
		{
			for (var i = 0; i < definition.SpellKeys.Count + 1; i++)
			{
				foreach (var admission in cap.CastingPolicy.Admissions.Where(x => f.Service.Acquisition(f.Actor.Object, x.SpellId) is not null))
					f.Skills[admission.TraitId!.Value] = admission.RawSkillCap!.Value;
				foreach (var support in cap.CastingPolicy.Supports.Where(x => f.Store.SupportGrant(100, cap.CastingPolicy.Identity, x.Key) is not null))
					f.Skills[support.TraitId] = support.RawSkillCap!.Value;
				f.Service.Reconcile(f.Actor.Object);
			}
			Assert.AreEqual(definition.SpellKeys.Count, f.Store.Acquired.Count);
			if (tradition == "sorcerer") Assert.IsTrue(cap.CastingPolicy.Admissions.Any(x => x.RawSkillCap == 60));
		}
		Assert.AreEqual(definition.SpellKeys.Count, f.Store.Acquired.Count);
		Assert.IsTrue(f.Store.Acquired.Values.All(x => x.ControlledGrade == 1));
		var writes = f.Store.Writes;
		var skills = f.Skills.ToDictionary();
		f.Restart();
		Assert.IsTrue(f.Service.EnrolFromSelectedClass(f.Actor.Object, role.Object).Single().Result.Allowed);
		Assert.AreEqual(writes, f.Store.Writes);
		CollectionAssert.AreEquivalent(skills.ToArray(), f.Skills.ToArray());
		Assert.AreEqual(100.0, f.Balances[f.Resources[0]]);
		Assert.AreEqual(0, f.Rolls);
		Assert.AreEqual(0, f.SkillUses);
	}

	[DataTestMethod]
	[DataRow("unrelated-role")]
	[DataRow("no-permanent-merit")]
	[DataRow("disabled")]
	[DataRow("opt-out")]
	[DataRow("unsaved")]
	public void ClassEnrolment_RequiresSelectedClassOptInPermanentMeritAndDurableIdentity(string restriction)
	{
		var (f, _, cap, role) = Fixture(ArmageddonProgressionMode.Simple, "earth");
		switch (restriction)
		{
			case "unrelated-role": role.SetupProperty(x => x.RoleType, ChargenRoleType.Profession); break;
			case "no-permanent-merit": f.Actor.SetupGet(x => x.Merits).Returns(Array.Empty<MudSharp.RPG.Merits.IMerit>()); break;
			case "disabled": Assert.IsTrue(cap.BuildingCommand(f.Actor.Object, new StringStack("casting enable off"))); break;
			case "opt-out": Assert.IsTrue(cap.BuildingCommand(f.Actor.Object, new StringStack("casting classenrol off"))); break;
			case "unsaved": f.Actor.SetupGet(x => x.Id).Returns(0); break;
		}
		var results = f.Service.EnrolFromSelectedClass(f.Actor.Object, role.Object);
		Assert.IsTrue(results.Count == 0 || results.All(x => !x.Result.Allowed));
		Assert.AreEqual(0, f.Store.Enrolments.Count);
		Assert.AreEqual(0, f.Store.Acquired.Count);
		Assert.AreEqual(0, f.Skills.Count);
	}

	[TestMethod]
	public void SharedStockSpell_CommonSource_QuotesAndChargesOnlySelectedReserve()
	{
		var f = new MagicCastingFixture();
		var stock = new ArmageddonStockProgression(ArmageddonProgressionMode.Simple);
		var definitions = stock.Capabilities.Where(x => x.Key is "arm.capability.water" or "arm.capability.sorcerer").ToArray();
		var traits = stock.Bindings.Where(x => definitions.Any(y => y.Key == x.CapabilityKey)).Select(x => x.TraitKey).Distinct()
			.Select((key, i) => (key, id: 100L + i)).ToDictionary(x => x.key, x => x.id);
		foreach (var pair in traits)
		{
			var trait = new Mock<ITraitDefinition>();
			trait.SetupGet(x => x.Id).Returns(pair.Value);
			trait.SetupGet(x => x.TraitType).Returns(TraitType.Skill);
			trait.SetupGet(x => x.OwnerScope).Returns(TraitOwnerScope.Character);
			f.Traits.Add(trait.Object);
		}
		var spells = definitions.SelectMany(x => x.SpellKeys).Distinct().Select((key, i) => (key, id: 1000L + i)).ToDictionary(x => x.key, x => x.id);
		foreach (var pair in spells)
		{
			var spell = f.NewSpell(pair.Value, stock.SpellNames[pair.Key], "<Effect type='personaltagward'/>");
			Assert.IsTrue(spell.BuildingCommand(f.Actor.Object, new StringStack("grades fixture")));
		}
		var caps = definitions.Select((x, i) => f.NewCapability(100 + i, traits[x.MainTraitKey], 11 + i, true,
			stock.Casting(x, traits, 11 + i, spells, sourceResource: 10))).ToArray();
		f.ActiveCapabilities.Clear();
		f.ActiveCapabilities.AddRange(caps);
		var merit = new Mock<IMagicCapabilityMerit>();
		merit.SetupGet(x => x.Capabilities).Returns(caps);
		merit.Setup(x => x.Applies(f.Actor.Object)).Returns(true);
		f.Actor.SetupGet(x => x.Merits).Returns(new[] { merit.Object });
		f.Service.DefinitionsChanged();
		var shared = spells["arm.spell.mend_flesh"];
		foreach (var cap in caps)
		{
			Assert.AreEqual(0, cap.CastingConfigurationErrors().Count, string.Join("\n", cap.CastingConfigurationErrors()));
			Assert.IsTrue(f.Service.Enrol(f.Staff.Object, f.Actor.Object, cap.Id, "shared stock cost fixture").Allowed);
			Assert.IsTrue(f.Service.Grant(f.Staff.Object, f.Actor.Object, cap.Id, shared, "payment mapping fixture").Allowed);
			var quote = f.Service.Quote(new(f.Actor.Object, cap.Id, shared, 1, false, "self"));
			Assert.IsTrue(quote.Allowed, quote.Reason);
			Assert.AreEqual(cap.CastingPolicy!.ReserveResourceId, quote.Invocation!.Costs.Single().ResourceId);
			var before = f.Balances.ToDictionary();
			var result = f.Service.Cast(new(f.Actor.Object, cap.Id, shared, 1, false, "self"));
			Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
			foreach (var resource in f.Resources)
				Assert.AreEqual(before[resource] - (resource.Id == cap.CastingPolicy.ReserveResourceId ? quote.Invocation.Costs.Single().Amount : 0), f.Balances[resource]);
			f.Actor.Setup(x => x.CombinedEffectsOfType<MudSharp.Effects.Concrete.MagicSpellLockout>()).Returns(Array.Empty<MudSharp.Effects.Concrete.MagicSpellLockout>());
		}
	}

	[TestMethod]
	public void ClassEnrolment_XmlDefaultAndBuilderSetting_PreserveLegacyOptOut()
	{
		var f = new MagicCastingFixture();
		Assert.IsFalse(f.Earth.CastingPolicy!.EnrolOnClassSelection);
		Assert.IsTrue(f.Earth.BuildingCommand(f.Actor.Object, new StringStack("casting classenrol on")));
		var xml = System.Xml.Linq.XElement.Parse(f.Earth.SaveToXml());
		Assert.IsTrue(f.NewCapability(99, 1, 11, true, xml.Element("Casting")).CastingPolicy!.EnrolOnClassSelection);
	}

	private static (MagicCastingFixture Fixture, ArmageddonStockProgression Stock, SkillLevelBasedMagicCapability Capability, Mock<IChargenRole> Role)
		Fixture(ArmageddonProgressionMode mode, string tradition)
	{
		var f = new MagicCastingFixture();
		var stock = new ArmageddonStockProgression(mode);
		var definition = stock.Capabilities.Single(x => x.Key == "arm.capability." + tradition);
		var own = stock.Bindings.Where(x => x.CapabilityKey == definition.Key).ToArray();
		var traits = own.Select(x => x.TraitKey).Distinct().Select((key, i) => (key, id: 100L + i)).ToDictionary(x => x.key, x => x.id);
		foreach (var entry in traits)
		{
			var trait = new Mock<ITraitDefinition>();
			trait.SetupGet(x => x.Id).Returns(entry.Value);
			trait.SetupGet(x => x.Name).Returns(entry.Key);
			trait.SetupGet(x => x.TraitType).Returns(TraitType.Skill);
			trait.SetupGet(x => x.OwnerScope).Returns(TraitOwnerScope.Character);
			f.Traits.Add(trait.Object);
		}
		var spells = definition.SpellKeys.Select((key, i) => (key, id: 1000L + i)).ToDictionary(x => x.key, x => x.id);
		foreach (var entry in spells)
		{
			// Real native spell/controlled-profile loaders, with a bounded test effect, qualify progression only.
			var spell = f.NewSpell(entry.Value, stock.SpellNames[entry.Key], "<Effect type='personaltagward'/>");
			Assert.IsTrue(spell.BuildingCommand(f.Actor.Object, new StringStack("grades fixture")));
		}
		var casting = stock.Casting(definition, traits, 10, spells);
		var cap = f.NewCapability(100, (long)casting.Attribute("trait")!, 10, true, casting);
		Assert.AreEqual(0, cap.CastingConfigurationErrors().Count, string.Join("\n", cap.CastingConfigurationErrors()));
		f.ActiveCapabilities.Clear();
		f.ActiveCapabilities.Add(cap);
		f.Skills.Clear();
		var merit = new Mock<IMagicCapabilityMerit>();
		merit.Setup(x => x.Applies(f.Actor.Object)).Returns(true);
		merit.SetupGet(x => x.Capabilities).Returns(new[] { cap });
		f.Actor.SetupGet(x => x.Merits).Returns(new[] { merit.Object });
		var role = new Mock<IChargenRole>();
		role.SetupGet(x => x.Id).Returns(50);
		role.SetupProperty(x => x.RoleType, ChargenRoleType.Class);
		role.SetupGet(x => x.AdditionalMerits).Returns(new[] { merit.Object });
		f.Service.DefinitionsChanged();
		return (f, stock, cap, role);
	}
}
