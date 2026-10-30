#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using MudSharp.Magic;
using MudSharp.Magic.SpellEffects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Body.Needs;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.Health;
using MudSharp.Health.Infections;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ConfiguredInfectionTests
{
	[TestMethod]
	public void Exposure_CreatesOneBodyInfectionWithoutDrugDosesAndRejectsRecursiveProfiles()
	{
		var f = new Fixture();
		Assert.IsTrue(ConfiguredInfection.TryExpose(f.Body.Object, f.Carrier, "source"));
		Assert.IsFalse(ConfiguredInfection.TryExpose(f.Body.Object, f.Carrier, "source"));
		Assert.AreEqual(1, f.Infections.Count);
		Assert.AreEqual(30000, f.Infections.OfType<ConfiguredInfection>().Single().RemainingSeconds);
		Assert.IsFalse(DrugDosePolicy.CanAccept(f.Body.Object, f.Carrier));
		Assert.IsFalse(f.Body.Object.ActiveDrugDosages.Any());
		f.Body.Verify(x => x.RefreshDrugEffects(), Times.Once);
		f.Config.SymptomDrugId = f.Carrier.Id;
		f.Infections.Clear();
		Assert.IsFalse(ConfiguredInfection.TryExpose(f.Body.Object, f.Carrier, null));
	}

	[TestMethod]
	public void NaturalCompletion_AppliesExpiryOnceAndLeavesUnrelatedSymptoms()
	{
		var f = new Fixture();
		ConfiguredInfection.TryExpose(f.Body.Object, f.Carrier, null);
		var infection = f.Infections.OfType<ConfiguredInfection>().Single();
		var owned = f.AddSymptoms(infection.DefinitionDrugId);
		var unrelated = f.AddSymptoms(0);
		owned.ApplyOnset();
		infection.Advance(29999);
		Assert.IsFalse(infection.InfectionHealed());
		infection.Advance(1);
		Assert.IsTrue(infection.InfectionHealed());
		Assert.IsFalse(f.Effects.Contains(owned));
		Assert.IsTrue(f.Effects.Contains(unrelated));
		f.Actor.Verify(x => x.SetTraitValue(f.Wisdom.Object, 9), Times.Once);
		infection.Advance(10);
		f.Actor.Verify(x => x.SetTraitValue(f.Wisdom.Object, 9), Times.Once);
	}

	[TestMethod]
	public void Cure_SelectsDefinitionAndFamilyCancelsPendingSaveWithoutExpiryPenalty()
	{
		var f = new Fixture();
		ConfiguredInfection.TryExpose(f.Body.Object, f.Carrier, "source");
		var infection = f.Infections.Single();
		f.AddSymptoms(f.Carrier.Id).ApplyOnset();
		Assert.IsTrue(DrugPurgeRules.HasMatching(f.Actor.Object, f.Carrier, "source"));
		Assert.IsFalse(DrugPurgeRules.HasMatching(f.Actor.Object, f.Carrier, "other"));
		Assert.IsTrue(DrugPurgeRules.Purge(f.Actor.Object, f.Carrier, "source"));
		Assert.AreEqual(0, f.Infections.Count);
		Assert.AreEqual(0, f.Effects.Count);
		f.Saves.Verify(x => x.Abort((ISaveable)infection), Times.Once);
		f.Actor.Verify(x => x.SetTraitValue(It.IsAny<ITraitDefinition>(), It.IsAny<double>()), Times.Never);
	}

	[TestMethod]
	public void PersistenceAndSimulationClone_PreserveRemainingTimeAndDefinition()
	{
		var f = new Fixture();
		ConfiguredInfection.TryExpose(f.Body.Object, f.Carrier, "source");
		var infection = f.Infections.OfType<ConfiguredInfection>().Single();
		infection.Advance(120);
		var state = (string)typeof(ConfiguredInfection).GetMethod("SaveExtraInformation", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(infection, null)!;
		var loaded = (ConfiguredInfection)Infection.LoadInfection(new MudSharp.Models.Infection
		{
			Id = 15, InfectionType = (int)InfectionType.Configured, ExtraInformation = state, Intensity = 1,
			Virulence = f.Config.VirulenceDifficulty, VirulenceMultiplier = 1
		}, f.Body.Object);
		Assert.AreEqual(29880, loaded.RemainingSeconds);
		Assert.AreEqual("illness", loaded.Name);
		Assert.AreEqual("source", loaded.SourceFamily);
		Assert.AreSame(f.Symptoms, loaded.SymptomDrug);
		var clone = new ConfiguredInfection(loaded, f.Body.Object);
		Assert.AreEqual(loaded.RemainingSeconds, clone.RemainingSeconds);
		Assert.AreEqual(loaded.DefinitionDrugId, clone.DefinitionDrugId);
	}

	[TestMethod]
	public void SymptomTriggeredSpread_UsesConfiguredProbabilityAndDuplicateRefusal()
	{
		var f = new Fixture();
		var target = new Fixture();
		target.World.SetupGet(x => x.Drugs).Returns(f.World.Object.Drugs);
		var room = new Mock<IRoom>();
		room.SetupGet(x => x.Characters).Returns(new[] { f.Actor.Object, target.Actor.Object });
		f.Actor.SetupGet(x => x.Location).Returns(room.Object);
		target.Actor.SetupGet(x => x.Location).Returns(room.Object);
		f.Config.SpreadOnSymptomEpisode = true;
		f.Config.SpreadChance = 1;
		ConfiguredInfection.TryExpose(f.Body.Object, f.Carrier, null);
		var infection = f.Infections.OfType<ConfiguredInfection>().Single();
		infection.Advance(10);
		Assert.AreEqual(0, target.Infections.Count, "Independent clock does not spread a symptom-triggered infection.");
		f.Config.SpreadEpisodeChance = 0;
		infection.OnSymptomEpisode();
		Assert.AreEqual(0, target.Infections.Count);
		f.Config.SpreadEpisodeChance = 1;
		infection.OnSymptomEpisode();
		Assert.AreEqual(1, target.Infections.Count);
		infection.OnSymptomEpisode();
		Assert.AreEqual(1, target.Infections.Count);
	}

	[TestMethod]
	public void EnumOnlyDiseaseSpells_RejectConfiguredInfectionsWithoutDefinition()
	{
		var f = new MagicCastingFixture();
		SpellEffectFactory.SetupFactory();
		var xml = new XElement("Effect", new XAttribute("type", "disease"),
			new XElement("InfectionType", (int)InfectionType.Configured));
		var invalid = (DiseaseEffect)SpellEffectFactory.LoadEffect(xml, f.Spell);
		Assert.IsNotNull(invalid.DefinitionError);
		xml.Element("InfectionType")!.Value = ((int)InfectionType.Simple).ToString();
		var valid = (DiseaseEffect)SpellEffectFactory.LoadEffect(xml, f.Spell);
		Assert.IsFalse(valid.BuildingCommand(f.Actor.Object, new StringStack("type Configured")));
		Assert.AreEqual(InfectionType.Simple, valid.InfectionType);
	}

	private sealed class Fixture
	{
		public Mock<IBody> Body { get; } = new();
		public Mock<ICharacter> Actor { get; } = new();
		public Mock<IFuturemud> World { get; } = new();
		public Mock<ISaveManager> Saves { get; } = new();
		public Mock<ITraitDefinition> Wisdom { get; } = new();
		public List<IInfection> Infections { get; } = [];
		public List<DrugSymptomEffect> Effects { get; } = [];
		public DrugInfectionAdditionalInfo Config { get; } = new() { Name = "illness", SymptomDrugId = 2, RecoveryMode = InfectionRecoveryMode.FixedDuration };
		public IDrug Carrier { get; }
		public IDrug Symptoms { get; }
		public Fixture()
		{
			Body.SetupGet(x => x.Gameworld).Returns(World.Object);
			Body.SetupGet(x => x.Actor).Returns(Actor.Object);
			Actor.SetupGet(x => x.Body).Returns(Body.Object);
			Actor.SetupGet(x => x.State).Returns(CharacterState.Awake);
			World.SetupGet(x => x.SaveManager).Returns(Saves.Object);
			World.SetupGet(x => x.Traits).Returns(MagicCastingFixture.Collection(() => new[] { Wisdom.Object }));
			Wisdom.SetupGet(x => x.Id).Returns(3);
			Actor.Setup(x => x.HasTrait(Wisdom.Object)).Returns(true);
			Actor.Setup(x => x.TraitRawValue(Wisdom.Object)).Returns(10);
			Body.SetupGet(x => x.Bodyparts).Returns(Array.Empty<IBodypart>());
			Body.SetupGet(x => x.Organs).Returns(Array.Empty<IOrganProto>());
			Body.SetupGet(x => x.Wounds).Returns(Array.Empty<IWound>());
			Body.SetupGet(x => x.PartInfections).Returns(() => Infections);
			Body.Setup(x => x.AddInfection(It.IsAny<IInfection>())).Callback<IInfection>(Infections.Add);
			Body.Setup(x => x.RemoveInfection(It.IsAny<IInfection>())).Callback<IInfection>(x => Infections.Remove(x));
			Body.Setup(x => x.EffectsOfType<DrugSymptomEffect>(It.IsAny<Predicate<DrugSymptomEffect>>())).Returns(() => Effects);
			Body.Setup(x => x.RemoveEffect(It.IsAny<IEffect>(), It.IsAny<bool>())).Callback<IEffect, bool>((effect, _) => Effects.Remove((DrugSymptomEffect)effect));
			Body.SetupGet(x => x.ActiveDrugDosages).Returns(Array.Empty<DrugDosage>());
			Body.SetupGet(x => x.LatentDrugDosages).Returns(Array.Empty<DrugDosage>());
			Body.SetupGet(x => x.NeedsModel).Returns(Mock.Of<INeedsModel>());
			var carrier = new Mock<IDrug>();
			carrier.SetupGet(x => x.Id).Returns(1);
			carrier.SetupGet(x => x.DrugTypes).Returns(new[] { DrugType.Infection });
			carrier.SetupGet(x => x.DoseBehaviour).Returns(DrugDoseBehaviour.IgnoreAdditionalExposure);
			carrier.Setup(x => x.IntensityForType(DrugType.Infection)).Returns(1);
			carrier.Setup(x => x.AdditionalInfoFor<DrugInfectionAdditionalInfo>(DrugType.Infection)).Returns(Config);
			Carrier = carrier.Object;
			var profile = new Mock<IDrug>();
			profile.SetupGet(x => x.Id).Returns(2);
			profile.SetupGet(x => x.DrugTypes).Returns(new[] { DrugType.Symptoms });
			profile.Setup(x => x.AdditionalInfoFor<DrugSymptomsAdditionalInfo>(DrugType.Symptoms)).Returns(new DrugSymptomsAdditionalInfo
			{ ExpiryTraitChanges = [new DrugPermanentTraitChange { TraitId = 3, Chance = "1", Amount = -1 }] });
			Symptoms = profile.Object;
			World.SetupGet(x => x.Drugs).Returns(MagicCastingFixture.Collection(() => new[] { Carrier, Symptoms }));
		}
		public DrugSymptomEffect AddSymptoms(long source)
		{
			var effect = new DrugSymptomEffect(Body.Object, Symptoms.Id, 0, source) { Intensity = 1 };
			Effects.Add(effect);
			return effect;
		}
	}
}
