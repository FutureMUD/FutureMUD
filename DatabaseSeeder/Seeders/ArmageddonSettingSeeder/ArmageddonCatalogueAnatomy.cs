#nullable enable
extern alias EngineCompiler;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Body;
using MudSharp.Database;
using MudSharp.Models;
using PositionFlying = EngineCompiler::MudSharp.Body.Position.PositionStates.PositionFlying;
using PositionStanding = EngineCompiler::MudSharp.Body.Position.PositionStates.PositionStanding;
using PositionFloatingInZeroGravity = EngineCompiler::MudSharp.Body.Position.PositionStates.PositionFloatingInZeroGravity;

namespace DatabaseSeeder.Seeders;

/// <summary>Independent native anatomy with explicitly owned parts, organs, limbs and movement links.</summary>
public static class ArmageddonCatalogueAnatomy
{
	public static ArmageddonInstallResult Install(Func<FuturemudDatabaseContext> factory,
		Action<ArmageddonInstallCheckpoint>? checkpoint = null)
	{
		using var db = factory();
		var settings = ArmageddonInstallation.Read(db) ?? throw new InvalidOperationException("Fix Armageddon settings before creature anatomy.");
		var errors = ArmageddonInstallation.ValidateFoundations(db, settings).ToList();
		if (errors.Count != 0) return new(ArmageddonInstallStatus.Blocked, errors, new Dictionary<string, long>());
		var messages = new List<string>();
		var ids = new Dictionary<string, long>(StringComparer.Ordinal);
		var committing = false;
		var committed = false;
		try
		{
			using var transaction = db.Database.IsRelational() ? db.Database.BeginTransaction(IsolationLevel.Serializable) : null;
			var owned = new ArmageddonCatalogueOwnership(db, settings.NativeBindings[ArmageddonNativeBinding.BuilderAccount], messages);
			var sourceId = ArmageddonCatalogueLinks.Bind(db, "arm.binding.anatomy.source", nameof(BodyProto), () =>
				db.Races.Single(x => x.Id == settings.NativeBindings[ArmageddonNativeBinding.HumanRace]).BaseBodyId);
			var source = Source.Read(db, sourceId);
			checkpoint?.Invoke(ArmageddonInstallCheckpoint.PreflightComplete);
			var wingRootShape = owned.Put("arm.anatomy.shape.wingbase", new BodypartShape { Name = "Armageddon Wing Base" }, []);
			var wingShape = owned.Put("arm.anatomy.shape.wing", new BodypartShape { Name = "Armageddon Wing" }, []);
			foreach (var blueprint in new ArmageddonStockCatalogue().Blueprints.Where(x => x.Kind == ArmageddonBlueprintKind.RaceBundle))
			{
				var body = Build(owned, source, blueprint, wingRootShape.Id, wingShape.Id);
				ids.Add(blueprint.Key + ".body", body.Id);
			}
			owned.Flush();
			checkpoint?.Invoke(ArmageddonInstallCheckpoint.ContentCreated);
			checkpoint?.Invoke(ArmageddonInstallCheckpoint.BeforeCommit);
			committing = true;
			transaction?.Commit();
			committed = true;
			checkpoint?.Invoke(ArmageddonInstallCheckpoint.AfterCommit);
			messages.Add("S3 anatomy batch committed: seven independent native bodies with owned organ, limb and movement relationships. Race/template/controller qualification remains in progress.");
			return new(ArmageddonInstallStatus.Completed, messages.AsReadOnly(), ids);
		}
		catch (Exception error)
		{
			messages.Add((committed ? "Committed; confirmation failed. Rerun preserves owned identities. " : committing ? "Commit outcome unknown; reconnect before rerunning. " : "Anatomy batch failed before commit; relational transaction rolled back. ") + error.Message);
			if (!committed) ids.Clear();
			return new(committed ? ArmageddonInstallStatus.CommittedConfirmationFailed : committing ? ArmageddonInstallStatus.CommitOutcomeUnknown : ArmageddonInstallStatus.Failed, messages.AsReadOnly(), ids);
		}
	}

	private static BodyProto Build(ArmageddonCatalogueOwnership owned, Source source, ArmageddonStockBlueprint blueprint, long wingRootShape, long wingShape)
	{
		var key = blueprint.Key + ".body";
		var plan = blueprint.Configuration.GetProperty("body_plan").GetString()!;
		var quadruped = plan.Contains("quadruped", StringComparison.Ordinal);
		var winged = plan.StartsWith("winged ", StringComparison.Ordinal);
		var body = owned.Copy(source.Body);
		body.Id = owned.Id<BodyProto>(key);
		body.Name = "Armageddon " + blueprint.Name;
		body.CountsAsId = null;
		body.ConsiderString = "A conjured creature with " + plan + " anatomy.";
		body.NameForTracking = blueprint.Name;
		body.MinimumLegsToStand = quadruped ? 4 : 2;
		body.MinimumWingsToFly = 2;
		owned.Stage(key, body, ["arm.binding.anatomy.source"]);
		var parts = source.Parts.ToDictionary(x => x.Id, owned.Copy);
		foreach (var entry in parts)
		{
			entry.Value.Id = owned.Id<BodypartProto>($"{key}.part.{entry.Key}");
			entry.Value.BodyId = body.Id;
		}
		long Part(long id) => parts.TryGetValue(id, out var part) ? part.Id :
			throw new InvalidOperationException($"{blueprint.Key}: source anatomy relationship points outside its explicit body chain: {id}.");
		foreach (var entry in parts)
		{
			if (entry.Value.CountAsId is { } countAs && parts.ContainsKey(countAs)) entry.Value.CountAsId = Part(countAs);
			owned.Stage($"{key}.part.{entry.Key}", entry.Value, [key]);
		}
		body.DefaultSmashingBodypartId = source.Body.DefaultSmashingBodypartId is { } smashing ? Part(smashing) : null;
		foreach (var link in source.Upstreams) parts[link.Child].BodypartProtoBodypartProtoUpstreamChildNavigation.Add(new() { Child = Part(link.Child), Parent = Part(link.Parent) });
		foreach (var link in source.Organs) parts[link.BodypartProtoId].BodypartInternalInfosBodypartProto.Add(new()
		{ BodypartProtoId = Part(link.BodypartProtoId), InternalPartId = Part(link.InternalPartId), HitChance = link.HitChance, IsPrimaryOrganLocation = link.IsPrimaryOrganLocation, ProximityGroup = link.ProximityGroup });
		foreach (var link in source.Coverage) parts[link.BoneId].BoneOrganCoveragesBone.Add(new()
		{ BoneId = Part(link.BoneId), OrganId = Part(link.OrganId), CoverageChance = link.CoverageChance });
		foreach (var link in source.Usages) body.BodyProtosAdditionalBodyparts.Add(new() { BodyProtoId = body.Id, BodypartId = Part(link.BodypartId), Usage = link.Usage });
		foreach (var hit in source.AlignmentHits) owned.Stage($"{key}.alignment.{hit.Id}", new BodypartProtoAlignmentHits
		{ Id = owned.Id<BodypartProtoAlignmentHits>($"{key}.alignment.{hit.Id}"), BodypartProtoId = Part(hit.BodypartProtoId), Alignment = hit.Alignment, HitChance = hit.HitChance }, [key]);
		foreach (var hit in source.OrientationHits) owned.Stage($"{key}.orientation.{hit.Id}", new BodypartProtoOrientationHits
		{ Id = owned.Id<BodypartProtoOrientationHits>($"{key}.orientation.{hit.Id}"), BodypartProtoId = Part(hit.BodypartProtoId), Orientation = hit.Orientation, HitChance = hit.HitChance }, [key]);
		var limbs = source.Limbs.ToDictionary(x => x.Id, owned.Copy);
		foreach (var entry in limbs)
		{
			var limb = entry.Value;
			limb.Id = owned.Id<Limb>($"{key}.limb.{entry.Key}");
			limb.RootBodyId = body.Id;
			limb.RootBodypartId = Part(limb.RootBodypartId);
			if (quadruped && limb.LimbType == (int)LimbType.Arm) limb.LimbType = (int)LimbType.Leg;
			foreach (var link in source.Members.Where(x => x.LimbId == entry.Key)) limb.LimbsBodypartProto.Add(new() { LimbId = limb.Id, BodypartProtoId = Part(link.BodypartProtoId) });
			foreach (var link in source.Spinal.Where(x => x.LimbId == entry.Key)) limb.LimbsSpinalParts.Add(new() { LimbId = limb.Id, BodypartProtoId = Part(link.BodypartProtoId) });
			owned.Stage($"{key}.limb.{entry.Key}", limb, [key]);
		}
		if (quadruped)
		{
			var standing = source.Parts.First(x => x.BodypartType == (int)BodypartTypeEnum.Standing);
			foreach (var forelimb in source.Limbs.Where(x => x.LimbType == (int)LimbType.Arm))
			{
				var grasp = source.Members.Where(x => x.LimbId == forelimb.Id).Select(x => source.Parts.Single(p => p.Id == x.BodypartProtoId))
					.Single(x => x.BodypartType is (int)BodypartTypeEnum.GrabbingWielding or (int)BodypartTypeEnum.BonyGrabbingWielding);
				parts[grasp.Id].Description = forelimb.Name.StartsWith("Right", StringComparison.OrdinalIgnoreCase) ? "right forepaw" : "left forepaw";
				var padKey = $"{key}.forepad.{forelimb.Id}";
				var pad = owned.Copy(standing);
				pad.Id = owned.Id<BodypartProto>(padKey); pad.BodyId = body.Id; pad.CountAsId = null;
				pad.Name = $"forepad{forelimb.Id}"; pad.Description = parts[grasp.Id].Description + " pad";
				pad.Alignment = grasp.Alignment;
				pad.BodypartProtoBodypartProtoUpstreamChildNavigation.Add(new() { Child = pad.Id, Parent = Part(grasp.Id) });
				owned.Stage(padKey, pad, [key]);
				limbs[forelimb.Id].LimbsBodypartProto.Add(new() { LimbId = limbs[forelimb.Id].Id, BodypartProtoId = pad.Id });
			}
		}
		if (winged)
		{
			var torso = source.Limbs.Single(x => x.LimbType == (int)LimbType.Torso);
			foreach (var side in new[] { "right", "left" })
			{
				var wingKey = key + ".wing." + side;
				var root = owned.Copy(source.Parts.Single(x => x.Id == torso.RootBodypartId));
				root.Id = owned.Id<BodypartProto>(wingKey + ".root"); root.BodyId = body.Id; root.CountAsId = null;
				root.Name = side + "wingbase"; root.Description = side + " wing base";
				root.BodypartShapeId = wingRootShape;
				root.BodypartType = (int)BodypartTypeEnum.BonyDrapeable; root.IsCore = true; root.IsVital = false; root.IsOrgan = 0;
				root.BodypartProtoBodypartProtoUpstreamChildNavigation.Add(new() { Child = root.Id, Parent = Part(torso.RootBodypartId) });
				var wing = owned.Copy(root);
				wing.Id = owned.Id<BodypartProto>(wingKey + ".part"); wing.Name = side + "wing"; wing.Description = side + " wing";
				wing.BodypartType = (int)BodypartTypeEnum.Wing;
				wing.BodypartShapeId = wingShape;
				wing.BodypartProtoBodypartProtoUpstreamChildNavigation.Add(new() { Child = wing.Id, Parent = root.Id });
				owned.Stage(wingKey + ".root", root, [key]); owned.Stage(wingKey + ".part", wing, [key]);
				var limb = new Limb { Id = owned.Id<Limb>(wingKey), Name = side + " wing", LimbType = (int)LimbType.Wing,
					RootBodyId = body.Id, RootBodypartId = root.Id, LimbDamageThresholdMultiplier = 0.5, LimbPainThresholdMultiplier = 0.5 };
				limb.LimbsBodypartProto.Add(new() { LimbId = limb.Id, BodypartProtoId = root.Id });
				limb.LimbsBodypartProto.Add(new() { LimbId = limb.Id, BodypartProtoId = wing.Id });
				foreach (var spinal in source.Spinal.Where(x => x.LimbId == torso.Id)) limb.LimbsSpinalParts.Add(new() { LimbId = limb.Id, BodypartProtoId = Part(spinal.BodypartProtoId) });
				owned.Stage(wingKey, limb, [key]);
			}
		}
		foreach (var position in source.Positions.Where(x => winged || x != PositionFlying.Instance.Id)
			.Concat(winged ? [(int)PositionFlying.Instance.Id] : Array.Empty<int>()).Distinct())
			body.BodyProtosPositions.Add(new() { BodyProtoId = body.Id, Position = position });
		// Native boot may subsequently add a zero-gravity speed to the shared source.
		// An already-owned compatibility speed keeps its original identity and field
		// baselines; never adopt that later source row or collide with builder edits.
		var retainedFloat = owned.Has<MoveSpeed>(key + ".speed.float");
		foreach (var speed in source.Speeds.Where(x => (winged || x.PositionId != PositionFlying.Instance.Id) &&
			(!retainedFloat || x.PositionId != PositionFloatingInZeroGravity.Instance.Id)))
		{
			var speedKey = $"{key}.speed.{speed.Id}";
			var clone = owned.Copy(speed); clone.Id = owned.Id<MoveSpeed>(speedKey); clone.BodyProtoId = body.Id;
			owned.Stage(speedKey, clone, [key]);
		}
		if (winged && !source.Speeds.Any(x => x.PositionId == PositionFlying.Instance.Id))
			owned.Stage(key + ".speed.fly", new MoveSpeed { Id = owned.Id<MoveSpeed>(key + ".speed.fly"), BodyProtoId = body.Id,
			Alias = "fly", PositionId = PositionFlying.Instance.Id, FirstPersonVerb = "fly", ThirdPersonVerb = "flies", PresentParticiple = "flying", Multiplier = 1, StaminaMultiplier = 1 }, [key]);
		// BodyPrototype's native loader would otherwise add this compatibility speed on boot.
		// Author and own it here so cold catalogue loading performs no content repair writes.
		if (retainedFloat || !source.Speeds.Any(x => x.PositionId == PositionFloatingInZeroGravity.Instance.Id))
		{
			var standingSpeed = source.Speeds.First(x => x.PositionId == PositionStanding.Instance.Id);
			owned.Stage(key + ".speed.float", new MoveSpeed { Id = owned.Id<MoveSpeed>(key + ".speed.float"), BodyProtoId = body.Id,
				Alias = "float", PositionId = PositionFloatingInZeroGravity.Instance.Id, FirstPersonVerb = "float", ThirdPersonVerb = "floats", PresentParticiple = "floating", Multiplier = standingSpeed.Multiplier, StaminaMultiplier = standingSpeed.StaminaMultiplier }, [key]);
		}
		return body;
	}

	private sealed record Source(BodyProto Body, BodypartProto[] Parts, Limb[] Limbs,
		BodypartProtoBodypartProtoUpstream[] Upstreams, BodypartInternalInfos[] Organs, BoneOrganCoverage[] Coverage,
		BodyProtosAdditionalBodyparts[] Usages, BodypartProtoAlignmentHits[] AlignmentHits, BodypartProtoOrientationHits[] OrientationHits,
		LimbBodypartProto[] Members, LimbsSpinalPart[] Spinal, int[] Positions, MoveSpeed[] Speeds)
	{
		internal static Source Read(FuturemudDatabaseContext db, long bodyId)
		{
			var chain = new List<BodyProto>();
			var seen = new HashSet<long>();
			for (long? id = bodyId; id is not null;)
			{
				if (!seen.Add(id.Value)) throw new InvalidOperationException("The bound anatomy foundation has cyclic CountsAs relationships.");
				var body = db.BodyProtos.AsNoTracking().Single(x => x.Id == id);
				chain.Add(body); id = body.CountsAsId;
			}
			chain.Reverse();
			var bodies = chain.Select(x => x.Id).ToArray();
			var parts = db.BodypartProtos.AsNoTracking().Where(x => bodies.Contains(x.BodyId)).OrderBy(x => x.Id).ToArray();
			var partIds = parts.Select(x => x.Id).ToArray();
			var limbs = db.Limbs.AsNoTracking().Where(x => bodies.Contains(x.RootBodyId)).OrderBy(x => x.Id).ToArray();
			var limbIds = limbs.Select(x => x.Id).ToArray();
			if (parts.Count(x => x.BodypartType == (int)BodypartTypeEnum.Standing) != 2 ||
				limbs.Count(x => x.LimbType == (int)LimbType.Arm) != 2 || limbs.Count(x => x.LimbType == (int)LimbType.Leg) != 2 ||
				limbs.Count(x => x.LimbType == (int)LimbType.Torso) != 1 || !parts.Any(x => x.IsOrgan != 0))
				throw new InvalidOperationException("The bound native humanoid foundation must have two standing feet, two arm and leg limbs, one torso and ordinary organs.");
			var speeds = db.MoveSpeeds.AsNoTracking().Where(x => bodies.Contains(x.BodyProtoId)).ToArray();
			var selected = chain.SelectMany(x => speeds.Where(s => s.BodyProtoId == x.Id)).GroupBy(x => x.Alias, StringComparer.OrdinalIgnoreCase).Select(x => x.Last()).ToArray();
			if (!selected.Any(x => x.PositionId == PositionStanding.Instance.Id)) throw new InvalidOperationException("The bound anatomy foundation has no native standing movement speed.");
			return new(chain.Last(), parts, limbs,
				// Inherited native limbs/internal links may also serve derived AnimalSeeder bodies.
				// Match the existing flattened clone contract: both endpoints must belong to
				// this exact BodyId/CountsAs chain; unrelated derived anatomy is not imported.
				db.BodypartProtoBodypartProtoUpstream.AsNoTracking().Where(x => partIds.Contains(x.Child) && partIds.Contains(x.Parent)).ToArray(),
				db.BodypartInternalInfos.AsNoTracking().Where(x => partIds.Contains(x.BodypartProtoId) && partIds.Contains(x.InternalPartId)).ToArray(),
				db.BoneOrganCoverages.AsNoTracking().Where(x => partIds.Contains(x.BoneId) && partIds.Contains(x.OrganId)).ToArray(),
				db.BodyProtosAdditionalBodyparts.AsNoTracking().Where(x => bodies.Contains(x.BodyProtoId) && partIds.Contains(x.BodypartId)).ToArray().DistinctBy(x => (x.BodypartId, x.Usage)).ToArray(),
				db.BodypartProtoAlignmentHits.AsNoTracking().Where(x => partIds.Contains(x.BodypartProtoId)).ToArray(),
				db.BodypartProtoOrientationHits.AsNoTracking().Where(x => partIds.Contains(x.BodypartProtoId)).ToArray(),
				db.LimbsBodypartProto.AsNoTracking().Where(x => limbIds.Contains(x.LimbId) && partIds.Contains(x.BodypartProtoId)).ToArray(),
				db.LimbsSpinalParts.AsNoTracking().Where(x => limbIds.Contains(x.LimbId) && partIds.Contains(x.BodypartProtoId)).ToArray(),
				db.BodyProtosPositions.AsNoTracking().Where(x => bodies.Contains(x.BodyProtoId)).Select(x => x.Position).Distinct().ToArray(), selected);
		}
	}
}
