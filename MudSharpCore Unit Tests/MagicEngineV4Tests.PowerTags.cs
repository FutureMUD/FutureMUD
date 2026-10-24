#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Accounts;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.Framework.Save;
using MudSharp.Magic;
using MudSharp.Magic.Casting;
using MudSharp.Magic.Powers;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Merits;

namespace MudSharp_Unit_Tests;

public partial class MagicEngineV4Tests
{
	[TestMethod]
	[DataRow(false, "psionic", false)]
	[DataRow(true, "psionic", true)]
	[DataRow(false, "flame", true)]
	[DataRow(false, "ice", false)]
	public void PowerTags_OnlyExplicitMatchingTagsSuppressExistingBaseAdmission(bool psionic, string suppressed, bool blocked)
	{
		var prog = CreateProg(0, true); var world = CreateGameworld([prog.Object]);
		var model = CreateConnectMindModel(0); var xml = XElement.Parse(model.Definition);
		xml.SetElementValue("IsPsionic", psionic); xml.Add(new XElement("PowerTags", new XElement("Tag", "flame"))); model.Definition = xml.ToString();
		var power = (MagicPowerBase)MagicPowerFactory.LoadPower(model, world.Object);
		var actor = CreateCharacter(1, world.Object); var suppression = new Mock<IMagicPowerTagSuppression>();
		suppression.SetupGet(x => x.SuppressedPowerTags).Returns([suppressed]); suppression.Setup(x => x.IsActiveFor(actor.Object)).Returns(true);
		actor.SetupGet(x => x.Effects).Returns([suppression.Object]);
		Assert.AreEqual(!blocked, power.CanInvokePower(actor.Object));
		prog.Verify(x => x.ExecuteBool(It.IsAny<object[]>()), Times.Exactly(blocked ? 0 : 1));
	}

	[TestMethod]
	public void PowerTags_BaseAdmissionRechecksAfterProgAndPreservesFalseProg()
	{
		var prog = CreateProg(0, true); var world = CreateGameworld([prog.Object]);
		var model = CreateConnectMindModel(0); var xml = XElement.Parse(model.Definition); xml.SetElementValue("IsPsionic", true); model.Definition = xml.ToString();
		var power = (MagicPowerBase)MagicPowerFactory.LoadPower(model, world.Object); var actor = CreateCharacter(1, world.Object);
		var effects = new List<IEffect>(); actor.SetupGet(x => x.Effects).Returns(effects);
		var suppression = new Mock<IMagicPowerTagSuppression>(); suppression.SetupGet(x => x.SuppressedPowerTags).Returns(["psionic"]);
		suppression.Setup(x => x.IsActiveFor(actor.Object)).Returns(true);
		prog.Setup(x => x.ExecuteBool(It.IsAny<object[]>())).Callback(() => effects.Add(suppression.Object)).Returns(true);
		Assert.IsFalse(power.CanInvokePower(actor.Object));
		effects.Clear(); prog.Setup(x => x.ExecuteBool(It.IsAny<object[]>())).Returns(false); Assert.IsFalse(power.CanInvokePower(actor.Object));
	}

	[TestMethod]
	public void PowerTags_BuilderTagsRoundTripAndRefuseMalformedIdentifiers()
	{
		var world = CreateGameworld(); world.SetupGet(x => x.SaveManager).Returns(Mock.Of<ISaveManager>());
		var power = (MagicPowerBase)MagicPowerFactory.LoadPower(CreateConnectMindModel(0), world.Object);
		var actor = CreateCharacter(1, world.Object); actor.SetupGet(x => x.OutputHandler).Returns(Mock.Of<MudSharp.PerceptionEngine.IOutputHandler>());
		Assert.IsTrue(power.BuildingCommand(actor.Object, new StringStack("tag elemental.fire")));
		Assert.IsFalse(power.BuildingCommand(actor.Object, new StringStack("tag two words")));
		Assert.IsFalse(power.BuildingCommand(actor.Object, new StringStack("tag bad/tag")));
		var saved = (XElement)power.GetType().GetMethod("SaveDefinition", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(power, null)!;
		var model = CreateConnectMindModel(0); model.Definition = saved.ToString();
		var loaded = (MagicPowerBase)MagicPowerFactory.LoadPower(model, world.Object);
		CollectionAssert.AreEquivalent(power.PowerTags.ToArray(), loaded.PowerTags.ToArray());
		Assert.IsTrue(loaded.PowerTags.Contains("elemental.fire"));
	}

	[TestMethod]
	[DataRow(1)] [DataRow(2)] [DataRow(3)] [DataRow(4)] [DataRow(5)] [DataRow(6)] [DataRow(7)]
	public void SilenceMind_AllGradesSuppressOnlyPsionicActivationUntilAbsoluteExpiry(int grade)
	{
		var f = new SilenceFixture(); using var clock = RuntimeClock.Push(f.Clock);
		var cast = f.Cast(grade); Assert.AreEqual(MagicCastingStatus.Succeeded, cast.Status, cast.Message);
		var child = f.Child; Assert.AreEqual(f.Clock.Now.AddSeconds(30 * grade), child.ExpiryUtc);
		Assert.IsTrue(child.IsActiveFor(f.Pair.Target.Object));
		var model = CreateConnectMindModel(0); var xml = XElement.Parse(model.Definition); xml.SetElementValue("IsPsionic", true); model.Definition = xml.ToString();
		var power = (MagicPowerBase)MagicPowerFactory.LoadPower(model, f.Pair.F.World.Object);
		Assert.IsFalse(power.CanInvokePower(f.Pair.Target.Object));
		xml.SetElementValue("IsPsionic", false); model.Definition = xml.ToString();
		Assert.IsTrue(((MagicPowerBase)MagicPowerFactory.LoadPower(model, f.Pair.F.World.Object)).CanInvokePower(f.Pair.Target.Object));
		f.Clock.Now = child.ExpiryUtc!.Value; Assert.IsFalse(child.IsActiveFor(f.Pair.Target.Object)); Assert.IsTrue(power.CanInvokePower(f.Pair.Target.Object));
	}

	[TestMethod]
	[DataRow("undead")] [DataRow("staff")] [DataRow("duplicate")] [DataRow("changed")]
	public void SilenceMind_InvalidOrChangedTargetsRefuseBeforePayment(string condition)
	{
		var f = new SilenceFixture(); using var clock = RuntimeClock.Push(f.Clock);
		if (condition == "undead") f.Pair.Target.SetupGet(x => x.Merits).Returns([f.Undead]);
		if (condition == "staff") f.Pair.Target.Setup(x => x.IsAdministrator(PermissionLevel.JuniorAdmin)).Returns(true);
		if (condition == "duplicate") Assert.AreEqual(MagicCastingStatus.Succeeded, f.Cast(1).Status);
		if (condition == "changed") f.Pair.F.Checkpoint = stage => { if (stage == "BeforePayment") f.Pair.Target.SetupGet(x => x.Merits).Returns([f.Undead]); };
		var balance = f.Pair.F.Balances[f.Pair.F.Resources[1]];
		Assert.AreEqual(MagicCastingStatus.Refused, f.Cast(2).Status); Assert.AreEqual(balance, f.Pair.F.Balances[f.Pair.F.Resources[1]]);
	}

	[TestMethod]
	public void SilenceMind_ResistedTargetIsUnaffectedAndSavedBindingCannotMoveBodies()
	{
		var resisted = new SilenceFixture(); using var clock = RuntimeClock.Push(resisted.Clock);
		resisted.SetResistance(Outcome.MajorPass);
		var cast = resisted.Cast(1); Assert.AreEqual(MagicCastingStatus.Failed, cast.Status);
		Assert.AreEqual(0, resisted.Pair.TargetEffects.OfType<SpellPowerTagSuppressionEffect>().Count());
		var f = new SilenceFixture(); Assert.AreEqual(MagicCastingStatus.Succeeded, f.Cast(2).Status);
		var child = f.Child; SpellPowerTagSuppressionEffect.InitialiseEffectType();
		var loaded = (SpellPowerTagSuppressionEffect)Effect.LoadEffect(child.SaveToXml(new Dictionary<IEffect, TimeSpan>()), f.Pair.Target.Object);
		var parent = new MagicSpellParent(f.Pair.Target.Object, f.Pair.F.Spells.Single(), f.Pair.F.Actor.Object);
		loaded.ParentEffect = parent; parent.AddSpellEffect(loaded); f.Pair.TargetEffects.Clear(); f.Pair.TargetEffects.Add(parent); f.Pair.TargetEffects.Add(loaded);
		Assert.AreEqual(child.ExpiryUtc, loaded.ExpiryUtc); Assert.IsTrue(loaded.IsActiveFor(f.Pair.Target.Object));
		f.Pair.TargetBody.SetupGet(x => x.Id).Returns(9999); Assert.IsFalse(loaded.IsActiveFor(f.Pair.Target.Object));
	}

	[TestMethod]
	public void SilenceMind_MountsAndRidersDoNotConferSuppressionImmunity()
	{
		var f = new SilenceFixture(); using var clock = RuntimeClock.Push(f.Clock);
		f.Pair.F.Actor.SetupGet(x => x.RidingMount).Returns(Mock.Of<ICharacter>());
		f.Pair.Target.SetupGet(x => x.Riders).Returns([Mock.Of<ICharacter>()]);
		Assert.AreEqual(MagicCastingStatus.Succeeded, f.Cast(1).Status);
		Assert.IsTrue(f.Child.IsActiveFor(f.Pair.Target.Object));
	}

	[TestMethod]
	[DataRow(true)] [DataRow(false)]
	public void SilenceMind_SeversMatchingReciprocalNativeContactOnly(bool matching)
	{
		var f = new SilenceFixture(); using var clock = RuntimeClock.Push(f.Clock);
		var contact = f.Pair.CasterEffects.OfType<ConnectMindEffect>().Single();
		if (!matching) contact.MindPower.BuildingCommand(f.Pair.F.Actor.Object, new StringStack("psionic"));
		Assert.AreEqual(MagicCastingStatus.Succeeded, f.Cast(1).Status);
		Assert.AreEqual(!matching, f.Pair.CasterEffects.Contains(contact)); Assert.AreEqual(!matching, f.Pair.TargetEffects.Contains(contact.ChildEffect));
	}

	private sealed class SilenceFixture
	{
		public ExchangeFixture Pair { get; } = new();
		public ReturnClock Clock { get; } = new();
		public IMerit Undead { get; } = Mock.Of<IMerit>(x => x.Id == 9);
		public SpellPowerTagSuppressionEffect Child => Pair.TargetEffects.OfType<SpellPowerTagSuppressionEffect>().Single();
		public SilenceFixture()
		{
			var f = Pair.F; Pair.TargetLocation = Pair.Source.Object; Pair.DestinationMembers.Clear(); Pair.SourceMembers.Add(Pair.Target.Object);
			f.World.SetupGet(x => x.Merits).Returns(MagicCastingFixture.Collection<IMerit>(() => [Undead]));
			f.Actor.Setup(x => x.TargetActorOrCorpse("traveller", It.IsAny<PerceiveIgnoreFlags>())).Returns(Pair.Target.Object);
			f.World.SetupGet(x => x.SaveManager).Returns(Mock.Of<ISaveManager>());
			foreach (var (actor, effects) in new[] { (f.Actor, Pair.CasterEffects), (Pair.Target, Pair.TargetEffects) })
			{
				void Add(IEffect effect) { effects.Add(effect); effect.InitialEffect(); }
				actor.Setup(x => x.AddEffect(It.IsAny<IEffect>())).Callback<IEffect>(Add);
				actor.Setup(x => x.AddEffect(It.IsAny<IEffect>(), It.IsAny<TimeSpan>())).Callback<IEffect, TimeSpan>((effect, _) => Add(effect));
				actor.Setup(x => x.RemoveEffect(It.IsAny<IEffect>(), It.IsAny<bool>())).Callback<IEffect, bool>((effect, fire) => { effects.Remove(effect); if (fire) effect.RemovalEffect(); });
			}
			SetResistance(Outcome.MajorFail);
			var contribution = ArmageddonMindSilenceSpellContent.Create(9, 1); var stock = contribution.Content;
			var row = stock.SpellRow(1, 1, 1); row.Id = 1; row.EffectDurationExpressionId = 2; row.ResistingTraitDefinitionId = 1; row.ResistingDifficulty = (int)Difficulty.Normal; row.Definition = stock.BuildDefinition(10, 1, 0).ToString();
			f.Spells.Clear(); f.Spells.Add(new MagicSpell(row, f.World.Object) { EffectDurationExpression = new TraitExpression("30*grade", f.World.Object) });
		}
		public void SetResistance(Outcome outcome) => Pair.F.Check.Setup(x => x.CheckAgainstAllDifficulties(It.Is<IPerceivableHaveTraits>(x => ReferenceEquals(x, Pair.Target.Object)),
			It.IsAny<Difficulty>(), It.IsAny<ITraitDefinition>(), It.IsAny<IPerceivable>(), It.IsAny<double>(), It.IsAny<TraitUseType>(), It.IsAny<(string, object)[]>()))
			.Returns(Enum.GetValues<Difficulty>().ToDictionary(x => x, _ => CheckOutcome.SimpleOutcome(CheckType.ResistMagicSpellCheck, outcome)));
		public MagicCastingResult Cast(int grade) => Pair.F.Service.Cast(new(Pair.F.Actor.Object, Pair.F.Earth.Id, 1, grade, false, "traveller", OriginId: Guid.NewGuid()));
	}
}
