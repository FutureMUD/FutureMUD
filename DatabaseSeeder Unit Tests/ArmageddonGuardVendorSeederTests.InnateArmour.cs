#nullable enable

using System;
using System.Linq;
using System.Reflection;
using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Framework;

namespace MudSharp_Unit_Tests;

public partial class ArmageddonGuardVendorSeederTests
{
	[TestMethod]
	public void InnateArmour_ComposesSavedAbsorptionEvenWithoutStartingStatusFlags()
	{
		using var db = Fixture(out var writer, out _);
		var source = Field<ArmageddonWorldfilePackage>(writer, "_source");
		var index = Array.FindIndex(source.Npcs, x => x.Vnum == 46704);
		var flags = source.Npcs[index].Flags.ToArray();
		flags[2] = 0;
		var combat = source.Npcs[index].Combat.ToArray();
		combat[3] = 35;
		source.Npcs[index] = source.Npcs[index] with { Flags = flags, Combat = combat };
		typeof(ArmageddonWorldfileWriter).GetMethod("InstallNpcInitialStates", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(writer, null);
		db.SaveChanges();
		var onload = db.FutureProgs.Single(x => x.FunctionName == "FixtureCash46704");
		StringAssert.Contains(onload.FunctionText, "addphysicalabsorptioneffect(@npc, 3.5)");
		StringAssert.Contains(onload.FunctionText, "setregister @npc \"fixture_cash\" 123");
		using var compiler = new OfflineProgCompilation(db.FutureProgs.Include(x => x.FutureProgsParameters).ToArray(), db.VariableDefinitions.ToArray());
		compiler.Compile(onload.Id);
	}
}
