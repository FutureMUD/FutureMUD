using ExpressionEngine;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MudSharp.Body;
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Body.Traits;
using MudSharp.Climate;
using MudSharp.Combat;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Database;
using MudSharp.Economy.Currency;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Events;
using MudSharp.Events.Hooks;
using MudSharp.Form.Characteristics;
using MudSharp.Form.Material;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using MudSharp.Framework.Scheduling;
using MudSharp.Framework.Units;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Prototypes;
using MudSharp.Health;
using MudSharp.Models;
using MudSharp.PerceptionEngine.Handlers;
using MudSharp.RPG.Checks;
using MudSharp.Magic;
using MudSharp.Planes;
using Org.BouncyCastle.Asn1.X509;
using System.Numerics;
using System.Text.RegularExpressions;
using CharacteristicValue = MudSharp.Form.Characteristics.CharacteristicValue;

#nullable enable annotations

namespace MudSharp.GameItems;

public partial class GameItem : PerceiverItem, IGameItem, IDisposable, IPostCharacterLoadFinalisable
{
    #region IGameItem Related Code

    protected readonly List<IGameItemComponent> _components = new();

	internal static IGameItem? ResolveSpatialHost(IEnumerable<IGameItemComponent> components, IGameItem? parent = null)
	{
		return components
		       .OfType<IProvideItemSpatialHost>()
		       .Select(x => x.SpatialHost)
		       .FirstOrDefault(x => x is not null && !ReferenceEquals(x, parent));
	}

	internal static IPerceivable? ResolveEffectSpatialHost(IEnumerable<IEffect> effects, IGameItem? parent = null)
	{
		return effects
		       .OfType<IProvideItemSpatialHostEffect>()
		       .Select(x => x.SpatialHost)
		       .FirstOrDefault(x => x is not null && !ReferenceEquals(x, parent));
	}

	private bool IsSpatiallyHosted => _components.Any(x => x is IProvideItemSpatialHost);
	private IGameItem? SpatialHost => ResolveSpatialHost(_components, this);
	private IProvideItemSpatialHostEffect? EffectSpatialHostProvider => Effects
		.OfType<IProvideItemSpatialHostEffect>()
		.FirstOrDefault(x => x.SpatialHost is not null && !ReferenceEquals(x.SpatialHost, this));
	private IPerceivable? EffectSpatialHost => EffectSpatialHostProvider?.SpatialHost;
	private IRoom? EffectSpatialHostLocation => EffectSpatialHost as IRoom ?? EffectSpatialHost?.Location;
	private bool HasEffectiveSpatialHost => IsSpatiallyHosted || EffectSpatialHost is not null;
	private double? PersistedRoutePositionMetres => HasEffectiveSpatialHost ? null : RoutePositionMetres;

    #endregion

    public override InitialisationPhase InitialisationPhase => InitialisationPhase.First;

    #region IDisposable Members

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    #endregion

    public bool Destroyed { get; set; }

    public bool CheckPrototypeForUpdate()
    {
		if (SpellCreationOrigin?.IsTemporary == true) return false;
		if (GetItemType<ICorpse>() is not null && Gameworld.SpellOwnedCorpseAnimations?.IsBorrowedCorpse(Id) == true) return false;
        if (Prototype.Status == RevisionStatus.Obsolete || Prototype.Status == RevisionStatus.Revised)
        {
            IGameItemProto newProto =
                Gameworld.ItemProtos.GetAll(Prototype.Id).FirstOrDefault(x => x.Status == RevisionStatus.Current);

            if (newProto == null)
            {
                return false;
            }

            Models.GameItem dbitem = new()
            {
                Id = Id,
                GameItemProtoRevision = newProto.RevisionNumber,
                Quality = (int)newProto.BaseItemQuality,
                ContainerId = ContainedIn?.Id,
                GameItemProtoId = Prototype.Id,
                EffectData = SaveEffects().ToString()
            };
            FMDB.Context.GameItems.Attach(dbitem);
            EntityEntry<Models.GameItem> entry = FMDB.Context.Entry(dbitem);
            entry.State = EntityState.Modified;
            entry.Property(x => x.GameItemProtoRevision).IsModified = true;
            entry.Property(x => x.Quality).IsModified = true;
            _quality = newProto.BaseItemQuality;

            // Create components that didn't previously exist
            foreach (
                IGameItemComponentProto component in
                newProto.Components.Where(x => Prototype.Components.All(y => y.Id != x.Id)).ToList())
            {
                IGameItemComponent newComp = component.CreateNew(this);
                _components.Add(newComp);
            }

            // Delete components that no longer exist
            foreach (
                IGameItemComponentProto removedComponent in Prototype.Components
                                                 .Where(x => newProto.Components.All(y => y.Id != x.Id))
                                                 .ToList())
            {
                IGameItemComponent myComponent = _components.FirstOrDefault(x => x.Prototype.Id == removedComponent.Id);
                if (myComponent != null)
                {
                    _components.Remove(myComponent);
                    myComponent.Delete();
                    Models.GameItemComponent dbcomp = FMDB.Context.GameItemComponents.Find(myComponent.Id);
                    if (dbcomp is not null)
                    {
                        dbitem.GameItemComponents.Remove(dbcomp);
                        FMDB.Context.GameItemComponents.Remove(dbcomp);
                    }
                }
            }

            // Update remaining components
            foreach (IGameItemComponent component in Components.ToList())
            {
                component.CheckPrototypeForUpdate();
            }

            Prototype = newProto;
            _overridingWoundBehaviourComponent = _components.OfType<IOverrideItemWoundBehaviour>().FirstOrDefault();
            return true;
        }

        bool result = false;
        foreach (IGameItemComponent component in Components)
        {
            if (Prototype.Components.All(x => x.Id != component.Prototype.Id))
            {
                _components.Remove(component);
                component.Delete();
                if (component.Id != 0)
                {
                    Models.GameItemComponent dbcomp = FMDB.Context.GameItemComponents.Find(component.Id); // TODO - possible crash here
                    if (dbcomp is not null)
                    {
                        FMDB.Context.GameItems.Find(Id)?.GameItemComponents.Remove(dbcomp);
                        FMDB.Context.GameItemComponents.Remove(dbcomp);
                    }
                }

                result = true;
            }
        }

        foreach (IGameItemComponentProto component in Prototype.Components)
        {
            if (Components.All(x => x.Prototype.Id != component.Id))
            {
                IGameItemComponent newComponent = component.CreateNew(this);
                _components.Add(newComponent);
                result = true;
            }
        }

        foreach (IGameItemComponent component in Components.ToList())
        {
            bool compResult = component.CheckPrototypeForUpdate();
            result = result || compResult;
        }

        return result;
    }

    /// <summary>
    ///     Indicates whether this game item can move to another room, whether in the inventory of someone who is moving, or
    ///     when being dragged
    /// </summary>
    /// <returns></returns>
    public bool PreventsMovement()
    {
        return _components.Any(x => x.PreventsMovement());
    }

    /// <summary>
    ///     Indicates why an item cannot be moved if it cannot be moved
    /// </summary>
    /// <param name="mover"></param>
    /// <returns></returns>
    public string WhyPreventsMovement(ICharacter mover)
    {
        return _components.First(x => x.PreventsMovement()).WhyPreventsMovement(mover);
    }

    /// <summary>
    ///     Called when an item has been forcefully moved (such as admin teleportation or spell)
    /// </summary>
    public void ForceMove()
    {
        foreach (IGameItemComponent component in _components)
        {
            component.ForceMove();
        }
    }

	public override bool TrySetRoutePosition(double? metres, out string error, bool noSave = false)
	{
		var previousPosition = RoutePositionMetres;
		if (!base.TrySetRoutePosition(metres, out error, noSave))
		{
			return false;
		}

		if (!Nullable.Equals(previousPosition, RoutePositionMetres))
		{
			// Longitudinal movement is real movement even though the containing room does not change.
			// Independent cables, hoses and other ordinary connectors must therefore revalidate and
			// disconnect just as they do for a room transition.
			ForceMove();
		}

		return true;
	}

    public override bool CanBePositionedAgainst(IPositionState state, PositionModifier modifier)
    {
        return _components.All(x => x.CanBePositionedAgainst(state, modifier));
    }

    public bool AllowReposition()
    {
        return IsItemType<IHoldable>() && !Components.Any(x => x.PreventsRepositioning());
    }

    public string WhyCannotReposition()
    {
        return IsItemType<IHoldable>()
            ? Components.First(x => x.PreventsRepositioning()).WhyPreventsRepositioning()
            : " is not something that can be picked up.";
    }

    public override bool CanHear(IPerceivable thing)
    {
        return false;
    }

    public override bool CanSense(IPerceivable thing, bool ignoreFuzzy = false)
    {
        return false;
    }

    public override bool CanSee(IPerceivable thing, PerceiveIgnoreFlags flags = PerceiveIgnoreFlags.None)
    {
        return false;
    }

    public override bool CanSmell(IPerceivable thing)
    {
        return false;
    }

    public override PerceptionTypes NaturalPerceptionTypes => PerceptionTypes.None;
    public override int LineFormatLength => int.MaxValue;
    public override int InnerLineFormatLength => int.MaxValue;

    public int Quantity => GetItemType<IStackable>()?.Quantity ?? 1;

    private FrameworkItemReference _ownerReference;
    private IFrameworkItem _owner;

    public IFrameworkItem Owner
    {
        get
        {
            if (_owner == null && _ownerReference != null)
            {
                _owner = _ownerReference.GetItem;
            }

            return _owner;
        }
    }

    public bool HasOwner => _ownerReference != null;

    public ItemOwnershipReference? OwnershipReference => _ownerReference is null
        ? null
        : new ItemOwnershipReference(_ownerReference.FrameworkItemType, _ownerReference.Id);

    public bool IsOwnedBy(IFrameworkItem owner)
    {
        if (owner is ICharacter character && _ownerReference?.FrameworkItemType == "Character")
        {
            return _ownerReference.Id == CharacterInstanceIdentityComparer.IdentityId(character);
        }

        return _ownerReference?.Equals(owner) == true;
    }

    public void SetOwner(IFrameworkItem owner)
    {
        var oldReference = OwnershipReference;
        _owner = owner;
        _ownerReference = owner == null
            ? null
            : new FrameworkItemReference(CharacterInstanceIdentityComparer.FrameworkItemId(owner), owner.FrameworkItemType, Gameworld);
        Changed = true;
        HandleEvent(EventType.ItemOwnershipChanged, this,
            oldReference?.FrameworkItemType ?? string.Empty, oldReference?.Id ?? 0L,
            OwnershipReference?.FrameworkItemType ?? string.Empty, OwnershipReference?.Id ?? 0L);
    }

    public void ClearOwner()
    {
        var oldReference = OwnershipReference;
        _owner = null;
        _ownerReference = null;
        Changed = true;
        HandleEvent(EventType.ItemOwnershipChanged, this,
            oldReference?.FrameworkItemType ?? string.Empty, oldReference?.Id ?? 0L,
            string.Empty, 0L);
    }

    public void CopyOwnerFrom(IGameItem source)
    {
		MudSharp.Magic.PsychometricRecorder.CopyHistory(source, this);
        var oldReference = OwnershipReference;
        if (source.OwnershipReference is not { } reference)
        {
            ClearOwner();
            return;
        }

        _owner = source.Owner;
        _ownerReference = new FrameworkItemReference(reference.Id, reference.FrameworkItemType, Gameworld);
        Changed = true;
        HandleEvent(EventType.ItemOwnershipChanged, this,
            oldReference?.FrameworkItemType ?? string.Empty, oldReference?.Id ?? 0L,
            reference.FrameworkItemType, reference.Id);
    }

    public bool HasSameOwnerAs(IGameItem other)
    {
        return OwnershipReference == other.OwnershipReference;
    }

    public event PerceivableEvent OnRemovedFromLocation;

    public event InventoryChangeEvent OnInventoryChange;

    public void InvokeInventoryChange(InventoryState oldState, InventoryState newState)
    {
		if (MudSharp.Magic.PsychometricRecorder.Enabled(Gameworld))
		{
			foreach (var item in DeepItems.Prepend(this).Distinct()) MudSharp.Magic.PsychometricRecorder.ObserveCustody(item);
		}
        OnInventoryChange?.Invoke(oldState, newState, this);
    }

    #region IEquatable<IGameItem> Members

    public bool Equals(IGameItem other)
    {
        if (other == null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        if (!IdInitialised || !((other as GameItem)?.IdInitialised ?? false))
        {
            return Equals(this, other);
        }

        return Id == other.Id && FrameworkItemType.Equals(other.FrameworkItemType);
    }

    #endregion

    public override void Register(IOutputHandler handler)
    {
        OutputHandler = handler;
        handler.Register(this);
    }

    public override string FrameworkItemType => "GameItem";

    public override double IlluminationProvided
    {
        get
        {
            double total = 0.0D;
            if (IsItemType<IProduceLight>())
            {
                total += GetItemType<IProduceLight>().CurrentIllumination;
            }

            if (IsItemType<IContainer>() && GetItemType<IContainer>().Transparent)
            {
                total += GetItemType<IContainer>().Contents.Sum(x => x.IlluminationProvided);
            }

            if (IsItemType<ISheath>())
            {
                ISheath sheath = GetItemType<ISheath>();
                total += sheath.Content?.Parent.IlluminationProvided ?? 0;
            }

            if (IsItemType<ICorpse>())
            {
                ICorpse corpse = GetItemType<ICorpse>();
                total += corpse.Body?.ExternalItems.Sum(x => x.IlluminationProvided) ?? 0.0;
            }

            foreach (IProduceIllumination effect in EffectsOfType<IProduceIllumination>().Where(x => x.Applies()))
            {
                total += effect.ProvidedLux;
            }

            return total;
        }
    }

    #region ISaveable Members

    public override void Save()
    {
		ForeignCustodyTransferContext.RecordSave(this);
        Models.GameItem dbitem = FMDB.Context.GameItems.Find(Id);
        if (dbitem is null)
        {
            Gameworld.DebugMessage($"An item couldn't find itself in the database - {Id:N0} {HowSeen(this, colour: false, flags: PerceiveIgnoreFlags.TrueDescription)} (Proto {Prototype.Id:N0}r{Prototype.RevisionNumber:N0})");
            Changed = false;
            return;
        }
        dbitem.Quality = (int)_quality;
        dbitem.MaterialId = _overrideMaterial?.Id ?? 0;
        dbitem.Size = (int)Size;
        dbitem.ContainerId = ContainedIn?.Id;
        dbitem.OwnerId = _ownerReference?.Id;
        dbitem.OwnerType = _ownerReference?.FrameworkItemType;
        dbitem.Condition = Condition;
		dbitem.RoomLayer = (int)(HasEffectiveSpatialHost ? _roomLayer : RoomLayer);
		dbitem.RoutePosition = PersistedRoutePositionMetres.HasValue
			? (decimal)PersistedRoutePositionMetres.Value
			: null;
        dbitem.SkinId = _skinId;
        dbitem.OverrideSdesc = OverrideSdesc;
        dbitem.OverrideDesc = OverrideDesc;
        if (PositionChanged)
        {
            SavePosition(dbitem);
        }

        SaveMorphProgress(dbitem);
        if (EffectsChanged)
        {
            dbitem.EffectData = SaveEffects().ToString();
            EffectsChanged = false;
        }

        if (_surfaceLiquidChanged)
        {
            dbitem.SurfaceLiquidData = SaveSurfaceLiquidState();
            _surfaceLiquidChanged = false;
        }

        if (ResourcesChanged)
        {
            SaveMagic(dbitem);
        }

        if (HooksChanged)
        {
            FMDB.Context.HooksPerceivables.RemoveRange(dbitem.HooksPerceivables);
            foreach (IHook hook in _installedHooks)
            {
                dbitem.HooksPerceivables.Add(new HooksPerceivable
                {
                    GameItem = dbitem,
                    HookId = hook.Id
                });
            }

            HooksChanged = false;
        }

        base.Save();
    }

    #endregion


    /// <inheritdoc />
    public override IEnumerable<string> GetKeywordsFor(IPerceiver voyeur)
    {
        return GetKeywordsFromSDesc((this as IHaveCharacteristics).ParseCharacteristics(
                   HowSeen(voyeur, colour: false, flags: PerceiveIgnoreFlags.IgnoreNamesSetting), voyeur))
               .Concat(GetKeywordsFromSDesc((this as IHaveCharacteristics).ParseCharacteristics(
                   HowSeen(voyeur, type: DescriptionType.Long, colour: false,
                       flags: PerceiveIgnoreFlags.IgnorePositionInformationForLongDesc | PerceiveIgnoreFlags.IgnoreNamesSetting), voyeur)))
               .Distinct()
               .ToList();
    }

    public override string HowSeen(IPerceiver voyeur, bool proper = false,
        DescriptionType type = DescriptionType.Short, bool colour = true,
        PerceiveIgnoreFlags flags = PerceiveIgnoreFlags.None)
    {
        if (voyeur == null)
        {
            voyeur = this;
        }

        var overrideEffect = EffectsOfType<IOverrideDescEffect>()
                             .Where(x => x.OverrideApplies(voyeur, type))
                             .OrderByDescending(x => (x as IPrioritisedOverrideDescEffect)?.OverridePriority ?? 0)
                             .FirstOrDefault();
        if (overrideEffect is not null && voyeur.CanSee(this))
        {
            return overrideEffect.Description(type, colour);
        }

        switch (type)
        {
            case DescriptionType.Short:
                return ShortDescription(voyeur, proper, colour, flags);
            case DescriptionType.Possessive:
                return HowSeen(voyeur, proper, DescriptionType.Short, colour) + "'s";
            case DescriptionType.Long:
                return LongDescription(voyeur, proper, colour, flags);
            case DescriptionType.Full:
                return FullDescription(voyeur, colour, flags, false);
            case DescriptionType.Contents:
                return
                    (this as IHaveCharacteristics).ParseCharacteristics(DisplayContents(voyeur, colour, flags), voyeur)
                                                  .Wrap(voyeur.InnerLineFormatLength);
            default:
                throw new NotImplementedException();
        }
    }

    private string LongDescription(IPerceiver voyeur, bool proper, bool colour, PerceiveIgnoreFlags flags)
    {
        var ignorePosition = flags.HasFlag(PerceiveIgnoreFlags.IgnorePositionInformationForLongDesc);
        string ldesc = HowSeen(voyeur, true, DescriptionType.Short, colour, flags);
        bool alteredldesc = false;
        string name = Name;
        if (Skin is { } skin)
        {
            name = skin.Name ?? Name;
            if (skin.LongDescription is not null)
            {
                ldesc = skin.LongDescription;
                alteredldesc = true;
            }
        }

        else if (Prototype.OverridesLongDescription)
        {
            ldesc = Prototype.LongDescription;
            alteredldesc = true;
        }

        ldesc = ItemMaterialRegex.Replace(
            ldesc,
            match => match.Groups["which"].Value.Equals("material")
                ? Material.Name.ToLowerInvariant()
                : Material.MaterialDescription.ToLowerInvariant());

        if (alteredldesc)
        {
            ldesc = (this as IHaveCharacteristics).ParseCharacteristics(ldesc, voyeur)
                                                   .AppendRemoteObservationTag(voyeur, this, colour, flags);
        }

        if ((alteredldesc && PositionTarget == null && PositionEmote == null))
        {
            return DressLongDescription(voyeur,
                ldesc.Fullstop()).FluentProper(proper).FluentColourIncludingReset(Prototype.CustomColour ?? Telnet.Green, colour);
        }

        if (ignorePosition)
        {
            return $"{ldesc}{(ldesc.Contains(name.Pluralise(), StringComparison.InvariantCultureIgnoreCase) ? " are " : " is ")} here.";
        }

        string description = DressLongDescription(voyeur, ldesc);
        return $"{description}{(description.Contains(name.Pluralise(), StringComparison.InvariantCultureIgnoreCase) ? " are " : " is ")}{DescribePosition(voyeur).Fullstop()}";
    }

    private string ShortDescription(IPerceiver voyeur, bool proper, bool colour,
        PerceiveIgnoreFlags flags = PerceiveIgnoreFlags.None)
    {
        if (!voyeur.CanSee(this, flags) && voyeur != this)
        {
            return (colour ? "something".ColourIncludingReset(Telnet.Green) : "something").FluentProper(proper);
        }

        string description = OverrideSdesc ?? Skin?.ShortDescription ?? Prototype.ShortDescription;
        if (OverrideSdesc is null && Skin?.ShortDescription is null && voyeur is ICharacter ch)
        {
            (FutureProg.IFutureProg Prog, string ShortDescription, string FullDescription, string FullDescriptionAddendum) descValue = Prototype.ExtraDescriptions.Where(x => !string.IsNullOrEmpty(x.ShortDescription))
                                     .FirstOrDefault(x => x.Prog.Execute<bool?>(ch) == true);
            if (descValue.Prog != null)
            {
                description = descValue.ShortDescription;
            }
        }

        description = description.SubstituteANSIColour();
        string text = description;
        text = text.SubstituteWrittenLanguage(voyeur, Gameworld);
        text = (this as IHaveCharacteristics).ParseCharacteristics(text, voyeur);
        text = ParseDescription(voyeur, text, DescriptionType.Short,
            colour ? Prototype.CustomColour ?? Telnet.Green : null, flags, false);
        if (!flags.HasFlag(PerceiveIgnoreFlags.IgnoreLiquidsAndFlags))
        {
            text = ProcessDescriptionAdditions(text, voyeur, colour, flags);
        }

        if (!colour)
        {
            text = text.StripANSIColour();
        }

        return text.FluentProper(proper);
    }

    public override bool HandleEvent(EventType type, params dynamic[] arguments)
    {
        bool truth = false;
        foreach (IGameItemComponent comp in _components)
        {
            truth |= comp.HandleEvent(type, arguments);
        }

        foreach (IHandleEventsEffect effect in EffectsOfType<IHandleEventsEffect>().ToList())
        {
            truth |= effect.HandleEvent(type, arguments);
        }

        return truth || base.HandleEvent(type, arguments);
    }

    public override object DatabaseInsert()
    {
        Models.GameItem dbitem = new()
        {
            GameItemProtoId = Prototype.Id,
            GameItemProtoRevision = Prototype.RevisionNumber,
            MaterialId = 0,
            Quality = (int)_quality,
            Size = (int)Size,
            PositionId = (int)PositionUndefined.Instance.Id,
            PositionModifier = (int)PositionModifier.None,
			RoomLayer = (int)(HasEffectiveSpatialHost ? _roomLayer : RoomLayer),
			RoutePosition = PersistedRoutePositionMetres.HasValue
				? (decimal)PersistedRoutePositionMetres.Value
				: null,
            EffectData = SaveEffects().ToString(),
            SurfaceLiquidData = SaveSurfaceLiquidState(),
            OverrideSdesc = OverrideSdesc,
            OverrideDesc = OverrideDesc,
            OwnerId = _ownerReference?.Id,
            OwnerType = _ownerReference?.FrameworkItemType
        };
        SaveMorphProgress(dbitem);
        FMDB.Context.GameItems.Add(dbitem);

        foreach (IGameItemComponent item in Components)
        {
            item.PrimeComponentForInsertion(dbitem);
        }

        foreach (IHook hook in _installedHooks)
        {
            HooksPerceivable dbhook = new();
            FMDB.Context.HooksPerceivables.Add(dbhook);
            dbhook.HookId = hook.Id;
            dbhook.GameItem = dbitem;
        }

        return dbitem;
    }

    public override void SetIDFromDatabase(object item)
    {
        Models.GameItem dbitem = (MudSharp.Models.GameItem)item;
        _id = dbitem.Id;
    }

    public string ProcessDescriptionAdditions(string description, IPerceiver voyeur, bool colour,
        PerceiveIgnoreFlags flags = PerceiveIgnoreFlags.None)
    {
        description = description.AppendRemoteObservationTag(voyeur, this, colour, flags);
        description =
            EffectsOfType<ISDescAdditionEffect>()
                .Where(x => x.DescriptionAdditionApplies(voyeur))
                .DistinctBy(x => x.AddendumText)
                .Select(effect => effect.GetAddendumText(colour))
                .Aggregate(description, (current, text) => $"{current} {text}");

        var (coating, absorb) = LiquidAbsorbtionAmounts;
        var surfaceAddendum = SurfaceLiquidState.GetAddendumText(coating, absorb, colour);
        return string.IsNullOrWhiteSpace(surfaceAddendum) ? description : $"{description} {surfaceAddendum}";
    }

    private string FullDescription(IPerceiver voyeur, bool colour, PerceiveIgnoreFlags flags, bool excludeComponents)
    {
        if (!voyeur.CanSee(this))
        {
            return "You cannot make out anything about it.";
        }

        string description = OverrideDesc ?? Skin?.FullDescription ?? Prototype.FullDescription;

        if (voyeur is ICharacter ch)
        {
            if (OverrideDesc is null && Skin?.FullDescription is null)
            {
                var descValue = Prototype.ExtraDescriptions
                    .Where(x => !string.IsNullOrEmpty(x.FullDescription))
                    .FirstOrDefault(x => x.Prog.Execute<bool?>(ch) == true);
                if (descValue.Prog != null)
                {
                    description = descValue.FullDescription;
                }
            }

            (FutureProg.IFutureProg Prog, string ShortDescription, string FullDescription, string FullDescriptionAddendum) addendumValue = Prototype.ExtraDescriptions.Where(x => !string.IsNullOrEmpty(x.FullDescriptionAddendum))
                                         .FirstOrDefault(x => x.Prog.Execute<bool?>(ch) == true);
            if (addendumValue.Prog != null)
            {
                description = $"{description}\n\n{addendumValue.FullDescriptionAddendum}";
            }
        }

        description = description.SubstituteANSIColour().Append("\n");

        string text = ParseDescription(voyeur, description, DescriptionType.Full, colour, flags, excludeComponents);

        text = text.SubstituteWrittenLanguage(voyeur, Gameworld);

        text = (this as IHaveCharacteristics).ParseCharacteristics(text, voyeur);

        text = EffectsOfType<IDescriptionAdditionEffect>().Where(x => x.DescriptionAdditionApplies(voyeur))
                                                          .Aggregate(text,
                                                              (current, component) =>
                                                                  $"{current}\n\t{component.GetAdditionalText(voyeur, true)}");
        if (!flags.HasFlag(PerceiveIgnoreFlags.IgnoreLiquidsAndFlags))
        {
            var (coating, absorb) = LiquidAbsorbtionAmounts;
            var surfaceText = SurfaceLiquidState.GetAdditionalText(coating, absorb, voyeur, colour);
            if (!string.IsNullOrWhiteSpace(surfaceText))
            {
                text = $"{text}\n\t{surfaceText}";
            }
        }

        string auraText = MagicPerceptionUtilities.DescribeMagicAuras(voyeur, Effects);
        if (!string.IsNullOrEmpty(auraText))
        {
            text = $"{text}\n\t{auraText.Replace("\n", "\n\t")}";
        }

        return _components.Any(x => x.WrapFullDescription) ? text.Wrap(voyeur.InnerLineFormatLength) : text;
    }

    public static void RegisterPerceivableType(IFuturemud gameworld)
    {
        gameworld.RegisterPerceivableType("GameItem", id => gameworld.Items.Get(id));
    }

    private static readonly Regex ItemMaterialRegex = new("@(?<which>mat(?:erial|desc))");

    protected virtual string ParseDescription(IPerceiver voyeur, string input, DescriptionType type,
        ANSIColour colour, PerceiveIgnoreFlags flags, bool excludeComponents)
    {
        input = ItemMaterialRegex.Replace(
            input,
            match => match.Groups["which"].Value.Equals("material")
                ? Material.Name.ToLowerInvariant()
                : Material.MaterialDescription.ToLowerInvariant());
        if (excludeComponents)
        {
            return input;
        }

        string preColour =
            _components.Where(x => x.DescriptionDecorator(type) && x.DecorationPriority >= 0)
                       .OrderBy(x => x.DecorationPriority)
                       .Aggregate(input,
                           (current, component) =>
                               component.Decorate(voyeur, _name.ToLowerInvariant(), current, type, colour != null,
                                   flags));
        if (colour != null)
        {
            preColour = preColour.ColourIncludingReset(colour);
        }

        return
            _components.Where(x => x.DescriptionDecorator(type) && x.DecorationPriority < 0)
                       .OrderByDescending(x => x.DecorationPriority)
                       .Aggregate(preColour,
                           (current, component) =>
                               component.Decorate(voyeur, _name.ToLowerInvariant(), current, type, colour != null,
                                   flags));
    }

    protected virtual string ParseDescription(IPerceiver voyeur, string input, DescriptionType type, bool colour,
        PerceiveIgnoreFlags flags, bool excludeComponents)
    {
        input = ItemMaterialRegex.Replace(
            input,
            match => match.Groups["which"].Value.Equals("material")
                ? Material.Name.ToLowerInvariant()
                : Material.MaterialDescription.ToLowerInvariant());
        if (excludeComponents)
        {
            return input;
        }

        return _components.Where(x => x.DescriptionDecorator(type))
                          .OrderBy(x => x.DecorationPriority)
                          .Aggregate(input,
                              (current, component) =>
                                  component.Decorate(voyeur, _name.ToLowerInvariant(), current, type, colour, flags))
            ;
    }

    private string DisplayContents(IPerceiver voyeur, bool colour, PerceiveIgnoreFlags flags)
    {
        string description = OverrideDesc ?? Skin?.FullDescription ?? Prototype.FullDescription;
        if (OverrideDesc is null && Skin?.FullDescription is null && voyeur is ICharacter ch)
        {
            (FutureProg.IFutureProg Prog, string ShortDescription, string FullDescription, string FullDescriptionAddendum) descValue = Prototype.ExtraDescriptions.Where(x => !string.IsNullOrEmpty(x.FullDescription))
                                     .FirstOrDefault(x => x.Prog.Execute<bool?>(ch) == true);
            if (descValue.Prog != null)
            {
                description = descValue.FullDescription;
            }
        }

        description = description.SubstituteANSIColour().Append("\n");

        string text = ParseDescription(voyeur, description, DescriptionType.Full, colour, flags, true);

        text = text.SubstituteWrittenLanguage(voyeur, Gameworld);

        text = (this as IHaveCharacteristics).ParseCharacteristics(text, voyeur);

        return
            _components.Where(x => x.DescriptionDecorator(DescriptionType.Contents))
                       .OrderBy(x => x.DecorationPriority)
                       .Aggregate(text,
                           (current, component) =>
                               component.Decorate(voyeur, Name, current, DescriptionType.Contents, colour, flags));
    }

    private string DressLongDescription(IPerceiver voyeur, string description)
    {
        StringBuilder sb = new(description);
        if (EffectHandler.EffectsOfType<IAdminInvisEffect>().Any())
        {
            sb.Append(" (wizinvis)".ColourBold(Telnet.Blue));
        }

        if (EffectHandler.EffectsOfType<IItemHiddenEffect>().Any())
        {
            sb.Append(" (hidden)".Colour(Telnet.Red));
        }

        return sb.ToString();
    }

    public bool HighPriority => Prototype.HighPriority;

    #region Constructors and Setup

    public override string ToString()
    {
        return
            $"Game Item {Id}, Proto {Prototype.Id}r{Prototype.RevisionNumber} - {Prototype.ShortDescription}";
    }

    // The original database containment is evidence for component reconstruction before
    // another root has loaded this child's hand or room membership.
    internal bool LoadedFromDatabase { get; }
    internal long? ContainerIdAtLoad { get; }

    public GameItem(MudSharp.Models.GameItem item, IFuturemud game)
        : base(item.Id)
    {
        _noSave = true;
        Register(new IgnorantItemOutputHandler(this));
        Gameworld = game;
        _id = item.Id;
		LoadedFromDatabase = true;
		ContainerIdAtLoad = item.ContainerId;
		SpellCreationOrigin = game.SpellOwnedItems?.FindOrigin(item.Id);
        Prototype = game.ItemProtos.Get(item.GameItemProtoId, item.GameItemProtoRevision);
        if (Prototype == null)
        {
            throw new ApplicationException(
                $"GameItem {Id} was loaded with an invalid prototype {item.GameItemProtoId}r{item.GameItemProtoRevision}");
        }

        _name = Prototype.Name;
        _keywords = new Lazy<List<string>>(() => ((GameItemProto)Prototype).Keywords.ToList());
        _quality = (ItemQuality)item.Quality;
        _condition = item.Condition;
        _overrideMaterial = game.Materials.Get(item.MaterialId);
        _roomLayer = (RoomLayer)item.RoomLayer;
        _skinId = item.SkinId;
        _overrideSdesc = item.OverrideSdesc;
        _overrideDesc = item.OverrideDesc;
        if (item.OwnerId.HasValue && !string.IsNullOrWhiteSpace(item.OwnerType))
        {
            _ownerReference = new FrameworkItemReference(item.OwnerId.Value, item.OwnerType, game);
        }
        foreach (Models.GameItemComponent component in item.GameItemComponents)
        {
            _components.Add(
                Gameworld.ItemComponentProtos.Get(component.GameItemComponentProtoId,
                             component.GameItemComponentProtoRevision)
                         .LoadComponent(component, this));
        }

        LoadPosition(item.PositionId, item.PositionModifier, item.PositionEmote, item.PositionTargetId,
            item.PositionTargetType);
        LoadEffects(XElement.Parse(item.EffectData.IfNullOrWhiteSpace("<Effects/>")));
        LoadSurfaceLiquidState(item.SurfaceLiquidData);
        LoadHooks(item.HooksPerceivables, "GameItem");
		_overridingWoundBehaviourComponent = _components.OfType<IOverrideItemWoundBehaviour>().FirstOrDefault();
        LoadWounds(item.WoundsGameItem);
        LoadMagic(item);
        if (Prototype.Morphs)
        {
            if (item.MorphTimeRemaining.HasValue)
            {
                if (item.MorphTimeRemaining < 0)
                {
                    CachedMorphTime = TimeSpan.FromSeconds(60);
                }
                else
                {
                    CachedMorphTime = TimeSpan.FromSeconds(item.MorphTimeRemaining.Value);
                }
            }
            else
            {
                MorphTime = DateTime.MinValue;
            }
        }

        _noSave = false;
    }

    private void LoadWounds(IEnumerable<Wound> wounds)
    {
        foreach (Wound wound in wounds)
        {
            _wounds.Add(WoundFactory.LoadWound(wound, this, Gameworld));
        }

        if (_overridingWoundBehaviourComponent is IBodyRemains && Gameworld.SaveManager.MudBootingMode)
        {
            _deferredInitialHealthTick = true;
            Gameworld.RegisterPostCharacterLoadFinalisable(this);
            return;
        }

        StartHealthTick();
    }

    void IPostCharacterLoadFinalisable.FinaliseLoading()
    {
        if (!_deferredInitialHealthTick)
        {
            return;
        }

        _deferredInitialHealthTick = false;
        StartHealthTick();
    }

    public GameItem(IGameItemProto proto, ICharacter? loader = null, ItemQuality quality = ItemQuality.Standard)
		: this(proto, loader, quality, deferSpellInitialisation: false)
	{
	}

	internal GameItem(IGameItemProto proto, ICharacter? loader, ItemQuality quality, bool deferSpellInitialisation)
		: this(proto, loader, quality, deferSpellInitialisation, currencyPreview: false)
	{
	}

	internal GameItem(IGameItemProto proto, ICharacter? loader, ItemQuality quality, bool deferSpellInitialisation, bool currencyPreview)
    {
		if (currencyPreview && deferSpellInitialisation) throw new ArgumentException("Currency previews cannot use deferred spell initialisation.");
		_currencyPreview = currencyPreview;
		_noSave = deferSpellInitialisation || currencyPreview;
        Register(new IgnorantItemOutputHandler(this));
        if (proto == null)
        {
            throw new ApplicationException("GameItem loaded with null proto.");
        }

        Gameworld = proto.Gameworld;
        if (Gameworld == null)
        {
            throw new ApplicationException("GameItem loaded with null gameworld");
        }

        _condition = 1.0;
        Prototype = proto;
        _name = Prototype.Name;
        _quality = quality;
        _keywords = new Lazy<List<string>>(() => proto.Keywords.ToList());

        foreach (IGameItemComponentProto component in proto.Components)
        {
            _components.Add(component.CreateNew(this, loader, temporary: deferSpellInitialisation || currencyPreview));
        }

        List<IHook> hooks = currencyPreview ? [] : Gameworld.DefaultHooks.Where(
            x => x.Applies(this, "GameItem")).Select(x => x.Hook).ToList();

		if (deferSpellInitialisation && hooks.Any()) throw new InvalidOperationException("Lifecycle weapons require a prototype without applicable default hooks.");
        if (!deferSpellInitialisation && !currencyPreview && hooks.Any())
        {
            foreach (IHook hook in hooks)
            {
                InstallHook(hook);
            }
        }

        SetState(PositionUndefined.Instance);
		if (!deferSpellInitialisation && !currencyPreview) Gameworld.SaveManager.AddInitialisation(this);
        foreach (IGameItemComponent comp in deferSpellInitialisation || currencyPreview ? Enumerable.Empty<IGameItemComponent>() : Components)
        {
            comp.FinaliseLoad();
        }

        if (Prototype.Morphs)
        {
            CachedMorphTime = Prototype.MorphTimeSpan;
        }

        _overridingWoundBehaviourComponent = _components.OfType<IOverrideItemWoundBehaviour>().FirstOrDefault();
    }

	private bool _currencyPreview;

	internal void SetPreparedCurrencyCustody(IBody? holder, IGameItem? container, SpatialLocation? floor)
	{
		// Only the prepared currency transfer calls this after validating native participants.
		GetItemType<HoldableGameItemComponent>().HeldBy = holder;
		_containedIn = container;
		SetPreparedSpatialState(floor);
	}

	internal void ClearPreparedContainerSourcePosition() => SetPreparedSpatialState(null);

	internal void CopyCurrencyPreviewOwner(IGameItem source)
	{
		if (!_currencyPreview) throw new InvalidOperationException("Only an unpublished currency preview can copy an owner silently.");
		var owner = source.OwnershipReference;
		_ownerReference = owner is null ? null : new FrameworkItemReference(owner.Value.Id, owner.Value.FrameworkItemType, Gameworld);
		// Do not resolve the owner or dispatch ItemOwnershipChanged during eligibility checks.
		_owner = null;
	}

	internal void NotifyCommittedCurrencyOwner(IGameItem source)
	{
		MudSharp.Magic.PsychometricRecorder.CopyHistory(source, this);
		Changed = true;
		var reference = OwnershipReference;
		HandleEvent(EventType.ItemOwnershipChanged, this, string.Empty, 0L,
			reference?.FrameworkItemType ?? string.Empty, reference?.Id ?? 0L);
	}

	internal void ActivateCurrencySplit()
	{
		if (!_currencyPreview) throw new InvalidOperationException("Only this unpublished currency item can be activated.");
		_currencyPreview = false;
		_noSave = false;
		Gameworld.SaveManager.AddInitialisation(this);
		foreach (var component in Components.Cast<GameItemComponent>())
		{
			component.SetNoSave(false);
			Gameworld.SaveManager.AddInitialisation(component);
		}
	}

	internal void FinishCurrencySplitLoading(Func<bool> mayContinue)
	{
		if (_currencyPreview) throw new InvalidOperationException("The split has not committed.");
		foreach (var hook in Gameworld.DefaultHooks.ToArray())
		{
			if (!mayContinue()) return;
			var applies = hook.Applies(this, "GameItem");
			if (!mayContinue()) return;
			if (applies) InstallHook(hook.Hook);
		}
		foreach (var component in Components.ToArray())
		{
			if (!mayContinue()) return;
			component.FinaliseLoad();
		}
		if (!mayContinue()) return;
		foreach (var prog in ((GameItemProto)Prototype).OnLoadProgs.ToArray())
		{
			if (!mayContinue()) return;
			prog.Execute(this, null);
			if (!mayContinue()) return;
		}
		Login();
		if (!mayContinue()) return;
		HandleEvent(EventType.ItemFinishedLoading, this);
	}

    public GameItem(GameItem rhs, bool temporary = false, bool preserveMorphTime = false)
		: this(rhs, temporary, preserveMorphTime, null)
	{
	}

	internal GameItem(GameItem rhs, bool temporary, bool preserveMorphTime, Action<GameItem> beforeExposure)
    {
		if (!temporary) SpellOwnedItemValuePolicy.RequireOrdinaryValue(rhs, "copying");
		if (rhs.GetItemType<ICorpse>() is not null && rhs.Gameworld.SpellOwnedCorpseAnimations?.IsBorrowedCorpse(rhs.Id) == true)
			throw new InvalidOperationException("A borrowed animated corpse cannot be copied.");
		SpellCreationOrigin = rhs.SpellCreationOrigin;
        if (temporary)
        {
            _noSave = true;
        }

        Register(new IgnorantItemOutputHandler(this));
        Prototype = rhs.Prototype;
        _quality = rhs._quality;
        _overrideMaterial = rhs._overrideMaterial;
        _overrideSdesc = rhs._overrideSdesc;
        _overrideDesc = rhs._overrideDesc;
        Gameworld = rhs.Gameworld;
		if (beforeExposure is null) MudSharp.Magic.PsychometricRecorder.CopyHistory(rhs, this);
        _name = rhs.Name;
        _keywords = new Lazy<List<string>>(() => rhs.Keywords.ToList());
		if (beforeExposure is null)
		{
			PositionState = rhs.PositionState;
			PositionModifier = rhs.PositionModifier;
			PositionTarget = rhs.PositionTarget;
			PositionEmote = rhs.PositionEmote;
		}
        _condition = rhs.Condition;
		if (beforeExposure is null) Location = rhs.Location;
        _ownerReference = rhs._ownerReference == null
            ? null
            : new FrameworkItemReference(rhs._ownerReference.Id, rhs._ownerReference.FrameworkItemType, rhs.Gameworld);
        _owner = rhs._owner;
        if (beforeExposure is not null && Prototype.Morphs)
        {
            if (preserveMorphTime)
            {
                if (rhs.CachedMorphTime is not null)
                {
                    CachedMorphTime = rhs.CachedMorphTime;
                }
                else
                {
					CachedMorphTime = ItemTimeRateMath.PreservedMorphRemaining(
						rhs.MorphTime - RuntimeClock.UtcNow,
						Prototype.RefrigerationSensitive,
						rhs._morphRateAtSchedule);
                }
            }
            else
            {
                CachedMorphTime = Prototype.MorphTimeSpan;
            }
        }

        foreach (IGameItemComponent component in rhs._components)
        {
            _components.Add(component.Copy(this, temporary));
        }

        if (!temporary)
        {
			if (beforeExposure is not null)
			{
				try { beforeExposure(this); }
				catch
				{
					// A rejected preparation has not exposed its copy or committed value.
					foreach (var component in _components) Gameworld.SaveManager.Abort(component);
					throw;
				}
				// An Add observer may request an ID/flush. Queue the parent first.
				Gameworld.SaveManager.AddInitialisation(this);
			}
            Gameworld.Add(this);
			if (beforeExposure is null) Gameworld.SaveManager.AddInitialisation(this);
        }

		if (beforeExposure is not null)
		{
			MudSharp.Magic.PsychometricRecorder.CopyHistory(rhs, this);
			PositionState = rhs.PositionState;
			PositionModifier = rhs.PositionModifier;
			PositionTarget = rhs.PositionTarget;
			PositionEmote = rhs.PositionEmote;
		}

        foreach (IGameItemComponent component in Components)
        {
            component.FinaliseLoad();
        }

        LoadSurfaceLiquidState(rhs.SaveSurfaceLiquidState());

        if (beforeExposure is null && Prototype.Morphs)
        {
            if (preserveMorphTime)
            {
                if (rhs.CachedMorphTime is not null)
                {
                    CachedMorphTime = rhs.CachedMorphTime;
                }
                else
                {
					CachedMorphTime = ItemTimeRateMath.PreservedMorphRemaining(
						rhs.MorphTime - RuntimeClock.UtcNow,
						Prototype.RefrigerationSensitive,
						rhs._morphRateAtSchedule);
                }
            }
            else
            {
                CachedMorphTime = Prototype.MorphTimeSpan;
            }
        }

        if (!temporary)
        {
            foreach (IEffect effect in rhs.Effects)
            {
                IEffect newEffect = effect.NewEffectOnItemMorph(rhs, this);
                if (newEffect != null && newEffect != effect)
                {
                    TimeSpan duration = rhs.ScheduledDuration(effect);
                    if (duration > TimeSpan.Zero)
                    {
                        AddEffect(newEffect, duration);
                    }
                    else
                    {
                        AddEffect(newEffect);
                    }
                }
            }
        }

        _overridingWoundBehaviourComponent = _components.OfType<IOverrideItemWoundBehaviour>().FirstOrDefault();
    }

    public void LoadPosition(MudSharp.Models.GameItem item)
    {
        LoadPosition(item.PositionId, item.PositionModifier, item.PositionEmote, item.PositionTargetId,
            item.PositionTargetType);
    }

    /// <summary>
    ///     This function is called at the end of a batch of loading for tasks that require other objects to be fully loaded
    ///     and ready
    /// </summary>
    public void FinaliseLoadTimeTasks()
    {
		if (_loadTimeTasksFinalised)
		{
			return;
		}

		// Mark this before walking components because connected and contained item graphs can be cyclic.
		_loadTimeTasksFinalised = true;
        foreach (IGameItemComponent component in Components)
        {
            component.FinaliseLoad();
        }

        foreach (IGameItem item in _wounds.SelectNotNull(x => x.Lodged))
        {
            item.FinaliseLoadTimeTasks();
        }
    }

	private bool _loadTimeTasksFinalised;

    #endregion

    #region IGameItem Members

    private long? _skinId;

    public IGameItemSkin Skin
    {
        get => Gameworld.ItemSkins.Get(_skinId ?? 0);
        set
        {
            _skinId = value?.Id;
            Changed = true;
        }
    }

    double LiquidVolumeFromPrecipitation(PrecipitationLevel level)
    {
        return Gameworld.GetStaticDouble($"PrecipitationAmountPerItemSize{Size.DescribeEnum()}{level.DescribeEnum()}");
    }

    public void ExposeToPrecipitation(PrecipitationLevel level, ILiquid liquid)
    {
        LiquidMixture mixture = new(liquid, LiquidVolumeFromPrecipitation(level), Gameworld);
        ExposeToLiquid(mixture, null, LiquidExposureDirection.FromOnTop);
    }

    public void ExposeToLiquid(LiquidMixture mixture, IBodypart part, LiquidExposureDirection direction)
    {
        ExposureTransport.Item(this, mixture, part, direction);
    }


    public ItemSaturationLevel SaturationLevel
    {
        get
        {
            (double coating, double absorb) = LiquidAbsorbtionAmounts;
            return SurfaceLiquidState.SaturationLevel(coating, absorb);
        }
    }

    public ItemSaturationLevel SaturationLevelForLiquid(LiquidInstance instance)
    {
        (double coating, double absorb) = LiquidAbsorbtionAmounts;
        return SurfaceLiquidState.SaturationLevelForLiquid(instance.Amount, coating, absorb);
    }

    public ItemSaturationLevel SaturationLevelForLiquid(double total)
    {
        (double coating, double absorb) = LiquidAbsorbtionAmounts;
        return SurfaceLiquidState.SaturationLevelForLiquid(total, coating, absorb);
    }

    public (double Coating, double Absorb) LiquidAbsorbtionAmounts
    {
        get
        {
            if (Material == null)
            {
                return (0.0, 0.0);
            }

            // Coating amount is based on ~0.2L for a "normal" sized object and loosely tied to surface area
            // Absorbency amount is an abstracted value from material properties
            return (Math.Pow((int)Size, 2) * 0.008 / Gameworld.UnitManager.BaseFluidToLitres,
                Material.Absorbency * Prototype.Weight * Gameworld.UnitManager.BaseWeightToKilograms /
                Gameworld.UnitManager.BaseFluidToLitres);
        }
    }

    public override bool ShouldFall()
    {
        return IsItemType<IHoldable>() &&
               !EffectsOfType<IPreventFallingEffect>().Any(x => x.Applies()) &&
               base.ShouldFall();
    }

    private double _condition;

    public double Condition
    {
        get => _condition;
        set
        {
            _condition = Math.Clamp(value, 0.0, 1.0);
            Changed = true;
        }
    }

    public bool WarnBeforePurge => _components.Any(x => x.WarnBeforePurge);

    private static Expression _fallDamageExpression;

    public static Expression FallDamageExpression
    {
        get
        {
            if (_fallDamageExpression == null)
            {
                _fallDamageExpression =
                    new Expression(Futuremud.Games.First().GetStaticConfiguration("ItemFallDamageExpression"));
            }

            return _fallDamageExpression;
        }
    }

    public override void DoFallDamage(double fallDistance)
    {
        var fallMitigationEffects = EffectsOfType<IFallDamageMitigationEffect>()
            .Where(x => x.Applies())
            .ToList();
        var effectiveFallDistance = fallMitigationEffects.Aggregate(fallDistance,
            (current, effect) => current * Math.Max(0.0, effect.FallDistanceMultiplier));
        var damageMultiplier = fallMitigationEffects.Aggregate(1.0,
            (current, effect) => current * Math.Max(0.0, effect.FallDamageMultiplier));

        var damageAmount = FallDamageExpression.EvaluateDoubleWith(("weight", Weight),
            ("rooms", effectiveFallDistance)) * damageMultiplier;
        if (damageAmount <= 0.0)
        {
            return;
        }

        Damage damage = new()
        {
            DamageType = DamageType.Falling,
            DamageAmount = damageAmount
        };
        SufferDamage(damage);
    }

    private static ICharacterCombatSettings _gameItemSettings;

    public static ICharacterCombatSettings GameItemSettings => _gameItemSettings ??= Futuremud.Games.First()
        .CharacterCombatSettings.Get(Futuremud.Games.First().GetStaticLong("GameItemCombatSettingsId"));

    public override ICharacterCombatSettings CombatSettings
    {
        get => GameItemSettings;
        set { }
    }

    public IEnumerable<IGameItem> AttachedAndConnectedItems => (GetItemType<IConnectable>()?.ConnectedItems.Select(x => x.Item2.Parent) ??
                    Enumerable.Empty<IGameItem>())
                   .Concat(GetItemType<IBelt>()?.ConnectedItems.Select(x => x.Parent) ?? [])
                   .Concat(GetItemType<IFirearmAttachmentHost>()?.InstalledAttachments.Values.Select(x => x.Parent) ?? [])
                   .Concat(Components.OfType<IProvideItemTargetProjections>().SelectMany(x => x.TargetProjections))
                   .Concat(Wounds.SelectNotNull(x => x.Lodged));

    public IEnumerable<IGameItem> LodgedItems => Wounds.SelectNotNull(x => x.Lodged).ToArray();
    public IEnumerable<IGameItem> AttachedItems =>
        (GetItemType<IBelt>()?.ConnectedItems.Select(x => x.Parent) ?? [])
        .Concat(GetItemType<IFirearmAttachmentHost>()?.InstalledAttachments.Values.Select(x => x.Parent) ?? [])
        .ToArray();
    public IEnumerable<ConnectorType> Connections => GetItemType<IConnectable>()?.Connections.ToArray() ?? [];
    public IEnumerable<Tuple<ConnectorType, IConnectable>> ConnectedItems => GetItemType<IConnectable>()?.ConnectedItems ?? [];
    public IEnumerable<ConnectorType> FreeConnections => GetItemType<IConnectable>()?.FreeConnections ?? [];

    public bool DesignedForOffhandUse => _components.Any(x => x.DesignedForOffhandUse);

    public bool SwapInPlace(IGameItem existingItem, IGameItem newItem)
    {
        return Components.Any(x => x.SwapInPlace(existingItem, newItem));
    }

    public void Take(IGameItem item)
    {
        foreach (IGameItemComponent component in Components)
        {
            if (component.Take(item))
            {
                item.ContainedIn = null;
                break;
            }
        }
    }

    public IGameItemGroup ItemGroup => Prototype.ItemGroup;

    private IGameItem _containedIn;
	private long _containmentMutationVersion;

    public IGameItem ContainedIn
    {
        get => _containedIn;
        set => TrySetContainedIn(value);
    }

    // The callback is a raw owner-field commit only: no progs, output or public actions.
    // Public setters and completion retain ordinary behavior unless a caller supplies a checkpoint.
    internal bool TrySetContainedIn(IGameItem value, Func<bool> mayCommit = null, Action ownerCommit = null)
    {
			ForeignCustodyTransferContext.EnsureItem(this);
			if (value is not null) ForeignCustodyTransferContext.EnsurePair(value, this);
			using var exposureChange = EnvironmentalExposureService.Changing(this);
			if (ReferenceEquals(_containedIn, value))
			{
				return false;
			}

			var version = ++_containmentMutationVersion;
			var originalContainer = _containedIn;
			var originalBody = InInventoryOf;
			var originalRoom = base.Location;
			var originalBelt = GetItemType<IBeltable>()?.ConnectedTo;
			bool OriginalCustody() => _containmentMutationVersion == version && !Deleted && !Destroyed &&
				ReferenceEquals(_containedIn, originalContainer) && ReferenceEquals(InInventoryOf, originalBody) &&
				ReferenceEquals(base.Location, originalRoom) && ReferenceEquals(GetItemType<IBeltable>()?.ConnectedTo, originalBelt);
			var timeSensitiveItems = DeepItems.ToList();
			if (!OriginalCustody()) return false;
			foreach (var item in timeSensitiveItems)
			{
				item.RebaseItemTimeRates();
				if (!OriginalCustody()) return false;
			}

			using var proximityChange = Gameworld?.ProximityEventService?.BeginChange(ProximityChangeCause.Containment, this);
			if (!OriginalCustody()) return false;
            if (mayCommit is not null && !mayCommit()) return false;
            if (!OriginalCustody()) return false;
            _containedIn = value;
            Changed = true;
            ownerCommit?.Invoke();
			proximityChange?.Complete();

			foreach (var item in timeSensitiveItems)
			{
				if (_containmentMutationVersion != version || Deleted || Destroyed || !ReferenceEquals(_containedIn, value)) break;
				item.RebaseItemTimeRates();
			}
        return true;
    }

    /// <summary>
    /// This returns an IPerceivable (which may be this item) that represents the perceivable "thing" that is actually in the room, for purposes of working out proximity of this item irrespestive of whether it is sitting in the room, being carried, in a container, attached to something etc.
    /// </summary>
	public IPerceivable LocationLevelPerceivable => SpatialHost?.LocationLevelPerceivable ??
                   ContainedIn?.LocationLevelPerceivable ??
                   InInventoryOf?.Actor ??
	               (EffectSpatialHost is not null ? this : null) ??
                   GetItemType<IChair>()?.Table?.Parent.LocationLevelPerceivable ??
                   (GetItemType<IDoor>()?.InstalledExit != null ? this : null) ??
                   GetItemType<IBeltable>()?.ConnectedTo?.Parent.LocationLevelPerceivable ??
                   (GetItemType<IConnectable>() is IConnectable conn && !conn.Independent
                       ? conn.ConnectedItems.FirstOrDefault(x => x.Item2.Independent)?.Item2.Parent
                       : null) ??
                   this;

    public void LoadTimeSetContainedIn(IGameItem item)
    {
        _containedIn = item;
    }

    public IBody InInventoryOf =>
        GetItemType<IHoldable>()?.HeldBy ??
        GetItemType<IWearable>()?.WornBy ??
        GetItemType<IProsthetic>()?.InstalledBody ??
        GetItemType<IImplant>()?.InstalledBody ??
        ContainedIn?.InInventoryOf;


    public bool IsInInventory(IBody body)
    {
        return (ContainedIn?.IsInInventory(body) ?? false) ||
               InInventoryOf == body;
    }

    private ISolid _overrideMaterial;

    public ISolid Material
    {
        get => Components.FirstOrDefault(x => x.OverridesMaterial)?.OverridenMaterial ??
                   _overrideMaterial ?? Prototype.Material;
        set
        {
            _overrideMaterial = value;
            Changed = true;
        }
    }

    public bool CanBeBundled
    {
        get
        {
            if (IsItemType<PileGameItemComponent>())
            {
                return false;
            }

            if (!IsItemType<IHoldable>())
            {
                return false;
            }

            return true;
        }
    }

    public double Buoyancy(double fluidDensity)
    {
        if (Material == null)
        {
            return 1.0;
        }

        return (fluidDensity - Material.Density) * Prototype.Weight +
               _components.Sum(x => x.ComponentBuoyancy(fluidDensity));
    }

    public event ConnectedEvent OnConnected;
    public event ConnectedEvent OnDisconnected;

    public void ConnectedItem(IConnectable other, ConnectorType type)
    {
        OnConnected?.Invoke(other, type);
    }

    public void DisconnectedItem(IConnectable other, ConnectorType type)
    {
        OnDisconnected?.Invoke(other, type);
    }

    #region Overrides of PerceivedItem

    public override IRoom Location
    {
		get => SpatialHost?.Location ?? EffectSpatialHostLocation ?? base.Location ?? TrueLocations?.FirstOrDefault();
        protected set => base.Location = value;
    }

	// Custody checks must distinguish a spatial record from location inherited through a holder/container.
	internal IRoom DirectLocation => base.Location;

	public override RoomLayer RoomLayer
	{
		get => SpatialHost?.RoomLayer ?? EffectSpatialHostProvider?.SpatialLayer ?? base.RoomLayer;
		set => base.RoomLayer = value;
	}

	public override double? RoutePositionMetres => SpatialHost?.RoutePositionMetres ??
	                                              EffectSpatialHostProvider?.SpatialRoutePositionMetres ??
	                                              base.RoutePositionMetres;

    #endregion

    public IEnumerable<IRoom> TrueLocationsExcept(List<IGameItem> itemsConsidered)
    {
		var spatialHost = SpatialHost;
		if (spatialHost is not null && !itemsConsidered.Contains(spatialHost))
		{
			itemsConsidered.Add(spatialHost);
			return spatialHost.TrueLocationsExcept(itemsConsidered);
		}
		var effectSpatialHost = EffectSpatialHost;
		if (effectSpatialHost is IGameItem effectHostItem && !itemsConsidered.Contains(effectHostItem))
		{
			itemsConsidered.Add(effectHostItem);
			return effectHostItem.TrueLocationsExcept(itemsConsidered);
		}
		if (effectSpatialHost is IRoom effectHostRoom)
		{
			return [effectHostRoom];
		}
		if (effectSpatialHost?.Location is { } effectHostLocation)
		{
			return [effectHostLocation];
		}

        if (base.Location != null)
        {
            return new[] { Location };
        }

        IChair chair = GetItemType<IChair>();
        if (chair?.Table?.Parent.Location != null)
        {
            return chair.Table.Parent.TrueLocations;
        }

        IBeltable beltable = GetItemType<IBeltable>();
        if (beltable?.ConnectedTo != null)
        {
            return beltable.ConnectedTo.Parent.TrueLocations;
        }

        IAutomationMountable mountable = GetItemType<IAutomationMountable>();
        if (mountable?.MountHost != null && !itemsConsidered.Contains(mountable.MountHost.Parent))
        {
            itemsConsidered.Add(mountable.MountHost.Parent);
            List<IRoom> location = mountable.MountHost.Parent.TrueLocationsExcept(itemsConsidered).ToList();
            if (location.Any())
            {
                return location;
            }
        }

        IConnectable connectable = GetItemType<IConnectable>();
        if (connectable?.ConnectedItems.Any() ?? false)
        {
            foreach (Tuple<ConnectorType, IConnectable> item in connectable.ConnectedItems.Where(x => !itemsConsidered.Contains(x.Item2.Parent)))
            {
                itemsConsidered.Add(item.Item2.Parent);
                List<IRoom> location = item.Item2.Parent.TrueLocationsExcept(itemsConsidered).ToList();
                if (location.Any())
                {
                    return location;
                }
            }
        }

        IDoor door = GetItemType<IDoor>();
        if (door?.InstalledExit != null)
        {
            return door.InstalledExit.Rooms;
        }

        if (InInventoryOf?.Location != null)
        {
            return new[] { InInventoryOf.Location };
        }

        return ContainedIn?.TrueLocations ?? Enumerable.Empty<IRoom>();
    }

    public IEnumerable<IRoom> TrueLocations
    {
        get
        {
			var spatialHost = SpatialHost;
			if (spatialHost is not null)
			{
				return spatialHost.TrueLocationsExcept([this, spatialHost]);
			}
			var effectSpatialHost = EffectSpatialHost;
			if (effectSpatialHost is IGameItem effectHostItem)
			{
				return effectHostItem.TrueLocationsExcept([this, effectHostItem]);
			}
			if (effectSpatialHost is IRoom effectHostRoom)
			{
				return [effectHostRoom];
			}
			if (effectSpatialHost?.Location is { } effectHostLocation)
			{
				return [effectHostLocation];
			}

            if (base.Location != null)
            {
                return new[] { Location };
            }

            IChair chair = GetItemType<IChair>();
            if (chair?.Table?.Parent.Location != null)
            {
                return chair.Table.Parent.TrueLocations;
            }

            IBeltable beltable = GetItemType<IBeltable>();
            if (beltable?.ConnectedTo != null)
            {
                return beltable.ConnectedTo.Parent.TrueLocations;
            }

            IAutomationMountable mountable = GetItemType<IAutomationMountable>();
            if (mountable?.MountHost != null)
            {
                List<IRoom> location = mountable.MountHost.Parent.TrueLocationsExcept(new List<IGameItem> { this }).ToList();
                if (location.Any())
                {
                    return location;
                }
            }

            IConnectable connectable = GetItemType<IConnectable>();
            if (connectable?.ConnectedItems.Any() ?? false)
            {
                List<IRoom> location = connectable.Parent.TrueLocationsExcept(new List<IGameItem> { this }).ToList();
                if (location.Any())
                {
                    return location;
                }
            }

            IImplant implant = GetItemType<IImplant>();
            if (implant?.InstalledBody != null)
            {
                return new[] { implant.InstalledBody.Location };
            }

            IDoor door = GetItemType<IDoor>();
            if (door?.InstalledExit != null)
            {
                return door.InstalledExit.Rooms;
            }

            if (InInventoryOf?.Location != null)
            {
                return new[] { InInventoryOf.Location };
            }

            return ContainedIn?.TrueLocations ?? Enumerable.Empty<IRoom>();
        }
    }

    public void Handle(string text, OutputRange range = OutputRange.Personal)
    {
        switch (range)
        {
            case OutputRange.Local:
                foreach (IRoom location in TrueLocations)
                {
					location.HandleLocal(this, RoomLayer, text);
                }

                break;
            case OutputRange.Room:
                foreach (IRoom location in TrueLocations)
                {
                    location.Handle(text);
                }

                break;
            case OutputRange.Shard:
                foreach (IRoom location in TrueLocations)
                {
                    location.Shard.Handle(text);
                }

                break;
            case OutputRange.Zone:
                foreach (IRoom location in TrueLocations)
                {
                    location.Zone.Handle(text);
                }

                break;
            case OutputRange.Surrounds:
                foreach (IRoom location in TrueLocations.SelectMany(x => x.Surrounds).Except(TrueLocations))
                {
                    location.Handle(text);
                }

                break;
            default:
                OutputHandler.Handle(text, range);
                break;
        }
    }

    public void Handle(IOutput output, OutputRange range = OutputRange.Personal)
    {
        switch (range)
        {
            case OutputRange.Local:
                foreach (IRoom location in TrueLocations)
                {
					location.HandleLocal(this, RoomLayer, output);
                }

                break;
            case OutputRange.Room:
                foreach (IRoom location in TrueLocations)
                {
                    location.Handle(output);
                }

                break;
            case OutputRange.Shard:
                foreach (IRoom location in TrueLocations)
                {
                    location.OwningZone.Shard.Handle(output);
                }

                break;
            case OutputRange.Zone:
                foreach (IRoom location in TrueLocations)
                {
                    location.OwningZone.Handle(output);
                }

                break;
            case OutputRange.Surrounds:
                foreach (IRoom location in TrueLocations.SelectMany(x => x.Surrounds).Except(TrueLocations))
                {
                    location.Handle(output);
                }

                break;
            default:
                OutputHandler.Handle(output, range);
                break;
        }
    }

    public bool Deleted { get; private set; }
	internal override Action CaptureCustodySaveRollback()
	{
		var restoreShared = base.CaptureCustodySaveRollback();
		var resources = _resourcesChanged; var liquid = _surfaceLiquidChanged;
		return () =>
		{
			_resourcesChanged |= resources; _surfaceLiquidChanged |= liquid;
			restoreShared();
		};
	}

	internal Action CaptureCustodyRollback()
	{
		var contained = _containedIn; var location = base.Location; var restorePosition = CaptureCustodyPositionRollback();
		return () =>
		{
			_containedIn = contained; base.Location = location; restorePosition(); Changed = true;
		};
	}
	private bool _deletionObserversNotified;
	private bool _notifyingDeletionObservers;
	private Exception _deletionObserverFailure;


    public void Delete()
    {
		DeleteCore(null);
    }

	internal void DeleteCommittedEmptyCurrency(Func<bool> unchangedAndEmpty, Action detachCapturedMembership)
	{
		if (GetItemType<CurrencyGameItemComponent>() is null) throw new InvalidOperationException("Currency cleanup requires a native currency pile.");
		DeleteCore(() =>
		{
			if (!unchangedAndEmpty()) return false;
			detachCapturedMembership();
			return true;
		}, unchangedAndEmpty);
	}

	private void DeleteCore(Func<bool>? beforeNativeRemoval, Func<bool>? mayNotify = null, bool persistedAlready = false)
    {
		ForeignCustodyTransferContext.EnsureItem(this, destructive: true);
          if (Deleted || _notifyingDeletionObservers) return;
		if (mayNotify is not null && !mayNotify()) return;
		if (GetItemType<ICorpse>() is not null && Gameworld.SpellOwnedCorpseAnimations?.IsBorrowedCorpse(Id) == true) return;
		if (SpellCreationOrigin?.IsTemporary == true && Gameworld.SpellOwnedItems?.TryPrepareRemoval(this, out _) != true) return;
        if (_deletionObserverFailure is not null)
            throw new InvalidOperationException("Deletion observers failed; native removal remains held.", _deletionObserverFailure);
        if (GetItemType<ICorpse>() is not null && Gameworld.SpellOwnedNpcs?.TryPrepareRemainsRemoval(this, out _) == false) return;
        // Deletion callbacks can change possessions. Recheck before invoking components
        // whose native deletion would recursively destroy an owned corpse's inventory.
        if (!_deletionObserversNotified)
        {
            _notifyingDeletionObservers = true;
            try
            {
                if (GetItemType<ICorpse>() is not null && Gameworld.SpellOwnedNpcs is { } ownedNpcs)
                {
                    if (!ownedNpcs.TryNotifyRemainsDeletion(this, NotifyDeletionObservers)) return;
                }
                else NotifyDeletionObservers();
                _deletionObserversNotified = true;
            }
            catch (Exception ex) { _deletionObserversNotified = true; _deletionObserverFailure = ex; throw; }
            finally { _notifyingDeletionObservers = false; }
        }
        if (GetItemType<ICorpse>() is not null && Gameworld.SpellOwnedNpcs?.TryPrepareRemainsRemoval(this, out _) == false) return;
        if (SpellCreationOrigin?.IsTemporary == true && Gameworld.SpellOwnedItems?.TryPrepareRemoval(this, out _) != true) return;
		if (GetItemType<ICorpse>() is not null && Gameworld.SpellOwnedCorpseAnimations?.IsBorrowedCorpse(Id) == true) return;
        if (SpellCreationOrigin?.IsTemporary == true) { DeleteSpellOwnedItem(); return; }
		if (beforeNativeRemoval is not null && !beforeNativeRemoval()) return;
          DeleteNative(persistedAlready);
    }

    private void DeleteNative(bool persistedAlready = false)
    {
        ReleaseEvents();
        if (GetItemType<ICorpse>()?.OriginalBody?.Actor.State.HasFlag(CharacterState.Dead) == true) EndHealthTick();
        Changed = false;
        _noSave = true;
        Gameworld.SaveManager.Abort(this);
        foreach (IGameItemComponent component in Components)
        {
            Gameworld.SaveManager.Abort(component);
        }

        InvalidatePositionTargets();
        SoftReleasePositionTarget();
        ContainedIn?.Take(this);
        ContainedIn = null;
        if (InInventoryOf is MudSharp.Body.Implementations.Body nativeBody) nativeBody.TakeForNativeDeletion(this);
		else InInventoryOf?.Take(this);
        Location?.Extract(this);
        Get(null);

        foreach (IGameItemComponent component in Components)
        {
            component.Delete();
        }

        Gameworld.EffectScheduler.Destroy(this);
        Gameworld.Scheduler.Destroy(this);
        EffectHandler.RemoveAllEffects();
        Gameworld.Destroy(this);
        EndMorphTimer();

        foreach (IWound wound in Wounds.ToList())
        {
            wound.Delete();
        }

        if (_id != 0 && !persistedAlready)
        {
            using (new FMDB())
            {
                Gameworld.SaveManager.Flush();
                var towLinks = FMDB.Context.VehicleTowLinks
                                      .Where(x => x.HitchItemId == Id && !x.IsDisabled)
                                      .ToList();
                foreach (var link in towLinks)
                {
                    link.IsDisabled = true;
                }

                var hitchLinks = FMDB.Context.VehicleHitchLinks
                                      .Where(x => x.HitchItemId == Id && !x.IsDisabled)
                                      .ToList();
                foreach (var link in hitchLinks)
                {
                    link.IsDisabled = true;
                }

                Models.GameItem dbitem = FMDB.Context.GameItems.Find(Id);
                if (dbitem != null)
                {
                    FMDB.Context.GameItems.Remove(dbitem);
                    FMDB.Context.SaveChanges();
                }
            }
        }

        Deleted = true;
		Gameworld.SpellOwnedItems?.ObserveRemoval(this);
    }

    protected override void ReleaseEvents()
    {
        base.ReleaseEvents();
        OnConnected = null;
        OnDisconnected = null;
        OnHeal = null;
        OnRemovedFromLocation = null;
        OnRemoveWound = null;
        OnWounded = null;
    }

    public void Login()
    {
        foreach (IGameItemComponent component in Components)
        {
            component.Login();
        }

        if (CachedMorphTime is not null)
        {
            StartMorphTimer();
        }

        // Effects without a duration are already on the item rather than the cache
        foreach (IEffect effect in Effects)
        {
            effect.Login();
        }

        // Scheduled effects will call Login() when they become scheduled
        ScheduleCachedEffects();

		if (_overridingWoundBehaviourComponent is null && Wounds.Any() && !HealthStrategy.RequiresPeriodicHealthTick)
		{
			CheckHealthStatus();
		}
    }

    public void Quit()
    {
		ForeignCustodyTransferContext.EnsureItem(this, destructive: true);
		EndHealthTick();
        EffectsChanged = true;
        if (Changed || Components.Any(x => x.Changed))
        {
            Gameworld.SaveManager.Flush();
        }

        InvalidatePositionTargets();
        SoftReleasePositionTarget();
        PerceivableQuit();

        Location?.Extract(this);

        Drop(null);

        CacheScheduledEffects();
        foreach (IGameItemComponent component in Components)
        {
            component.Quit();
        }

        Gameworld.EffectScheduler.Destroy(this);
        Gameworld.Destroy(this);
        EndMorphTimer();
    }

    public bool IsItemType<T>() where T : IGameItemComponent
    {
        return _components.OfType<T>().Any();
    }

    public T GetItemType<T>() where T : IGameItemComponent
    {
        if (typeof(T) == typeof(IContainer))
        {
            T vehicleCargo = _components.OfType<T>().FirstOrDefault(x => x is IVehicleCargoSpaceItem);
            if (vehicleCargo != null)
            {
                return vehicleCargo;
            }
        }

        if (typeof(T) == typeof(IOpenable) || typeof(T) == typeof(ILockable))
        {
            T vehicleAccess = _components.OfType<T>().FirstOrDefault(x => x is IVehicleAccessPointItem);
            if (vehicleAccess != null)
            {
                return vehicleAccess;
            }
        }

        return _components.OfType<T>().FirstOrDefault();
    }

    public IEnumerable<T> GetItemTypes<T>() where T : IGameItemComponent
    {
        return _components.OfType<T>();
    }

    private ItemQuality _quality;

    public ItemQuality Quality
    {
        get => (Skin?.Quality ?? _quality)
                .StageUp(
                    _components
                        .OfType<IAffectQuality>()
                        .Select(x => x.ItemQualityStages)
                        .DefaultIfEmpty(0)
                        .Sum()
                );
        set
        {
            _quality = value;
            Changed = true;
        }
    }

    public ItemQuality RawQuality => _quality;

    public IGameItemProto Prototype { get; protected set; }
    public PlanarPresenceDefinition BasePlanarPresence => Prototype.BasePlanarPresence;

    /// <summary>
    /// Creates a new item that is a copy of this item, including similar copies of all contained items
    /// </summary>
    /// <returns></returns>
    public IGameItem DeepCopy(bool addToGameworld, bool preserveMorphTime)
    {
        GameItem newItem = new(this, !addToGameworld, preserveMorphTime);
        foreach (IContainer component in Components.OfType<IContainer>())
        {
            IContainer newComponent = (IContainer)newItem.Components.First(x => x.Prototype == component.Prototype);
            foreach (IGameItem item in component.Contents)
            {
                IGameItem newContent = item.DeepCopy(addToGameworld, preserveMorphTime);
                newComponent.Put(null, newContent, false);
            }
        }

        return newItem;
    }

    public double DamageCondition
    {
        get
        {
            IDestroyable destroyable = GetItemType<IDestroyable>();
            if (destroyable == null)
            {
                return 1.0;
            }

            return destroyable.MaximumDamage == 0.0
                ? 1.0
                : (1.0 - (_wounds.Sum(x => x.CurrentDamage) / destroyable.MaximumDamage));
        }
    }

    public string Evaluate(ICharacter actor)
    {
        StringBuilder sb = new();
        sb.AppendLine($"You evaluate {HowSeen(actor)}:");
        sb.AppendLine();
        if (actor.IsAdministrator() && _skinId is not null)
        {
            sb.AppendLine($"{Skin.EditHeader().Colour(Telnet.Cyan)} skin applied.");
        }

        sb.AppendLine($"Its quality is {Quality.Describe().ColourValue()}.");
        sb.AppendLine($"It weighs {actor.Gameworld.UnitManager.Describe(Weight, UnitType.Mass, actor).ColourValue()}.");
        sb.AppendLine($"It is {Size.Describe().Colour(Telnet.Green)} in size.");
        sb.AppendLine($"It is made primarily out of {Material?.MaterialDescription.Colour(Telnet.Green) ?? "an unknown material".Colour(Telnet.Red)}.");
        sb.AppendLine($"It is at {Condition.ToString("P0", actor).ColourValue()} condition.");
        double percentage = 1.0 - DamageCondition;
        ANSIColour colour = Telnet.Green;

        if (percentage >= 0.95)
        {
            colour = Telnet.BoldMagenta;
        }
        else if (percentage >= 0.85)
        {
            colour = Telnet.BoldRed;
        }
        else if (percentage >= 0.7)
        {
            colour = Telnet.Red;
        }
        else if (percentage >= 0.55)
        {
            colour = Telnet.Orange;
        }
        else if (percentage >= 0.4)
        {
            colour = Telnet.Yellow;
        }
        else if (percentage >= 0.25)
        {
            colour = Telnet.BoldYellow;
        }
        else if (percentage >= 0.1)
        {
            colour = Telnet.BoldGreen;
        }

        sb.AppendLine($"It is {percentage.ToString("P0", actor).Colour(colour)} damaged.");
        if (actor.Currency is not null)
        {
            ITraitDefinition td = actor.Gameworld.Traits.Get(actor.Gameworld.GetStaticLong("AppraiseCommandSkill"));
            if (actor.IsAdministrator() ||
                !actor.Gameworld.GetStaticBool("AppraiseCommandRequiresSkill") ||
                (td is not null &&
                actor.TraitValue(td) > 0.0))
            {
                decimal fuzzinessFloor = 1.0M;
                decimal fuzzinessCeiling = 1.0M;
                if (td is not null && !actor.IsAdministrator())
                {
                    ICheck check = actor.Gameworld.GetCheck(CheckType.AppraiseItemCheck);
                    Difficulty difficulty = Difficulty.Easy;
                    CheckOutcome result = check.Check(actor, difficulty, td, this);
                    decimal skew = 0.0M;
                    switch (result.Outcome)
                    {
                        case Outcome.MajorFail:
                            skew = (decimal)RandomUtilities.DoubleRandom(-0.5, 0.5);
                            fuzzinessCeiling = 1.5M + skew;
                            fuzzinessFloor = 0.5M + skew;
                            break;
                        case Outcome.Fail:
                            skew = (decimal)RandomUtilities.DoubleRandom(-0.3, 0.3);
                            fuzzinessCeiling = 1.3M + skew;
                            fuzzinessFloor = 0.7M + skew;
                            break;
                        case Outcome.MinorFail:
                            skew = (decimal)RandomUtilities.DoubleRandom(-0.2, 0.2);
                            fuzzinessCeiling = 1.2M + skew;
                            fuzzinessFloor = 0.8M + skew;
                            break;
                        case Outcome.MinorPass:
                            skew = (decimal)RandomUtilities.DoubleRandom(-0.1, 0.1);
                            fuzzinessCeiling = 1.1M + skew;
                            fuzzinessFloor = 0.9M + skew;
                            break;
                        case Outcome.Pass:
                            skew = (decimal)RandomUtilities.DoubleRandom(-0.05, 0.05);
                            fuzzinessCeiling = 1.05M + skew;
                            fuzzinessFloor = 0.95M + skew;
                            break;
                        case Outcome.MajorPass:
                            break;
                    }
                }

                (decimal minimum, decimal maximum) CalculateMinimumMaximum(IGameItem item)
                {
                    if (item.GetItemType<ICurrencyPile>() is { } cp)
                    {
                        return (
                            cp.TotalValue * cp.Currency.BaseCurrencyToGlobalBaseCurrencyConversion * fuzzinessFloor / actor.Currency.BaseCurrencyToGlobalBaseCurrencyConversion,
                            cp.TotalValue * cp.Currency.BaseCurrencyToGlobalBaseCurrencyConversion * fuzzinessCeiling / actor.Currency.BaseCurrencyToGlobalBaseCurrencyConversion);
                    }
                    return (item.Prototype.CostInBaseCurrency * fuzzinessFloor / actor.Currency.BaseCurrencyToGlobalBaseCurrencyConversion,
                            item.Prototype.CostInBaseCurrency * fuzzinessCeiling / actor.Currency.BaseCurrencyToGlobalBaseCurrencyConversion);
                }

                string DescribeCurrencyRange(decimal minimum, decimal maximum)
                {
                    if (minimum == maximum)
                    {
                        return actor.Currency.Describe(minimum, CurrencyDescriptionPatternType.ShortDecimal).ColourValue();
                    }

                    return $"{actor.Currency.Describe(minimum, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()} to {actor.Currency.Describe(maximum, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}";
                }

                void EvaluateItem(IGameItem item, List<(string ItemDescription, string ValueDescription, int Levels)> list,
                    ref decimal minTotal, ref decimal maxTotal, int level, bool includeContents)
                {
                    (decimal min, decimal max) = CalculateMinimumMaximum(item);
                    minTotal += min;
                    maxTotal += max;
                    list.Add((item.HowSeen(actor), DescribeCurrencyRange(min, max), level));
                    if (includeContents &&
                        item.GetItemType<IContainer>() is { } container &&
                        (actor.IsAdministrator() ||
                         container.Transparent ||
                         (container is IOpenable op && op.IsOpen)
                        )
                       )
                    {
                        foreach (IGameItem content in container.Contents)
                        {
                            EvaluateItem(content, list, ref minTotal, ref maxTotal, level + 1, true);
                        }
                    }
                }

                List<(string ItemDescription, string ValueDescription, int Levels)> results = new();
                decimal minTotal = 0.0M;
                decimal maxTotal = 0.0M;
                EvaluateItem(this, results, ref minTotal, ref maxTotal, 0, false);
                sb.AppendLine($"Estimated Value: {results[0].ValueDescription}");
            }
        }



        List<ITag> tags = Tags.Where(x => x.ShouldSee(actor)).ToList();
        if (tags.Any())
        {
            sb.AppendLine($"It is tagged as {tags.Select(x => x.Name.Colour(Telnet.Cyan)).ListToString()}");
        }

        foreach (IGameItemComponent component in Components.Where(x => x.DescriptionDecorator(DescriptionType.Evaluate)))
        {
            sb.AppendLine(component.Decorate(actor, Name, "", DescriptionType.Evaluate, true,
                PerceiveIgnoreFlags.None));
        }

		foreach (var effect in EffectsOfType<IEvaluateDescriptionAdditionEffect>()
		         .Where(x => x.DescriptionAdditionApplies(actor)))
		{
			sb.AppendLine(effect.GetAdditionalText(actor, true));
		}

        return sb.ToString();
    }

    public double Weight
    {
        get
        {
#if DEBUG
            double weight = (Prototype.Weight + _components.Sum(x => x.ComponentWeight) + SurfaceLiquidState.AddedWeight +
                          EffectsOfType<IEffectAddsWeight>().Sum(x => x.AddedWeight)) *
                         _components.Aggregate(1.0, (a, b) => a * b.ComponentWeightMultiplier);
#endif
            return (Prototype.Weight + _components.Sum(x => x.ComponentWeight) + SurfaceLiquidState.AddedWeight +
                    EffectsOfType<IEffectAddsWeight>().Sum(x => x.AddedWeight)) *
                   _components.Aggregate(1.0, (a, b) => a * b.ComponentWeightMultiplier);
        }
        set
        {
            // Do nothing
        }
    }

    public override SizeCategory Size => Prototype.Size;

    public bool CanMerge(IGameItem otherItem)
    {
		if (SpellCreationOrigin?.IsTemporary == true || otherItem.SpellCreationOrigin?.IsTemporary == true) return false;
        if (Deleted || Destroyed || otherItem.Deleted || otherItem.Destroyed ||
			GetItemType<IStackable>() is { Quantity: <= 0 } || otherItem.GetItemType<IStackable>() is { Quantity: <= 0 })
        {
            return false;
        }

        if (otherItem.Prototype != Prototype)
        {
            return false;
        }

        if (OverrideSdesc != otherItem.OverrideSdesc || OverrideDesc != otherItem.OverrideDesc)
        {
            return false;
        }

        if (!HasSameOwnerAs(otherItem))
        {
            return false;
        }

        if (!otherItem.IsItemType<IStackable>() && !otherItem.IsItemType<ICurrencyPile>() && !otherItem.IsItemType<ICommodity>())
        {
            // We don't need to check this item against these criteria because they are both the same prototype, so have the same component types
            return false;
        }

        if (Effects.Any(x => x.PreventsItemFromMerging(this, otherItem)))
        {
            return false;
        }

        if (otherItem.Effects.Any(x => x.PreventsItemFromMerging(otherItem, this)))
        {
            return false;
        }

        foreach (IGameItemComponent component in _components)
        {
            if (otherItem.Components.Any(x => component.PreventsMerging(x)))
            {
                return false;
            }
        }

        return true;
    }

    public void Merge(IGameItem otherItem)
    {
		SpellOwnedItemValuePolicy.RequireOrdinaryValue(this, "merging");
		SpellOwnedItemValuePolicy.RequireOrdinaryValue(otherItem, "merging");
		ForeignCustodyTransferContext.EnsureItem(this, destructive: true);
		ForeignCustodyTransferContext.EnsureItem(otherItem, destructive: true);
        IStackable thisStackable = GetItemType<IStackable>();
        IStackable thatStackable = otherItem.GetItemType<IStackable>();

        if (thisStackable != null && thatStackable != null)
        {
			MudSharp.Magic.PsychometricRecorder.MergeHistory(this, otherItem);
            thisStackable.Quantity += thatStackable.Quantity;
            NotifyStockItemMerge(otherItem);
            return;
        }

        ICurrencyPile thisCurrency = GetItemType<ICurrencyPile>();
        ICurrencyPile thatCurrency = otherItem.GetItemType<ICurrencyPile>();
        if (thisCurrency != null && thatCurrency != null && thisCurrency.Currency == thatCurrency.Currency)
        {
			MudSharp.Magic.PsychometricRecorder.MergeHistory(this, otherItem);
            thisCurrency.AddCoins(thatCurrency.Coins);
            NotifyStockItemMerge(otherItem);
            return;
        }

        ICommodity thisCommodity = GetItemType<ICommodity>();
        ICommodity thatCommodity = otherItem.GetItemType<ICommodity>();
        if (thisCommodity != null && thatCommodity != null && CommodityCharacteristicRequirement.CommodityIdentityEqual(thisCommodity, thatCommodity))
        {
			MudSharp.Magic.PsychometricRecorder.MergeHistory(this, otherItem);
            thisCommodity.Weight += thatCommodity.Weight;
            thisCommodity.MergeSpoilageFrom(thatCommodity);
            NotifyStockItemMerge(otherItem);
            return;
        }
        // TODO - anything else that might occur on merging?
    }

	internal void NotifyCommittedCurrencyMerge(GameItem absorbed)
	{
		if (GetItemType<CurrencyGameItemComponent>() is null || absorbed.GetItemType<CurrencyGameItemComponent>() is null)
			throw new InvalidOperationException("Currency merge notification requires its exact native participants.");
		MudSharp.Magic.PsychometricRecorder.MergeHistory(this, absorbed);
		NotifyStockItemMerge(absorbed);
	}

    private void NotifyStockItemMerge(IGameItem absorbed)
    {
        foreach (var display in absorbed.EffectsOfType<ItemOnDisplayInShop>())
        {
            if (AffectedBy<ItemOnDisplayInShop>(display.Merchandise))
            {
                display.Shop.RegisterStockItemMerge(this, absorbed);
            }
        }
    }

    public IEnumerable<IGameItemComponent> Components => _components;

    /// <summary>
    /// A collection of all items, including the item itself, that are contained within this item. This will recursively go down as many layers as it has to.
    /// </summary>
    public IEnumerable<IGameItem> DeepItems
    {
        get
        {
            List<IGameItem> items = new() { this };

            foreach (IContainer component in _components.OfType<IContainer>())
            {
                items.AddRange(component.Contents.SelectMany(x => x.DeepItems));
            }

            return items;
        }
    }

    public IEnumerable<IGameItem> ShallowItems
    {
        get
        {
            List<IGameItem> items = new() { this };

            foreach (IContainer component in _components.OfType<IContainer>())
            {
                items.AddRange(component.Contents);
            }

            return items;
        }
    }

    public IEnumerable<IGameItem> ShallowAccessibleItems(ICharacter potentialGetter)
    {
        List<IGameItem> items = new() { this };
        if (_components.OfType<IOpenable>().Any(x => !x.IsOpen && !x.CanOpen(potentialGetter.Body)))
        {
            return items;
        }

        foreach (IContainer component in _components.OfType<IContainer>())
        {
            items.AddRange(component.Contents);
        }

        return items;
    }

    #endregion

    #region IGameItem Inventory-Related Members

    public ItemGetResponse CanGet(ItemCanGetIgnore ignoreFlags = ItemCanGetIgnore.None)
    {
		if (Deleted || Destroyed || GetItemType<IStackable>() is { Quantity: <= 0 })
		{
			return ItemGetResponse.Unpositionable;
		}

        if (!IsItemType<IHoldable>() || !GetItemType<IHoldable>().IsHoldable)
        {
            return ItemGetResponse.NotIHoldable;
        }

        if (Components.Any(x => x.PreventsRepositioning()))
        {
            return ItemGetResponse.Unpositionable;
        }

        foreach (INoGetEffect effect in EffectHandler.EffectsOfType<INoGetEffect>())
        {
            if (!ignoreFlags.HasFlag(ItemCanGetIgnore.IgnoreCombat) && effect.CombatRelated)
            {
                return ItemGetResponse.NoGetEffectCombat;
            }

            if (!ignoreFlags.HasFlag(ItemCanGetIgnore.IgnoreInventoryPlans) && effect is IInventoryPlanItemEffect)
            {
                return ItemGetResponse.NoGetEffectPlan;
            }

            return ItemGetResponse.NoGetEffect;
        }

        return ItemGetResponse.CanGet;
    }

    public ItemGetResponse CanGet(int quantity, ItemCanGetIgnore ignoreFlags = ItemCanGetIgnore.None)
    {
        ItemGetResponse canGet = CanGet(ignoreFlags);
        if (canGet != ItemGetResponse.CanGet)
        {
            return canGet;
        }

        IStackable stackable = GetItemType<IStackable>();
        if (stackable == null)
        {
            return ItemGetResponse.CanGet;
        }

        return quantity == 0 ? ItemGetResponse.CanGet : stackable.CanGet(quantity);
    }

    public IGameItem Get(IBody getter)
    {
		ForeignCustodyTransferContext.EnsureItem(this);
		if (getter is not null) ForeignCustodyTransferContext.EnsureBody(getter, this);
		using var proximityChange = Gameworld?.ProximityEventService?.BeginChange(ProximityChangeCause.Containment, this);
        IHoldable holdable = GetItemType<IHoldable>();
        var sourceLocation = base.Location;
        var sourceContainer = ContainedIn;
        holdable?.HeldBy = getter;

        bool PreserveCallbackCustody()
        {
            if (!Deleted && !Destroyed && ReferenceEquals(base.Location, sourceLocation) &&
                ReferenceEquals(ContainedIn, sourceContainer) && (holdable is null || ReferenceEquals(holdable.HeldBy, getter))) return false;
            if (ReferenceEquals(base.Location, sourceLocation) && sourceLocation?.GameItems.Contains(this) != true) Location = null;
            if (holdable is not null && ReferenceEquals(holdable.HeldBy, getter)) holdable.HeldBy = null;
            proximityChange?.Complete();
            return true;
        }

        if (sourceLocation != null)
        {
            // Detach the captured source before callbacks can establish new spatial custody.
            // Keep Location available to removal listeners until they have run.
            sourceLocation.Extract(this);
            OnRemovedFromLocation?.Invoke(this);
        }

        if (PreserveCallbackCustody()) return this;
		ClearRoutePositionForDetachment();
        Location = null;
        sourceLocation = null;
        InvalidatePositionTargets();
        if (PreserveCallbackCustody()) return this;
        EffectHandler.RemoveAllEffects(x => x.IsEffectType<IRemoveOnGet>(), true);
        if (PreserveCallbackCustody()) return this;
        PositionState = PositionUndefined.Instance;
        PositionModifier = PositionModifier.None;
        PositionTarget = null;
        PositionEmote = null;
		proximityChange?.Complete();
        return this;
    }

	internal void HideBorrowedCorpseForAnimation()
	{
		Location?.Extract(this);
		ClearRoutePositionForDetachment();
		Location = null;
		Changed = true;
	}

    public IGameItem Get(IBody getter, int quantity)
    {
		ForeignCustodyTransferContext.EnsureItem(this, destructive: !DropsWhole(quantity));
        IStackable stackable = GetItemType<IStackable>();
        if (stackable is null)
        {
            return Get(getter);
        }
        return stackable.Get(quantity).Get(getter);
    }

    public bool DropsWhole(int quantity)
    {
        IStackable stackable = GetItemType<IStackable>();
        return stackable == null || stackable.DropsWhole(quantity);
    }

    public IGameItem Drop(IRoom location)
    {
		DropCore(location, null);
		return this;
    }

	internal bool TryDropPrepared(SpatialLocation destination) => DropCore(destination.Room, destination);

	private bool DropCore(IRoom location, SpatialLocation? destination)
    {
		ForeignCustodyTransferContext.EnsureItem(this);
		if (location is not null) ForeignCustodyTransferContext.EnsureRoom(location, this);
        var sourceLocation = base.Location;
        var sourceContainer = ContainedIn;
        var holdable = GetItemType<IHoldable>();
          var sourceHolder = holdable?.HeldBy;
		var sourceLayer = RoomLayer;
		var sourceRoutePosition = RoutePositionMetres;
        foreach (IGameItemComponent component in _components.ToArray())
        {
            component.Taken();
            if (Deleted || Destroyed || !ReferenceEquals(base.Location, sourceLocation) ||
                  !ReferenceEquals(ContainedIn, sourceContainer) || !ReferenceEquals(holdable?.HeldBy, sourceHolder) ||
				RoomLayer != sourceLayer || RoutePositionMetres != sourceRoutePosition) return false;
        }

		// Prepared recovery must not publish a floor claim for a stack emptied by Taken callbacks.
		if (destination is not null && GetItemType<IStackable>() is { Quantity: <= 0 }) return false;

        holdable?.HeldBy = null;

		if (location is null)
		{
			ClearRoutePositionForDetachment();
		}

		Location = location;
		if (destination is { } point) RestoreInterruptedNativePosition(point.Room, point.Layer, point.RoutePositionMetres);
		return true;
    }

    public IGameItem Drop(IRoom location, int quantity)
    {
		ForeignCustodyTransferContext.EnsureItem(this, destructive: true);
        IStackable stackable = GetItemType<IStackable>();
        IGameItem newItem = stackable.Split(quantity);
        return newItem.Drop(location);
    }

    public IGameItem PeekSplit(int quantity)
    {
        IStackable stackable = GetItemType<IStackable>();
        return stackable == null || quantity == 0 || quantity == Quantity ? this : stackable.PeekSplit(quantity);
    }

    public bool DropsWholeByWeight(double weight)
    {
        ICommodity commodity = GetItemType<ICommodity>();
        if (commodity != null)
        {
            return weight >= commodity.Weight;
        }

        IStackable stackable = GetItemType<IStackable>();
        if (stackable != null)
        {
            return Quantity <= 1 || (Quantity - 1) * Weight / Quantity >= weight;
        }

        return true;
    }

    public IGameItem DropByWeight(IRoom location, double weight)
    {
		if (!DropsWholeByWeight(weight)) SpellOwnedItemValuePolicy.RequireOrdinaryValue(this, "splitting");
		ForeignCustodyTransferContext.EnsureItem(this, destructive: !DropsWholeByWeight(weight));
        if (DropsWholeByWeight(weight))
        {
            return Drop(location);
        }

        ICommodity commodity = GetItemType<ICommodity>();
        if (commodity != null)
        {
            IGameItem newItem = CommodityGameItemComponentProto.CreateNewCommodity(commodity.Material, weight, commodity.Tag,
                commodity.UseIndirectQuantityDescription, commodity.CommodityCharacteristics.Select(x => (x.Key, x.Value)));
            newItem.RoomLayer = RoomLayer;
            newItem.CopyOwnerFrom(this);
            newItem.OverrideSdesc = OverrideSdesc;
            newItem.OverrideDesc = OverrideDesc;
            newItem.GetItemType<ICommodity>().CopySpoilageFrom(commodity);
            commodity.Weight -= weight;
            newItem.Drop(location);
            newItem.Login();
            newItem.HandleEvent(EventType.ItemFinishedLoading, newItem);
            return newItem;
        }

        return Drop(location, (int)Math.Ceiling(weight / (Weight / Quantity)));
    }

    public IGameItem GetByWeight(IBody getter, double weight)
    {
		if (!DropsWholeByWeight(weight)) SpellOwnedItemValuePolicy.RequireOrdinaryValue(this, "splitting");
		ForeignCustodyTransferContext.EnsureItem(this, destructive: !DropsWholeByWeight(weight));
        if (DropsWholeByWeight(weight))
        {
            return Get(getter);
        }

        ICommodity commodity = GetItemType<ICommodity>();
        if (commodity != null)
        {
            IGameItem newItem = new GameItem(this, temporary: false, preserveMorphTime: true);
            newItem.RoomLayer = RoomLayer;
            newItem.GetItemType<ICommodity>().Weight = weight;
            CopyStockDisplayToSplit(newItem);
            commodity.Weight -= weight;
            newItem.Get(getter);
            newItem.Login();
            newItem.HandleEvent(EventType.ItemFinishedLoading, newItem);
            return newItem;
        }

        return Get(getter, (int)Math.Ceiling(weight / (Weight / Quantity)));
    }

    internal void CopyStockDisplayToSplit(IGameItem split)
    {
        foreach (var display in EffectsOfType<ItemOnDisplayInShop>())
        {
            if (display.Merchandise is { } merchandise && !split.AffectedBy<ItemOnDisplayInShop>(merchandise))
            {
                split.AddEffect(new ItemOnDisplayInShop(split, display.Shop, merchandise));
                display.Shop.RegisterStockItemSplit(this, split);
            }
        }
    }

    public IGameItem PeekSplitByWeight(double weight)
    {
        if (DropsWholeByWeight(weight))
        {
            return this;
        }

        ICommodity commodity = GetItemType<ICommodity>();
        if (commodity == null)
        {
            return PeekSplit((int)Math.Ceiling(weight / (Weight / Quantity)));
        }

        IGameItem newItem = new GameItem(this, temporary: true, preserveMorphTime: true);
        newItem.RoomLayer = RoomLayer;
        newItem.GetItemType<ICommodity>().Weight = weight;
        return newItem;
    }

    #endregion

    #region ICombatant Overrides

    public override double DefensiveAdvantage
    {
        get => 0;
        set
        {
            // Do nothing
        }
    }

    public override double OffensiveAdvantage
    {
        get => 0;
        set
        {
            // Do nothing
        }
    }

    public override DefenseType PreferredDefenseType
    {
        get => DefenseType.None;
        set
        {
            // Do nothing
        }
    }

    public override ICombatMove ResponseToMove(ICombatMove move, IPerceiver assailant)
    {
        return null;
    }

    public override bool CheckCombatStatus()
    {
        return true;
    }

    #endregion

    #region IHaveCharacteristics Members

    public IEnumerable<ICharacteristicDefinition> CharacteristicDefinitions
    {
        get
        {
            IVariable variable = GetItemType<IVariable>();
            ICommodity commodity = GetItemType<ICommodity>();
            return (variable?.CharacteristicDefinitions ?? Enumerable.Empty<ICharacteristicDefinition>())
                   .Concat(commodity?.CommodityCharacteristics.Keys ?? Enumerable.Empty<ICharacteristicDefinition>())
                   .Distinct();
        }
    }

    public IEnumerable<ICharacteristicValue> RawCharacteristicValues =>
        CharacteristicDefinitions.Select(x => GetCharacteristic(x, null));

    public IEnumerable<(ICharacteristicDefinition Definition, ICharacteristicValue Value)> RawCharacteristics =>
        CharacteristicDefinitions
            .Select(x => (x, GetCharacteristic(x, null)));

    private static readonly Regex BasicCharacteristicRegex = new(@"(.+)basic", RegexOptions.IgnoreCase);
    private static readonly Regex FancyCharacteristicRegex = new(@"(.+)fancy", RegexOptions.IgnoreCase);

    public Tuple<ICharacteristicDefinition, CharacteristicDescriptionType> GetCharacteristicDefinition(
        string pattern)
    {
        CharacteristicDescriptionType descType = CharacteristicDescriptionType.Normal;

        ICharacteristicDefinition type;
        if (BasicCharacteristicRegex.IsMatch(pattern))
        {
            type =
                CharacteristicDefinitions.FirstOrDefault(
                    x => x.Pattern.IsMatch(BasicCharacteristicRegex.Match(pattern).Groups[1].Value));
            descType = CharacteristicDescriptionType.Basic;
        }
        else if (FancyCharacteristicRegex.IsMatch(pattern))
        {
            type =
                CharacteristicDefinitions.FirstOrDefault(
                    x => x.Pattern.IsMatch(FancyCharacteristicRegex.Match(pattern).Groups[1].Value));
            descType = CharacteristicDescriptionType.Fancy;
        }
        else
        {
            type = CharacteristicDefinitions.FirstOrDefault(x => x.Pattern.IsMatch(pattern));
        }

        return Tuple.Create(type, descType);
    }

    public ICharacteristicValue GetCharacteristic(string type, IPerceiver voyeur)
    {
        ICharacteristicDefinition definition = (this as IHaveCharacteristics).GetCharacteristicDefinition(type).Item1;
        return definition != null ? GetCharacteristic(definition, voyeur) : null;
    }

    public ICharacteristicValue GetCharacteristic(ICharacteristicDefinition type, IPerceiver voyeur)
    {
        IChangeCharacteristicEffect effect = EffectsOfType<IChangeCharacteristicEffect>().FirstOrDefault(x => x.Applies(this) && x.ChangesCharacteristic(type));
        IVariable variable = GetItemType<IVariable>();
        if (variable?.CharacteristicDefinitions.Contains(type) == true)
        {
            return effect?.GetChangedCharacteristic(type) ?? variable.GetCharacteristic(type);
        }

        return effect?.GetChangedCharacteristic(type) ?? GetItemType<ICommodity>()?.GetCommodityCharacteristic(type);
    }

    public void SetCharacteristic(ICharacteristicDefinition type, ICharacteristicValue value)
    {
        IVariable variable = GetItemType<IVariable>();
        if (variable?.CharacteristicDefinitions.Contains(type) == true)
        {
            variable.SetCharacteristic(type, value);
            return;
        }

        GetItemType<ICommodity>()?.SetCommodityCharacteristic(type, value);
    }

    public string DescribeCharacteristic(ICharacteristicDefinition definition, IPerceiver voyeur,
        CharacteristicDescriptionType type = CharacteristicDescriptionType.Normal)
    {
        ICharacteristicValue characteristic = GetCharacteristic(definition, voyeur);
        if (characteristic != null)
        {
            switch (type)
            {
                case CharacteristicDescriptionType.Normal:
                    return characteristic.GetValue;
                case CharacteristicDescriptionType.Basic:
                    return characteristic.GetBasicValue;
                case CharacteristicDescriptionType.Fancy:
                    return characteristic.GetFancyValue;
                default:
                    throw new NotSupportedException();
            }
        }

        return "--Invalid Characteristic--";
    }

    public string DescribeCharacteristic(string type, IPerceiver voyeur)
    {
        Tuple<ICharacteristicDefinition, CharacteristicDescriptionType> definition = (this as IHaveCharacteristics).GetCharacteristicDefinition(type);
        return definition.Item1 == null
            ? "--Invalid Characteristic--"
            : DescribeCharacteristic(definition.Item1, voyeur, definition.Item2);
    }

    public IObscureCharacteristics GetObscurer(ICharacteristicDefinition type, IPerceiver voyeur)
    {
        return null;
    }

    public void ExpireDefinition(ICharacteristicDefinition definition)
    {
        GetItemType<IVariable>()?.ExpireDefinition(definition);
        GetItemType<ICommodity>()?.RemoveCommodityCharacteristic(definition);
    }

    public void RecalculateCharacteristicsDueToExternalChange()
    {
        // Do nothing
    }

    #endregion

    #region IHaveTags Members

    public IEnumerable<ITag> Tags => Prototype.Tags;

    public bool AddTag(ITag tag)
    {
        return false;
    }

    public bool RemoveTag(ITag tag)
    {
        return false;
    }

    public bool IsA(ITag tag)
    {
        return tag == null || Tags.Any(x => x.IsA(tag));
    }

    #endregion

    #region Morphing

	private double _morphRateAtSchedule = 1.0;

    private void SaveMorphProgress(MudSharp.Models.GameItem dbitem)
    {
        if (CachedMorphTime is not null)
        {
            dbitem.MorphTimeRemaining = (int)CachedMorphTime.Value.TotalSeconds;
            return;
        }

        if (MorphTime == DateTime.MinValue)
        {
            dbitem.MorphTimeRemaining = null;
            return;
        }

		var remaining = Gameworld.Scheduler.RemainingDuration(this, ScheduleType.Morph);
		dbitem.MorphTimeRemaining = (int)(remaining.TotalSeconds *
			(Prototype.RefrigerationSensitive ? _morphRateAtSchedule : 1.0));
    }

    public TimeSpan? CachedMorphTime { get; set; }
    public DateTime MorphTime { get; set; }

    public void ResetMorphTimer()
    {
        bool morphing = MorphTime != DateTime.MinValue;
        EndMorphTimer();
        if (CachedMorphTime is not null)
        {
            CachedMorphTime = Prototype.MorphTimeSpan;
        }

        if (morphing)
        {
            StartMorphTimer();
        }
    }

    public void StartMorphTimer()
    {
        if (CachedMorphTime is not null)
        {
			_morphRateAtSchedule = Prototype.RefrigerationSensitive
				? this.TimeRateMultiplier(ItemTimeRateType.Morph)
				: 1.0;
			if (_morphRateAtSchedule <= 0.0)
			{
				MorphTime = DateTime.MinValue;
				return;
			}

			MorphTime = RuntimeClock.UtcNow + ItemTimeRateMath.WallDuration(
				CachedMorphTime.Value, _morphRateAtSchedule)!.Value;
            CachedMorphTime = null;
        }

        if (MorphTime != DateTime.MinValue)
        {
            Gameworld.Scheduler.AddSchedule(new RepeatingSchedule<IGameItem>(this, Gameworld,
                item => item.Changed = true, ScheduleType.MorphSaving, TimeSpan.FromSeconds(30),
                $"Morph Saver for item #{Id}"));
            Gameworld.Scheduler.AddSchedule(new Schedule<IGameItem>(this, Morph, ScheduleType.Morph,
                MorphTime > RuntimeClock.UtcNow ? MorphTime - RuntimeClock.UtcNow : TimeSpan.FromTicks(1),
                $"Morph checker for item #{Id}"));
        }
    }

    public void EndMorphTimer()
    {
        if (MorphTime != DateTime.MinValue)
        {
            Gameworld.Scheduler.Destroy(this, ScheduleType.Morph);
            Gameworld.Scheduler.Destroy(this, ScheduleType.MorphSaving);
			var wallTimeRemaining = MorphTime - RuntimeClock.UtcNow;
			CachedMorphTime = Prototype.RefrigerationSensitive
				? ItemTimeRateMath.EffectiveElapsed(wallTimeRemaining, _morphRateAtSchedule)
				: wallTimeRemaining;
			if (CachedMorphTime < TimeSpan.Zero)
			{
				CachedMorphTime = TimeSpan.Zero;
			}
            MorphTime = DateTime.MinValue;
        }
    }

	public void RebaseItemTimeRates()
	{
		var now = RuntimeClock.UtcNow;
		foreach (var component in Components.OfType<IItemTimeRateSensitive>())
		{
			component.ResolveTimeRate(now);
		}
		foreach (var container in Components.OfType<ILiquidContainer>())
		{
			container.ResolveLiquidFreshness(now);
		}

		ResolveSurfaceLiquidDrying();
		if (!Prototype.RefrigerationSensitive ||
			(MorphTime == DateTime.MinValue && CachedMorphTime is null))
		{
			return;
		}

		var wasScheduled = MorphTime != DateTime.MinValue;
		if (wasScheduled)
		{
			EndMorphTimer();
		}

		if (wasScheduled || CachedMorphTime is not null)
		{
			StartMorphTimer();
		}
	}

    private void Morph(IGameItem item)
    {
		ForeignCustodyTransferContext.EnsureItem(this, destructive: true);
		if (SpellCreationOrigin?.IsTemporary == true && Gameworld.SpellOwnedItems?.TryPrepareRemoval(this, out _) != true) return;
        if (GetItemType<ICorpse>() is not null && Gameworld.SpellOwnedNpcs?.TryPrepareRemainsRemoval(this, out _, morphing: true) == false) return;
		if (GetItemType<ICorpse>() is not null && Gameworld.SpellOwnedCorpseAnimations?.IsBorrowedCorpse(Id) == true) return;
        IGameItem newItem = Prototype.LoadMorphedItem(this);
        IRoom location = TrueLocations.FirstOrDefault();
		var originalSpatialLocation = location is null
			? (SpatialLocation?)null
			: CaptureComponentLifecycleSpatialLocation(location);
        if (!string.IsNullOrEmpty(Prototype.MorphEmote))
        {
            OutputHandler.Handle(new EmoteOutput(new Emote(Prototype.MorphEmote.SubstituteANSIColour(), this, this, newItem)));
        }

        if (newItem != null)
        {
            newItem.CopyOwnerFrom(this);
            InInventoryOf?.SwapInPlace(this, newItem);
            ContainedIn?.SwapInPlace(this, newItem);
            newItem.RoomLayer = RoomLayer;
			if (Location is not null && originalSpatialLocation.HasValue)
			{
				newItem.InsertAtSpatialLocation(originalSpatialLocation.Value);
			}
            foreach (IEffect effect in Effects)
            {
                IEffect newEffect = effect.NewEffectOnItemMorph(this, newItem);
                if (newEffect != null)
                {
                    if (Gameworld.EffectScheduler.IsScheduled(effect))
                    {
                        newItem.AddEffect(newEffect, Gameworld.EffectScheduler.RemainingDuration(effect));
                    }
                    else
                    {
                        newItem.AddEffect(newEffect);
                    }
                }
            }

            foreach (IGameItemComponent comp in Components)
            {
                comp.HandleDieOrMorph(newItem, location, originalSpatialLocation);
            }

            newItem.Login();
        }

        Delete();
    }

	private SpatialLocation CaptureComponentLifecycleSpatialLocation(IRoom selectedRoom)
	{
		var spatialService = RouteSpatialService.Instance;
		var currentLocation = spatialService.GetEffectiveLocation(LocationLevelPerceivable);
		if (ReferenceEquals(currentLocation.Room, selectedRoom) &&
			spatialService.TryValidateLocation(currentLocation, out _))
		{
			return currentLocation;
		}

		var installedExit = GetItemType<IDoor>()?.InstalledExit;
		var selectedExit = installedExit?.RoomExitFor(selectedRoom);
		if (selectedRoom.RouteDefinition is not null &&
			selectedExit is not null &&
			spatialService.TryGetExitAnchor(selectedExit, selectedRoom, out var anchor) &&
			anchor is not null)
		{
			return new SpatialLocation(selectedRoom, RoomLayer, anchor.ArrivalPositionMetres);
		}

		spatialService.TryValidateLocation(currentLocation, out var error);
		throw new InvalidOperationException(
			$"Cannot capture the destruction or morph position of item #{Id:N0} in Room #{selectedRoom.Id:N0}: " +
			$"{error} Installed RouteRoom doors require an anchor on the selected exit side.");
	}

    #endregion

    #region Implementation of IHaveHeight

    /// <summary>
    ///     The height of the thing, in base units
    /// </summary>
    public double Height { get; set; }

    #endregion

    #region Implementation of IHaveABody

    public IBody Body => _components.SelectNotNull(x => x.HaveABody).FirstOrDefault();
    public Alignment Handedness => Alignment.Irrelevant;

    #endregion

    #region Proximity Related Overrides

    public override IEnumerable<(IPerceivable Thing, Proximity Proximity)> LocalThingsAndProximities()
    {
		var effectiveLocation = RouteSpatialService.Instance.GetEffectiveLocation(this);
		IEnumerable<IPerceivable> candidates;
		if (effectiveLocation.Room?.RouteDefinition is null)
		{
			candidates = TrueLocations.SelectMany(x => x.Perceivables);
		}
		else
		{
			var maximumDistance = Gameworld.GetStaticDouble("RouteCellVeryDistantDistanceMetres");
			if (!double.IsFinite(maximumDistance) || maximumDistance <= 0.0)
			{
				maximumDistance = RouteSpatialConfiguration.Default.VeryDistantDistanceMetres;
			}

			candidates = RouteSpatialService.Instance.GetPerceivablesWithinAcrossLayers(
				effectiveLocation,
				maximumDistance);
		}

		var proximityEffects = EffectsOfType<IAffectProximity>().ToList();
		foreach (var candidate in candidates
			         .Where(x => !ReferenceEquals(x, this))
			         .Distinct())
		{
			var proximity = InVicinity(candidate)
				? Proximity.Immediate
				: GetProximity(candidate);
			var affectedProximity = proximityEffects
				.Select(x => x.GetProximityFor(candidate))
				.Where(x => x.Affects)
				.Select(x => x.Proximity)
				.DefaultIfEmpty(Proximity.Unapproximable)
				.Min();
			proximity = (Proximity)Math.Min((int)proximity, (int)affectedProximity);
			if (proximity != Proximity.Unapproximable)
			{
				yield return (candidate, proximity);
			}
		}

        foreach (AdjacentToExit effect in EffectsOfType<AdjacentToExit>().Where(x => x.Exit.Exit.Door != null).ToList())
        {
            yield return (effect.Exit.Exit.Door.Parent, Proximity.Proximate);
        }
    }

    public override Proximity GetProximity(IPerceivable thing)
    {
        if (thing == null)
        {
            return Proximity.Unapproximable;
        }

        if (thing.IsSelf(this))
        {
            return Proximity.Intimate;
        }

        if ((PositionTarget != null &&
             (PositionTarget.IsSelf(thing.PositionTarget) || PositionTarget.IsSelf(thing))) ||
            thing.PositionTarget?.IsSelf(this) == true)
        {
            return Proximity.Immediate;
        }

        if (Cover == thing)
        {
            return Proximity.Immediate;
        }

        IGameItem ptGameItem = PositionTarget as IGameItem;
        if (ptGameItem?.IsItemType<IChair>() == true)
        {
            IChair chair = ptGameItem.GetItemType<IChair>();
            if (chair.Table != null)
            {
                if (thing.IsSelf(chair.Table.Parent))
                {
                    return Proximity.Immediate;
                }

                if (thing.PositionTarget?.IsSelf(chair.Table.Parent) == true)
                {
                    return Proximity.Immediate;
                }

                if (thing is IGameItem taig && taig.IsItemType<IChair>())
                {
                    IChair otherChair = taig.GetItemType<IChair>();
                    if (otherChair.Table == chair.Table)
                    {
                        return Proximity.Immediate;
                    }
                }
            }
        }

        return base.GetProximity(thing);
    }

    #endregion
}
