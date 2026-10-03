#nullable enable

using System.Reflection;
using System.Runtime.CompilerServices;
using Moq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Commands.Modules;
using MudSharp.Construction;
using MudSharp.Economy;
using MudSharp.Economy.Currency;
using MudSharp.Economy.Estates;
using MudSharp.Effects;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.Health;
using MudSharp.Planes;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Law;
using MudSharp.RPG.Law.PatrolStrategies;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private static string LegacyRemainsScenario(LegacyRemainsReader input) => input.MissingOwner ? "missing-owner-" :
		input.MissingBody ? input.FinalDeath ? "missing-final-" : "missing-" :
		input.CurrentBody ? input.FinalDeath ? "final-current-" : "current-" : input.FinalDeath ? "final-inactive-" : "";

	private static void VerifyLegacyRemainsRuntimeBoundaries(LegacyRemainsReader input, NativeRuntime native,
		Mock<IGameItem> parent, GameItemComponent component, bool unresolved)
	{
		var source = new Mock<ICell>(); parent.SetupGet(x => x.Location).Returns(source.Object);
		parent.SetupGet(x => x.InInventoryOf).Returns((IBody)null!); parent.SetupGet(x => x.LocationLevelPerceivable).Returns(parent.Object);
		parent.SetupGet(x => x.BasePlanarPresence).Returns(PlanarPresenceDefinition.DefaultMaterial(1));
		parent.SetupGet(x => x.SurfaceLiquidState).Returns(new SurfaceLiquidState(native.World));
		var material = new Mock<ISolid>(); material.SetupGet(x => x.HeatDamagePoint).Returns(100.0);
		parent.SetupGet(x => x.Material).Returns(material.Object); source.Setup(x => x.CurrentTemperature(It.IsAny<IPerceiver>())).Returns(120.0);
		native.WorldMock.Setup(x => x.GetStaticConfiguration("EnvironmentalExposureMode")).Returns("Enabled");
		native.WorldMock.SetupGet(x => x.Actors).Returns(new All<ICharacter>()); native.WorldMock.SetupGet(x => x.Cells).Returns(new All<ICell>());
		native.WorldMock.SetupGet(x => x.DefaultPlane).Returns((IPlane)null!);
		var clock = new DateTime(2026, 10, 3, 0, 0, 0, DateTimeKind.Utc);
		var exposure = new EnvironmentalExposureService(native.World, () => clock);
		exposure.Track(parent.Object); exposure.Refresh();
		if (unresolved || !input.Corpse)
		{
			Require(exposure.ActiveCount == 1, "Unresolved remains or severed-part ordinary item exposure was dropped.");
		}
		else
		{
			var registered = (ConditionalWeakTable<IBody, IGameItem>)typeof(EnvironmentalExposureService)
				.GetField("_remains", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(exposure)!;
			Require(registered.TryGetValue(((ICorpse)component).OriginalBody, out var registeredItem) && ReferenceEquals(registeredItem, parent.Object),
				"Resolved corpse did not register its exact body for anatomy-based exposure.");
		}
		if (unresolved)
		{
			var eater = native.Actor.Body;
			var eligibility = input.Corpse ? eater.CanEat((ICorpse)component, 1.0) : eater.CanEat((ISeveredBodypart)component, 1.0);
			var execution = input.Corpse ? eater.Eat((ICorpse)component, 1.0, null) : eater.Eat((ISeveredBodypart)component, 1.0, null);
			Require(!eligibility.Success && !execution.Success && eligibility.ErrorMessage.Contains("original body") &&
				(input.Corpse ? ((ICorpse)component).EatenWeight : ((ISeveredBodypart)component).EatenWeight) == 0,
				"Unresolved anatomy was consumed by the real loaded character body.");
		}
		if (input.Corpse)
		{
			var corpse = (ICorpse)component; var targeter = new Mock<ITarget>();
			targeter.Setup(x => x.TargetCorpse("corpse", PerceiveIgnoreFlags.None)).Returns(corpse);
			Require((targeter.Object.TargetActorOrCorpseBody("corpse") is null) == unresolved,
				"Physical corpse target did not follow resolved exact anatomy.");
			Require(ReferenceEquals(corpse.GetOriginalCharacterWithMatchingBody(), input.FinalDeath && !unresolved && input.CurrentBody ? native.Actor : null),
				"Legacy actor target redirected to a different current body.");
			if (unresolved)
			{
				Require(!native.Actor.Body.CanGive(parent.Object, corpse) && !native.Actor.Body.CanGive(Mock.Of<ICurrency>(), corpse, 1, true),
					"Unresolved corpse accepted item/currency transfer.");
				native.Actor.Body.Give(parent.Object, corpse); native.Actor.Body.Give(Mock.Of<ICurrency>(), corpse, 1, true);
				var implantItem = new Mock<IGameItem>(); var implant = new Mock<IImplant>(); var surgeon = new Mock<ICharacter>(); var surgeonBody = new Mock<IBody>();
				implant.SetupGet(x => x.TargetBodypart).Returns(Mock.Of<IBodypart>()); implantItem.Setup(x => x.GetItemType<IImplant>()).Returns(implant.Object);
				surgeon.SetupGet(x => x.Body).Returns(surgeonBody.Object); surgeon.Setup(x => x.TargetHeldItem("implant")).Returns(implantItem.Object);
				surgeon.Setup(x => x.TargetCorpse("corpse", PerceiveIgnoreFlags.None)).Returns(corpse);
				typeof(MudSharp.Character.Character).Assembly.GetType("MudSharp.Commands.Modules.HealthModule")!
					.GetMethod("InstallImplant", BindingFlags.NonPublic | BindingFlags.Static)!.Invoke(null, [surgeon.Object, "installimplant implant corpse"]);
				surgeonBody.Verify(x => x.Take(implantItem.Object), Times.Never);
			}
			if (unresolved || input.FinalDeath && !input.CurrentBody)
			{
				VerifyRemainsMorgueRefusal(native, parent, source);
			}
		}
		else
		{
			var part = (ISeveredBodypart)component; var wound = part.Wounds.Single();
			parent.Setup(x => x.GetSeverityFor(It.IsAny<IWound>())).Returns(WoundSeverity.Severe);
			parent.Setup(x => x.VisibleWounds(native.Actor, WoundExaminationType.Look)).Returns(part.Wounds);
			Require(native.Actor.Body.LookWoundsText(parent.Object).Contains("cut"), "Persisted part look-wounds path failed.");
			Require(wound.Describe(WoundExaminationType.Omniscient, Outcome.MajorPass).Contains("cut"), "Persisted part wound description failed.");
			_ = wound.TextForAdminWoundsCommand;
			Require(wound.BleedStatus == BleedStatus.Bleeding, "Inspection changed the persisted bleeding state.");
		}
		Console.WriteLine($"ARM03-runtime-{LegacyRemainsScenario(input)}{(input.Corpse ? "corpse" : "part")}=passed " +
			$"exposure-refresh-registration eating:{(unresolved ? "refused" : "not-run")} " +
			$"physical-and-actor-targets:{(input.Corpse ? "checked" : "not-applicable")} " +
			$"gift-and-implant-conservation:{(input.Corpse && unresolved ? "checked" : "not-run")} " +
			$"morgue-recovery-refusal:{(input.Corpse && (unresolved || input.FinalDeath && !input.CurrentBody) ? "checked" : "not-run")} " +
			$"persisted-part-wound-inspection:{(!input.Corpse ? "checked" : "not-applicable")}");
	}

	private static void VerifyRemainsMorgueRefusal(NativeRuntime native, Mock<IGameItem> parent, Mock<ICell> source)
	{
		var estateReads = native.WorldMock.Invocations.Count(x => x.Method.Name == "get_Estates");
		var zone = new Mock<IEconomicZone>(MockBehavior.Strict);
		Require(!MorgueService.TryIntakeCorpse(zone.Object, parent.Object, out var estate) && estate is null,
			"Unresolved or different-body corpse was admitted to morgue custody.");
		var report = new Mock<ICorpseRecoveryReport>(); report.SetupGet(x => x.Corpse).Returns(parent.Object);
		report.SetupGet(x => x.SourceCell).Returns(source.Object); report.SetupGet(x => x.EconomicZone).Returns(zone.Object);
		report.SetupProperty(x => x.Status, CorpseRecoveryReportStatus.Assigned);
		var patrol = new Mock<IPatrol>(); var leader = new Mock<ICharacter>(); leader.Setup(x => x.ColocatedWith(parent.Object)).Returns(true);
		patrol.SetupGet(x => x.PatrolLeader).Returns(leader.Object); patrol.SetupProperty(x => x.PatrolPhase, PatrolPhase.Patrol);
		patrol.SetupProperty(x => x.ActiveCorpseRecoveryReport, report.Object);
		var strategy = (CorpseRecoveryPatrolStrategy)RuntimeHelpers.GetUninitializedObject(typeof(CorpseRecoveryPatrolStrategy));
		typeof(PatrolStrategyBase).GetField("<Gameworld>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(strategy, native.World);
		strategy.HandlePatrolTick(patrol.Object);
		Require(native.WorldMock.Invocations.Count(x => x.Method.Name == "get_Estates") == estateReads,
			"Refused remains accessed a survivor's estate state.");
		Require(report.Object.Status == CorpseRecoveryReportStatus.Assigned && ReferenceEquals(patrol.Object.ActiveCorpseRecoveryReport, report.Object),
			"Refused recovery report changed status or custody assignment.");
		report.Verify(x => x.MarkCompleted(), Times.Never); report.Verify(x => x.MarkFailed(), Times.Never); patrol.Verify(x => x.ConcludePatrol(), Times.Never);
		parent.Verify(x => x.AddEffect(It.IsAny<IEffect>()), Times.Never); source.Verify(x => x.Extract(parent.Object), Times.Never);
		parent.VerifySet(x => x.RoomLayer = It.IsAny<RoomLayer>(), Times.Never);
	}
}
