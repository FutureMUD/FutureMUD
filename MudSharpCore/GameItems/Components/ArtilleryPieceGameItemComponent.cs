using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Construction;
using MudSharp.Events;
using MudSharp.Form.Audio;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Inventory.Plans;
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
	private IGameItem? _powderCharge;
	private IGameItem? _wad;
	private IGameItem? _primerCharge;
	private IGameItem? _fuse;

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
		_powderCharge = LoadContainedItem(root, "PowderCharge");
		_wad = LoadContainedItem(root, "Wad");
		_primerCharge = LoadContainedItem(root, "PrimerCharge");
		_fuse = LoadContainedItem(root, "Fuse");
		NormaliseLoadingStage();
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
		// A copy cannot share the source piece's physical charge, projectile, chamber, fuse, or primer.
		LoadingStage = ArtilleryLoadingStage.Empty;
		IsReadied = false;
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
			new XElement("PowderCharge", _powderCharge?.Id ?? 0),
			new XElement("Wad", _wad?.Id ?? 0),
			new XElement("PrimerCharge", _primerCharge?.Id ?? 0),
			new XElement("Fuse", _fuse?.Id ?? 0),
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
	public IEnumerable<IGameItem> AllContainedItems => MagazineContents
		.Concat(_installedChamber is null ? [] : new[] { _installedChamber.Parent })
		.Concat(_powderCharge is null ? [] : new[] { _powderCharge })
		.Concat(_wad is null ? [] : new[] { _wad })
		.Concat(_primerCharge is null ? [] : new[] { _primerCharge })
		.Concat(_fuse is null ? [] : new[] { _fuse });
	public Difficulty AimDifficulty => WeaponType.BaseAimDifficulty;
	public Difficulty BaseBlockDifficulty => CurrentAmmo?.AmmoType.DamageProfile.BaseBlockDifficulty ?? Difficulty.Impossible;
	public Difficulty BaseDodgeDifficulty => CurrentAmmo?.AmmoType.DamageProfile.BaseDodgeDifficulty ?? Difficulty.Impossible;
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
		if (fuse is null)
		{
			ReturnContainedItem(actor, ref _fuse);
			_firingSolution = _firingSolution with { Fuse = null };
			Changed = true;
			reason = string.Empty;
			return true;
		}

		var plan = CreateTaggedPlan(actor, _prototype.FuseTag, "fuse");
		if (plan.PlanIsFeasible() != InventoryPlanFeasibility.Feasible)
		{
			reason = "You need a physical artillery fuse with the configured fuse tag.";
			return false;
		}
		var results = plan.ExecuteWholePlan();
		var source = results.First(x => x.OriginalReference?.ToString() == "fuse").PrimaryTarget;
		var installedFuse = TakeOnePhysicalItem(actor, source);
		ReturnContainedItem(actor, ref _fuse);
		ContainItem(installedFuse, ref _fuse);
		plan.FinalisePlanWithExemptions([installedFuse]);
		_firingSolution = _firingSolution with { Fuse = fuse };
		Changed = true;
		reason = string.Empty;
		return true;
	}

	public void Limber(ICharacter actor)
	{
		if (!IsCrewedBy(actor) || LoadingStage != ArtilleryLoadingStage.Empty || AllContainedItems.Any()) return;
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
		if (!IsOperationalFor(loader) || IsReadied || LoadingStage == ArtilleryLoadingStage.Primed ||
			!BlackPowderWeaponEnvironment.CanHandlePowder(loader)) return false;
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
		if (!CanPerform(loader, action, out _)) return false;
		if (LoadingStage is ArtilleryLoadingStage.Cleared or ArtilleryLoadingStage.Vented &&
			!BlackPowderWeaponEnvironment.CanHandleExposedPowder(loader)) return false;
		return CreateStagePlan(loader).PlanIsFeasible() == InventoryPlanFeasibility.Feasible;
	}

	public string WhyCannotLoad(ICharacter loader, bool ignoreEmpty = false, LoadMode mode = LoadMode.Normal)
	{
		if (!IsOperationalFor(loader)) return OperationalReason(loader);
		if (IsReadied) return "The artillery piece is already ignition ready.";
		if (LoadingStage == ArtilleryLoadingStage.Primed) return "The artillery loading drill is complete; ready the piece or unload it.";
		if (!BlackPowderWeaponEnvironment.CanHandlePowder(loader)) return "You cannot work an artillery loading drill while the piece is submerged.";
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
		if (LoadingStage is ArtilleryLoadingStage.Cleared or ArtilleryLoadingStage.Vented &&
			!BlackPowderWeaponEnvironment.CanHandleExposedPowder(loader))
			return "The precipitation is too heavy to handle an exposed powder charge safely.";
		return LoadingStage switch
		{
			ArtilleryLoadingStage.Empty => "You need a tool tagged as an artillery sponge.",
			ArtilleryLoadingStage.Cleared => "You do not have enough physical gunpowder for the main charge.",
			ArtilleryLoadingStage.Charged => "You need a physical item tagged as artillery wadding.",
			ArtilleryLoadingStage.Wadded => LoadingMechanism == ArtilleryLoadingMechanism.RemovableChamber
				? "You need a compatible loaded artillery chamber."
				: "You need compatible physical artillery ammunition.",
			ArtilleryLoadingStage.ProjectileLoaded => "You need a tool tagged as an artillery rammer.",
			ArtilleryLoadingStage.Rammed => "You need a tool tagged as an artillery vent tool.",
			ArtilleryLoadingStage.Vented => "You do not have enough physical gunpowder for the priming charge.",
			_ => "The artillery piece cannot be loaded further."
		};
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

		var stage = LoadingStage;
		var description = stage switch
		{
			ArtilleryLoadingStage.Empty => "sponge and clear",
			ArtilleryLoadingStage.Cleared => "load a charge into",
			ArtilleryLoadingStage.Charged => "wad",
			ArtilleryLoadingStage.Wadded => "load ammunition into",
			ArtilleryLoadingStage.ProjectileLoaded => "ram the load home in",
			ArtilleryLoadingStage.Rammed => "vent",
			ArtilleryLoadingStage.Vented => "prime",
			_ => "work on"
		};
		var plan = CreateStagePlan(loader);
		var results = plan.ExecuteWholePlan().ToList();
		var primary = results.FirstOrDefault(x => x.OriginalReference?.ToString() != "piece")?.PrimaryTarget;
		var exemptions = new List<IGameItem>();
		switch (stage)
		{
			case ArtilleryLoadingStage.Cleared:
				var powderSource = results.First(x => x.OriginalReference?.ToString() == "powder").PrimaryTarget;
				var powder = powderSource.GetByWeight(loader.Body, _prototype.PowderMass);
				ContainItem(powder, ref _powderCharge);
				exemptions.Add(powder);
				primary = powder;
				break;
			case ArtilleryLoadingStage.Charged:
				var wadSource = results.First(x => x.OriginalReference?.ToString() == "wad").PrimaryTarget;
				var wad = TakeOnePhysicalItem(loader, wadSource);
				ContainItem(wad, ref _wad);
				exemptions.Add(wad);
				primary = wad;
				break;
			case ArtilleryLoadingStage.Wadded when LoadingMechanism == ArtilleryLoadingMechanism.RemovableChamber:
				var chamber = results.First(x => x.OriginalReference?.ToString() == "chamber").PrimaryTarget
					.GetItemType<IArtilleryChamber>()!;
				loader.Body.Take(chamber.Parent);
				chamber.Parent.ContainedIn = Parent;
				_installedChamber = chamber;
				exemptions.Add(chamber.Parent);
				primary = chamber.Parent;
				break;
			case ArtilleryLoadingStage.Wadded:
				var ammoSource = results.First(x => x.OriginalReference?.ToString() == "ammunition").PrimaryTarget;
				var ammoItem = TakeOnePhysicalItem(loader, ammoSource);
				ammoItem.ContainedIn = Parent;
				_loadedAmmo = ammoItem.GetItemType<IAmmo>();
				exemptions.Add(ammoItem);
				primary = ammoItem;
				break;
			case ArtilleryLoadingStage.Vented:
				var primerSource = results.First(x => x.OriginalReference?.ToString() == "primer").PrimaryTarget;
				var primer = primerSource.GetByWeight(loader.Body, _prototype.PrimingPowderMass);
				ContainItem(primer, ref _primerCharge);
				exemptions.Add(primer);
				primary = primer;
				break;
		}
		plan.FinalisePlanWithExemptions(exemptions);

		LoadingStage = stage switch
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
		loader.Send($"You {description} {Parent.HowSeen(loader)}{(primary is null ? "." : $" using {primary.HowSeen(loader)}.")}");
		loader.OutputHandler.Handle(new EmoteOutput(new Emote("@ work|works on $0 with $1 as part of its loading drill.",
			loader, Parent, primary ?? (IPerceivable)new DummyPerceivable("the required equipment"))));
	}

	public bool CanReady(ICharacter readier) => IsOperationalFor(readier) &&
		LoadingStage == ArtilleryLoadingStage.Primed && !IsReadied &&
		(!LoadedAmmunitionRequiresFuse() || _fuse is not null) &&
		BlackPowderWeaponEnvironment.CanSustainOpenFlame(readier) &&
		CanPerform(readier, ArtilleryCrewAction.Prime, out _) &&
		CreateTaggedPlan(readier, _prototype.LinstockTag, "linstock").PlanIsFeasible() == InventoryPlanFeasibility.Feasible;
	public string WhyCannotReady(ICharacter readier) =>
		!IsOperationalFor(readier) ? OperationalReason(readier) :
		LoadingStage != ArtilleryLoadingStage.Primed ? "The artillery drill must be completed before it can be readied." :
		IsReadied ? "It is already primed and ready." :
		LoadedAmmunitionRequiresFuse() && _fuse is null ? "The loaded shell or carcass needs a physical artillery fuse." :
		!BlackPowderWeaponEnvironment.CanSustainOpenFlame(readier) ? "A linstock cannot remain lit here; it needs a gaseous atmosphere and reasonably dry weather." :
		"You need a physical tool tagged as an artillery linstock.";
	public bool Ready(ICharacter readier)
	{
		if (!CanReady(readier))
		{
			readier.Send(WhyCannotReady(readier));
			return false;
		}
		var plan = CreateTaggedPlan(readier, _prototype.LinstockTag, "linstock");
		var linstock = plan.ExecuteWholePlan().First(x => x.OriginalReference?.ToString() == "linstock").PrimaryTarget;
		readier.OutputHandler.Handle(new EmoteOutput(new Emote("@ bring|brings $1's glowing match to the prepared vent of $0.",
			readier, Parent, linstock)));
		plan.FinalisePlan();
		IsReadied = true;
		Changed = true;
		return true;
	}

	public bool CanUnready(ICharacter readier) => IsCrewedBy(readier) && IsReadied;
	public string WhyCannotUnready(ICharacter readier) => !IsCrewedBy(readier)
		? "You are not assigned to that artillery crew."
		: "That artillery piece is not readied.";
	public bool Unready(ICharacter readier)
	{
		if (!CanUnready(readier)) return false;
		readier.OutputHandler.Handle(new EmoteOutput(new Emote(
			"@ stand|stands down $0 from immediate ignition readiness.", readier, Parent)));
		IsReadied = false;
		Changed = true;
		return true;
	}

	public bool CanUnload(ICharacter loader) => IsCrewedBy(loader) && !IsReadied &&
		(LoadingStage != ArtilleryLoadingStage.Empty || AllContainedItems.Any());
	public string WhyCannotUnload(ICharacter loader) => !IsCrewedBy(loader)
		? "You are not assigned to that artillery crew."
		: IsReadied
			? "Stand down the artillery piece from ignition readiness before unloading it."
			: "The artillery piece is already empty.";
	public IEnumerable<IGameItem> Unload(ICharacter loader)
	{
		if (!CanUnload(loader)) return [];
		var items = AllContainedItems.Distinct().ToList();
		_installedChamber = null;
		_loadedAmmo = null;
		_powderCharge = null;
		_wad = null;
		_primerCharge = null;
		_fuse = null;
		LoadingStage = ArtilleryLoadingStage.Empty;
		IsReadied = false;
		foreach (var item in items)
		{
			item.ContainedIn = null;
			item.InsertAtSource(Parent.LocationLevelPerceivable ?? loader);
		}
		loader.OutputHandler.Handle(new EmoteOutput(new Emote(
			"@ unload|unloads the physical charge, wadding, ammunition, primer and fuse from $0.", loader, Parent)));
		Changed = true;
		return items;
	}

	public bool CanFire(ICharacter actor, IPerceivable target) =>
		IsOperationalFor(actor) && HasMinimumCrew && ReadyToFire &&
		BlackPowderWeaponEnvironment.CanSustainOpenFlame(actor) &&
		CanPerform(actor, ArtilleryCrewAction.Fire, out _) &&
		CreateTaggedPlan(actor, _prototype.LinstockTag, "linstock").PlanIsFeasible() == InventoryPlanFeasibility.Feasible;
	public string WhyCannotFire(ICharacter actor, IPerceivable target) =>
		!IsOperationalFor(actor) ? OperationalReason(actor) :
		!HasMinimumCrew ? $"That artillery piece requires at least {_prototype.MinimumCrew.ToString(actor)} active crew members." :
		!ReadyToFire ? "The artillery piece is not ready to fire." :
		!BlackPowderWeaponEnvironment.CanSustainOpenFlame(actor) ? "The ignition flame cannot burn in this atmosphere or precipitation." :
		!CanPerform(actor, ArtilleryCrewAction.Fire, out var reason) ? reason :
		"You need a physical tool tagged as an artillery linstock to fire the piece.";
	public void Fire(ICharacter actor, IPerceiver target, Outcome shotOutcome, Outcome coverOutcome, OpposedOutcome defenseOutcome,
		IBodypart bodypart, IEmoteOutput defenseEmote, IPerceiver originalTarget)
	{
		if (!CanFire(actor, target))
		{
			actor.Send(WhyCannotFire(actor, target));
			return;
		}

		var firingTarget = target ?? ResolveIndirectTarget();
		var linstockPlan = CreateTaggedPlan(actor, _prototype.LinstockTag, "linstock");
		var linstock = linstockPlan.ExecuteWholePlan()
			.First(x => x.OriginalReference?.ToString() == "linstock").PrimaryTarget;

		if (shotOutcome == Outcome.NotTested)
		{
			shotOutcome = Gameworld.GetCheck(CheckType.FireArtillery)
				.Check(actor, WeaponType.BaseAimDifficulty, WeaponType.FireTrait, firingTarget ?? Parent).Outcome;
		}

		var ammo = LoadingMechanism == ArtilleryLoadingMechanism.RemovableChamber
			? _installedChamber!.LoadedAmmunition!
			: _loadedAmmo!;
		var projectile = ammo.GetFiredItem ?? ammo.Parent;
		var firedSeparateProjectile = projectile != ammo.Parent;
		var dischargeEmote = BlackPowderWeaponEnvironment.CanPropagateSound(actor)
			? "@ apply|applies $1's burning match to $0, firing it with a thunderous discharge."
			: "@ apply|applies $1's burning match to $0; its charge flashes and drives the projectile without a report.";
		actor.OutputHandler.Handle(new EmoteOutput(new Emote(dischargeEmote, actor, Parent, linstock),
			style: OutputStyle.CombatMessage));
		linstockPlan.FinalisePlan();
		ammo.Fire(actor, firingTarget, shotOutcome, coverOutcome, defenseOutcome, bodypart, projectile, WeaponType, defenseEmote,
			new RangedFireContext(0, Math.Max(1, ammo.AmmoType.ProjectileCount), ammo.AmmoType.ScatterType));
		if (BlackPowderWeaponEnvironment.CanPropagateSound(actor))
		{
			actor.Location.HandleAudioEcho("An artillery discharge can be heard {0}.", AudioVolume.ExtremelyLoud,
				Parent, actor.RoomLayer, true, "artillery");
		}
		if (LoadingMechanism == ArtilleryLoadingMechanism.RemovableChamber)
		{
			var firedAmmunition = _installedChamber!.Unload();
			if (firedSeparateProjectile)
			{
				firedAmmunition?.Parent.Delete();
			}
			_installedChamber.Parent.ContainedIn = null;
			_installedChamber.Parent.InsertAtSource(Parent.LocationLevelPerceivable ?? actor);
			_installedChamber = null;
		}
		else
		{
			if (firedSeparateProjectile)
			{
				_loadedAmmo?.Parent.Delete();
			}
			_loadedAmmo = null;
		}
		DeleteConsumedLoad(ref _powderCharge);
		DeleteConsumedLoad(ref _wad);
		DeleteConsumedLoad(ref _primerCharge);
		DeleteConsumedLoad(ref _fuse);
		LoadingStage = ArtilleryLoadingStage.Empty;
		_firingSolution = null;
		IsReadied = false;
		Changed = true;
	}

	public override bool PreventsMovement()
	{
		PruneCrew();
		return IsEmplaced || IsMounted || LoadingStage != ArtilleryLoadingStage.Empty ||
		       AllContainedItems.Any() || _crew.Count > 0;
	}
	public override string WhyPreventsMovement(ICharacter mover) =>
		IsEmplaced ? "it is emplaced and must be limbered first" :
		IsMounted ? "it is installed on an artillery mount" :
		LoadingStage != ArtilleryLoadingStage.Empty || AllContainedItems.Any()
			? "it contains an unsafe artillery load"
			: "it has an active artillery crew";

	public override double ComponentWeight => AllContainedItems.Distinct().Sum(x => x.Weight);

	public override double ComponentBuoyancy(double fluidDensity) =>
		AllContainedItems.Distinct().Sum(x => x.Buoyancy(fluidDensity));

	public override void FinaliseLoad()
	{
		foreach (var item in AllContainedItems.Distinct()) item.FinaliseLoadTimeTasks();
	}

	public override void Login()
	{
		foreach (var item in AllContainedItems.Distinct()) item.Login();
	}

	public override void Quit()
	{
		base.Quit();
		_crew.Clear();
		foreach (var item in AllContainedItems.Distinct()) item.Quit();
	}

	public override void Delete()
	{
		base.Delete();
		_crew.Clear();
		foreach (var item in AllContainedItems.Distinct().ToList()) item.Delete();
	}

	private bool IsOperationalFor(ICharacter actor)
	{
		return IsAssignedAndPresent(actor) && (!_prototype.RequiresEmplacement || IsEmplaced || IsMounted);
	}
	private bool IsAssignedAndPresent(ICharacter actor)
	{
		PruneCrew();
		return _crew.ContainsKey(actor) && actor.Location == Parent.Location && !actor.State.HasFlag(CharacterState.Dead) &&
		       CharacterState.Able.HasFlag(actor.State);
	}
	private string AssignmentReason(ICharacter actor) =>
		!_crew.ContainsKey(actor) ? "You must join that artillery crew first." :
		actor.Location != Parent.Location ? "You must remain beside the artillery piece to operate it." :
		"You must be able to operate that artillery piece.";
	private string OperationalReason(ICharacter actor) =>
		!IsAssignedAndPresent(actor) ? AssignmentReason(actor) : "The artillery piece must be emplaced or mounted first.";

	private bool LoadedAmmunitionRequiresFuse()
	{
		var ammunition = LoadingMechanism == ArtilleryLoadingMechanism.RemovableChamber
			? _installedChamber?.LoadedAmmunition
			: _loadedAmmo as IArtilleryAmmunition;
		return ammunition?.PayloadType is ArtilleryPayloadType.Shell or ArtilleryPayloadType.Carcass;
	}

	private IAmmo? CurrentAmmo => LoadingMechanism == ArtilleryLoadingMechanism.RemovableChamber
		? _installedChamber?.LoadedAmmunition
		: _loadedAmmo;

	private IInventoryPlan CreateStagePlan(ICharacter actor)
	{
		return LoadingStage switch
		{
			ArtilleryLoadingStage.Empty => CreateTaggedPlan(actor, _prototype.SpongeTag, "sponge"),
			ArtilleryLoadingStage.Cleared => CreateCommodityPlan(actor, _prototype.PowderMass, "powder"),
			ArtilleryLoadingStage.Charged => CreateTaggedPlan(actor, _prototype.WaddingTag, "wad"),
			ArtilleryLoadingStage.Wadded when LoadingMechanism == ArtilleryLoadingMechanism.RemovableChamber =>
				CreateHoldPlan(actor, item => item.GetItemType<IArtilleryChamber>() is { IsLoaded: true } chamber &&
					ProfilesAreCompatible(_prototype.ArtilleryProfile, chamber.ArtilleryProfile), "chamber"),
			ArtilleryLoadingStage.Wadded => CreateHoldPlan(actor, item =>
				item.GetItemType<IArtilleryAmmunition>() is { } ammo &&
				ProfilesAreCompatible(_prototype.ArtilleryProfile, ammo.ArtilleryProfile), "ammunition"),
			ArtilleryLoadingStage.ProjectileLoaded => CreateTaggedPlan(actor, _prototype.RammerTag, "rammer"),
			ArtilleryLoadingStage.Rammed => CreateTaggedPlan(actor, _prototype.VentToolTag, "venttool"),
			ArtilleryLoadingStage.Vented => CreateCommodityPlan(actor, _prototype.PrimingPowderMass, "primer"),
			_ => CreateHoldPlan(actor, _ => false, "invalid")
		};
	}

	private IInventoryPlan CreateTaggedPlan(ICharacter actor, ITag? tag, string reference)
	{
		return CreateHoldPlan(actor, item => tag is not null && item.IsA(tag), reference);
	}

	private IInventoryPlan CreateCommodityPlan(ICharacter actor, double mass, string reference)
	{
		return CreateHoldPlan(actor, item => item.GetItemType<ICommodity>() is { } commodity &&
			commodity.Material == _prototype.GunpowderMaterial && commodity.Weight >= mass, reference, 0);
	}

	private IInventoryPlan CreateHoldPlan(ICharacter actor, Func<IGameItem, bool> predicate, string reference,
		int quantity = 1)
	{
		var template = new InventoryPlanTemplate(Gameworld,
		[
			new InventoryPlanPhaseTemplate(1,
			[
				new InventoryPlanActionHold(Gameworld, 0, 0, predicate, null, quantity)
				{
					OriginalReference = reference,
					QuantityIsOptional = true,
					ItemsAlreadyInPlaceOverrideFitnessScore = true
				}
			])
		]);
		return template.CreatePlan(actor);
	}

	private static IGameItem TakeOnePhysicalItem(ICharacter actor, IGameItem source)
	{
		var stack = source.GetItemType<IStackable>();
		if (stack is not null && stack.Quantity > 1)
		{
			var split = stack.Split(1);
			split.Login();
			split.HandleEvent(EventType.ItemFinishedLoading, split);
			return split;
		}
		actor.Body.Take(source);
		return source;
	}

	private void ContainItem(IGameItem item, ref IGameItem? field)
	{
		item.ContainedIn = Parent;
		field = item;
	}

	private static void ReturnContainedItem(ICharacter actor, ref IGameItem? item)
	{
		if (item is null) return;
		item.ContainedIn = null;
		item.InsertAtSource(actor);
		item = null;
	}

	private static void DeleteConsumedLoad(ref IGameItem? item)
	{
		if (item is null) return;
		item.ContainedIn = null;
		item.Delete();
		item = null;
	}

	private IGameItem? LoadContainedItem(XElement root, string elementName)
	{
		var item = Gameworld.TryGetItem((long?)root.Element(elementName) ?? 0, true);
		if (item is not null) item.ContainedIn = Parent;
		return item;
	}

	private void NormaliseLoadingStage()
	{
		if (LoadingStage >= ArtilleryLoadingStage.Charged && _powderCharge is null)
		{
			LoadingStage = ArtilleryLoadingStage.Cleared;
		}
		if (LoadingStage >= ArtilleryLoadingStage.Wadded && _wad is null)
		{
			LoadingStage = ArtilleryLoadingStage.Charged;
		}
		if (LoadingStage >= ArtilleryLoadingStage.ProjectileLoaded &&
			(LoadingMechanism == ArtilleryLoadingMechanism.RemovableChamber
				? _installedChamber?.IsLoaded != true
				: _loadedAmmo is null))
		{
			LoadingStage = ArtilleryLoadingStage.Wadded;
		}
		if (LoadingStage >= ArtilleryLoadingStage.Primed && _primerCharge is null)
		{
			LoadingStage = ArtilleryLoadingStage.Vented;
		}
	}

	private IReadOnlyCollection<ICharacter> ActiveCrew()
	{
		PruneCrew();
		return _crew.Keys.ToList();
	}

	private void PruneCrew()
	{
		var departed = _crew.Keys
			.Where(x => x.Location != Parent.Location || x.State.HasFlag(CharacterState.Dead) ||
			            !CharacterState.Able.HasFlag(x.State))
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
