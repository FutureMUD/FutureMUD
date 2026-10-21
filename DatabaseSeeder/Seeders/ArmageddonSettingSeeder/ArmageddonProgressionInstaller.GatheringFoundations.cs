#nullable enable
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

public static partial class ArmageddonProgressionInstaller
{
	public const string GatheringTraitKey = "arm.support.gather.skill";
	public const string LandMarkerResourceKey = "arm.resource.land_marker";
	public const string LandProfileKey = "arm.environment.living_land";
	private const string GatheringImproverKey = "arm.improver.gather";

	/// <summary>Source Gather support and inert native Land accounting; no capabilities or players are granted here.</summary>
	public static ArmageddonInstallResult InstallGatheringFoundations(Func<FuturemudDatabaseContext> factory)
	{
		using var db = factory();
		var settings = ArmageddonInstallation.Read(db);
		var messages = new List<string>();
		var ids = new Dictionary<string, long>(StringComparer.Ordinal);
		var committed = false;
		var committing = false;
		try
		{
			if (db.ChangeTracker.HasChanges() || db.Database.CurrentTransaction is not null)
				throw new InvalidOperationException("Use a fresh dedicated gathering-foundation context.");
			if (settings is null) throw new InvalidOperationException("Install fixed Armageddon settings first.");
			messages.AddRange(ArmageddonInstallation.ValidateFoundations(db, settings));
			if (messages.Count > 0) return new(ArmageddonInstallStatus.Blocked, messages, ids);
			var records = db.SeederManagedRecords.Where(x => x.Seeder == ArmageddonMagicInstaller.Package)
				.ToDictionary(x => x.StableKey, StringComparer.Ordinal);
			var specs = new[] { new Spec(typeof(TraitExpression), GatheringTraitKey + ".cap", "Armageddon Gathering cap"),
				new Spec(typeof(Improver), GatheringImproverKey, "Armageddon Gathering practice"),
				new Spec(typeof(TraitDefinition), GatheringTraitKey, "Armageddon Gathering"),
				new Spec(typeof(MagicResource), LandMarkerResourceKey, "Armageddon land accounting marker"),
				new Spec(typeof(MagicGenerator), LandProfileKey, "Armageddon living land") };
			foreach (var spec in specs)
			{
				if (records.TryGetValue(spec.Key, out var claim))
				{
					if (claim.Module != Module || claim.ManifestVersion != Version || claim.EntityType != spec.Type.Name ||
						claim.Retired || claim.LogicalId is not > 0 || claim.RevisionNumber is not null ||
						db.Find(spec.Type, claim.LogicalId.Value) is null || db.SeederManagedRecords.Any(x =>
							x.Id != claim.Id && x.EntityType == claim.EntityType && x.LogicalId == claim.LogicalId))
						throw new InvalidOperationException($"{spec.Key}: incompatible owned gathering foundation; no adoption.");
				}
				else if (Collision(db, spec)) throw new InvalidOperationException($"{spec.Key}: unowned gathering foundation collision; no adoption.");
			}
			long Require(string key, string type)
			{
				if (!records.TryGetValue(key, out var claim) || claim.EntityType != type || claim.Module != Module ||
					claim.ManifestVersion != Version || claim.Retired || claim.LogicalId is not > 0 || claim.RevisionNumber is not null ||
					db.Find(type == nameof(FutureProg) ? typeof(FutureProg) : typeof(TraitExpression), claim.LogicalId.Value) is null)
					throw new InvalidOperationException($"{key}: exact progression dependency is missing.");
				return claim.LogicalId.Value;
			}
			var zero = Require("arm.prog.resource_zero", nameof(FutureProg));
			var capacity = Require("arm.resource.capacity", nameof(TraitExpression));
			using var transaction = db.Database.IsRelational() ? db.Database.BeginTransaction(IsolationLevel.Serializable) : null;
			T Put<T>(string key, T desired) where T : class
			{
				var row = Apply(db, specs.Single(x => x.Key == key), desired, records, messages);
				ids.Add(key, (long)db.Entry(row).Property("Id").CurrentValue!);
				return row;
			}
			var cap = Put(GatheringTraitKey + ".cap", new TraitExpression { Name = "Armageddon Gathering cap", Expression = "90" });
			var improver = Put(GatheringImproverKey, new Improver { Name = "Armageddon Gathering practice", Type = "classic",
				Definition = "<Definition Chance='0.3333333333333333' Expression='1' ImproveOnFail='true' ImproveOnSuccess='false' DifficultyThresholdInterval='0' NoGainSecondsDiceExpression='0' ImprovementProg='0'/>" });
			Put(GatheringTraitKey, new TraitDefinition { Name = "Armageddon Gathering", Type = 0, OwnerScope = 1,
				DecoratorId = settings.NativeBindings[ArmageddonNativeBinding.SkillDecorator], ImproverId = improver.Id,
				ExpressionId = cap.Id, TraitGroup = "Armageddon Support", Hidden = false, ShowInScoreCommand = true,
				ChargenBlurb = "Required native gathering support; source opening10/cap90 on legitimate enrolment. A skill alone grants no capability.",
				AvailabilityProgId = settings.NativeBindings[ArmageddonNativeBinding.AlwaysFalseProg],
				LearnableProgId = settings.NativeBindings[ArmageddonNativeBinding.AlwaysTrueProg],
				TeachableProgId = settings.NativeBindings[ArmageddonNativeBinding.AlwaysFalseProg],
				BranchMultiplier = 0, Alias = "", ValueExpression = "", TeachDifficulty = 7, LearnDifficulty = 7 });
			var marker = Put(LandMarkerResourceKey, new MagicResource { Name = "Armageddon land accounting marker", ShortName = "armland_marker",
				Type = "simple", MagicResourceType = 4, BottomColour = "red", MidColour = "yellow", TopColour = "green",
				Definition = Resource(settings.NativeBindings[ArmageddonNativeBinding.AlwaysFalseProg], zero, capacity,
					settings.CapacityAttribute, false).ToString() });
			Put(LandProfileKey, new MagicGenerator { Name = "Armageddon living land", Type = "environmental",
				Definition = new XElement("Definition", new XAttribute("version", 1), new XElement("PressureHalfLifeSeconds", 3600),
					new XElement("NaturalRepairPerMinute", 0), new XElement("Outputs", new XElement("Output", new XAttribute("resource", marker.Id),
						new XAttribute("basecapacity", 0), new XAttribute("baserate", 0), new XElement("Maximum", "0"), new XElement("Rate", "0"))),
					new XElement("Inputs"), new XElement("Organic", new XAttribute("version", 1), new XAttribute("scaraddendum", false),
						new XElement("Sources", new[] { ("crop", "Crop", new[] { "Crop", "Orchard" }), ("woodland", "Woodland", new[] { "Woodland" }),
							("pasture", "Pasture", new[] { "Pasture" }) }.Select(x => new XElement("Source", new XAttribute("selector", x.Item1),
								new XAttribute("kind", x.Item2), new XElement("Uses", x.Item3.Select(use => new XElement("Use", use))), new XElement("Definitions")))),
						new XElement("Penalties"))).ToString() });
			db.SaveChanges();
			committing = true; transaction?.Commit(); committed = true;
			messages.Add("Gathering foundations committed: source Gather skill/cap/improver and an unbound, zero-output Land accounting profile. No ambient mana, terrain assignments, player grants, refill or vegetation were created.");
			return new(ArmageddonInstallStatus.Completed, messages, ids);
		}
		catch (Exception error)
		{
			if (!committed) ids.Clear();
			messages.Add((committed ? "Committed; confirmation failed. " : committing ? "Commit outcome unknown; reconnect before rerun. " : "Gathering foundations refused before commit. ") + error.Message);
			return new(committed ? ArmageddonInstallStatus.CommittedConfirmationFailed : committing ? ArmageddonInstallStatus.CommitOutcomeUnknown : ArmageddonInstallStatus.Blocked, messages, ids);
		}
	}
}
