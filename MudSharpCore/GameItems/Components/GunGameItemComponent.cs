using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Combat;
using MudSharp.Construction;
using MudSharp.Events;
using MudSharp.Form.Audio;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.GameItems.Prototypes;
using MudSharp.Health;
using MudSharp.RPG.Checks;

namespace MudSharp.GameItems.Components;

public class GunGameItemComponent : FirearmBaseGameItemComponent, IRangedWeapon, ISwitchable, IMeleeWeapon
{
    protected GunGameItemComponentProto _prototype;
    public override IGameItemComponentProto Prototype => _prototype;

    protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
    {
        _prototype = (GunGameItemComponentProto)newProto;
    }

    #region Constructors

    public GunGameItemComponent(GunGameItemComponentProto proto, IGameItem parent, bool temporary = false) : base(
        proto, parent, temporary)
    {
        _prototype = proto;
    }

    public GunGameItemComponent(MudSharp.Models.GameItemComponent component, GunGameItemComponentProto proto,
        IGameItem parent) : base(component, proto, parent)
    {
        _prototype = proto;
        _noSave = true;
        LoadFromXml(XElement.Parse(component.Definition));
        _noSave = false;
    }

    public GunGameItemComponent(GunGameItemComponent rhs, IGameItem newParent, bool temporary = false) : base(rhs,
        newParent, temporary)
    {
        _prototype = rhs._prototype;
		if (rhs.Magazine is not null)
		{
			var magazine = rhs.Magazine.Parent.DeepCopy(!temporary, true);
			magazine.ContainedIn = newParent;
			Magazine = magazine.GetItemType<IContainer>();
		}
    }

    protected override void LoadFromXml(XElement root)
    {
        base.LoadFromXml(root);
        Magazine = Gameworld.TryGetItem(long.Parse(root.Element("Magazine").Value), true)?.GetItemType<IContainer>();
    }

    public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
    {
        return new GunGameItemComponent(this, newParent, temporary);
    }

    #endregion

    #region Saving

    protected override string SaveToXml()
    {
        return new XElement("Definition",
            new XElement("Magazine", Magazine?.Parent.Id ?? 0),
            new XElement("ChamberedRound", ChamberedRound?.Parent.Id ?? 0),
            new XElement("Wielded", PrimaryWieldedLocation?.Id ?? 0),
            new XElement("Safety", Safety ? "true" : "false"),
            SaveFirearmState()
        ).ToString();
    }

    #endregion

    #region IRangedWeapon Implementation

    public IContainer Magazine { get; set; }

    public override IEnumerable<IGameItem> MagazineContents => Magazine?.Contents ?? [];
    public override IEnumerable<IGameItem> AllContainedItems => MagazineContents.Concat([ChamberedRound?.Parent, Magazine?.Parent]).SelectNotNull(x => x);

    public override bool CanLoad(ICharacter loader, bool ignoreEmpty = false, LoadMode mode = LoadMode.Normal)
    {
        using var orderedComponentExecution = MudSharp.NPC.AI.CommandExecutionScope.EnterBodyOperation(loader);
        if (!MudSharp.NPC.AI.CommandExecutionScope.TryContinue(loader)) return false;


        if (!ItemManipulationGuard.CanManipulate(loader, out var manipulationReason, Parent))
        {
            return false;
        }

        if (Magazine != null)
        {
            return false;
        }

        IInventoryPlan plan = ignoreEmpty
            ? _prototype.LoadTemplateIgnoreEmpty.CreatePlan(loader)
            : _prototype.LoadTemplate.CreatePlan(loader);
        if (plan.PlanIsFeasible() != InventoryPlanFeasibility.Feasible)
        {
            return false;
        }

        return true;
    }

    public override string WhyCannotLoad(ICharacter loader, bool ignoreEmpty = false, LoadMode mode = LoadMode.Normal)
    {
        using var orderedComponentExecution = MudSharp.NPC.AI.CommandExecutionScope.EnterBodyOperation(loader);
        if (!MudSharp.NPC.AI.CommandExecutionScope.TryContinue(loader)) return string.Empty;


        if (!ItemManipulationGuard.CanManipulate(loader, out var manipulationReason, Parent))
        {
            return manipulationReason;
        }

        if (Magazine != null)
        {
            return $"There is already a clip in the magazine of {Parent.HowSeen(loader)}, you should eject that first.";
        }

        IInventoryPlan plan = ignoreEmpty
            ? _prototype.LoadTemplateIgnoreEmpty.CreatePlan(loader)
            : _prototype.LoadTemplate.CreatePlan(loader);
        if (plan.PlanIsFeasible() != InventoryPlanFeasibility.Feasible)
        {
            switch (plan.PlanIsFeasible())
            {
                case InventoryPlanFeasibility.NotFeasibleNotEnoughHands:
                case InventoryPlanFeasibility.NotFeasibleNotEnoughWielders:
                    return $"You don't have enough {loader.Body.WielderDescriptionPlural} to carry out that action.";
                case InventoryPlanFeasibility.NotFeasibleMissingItems:
                    return $"You don't have a suitable magazine of ammunition to load {Parent.HowSeen(loader)}.";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        throw new ApplicationException("Unknown WhyCannotLoad reason in PistolGameItemComponent.WhyCannotLoad");
    }

    protected override void ChamberRound(ICharacter loader)
    {
        using var orderedComponentExecution = MudSharp.NPC.AI.CommandExecutionScope.EnterBodyOperation(loader);
        if (!MudSharp.NPC.AI.CommandExecutionScope.TryContinue(loader)) return;


        if (ChamberedRound != null)
        {
            loader.OutputHandler.Handle(new EmoteOutput(new Emote("$1 is ejected from $0 by the action.", loader,
                Parent, ChamberedRound.Parent)));
            ChamberedRound.Parent.RoomLayer = loader.RoomLayer;
            ChamberedRound.Parent.InsertAtSource(loader);
            ChamberedRound.Parent.ContainedIn = null;
        }

        IAmmo newRound = MagazineContents.SelectNotNull(x => x.GetItemType<IAmmo>()).FirstOrDefault(x =>
            x.AmmoType.SpecificType == SpecificAmmoGrade &&
            x.AmmoType.RangedWeaponTypes.Contains(RangedWeaponType.ModernFirearm));
        if (newRound != null)
        {
            ChamberedRound = Magazine.Take(null, newRound.Parent, 1)?.GetItemType<IAmmo>();
        }
        else
        {
            ChamberedRound = null;
            loader.HandleEvent(EventType.ReadyGunEmpty, loader, this);
        }

        Changed = true;
    }

    public override void Load(ICharacter loader, bool ignoreEmpty = false, LoadMode mode = LoadMode.Normal)
    {
        using var orderedComponentExecution = MudSharp.NPC.AI.CommandExecutionScope.EnterBodyOperation(loader);
        if (!MudSharp.NPC.AI.CommandExecutionScope.TryContinue(loader)) return;


        if (!ItemManipulationGuard.CanManipulate(loader, out var manipulationReason, Parent))
        {
            loader?.OutputHandler.Send(manipulationReason);
            return;
        }

        if (!CanLoad(loader))
        {
            loader.Send(WhyCannotLoad(loader));
            return;
        }

        IInventoryPlan plan = ignoreEmpty
            ? _prototype.LoadTemplateIgnoreEmpty.CreatePlan(loader)
            : _prototype.LoadTemplate.CreatePlan(loader);
        IEnumerable<InventoryPlanActionResult> results = plan.ExecuteWholePlan();
        if (!MudSharp.NPC.AI.CommandExecutionScope.TryContinue(loader)) return;

        IAmmoClip ammo = results.Where(x => (string)x.OriginalReference == "loaditem")
                          .SelectNotNull(x => x.PrimaryTarget.GetItemType<IAmmoClip>()).First();
        loader.OutputHandler.Handle(new EmoteOutput(
            new Emote(_prototype.LoadEmote, loader, loader, Parent, ammo.Parent), flags: OutputFlags.InnerWrap));
        if (!MudSharp.NPC.AI.CommandExecutionScope.TryContinue(loader)) return;
        loader.Body.Take(ammo.Parent);
        if (ammo.Parent.Deleted || ammo.Parent.Destroyed || ammo.Parent.InInventoryOf is not null || ammo.Parent.ContainedIn is not null || ammo.Parent.Location is not null) return;
        Magazine = ammo;
        ammo.Parent.ContainedIn = Parent;

        List<IGameItem> exemptions = new()
        { ammo.Parent };
        plan.FinalisePlanWithExemptions(exemptions);
        Changed = true;
    }

    public override bool CanUnload(ICharacter loader)
    {
        using var orderedComponentExecution = MudSharp.NPC.AI.CommandExecutionScope.EnterBodyOperation(loader);
        if (!MudSharp.NPC.AI.CommandExecutionScope.TryContinue(loader)) return false;


        if (!ItemManipulationGuard.CanManipulate(loader, out var manipulationReason, Parent))
        {
            return false;
        }

        return Magazine != null;
    }

    public override string WhyCannotUnload(ICharacter loader)
    {
        using var orderedComponentExecution = MudSharp.NPC.AI.CommandExecutionScope.EnterBodyOperation(loader);
        if (!MudSharp.NPC.AI.CommandExecutionScope.TryContinue(loader)) return string.Empty;


        if (!ItemManipulationGuard.CanManipulate(loader, out var manipulationReason, Parent))
        {
            return manipulationReason;
        }

        if (Magazine == null)
        {
            return $"{Parent.HowSeen(loader, true)} is already unloaded.";
        }

        throw new ApplicationException("Unknown reason in GunGameItemComponent.WhyCannotUnload");
    }

    public override IEnumerable<IGameItem> Unload(ICharacter loader)
    {
		using var execution = MudSharp.NPC.AI.CommandExecutionScope.EnterBodyOperation(loader);
		if (!MudSharp.NPC.AI.CommandExecutionScope.TryContinue(loader)) return [];
		var canUnload = CanUnload(loader);
		if (!MudSharp.NPC.AI.CommandExecutionScope.TryContinue(loader)) return [];
		if (!canUnload)
		{
			var reason = WhyCannotUnload(loader);
			if (MudSharp.NPC.AI.CommandExecutionScope.TryContinue(loader)) loader.Send(reason);
			return [];
		}
		var magazine = Magazine;
		if (magazine is null) return [];
		var item = magazine.Parent;
		var completion = ComponentUnloadCompletion.PrepareReceive(loader, item);
		if (completion is null || !ReferenceEquals(Magazine, magazine) ||
		    !ComponentUnloadCompletion.OwnedBy(item, Parent) || !MudSharp.NPC.AI.CommandExecutionScope.TryContinue(loader)) return [];
		loader.OutputHandler.Handle(new EmoteOutput(new Emote(_prototype.UnloadEmote, loader, loader, Parent, item)));
		if (!ReferenceEquals(Magazine, magazine) || !ComponentUnloadCompletion.Detach(loader, item, Parent,
			() => { if (ReferenceEquals(Magazine, magazine)) Magazine = null; },
			() => ReferenceEquals(Magazine, magazine))) return [];
		Changed = true;
		completion();
		return [item];
    }

    public override bool CanFire(ICharacter actor, IPerceivable target)
    {
        using var orderedComponentExecution = MudSharp.NPC.AI.CommandExecutionScope.EnterBodyOperation(actor);
        if (!MudSharp.NPC.AI.CommandExecutionScope.TryContinue(actor)) return false;


        if (!ItemManipulationGuard.CanManipulate(actor, out var manipulationReason, Parent))
        {
            return false;
        }

        return true;
    }

    public override string WhyCannotFire(ICharacter actor, IPerceivable target)
    {
        using var orderedComponentExecution = MudSharp.NPC.AI.CommandExecutionScope.EnterBodyOperation(actor);
        if (!MudSharp.NPC.AI.CommandExecutionScope.TryContinue(actor)) return string.Empty;


        if (!ItemManipulationGuard.CanManipulate(actor, out var manipulationReason, Parent))
        {
            return manipulationReason;
        }

        throw new ApplicationException("Guns should always be able to fire.");
    }

    public override bool IsLoaded => Magazine != null;

    #endregion

    #region IGameItemComponent Overrides

    public override double ComponentWeight =>
        MagazineContents.Sum(x => x.Weight) + (ChamberedRound?.Parent.Weight ?? 0.0) + AttachedItemsWeight;

    public override double ComponentBuoyancy(double fluidDensity)
    {
        return MagazineContents.Sum(x => x.Buoyancy(fluidDensity)) +
               (ChamberedRound?.Parent.Buoyancy(fluidDensity) ?? 0.0) +
               AttachedItemsBuoyancy(fluidDensity);
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
            sb.AppendLine(Magazine != null
                ? $"It has {Magazine.Parent.HowSeen(voyeur)} in the magazine."
                : "It does not currently have any clip in the magazine.");
            sb.AppendLine($"The safety is currently {(Safety ? "on" : "off")}.");
            sb.AppendLine($"It is set to {CurrentFireMode.Type.DescribeEnum()} fire.");
            sb.AppendLine(InstalledAttachments.Any()
                ? $"It has {InstalledAttachments.Select(x => $"{x.Value.Parent.HowSeen(voyeur)} in its {x.Key} slot").ListToString()}."
                : "It has no firearm attachments installed.");
            return sb.ToString();
        }

        if (type == DescriptionType.Evaluate)
        {
            IMeleeWeapon mw = (IMeleeWeapon)this;
            return
                $"This is a {CycleType.DescribeEnum(true)} modular firearm of type {WeaponType.Name.Colour(Telnet.Cyan)}.\nIt supports {FireModes.Select(x => x.Type.DescribeEnum().ColourName()).ListToString()} fire.\nIt uses the {WeaponType.FireTrait.Name.Colour(Telnet.Green)} skill for firing.\nIt takes ammunition of type {WeaponType.SpecificAmmunitionGrade.Colour(Telnet.Green)} and accepts clips of type {_prototype.ClipType.Colour(Telnet.Green)}.\n This is also a melee weapon of type {mw.WeaponType.Name.Colour(Telnet.Cyan)}.\nIt uses the {mw.WeaponType.AttackTrait.Name.Colour(Telnet.Green)} skill for attack and {(mw.WeaponType.ParryTrait == mw.WeaponType.AttackTrait ? "defense" : $"the {mw.WeaponType.ParryTrait.Name.Colour(Telnet.Green)} skill for defense")}.\nIt is classified as {WeaponType.Classification.Describe().Colour(Telnet.Green)}.";
        }

        return base.Decorate(voyeur, name, description, type, colour, flags);
    }

    public override void Quit()
    {
        base.Quit();
        ChamberedRound?.Parent.Quit();
        Magazine?.Parent.Quit();
    }

    public override void Delete()
    {
        base.Delete();
        ChamberedRound?.Parent.ContainedIn = null;
        ChamberedRound?.Parent.Delete();

        Magazine?.Parent.ContainedIn = null;
        Magazine?.Parent.Delete();
    }

    public override void Login()
    {
        ChamberedRound?.Parent.Login();
        Magazine?.Parent.Login();
    }

    #endregion
}
