#nullable enable

using System.Collections.Generic;
using System.Linq;
using MudSharp.Body;
using MudSharp.GameItems;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

public partial class RobotSeeder
{
	private BodypartProto AddRobotOrgan(
		BodyProto body,
		string alias,
		string description,
		string shapeName,
		BodypartTypeEnum type,
		string parentAlias,
		Alignment alignment,
		Orientation orientation,
		SizeCategory size,
		bool isVital,
		bool isCore,
		IEnumerable<string> internalLocations)
	{
		var organ = AddBodypart(body, alias, description, shapeName, type, parentAlias, alignment, orientation, 0, 60, 75,
			2000 + _context.BodypartProtos.Count(x => x.BodyId == body.Id), _circuitryMaterial, _robotInternalArmour, size,
			isVital: isVital, isCore: isCore, isOrgan: true, bleedModifier: 0.0, painModifier: 0.0, stunModifier: 1.0, damageModifier: 1.0);

		foreach (var locationAlias in internalLocations)
		{
			var location = FindBodypartOnBody(body, locationAlias);
			if (location is null)
			{
				continue;
			}

			if (_context.BodypartInternalInfos.Any(x => x.BodypartProtoId == location.Id && x.InternalPartId == organ.Id))
			{
				continue;
			}

			_context.BodypartInternalInfos.Add(new BodypartInternalInfos
			{
				BodypartProto = location,
				InternalPart = organ,
				HitChance = 1.0,
				IsPrimaryOrganLocation = true,
				ProximityGroup = string.Empty
			});
		}

		_context.SaveChanges();
		return organ;
	}

	private BodypartProto AddBodypart(
		BodyProto body,
		string alias,
		string description,
		string shapeName,
		BodypartTypeEnum type,
		string? parentAlias,
		Alignment alignment,
		Orientation orientation,
		int relativeHitChance,
		int severedThreshold,
		int maxLife,
		int displayOrder,
		Material material,
		ArmourType armour,
		SizeCategory size,
		bool significant = true,
		bool isVital = false,
		bool isCore = false,
		bool isOrgan = false,
		double bleedModifier = 0.0,
		double painModifier = 0.0,
		double damageModifier = 1.0,
		double stunModifier = 1.0,
		BodypartProto? countAs = null)
	{
		var part = new BodypartProto
		{
			Body = body,
			Name = alias,
			Description = description,
			BodypartShape = _context.BodypartShapes.First(x => x.Name == shapeName),
			BodypartType = (int)type,
			Alignment = (int)alignment,
			Location = (int)orientation,
			RelativeHitChance = relativeHitChance,
			SeveredThreshold = severedThreshold,
			MaxLife = maxLife,
			DisplayOrder = displayOrder,
			DefaultMaterial = material,
			Size = (int)size,
			Significant = significant,
			IsVital = isVital,
			IsCore = isCore,
			IsOrgan = isOrgan ? 1 : 0,
			ImplantSpace = isOrgan ? 0.0 : 1.0,
			ImplantSpaceOccupied = 0.0,
			BleedModifier = bleedModifier,
			PainModifier = painModifier,
			DamageModifier = damageModifier,
			StunModifier = stunModifier,
			RelativeInfectability = 0.0,
			ArmourType = armour,
			CountAs = countAs
		};
		_context.BodypartProtos.Add(part);
		_context.SaveChanges();

		if (!string.IsNullOrWhiteSpace(parentAlias) && FindBodypartOnBody(body, parentAlias) is { } parent)
		{
			_context.BodypartProtoBodypartProtoUpstream.Add(new BodypartProtoBodypartProtoUpstream
			{
				ChildNavigation = part,
				ParentNavigation = parent
			});
			_context.SaveChanges();
		}

		return part;
	}

	private void AddMissingBodyMovement(BodyProto source, BodyProto target)
	{
		foreach (var position in _context.BodyProtosPositions.Where(x => x.BodyProtoId == source.Id).ToList())
		{
			if (_context.BodyProtosPositions.Any(x => x.BodyProtoId == target.Id && x.Position == position.Position))
			{
				continue;
			}

			_context.BodyProtosPositions.Add(new BodyProtosPositions
			{
				BodyProto = target,
				Position = position.Position
			});
		}

		var nextId = _context.MoveSpeeds.Select(x => x.Id).AsEnumerable().DefaultIfEmpty(0).Max() + 1;
		foreach (var speed in _context.MoveSpeeds.Where(x => x.BodyProtoId == source.Id).OrderBy(x => x.Id).ToList())
		{
			if (_context.MoveSpeeds.Any(x => x.BodyProtoId == target.Id && x.Alias == speed.Alias))
			{
				continue;
			}

			_context.MoveSpeeds.Add(new MoveSpeed
			{
				Id = nextId++,
				BodyProto = target,
				PositionId = speed.PositionId,
				Alias = speed.Alias,
				FirstPersonVerb = speed.FirstPersonVerb,
				ThirdPersonVerb = speed.ThirdPersonVerb,
				PresentParticiple = speed.PresentParticiple,
				Multiplier = speed.Multiplier,
				StaminaMultiplier = speed.StaminaMultiplier
			});
		}

		_context.SaveChanges();
	}

	private void AddLimb(BodyProto body, string name, LimbType limbType, BodypartProto root, IEnumerable<BodypartProto> parts)
	{
		var limb = new Limb
		{
			Name = name,
			LimbType = (int)limbType,
			RootBody = body,
			RootBodypart = root,
			LimbDamageThresholdMultiplier = 0.5,
			LimbPainThresholdMultiplier = 0.0
		};
		_context.Limbs.Add(limb);
		_context.SaveChanges();

		foreach (var part in parts)
		{
			_context.LimbsBodypartProto.Add(new LimbBodypartProto
			{
				Limb = limb,
				BodypartProto = part
			});
		}

		_context.SaveChanges();
	}

	private void AddLimbPart(BodyProto body, string limbRootAlias, string partAlias)
	{
		var root = FindBodypartOnBody(body, limbRootAlias);
		var part = FindBodypartOnBody(body, partAlias);
		if (root is null || part is null)
		{
			return;
		}

		var limb = _context.Limbs.FirstOrDefault(x => x.RootBodyId == body.Id && x.RootBodypartId == root.Id);
		if (limb is null)
		{
			return;
		}

		if (_context.LimbsBodypartProto.Any(x => x.LimbId == limb.Id && x.BodypartProtoId == part.Id))
		{
			return;
		}

		_context.LimbsBodypartProto.Add(new LimbBodypartProto
		{
			Limb = limb,
			BodypartProto = part
		});
		_context.SaveChanges();
	}

	private void EnsureDefaultSmashingBodypart(BodyProto body, string alias)
	{
		if (FindBodypartOnBody(body, alias) is not { } part)
		{
			return;
		}

		body.DefaultSmashingBodypart = part;
		_context.SaveChanges();
	}

	private BodypartProto? FindBodypartOnBody(BodyProto body, string alias)
	{
		return _context.BodypartProtos.FirstOrDefault(x => x.BodyId == body.Id && x.Name == alias);
	}
}
