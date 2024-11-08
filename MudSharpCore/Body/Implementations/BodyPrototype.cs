using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MudSharp.Models;
using MudSharp.Body.PartProtos;
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems.Inventory.Size;
using MudSharp.Strategies.BodyStratagies;
using BodypartGroupDescriber = MudSharp.Body.Grouping.BodypartGroupDescriber;
using MoveSpeed = MudSharp.Movement.MoveSpeed;
using MudSharp.Database;
using MudSharp.Framework.Save;
using MudSharp.Movement;

namespace MudSharp.Body.Implementations;

public class BodyPrototype : SaveableItem, IBodyPrototype
{
	protected readonly List<IBodypart> _allBodyparts = new();
	protected readonly List<IBodypartGroupDescriber> _bodypartGroupDescribers = new();

	protected readonly List<IBodypart> _coreBodyparts = new();

	protected readonly List<IBodypart> _femaleOnlyAdditions = new();
	protected readonly List<ILimb> _limbs = new();

	protected readonly List<IBodypart> _maleOnlyAdditions = new();

	protected readonly List<IOrganProto> _organs = new();
	protected readonly List<IBone> _bones = new();
	protected readonly List<IExternalBodypart> _externals = new();

	private bool _limbsInitialised;

	public BodyPrototype(BodyProto proto, IFuturemud game)
	{
		Gameworld = game;
		_countsAsId = proto.CountsAsId;
		WearRulesParameter = new WearableSizeRules(proto.WearSizeParameter, this);
		ConsiderString = proto.ConsiderString;

		if (!proto.BodyProtosPositions.Any())
		{
			_validPositions = new List<IPositionState>
			{
				PositionStanding.Instance,
				PositionSitting.Instance,
				PositionKneeling.Instance,
				PositionLounging.Instance,
				PositionLyingDown.Instance,
				PositionSprawled.Instance,
				PositionProne.Instance,
				PositionProstrate.Instance,
				PositionStandingAttention.Instance,
				PositionStandingEasy.Instance,
				PositionLeaning.Instance,
				PositionSquatting.Instance,
				PositionFlying.Instance,
				PositionSwimming.Instance,
				PositionClimbing.Instance
			};
			Changed = true;
		}
		else
		{
			_validPositions = new List<IPositionState>();
			foreach (var position in proto.BodyProtosPositions)
			{
				_validPositions.Add(PositionState.GetState(position.Position));
			}
		}

		_id = proto.Id;
		_name = proto.Name;
		NameForTracking = proto.NameForTracking ?? _name;

		StaminaRecoveryProg = Gameworld.FutureProgs.Get(proto.StaminaRecoveryProgId ?? 0);

		foreach (var rule in proto.BodypartGroupDescribersBodyProtos)
		{
			_bodypartGroupDescribers.Add(game.BodypartGroupDescriptionRules.Get(rule.BodypartGroupDescriberId));
		}

		WielderDescriptionPlural = proto.WielderDescriptionPlural;
		WielderDescriptionSingular = proto.WielderDescriptionSingle;

		MinimumLegsToStand = proto.MinimumLegsToStand;
		MinimumWingsToFly = proto.MinimumWingsToFly;

		LegDescriptionPlural = proto.LegDescriptionPlural;
		LegDescriptionSingular = proto.LegDescriptionSingular;

		foreach (var speed in proto.MoveSpeeds)
		{
			var gspeed = new MoveSpeed(speed);
			Gameworld.Add(gspeed);
			_speeds.Add(gspeed);
		}


		var parent = Gameworld.BodyPrototypes.Get(_countsAsId ?? 0);
		if (parent != null)
		{
			foreach (var speed in parent.Speeds)
			{
				if (!_speeds.Has(speed))
				{
					_speeds.Add(speed);
				}
			}
		}

		// TODO - make this configurable
		foreach (var state in _speeds.Select(x => x.Position).Distinct())
		{
			DefaultSpeeds[state] = _speeds.First(x => x.Position == state);
		}

		// Load bodyparts
		_defaultDoorSmashingPartID = proto.DefaultSmashingBodypartId ?? 0;
	}

	public void FinaliseBodyparts(Models.BodyProto proto)
	{
		var allParts = Gameworld.BodypartPrototypes.Where(x => x.Body == this).ToList();
		var partsOnly = allParts.OfType<IExternalBodypart>().ToList();
		_coreBodyparts.AddRange(allParts.Where(x => x.IsCore));
		_externals.AddRange(partsOnly);
		_allBodyparts.AddRange(allParts);
		_organs.AddRange(allParts.OfType<IOrganProto>());
		_bones.AddRange(allParts.OfType<IBone>());
		_maleOnlyAdditions.AddRange(allParts.Where(x => proto.BodyProtosAdditionalBodyparts.Any(y => y.BodypartId == x.Id && y.Usage == "male")));
		_femaleOnlyAdditions.AddRange(allParts.Where(x => proto.BodyProtosAdditionalBodyparts.Any(y => y.BodypartId == x.Id && y.Usage == "female")));
	}

	public BodypartRole GetBodypartRole(IBodypart bodypart)
	{
		
		if ((Parent?.CoreBodyparts?.Concat(_coreBodyparts) ?? _coreBodyparts).Contains(bodypart))
		{
			return BodypartRole.Core;
		}

		if ((Parent?.MaleOnlyAdditions?.Concat(_maleOnlyAdditions) ?? _maleOnlyAdditions).Contains(bodypart))
		{
			return BodypartRole.MaleAddition;
		}

		if ((Parent?.FemaleOnlyAdditions?.Concat(_femaleOnlyAdditions) ?? _femaleOnlyAdditions).Contains(bodypart))
		{
			return BodypartRole.FemaleAddition;
		}

		return BodypartRole.Extra;
	}

	public void UpdateBodypartRole(IBodypart bodypart, BodypartRole role)
	{
		if (bodypart is IBone bone)
		{
			if (!_bones.Contains(bone))
			{
				_bones.Add(bone);
			}
		}
		else if (bodypart is IOrganProto organ)
		{
			if (!_organs.Contains(organ))
			{
				_organs.Add(organ);
			}
		}
		else if (!_allBodyparts.Contains(bodypart))
		{
			_allBodyparts.Add(bodypart);
		}

		_coreBodyparts.Remove(bodypart);
		_femaleOnlyAdditions.Remove(bodypart);
		_maleOnlyAdditions.Remove(bodypart);
		switch (role)
		{
			case BodypartRole.Core:
				_coreBodyparts.Add(bodypart);
				break;
			case BodypartRole.MaleAddition:
				_maleOnlyAdditions.Add(bodypart);
				break;
			case BodypartRole.FemaleAddition:
				_femaleOnlyAdditions.Add(bodypart);
				break;
		}
	}

	public void InvalidateCachedBodyparts()
	{
		_cachedBodypartDictionary.Clear();
	}

	public IEnumerable<IBodypartGroupDescriber> BodypartGroupDescribers
	{
		get
		{
			if (_countsAsId.HasValue)
			{
				return Gameworld.BodyPrototypes.Get(_countsAsId.Value).BodypartGroupDescribers
				                .Concat(_bodypartGroupDescribers);
			}

			return _bodypartGroupDescribers;
		}
	}

	public override string FrameworkItemType => "BodyPrototype";

	#region Strategies

	public IBodyCommunicationStrategy Communications { get; protected set; }

	#endregion

	private long? _countsAsId;

	public IBodyPrototype Parent => Gameworld.BodyPrototypes.Get(_countsAsId ?? 0L);

	public IWearableSizeRules WearRulesParameter { get; protected set; }

	public IFutureProg StaminaRecoveryProg { get; protected set; }

	public string ConsiderString { get; protected set; }

	public string NameForTracking { get; protected set; }

	public IEnumerable<IBodypart> CoreBodyparts
	{
		get
		{
			if (_countsAsId.HasValue)
			{
				return Gameworld.BodyPrototypes.Get(_countsAsId.Value).CoreBodyparts.Concat(_coreBodyparts);
			}

			return _coreBodyparts;
		}
	}

	public IEnumerable<IExternalBodypart> AllExternalBodyparts
	{
		get
		{
			if (_countsAsId.HasValue)
			{
				return Gameworld.BodyPrototypes.Get(_countsAsId.Value).AllExternalBodyparts.Concat(_externals);
			}

			return _externals;
		}
	}

	public IEnumerable<IBodypart> AllBodyparts
	{
		get
		{
			if (_countsAsId.HasValue)
			{
				return Gameworld.BodyPrototypes.Get(_countsAsId.Value).AllBodyparts.Concat(_allBodyparts);
			}

			return _allBodyparts;
		}
	}

	public IEnumerable<IOrganProto> Organs
	{
		get
		{
			if (_countsAsId.HasValue)
			{
				return Gameworld.BodyPrototypes.Get(_countsAsId.Value).Organs.Concat(_organs);
			}

			return _organs;
		}
	}

	public IEnumerable<IBodypart> AllBodypartsBonesAndOrgans
	{
		get
		{
			if (_countsAsId.HasValue)
			{
				return Gameworld.BodyPrototypes.Get(_countsAsId.Value).AllBodypartsBonesAndOrgans.Concat(_allBodyparts);
			}

			return _allBodyparts;
		}
	}

	public IEnumerable<IBone> Bones
	{
		get
		{
			if (_countsAsId.HasValue)
			{
				return Gameworld.BodyPrototypes.Get(_countsAsId.Value).Bones.Concat(_bones);
			}

			return _bones;
		}
	}

	public IEnumerable<ILimb> Limbs
	{
		get
		{
			if (!_limbsInitialised)
			{
				InitialiseLimbs();
			}

			if (_countsAsId.HasValue)
			{
				return Gameworld.BodyPrototypes.Get(_countsAsId.Value).Limbs.Concat(_limbs).Distinct();
			}

			return _limbs;
		}
	}

	public int MinimumLegsToStand { get; set; }
	public int MinimumWingsToFly { get; set; }

	private readonly Dictionary<(IRace Race, Gender Gender), IEnumerable<IBodypart>> _cachedBodypartDictionary = new();

	public bool CountsAs(IBodyPrototype bodyPrototype)
	{
		return bodyPrototype == this || (_countsAsId.HasValue &&
		                                 Gameworld.BodyPrototypes.Get(_countsAsId.Value).CountsAs(bodyPrototype));
	}

	public IEnumerable<IBodypart> BodypartsFor(IRace race, Gender gender)
	{
		if (_cachedBodypartDictionary.ContainsKey((race, gender)))
		{
			return _cachedBodypartDictionary[(race, gender)];
		}

		var parts = new List<IBodypart>();
		if (_countsAsId.HasValue)
		{
			var parent = Gameworld.BodyPrototypes.Get(_countsAsId.Value);
			parts.AddRange(parent.BodypartsFor(race, gender));
		}

		parts.AddRange(_coreBodyparts);
		parts.AddRange(race?.BodypartAdditions ?? Enumerable.Empty<IBodypart>());

		switch (gender)
		{
			case Gender.Male:
				parts.AddRange(_maleOnlyAdditions);
				parts.AddRange(race?.MaleOnlyAdditions ?? Enumerable.Empty<IBodypart>());
				break;
			case Gender.Female:
				parts.AddRange(_femaleOnlyAdditions);
				parts.AddRange(race?.FemaleOnlyAdditions ?? Enumerable.Empty<IBodypart>());
				break;
		}

		foreach (var part in race?.BodypartRemovals ?? Enumerable.Empty<IBodypart>())
		{
			parts.Remove(part);
		}

		_cachedBodypartDictionary[(race, gender)] = parts;
		return parts;
	}

	public string DescribeBodypartGroup(IEnumerable<IBodypart> group)
	{
		if (group.Count() == 1)
		{
			return group.Single().ShortDescription(colour: false);
		}

		return BodypartGroupDescriber.DescribeGroups(BodypartGroupDescribers, group);
	}

	#region IHaveFuturemud Members

	public override void Save()
	{
		var dbitem = FMDB.Context.BodyProtos.Find(Id);

		// For now, only save the things that change
		FMDB.Context.BodyProtosPositions.RemoveRange(dbitem.BodyProtosPositions);
		foreach (var position in _validPositions)
		{
			dbitem.BodyProtosPositions.Add(new BodyProtosPositions
			{
				BodyProto = dbitem, Position = (int)position.Id
			});
		}

		Changed = false;
	}

	#endregion

	public IEnumerable<IBodypart> FemaleOnlyAdditions => _femaleOnlyAdditions;
	public IEnumerable<IBodypart> MaleOnlyAdditions => _maleOnlyAdditions;

	private void InitialiseLimbs()
	{
		_limbs.Clear();
		_limbs.AddRange(Gameworld.Limbs.Where(x => CountsAs(x.Prototype)).ToList());
		_limbsInitialised = true;

#if DEBUG
		foreach (var item in AllExternalBodyparts)
		{
			if (!Limbs.Any(x => x.Parts.Contains(item)))
			{
				throw new ApplicationException($"Bodypart without a limb: {item.FullDescription()} for {Name}");
			}
		}
#endif
	}

	#region IBodyPrototype Members

	private long _defaultDoorSmashingPartID;
	private IBodypart _defaultDoorSmashingPart;

	public IBodypart DefaultDoorSmashingPart
	{
		get
		{
			if (_defaultDoorSmashingPart == null && _defaultDoorSmashingPartID > 0)
			{
				_defaultDoorSmashingPart = Gameworld.BodypartPrototypes.Get(_defaultDoorSmashingPartID);
				_defaultDoorSmashingPartID = 0;
			}

			return _defaultDoorSmashingPart;
		}
	}

	public string WielderDescriptionSingular { get; protected set; }

	public string WielderDescriptionPlural { get; protected set; }

	public string LegDescriptionSingular { get; protected set; }
	public string LegDescriptionPlural { get; protected set; }

	protected List<IPositionState> _validPositions;
	public IEnumerable<IPositionState> ValidPositions => _validPositions;

	protected readonly Dictionary<IPositionState, IMoveSpeed> _defaultSpeeds =
		new();

	public Dictionary<IPositionState, IMoveSpeed> DefaultSpeeds => _defaultSpeeds;

	protected readonly All<IMoveSpeed> _speeds = new();
	public IUneditableAll<IMoveSpeed> Speeds => _speeds;

	public string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Body Prototype #{Id.ToString("N0", actor)} - {Name.ColourName()}");
		sb.AppendLine();
		sb.AppendLine("Core Properties".GetLineWithTitle(actor, Telnet.Red, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine($"Parent: {Parent?.Name.ColourName() ?? "None".ColourError()}");
		sb.AppendLine($"Name For Tracking: {NameForTracking.ColourCharacter()}");
		sb.AppendLine($"Minimum Legs to Stand: {MinimumLegsToStand.ToString("N0", actor).ColourValue()}");
		sb.AppendLine($"Minimum Wings to Fly: {MinimumWingsToFly.ToString("N0", actor).ColourValue()}");
		sb.AppendLine($"Stamina Prog: {StaminaRecoveryProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Grasping Parts: {WielderDescriptionSingular.ColourValue()} / {WielderDescriptionPlural.ColourValue()}");
		sb.AppendLine($"Standing Limbs: {LegDescriptionSingular.ColourValue()} / {LegDescriptionPlural.ColourValue()}");
		sb.AppendLine($"Default Smashing Part: {DefaultDoorSmashingPart?.Name.ColourValue() ?? "None".ColourError()}");
		sb.AppendLine();
		sb.AppendLine("Positions".GetLineWithTitle(actor, Telnet.Red, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine(CommonStringUtilities.ArrangeStringsOntoLines(ValidPositions.Select(x => x.Name.ColourName()),
			(uint)actor.LineFormatLength / 40, (uint)actor.LineFormatLength));
		sb.AppendLine();
		sb.AppendLine("Speeds".GetLineWithTitle(actor, Telnet.Red, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine(StringUtilities.GetTextTable(
			from speed in Speeds.OrderBy(x => x.Position.Id).ThenByDescending(x => x.Multiplier)
			select new List<string>
			{
				speed.Name,
				speed.Position.Name,
				speed.Multiplier.ToString("P2", actor),
				speed.StaminaMultiplier.ToString("P2", actor),
				speed.FirstPersonVerb,
				speed.ThirdPersonVerb,
				speed.PresentParticiple
			},
			new List<string>
			{
				"Name",
				"Position",
				"Rate",
				"Stamina",
				"Verb 1st",
				"Verb 3rd",
				"Participle"
			},
			actor,
			Telnet.Red
		));
		sb.AppendLine();
		sb.AppendLine("External Bodyparts".GetLineWithTitle(actor, Telnet.Red, Telnet.BoldWhite));
		sb.AppendLine();
		var parts = new List<(IBodypart Part, bool FromParent)>();
		foreach (var part in AllBodypartsBonesAndOrgans)
		{
			parts.Add((part, _allBodyparts.Contains(part)));
		}
		sb.AppendLine(StringUtilities.GetTextTable(
			from part in parts.Where(x => x.Part is IExternalBodypart)
			let limb = Limbs.FirstOrDefault(x => x.Parts.Contains(part.Part))
			select new List<string>
			{
				part.Part.Id.ToString("N0", actor),
				part.Part.Name,
				part.Part.FullDescription(),
				GetBodypartRole(part.Part).DescribeEnum(),
				part.Part.Shape.Name,
				limb?.Name ?? "",
				part.Part.MaxLife.ToString("N0", actor),
				part.Part.CanSever ? part.Part.SeveredThreshold.ToString("N0", actor) : "--",
				part.Part.Alignment.Describe(),
				part.Part.Orientation.Describe(),
				part.Part.IsVital.ToString(),
				part.Part.Significant.ToString(),
				part.Part.ImplantSpace.ToString("N2", actor),
				part.FromParent.ToString()
			},
			new List<string>
			{
				"Id",
				"Alias",
				"Name",
				"Role",
				"Shape",
				"Limb",
				"HP",
				"Sever",
				"Align",
				"Orient",
				"Vital?",
				"Sig?",
				"Space",
				"Parent?"
			},
			actor,
			Telnet.Yellow
		));
		sb.AppendLine();
		sb.AppendLine("Organs".GetLineWithTitle(actor, Telnet.Red, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine(StringUtilities.GetTextTable(
				from part in parts.Where(x => x.Part is IOrganProto)
				let organ = part.Part as IOrganProto
				let contain = AllExternalBodyparts.FirstOrDefault(x => x.OrganInfo.Any(y => y.Key == organ && y.Value.IsPrimaryInternalLocation))
				select new List<string>
			{
				part.Part.Id.ToString("N0", actor),
				part.Part.Name,
				part.Part.FullDescription(),
				GetBodypartRole(part.Part).DescribeEnum(),
				part.Part.MaxLife.ToString("N0", actor),
				contain?.Name ?? "",
				organ.ImplantSpaceOccupied.ToString("N2", actor),
				organ.RequiresSpinalConnection.ToString(),
				organ.RelativeInfectability.ToString("P2", actor),
				organ.HypoxiaDamagePerTick.ToString("N2", actor),
				part.FromParent.ToString()
			},
			new List<string>
			{
				"Id",
				"Alias",
				"Name",
				"Role",
				"HP",
				"Inside",
				"Space Cost",
				"Req. Spine?",
				"Infect?",
				"Hypoxia/Tick",
				"From Parent?"
			},
			actor,
			Telnet.Yellow
		));
		sb.AppendLine();
		sb.AppendLine("Bones".GetLineWithTitle(actor, Telnet.Red, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine(StringUtilities.GetTextTable(
			from part in parts.Where(x => x.Part is IBone and not IExternalBodypart)
			let bone = part.Part as IBone
			let contain = AllExternalBodyparts.FirstOrDefault(x => x.BoneInfo.Any(y => y.Key == bone && y.Value.IsPrimaryInternalLocation))
			select new List<string>
			{
				part.Part.Id.ToString("N0", actor),
				part.Part.Name,
				part.Part.FullDescription(),
				GetBodypartRole(part.Part).DescribeEnum(),
				part.Part.MaxLife.ToString("N0", actor),
				contain?.Name ?? "",
				bone.CriticalBone.ToString(),
				bone.BoneHealingModifier.ToString("P2", actor),
				bone.CanBeImmobilised.ToString(),
				part.FromParent.ToString()
			},
			new List<string>
			{
				"Id",
				"Alias",
				"Name",
				"Role",
				"HP",
				"Inside",
				"Critical?",
				"Healing",
				"Immobilise?",
				"From Parent?"
			},
			actor,
			Telnet.Yellow
		));
		return sb.ToString();
	}

	#endregion
}