#nullable enable
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Accounts;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Economy.Property;
using MudSharp.Effects.Concrete;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Health;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Law;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MudSharp_Unit_Tests.RPG.Law;

[TestClass]
public class AutomaticCrimeExtensionsTests
{
	[TestMethod]
	public void CheckMurderForDeath_ResponsibleWound_ReportsMurderAtDeathLocation()
	{
		Mock<IFuturemud> gameworld = CreateGameworld(out Mock<ILegalAuthority> authority, out _, out _);
		Mock<ICell> deathLocation = CreateCell(1L, CreateZone(10L).Object);
		Mock<ICharacter> attacker = CreateCharacter(20L, gameworld.Object, deathLocation.Object);
		Mock<ICharacter> victim = CreateCharacter(30L, gameworld.Object, deathLocation.Object);
		Mock<IGameItem> weapon = CreateItem(40L, "knife");
		Mock<IWound> wound = new();
		wound.SetupGet(x => x.ActorOrigin).Returns(attacker.Object);
		wound.SetupGet(x => x.ToolOrigin).Returns(weapon.Object);
		wound.SetupGet(x => x.Severity).Returns(WoundSeverity.Horrifying);
		wound.SetupGet(x => x.DamageType).Returns(DamageType.Piercing);
		wound.SetupGet(x => x.RealTimeOfWound).Returns(DateTime.UtcNow.AddMinutes(-5));
		wound.SetupGet(x => x.IsFriendlyWound).Returns(false);
		Mock<IBody> body = new();
		body.SetupGet(x => x.Wounds).Returns(new[] { wound.Object });
		victim.SetupGet(x => x.Body).Returns(body.Object);

		AutomaticCrimeExtensions.CheckMurderForDeath(victim.Object);

		authority.Verify(x => x.CheckPossibleCrime(attacker.Object, CrimeTypes.Murder, victim.Object, weapon.Object,
			It.Is<string>(s => s.Contains("automatic=death") && s.Contains("maxseverity=Horrifying")),
			It.IsAny<IEnumerable<ICharacter>>(), false, deathLocation.Object), Times.Once);
	}

	[TestMethod]
	public void CheckMurderForDeath_FriendlyWound_DoesNotReportMurderByDefault()
	{
		Mock<IFuturemud> gameworld = CreateGameworld(out Mock<ILegalAuthority> authority, out _, out _);
		Mock<ICell> deathLocation = CreateCell(1L, CreateZone(10L).Object);
		Mock<ICharacter> attacker = CreateCharacter(20L, gameworld.Object, deathLocation.Object);
		Mock<ICharacter> victim = CreateCharacter(30L, gameworld.Object, deathLocation.Object);
		Mock<IWound> wound = CreateMurderWound(attacker.Object, null!, WoundSeverity.Horrifying,
			DateTime.UtcNow.AddMinutes(-5), true);
		Mock<IBody> body = new();
		body.SetupGet(x => x.Wounds).Returns(new[] { wound.Object });
		victim.SetupGet(x => x.Body).Returns(body.Object);

		AutomaticCrimeExtensions.CheckMurderForDeath(victim.Object);

		authority.Verify(x => x.CheckPossibleCrime(It.IsAny<ICharacter>(), CrimeTypes.Murder,
			It.IsAny<ICharacter>(), It.IsAny<IGameItem>(), It.IsAny<string>(),
			It.IsAny<IEnumerable<ICharacter>>(), It.IsAny<bool>(), It.IsAny<ICell>()), Times.Never);
	}

	[TestMethod]
	public void CheckMurderForDeath_OldWound_DoesNotReportMurder()
	{
		Mock<IFuturemud> gameworld = CreateGameworld(out Mock<ILegalAuthority> authority, out _, out _);
		Mock<ICell> deathLocation = CreateCell(1L, CreateZone(10L).Object);
		Mock<ICharacter> attacker = CreateCharacter(20L, gameworld.Object, deathLocation.Object);
		Mock<ICharacter> victim = CreateCharacter(30L, gameworld.Object, deathLocation.Object);
		Mock<IWound> wound = CreateMurderWound(attacker.Object, null!, WoundSeverity.Horrifying,
			DateTime.UtcNow.AddHours(-3), false);
		Mock<IBody> body = new();
		body.SetupGet(x => x.Wounds).Returns(new[] { wound.Object });
		victim.SetupGet(x => x.Body).Returns(body.Object);

		AutomaticCrimeExtensions.CheckMurderForDeath(victim.Object);

		authority.Verify(x => x.CheckPossibleCrime(It.IsAny<ICharacter>(), CrimeTypes.Murder,
			It.IsAny<ICharacter>(), It.IsAny<IGameItem>(), It.IsAny<string>(),
			It.IsAny<IEnumerable<ICharacter>>(), It.IsAny<bool>(), It.IsAny<ICell>()), Times.Never);
	}

	[TestMethod]
	public void CheckMurderForDeath_UnknownWoundTime_DoesNotReportMurder()
	{
		Mock<IFuturemud> gameworld = CreateGameworld(out Mock<ILegalAuthority> authority, out _, out _);
		Mock<ICell> deathLocation = CreateCell(1L, CreateZone(10L).Object);
		Mock<ICharacter> attacker = CreateCharacter(20L, gameworld.Object, deathLocation.Object);
		Mock<ICharacter> victim = CreateCharacter(30L, gameworld.Object, deathLocation.Object);
		Mock<IWound> wound = CreateMurderWound(attacker.Object, null!, WoundSeverity.Horrifying, null, false);
		Mock<IBody> body = new();
		body.SetupGet(x => x.Wounds).Returns(new[] { wound.Object });
		victim.SetupGet(x => x.Body).Returns(body.Object);

		AutomaticCrimeExtensions.CheckMurderForDeath(victim.Object);

		authority.Verify(x => x.CheckPossibleCrime(It.IsAny<ICharacter>(), CrimeTypes.Murder,
			It.IsAny<ICharacter>(), It.IsAny<IGameItem>(), It.IsAny<string>(),
			It.IsAny<IEnumerable<ICharacter>>(), It.IsAny<bool>(), It.IsAny<ICell>()), Times.Never);
	}

	[TestMethod]
	public void CheckMurderForDeath_BelowConfiguredSeverity_DoesNotReportMurder()
	{
		Mock<IFuturemud> gameworld = CreateGameworld(out Mock<ILegalAuthority> authority, out _, out _);
		Mock<ICell> deathLocation = CreateCell(1L, CreateZone(10L).Object);
		Mock<ICharacter> attacker = CreateCharacter(20L, gameworld.Object, deathLocation.Object);
		Mock<ICharacter> victim = CreateCharacter(30L, gameworld.Object, deathLocation.Object);
		Mock<IWound> wound = CreateMurderWound(attacker.Object, null!, WoundSeverity.Moderate,
			DateTime.UtcNow.AddMinutes(-5), false);
		Mock<IBody> body = new();
		body.SetupGet(x => x.Wounds).Returns(new[] { wound.Object });
		victim.SetupGet(x => x.Body).Returns(body.Object);

		AutomaticCrimeExtensions.CheckMurderForDeath(victim.Object);

		authority.Verify(x => x.CheckPossibleCrime(It.IsAny<ICharacter>(), CrimeTypes.Murder,
			It.IsAny<ICharacter>(), It.IsAny<IGameItem>(), It.IsAny<string>(),
			It.IsAny<IEnumerable<ICharacter>>(), It.IsAny<bool>(), It.IsAny<ICell>()), Times.Never);
	}

	[TestMethod]
	public void CheckMurderForDeath_AttackerAbsent_DoesNotPassDeathWitnesses()
	{
		Mock<IFuturemud> gameworld = CreateGameworld(out Mock<ILegalAuthority> authority, out _, out _);
		Mock<ICell> deathLocation = CreateCell(1L, CreateZone(10L).Object);
		Mock<ICharacter> attacker = CreateCharacter(20L, gameworld.Object, deathLocation.Object);
		Mock<ICharacter> victim = CreateCharacter(30L, gameworld.Object, deathLocation.Object);
		Mock<ICharacter> witness = CreateCharacter(40L, gameworld.Object, deathLocation.Object);
		witness.Setup(x => x.CanSee(victim.Object)).Returns(true);
		deathLocation.Setup(x => x.LayerCharacters(RoomLayer.GroundLevel)).Returns(new[] { witness.Object });
		Mock<IWound> wound = CreateMurderWound(attacker.Object, null!, WoundSeverity.Horrifying,
			DateTime.UtcNow.AddMinutes(-5), false);
		Mock<IBody> body = new();
		body.SetupGet(x => x.Wounds).Returns(new[] { wound.Object });
		victim.SetupGet(x => x.Body).Returns(body.Object);

		AutomaticCrimeExtensions.CheckMurderForDeath(victim.Object);

		authority.Verify(x => x.CheckPossibleCrime(attacker.Object, CrimeTypes.Murder, victim.Object, null!,
			It.Is<string>(s => s.Contains("automatic=death") && s.Contains("attackerpresent=false")),
			It.Is<IEnumerable<ICharacter>>(w => !w.Any()), false, deathLocation.Object), Times.Once);
	}

	[TestMethod]
	public void AutomaticCrimeContext_DeathEvidenceForPcJudge_HidesRawContext()
	{
		Mock<ICharacter> enforcer = new();
		enforcer.Setup(x => x.IsAdministrator(It.IsAny<PermissionLevel>())).Returns(false);
		var output = AutomaticCrimeContext.DescribeForCrimeInfo(
			"automatic=death; victim=#30; wounds=1; maxseverity=Severe; damagetype=Piercing; bodypart=left arm; woundage=5 minutes; friendly=false; attackerpresent=true",
			enforcer.Object, null, false);

		StringAssert.Contains(output, "Automatic death investigation");
		StringAssert.Contains(output, "recent non-friendly wound");
		StringAssert.Contains(output, "left arm");
		Assert.IsFalse(output.Contains("automatic=death"));
		Assert.IsFalse(output.Contains("victim=#30"));
		Assert.IsFalse(output.Contains("Raw Context"));
	}

	[TestMethod]
	public void CrimeConstructor_ExplicitLocation_DerivesMudTimeFromCrimeLocation()
	{
		var crime = File.ReadAllText(GetSourcePath("MudSharpCore", "RPG", "Law", "Crime.cs"));
		var constructorStart = crime.IndexOf(
			"public Crime(ICharacter criminal, ICharacter? victim, IEnumerable<ICharacter> witnesses, ILaw law",
			StringComparison.Ordinal);
		var constructorEnd = crime.IndexOf("Gameworld.SaveManager.AddInitialisation(this);", constructorStart,
			StringComparison.Ordinal);
		Assert.IsTrue(constructorStart > 0);
		Assert.IsTrue(constructorEnd > constructorStart);

		var constructor = crime[constructorStart..constructorEnd];
		StringAssert.Contains(constructor, "var resolvedCrimeLocation = crimeLocation ?? criminal.Location;");
		StringAssert.Contains(constructor, "TimeOfCrime = resolvedCrimeLocation.DateTime();");
		StringAssert.Contains(constructor, "CrimeLocation = resolvedCrimeLocation;");
	}

	[TestMethod]
	public void SimpleWoundDatabaseInsert_PersistsFriendlyWoundExtras()
	{
		var simpleWound = File.ReadAllText(GetSourcePath("MudSharpCore", "Health", "Wounds", "SimpleWound.cs"));
		var insertStart = simpleWound.IndexOf("public override object DatabaseInsert()", StringComparison.Ordinal);
		var insertEnd = simpleWound.IndexOf("return dbitem;", insertStart, StringComparison.Ordinal);
		Assert.IsTrue(insertStart > 0);
		Assert.IsTrue(insertEnd > insertStart);

		var insert = simpleWound[insertStart..insertEnd];
		StringAssert.Contains(insert, "dbitem.ExtraInformation = SaveExtras();");
	}

	[TestMethod]
	public void CheckGreviousBodilyHarmForWound_ConfiguredSeverity_ReportsCrime()
	{
		Mock<IFuturemud> gameworld = CreateGameworld(out Mock<ILegalAuthority> authority, out _, out _);
		gameworld.Setup(x => x.GetStaticConfiguration(AutomaticCrimeExtensions.GreviousBodilyHarmMinimumSeveritySetting))
		         .Returns("Grievous");
		Mock<ICell> location = CreateCell(1L, CreateZone(10L).Object);
		Mock<ICharacter> attacker = CreateCharacter(20L, gameworld.Object, location.Object);
		Mock<ICharacter> victim = CreateCharacter(30L, gameworld.Object, location.Object);
		Mock<IGameItem> weapon = CreateItem(40L, "club");
		Mock<IWound> wound = new();
		wound.SetupGet(x => x.Parent).Returns(victim.Object);
		wound.SetupGet(x => x.ActorOrigin).Returns(attacker.Object);
		wound.SetupGet(x => x.ToolOrigin).Returns(weapon.Object);
		wound.SetupGet(x => x.Severity).Returns(WoundSeverity.Grievous);
		wound.SetupGet(x => x.DamageType).Returns(DamageType.Crushing);

		AutomaticCrimeExtensions.CheckGreviousBodilyHarmForWound(wound.Object);

		authority.Verify(x => x.CheckPossibleCrime(attacker.Object, CrimeTypes.GreviousBodilyHarm, victim.Object,
			weapon.Object, It.Is<string>(s => s.Contains("automatic=wound") && s.Contains("severity=Grievous")),
			null!, true, location.Object), Times.Once);
	}

	[TestMethod]
	public void IsLawfulEnforcementActionAgainst_ActiveArrestTarget_AllowsAssaultButNotMurder()
	{
		Mock<IFuturemud> gameworld = CreateGameworld(out _, out _, out _);
		Mock<ICell> location = CreateCell(1L, CreateZone(10L).Object);
		Mock<ICharacter> enforcer = CreateCharacter(20L, gameworld.Object, location.Object);
		Mock<ICharacter> suspect = CreateCharacter(30L, gameworld.Object, location.Object);
		Mock<IPatrol> patrol = CreatePatrol(enforcer.Object, suspect.Object, EnforcementStrategy.ArrestAndDetain);
		PatrolMemberEffect effect = new(enforcer.Object, patrol.Object);
		enforcer.Setup(x => x.CombinedEffectsOfType<PatrolMemberEffect>()).Returns([effect]);
		suspect.Setup(x => x.EffectsOfType<ExecutionPatrolNoQuit>(It.IsAny<Predicate<ExecutionPatrolNoQuit>>()))
		       .Returns(Enumerable.Empty<ExecutionPatrolNoQuit>());

		Assert.IsTrue(enforcer.Object.IsLawfulEnforcementActionAgainst(suspect.Object, CrimeTypes.Assault));
		Assert.IsFalse(enforcer.Object.IsLawfulEnforcementActionAgainst(suspect.Object, CrimeTypes.Murder));
	}

	[TestMethod]
	public void CheckGreviousBodilyHarmForWound_LawfulArrestEnforcement_DoesNotReportCrime()
	{
		Mock<IFuturemud> gameworld = CreateGameworld(out Mock<ILegalAuthority> authority, out _, out _);
		Mock<ICell> location = CreateCell(1L, CreateZone(10L).Object);
		Mock<ICharacter> enforcer = CreateCharacter(20L, gameworld.Object, location.Object);
		Mock<ICharacter> suspect = CreateCharacter(30L, gameworld.Object, location.Object);
		Mock<IPatrol> patrol = CreatePatrol(enforcer.Object, suspect.Object, EnforcementStrategy.ArrestAndDetain);
		PatrolMemberEffect effect = new(enforcer.Object, patrol.Object);
		enforcer.Setup(x => x.CombinedEffectsOfType<PatrolMemberEffect>()).Returns([effect]);
		suspect.Setup(x => x.EffectsOfType<ExecutionPatrolNoQuit>(It.IsAny<Predicate<ExecutionPatrolNoQuit>>()))
		       .Returns(Enumerable.Empty<ExecutionPatrolNoQuit>());
		Mock<IWound> wound = new();
		wound.SetupGet(x => x.Parent).Returns(suspect.Object);
		wound.SetupGet(x => x.ActorOrigin).Returns(enforcer.Object);
		wound.SetupGet(x => x.Severity).Returns(WoundSeverity.Grievous);
		wound.SetupGet(x => x.DamageType).Returns(DamageType.Crushing);

		AutomaticCrimeExtensions.CheckGreviousBodilyHarmForWound(wound.Object);

		authority.Verify(x => x.CheckPossibleCrime(It.IsAny<ICharacter>(), CrimeTypes.GreviousBodilyHarm,
			It.IsAny<ICharacter>(), It.IsAny<IGameItem>(), It.IsAny<string>(),
			It.IsAny<IEnumerable<ICharacter>>(), It.IsAny<bool>(), It.IsAny<ICell>()), Times.Never);
	}

	[TestMethod]
	public void CheckMurderForDeath_LawfulLethalEnforcement_DoesNotReportCrime()
	{
		Mock<IFuturemud> gameworld = CreateGameworld(out Mock<ILegalAuthority> authority, out _, out _);
		Mock<ICell> deathLocation = CreateCell(1L, CreateZone(10L).Object);
		Mock<ICharacter> enforcer = CreateCharacter(20L, gameworld.Object, deathLocation.Object);
		Mock<ICharacter> suspect = CreateCharacter(30L, gameworld.Object, deathLocation.Object);
		Mock<IPatrol> patrol = CreatePatrol(enforcer.Object, suspect.Object, EnforcementStrategy.LethalForceArrestAndDetain);
		PatrolMemberEffect effect = new(enforcer.Object, patrol.Object);
		enforcer.Setup(x => x.CombinedEffectsOfType<PatrolMemberEffect>()).Returns([effect]);
		suspect.Setup(x => x.EffectsOfType<ExecutionPatrolNoQuit>(It.IsAny<Predicate<ExecutionPatrolNoQuit>>()))
		       .Returns(Enumerable.Empty<ExecutionPatrolNoQuit>());
		Mock<IWound> wound = CreateMurderWound(enforcer.Object, null!, WoundSeverity.Horrifying,
			DateTime.UtcNow.AddMinutes(-5), false);
		Mock<IBody> body = new();
		body.SetupGet(x => x.Wounds).Returns([wound.Object]);
		suspect.SetupGet(x => x.Body).Returns(body.Object);

		AutomaticCrimeExtensions.CheckMurderForDeath(suspect.Object);

		authority.Verify(x => x.CheckPossibleCrime(It.IsAny<ICharacter>(), CrimeTypes.Murder,
			It.IsAny<ICharacter>(), It.IsAny<IGameItem>(), It.IsAny<string>(),
			It.IsAny<IEnumerable<ICharacter>>(), It.IsAny<bool>(), It.IsAny<ICell>()), Times.Never);
	}

	[TestMethod]
	public void CheckLawfulMovement_TrespassingWouldBeCrime_BlocksMovement()
	{
		Mock<IFuturemud> gameworld = CreateGameworld(out Mock<ILegalAuthority> authority, out All<IProperty> properties,
			out _);
		Mock<IZone> zone = CreateZone(10L);
		Mock<ICell> origin = CreateCell(1L, zone.Object);
		Mock<ICell> destination = CreateCell(2L, zone.Object);
		Mock<ICellExit> exit = CreateExit(origin.Object, destination.Object);
		Mock<IProperty> property = CreateProperty(50L, "Warehouse", destination.Object);
		properties.Add(property.Object);
		Mock<IAccount> account = new();
		account.SetupGet(x => x.ActLawfully).Returns(true);
		Mock<IOutputHandler> output = new();
		Mock<ICharacter> actor = CreateCharacter(20L, gameworld.Object, origin.Object);
		actor.SetupGet(x => x.Account).Returns(account.Object);
		actor.SetupGet(x => x.OutputHandler).Returns(output.Object);
		authority.Setup(x => x.WouldBeACrimeAtLocation(actor.Object, CrimeTypes.Trespassing, null!, null!,
				It.Is<string>(s => s.Contains("automatic=property-entry")), destination.Object))
			.Returns(true);

		var blocked = AutomaticCrimeExtensions.CheckLawfulMovement(actor.Object, exit.Object);

		Assert.IsTrue(blocked);
		output.Verify(x => x.Send(It.Is<string>(s => s.Contains("That movement would be a crime")),
			It.IsAny<bool>(), It.IsAny<bool>()), Times.Once);
	}

	[TestMethod]
	public void CheckLawfulMovement_PrivateMarkerUsesControllerContextEvenWithoutLegacyPropertyToggle()
	{
		Mock<IFuturemud> gameworld = CreateGameworld(out Mock<ILegalAuthority> authority, out _, out _);
		Mock<IZone> zone = CreateZone(10L);
		Mock<ICell> origin = CreateCell(1L, zone.Object);
		Mock<ICell> destination = CreateCell(2L, zone.Object);
		destination.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		Mock<ICellExit> exit = CreateExit(origin.Object, destination.Object);
		Mock<IProperty> property = CreateProperty(50L, "Warehouse", destination.Object);
		property.SetupGet(x => x.ApplyCriminalCodeInProperty).Returns(false);
		property.SetupGet(x => x.PropertyOwners).Returns(Array.Empty<IPropertyOwner>());
		property.Setup(x => x.IsAuthorisedOwner(It.IsAny<ICharacter>())).Returns(false);
		property.Setup(x => x.IsAuthorisedLeaseHolder(It.IsAny<ICharacter>())).Returns(false);
		var effect = new PrivatePropertyEffect(destination.Object, property.Object);
		destination.Setup(x => x.EffectsOfType<PrivatePropertyEffect>(It.IsAny<Predicate<PrivatePropertyEffect>>()))
		           .Returns([effect]);
		Mock<IAccount> account = new();
		account.SetupGet(x => x.ActLawfully).Returns(true);
		Mock<ICharacter> actor = CreateCharacter(20L, gameworld.Object, origin.Object);
		actor.SetupGet(x => x.Account).Returns(account.Object);
		actor.SetupGet(x => x.OutputHandler).Returns(new Mock<IOutputHandler>().Object);
		authority.Setup(x => x.WouldBeACrimeAtLocation(actor.Object, CrimeTypes.Trespassing, null!, null!,
				It.Is<string>(s => s.Contains("automatic=private-property-entry") &&
				                   s.Contains("controllertype=Property") && s.Contains("denial=")), destination.Object))
		         .Returns(true);

		Assert.IsTrue(AutomaticCrimeExtensions.CheckLawfulMovement(actor.Object, exit.Object));
	}

	[TestMethod]
	public void CheckLawfulMovement_LawfulFollowerWouldTrespass_BlocksWholeMovement()
	{
		Mock<IFuturemud> gameworld = CreateGameworld(out Mock<ILegalAuthority> authority, out All<IProperty> properties,
			out _);
		Mock<IZone> zone = CreateZone(10L);
		Mock<ICell> origin = CreateCell(1L, zone.Object);
		Mock<ICell> destination = CreateCell(2L, zone.Object);
		Mock<ICellExit> exit = CreateExit(origin.Object, destination.Object);
		Mock<IProperty> property = CreateProperty(50L, "Warehouse", destination.Object);
		properties.Add(property.Object);
		Mock<IAccount> followerAccount = new();
		followerAccount.SetupGet(x => x.ActLawfully).Returns(true);
		Mock<IOutputHandler> leaderOutput = new();
		Mock<IOutputHandler> followerOutput = new();
		Mock<ICharacter> leader = CreateCharacter(20L, gameworld.Object, origin.Object);
		leader.SetupGet(x => x.OutputHandler).Returns(leaderOutput.Object);
		Mock<ICharacter> follower = CreateCharacter(21L, gameworld.Object, origin.Object);
		follower.SetupGet(x => x.Account).Returns(followerAccount.Object);
		follower.SetupGet(x => x.OutputHandler).Returns(followerOutput.Object);
		follower.Setup(x => x.HowSeen(leader.Object, true, DescriptionType.Short, true, PerceiveIgnoreFlags.None))
		        .Returns("Follower");
		authority.Setup(x => x.WouldBeACrimeAtLocation(follower.Object, CrimeTypes.Trespassing, null!, null!,
				It.Is<string>(s => s.Contains("automatic=property-entry")), destination.Object))
			.Returns(true);

		var blocked = AutomaticCrimeExtensions.CheckLawfulMovement(new[] { leader.Object, follower.Object }, exit.Object,
			leader.Object);

		Assert.IsTrue(blocked);
		followerOutput.Verify(x => x.Send(It.Is<string>(s => s.Contains("That movement would be a crime")),
			It.IsAny<bool>(), It.IsAny<bool>()), Times.Once);
		leaderOutput.Verify(x => x.Send(It.Is<string>(s => s.Contains("refuses to move")),
			It.IsAny<bool>(), It.IsAny<bool>()), Times.Once);
	}

	[TestMethod]
	public void CheckLocationEntryCrimes_ContrabandBoundary_ReportsTrafficking()
	{
		Mock<IFuturemud> gameworld = CreateGameworld(out Mock<ILegalAuthority> authority, out _, out _);
		Mock<IZone> originZone = CreateZone(10L);
		Mock<IZone> destinationZone = CreateZone(11L);
		Mock<ICell> origin = CreateCell(1L, originZone.Object);
		Mock<ICell> destination = CreateCell(2L, destinationZone.Object);
		Mock<ICellExit> exit = CreateExit(origin.Object, destination.Object);
		authority.SetupGet(x => x.EnforcementZones).Returns(new[] { destinationZone.Object });
		Mock<ICharacter> actor = CreateCharacter(20L, gameworld.Object, destination.Object);
		Mock<IGameItem> contraband = CreateItem(40L, "contraband");
		actor.SetupGet(x => x.Inventory).Returns(new[] { contraband.Object });
		authority.Setup(x => x.WouldBeACrimeAtLocation(actor.Object, CrimeTypes.TrafficingContraband, null!,
				contraband.Object, It.Is<string>(s => s.Contains("automatic=contraband-boundary")), destination.Object))
			.Returns(true);

		AutomaticCrimeExtensions.CheckLocationEntryCrimes(actor.Object, destination.Object, exit.Object);

		authority.Verify(x => x.CheckPossibleCrime(actor.Object, CrimeTypes.TrafficingContraband, null!,
			contraband.Object, It.Is<string>(s => s.Contains("automatic=contraband-boundary")),
			null!, true, destination.Object), Times.Once);
	}

	[TestMethod]
	public void CheckLocationEntryCrimes_DraggedMovementTarget_DoesNotReportEntryCrime()
	{
		Mock<IFuturemud> gameworld = CreateGameworld(out Mock<ILegalAuthority> authority, out All<IProperty> properties,
			out _);
		Mock<IZone> zone = CreateZone(10L);
		Mock<ICell> origin = CreateCell(1L, zone.Object);
		Mock<ICell> destination = CreateCell(2L, zone.Object);
		Mock<ICellExit> exit = CreateExit(origin.Object, destination.Object);
		Mock<IProperty> property = CreateProperty(50L, "Warehouse", destination.Object);
		properties.Add(property.Object);
		Mock<ICharacter> actor = CreateCharacter(20L, gameworld.Object, destination.Object);

		AutomaticCrimeExtensions.CheckLocationEntryCrimes(actor.Object, destination.Object, exit.Object, false);

		authority.Verify(x => x.CheckPossibleCrime(It.IsAny<ICharacter>(), It.IsAny<CrimeTypes>(),
			It.IsAny<ICharacter>(), It.IsAny<IGameItem>(), It.IsAny<string>(), It.IsAny<IEnumerable<ICharacter>>(),
			It.IsAny<bool>(), It.IsAny<ICell>()), Times.Never);
		authority.Verify(x => x.WouldBeACrimeAtLocation(It.IsAny<ICharacter>(), It.IsAny<CrimeTypes>(),
			It.IsAny<ICharacter>(), It.IsAny<IGameItem>(), It.IsAny<string>(), It.IsAny<ICell>()), Times.Never);
	}

	private static Mock<IFuturemud> CreateGameworld(out Mock<ILegalAuthority> authority,
		out All<IProperty> properties, out All<ILegalAuthority> authorities)
	{
		authority = new Mock<ILegalAuthority>();
		authority.SetupGet(x => x.Id).Returns(100L);
		authority.SetupGet(x => x.Name).Returns("Test Authority");
		authority.SetupGet(x => x.FrameworkItemType).Returns("LegalAuthority");
		authority.SetupGet(x => x.EnforcementZones).Returns(Array.Empty<IZone>());
		authority.Setup(x => x.CheckPossibleCrime(It.IsAny<ICharacter>(), It.IsAny<CrimeTypes>(),
				It.IsAny<ICharacter>(), It.IsAny<IGameItem>(), It.IsAny<string>(), It.IsAny<IEnumerable<ICharacter>>(),
				It.IsAny<bool>(), It.IsAny<ICell>()))
			.Returns(Array.Empty<ICrime>());
		authorities = new All<ILegalAuthority>();
		authorities.Add(authority.Object);
		properties = new All<IProperty>();
		Mock<IFuturemud> gameworld = new();
		gameworld.SetupGet(x => x.LegalAuthorities).Returns(authorities);
		gameworld.SetupGet(x => x.Properties).Returns(properties);
		gameworld.Setup(x => x.GetStaticConfiguration(It.IsAny<string>())).Returns<string>(key => key switch
		{
			AutomaticCrimeExtensions.GreviousBodilyHarmMinimumSeveritySetting => "Grievous",
			AutomaticCrimeExtensions.MurderMinimumSeveritySetting => "Severe",
			AutomaticCrimeExtensions.MurderWoundAttributionWindowSecondsSetting => "7200",
			AutomaticCrimeExtensions.MurderIncludeFriendlyWoundsSetting => "false",
			_ => "0"
		});
		return gameworld;
	}

	private static Mock<IWound> CreateMurderWound(ICharacter attacker, IGameItem weapon, WoundSeverity severity,
		DateTime? realTimeOfWound, bool isFriendly)
	{
		Mock<IWound> wound = new();
		wound.SetupGet(x => x.ActorOrigin).Returns(attacker);
		wound.SetupGet(x => x.ToolOrigin).Returns(weapon);
		wound.SetupGet(x => x.Severity).Returns(severity);
		wound.SetupGet(x => x.DamageType).Returns(DamageType.Piercing);
		wound.SetupGet(x => x.RealTimeOfWound).Returns(realTimeOfWound);
		wound.SetupGet(x => x.IsFriendlyWound).Returns(isFriendly);
		return wound;
	}

	private static Mock<ICharacter> CreateCharacter(long id, IFuturemud gameworld, ICell location)
	{
		Mock<ICharacter> character = new();
		character.SetupGet(x => x.Id).Returns(id);
		character.SetupGet(x => x.Name).Returns($"Character {id}");
		character.SetupGet(x => x.FrameworkItemType).Returns("Character");
		character.SetupGet(x => x.Gameworld).Returns(gameworld);
		character.SetupGet(x => x.Location).Returns(location);
		character.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		character.Setup(x => x.IsAdministrator(It.IsAny<PermissionLevel>())).Returns(false);
		character.Setup(x => x.AffectedBy(It.IsAny<Predicate<PermitWork>>())).Returns(false);
		return character;
	}

	private static Mock<ICell> CreateCell(long id, IZone zone)
	{
		Mock<ICell> cell = new();
		cell.SetupGet(x => x.Id).Returns(id);
		cell.SetupGet(x => x.Name).Returns($"Cell {id}");
		cell.SetupGet(x => x.FrameworkItemType).Returns("Cell");
		cell.SetupGet(x => x.Zone).Returns(zone);
		cell.Setup(x => x.LayerCharacters(It.IsAny<RoomLayer>())).Returns(Array.Empty<ICharacter>());
		return cell;
	}

	private static Mock<IZone> CreateZone(long id)
	{
		Mock<IZone> zone = new();
		zone.SetupGet(x => x.Id).Returns(id);
		zone.SetupGet(x => x.Name).Returns($"Zone {id}");
		zone.SetupGet(x => x.FrameworkItemType).Returns("Zone");
		return zone;
	}

	private static Mock<ICellExit> CreateExit(ICell origin, ICell destination)
	{
		Mock<ICellExit> exit = new();
		exit.SetupGet(x => x.Origin).Returns(origin);
		exit.SetupGet(x => x.Destination).Returns(destination);
		return exit;
	}

	private static Mock<IProperty> CreateProperty(long id, string name, ICell location)
	{
		Mock<IProperty> property = new();
		property.SetupGet(x => x.Id).Returns(id);
		property.SetupGet(x => x.Name).Returns(name);
		property.SetupGet(x => x.FrameworkItemType).Returns("Property");
		property.SetupGet(x => x.ApplyCriminalCodeInProperty).Returns(true);
		property.SetupGet(x => x.PropertyLocations).Returns(new[] { location });
		property.Setup(x => x.HotelRoomForCell(It.IsAny<ICell>())).Returns((IHotelRoom)null!);
		return property;
	}

	private static Mock<IPatrol> CreatePatrol(ICharacter enforcer, ICharacter target, EnforcementStrategy strategy)
	{
		Mock<ILaw> law = new();
		law.SetupGet(x => x.EnforcementStrategy).Returns(strategy);
		Mock<ICrime> crime = new();
		crime.SetupGet(x => x.Law).Returns(law.Object);
		Mock<IPatrol> patrol = new();
		patrol.SetupGet(x => x.Id).Returns(200L);
		patrol.SetupGet(x => x.PatrolMembers).Returns([enforcer]);
		patrol.SetupGet(x => x.ActiveEnforcementTarget).Returns(target);
		patrol.SetupGet(x => x.ActiveEnforcementCrime).Returns(crime.Object);
		return patrol;
	}

	private static Mock<IGameItem> CreateItem(long id, string name)
	{
		Mock<IGameItem> item = new();
		item.SetupGet(x => x.Id).Returns(id);
		item.SetupGet(x => x.Name).Returns(name);
		item.SetupGet(x => x.FrameworkItemType).Returns("Item");
		item.Setup(x => x.GetItemType<IContainer>()).Returns((IContainer)null!);
		return item;
	}

	private static string GetSourcePath(params string[] parts)
	{
		return Path.GetFullPath(Path.Combine(
			AppContext.BaseDirectory,
			"..",
			"..",
			"..",
			"..",
			Path.Combine(parts)));
	}
}
