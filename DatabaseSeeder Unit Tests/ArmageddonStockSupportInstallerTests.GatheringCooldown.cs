#nullable enable
using System;
using System.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Framework;
using MudSharp.FutureProg;
using Db = MudSharp.Models;

namespace MudSharp_Unit_Tests;

public partial class ArmageddonStockSupportInstallerTests
{
	[DataTestMethod]
	[DataRow(10, 0, 599)]
	[DataRow(10, 1, 1199)]
	[DataRow(10, 2, 1799)]
	[DataRow(14, 0, 599)]
	[DataRow(15, 0, 1199)]
	[DataRow(100, 0, 4199)]
	public void SourceGathering_AuthoredCooldown_PreservesSkillBandAndInclusiveRoll(int skill, int roll, int expectedSeconds)
	{
		using var db = Fixture();
		var factory = ArmageddonInstallationSettingsTests.Factory(db);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, ArmageddonStockSupportInstaller.Install(factory).Status);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, ArmageddonProgressionInstaller.InstallGatheringFoundations(factory).Status);
		var installed = ArmageddonStockGatheringInstaller.Install(factory);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, installed.Status, string.Join("\n", installed.Messages));
		db.ChangeTracker.Clear();
		var body = db.FutureProgs.Single(x => x.Id == installed.Identities["arm.prog.gather.land.success"]).FunctionText;
		const string prefix = """setregister @owner "armageddon_gather_until" (now() + timespanfromseconds(""";
		var line = body.Split('\n').Select(x => x.Trim()).Single(x => x.StartsWith(prefix, StringComparison.Ordinal));
		Assert.IsTrue(line.EndsWith("))", StringComparison.Ordinal));
		// Execute the installed expression, controlling only its random draw. FutureProg
		// requires explicit arithmetic grouping around multiplication before subtraction.
		var expression = line[prefix.Length..^2].Replace("random(0, 2)", "@roll", StringComparison.Ordinal);
		var probe = new Db.FutureProg { Id = long.MaxValue, FunctionName = "AuthoredGatheringCooldownProbe",
			ReturnTypeDefinition = ProgVariableTypes.TimeSpan.ToStorageString(), FunctionText = "return timespanfromseconds(" + expression + ")" };
		probe.FutureProgsParameters.Add(new() { ParameterIndex = 0, ParameterName = "skill", ParameterTypeDefinition = ProgVariableTypes.Number.ToStorageString() });
		probe.FutureProgsParameters.Add(new() { ParameterIndex = 1, ParameterName = "roll", ParameterTypeDefinition = ProgVariableTypes.Number.ToStorageString() });
		using var compiler = new OfflineProgCompilation(db.FutureProgs.Include(x => x.FutureProgsParameters).AsNoTracking().ToArray().Append(probe), db.VariableDefinitions.ToArray());
		Assert.AreEqual(TimeSpan.FromSeconds(expectedSeconds), compiler.Compile(probe.Id).Execute<TimeSpan>((decimal)skill, (decimal)roll));
	}
}
