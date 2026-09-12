using System;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.FutureProg.Functions.Magic;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Prototypes;
using MudSharp.Magic;
using MudSharp.Magic.Vancian;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Law;

#nullable enable
namespace MudSharp_Unit_Tests;

[TestClass]
public class VancianSnapshotTests
{
	internal static MagicSpell Spell(VancianTestFixture f, string effects = "", string casterEffects = "", string triggerType = "self", string triggerContent = "")
	{
		var spell = new MagicSpell(new MudSharp.Models.MagicSpell { Id = 2, Name = "Stored Test", Blurb = "A test", Description = "Test spell.",
			MagicSchoolId = 1, SpellLevel = 1, ScrollInscriptionAllowed = true, CastingTraitDefinitionId = 1, SpellKnownProgId = 0,
			CastingDifficulty = (int)Difficulty.Normal, MinimumSuccessThreshold = (int)Outcome.Pass,
			CastingEmote = "$0 release|releases a spell.", TargetEmote = "$0 affect|affects $1.", TargetNullEmote = "$0 cannot find a target.",
			FailCastingEmote = "$0 fail|fails.", TargetResistedEmote = "$1 resist|resists $0.",
			Definition = $"<Spell><Trigger type='{triggerType}'><MinimumPower>{(int)SpellPower.Insignificant}</MinimumPower><MaximumPower>{(int)SpellPower.RecklesslyPowerful}</MaximumPower>{triggerContent}</Trigger><Costs/><Effects>{effects}</Effects><CasterEffects>{casterEffects}</CasterEffects><Plan><Phase/></Plan></Spell>" }, f.World.Object);
		f.Spells.RemoveAll(x => x.Id == 2); f.Spells.Add(spell); return spell;
	}
	[TestMethod]
	public void NumericBindings_FreezePerLocationAndBonusContext_KeepTargetOutcomeLive()
	{
		var f = new VancianTestFixture(); var reader = new Mock<ICharacter>();
		f.Actor.Setup(x => x.TraitValue(f.Trait.Object,TraitBonusContext.SpellDuration)).Returns(40);
		f.Actor.Setup(x => x.TraitValue(f.Trait.Object,TraitBonusContext.None)).Returns(10);
		reader.Setup(x => x.TraitValue(It.IsAny<ITraitDefinition>(),It.IsAny<TraitBonusContext>())).Returns(999);
		var expression = new TraitExpression("variable + casterlevel + castinglevel + spelllevel + power + outcome + degrees + success",f.World.Object);
		var numeric = new SpellNumericalContext(1,3,7,SpellPower.Strong,Outcome.Pass,true);
		numeric.Capture("duration",expression,f.Actor.Object,f.Trait.Object,TraitBonusContext.SpellDuration);
		numeric.Capture("damage",expression,f.Actor.Object,f.Trait.Object);
		var restored = SpellNumericalContext.Load(numeric.Save());
		var duration = restored.Evaluate("duration",expression,reader.Object,f.Trait.Object,TraitBonusContext.SpellDuration,[("outcome",2)]);
		var damage = restored.Evaluate("damage",expression,reader.Object,f.Trait.Object,TraitBonusContext.None,[("outcome",2)]);
		Assert.AreEqual(30,duration-damage);
		Assert.AreEqual(3,restored.Evaluate("damage",expression,reader.Object,f.Trait.Object,TraitBonusContext.None,[("outcome",5)])-damage);
		reader.Verify(x => x.TraitValue(It.IsAny<ITraitDefinition>(),It.IsAny<TraitBonusContext>()),Times.Never);
	}
	[TestMethod]
	public void CaptureDoesNotEvaluateFormula_AndRejectsNonFiniteCreatorBindings()
	{
		var f = new VancianTestFixture(); var expression = new TraitExpression("1 / 0 + variable",f.World.Object);
		var numeric = new SpellNumericalContext(0,0,0,SpellPower.Standard,Outcome.Pass,true);
		numeric.Capture("x",expression,f.Actor.Object,f.Trait.Object);
		Assert.IsNotNull(numeric.Save().Element("Expression"));
		f.Actor.Setup(x => x.TraitValue(f.Trait.Object,TraitBonusContext.None)).Returns(double.NaN);
		Assert.ThrowsException<InvalidOperationException>(() => numeric.Capture("y",expression,f.Actor.Object,f.Trait.Object));
	}
	[TestMethod]
	public void NumericContextsAreIsolated_AndExtendedOptionsAreStored()
	{
		var f = new VancianTestFixture(); var expression = new TraitExpression("skill:1 + variable + {flag unknown=unused}",f.World.Object);
		f.Actor.Setup(x => x.TraitValue(f.Trait.Object,TraitBonusContext.None)).Returns(8);
		var first = new SpellNumericalContext(0,0,0,SpellPower.Standard,Outcome.Pass,true); first.Capture("x",expression,f.Actor.Object,f.Trait.Object);
		f.Actor.Setup(x => x.TraitValue(f.Trait.Object,TraitBonusContext.None)).Returns(17);
		var second = new SpellNumericalContext(0,0,0,SpellPower.Standard,Outcome.Pass,true); second.Capture("x",expression,f.Actor.Object,f.Trait.Object);
		Assert.AreEqual(16,first.Evaluate("x",expression,f.Actor.Object,f.Trait.Object,TraitBonusContext.None,[]));
		Assert.AreEqual(34,second.Evaluate("x",expression,f.Actor.Object,f.Trait.Object,TraitBonusContext.None,[]));
		Assert.IsTrue(first.Save().Descendants("Binding").Any(x => (string?)x.Attribute("name") == "flag"));
		Assert.ThrowsException<InvalidOperationException>(() => first.ValidateBinding("missing",expression));
	}
	[TestMethod]
	public void SnapshotRoundTripPreservesSourceConfigurationAndCreatorNumbersAfterEdits()
	{
		var f = new VancianTestFixture(); var spell = Spell(f);
		spell.EffectDurationExpression = new TraitExpression("variable + casterlevel",f.World.Object);
		f.Actor.Setup(x => x.TraitValue(f.Trait.Object,TraitBonusContext.SpellDuration)).Returns(24);
		var snapshot = StoredSpellSnapshot.Capture(spell,f.Actor.Object,f.Capability.Object,3,SpellPower.Strong,7,f.Clock.Now.UtcDateTime);
		var saved = snapshot.Save(); spell.EffectDurationExpression = new TraitExpression("999",f.World.Object); spell.SpellLevel = 5;
		f.Actor.Setup(x => x.TraitValue(It.IsAny<ITraitDefinition>(),It.IsAny<TraitBonusContext>())).Returns(900);
		var restored = StoredSpellSnapshot.Load(saved).CreateSpell(f.World.Object);
		Assert.AreEqual(1,restored.SpellLevel); Assert.AreEqual(31,restored.EffectDurationExpression.Evaluate(f.Actor.Object,f.Trait.Object,TraitBonusContext.SpellDuration));
		f.World.Verify(x => x.TryGetCharacter(It.IsAny<long>(),It.IsAny<bool>()),Times.Never);
		spell.ScrollInscriptionAllowed = false;
		Assert.ThrowsException<InvalidOperationException>(() => snapshot.CreateSpell(f.World.Object));
		Assert.AreEqual(31,snapshot.CreateSpell(f.World.Object,false).EffectDurationExpression.Evaluate(f.Actor.Object,f.Trait.Object,TraitBonusContext.SpellDuration));
	}
	[TestMethod]
	public void CorruptSnapshotBindingIsRejectedBeforeRelease()
	{
		var f = new VancianTestFixture(); var spell = Spell(f); spell.EffectDurationExpression = new TraitExpression("variable",f.World.Object);
		var xml = StoredSpellSnapshot.Capture(spell,f.Actor.Object,f.Capability.Object,1,SpellPower.Standard,3,f.Clock.Now.UtcDateTime).Save();
		xml.Element("Numbers")!.RemoveNodes();
		Assert.ThrowsException<InvalidOperationException>(() => StoredSpellSnapshot.Load(xml).CreateSpell(f.World.Object));
	}
	[TestMethod]
	public void CompatibilityManifestCoversEveryRegisteredTypeAndRequiredFamilies()
	{
		var inventory = ScrollSpellCompatibility.Inventory.ToDictionary(x => x.Type,StringComparer.OrdinalIgnoreCase);
		var missing = SpellEffectFactory.RegisteredLoadTypes.Concat(SpellEffectFactory.MagicEffectTypes).Distinct(StringComparer.OrdinalIgnoreCase).Except(inventory.Keys,StringComparer.OrdinalIgnoreCase).ToArray();
		Assert.AreEqual(0,missing.Length,"Missing compatibility rows: " + string.Join(", ",missing));
		foreach (var type in new[] { "damage","heal","selfdamage","boost","glow","invisibility","spellarmour","infravision","sleep","removesleep","teleport" })
			Assert.IsFalse(inventory[type].Status == "Unsupported",type);
		Assert.AreEqual("Unsupported",inventory["executeprog"].Status);
	}
	[TestMethod]
	public void DirectVancianInvocationUsesSyntheticOutcomeAndExactFiniteDebit()
	{
		var f = new VancianTestFixture(); var spell = Spell(f);
		f.World.SetupGet(x => x.LegalAuthorities).Returns(VancianTestFixture.Collection<ILegalAuthority>(() => []));
		f.Actor.SetupGet(x => x.Location).Returns(() => null!);
		f.Select(2); f.Plan(2,2); f.Refresh();
		var check = new Mock<ICheck>(); f.World.Setup(x => x.GetCheck(CheckType.CastSpellCheck)).Returns(check.Object);
		var result = f.Service.Cast(f.Actor.Object,f.Capability.Object,f.Rules[0].Key,f.Allowances[0].Key,spell,2,new StringStack(""));
		Assert.IsTrue(result.Success,result.Message); Assert.AreEqual(VancianSlotStatus.Prepared,f.State.Slots[0].Status); Assert.AreEqual(VancianSlotStatus.Spent,f.State.Slots[1].Status);
		check.Verify(x => x.CheckAgainstAllDifficulties(It.IsAny<IPerceivableHaveTraits>(),It.IsAny<Difficulty>(),It.IsAny<ITraitDefinition>(),It.IsAny<IPerceivable>(),It.IsAny<double>(),It.IsAny<TraitUseType>(),It.IsAny<(string,object)[]>()),Times.Never);
	}
	[TestMethod]
	public void LegacyDirectRouteCannotBypassVancian_AndMixedCapabilityRemainsValid()
	{
		var f = new VancianTestFixture(); var spell = Spell(f); var known = f.Prog("candidates",_ => true);
		known.Setup(x => x.Execute<bool?>(It.IsAny<object[]>())).Returns(true);
		spell.SpellKnownProg = known.Object;
		Assert.IsFalse(spell.HasLegacyRoute(f.Actor.Object));
		var legacy = new Mock<IMagicCapability>(); legacy.SetupGet(x => x.School).Returns(f.School.Object);
		f.Actor.SetupGet(x => x.Capabilities).Returns([f.Capability.Object,legacy.Object]);
		Assert.IsTrue(spell.HasLegacyRoute(f.Actor.Object));
	}
	[TestMethod]
	public void ActorOwnedMovementUsesReaderAndFrozenConfiguration()
	{
		var f = new VancianTestFixture(); var spell = Spell(f,"<Effect type='teleport'><TeleportParty>true</TeleportParty><PreserveLayer>true</PreserveLayer><TargetLayer>0</TargetLayer></Effect>",triggerType:"room");
		var snapshot = StoredSpellSnapshot.Capture(spell,f.Actor.Object,f.Capability.Object,1,SpellPower.Standard,3,f.Clock.Now.UtcDateTime);
		var reader = new Mock<ICharacter>(); var destination = new Mock<ICell>(); var invocation = snapshot.CreateSpell(f.World.Object);
		invocation.SpellEffects.Single().GetOrApplyEffect(reader.Object,destination.Object,OpposedOutcomeDegree.Marginal,SpellPower.Standard,null!,[]);
		reader.Verify(x => x.Teleport(destination.Object,reader.Object.RoomLayer,true,true),Times.Once);
		f.Actor.Verify(x => x.Teleport(It.IsAny<ICell>(),It.IsAny<RoomLayer>(),It.IsAny<bool>(),It.IsAny<bool>()),Times.Never);
	}
	[TestMethod]
	public void AllFutureProgContractsAreRegisteredWithTypedReturns()
	{
		FutureProgTestBootstrap.EnsureInitialised();
		var registered = MudSharp.FutureProg.FutureProg.GetFunctionCompilerInformations();
		foreach (var contract in VancianFunction.Contracts)
			Assert.IsTrue(registered.Any(x => x.FunctionName.EqualTo(contract.Name) && x.Parameters.SequenceEqual(contract.Parameters)),contract.Name);
	}
}
