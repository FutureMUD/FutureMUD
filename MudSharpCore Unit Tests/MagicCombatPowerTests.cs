#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Combat.Moves;
using MudSharp.Construction;
using MudSharp.GameItems;
using MudSharp.Vehicles;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Health;
using MudSharp.Magic;
using MudSharp.Magic.Powers;
using MudSharp.Magic.SpellTriggers;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests;

[TestClass]
public class MagicCombatPowerTests
{
	[TestMethod]
	public void LegacyTape_LoadsAsModernStorageWithoutLosingCapacity()
	{
		var manager = new GameItemComponentManager();
		foreach (var (name, milliseconds, format) in new[] { ("Tape_Cassette30", 1800000, "compact-cassette"), ("Tape_Microcassette10", 600000, "microcassette") })
		{
			var model = new MudSharp.Models.GameItemComponentProto { Id = 1252, Name = name, Type = "Tape", Description = "Legacy tape",
				EditableItem = new MudSharp.Models.EditableItem { BuilderDate = DateTime.UtcNow }, Definition = $"<Definition><CapacityMs>{milliseconds}</CapacityMs></Definition>" };
			var proto = (MediaStorageMediumGameItemComponentProto)manager.GetProto(model, new Fixture().World.Object);
			Assert.AreEqual(TimeSpan.FromMilliseconds(milliseconds), proto.Capacity);
			Assert.AreEqual(format, proto.FormatKey);
			Assert.AreEqual("Media Storage Medium", proto.TypeDescription);
			model.Definition = (string)typeof(MediaStorageMediumGameItemComponentProto).GetMethod("SaveToXml", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(proto, null)!;
			var reloaded = (MediaStorageMediumGameItemComponentProto)manager.GetProto(model, new Fixture().World.Object);
			Assert.AreEqual(proto.Capacity, reloaded.Capacity);
			Assert.AreEqual(proto.FormatKey, reloaded.FormatKey);
		}
	}

	[TestMethod]
	public void AttackTriggers_AreTypedAndNeverCastable()
	{
		foreach (var items in new[] { false, true })
		{
			var trigger = new AttackHitTrigger(items);
			var loaded = SpellTriggerFactory.LoadTrigger(trigger.SaveToXml(), new Mock<IMagicSpell>().Object);
			Assert.AreEqual(items ? "item" : "character", loaded.TargetTypes);
			Assert.IsFalse(loaded is ICastMagicTrigger);
			Assert.AreEqual(loaded.SaveToXml().ToString(), loaded.Clone().SaveToXml().ToString());
		}
	}

	[TestMethod]
	public void AttackPayload_UsesMatchingContextAndPersistsReference()
	{
		var f = new Fixture(); MagicAttackPower.RegisterLoader();
		var model = Fixture.Model(PsionicStockContent.CombatPowers.First());
		var spell = new Mock<IMagicSpell>(); spell.SetupGet(x => x.Id).Returns(9);
		spell.SetupGet(x => x.ReadyForGame).Returns(true);
		spell.SetupGet(x => x.School).Returns(f.World.Object.MagicSchools.Get(1));
		spell.SetupGet(x => x.Trigger).Returns(new AttackHitTrigger(false));
		spell.SetupGet(x => x.CasterSpellEffects).Returns([]);
		f.World.SetupGet(x => x.MagicSpells).Returns(Collection(spell.Object));
		var xml = XElement.Parse(model.Definition); xml.Add(new XElement("AttackSpell", 9), new XElement("AttackSpellPower", "Strong")); model.Definition = xml.ToString();
		var power = (MagicAttackPower)MagicPowerFactory.LoadPower(model, f.World.Object);
		Assert.IsTrue(power.HasValidAttackSpell);
		power.ApplyAttackSpell(f.Actor.Object, f.Attacker.Object, CheckOutcome.SimpleOutcome(CheckType.GenericSkillCheck, Outcome.Fail));
		spell.Verify(x => x.ResolveAttackSpell(It.IsAny<ICharacter>(), It.IsAny<IPerceivable>(), It.IsAny<SpellPower>(), It.IsAny<CheckOutcome>()), Times.Never);
		power.ApplyAttackSpell(f.Actor.Object, f.Attacker.Object, CheckOutcome.SimpleOutcome(CheckType.GenericSkillCheck, Outcome.Pass));
		spell.Verify(x => x.ResolveAttackSpell(f.Actor.Object, f.Attacker.Object, SpellPower.Strong, It.IsAny<CheckOutcome>()), Times.Once);
		var saved = (XElement)typeof(MagicAttackPower).GetMethod("SaveDefinition", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(power, null)!;
		Assert.AreEqual("9", saved.Element("AttackSpell")!.Value);
		spell.SetupGet(x => x.Trigger).Returns(new AttackHitTrigger(true)); Assert.IsFalse(power.HasValidAttackSpell);
	}

	[TestMethod]
	public void MagicalSmash_InvalidatedQueueDoesNotSpend()
	{
		var f = new Fixture(); var power = new Mock<IMagicSmashPower>(); power.SetupGet(x => x.WeaponAttack).Returns(f.Attack.Object);
		var item = new Mock<IGameItem>();
		var move = new MagicPowerSmashItemMove(f.Actor.Object, item.Object, power.Object);
		Assert.AreSame(CombatMoveResult.Irrelevant, move.ResolveMove(null!));
		Assert.IsFalse(move.UsesStaminaWithResult(CombatMoveResult.Irrelevant));
		power.Verify(x => x.UseAttackPower(It.IsAny<IMagicPowerAttackMove>()), Times.Never);
	}

	[TestMethod]
	public void PreparedAttackSpell_AppliesBothEffectListsWithoutCastingOrKnowledge()
	{
		var f = new Fixture();
		var spell = new MagicSpell(new MudSharp.Models.MagicSpell { Id = 20, Name = "Hidden Payload", MagicSchoolId = 1,
			CastingTraitDefinitionId = 1, SpellKnownProgId = 0,
			Definition = "<Definition><Trigger type='attackcharacter'/><Costs/><Effects/><CasterEffects/><Plan/></Definition>" }, f.World.Object);
		var targetEffect = new Mock<IMagicSpellEffectTemplate>();
		var casterEffect = new Mock<IMagicSpellEffectTemplate>();
		foreach (var effect in new[] { targetEffect, casterEffect })
		{
			effect.SetupGet(x => x.IsInstantaneous).Returns(true);
			effect.Setup(x => x.IsCompatibleWithTrigger(It.IsAny<IMagicTrigger>())).Returns(true);
		}
		((List<IMagicSpellEffectTemplate>)spell.SpellEffects).Add(targetEffect.Object);
		((List<IMagicSpellEffectTemplate>)spell.CasterSpellEffects).Add(casterEffect.Object);
		Assert.IsTrue(spell.ReadyForGame); Assert.IsFalse(spell.CharacterKnowsSpell(f.Actor.Object));
		Assert.IsFalse(spell.CharacterCanCast(f.Actor.Object, f.Attacker.Object));
		spell.ResolveAttackSpell(f.Actor.Object, f.Attacker.Object, SpellPower.Standard, CheckOutcome.SimpleOutcome(CheckType.GenericSkillCheck, Outcome.Pass));
		targetEffect.Verify(x => x.GetOrApplyEffect(f.Actor.Object, f.Attacker.Object, It.IsAny<OpposedOutcomeDegree>(), SpellPower.Standard,
			It.IsAny<MudSharp.Effects.Interfaces.IMagicSpellEffectParent>(), It.IsAny<SpellAdditionalParameter[]>()), Times.Once);
		casterEffect.Verify(x => x.GetOrApplyEffect(f.Actor.Object, f.Actor.Object, It.IsAny<OpposedOutcomeDegree>(), SpellPower.Standard,
			It.IsAny<MudSharp.Effects.Interfaces.IMagicSpellEffectParent>(), It.IsAny<SpellAdditionalParameter[]>()), Times.Once);
		f.Actor.Verify(x => x.UseResource(It.IsAny<IMagicResource>(), It.IsAny<double>()), Times.Never);
	}

	[TestMethod]
	public void MagicalSmash_CommitsPayloadAndCostsOnce_WithoutCharacterDefense()
	{
		var f = new Fixture(); var item = new Mock<IGameItem>();
		var power = new Mock<IMagicSmashPower>(); power.SetupGet(x => x.WeaponAttack).Returns(f.Attack.Object);
		power.Setup(x => x.CanInvokePower(f.Actor.Object, item.Object)).Returns(true);
		var move = new MagicPowerSmashItemMove(f.Actor.Object, item.Object, power.Object);
		Assert.AreEqual(0, move.CharacterTargets.Count());
		var result = move.ResolveMove(null!); Assert.IsTrue(result.MoveWasSuccessful);
		Assert.AreSame(CombatMoveResult.Irrelevant, move.ResolveMove(null!));
		power.Verify(x => x.UseAttackPower(move), Times.Once);
		power.Verify(x => x.ApplyAttackSpell(f.Actor.Object, item.Object, It.IsAny<CheckOutcome>()), Times.Once);
	}

	[TestMethod]
	public void MagicalSmashPower_RejectsDestroyedAndOtherCarriedItems_AndRoundTrips()
	{
		var f = new Fixture(); MagicSmashPower.RegisterLoader();
		var model = Fixture.Model(PsionicStockContent.CombatPowers.First()); model.PowerModel = "magicsmash";
		f.Attack.SetupGet(x => x.UsabilityProg).Returns((IFutureProg)null!);
		var power = (MagicSmashPower)MagicPowerFactory.LoadPower(model, f.World.Object);
		var item = new Mock<IGameItem>();
		f.Actor.SetupGet(x => x.Powers).Returns([power]);
		f.Actor.SetupGet(x => x.Movement).Returns((MudSharp.Movement.IMovement)null!);
		f.Actor.Setup(x => x.ColocatedWith(item.Object)).Returns(true);
		Assert.IsTrue(power.CanInvokePower(f.Actor.Object, item.Object));
		Assert.IsFalse(power.CanInvokePower(f.Actor.Object, f.Attacker.Object));
		item.SetupGet(x => x.Destroyed).Returns(true); Assert.IsFalse(power.CanInvokePower(f.Actor.Object, item.Object));
		item.SetupGet(x => x.Destroyed).Returns(false); item.SetupGet(x => x.InInventoryOf).Returns(f.Attacker.Object.Body);
		Assert.IsFalse(power.CanInvokePower(f.Actor.Object, item.Object));
		var saved = (XElement)typeof(MagicAttackPower).GetMethod("SaveDefinition", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(power, null)!;
		model.Definition = saved.ToString();
		Assert.AreEqual(BuiltInCombatMoveType.MagicPowerSmashItem, ((MagicSmashPower)MagicPowerFactory.LoadPower(model, f.World.Object)).MoveType);
	}
	private static IUneditableAll<T> Collection<T>(params T[] values) where T : class, IFrameworkItem
	{
		var mock = new Mock<IUneditableAll<T>>();
		mock.Setup(x => x.Get(It.IsAny<long>())).Returns<long>(id => values.FirstOrDefault(x => x.Id == id));
		mock.Setup(x => x.GetEnumerator()).Returns(() => values.AsEnumerable().GetEnumerator());
		return mock.Object;
	}

	private sealed class Fixture
	{
		public Mock<IFuturemud> World { get; } = new() { DefaultValue = DefaultValue.Mock };
		public Mock<ICharacter> Actor { get; } = new() { DefaultValue = DefaultValue.Mock };
		public Mock<ICharacter> Attacker { get; } = new() { DefaultValue = DefaultValue.Mock };
		public Mock<ICheck> Check { get; } = new();
		public Mock<IWeaponAttack> Attack { get; } = new() { DefaultValue = DefaultValue.Mock };
		public Mock<IMagicResource> Resource { get; } = new();
		public List<IEffect> Effects { get; } = [];
		public Dictionary<IMagicResource, double> Resources { get; } = [];
		public Fixture()
		{
			var school = new Mock<IMagicSchool>(); school.SetupGet(x => x.Id).Returns(1); school.SetupGet(x => x.Name).Returns("Psionics");
			var trait = new Mock<ITraitDefinition>(); trait.SetupGet(x => x.Id).Returns(1);
			Resource.SetupGet(x => x.Id).Returns(1); Resource.SetupGet(x => x.Name).Returns("Focus"); Resources[Resource.Object] = 100;
			Attack.SetupGet(x => x.Id).Returns(1); Attack.SetupGet(x => x.MoveType).Returns(BuiltInCombatMoveType.MagicPowerAttack);
			Attack.SetupGet(x => x.Profile.BaseAttackerDifficulty).Returns(Difficulty.Normal);
			Attack.SetupGet(x => x.Profile.DamageType).Returns(DamageType.Crushing);
			World.SetupGet(x => x.MagicSchools).Returns(Collection(school.Object));
			World.SetupGet(x => x.Traits).Returns(Collection(trait.Object));
			World.SetupGet(x => x.MagicResources).Returns(Collection(Resource.Object));
			var prog = new Mock<IFutureProg>(); prog.SetupGet(x => x.Id).Returns(0);
			prog.Setup(x => x.ExecuteBool(It.IsAny<object[]>())).Returns(true);
			prog.Setup(x => x.Execute<bool?>(It.IsAny<object[]>())).Returns(true);
			World.SetupGet(x => x.FutureProgs).Returns(Collection(prog.Object));
			World.SetupGet(x => x.WeaponAttacks).Returns(Collection(Attack.Object));
			World.Setup(x => x.GetCheck(It.IsAny<CheckType>())).Returns(Check.Object);
			Actor.SetupGet(x => x.Gameworld).Returns(World.Object); Attacker.SetupGet(x => x.Gameworld).Returns(World.Object);
			Actor.SetupGet(x => x.State).Returns(CharacterState.Awake);
			Actor.SetupGet(x => x.Race.CombatSettings).Returns(new RacialCombatSettings { CanDefend = true });
			Actor.Setup(x => x.IsBlocked("general")).Returns((false, ""));
			Actor.Setup(x => x.CanSpendStamina(It.IsAny<double>())).Returns(true);
			Actor.Setup(x => x.CanSee(It.IsAny<IPerceivable>(), It.IsAny<PerceiveIgnoreFlags>())).Returns(true);
			Attacker.Setup(x => x.CanSee(It.IsAny<IPerceivable>(), It.IsAny<PerceiveIgnoreFlags>())).Returns(true);
			Actor.SetupGet(x => x.MagicResourceAmounts).Returns(Resources);
			Actor.Setup(x => x.UseResource(Resource.Object, It.IsAny<double>())).Returns<IMagicResource, double>((_, amount) => { Resources[Resource.Object] -= amount; return true; });
			Actor.SetupGet(x => x.Effects).Returns(Effects);
			Actor.Setup(x => x.EffectsOfType<MagicDefense>(It.IsAny<Predicate<MagicDefense>>())).Returns((Predicate<MagicDefense>? predicate) => Effects.OfType<MagicDefense>().Where(x => predicate?.Invoke(x) ?? true));
			SetOutcome(Outcome.Pass);
		}
		public void SetOutcome(Outcome result)
		{
			Check.Setup(x => x.Check(It.IsAny<IPerceivableHaveTraits>(), It.IsAny<Difficulty>(), It.IsAny<ITraitDefinition>(),
				It.IsAny<IPerceivable>(), It.IsAny<double>(), It.IsAny<TraitUseType>(), It.IsAny<(string, object)[]>()))
				.Returns(CheckOutcome.SimpleOutcome(CheckType.GenericSkillCheck, result));
		}
		public MagicDefensePower Defense(string verb)
		{
			var stock = PsionicStockContent.CombatPowers.Single(x => x.Verb == verb);
			var power = new MagicDefensePower(Model(stock), World.Object);
			Actor.SetupGet(x => x.Powers).Returns([power]);
			World.SetupGet(x => x.MagicPowers).Returns(Collection<IMagicPower>(power));
			return power;
		}
		public static MudSharp.Models.MagicPower Model(PsionicCombatPower stock) => new() { Id = 10, Name = stock.Name,
			MagicSchoolId = 1, PowerModel = stock.Defense.HasValue ? "magicdefense" : "magicattack",
			Definition = PsionicStockContent.CombatDefinition(stock, 1, 1, 0, 0, 1).ToString() };
		public MagicPowerAttackMove Incoming() => new(Attacker.Object, Actor.Object, Mock.Of<IMagicAttackPower>(x =>
			x.WeaponAttack == Attack.Object && x.DealsDamage == true && x.ValidDefenseTypes == new[] { DefenseType.Magic }));
	}

	[TestMethod]
	public void StockPowers_AllLoadAndRoundTripTheirCombatConfiguration()
	{
		var f = new Fixture();
		MagicAttackPower.RegisterLoader(); MagicDefensePower.RegisterLoader();
		foreach (var stock in PsionicStockContent.CombatPowers)
		{
			var model = Fixture.Model(stock);
			var power = MagicPowerFactory.LoadPower(model, f.World.Object);
			var saved = (XElement)power.GetType().GetMethod("SaveDefinition", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(power, null)!;
			model.Definition = saved.ToString();
			var reloaded = MagicPowerFactory.LoadPower(model, f.World.Object);
			Assert.IsTrue(reloaded.IsPsionic);
			CollectionAssert.Contains(reloaded.Verbs.ToArray(), stock.Verb);
			foreach (var (field, echo) in PsionicPowerEmotes.Combat[stock.Verb].Where(x => x.Key != "RiderSuccess")) Assert.AreEqual(echo, saved.Element(field)?.Value);
			if (reloaded is IMagicAttackPower attack)
			{
				Assert.AreEqual(stock.Range, attack.AttackRange);
				Assert.AreEqual(stock.Rider.HasValue ? 1 : 0, attack.AttackEffects.Count);
				Assert.AreEqual(!stock.Rider.HasValue, attack.DealsDamage);
			}
		}
	}

	[TestMethod]
	public void LegacyAttack_DefaultsToMeleeWithoutRiders()
	{
		var f = new Fixture(); MagicAttackPower.RegisterLoader();
		var model = Fixture.Model(PsionicStockContent.CombatPowers.First());
		var xml = XElement.Parse(model.Definition);
		foreach (var field in new[] { "AttackRange", "RangeInRooms", "DealsDamage", "AttackEffects", "AttackEmote" }) xml.Element(field)?.Remove();
		model.Definition = xml.ToString();
		var power = (IMagicAttackPower)MagicPowerFactory.LoadPower(model, f.World.Object);
		Assert.AreEqual(MagicAttackRange.Melee, power.AttackRange); Assert.IsTrue(power.DealsDamage); Assert.AreEqual(0, power.AttackEffects.Count);
	}

	[TestMethod]
	public void QueuedAttack_InvalidatedBeforeResolution_DoesNotSpendOrRoll()
	{
		var f = new Fixture();
		var power = new Mock<IMagicAttackPower>(); power.SetupGet(x => x.WeaponAttack).Returns(f.Attack.Object);
		var move = new MagicPowerAttackMove(f.Attacker.Object, f.Actor.Object, power.Object);
		var result = move.ResolveMove(null!);
		Assert.AreSame(CombatMoveResult.Irrelevant, result); Assert.IsFalse(move.UsesStaminaWithResult(result));
		power.Verify(x => x.UseAttackPower(It.IsAny<IMagicPowerAttackMove>()), Times.Never);
		f.World.Verify(x => x.GetCheck(It.IsAny<CheckType>()), Times.Never);
	}

	[TestMethod]
	public void RangedPower_RechecksVisibilityLocationAndResources()
	{
		var f = new Fixture();
		MagicAttackPower.RegisterLoader();
		var power = (MagicAttackPower)MagicPowerFactory.LoadPower(Fixture.Model(PsionicStockContent.CombatPowers.Single(x => x.Verb == "forcelance")), f.World.Object);
		var cell = new Mock<ICell>();
		f.Actor.SetupGet(x => x.Powers).Returns([power]);
		f.Actor.SetupGet(x => x.Movement).Returns((MudSharp.Movement.IMovement)null!);
		f.Actor.SetupGet(x => x.Location).Returns(cell.Object);
		f.Attacker.SetupGet(x => x.Location).Returns(cell.Object);
		Assert.IsTrue(power.CanInvokePower(f.Actor.Object, f.Attacker.Object));
		f.Actor.Setup(x => x.CanSee(It.IsAny<IPerceivable>(), It.IsAny<PerceiveIgnoreFlags>())).Returns(false);
		Assert.IsFalse(power.CanInvokePower(f.Actor.Object, f.Attacker.Object));
		f.Actor.Setup(x => x.CanSee(It.IsAny<IPerceivable>(), It.IsAny<PerceiveIgnoreFlags>())).Returns(true);
		f.Attacker.SetupGet(x => x.Location).Returns((ICell)null!);
		Assert.IsFalse(power.CanInvokePower(f.Actor.Object, f.Attacker.Object));
		f.Attacker.SetupGet(x => x.Location).Returns(cell.Object);
		f.Resources[f.Resource.Object] = 0;
		Assert.IsFalse(power.CanInvokePower(f.Actor.Object, f.Attacker.Object));
		Assert.AreEqual(0, f.Resources[f.Resource.Object]);
	}

	[TestMethod]
	public void Builder_RejectsContradictoryMovementAndInvalidProtection()
	{
		var f = new Fixture();
		MagicAttackPower.RegisterLoader();
		var attack = (MagicAttackPower)MagicPowerFactory.LoadPower(Fixture.Model(PsionicStockContent.CombatPowers.Single(x => x.Verb == "repulse")), f.World.Object);
		Assert.IsFalse(attack.BuildingCommand(f.Actor.Object, new StringStack("rider Pull Normal 1 2")));
		Assert.AreEqual(MagicAttackEffectType.Pushback, attack.AttackEffects.Single().Type);
		var defense = f.Defense("kineticbarrier");
		Assert.IsFalse(defense.BuildingCommand(f.Actor.Object, new StringStack("capacity NaN")));
		Assert.AreEqual(30, defense.MaximumCapacity);
		Assert.IsFalse(defense.BuildingCommand(f.Actor.Object, new StringStack("echo begin $1 raise|raises a wall.")));
		Assert.IsTrue(attack.BuildingCommand(f.Actor.Object, new StringStack("verb repulsion")));
		Assert.AreEqual("repulsion", attack.Verbs.Single());
		var saved = (XElement)typeof(MagicAttackPower).GetMethod("SaveDefinition", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(attack, null)!;
		Assert.AreEqual("repulsion", saved.Element("InvocationCosts")!.Element("Verbs")!.Elements("Verb").Single().Attribute("verb")!.Value);
	}

	[TestMethod]
	public void ChargedDefense_SuccessSpendsOneChargeAndReactionCost_FailureKeepsCharges()
	{
		var f = new Fixture(); var power = f.Defense("phantomdoubles");
		var effect = new MagicDefense(f.Actor.Object, power); f.Effects.Add(effect);
		f.SetOutcome(Outcome.MajorPass);
		var move = new MagicDefenseMove(f.Actor.Object, effect);
		Assert.IsTrue(move.TryDefend(f.Incoming(), CheckOutcome.SimpleOutcome(CheckType.GenericSkillCheck, Outcome.MinorPass), out _));
		Assert.AreEqual(2, effect.Charges); Assert.AreEqual(99, f.Resources[f.Resource.Object]);
		f.SetOutcome(Outcome.MajorFail);
		move = new MagicDefenseMove(f.Actor.Object, effect);
		Assert.IsFalse(move.TryDefend(f.Incoming(), CheckOutcome.SimpleOutcome(CheckType.GenericSkillCheck, Outcome.MajorPass), out _));
		Assert.AreEqual(2, effect.Charges); Assert.AreEqual(98, f.Resources[f.Resource.Object]);
	}

	[TestMethod]
	public void Defense_MissOrUnavailable_DoesNotConsumeResourcesOrProtection()
	{
		var f = new Fixture(); var power = f.Defense("phantomdoubles"); var effect = new MagicDefense(f.Actor.Object, power); f.Effects.Add(effect);
		var move = new MagicDefenseMove(f.Actor.Object, effect);
		Assert.IsTrue(move.TryDefend(f.Incoming(), CheckOutcome.SimpleOutcome(CheckType.GenericSkillCheck, Outcome.Fail), out _));
		Assert.IsFalse(move.Committed); Assert.AreEqual(3, effect.Charges); Assert.AreEqual(100, f.Resources[f.Resource.Object]);
		f.Resources[f.Resource.Object] = 0;
		Assert.IsFalse(power.CanDefend(f.Actor.Object, f.Incoming()));
	}

	[TestMethod]
	public void Barrier_AbsorbsLargestChannelProportionally_AndPersistsDepletion()
	{
		var f = new Fixture(); var power = f.Defense("kineticbarrier"); var effect = new MagicDefense(f.Actor.Object, power); f.Effects.Add(effect);
		var move = new MagicDefenseMove(f.Actor.Object, effect);
		move.TryDefend(f.Incoming(), CheckOutcome.SimpleOutcome(CheckType.GenericSkillCheck, Outcome.Pass), out _);
		Assert.IsNull(move.Absorb(new Damage { ActorOrigin = f.Attacker.Object, DamageType = DamageType.Crushing, DamageAmount = 5, StunAmount = 10, PainAmount = 2 }));
		Assert.AreEqual(20, effect.Capacity);
		var xml = effect.SaveToXml(new Dictionary<IEffect, TimeSpan> { [effect] = TimeSpan.FromSeconds(40) });
		var reloaded = new MagicDefense(xml, f.Actor.Object);
		Assert.AreEqual(20, reloaded.Capacity); Assert.AreEqual("40000", xml.Element("Remaining")!.Value);
		var damage = move.Absorb(new Damage { ActorOrigin = f.Attacker.Object, DamageType = DamageType.Crushing, DamageAmount = 10, StunAmount = 40, PainAmount = 20 });
		Assert.IsNotNull(damage); Assert.AreEqual(5, damage.DamageAmount); Assert.AreEqual(20, damage.StunAmount); Assert.AreEqual(10, damage.PainAmount);
		Assert.AreEqual(0, effect.Capacity); Assert.IsFalse(effect.Available);
	}

	[TestMethod]
	public void DefenseScope_AbsorbsOnlySelectedTargetAndAttacker_AndRestoresOnDispose()
	{
		var f = new Fixture(); var power = f.Defense("kineticbarrier"); var effect = new MagicDefense(f.Actor.Object, power); f.Effects.Add(effect);
		var move = new MagicDefenseMove(f.Actor.Object, effect); var incoming = f.Incoming();
		move.TryDefend(incoming, CheckOutcome.SimpleOutcome(CheckType.GenericSkillCheck, Outcome.Pass), out _);
		var damage = new Damage { ActorOrigin = f.Attacker.Object, DamageType = DamageType.Crushing, DamageAmount = 10 };
		using (var scope = new MagicDefenseDamageScope(incoming, move))
		{
			Assert.IsNull(MagicDefenseDamageScope.Filter(f.Actor.Object, null));
			Assert.AreSame(damage, MagicDefenseDamageScope.Filter(f.Attacker.Object, damage));
			Assert.IsNull(MagicDefenseDamageScope.Filter(f.Actor.Object, damage));
			Assert.IsFalse(scope.Finish(new CombatMoveResult { MoveWasSuccessful = true }).MoveWasSuccessful);
		}
		Assert.AreSame(damage, MagicDefenseDamageScope.Filter(f.Actor.Object, damage)); Assert.AreEqual(20, effect.Capacity);
	}

	[TestMethod]
	public void Barrier_RejectsControlOnlyAttacksAndMentalIntrusion()
	{
		var f = new Fixture(); var power = f.Defense("kineticbarrier");
		Assert.IsFalse(power.CanDefend(f.Actor.Object, new BreakClinchMove(f.Attacker.Object, f.Actor.Object)));
		Assert.AreEqual(MagicDefenseThreat.None, MagicDefenseMove.Classify(Mock.Of<ICombatMove>()));
	}

	[TestMethod]
	public void Attack_CommittedOnce_UsesOneRollAndOneResourceCharge()
	{
		var f = new Fixture(); f.Actor.SetupGet(x => x.Body).Returns((IBody)null!);
		var power = new Mock<IMagicAttackPower>(); power.SetupGet(x => x.WeaponAttack).Returns(f.Attack.Object);
		power.SetupGet(x => x.AttackEffects).Returns([]);
		power.Setup(x => x.CanInvokePower(f.Attacker.Object, f.Actor.Object)).Returns(true);
		f.Check.Setup(x => x.CheckAgainstAllDifficulties(It.IsAny<IPerceivableHaveTraits>(), It.IsAny<Difficulty>(), It.IsAny<ITraitDefinition>(),
			It.IsAny<IPerceivable>(), It.IsAny<double>(), It.IsAny<TraitUseType>(), It.IsAny<(string, object)[]>()))
			.Returns(Enum.GetValues<Difficulty>().ToDictionary(x => x, _ => CheckOutcome.SimpleOutcome(CheckType.GenericSkillCheck, Outcome.Pass)));
		var move = new MagicPowerAttackMove(f.Attacker.Object, f.Actor.Object, power.Object);
		var first = move.ResolveMove(new HelplessDefenseMove { Assailant = f.Actor.Object });
		Assert.IsTrue(first.MoveWasSuccessful); Assert.IsTrue(move.UsesStaminaWithResult(first));
		var second = move.ResolveMove(null!); Assert.AreSame(CombatMoveResult.Irrelevant, second); Assert.IsFalse(move.UsesStaminaWithResult(second));
		power.Verify(x => x.UseAttackPower(move), Times.Once);
	}

	[TestMethod]
	public void DefenseSelection_TiesPreferMundane_ExplicitMagicWins_AndCancellationRemovesCandidate()
	{
		var f = new Fixture(); var power = f.Defense("kineticparry"); var effect = new MagicDefense(f.Actor.Object, power); f.Effects.Add(effect);
		f.Check.Setup(x => x.TargetNumber(It.IsAny<IPerceivableHaveTraits>(), It.IsAny<Difficulty>(), It.IsAny<ITraitDefinition>(),
			It.IsAny<IPerceivable>(), It.IsAny<double>(), It.IsAny<(string, object)[]>())).Returns(50);
		var mundane = new DodgeMove { Assailant = f.Actor.Object };
		Assert.AreSame(mundane, MagicDefenseMove.Select(f.Actor.Object, f.Incoming(), mundane));
		f.Actor.SetupGet(x => x.PreferredDefenseType).Returns(DefenseType.Magic);
		var selected = MagicDefenseMove.Select(f.Actor.Object, f.Incoming(), mundane);
		Assert.IsInstanceOfType(selected, typeof(MagicDefenseMove));
		f.Effects.Clear(); Assert.AreSame(mundane, MagicDefenseMove.Select(f.Actor.Object, f.Incoming(), mundane));
		Assert.AreSame(mundane, MagicDefenseMove.Revalidate(selected, f.Incoming()));
	}

	[TestMethod]
	public void DefenseLifecycle_InitialAndLoginRegisterOnlyOnce_AndReleaseUnsubscribes()
	{
		var f = new Fixture(); var effect = new MagicDefense(f.Actor.Object, f.Defense("kineticparry"));
		effect.InitialEffect(); effect.Login();
		f.Actor.SetupGet(x => x.State).Returns(CharacterState.Unconscious);
		f.Actor.Raise(x => x.OnStateChanged += null, f.Actor.Object);
		f.Actor.Verify(x => x.RemoveEffect(effect, true), Times.Once);
		effect.ReleaseEvents(); f.Actor.Raise(x => x.OnStateChanged += null, f.Actor.Object);
		f.Actor.Verify(x => x.RemoveEffect(effect, true), Times.Once);
	}

	[DataTestMethod]
	[DataRow(MagicAttackEffectType.Knockdown)]
	[DataRow(MagicAttackEffectType.Stagger)]
	[DataRow(MagicAttackEffectType.Pushback)]
	[DataRow(MagicAttackEffectType.Pull)]
	[DataRow(MagicAttackEffectType.Disarm)]
	[DataRow(MagicAttackEffectType.BreakClinch)]
	public void ControlRiders_SuccessUsesExistingRuntimeActions_ResistancePreventsThem(MagicAttackEffectType type)
	{
		var f = new Fixture(); var cell = new Mock<ICell>();
		f.Actor.SetupGet(x => x.Location).Returns(cell.Object); f.Attacker.SetupGet(x => x.Location).Returns(cell.Object);
		f.Attacker.SetupGet(x => x.Combat).Returns(f.Actor.Object.Combat);
		f.Attacker.SetupGet(x => x.CombatTarget).Returns(f.Actor.Object);
		f.Attacker.Setup(x => x.ColocatedWith(f.Actor.Object)).Returns(true);
		f.Actor.SetupGet(x => x.CombatTarget).Returns(f.Attacker.Object);
		var item = new Mock<IGameItem>() { DefaultValue = DefaultValue.Mock };
		f.Actor.SetupGet(x => x.Body.WieldedItems).Returns([item.Object]);
		f.Actor.Setup(x => x.Body.CanBeDisarmed(item.Object, f.Attacker.Object)).Returns(true);
		f.Attacker.Setup(x => x.EffectsOfType<ClinchEffect>(It.IsAny<Predicate<ClinchEffect>>())).Returns([new ClinchEffect(f.Attacker.Object, f.Actor.Object)]);
		var power = new Mock<IMagicAttackPower>(); power.SetupGet(x => x.WeaponAttack).Returns(f.Attack.Object);
		power.SetupGet(x => x.AttackEffects).Returns([new MagicAttackEffect(type, Difficulty.Normal, 1, 2, "$0 moves $1.", "$1 resists $0.")]);
		f.SetOutcome(Outcome.MajorFail);
		f.Check.Setup(x => x.Check(f.Attacker.Object, It.IsAny<Difficulty>(), It.IsAny<ITraitDefinition>(),
			It.IsAny<IPerceivable>(), It.IsAny<double>(), It.IsAny<TraitUseType>(), It.IsAny<(string, object)[]>()))
			.Returns(CheckOutcome.SimpleOutcome(CheckType.GenericSkillCheck, Outcome.MajorPass));
		MagicAttackEffectResolver.Apply(f.Attacker.Object, f.Actor.Object, power.Object);
		f.World.Verify(x => x.GetCheck(MagicAttackEffectResolver.ResistanceCheck(type)), Times.Once);
		if (type == MagicAttackEffectType.Knockdown) f.Actor.Verify(x => x.DoCombatKnockdown(It.IsAny<int>(), VehicleCombatDisplacementType.Knockdown), Times.Once);
		if (type == MagicAttackEffectType.Disarm) Mock.Get(f.Actor.Object.Body).Verify(x => x.Take(item.Object), Times.Once);
		f.SetOutcome(Outcome.MajorPass);
		MagicAttackEffectResolver.Apply(f.Attacker.Object, f.Actor.Object, power.Object);
		if (type == MagicAttackEffectType.Knockdown) f.Actor.Verify(x => x.DoCombatKnockdown(It.IsAny<int>(), VehicleCombatDisplacementType.Knockdown), Times.Once);
		if (type == MagicAttackEffectType.Disarm) Mock.Get(f.Actor.Object.Body).Verify(x => x.Take(item.Object), Times.Once);
	}

	[DataTestMethod]
	[DataRow("-1")]
	[DataRow("NaN")]
	[DataRow("Infinity")]
	public void Defense_InvalidCapacityDefinitionIsRejected(string capacity)
	{
		var f = new Fixture(); var model = Fixture.Model(PsionicStockContent.CombatPowers.Last());
		var xml = XElement.Parse(model.Definition); xml.SetElementValue("MaximumCapacity", capacity); model.Definition = xml.ToString();
		Assert.ThrowsException<ApplicationException>(() => new MagicDefensePower(model, f.World.Object));
	}
}
