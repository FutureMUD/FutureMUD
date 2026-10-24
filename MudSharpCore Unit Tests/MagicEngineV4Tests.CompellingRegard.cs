#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Accounts;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Commands;
using MudSharp.Commands.Trees;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.Magic;
using MudSharp.Magic.Casting;
using MudSharp.Magic.SpellEffects;

namespace MudSharp_Unit_Tests;

public partial class MagicEngineV4Tests
{
	[TestMethod]
	[DataRow(false, 1)] [DataRow(false, 2)] [DataRow(false, 3)] [DataRow(false, 4)] [DataRow(false, 5)] [DataRow(false, 6)] [DataRow(false, 7)]
	[DataRow(true, 1)] [DataRow(true, 7)]
	public void CompellingRegard_GradesAndPcNpcBindingsExpireAtAbsoluteDeadline(bool pc, int grade)
	{
		var f = new CompellingFixture(); using var clock = RuntimeClock.Push(f.Base.Clock);
		f.Pair.Target.SetupGet(x => x.IsPlayerCharacter).Returns(pc);
		var result = f.Base.Cast(grade); Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		Assert.IsTrue(f.Child.CanControl(f.Pair.F.Actor.Object)); Assert.AreEqual(f.Base.Clock.Now.AddSeconds(30 * grade), f.Child.ExpiryUtc);
		f.Base.Clock.Now = f.Child.ExpiryUtc!.Value; Assert.IsFalse(f.Child.CanControl(f.Pair.F.Actor.Object));
	}

	[TestMethod]
	[DataRow("pcs")] [DataRow("undead")] [DataRow("staff")] [DataRow("self")] [DataRow("duplicate")] [DataRow("changed")]
	public void CompellingRegard_IneligibleAndChangedTargetsRefuseBeforePayment(string condition)
	{
		var f = new CompellingFixture(); using var clock = RuntimeClock.Push(f.Base.Clock);
		if (condition == "pcs")
		{ f.Pair.Target.SetupGet(x => x.IsPlayerCharacter).Returns(true); Assert.IsTrue(f.Pair.F.Spells.Single().SpellEffects.OfType<CompellingRegardEffect>().Single().BuildingCommand(f.Pair.F.Actor.Object, new StringStack("pcs"))); }
		if (condition == "undead") f.Pair.Target.SetupGet(x => x.Merits).Returns([f.Base.Undead]);
		if (condition == "staff") f.Pair.Target.Setup(x => x.IsAdministrator(PermissionLevel.JuniorAdmin)).Returns(true);
		if (condition == "self") f.Pair.F.Actor.Setup(x => x.TargetActorOrCorpse("traveller", It.IsAny<PerceiveIgnoreFlags>())).Returns(f.Pair.F.Actor.Object);
		if (condition == "duplicate") Assert.AreEqual(MagicCastingStatus.Succeeded, f.Base.Cast(1).Status);
		if (condition == "changed") f.Pair.F.Checkpoint = stage => { if (stage == "BeforePayment") f.Pair.Target.SetupGet(x => x.Merits).Returns([f.Base.Undead]); };
		var balance = f.Pair.F.Balances[f.Pair.F.Resources[1]];
		Assert.AreEqual(MagicCastingStatus.Refused, f.Base.Cast(2).Status); Assert.AreEqual(balance, f.Pair.F.Balances[f.Pair.F.Resources[1]]);
	}

	[TestMethod]
	[DataRow("l")] [DataRow("lo")] [DataRow("alias look")] [DataRow("quit")] [DataRow("force all look")] [DataRow("prog 1")]
	[DataRow("north")] [DataRow("kill traveller")] [DataRow("look\nquit")]
	public void CompellingRegard_OrdersRejectAliasesIndirectPrivilegedAndQueuedCommands(string input)
	{
		var f = new CompellingFixture(); using var clock = RuntimeClock.Push(f.Base.Clock); f.Base.Cast(1);
		Assert.IsFalse(SpellCompulsionExecution.Execute(f.Pair.F.Actor.Object, f.Pair.Target.Object, f.Child, input));
		f.Pair.TargetBody.Verify(x => x.Look(It.IsAny<bool>()), Times.Never);
	}

	[TestMethod]
	public void CompellingRegard_ExactNativeLookRunsOnceAndNestedDelegateCannotBypassFinalAdmission()
	{
		var f = new CompellingFixture(); using var clock = RuntimeClock.Push(f.Base.Clock); f.Base.Cast(1);
		var nested = new Command<ICharacter>((_, _) => Assert.Fail("Nested command executed."));
		f.Pair.TargetBody.Setup(x => x.Look(It.IsAny<bool>())).Callback(() =>
		{
			Assert.IsTrue(SpellCompulsionExecution.RejectNestedCommand);
			Assert.IsFalse(nested.Execute(f.Pair.Target.Object, "look", f.Pair.Target.Object.State, f.Pair.Target.Object.PermissionLevel));
			Assert.IsFalse(SpellCompulsionExecution.Execute(f.Pair.F.Actor.Object, f.Pair.Target.Object, f.Child, "look"));
		});
		Assert.IsTrue(SpellCompulsionExecution.Execute(f.Pair.F.Actor.Object, f.Pair.Target.Object, f.Child, "look"));
		f.Pair.TargetBody.Verify(x => x.Look(It.IsAny<bool>()), Times.Once);
		Assert.IsFalse(SpellCompulsionExecution.RejectNestedCommand);
	}

	[TestMethod]
	public void CompellingRegard_CommandOverrideAndNativeStateRestrictionsCannotBeBypassed()
	{
		var f = new CompellingFixture(); using var clock = RuntimeClock.Push(f.Base.Clock); f.Base.Cast(1);
		f.Manager.Override("look", new Command<ICharacter>((_, _) => Assert.Fail("Replaced command executed.")));
		Assert.IsFalse(SpellCompulsionExecution.Execute(f.Pair.F.Actor.Object, f.Pair.Target.Object, f.Child, "look"));
		f.Manager.Override("look", SpellCompulsionExecution.PermittedNativeCommand("look")!);
		f.Pair.Target.SetupGet(x => x.State).Returns(CharacterState.Sleeping);
		Assert.IsFalse(SpellCompulsionExecution.Execute(f.Pair.F.Actor.Object, f.Pair.Target.Object, f.Child, "look"));
	}

	[TestMethod]
	public void CompellingRegard_WardCallbackCannotAuthoriseAControllerWhoMovedAway()
	{
		var f = new CompellingFixture(); using var clock = RuntimeClock.Push(f.Base.Clock); f.Base.Cast(1);
		var ward = new Mock<MudSharp.Effects.Interfaces.IMagicInterdictionEffect>();
		ward.SetupGet(x => x.Coverage).Returns(MudSharp.Magic.MagicInterdictionCoverage.Incoming);
		ward.Setup(x => x.ShouldInterdict(f.Pair.F.Actor.Object, f.Pair.F.School)).Returns(() => { f.Pair.CasterLocation = f.Pair.Destination.Object; return false; });
		f.Pair.Target.Setup(x => x.EffectsOfType<MudSharp.Effects.Interfaces.IMagicInterdictionEffect>(It.IsAny<Predicate<MudSharp.Effects.Interfaces.IMagicInterdictionEffect>>())).Returns([ward.Object]);
		Assert.IsFalse(f.Child.CanControl(f.Pair.F.Actor.Object));
		Assert.IsFalse(SpellCompulsionExecution.Execute(f.Pair.F.Actor.Object, f.Pair.Target.Object, f.Child, "look"));
	}

	[TestMethod]
	public void CompellingRegard_SavedControllerAndTargetBodiesRetainExactBindingAndLogoutCancels()
	{
		var f = new CompellingFixture(); using var clock = RuntimeClock.Push(f.Base.Clock); f.Base.Cast(2); var child = f.Child;
		SpellCompellingRegardEffect.InitialiseEffectType();
		var loaded = (SpellCompellingRegardEffect)Effect.LoadEffect(child.SaveToXml(new Dictionary<IEffect, TimeSpan>()), f.Pair.Target.Object);
		var parent = new MagicSpellParent(f.Pair.Target.Object, f.Pair.F.Spells.Single(), f.Pair.F.Actor.Object);
		child.RemovalEffect(); f.Pair.TargetEffects.Clear(); parent.AddSpellEffect(loaded); loaded.ParentEffect = parent; f.Pair.TargetEffects.Add(parent); f.Pair.TargetEffects.Add(loaded); loaded.Login();
		Assert.AreEqual(child.ExpiryUtc, loaded.ExpiryUtc); Assert.IsTrue(loaded.CanControl(f.Pair.F.Actor.Object));
		f.Pair.F.Actor.Raise(x => x.OnQuit += null, f.Pair.F.Actor.Object);
		Assert.IsFalse(f.Pair.TargetEffects.Contains(loaded)); Assert.IsFalse(loaded.CanControl(f.Pair.F.Actor.Object));
	}

	private sealed class CompellingFixture
	{
		public SilenceFixture Base { get; } = new();
		public ExchangeFixture Pair => Base.Pair;
		public CharacterCommandManager Manager { get; } = new("No such command", PermissionLevel.Inaccessible);
		public SpellCompellingRegardEffect Child => Pair.TargetEffects.OfType<SpellCompellingRegardEffect>().Single();
		public CompellingFixture()
		{
			var f = Pair.F; var content = ArmageddonCompellingRegardSpellContent.Create(9, 1).Content;
			var row = content.SpellRow(1, 1, 1); row.Id = 1; row.EffectDurationExpressionId = 2; row.ResistingTraitDefinitionId = 1;
			row.ResistingDifficulty = (int)MudSharp.RPG.Checks.Difficulty.Normal; row.Definition = content.BuildDefinition(10, 1, 0).ToString();
			f.Spells.Clear(); f.Spells.Add(new MagicSpell(row, f.World.Object) { EffectDurationExpression = new TraitExpression("30*grade", f.World.Object) });
			f.Actor.Setup(x => x.CanSee(Pair.Target.Object, It.IsAny<PerceiveIgnoreFlags>())).Returns(true);
			Manager.Add("look", SpellCompulsionExecution.PermittedNativeCommand("look")!);
			var tree = new Mock<ICharacterCommandTree>(); tree.SetupGet(x => x.Commands).Returns(Manager); Pair.Target.SetupGet(x => x.CommandTree).Returns(tree.Object);
		}
	}
}
