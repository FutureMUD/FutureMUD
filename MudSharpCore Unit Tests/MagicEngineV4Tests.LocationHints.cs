#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body.Traits;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Framework.Scheduling;
using MudSharp.Health;
using MudSharp.Magic;
using MudSharp.Magic.SpellEffects;
using MudSharp.Framework;

namespace MudSharp_Unit_Tests;

public partial class MagicEngineV4Tests
{
	private static void InstallHint(ReturnFixture f)
	{
		var row = ((MagicSpell)f.Pair.F.Spells.Single()).SnapshotModel();
		var xml = XElement.Parse(row.Definition);
		xml.Element("Effects")!.ReplaceNodes(new XElement("Effect", new XAttribute("type", "delayedlocationhint"),
			new XElement("BaseDelay", 1.875), new XElement("DelayPerGrade", 1.875), new XElement("BaseRange", 10), new XElement("RangePerGrade", 10)));
		row.Definition = xml.ToString(); f.Pair.F.Spells.Clear();
		f.Pair.F.Spells.Add(new MagicSpell(row, f.Pair.F.World.Object) { EffectDurationExpression = new TraitExpression("0", f.Pair.F.World.Object) });
	}

	[TestMethod]
	[DataRow(1)] [DataRow(7)]
	public void LocationHint_PreparationRecordsGradeDeadlineAndHintsOnceWithoutMoving(int grade)
	{
		var f = new ReturnFixture(); using var clock = RuntimeClock.Push(f.Clock); InstallHint(f);
		var entry = PreparedEntry(); entry.NativeSpellGrade = grade;
		var substance = PreparedSubstance(f.Pair.F, entry);
		Assert.AreEqual("", string.Join("; ", substance.ReadinessErrors));
		Assert.IsTrue(MagicalExposure.Scripted(f.Pair.F.Actor.Object, substance, 1, DrugVector.Ingested));
		var child = f.Pair.CasterEffects.OfType<SpellDelayedLocationHintEffect>().Single();
		Assert.AreEqual(f.Clock.Now.AddSeconds(1.875 * (grade + 1)), child.ExpiryUtc);
		Assert.AreEqual(10 * (grade + 1), child.MaximumRange);
		var messages = f.Pair.F.Messages.Count;
		child.CheckHint(); Assert.AreEqual(messages, f.Pair.F.Messages.Count);
		f.Clock.Now = child.ExpiryUtc!.Value;
		child.CheckHint(); child.CheckHint();
		Assert.AreEqual(messages + 1, f.Pair.F.Messages.Count);
		Assert.AreEqual(0, f.Pair.Moves);
		Assert.IsFalse(f.Pair.CasterEffects.Contains(child));
	}

	[TestMethod]
	[DataRow("logout")] [DataRow("death")] [DataRow("dispel")]
	public void LocationHint_CancellationNeverEmitsOrMoves(string reason)
	{
		var f = new ReturnFixture(); using var clock = RuntimeClock.Push(f.Clock); InstallHint(f);
		MagicalExposure.Scripted(f.Pair.F.Actor.Object, PreparedSubstance(f.Pair.F, PreparedEntry()), 1, DrugVector.Ingested);
		var child = f.Pair.CasterEffects.OfType<SpellDelayedLocationHintEffect>().Single();
		child.Login();
		if (reason == "logout") f.Pair.F.Actor.Raise(x => x.OnQuit += null!, f.Pair.F.Actor.Object);
		else if (reason == "death") f.Pair.F.Actor.Raise(x => x.OnDeath += null!, f.Pair.F.Actor.Object);
		else f.Pair.F.Actor.Object.RemoveEffect(child, true);
		var messages = f.Pair.F.Messages.Count; f.Clock.Now = child.ExpiryUtc!.Value; child.CheckHint();
		Assert.AreEqual(messages, f.Pair.F.Messages.Count); Assert.AreEqual(0, f.Pair.Moves);
	}

	[TestMethod]
	[DataRow(true)] [DataRow(false)]
	public void LocationHint_MixedTeleportPayloadRemembersLocationAtItsOwnSlot(bool teleportFirst)
	{
		var f = new WeightedFixture(false);
		f.Pair.F.Actor.Setup(x => x.AddEffect(It.IsAny<IEffect>())).Callback<IEffect>(x => f.Pair.CasterEffects.Add(x));
		f.Pair.F.Actor.Setup(x => x.AddEffect(It.IsAny<IEffect>(), It.IsAny<TimeSpan>())).Callback<IEffect, TimeSpan>((x, _) => f.Pair.CasterEffects.Add(x));
		var hint = f.Pair.F.NewSpell(50, "Prepared location hint", "<Effect type='delayedlocationhint'><BaseDelay>1</BaseDelay><DelayPerGrade>1</DelayPerGrade><BaseRange>10</BaseRange><RangePerGrade>10</RangePerGrade></Effect>");
		hint.EffectDurationExpression = new TraitExpression("0", f.Pair.F.World.Object);
		Assert.IsTrue(hint.BuildingCommand(f.Pair.F.Actor.Object, new StringStack("grades fixture")));
		var origin = f.Pair.CasterLocation.Id;
		var entries = teleportFirst ? new[] { PreparedEntry(), PreparedEntry(50) } : new[] { PreparedEntry(50), PreparedEntry() };
		var substance = PreparedSubstance(f.Pair.F, entries);
		Assert.AreEqual("", string.Join("; ", substance.ReadinessErrors));
		Assert.IsTrue(MagicalExposure.Scripted(f.Pair.F.Actor.Object, substance, 1, DrugVector.Ingested));
		Assert.AreEqual(1, f.Moves);
		var child = f.Pair.CasterEffects.OfType<SpellDelayedLocationHintEffect>().Single();
		Assert.AreEqual(teleportFirst ? f.Pair.CasterLocation.Id : origin, child.AnchorRoomId);
	}

	[TestMethod]
	public void LocationHint_SavedChildPreservesAnchorBodyRangeAndAbsoluteDeadline()
	{
		var f = new ReturnFixture(); using var clock = RuntimeClock.Push(f.Clock); InstallHint(f);
		MagicalExposure.Scripted(f.Pair.F.Actor.Object, PreparedSubstance(f.Pair.F, PreparedEntry()), 1, DrugVector.Ingested);
		var child = f.Pair.CasterEffects.OfType<SpellDelayedLocationHintEffect>().Single();
		SpellDelayedLocationHintEffect.InitialiseEffectType();
		var saved = child.SaveToXml(new Dictionary<IEffect, TimeSpan>());
		f.Clock.Now = f.Clock.Now.AddSeconds(3);
		var loaded = (SpellDelayedLocationHintEffect)Effect.LoadEffect(saved, f.Pair.F.Actor.Object);
		Assert.AreEqual(child.AnchorRoomId, loaded.AnchorRoomId); Assert.AreEqual(child.BodyId, loaded.BodyId);
		Assert.AreEqual(child.MaximumRange, loaded.MaximumRange); Assert.AreEqual(child.ExpiryUtc, loaded.ExpiryUtc);
	}

	[TestMethod]
	public void LocationHint_RejectsRangesBeyondNativePathBudget()
	{
		var f = new ReturnFixture(); InstallHint(f);
		var effect = (DelayedLocationHintEffect)f.Pair.F.Spells.Single().SpellEffects.Single();
		var xml = effect.SaveToXml(); xml.Element("BaseRange")!.Value = "141";
		Assert.IsNotNull(new DelayedLocationHintEffect(xml, f.Pair.F.Spells.Single()).DefinitionError);
	}
}
