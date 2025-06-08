using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Prototypes;
using MudSharp.Health;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;

namespace MudSharp.GameItems.Components;

public class RevolverGameItemComponent : GameItemComponent, IRevolver, ISwitchable, IMeleeWeapon
{
    protected RevolverGameItemComponentProto _prototype;
    public override IGameItemComponentProto Prototype => _prototype;

    protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
    {
        _prototype = (RevolverGameItemComponentProto)newProto;
    }

    private List<IGameItem?> _chambers = new();

    public RevolverGameItemComponent(RevolverGameItemComponentProto proto, IGameItem parent, bool temporary = false) : base(parent, proto, temporary)
    {
        _prototype = proto;
        _chambers = Enumerable.Repeat<IGameItem?>(null, _prototype.Chambers).ToList();
    }

    public RevolverGameItemComponent(MudSharp.Models.GameItemComponent component, RevolverGameItemComponentProto proto, IGameItem parent) : base(component, parent)
    {
        _prototype = proto;
        _noSave = true;
        LoadFromXml(XElement.Parse(component.Definition));
        _noSave = false;
    }

    public RevolverGameItemComponent(RevolverGameItemComponent rhs, IGameItem newParent, bool temporary = false) : base(rhs, newParent, temporary)
    {
        _prototype = rhs._prototype;
        _chambers = rhs._chambers.ToList();
        CurrentChamber = rhs.CurrentChamber;
        IsOpen = rhs.IsOpen;
        Safety = rhs.Safety;
    }

    private void LoadFromXml(XElement root)
    {
        CurrentChamber = int.Parse(root.Element("CurrentChamber").Value);
        IsOpen = bool.Parse(root.Element("IsOpen").Value);
        Safety = bool.Parse(root.Element("Safety").Value);
        _chambers = root.Element("Chambers").Elements("Item").Select(x =>
        {
            var item = Gameworld.TryGetItem(long.Parse(x.Value), true);
            return item;
        }).Cast<IGameItem?>().ToList();
        while (_chambers.Count < _prototype.Chambers)
        {
            _chambers.Add(null);
        }
    }

    public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
    {
        return new RevolverGameItemComponent(this, newParent, temporary);
    }

    protected override string SaveToXml()
    {
        return new XElement("Definition",
            new XElement("CurrentChamber", CurrentChamber),
            new XElement("IsOpen", IsOpen),
            new XElement("Safety", Safety),
            new XElement("Chambers", from item in _chambers select new XElement("Item", item?.Id ?? 0))
        ).ToString();
    }

    #region IRangedWeapon Implementation

    public string FireVerbForEchoes => "fire|fires";
    public bool CanBeAimedAtSelf => true;
    public IRangedWeaponType WeaponType => _prototype.RangedWeaponType;

    public bool ReadyToFire => !IsOpen && !Safety && _chambers[CurrentChamber] != null;

    public int LoadStage => 0;

    public IEnumerable<IGameItem> CylinderContents => _chambers.SelectNotNull(x => x);
    public IEnumerable<IGameItem> MagazineContents => CylinderContents;
    public IEnumerable<IGameItem> AllContainedItems => CylinderContents;

    public string SpecificAmmoGrade => _prototype.RangedWeaponType.SpecificAmmunitionGrade;

    public Difficulty AimDifficulty => WeaponType.BaseAimDifficulty;

    public Difficulty BaseBlockDifficulty => _chambers[CurrentChamber]?.GetItemType<IAmmo>()?.AmmoType.DamageProfile.BaseBlockDifficulty ?? Difficulty.Automatic;

    public Difficulty BaseDodgeDifficulty => _chambers[CurrentChamber]?.GetItemType<IAmmo>()?.AmmoType.DamageProfile.BaseDodgeDifficulty ?? Difficulty.Automatic;

    private IWield _primaryWieldedLocation;
    public IWield PrimaryWieldedLocation
    {
        get => _primaryWieldedLocation;
        set { _primaryWieldedLocation = value; Changed = true; }
    }

    public bool AlwaysRequiresTwoHandsToWield => WeaponType.AlwaysRequiresTwoHandsToWield;

    public bool CanWield(ICharacter actor) => _prototype.CanWieldProg?.ExecuteBool(false, actor, Parent) ?? true;
    public string WhyCannotWield(ICharacter actor) => _prototype.WhyCannotWieldProg?.ExecuteString(actor, Parent) ?? "You can't wield that for an unknown reason.";
    public ITraitDefinition Trait => WeaponType.FireTrait;
    WeaponClassification IRangedWeapon.Classification => _prototype.RangedWeaponType.Classification;
    WeaponClassification IMeleeWeapon.Classification => _prototype.MeleeWeaponType.Classification;

    public bool CanLoad(ICharacter loader, bool ignoreEmpty = false, LoadMode mode = LoadMode.Normal)
    {
        if (!IsOpen) return false;
        if (_chambers.All(x => x != null)) return false;
        var plan = _prototype.LoadTemplate.CreatePlan(loader);
        return plan.PlanIsFeasible() == InventoryPlanFeasibility.Feasible;
    }

    public string WhyCannotLoad(ICharacter loader, bool ignoreEmpty = false, LoadMode mode = LoadMode.Normal)
    {
        if (!IsOpen) return $"You must open {Parent.HowSeen(loader)} first.";
        if (_chambers.All(x => x != null)) return $"All of the chambers in {Parent.HowSeen(loader)} are already loaded.";
        var plan = _prototype.LoadTemplate.CreatePlan(loader);
        return plan.PlanIsFeasible() switch
        {
            InventoryPlanFeasibility.NotFeasibleNotEnoughHands => $"You don't have enough {loader.Body.WielderDescriptionPlural} to do that.",
            InventoryPlanFeasibility.NotFeasibleNotEnoughWielders => $"You don't have enough {loader.Body.WielderDescriptionPlural} to do that.",
            InventoryPlanFeasibility.NotFeasibleMissingItems => $"You don't have any suitable ammunition to load {Parent.HowSeen(loader)}.",
            _ => "You cannot do that."};
    }

    public bool CanLoadChamber(ICharacter loader, int chamber, bool ignoreEmpty = false, LoadMode mode = LoadMode.Normal)
    {
        if (!IsOpen) return false;
        if (chamber < 0 || chamber >= _prototype.Chambers) return false;
        if (_chambers[chamber] != null) return false;
        var plan = _prototype.LoadTemplate.CreatePlan(loader);
        return plan.PlanIsFeasible() == InventoryPlanFeasibility.Feasible;
    }

    public string WhyCannotLoadChamber(ICharacter loader, int chamber, bool ignoreEmpty = false, LoadMode mode = LoadMode.Normal)
    {
        if (!IsOpen) return $"You must open {Parent.HowSeen(loader)} first.";
        if (chamber < 0 || chamber >= _prototype.Chambers) return "That is not a valid chamber.";
        if (_chambers[chamber] != null) return $"Chamber {chamber + 1} already has a round loaded.";
        var plan = _prototype.LoadTemplate.CreatePlan(loader);
        return plan.PlanIsFeasible() switch
        {
            InventoryPlanFeasibility.NotFeasibleNotEnoughHands => $"You don't have enough {loader.Body.WielderDescriptionPlural} to do that.",
            InventoryPlanFeasibility.NotFeasibleNotEnoughWielders => $"You don't have enough {loader.Body.WielderDescriptionPlural} to do that.",
            InventoryPlanFeasibility.NotFeasibleMissingItems => $"You don't have any suitable ammunition to load {Parent.HowSeen(loader)}.",
            _ => "You cannot do that."};
    }

    public void Load(ICharacter loader, bool ignoreEmpty = false, LoadMode mode = LoadMode.Normal)
    {
        if (!CanLoad(loader, ignoreEmpty, mode))
        {
            loader.Send(WhyCannotLoad(loader, ignoreEmpty, mode));
            return;
        }

        var chamber = _chambers.IndexOf(null);
        LoadChamber(loader, chamber, ignoreEmpty, mode);
    }

    public void LoadChamber(ICharacter loader, int chamber, bool ignoreEmpty = false, LoadMode mode = LoadMode.Normal)
    {
        if (!CanLoadChamber(loader, chamber, ignoreEmpty, mode))
        {
            loader.Send(WhyCannotLoadChamber(loader, chamber, ignoreEmpty, mode));
            return;
        }
        var plan = _prototype.LoadTemplate.CreatePlan(loader);
        var results = plan.ExecuteWholePlan();
        var ammo = results.First(x => (string)x.OriginalReference == "loaditem").PrimaryTarget.GetItemType<IAmmo>();
        _chambers[chamber] = ammo.Parent;
        ammo.Parent.ContainedIn = Parent;
        loader.OutputHandler.Handle(new EmoteOutput(new Emote(_prototype.LoadEmote, loader, loader, Parent, ammo.Parent), flags: OutputFlags.InnerWrap));
        plan.FinalisePlanWithExemptions(new List<IGameItem> { ammo.Parent });
        Changed = true;
    }

    public bool CanUnload(ICharacter loader)
    {
        return IsOpen && _chambers.Any(x => x != null);
    }

    public string WhyCannotUnload(ICharacter loader)
    {
        if (!IsOpen) return $"You must open {Parent.HowSeen(loader)} first.";
        if (_chambers.All(x => x == null)) return $"{Parent.HowSeen(loader, true)} is already unloaded.";
        return "You cannot unload that.";
    }

    public IEnumerable<IGameItem> Unload(ICharacter loader)
    {
        if (!CanUnload(loader))
        {
            loader.Send(WhyCannotUnload(loader));
            return Enumerable.Empty<IGameItem>();
        }
        var results = new List<IGameItem>();
        for (var i = 0; i < _chambers.Count; i++)
        {
            var item = _chambers[i];
            if (item == null) continue;
            loader.OutputHandler.Handle(new EmoteOutput(new Emote(_prototype.UnloadEmote, loader, loader, Parent, item), flags: OutputFlags.InnerWrap));
            if (loader.Body.CanGet(item, 0))
            {
                loader.Body.Get(item, silent: true);
            }
            else
            {
                item.RoomLayer = loader.RoomLayer;
                loader.Location.Insert(item);
            }
            results.Add(item);
            _chambers[i] = null;
        }
        Changed = true;
        return results;
    }

    public bool CanFire(ICharacter actor, IPerceivable target) => true;
    public string WhyCannotFire(ICharacter actor, IPerceivable target) => string.Empty;

    public void Fire(ICharacter actor, IPerceiver target, Outcome shotOutcome, Outcome coverOutcome, OpposedOutcome defenseOutcome, IBodypart bodypart, IEmoteOutput defenseEmote, IPerceiver originalTarget)
    {
        if (!ReadyToFire)
        {
            actor.OutputHandler.Handle(new EmoteOutput(new Emote(_prototype.FireEmoteNoRound, actor, actor, target ?? (IPerceivable)new DummyPerceivable("the air"), Parent), style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
            actor.HandleEvent(EventType.FireGunEmpty, actor, target, Parent);
            return;
        }
        actor.OutputHandler.Handle(new EmoteOutput(new Emote(_prototype.FireEmote, actor, actor, target ?? (IPerceivable)new DummyPerceivable("the air"), Parent), style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
        var ammo = _chambers[CurrentChamber].GetItemType<IAmmo>();
        _chambers[CurrentChamber] = null;
        Changed = true;
        var bullet = ammo.GetFiredItem ?? ammo.Parent;
        if (bullet != ammo.Parent)
        {
            ammo.Parent.Delete();
        }
        var originalLocation = actor.Location;
        ammo.Fire(actor, target, shotOutcome, coverOutcome, defenseOutcome, bodypart, bullet, WeaponType, defenseEmote);
        CurrentChamber = (CurrentChamber + 1) % _prototype.Chambers;
    }

    public bool IsLoaded => _chambers.Any(x => x != null);
    public bool IsReadied => true;
    public bool CanReady(ICharacter readier) => true;
    public string WhyCannotReady(ICharacter readier) => string.Empty;
    public bool Ready(ICharacter readier) => true;
    public bool CanUnready(ICharacter readier) => true;
    public string WhyCannotUnready(ICharacter readier) => string.Empty;
    public bool Unready(ICharacter readier) => true;

    #endregion

    #region ISwitchable Implementation

    public bool Safety { get; set; }
    public bool CanSwitch(ICharacter actor, string setting)
    {
        switch (setting.ToLowerInvariant())
        {
            case "safe":
            case "safety":
                return !Safety;
            case "fire":
            case "unsafe":
                return Safety;
        }
        return false;
    }

    public string WhyCannotSwitch(ICharacter actor, string setting)
    {
        switch (setting.ToLowerInvariant())
        {
            case "safe":
            case "safety":
                return $"{Parent.HowSeen(actor, true)} already has its safety switched on.";
            case "fire":
            case "unsafe":
                return $"{Parent.HowSeen(actor, true)} is already in fire mode.";
        }
        return $"That is not a valid option for switching in {Parent.HowSeen(actor)}. Valid options are safe, or unsafe.";
    }

    public bool Switch(ICharacter actor, string setting)
    {
        if (!CanSwitch(actor, setting))
        {
            actor.Send(WhyCannotSwitch(actor, setting));
            return false;
        }
        Safety = setting.EqualTo("fire") || setting.EqualTo("unsafe") ? false : true;
        Changed = true;
        actor.OutputHandler.Handle(new EmoteOutput(new Emote($"@ switch|switches the safety on $0 {(Safety ? "on" : "off")}.", actor, Parent)));
        return true;
    }

    public IEnumerable<string> SwitchSettings => new[] { "safe", "unsafe" };

    #endregion

    #region IGameItemComponent Overrides

    public override double ComponentWeight => CylinderContents.Sum(x => x.Weight);
    public override double ComponentBuoyancy(double fluidDensity) => CylinderContents.Sum(x => x.Buoyancy(fluidDensity));
    public override bool DescriptionDecorator(DescriptionType type) => type == DescriptionType.Full || type == DescriptionType.Evaluate;

    public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type, bool colour, PerceiveIgnoreFlags flags)
    {
        if (type == DescriptionType.Full)
        {
            var sb = new StringBuilder();
            sb.AppendLine(description);
            sb.AppendLine();
            var rounds = CylinderContents.Select(x => x.HowSeen(voyeur)).ToList();
            sb.AppendLine(rounds.Any() ? $"It has {rounds.ListToString()} in the chambers." : "It does not currently have any ammunition loaded.");
            sb.AppendLine($"The safety is currently {(Safety ? "on" : "off")}.");
            sb.AppendLine(IsOpen ? "The cylinder is open." : "The cylinder is closed.");
            return sb.ToString();
        }
        if (type == DescriptionType.Evaluate)
        {
            var mw = (IMeleeWeapon)this;
            return $"This is a revolver firearm of type {WeaponType.Name.Colour(Telnet.Cyan)}.\nIt uses the {WeaponType.FireTrait.Name.Colour(Telnet.Green)} skill for firing.\nIt takes ammunition of type {WeaponType.SpecificAmmunitionGrade.Colour(Telnet.Green)}.\nThis is also a melee weapon of type {mw.WeaponType.Name.Colour(Telnet.Cyan)}.\nIt uses the {mw.WeaponType.AttackTrait.Name.Colour(Telnet.Green)} skill for attack and {(mw.WeaponType.ParryTrait == mw.WeaponType.AttackTrait ? "defense" : $"the {mw.WeaponType.ParryTrait.Name.Colour(Telnet.Green)} skill for defense")}.\nIt is classified as {WeaponType.Classification.Describe().Colour(Telnet.Green)}.";
        }
        return base.Decorate(voyeur, name, description, type, colour, flags);
    }

    public override void Quit()
    {
        base.Quit();
        foreach (var item in CylinderContents)
        {
            item.Quit();
        }
    }

    public override void Delete()
    {
        base.Delete();
        foreach (var item in CylinderContents.ToList())
        {
            item.ContainedIn = null;
            item.Delete();
        }
    }

    public override void Login()
    {
        foreach (var item in CylinderContents)
        {
            item.Login();
        }
    }

    #endregion

    #region IRevolver Implementation

    public int Chambers => _prototype.Chambers;
    public int CurrentChamber { get; private set; }

    public bool IsOpen { get; private set; }
    public bool CanOpen(IBody opener) => !IsOpen;
    public WhyCannotOpenReason WhyCannotOpen(IBody opener) => IsOpen ? WhyCannotOpenReason.AlreadyOpen : WhyCannotOpenReason.Unknown;
    public void Open() { IsOpen = true; Changed = true; OnOpen?.Invoke(this); }
    public bool CanClose(IBody closer) => IsOpen;
    public WhyCannotCloseReason WhyCannotClose(IBody closer) => !IsOpen ? WhyCannotCloseReason.AlreadyClosed : WhyCannotCloseReason.Unknown;
    public void Close() { IsOpen = false; Changed = true; OnClose?.Invoke(this); }
    public event OpenableEvent OnOpen;
    public event OpenableEvent OnClose;

    public void Spin(int positions)
    {
        CurrentChamber = (CurrentChamber + positions) % _prototype.Chambers;
        if (CurrentChamber < 0) CurrentChamber += _prototype.Chambers;
        Changed = true;
    }

    public void Spin()
    {
        CurrentChamber = RandomUtilities.Random(0, _prototype.Chambers - 1);
        Changed = true;
    }

    #endregion

    IWeaponType IMeleeWeapon.WeaponType => _prototype.MeleeWeaponType;
}
