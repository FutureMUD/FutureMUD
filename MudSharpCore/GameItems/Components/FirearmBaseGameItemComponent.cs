using MudSharp.Accounts;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Combat;
using MudSharp.Construction;
using MudSharp.Events;
using MudSharp.Form.Audio;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.GameItems.Prototypes;
using MudSharp.Health;
using MudSharp.RPG.Checks;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace MudSharp.GameItems.Components;

public abstract class FirearmBaseGameItemComponent : GameItemComponent, IFirearm, ISwitchable, IMeleeWeapon,
    IConditionDegradingComponent
{
    private FirearmBaseGameItemComponentProto _prototype;
    private readonly Dictionary<string, IFirearmAttachment> _installedAttachments =
        new(StringComparer.InvariantCultureIgnoreCase);
    private FirearmFireModeType _currentFireMode = FirearmFireModeType.Single;
    public bool ConditionDegradesOnUse => _prototype.ConditionMaintenance.ConditionDegradesOnUse;
    public int ItemQualityStages => _prototype.ConditionMaintenance.QualityPenaltyStages(Parent);

    public void UseCondition(ItemConditionUseContext context)
    {
        _prototype.ConditionMaintenance.UseCondition(Parent, context);
    }

    protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
    {
        _prototype = (FirearmBaseGameItemComponentProto)newProto;
    }

    #region Constructors

    public FirearmBaseGameItemComponent(FirearmBaseGameItemComponentProto proto, IGameItem parent,
        bool temporary = false) : base(parent, proto, temporary)
    {
        _prototype = proto;
    }

    public FirearmBaseGameItemComponent(MudSharp.Models.GameItemComponent component,
        FirearmBaseGameItemComponentProto proto,
        IGameItem parent) : base(component, parent)
    {
        _prototype = proto;
    }

    public FirearmBaseGameItemComponent(FirearmBaseGameItemComponent rhs, IGameItem newParent, bool temporary = false) :
        base(
            rhs, newParent, temporary)
    {
        _prototype = rhs._prototype;
    }

    protected virtual void LoadFromXml(XElement root)
    {
        ChamberedRound = Gameworld.TryGetItem(long.Parse(root.Element("ChamberedRound").Value), true)
                                  ?.GetItemType<IAmmo>();
        PrimaryWieldedLocation = Gameworld.BodypartPrototypes.Get(long.Parse(root.Element("Wielded")?.Value ?? "0")) as IWield;


        XElement element = root.Element("Safety");
        if (element != null)
        {
            Safety = element.Value == "true";
        }

        if (CycleType == FirearmCycleType.Manual &&
            _prototype.FireModes.Any(x => x.Type == FirearmFireModeType.Single))
        {
            _currentFireMode = FirearmFireModeType.Single;
        }
        else if (root.Element("FireMode")?.Value.TryParseEnum<FirearmFireModeType>(out var mode) == true &&
            _prototype.FireModes.Any(x => x.Type == mode))
        {
            _currentFireMode = mode;
        }
        else
        {
            _currentFireMode = _prototype.FireModes.First().Type;
        }

        foreach (var attachmentElement in root.Element("Attachments")?.Elements("Attachment") ?? [])
        {
            var slotName = attachmentElement.Attribute("slot")?.Value;
            if (string.IsNullOrWhiteSpace(slotName) ||
                !long.TryParse(attachmentElement.Value, out var itemId))
            {
                continue;
            }

            var attachment = Gameworld.TryGetItem(itemId, true)?.GetItemType<IFirearmAttachment>();
            if (attachment is null)
            {
                continue;
            }

            _installedAttachments[slotName] = attachment;
            attachment.InstalledIn = this;
            attachment.Parent.ContainedIn = Parent;
        }
    }

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
            default:
                return setting.TryParseEnum<FirearmFireModeType>(out var mode) &&
                       FireModes.Any(x => x.Type == mode) &&
                       CurrentFireMode.Type != mode &&
                       (CycleType == FirearmCycleType.SelfLoading || mode == FirearmFireModeType.Single);
        }
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

        if (setting.TryParseEnum<FirearmFireModeType>(out var mode))
        {
            if (CycleType == FirearmCycleType.Manual && mode != FirearmFireModeType.Single)
            {
                return $"{Parent.HowSeen(actor, true)} has a manual action and cannot use multi-round fire modes.";
            }

            if (FireModes.All(x => x.Type != mode))
            {
                return $"{Parent.HowSeen(actor, true)} does not support {mode.DescribeEnum()} fire.";
            }

            return $"{Parent.HowSeen(actor, true)} is already set to {mode.DescribeEnum()} fire.";
        }

        return
            $"That is not a valid option for switching in {Parent.HowSeen(actor)}. Valid options are safe, or unsafe.";
    }

    public bool Switch(ICharacter actor, string setting)
    {
        if (!CanSwitch(actor, setting))
        {
            actor.Send(WhyCannotSwitch(actor, setting));
            return false;
        }

        if (setting.EqualTo("fire") || setting.EqualTo("unsafe"))
        {
            Safety = false;
        }
        else
        {
            if (setting.EqualTo("safe") || setting.EqualTo("safety"))
            {
                Safety = true;
            }
            else
            {
                setting.TryParseEnum<FirearmFireModeType>(out var mode);
                _currentFireMode = mode;
            }
        }

        Changed = true;
        actor.OutputHandler.Handle(new EmoteOutput(new Emote(
            setting.TryParseEnum<FirearmFireModeType>(out var selectedMode)
                ? $"@ switch|switches $0 to {selectedMode.DescribeEnum()} fire."
                : $"@ switch|switches the safety on $0 {(Safety ? "on" : "off")}.",
            actor, Parent)));
        return true;
    }

    public IEnumerable<string> SwitchSettings =>
        new[] { "safe", "unsafe" }.Concat(FireModes
            .Where(x => CycleType == FirearmCycleType.SelfLoading || x.Type == FirearmFireModeType.Single)
            .Select(x => x.Type.DescribeEnum()));

    #endregion

    #region Implementation of IMeleeWeapon

    IWeaponType IMeleeWeapon.WeaponType =>
        InstalledAttachments.Values
            .Where(x => x.SlotType == FirearmAttachmentSlotType.Bayonet)
            .SelectNotNull(x => x.Parent.GetItemType<IMeleeWeapon>())
            .Select(x => x.WeaponType)
            .FirstOrDefault() ?? _prototype.MeleeWeaponType;

    #endregion

    #region IRangedWeapon Implementation
    public virtual string FireVerbForEchoes => "fire|fires";
    public virtual bool CanBeAimedAtSelf => true;
    public IRangedWeaponType WeaponType => _prototype.RangedWeaponType;

    public virtual bool ReadyToFire => ChamberedRound != null && !Safety;

    public int LoadStage => 0;

    /// <inheritdoc />
    public abstract bool IsLoaded { get; }

    /// <inheritdoc />
    public bool IsReadied => ChamberedRound != null;

    /// <inheritdoc />
    public abstract IEnumerable<IGameItem> MagazineContents { get; }

    /// <inheritdoc />
    public abstract IEnumerable<IGameItem> AllContainedItems { get; }

    public IAmmo ChamberedRound { get; set; }
    public string SpecificAmmoGrade => _prototype.RangedWeaponType.SpecificAmmunitionGrade;

    public Difficulty AimDifficulty =>
        WeaponType.BaseAimDifficulty.ApplyBonus(CombinedAttachmentModifiers.AimBonus);

    public Difficulty BaseBlockDifficulty
        => ChamberedRound?.AmmoType.DamageProfile.BaseBlockDifficulty ?? Difficulty.Automatic;

    public Difficulty BaseDodgeDifficulty
        => ChamberedRound?.AmmoType.DamageProfile.BaseDodgeDifficulty ?? Difficulty.Automatic;

    /// <inheritdoc />
    public abstract bool CanLoad(ICharacter loader, bool ignoreEmpty = false, LoadMode mode = LoadMode.Normal);

    /// <inheritdoc />
    public abstract string WhyCannotLoad(ICharacter loader, bool ignoreEmpty = false, LoadMode mode = LoadMode.Normal);

    /// <inheritdoc />
    public abstract void Load(ICharacter loader, bool ignoreEmpty = false, LoadMode mode = LoadMode.Normal);

    /// <inheritdoc />
    public bool CanReady(ICharacter readier)
    {
        if (WeaponType.RequiresFreeHandToReady &&
            readier.Body.FunctioningWieldingLocationsAvailableFor(Parent).Count() < 2)
        {
            return false;
        }

        return true;
    }

    public string WhyCannotReady(ICharacter readier)
    {
        if (WeaponType.RequiresFreeHandToReady &&
            readier.Body.FunctioningWieldingLocationsAvailableFor(Parent).Count() < 2)
        {
            return
                $"You need at least one free {readier.Body.WielderDescriptionSingular} to ready {Parent.HowSeen(readier)}.";
        }

        throw new ApplicationException("Unknown WhyCannotReady reason in ready BoltActionGameItemComponent.");
    }

    public bool Ready(ICharacter readier)
    {
        if (!CanReady(readier))
        {
            readier.Send(WhyCannotReady(readier));
            return false;
        }

        readier.OutputHandler.Handle(new EmoteOutput(new Emote(_prototype.ReadyEmote, readier, readier, Parent),
            flags: OutputFlags.InnerWrap));
        ChamberRound(readier);
        return true;
    }

    protected abstract void ChamberRound(ICharacter readier);

    /// <inheritdoc />
    public bool CanUnready(ICharacter readier)
    {
        return true;
    }

    public string WhyCannotUnready(ICharacter readier)
    {
        throw new ApplicationException($"Should always be able to unready a {GetType().FullName}.");
    }

    public virtual bool Unready(ICharacter readier)
    {
        if (!CanUnready(readier))
        {
            readier.Send(WhyCannotUnready(readier));
            return false;
        }

        if (ChamberedRound != null)
        {
            readier.OutputHandler.Handle(new EmoteOutput(new Emote(_prototype.UnreadyEmote, readier, readier, Parent,
                ChamberedRound.Parent)));
            ChamberedRound.Parent.ContainedIn = null;
            if (readier.Body.CanGet(ChamberedRound.Parent, 0))
            {
                readier.Body.Get(ChamberedRound.Parent, silent: true);
            }
            else
            {
                ChamberedRound.Parent.InsertAtSource(readier);
            }

            ChamberedRound = null;
        }
        else
        {
            readier.OutputHandler.Handle(new EmoteOutput(new Emote(_prototype.UnreadyEmoteNoChamberedRound, readier,
                readier, Parent)));
        }

        return true;
    }

    /// <inheritdoc />
    public abstract bool CanUnload(ICharacter loader);

    /// <inheritdoc />
    public abstract string WhyCannotUnload(ICharacter loader);

    /// <inheritdoc />
    public abstract IEnumerable<IGameItem> Unload(ICharacter loader);

    /// <inheritdoc />
    public abstract bool CanFire(ICharacter actor, IPerceivable target);

    /// <inheritdoc />
    public abstract string WhyCannotFire(ICharacter actor, IPerceivable target);

    /// <inheritdoc />
    public virtual void Fire(ICharacter actor, IPerceiver target, Outcome shotOutcome, Outcome coverOutcome, OpposedOutcome defenseOutcome, IBodypart bodypart, IEmoteOutput defenseEmote, IPerceiver originalTarget)
    {
        if (!ReadyToFire)
        {
            actor.OutputHandler.Handle(new EmoteOutput(
                new Emote(_prototype.FireEmoteNoChamberedRound, actor, actor,
                    target ?? (IPerceivable)new DummyPerceivable("the air"), Parent), style: OutputStyle.CombatMessage,
                flags: OutputFlags.InnerWrap));
            actor.HandleEvent(EventType.FireGunEmpty, actor, target, Parent);
            return;
        }

        var attachmentFireEmote = AttachmentSlots
            .Select(x => InstalledAttachments.GetValueOrDefault(x.Name))
            .SelectNotNull(x => x?.FireEmote)
            .FirstOrDefault();
        actor.OutputHandler.Handle(new EmoteOutput(
            new Emote(attachmentFireEmote ?? _prototype.FireEmote, actor, actor,
                target ?? (IPerceivable)new DummyPerceivable("the air"),
                Parent), style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));

        var configuredRounds = CycleType == FirearmCycleType.Manual
            ? 1
            : Math.Clamp(CurrentFireMode.RoundsPerTrigger, 1, FirearmFireMode.MaximumRoundsPerTrigger);
        var firedRounds = 0;
        var loudestShot = AudioVolume.Silent;
        var originalLocation = RouteSpatialService.Instance.GetEffectiveLocation(actor);
        while (ChamberedRound is not null && firedRounds < configuredRounds)
        {
            var ammo = ChamberedRound;
            ChamberedRound = null;
            var firstBullet = ammo.GetFiredItem;
            var usesSeparateProjectile = firstBullet is not null;
            var projectileCount = usesSeparateProjectile
                ? Math.Clamp(ammo.AmmoType.ProjectileCount, 1, 32)
                : 1;
            var shell = ammo.GetFiredWasteItem;
            for (var projectileIndex = 0; projectileIndex < projectileCount; projectileIndex++)
            {
                var bullet = projectileIndex == 0
                    ? firstBullet ?? ammo.Parent
                    : ammo.GetFiredItem;
                WeaponPoisonDeliveryHelper.CopyPoisonCoating(ammo.Parent, bullet);
                var projectileOutcome = FirearmMath.ProjectileOutcome(shotOutcome, CurrentFireMode, firedRounds,
                    projectileIndex, ammo.AmmoType.SpreadPenalty,
                    CombinedAttachmentModifiers.RecoilMultiplier);
                var projectileBodypart = projectileIndex == 0
                    ? bodypart
                    : (target as IHaveABody)?.Body?.RandomBodyPartGeometry(
                        Orientation.Centre, Alignment.Front, Facing.Front) ?? bodypart;
                ammo.Fire(actor, target, projectileOutcome, coverOutcome, defenseOutcome, projectileBodypart, bullet,
                    WeaponType, projectileIndex == 0 ? defenseEmote : null,
                    new RangedFireContext(projectileIndex, projectileCount, ammo.AmmoType.ScatterType,
                        CombinedAttachmentModifiers.DamageMultiplier));
                if (!bullet.Deleted &&
                    bullet.IsItemType<IImpactDetonator>() &&
                    bullet.LocationLevelPerceivable?.Location is not null)
                {
                    bullet.GetItemType<IDetonatable>()?.Detonate();
                }
            }

            if (usesSeparateProjectile)
            {
                ammo.Parent.Delete();
            }

            HandleShellCasingOnFire(actor, originalLocation, shell);
            loudestShot = (AudioVolume)Math.Clamp(
                Math.Max((int)loudestShot,
                    (int)ammo.AmmoType.Loudness + CombinedAttachmentModifiers.LoudnessOffset),
                (int)AudioVolume.Silent, (int)AudioVolume.DangerouslyLoud);
            firedRounds++;
            if (CycleType == FirearmCycleType.SelfLoading)
            {
                ChamberRound(actor);
            }
        }

        Changed = true;
        UseCondition(new ItemConditionUseContext(ItemConditionUseKind.RangedFire, shotOutcome,
            (int)(defenseOutcome?.Degree ?? OpposedOutcomeDegree.None)));
        if (loudestShot > AudioVolume.Silent)
        {
            actor.Location.HandleAudioEcho(Gameworld.GetStaticString("GunshotHeardEcho"), loudestShot, Parent,
                actor.RoomLayer, true, "gunshot");
        }
    }

    protected virtual void HandleShellCasingOnFire(ICharacter actor, SpatialLocation originalLocation, IGameItem shell)
    {
        if (shell != null)
        {
            originalLocation.Cell.Handle(new EmoteOutput(new Emote("@ tumble|tumbles to the ground.", shell), flags: OutputFlags.Insigificant));
			shell.InsertAtSpatialLocation(originalLocation);
        }
    }

    protected IWield _primaryWieldedLocation;

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
        return _prototype.CanWieldProg?.ExecuteBool(false, actor, Parent) ?? true;
    }

    /// <inheritdoc />
    public string WhyCannotWield(ICharacter actor)
    {
        return _prototype.WhyCannotWieldProg?.ExecuteString(actor, Parent) ?? "You can't wield that for an unknown reason.";
    }

    public ITraitDefinition Trait => WeaponType.FireTrait;

    WeaponClassification IRangedWeapon.Classification => _prototype.RangedWeaponType.Classification;
    WeaponClassification IMeleeWeapon.Classification => ((IMeleeWeapon)this).WeaponType.Classification;

    #endregion

    #region IFirearm Attachment and Fire Mode Implementation

    public IReadOnlyCollection<FirearmAttachmentSlot> AttachmentSlots => _prototype.AttachmentSlots;
    public IReadOnlyDictionary<string, IFirearmAttachment> InstalledAttachments => _installedAttachments;
    public IReadOnlyCollection<FirearmFireMode> FireModes => _prototype.FireModes;
    public FirearmFireMode CurrentFireMode =>
        (CycleType == FirearmCycleType.Manual
            ? FireModes.FirstOrDefault(x => x.Type == FirearmFireModeType.Single)
            : FireModes.FirstOrDefault(x => x.Type == _currentFireMode)) ??
        FireModes.First();
    public FirearmCycleType CycleType => _prototype.CycleType;
    public double EffectiveAccuracyBonus =>
        (ChamberedRound?.AmmoType.BaseAccuracy ?? 0.0) + CombinedAttachmentModifiers.AccuracyBonus;
    public double EffectiveStaminaToFire =>
        Math.Max(0.0,
            (WeaponType.StaminaToFire +
             CurrentFireMode.ExtraStaminaPerRound * Math.Max(0, CurrentFireMode.RoundsPerTrigger - 1)) *
            CombinedAttachmentModifiers.StaminaMultiplier);
    public double EffectiveFireDelay =>
        Math.Max(0.0,
            (WeaponType.FireCombatDelay +
             CurrentFireMode.ExtraDelayPerRound * Math.Max(0, CurrentFireMode.RoundsPerTrigger - 1)) *
            CombinedAttachmentModifiers.DelayMultiplier);
    public double EffectiveAimLoss =>
        Math.Max(0.0,
            WeaponType.AimBonusLostPerShot *
            Math.Max(1, CurrentFireMode.RoundsPerTrigger) *
            CombinedAttachmentModifiers.AimLossMultiplier);
    public int EffectiveRangeInRooms =>
        Math.Max(0, (int)Math.Floor(WeaponType.DefaultRangeInRooms * CombinedAttachmentModifiers.RangeMultiplier));

    public FirearmAttachmentModifiers CombinedAttachmentModifiers
    {
        get
        {
            return FirearmMath.CombineModifiers(InstalledAttachments.Values.Select(x => x.Modifiers));
        }
    }

    public bool SetFireMode(FirearmFireModeType mode)
    {
        if (FireModes.All(x => x.Type != mode) ||
            CycleType == FirearmCycleType.Manual && mode != FirearmFireModeType.Single)
        {
            return false;
        }

        _currentFireMode = mode;
        Changed = true;
        return true;
    }

    public bool CanAttach(IFirearmAttachment attachment, string slotName, out string whyNot)
    {
        if (attachment.InstalledIn is not null)
        {
            whyNot = "That attachment is already installed on a firearm.";
            return false;
        }

        var matchingSlots = AttachmentSlots
            .Where(attachment.Fits)
            .Where(x => !_installedAttachments.ContainsKey(x.Name))
            .ToList();
        if (!string.IsNullOrWhiteSpace(slotName))
        {
            matchingSlots = matchingSlots.Where(x => x.Name.EqualTo(slotName)).ToList();
        }

        if (matchingSlots.Count == 0)
        {
            whyNot = "There is no free, compatible attachment slot.";
            return false;
        }

        if (matchingSlots.Count > 1)
        {
            whyNot =
                $"More than one slot is compatible; specify one of {matchingSlots.Select(x => x.Name).ListToString()}.";
            return false;
        }

        whyNot = string.Empty;
        return true;
    }

    public bool Attach(IFirearmAttachment attachment, string slotName, out string whyNot)
    {
        if (!CanAttach(attachment, slotName, out whyNot))
        {
            return false;
        }

        var slot = AttachmentSlots
            .Where(attachment.Fits)
            .Where(x => !_installedAttachments.ContainsKey(x.Name))
            .Single(x => string.IsNullOrWhiteSpace(slotName) || x.Name.EqualTo(slotName));
        _installedAttachments[slot.Name] = attachment;
        attachment.InstalledIn = this;
        attachment.Parent.ContainedIn = Parent;
        attachment.Changed = true;
        Changed = true;
        return true;
    }

    public bool CanDetach(IFirearmAttachment attachment, out string whyNot)
    {
        if (_installedAttachments.Values.All(x => x != attachment))
        {
            whyNot = "That attachment is not installed on this firearm.";
            return false;
        }

        whyNot = string.Empty;
        return true;
    }

    public bool Detach(IFirearmAttachment attachment, out string whyNot)
    {
        if (!CanDetach(attachment, out whyNot))
        {
            return false;
        }

        var slot = _installedAttachments.First(x => x.Value == attachment).Key;
        _installedAttachments.Remove(slot);
        attachment.InstalledIn = null;
        attachment.Parent.ContainedIn = null;
        attachment.Changed = true;
        Changed = true;
        return true;
    }

    protected IEnumerable<XElement> SaveFirearmState()
    {
        yield return new XElement("FireMode", CurrentFireMode.Type);
        yield return new XElement("Attachments",
            _installedAttachments.Select(x => new XElement("Attachment",
                new XAttribute("slot", x.Key),
                x.Value.Parent.Id)));
    }

    protected double AttachedItemsWeight => InstalledAttachments.Values.Sum(x => x.Parent.Weight);
    protected double AttachedItemsBuoyancy(double fluidDensity) =>
        InstalledAttachments.Values.Sum(x => x.Parent.Buoyancy(fluidDensity));

    public override bool Take(IGameItem item)
    {
        var attachment = item.GetItemType<IFirearmAttachment>();
        if (attachment is null || InstalledAttachments.Values.All(x => x != attachment))
        {
            return false;
        }

        Detach(attachment, out _);
        return true;
    }

    public override void FinaliseLoad()
    {
        foreach (var attachment in InstalledAttachments.Values)
        {
            attachment.Parent.FinaliseLoadTimeTasks();
        }
    }

    public override bool HandleDieOrMorph(IGameItem newItem, ICell location)
    {
        if (!InstalledAttachments.Any())
        {
            return false;
        }

        var newHost = newItem?.GetItemType<IFirearmAttachmentHost>();
        foreach (var attachment in InstalledAttachments.Values.ToList())
        {
            var slotName = InstalledAttachments.First(x => x.Value == attachment).Key;
            Detach(attachment, out _);
            if (newHost is not null && newHost.Attach(attachment, slotName, out _))
            {
                continue;
            }

            if (location is not null)
            {
                InsertAtParentSpatialLocation(attachment.Parent, location);
            }
            else
            {
                attachment.Parent.Delete();
            }
        }

        return false;
    }

    public override bool AffectsLocationOnDestruction => true;
    public override int ComponentDieOrder => 1;

    public override void Quit()
    {
        base.Quit();
        foreach (var attachment in InstalledAttachments.Values)
        {
            attachment.Parent.Quit();
        }
    }

    public override void Login()
    {
        base.Login();
        foreach (var attachment in InstalledAttachments.Values)
        {
            attachment.Parent.Login();
        }
    }

    public override void Delete()
    {
        base.Delete();
        foreach (var attachment in InstalledAttachments.Values.ToList())
        {
            attachment.Parent.ContainedIn = null;
            attachment.Parent.Delete();
        }

        _installedAttachments.Clear();
    }

    #endregion
}
