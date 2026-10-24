#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Body.Implementations;
using MudSharp.Body.PartProtos;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Commands.Modules;
using MudSharp.Framework;
using MudSharp.Form.Shape;
using MudSharp.Models;

namespace MudSharp_Unit_Tests.Commands;

[TestClass]
[DoNotParallelize]
public class BodyPrototypeEditorTests
{
	[TestMethod]
	public void OwnedPartCopyAndDerivedMovement_PreserveOwnershipAndRefreshInheritedState()
	{
		using var database = new InMemoryDatabaseTestScope();
		var (world, bodies, parts) = World();
		var limbs = new All<ILimb>();
		world.SetupGet(x => x.Limbs).Returns(limbs);
		world.SetupGet(x => x.BodypartShapes).Returns(new All<IBodypartShape>());
		world.SetupGet(x => x.Materials).Returns(new All<MudSharp.Form.Material.ISolid>());
		world.SetupGet(x => x.ArmourTypes).Returns(new All<MudSharp.Combat.IArmourType>());
		world.Setup(x => x.Add(It.IsAny<IBodypart>())).Callback<IBodypart>(x => parts.Add(x));
		world.Setup(x => x.Add(It.IsAny<ILimb>())).Callback<ILimb>(x => limbs.Add(x));
		database.Context.WearableSizeParameterRule.Add(WearRules());
		database.Context.SaveChanges();
		var actor = BuilderWorkflowRegressionTests.Actor(world.Object).Object;
		foreach (var name in new[] { "Original", "Independent" })
			BuilderWorkflowRegressionTests.Invoke(typeof(BuilderModule), "BodyPrototypeCreate", actor, new StringStack($"{name} size 1"));
		var original = (BodyPrototype)bodies.Single(x => x.Name == "Original");
		var independent = (BodyPrototype)bodies.Single(x => x.Name == "Independent");
		var templateRow = new BodypartProto { Name = "arm", Description = "an arm", BodyId = original.Id,
			BodypartType = (int)BodypartTypeEnum.Wear, IsCore = true, MaxLife = 100, CountAsId = 999 };
		database.Context.BodypartProtos.Add(templateRow);
		database.Context.SaveChanges();
		var template = BodypartPrototype.LoadFromDatabase(templateRow, world.Object);
		template.SetBodyProto(original);
		parts.Add(template);
		original.UpdateBodypartRole(template, BodypartRole.Core);
		var copy = independent.AddPartFromTemplate(template, "newarm", out var error);
		Assert.IsNotNull(copy, error);
		Assert.AreNotEqual(template.Id, copy.Id);
		Assert.AreSame(original, template.Body);
		Assert.AreSame(independent, copy.Body);
		Assert.IsNull(copy.UpstreamConnection);
		var savedCopy = database.Context.BodypartProtos.Find(copy.Id)!;
		Assert.IsNull(savedCopy.CountAsId);
		Assert.AreEqual(independent.Id, savedCopy.BodyId);
		Assert.AreEqual(100, savedCopy.MaxLife);
		Assert.IsNull(independent.CreateLimb("invalid", LimbType.Arm, template, [copy], [], 1, 1, out _));
		var limb = independent.CreateLimb("newarm", LimbType.Arm, copy, [copy], [], 1.5, 2, out error);
		Assert.IsNotNull(limb, error);
		Assert.IsTrue(independent.Limbs.Contains(limb));
		var savedLimb = database.Context.Limbs.Include(x => x.LimbsBodypartProto).Single();
		var reloadedLimb = new MudSharp.Body.PartProtos.Limb(savedLimb, world.Object);
		Assert.AreSame(copy, reloadedLimb.RootBodypart);
		Assert.AreSame(copy, reloadedLimb.Parts.Single());
		Assert.AreEqual(1.5, reloadedLimb.LimbDamageThresholdMultiplier);

		BuilderWorkflowRegressionTests.Invoke(typeof(BuilderModule), "BodyPrototypeClone", actor, new StringStack("Independent as Derived"));
		var derived = (BodyPrototype)bodies.Single(x => x.Name == "Derived");
		Assert.IsTrue(independent.AddSpeed(PositionStanding.Instance, "walk", "walk", "walks", "walking", 1, 1, out error), error);
		var inherited = independent.Speeds.Single();
		Assert.AreSame(inherited, derived.DefaultSpeeds[PositionStanding.Instance]);
		Assert.IsTrue(derived.AddSpeed(PositionStanding.Instance, "amble", "amble", "ambles", "ambling", 2, 1, out error), error);
		Assert.IsFalse(derived.Speeds.Contains(inherited));
		Assert.AreEqual("amble", derived.DefaultSpeeds[PositionStanding.Instance].Name);
		Assert.AreSame(inherited, independent.DefaultSpeeds[PositionStanding.Instance]);
	}

	[TestMethod]
	public void CreateAndDerive_PreservesPartOwnershipAndRejectsCyclesAndUsedAnatomy()
	{
		using var database = new InMemoryDatabaseTestScope();
		var (world, bodies, parts) = World();
		var actor = BuilderWorkflowRegressionTests.Actor(world.Object);
		database.Context.WearableSizeParameterRule.Add(WearRules()); database.Context.SaveChanges();
		BuilderWorkflowRegressionTests.Invoke(typeof(BuilderModule), "BodyPrototypeCreate", actor.Object, new StringStack("Original size 1"));
		var original = (BodyPrototype)bodies.Single();
		var part = Mock.Of<IExternalBodypart>(x => x.Id == 10 && x.Name == "arm" && x.IdHasBeenRegistered && x.Body == original && x.IsCore);
		parts.Add(part);
		original.UpdateBodypartRole(part, BodypartRole.Core);
		BuilderWorkflowRegressionTests.Invoke(typeof(BuilderModule), "BodyPrototypeClone", actor.Object, new StringStack("Original as Derived"));
		var derived = (BodyPrototype)bodies.Single(x => x.Name == "Derived");
		Assert.AreSame(original, derived.Parent);
		Assert.IsTrue(derived.BodypartsFor(null!, Gender.Neuter).Contains(part));
		Assert.AreSame(original, part.Body);
		Assert.IsFalse(original.SetParent(derived, out _));
		Assert.IsFalse(derived.SetParent(derived, out _));
		Assert.IsTrue(derived.SetPartRemoved(part, true, out _));
		Assert.IsFalse(derived.BodypartsFor(null!, Gender.Neuter).Contains(part));
		BuilderWorkflowRegressionTests.Invoke(typeof(BuilderModule), "BodyPrototypeSetPart", actor.Object, derived, new StringStack("restore 10"));
		Assert.IsTrue(derived.BodypartsFor(null!, Gender.Neuter).Contains(part));
		Assert.AreEqual(0, database.Context.BodyProtosAdditionalBodyparts.Count());
		Assert.IsFalse(original.SetPartRemoved(part, true, out _));
		database.Context.Races.Add(new Race { Name = "Dependent", BaseBodyId = derived.Id, Description = "Dependent race", AllowedGenders = "Male Female",
			BreathingVolumeExpression = "1", CommunicationStrategyType = "Humanoid", DiceExpression = "1d6", EatCorpseEmoteText = "eat",
			HandednessOptions = "1", HoldBreathLengthExpression = "1", MaximumDragWeightExpression = "1", MaximumLiftWeightExpression = "1" }); database.Context.SaveChanges();
		Assert.IsTrue(original.HasInstantiatedBodiesInAffectedTree(out _));
		Assert.IsFalse(original.SetParent(null, out _));
		Assert.IsFalse(derived.SetPartRemoved(part, true, out _));
		Assert.IsFalse(derived.AddSpeed(PositionStanding.Instance, "walk", "walk", "walks", "walking", 1, 1, out _));
		Assert.IsTrue(original.SetMetadata("tracking", "footprints", out _));
		Assert.AreEqual("footprints", database.Context.BodyProtos.Find(original.Id)!.NameForTracking);
	}

	[TestMethod]
	public void BodypartRoles_ChangingGenderRoleReplacesKeyedRowAndInvalidatesDerivedAnatomy()
	{
		using var database = new InMemoryDatabaseTestScope();
		var (world, bodies, parts) = World();
		database.Context.WearableSizeParameterRule.Add(WearRules()); database.Context.SaveChanges();
		var actor = BuilderWorkflowRegressionTests.Actor(world.Object).Object;
		BuilderWorkflowRegressionTests.Invoke(typeof(BuilderModule), "BodyPrototypeCreate", actor, new StringStack("Original size 1"));
		var body = (BodyPrototype)bodies.Single();
		var row = new BodypartProto { Id = 10, BodyId = body.Id, Name = "arm", Description = "arm" };
		database.Context.BodypartProtos.Add(row); database.Context.SaveChanges();
		var part = Mock.Of<IExternalBodypart>(x => x.Id == 10 && x.Name == "arm" && x.IdHasBeenRegistered && x.Body == body);
		parts.Add(part); body.UpdateBodypartRole(part, BodypartRole.Extra);
		foreach (var role in new[] { "male", "female", "extra", "core" })
		{
			BuilderWorkflowRegressionTests.Invoke(typeof(BuilderModule), "BodyPrototypeSetPart", actor, body, new StringStack($"role arm {role}"));
			Assert.AreEqual(role is "male" or "core", body.BodypartsFor(null!, Gender.Male).Contains(part));
			Assert.AreEqual(role is "female" or "core", body.BodypartsFor(null!, Gender.Female).Contains(part));
			Assert.AreEqual(role is "male" or "female" ? 1 : 0, database.Context.BodyProtosAdditionalBodyparts.Count());
		}
		Assert.IsTrue(database.Context.BodypartProtos.Single().IsCore);
	}

	[TestMethod]
	public void BodyMovement_ValidatedChangesSurviveReloadWithoutChangingSpeedIdentity()
	{
		using var database = new InMemoryDatabaseTestScope();
		var (world, bodies, _) = World();
		var rule = WearRules();
		var row = new BodyProto { Name = "Moving", ConsiderString = "", LegDescriptionSingular = "leg", LegDescriptionPlural = "legs", WielderDescriptionSingle = "hand", WielderDescriptionPlural = "hands", WearSizeParameter = rule };
		row.BodyProtosPositions.Add(new BodyProtosPositions { Position = (int)PositionStanding.Instance.Id });
		database.Context.BodyProtos.Add(row); database.Context.SaveChanges();
		var body = new BodyPrototype(row, world.Object); bodies.Add(body);
		Assert.IsFalse(body.SetPosition(PositionStanding.Instance, false, out _));
		Assert.IsTrue(body.AddSpeed(PositionStanding.Instance, "walk", "walk", "walks", "walking", 1, 1, out _));
		var speed = body.Speeds.Single();
		Assert.IsFalse(body.AddSpeed(PositionStanding.Instance, "bad", "bad", "bad", "bad", double.NaN, 1, out _));
		Assert.IsTrue(body.EditSpeed(speed, "stroll", "stroll", "strolls", "strolling", 2, 0.5, out _));
		Assert.IsTrue(body.SetPosition(PositionProne.Instance, true, out _));
		database.Context.ChangeTracker.Clear();
		var saved = database.Context.BodyProtos.Include(x => x.WearSizeParameter).Include(x => x.MoveSpeeds).Include(x => x.BodyProtosPositions).Single();
		var loaded = new BodyPrototype(saved, world.Object);
		Assert.IsTrue(loaded.ValidPositions.Contains(PositionProne.Instance));
		var reloadedSpeed = loaded.Speeds.Single(x => x.Id == speed.Id);
		Assert.AreEqual("stroll", reloadedSpeed.Name);
		Assert.AreEqual(2.0, reloadedSpeed.Multiplier);
		Assert.AreEqual(speed.Id, loaded.DefaultSpeeds[PositionStanding.Instance].Id);
	}

	[TestMethod]
	[DataRow("broken XML")]
	[DataRow("<Ratios><Ratio Item=\"2\" Min=\"0\" Max=\"1\" /><Ratio Item=\"3\" Min=\"2\" Max=\"3\" /></Ratios>")]
	public void Create_InvalidWearRules_LeavesNoPersistedPrototype(string ratios)
	{
		using var database = new InMemoryDatabaseTestScope();
		var (world, bodies, _) = World();
		var rule = WearRules(); rule.WeightVolumeRatios = ratios;
		database.Context.WearableSizeParameterRule.Add(rule); database.Context.SaveChanges();
		BuilderWorkflowRegressionTests.Invoke(typeof(BuilderModule), "BodyPrototypeCreate", BuilderWorkflowRegressionTests.Actor(world.Object).Object, new StringStack("Broken size 1"));
		Assert.AreEqual(0, database.Context.BodyProtos.Count());
		Assert.AreEqual(0, bodies.Count());
	}

	private static (Mock<IFuturemud>, All<IBodyPrototype>, All<IBodypart>) World()
	{
		var world = BuilderWorkflowRegressionTests.World();
		var bodies = new All<IBodyPrototype>(); var parts = new All<IBodypart>();
		world.SetupGet(x => x.BodyPrototypes).Returns(bodies);
		world.SetupGet(x => x.BodypartPrototypes).Returns(parts);
		world.Setup(x => x.Add(It.IsAny<IBodyPrototype>())).Callback<IBodyPrototype>(x => bodies.Add(x));
		return (world, bodies, parts);
	}

	private static WearableSizeParameterRule WearRules() => new()
	{
		Id = 1, IgnoreTrait = true, MinHeightFactor = 0.5, MaxHeightFactor = 2, MinWeightFactor = 0.5, MaxWeightFactor = 2,
		WeightVolumeRatios = "<Ratios><Ratio Item=\"2\" Min=\"0\" Max=\"1000\" /></Ratios>",
		TraitVolumeRatios = "<Ratios><Ratio Item=\"2\" Min=\"0\" Max=\"1000\" /></Ratios>",
		HeightLinearRatios = "<Ratios><Ratio Item=\"2\" Min=\"0\" Max=\"1000\" /></Ratios>"
	};
}
