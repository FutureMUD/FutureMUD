#nullable enable

using System;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Accounts;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Combat.Moves;
using MudSharp.Commands.Modules;
using MudSharp.Construction;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.Framework.Units;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Law;

namespace MudSharp_Unit_Tests.Commands;

[TestClass]
public class SecurityCommandBoundaryTests
{
	[DataTestMethod]
	[DataRow(1.0, true)]
	[DataRow(1.0, false)]
	[DataRow(0.0, false)]
	public void WeightedDrop_RejectsZeroAndChecksLawBeforeSplitting(double amount, bool unlawful)
	{
		var actor = new Mock<ICharacter>(); var world = new Mock<IFuturemud>();
		var body = new Mock<IBody>(); var item = new Mock<IGameItem>(); var units = new Mock<IUnitManager>();
		var authority = new Mock<ILegalAuthority>(); authority.SetupGet(x => x.Id).Returns(1);
		var authorities = new All<ILegalAuthority>(); authorities.Add(authority.Object);
		actor.SetupGet(x => x.Gameworld).Returns(world.Object);
		actor.SetupGet(x => x.Body).Returns(body.Object);
		actor.SetupGet(x => x.Account).Returns(Mock.Of<IAccount>(x => x.ActLawfully));
		actor.SetupGet(x => x.OutputHandler).Returns(Mock.Of<IOutputHandler>());
		actor.SetupGet(x => x.Location).Returns(Mock.Of<IRoom>());
		actor.Setup(x => x.TargetHeldItem("commodity")).Returns(item.Object);
		world.SetupGet(x => x.UnitManager).Returns(units.Object);
		world.SetupGet(x => x.LegalAuthorities).Returns(authorities);
		var success = true;
		units.Setup(x => x.GetBaseUnits(It.IsAny<string>(), UnitType.Mass, out success)).Returns(amount);
		item.Setup(x => x.GetItemType<ICommodity>()).Returns(Mock.Of<ICommodity>());
		item.Setup(x => x.DropByWeight(actor.Object.Location, amount)).Returns(Mock.Of<IGameItem>());
		authority.Setup(x => x.WouldBeACrime(actor.Object, CrimeTypes.Littering, null, item.Object, "")).Returns(unlawful);
		Invoke(typeof(InventoryModule), "Drop", actor.Object, "drop 1kg commodity");
		var allowed = amount > 0 && !unlawful;
		item.Verify(x => x.DropByWeight(It.IsAny<IRoom>(), It.IsAny<double>()), allowed ? Times.Once() : Times.Never());
		Assert.AreEqual(allowed ? 1 : 0, body.Invocations.Count(x => x.Method.Name == "Drop"));
	}

	[TestMethod]
	public void FriendlyForcedMovement_RejectsBystanderAtCommandSelectionAndResolution()
	{
		var combat = new Mock<ICombat>(); combat.SetupGet(x => x.Friendly).Returns(true);
		var actor = new Mock<ICharacter>(); var target = new Mock<ICharacter>();
		actor.SetupGet(x => x.Combat).Returns(combat.Object);
		actor.SetupGet(x => x.OutputHandler).Returns(Mock.Of<IOutputHandler>());
		actor.Setup(x => x.TargetActor("target", PerceiveIgnoreFlags.None)).Returns(target.Object);
		actor.Setup(x => x.ColocatedWith(target.Object)).Returns(true);
		actor.SetupGet(x => x.MeleeRange).Returns(true);
		Invoke(typeof(CombatModule), "Shove", actor.Object, "shove target invalid destination");
		Assert.IsTrue(Mock.Get(actor.Object.OutputHandler).Invocations.Any(x => x.Arguments.OfType<string>()
			.Any(s => s.Contains("another participant"))));
		Assert.IsFalse(CombatForcedMovementUtilities.GetForcedMovementAttacks(actor.Object, target.Object,
			ForcedMovementVerbs.Shove, ForcedMovementTypes.Exit).Any());
		Assert.IsFalse(CombatForcedMovementUtilities.IsInRange(actor.Object, target.Object, ForcedMovementRange.Melee));
		target.SetupGet(x => x.Combat).Returns(combat.Object);
		Assert.IsTrue(CombatForcedMovementUtilities.IsInRange(actor.Object, target.Object, ForcedMovementRange.Melee));
	}

	[DataTestMethod]
	[DataRow(TrialPhase.Introduction, false, false)]
	[DataRow(TrialPhase.Charges, false, true)]
	[DataRow(TrialPhase.Introduction, true, true)]
	public void TrialProgress_RevealsChargeNamesOnlyAtAuthoredPhaseOrInManualTrial(TrialPhase phase, bool manual, bool revealed)
	{
		var actor = new Mock<ICharacter>(); var crime = new Mock<ICrime>();
		actor.SetupGet(x => x.LineFormatLength).Returns(120);
		actor.SetupGet(x => x.Account).Returns(Mock.Of<IAccount>());
		crime.SetupGet(x => x.Name).Returns("UnannouncedCharge");
		var trial = new OnTrial(actor.Object, Mock.Of<ILegalAuthority>(), DateTime.UtcNow, [crime.Object])
		{
			Phase = phase, ManualTrial = manual
		};
		var text = new StringBuilder();
		typeof(CrimeModule).GetMethod("AppendTrialProgressTable", BindingFlags.Static | BindingFlags.NonPublic)!
			.Invoke(null, [text, actor.Object, trial]);
		Assert.AreEqual(revealed, text.ToString().Contains("UnannouncedCharge", StringComparison.Ordinal));
		if (!revealed) crime.VerifyGet(x => x.Name, Times.Never);
	}

	private static void Invoke(Type module, string method, ICharacter actor, string command) =>
		module.GetMethod(method, BindingFlags.Static | BindingFlags.NonPublic)!.Invoke(null, [actor, command]);
}
