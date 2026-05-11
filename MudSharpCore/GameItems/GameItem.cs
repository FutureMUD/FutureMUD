using ExpressionEngine;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MudSharp.Body;
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Body.Traits;
using MudSharp.Character;
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
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using MudSharp.Framework.Scheduling;
using MudSharp.Framework.Units;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.Health;
using MudSharp.Models;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Handlers;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using MudSharp.Magic;
using MudSharp.Planes;
using Org.BouncyCastle.Asn1.X509;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using CharacteristicValue = MudSharp.Form.Characteristics.CharacteristicValue;

#nullable enable annotations

namespace MudSharp.GameItems;

public partial class GameItem : PerceiverItem, IGameItem, IDisposable
{
    #region IGameItem Related Code

    protected readonly List<IGameItemComponent> _components = new();

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

    public bool IsOwnedBy(IFrameworkItem owner)
    {
        return _ownerReference?.Equals(owner) == true;
    }

    public void SetOwner(IFrameworkItem owner)
    {
        _owner = owner;
        _ownerReference = owner == null
            ? null
            : new FrameworkItemReference(owner.Id, owner.FrameworkItemType, Gameworld);
        Changed = true;
    }

    public void ClearOwner()
    {
        _owner = null;
        _ownerReference = null;
        Changed = true;
    }

    public event PerceivableEvent OnRemovedFromLocation;

    public event InventoryChangeEvent OnInventoryChange;

    public void InvokeInventoryChange(InventoryState oldState, InventoryState newState)
    {
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
                total += corpse.Body.ExternalItems.Sum(x => x.IlluminationProvided);
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
        dbitem.RoomLayer = (int)RoomLayer;
        dbitem.SkinId = _skinId;
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

        string description = Prototype.ShortDescription;
        if (Skin is { ShortDescription: { } } skin)
        {
            description = skin.ShortDescription;
        }
        else if (voyeur is ICharacter ch)
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
            EffectData = SaveEffects().ToString(),
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
        return
            EffectsOfType<ISDescAdditionEffect>()
                .Where(x => x.DescriptionAdditionApplies(voyeur))
                .DistinctBy(x => x.AddendumText)
                .Select(effect => effect.GetAddendumText(colour))
                .Aggregate(description, (current, text) => $"{current} {text}");
    }

    private string FullDescription(IPerceiver voyeur, bool colour, PerceiveIgnoreFlags flags, bool excludeComponents)
    {
        if (!voyeur.CanSee(this))
        {
            return "You cannot make out anything about it.";
        }

        string description = Prototype.FullDescription;
        if (Skin is { FullDescription: { } } skin)
        {
            description = skin.FullDescription;
        }

        if (voyeur is ICharacter ch)
        {
            (FutureProg.IFutureProg Prog, string ShortDescription, string FullDescription, string FullDescriptionAddendum) descValue = Prototype.ExtraDescriptions.Where(x => !string.IsNullOrEmpty(x.FullDescription))
                                     .FirstOrDefault(x => x.Prog.Execute<bool?>(ch) == true);
            if (descValue.Prog != null)
            {
                description = descValue.FullDescription;
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
        string description = Prototype.FullDescription;
        if (voyeur is ICharacter ch)
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

    public GameItem(MudSharp.Models.GameItem item, IFuturemud game)
        : base(item.Id)
    {
        _noSave = true;
        Register(new IgnorantItemOutputHandler(this));
        Gameworld = game;
        _id = item.Id;
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
        LoadHooks(item.HooksPerceivables, "GameItem");
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

        _overridingWoundBehaviourComponent = _components.OfType<IOverrideItemWoundBehaviour>().FirstOrDefault();
        _noSave = false;
    }

    private void LoadWounds(IEnumerable<Wound> wounds)
    {
        foreach (Wound wound in wounds)
        {
            _wounds.Add(WoundFactory.LoadWound(wound, this, Gameworld));
        }

        StartHealthTick();
    }

    public GameItem(IGameItemProto proto, ICharacter? loader = null, ItemQuality quality = ItemQuality.Standard)
    {
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
            _components.Add(component.CreateNew(this, loader));
        }

        List<IHook> hooks = Gameworld.DefaultHooks.Where(
            x => x.Applies(this, "GameItem")).Select(x => x.Hook).ToList();

        if (hooks.Any())
        {
            foreach (IHook hook in hooks)
            {
                InstallHook(hook);
            }
        }

        SetState(PositionUndefined.Instance);
        Gameworld.SaveManager.AddInitialisation(this);
        foreach (IGameItemComponent comp in Components)
        {
            comp.FinaliseLoad();
        }

        if (Prototype.Morphs)
        {
            CachedMorphTime = Prototype.MorphTimeSpan;
        }

        _overridingWoundBehaviourComponent = _components.OfType<IOverrideItemWoundBehaviour>().FirstOrDefault();
    }

    public GameItem(GameItem rhs, bool temporary = false, bool preserveMorphTime = false)
    {
        if (temporary)
        {
            _noSave = true;
        }

        Register(new IgnorantItemOutputHandler(this));
        Prototype = rhs.Prototype;
        _quality = rhs._quality;
        _overrideMaterial = rhs._overrideMaterial;
        Gameworld = rhs.Gameworld;
        _name = rhs.Name;
        _keywords = new Lazy<List<string>>(() => rhs.Keywords.ToList());
        PositionState = rhs.PositionState;
        PositionModifier = rhs.PositionModifier;
        PositionTarget = rhs.PositionTarget;
        PositionEmote = rhs.PositionEmote;
        _condition = rhs.Condition;
        Location = rhs.Location;
        _ownerReference = rhs._ownerReference == null
            ? null
            : new FrameworkItemReference(rhs._ownerReference.Id, rhs._ownerReference.FrameworkItemType, rhs.Gameworld);
        _owner = rhs._owner;
        foreach (IGameItemComponent component in rhs._components)
        {
            _components.Add(component.Copy(this, temporary));
        }

        if (!temporary)
        {
            Gameworld.Add(this);
            Gameworld.SaveManager.AddInitialisation(this);
        }

        foreach (IGameItemComponent component in Components)
        {
            component.FinaliseLoad();
        }

        if (Prototype.Morphs)
        {
            if (preserveMorphTime)
            {
                if (rhs.CachedMorphTime is not null)
                {
                    CachedMorphTime = rhs.CachedMorphTime;
                }
                else
                {
                    CachedMorphTime = rhs.MorphTime - DateTime.UtcNow;
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
        foreach (IGameItemComponent component in Components)
        {
            component.FinaliseLoad();
        }

        foreach (IGameItem item in _wounds.SelectNotNull(x => x.Lodged))
        {
            item.FinaliseLoadTimeTasks();
        }
    }

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
        if (mixture.TotalVolume <= 0)
        {
            return;
        }

        foreach (IGameItemComponent component in _components)
        {
            if (component.ExposeToLiquid(mixture) || mixture.TotalVolume <= 0)
            {
                return;
            }
        }

        ILiquidContaminationEffect effect = EffectsOfType<ILiquidContaminationEffect>()
            .FirstOrDefault(x => x.ContaminatingLiquid.CanMerge(mixture));
        if (effect == null)
        {
            LiquidMixture newMixture = LiquidMixture.CreateEmpty(Gameworld);
            effect = new LiquidContamination(this, newMixture);
            AddEffect(effect, LiquidContamination.EffectDuration(effect.ContaminatingLiquid));
        }

        List<ICleanableEffect> cleanableEffects =
            EffectsOfType<ICleanableEffect>(x => mixture.Instances.Any(y => y.Liquid.LiquidCountsAs(x.LiquidRequired)))
                .ToList();
        foreach (ICleanableEffect cleanable in cleanableEffects)
        {
            if (cleanable.CleanWithLiquid(mixture, mixture.TotalVolume))
            {
                RemoveEffect(cleanable, true);
            }
        }

        (double coatingAmount, double absorbAmount) = LiquidAbsorbtionAmounts;
        double totalAbsorbCapacity = coatingAmount + absorbAmount - effect.ContaminatingLiquid.TotalVolume;
        double amountToAbsorb = totalAbsorbCapacity;
        if (totalAbsorbCapacity > mixture.TotalVolume)
        {
            amountToAbsorb = mixture.TotalVolume;
        }

        if (amountToAbsorb > 0)
        {
            LiquidMixture newMixture = mixture.RemoveLiquidVolume(amountToAbsorb);
            if (newMixture?.IsEmpty == false)
            {
                effect.ContaminatingLiquid.AddLiquid(newMixture);
                Reschedule(effect, LiquidContamination.EffectDuration(effect.ContaminatingLiquid));
            }
        }

        if (mixture.TotalVolume > 0)
        {
            // Item is saturated
            IEnumerable<IContainer> containers = GetItemTypes<IContainer>();
            foreach (IContainer container in containers)
            {
                foreach (IGameItem content in container.Contents)
                {
                    content.ExposeToLiquid(mixture, null, LiquidExposureDirection.FromContainer);
                }
            }

            IBelt attach = GetItemType<IBelt>();
            if (attach != null)
            {
                foreach (IBeltable attached in attach.ConnectedItems)
                {
                    attached.Parent.ExposeToLiquid(mixture, null, LiquidExposureDirection.FromOnTop);
                }
            }

            switch (direction)
            {
                case LiquidExposureDirection.FromInside:
                    ContainedIn?.ExposeToLiquid(mixture, part, LiquidExposureDirection.FromInside);
                    break;
                case LiquidExposureDirection.FromUnderneath:
                    if (part == null)
                    {
                        foreach (IWear wornPart in GetItemType<IWearable>()?.CurrentProfile?.AllProfiles.Keys ??
                                                 Enumerable.Empty<IWear>())
                        {
                            InInventoryOf.WornItemsFor(wornPart).SkipWhile(x => x != this).Skip(1).FirstOrDefault()
                                         ?.ExposeToLiquid(mixture, wornPart, LiquidExposureDirection.FromUnderneath);
                        }
                    }
                    else
                    {
                        InInventoryOf.WornItemsFor(part).SkipWhile(x => x != this).Skip(1).FirstOrDefault()
                                     ?.ExposeToLiquid(mixture, part, LiquidExposureDirection.FromUnderneath);
                    }

                    break;
                case LiquidExposureDirection.FromOnTop:
                    if (part == null)
                    {
                        foreach (IWear wornPart in GetItemType<IWearable>()?.CurrentProfile?.AllProfiles.Keys ??
                                                 Enumerable.Empty<IWear>())
                        {
                            InInventoryOf.WornItemsFor(wornPart).Reverse().SkipWhile(x => x != this).Skip(1)
                                         .FirstOrDefault()?.ExposeToLiquid(mixture, wornPart,
                                             LiquidExposureDirection.FromOnTop);
                        }
                    }
                    else
                    {
                        InInventoryOf.WornItemsFor(part).Reverse().SkipWhile(x => x != this).Skip(1).FirstOrDefault()
                                     ?.ExposeToLiquid(mixture, part, LiquidExposureDirection.FromOnTop);
                        InInventoryOf.ExposeToLiquid(mixture, part, LiquidExposureDirection.FromOnTop);
                    }

                    break;
                case LiquidExposureDirection.Irrelevant:
                    ContainedIn?.ExposeToLiquid(mixture, part, LiquidExposureDirection.FromInside);
                    break;
            }

            // Drip onto ground if we have any left and are outerwear
            if (
                Gameworld.GetStaticBool("PuddlesEnabled") &&
                mixture.TotalVolume > 0 &&
                ContainedIn is null &&
                direction != LiquidExposureDirection.FromOnTop &&
                direction != LiquidExposureDirection.FromContainer &&
                InInventoryOf?.ExternalItemsForOtherActors.Contains(this) != false
            )
            {
                IPerceiver topLevel = (IPerceiver)LocationLevelPerceivable;
                PuddleGameItemComponentProto.TopUpOrCreateNewPuddle(mixture, topLevel?.Location, topLevel?.RoomLayer ?? RoomLayer.GroundLevel, topLevel);
                mixture.SetLiquidVolume(0.0);
            }

            return;
        }
    }

    public ItemSaturationLevel SaturationLevel
    {
        get
        {
            (double coating, double absorb) = LiquidAbsorbtionAmounts;
            IEnumerable<ILiquidContaminationEffect> effects = EffectsOfType<ILiquidContaminationEffect>();
            double total = effects.Sum(x => x.ContaminatingLiquid.TotalVolume);
            if (total <= 0)
            {
                return ItemSaturationLevel.Dry;
            }

            if (total >= absorb)
            {
                if (total > absorb + coating)
                {
                    return ItemSaturationLevel.Saturated;
                }

                return ItemSaturationLevel.Soaked;
            }

            if (total >= absorb * 0.5)
            {
                return ItemSaturationLevel.Wet;
            }

            return ItemSaturationLevel.Damp;
        }
    }

    public ItemSaturationLevel SaturationLevelForLiquid(LiquidInstance instance)
    {
        (double coating, double absorb) = LiquidAbsorbtionAmounts;
        double total = instance.Amount;
        if (total <= 0)
        {
            return ItemSaturationLevel.Dry;
        }

        if (total >= absorb)
        {
            if (total > absorb + coating)
            {
                return ItemSaturationLevel.Saturated;
            }

            return ItemSaturationLevel.Soaked;
        }

        if (total >= absorb * 0.5)
        {
            return ItemSaturationLevel.Wet;
        }

        return ItemSaturationLevel.Damp;
    }

    public ItemSaturationLevel SaturationLevelForLiquid(double total)
    {
        (double coating, double absorb) = LiquidAbsorbtionAmounts;
        if (total <= 0)
        {
            return ItemSaturationLevel.Dry;
        }

        if (total >= absorb)
        {
            if (total > absorb + coating)
            {
                return ItemSaturationLevel.Saturated;
            }

            return ItemSaturationLevel.Soaked;
        }

        if (total >= absorb * 0.5)
        {
            return ItemSaturationLevel.Wet;
        }

        return ItemSaturationLevel.Damp;
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
            if (value < 0.0)
            {
                value = 0.0;
            }

            _condition = value;
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

        FallDamageExpression.Parameters["weight"] = Weight;
        FallDamageExpression.Parameters["rooms"] = effectiveFallDistance;
        var damageAmount = Convert.ToDouble(FallDamageExpression.Evaluate()) * damageMultiplier;
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
                   .Concat(Wounds.SelectNotNull(x => x.Lodged));

    public IEnumerable<IGameItem> LodgedItems => Wounds.SelectNotNull(x => x.Lodged).ToArray();
    public IEnumerable<IGameItem> AttachedItems => (GetItemType<IBelt>()?.ConnectedItems.Select(x => x.Parent) ?? []).ToArray();
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

    public IGameItem ContainedIn
    {
        get => _containedIn;
        set
        {
            _containedIn = value;
            Changed = true;
        }
    }

    /// <summary>
    /// This returns an IPerceivable (which may be this item) that represents the perceivable "thing" that is actually in the room, for purposes of working out proximity of this item irrespestive of whether it is sitting in the room, being carried, in a container, attached to something etc.
    /// </summary>
    public IPerceivable LocationLevelPerceivable => ContainedIn?.LocationLevelPerceivable ??
                   InInventoryOf?.Actor ??
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

    public override ICell Location
    {
        get => base.Location ?? TrueLocations?.FirstOrDefault();
        protected set => base.Location = value;
    }

    #endregion

    public IEnumerable<ICell> TrueLocationsExcept(List<IGameItem> itemsConsidered)
    {
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
            List<ICell> location = mountable.MountHost.Parent.TrueLocationsExcept(itemsConsidered).ToList();
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
                List<ICell> location = item.Item2.Parent.TrueLocationsExcept(itemsConsidered).ToList();
                if (location.Any())
                {
                    return location;
                }
            }
        }

        IDoor door = GetItemType<IDoor>();
        if (door?.InstalledExit != null)
        {
            return door.InstalledExit.Cells;
        }

        if (InInventoryOf?.Location != null)
        {
            return new[] { InInventoryOf.Location };
        }

        return ContainedIn?.TrueLocations ?? Enumerable.Empty<ICell>();
    }

    public IEnumerable<ICell> TrueLocations
    {
        get
        {
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
                List<ICell> location = mountable.MountHost.Parent.TrueLocationsExcept(new List<IGameItem> { this }).ToList();
                if (location.Any())
                {
                    return location;
                }
            }

            IConnectable connectable = GetItemType<IConnectable>();
            if (connectable?.ConnectedItems.Any() ?? false)
            {
                List<ICell> location = connectable.Parent.TrueLocationsExcept(new List<IGameItem> { this }).ToList();
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
                return door.InstalledExit.Cells;
            }

            if (InInventoryOf?.Location != null)
            {
                return new[] { InInventoryOf.Location };
            }

            return ContainedIn?.TrueLocations ?? Enumerable.Empty<ICell>();
        }
    }

    public void Handle(string text, OutputRange range = OutputRange.Personal)
    {
        switch (range)
        {
            case OutputRange.Local:
                foreach (ICell location in TrueLocations)
                {
                    location.Handle(text);
                }

                break;
            case OutputRange.Room:
                foreach (ICell location in TrueLocations)
                {
                    location.Room.Handle(text);
                }

                break;
            case OutputRange.Shard:
                foreach (ICell location in TrueLocations)
                {
                    location.Shard.Handle(text);
                }

                break;
            case OutputRange.Zone:
                foreach (ICell location in TrueLocations)
                {
                    location.Zone.Handle(text);
                }

                break;
            case OutputRange.Surrounds:
                foreach (ICell location in TrueLocations.SelectMany(x => x.Surrounds).Except(TrueLocations))
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
                foreach (ICell location in TrueLocations)
                {
                    location.Handle(output);
                }

                break;
            case OutputRange.Room:
                foreach (ICell location in TrueLocations)
                {
                    location.Room.Handle(output);
                }

                break;
            case OutputRange.Shard:
                foreach (ICell location in TrueLocations)
                {
                    location.Room.Zone.Shard.Handle(output);
                }

                break;
            case OutputRange.Zone:
                foreach (ICell location in TrueLocations)
                {
                    location.Room.Zone.Handle(output);
                }

                break;
            case OutputRange.Surrounds:
                foreach (ICell location in TrueLocations.SelectMany(x => x.Surrounds).Except(TrueLocations))
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


    public void Delete()
    {
        Changed = false;
        _noSave = true;
        Gameworld.SaveManager.Abort(this);
        foreach (IGameItemComponent component in Components)
        {
            Gameworld.SaveManager.Abort(component);
        }

        InvalidatePositionTargets();
        SoftReleasePositionTarget();
        PerceivableDeleted();
        ContainedIn?.Take(this);
        ContainedIn = null;
        InInventoryOf?.Take(this);
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

        if (_id != 0)
        {
            using (new FMDB())
            {
                Gameworld.SaveManager.Flush();
                Models.GameItem dbitem = FMDB.Context.GameItems.Find(Id);
                if (dbitem != null)
                {
                    FMDB.Context.GameItems.Remove(dbitem);
                    FMDB.Context.SaveChanges();
                }
            }
        }

        Deleted = true;
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
    }

    public void Quit()
    {
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

        return sb.ToString();
    }

    public double Weight
    {
        get
        {
#if DEBUG
            double weight = (Prototype.Weight + _components.Sum(x => x.ComponentWeight) +
                          EffectsOfType<IEffectAddsWeight>().Sum(x => x.AddedWeight)) *
                         _components.Aggregate(1.0, (a, b) => a * b.ComponentWeightMultiplier);
#endif
            return (Prototype.Weight + _components.Sum(x => x.ComponentWeight) +
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
        if (Deleted)
        {
            return false;
        }

        if (otherItem.Prototype != Prototype)
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
        IStackable thisStackable = GetItemType<IStackable>();
        IStackable thatStackable = otherItem.GetItemType<IStackable>();

        if (thisStackable != null && thatStackable != null)
        {
            thisStackable.Quantity += thatStackable.Quantity;
            return;
        }

        ICurrencyPile thisCurrency = GetItemType<ICurrencyPile>();
        ICurrencyPile thatCurrency = otherItem.GetItemType<ICurrencyPile>();
        if (thisCurrency != null && thatCurrency != null && thisCurrency.Currency == thatCurrency.Currency)
        {
            thisCurrency.AddCoins(thatCurrency.Coins);
            return;
        }

        ICommodity thisCommodity = GetItemType<ICommodity>();
        ICommodity thatCommodity = otherItem.GetItemType<ICommodity>();
        if (thisCommodity != null && thatCommodity != null && CommodityCharacteristicRequirement.CommodityIdentityEqual(thisCommodity, thatCommodity))
        {
            thisCommodity.Weight += thatCommodity.Weight;
            return;
        }
        // TODO - anything else that might occur on merging?
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
        IHoldable holdable = GetItemType<IHoldable>();
        holdable?.HeldBy = getter;

        if (Location != null)
        {
            OnRemovedFromLocation?.Invoke(this);
        }

        Location?.Extract(this);
        Location = null;
        InvalidatePositionTargets();
        EffectHandler.RemoveAllEffects(x => x.IsEffectType<IRemoveOnGet>(), true);
        PositionState = PositionUndefined.Instance;
        PositionModifier = PositionModifier.None;
        PositionTarget = null;
        PositionEmote = null;
        return this;
    }

    public IGameItem Get(IBody getter, int quantity)
    {
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

    public IGameItem Drop(ICell location)
    {
        foreach (IGameItemComponent component in _components)
        {
            component.Taken();
        }

        IHoldable holdable = GetItemType<IHoldable>();
        holdable?.HeldBy = null;

        Location = location;
        return this;
    }

    public IGameItem Drop(ICell location, int quantity)
    {
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

    public IGameItem DropByWeight(ICell location, double weight)
    {
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
        if (DropsWholeByWeight(weight))
        {
            return Get(getter);
        }

        ICommodity commodity = GetItemType<ICommodity>();
        if (commodity != null)
        {
            IGameItem newItem = CommodityGameItemComponentProto.CreateNewCommodity(commodity.Material, weight, commodity.Tag,
                commodity.UseIndirectQuantityDescription, commodity.CommodityCharacteristics.Select(x => (x.Key, x.Value)));
            newItem.RoomLayer = RoomLayer;
            commodity.Weight -= weight;
            newItem.Get(getter);
            newItem.Login();
            newItem.HandleEvent(EventType.ItemFinishedLoading, newItem);
            return newItem;
        }

        return Get(getter, (int)Math.Ceiling(weight / (Weight / Quantity)));
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

        IGameItem newItem =
            CommodityGameItemComponentProto.CreateNewCommodity(commodity.Material, weight, commodity.Tag,
                commodity.UseIndirectQuantityDescription, commodity.CommodityCharacteristics.Select(x => (x.Key, x.Value)));
        newItem.RoomLayer = RoomLayer;
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

        dbitem.MorphTimeRemaining = (int)Gameworld.Scheduler.RemainingDuration(this, ScheduleType.Morph).TotalSeconds;
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
            MorphTime = DateTime.UtcNow + CachedMorphTime.Value;
            CachedMorphTime = null;
        }

        if (MorphTime != DateTime.MinValue)
        {
            Gameworld.Scheduler.AddSchedule(new RepeatingSchedule<IGameItem>(this, Gameworld,
                item => item.Changed = true, ScheduleType.MorphSaving, TimeSpan.FromSeconds(30),
                $"Morph Saver for {HowSeen(this)} #{Id}"));
            Gameworld.Scheduler.AddSchedule(new Schedule<IGameItem>(this, Morph, ScheduleType.Morph,
                MorphTime > DateTime.UtcNow ? MorphTime - DateTime.UtcNow : TimeSpan.FromTicks(1),
                $"Morph checker for {HowSeen(this)} #{Id}"));
        }
    }

    public void EndMorphTimer()
    {
        if (MorphTime != DateTime.MinValue)
        {
            Gameworld.Scheduler.Destroy(this, ScheduleType.Morph);
            Gameworld.Scheduler.Destroy(this, ScheduleType.MorphSaving);
            CachedMorphTime = MorphTime - DateTime.UtcNow;
            MorphTime = DateTime.MinValue;
        }
    }

    private void Morph(IGameItem item)
    {
        IGameItem newItem = Prototype.LoadMorphedItem(this);
        ICell location = TrueLocations.FirstOrDefault();
        if (!string.IsNullOrEmpty(Prototype.MorphEmote))
        {
            OutputHandler.Handle(new EmoteOutput(new Emote(Prototype.MorphEmote.SubstituteANSIColour(), this, this, newItem)));
        }

        if (newItem != null)
        {
            InInventoryOf?.SwapInPlace(this, newItem);
            ContainedIn?.SwapInPlace(this, newItem);
            newItem.RoomLayer = RoomLayer;
            Location?.Insert(newItem);
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
                comp.HandleDieOrMorph(newItem, location);
            }

            newItem.Login();
        }

        Delete();
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
        List<IAffectProximity> proximityEffects = EffectsOfType<IAffectProximity>().ToList();
        foreach (IGameItem item in TrueLocations.SelectMany(x => x.GameItems).ToList())
        {
            if (item.RoomLayer != RoomLayer)
            {
                yield return (item, Proximity.VeryDistant);
            }

            if (Cover?.CoverItem?.Parent == item)
            {
                yield return (item, Proximity.Immediate);
            }

            if (PositionTarget == item || item.PositionTarget == this)
            {
                yield return (item, Proximity.Immediate);
            }

            if (InVicinity(item))
            {
                yield return (item, Proximity.Immediate);
            }

            List<(bool Affects, Proximity Proximity)> proximities = proximityEffects.Select(x => x.GetProximityFor(item)).Where(x => x.Affects).ToList();
            if (proximities.Any())
            {
                yield return (item, proximities.Select(x => x.Proximity).Min());
            }

            yield return (item, Proximity.Distant);
        }

        foreach (ICharacter actor in Location.Characters)
        {
            if (actor.RoomLayer != RoomLayer)
            {
                yield return (actor, Proximity.VeryDistant);
            }

            if (PositionTarget == actor || actor.PositionTarget == this)
            {
                yield return (actor, Proximity.Immediate);
            }

            if (InVicinity(actor))
            {
                yield return (actor, Proximity.Immediate);
            }

            List<(bool Affects, Proximity Proximity)> proximities = proximityEffects.Select(x => x.GetProximityFor(actor)).Where(x => x.Affects).ToList();
            if (proximities.Any())
            {
                yield return (actor, proximities.Select(x => x.Proximity).Min());
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
