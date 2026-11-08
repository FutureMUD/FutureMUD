#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Body.Traits;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Magic;
using MudSharp.Magic.Casting;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests;

public partial class ArmageddonStockProgressionRuntimeTests
{
	[TestMethod]
	[DataRow(ArmageddonProgressionMode.Classic, false)]
	[DataRow(ArmageddonProgressionMode.Classic, true)]
	[DataRow(ArmageddonProgressionMode.Simple, false)]
	[DataRow(ArmageddonProgressionMode.Simple, true)]
	public void StockGraph_PaidPracticeAtNativeGainBoundary_AcquiresOnlyAfterAnActualGain(ArmageddonProgressionMode mode, bool gain)
	{
		var (f, stock, cap, role) = Fixture(mode, "earth");
		var own = stock.Bindings.Where(x => x.CapabilityKey == "arm.capability.earth").ToArray();
		var root = own.Single(x => x.ContentKey == "arm.spell.earthen_ward");
		var child = own.Single(x => x.ContentKey == "arm.spell.stone_skin");
		var rootSpell = f.Spells.Single(x => x.Name == stock.SpellNames[root.ContentKey]);
		var childSpell = f.Spells.Single(x => x.Id >= 1000 && x.Name == stock.SpellNames[child.ContentKey]);
		foreach (var command in new[] { "grades practice fixture", "grades efficiency source 7 1", "grades mastery 0 600" })
			Assert.IsTrue(rootSpell.BuildingCommand(f.Actor.Object, new StringStack(command)));
		f.Actor.SetupGet(x => x.Combat).Returns((MudSharp.Combat.ICombat)null!);
		f.Actor.SetupGet(x => x.Movement).Returns((MudSharp.Movement.IMovement)null!);
		var effects = new List<IEffect>();
		f.Actor.Setup(x => x.AddEffect(It.IsAny<IEffect>(), It.IsAny<TimeSpan>())).Callback<IEffect, TimeSpan>((e, _) => effects.Add(e));
		f.Actor.Setup(x => x.RemoveEffect(It.IsAny<IEffect>(), It.IsAny<bool>())).Callback<IEffect, bool>((e, _) => effects.Remove(e));
		f.Actor.Setup(x => x.CombinedEffectsOfType<IActionEffect>()).Returns(() => effects.OfType<IActionEffect>());
		Assert.IsTrue(f.Service.EnrolFromSelectedClass(f.Actor.Object, role.Object).Single().Result.Allowed);
		var trait = cap.CastingPolicy!.Admissions.Single(x => x.SpellId == rootSpell.Id).TraitId!.Value;
		Assert.AreEqual(mode == ArmageddonProgressionMode.Classic ? 60 : 10, f.Skills[trait]);
		Assert.AreEqual(1, f.Service.Acquisition(f.Actor.Object, rootSpell.Id)!.ControlledGrade);
		Assert.IsNull(f.Service.Acquisition(f.Actor.Object, childSpell.Id));
		// Explicit boundary setup tests the shared event contract, not a natural opening-to-branch campaign.
		var threshold = mode == ArmageddonProgressionMode.Classic ? child.ParentThreshold!.Value : child.MainThreshold!.Value;
		Assert.AreEqual(mode == ArmageddonProgressionMode.Classic ? 80 : 15, threshold);
		f.Skills[trait] = threshold - 0.5;
		f.NativeSkill.Setup(x => x.TraitUsed(f.Actor.Object, It.IsAny<Outcome>(), It.IsAny<Difficulty>(), It.IsAny<TraitUseType>(),
			It.IsAny<IEnumerable<Tuple<string, double>>>())).Returns(() => { if (gain) f.Skills[trait] += 0.5; return gain; });
		var reserve = f.Resources.Single(x => x.Id == cap.CastingPolicy.ReserveResourceId);
		var foreign = f.Resources.First(x => x.Id != reserve.Id);
		f.Balances[foreign] = 123;
		f.Balances[reserve] = 0;
		var intent = new MagicCastingIntent(f.Actor.Object, cap.Id, rootSpell.Id, 1, false, "", MagicCastingMode.Practice);
		var operationsBefore = f.Store.Operations.Count;
		Assert.AreEqual(MagicCastingStatus.Refused, f.Service.Cast(intent).Status);
		Assert.AreEqual(operationsBefore, f.Store.Operations.Count);
		f.Balances[reserve] = 50; // Funding itself is covered by the native zero-to-paid scenario.
		var started = f.Service.Cast(intent);
		Assert.AreEqual(MagicCastingStatus.Started, started.Status, started.Message);
		Assert.AreEqual(0, f.Balances[reserve]);
		Assert.AreEqual(threshold - 0.5, f.Skills[trait]);
		Assert.IsNull(f.Service.Acquisition(f.Actor.Object, childSpell.Id));
		f.Now += TimeSpan.FromSeconds(30);
		var action = effects.OfType<MagicPracticeAction>().Single();
		action.ExpireEffect();
		Assert.AreEqual("Completed", f.Store.Operation(started.OperationId!.Value)!.Stage);
		Assert.AreEqual(threshold - (gain ? 0 : 0.5), f.Skills[trait]);
		Assert.AreEqual(gain, f.Service.Acquisition(f.Actor.Object, childSpell.Id) is not null);
		if (gain) Assert.AreEqual(1, f.Service.Acquisition(f.Actor.Object, childSpell.Id)!.ControlledGrade);
		Assert.AreEqual(123.0, f.Balances[foreign]);
		var state = f.Store.Acquired.ToDictionary(); var opportunity = f.Store.Opportunity(100, trait); var writes = f.Store.Writes;
		action.ExpireEffect();
		Assert.AreEqual(writes, f.Store.Writes);
		f.Restart(); f.Service.Reconcile(f.Actor.Object);
		CollectionAssert.AreEquivalent(state.ToArray(), f.Store.Acquired.ToArray());
		Assert.AreEqual(opportunity, f.Store.Opportunity(100, trait));
		Assert.AreEqual(0, f.Balances[reserve]);
	}
	[TestMethod]
	[DataRow(CharacterState.Awake, "Standing", false, 2.0)]
	[DataRow(CharacterState.Awake, "Sitting", false, 4.0)]
	[DataRow(CharacterState.Sleeping, "Sitting", false, 0.0)]
	[DataRow(CharacterState.Awake, "Sitting", true, 2.0)]
	public void StockNativeGenerator_ConsciousRestAndCombat_UseLockedFundingRate(CharacterState state, string posture, bool combat, double expected)
	{
		var f = new MagicCastingFixture(); var reserve = f.Resources[1];
		f.Balances[reserve] = 0;
		f.Actor.SetupGet(x => x.State).Returns(state);
		f.Actor.SetupGet(x => x.Combat).Returns(combat ? new Mock<MudSharp.Combat.ICombat>().Object : null!);
		f.Actor.SetupGet(x => x.Movement).Returns((MudSharp.Movement.IMovement)null!);
		var position = new Mock<MudSharp.Body.Position.IPositionState>(); position.SetupGet(x => x.Name).Returns(posture);
		f.Actor.SetupGet(x => x.PositionState).Returns(position.Object);
		f.Actor.Setup(x => x.AddResource(reserve, It.IsAny<double>())).Callback<MudSharp.Magic.IMagicResource, double>((_, n) => f.Balances[reserve] += n);
		var generator = new MudSharp.Magic.Generators.LinearTimeBasedGenerator(new MudSharp.Models.MagicGenerator { Id = 41, Name = "Locked Earth funding",
			Definition = "<Definition><WhichResource>11</WhichResource><AmountPerMinute>2</AmountPerMinute><ConsciousOnly>true</ConsciousOnly><RestMultiplier>2</RestMultiplier></Definition>" }, f.World.Object);
		generator.GetOnMinuteDelegate(f.Actor.Object)();
		Assert.AreEqual(expected, f.Balances[reserve]);
	}

}
