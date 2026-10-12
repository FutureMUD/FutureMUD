#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

internal sealed partial class ArmageddonCatalogueOwnership
{
	private static IReadOnlyDictionary<Type, string[]> AnatomyOwnedFields(Dictionary<Type, string[]> fields)
	{
		fields.Add(typeof(BodyProto), ["Name", "CountsAsId", "WearSizeParameterId", "WielderDescriptionPlural", "WielderDescriptionSingle", "ConsiderString", "StaminaRecoveryProgId", "MinimumLegsToStand", "MinimumWingsToFly", "LegDescriptionSingular", "LegDescriptionPlural", "DefaultSmashingBodypartId", "NameForTracking", "PlanarData"]);
		fields.Add(typeof(BodypartShape), ["Name"]);
		fields.Add(typeof(BodypartProto), ["BodypartType", "BodyId", "Name", "Description", "CountAsId", "BodypartShapeId", "DisplayOrder", "MaxLife", "SeveredThreshold", "SeverFormula", "PainModifier", "BleedModifier", "RelativeHitChance", "Location", "Alignment", "Unary", "MaxSingleSize", "IsOrgan", "WeightLimit", "IsCore", "StunModifier", "DamageModifier", "DefaultMaterialId", "Significant", "UseLimbSeverDescription", "RelativeInfectability", "HypoxiaDamagePerTick", "IsVital", "ArmourTypeId", "ImplantSpace", "ImplantSpaceOccupied", "Size"]);
		fields.Add(typeof(Limb), ["Name", "RootBodypartId", "LimbType", "RootBodyId", "LimbDamageThresholdMultiplier", "LimbPainThresholdMultiplier"]);
		fields.Add(typeof(MoveSpeed), ["BodyProtoId", "Multiplier", "Alias", "FirstPersonVerb", "ThirdPersonVerb", "PresentParticiple", "PositionId", "StaminaMultiplier"]);
		fields.Add(typeof(BodypartProtoAlignmentHits), ["BodypartProtoId", "Alignment", "HitChance"]);
		fields.Add(typeof(BodypartProtoOrientationHits), ["BodypartProtoId", "Orientation", "HitChance"]);
		return fields;
	}

	private bool NameCollision<T>(string key, T desired, string field, string name) where T : class
	{
		var self = _staged.TryGetValue(key, out var staged) && ReferenceEquals(staged, desired)
			? (long)_db.Entry(desired).Property("Id").CurrentValue! : 0;
		if (desired is BodypartProto part)
			return _db.BodypartProtos.Any(x => x.Id != self && x.BodyId == part.BodyId && x.Name == name);
		if (desired is Limb limb)
			return _db.Limbs.Any(x => x.Id != self && x.RootBodyId == limb.RootBodyId && x.Name == name);
		return _db.Set<T>().Any(x => EF.Property<long>(x, "Id") != self && EF.Property<string>(x, field) == name);
	}

	private bool StagedNameCollision(string key, object row) => row switch
	{
		BodyProto body => NameCollision(key, body, "Name", body.Name),
		BodypartProto part => NameCollision(key, part, "Name", part.Name),
		Limb limb => NameCollision(key, limb, "Name", limb.Name),
		Race race => NameCollision(key, race, "Name", race.Name),
		Culture culture => NameCollision(key, culture, "Name", culture.Name),
		Ethnicity ethnicity => NameCollision(key, ethnicity, "Name", ethnicity.Name),
		Merit merit => NameCollision(key, merit, "Name", merit.Name),
		NpcTemplate template => NameCollision(key, template, "Name", template.Name),
		ArtificialIntelligence ai => NameCollision(key, ai, "Name", ai.Name),
		WearProfile profile => NameCollision(key, profile, "Name", profile.Name),
		Drug drug => NameCollision(key, drug, "Name", drug.Name),
		_ => throw new InvalidOperationException($"{key}: undeclared staged name scope.")
	};

	private void LoadAnatomyChildren(object row)
	{
		if (row is BodyProto body)
		{
			_db.Entry(body).Collection(x => x.BodyProtosPositions).Load();
			_db.Entry(body).Collection(x => x.BodyProtosAdditionalBodyparts).Load();
		}
		if (row is BodypartProto part)
		{
			_db.Entry(part).Collection(x => x.BodypartProtoBodypartProtoUpstreamChildNavigation).Load();
			_db.Entry(part).Collection(x => x.BodypartInternalInfosBodypartProto).Load();
			_db.Entry(part).Collection(x => x.BoneOrganCoveragesBone).Load();
		}
		if (row is Limb limb)
		{
			_db.Entry(limb).Collection(x => x.LimbsBodypartProto).Load();
			_db.Entry(limb).Collection(x => x.LimbsSpinalParts).Load();
		}
	}

	private static void AnatomyFields(object row, Dictionary<string, string> fields)
	{
		if (row is BodyProto body)
		{
			fields["Positions"] = JsonSerializer.Serialize(body.BodyProtosPositions.Select(x => x.Position).Order().ToArray());
			fields["AdditionalParts"] = JsonSerializer.Serialize(body.BodyProtosAdditionalBodyparts.OrderBy(x => x.BodypartId).ThenBy(x => x.Usage)
				.Select(x => new PartUsage(x.BodypartId, x.Usage)).ToArray());
		}
		if (row is BodypartProto part)
		{
			fields["Upstreams"] = JsonSerializer.Serialize(part.BodypartProtoBodypartProtoUpstreamChildNavigation.Select(x => x.Parent).Order().ToArray());
			fields["Organs"] = JsonSerializer.Serialize(part.BodypartInternalInfosBodypartProto.OrderBy(x => x.InternalPartId)
				.Select(x => new OrganLocation(x.InternalPartId, x.HitChance, x.IsPrimaryOrganLocation, x.ProximityGroup)).ToArray());
			fields["BoneCoverage"] = JsonSerializer.Serialize(part.BoneOrganCoveragesBone.OrderBy(x => x.OrganId)
				.Select(x => new BoneCoverage(x.OrganId, x.CoverageChance)).ToArray());
		}
		if (row is Limb limb)
		{
			fields["Parts"] = JsonSerializer.Serialize(limb.LimbsBodypartProto.Select(x => x.BodypartProtoId).Order().ToArray());
			fields["SpinalParts"] = JsonSerializer.Serialize(limb.LimbsSpinalParts.Select(x => x.BodypartProtoId).Order().ToArray());
		}
	}

	private void Replace<T, TKey>(ICollection<T> rows, IEnumerable<T> desired, Func<T, TKey> key,
		Action<T, T>? update = null) where T : class where TKey : notnull
	{
		var next = desired.ToDictionary(key);
		foreach (var row in rows.ToArray())
		{
			if (next.Remove(key(row), out var replacement)) update?.Invoke(row, replacement);
			else { _db.Remove(row); rows.Remove(row); }
		}
		foreach (var row in next.Values) rows.Add(row);
	}

	private void ApplyAnatomyChildren(object row, IReadOnlyDictionary<string, string> current, IReadOnlyDictionary<string, string> merged)
	{
		bool Changed(string field) => current[field] != merged[field];
		T[] Values<T>(string field) => JsonSerializer.Deserialize<T[]>(merged[field])!;
		if (row is BodyProto body)
		{
			if (Changed("Positions")) Replace(body.BodyProtosPositions, Values<int>("Positions").Select(x => new BodyProtosPositions { BodyProtoId = body.Id, Position = x }), x => x.Position);
			if (Changed("AdditionalParts")) Replace(body.BodyProtosAdditionalBodyparts, Values<PartUsage>("AdditionalParts").Select(x => new BodyProtosAdditionalBodyparts { BodyProtoId = body.Id, BodypartId = x.Id, Usage = x.Usage }), x => (x.BodypartId, x.Usage));
		}
		if (row is BodypartProto part)
		{
			if (Changed("Upstreams")) Replace(part.BodypartProtoBodypartProtoUpstreamChildNavigation, Values<long>("Upstreams").Select(x => new BodypartProtoBodypartProtoUpstream { Child = part.Id, Parent = x }), x => x.Parent);
			if (Changed("Organs")) Replace(part.BodypartInternalInfosBodypartProto, Values<OrganLocation>("Organs").Select(x => new BodypartInternalInfos { BodypartProtoId = part.Id, InternalPartId = x.Id, HitChance = x.Chance, IsPrimaryOrganLocation = x.Primary, ProximityGroup = x.Group }), x => x.InternalPartId,
				(actual, next) => { actual.HitChance = next.HitChance; actual.IsPrimaryOrganLocation = next.IsPrimaryOrganLocation; actual.ProximityGroup = next.ProximityGroup; });
			if (Changed("BoneCoverage")) Replace(part.BoneOrganCoveragesBone, Values<BoneCoverage>("BoneCoverage").Select(x => new BoneOrganCoverage { BoneId = part.Id, OrganId = x.Id, CoverageChance = x.Chance }), x => x.OrganId,
				(actual, next) => actual.CoverageChance = next.CoverageChance);
		}
		if (row is Limb limb)
		{
			if (Changed("Parts")) Replace(limb.LimbsBodypartProto, Values<long>("Parts").Select(x => new LimbBodypartProto { LimbId = limb.Id, BodypartProtoId = x }), x => x.BodypartProtoId);
			if (Changed("SpinalParts")) Replace(limb.LimbsSpinalParts, Values<long>("SpinalParts").Select(x => new LimbsSpinalPart { LimbId = limb.Id, BodypartProtoId = x }), x => x.BodypartProtoId);
		}
	}

	private sealed record PartUsage(long Id, string Usage);
	private sealed record OrganLocation(long Id, double Chance, bool Primary, string? Group);
	private sealed record BoneCoverage(long Id, double Chance);
}
