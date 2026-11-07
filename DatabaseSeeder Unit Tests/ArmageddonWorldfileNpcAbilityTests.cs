#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Xml.Linq;
using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Models;
using FutureProg = MudSharp.Models.FutureProg;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonWorldfileNpcAbilityTests
{
	[TestMethod]
	[DataRow(100, 50.0, 7)]
	[DataRow(100, 33.0, 6)]
	[DataRow(100, 20.0, 5)]
	[DataRow(100, 10.0, 4)]
	[DataRow(100, 9.99, 0)]
	[DataRow(0, 50.0, 1)]
	public void Power_SavedProficiencyAndMana_MapToNativeTier(int proficiency, double mana, int grade) =>
		Assert.AreEqual(grade, ArmageddonWorldfileWriter.NpcSpellGrade(proficiency, mana));

	[TestMethod]
	public void Repertoire_RestrictsGuildDispatchAndDoesNotInventUnsavedAbilities()
	{
		string[] names = ["unused", "heal", "fireball", "armor", "kick", "bash", "disarm"];
		var npc = new ArmWorldNpc(new("fixture", 1), 1, 1, "npc", "npc", "npc", "npc", "", [0, 0, 0, 0], [], [],
			[0, 0, 0, 1, 0, 0, 0], 20, [[1, 100], [2, 80], [3, 100], [4, 80], [5, 80], [6, 80]], [], [], []);
		CollectionAssert.AreEqual(new[] { "fireball" }, ArmageddonWorldfileWriter.NpcSavedCombatSpells(npc, names).Select(x => x.Rule.SourceName).ToArray());
		Assert.AreEqual(0, ArmageddonWorldfileWriter.NpcSavedCombatManoeuvres(npc, names).Length, "Caster dispatch does not fall through to ordinary manoeuvres.");
		var identity = npc.Identity.ToArray(); identity[3] = 4;
		CollectionAssert.AreEqual(new[] { "disarm", "kick", "bash" }, ArmageddonWorldfileWriter.NpcSavedCombatManoeuvres(npc with { Identity = identity }, names));
		Assert.AreEqual(0, ArmageddonWorldfileWriter.NpcSavedCombatSpells(npc with { Identity = identity }, names).Count);
	}

	[TestMethod]
	public void PrivateCorpus_CombatPoliciesCompileAndGrantOnlySavedDispatchedSpells()
	{
		using var db = ArmageddonInstallationSettingsTests.Fixture();
		ArmageddonInstallation.Prepare(ArmageddonInstallationSettingsTests.Factory(db));
		var archive = ArmageddonWorldfileTestData.Load().ForInstallation();
		var representatives = archive.Npcs.GroupBy(x => x.Identity[3])
			.Select(group => group.OrderByDescending(x => ArmageddonWorldfileWriter.NpcSavedCombatSpells(x, archive.Skills).Count)
				.ThenByDescending(x => ArmageddonWorldfileWriter.NpcSavedCombatManoeuvres(x, archive.Skills).Length).First()).ToArray();
		var source = new ArmageddonWorldfilePackage { Npcs = representatives, Skills = archive.Skills };
		var writer = new ArmageddonWorldfileWriter(db, source);
		var flags = BindingFlags.Instance | BindingFlags.NonPublic;
		T Field<T>(string name) => (T)typeof(ArmageddonWorldfileWriter).GetField(name, flags)!.GetValue(writer)!;
		using var manifest = JsonDocument.Parse(ArmageddonStockManifest.Read());
		var mapping = manifest.RootElement.GetProperty("entries").EnumerateArray()
			.Where(x => x.GetProperty("key").GetString()!.StartsWith("arm.spell.", StringComparison.Ordinal) &&
				x.TryGetProperty("historical_source", out var history) && history.ValueKind == JsonValueKind.Object &&
				history.TryGetProperty("skill_id", out var id) && id.ValueKind == JsonValueKind.Number)
			.ToDictionary(x => x.GetProperty("historical_source").GetProperty("skill_id").GetInt32(), x => x.GetProperty("key").GetString()!);
		void Own(string key, string type, long id) => db.SeederManagedRecords.Add(new SeederManagedRecord
		{ Seeder = ArmageddonMagicInstaller.Package, Module = "fixture", StableKey = key, EntityType = type, LogicalId = id });
		var needed = source.Npcs.SelectMany(x => ArmageddonWorldfileWriter.NpcSavedCombatSpells(x, source.Skills)).Select(x => x.SourceIndex).Distinct().ToArray();
		var basis = new TraitDefinition { Id = 500000, Name = "NPC casting fixture", Type = 0, OwnerScope = 1, Alias = "npc_fixture", TraitGroup = "Magic", ValueExpression = "", ChargenBlurb = "", DecoratorId = 1 };
		db.TraitDefinitions.Add(basis);
		if (!db.Improvers.Any(x => x.Type == "non-improving")) db.Improvers.Add(new Improver { Id = 500000, Name = "Fixed", Type = "non-improving", Definition = "<Definition/>" });
		foreach (var index in needed)
		{
			var id = 20000 + index;
			db.MagicSpells.Add(new MagicSpell { Id = id, Name = source.Skills[index], Definition = "<Definition><Trigger type='character'/><Effects><Effect type='waystep'/></Effects></Definition>" });
			Own(mapping[index], nameof(MagicSpell), id);
		}
		var capId = 30000;
		db.Drugs.Add(new Drug { Id = 500001, Name = "Fixture Magical Poison" });
		Own("arm.effect.poison", nameof(Drug), 500001);
		db.CombatActions.Add(new CombatAction { Id = 500002, Name = "Beat the Weapon" });
		Own("arm.auxiliary.beat_the_weapon", nameof(CombatAction), 500002);
		db.CombatActions.AddRange(new CombatAction { Id = 500003, Name = "Bash" }, new CombatAction { Id = 500004, Name = "Kick" });
		Own("arm.action.bash", nameof(CombatAction), 500003); Own("arm.action.kick", nameof(CombatAction), 500004);
		foreach (var key in new[] { "fire", "water", "earth", "wind", "shadow", "lightning", "void", "defiler", "southern_templar" })
		{
			var policy = new XElement("Casting", new XAttribute("version", 1), new XAttribute("identity", Guid.NewGuid()),
				new XAttribute("enabled", true), new XAttribute("trait", basis.Id), new XAttribute("source", 10), new XAttribute("reserve", 11),
				new XAttribute("passive", key != "defiler" && key != "southern_templar"), new XAttribute("startingVersion", 1),
				needed.Select(index => new XElement("Admission", new XAttribute("spell", 20000 + index), new XAttribute("trait", basis.Id))));
			db.MagicCapabilities.Add(new MagicCapability { Id = capId, Name = key, CapabilityModel = "skilllevel", Definition = new XElement("Definition", policy).ToString() });
			Own("arm.capability." + key, nameof(MagicCapability), capId++);
		}
		var templates = Field<Dictionary<int, NpcTemplate>>("_npcs");
		var progs = Field<List<FutureProg>>("_progs");
		foreach (var npc in source.Npcs)
		{
			var template = new NpcTemplate { Id = npc.Vnum + 100000, RevisionNumber = 0,
				Definition = "<Definition><SelectedRace>1</SelectedRace><SelectedSkills/><TemplateLoadAdditions><Hooks/></TemplateLoadAdditions></Definition>" };
			db.NpcTemplates.Add(template); templates.Add(npc.Vnum, template);
			var onload = new FutureProg { Id = npc.Vnum + 200000, FunctionName = "ArmageddonWorldfileNpcCash" + npc.Vnum, FunctionText = "return true", ReturnTypeDefinition = ProgVariableTypes.Boolean.ToStorageString() };
			onload.FutureProgsParameters.Add(new FutureProgsParameter { ParameterIndex = 0, ParameterName = "npc", ParameterTypeDefinition = ProgVariableTypes.Character.ToStorageString() });
			db.FutureProgs.Add(onload); progs.Add(onload);
		}
		foreach (var clan in new[] { 4, 14, 24 }) Field<Dictionary<int, Clan>>("_clans").Add(clan, new Clan { Id = 900000 + clan });
		var familySource = source.Npcs.First(x => ArmageddonWorldfileWriter.NpcSavedCombatSpells(x, source.Skills).Count > 0);
		var variant = new NpcTemplate { Id = 1000000 + familySource.Vnum, RevisionNumber = 0, Definition = templates[familySource.Vnum].Definition };
		db.NpcTemplates.Add(variant);
		Field<Dictionary<int, long[]>>("_npcFamilyIds").Add(familySource.Vnum, [templates[familySource.Vnum].Id, variant.Id]);
		db.SaveChanges();
		var originals = db.MagicCapabilities.ToDictionary(x => x.Id, x => x.Definition);
		typeof(ArmageddonWorldfileWriter).GetMethod("InstallNpcCombatAbilities", flags)!.Invoke(writer, null);
		db.SaveChanges();
		foreach (var npc in source.Npcs)
		{
			var saved = ArmageddonWorldfileWriter.NpcSavedCombatSpells(npc, source.Skills);
			if (saved.Count == 0) continue;
			var capability = db.MagicCapabilities.Single(x => x.Name == "Vnum" + npc.Vnum + " Spellcaster");
			var admissions = XElement.Parse(capability.Definition).Element("Casting")!.Elements("Admission").ToArray();
			Assert.AreEqual(saved.Count, admissions.Length, "No extra curriculum for " + npc.Vnum);
			CollectionAssert.AreEqual(saved.Select(x => x.MaximumGrade).ToArray(), admissions.Select(x => (int)x.Attribute("initialGrade")!).ToArray());
			Assert.IsTrue(admissions.All(x => !x.Elements().Any()), "No progression edges or support grants.");
		}
		foreach (var original in originals) Assert.AreEqual(original.Value, db.MagicCapabilities.Find(original.Key)!.Definition);
		Assert.IsTrue(db.Hooks.Any(x => x.TargetEventType == (int)EventType.CharacterCombatAttackResolved));
		Assert.AreEqual(0, db.ChargenRolesMerits.Count(), "No player class grants.");
		Assert.IsTrue(db.FutureProgs.Any(x => x.FunctionText.Contains("performcombataction(@npc, @target, 500002)")), "Disarm uses its native combat action, never the trap-disarming command.");
		Assert.IsTrue(db.RacesCombatActions.Any(x => x.RaceId == 1 && x.CombatActionId == 500002), "Authored disarming has native race availability.");
		var baseDefinition = XElement.Parse(templates[familySource.Vnum].Definition);
		var variantDefinition = XElement.Parse(variant.Definition);
		foreach (var field in new[] { "SelectedSkills", "Merits", "TemplateLoadAdditions" })
			Assert.AreEqual(baseDefinition.Element(field)!.ToString(), variantDefinition.Element(field)!.ToString(), "Family variants preserve " + field);
		StringAssert.Contains(db.FutureProgs.Single(x => x.FunctionName == "ArmageddonWorldfileCombatAllies").FunctionText, "toclan(900024)");
		using var compiler = new OfflineProgCompilation(db.FutureProgs.Include(x => x.FutureProgsParameters).ToArray(), db.VariableDefinitions.ToArray());
		foreach (var prog in db.FutureProgs.ToArray())
		{
			try { compiler.Compile(prog.Id); }
			catch (InvalidOperationException error) { throw new InvalidOperationException(error.Message + "\nGenerated program:\n" + prog.FunctionText, error); }
		}
	}
}
