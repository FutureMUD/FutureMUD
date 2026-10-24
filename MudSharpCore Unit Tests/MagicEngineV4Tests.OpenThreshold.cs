#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body.Traits;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Database;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.GameItems;
using MudSharp.Magic;
using MudSharp.Magic.Casting;
using MudSharp.Magic.Lifecycle;

namespace MudSharp_Unit_Tests;

public partial class MagicEngineV4Tests
{
	[TestMethod]
	public void OpenThreshold_RepeatedPreparationPreservesQuoteAndTypedSelector()
	{
		var f = new ThresholdFixture(); using var clock = RuntimeClock.Push(f.Clock);
		var prepare = typeof(MagicCastingService).GetMethod("Prepare", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!;
		var intent = new MagicCastingIntent(f.Pair.F.Actor.Object, f.Pair.F.Earth.Id, 1, 1, false, "traveller traveller", OriginId: Guid.NewGuid());
		var first = prepare.Invoke(f.Pair.F.Service, [intent, null])!;
		var spell = (MagicSpell)first.GetType().GetProperty("Spell")!.GetValue(first)!;
		var second = prepare.Invoke(f.Pair.F.Service, [intent, spell])!;
		var left = (string)first.GetType().GetProperty("Configuration")!.GetValue(first)!;
		var right = (string)second.GetType().GetProperty("Configuration")!.GetValue(second)!;
		Assert.AreEqual(left, right, "Preparing a casting copy changed the installed spell configuration.");
		var leftTarget = (MudSharp.Magic.Vancian.SpellTargetResolution)first.GetType().GetProperty("Target")!.GetValue(first)!;
		var rightTarget = (MudSharp.Magic.Vancian.SpellTargetResolution)second.GetType().GetProperty("Target")!.GetValue(second)!;
		Assert.IsTrue(leftTarget.Parameters.SequenceEqual(rightTarget.Parameters), $"Selectors differ: {leftTarget.Parameters[0].Item} / {rightTarget.Parameters[0].Item}");
		Assert.IsTrue((bool)typeof(MagicCastingService).GetMethod("Equivalent", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!.Invoke(null, [first, second])!);
	}
	[TestMethod]
	[DataRow(1)] [DataRow(2)] [DataRow(3)] [DataRow(4)] [DataRow(5)] [DataRow(6)] [DataRow(7)]
	public void OpenThreshold_AllGradesCreateOnePairedExitAndExpireWithoutRemovingOccupants(int grade)
	{
		var f = new ThresholdFixture(); using var clock = RuntimeClock.Push(f.Clock);
		var result = f.Cast(grade); Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		var child = f.Child; var exit = f.Exits.Single(); Assert.AreEqual(f.Clock.Now.AddSeconds(120 * grade), child.ExpiryUtc);
		Assert.AreSame(f.Pair.Destination.Object, exit.RoomExitFor(f.Pair.Source.Object).Destination);
		Assert.AreSame(f.Pair.Source.Object, exit.RoomExitFor(f.Pair.Destination.Object).Destination);
		Assert.IsTrue(((IAdmittedMagicPortalExit)exit).AllowsTraversal(f.Pair.F.Actor.Object, exit.RoomExitFor(f.Pair.Source.Object)));
		f.Clock.Now = child.ExpiryUtc!.Value;
		Assert.IsFalse(((IAdmittedMagicPortalExit)exit).AllowsTraversal(f.Pair.F.Actor.Object, exit.RoomExitFor(f.Pair.Source.Object)));
		child.RemovalEffect(); Assert.AreEqual(0, f.Exits.Count); Assert.AreEqual(0, f.Pair.Moves);
		Assert.AreSame(f.Pair.Source.Object, f.Pair.CasterLocation); Assert.AreSame(f.Pair.Destination.Object, f.Pair.TargetLocation);
	}

	[TestMethod]
	[DataRow("restricted")] [DataRow("temporary")] [DataRow("same")] [DataRow("recognition")] [DataRow("changed")]
	public void OpenThreshold_InvalidOrChangedEndpointRefusesBeforePayment(string condition)
	{
		var f = new ThresholdFixture(); using var clock = RuntimeClock.Push(f.Clock);
		if (condition == "restricted") f.Pair.Destination.Setup(x => x.IsA(f.Pair.Restricted)).Returns(true);
		if (condition == "temporary") f.Pair.Destination.SetupGet(x => x.Temporary).Returns(true);
		if (condition == "same") f.Pair.TargetLocation = f.Pair.Source.Object;
		if (condition == "recognition") f.Trace.Dubs.Clear();
		if (condition == "changed") f.Pair.F.Checkpoint = stage => { if (stage == "BeforePayment") f.Pair.TargetLocation = f.Pair.Source.Object; };
		Assert.AreEqual(MagicCastingStatus.Refused, f.Cast(1).Status); Assert.AreEqual(100.0, f.Pair.F.Balances[f.Pair.F.Resources[1]]); Assert.AreEqual(0, f.Exits.Count);
	}

	[TestMethod]
	public void OpenThreshold_LiveWardAndOverlayChangesRefuseTraversalOfExistingPortal()
	{
		var f = new ThresholdFixture(); using var clock = RuntimeClock.Push(f.Clock); f.Cast(1); var exit = f.Exits.Single();
		var roomExit = exit.RoomExitFor(f.Pair.Source.Object); var admission = (IAdmittedMagicPortalExit)exit;
		Assert.IsTrue(admission.AllowsTraversal(f.Pair.F.Actor.Object, roomExit));
		var ward = new Mock<IMagicInterdictionEffect>(); ward.SetupGet(x => x.Coverage).Returns(MagicInterdictionCoverage.Incoming);
		ward.Setup(x => x.ShouldInterdict(f.Pair.F.Actor.Object, f.Pair.F.School)).Returns(true);
		f.Pair.Destination.Setup(x => x.EffectsOfType<IMagicInterdictionEffect>(It.IsAny<Predicate<IMagicInterdictionEffect>>())).Returns([ward.Object]);
		Assert.IsFalse(admission.AllowsTraversal(f.Pair.F.Actor.Object, roomExit));
		f.Pair.Destination.Setup(x => x.EffectsOfType<IMagicInterdictionEffect>(It.IsAny<Predicate<IMagicInterdictionEffect>>())).Returns([]);
		Mock.Get(f.Pair.Destination.Object.CurrentOverlay).SetupGet(x => x.Id).Returns(999);
		Assert.IsFalse(admission.AllowsTraversal(f.Pair.F.Actor.Object, roomExit));
	}

	[TestMethod]
	public void OpenThreshold_SavedPortalRetainsPairIdentityAndDoesNotReopenAfterExpiry()
	{
		var f = new ThresholdFixture(); using var clock = RuntimeClock.Push(f.Clock); f.Cast(2); var child = f.Child;
		SpellOpenThresholdEffect.InitialiseEffectType(); var saved = child.SaveToXml(new Dictionary<IEffect, TimeSpan>());
		child.RemovalEffect(); var loaded = (SpellOpenThresholdEffect)Effect.LoadEffect(saved, f.Pair.Source.Object);
		var parent = new MagicSpellParent(f.Pair.Source.Object, f.Pair.F.Spells.Single(), f.Pair.F.Actor.Object); parent.AddSpellEffect(loaded); loaded.ParentEffect = parent;
		f.Effects.Clear(); f.Effects.Add(parent); f.Effects.Add(loaded); loaded.Login();
		Assert.AreEqual(child.PortalInstanceId, loaded.PortalInstanceId); Assert.AreEqual(child.ExpiryUtc, loaded.ExpiryUtc); Assert.AreEqual(1, f.Exits.Count);
		loaded.RemovalEffect(); f.Clock.Now = loaded.ExpiryUtc!.Value; loaded.Login(); Assert.AreEqual(0, f.Exits.Count);
	}

	[TestMethod]
	[DataRow(false)] [DataRow(true)]
	public void OpenThreshold_ArcaneMarkUseCommitsBeforePortalAndUncertaintyNeverReplays(bool uncertain)
	{
		var f = new ThresholdFixture(); using var clock = RuntimeClock.Push(f.Clock); var mark = f.AddMark(); var debits = 0;
		var committer = new Mock<ISpellMarkedRoomUseService>(); f.Pair.F.World.SetupGet(x => x.SpellMarkedRoomUses).Returns(committer.Object);
		committer.Setup(x => x.Debit(500, f.Pair.F.Body.Object.Id, It.IsAny<Guid>(), 81, f.Pair.Destination.Object.Id, 2, It.IsAny<Func<bool>>()))
			.Callback<long, long, Guid, long, long, int, Func<bool>>((_, _, _, _, _, _, current) =>
			{ Assert.IsTrue(current()); Assert.AreEqual(0, f.Exits.Count); debits++; if (uncertain) throw new InvalidOperationException("Commit acknowledgement lost."); });
		var result = f.Cast(1, "mark sigil"); Assert.AreEqual(1, debits);
		if (uncertain)
		{ Assert.AreEqual(MagicCastingStatus.NeedsReview, result.Status); Assert.AreEqual(0, f.Exits.Count); Assert.AreEqual(2, mark.RemainingUses); }
		else { Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message); Assert.AreEqual(1, f.Exits.Count); Assert.AreEqual(1, mark.RemainingUses); }
		Assert.AreEqual(MagicCastingStatus.Refused, f.Pair.F.Service.Cast(new(f.Pair.F.Actor.Object, f.Pair.F.Earth.Id, 1, 1, false, "mark sigil", OriginId: result.OperationId)).Status);
		Assert.AreEqual(1, debits);
	}

	[TestMethod]
	public void OpenThreshold_PersistedMarkDebitKeepsOtherEffectsAndRefusesStaleUseOrCustody()
	{
		using var db = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>().UseInMemoryDatabase(Guid.NewGuid().ToString(), x => x.EnableNullChecks(false)).Options);
		var identity = Guid.NewGuid(); var xml = new XElement("Effects", new XElement("Effect", new XElement("Type", "MagicSpellParent"),
			new XElement("Effect", new XElement("Identity", identity), new XElement("Spell", 81), new XElement("Children", new XElement("Effect", new XElement("Type", "SpellMagicTag"),
				new XElement("Effect", new XElement("Tag", ArmageddonSimpleStockContent.ArcaneMarkTag), new XElement("ItemId", 500), new XElement("SavedRoomId", 200), new XElement("RemainingUses", 2)))))));
		xml.Add(new XElement("Unrelated", "keep")); var item = new MudSharp.Models.GameItem { Id = 500, EffectData = xml.ToString() };
		db.GameItems.Add(item); db.BodiesGameItems.Add(new() { BodyId = 91, GameItemId = 500 }); db.SaveChanges();
		SpellMarkedRoomDebit.Apply(db, 500, 91, identity, 81, 200, 2); Assert.AreEqual("keep", XElement.Parse(item.EffectData).Element("Unrelated")!.Value);
		Assert.ThrowsExactly<InvalidOperationException>(() => SpellMarkedRoomDebit.Apply(db, 500, 91, identity, 81, 200, 2));
		item.ContainerId = 33; Assert.ThrowsExactly<InvalidOperationException>(() => SpellMarkedRoomDebit.Apply(db, 500, 91, identity, 81, 200, 1));
	}

	private sealed class ThresholdFixture
	{
		public TraceFixture Trace { get; } = new(1);
		public ExchangeFixture Pair => Trace.Pair;
		public ReturnClock Clock { get; } = new();
		public List<IEffect> Effects { get; } = [];
		public List<IExit> Exits { get; } = [];
		public SpellOpenThresholdEffect Child => Effects.OfType<SpellOpenThresholdEffect>().Single();
		public ThresholdFixture()
		{
			var f = Pair.F; Mock.Get(Pair.Source.Object.CurrentOverlay).SetupGet(x => x.Id).Returns(301); Mock.Get(Pair.Destination.Object.CurrentOverlay).SetupGet(x => x.Id).Returns(302);
			Pair.Source.SetupGet(x => x.Effects).Returns(Effects);
			void Add(IEffect effect) { Effects.Add(effect); effect.InitialEffect(); }
			Pair.Source.Setup(x => x.AddEffect(It.IsAny<IEffect>())).Callback<IEffect>(Add);
			Pair.Source.Setup(x => x.AddEffect(It.IsAny<IEffect>(), It.IsAny<TimeSpan>())).Callback<IEffect, TimeSpan>((effect, _) => Add(effect));
			Pair.Source.Setup(x => x.RemoveEffect(It.IsAny<IEffect>(), It.IsAny<bool>())).Callback<IEffect, bool>((effect, fire) => { Effects.Remove(effect); if (fire) effect.RemovalEffect(); });
			var exits = new Mock<IExitManager>(); exits.Setup(x => x.RegisterTransientExit(It.IsAny<IExit>())).Callback<IExit>(Exits.Add);
			exits.Setup(x => x.UnregisterTransientExit(It.IsAny<IExit>())).Callback<IExit>(exit => Exits.Remove(exit)); f.World.SetupGet(x => x.ExitManager).Returns(exits.Object);
			var content = ArmageddonOpenThresholdSpellContent.Create(Pair.Restricted.Id, Pair.Populated.Id).Content;
			var row = content.SpellRow(1, 1, 1); row.Id = 1; row.EffectDurationExpressionId = 2; row.Definition = content.BuildDefinition(10, 1, 0).ToString();
			f.Spells.Clear(); f.Spells.Add(new MagicSpell(row, f.World.Object) { EffectDurationExpression = new TraitExpression("120*grade", f.World.Object) });
		}
		public MagicCastingResult Cast(int grade, string selector = "traveller traveller") => Pair.F.Service.Cast(new(Pair.F.Actor.Object, Pair.F.Earth.Id, 1, grade, false, selector, OriginId: Guid.NewGuid()));
		public SpellMagicTagEffect AddMark()
		{
			var f = Pair.F; var item = new Mock<IGameItem>(); var effects = new List<IEffect>();
			item.SetupGet(x => x.Id).Returns(500); item.SetupGet(x => x.Gameworld).Returns(f.World.Object); item.SetupGet(x => x.Effects).Returns(effects);
			item.SetupGet(x => x.InInventoryOf).Returns(f.Body.Object); item.SetupGet(x => x.TrueLocations).Returns([Pair.Source.Object]);
			item.SetupGet(x => x.OwnershipReference).Returns(new ItemOwnershipReference("Character", 100)); item.Setup(x => x.IsOwnedBy(f.Actor.Object)).Returns(true);
			item.Setup(x => x.HasKeyword("sigil", f.Actor.Object, true)).Returns(true); Pair.Items.Add(item.Object);
			var content = ArmageddonReturnTetherSpellContent.Create().Content; var row = content.SpellRow(1, 1, 1); row.Id = 81;
			var definition = content.BuildDefinition(10, 1, 0); definition.SetElementValue("StockIdentity", "arm.spell.arcane_mark"); row.Definition = definition.ToString();
			var spell = new MagicSpell(row, f.World.Object); var parent = new MagicSpellParent(item.Object, spell, f.Actor.Object);
			var mark = new SpellMagicTagEffect(item.Object, parent, ArmageddonSimpleStockContent.ArcaneMarkTag, "glyph", savedRoomId: Pair.Destination.Object.Id, remainingUses: 2);
			parent.AddSpellEffect(mark); effects.Add(parent); effects.Add(mark); return mark;
		}
	}
}
