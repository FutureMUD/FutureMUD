#nullable enable

using MudSharp.Body.PartProtos;
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Movement;
using MudSharp.Models;
using MudSharp.GameItems.Inventory.Size;
using Limb = MudSharp.Body.PartProtos.Limb;
using MoveSpeed = MudSharp.Movement.MoveSpeed;

namespace MudSharp.Body.Implementations;

public partial class BodyPrototype
{
	public bool HasInstantiatedBodiesInAffectedTree(out string reason)
	{
		var affectedIds = Gameworld.BodyPrototypes
			.Where(IsInAffectedTree)
			.Select(x => x.Id)
			.ToHashSet();
		using (new FMDB())
		{
			var raceId = FMDB.Context.Races
				.Where(x => affectedIds.Contains(x.BaseBodyId))
				.Select(x => x.Id)
				.FirstOrDefault();
			if (raceId != 0)
			{
				reason = $"Race #{raceId:N0} already uses this prototype or one of its descendants. Structural changes could alter its chargen and body anatomy.";
				return true;
			}

			var instantiated = FMDB.Context.Bodies
				.Where(x => affectedIds.Contains(x.BodyPrototypeId))
				.Select(x => x.BodyPrototypeId)
				.FirstOrDefault();
			if (instantiated != 0)
			{
				reason = $"Body prototype #{instantiated:N0} already has instantiated bodies. Structural changes to this prototype could change their anatomy.";
				return true;
			}
		}

		reason = string.Empty;
		return false;
	}

	public bool SetParent(IBodyPrototype? parent, out string error)
	{
		if (parent is not null && Gameworld.BodyPrototypes.Get(parent.Id) != parent)
		{
			error = "Choose a parent body prototype from this game world.";
			return false;
		}
		var ancestry = new HashSet<IBodyPrototype>();
		for (var ancestor = parent; ancestor is not null; ancestor = ancestor.Parent)
		{
			if (ancestor == this || !ancestry.Add(ancestor))
			{
				error = "A body prototype cannot inherit from itself or a cyclic ancestry chain.";
				return false;
			}
		}
		if (HasInstantiatedBodiesInAffectedTree(out error))
		{
			return false;
		}

		using (new FMDB())
		{
			var dbitem = FMDB.Context.BodyProtos.Find(Id);
			dbitem.CountsAsId = parent?.Id;
			FMDB.Context.SaveChanges();
		}

		_countsAsId = parent?.Id;
		RefreshDescendantCaches();
		error = string.Empty;
		return true;
	}

	public bool SetPartRole(IBodypart? part, BodypartRole role, out string error)
	{
		if (part is null || part.Body != this)
		{
			error = "Only a bodypart owned by this prototype can be assigned a role. Inherited parts keep their source IDs and must be edited on their owning prototype.";
			return false;
		}
		if (HasInstantiatedBodiesInAffectedTree(out error))
		{
			return false;
		}

		using (new FMDB())
		{
			var dbpart = FMDB.Context.BodypartProtos.Find(part.Id);
			dbpart.IsCore = role == BodypartRole.Core;
			var roleRow = FMDB.Context.BodyProtosAdditionalBodyparts
				.FirstOrDefault(x => x.BodyProtoId == Id && x.BodypartId == part.Id);
			if (role is BodypartRole.MaleAddition or BodypartRole.FemaleAddition)
			{
				var usage = role == BodypartRole.MaleAddition ? "male" : "female";
				if (roleRow is not null && roleRow.Usage != usage)
				{
					FMDB.Context.BodyProtosAdditionalBodyparts.Remove(roleRow);
					roleRow = null;
				}
				if (roleRow is null)
				{
					roleRow = new BodyProtosAdditionalBodyparts { BodyProtoId = Id, BodypartId = part.Id, Usage = usage };
					FMDB.Context.BodyProtosAdditionalBodyparts.Add(roleRow);
				}
			}
			else if (roleRow is not null)
			{
				FMDB.Context.BodyProtosAdditionalBodyparts.Remove(roleRow);
			}
			FMDB.Context.SaveChanges();
		}

		UpdateBodypartRole(part, role);
		if (part is BodypartPrototype bodypartPrototype)
		{
			bodypartPrototype.IsCore = role == BodypartRole.Core;
		}
		RefreshDescendantCaches();
		error = string.Empty;
		return true;
	}

	public bool SetPartRemoved(IBodypart? part, bool removed, out string error)
	{
		if (part is null)
		{
			error = "That bodypart is not part of this prototype's inherited or owned anatomy.";
			return false;
		}
		if (removed)
		{
			if (part.Body == this)
			{
				error = "Owned parts cannot be removed with this command. Set an owned part to the extra role to exclude it, and use part role to change it back later.";
				return false;
			}
			if (!AllBodypartsBonesAndOrgans.Contains(part) || _bodypartRemovalIds.Contains(part.Id))
			{
				error = "Choose an inherited part that is currently available and not already removed.";
				return false;
			}
		}
		else
		{
			if (!_bodypartRemovalIds.Contains(part.Id))
			{
				error = "That bodypart does not have a removal on this prototype to restore.";
				return false;
			}
			if (part.Body == this || Gameworld.BodypartPrototypes.Get(part.Id) != part)
			{
				error = "Only the original inherited part recorded by this removal can be restored.";
				return false;
			}
		}
		if (HasInstantiatedBodiesInAffectedTree(out error))
		{
			return false;
		}

		using (new FMDB())
		{
			var existing = FMDB.Context.BodyProtosAdditionalBodyparts
				.FirstOrDefault(x => x.BodyProtoId == Id && x.BodypartId == part.Id && x.Usage == "remove");
			if (removed)
			{
				if (existing is null)
				{
					existing = new BodyProtosAdditionalBodyparts { BodyProtoId = Id, BodypartId = part.Id, Usage = "remove" };
					FMDB.Context.BodyProtosAdditionalBodyparts.Add(existing);
				}
				existing.Usage = "remove";
			}
			else if (existing is not null)
			{
				FMDB.Context.BodyProtosAdditionalBodyparts.Remove(existing);
			}
			FMDB.Context.SaveChanges();
		}

		if (removed) _bodypartRemovalIds.Add(part.Id);
		else _bodypartRemovalIds.Remove(part.Id);

		RefreshDescendantCaches();
		error = string.Empty;
		return true;
	}

	public bool SetDefaultSmashingPart(IBodypart? part, out string error)
	{
		if (part is not IExternalBodypart || !AllBodypartsBonesAndOrgans.Contains(part))
		{
			error = "The default smashing part must be an external bodypart in this prototype's anatomy.";
			return false;
		}
		if (HasInstantiatedBodiesInAffectedTree(out error)) return false;
		using (new FMDB())
		{
			var dbitem = FMDB.Context.BodyProtos.Find(Id);
			dbitem.DefaultSmashingBodypartId = part.Id;
			FMDB.Context.SaveChanges();
		}
		_defaultDoorSmashingPartID = part.Id;
		_defaultDoorSmashingPart = part;
		error = string.Empty;
		return true;
	}

	public IBodypart? AddPartFromTemplate(IBodypart? template, string newName, out string error)
	{
		if (template is not BodypartPrototype source || Gameworld.BodypartPrototypes.Get(template.Id) != template || string.IsNullOrWhiteSpace(newName))
		{
			error = "Choose a valid bodypart template and a non-empty new name.";
			return null;
		}
		if (AllBodypartsBonesAndOrgans.Any(x => x.Name.EqualTo(newName)))
		{
			error = "This prototype already has a part with that name.";
			return null;
		}
		if (HasInstantiatedBodiesInAffectedTree(out error)) return null;

		Models.BodypartProto row;
		using (new FMDB())
		{
			var original = FMDB.Context.BodypartProtos.Find(template.Id);
			if (original is null) { error = "The template bodypart no longer exists in the database."; return null; }
			row = new Models.BodypartProto
			{
				BodyId = Id,
				BodypartType = original.BodypartType,
				Name = newName,
				Description = original.Description,
				CountAsId = null,
				BodypartShapeId = original.BodypartShapeId,
				DisplayOrder = original.DisplayOrder,
				MaxLife = original.MaxLife,
				SeveredThreshold = original.SeveredThreshold,
				SeverFormula = original.SeverFormula,
				PainModifier = original.PainModifier,
				BleedModifier = original.BleedModifier,
				RelativeHitChance = original.RelativeHitChance,
				Location = original.Location,
				Alignment = original.Alignment,
				Unary = original.Unary,
				MaxSingleSize = original.MaxSingleSize,
				IsOrgan = original.IsOrgan,
				WeightLimit = original.WeightLimit,
				IsCore = original.IsCore && original.IsOrgan != 1 && !((BodypartTypeEnum)original.BodypartType).IsBone(),
				StunModifier = original.StunModifier,
				DamageModifier = original.DamageModifier,
				DefaultMaterialId = original.DefaultMaterialId,
				Significant = original.Significant,
				UseLimbSeverDescription = original.UseLimbSeverDescription,
				RelativeInfectability = original.RelativeInfectability,
				HypoxiaDamagePerTick = original.HypoxiaDamagePerTick,
				IsVital = original.IsVital,
				ArmourTypeId = original.ArmourTypeId,
				ImplantSpace = original.ImplantSpace,
				ImplantSpaceOccupied = original.ImplantSpaceOccupied,
				Size = original.Size
			};
			foreach (var hit in FMDB.Context.BodypartProtoAlignmentHits.Where(x => x.BodypartProtoId == template.Id))
			{
				row.BodypartProtoAlignmentHits.Add(new BodypartProtoAlignmentHits { Alignment = hit.Alignment, HitChance = hit.HitChance });
			}
			foreach (var hit in FMDB.Context.BodypartProtoOrientationHits.Where(x => x.BodypartProtoId == template.Id))
			{
				row.BodypartProtoOrientationHits.Add(new BodypartProtoOrientationHits { Orientation = hit.Orientation, HitChance = hit.HitChance });
			}
			FMDB.Context.BodypartProtos.Add(row);
			FMDB.Context.SaveChanges();
		}

		var part = BodypartPrototype.LoadFromDatabase(row, Gameworld);
		part.SetBodyProto(this);
		Gameworld.Add(part);
		_allBodyparts.Add(part);
		if (part is IExternalBodypart external) _externals.Add(external);
		if (part is IOrganProto organ) _organs.Add(organ);
		if (part is IBone bone) _bones.Add(bone);
		if (row.IsCore) _coreBodyparts.Add(part);
		InvalidateCachedBodyparts();
		RefreshDescendantCaches();
		error = string.Empty;
		return part;
	}

	public bool SetPosition(IPositionState? position, bool valid, out string error)
	{
		if (position is null || !PositionState.States.Contains(position))
		{
			error = "That is not a valid position.";
			return false;
		}
		if (valid && _validPositions.Contains(position) || !valid && !_validPositions.Contains(position))
		{
			error = valid ? "That position is already allowed." : "That position is not currently allowed.";
			return false;
		}
		if (HasInstantiatedBodiesInAffectedTree(out error))
		{
			return false;
		}
		if (valid)
		{
			_validPositions.Add(position);
		}
		else
		{
			if (_validPositions.Count == 1)
			{
				error = "A body prototype must retain at least one valid position.";
				return false;
			}
			_validPositions.Remove(position);
		}
		PersistPositions();
		RefreshDescendantCaches();
		error = string.Empty;
		return true;
	}

	public bool SetMetadata(string field, string value, out string error)
	{
		using (new FMDB())
		{
			var dbitem = FMDB.Context.BodyProtos.Find(Id);
			switch (field)
			{
				case "tracking": NameForTracking = dbitem.NameForTracking = value; break;
				case "consider": ConsiderString = dbitem.ConsiderString = value; break;
				case "wielder singular": WielderDescriptionSingular = dbitem.WielderDescriptionSingle = value; break;
				case "wielder plural": WielderDescriptionPlural = dbitem.WielderDescriptionPlural = value; break;
				case "leg singular": LegDescriptionSingular = dbitem.LegDescriptionSingular = value; break;
				case "leg plural": LegDescriptionPlural = dbitem.LegDescriptionPlural = value; break;
				default: error = "Unknown body prototype property."; return false;
			}
			FMDB.Context.SaveChanges();
		}
		error = string.Empty;
		return true;
	}

	public bool SetThreshold(string field, int value, out string error)
	{
		if (value < 0)
		{
			error = "Thresholds cannot be negative.";
			return false;
		}
		using (new FMDB())
		{
			var dbitem = FMDB.Context.BodyProtos.Find(Id);
			if (field == "legs") MinimumLegsToStand = dbitem.MinimumLegsToStand = value;
			else if (field == "wings") MinimumWingsToFly = dbitem.MinimumWingsToFly = value;
			else { error = "Use legs or wings."; return false; }
			FMDB.Context.SaveChanges();
		}
		error = string.Empty;
		return true;
	}

	public bool SetWearSize(long id, out string error)
	{
		using (new FMDB())
		{
			var rule = FMDB.Context.WearableSizeParameterRule.Find(id);
			if (rule is null)
			{
				error = "There is no wearable-size rule with that ID.";
				return false;
			}
			WearableSizeRules parsedRule;
			try
			{
				parsedRule = new WearableSizeRules(rule, this);
			}
			catch (Exception ex) when (ex is System.Xml.XmlException or FormatException or OverflowException or ArgumentNullException or NullReferenceException or ApplicationException)
			{
				error = $"The wearable-size rule is invalid: {ex.Message}";
				return false;
			}
			var dbitem = FMDB.Context.BodyProtos.Find(Id);
			dbitem.WearSizeParameterId = id;
			FMDB.Context.SaveChanges();
			WearRulesParameter = parsedRule;
		}
		error = string.Empty;
		return true;
	}

	public ILimb? CreateLimb(string name, LimbType type, IBodypart? root, IEnumerable<IBodypart> parts,
		IEnumerable<IBodypart> spinalParts, double damageMultiplier, double painMultiplier, out string error)
	{
		var partList = parts.Append(root).Distinct().ToList();
		if (root is not IExternalBodypart || partList.Any(x => !AllBodypartsBonesAndOrgans.Contains(x)) || partList.Any(x => x is not IExternalBodypart) ||
			spinalParts.Any(x => !AllBodypartsBonesAndOrgans.Contains(x) || x is not ISpineProto))
		{
			error = "A limb must use external bodyparts in this prototype's anatomy, include its external root, and any spinal parts must be spine prototypes in the body's anatomy.";
			return null;
		}
		if (string.IsNullOrWhiteSpace(name) || !double.IsFinite(damageMultiplier) || !double.IsFinite(painMultiplier) || damageMultiplier <= 0.0 || painMultiplier <= 0.0)
		{
			error = "Enter a limb name and positive damage and pain threshold multipliers.";
			return null;
		}
		if (HasInstantiatedBodiesInAffectedTree(out error)) return null;

		Models.Limb dbLimb;
		using (new FMDB())
		{
			dbLimb = new Models.Limb
			{
				Name = name,
				RootBodyId = Id,
				RootBodypartId = root.Id,
				LimbType = (int)type,
				LimbDamageThresholdMultiplier = damageMultiplier,
				LimbPainThresholdMultiplier = painMultiplier
			};
			foreach (var part in partList)
			{
				dbLimb.LimbsBodypartProto.Add(new LimbBodypartProto { Limb = dbLimb, BodypartProtoId = part.Id });
			}
			foreach (var part in spinalParts)
			{
				dbLimb.LimbsSpinalParts.Add(new LimbsSpinalPart { Limb = dbLimb, BodypartProtoId = part.Id });
			}
			FMDB.Context.Limbs.Add(dbLimb);
			FMDB.Context.SaveChanges();
		}
		var limb = new Limb(dbLimb, Gameworld);
		Gameworld.Add(limb);
		RefreshDescendantCaches();
		error = string.Empty;
		return limb;
	}

	public bool AddSpeed(IPositionState? position, string alias, string firstPerson, string thirdPerson,
		string participle, double rate, double stamina, out string error)
	{
		if (HasInstantiatedBodiesInAffectedTree(out error)) return false;
		if (position is null || !_validPositions.Contains(position) || !double.IsFinite(rate) || !double.IsFinite(stamina) || rate < 0.0 || stamina < 0.0 ||
			string.IsNullOrWhiteSpace(alias) || string.IsNullOrWhiteSpace(firstPerson) ||
			string.IsNullOrWhiteSpace(thirdPerson) || string.IsNullOrWhiteSpace(participle))
		{
			error = "Provide an allowed position, non-negative rates, and all movement wording.";
			return false;
		}
		if (Speeds.Any(x => x.Position == position && x.Name.EqualTo(alias)))
		{
			error = "A speed with that alias already exists for this body.";
			return false;
		}
		using (new FMDB())
		{
			var row = new Models.MoveSpeed
			{
				BodyProtoId = Id, PositionId = position.Id, Alias = alias,
				FirstPersonVerb = firstPerson, ThirdPersonVerb = thirdPerson,
				PresentParticiple = participle, Multiplier = rate, StaminaMultiplier = stamina
			};
			FMDB.Context.MoveSpeeds.Add(row);
			FMDB.Context.SaveChanges();
			var speed = new MoveSpeed(row);
			Gameworld.Add(speed);
			_localSpeeds.Add(speed);
			RefreshDescendantCaches();
		}
		error = string.Empty;
		return true;
	}

	public bool EditSpeed(IMoveSpeed speed, string alias, string firstPerson, string thirdPerson,
		string participle, double rate, double stamina, out string error)
	{
		if (speed is not MoveSpeed runtimeSpeed || !Speeds.Contains(speed) || string.IsNullOrWhiteSpace(alias) ||
			string.IsNullOrWhiteSpace(firstPerson) || string.IsNullOrWhiteSpace(thirdPerson) ||
			string.IsNullOrWhiteSpace(participle) || !double.IsFinite(rate) || !double.IsFinite(stamina) ||
			rate < 0.0 || stamina < 0.0)
		{
			error = "Choose a valid speed and provide non-negative rates and all movement wording.";
			return false;
		}
		using (new FMDB())
		{
			var row = FMDB.Context.MoveSpeeds.Find(speed.Id);
			if (row is null || row.BodyProtoId != Id)
			{
				error = "Inherited movement speeds are edited on their owning prototype.";
				return false;
			}
			if (Speeds.Any(x => x.Id != speed.Id && x.Position == speed.Position && x.Name.EqualTo(alias)))
			{
				error = "A speed with that alias already exists for this position.";
				return false;
			}
			row.Alias = alias;
			row.FirstPersonVerb = firstPerson;
			row.ThirdPersonVerb = thirdPerson;
			row.PresentParticiple = participle;
			row.Multiplier = rate;
			row.StaminaMultiplier = stamina;
			FMDB.Context.SaveChanges();
		}
		runtimeSpeed.Update(alias, firstPerson, thirdPerson, participle, rate, stamina);
		error = string.Empty;
		return true;
	}

	private void PersistPositions()
	{
		using (new FMDB())
		{
			var dbitem = FMDB.Context.BodyProtos.Find(Id);
			FMDB.Context.BodyProtosPositions.RemoveRange(dbitem.BodyProtosPositions);
			dbitem.BodyProtosPositions.Clear();
			foreach (var position in _validPositions)
			{
				dbitem.BodyProtosPositions.Add(new BodyProtosPositions { BodyProtoId = Id, Position = (int)position.Id });
			}
			FMDB.Context.SaveChanges();
		}
	}

	private void RefreshDescendantCaches()
	{
		var affected = Gameworld.BodyPrototypes.OfType<BodyPrototype>().Where(IsInAffectedTree).ToHashSet();
		var refreshed = new HashSet<BodyPrototype>();
		foreach (var body in affected) Refresh(body);

		void Refresh(BodyPrototype body)
		{
			if (!refreshed.Add(body)) return;
			if (body.Parent is BodyPrototype parent && affected.Contains(parent)) Refresh(parent);
			body.InvalidateCachedBodyparts();
			body._limbsInitialised = false;
			body._speeds.Clear();
			foreach (var speed in body._localSpeeds.OrderBy(x => x.Id)) body._speeds.Add(speed);
			var localPositions = body._localSpeeds.Select(x => x.Position).ToHashSet();
			if (body.Parent is not null)
				foreach (var speed in body.Parent.Speeds.Where(x => !localPositions.Contains(x.Position))) body._speeds.Add(speed);
			body._defaultSpeeds.Clear();
			foreach (var group in body._speeds.GroupBy(x => x.Position)) body._defaultSpeeds[group.Key] = group.First();
		}
	}

	private bool IsInAffectedTree(IBodyPrototype body)
	{
		var visited = new HashSet<IBodyPrototype>();
		for (var current = body; current is not null && visited.Add(current); current = current.Parent)
		{
			if (current == this) return true;
		}
		return false;
	}
}
