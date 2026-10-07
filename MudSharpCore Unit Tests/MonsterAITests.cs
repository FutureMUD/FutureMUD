#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body.Needs;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Celestial;
using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.Climate;
using MudSharp.Combat;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Movement;
using MudSharp.NPC;
using MudSharp.NPC.AI;
using MudSharp.PerceptionEngine;
using MudSharp.TimeAndDate.Date;

namespace MudSharp_Unit_Tests;

[TestClass]
public class MonsterAITests
{
	[TestMethod]
	public void CreatureHierarchy_UsesSiblingPolicies_AndMonsterDoesNotChangeNeeds()
	{
		Assert.AreEqual(typeof(CreatureAIBase), typeof(AnimalAI).BaseType);
		Assert.AreEqual(typeof(CreatureAIBase), typeof(MonsterAI).BaseType);
		var f = new Fixture();
		var needs = f.Actor.Object.NeedsModel;
		Assert.IsInstanceOfType(needs, typeof(NoNeedsModel));
		Assert.IsNull(f.Ai.PreyRejection(f.Actor.Object, f.Target.Object), "A satiated monster may hunt an inedible target.");
		f.Ai.HandleEvent(EventType.TrapCaughtPrey, f.Actor.Object, f.Target.Object, Guid.NewGuid().ToString());
		Assert.AreSame(needs, f.Actor.Object.NeedsModel);
		var animal = (AnimalAI)typeof(AnimalAI).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null,
			[typeof(MudSharp.Models.ArtificialIntelligence), typeof(IFuturemud)], null)!.Invoke([
			new MudSharp.Models.ArtificialIntelligence { Id = 9, Name = "Animal", Type = "Animal", Definition = "<Definition><Feeding type='Predator'/><Hunting enabled='true'><People>Eligible</People></Hunting></Definition>" }, f.World.Object]);
		Assert.IsNotNull(animal.PreyRejection(f.Actor.Object, f.Target.Object));
	}

	[TestMethod]
	public void EmptyMonster_RemainsNonProactive_WhileSelfDefenceIsSeparate()
	{
		var f = new Fixture(motive: "");
		Assert.IsTrue(f.Ai.IsReadyToBeUsed);
		Assert.IsFalse(f.Ai.CountsAsAggressive);
		Assert.AreEqual(MonsterMotive.None, f.Ai.SelectMotive(f.Actor.Object, f.Target.Object));
		f.Ai.ActivityWindow.Times.Add(TimeOfDay.Night);
		f.Target.SetupGet(x => x.CombatTarget).Returns(f.Actor.Object);
		Assert.AreEqual(MonsterMotive.SelfDefence, f.Ai.SelectMotive(f.Actor.Object, f.Target.Object));
	}

	[TestMethod]
	public void ScheduledHunt_ClosingWindowDuringDelay_PreventsEngagement()
	{
		var f = new Fixture();
		f.Ai.ActivityWindow.Times.Add(TimeOfDay.Night);
		f.Room.SetupGet(x => x.CurrentTimeOfDay).Returns(TimeOfDay.Night);
		Assert.IsTrue(f.Ai.HandleEvent(EventType.CharacterEnterRoomWitness, f.Target.Object, f.Room.Object, null!, f.Actor.Object));
		var hunt = f.Effects.OfType<MonsterIntentEffect>().Single();
		var delay = f.Effects.OfType<CreatureEngagementDelay>().Single();
		f.Room.SetupGet(x => x.CurrentTimeOfDay).Returns(TimeOfDay.Morning);
		delay.ExpireEffect();
		Assert.AreEqual(AnimalHuntPhase.Abandoned, hunt.Phase);
		f.Actor.Verify(x => x.Engage(It.IsAny<IPerceiver>(), It.IsAny<bool>()), Times.Never);
		f.Ai.HandleEvent(EventType.TenSecondTick, f.Actor.Object);
		Assert.AreEqual(0, f.Effects.OfType<MonsterIntentEffect>().Count());
		Assert.IsTrue(f.Effects.OfType<MonsterStateEffect>().Single().CooldownUntil > DateTime.UtcNow);
	}

	[TestMethod]
	public void ScheduledHunt_RepeatedEvents_EngageOnceThroughNativeApi()
	{
		var f = new Fixture();
		for (var i = 0; i < 4; i++) f.Ai.HandleEvent(EventType.CharacterEnterRoomWitness, f.Target.Object, f.Room.Object, null!, f.Actor.Object);
		Assert.AreEqual(1, f.Effects.OfType<MonsterIntentEffect>().Count());
		var delay = f.Effects.OfType<CreatureEngagementDelay>().Single();
		delay.ExpireEffect();
		f.Actor.Verify(x => x.Engage(f.Target.Object, false), Times.Once);
		Assert.AreSame(f.Ai, f.Effects.OfType<MonsterIntentEffect>().Single().Ai);
	}

	[TestMethod]
	public void SubmergedMonster_AcquiresVisibleSurfaceTargetWithoutARemoteScanReceipt()
	{
		var f = new Fixture();
		f.Actor.SetupGet(x => x.RoomLayer).Returns(RoomLayer.Underwater);
		f.Room.Setup(x => x.LayerCharacters(RoomLayer.Underwater)).Returns([f.Actor.Object]);
		f.Actor.SetupGet(x => x.SeenTargets).Returns([]);
		f.Ai.HandleEvent(EventType.CharacterEnterRoomWitness, f.Target.Object, f.Room.Object, null!, f.Actor.Object);
		Assert.AreEqual(f.Target.Object.Id, f.Effects.OfType<MonsterIntentEffect>().Single().TargetId);
		f.Effects.OfType<CreatureEngagementDelay>().Single().ExpireEffect();
		f.Actor.Verify(x => x.Engage(f.Target.Object, false), Times.Once);
	}

	[TestMethod]
	public void OwnedLayerPreparation_CompletesWithoutWandering_OnlyInsideActivityWindow()
	{
		var f = new Fixture();
		f.Actor.SetupGet(x => x.RoomLayer).Returns(RoomLayer.Underwater);
		var path = new FollowingMultiLayerPath(f.Actor.Object, [], RoomLayer.Underwater, RoomLayer.Underwater)
			{ PathingOwner = f.Ai };
		f.Effects.Add(path);
		f.Ai.ActivityWindow.Times.Add(TimeOfDay.Night);
		f.Room.SetupGet(x => x.CurrentTimeOfDay).Returns(TimeOfDay.Morning);
		f.Ai.HandleEvent(EventType.FiveSecondTick, f.Actor.Object);
		Assert.IsTrue(f.Effects.Contains(path));
		f.Room.SetupGet(x => x.CurrentTimeOfDay).Returns(TimeOfDay.Night);
		f.Ai.HandleEvent(EventType.FiveSecondTick, f.Actor.Object);
		Assert.IsFalse(f.Effects.Contains(path));
		Assert.AreEqual(0, f.Effects.OfType<MonsterIntentEffect>().Count());
	}

	[TestMethod]
	public void ClosingWindow_DeferredDoorCallback_ClearsOwnedWorkBeforeItCanAct()
	{
		var f = new Fixture();
		f.Ai.ActivityWindow.Times.Add(TimeOfDay.Night);
		f.Room.SetupGet(x => x.CurrentTimeOfDay).Returns(TimeOfDay.Night);
		f.Ai.HandleEvent(EventType.CharacterEnterRoomWitness, f.Target.Object, f.Room.Object, null!, f.Actor.Object);
		var path = new FollowingPath(f.Actor.Object, []) { PathingOwner = f.Ai };
		var door = new BreakDownDoor(f.Actor.Object, Mock.Of<IRoomExit>()) { PathingEpisode = path };
		f.Effects.Add(path); f.Effects.Add(door);
		f.Room.SetupGet(x => x.CurrentTimeOfDay).Returns(TimeOfDay.Morning);
		f.Ai.HandleEvent(EventType.CommandDelayExpired, f.Actor.Object, "open");
		Assert.AreEqual(0, f.Effects.OfType<MonsterIntentEffect>().Count());
		Assert.AreEqual(0, f.Effects.OfType<CreatureEngagementDelay>().Count());
		Assert.IsFalse(f.Effects.Contains(path)); Assert.IsFalse(f.Effects.Contains(door));
		f.Actor.Verify(x => x.Engage(It.IsAny<IPerceiver>(), It.IsAny<bool>()), Times.Never);
	}

	[TestMethod]
	public void HomeReturn_UnreachableRouteBacksOff_AndSearchCoversOutboundAndPursuitRanges()
	{
		var f = new Fixture();
		f.Ai.Hunting.PursuitRange = 17;
		Assert.IsTrue(f.Ai.ReturnSearchRange >= f.Ai.MovementRange + 17);
		var homeRoom = new Mock<IRoom>(); homeRoom.SetupGet(x => x.Id).Returns(99);
		var home = new NpcHomeBaseEffect(f.Actor.Object); home.SetHomeRoom(homeRoom.Object); f.Effects.Add(home);
		f.World.SetupGet(x => x.Rooms).Returns(Collection(f.Room.Object, homeRoom.Object));
		f.Room.Setup(x => x.ExitsFor(It.IsAny<IPerceiver>(), It.IsAny<bool>())).Returns([]);
		var method = typeof(MonsterAI).GetMethod("GetPath", BindingFlags.Instance | BindingFlags.NonPublic)!;
		var result = ((IRoom?, IEnumerable<IRoomExit>))method.Invoke(f.Ai, [f.Actor.Object])!;
		Assert.AreSame(homeRoom.Object, result.Item1); Assert.IsFalse(result.Item2.Any());
		var state = f.Effects.OfType<MonsterStateEffect>().Single();
		Assert.IsTrue(state.ReturnRetryUntil > DateTime.UtcNow);
		result = ((IRoom?, IEnumerable<IRoomExit>))method.Invoke(f.Ai, [f.Actor.Object])!;
		Assert.IsNull(result.Item1);
		MonsterStateEffect.InitialiseEffectType();
		Assert.AreEqual(state.ReturnRetryUntil, ((MonsterStateEffect)Effect.LoadEffect(state.SaveToXml([]), f.Actor.Object)).ReturnRetryUntil);
	}

	[TestMethod]
	public void TargetPolicy_AlwaysChecksObservationExclusionsAlliesAndPhysicalPermission()
	{
		var f = new Fixture();
		f.Ai.Hunting.ConfidenceBias = 100;
		f.Ai.Hunting.ExcludedRaces.Add(8);
		Assert.AreEqual("excluded lineage", f.Ai.PreyRejection(f.Actor.Object, f.Target.Object));
		f.Ai.Hunting.ExcludedRaces.Clear();
		f.Actor.Setup(x => x.CanSee(f.Target.Object, It.IsAny<PerceiveIgnoreFlags>())).Returns(false);
		Assert.AreEqual("not currently observed", f.Ai.PreyRejection(f.Actor.Object, f.Target.Object));
		f.Actor.Setup(x => x.CanSee(f.Target.Object, It.IsAny<PerceiveIgnoreFlags>())).Returns(true);
		f.Actor.Setup(x => x.Race.SameRace(f.TargetRace.Object)).Returns(true);
		Assert.AreEqual("socially protected", f.Ai.PreyRejection(f.Actor.Object, f.Target.Object));
		f.Actor.Setup(x => x.Race.SameRace(f.TargetRace.Object)).Returns(false);
		f.Target.SetupGet(x => x.RoomLayer).Returns(RoomLayer.InAir);
		Assert.AreEqual("unreachable layer", f.Ai.PreyRejection(f.Actor.Object, f.Target.Object));
		f.Target.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		f.Actor.SetupGet(x => x.Race.CombatSettings).Returns(new RacialCombatSettings { CanAttack = false });
		Assert.AreEqual("hunter's race cannot attack", f.Ai.PreyRejection(f.Actor.Object, f.Target.Object));
	}

	[TestMethod]
	public void NeedsMotive_RequiresActualHungerAndEdiblePrey()
	{
		var f = new Fixture("<Feeding>Needs</Feeding>", "Hunger");
		Assert.AreEqual(MonsterMotive.None, f.Ai.SelectMotive(f.Actor.Object, f.Target.Object));
		var needs = new Mock<INeedsModel>(); needs.SetupGet(x => x.Status).Returns(NeedsResult.Hungry);
		f.Actor.SetupGet(x => x.NeedsModel).Returns(needs.Object);
		Assert.AreEqual("inedible prey for hunger motive", f.Ai.PreyRejection(f.Actor.Object, f.Target.Object));
		f.Actor.SetupGet(x => x.Race.CanEatCorpses).Returns(true);
		f.TargetRace.SetupGet(x => x.CorpseModel.CreateCorpse).Returns(true);
		f.Actor.Setup(x => x.Race.CanEatCorpseMaterial(It.IsAny<MudSharp.Form.Material.ISolid>())).Returns(true);
		Assert.IsNull(f.Ai.PreyRejection(f.Actor.Object, f.Target.Object));
	}

	[TestMethod]
	public void ActivityWindow_CombinesLocalTimeSeasonAndSelectedMoon()
	{
		var f = new Fixture();
		var winter = new Mock<ISeason>(); winter.SetupGet(x => x.Id).Returns(1); winter.SetupGet(x => x.SeasonGroup).Returns("Winter");
		f.World.SetupGet(x => x.Seasons).Returns(Collection(winter.Object)); f.Room.Setup(x => x.CurrentSeason(f.Actor.Object)).Returns(winter.Object);
		var moon = new Mock<ICelestialObject>(); moon.SetupGet(x => x.Id).Returns(4);
		var lunar = moon.As<ILunarPhase>(); var phase = Enum.GetValues<MoonPhase>()[0]; lunar.Setup(x => x.CurrentPhase()).Returns(phase);
		f.World.SetupGet(x => x.CelestialObjects).Returns(Collection(moon.Object));
		f.Room.SetupGet(x => x.Zone.Celestials).Returns([moon.Object]);
		var rule = f.Ai.ActivityWindow; rule.Times.UnionWith([TimeOfDay.Dusk, TimeOfDay.Night]); rule.Seasons.Add("winter"); rule.MoonId = 4; rule.Phases.Add(phase);
		f.Room.SetupGet(x => x.CurrentTimeOfDay).Returns(TimeOfDay.Night);
		Assert.IsNull(rule.InactiveReason(f.Actor.Object));
		f.Room.SetupGet(x => x.CurrentTimeOfDay).Returns(TimeOfDay.Morning);
		Assert.AreEqual("outside the active time bands", rule.InactiveReason(f.Actor.Object));
		f.Room.SetupGet(x => x.CurrentTimeOfDay).Returns(TimeOfDay.Dusk);
		f.Room.SetupGet(x => x.Zone.Celestials).Returns([]);
		Assert.AreEqual("the selected moon is not present in this zone", rule.InactiveReason(f.Actor.Object));
	}

	[TestMethod]
	public void ActivityWindow_UsesFictionalShortAndIntercalaryMonths()
	{
		var f = new Fixture(); var calendar = new Mock<ICalendar>(); calendar.SetupGet(x => x.Id).Returns(3); calendar.SetupGet(x => x.Weekdays).Returns(["day"]);
		var shortMonth = new MonthDefinition { Alias = "short", FullName = "Short", ShortName = "S", NormalDays = 4 };
		var festival = new MonthDefinition { Alias = "festival", FullName = "Festival", ShortName = "F", NormalDays = 2 };
		calendar.SetupGet(x => x.Months).Returns([shortMonth]); calendar.SetupGet(x => x.Intercalaries).Returns([new IntercalaryMonth { Month = festival }]);
		var months = new List<Month> { new(shortMonth, 77), new(festival, 77) }; var year = new Year(months, 77, calendar.Object);
		f.World.SetupGet(x => x.Calendars).Returns(Collection(calendar.Object));
		var rule = f.Ai.ActivityWindow; rule.CalendarId = 3; rule.Months.Add("festival"); rule.FirstDay = 1; rule.LastDay = 2;
		calendar.SetupGet(x => x.CurrentDate).Returns(new MudDate(calendar.Object, 2, 77, months[1], year, false));
		Assert.IsNull(rule.InactiveReason(f.Actor.Object));
		calendar.SetupGet(x => x.CurrentDate).Returns(new MudDate(calendar.Object, 4, 77, months[0], year, false));
		Assert.AreEqual("outside the active calendar dates", rule.InactiveReason(f.Actor.Object));
		rule.Months.Clear(); rule.FirstDay = 5; rule.LastDay = 9;
		Assert.AreEqual("outside the active calendar dates", rule.InactiveReason(f.Actor.Object));
	}

	[DataTestMethod]
	[DataRow("<Calendar>999</Calendar>")]
	[DataRow("<Moon>999</Moon>")]
	[DataRow("<ConditionProg>999</ConditionProg>")]
	[DataRow("<Time>InvalidBand</Time>")]
	[DataRow("<FirstDay>2</FirstDay><LastDay>1</LastDay>")]
	public void ActivityWindow_MissingOrInvalidBindings_FailClosedAndRoundTrip(string xml)
	{
		var f = new Fixture(); var rule = MonsterActivityWindow.Load(XElement.Parse($"<ActivityWindow>{xml}</ActivityWindow>"));
		Assert.IsNotNull(rule.InactiveReason(f.Actor.Object));
		Assert.IsNotNull(MonsterActivityWindow.Load(rule.Save()).InactiveReason(f.Actor.Object));
	}

	[TestMethod]
	public void Provocation_UsesOneObservedReceipt_AndRespectsDefenceWindow()
	{
		var f = new Fixture(motive: "Provocation"); f.Ai.ActivityWindow.Times.Add(TimeOfDay.Night);
		f.Room.SetupGet(x => x.CurrentTimeOfDay).Returns(TimeOfDay.Morning);
		for (var i = 0; i < 3; i++) f.Ai.HandleEvent(EventType.EngagedInCombat, f.Target.Object, f.Actor.Object);
		Assert.AreEqual(1, f.Effects.OfType<MonsterStateEffect>().Count());
		Assert.AreEqual(MonsterMotive.Provocation, f.Ai.SelectMotive(f.Actor.Object, f.Target.Object));
		var state = f.Effects.OfType<MonsterStateEffect>().Single(); var xml = state.SaveToXml([]);
		xml.Element("Effect")!.Element("ProvokedUntil")!.Value = DateTime.UtcNow.AddHours(-1).ToString("O");
		MonsterStateEffect.InitialiseEffectType(); f.Effects.Clear(); f.Effects.Add(Effect.LoadEffect(xml, f.Actor.Object));
		Assert.AreEqual(MonsterMotive.None, f.Ai.SelectMotive(f.Actor.Object, f.Target.Object));
		var bounded = new Fixture("<DefenceUsesWindow>true</DefenceUsesWindow>", "Provocation"); bounded.Ai.ActivityWindow.Times.Add(TimeOfDay.Night);
		bounded.Room.SetupGet(x => x.CurrentTimeOfDay).Returns(TimeOfDay.Morning);
		bounded.Ai.HandleEvent(EventType.EngagedInCombat, bounded.Target.Object, bounded.Actor.Object);
		Assert.AreEqual(MonsterMotive.None, bounded.Ai.SelectMotive(bounded.Actor.Object, bounded.Target.Object));
	}

	[TestMethod]
	public void Guardian_WarnsOnce_ThenStartsAnIntentAfterWarning()
	{
		var f = new Fixture(motive: "Territory"); var home = new NpcHomeBaseEffect(f.Actor.Object); home.SetHomeRoom(f.Room.Object); f.Effects.Add(home);
		f.Ai.HandleEvent(EventType.CharacterEnterRoomWitness, f.Target.Object, f.Room.Object, null!, f.Actor.Object);
		var state = f.Effects.OfType<MonsterStateEffect>().Single(); Assert.AreEqual(f.Target.Object.Id, state.WarningTargetId);
		Assert.AreEqual(0, f.Effects.OfType<MonsterIntentEffect>().Count());
		var xml = state.SaveToXml([]); xml.Element("Effect")!.Element("WarningUntil")!.Value = DateTime.UtcNow.AddSeconds(-1).ToString("O");
		MonsterStateEffect.InitialiseEffectType(); f.Effects.Remove(state); f.Effects.Add(Effect.LoadEffect(xml, f.Actor.Object));
		f.Ai.HandleEvent(EventType.CharacterEnterRoomWitness, f.Target.Object, f.Room.Object, null!, f.Actor.Object);
		Assert.AreEqual(MonsterMotive.Territory, f.Effects.OfType<MonsterIntentEffect>().Single().Motive);
	}

	[TestMethod]
	public void SavedIntent_LoadsWithoutReplay_AndExpiredStateGetsCooldown()
	{
		var f = new Fixture(); var intent = new MonsterIntentEffect(f.Actor.Object, f.Ai, f.Target.Object, MonsterMotive.Scheduled);
		intent.RecordVenomDelivery(); intent.Phase = AnimalHuntPhase.Shadowing; var xml = intent.SaveToXml([]);
		MonsterIntentEffect.InitialiseEffectType(); var restored = (MonsterIntentEffect)Effect.LoadEffect(xml, f.Actor.Object);
		Assert.AreEqual(intent.Deadline, restored.Deadline); Assert.AreEqual(intent.LastSeen, restored.LastSeen);
		Assert.AreEqual(intent.Motive, restored.Motive); Assert.IsTrue(restored.VenomDelivered);
		f.Actor.Verify(x => x.Engage(It.IsAny<IPerceiver>(), It.IsAny<bool>()), Times.Never);
		f.World.Verify(x => x.TryGetCharacter(It.IsAny<long>(), It.IsAny<bool>()), Times.Never);
		xml.Element("Effect")!.Element("Deadline")!.Value = DateTime.UtcNow.AddDays(-1).ToString("O");
		f.Effects.Add(Effect.LoadEffect(xml, f.Actor.Object)); f.Ai.HandleEvent(EventType.NPCOnGameLoadFinished, f.Actor.Object);
		Assert.AreEqual(0, f.Effects.OfType<MonsterIntentEffect>().Count());
		Assert.IsTrue(f.Effects.OfType<MonsterStateEffect>().Single().CooldownUntil > DateTime.UtcNow);
		f.Actor.Verify(x => x.Engage(It.IsAny<IPerceiver>(), It.IsAny<bool>()), Times.Never);
	}

	[TestMethod]
	public void SharedDefinition_StateAndCooldownBelongToEachOwner()
	{
		var f = new Fixture(); var other = new Mock<ICharacter>(); other.SetupGet(x => x.Gameworld).Returns(f.World.Object);
		var one = new MonsterStateEffect(f.Actor.Object, f.Ai.Id); var two = new MonsterStateEffect(other.Object, f.Ai.Id);
		one.Provoke(20, TimeSpan.FromMinutes(1)); one.Finish(TimeSpan.FromMinutes(2), "lost");
		Assert.AreEqual(DateTime.MinValue, two.CooldownUntil); Assert.AreEqual(0, two.ProvokerId);
		MonsterStateEffect.InitialiseEffectType(); var loaded = (MonsterStateEffect)Effect.LoadEffect(one.SaveToXml([]), f.Actor.Object);
		Assert.AreEqual(one.CooldownUntil, loaded.CooldownUntil); Assert.AreEqual("lost", loaded.LastEndReason);
	}

	[TestMethod]
	public void AfterKillFeeding_WithNoNeeds_ConsumesOnlyConfiguredBites()
	{
		var f = new Fixture("<Feeding>AfterKill</Feeding><FeedingBites>2</FeedingBites>");
		var item = new Mock<IGameItem>(); item.SetupGet(x => x.Id).Returns(80); item.SetupGet(x => x.Location).Returns(f.Room.Object);
		item.Setup(x => x.ShallowAccessibleItems(f.Actor.Object)).Returns([item.Object]);
		var corpse = new Mock<ICorpse>(); corpse.SetupGet(x => x.Parent).Returns(item.Object); item.Setup(x => x.GetItemType<ICorpse>()).Returns(corpse.Object);
		f.Room.Setup(x => x.LayerGameItems(RoomLayer.GroundLevel)).Returns([item.Object]);
		f.Actor.SetupGet(x => x.Race.BiteWeight).Returns(1); f.Actor.Setup(x => x.CanEat(corpse.Object, 1)).Returns((true, ""));
		f.Target.SetupGet(x => x.Corpse).Returns(corpse.Object);
		f.Target.SetupGet(x => x.State).Returns(CharacterState.Dead);
		f.Effects.Add(new MonsterIntentEffect(f.Actor.Object, f.Ai, f.Target.Object, MonsterMotive.Scheduled));
		f.Ai.HandleEvent(EventType.CharacterDiesWitness, f.Target.Object, f.Actor.Object);
		var state = f.Effects.OfType<MonsterStateEffect>().Single();
		Assert.AreEqual("target died", state.LastEndReason);
		Assert.AreEqual(0, f.Effects.OfType<MonsterIntentEffect>().Count());
		f.Ai.Motives.Clear();
		for (var i = 0; i < 5; i++) f.Ai.HandleEvent(EventType.CharacterEnterRoomWitness, f.Target.Object, f.Room.Object, null!, f.Actor.Object);
		f.Actor.Verify(x => x.Eat(corpse.Object, 1, null), Times.Exactly(2)); Assert.AreEqual(0, state.BitesRemaining);
		Assert.IsInstanceOfType(f.Actor.Object.NeedsModel, typeof(NoNeedsModel));
	}

	[TestMethod]
	public void Detach_RemovesOnlyItsOwnIntentDelayAndPaths()
	{
		var f = new Fixture(); var other = new MonsterAI(new MudSharp.Models.ArtificialIntelligence { Id = 2, Name = "other", Definition = "<Definition/>" }, f.World.Object);
		var own = new FollowingPath(f.Actor.Object, []) { PathingOwner = f.Ai };
		var foreign = new FollowingPath(f.Actor.Object, []) { PathingOwner = other };
		var delay = new CreatureEngagementDelay(f.Actor.Object, f.Ai.Id, _ => Assert.Fail("Detached delay replayed"));
		f.Effects.AddRange([own, foreign, delay, new MonsterIntentEffect(f.Actor.Object, f.Ai, f.Target.Object, MonsterMotive.Scheduled)]);
		f.Ai.Detach(f.Actor.Object);
		CollectionAssert.AreEqual(new List<IEffect> { foreign }, f.Effects);
		Assert.IsNotNull(MonsterAI.AttachmentError([f.Ai], other));
		Assert.IsNull(MonsterAI.AttachmentError([], f.Ai));
	}

	[TestMethod]
	public void MonsterXml_RoundTripsAllConfigurationWithoutRuntimeState()
	{
		var f = new Fixture("<Feeding>AfterKill</Feeding><SameRaceAllies>false</SameRaceAllies><DefenceUsesWindow>true</DefenceUsesWindow><TrapProvokes>true</TrapProvokes><ReturnHome>false</ReturnHome><GuardRange>2</GuardRange><WarningSeconds>12</WarningSeconds><ProvocationSeconds>81</ProvocationSeconds><CooldownSeconds>44</CooldownSeconds><FeedingBites>7</FeedingBites><FeedingSeconds>60</FeedingSeconds>");
		f.Ai.ActivityWindow.Times.UnionWith([TimeOfDay.Night, TimeOfDay.Dusk]); f.Ai.Hunting.Followup = AnimalHuntFollowup.VenomWithdrawal;
		var xml = f.Ai.SaveDefinition(); var loaded = new MonsterAI(new MudSharp.Models.ArtificialIntelligence { Id = 8, Name = "clone", Type = "Monster", Definition = xml.ToString() }, f.World.Object);
		Assert.IsTrue(XNode.DeepEquals(xml, loaded.SaveDefinition())); Assert.AreEqual(0, f.Effects.Count);
		Assert.IsTrue(loaded.HandlesEvent(EventType.TrapCaughtPrey)); Assert.IsFalse(loaded.HandlesEvent(EventType.HourTick));
	}

	private static All<T> Collection<T>(params T[] values) where T : class, IFrameworkItem
	{
		var result = new All<T>(); foreach (var value in values) result.Add(value); return result;
	}
	private sealed class Fixture
	{
		public Mock<IFuturemud> World { get; } = new() { DefaultValue = DefaultValue.Mock };
		public Mock<INPC> Actor { get; } = new() { DefaultValue = DefaultValue.Mock };
		public Mock<ICharacter> Target { get; } = new() { DefaultValue = DefaultValue.Mock };
		public Mock<IRace> TargetRace { get; } = new() { DefaultValue = DefaultValue.Mock };
		public Mock<IRoom> Room { get; } = new() { DefaultValue = DefaultValue.Mock };
		public List<IEffect> Effects { get; } = [];
		public MonsterAI Ai { get; }
		public Fixture(string options = "", string motive = "Scheduled")
		{
			var always = new Mock<IFutureProg>(); always.SetupGet(x => x.Id).Returns(1);
			always.Setup(x => x.ExecuteBool(It.IsAny<bool>(), It.IsAny<object[]>())).Returns(true);
			always.Setup(x => x.ExecuteBool(It.IsAny<object[]>())).Returns(true);
			var never = new Mock<IFutureProg>(); never.SetupGet(x => x.Id).Returns(2);
			World.SetupGet(x => x.AlwaysTrueProg).Returns(always.Object); World.SetupGet(x => x.AlwaysFalseProg).Returns(never.Object); World.SetupGet(x => x.AlwaysOneProg).Returns(always.Object);
			World.SetupGet(x => x.FutureProgs).Returns(Collection(always.Object, never.Object));
			World.SetupGet(x => x.Calendars).Returns(Collection<ICalendar>()); World.SetupGet(x => x.CelestialObjects).Returns(Collection<ICelestialObject>()); World.SetupGet(x => x.Seasons).Returns(Collection<ISeason>());
			Room.SetupGet(x => x.Id).Returns(7); Room.SetupGet(x => x.Location).Returns(Room.Object); Room.SetupGet(x => x.RouteDefinition).Returns((IRouteRoomDefinition)null!);
			Room.SetupGet(x => x.Characters).Returns([Actor.Object, Target.Object]); Room.Setup(x => x.LayerCharacters(RoomLayer.GroundLevel)).Returns([Actor.Object, Target.Object]);
			Room.SetupGet(x => x.GameItems).Returns([]); Room.Setup(x => x.LayerGameItems(RoomLayer.GroundLevel)).Returns([]);
			Actor.SetupGet(x => x.Location).Returns(Room.Object); Target.SetupGet(x => x.Location).Returns(Room.Object);
			Actor.SetupGet(x => x.Id).Returns(10); Target.SetupGet(x => x.Id).Returns(20);
			Actor.SetupGet(x => x.Gameworld).Returns(World.Object); Target.SetupGet(x => x.Gameworld).Returns(World.Object);
			Actor.SetupGet(x => x.State).Returns(CharacterState.Awake); Target.SetupGet(x => x.State).Returns(CharacterState.Awake);
			Actor.SetupGet(x => x.NeedsModel).Returns(new NoNeedsModel());
			Actor.Setup(x => x.CanSee(Target.Object, It.IsAny<PerceiveIgnoreFlags>())).Returns(true); Actor.Setup(x => x.CanEngage(Target.Object)).Returns(true);
			Actor.SetupGet(x => x.Race.CombatSettings).Returns(new RacialCombatSettings { CanAttack = true });
			Actor.SetupGet(x => x.CombatSettings.AttackHelpless).Returns(true);
			Actor.SetupGet(x => x.Combat).Returns((ICombat)null!); Actor.SetupGet(x => x.CombatTarget).Returns((IPerceiver)null!);
			Actor.SetupGet(x => x.Movement).Returns((IMovement)null!); Actor.SetupGet(x => x.RidingMount).Returns((ICharacter)null!); Actor.SetupGet(x => x.Corpse).Returns((ICorpse)null!);
			Actor.SetupGet(x => x.GroupAI).Returns((MudSharp.NPC.AI.Groups.IGroupAI)null!);
			TargetRace.SetupGet(x => x.Id).Returns(8); Target.SetupGet(x => x.Race).Returns(TargetRace.Object); Target.SetupGet(x => x.CombatTarget).Returns((IPerceiver)null!);
			Actor.SetupGet(x => x.PositionState).Returns(PositionStanding.Instance); Target.SetupGet(x => x.PositionState).Returns(PositionStanding.Instance);
			Actor.SetupGet(x => x.MaximumStamina).Returns(100); Actor.SetupGet(x => x.CurrentStamina).Returns(100);
			Actor.Setup(x => x.HealthStrategy.CurrentHealthPercentage(Actor.Object)).Returns(1);
			World.SetupGet(x => x.Rooms).Returns(Collection(Room.Object)); World.Setup(x => x.TryGetCharacter(20, true)).Returns(Target.Object);
			Actor.SetupGet(x => x.Effects).Returns(() => Effects); Actor.Setup(x => x.AddEffect(It.IsAny<IEffect>())).Callback<IEffect>(Effects.Add);
			Actor.Setup(x => x.AddEffect(It.IsAny<IEffect>(), It.IsAny<TimeSpan>())).Callback<IEffect, TimeSpan>((effect, _) => Effects.Add(effect));
			Actor.Setup(x => x.RemoveEffect(It.IsAny<IEffect>(), It.IsAny<bool>())).Callback<IEffect, bool>((effect, _) => Effects.Remove(effect));
			SetupEffects<IEffect>(); SetupEffects<CreaturePursuitEffect>(); SetupEffects<MonsterIntentEffect>(); SetupEffects<MonsterStateEffect>(); SetupEffects<CreatureEngagementDelay>(); SetupEffects<FollowingPath>(); SetupEffects<BreakDownDoor>(); SetupEffects<NpcHomeBaseEffect>();
			Ai = new MonsterAI(new MudSharp.Models.ArtificialIntelligence { Id = 1, Name = "Monster", Type = "Monster", Definition = $"<Definition><Hunting enabled='true'><Engage>0</Engage><Abandon>0</Abandon></Hunting><Monster version='1'>{(motive.Length == 0 ? "" : $"<Motive>{motive}</Motive>")}{options}</Monster></Definition>" }, World.Object);
			Actor.SetupGet(x => x.AIs).Returns([Ai]);
		}
		private void SetupEffects<T>() where T : class, IEffect
		{
			Actor.Setup(x => x.EffectsOfType<T>(It.IsAny<Predicate<T>>())).Returns<Predicate<T>>(predicate => Effects.OfType<T>().Where(x => predicate?.Invoke(x) != false).ToList());
			Actor.Setup(x => x.CombinedEffectsOfType<T>()).Returns(() => Effects.OfType<T>().ToList());
			Actor.Setup(x => x.RemoveAllEffects<T>(It.IsAny<Predicate<T>>(), It.IsAny<bool>())).Returns<Predicate<T>, bool>((predicate, _) => Effects.RemoveAll(x => x is T effect && predicate?.Invoke(effect) != false) > 0);
		}
	}
}
