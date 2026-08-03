using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Construction;
using MudSharp.GameItems.Prototypes;
using MudSharp.Health;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Checks;

#nullable enable

namespace MudSharp.GameItems.Components;

/// <summary>
/// A crew-served ranged platform. This deliberately does not implement IWieldable;
/// command resolution must find it in the local cell rather than a character's hands.
/// </summary>
public sealed class ArtilleryPieceGameItemComponent : GameItemComponent, IArtilleryPiece
{
	private ArtilleryPieceGameItemComponentProto _prototype;
	private readonly Dictionary<ICharacter, string> _crew = new();
	private IAmmo? _loadedAmmo;
	private IArtilleryChamber? _installedChamber;
	private ArtilleryFiringSolution? _firingSolution;

	public ArtilleryPieceGameItemComponent(ArtilleryPieceGameItemComponentProto proto, IGameItem parent, bool temporary = false)
		: base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public ArtilleryPieceGameItemComponent(MudSharp.Models.GameItemComponent component,
		ArtilleryPieceGameItemComponentProto proto, IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		var root = XElement.Parse(component.Definition);
		LoadingStage = root.Element("LoadStage")?.Value.TryParseEnum<ArtilleryLoadingStage>(out var loadingStage) == true
			? loadingStage
			: (ArtilleryLoadingStage)Math.Clamp((int?)root.Element("LoadStage") ?? 0, 0, (int)ArtilleryLoadingStage.Primed);
		IsReadied = (bool?)root.Element("IsReadied") ?? false;
		IsEmplaced = (bool?)root.Element("IsEmplaced") ?? false;
		_loadedAmmo = Gameworld.TryGetItem((long?)root.Element("LoadedAmmo") ?? 0, true)?.GetItemType<IAmmo>();
		_installedChamber = Gameworld.TryGetItem((long?)root.Element("InstalledChamber") ?? 0, true)
			?.GetItemType<IArtilleryChamber>();
		var solution = root.Element("FiringSolution");
		if (solution is not null && double.TryParse(solution.Attribute("bearing")?.Value, out var bearing) &&
			double.TryParse(solution.Attribute("distance")?.Value, out var distance) &&
			double.TryParse(solution.Attribute("elevation")?.Value, out var elevation) &&
			double.TryParse(solution.Attribute("traverse")?.Value, out var traverse))
		{
			_firingSolution = new ArtilleryFiringSolution(bearing, distance, elevation, traverse,
				solution.Elements("Cell").Select(x => long.TryParse(x.Value, out var id) ? id : 0).Where(x => x > 0).ToList(),
				long.TryParse(solution.Attribute("routePosition")?.Value, out var routePosition) ? routePosition : null,
				double.TryParse(solution.Attribute("fuseSeconds")?.Value, out var fuseSeconds) && fuseSeconds >= 0.0
					? TimeSpan.FromSeconds(fuseSeconds) : null);
		}
	}

	private ArtilleryPieceGameItemComponent(ArtilleryPieceGameItemComponent rhs, IGameItem newParent, bool temporary = false)
		: base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
		LoadingStage = rhs.LoadingStage;
		IsReadied = rhs.IsReadied;
		IsEmplaced = rhs.IsEmplaced;
	}

	public override IGameItemComponentProto Prototype => _prototype;
	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto) => _prototype = (ArtilleryPieceGameItemComponentProto)newProto;
	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false) => new ArtilleryPieceGameItemComponent(this, newParent, temporary);

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("LoadStage", LoadStage),
			new XElement("IsReadied", IsReadied),
			new XElement("IsEmplaced", IsEmplaced),
			new XElement("LoadedAmmo", _loadedAmmo?.Parent.Id ?? 0),
			new XElement("InstalledChamber", _installedChamber?.Parent.Id ?? 0),
			_firingSolution is null ? null : new XElement("FiringSolution",
				new XAttribute("bearing", _firingSolution.Bearing), new XAttribute("distance", _firingSolution.Distance),
				new XAttribute("elevation", _firingSolution.Elevation), new XAttribute("traverse", _firingSolution.Traverse),
				_firingSolution.RouteCellPositionId is long routePosition ? new XAttribute("routePosition", routePosition) : null,
				_firingSolution.Fuse is TimeSpan fuse ? new XAttribute("fuseSeconds", fuse.TotalSeconds) : null,
				_firingSolution.CellPath.Select(x => new XElement("Cell", x)))).ToString();
	}

	public string FireVerbForEchoes => "fire|fires";
	public ITraitDefinition Trait => WeaponType.FireTrait;
	public bool CanBeAimedAtSelf => false;
	public WeaponClassification Classification => _prototype.RangedWeaponType?.Classification ?? WeaponClassification.Military;
	public IRangedWeaponType WeaponType => _prototype.RangedWeaponType!;
	public bool ReadyToFire => IsLoaded && IsReadied;
	public ArtilleryLoadingStage LoadingStage { get; private set; }
	public int LoadStage => (int)LoadingStage;
	public ArtilleryFiringSolution? FiringSolution => _firingSolution;
	public bool IsLoaded => (LoadingMechanism == ArtilleryLoadingMechanism.RemovableChamber
			? _installedChamber?.IsLoaded == true
			: _loadedAmmo is not null) && LoadingStage >= ArtilleryLoadingStage.ProjectileLoaded;
	public bool IsReadied { get; private set; }
	public IEnumerable<IGameItem> MagazineContents => _loadedAmmo is null ? [] : [_loadedAmmo.Parent];
	public IEnumerable<IGameItem> AllContainedItems => MagazineContents;
	public Difficulty AimDifficulty => WeaponType.BaseAimDifficulty;
	public Difficulty BaseBlockDifficulty => _loadedAmmo?.AmmoType.DamageProfile.BaseBlockDifficulty ?? Difficulty.Impossible;
	public Difficulty BaseDodgeDifficulty => _loadedAmmo?.AmmoType.DamageProfile.BaseDodgeDifficulty ?? Difficulty.Impossible;
	public ArtilleryLoadingMechanism LoadingMechanism => _prototype.LoadingMechanism;
	public bool IsEmplaced { get; private set; }
	public bool IsMounted => Parent.ContainedIn?.GetItemType<IArtilleryMount>()?.InstalledPiece == Parent;
	public IEnumerable<ICharacter> Crew => ActiveCrew();
	public IEnumerable<string> CrewRoles => _prototype.CrewRoles;
	public ArtilleryCrewAction? NextRequiredAction => LoadingStage switch
	{
		ArtilleryLoadingStage.Empty => ArtilleryCrewAction.Sponge,
		ArtilleryLoadingStage.Cleared => ArtilleryCrewAction.LoadCharge,
		ArtilleryLoadingStage.Charged => ArtilleryCrewAction.LoadWad,
		ArtilleryLoadingStage.Wadded => ArtilleryCrewAction.LoadProjectile,
		ArtilleryLoadingStage.ProjectileLoaded => ArtilleryCrewAction.Ram,
		ArtilleryLoadingStage.Rammed => ArtilleryCrewAction.Vent,
		ArtilleryLoadingStage.Vented => ArtilleryCrewAction.Prime,
		_ => null
	};
	public bool HasMinimumCrew => ActiveCrew().Count >= _prototype.MinimumCrew;

	public bool IsCrewedBy(ICharacter character)
	{
		PruneCrew();
		return _crew.ContainsKey(character);
	}

	public bool TryJoinCrew(ICharacter character, string role, out string reason)
	{
		if (character.Location != Parent.Location)
		{
			reason = "You must be beside the artillery piece to join its crew.";
			return false;
		}

		role = role.IfNullOrWhiteSpace("crew");
		if (!_prototype.HasCrewRole(role))
		{
			reason = $"{role.ColourName()} is not a configured crew role for that artillery piece.";
			return false;
		}
		PruneCrew();
		if (!role.EqualTo("crew") && _crew.Any(x => x.Key != character && x.Value.EqualTo(role)))
		{
			reason = $"The {role.ColourName()} role is already occupied.";
			return false;
		}

		if (Crew.Count() >= _prototype.MaximumCrew && !_crew.ContainsKey(character))
		{
			reason = "That artillery piece already has its configured crew.";
			return false;
		}

		_crew[character] = role;
		Changed = true;
		reason = string.Empty;
		return true;
	}

	public bool LeaveCrew(ICharacter character)
	{
		var result = _crew.Remove(character);
		Changed |= result;
		return result;
	}

	public bool CanPerform(ICharacter character, ArtilleryCrewAction action, out string reason)
	{
		PruneCrew();
		if (!IsAssignedAndPresent(character))
		{
			reason = AssignmentReason(character);
			return false;
		}
		if (action != ArtilleryCrewAction.Command && _prototype.RequiresEmplacement && !IsEmplaced && !IsMounted)
		{
			reason = "The artillery piece must be emplaced or mounted first.";
			return false;
		}

		if (!_prototype.AllowedActionsForRole(_crew[character]).Contains(action))
		{
			reason = $"Your {_crew[character].ColourName()} crew role cannot perform {action.DescribeEnum().ColourCommand()}.";
			return false;
		}

		reason = string.Empty;
		return true;
	}

	public bool SetFiringSolution(ICharacter actor, ArtilleryFiringSolution solution, out string reason)
	{
		if (!CanPerform(actor, ArtilleryCrewAction.Aim, out reason)) return false;
		var mount = Parent.ContainedIn?.GetItemType<IArtilleryMount>();
		var maximumTraverse = Math.Min(_prototype.MaximumTraverse, mount?.TraverseArc ?? _prototype.MaximumTraverse);
		var maximumElevation = Math.Min(_prototype.MaximumElevation, mount?.ElevationArc ?? _prototype.MaximumElevation);
		if (solution.Distance <= 0 || solution.Distance > WeaponType.DefaultRangeInRooms ||
			solution.Elevation < -_prototype.MaximumDepression || solution.Elevation > maximumElevation ||
			Math.Abs(solution.Traverse) > maximumTraverse)
		{
			reason = "That firing solution is outside this artillery piece's configured range or arcs.";
			return false;
		}
		if (!ValidateCellPath(actor, solution.CellPath, out reason))
		{
			return false;
		}

		var aimCheck = Gameworld.GetCheck(CheckType.AimArtillery)
			.Check(actor, WeaponType.BaseAimDifficulty, WeaponType.OperateTrait, Parent);
		if (aimCheck.IsFail())
		{
			reason = "You fail to establish a reliable firing solution.";
			return false;
		}

		_firingSolution = solution;
		Changed = true;
		reason = string.Empty;
		return true;
	}

	public void ClearFiringSolution()
	{
		_firingSolution = null;
		Changed = true;
	}

	public bool SetFuse(ICharacter actor, TimeSpan? fuse, out string reason)
	{
		if (!CanPerform(actor, ArtilleryCrewAction.Prime, out reason))
		{
			return false;
		}
		if (_firingSolution is null)
		{
			reason = "Set an artillery firing solution before setting a fuse.";
			return false;
		}
		var ammunition = LoadingMechanism == ArtilleryLoadingMechanism.RemovableChamber
			? _installedChamber?.LoadedAmmunition
			: _loadedAmmo as IArtilleryAmmunition;
		if (ammunition is null || ammunition.PayloadType is not (ArtilleryPayloadType.Shell or ArtilleryPayloadType.Carcass))
		{
			reason = "Only a loaded shell or carcass round can use an artillery fuse.";
			return false;
		}
		if (fuse is { } value && (value < TimeSpan.Zero || value > TimeSpan.FromMinutes(10)))
		{
			reason = "An artillery fuse must be between zero and ten minutes.";
			return false;
		}
		_firingSolution = _firingSolution with { Fuse = fuse };
		Changed = true;
		reason = string.Empty;
		return true;
	}

	public void Limber(ICharacter actor)
	{
		if (!IsCrewedBy(actor)) return;
		IsEmplaced = false;
		IsReadied = false;
		_firingSolution = null;
		_crew.Clear();
		Changed = true;
	}

	public void Emplace(ICharacter actor)
	{
		if (!IsCrewedBy(actor)) return;
		IsEmplaced = true;
		Changed = true;
	}

	public bool CanLoad(ICharacter loader, bool ignoreEmpty = false, LoadMode mode = LoadMode.Normal)
	{
		if (!IsOperationalFor(loader) || IsReadied || LoadingStage == ArtilleryLoadingStage.Primed) return false;
		var action = LoadingStage switch
		{
			ArtilleryLoadingStage.Empty => ArtilleryCrewAction.Sponge,
			ArtilleryLoadingStage.Cleared => ArtilleryCrewAction.LoadCharge,
			ArtilleryLoadingStage.Charged => ArtilleryCrewAction.LoadWad,
			ArtilleryLoadingStage.Wadded => ArtilleryCrewAction.LoadProjectile,
			ArtilleryLoadingStage.ProjectileLoaded => ArtilleryCrewAction.Ram,
			ArtilleryLoadingStage.Rammed => ArtilleryCrewAction.Vent,
			ArtilleryLoadingStage.Vented => ArtilleryCrewAction.Prime,
			_ => ArtilleryCrewAction.Command
		};
		return CanPerform(loader, action, out _) &&
			(action != ArtilleryCrewAction.LoadProjectile ||
				(LoadingMechanism == ArtilleryLoadingMechanism.RemovableChamber
					? FindCompatibleChamber(loader) is not null
					: FindCompatibleAmmo(loader) is not null));
	}

	public string WhyCannotLoad(ICharacter loader, bool ignoreEmpty = false, LoadMode mode = LoadMode.Normal)
	{
		if (!IsOperationalFor(loader)) return OperationalReason(loader);
		if (IsReadied || LoadingStage == ArtilleryLoadingStage.Primed) return "The artillery piece is already primed and ready.";
		var action = LoadingStage switch
		{
			ArtilleryLoadingStage.Empty => ArtilleryCrewAction.Sponge,
			ArtilleryLoadingStage.Cleared => ArtilleryCrewAction.LoadCharge,
			ArtilleryLoadingStage.Charged => ArtilleryCrewAction.LoadWad,
			ArtilleryLoadingStage.Wadded => ArtilleryCrewAction.LoadProjectile,
			ArtilleryLoadingStage.ProjectileLoaded => ArtilleryCrewAction.Ram,
			ArtilleryLoadingStage.Rammed => ArtilleryCrewAction.Vent,
			ArtilleryLoadingStage.Vented => ArtilleryCrewAction.Prime,
			_ => ArtilleryCrewAction.Command
		};
		if (!CanPerform(loader, action, out var reason)) return reason;
		return "You do not have compatible artillery ammunition.";
	}

	public void Load(ICharacter loader, bool ignoreEmpty = false, LoadMode mode = LoadMode.Normal)
	{
		if (!CanLoad(loader, ignoreEmpty, mode))
		{
			loader.Send(WhyCannotLoad(loader, ignoreEmpty, mode));
			return;
		}
		if (Gameworld.GetCheck(CheckType.LoadArtillery)
			.Check(loader, WeaponType.BaseAimDifficulty, WeaponType.OperateTrait, Parent).IsFail())
		{
			loader.Send($"You fail to complete the {LoadingStage.DescribeEnum().ToLowerInvariant()} stage of the artillery drill.");
			return;
		}

		var description = LoadingStage switch
		{
			ArtilleryLoadingStage.Empty => "sponges and clears",
			ArtilleryLoadingStage.Cleared => "loads a charge into",
			ArtilleryLoadingStage.Charged => "wads",
			ArtilleryLoadingStage.Wadded => "loads ammunition into",
			ArtilleryLoadingStage.ProjectileLoaded => "rams the load home in",
			ArtilleryLoadingStage.Rammed => "vents",
			ArtilleryLoadingStage.Vented => "primes",
			_ => "works on"
		};
		if (LoadingStage == ArtilleryLoadingStage.Wadded && LoadingMechanism == ArtilleryLoadingMechanism.RemovableChamber)
		{
			var chamber = FindCompatibleChamber(loader)!;
			loader.Body.Take(chamber.Parent);
			chamber.Parent.ContainedIn = Parent;
			_installedChamber = chamber;
		}
		else if (LoadingStage == ArtilleryLoadingStage.Wadded)
		{
			var ammo = FindCompatibleAmmo(loader)!;
			loader.Body.Take(ammo.Parent);
			ammo.Parent.ContainedIn = Parent;
			_loadedAmmo = ammo;
		}

		LoadingStage = LoadingStage switch
		{
			ArtilleryLoadingStage.Empty => ArtilleryLoadingStage.Cleared,
			ArtilleryLoadingStage.Cleared => ArtilleryLoadingStage.Charged,
			ArtilleryLoadingStage.Charged => ArtilleryLoadingStage.Wadded,
			ArtilleryLoadingStage.Wadded => ArtilleryLoadingStage.ProjectileLoaded,
			ArtilleryLoadingStage.ProjectileLoaded => ArtilleryLoadingStage.Rammed,
			ArtilleryLoadingStage.Rammed => ArtilleryLoadingStage.Vented,
			ArtilleryLoadingStage.Vented => ArtilleryLoadingStage.Primed,
			_ => LoadingStage
		};
		Changed = true;
		loader.Send($"You {description} {Parent.HowSeen(loader)}.");
		loader.OutputHandler.Handle(new EmoteOutput(new Emote("@ work|works on $0 as part of its loading drill.", loader, Parent)));
	}

	public bool CanReady(ICharacter readier) => IsOperationalFor(readier) && LoadingStage == ArtilleryLoadingStage.Primed && !IsReadied && CanPerform(readier, ArtilleryCrewAction.Prime, out _);
	public string WhyCannotReady(ICharacter readier) => !IsOperationalFor(readier) ? OperationalReason(readier) : LoadingStage != ArtilleryLoadingStage.Primed ? "The artillery drill must be completed before it can be readied." : "It is already primed and ready.";
	public bool Ready(ICharacter readier)
	{
		if (!CanReady(readier)) return false;
		IsReadied = true;
		Changed = true;
		return true;
	}

	public bool CanUnready(ICharacter readier) => IsCrewedBy(readier);
	public string WhyCannotUnready(ICharacter readier) => "You are not assigned to that artillery crew.";
	public bool Unready(ICharacter readier)
	{
		if (!CanUnready(readier)) return false;
		IsReadied = false;
		Changed = true;
		return true;
	}

	public bool CanUnload(ICharacter loader) => IsCrewedBy(loader) && (IsLoaded || _installedChamber is not null);
	public string WhyCannotUnload(ICharacter loader) => !IsCrewedBy(loader) ? "You are not assigned to that artillery crew." : "The artillery piece is already empty.";
	public IEnumerable<IGameItem> Unload(ICharacter loader)
	{
		if (!CanUnload(loader)) return [];
		var item = LoadingMechanism == ArtilleryLoadingMechanism.RemovableChamber
			? _installedChamber!.Parent
			: _loadedAmmo!.Parent;
		if (LoadingMechanism == ArtilleryLoadingMechanism.RemovableChamber)
		{
			_installedChamber = null;
		}
		else
		{
			_loadedAmmo = null;
		}
		LoadingStage = ArtilleryLoadingStage.Empty;
		IsReadied = false;
		item.ContainedIn = null;
		item.InsertAtSource(Parent.LocationLevelPerceivable ?? loader);
		Changed = true;
		return [item];
	}

	public bool CanFire(ICharacter actor, IPerceivable target) =>
		IsOperationalFor(actor) && HasMinimumCrew && ReadyToFire &&
		(target is not null || ResolveIndirectTarget() is not null) &&
		CanPerform(actor, ArtilleryCrewAction.Fire, out _);
	public string WhyCannotFire(ICharacter actor, IPerceivable target) =>
		!IsOperationalFor(actor) ? OperationalReason(actor) :
		!HasMinimumCrew ? $"That artillery piece requires at least {_prototype.MinimumCrew.ToString(actor)} active crew members." :
		"The artillery piece is not ready to fire.";
	public void Fire(ICharacter actor, IPerceiver target, Outcome shotOutcome, Outcome coverOutcome, OpposedOutcome defenseOutcome,
		IBodypart bodypart, IEmoteOutput defenseEmote, IPerceiver originalTarget)
	{
		if (!CanFire(actor, target))
		{
			actor.Send(WhyCannotFire(actor, target));
			return;
		}

		var firingTarget = target ?? ResolveIndirectTarget();
		if (firingTarget is null)
		{
			actor.Send("Set a reachable indirect firing solution or select a visible target before firing.");
			return;
		}

		if (shotOutcome == Outcome.NotTested)
		{
			shotOutcome = Gameworld.GetCheck(CheckType.FireArtillery)
				.Check(actor, WeaponType.BaseAimDifficulty, WeaponType.FireTrait, firingTarget).Outcome;
		}

		var ammo = LoadingMechanism == ArtilleryLoadingMechanism.RemovableChamber
			? _installedChamber!.LoadedAmmunition!
			: _loadedAmmo!;
		var projectile = ammo.GetFiredItem ?? ammo.Parent;
		ammo.Fire(actor, firingTarget, shotOutcome, coverOutcome, defenseOutcome, bodypart, projectile, WeaponType, defenseEmote,
			new RangedFireContext(0, Math.Max(1, ammo.AmmoType.ProjectileCount), ammo.AmmoType.ScatterType));
		if (LoadingMechanism == ArtilleryLoadingMechanism.RemovableChamber)
		{
			_installedChamber!.Unload();
			_installedChamber.Parent.ContainedIn = null;
			_installedChamber.Parent.InsertAtSource(Parent.LocationLevelPerceivable ?? actor);
			_installedChamber = null;
		}
		else
		{
			_loadedAmmo = null;
		}
		LoadingStage = ArtilleryLoadingStage.Empty;
		_firingSolution = null;
		IsReadied = false;
		Changed = true;
	}

	public override bool PreventsMovement()
	{
		PruneCrew();
		return IsEmplaced || IsMounted || IsLoaded || _crew.Count > 0;
	}
	public override string WhyPreventsMovement(ICharacter mover) =>
		IsEmplaced ? "it is emplaced and must be limbered first" :
		IsMounted ? "it is installed on an artillery mount" :
		IsLoaded ? "it contains an unsafe artillery load" : "it has an active artillery crew";

	public override double ComponentWeight =>
		(_loadedAmmo?.Parent.Weight ?? 0.0) + (_installedChamber?.Parent.Weight ?? 0.0);

	public override double ComponentBuoyancy(double fluidDensity) =>
		(_loadedAmmo?.Parent.Buoyancy(fluidDensity) ?? 0.0) +
		(_installedChamber?.Parent.Buoyancy(fluidDensity) ?? 0.0);

	public override void FinaliseLoad()
	{
		_loadedAmmo?.Parent.FinaliseLoadTimeTasks();
		_installedChamber?.Parent.FinaliseLoadTimeTasks();
	}

	public override void Login()
	{
		_loadedAmmo?.Parent.Login();
		_installedChamber?.Parent.Login();
	}

	public override void Quit()
	{
		base.Quit();
		_crew.Clear();
		_loadedAmmo?.Parent.Quit();
		_installedChamber?.Parent.Quit();
	}

	public override void Delete()
	{
		base.Delete();
		_crew.Clear();
		_loadedAmmo?.Parent.Delete();
		_installedChamber?.Parent.Delete();
	}

	private bool IsOperationalFor(ICharacter actor)
	{
		return IsAssignedAndPresent(actor) && (!_prototype.RequiresEmplacement || IsEmplaced || IsMounted);
	}
	private bool IsAssignedAndPresent(ICharacter actor)
	{
		PruneCrew();
		return _crew.ContainsKey(actor) && actor.Location == Parent.Location && !actor.State.HasFlag(CharacterState.Dead) &&
		       actor.State.HasFlag(CharacterState.Able);
	}
	private string AssignmentReason(ICharacter actor) =>
		!_crew.ContainsKey(actor) ? "You must join that artillery crew first." :
		actor.Location != Parent.Location ? "You must remain beside the artillery piece to operate it." :
		"You must be able to operate that artillery piece.";
	private string OperationalReason(ICharacter actor) =>
		!IsAssignedAndPresent(actor) ? AssignmentReason(actor) : "The artillery piece must be emplaced or mounted first.";
	private IAmmo? FindCompatibleAmmo(ICharacter actor)
	{
		return actor.Inventory.SelectMany(x => x.GetItemTypes<IAmmo>()).FirstOrDefault(x =>
			x is IArtilleryAmmunition artilleryAmmo && ProfilesAreCompatible(_prototype.ArtilleryProfile, artilleryAmmo.ArtilleryProfile));
	}

	private IArtilleryChamber? FindCompatibleChamber(ICharacter actor)
	{
		return actor.Inventory.SelectMany(x => x.GetItemTypes<IArtilleryChamber>()).FirstOrDefault(x =>
			x.IsLoaded && ProfilesAreCompatible(_prototype.ArtilleryProfile, x.ArtilleryProfile));
	}

	private IReadOnlyCollection<ICharacter> ActiveCrew()
	{
		PruneCrew();
		return _crew.Keys.ToList();
	}

	private void PruneCrew()
	{
		var departed = _crew.Keys
			.Where(x => x.Location != Parent.Location || x.State.HasFlag(CharacterState.Dead) || !x.State.HasFlag(CharacterState.Able))
			.ToList();
		foreach (var crewMember in departed)
		{
			_crew.Remove(crewMember);
			Changed = true;
		}
	}

	private bool ValidateCellPath(ICharacter actor, IReadOnlyList<long> cellPath, out string reason)
	{
		if (cellPath.Count == 0)
		{
			reason = string.Empty;
			return true;
		}

		var current = Parent.Location;
		if (current is null)
		{
			reason = "The artillery piece is not located in a cell.";
			return false;
		}
		foreach (var cellId in cellPath)
		{
			var next = Gameworld.Cells.Get(cellId);
			if (next is null || !current.ExitsFor(actor).Any(x => x.Destination == next))
			{
				reason = "An indirect firing path must be an explicit, reachable chain of cells.";
				return false;
			}
			current = next;
		}

		reason = string.Empty;
		return true;
	}

	private IPerceiver? ResolveIndirectTarget()
	{
		if (_firingSolution?.CellPath.LastOrDefault() is not long targetCellId || targetCellId <= 0)
		{
			return null;
		}
		var cell = Gameworld.Cells.Get(targetCellId);
		if (cell is null)
		{
			return null;
		}
		return (IPerceiver?)cell.Characters.FirstOrDefault() ?? new DummyPerceiver(location: cell);
	}

	/// <summary>
	/// Artillery profiles are deliberately data-owned rather than tied to database IDs. A
	/// profile may name several compatible calibre classes (for example, "light|medium")
	/// and a wildcard is useful for builder-authored training pieces.
	/// </summary>
	private static bool ProfilesAreCompatible(string pieceProfile, string ammunitionProfile)
	{
		var pieceProfiles = pieceProfile.Split('|', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
		var ammunitionProfiles = ammunitionProfile.Split('|', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
		return pieceProfiles.Any(x => x == "*" || ammunitionProfiles.Any(y => y == "*" || x.EqualTo(y)));
	}
}
