#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.Combat;
using MudSharp.Combat.Moves;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Health;
using MudSharp.Magic;
using MudSharp.Magic.Powers;
using MudSharp.FutureProg;
using MudSharp.Planes;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Law;
using MudSharp.NPC.AI;

namespace MudSharp_Unit_Tests;

public partial class PhysicalImpactTests
{
	[TestMethod]
	[DataRow(false, false)]
	[DataRow(true, false)]
	[DataRow(false, true)]
	public void Guard_InterceptionStopsOriginalActionAndEngagesIndependently(bool alreadyEngaged, bool invalidatesWard)
	{
		using var f = new Fixture(true);
		var guard = new Mock<ICharacter>(); guard.As<ICharacterInstance>(); var body = new Mock<IBody>();
		guard.SetupGet(x => x.Id).Returns(301);
		guard.Setup(x => x.Equals(It.IsAny<ICharacter>())).Returns<ICharacter>(x => ReferenceEquals(x, guard.Object));
		var applies = true;
		var effect = new Mock<IGuardCharacterEffect>();
		effect.SetupGet(x => x.Targets).Returns([f.Target.Object]);
		effect.Setup(x => x.Applies()).Returns(() => applies);
		guard.SetupGet(x => x.Gameworld).Returns(f.World.Object);
		guard.SetupGet(x => x.Body).Returns(body.Object); body.SetupGet(x => x.Actor).Returns(guard.Object);
		body.SetupGet(x => x.BasePlanarPresence).Returns(PlanarPresenceDefinition.DefaultMaterial(1));
		guard.SetupGet(x => x.Identity).Returns(Mock.Of<ICharacterIdentity>(x => x.Id == 301 && x.Instances == new[] { (ICharacterInstance)guard.Object }));
		guard.SetupGet(x => x.Location).Returns(f.Room.Object);
		guard.SetupGet(x => x.State).Returns(CharacterState.Awake);
		guard.SetupGet(x => x.PositionState).Returns(PositionStanding.Instance);
		guard.SetupGet(x => x.OutputHandler).Returns(f.Actor.Object.OutputHandler);
		guard.Setup(x => x.EffectsOfType<IGuardCharacterEffect>(It.IsAny<Predicate<IGuardCharacterEffect>>())).Returns([effect.Object]);
		guard.Setup(x => x.ColocatedWith(f.Target.Object)).Returns(true);
		guard.Setup(x => x.CanSee(f.Actor.Object)).Returns(true);
		guard.SetupGet(x => x.Combat).Returns(alreadyEngaged ? f.Combat.Object : null);
		guard.SetupGet(x => x.CombatTarget).Returns(alreadyEngaged ? f.Actor.Object : null);
		guard.Setup(x => x.CanEngage(f.Actor.Object)).Returns(true);
		var reactionContinued = true;
		guard.Setup(x => x.Engage(f.Actor.Object, false, false)).Returns(() =>
		{
			if (invalidatesWard) { applies = false; reactionContinued = CommandExecutionScope.TryContinue(guard.Object); }
			return reactionContinued;
		});
		f.Room.SetupGet(x => x.Characters).Returns([f.Actor.Object, f.Target.Object, guard.Object]);
		f.World.SetupGet(x => x.Actors).Returns(new All<ICharacter> { f.Actor.Object, f.Target.Object, guard.Object });
		f.World.SetupGet(x => x.Characters).Returns(new All<ICharacter>());
		f.World.SetupGet(x => x.NPCs).Returns(new All<ICharacter>());
		f.World.SetupGet(x => x.CachedActors).Returns(new All<ICharacter>());
		f.Check.Setup(x => x.Check(guard.Object, It.IsAny<Difficulty>(), f.Guard, f.Actor.Object, It.IsAny<double>(), It.IsAny<TraitUseType>(), It.IsAny<(string, object)[]>()))
			.Returns(CheckOutcome.SimpleOutcome(CheckType.GenericSkillCheck, Outcome.Pass));
		var move = f.Move(); var result = move.ResolveMove(null!);
		Assert.AreEqual(invalidatesWard, result.MoveWasSuccessful);
		Assert.AreEqual(!invalidatesWard, reactionContinued);
		Assert.IsTrue(move.UsesStaminaWithResult(result));
		Assert.AreSame(f.Target.Object, f.Actor.Object.CombatTarget);
		guard.Verify(x => x.Engage(f.Actor.Object, false, false), alreadyEngaged ? Times.Never() : Times.Once());
		Assert.AreEqual(invalidatesWard ? 1 : 0, f.Damage.Count);
		Assert.AreEqual(0, f.SelfDamage.Count + f.ActorSeats);
		Assert.AreEqual(invalidatesWard, f.Check.Invocations.Any(x => ReferenceEquals(x.Arguments[0], f.Actor.Object)));
	}

	[TestMethod]
	[DataRow(2.0, 3.0, 1.0)]
	[DataRow(0.0, 3.0, 0.0)]
	public void ShieldPartial_UsesDefenderAwareArmourAndPreservesAllRemainderChannels(double amount, double stun, double pain)
	{
		using var f = new Fixture(false);
		var properties = new[] { "PowerMoveStaminaCost", "StrengthForRelativeStrengthCheck", "RelativeStrengthDefenseStaminaCost" }
			.Select(x => typeof(CombatBase).GetProperty(x, BindingFlags.Static | BindingFlags.NonPublic)!).ToArray();
		var previous = properties.Select(x => x.GetValue(null)).ToArray();
		try
		{
			properties[0].SetValue(null, new MudSharp.Body.Traits.TraitExpression("1", f.World.Object));
			properties[1].SetValue(null, new MudSharp.Body.Traits.TraitExpression("1", f.World.Object));
			properties[2].SetValue(null, new ExpressionEngine.Expression("1"));
			var item = new Mock<IGameItem>(); var shield = new Mock<IShield>(); var armour = new Mock<IArmour>(); var type = new Mock<IArmourType>();
			item.Setup(x => x.Equals(It.IsAny<IGameItem>())).Returns<IGameItem>(x => ReferenceEquals(x, item.Object));
			shield.SetupGet(x => x.Parent).Returns(item.Object);
			shield.SetupGet(x => x.ShieldType).Returns(Mock.Of<IShieldType>(x => x.BlockTrait == f.Guard && x.StaminaPerBlock == 2));
			item.Setup(x => x.GetItemType<IShield>()).Returns(shield.Object);
			item.Setup(x => x.GetItemType<IArmour>()).Returns(armour.Object);
			armour.SetupGet(x => x.ArmourType).Returns(type.Object);
			armour.SetupGet(x => x.Parent).Returns(item.Object);
			f.TargetBody.SetupGet(x => x.WieldedItems).Returns([item.Object]);
			f.TargetBody.Setup(x => x.CanWield(item.Object, ItemCanWieldFlags.IgnoreFreeHands)).Returns(true);
			f.Check.Setup(x => x.Check(f.Target.Object, It.IsAny<Difficulty>(), f.Guard, f.Actor.Object, It.IsAny<double>(), It.IsAny<TraitUseType>(), It.IsAny<(string, object)[]>()))
				.Returns(CheckOutcome.SimpleOutcome(CheckType.BlockCheck, Outcome.MajorPass));
						type.Setup(x => x.AbsorbDamage(It.IsAny<IDamage>(), armour.Object, f.Target.Object, ref It.Ref<List<IWound>>.IsAny, true))
				.Returns(new Damage { DamageType = DamageType.Shearing, DamageAmount = amount, StunAmount = stun, PainAmount = pain });
			var result = f.Move().ResolveMove(null!);
			Assert.IsTrue(result.MoveWasSuccessful);
			Assert.AreEqual(1, f.Damage.Count);
			Assert.AreEqual(amount, f.Damage[0].DamageAmount);
			Assert.AreEqual(stun, f.Damage[0].StunAmount);
			Assert.AreEqual(pain, f.Damage[0].PainAmount);
			Assert.AreEqual(DamageType.Shearing, f.Damage[0].DamageType);
			armour.Verify(x => x.PassiveSufferDamage(It.IsAny<IDamage>(), ref It.Ref<List<IWound>>.IsAny), Times.Never);
			f.TargetBody.Verify(x => x.SpendStamina(2), Times.Once);
			Assert.AreEqual(0, f.ActorSeats + f.TargetSeats);
		}
		finally { for (var i = 0; i < properties.Length; i++) properties[i].SetValue(null, previous[i]); }
	}

	[TestMethod]
	[DataRow(30.0, false)]
	[DataRow(3.0, true)]
	public void RealBarrier_FiltersDamageBeforeBashContactControl(double capacity, bool contact)
	{
		using var f = new Fixture(true);
		var school = Mock.Of<IMagicSchool>(x => x.Id == 1);
		var resource = Mock.Of<IMagicResource>(x => x.Id == 1 && x.ShortName == "test");
		f.World.SetupGet(x => x.MagicSchools).Returns(new All<IMagicSchool> { school });
		f.World.SetupGet(x => x.MagicCapabilities).Returns(new All<IMagicCapability>());
		f.World.SetupGet(x => x.LegalAuthorities).Returns(new All<ILegalAuthority>());
		f.World.SetupGet(x => x.Characters).Returns(new All<ICharacter>());
		f.World.SetupGet(x => x.MagicResources).Returns(new All<IMagicResource> { resource });
		var prog = new Mock<IFutureProg>(); prog.SetupGet(x => x.Id).Returns(0);
		prog.Setup(x => x.ExecuteBool(It.IsAny<object[]>())).Returns(true);
		prog.Setup(x => x.Execute<bool?>(It.IsAny<object[]>())).Returns(true);
		var progs = new Mock<IUneditableAll<IFutureProg>>();
		progs.Setup(x => x.Get(It.IsAny<long>())).Returns(prog.Object);
		f.World.SetupGet(x => x.FutureProgs).Returns(progs.Object);
		var stock = PsionicStockContent.CombatPowers.Single(x => x.Verb == "kineticbarrier");
		var definition = PsionicStockContent.CombatDefinition(stock, f.Skill.Id, 1, 0, 0, 1);
		var model = new MudSharp.Models.MagicPower { Id = 10, Name = stock.Name, MagicSchoolId = 1, PowerModel = "magicdefense", Definition = definition.ToString() };
		var power = new MagicDefensePower(model, f.World.Object);
		var effects = new List<MudSharp.Effects.IEffect>();
		f.Target.SetupGet(x => x.Powers).Returns([power]);
		f.Target.SetupGet(x => x.Effects).Returns(effects);
		f.Target.SetupGet(x => x.MagicResourceAmounts).Returns(new Dictionary<IMagicResource, double> { [resource] = 100 });
		f.Target.Setup(x => x.UseResource(resource, It.IsAny<double>())).Returns(true);
		f.Target.Setup(x => x.IsBlocked("general")).Returns((false, ""));
		f.Race.SetupGet(x => x.CombatSettings).Returns(new RacialCombatSettings { CanDefend = true });
		f.Target.Setup(x => x.CanSee(f.Actor.Object)).Returns(true);
		var effect = new MagicDefense(f.Target.Object, power);
		if (capacity < effect.Capacity) effect.UseCapacity(effect.Capacity - capacity);
		effects.Add(effect);
		// This mock body implements the production BodyBiology damage-filter boundary.
		f.TargetBody.Setup(x => x.PassiveSufferDamage(It.IsAny<IDamage>())).Callback<IDamage>(d =>
		{
			var filtered = MagicDefenseDamageScope.Filter(f.Target.Object, d);
			if (filtered is not null) f.Damage.Add(filtered);
		}).Returns([]);
		var move = f.Move();
		Assert.IsTrue(power.CanDefend(f.Target.Object, move));
		var result = move.ResolveMove(new MagicDefenseMove(f.Target.Object, effect));
		Assert.AreEqual(contact, result.MoveWasSuccessful);
		Assert.AreEqual(contact ? 1 : 0, f.TargetSeats);
		Assert.AreEqual(contact ? 1 : 0, f.Damage.Count);
		Assert.AreEqual(Math.Max(0, capacity - 7), effect.Capacity);
		if (contact) { Assert.AreEqual(20.0 / 7, f.Damage[0].DamageAmount, 0.0001); Assert.AreEqual(4.0, f.Damage[0].StunAmount); }
		Assert.IsTrue(result.DefenderResponseWasUsed);
	}
}
