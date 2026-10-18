#nullable enable
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.FutureProg;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

public static partial class ArmageddonStockSupportInstaller
{
	public const string GatheringAvailabilityKey = "arm.prog.gathering_available";

	private static FutureProg GatheringAvailabilityProg() => Prog("ArmageddonGatheringAvailable",
		ProgVariableTypes.Boolean, "return false", ("actor", ProgVariableTypes.Character),
		("owner", ProgVariableTypes.Character), ("capability", ProgVariableTypes.MagicCapability),
		("methodKey", ProgVariableTypes.Text), ("amount", ProgVariableTypes.Number),
		("room", ProgVariableTypes.Location));

	// Gathering has its own admission check; Casting policy alone does not deny it.
	// Reject an incompatible signature before writes; preserve builder policy bodies through ownership reconciliation.
	internal static void ValidateGatheringGate(FuturemudDatabaseContext db)
	{
		var desired = GatheringAvailabilityProg();
		var claim = db.SeederManagedRecords.AsNoTracking().SingleOrDefault(x =>
			x.Seeder == ArmageddonMagicInstaller.Package && x.StableKey == GatheringAvailabilityKey);
		if (claim is null)
		{
			if (db.FutureProgs.Any(x => x.FunctionName == desired.FunctionName))
				throw new InvalidOperationException($"{GatheringAvailabilityKey}: unowned gathering availability prog collision; no adoption.");
			return;
		}

		var gate = Require<FutureProg>(db, GatheringAvailabilityKey, _ => true, Module);
		db.Entry(gate).Collection(x => x.FutureProgsParameters).Load();
		var expected = desired.FutureProgsParameters.OrderBy(x => x.ParameterIndex)
			.Select(x => (x.ParameterIndex, x.ParameterName, x.ParameterTypeDefinition));
		var actual = gate.FutureProgsParameters.OrderBy(x => x.ParameterIndex)
			.Select(x => (x.ParameterIndex, x.ParameterName, x.ParameterTypeDefinition));
		if (gate.ReturnTypeDefinition != desired.ReturnTypeDefinition ||
			gate.AcceptsAnyParameters || gate.StaticType != desired.StaticType || !actual.SequenceEqual(expected))
			throw new InvalidOperationException($"{GatheringAvailabilityKey}: stock gathering availability requires its exact six-parameter signature; preserve and resolve the edited gate explicitly.");
	}
}
