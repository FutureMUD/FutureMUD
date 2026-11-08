#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Events;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.Models;
using MudSharp.NPC.AI;

namespace MudSharp_Unit_Tests;

public partial class ArmageddonGuardVendorSeederTests
{
	[TestMethod]
	public void MemoryPolicies_CompileAndPreserveOrdinaryRawAndEscapeRules()
	{
		using var db = ArmageddonInstallationSettingsTests.Fixture();
		ArmageddonInstallation.Prepare(ArmageddonInstallationSettingsTests.Factory(db));
		ArmWorldNpc Npc(int id, int flags) => new(new("fixture", 1), id, 1, "npc", "npc", "", "", "", [flags, 0, 0, 0], [0, 0, 0, 0, 100, 0], [0, 0], [0, 0, 0, 0, 1, 0, 1], 20, [[0, 90]], [], [], []);
		var source = new ArmageddonWorldfilePackage { Npcs = [Npc(1, 0), Npc(2, 2), Npc(3, 1 << 29), Npc(4, 0) with { Skills = [[0, 0]] }], Skills = ["hunt"] };
		var writer = new ArmageddonWorldfileWriter(db, source);
		foreach (var npc in source.Npcs)
		{
			var template = new NpcTemplate { Id = npc.Vnum + 1000, Definition = "<Definition><TemplateLoadAdditions><Hooks/></TemplateLoadAdditions></Definition>" };
			db.NpcTemplates.Add(template); Field<Dictionary<int, NpcTemplate>>(writer, "_npcs").Add(npc.Vnum, template);
		}
		long spellId = 100;
		foreach (var key in new[] { "leaping_lightning", "immolate", "cinder_rain", "ground_tremor", "clay_sentinel", "air_guardian" })
		{
			var kind = key == "ground_tremor" ? "groundtremor" : key is "clay_sentinel" or "air_guardian" ? "createnpc" : "damage";
			db.MagicSpells.Add(new MagicSpell { Id = spellId, Name = key, Blurb = "", Definition = $"<Definition><Effects><Effect type='{kind}'><OnLoadProg>0</OnLoadProg></Effect></Effects></Definition>" });
			db.SeederManagedRecords.Add(new SeederManagedRecord { Seeder = ArmageddonMagicInstaller.Package, Module = "fixture", StableKey = "arm.spell." + key, EntityType = nameof(MagicSpell), LogicalId = spellId++ });
		}
		db.SaveChanges();
		foreach (var method in new[] { "InstallGenericNpcAi", "InstallNpcMemories" })
			typeof(ArmageddonWorldfileWriter).GetMethod(method, BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(writer, null);
		db.SaveChanges();
		var pursuit = db.ArtificialIntelligences.Where(x => x.Type == "TrackingAggressor").AsEnumerable().Select(x => XElement.Parse(x.Definition)).ToArray();
		Assert.AreEqual(2, pursuit.Length, "Sentinel/Unique actors do not hunt; a saved zero-level entry retains the source minimum radius.");
		CollectionAssert.AreEqual(new[] { 5, 9 }, pursuit.Select(x => (int)x.Element("MaximumRange")!).OrderBy(x => x).ToArray());
		Assert.IsTrue(pursuit.All(x => (bool)x.Element("PursuitOnly")!));
		Assert.AreEqual(3, db.Hooks.Count(), "Own damage, witnessed damage and actual escape.");
		Assert.IsTrue(db.Hooks.Any(x => x.TargetEventType == (int)EventType.CharacterEscapedCombat));
		Assert.AreEqual(0, XElement.Parse(db.NpcTemplates.Single(x => x.Id == 1003).Definition).Descendants("Hook").Count());
		decimal tribe = 1;
		var (_, progs, _) = Runtime(db, registers =>
		{
			registers.Setup(x => x.GetType(ProgVariableTypes.Character, "arm_npc_legacy_tribe")).Returns(ProgVariableTypes.Number);
			registers.Setup(x => x.GetValue(It.IsAny<IProgVariable>(), "arm_npc_legacy_tribe")).Returns(() => new NumberVariable(tribe));
			registers.Setup(x => x.GetType(ProgVariableTypes.Character, ArmageddonWorldfileWriter.MountTrainedRegister)).Returns(ProgVariableTypes.Number);
			registers.Setup(x => x.GetValue(It.IsAny<IProgVariable>(), ArmageddonWorldfileWriter.MountTrainedRegister)).Returns(new NumberVariable(0));
		});
		var observer = Actor(); var victim = Actor(); var attacker = Actor();
		observer.Setup(x => x.GetProperty("npc")).Returns(new BooleanVariable(true));
		observer.Setup(x => x.GetProperty("npctemplateid")).Returns(new NumberVariable(1001));
		observer.Setup(x => x.GetProperty("clans")).Returns(new CollectionVariable(new List<MudSharp.Community.IClan>(), ProgVariableTypes.Clan));
		var effects = new List<IEffect>(); observer.SetupGet(x => x.Effects).Returns(effects);
		observer.Setup(x => x.AddEffect(It.IsAny<IEffect>())).Callback<IEffect>(effects.Add);
		observer.Setup(x => x.RemoveEffect(It.IsAny<IEffect>(), true)).Callback<IEffect, bool>((effect, _) => { effects.Remove(effect); effect.RemovalEffect(); });
		progs.GetByName("ArmageddonWorldfileOrdinaryHate").Execute(victim.Object, null, attacker.Object, observer.Object);
		Assert.IsTrue(NpcDispositionMemory.Has(observer.Object, attacker.Object, NpcDisposition.Hate));
		foreach (var effect in effects.ToArray()) observer.Object.RemoveEffect(effect, true);
		tribe = 12;
		progs.GetByName("ArmageddonWorldfileOrdinaryHate").Execute(victim.Object, null, attacker.Object, observer.Object);
		Assert.AreEqual(0, effects.Count, "Ordinary hate preserves militia exclusion.");
		progs.GetByName("ArmageddonWorldfileRawHate").Execute(attacker.Object, observer.Object);
		Assert.IsTrue(NpcDispositionMemory.Has(observer.Object, attacker.Object, NpcDisposition.Hate), "Raw spell hate bypasses that exclusion.");
		progs.GetByName("ArmageddonWorldfileRememberEscape").Execute(observer.Object, null, new List<ICharacter> { attacker.Object, victim.Object });
		Assert.IsFalse(NpcDispositionMemory.Has(observer.Object, attacker.Object, NpcDisposition.Hate));
		Assert.IsTrue(NpcDispositionMemory.Has(observer.Object, attacker.Object, NpcDisposition.Fear));
		Assert.IsTrue(NpcDispositionMemory.Has(observer.Object, victim.Object, NpcDisposition.Fear), "Visible bystanders are included.");
		foreach (var effect in effects.ToArray()) observer.Object.RemoveEffect(effect, true);
	}
}
