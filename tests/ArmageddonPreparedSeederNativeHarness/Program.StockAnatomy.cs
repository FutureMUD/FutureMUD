#nullable enable
using System.Diagnostics;
using System.Reflection;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Moq;
using MudSharp.Body;
using MudSharp.Body.Implementations;
using MudSharp.Body.PartProtos;
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Combat;
using MudSharp.Database;
using MudSharp.Form.Material;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using Db = MudSharp.Models;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private static void PreparedStockAnatomyAcceptance(TestDatabase database)
	{
		Func<FuturemudDatabaseContext> factory = () => NewIndependentContext(database.ConnectionString);
		using (var db = factory()) ArmageddonInstallation.Prepare(factory, ArmageddonProgressionMode.Simple,
			capacityAttribute: db.TraitDefinitions.Single(x => x.Name == "Willpower" && x.Type == 1 && x.OwnerScope == 0 && x.DerivedType == 0).Id);
		var before = PreparedDatabaseChecksum(database);
		var faults = 0;
		var rollback = ArmageddonCatalogueAnatomy.Install(factory, stage =>
		{ if (stage == ArmageddonInstallCheckpoint.BeforeCommit) { ++faults; throw new InvalidOperationException("S3 anatomy injected precommit failure"); } });
		Require(faults == 1 && rollback.Status == ArmageddonInstallStatus.Failed && before == PreparedDatabaseChecksum(database),
			"Native anatomy rollback changed tables: " + string.Join("\n", rollback.Messages));
		var ack = ArmageddonCatalogueAnatomy.Install(factory, stage =>
		{ if (stage == ArmageddonInstallCheckpoint.AfterCommit) throw new InvalidOperationException("S3 anatomy injected lost acknowledgement"); });
		Require(ack.Status == ArmageddonInstallStatus.CommittedConfirmationFailed && ack.Identities.Count == 7, string.Join("\n", ack.Messages));
		var committed = PreparedDatabaseChecksum(database);
		var rerun = ArmageddonCatalogueAnatomy.Install(factory);
		Require(rerun.Status == ArmageddonInstallStatus.Completed && committed == PreparedDatabaseChecksum(database),
			"Native anatomy no-op rerun changed tables: " + string.Join("\n", rerun.Messages));
		Require(!rerun.Messages.Any(x => x.Contains("preserved builder edit")), "Native anatomy defaults were mistaken for builder edits: " + string.Join("\n", rerun.Messages));
		RunStockAnatomyReader(database.Name);
		// Exercise application of a changed native source link, retaining the cloned composite keys.
		long sourcePart, sourceOrgan;
		double changedChance;
		using (var db = factory())
		{
			var link = db.BodypartInternalInfos.OrderBy(x => x.BodypartProtoId).ThenBy(x => x.InternalPartId).First();
			sourcePart = link.BodypartProtoId; sourceOrgan = link.InternalPartId;
			changedChance = link.HitChance + 0.01; link.HitChance = changedChance;
			db.SaveChanges();
		}
		var updated = ArmageddonCatalogueAnatomy.Install(factory);
		Require(updated.Status == ArmageddonInstallStatus.Completed, "Native source link update failed: " + string.Join("\n", updated.Messages));
		using (var db = factory())
		{
			foreach (var bodyKey in ack.Identities.Keys)
			{
				var partId = db.SeederManagedRecords.Single(x => x.StableKey == $"{bodyKey}.part.{sourcePart}").LogicalId!.Value;
				var organId = db.SeederManagedRecords.Single(x => x.StableKey == $"{bodyKey}.part.{sourceOrgan}").LogicalId!.Value;
				Require(db.BodypartInternalInfos.Single(x => x.BodypartProtoId == partId && x.InternalPartId == organId).HitChance == changedChance,
					"Native cloned organ location did not update in place.");
			}
			var guardian = ack.Identities["arm.race.air_guardian.body"];
			db.BodyProtos.Single(x => x.Id == guardian).Name = "Builder guardian anatomy";
			db.BodypartProtos.First(x => x.BodyId == guardian && x.IsOrgan == 1).MaxLife = 777;
			db.MoveSpeeds.Single(x => x.BodyProtoId == guardian && x.Alias == "fly").Multiplier = 2;
			db.SaveChanges();
		}
		var edited = PreparedDatabaseChecksum(database);
		var preserved = ArmageddonCatalogueAnatomy.Install(factory);
		Require(preserved.Status == ArmageddonInstallStatus.Completed && edited == PreparedDatabaseChecksum(database),
			"Native anatomy builder edits changed on rerun: " + string.Join("\n", preserved.Messages));
		RunStockAnatomyReader(database.Name);
		Console.WriteLine("ARMS3-anatomy-installation=passed actual-MySQL seven-owned-bodies rollback lost-ack no-op-all-table-checksums stable-native-identities stock-organ-link-update builder-edits; complete-S3:NOT_QUALIFIED");
		RunStockCreatureFoundations(database, factory);
	}

	private static void RunStockAnatomyReader(string name)
	{
		var start = new ProcessStartInfo("dotnet") { UseShellExecute = false, RedirectStandardOutput = true, RedirectStandardError = true, CreateNoWindow = true };
		start.ArgumentList.Add(Assembly.GetExecutingAssembly().Location); start.ArgumentList.Add("--stock-anatomy-reader"); start.ArgumentList.Add(name);
		using var child = Process.Start(start)!;
		var output = child.StandardOutput.ReadToEndAsync(); var errors = child.StandardError.ReadToEndAsync();
		if (!child.WaitForExit(60000)) { child.Kill(true); Require(child.WaitForExit(30000), "Owned anatomy reader did not stop."); throw new TimeoutException("Anatomy reader exceeded 60 seconds."); }
		Require(child.ExitCode == 0, output.GetAwaiter().GetResult() + errors.GetAwaiter().GetResult());
		Console.Write(output.GetAwaiter().GetResult());
	}

	private static int ReadStockAnatomyCatalogue(string name)
	{
		using var database = TestDatabase.OpenExistingOwned(name); ConfigureNativeDatabase(database.ConnectionString);
		var before = PreparedDatabaseChecksum(database);
		using var db = NewIndependentContext(database.ConnectionString);
		var world = new Mock<IFuturemud>(); world.SetupGet(x => x.SaveManager).Returns(new Mock<ISaveManager>().Object);
		world.SetupGet(x => x.FutureProgs).Returns(new All<IFutureProg>());
		world.SetupGet(x => x.ArmourTypes).Returns(new All<IArmourType>());
		var shapes = new All<IBodypartShape>(); world.SetupGet(x => x.BodypartShapes).Returns(shapes);
		foreach (var row in db.BodypartShapes) shapes.Add(new BodypartShape(row, world.Object));
		var materials = new All<ISolid>(); world.SetupGet(x => x.Materials).Returns(materials);
		foreach (var row in db.Materials) { var material = new Mock<ISolid>(); material.SetupGet(x => x.Id).Returns(row.Id); materials.Add(material.Object); }
		var bodies = new All<IBodyPrototype>(); world.SetupGet(x => x.BodyPrototypes).Returns(bodies);
		var parts = new All<IBodypart>(); world.SetupGet(x => x.BodypartPrototypes).Returns(parts);
		var limbs = new All<ILimb>(); world.SetupGet(x => x.Limbs).Returns(limbs);
		PositionState.SetupPositions();
		var ownedIds = db.SeederManagedRecords.Where(x => x.EntityType == nameof(Db.BodyProto) && x.Module == "stock-catalogue").Select(x => x.LogicalId!.Value).ToArray();
		var bodyRows = db.BodyProtos.Include(x => x.WearSizeParameter).Include(x => x.BodyProtosPositions).Include(x => x.MoveSpeeds)
			.Include(x => x.BodyProtosAdditionalBodyparts).Where(x => ownedIds.Contains(x.Id)).AsSplitQuery().ToArray();
		using (new FMDB()) foreach (var row in bodyRows) bodies.Add(new BodyPrototype(row, world.Object));
		var partRows = db.BodypartProtos.Include(x => x.BodypartProtoBodypartProtoUpstreamChildNavigation)
			.Include(x => x.BodypartInternalInfosInternalPart).Include(x => x.BoneOrganCoveragesBone)
			.Where(x => ownedIds.Contains(x.BodyId)).AsSplitQuery().ToArray();
		foreach (var row in partRows)
		{
			var part = BodypartPrototype.LoadFromDatabase(row, world.Object); part.SetBodyProto(bodies.Get(row.BodyId)); parts.Add(part);
		}
		foreach (var row in partRows)
		{
			var part = parts.Get(row.Id)!;
			if (part is IOrganProto organ) foreach (var link in row.BodypartInternalInfosInternalPart)
				parts.Get(link.BodypartProtoId).LinkOrgan(organ, new BodypartInternalInfo(link.HitChance, link.IsPrimaryOrganLocation, link.ProximityGroup));
			if (part is IBone bone)
			{
				bone.PostLoadProcessing(null, row);
				foreach (var link in row.BodypartInternalInfosInternalPart) parts.Get(link.BodypartProtoId)!.LinkBone(bone,
					new BodypartInternalInfo(link.HitChance, link.IsPrimaryOrganLocation, link.ProximityGroup));
				Require(bone.CoveredOrgans.Count() == row.BoneOrganCoveragesBone.Count, "Native bone coverage did not reload.");
			}
			if (part is IExternalBodypart && row.BodypartProtoBodypartProtoUpstreamChildNavigation.FirstOrDefault() is { } upstream) part.LinkUpstream(parts.Get(upstream.Parent)!);
		}
		foreach (var row in bodyRows) bodies.Get(row.Id)!.FinaliseBodyparts(row);
		foreach (var row in db.Limbs.Include(x => x.LimbsBodypartProto).Include(x => x.LimbsSpinalParts).Where(x => ownedIds.Contains(x.RootBodyId)).AsSplitQuery())
			limbs.Add(new Limb(row, world.Object));
		foreach (var blueprint in new ArmageddonStockCatalogue().Blueprints.Where(x => x.Kind == ArmageddonBlueprintKind.RaceBundle))
		{
			var id = db.SeederManagedRecords.Single(x => x.StableKey == blueprint.Key + ".body").LogicalId!.Value;
			var body = bodies.Get(id)!; var plan = blueprint.Configuration.GetProperty("body_plan").GetString()!;
			Require(body.Organs.Any(x => x.BodypartType == BodypartTypeEnum.Heart) && body.Organs.Any(x => x.BodypartType == BodypartTypeEnum.Lung), "Native ordinary organs missing: " + blueprint.Key);
			Require(body.Limbs.Count(x => x.LimbType == LimbType.Leg) == (plan.Contains("quadruped") ? 4 : 2), "Native standing limbs incorrect.");
			Require(body.Limbs.SelectMany(x => x.Parts).OfType<IGrab>().Count() >= 2, "Native grasping parts missing.");
			if (plan.StartsWith("winged ")) Require(body.Limbs.Count(x => x.LimbType == LimbType.Wing) == 2 && body.Speeds.Any(x => x.Position == PositionFlying.Instance), "Native flight anatomy/speed missing.");
		}
		ReadStockCreatureFoundations(db, world);
		Require(before == PreparedDatabaseChecksum(database), "Cold anatomy/foundation reader wrote native content.");
		Console.WriteLine("ARMS3-anatomy-reader=passed fresh-process native-BodyPrototype/bodypart/limb-factories ordinary-organs bone-coverage grasping four-leg-quadrupeds winged-flight-data no-writes; actual-NPC-movement:NOT_RUN full-MUD-boot:NOT_RUN");
		return 0;
	}
}
