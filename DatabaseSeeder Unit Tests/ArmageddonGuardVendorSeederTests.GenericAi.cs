#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CultureInfo = System.Globalization.CultureInfo;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Character;
using MudSharp.Community;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.Models;
using MudSharp.Character.Heritage;
using MudSharp.CharacterCreation;
using MudSharp.CharacterCreation.Roles;
using MudSharp.Body.Traits;
using Microsoft.EntityFrameworkCore;
using MudSharp.Planes;

namespace MudSharp_Unit_Tests;

public partial class ArmageddonGuardVendorSeederTests
{
	[TestMethod]
	public void GenericAi_CompilesAndAttachesFlagsToBaseAndResetVariants()
	{
		using var db = ArmageddonInstallationSettingsTests.Fixture();
		ArmageddonInstallation.Prepare(ArmageddonInstallationSettingsTests.Factory(db));
		ArmWorldNpc Npc(int id, int flags) => new(new("fixture", 1), id, 1, "npc", "npc", "", "", "", [flags, 0, 0, 0], [0, 0, 0, 0, 100, 0], [0, 0], [0, 0, 0, 0, 0, 0, 1], 20, [], [], [], []);
		var source = new ArmageddonWorldfilePackage { Npcs = [Npc(1, 32 | 2), Npc(2, 32), Npc(3, 2 | (1 << 26)), Npc(4, 64), Npc(5, 2)] };
		var writer = new ArmageddonWorldfileWriter(db, source);
		foreach (var npc in source.Npcs)
		{
			var template = new NpcTemplate { Id = npc.Vnum + 1000, Definition = "<Definition/>" };
			var variant = new NpcTemplate { Id = npc.Vnum + 2000, Definition = "<Definition/>" };
			db.NpcTemplates.AddRange(template, variant);
			Field<Dictionary<int, NpcTemplate>>(writer, "_npcs").Add(npc.Vnum, template);
			Field<Dictionary<int, long[]>>(writer, "_npcFamilyIds").Add(npc.Vnum, [template.Id, variant.Id]);
		}
		db.SaveChanges();
		var old = new ArtificialIntelligence { Id = 9000, Name = "Existing Special", Type = "Aggressor", Definition = $"<Definition><WillAttackProg>{db.FutureProgs.Single(x => x.FunctionName == "AlwaysFalse").Id}</WillAttackProg><EngageDelayDiceExpression>1000</EngageDelayDiceExpression><EngageEmote/></Definition>" };
		db.ArtificialIntelligences.Add(old);
		foreach (var id in new long[] { 1001, 2001 }) db.NpcTemplates.Single(x => x.Id == id).NpctemplatesArtificalIntelligences.Add(new() { AiId = old.Id, NpcTemplateId = id });
		db.SaveChanges();
		typeof(ArmageddonWorldfileWriter).GetMethod("InstallGenericNpcAi", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(writer, null);
		var (_, progs, _) = Runtime(db, registers =>
		{
			registers.Setup(x => x.GetType(ProgVariableTypes.Character, ArmageddonWorldfileWriter.MountTrainedRegister)).Returns(ProgVariableTypes.Number);
			registers.Setup(x => x.GetValue(It.IsAny<IProgVariable>(), ArmageddonWorldfileWriter.MountTrainedRegister)).Returns(new NumberVariable(0));
			registers.Setup(x => x.GetType(ProgVariableTypes.Character, "arm_npc_legacy_tribe")).Returns(ProgVariableTypes.Number);
			registers.Setup(x => x.GetValue(It.IsAny<IProgVariable>(), "arm_npc_legacy_tribe")).Returns(new NumberVariable(0));
		});
		foreach (var template in db.NpcTemplates)
		{
			var id = (int)(template.Id % 1000);
			var ais = template.NpctemplatesArtificalIntelligences.Select(x => db.ArtificialIntelligences.Single(y => y.Id == x.AiId)).ToArray();
			Assert.AreEqual(1, ais.Count(x => x.Type == "Aggressor"), "Non-aggressive NPCs also react to learned hate/fear.");
			Assert.AreEqual(id is 1 or 2, (bool?)XElement.Parse(ais.Single(x => x.Type == "Aggressor").Definition).Element("AggressiveClassification") ?? true);
			Assert.AreEqual(id is 2 or 4 ? 1 : 0, ais.Count(x => x.Type == "Wanderer"));
			if (id is 1 or 2) Assert.AreEqual(id == 1 ? "0" : "1", XElement.Parse(ais.Single(x => x.Type == "Aggressor").Definition).Element("AcquisitionRange")!.Value);
		}
		var actor = Actor(); var target = Actor();
		var material = PlanarPresenceDefinition.Manifested(Mock.Of<IPlane>(x => x.Id == 1));
		actor.SetupGet(x => x.Body).Returns(Mock.Of<MudSharp.Body.IBody>(x => x.BasePlanarPresence == material));
		target.SetupGet(x => x.Body).Returns(Mock.Of<MudSharp.Body.IBody>(x => x.BasePlanarPresence == material));
		actor.SetupGet(x => x.PositionState).Returns(PositionStanding.Instance);
		actor.Setup(x => x.GetProperty("clans")).Returns(new CollectionVariable(new List<IClan>(), ProgVariableTypes.Clan));
		target.Setup(x => x.GetProperty("following")).Returns(new NullVariable(ProgVariableTypes.Character));
		target.Setup(x => x.GetProperty("npctemplateid")).Returns(new NumberVariable(2005));
		var room = Item<IRoom>(20, ProgVariableTypes.Location); var other = Item<IRoom>(21, ProgVariableTypes.Location);
		actor.Setup(x => x.GetProperty("location")).Returns(room.Object);
		target.Setup(x => x.GetProperty("location")).Returns(other.Object);
		Assert.IsFalse(progs.GetByName("ArmageddonWorldfileAggressive1").ExecuteBool(actor.Object, target.Object));
		target.Setup(x => x.GetProperty("location")).Returns(room.Object);
		Assert.IsTrue(progs.GetByName("ArmageddonWorldfileAggressive1").ExecuteBool(actor.Object, target.Object));
		target.Setup(x => x.GetProperty("npctemplateid")).Returns(new NumberVariable(2003));
		Assert.IsFalse(progs.GetByName("ArmageddonWorldfileAggressive1").ExecuteBool(actor.Object, target.Object), "Reset variants of mounts remain protected.");
		actor.Setup(x => x.GetProperty("following")).Returns(new NullVariable(ProgVariableTypes.Character));
		actor.Setup(x => x.GetProperty("canmove")).Returns(new BooleanVariable(true));
		actor.Setup(x => x.GetProperty("possessednpc")).Returns(new BooleanVariable(false));
		actor.Setup(x => x.CanSee(It.IsAny<IPerceivable>(), It.IsAny<PerceiveIgnoreFlags>())).Returns(true);
		var zone = Item<IZone>(30, ProgVariableTypes.Zone); var otherZone = Item<IZone>(31, ProgVariableTypes.Zone);
		room.Setup(x => x.GetProperty("zone")).Returns(zone.Object); other.Setup(x => x.GetProperty("zone")).Returns(otherZone.Object);
		Assert.IsFalse(progs.GetByName("ArmageddonWorldfileWanderWithinZone").ExecuteBool(actor.Object, other.Object, room.Object));
		other.Setup(x => x.GetProperty("zone")).Returns(zone.Object);
		var passage = new Mock<MudSharp.Construction.Boundary.IRoomExit>();
		passage.As<IProgVariable>().SetupGet(x => x.GetObject).Returns(passage.Object);
		passage.As<IProgVariable>().SetupGet(x => x.Type).Returns(ProgVariableTypes.Exit);
		passage.As<IProgVariable>().Setup(x => x.GetProperty("destination")).Returns(other.Object);
		passage.As<IProgVariable>().Setup(x => x.GetProperty("direction")).Returns(new TextVariable("East"));
		room.Setup(x => x.GetProperty("exits")).Returns(new CollectionVariable(new[] { passage.Object }, ProgVariableTypes.Exit));
		Assert.IsTrue(progs.GetByName("ArmageddonWorldfileWanderWithinZone").ExecuteBool(actor.Object, other.Object, room.Object));
		passage.As<IProgVariable>().Setup(x => x.GetProperty("direction")).Returns(new TextVariable("Up"));
		Assert.IsFalse(progs.GetByName("ArmageddonWorldfileWanderWithinZone").ExecuteBool(actor.Object, other.Object, room.Object));
	}

	[TestMethod]
	public void PlayerPoolFormula_UsesLiveRaceRegistersEnduranceAndWarriorRole()
	{
		using var db = ArmageddonHeritageTests.Fixture();
		db.HealthStrategies.Add(new HealthStrategy { Id = 900000, Name = "Human HP", Type = "BrainHitpoints", Definition = "<Definition><MaximumHitPointsExpression>1</MaximumHitPointsExpression><HealingTickDamageExpression>1</HealingTickDamageExpression></Definition>" });
		db.SaveChanges(); ArmageddonHeritage.Install(db, new List<string>());
		var (world, _, _) = Runtime(db, registers =>
		{
			foreach (var definition in db.VariableDefinitions)
				registers.Setup(x => x.GetType(ProgVariableTypes.FromStorageString(definition.OwnerTypeDefinition), definition.Property)).Returns(ProgVariableTypes.FromStorageString(definition.ContainedTypeDefinition));
			registers.Setup(x => x.GetValue(It.IsAny<IProgVariable>(), It.IsAny<string>())).Returns((IProgVariable owner, string property) =>
			{
				var race = (IRace)owner.GetObject;
				var value = db.VariableValues.Single(x => x.ReferenceId == race.Id && x.ReferenceProperty == property);
				var xml = XElement.Parse(value.ValueDefinition);
				return property == ArmageddonPlayerHealth.BasePools
					? (IProgVariable)new CollectionVariable(xml.Elements().Select(x => new NumberVariable(decimal.Parse(x.Value, CultureInfo.InvariantCulture))).ToList(), ProgVariableTypes.Number)
					: new NumberVariable(decimal.Parse(xml.Value, CultureInfo.InvariantCulture));
			});
		});
		var endurance = ArmageddonWorldfileWriter.ResolveRecoveryAttribute(db.TraitDefinitions.Where(x => x.Type == 1).ToArray());
		var trait = Mock.Of<ITraitDefinition>(x => x.Id == endurance.Id); world.SetupGet(x => x.Traits).Returns(new All<ITraitDefinition> { trait });
		var warrior = Mock.Of<IChargenRole>(x => x.Id == 700 && x.Name == "Warrior" && x.RoleType == ChargenRoleType.Class);
		world.SetupGet(x => x.Roles).Returns(new All<IChargenRole> { warrior });
		var actor = Actor(); actor.SetupGet(x => x.Gameworld).Returns(world.Object); actor.SetupGet(x => x.Roles).Returns(new List<IChargenRole>());
		double currentEndurance = 15; actor.Setup(x => x.TraitValue(trait, It.IsAny<TraitBonusContext>())).Returns(() => currentEndurance);
		var humanId = db.SeederManagedRecords.Single(x => x.StableKey == "arm.race.human").LogicalId!.Value;
		var race = Item<IRace>(humanId, ProgVariableTypes.Race); actor.Setup(x => x.GetProperty("race")).Returns(race.Object);
		var expression = new MudSharp.Body.Traits.TraitExpression(db.TraitExpressions.Single(x => x.Name == "human Player Health Capacity"), world.Object);
		Assert.AreEqual(100.5d, expression.Evaluate(actor.Object), 1e-8);
		actor.SetupGet(x => x.Roles).Returns(new List<IChargenRole> { warrior });
		Assert.AreEqual(105.5d, expression.Evaluate(actor.Object), 1e-8);
		currentEndurance = 20; Assert.AreEqual(130.5d, expression.Evaluate(actor.Object), 1e-8);
		var bonus = db.VariableValues.Single(x => x.ReferenceId == humanId && x.ReferenceProperty == ArmageddonPlayerHealth.SizeBonus);
		bonus.ValueDefinition = "<var>7</var>";
		Assert.AreEqual(137.5d, expression.Evaluate(actor.Object), 1e-8, "Register edits affect existing formulas without reseeding.");
	}

	[TestMethod]
	public void PrivateCorpus_GenericAiCompilesEverySourceTargetFilter()
	{
		using var db = ArmageddonInstallationSettingsTests.Fixture();
		ArmageddonInstallation.Prepare(ArmageddonInstallationSettingsTests.Factory(db));
		var source = ArmageddonWorldfileTestData.Load().ForInstallation();
		var writer = new ArmageddonWorldfileWriter(db, source);
		foreach (var npc in source.Npcs)
		{
			var template = new NpcTemplate { Id = npc.Vnum + 1000000, Definition = "<Definition/>" };
			db.NpcTemplates.Add(template); Field<Dictionary<int, NpcTemplate>>(writer, "_npcs").Add(npc.Vnum, template);
		}
		db.LegalAuthorities.Add(new LegalAuthority { Id = 9000, Name = "Allanak Militia" });
		db.SaveChanges();
		typeof(ArmageddonWorldfileWriter).GetMethod("InstallGenericNpcAi", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(writer, null);
		using var compiler = new OfflineProgCompilation(db.FutureProgs.ToArray(), db.VariableDefinitions.ToArray());
		foreach (var prog in db.FutureProgs.ToArray()) compiler.Compile(prog.Id);
		foreach (var npc in source.Npcs)
		{
			var template = db.NpcTemplates.Local.Single(x => x.Id == npc.Vnum + 1000000);
			Assert.AreEqual((npc.Flags[0] & 32) != 0 || (npc.Flags[0] & (1 << 29)) == 0 ? 1 : 0, template.NpctemplatesArtificalIntelligences.Count(x => db.ArtificialIntelligences.Local.Single(y => y.Id == x.AiId).Type == "Aggressor"), "Aggression " + npc.Vnum);
			Assert.AreEqual((npc.Flags[0] & 2) == 0 ? 1 : 0, template.NpctemplatesArtificalIntelligences.Count(x => db.ArtificialIntelligences.Local.Single(y => y.Id == x.AiId).Type == "Wanderer"), "Wandering " + npc.Vnum);
		}
	}
}
