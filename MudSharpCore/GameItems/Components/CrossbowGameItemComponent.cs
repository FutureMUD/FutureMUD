using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Combat;
using MudSharp.Construction;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Prototypes;
using MudSharp.Health;
using MudSharp.RPG.Checks;

namespace MudSharp.GameItems.Components;

public class CrossbowGameItemComponent : GameItemComponent, IEmplaceableRangedWeapon, IMeleeWeapon, IConditionDegradingComponent
{
    protected CrossbowGameItemComponentProto _prototype;
    public override IGameItemComponentProto Prototype => _prototype;
	private readonly List<IAmmo> _magazine = new();
    public bool ConditionDegradesOnUse => _prototype.ConditionMaintenance.ConditionDegradesOnUse;
    public int ItemQualityStages => _prototype.ConditionMaintenance.QualityPenaltyStages(Parent);

    public void UseCondition(ItemConditionUseContext context)
    {
        _prototype.ConditionMaintenance.UseCondition(Parent, context);
    }

    protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
    {
        _prototype = (CrossbowGameItemComponentProto)newProto;
    }

    #region Constructors

    public CrossbowGameItemComponent(CrossbowGameItemComponentProto proto, IGameItem parent, bool temporary = false) :
        base(parent, proto, temporary)
    {
        _prototype = proto;
    }

    public CrossbowGameItemComponent(MudSharp.Models.GameItemComponent component, CrossbowGameItemComponentProto proto,
        IGameItem parent) : base(component, parent)
    {
        _prototype = proto;
        _noSave = true;
        LoadFromXml(XElement.Parse(component.Definition));
        _noSave = false;
    }

    public CrossbowGameItemComponent(CrossbowGameItemComponent rhs, IGameItem newParent, bool temporary = false) : base(
        rhs, newParent, temporary)
    {
        _prototype = rhs._prototype;

        // Do not duplicate or share the physical bolt when copying an item.
        LoadedAmmo = null;
        PrimaryWieldedLocation = rhs.PrimaryWieldedLocation;
        IsReadied = rhs.IsReadied;
		IsEmplaced = false;
    }

    protected void LoadFromXml(XElement root)
    {
        PrimaryWieldedLocation =
            Gameworld.BodypartPrototypes.Get(long.Parse(root.Element("Wielded")?.Value ?? "0")) as IWield;
		LoadedAmmo = Gameworld.TryGetItem(long.Parse(root.Element("Loaded")?.Value ?? "0"), true)?.GetItemType<IAmmo>();
		foreach (var element in root.Element("Magazine")?.Elements("Item") ?? [])
		{
			var ammunition = Gameworld.TryGetItem(long.Parse(element.Value), true)?.GetItemType<IAmmo>();
			if (ammunition is not null && ammunition != LoadedAmmo)
			{
				_magazine.Add(ammunition);
			}
		}
        IsReadied = bool.TryParse(root.Element("IsReadied")?.Value, out var isReadied) && isReadied;
		IsEmplaced = (bool?)root.Element("IsEmplaced") ?? false;
    }

    public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
    {
        return new CrossbowGameItemComponent(this, newParent, temporary);
    }

    #endregion

    #region Saving

    protected override string SaveToXml()
    {
        return new XElement("Definition",
            new XElement("Wielded", PrimaryWieldedLocation?.Id ?? 0),
            new XElement("Loaded", LoadedAmmo?.Parent.Id ?? 0),
			new XElement("Magazine", _magazine.Select(x => new XElement("Item", x.Parent.Id))),
            new XElement("IsReadied", IsReadied),
			new XElement("IsEmplaced", IsEmplaced)
        ).ToString();
    }

    #endregion

    public override bool DescriptionDecorator(DescriptionType type)
    {
        switch (type)
        {
            case DescriptionType.Full:
                return true;
            case DescriptionType.Evaluate:
                return true;
            default:
                return false;
        }
    }

    public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
        bool colour, PerceiveIgnoreFlags flags)
    {
        switch (type)
        {
            case DescriptionType.Full:
                return
                    $"{description}\n\n{(LoadedAmmo != null ? $"It is loaded with {LoadedAmmo.Parent.HowSeen(voyeur)}" : "")}";
            case DescriptionType.Evaluate:
                return
                    $"This is a crossbow of type {WeaponType.Name.Colour(Telnet.Cyan)}.\nIt takes bolts of type {WeaponType.SpecificAmmunitionGrade.Colour(Telnet.Green)}.\nThis is also a melee weapon of type {((IMeleeWeapon)this).WeaponType.Name.Colour(Telnet.Cyan)}.\nIt is classified as {WeaponType.Classification.Describe().Colour(Telnet.Green)}.";
            default:
                return description;
        }
    }

    #region Implementation of IRangedWeapon

    public bool CanBeAimedAtSelf => true;
    public string FireVerbForEchoes => "fire|fires";
    public IRangedWeaponType WeaponType => _prototype.RangedWeaponType;
    public Difficulty AimDifficulty => _prototype.RangedWeaponType.BaseAimDifficulty;
    WeaponClassification IRangedWeapon.Classification => _prototype.RangedWeaponType.Classification;
    WeaponClassification IMeleeWeapon.Classification => _prototype.MeleeWeaponType.Classification;

    public Difficulty BaseBlockDifficulty
        => LoadedAmmo?.AmmoType.DamageProfile.BaseBlockDifficulty ?? Difficulty.Automatic;

    public Difficulty BaseDodgeDifficulty
        => LoadedAmmo?.AmmoType.DamageProfile.BaseDodgeDifficulty ?? Difficulty.Automatic;

    private IWield _primaryWieldedLocation;

    public IWield PrimaryWieldedLocation
    {
        get => _primaryWieldedLocation;
        set
        {
            _primaryWieldedLocation = value;
            Changed = true;
        }
    }

    public bool AlwaysRequiresTwoHandsToWield => WeaponType.AlwaysRequiresTwoHandsToWield;

    /// <inheritdoc />
    public bool CanWield(ICharacter actor)
    {
		if (_prototype.RequiresEmplacement)
		{
			return false;
		}
        return _prototype.CanWieldProg?.ExecuteBool(false, actor, Parent) ?? true;
    }

    /// <inheritdoc />
    public string WhyCannotWield(ICharacter actor)
    {
		if (_prototype.RequiresEmplacement)
		{
			return $"{Parent.HowSeen(actor, true)} is an emplaced crossbow and cannot be wielded.";
		}
        return _prototype.WhyCannotWieldProg?.ExecuteString(actor, Parent) ?? "You can't wield that for an unknown reason.";
    }

    public bool ReadyToFire => IsLoaded && IsReadied;

	public bool IsLoaded => LoadedAmmo != null;
    public bool IsReadied { get; set; }
	public bool IsEmplaced { get; private set; }

	public bool Emplace(ICharacter actor, out string reason)
	{
		if (!_prototype.RequiresEmplacement)
		{
			reason = $"{Parent.HowSeen(actor, true)} does not require emplacement.";
			return false;
		}
		if (Parent.InInventoryOf is not null || Parent.Location != actor.Location)
		{
			reason = "You must place that crossbow in the room before emplacing it.";
			return false;
		}
		IsEmplaced = true;
		Changed = true;
		reason = string.Empty;
		return true;
	}

	public bool Limber(ICharacter actor, out string reason)
	{
		if (!IsEmplaced)
		{
			reason = $"{Parent.HowSeen(actor, true)} is not emplaced.";
			return false;
		}
		if (IsReadied || LoadedAmmo is not null)
		{
			reason = "You must unready and unload that crossbow before moving it.";
			return false;
		}
		IsEmplaced = false;
		Changed = true;
		reason = string.Empty;
		return true;
	}

    public bool CanReady(ICharacter readier)
    {
		if (_prototype.RequiresEmplacement && !IsEmplaced)
		{
			return false;
		}

        if (!IsLoaded)
        {
            return false;
        }

        if (IsReadied)
        {
            return false;
        }

        if (WeaponType.RequiresFreeHandToReady &&
            readier.Body.FunctioningWieldingLocationsAvailableFor(Parent).Count() < 2)
        {
            return false;
        }

        if (_prototype.ReadyTemplate?.CreatePlan(readier).PlanIsFeasible() is { } feasibility &&
            feasibility != InventoryPlanFeasibility.Feasible)
        {
            return false;
        }

        return true;
    }

    public string WhyCannotReady(ICharacter readier)
    {
		if (_prototype.RequiresEmplacement && !IsEmplaced)
		{
			return $"You must emplace {Parent.HowSeen(readier)} before readying it.";
		}

        if (!IsLoaded)
        {
            return $"You must first load a quarrel before you can ready {Parent.HowSeen(readier)}.";
        }

        if (IsReadied)
        {
            return $"{Parent.HowSeen(readier, true)} is already ready to be fired, simply aim and fire.";
        }

        if (WeaponType.RequiresFreeHandToReady &&
            readier.Body.FunctioningWieldingLocationsAvailableFor(Parent).Count() < 2)
        {
            return
                $"You need at least one free {readier.Body.WielderDescriptionSingular} to ready {Parent.HowSeen(readier)}.";
        }

        if (_prototype.ReadyTemplate is not null)
        {
            var plan = _prototype.ReadyTemplate.CreatePlan(readier);
            return plan.PlanIsFeasible() switch
            {
                InventoryPlanFeasibility.NotFeasibleMissingItems =>
                    $"You need a spanning tool tagged {_prototype.RequiredSpanningToolTag!.Name.ColourName()} to ready {Parent.HowSeen(readier)}.",
                InventoryPlanFeasibility.NotFeasibleNotEnoughHands or
                    InventoryPlanFeasibility.NotFeasibleNotEnoughWielders =>
                    $"You don't have enough working {readier.Body.WielderDescriptionPlural} to use a spanning tool on {Parent.HowSeen(readier)}.",
                _ => throw new ApplicationException("Unknown spanning-tool feasibility in CrossbowGameItemComponent.")
            };
        }

        throw new ApplicationException("Unknown reason in CrossbowGameItemComponent.WhyCannotReady.");
    }

    public bool Ready(ICharacter readier)
    {
        if (!CanReady(readier))
        {
            readier.Send(WhyCannotReady(readier));
            return false;
        }

        if (_prototype.ReadyTemplate is not null)
        {
            var plan = _prototype.ReadyTemplate.CreatePlan(readier);
            var results = plan.ExecuteWholePlan();
            var tool = results.First(x => x.OriginalReference?.ToString() == "spanningtool").PrimaryTarget;
            readier.OutputHandler.Handle(new EmoteOutput(
                new Emote(_prototype.ReadyEmote, readier, readier, Parent, tool, LoadedAmmo.Parent)));
            plan.FinalisePlan();
        }
        else
        {
            readier.OutputHandler.Handle(new EmoteOutput(
                new Emote("@ wind|winds up $0 until it is ready to fire $1.", readier, Parent, LoadedAmmo.Parent)));
        }

        IsReadied = true;
        Changed = true;
        return true;
    }

    public bool CanUnready(ICharacter readier)
    {
        return IsReadied;
    }

    public string WhyCannotUnready(ICharacter readier)
    {
        if (!IsReadied)
        {
            return $"{Parent.HowSeen(readier, true)} is not ready to fire, and has no need of being unreadied.";
        }

        throw new ApplicationException("Unknown reason in CrossbowGameItemComponent.WhyCannotUnready.");
    }

    public bool Unready(ICharacter readier)
    {
        if (readier == null)
        {
            IsReadied = false;
            return true;
        }

        if (!CanUnready(readier))
        {
            readier.Send(WhyCannotUnready(readier));
            return false;
        }

        readier.OutputHandler.Handle(new EmoteOutput(
            new Emote("@ unwind|unwinds $0 until the bow returns to its neutral position.", readier, Parent)));
        IsReadied = false;
        Changed = true;
        return true;
    }

    public int LoadStage => 0;

    public IAmmo LoadedAmmo { get; set; }

	public IEnumerable<IGameItem> MagazineContents => new[] { LoadedAmmo?.Parent }.SelectNotNull(x => x).Concat(_magazine.Select(x => x.Parent));
    public IEnumerable<IGameItem> AllContainedItems => MagazineContents;

    public bool CanUnload(ICharacter loader)
    {
        return LoadedAmmo != null && !IsReadied;
    }

    public string WhyCannotUnload(ICharacter loader)
    {
        if (LoadedAmmo == null)
        {
            return $"You cannot unload {Parent.HowSeen(loader)} because it is not loaded.";
        }

        if (IsReadied)
        {
            return
                $"You cannot unload {Parent.HowSeen(loader)} until you have unwound the tension and unreadied it for fire.";
        }

		return $"You cannot unload {Parent.HowSeen(loader)} for an unknown reason.";
    }

    public IEnumerable<IGameItem> Unload(ICharacter loader)
    {
        IAmmo ammo = LoadedAmmo;
        LoadedAmmo = null;
        loader.OutputHandler.Handle(
            new EmoteOutput(new Emote("@ unload|unloads $0 from $1.", loader, ammo.Parent, Parent)));
        if (!loader.Body.CanGet(ammo.Parent, 0))
        {
            ammo.Parent.InsertAtSource(loader);
        }
        else
        {
            loader.Body.Get(ammo.Parent, silent: true);
        }

        Changed = true;
        return new[] { ammo.Parent };
    }

    public bool CanLoad(ICharacter loader, bool ignoreEmpty = false, LoadMode mode = LoadMode.Normal)
    {
		if (_prototype.RequiresEmplacement && !IsEmplaced)
		{
			return false;
		}

		if (LoadedAmmo != null && _magazine.Count + 1 >= _prototype.MagazineCapacity)
        {
            return false;
        }

        IInventoryPlan plan = _prototype.LoadTemplate.CreatePlan(loader);
        return plan.PlanIsFeasible() == InventoryPlanFeasibility.Feasible;
    }

    public string WhyCannotLoad(ICharacter loader, bool ignoreEmpty = false, LoadMode mode = LoadMode.Normal)
    {
		if (_prototype.RequiresEmplacement && !IsEmplaced)
		{
			return $"You must emplace {Parent.HowSeen(loader)} before loading it.";
		}

		if (LoadedAmmo != null && _magazine.Count + 1 >= _prototype.MagazineCapacity)
        {
            return $"You cannot load {Parent.HowSeen(loader)} because it is already loaded!";
        }

        IInventoryPlan plan = _prototype.LoadTemplate.CreatePlan(loader);
        switch (plan.PlanIsFeasible())
        {
            case InventoryPlanFeasibility.NotFeasibleMissingItems:
                return $"You cannot load {Parent.HowSeen(loader)} because you don't have any suitable ammunition";
            case InventoryPlanFeasibility.NotFeasibleNotEnoughHands:
            case InventoryPlanFeasibility.NotFeasibleNotEnoughWielders:
                return
                    $"You cannot load {Parent.HowSeen(loader)} because you don't have enough working {loader.Body.WielderDescriptionPlural}.";
        }

		return $"You cannot load {Parent.HowSeen(loader)} with the items you currently have available.";
    }

    public void Load(ICharacter loader, bool ignoreEmpty = false, LoadMode mode = LoadMode.Normal)
    {
        IInventoryPlan plan = _prototype.LoadTemplate.CreatePlan(loader);
        plan.ExecuteWholePlan();
        IGameItem ammo =
            loader.Body.HeldItems.FirstOrDefault(
                x =>
                    (x.GetItemType<IAmmo>()?.AmmoType.SpecificType.Equals(WeaponType.SpecificAmmunitionGrade) ??
                     false) &&
                    (x.GetItemType<IAmmo>()?.AmmoType.RangedWeaponTypes.Contains(RangedWeaponType.Crossbow) ?? false));
        if (ammo == null)
        {
            return;
        }

        IStackable ammoStack = ammo.GetItemType<IStackable>();
        if (ammoStack != null && ammoStack.Quantity > 1)
        {
            IGameItem newammo = ammoStack.Split(1);
			AddToMagazine(newammo.GetItemType<IAmmo>()!);
            loader.OutputHandler.Handle(new EmoteOutput(new Emote("@ load|loads $1 in $0.", loader, Parent, newammo)));
            plan.FinalisePlan();
        }
        else
        {
            loader.Body.Take(ammo);
			AddToMagazine(ammo.GetItemType<IAmmo>()!);
            loader.OutputHandler.Handle(new EmoteOutput(new Emote("@ load|loads $1 in $0.", loader, Parent, ammo)));
            plan.FinalisePlanWithExemptions(new List<IGameItem> { ammo });
        }

        Changed = true;
    }

    public bool CanFire(ICharacter actor, IPerceivable target)
    {
		return (!_prototype.RequiresEmplacement || IsEmplaced) && LoadedAmmo != null && IsReadied;
    }

    public string WhyCannotFire(ICharacter actor, IPerceivable target)
    {
		if (_prototype.RequiresEmplacement && !IsEmplaced)
		{
			return $"You must emplace {Parent.HowSeen(actor)} before firing it.";
		}

        if (LoadedAmmo == null)
        {
            return $"You cannot fire {Parent.HowSeen(actor)} because it is not loaded.";
        }

        if (!IsReadied)
        {
            return $"You have not wound up {Parent.HowSeen(actor)}. You must do so before firing.";
        }

		return $"You cannot fire {Parent.HowSeen(actor)} in its current state.";
    }

    public void Fire(ICharacter actor, IPerceiver target, Outcome shotOutcome, Outcome coverOutcome,
        OpposedOutcome defenseOutcome, IBodypart bodypart, IEmoteOutput defenseEmote, IPerceiver originalTarget)
    {
		if (!CanFire(actor, target))
		{
			actor.Send(WhyCannotFire(actor, target));
			return;
		}

        actor.OutputHandler.Handle(new EmoteOutput(
            new Emote("@ pull|pulls the trigger on $1 and send|sends $2 whizzing off towards $0.", actor,
                target ?? (IPerceivable)new DummyPerceivable("the sky"), Parent, LoadedAmmo.Parent),
            style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
        IAmmo ammo = LoadedAmmo;
		LoadedAmmo = _magazine.FirstOrDefault();
		if (LoadedAmmo is not null)
		{
			_magazine.RemoveAt(0);
		}
		IsReadied = LoadedAmmo is not null && _prototype.RepeatsWithoutReady;
        Changed = true;
        ammo.Fire(actor, target, shotOutcome, coverOutcome, defenseOutcome, bodypart, ammo.Parent, WeaponType,
            defenseEmote);
        UseCondition(new ItemConditionUseContext(ItemConditionUseKind.RangedFire, shotOutcome,
            (int)(defenseOutcome?.Degree ?? OpposedOutcomeDegree.None)));
    }

    #endregion

    #region Implementation of IDamageSource

    public IDamage GetDamage(IPerceiver perceiverSource, OpposedOutcome opposedOutcome)
    {
        throw new NotImplementedException();
    }

    #endregion

    #region Implementation of IUseTrait

    public ITraitDefinition Trait => _prototype.RangedWeaponType.FireTrait;

    #endregion

    public override void FinaliseLoad()
    {
        LoadedAmmo?.Parent.FinaliseLoadTimeTasks();
		foreach (var ammunition in _magazine)
		{
			ammunition.Parent.FinaliseLoadTimeTasks();
		}
    }

	private void AddToMagazine(IAmmo ammunition)
	{
		if (LoadedAmmo is null)
		{
			LoadedAmmo = ammunition;
			return;
		}

		_magazine.Add(ammunition);
		ammunition.Parent.ContainedIn = Parent;
	}


    #region Implementation of IMeleeWeapon

    IWeaponType IMeleeWeapon.WeaponType => _prototype.MeleeWeaponType;

    #endregion

    public override void Quit()
    {
        base.Quit();
        LoadedAmmo?.Parent.Quit();
		foreach (var ammunition in _magazine)
		{
			ammunition.Parent.Quit();
		}
    }

    public override void Delete()
    {
        base.Delete();
        LoadedAmmo?.Parent.ContainedIn = null;
        LoadedAmmo?.Parent.Delete();
		foreach (var ammunition in _magazine)
		{
			ammunition.Parent.ContainedIn = null;
			ammunition.Parent.Delete();
		}
		_magazine.Clear();
    }

    public override void Login()
    {
        LoadedAmmo?.Login();
		foreach (var ammunition in _magazine)
		{
			ammunition.Parent.Login();
		}
    }

	public override bool PreventsMovement() => _prototype.RequiresEmplacement && IsEmplaced;
	public override string WhyPreventsMovement(ICharacter mover) => "it is emplaced and must be limbered first";
}
