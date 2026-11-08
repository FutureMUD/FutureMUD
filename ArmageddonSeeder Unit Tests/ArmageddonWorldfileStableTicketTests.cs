#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonWorldfileStableTicketTests
{
	[TestMethod]
	public void HistoricalClaims_PrivateArchive_RejectMissingAnimalsAndUnservedOffices()
	{
		ArmageddonWorldfileTestData.RequireAvailable();
		using var db = ArmageddonInstallationSettingsTests.Fixture();
		ArmageddonInstallation.Prepare(ArmageddonInstallationSettingsTests.Factory(db));
		var source = ArmageddonWorldfileTestData.Load().ForInstallation();
		var writer = new ArmageddonWorldfileWriter(db, source);
		foreach (var room in source.Rooms)
			Field<Dictionary<int, Room>>(writer, "_rooms")[room.Vnum] = new Room { Id = room.Vnum + 900000 };
		foreach (var npc in source.Npcs)
			Field<Dictionary<int, NpcTemplate>>(writer, "_npcs")[npc.Vnum] = new NpcTemplate { Id = npc.Vnum + 900000 };
		var resolve = typeof(ArmageddonWorldfileWriter).GetMethod("ResolveHistoricalStableClaim", BindingFlags.NonPublic | BindingFlags.Instance)!;
		var tickets = source.Resets.Where(x => (x.Opcode is "O" or "G" or "E" or "P") && x.Arguments[1] == 1599).ToArray();
		Assert.AreEqual(56, tickets.Length);
		Assert.AreEqual(36, tickets.Count(x => resolve.Invoke(writer, [x]) is not null));
		Assert.IsTrue(tickets.All(x => source.Zones.Single(z => z.Index == x.Zone).Fields[2] == 0));
	}

	[TestMethod]
	public void HistoricalClaim_ValidAndMissingPayloads_CompileOnlyNativeOneTimeClaims()
	{
		using var db = ArmageddonInstallationSettingsTests.Fixture();
		ArmageddonInstallation.Prepare(ArmageddonInstallationSettingsTests.Factory(db));
		var keeper = new ArmWorldNpc(new("fixture", 1), 101, -1, "keeper", "a keeper", "", "", "",
			[], [], [], [], 0, [], [], ["C rent animal_to_store", "C offer animal_to_store"], []);
		var mobile = new ArmWorldReset(new("fixture", 1), 1, "M", [0, 101, 1, 201], [], []);
		var source = new ArmageddonWorldfilePackage { Npcs = [keeper], Resets = [mobile] };
		var writer = new ArmageddonWorldfileWriter(db, source);
		Field<Dictionary<int, Room>>(writer, "_rooms").Add(201, new Room { Id = 9201 });
		Field<Dictionary<int, NpcTemplate>>(writer, "_npcs").Add(301, new NpcTemplate { Id = 9301 });
		var valid = new ArmWorldReset(new("fixture", 2), 1, "P", [1, 1599, 1, 401, 201, 0, 301, 0, 0, 0, 0], [], []);
		var method = typeof(ArmageddonWorldfileWriter).GetMethod("HistoricalStableTicketReset", BindingFlags.NonPublic | BindingFlags.Instance)!;
		var body = (string)method.Invoke(writer, [valid, 0, "cmd0"])!;
		StringAssert.Contains(body, "stockstablemount(9301, tolocation(9201)");
		StringAssert.Contains(body, "setregister @ticketorigin \"armw_stable_claim_1_2\" true");
		Assert.IsFalse(body.Contains("loaditem(", StringComparison.Ordinal));
		foreach (var values in new[] { new[] { 201, 0, 0, 0, 0, 0 }, new[] { 999, 0, 301, 0, 0, 0 } })
		{
			var args = new[] { 1, 1599, 1, 401, values[0], values[1], values[2], values[3], values[4], values[5], 0 };
			var invalid = valid with { Arguments = args };
			var omitted = (string)method.Invoke(writer, [invalid, 1, "cmd1"])!;
			Assert.AreEqual("cmd1 = true", omitted, "Omitting a receipt must preserve sibling reset execution.");
		}
		var prog = new MudSharp.Models.FutureProg
		{
			Id = 99001, FunctionName = "StableClaimFixture", Category = "Fixture", Subcategory = "Fixture",
			ReturnTypeDefinition = ProgVariableTypes.Item.ToStorageString(),
			FunctionText = "var lastobj = @inputitem\nvar ticketorigin = @inputroom\nvar cmd0 as boolean\n" + body +
				"\nif (@cmd0)\nreturn @lastobj\nend if\nreturn null(\"item\")"
		};
		prog.FutureProgsParameters.Add(new() { ParameterIndex = 0, ParameterName = "inputitem", ParameterTypeDefinition = ProgVariableTypes.Item.ToStorageString() });
		prog.FutureProgsParameters.Add(new() { ParameterIndex = 1, ParameterName = "inputroom", ParameterTypeDefinition = ProgVariableTypes.Location.ToStorageString() });
		db.FutureProgs.Add(prog);
		db.VariableDefinitions.Add(new VariableDefinition { OwnerTypeDefinition = ProgVariableTypes.Character.ToStorageString(),
			Property = ArmageddonWorldfileWriter.MountTrainedRegister, ContainedTypeDefinition = ProgVariableTypes.Number.ToStorageString() });
		db.SaveChanges();
		using var compiler = new OfflineProgCompilation(db.FutureProgs.Include(x => x.FutureProgsParameters).ToArray(), db.VariableDefinitions.ToArray());
		var compiled = compiler.Compile(prog.Id);
		var world = (IFuturemud)typeof(OfflineProgCompilation).GetField("_world", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(compiler)!;
		var parent = new Mock<MudSharp.GameItems.IGameItem>();
		parent.SetupGet(x => x.GetObject).Returns(parent.Object);
		parent.SetupGet(x => x.Type).Returns(ProgVariableTypes.Item);
		parent.Setup(x => x.GetProperty("container")).Returns(new MudSharp.FutureProg.Variables.NullVariable(ProgVariableTypes.Item));
		parent.Setup(x => x.GetProperty("iscontainer")).Returns(new MudSharp.FutureProg.Variables.BooleanVariable(true));
		var room = new Mock<MudSharp.Construction.IRoom>();
		room.SetupGet(x => x.Id).Returns(987L);
		room.SetupGet(x => x.Type).Returns(ProgVariableTypes.Location);
		room.SetupGet(x => x.GetObject).Returns(room.Object);
		Assert.IsTrue(world.VariableRegister.SetValue(room.Object, "armw_stable_claim_1_2", new MudSharp.FutureProg.Variables.BooleanVariable(true)));
		Assert.AreSame(parent.Object, compiled.Execute(parent.Object, room.Object), "A guarded boot must preserve its following conditional sibling context.");
	}

	private static T Field<T>(object target, string name) =>
		(T)target.GetType().GetField(name, BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(target)!;
}
