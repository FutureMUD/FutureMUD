using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using MudSharp.Body;
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Body.Traits;
using MudSharp.Climate;
using MudSharp.Combat;
using MudSharp.Commands.Trees;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Concrete;
using MudSharp.Events;
using MudSharp.Form.Audio;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.GameItems.Prototypes;
using MudSharp.Health;
using MudSharp.RPG.Checks;
using System.Reflection.Metadata.Ecma335;

namespace MudSharp.GameItems.Components;

public class MusketGameItemComponent : GameItemComponent, IJammableWeapon, IBelt, IMeleeWeapon, IEmplaceableRangedWeapon,
    IConditionDegradingComponent
{
    protected MusketGameItemComponentProto _prototype;
    public override IGameItemComponentProto Prototype => _prototype;
    public bool ConditionDegradesOnUse => _prototype.ConditionMaintenance.ConditionDegradesOnUse;
    public int ItemQualityStages => _prototype.ConditionMaintenance.QualityPenaltyStages(Parent);

    public void UseCondition(ItemConditionUseContext context)
    {
        _prototype.ConditionMaintenance.UseCondition(Parent, context);
    }

    protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
    {
        _prototype = (MusketGameItemComponentProto)newProto;
    }

    #region Constructors
    public MusketGameItemComponent(MusketGameItemComponentProto proto, IGameItem parent, bool temporary = false) : base(parent, proto, temporary)
    {
        _prototype = proto;
		IgnitionStrikesRemaining = 0;
    }

    public MusketGameItemComponent(Models.GameItemComponent component, MusketGameItemComponentProto proto, IGameItem parent) : base(component, parent)
    {
        _prototype = proto;
        _noSave = true;
        LoadFromXml(XElement.Parse(component.Definition));
        _noSave = false;
    }

    public MusketGameItemComponent(MusketGameItemComponent rhs, IGameItem newParent, bool temporary = false) : base(rhs, newParent, temporary)
    {
		_prototype = rhs._prototype;
		// A copied item must not share physical barrel contents or attached items with its source.
		IsReadied = false;
		NeedsCleaning = rhs.NeedsCleaning;
		IsJammed = rhs.IsJammed;
		LoadStage = 0;
		TapLoaded = false;
		MatchLit = false;
		WheelWound = false;
		IgnitionStrikesRemaining = 0;
		IsBlankLoad = false;
		IsEmplaced = rhs.IsEmplaced;
		PrimaryWieldedLocation = rhs.PrimaryWieldedLocation;
    }

    protected void LoadFromXml(XElement root)
    {
        IsReadied = bool.TryParse(root.Element("IsReadied")?.Value, out var isReadied) && isReadied;
        NeedsCleaning = !bool.TryParse(root.Element("NeedsCleaning")?.Value, out var needsCleaning) || needsCleaning;
        IsJammed = bool.TryParse(root.Element("IsJammed")?.Value, out var isJammed) && isJammed;
        LoadStage = int.TryParse(root.Element("LoadStage")?.Value, out var loadStage) ? loadStage : 0;
		TapLoaded = bool.TryParse(root.Element("TapLoaded")?.Value, out var tapLoaded) && tapLoaded;
		MatchLit = bool.TryParse(root.Element("MatchLit")?.Value, out var matchLit) && matchLit;
		WheelWound = bool.TryParse(root.Element("WheelWound")?.Value, out var wheelWound) && wheelWound;
		IgnitionStrikesRemaining = Math.Max(0, int.TryParse(root.Element("IgnitionStrikesRemaining")?.Value, out var strikes) ? strikes : 0);
		IsBlankLoad = bool.TryParse(root.Element("IsBlankLoad")?.Value, out var isBlankLoad) && isBlankLoad;
		IsEmplaced = bool.TryParse(root.Element("IsEmplaced")?.Value, out var isEmplaced) && isEmplaced;
        PrimaryWieldedLocation = Gameworld.BodypartPrototypes.Get(long.Parse(root.Element("Wielded")?.Value ?? "0")) as IWield;
        foreach (XElement element in root.Element("Magazine")?.Elements("Item") ?? [])
        {
            IGameItem item = Gameworld.TryGetItem(long.Parse(element.Value), true);
            if (item is null)
            {
                continue;
            }

            _magazineContents.Add(item);
        }

        IGameItem connectedItem = Gameworld.TryGetItem(long.Parse(root.Element("Bayonet")?.Value ?? "0"), true);
        if (connectedItem?.IsItemType<IBeltable>() == true)
        {
            _bayonet = connectedItem.GetItemType<IBeltable>();
            connectedItem.GetItemType<IBeltable>().ConnectedTo = this;
        }

        connectedItem = Gameworld.TryGetItem(long.Parse(root.Element("Sights")?.Value ?? "0"), true);
        if (connectedItem?.IsItemType<IBeltable>() == true)
        {
            _sights = connectedItem.GetItemType<IBeltable>();
            connectedItem.GetItemType<IBeltable>().ConnectedTo = this;
        }

        connectedItem = Gameworld.TryGetItem(long.Parse(root.Element("Ramrod")?.Value ?? "0"), true);
        if (connectedItem?.IsItemType<IBeltable>() == true)
        {
            _ramrod = connectedItem.GetItemType<IBeltable>();
            connectedItem.GetItemType<IBeltable>().ConnectedTo = this;
        }

		_matchCord = Gameworld.TryGetItem(long.Parse(root.Element("MatchCord")?.Value ?? "0"), true);
		if (_matchCord is not null)
		{
			_matchCord.ContainedIn = Parent;
		}
		_ignitionStone = Gameworld.TryGetItem(long.Parse(root.Element("IgnitionStone")?.Value ?? "0"), true);
		if (_ignitionStone is not null)
		{
			_ignitionStone.ContainedIn = Parent;
		}
		if (_matchCord is null)
		{
			MatchLit = false;
		}
		if (_ignitionStone is null)
		{
			IgnitionStrikesRemaining = 0;
			WheelWound = false;
		}
    }

    public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
    {
        return new MusketGameItemComponent(this, newParent, temporary);
    }
    #endregion

    #region Saving
    protected override string SaveToXml()
    {
        return new XElement("Definition",
            new XElement("IsReadied", IsReadied),
            new XElement("LoadStage", LoadStage),
            new XElement("NeedsCleaning", NeedsCleaning),
            new XElement("IsJammed", IsJammed),
            new XElement("TapLoaded", TapLoaded),
			new XElement("MatchLit", MatchLit),
			new XElement("WheelWound", WheelWound),
			new XElement("IgnitionStrikesRemaining", IgnitionStrikesRemaining),
			new XElement("IsBlankLoad", IsBlankLoad),
			new XElement("IsEmplaced", IsEmplaced),
            new XElement("Wielded", PrimaryWieldedLocation?.Id ?? 0),
            new XElement("Bayonet", _bayonet?.Parent.Id ?? 0),
            new XElement("Sights", _sights?.Parent.Id ?? 0),
            new XElement("Ramrod", _ramrod?.Parent.Id ?? 0),
			new XElement("MatchCord", _matchCord?.Id ?? 0),
			new XElement("IgnitionStone", _ignitionStone?.Id ?? 0),
            new XElement("Magazine",
                from item in _magazineContents
                select new XElement("Item", item.Id)
            )
        ).ToString();
    }
    #endregion

    public override double ComponentWeight => AllContainedItems.Sum(x => x.Weight);

    public override double ComponentBuoyancy(double fluidDensity)
    {
		return AllContainedItems.Sum(x => x.Buoyancy(fluidDensity));
    }

    public override void Quit()
    {
        base.Quit();
        foreach (IGameItem item in MagazineContents)
        {
            item.Quit();
        }
        _bayonet?.Parent.Quit();
        _ramrod?.Parent.Quit();
        _sights?.Parent.Quit();
		_matchCord?.Quit();
		_ignitionStone?.Quit();
    }

    public override void Delete()
    {
        base.Delete();
        foreach (IGameItem item in MagazineContents.ToList())
        {
            item.ContainedIn = null;
            item.Delete();
        }
        _bayonet?.Parent.Delete();
        _ramrod?.Parent.Delete();
        _sights?.Parent.Delete();
		_matchCord?.Delete();
		_ignitionStone?.Delete();
    }

    public override void Login()
    {
        foreach (IGameItem item in MagazineContents)
        {
            item.Login();
        }

        _bayonet?.Parent.Login();
        _ramrod?.Parent.Login();
        _sights?.Parent.Login();
		_matchCord?.Login();
		_ignitionStone?.Login();
    }

    public override bool DescriptionDecorator(DescriptionType type)
    {
        return type == DescriptionType.Full || type == DescriptionType.Evaluate;
    }

    public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
        bool colour, PerceiveIgnoreFlags flags)
    {
        if (type == DescriptionType.Full)
        {
            StringBuilder sb = new();
            sb.AppendLine(description);
            sb.AppendLine();
            switch (LoadStage)
            {
                case 0:
                    sb.AppendLine($"It is not loaded.");
                    break;
                case 1:
                    sb.AppendLine($"It has been cleaned but not yet loaded with any shot or powder.");
                    break;
                case 2:
                    sb.AppendLine($"It has been loaded with powder but requires a musket ball to be ready to shoot.");
                    break;
                case 3:
                    sb.AppendLine($"It has been loaded with shot but requires tamping before it is ready to shoot.");
                    break;
                case 4:
                    sb.AppendLine($"It is fully loaded.");
                    break;
            }
            sb.AppendLine($"The firing hammer {(IsReadied ? "is" : "is not")} in a position to fire.");
            if (IsJammed)
            {
                sb.AppendLine($"It is jammed and needs to be unjammed before being used.");
            }
            return sb.ToString();
        }

        if (type == DescriptionType.Evaluate)
        {
            IMeleeWeapon mw = (IMeleeWeapon)this;
            return
                $@"This is a muzzle-loading firearm of type {WeaponType.Name.Colour(Telnet.Cyan)}.
It uses the {WeaponType.FireTrait.Name.Colour(Telnet.Green)} skill for firing and the {WeaponType.OperateTrait.Name.ColourValue()} skill for operation.
It takes musket balls of a bore equal or less than {$"{_prototype.BarrelBore.ToString("N2", voyeur)}in".ColourValue()}.
This is also a melee weapon of type {mw.WeaponType.Name.Colour(Telnet.Cyan)}.
It uses the {mw.WeaponType.AttackTrait.Name.Colour(Telnet.Green)} skill for attack and {(mw.WeaponType.ParryTrait == mw.WeaponType.AttackTrait ? "defense" : $"the {mw.WeaponType.ParryTrait.Name.Colour(Telnet.Green)} skill for defense")}.
It is classified as {WeaponType.Classification.Describe().Colour(Telnet.Green)}.";
        }

        return base.Decorate(voyeur, name, description, type, colour, flags);
    }

    #region Implementation of IWieldable

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

    /// <inheritdoc />
    public bool AlwaysRequiresTwoHandsToWield => WeaponType.AlwaysRequiresTwoHandsToWield;

    /// <inheritdoc />
    public bool CanWield(ICharacter actor)
    {
        return _prototype.CanWieldProg?.ExecuteBool(false, actor, Parent) ?? true;
    }

    /// <inheritdoc />
    public string WhyCannotWield(ICharacter actor)
    {
        return _prototype.WhyCannotWieldProg?.ExecuteString(actor, Parent) ?? "You can't wield that for an unknown reason.";
    }

    #endregion

    #region Implementation of IDamageSource

    /// <inheritdoc />
    public IDamage GetDamage(IPerceiver perceiverSource, OpposedOutcome opposedOutcome)
    {
        throw new NotImplementedException();
    }

    #endregion

    #region Implementation of IUseTrait

    /// <inheritdoc />
    public ITraitDefinition Trait => _prototype.RangedWeaponType.FireTrait;

    #endregion

    #region Implementation of IRangedWeapon

    /// <inheritdoc />
    public string FireVerbForEchoes => "fire|fires";

    /// <inheritdoc />
    public bool CanBeAimedAtSelf => true;

    /// <inheritdoc />
    IWeaponType IMeleeWeapon.WeaponType =>
        _bayonet?.Parent.GetItemType<IMeleeWeapon>()?.WeaponType ?? _prototype.MeleeWeaponType;

    /// <inheritdoc />
    public WeaponClassification Classification => _prototype.RangedWeaponType.Classification;

    /// <inheritdoc />
    public IRangedWeaponType WeaponType => _prototype.RangedWeaponType;

    /// <inheritdoc />
    public bool ReadyToFire => IsLoaded && IsReadied;

    /// <inheritdoc />
    /// 0 - not loaded, 1 - clean, 2 - powder, 3 - musketball, 4 - ramrod
    public int LoadStage { get; set; }

    /// <inheritdoc />
    public bool IsLoaded => LoadStage >= 4;

    /// <inheritdoc />
    public bool IsReadied { get; set; }

    private bool _isJammed;

    public bool IsJammed
    {
        get => _isJammed;
        set
        {
            _isJammed = value;
            Changed = true;
        }
    }

    /// <inheritdoc />
    public bool CanUnjam(ICharacter actor)
    {
        if (!IsJammed)
        {
            return false;
        }

		IInventoryPlan plan = _prototype.UnjamTemplate.CreatePlan(actor);
        if (plan.PlanIsFeasible() != InventoryPlanFeasibility.Feasible)
        {
            return false;
        }

        return true;
    }

    /// <inheritdoc />
    public string WhyCannotUnjam(ICharacter actor)
    {
        if (!IsJammed)
        {
            return $"You cannot unjam {Parent.HowSeen(actor)} because it is not jammed.";
        }

		IInventoryPlan plan = _prototype.UnjamTemplate.CreatePlan(actor);
        switch (plan.PlanIsFeasible())
        {
            case InventoryPlanFeasibility.NotFeasibleMissingItems:
                return $"You cannot unjam {Parent.HowSeen(actor)} because you do not have a suitable tool.";
            case InventoryPlanFeasibility.NotFeasibleNotEnoughHands:
                return $"You cannot unjam {Parent.HowSeen(actor)} because you don't have enough free, working {actor.Body.WielderDescriptionPlural}.";
            case InventoryPlanFeasibility.NotFeasibleNotEnoughWielders:
                return $"You cannot unjam {Parent.HowSeen(actor)} because you don't have enough free, working {actor.Body.WielderDescriptionPlural}.";
        }

        throw new ApplicationException("Unknown WhyCannotUnjam reason in MusketGameItemComponent.WhyCannotUnjam");
    }

    /// <inheritdoc />
    public bool Unjam(ICharacter actor)
    {
        if (!CanUnjam(actor))
        {
            actor.OutputHandler.Send(WhyCannotUnjam(actor));
            return false;
        }

		IInventoryPlan plan = _prototype.UnjamTemplate.CreatePlan(actor);
        IEnumerable<InventoryPlanActionResult> results = plan.ExecuteWholePlan();
        IGameItem ramrod = results.FirstOrDefault(x => x.OriginalReference?.ToString() == "ramrod").PrimaryTarget;
		var effect = new UnjammingGun(actor, this, ramrod,
			plan.AssociatedEffects.FirstOrDefault(x => x.TargetItem == ramrod).DesiredState)
		{
			OnStopAction = _ => plan.FinalisePlanWithExemptions([Parent])
		};
		actor.AddEffect(effect, UnjammingGun.EffectDuration(actor, this, ramrod));
        return true;
    }

    public string StartUnjamEmote => _prototype.StartUnjamEmote;
    public string FinishUnjamEmote => _prototype.FinishUnjamEmote;
    public string FailUnjamEmote => _prototype.FailUnjamEmote;

    public bool NeedsCleaning { get; set; }
    public bool TapLoaded { get; set; }
	public bool IsEmplaced { get; private set; }

	public bool Emplace(ICharacter actor, out string reason)
	{
		if (!_prototype.RequiresRest)
		{
			reason = $"{Parent.HowSeen(actor, true)} does not require a rest or emplacement.";
			return false;
		}
		if (Parent.InInventoryOf is not null || Parent.Location != actor.Location)
		{
			reason = "You must place that musket in the room before emplacing it on a rest.";
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
			reason = $"{Parent.HowSeen(actor, true)} is not emplaced on a rest.";
			return false;
		}
		if (IsReadied || IsLoaded)
		{
			reason = "You must unready and unload that musket before removing it from its rest.";
			return false;
		}
		IsEmplaced = false;
		Changed = true;
		reason = string.Empty;
		return true;
	}
	public bool MatchLit { get; private set; }
	public bool WheelWound { get; private set; }
	public int IgnitionStrikesRemaining { get; private set; }
	public bool IsBlankLoad { get; private set; }

    private readonly List<IGameItem> _magazineContents = new();
	private IGameItem _matchCord;
	private IGameItem _ignitionStone;

    /// <inheritdoc />
    public IEnumerable<IGameItem> MagazineContents => _magazineContents;

    public IEnumerable<IGameItem> AllContainedItems => MagazineContents
		.Concat(_matchCord is null ? [] : new[] { _matchCord })
		.Concat(_ignitionStone is null ? [] : new[] { _ignitionStone });

    /// <inheritdoc />
    public Difficulty AimDifficulty => WeaponType.BaseAimDifficulty;

    /// <inheritdoc />
    public Difficulty BaseBlockDifficulty => Difficulty.Insane;

    /// <inheritdoc />
    public Difficulty BaseDodgeDifficulty => Difficulty.Impossible;

    /// <inheritdoc />
    public bool CanLoad(ICharacter loader, bool ignoreEmpty = false, LoadMode mode = LoadMode.Normal)
    {
		if (IsJammed)
		{
			return false;
		}
		if (!BlackPowderWeaponEnvironment.CanHandlePowder(loader))
		{
			return false;
		}

        switch (LoadStage)
        {
            case 0:
				if (mode.In(LoadMode.NoClean, LoadMode.TapNoClean) || !NeedsCleaning)
				{
					goto case 1;
				}
                switch (mode)
                {
                    case LoadMode.Blank:
                    case LoadMode.Normal:
                    case LoadMode.Tap:
                        return _prototype.LoadTemplateClean.CreatePlan(loader).PlanIsFeasible() == InventoryPlanFeasibility.Feasible;
                }

                goto case 1;
            case 1:
                // Try Cartridges first
				if (mode != LoadMode.Blank &&
					_prototype.LoadTemplateLoadCartridge.CreatePlan(loader).PlanIsFeasible() == InventoryPlanFeasibility.Feasible)
                {
                    return true;
                }

				if (BlackPowderWeaponEnvironment.CanHandleExposedPowder(loader) &&
					_prototype.LoadTemplateLoadPowder.CreatePlan(loader).PlanIsFeasible() == InventoryPlanFeasibility.Feasible)
                {
                    return true;
                }

                return false;
            case 2:
				return (mode == LoadMode.Blank ? _prototype.LoadTemplateLoadWad : _prototype.LoadTemplateLoadBall)
					.CreatePlan(loader).PlanIsFeasible() == InventoryPlanFeasibility.Feasible;
            case 3:
                return _prototype.LoadTemplateLoadRamrod.CreatePlan(loader).PlanIsFeasible() == InventoryPlanFeasibility.Feasible;
            default:
                return false;
        }
    }

    /// <inheritdoc />
    public string WhyCannotLoad(ICharacter loader, bool ignoreEmpty = false, LoadMode mode = LoadMode.Normal)
    {
		if (IsJammed)
		{
			return $"You must unjam {Parent.HowSeen(loader)} before loading it.";
		}
		if (!BlackPowderWeaponEnvironment.CanHandlePowder(loader))
		{
			return "You cannot load a black-powder weapon while it is submerged.";
		}

        switch (LoadStage)
        {
            case 0:
				if (mode.In(LoadMode.NoClean, LoadMode.TapNoClean) || !NeedsCleaning)
				{
					goto case 1;
				}
                switch (mode)
                {
                    case LoadMode.Blank:
                    case LoadMode.Normal:
                    case LoadMode.Tap:
                        switch (_prototype.LoadTemplateClean.CreatePlan(loader).PlanIsFeasible())
                        {
                            case InventoryPlanFeasibility.NotFeasibleNotEnoughHands:
                            case InventoryPlanFeasibility.NotFeasibleNotEnoughWielders:
                                return $"You don't have enough {loader.Body.WielderDescriptionPlural} to carry out that action.";
                            case InventoryPlanFeasibility.NotFeasibleMissingItems:
                                return $"You don't have a ramrod that you can use to clean your weapon.";
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                }

                goto case 1;
            case 1:
                // Try Cartridges first
				if (mode != LoadMode.Blank)
                {
					switch (_prototype.LoadTemplateLoadCartridge.CreatePlan(loader).PlanIsFeasible())
					{
						case InventoryPlanFeasibility.NotFeasibleNotEnoughHands:
						case InventoryPlanFeasibility.NotFeasibleNotEnoughWielders:
							return $"You don't have enough {loader.Body.WielderDescriptionPlural} to carry out that action.";
						case InventoryPlanFeasibility.NotFeasibleMissingItems:
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
                }
				if (!BlackPowderWeaponEnvironment.CanHandleExposedPowder(loader))
				{
					return "The precipitation is too heavy to pour an exposed loose-powder charge safely.";
				}

                switch (_prototype.LoadTemplateLoadPowder.CreatePlan(loader).PlanIsFeasible())
                {
                    case InventoryPlanFeasibility.NotFeasibleNotEnoughHands:
                    case InventoryPlanFeasibility.NotFeasibleNotEnoughWielders:
                        return $"You don't have enough {loader.Body.WielderDescriptionPlural} to carry out that action.";
                    case InventoryPlanFeasibility.NotFeasibleMissingItems:
                        return $"You don't have either gunpowder or a cartridge that you could use to load your weapon.";
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            case 2:
				switch ((mode == LoadMode.Blank ? _prototype.LoadTemplateLoadWad : _prototype.LoadTemplateLoadBall)
				        .CreatePlan(loader).PlanIsFeasible())
                {
                    case InventoryPlanFeasibility.NotFeasibleNotEnoughHands:
                    case InventoryPlanFeasibility.NotFeasibleNotEnoughWielders:
                        return $"You don't have enough {loader.Body.WielderDescriptionPlural} to carry out that action.";
                    case InventoryPlanFeasibility.NotFeasibleMissingItems:
						return mode == LoadMode.Blank
							? "You don't have tagged musket wadding for the blank charge."
							: "You need both a compatible musket ball and tagged musket wadding.";
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            case 3:
                switch (_prototype.LoadTemplateLoadRamrod.CreatePlan(loader).PlanIsFeasible())
                {
                    case InventoryPlanFeasibility.NotFeasibleNotEnoughHands:
                    case InventoryPlanFeasibility.NotFeasibleNotEnoughWielders:
                        return $"You don't have enough {loader.Body.WielderDescriptionPlural} to carry out that action.";
                    case InventoryPlanFeasibility.NotFeasibleMissingItems:
                        return $"You don't have a ramrod that you can use to tamp the ammunition into your weapon.";
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            default:
				return IsLoaded
					? $"{Parent.HowSeen(loader, true)} is already fully loaded."
					: $"{Parent.HowSeen(loader, true)} cannot be loaded from its current state.";
        }
    }

    /// <inheritdoc />
    public void Load(ICharacter loader, bool ignoreEmpty = false, LoadMode mode = LoadMode.Normal)
    {
        void AddEffect()
        {
            ICheck check = Gameworld.GetCheck(CheckType.LoadMusket);
            Difficulty difficulty = Difficulty.Normal;
            if (loader.Combat is not null)
            {
                difficulty = difficulty.StageUp(2);
            }

            if (loader.IsEngagedInMelee)
            {
                difficulty = difficulty.StageUp(2);
            }

            CheckOutcome result = check.Check(loader, difficulty, Parent, this);
            loader.AddEffect(new LoadingMusket(loader, this, mode), TimeSpan.FromSeconds(
                LoadStage switch
                {
                    1 => 10.0 + (result.Outcome.CheckDegrees() * -2.0),
                    2 => 10.0 + (result.Outcome.CheckDegrees() * -2.0),
                    3 => mode.In(LoadMode.Tap, LoadMode.TapNoClean) ?
                        4.0 + (result.Outcome.CheckDegrees() * -1.0) :
                        10.0 + (result.Outcome.CheckDegrees() * -1.5),
                    _ => 1.0
                }
            ));
        }

		if (!CanLoad(loader, ignoreEmpty, mode))
        {
			loader.Send(WhyCannotLoad(loader, ignoreEmpty, mode));
            return;
        }

        IInventoryPlan plan;
        IEnumerable<InventoryPlanActionResult> results;
        IGameItem ramrod;
        switch (LoadStage)
        {
            case 0:
				if (!NeedsCleaning || mode.In(LoadMode.NoClean, LoadMode.TapNoClean))
                {
					LoadStage = 1;
                    goto case 1;
                }

                switch (mode)
                {
                    case LoadMode.Blank:
                    case LoadMode.Normal:
                    case LoadMode.Tap:
                        plan = _prototype.LoadTemplateClean.CreatePlan(loader);
                        results = plan.ExecuteWholePlan();
                        ramrod = results.FirstOrDefault(x => x.OriginalReference?.ToString() == "ramrod").PrimaryTarget;
                        loader.OutputHandler.Handle(new EmoteOutput(new Emote(_prototype.LoadEmoteClean, loader, loader, Parent, ramrod), flags: OutputFlags.InnerWrap));
                        plan.FinalisePlanWithExemptions([Parent]);
                        LoadStage = 1;
                        NeedsCleaning = false;
                        Changed = true;
                        AddEffect();
                        return;
                }

                goto case 1;
            case 1:
                // Try Cartridges first
                plan = _prototype.LoadTemplateLoadCartridge.CreatePlan(loader);
				if (mode != LoadMode.Blank && plan.PlanIsFeasible() == InventoryPlanFeasibility.Feasible)
                {
                    results = plan.ExecuteWholePlan();
					IGameItem cartridgeSource = results.First(x => x.OriginalReference?.ToString() == "cartridge").PrimaryTarget;
					IGameItem cartridge = TakeOnePhysicalItem(loader, cartridgeSource);
                    loader.OutputHandler.Handle(new EmoteOutput(new Emote(_prototype.LoadEmoteCartridge, loader, loader, Parent, cartridge), flags: OutputFlags.InnerWrap));
					ContainLoadedItem(cartridge);
                    plan.FinalisePlanWithExemptions([cartridge, Parent]);
                    LoadStage = 3;
					IsBlankLoad = false;
                    Changed = true;
                    AddEffect();
                    return;
                }

                plan = _prototype.LoadTemplateLoadPowder.CreatePlan(loader);
				results = plan.ExecuteWholePlan();
				IGameItem powderSource = results.First(x => x.OriginalReference?.ToString() == "gunpowder").PrimaryTarget;
				IGameItem powder = powderSource.GetByWeight(loader.Body, _prototype.PowderVolumePerShot);
				ContainLoadedItem(powder);
                loader.OutputHandler.Handle(new EmoteOutput(new Emote(_prototype.LoadEmotePowder, loader, loader, Parent, powder), flags: OutputFlags.InnerWrap));
				plan.FinalisePlanWithExemptions([powder, Parent]);
                LoadStage = 2;
				IsBlankLoad = mode == LoadMode.Blank;
                Changed = true;
                AddEffect();
                return;
            case 2:
				plan = (mode == LoadMode.Blank ? _prototype.LoadTemplateLoadWad : _prototype.LoadTemplateLoadBall).CreatePlan(loader);
                results = plan.ExecuteWholePlan();
				var wadSource = results.First(x => x.OriginalReference?.ToString() == "wad").PrimaryTarget;
				IGameItem wad = TakeOnePhysicalItem(loader, wadSource);
				ContainLoadedItem(wad);
				IGameItem ball = null;
				IGameItem ballSource = null;
				if (mode != LoadMode.Blank)
				{
					ballSource = results.First(x => x.OriginalReference?.ToString() == "ball").PrimaryTarget;
					ball = TakeOnePhysicalItem(loader, ballSource);
					ContainLoadedItem(ball);
				}
				var shotPerceivable = (IPerceivable)ball ?? new DummyPerceivable("the blank charge");
				loader.OutputHandler.Handle(new EmoteOutput(new Emote(_prototype.LoadEmoteBall, loader, loader,
					Parent, shotPerceivable, wad), flags: OutputFlags.InnerWrap));
				plan.FinalisePlanWithExemptions(ball is null ? [wad, Parent] : [ball, wad, Parent]);
				ReleaseSplitSourceFromHands(loader, wadSource, wad);
				ReleaseSplitSourceFromHands(loader, ballSource, ball);
                LoadStage = 3;
                Changed = true;
                AddEffect();
                return;
            case 3:
                if (mode.In(LoadMode.Tap, LoadMode.TapNoClean))
                {
                    plan = _prototype.LoadTemplateFinishLoading.CreatePlan(loader);
                    results = plan.ExecuteWholePlan();
                    loader.OutputHandler.Handle(new EmoteOutput(new Emote(_prototype.LoadEmoteTap, loader, loader, Parent), flags: OutputFlags.InnerWrap));
                    plan.FinalisePlanWithExemptions([Parent]);
                    LoadStage = 4;
                    TapLoaded = true;
                    Changed = true;
                    return;
                }
                plan = _prototype.LoadTemplateLoadRamrod.CreatePlan(loader);
                results = plan.ExecuteWholePlan();
                ramrod = results.FirstOrDefault(x => x.OriginalReference?.ToString() == "ramrod").PrimaryTarget;
                loader.OutputHandler.Handle(new EmoteOutput(new Emote(_prototype.LoadEmoteRamrod, loader, loader, Parent, ramrod), flags: OutputFlags.InnerWrap));
                plan.FinalisePlanWithExemptions([Parent]);
                plan = _prototype.LoadTemplateFinishLoading.CreatePlan(loader);
                results = plan.ExecuteWholePlan();
                plan.FinalisePlanNoRestore();
                LoadStage = 4;
                TapLoaded = false;
                Changed = true;
                return;
            default:
                return;
        }
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

	private static void ReleaseSplitSourceFromHands(ICharacter actor, IGameItem source, IGameItem installed)
	{
		if (source is null || installed is null || source == installed ||
		    !actor.Body.HeldOrWieldedItems.Contains(source))
		{
			return;
		}

		actor.Body.Drop(source, silent: true);
	}

	private void ContainLoadedItem(IGameItem item)
	{
		item.ContainedIn = Parent;
		_magazineContents.Add(item);
	}

    /// <inheritdoc />
    public bool CanReady(ICharacter readier)
    {
		if (IsReadied || !IsLoaded || !BlackPowderWeaponEnvironment.CanHandlePowder(readier))
		{
			return false;
		}
		if (_prototype.RequiresRest && !IsEmplaced)
		{
			return false;
		}
		if (_prototype.IgnitionFamily == MusketIgnitionFamily.Matchlock &&
			(!BlackPowderWeaponEnvironment.CanSustainOpenFlame(readier) ||
			 (_matchCord is null && !HasMatchCord(readier))))
		{
			return false;
		}
		if (_prototype.IgnitionFamily is not MusketIgnitionFamily.Matchlock && IgnitionStrikesRemaining <= 0)
		{
			return false;
		}
        if (WeaponType.RequiresFreeHandToReady &&
            readier.Body.FunctioningWieldingLocationsAvailableFor(Parent).Count() < 2)
        {
            return false;
        }

        return true;
    }

    /// <inheritdoc />
    public string WhyCannotReady(ICharacter readier)
    {
		if (IsReadied)
		{
			return $"{Parent.HowSeen(readier, true)} is already readied.";
		}
		if (!IsLoaded)
		{
			return $"{Parent.HowSeen(readier, true)} must be fully loaded before it can be readied.";
		}
		if (!BlackPowderWeaponEnvironment.CanHandlePowder(readier))
		{
			return "You cannot ready a black-powder weapon while it is submerged.";
		}
		if (_prototype.RequiresRest && !IsEmplaced)
		{
			return $"You must emplace {Parent.HowSeen(readier)} on a rest before readying it.";
		}
		if (_prototype.IgnitionFamily == MusketIgnitionFamily.Matchlock &&
			!BlackPowderWeaponEnvironment.CanSustainOpenFlame(readier))
		{
			return "A match cord cannot remain lit here; it needs a gaseous atmosphere and reasonably dry weather.";
		}
		if (_prototype.IgnitionFamily == MusketIgnitionFamily.Matchlock && _matchCord is null && !HasMatchCord(readier))
		{
			return $"{Parent.HowSeen(readier, true)} needs a length of match cord before it can be readied.";
		}
		if (_prototype.IgnitionFamily is not MusketIgnitionFamily.Matchlock && IgnitionStrikesRemaining <= 0)
		{
			return $"{Parent.HowSeen(readier, true)} needs a fresh {(_prototype.IgnitionFamily == MusketIgnitionFamily.Wheellock ? "pyrite" : "flint")} before it can be readied.";
		}
        if (WeaponType.RequiresFreeHandToReady &&
            readier.Body.FunctioningWieldingLocationsAvailableFor(Parent).Count() < 2)
        {
            return
                $"You need at least one free {readier.Body.WielderDescriptionSingular} to ready {Parent.HowSeen(readier)}.";
        }

        throw new ApplicationException("Unknown WhyCannotReady reason in ready MusketGameItemComponent.");
    }

    /// <inheritdoc />
    public bool Ready(ICharacter readier)
    {
        if (!CanReady(readier))
        {
            readier.Send(WhyCannotReady(readier));
            return false;
        }

		switch (_prototype.IgnitionFamily)
		{
			case MusketIgnitionFamily.Matchlock:
				if (_matchCord is null)
				{
					InstallMatchCord(readier);
				}
				MatchLit = true;
				break;
			case MusketIgnitionFamily.Wheellock:
				WheelWound = true;
				break;
		}
		readier.OutputHandler.Handle(new EmoteOutput(new Emote(_prototype.ReadyEmote, readier, readier, Parent),
			flags: OutputFlags.InnerWrap));
        IsReadied = true;
        Changed = true;
        return true;
    }

    /// <inheritdoc />
    public bool CanUnready(ICharacter readier)
    {
		return IsReadied;
    }

    /// <inheritdoc />
    public string WhyCannotUnready(ICharacter readier)
    {
		return $"{Parent.HowSeen(readier, true)} is not readied.";
    }

    /// <inheritdoc />
    public bool Unready(ICharacter readier)
    {
        if (!CanUnready(readier))
        {
            readier.Send(WhyCannotUnready(readier));
            return false;
        }

        readier.OutputHandler.Handle(new EmoteOutput(new Emote(_prototype.UnreadyEmote, readier, readier, Parent),
            flags: OutputFlags.InnerWrap));
        IsReadied = false;
		MatchLit = false;
		WheelWound = false;
        Changed = true;
        return true;
    }

    /// <inheritdoc />
    public bool CanUnload(ICharacter loader)
    {
        if (IsJammed)
        {
            return false;
        }
		if (IsReadied)
		{
			return false;
		}

        if (_magazineContents.Count == 0 && LoadStage <= 1)
        {
            return false;
        }

        return true;
    }

    /// <inheritdoc />
    public string WhyCannotUnload(ICharacter loader)
    {
        if (IsJammed)
        {
            return $"{Parent.HowSeen(loader, true)} is jammed and the jam must be cleared first.";
        }
		if (IsReadied)
		{
			return $"You must unready {Parent.HowSeen(loader)} before unloading it.";
		}

        if (_magazineContents.Count == 0 && LoadStage <= 1)
        {
            return $"{Parent.HowSeen(loader, true)} is already unloaded.";
        }

        throw new ApplicationException("Unknown reason in MusketGameItemComponent.WhyCannotUnload");
    }

    /// <inheritdoc />
    public IEnumerable<IGameItem> Unload(ICharacter loader)
    {
        if (!CanUnload(loader))
        {
            loader.Send(WhyCannotUnload(loader));
            return Enumerable.Empty<IGameItem>();
        }

        IGameItem removed = null;
        IPerceivable removedPerceivable = null;
        if (_magazineContents.Count > 1)
        {
            List<IGameItem> contents = _magazineContents.ToList();
            _magazineContents.Clear();
            foreach (IGameItem item in contents)
            {
                item.ContainedIn = null;
            }

            removed = PileGameItemComponentProto.CreateNewBundle(contents);
            removedPerceivable = removed;
        }
        else if (_magazineContents.Count == 1)
        {
            removed = _magazineContents[0];
            _magazineContents.Clear();
            removed.ContainedIn = null;
            removedPerceivable = removed;
        }
        else
        {
            removedPerceivable = new DummyPerceivable("nothing");
        }

        Changed = true;
        loader.OutputHandler.Handle(new EmoteOutput(new Emote(_prototype.UnloadEmote, loader, loader, Parent, removedPerceivable)));
        LoadStage = 0;
		IsBlankLoad = false;
		TapLoaded = false;
		IsReadied = false;
        if (removed is not null)
        {
            removed.InsertAtSource(loader);
            removed.RoomLayer = loader.RoomLayer;
            removed.SetPosition(PositionUndefined.Instance, PositionModifier.Behind, loader, null);
        }

        return removed is not null ? [removed] : [];
    }

    /// <inheritdoc />
    public bool CanFire(ICharacter actor, IPerceivable target)
    {
		return ReadyToFire &&
               !IsJammed &&
			   BlackPowderWeaponEnvironment.CanHandlePowder(actor) &&
			   (!_prototype.RequiresRest || IsEmplaced) &&
			   (_prototype.IgnitionFamily != MusketIgnitionFamily.Matchlock || MatchLit) &&
			   (_prototype.IgnitionFamily != MusketIgnitionFamily.Matchlock ||
				BlackPowderWeaponEnvironment.CanSustainOpenFlame(actor)) &&
			   (_prototype.IgnitionFamily != MusketIgnitionFamily.Wheellock || WheelWound) &&
			   (_prototype.IgnitionFamily == MusketIgnitionFamily.Matchlock || IgnitionStrikesRemaining > 0) &&
               _bayonet?.Parent.GetItemType<IBayonetAttachment>()?.BlocksFiring != true;
    }

    /// <inheritdoc />
    public string WhyCannotFire(ICharacter actor, IPerceivable target)
    {
        if (IsJammed)
        {
            return $"You cannot fire {Parent.HowSeen(actor)} because it is jammed.";
        }

        if (!IsLoaded)
        {
            return $"You cannot fire {Parent.HowSeen(actor)} because it is not loaded.";
        }

        if (!IsReadied)
        {
            return $"You cannot fire {Parent.HowSeen(actor)} because it has not been readied.";
        }

		if (!BlackPowderWeaponEnvironment.CanHandlePowder(actor))
		{
			return "You cannot fire a black-powder weapon while it is submerged.";
		}

		if (_prototype.RequiresRest && !IsEmplaced)
		{
			return $"You must emplace {Parent.HowSeen(actor)} on a rest before firing it.";
		}

        if (_bayonet?.Parent.GetItemType<IBayonetAttachment>()?.BlocksFiring == true)
        {
            return $"You cannot fire {Parent.HowSeen(actor)} while its plug bayonet is seated in the muzzle.";
        }

		if (_prototype.IgnitionFamily == MusketIgnitionFamily.Wheellock && !WheelWound)
		{
			return $"You must wind the wheel of {Parent.HowSeen(actor)} before firing it.";
		}

		if (_prototype.IgnitionFamily == MusketIgnitionFamily.Matchlock && !MatchLit)
		{
			return $"You must light the match of {Parent.HowSeen(actor)} before firing it.";
		}

		if (_prototype.IgnitionFamily == MusketIgnitionFamily.Matchlock &&
			!BlackPowderWeaponEnvironment.CanSustainOpenFlame(actor))
		{
			return $"The exposed match of {Parent.HowSeen(actor)} cannot burn in this atmosphere or precipitation.";
		}

		if (_prototype.IgnitionFamily is not MusketIgnitionFamily.Matchlock && IgnitionStrikesRemaining <= 0)
		{
			return $"{Parent.HowSeen(actor, true)} needs a fresh ignition stone before it can fire.";
		}
        throw new ApplicationException(
            "Unknown WhyCannotFire reason in MusketGameItemComponent.WhyCannotFire");
    }

    /// <inheritdoc />
    public void Fire(ICharacter actor, IPerceiver target, Outcome shotOutcome, Outcome coverOutcome, OpposedOutcome defenseOutcome, IBodypart bodypart, IEmoteOutput defenseEmote, IPerceiver originalTarget)
    {
		if (!CanFire(actor, target))
		{
			actor.Send(WhyCannotFire(actor, target));
			return;
		}

		if (_prototype.IgnitionFamily is not MusketIgnitionFamily.Matchlock)
		{
			IgnitionStrikesRemaining = Math.Max(0, IgnitionStrikesRemaining - 1);
			if (IgnitionStrikesRemaining == 0 && _ignitionStone is not null)
			{
				_ignitionStone.ContainedIn = null;
				_ignitionStone.Delete();
				_ignitionStone = null;
			}
		}
		if (_prototype.IgnitionFamily == MusketIgnitionFamily.Wheellock)
		{
			WheelWound = false;
		}
        IAmmunitionType type = null;
        IGameItemProto bulletProto = null;
        IGameItem ball = null;

        // Do we misfire?
        bool misfire = false;
        bool catastrophy = false;
        IMusketCartridge cartridge = _magazineContents.FirstOrDefault()?.GetItemType<IMusketCartridge>();
        bool wadused = false;
        IAmmo ammo = null;

        // If there isn't exactly what we expect in the magazine, we always misfire
		if (IsBlankLoad)
		{
			var powder = _magazineContents.FirstOrDefault(x =>
				x.GetItemType<ICommodity>() is { } commodity && commodity.Material == _prototype.GunpowderMaterial);
			var wad = _prototype.WadItemTag is { } wadTag
				? _magazineContents.FirstOrDefault(x => x.IsA(wadTag))
				: null;
			if (_magazineContents.Count != 2 || powder is null || wad is null)
			{
				misfire = true;
				Gameworld.DebugMessage("Musket blank misfired due to unexpected or missing physical barrel contents");
			}
			else
			{
				wadused = true;
			}
		}
		else switch (_magazineContents.Count)
        {
            case 1:
                if (cartridge is null)
                {
                    misfire = true;
                    Gameworld.DebugMessage("Musket misfired due to unexpected item in barrel (count 1)");
                }
                else
                {
                    type = cartridge.AmmoType;
                    bulletProto = cartridge.BulletProto;
                    ammo = cartridge;
                    wadused = cartridge.IncludesWad;
                }

                break;
            case 2:
            case 3:
				IGameItem powder = _magazineContents.FirstOrDefault(x => x.GetItemType<ICommodity>() is { } commodity && commodity.Material == _prototype.GunpowderMaterial);
                ball = _magazineContents.FirstOrDefault(x => x.IsItemType<MusketBallGameItemComponent>());
				IGameItem wad = _prototype.WadItemTag is { } wadTag
					? _magazineContents.FirstOrDefault(x => x.IsA(wadTag))
					: null;
                if (powder is null || ball is null || (wad is null && _magazineContents.Count == 3))
                {
                    misfire = true;
                    Gameworld.DebugMessage("Musket misfired due to unexpected or missing item in barrel (count 2 or 3)");
                }
                else
                {
                    MusketBallGameItemComponent ballComp = ball.GetItemType<MusketBallGameItemComponent>();
                    type = ballComp.AmmoType;
                    bulletProto = ballComp.BulletProto;
                    ammo = ballComp;
                }


                if (wad is not null)
                {
                    wadused = true;
                }
                break;
            default:
                misfire = true;
                Gameworld.DebugMessage("Musket misfired due to unexpected item in barrel (count >3)");
                break;
        }

        catastrophy = _magazineContents.Count(x =>
            x.IsItemType<MusketBallGameItemComponent>() ||
            x.IsItemType<IMusketCartridge>() ||
            x.IsItemType<CommodityGameItemComponent>()) > 2;

        // Otherwise use the expression
        if (!misfire)
        {
            ITraitExpression misExpression = _prototype.MisfireChance;
            misExpression.Formula.Parameters["operate"] = actor.TraitValue(WeaponType.OperateTrait);
            misExpression.Formula.Parameters["skipclean"] = NeedsCleaning ? 1.0 : 0.0;
            misExpression.Formula.Parameters["precipitation"] = actor.Location.CurrentWeather(actor) is not null ? (actor.Location.CurrentWeather(actor)?.Precipitation ?? PrecipitationLevel.Parched).PrecipitationIntensityForGunpowder() : 0.0;
            misExpression.Formula.Parameters["gunquality"] = (int)Parent.Quality;
            misExpression.Formula.Parameters["cartridgeused"] = _magazineContents.Count == 1 && cartridge is not null ? 1.0 : 0.0;
            misExpression.Formula.Parameters["cartridgequality"] = _magazineContents.Count == 1 && cartridge is not null ? (int)cartridge.Parent.Quality : (int)ItemQuality.Standard;
            misExpression.Formula.Parameters["condition"] = Parent.Condition;
            misExpression.Formula.Parameters["wadused"] = wadused ? 1.0 : 0.0;
            misExpression.Formula.Parameters["wetpowder"] = _magazineContents.Any(x => x.SurfaceLiquidState.IsWet) ? 1.0 : 0.0;
            misExpression.Formula.Parameters["taploaded"] = TapLoaded ? 1.0 : 0.0;
            double chance = misExpression.Evaluate(actor, WeaponType.FireTrait, TraitBonusContext.MusketMisfire);
            double roll = RandomUtilities.DoubleRandom(0.0, 1.0);
            Gameworld.DebugMessage($"Musket misfire chance #2{chance:P3}#0 rolled {roll:P3}");
            if (roll < chance)
            {
                misfire = true;
            }
        }

        // Handle all the unloading etc
        Changed = true;
		var wasBlank = IsBlankLoad;
		IsBlankLoad = false;
        LoadStage = 0;
        NeedsCleaning = true;
        IsReadied = false;

        // If we misfired, work out if it's a catastrophy
        if (misfire && catastrophy)
        {
            actor.OutputHandler.Handle(new EmoteOutput(new Emote(_prototype.FireEmoteCatastrophy, actor, actor, target ?? (IPerceivable)new DummyPerceivable("the air"), Parent), style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
            List<IGameItem> contents = _magazineContents.ToList();
            _magazineContents.Clear();
            foreach (IGameItem item in contents)
            {
                item.Delete();
            }

			if (BlackPowderWeaponEnvironment.CanPropagateSound(actor))
			{
				actor.Location.HandleAudioEcho("An explosion can be heard {0}.", AudioVolume.ExtremelyLoud, Parent,
					actor.RoomLayer, true, "explosion");
			}

            List<IDamage> damages = new();
            foreach (DamageType damageType in new[] { DamageType.Shrapnel, DamageType.Shockwave, DamageType.Burning })
            {
                double damageAmount = _prototype.CatastrophyDamageFormula.EvaluateDouble();
                damages.Add(new Damage
                {
                    DamageType = damageType,
                    DamageAmount = damageAmount,
                    PainAmount = damageAmount,
                    StunAmount = damageAmount
                });
            }

            ExplosiveDamage explosion = new(damages, 0.0, Parent.Size, Proximity.Immediate, false, null);
            List<IWound> wounds = new();
            wounds.AddRange(actor.Body.InventoryExploded(Parent, explosion));
            wounds.AddRange(Parent.PassiveSufferDamage(explosion, Proximity.Intimate, Facing.Front));
            wounds.ProcessPassiveWounds();
            UseCondition(new ItemConditionUseContext(ItemConditionUseKind.RangedFire, shotOutcome,
                (int)(defenseOutcome?.Degree ?? OpposedOutcomeDegree.None)));
            return;
        }

        // If we misfired, work out if it's a jam
        bool isJam = false;
        if (misfire)
        {
            ITraitExpression jamExpression = _prototype.JamChance;
            jamExpression.Formula.Parameters["operate"] = actor.TraitValue(WeaponType.OperateTrait);
            jamExpression.Formula.Parameters["skipclean"] = NeedsCleaning ? 1.0 : 0.0;
            jamExpression.Formula.Parameters["precipitation"] = actor.Location.CurrentWeather(actor) is not null ? (actor.Location.CurrentWeather(actor)?.Precipitation ?? PrecipitationLevel.Parched).PrecipitationIntensityForGunpowder() : 0.0;
            jamExpression.Formula.Parameters["gunquality"] = (int)Parent.Quality;
            jamExpression.Formula.Parameters["cartridgeused"] = _magazineContents.Count == 1 && cartridge is not null ? 1.0 : 0.0;
            jamExpression.Formula.Parameters["cartridgequality"] = _magazineContents.Count == 1 && cartridge is not null ? (int)cartridge.Parent.Quality : (int)ItemQuality.Standard;
            jamExpression.Formula.Parameters["condition"] = Parent.Condition;
            jamExpression.Formula.Parameters["wadused"] = wadused ? 1.0 : 0.0;
            jamExpression.Formula.Parameters["wetpowder"] = _magazineContents.Any(x => x.SurfaceLiquidState.IsWet) ? 1.0 : 0.0;
            jamExpression.Formula.Parameters["taploaded"] = TapLoaded ? 1.0 : 0.0;
            double chance = jamExpression.Evaluate(actor, WeaponType.FireTrait, TraitBonusContext.MusketMisfire);
            double roll = RandomUtilities.DoubleRandom(0.0, 1.0);
            Gameworld.DebugMessage($"Musket jam chance #2{chance:P3}#0 rolled {roll:P3}");
            if (roll < chance)
            {
                isJam = true;
            }
        }

        // Use the shared audio path so hooks observe one origin event and RouteCells retain
        // their coordinate-aware propagation.
		if (BlackPowderWeaponEnvironment.CanPropagateSound(actor))
		{
			actor.Location.HandleAudioEcho("A gun shot can be heard {0}.", AudioVolume.ExtremelyLoud, Parent,
				actor.RoomLayer, true, "gunshot");
		}

        List<IGameItem> magContents = _magazineContents.ToList();
        _magazineContents.Clear();
        foreach (IGameItem item in magContents)
        {
            item.Delete();
        }

        if (isJam)
        {
            actor.OutputHandler.Handle(new EmoteOutput(new Emote(_prototype.FireEmoteJam, actor, actor, target ?? (IPerceivable)new DummyPerceivable("the air"), Parent), style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
            IsJammed = true;
            UseCondition(new ItemConditionUseContext(ItemConditionUseKind.RangedFire, shotOutcome,
                (int)(defenseOutcome?.Degree ?? OpposedOutcomeDegree.None)));
            return;
        }

        if (misfire)
        {
            actor.OutputHandler.Handle(new EmoteOutput(new Emote(_prototype.FireEmoteMisfire, actor, actor, target ?? (IPerceivable)new DummyPerceivable("the air"), Parent), style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
            UseCondition(new ItemConditionUseContext(ItemConditionUseKind.RangedFire, shotOutcome,
                (int)(defenseOutcome?.Degree ?? OpposedOutcomeDegree.None)));
            return;
        }

        actor.OutputHandler.Handle(new EmoteOutput(new Emote(_prototype.FireEmote, actor, actor, target ?? (IPerceivable)new DummyPerceivable("the air"), Parent), style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
		if (wasBlank)
		{
			UseCondition(new ItemConditionUseContext(ItemConditionUseKind.RangedFire, shotOutcome,
				(int)(defenseOutcome?.Degree ?? OpposedOutcomeDegree.None)));
			return;
		}
        IGameItem bullet = bulletProto?.CreateNew() ?? ball;
        if (bullet is not null)
        {
            bullet.HandleEvent(EventType.ItemFinishedLoading, bullet);
            bullet.Login();
            if (ball is not null)
            {
                WeaponPoisonDeliveryHelper.CopyPoisonCoating(ball, bullet);
            }
        }

		var projectileCount = bulletProto is null ? 1 : Math.Clamp(ammo.AmmoType.ProjectileCount, 1, 32);
		for (var projectileIndex = 0; projectileIndex < projectileCount; projectileIndex++)
		{
			var projectile = projectileIndex == 0
				? bullet
				: bulletProto.CreateNew();
			if (projectile is null)
			{
				continue;
			}

			projectile.Login();
			var projectileOutcome = FirearmMath.ProjectileOutcome(shotOutcome, null, 0, projectileIndex,
				ammo.AmmoType.SpreadPenalty, 1.0);
			var projectileBodypart = projectileIndex == 0
				? bodypart
				: (target as IHaveABody)?.Body?.RandomBodyPartGeometry(Orientation.Centre, Alignment.Front,
					Facing.Front) ?? bodypart;
			ammo.Fire(actor, target, projectileOutcome, coverOutcome, defenseOutcome, projectileBodypart, projectile,
				WeaponType, projectileIndex == 0 ? defenseEmote : null,
				new RangedFireContext(projectileIndex, projectileCount, ammo.AmmoType.ScatterType));
		}
        UseCondition(new ItemConditionUseContext(ItemConditionUseKind.RangedFire, shotOutcome,
            (int)(defenseOutcome?.Degree ?? OpposedOutcomeDegree.None)));
    }

	/// <summary>
	/// Installs one physical gunflint or pyrite piece and restores the lock's strike reserve.
	/// The item must be a stackable source of the correct material; a one-unit split remains in the lock.
	/// </summary>
	public bool TryInstallIgnitionStone(ICharacter actor, IGameItem stone, out string reason)
	{
		if (_prototype.IgnitionFamily == MusketIgnitionFamily.Matchlock)
		{
			reason = "Matchlocks use match cord rather than an ignition stone.";
			return false;
		}
		if (stone == Parent || stone.GetItemType<IStackable>() is null ||
		    _prototype.IgnitionSourceTag is null || !stone.IsA(_prototype.IgnitionSourceTag))
		{
			reason = "You must use a stackable physical item tagged as an ignition source.";
			return false;
		}

		var requiredMaterial = _prototype.IgnitionFamily == MusketIgnitionFamily.Wheellock ? "pyrite" : "flint";
		if (stone.Material?.Name.EqualTo(requiredMaterial) != true)
		{
			reason = $"{Parent.HowSeen(actor, true)} requires {requiredMaterial.ColourName()}.";
			return false;
		}

		var stack = stone.GetItemType<IStackable>()!;
		var installed = stack.Quantity > 1 ? stack.Split(1) : stone;
		if (installed == stone)
		{
			actor.Body.Take(stone);
		}
		else
		{
			installed.Login();
			installed.HandleEvent(EventType.ItemFinishedLoading, installed);
		}
		if (_ignitionStone is not null)
		{
			_ignitionStone.ContainedIn = null;
			_ignitionStone.InsertAtSource(actor);
		}
		installed.ContainedIn = Parent;
		_ignitionStone = installed;
		IgnitionStrikesRemaining = 100;
		WheelWound = false;
		Changed = true;
		reason = string.Empty;
		return true;
	}

	private bool HasMatchCord(ICharacter actor) =>
		_prototype.MatchCordTemplate.CreatePlan(actor).PlanIsFeasible() == InventoryPlanFeasibility.Feasible;

	private void InstallMatchCord(ICharacter actor)
	{
		var plan = _prototype.MatchCordTemplate.CreatePlan(actor);
		if (plan.PlanIsFeasible() != InventoryPlanFeasibility.Feasible)
		{
			return;
		}
		var results = plan.ExecuteWholePlan();
		var source = results.First(x => x.OriginalReference?.ToString() == "matchcord").PrimaryTarget;
		var installed = TakeOnePhysicalItem(actor, source);
		installed.ContainedIn = Parent;
		_matchCord = installed;
		actor.OutputHandler.Handle(new EmoteOutput(new Emote("@ fit|fits $1 into the match holder of $0 and light|lights its exposed end.",
			actor, Parent, installed)));
		plan.FinalisePlanWithExemptions([installed, Parent]);
		ReleaseSplitSourceFromHands(actor, source, installed);
	}

    #endregion

    #region Implementation of IBelt

    /// <inheritdoc />
    public SizeCategory MaximumSize => Parent.Size;

    /// <inheritdoc />
    public int MaximumNumberOfBeltedItems => 3;

    /// <inheritdoc />
    private IBeltable _bayonet;

    private IBeltable _ramrod;
    private IBeltable _sights;
    public IEnumerable<IBeltable> ConnectedItems
    {
        get
        {
            if (_bayonet is not null)
            {
                yield return _bayonet;
            }

            if (_ramrod is not null)
            {
                yield return _ramrod;
            }

            if (_sights is not null)
            {
                yield return _sights;
            }
        }
    }

    public void AddConnectedItem(IBeltable item)
    {
        item.ConnectedTo?.RemoveConnectedItem(item);
        if (item.Parent.IsItemType<IBayonetAttachment>())
        {
            _bayonet = item;
        }
		else if (_prototype.RamrodTag is { } ramrodTag && item.Parent.IsA(ramrodTag))
        {
            _ramrod = item;
        }
        else if (_prototype.SightTag is not null && item.Parent.IsA(_prototype.SightTag))
        {
            _sights = item;
        }
        else
        {
            return;
        }

        item.ConnectedTo = this;
        Changed = true;
    }

    public void RemoveConnectedItem(IBeltable item)
    {
        if (_bayonet == item)
        {
            _bayonet = null;
        }

        if (_ramrod == item)
        {
            _ramrod = null;
        }

        if (_sights == item)
        {
            _sights = null;
        }

        item.ConnectedTo = null;
        Changed = true;
    }

    public IBeltCanAttachBeltableResult CanAttachBeltable(IBeltable beltable)
    {
        if (beltable.Parent.Size > MaximumSize)
        {
            return IBeltCanAttachBeltableResult.FailureTooLarge;
        }

        var bayonet = beltable.Parent.GetItemType<IBayonetAttachment>();
        if (bayonet is not null)
        {
            if (!bayonet.FitsBore(_prototype.BarrelBore))
            {
                return IBeltCanAttachBeltableResult.NotValidType;
            }

            return _bayonet is null
                ? IBeltCanAttachBeltableResult.Success
                : IBeltCanAttachBeltableResult.FailureExceedMaximumNumber;
        }

		if (_prototype.RamrodTag is { } ramrodTag && beltable.Parent.IsA(ramrodTag))
        {
            if (_ramrod is not null)
            {
                return IBeltCanAttachBeltableResult.FailureExceedMaximumNumber;
            }

            return IBeltCanAttachBeltableResult.Success;
        }

        if (_prototype.SightTag is not null && beltable.Parent.IsA(_prototype.SightTag))
        {
            return _sights is null
                ? IBeltCanAttachBeltableResult.Success
                : IBeltCanAttachBeltableResult.FailureExceedMaximumNumber;
        }

        return IBeltCanAttachBeltableResult.NotValidType;
    }

    #endregion
}
