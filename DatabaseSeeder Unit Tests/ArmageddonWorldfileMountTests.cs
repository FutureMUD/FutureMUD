#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonWorldfileMountTests
{
	[TestMethod]
	public void MountOwnershipAndTamingProgramsCompileForARestrictedMountFixture()
	{
		ArmageddonWorldfileTestData.RequireAvailable();
		using var db = ArmageddonInstallationSettingsTests.Fixture();
		ArmageddonInstallation.Prepare(ArmageddonInstallationSettingsTests.Factory(db));
		var archive = ArmageddonWorldfileTestData.Load();
		const int mountFlag = 1 << 26;
		const int aggressiveFlag = 1 << 5;
		var mount = archive.Npcs.First(x => (unchecked((uint)x.Flags[0]) & mountFlag) != 0);
		var mountableRaces = new HashSet<int> { 10, 13, 28, 29, 38, 44, 45, 60, 62, 71, 106, 113, 129, 138 };
		var wild = archive.Npcs.First(x => mountableRaces.Contains(x.Identity[6]) &&
			(unchecked((uint)x.Flags[0]) & (mountFlag | aggressiveFlag)) == 0);
		var source = new ArmageddonWorldfilePackage { Npcs = [mount, wild], Items = [archive.Items.Single(x => x.Vnum == 64129)] };
		var writer = new ArmageddonWorldfileWriter(db, source);
		var templates = Field<Dictionary<int, NpcTemplate>>(writer, "_npcs");
		var families = Field<Dictionary<int, long[]>>(writer, "_npcFamilyIds");
		var progs = Field<List<MudSharp.Models.FutureProg>>(writer, "_progs");
		var nextId = (db.NpcTemplates.Max(x => (long?)x.Id) ?? 0) + 9000;
		var nextProgId = (db.FutureProgs.Max(x => (long?)x.Id) ?? 0) + 9000;
		foreach (var npc in source.Npcs)
		{
			var onLoad = new MudSharp.Models.FutureProg
			{
				Id = nextProgId++, FunctionName = "ArmageddonMountFixtureOnLoad" + npc.Vnum, FunctionText = "return true",
				ReturnTypeDefinition = ProgVariableTypes.Boolean.ToStorageString(), Category = "Fixture", Subcategory = "Fixture"
			};
			onLoad.FutureProgsParameters.Add(new() { ParameterIndex = 0, ParameterName = "npc", ParameterTypeDefinition = ProgVariableTypes.Character.ToStorageString() });
			db.FutureProgs.Add(onLoad);
			progs.Add(onLoad);
			var template = new NpcTemplate
			{
				Id = nextId++, RevisionNumber = 0,
				Definition = $"<Definition><OnLoadProg>{onLoad.Id}</OnLoadProg><TemplateLoadAdditions><Hooks/></TemplateLoadAdditions></Definition>"
			};
			db.NpcTemplates.Add(template);
			templates.Add(npc.Vnum, template);
			families.Add(npc.Vnum, [template.Id]);
		}
		var roleId = 99001L;
		db.SeederManagedRecords.Add(new SeederManagedRecord
		{
			Id = roleId, Seeder = "Armageddon Setting", Module = "Mundane Progression", EntityType = nameof(ChargenRole),
			StableKey = "arm.class.ranger", LogicalId = 99002
		});
		db.TraitDefinitions.Add(new TraitDefinition { Id = 99003, Name = "Riding", Type = 0, TraitGroup = "Skill" });
		db.SaveChanges();

		var method = typeof(ArmageddonWorldfileWriter).GetMethod("InstallArmageddonMountPolicies", BindingFlags.Instance | BindingFlags.NonPublic)!;
		method.Invoke(writer, [new ArtificialIntelligence { Id = 99004, Name = "Fixture Mount", Type = "Mount" }, 99004L]);
		Field<Dictionary<int, GameItemProto>>(writer, "_items").Add(64129, new GameItemProto { Id = 99005, RevisionNumber = 0 });
		typeof(ArmageddonWorldfileWriter).GetMethod("InstallMountWhistle", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(writer, null);
		var permit = (string)typeof(ArmageddonWorldfileWriter).GetField("MountPermitProg", BindingFlags.Static | BindingFlags.NonPublic)!.GetRawConstantValue()!;
		db.FutureProgs.Add(new MudSharp.Models.FutureProg
		{
			Id = db.FutureProgs.Local.Max(x => x.Id) + 1, FunctionName = "ArmageddonWorldfilePermitMountFixture", FunctionText = permit,
			ReturnTypeDefinition = ProgVariableTypes.Boolean.ToStorageString(), Category = "Fixture", Subcategory = "Fixture",
			FutureProgsParameters =
			{
				new() { ParameterIndex = 0, ParameterName = "rider", ParameterTypeDefinition = ProgVariableTypes.Character.ToStorageString() },
				new() { ParameterIndex = 1, ParameterName = "mount", ParameterTypeDefinition = ProgVariableTypes.Character.ToStorageString() }
			}
		});
		db.SaveChanges();

		using var compiler = new OfflineProgCompilation(db.FutureProgs.Include(x => x.FutureProgsParameters).ToArray(), db.VariableDefinitions.ToArray());
		var errors = new List<string>();
		foreach (var prog in db.FutureProgs.Where(x => x.FunctionName.Contains("Mount", StringComparison.Ordinal)).ToArray())
		{
			try { compiler.Compile(prog.Id); }
			catch (InvalidOperationException error)
			{
				errors.Add(prog.FunctionName + ": " + error.Message);
			}
		}
		Assert.AreEqual(0, errors.Count, string.Join("\n", errors));
		Assert.IsTrue(db.VariableDefinitions.Any(x => x.Property == ArmageddonWorldfileWriter.MountOwnerRegister));
		Assert.IsTrue(db.VariableDefinitions.Any(x => x.Property == ArmageddonWorldfileWriter.MountApprovedRidersRegister));
		Assert.IsTrue(db.Hooks.Any(x => x.Name.Contains("Wild Mount", StringComparison.Ordinal) || x.Name.Contains("Mount Handling", StringComparison.Ordinal)));
	}

	private static T Field<T>(ArmageddonWorldfileWriter writer, string name) =>
		(T)typeof(ArmageddonWorldfileWriter).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(writer)!;
}
