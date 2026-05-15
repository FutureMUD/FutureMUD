using MudSharp.Body;
using MudSharp.Body.Disfigurements;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Effects.Concrete;
using MudSharp.Events;
using MudSharp.Form.Material;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.Health;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using MudSharp.Work.Butchering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

#nullable enable annotations

namespace MudSharp.GameItems.Components;

public class BodypartGameItemComponent : GameItemComponent, ISeveredBodypart, ILazyLoadDuringIdleTime
{
    protected BodypartGameItemComponentProto _prototype;

    public override bool PreventsMerging(IGameItemComponent component)
    {
        // bodyparts never merge
        return true;
    }

    public override bool WarnBeforePurge => Contents.Any();

    public override IGameItemComponentProto Prototype => _prototype;

    public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
    {
        return new BodypartGameItemComponent(this, newParent, temporary);
    }

    public override int DecorationPriority => 1;

    public override bool DescriptionDecorator(DescriptionType type)
    {
        return type == DescriptionType.Short || type == DescriptionType.Full ||
               type == DescriptionType.Contents;
    }

    public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
        bool colour, PerceiveIgnoreFlags flags)
    {
        if (type == DescriptionType.Contents)
        {
            if (_contents.Any())
            {
                return description + "\n\nIt has the following contents:\n" +
                       (from item in _contents
                        select "\t" + item.HowSeen(voyeur)).ListToString(separator: "\n", conjunction: "",
                           twoItemJoiner: "\n");
            }

            return description;
        }

        if (flags.HasFlag(PerceiveIgnoreFlags.IgnoreLoadThings))
        {
            return "a severed bodypart";
        }

        return Model.DescribeSevered(type, Decay, OriginalCharacter, OriginalBody, voyeur, this,
            EatenWeight / (Model.EdiblePercentage * BodypartWeight));
    }

    #region Overrides of GameItemComponent

    public override bool WrapFullDescription => false;

    #endregion

    public override void FinaliseLoad()
    {
        foreach (IGameItem item in _contents)
        {
            item.FinaliseLoadTimeTasks();
        }

        foreach (IGameItem item in _implants)
        {
            item.FinaliseLoadTimeTasks();
        }
    }

    protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
    {
        _prototype = (BodypartGameItemComponentProto)newProto;
    }

    protected override string SaveToXml()
    {
        return new XElement("Definition",
            new XElement("OriginalCharacterId", OriginalCharacterId),
            new XElement("OriginalBodyId", OriginalBodyId),
            new XElement("Model", Model?.Id ?? 0),
            new XElement("DecayPoints", DecayPoints),
            new XElement("DecayState", (int)Decay),
            new XElement("EatenWeight", EatenWeight),
            new XElement("Parts",
                from item in Parts
                select new XElement("Part", item.Id)
            ),
            new XElement("Bones",
                from item in Bones
                select new XElement("Bone", item.Id)
            ),
            new XElement("Contents",
                from item in Contents
                select new XElement("Item", item.Id)
            ),
            new XElement("Implants",
                from item in _implants
                select new XElement("Implant", item.Id)
            ),
            new XElement("Wounds",
                from item in Wounds
                select new XElement("Wound", item.Id)
            ),
            new XElement("Tattoos",
                from item in Tattoos
                select item.SaveToXml()
            ),
            new XElement("ButcheredSubcategories",
                from item in ButcheredSubcategories
                select new XElement("Subcategory", item)
            )
        ).ToString();
    }

    private void LoadFromXml(XElement definition)
    {
        OriginalCharacterId = long.Parse(definition.Element("OriginalCharacterId").Value);
        _originalBodyId = long.Parse(definition.Element("OriginalBodyId")?.Value ?? "0");

        Model = Gameworld.CorpseModels.Get(long.Parse(definition.Element("Model").Value));
        DecayPoints = double.Parse(definition.Element("DecayPoints").Value);
        Decay = (DecayState)int.Parse(definition.Element("DecayState").Value);
        EatenWeight = double.Parse(definition.Element("EatenWeight")?.Value ?? "0.0");
        foreach (XElement item in definition.Element("Parts").Elements())
        {
            _parts.Add(Gameworld.BodypartPrototypes.Get(long.Parse(item.Value)));
        }

        foreach (XElement item in definition.Element("Bones")?.Elements() ?? Enumerable.Empty<XElement>())
        {
            _bones.Add((IBone)Gameworld.BodypartPrototypes.Get(long.Parse(item.Value)));
        }

        foreach (IOrganProto organ in _parts.OfType<IOrganProto>())
        {
            _organs.Add(organ);
        }

        RootPart = _parts.Count > 1
            ? _parts.First(x => !(x is IOrganProto) && _parts.All(y => x.UpstreamConnection != y))
            : _parts.Single();
        foreach (XElement item in definition.Element("Contents").Elements())
        {
            IGameItem newItem = Gameworld.TryGetItem(long.Parse(item.Value), true);
            newItem.LoadTimeSetContainedIn(Parent);
            _contents.Add(newItem);
        }

        foreach (XElement item in definition.Element("Implants")?.Elements() ?? Enumerable.Empty<XElement>())
        {
            IGameItem newItem = Gameworld.TryGetItem(long.Parse(item.Value), true);
            newItem.LoadTimeSetContainedIn(Parent);
            _implants.Add(newItem);
        }

        foreach (XElement wound in definition.Element("Wounds")?.Elements() ?? Enumerable.Empty<XElement>())
        {
            _woundIDs.Add(long.Parse(wound.Value));
        }

        foreach (XElement tattoo in definition.Element("Tattoos")?.Elements() ?? Enumerable.Empty<XElement>())
        {
            _tattoos.Add(new Tattoo(tattoo, Gameworld));
        }

        _butcheredSubcategories.AddRange(
            definition.Element("ButcheredSubcategories")?.Elements("Subcategory")
                      .Select(x => x.Value.NormaliseButcherySubcategory())
                      .Where(x => !string.IsNullOrWhiteSpace(x)) ?? Enumerable.Empty<string>());
    }

    #region Constructors

    public BodypartGameItemComponent(BodypartGameItemComponentProto proto, IGameItem parent, bool temporary = false)
        : base(parent, proto, temporary)
    {
        _prototype = proto;
        Decay = DecayState.Fresh;
        if (!temporary)
        {
            SetupDecayListener();
        }
    }

    public BodypartGameItemComponent(MudSharp.Models.GameItemComponent component, BodypartGameItemComponentProto proto,
        IGameItem parent) : base(component, parent)
    {
        _prototype = proto;
        _noSave = true;
        LoadFromXml(XElement.Parse(component.Definition));
        Gameworld.SaveManager.AddLazyLoad(this);
        _noSave = false;
        SetupDecayListener();
    }

    public BodypartGameItemComponent(BodypartGameItemComponent rhs, IGameItem newParent, bool temporary = false) : base(
        rhs, newParent, temporary)
    {
        _prototype = rhs._prototype;
        if (!temporary)
        {
            SetupDecayListener();
        }

        _contents = rhs._contents.ToList();
        _implants = rhs._implants.ToList();
        Wounds = rhs.Wounds.ToList();
        Tattoos = rhs.Tattoos.ToList();
        _butcheredSubcategories.AddRange(rhs._butcheredSubcategories);
        OriginalCharacterId = rhs.OriginalCharacterId;
        _originalCharacter = rhs._originalCharacter;
        _originalBodyId = rhs._originalBodyId;
        _originalBody = rhs._originalBody;
    }

    private void SetupBodypartWeight()
    {
        _bodypartWeight = OriginalBody.Weight * _parts.Sum(x => x.RelativeHitChance) /
                          (OriginalBody.Bodyparts.Sum(x => x.RelativeHitChance) +
                           _parts.Sum(x => x.RelativeHitChance)) +
                          Wounds.Sum(x => x.Lodged?.Weight ?? 0.0);
    }

    private void SetupDecayListener()
    {
        Gameworld.HeartbeatManager.MinuteHeartbeat += HeartbeatManagerOnMinuteHeartbeat;
    }

    private void HeartbeatManagerOnMinuteHeartbeat()
    {
        // TODO - check for effects that halt or arrest decay
        DecayPoints += Model.DecayRate(
            Parent.Location != null
                ? Parent.Location.CurrentOverlay.Terrain
                : Parent.TrueLocations.FirstOrDefault()?.CurrentOverlay.Terrain
        );
    }

    public override void Delete()
    {
        base.Delete();
        foreach (IGameItem item in Contents.ToList())
        {
            _contents.Remove(item);
            item.ContainedIn = null;
            item.Delete();
        }

        foreach (IGameItem item in _implants.ToList())
        {
            _implants.Remove(item);
            item.Delete();
        }

        foreach (IWound wound in Wounds.ToList())
        {
            wound.Delete();
            _wounds.Remove(wound);
        }

        _tattoos.Clear();
        Gameworld.HeartbeatManager.MinuteHeartbeat -= HeartbeatManagerOnMinuteHeartbeat;
    }

    public override void Quit()
    {
        base.Quit();
        foreach (IGameItem item in Contents.ToList())
        {
            item.Quit();
        }

        foreach (IGameItem item in _implants.ToList())
        {
            item.Quit();
        }

        foreach (IWound wound in Wounds.ToList())
        {
            wound.Lodged?.Quit();
        }

        Gameworld.HeartbeatManager.MinuteHeartbeat -= HeartbeatManagerOnMinuteHeartbeat;
    }

    public override void Login()
    {
        base.Login();
        foreach (IGameItem item in Contents.ToList())
        {
            item.Login();
        }

        foreach (IGameItem item in _implants.ToList())
        {
            item.Login();
        }

        foreach (IWound wound in Wounds.ToList())
        {
            wound.Lodged?.Login();
        }
    }

    private double _bodypartWeight;

    public double BodypartWeight
    {
        get
        {
            if (_bodypartWeight == 0)
            {
                SetupBodypartWeight();
            }

            return _bodypartWeight;
        }
    }

    public override double ComponentWeight => Contents.Sum(x => x.Weight) + BodypartWeight - EatenWeight;

    public override double ComponentBuoyancy(double fluidDensity)
    {
        return Contents.Sum(x => x.Buoyancy(fluidDensity)) + (BodypartWeight - EatenWeight) * (fluidDensity - 1.01);
    }

    public override bool SwapInPlace(IGameItem existingItem, IGameItem newItem)
    {
        if (_contents.Contains(existingItem))
        {
            _contents[_contents.IndexOf(existingItem)] = newItem;
            newItem.ContainedIn = Parent;
            Changed = true;
            existingItem.ContainedIn = null;
            return true;
        }

        if (_implants.Contains(existingItem))
        {
            _implants[_implants.IndexOf(existingItem)] = newItem;
            newItem.ContainedIn = Parent;
            Changed = true;
            existingItem.ContainedIn = null;
            return true;
        }

        return false;
    }

    public override bool HandleDieOrMorph(IGameItem newItem, ICell location)
    {
        IContainer newItemContainer = newItem?.GetItemType<IContainer>();
        if (newItemContainer != null)
        {
            if (Contents.Any())
            {
                foreach (IGameItem item in Contents)
                {
                    if (newItemContainer.CanPut(item))
                    {
                        newItemContainer.Put(null, item);
                    }
                    else
                    {
                        if (location != null)
                        {
                            location.Insert(item);
                            item.ContainedIn = null;
                        }
                        else
                        {
                            item.Delete();
                        }
                    }
                }

                _contents.Clear();
            }

            if (_implants.Any())
            {
                foreach (IGameItem item in _implants)
                {
                    if (newItemContainer.CanPut(item))
                    {
                        newItemContainer.Put(null, item);
                    }
                    else
                    {
                        if (location != null)
                        {
                            location.Insert(item);
                            item.ContainedIn = null;
                        }
                        else
                        {
                            item.Delete();
                        }
                    }
                }

                _implants.Clear();
            }
        }
        else
        {
            foreach (IGameItem item in Contents)
            {
                if (location != null)
                {
                    location.Insert(item);
                }
                else
                {
                    item.Delete();
                }
            }

            foreach (IGameItem item in _implants)
            {
                if (location != null)
                {
                    location.Insert(item);
                    item.ContainedIn = null;
                }
                else
                {
                    item.Delete();
                }
            }

            _contents.Clear();
            _implants.Clear();
        }

        return false;
    }

    public override bool Take(IGameItem item)
    {
        if (Contents.Contains(item))
        {
            _contents.Remove(item);
            Changed = true;
            return true;
        }

        if (_implants.Contains(item))
        {
            _implants.Remove(item);
            Changed = true;
            return true;
        }

        return false;
    }

    #endregion

    #region ISeveredBodypart Members

    private List<IGameItem> _implants = new();

    private HashSet<IBodypart> _parts = new();

    private HashSet<IBone> _bones = new();

    private HashSet<IOrganProto> _organs = new();

    public IEnumerable<IBodypart> Parts
    {
        get => _parts;
        set
        {
            _parts = value.ToHashSet();
            RootPart = _parts.First(x => _parts.All(y => x.UpstreamConnection != y));
        }
    }

    public IEnumerable<IBone> Bones
    {
        get => _bones;
        set => _bones = value.ToHashSet();
    }

    public IEnumerable<IOrganProto> Organs
    {
        get => _organs;
        set => _organs = value.ToHashSet();
    }

    private List<IWound> _wounds = new();

    public IEnumerable<IWound> Wounds
    {
        get => _wounds;
        set => _wounds = value.ToList();
    }

    private List<ITattoo> _tattoos = new();

    public IEnumerable<ITattoo> Tattoos
    {
        get => _tattoos;
        set => _tattoos = value.ToList();
    }

    public IEnumerable<IGameItem> Implants
    {
        get => _implants;
        set => _implants = value.ToList();
    }

    private double _eatenWeight;

    public double EatenWeight
    {
        get => _eatenWeight;
        set
        {
            _eatenWeight = value;
            Changed = true;
        }
    }

    public double RemainingEdibleWeight => BodypartWeight * Model.EdiblePercentage - EatenWeight;

    public void SeveredBodypartWasInstalledInABody()
    {
        _tattoos.Clear();
        _implants.Clear();
        _contents.Clear();
        _wounds.Clear();
    }

    public IBodypart RootPart { get; set; }

    public long OriginalCharacterId { get; set; }
    private ICharacter _originalCharacter;
    private long _originalBodyId;
    private IBody _originalBody;
    private List<long> _woundIDs = new();

    private void LoadOriginalReferences(bool viaSaveManager)
    {
        _originalCharacter = Gameworld.TryGetCharacter(OriginalCharacterId, true);
        _originalBody = Gameworld.Bodies.Get(_originalBodyId) ??
                        _originalCharacter?.Bodies.FirstOrDefault(x => x.Id == _originalBodyId);
        if (_originalBody == null && _originalCharacter != null)
        {
            _originalBody = _originalCharacter.Body;
            _originalBodyId = _originalBody.Id;
        }

        if (!viaSaveManager)
        {
            Gameworld.SaveManager.AbortLazyLoad(this);
        }

        if (_woundIDs.Any())
        {
            using (new FMDB())
            {
                List<Models.Wound> wounds = FMDB.Context.Wounds.Where(x => _woundIDs.Contains(x.Id)).ToList();
                _wounds.AddRange(wounds.Select(x => WoundFactory.LoadWound(x, Parent, Gameworld)));
            }

            _woundIDs.Clear();
        }
    }

    public ICharacter OriginalCharacter
    {
        get
        {
            if (_originalCharacter == null)
            {
                LoadOriginalReferences(false);
            }

            return _originalCharacter;
        }
        set
        {
            _originalCharacter = value;
            OriginalCharacterId = value?.Id ?? 0;
            if (_originalBody == null && value != null)
            {
                OriginalBody = value.Body;
            }
            Changed = true;
        }
    }

    public long OriginalBodyId => _originalBodyId;

    public IBody OriginalBody
    {
        get
        {
            if (_originalBody == null)
            {
                LoadOriginalReferences(false);
            }

            return _originalBody;
        }
        set
        {
            _originalBody = value;
            _originalBodyId = value?.Id ?? 0;
            Changed = true;
        }
    }

    void ILazyLoadDuringIdleTime.DoLoad()
    {
        LoadOriginalReferences(true);
    }

    public ICorpseModel Model { get; set; }

    private double _decayPoints;
    private double _nextSaveDecayPoints;

    public double DecayPoints
    {
        get => _decayPoints;
        set
        {
            if (value == _decayPoints)
            {
                return;
            }

            _decayPoints = value;
            Decay = Model.GetDecayState(_decayPoints);
            if (_nextSaveDecayPoints == 0)
            {
                _nextSaveDecayPoints = _decayPoints + 50 + Constants.Random.NextDouble() * 100.0;
            }

            if (_decayPoints >= _nextSaveDecayPoints)
            {
                //We only save body part decay point changes when the decayPoints have changed
                //by between 50-150 points in order to avoid hammering the save manager with
                //every severed body part all at once. 
                _nextSaveDecayPoints = _decayPoints + 50 + Constants.Random.NextDouble() * 100.0;
                Changed = true;
            }
        }
    }

    public DecayState Decay { get; set; }

    private readonly List<string> _butcheredSubcategories = new();
    public IEnumerable<string> ButcheredSubcategories => _butcheredSubcategories;

    public bool Butcher(ICharacter butcher, string subcategory = null)
    {
        StringBuilder productSB = new();
        List<IGameItem> products = new();
        int count = 0;

        void LoadItem(IGameItemProto proto, int quantity)
        {
            if (proto.Components.Any(x => x is StackableGameItemComponentProto))
            {
                IGameItem newItem = proto.CreateNew(butcher);
                newItem.RoomLayer = Parent.RoomLayer;
                newItem.GetItemType<IStackable>().Quantity = quantity;
                Gameworld.Add(newItem);
                butcher.Location.Insert(newItem);
                newItem.HandleEvent(EventType.ItemFinishedLoading, newItem);
                newItem.Login();
                productSB.AppendLine($"\t${count++} has been produced.");
                products.Add(newItem);
                return;
            }

            for (int i = 0; i < quantity; i++)
            {
                IGameItem newItem = proto.CreateNew(butcher);
                newItem.RoomLayer = Parent.RoomLayer;
                Gameworld.Add(newItem);
                butcher.Location.Insert(newItem);
                newItem.HandleEvent(EventType.ItemFinishedLoading, newItem);
                newItem.Login();
                productSB.AppendLine($"\t${count++} has been produced.");
                products.Add(newItem);
            }
        }

        ICheck check = Gameworld.GetCheck(CheckType.ButcheryCheck);
        Butchering effect = butcher.EffectsOfType<Butchering>().First();

        foreach (IButcheryProduct product in OriginalCharacter.Race.ButcheryProfile.Products.Where(x =>
                     !x.IsPelt && x.AppliesTo(this) && x.CanProduce(butcher, Parent) &&
                     x.MatchesButcherySubcategory(subcategory, ButcheredSubcategories)))
        {
            List<IBodypart> productParts = product.MatchingBodyparts(this).ToList();
            double totalHitpoints = productParts.Sum(x => OriginalBody.HitpointsForBodypart(x));
            double damageRatio = totalHitpoints <= 0.0
                ? 1.0
                : Parent.Wounds.Where(x => productParts.Any(y => x.Bodypart.ButcheryBodypartMatches(y)))
                        .Sum(x => x.CurrentDamage) /
                  totalHitpoints;

            foreach (IButcheryProductItem item in product.ProductItems)
            {
                CheckOutcome result = check.Check(butcher, effect.CheckDifficulty, effect.Trait);
                if (ButcheryExtensions.ShouldUseDamagedButcheryProduct(damageRatio, item.DamagedThreshold,
	                    result.Outcome))
                {
                    if (item.DamagedProto != null)
                    {
                        LoadItem(item.DamagedProto, item.DamagedQuantity);
                        continue;
                    }
                }

                LoadItem(item.NormalProto, item.NormalQuantity);
            }

            List<IBodypart> affectedParts = Parts
                                                 .Where(x => product.RequiredBodyparts.Any(y =>
                                                     x.DownstreamOfPart(y) || x.ButcheryBodypartMatches(y))).ToList();

            foreach (IGameItem item in _implants.Where(x => affectedParts.Contains(x.GetItemType<IImplant>()?.TargetBodypart))
                                          .ToList())
            {
                _implants.Remove(item);
                butcher.Location.Insert(item);
            }

            foreach (ITattoo tattoo in Tattoos.Where(x => affectedParts.Contains(x.Bodypart)).ToList())
            {
                _tattoos.Remove(tattoo);
            }

            foreach (IWound wound in Wounds.Where(x => affectedParts.Contains(x.Bodypart)).ToList())
            {
                _wounds.Remove(wound);
                wound.Delete();
            }
        }

        if (productSB.Length > 0)
        {
            Parent.OutputHandler.Handle(
                new EmoteOutput(new Emote(productSB.ToString(), Parent, products.ToArray<IPerceivable>())));
        }

        if (!string.IsNullOrEmpty(subcategory))
        {
            _butcheredSubcategories.Add(subcategory.NormaliseButcherySubcategory());
            Changed = true;
            return false;
        }

        foreach (IGameItem item in Contents.ToList())
        {
            butcher.Location.Insert(item);
        }

        foreach (IGameItem item in _implants.ToList())
        {
            butcher.Location.Insert(item);
        }

        _contents.Clear();
        _implants.Clear();
        return true;
    }

    public void Skin(ICharacter skinner)
    {
    }

    #endregion

    #region Implementation of IContainer

    private List<IGameItem> _contents = new();

    public IEnumerable<IGameItem> Contents
    {
        get => _contents;
        set => _contents = value.ToList();
    }

    public string ContentsPreposition { get; } = "in";
    public bool Transparent { get; } = true;

    public bool CanPut(IGameItem item)
    {
        return false;
    }

    public int CanPutAmount(IGameItem item)
    {
        return 0;
    }

    public void Put(ICharacter? putter, IGameItem item, bool allowMerge = true)
    {
        throw new NotSupportedException("You cannot put things into severed bodyparts.");
    }

    public WhyCannotPutReason WhyCannotPut(IGameItem item)
    {
        return WhyCannotPutReason.NotContainer;
    }

    public bool CanTake(ICharacter taker, IGameItem item, int quantity)
    {
        return _contents.Contains(item) && item.CanGet(quantity).AsBool();
    }

    public IGameItem Take(ICharacter taker, IGameItem item, int quantity)
    {
        Changed = true;
        if (quantity == 0 || item.DropsWhole(quantity))
        {
            _contents.Remove(item);
            item.ContainedIn = null;
            return item;
        }

        return item.Get(null, quantity);
    }

    public WhyCannotGetContainerReason WhyCannotTake(ICharacter taker, IGameItem item)
    {
        return !_contents.Contains(item)
            ? WhyCannotGetContainerReason.NotContained
            : WhyCannotGetContainerReason.NotContainer;
    }

    public void Empty(ICharacter emptier, IContainer intoContainer, IEmote? playerEmote = null)
    {
        ICell location = emptier?.Location ?? Parent.TrueLocations.FirstOrDefault();
        List<IGameItem> contents = Contents.ToList();
        _contents.Clear();
        if (emptier is not null)
        {
            if (intoContainer == null)
            {
                emptier.OutputHandler.Handle(
                    new MixedEmoteOutput(new Emote("@ empty|empties $0 onto the ground.", emptier, Parent)).Append(
                        playerEmote));
            }
            else
            {
                emptier.OutputHandler.Handle(
                    new MixedEmoteOutput(new Emote($"@ empty|empties $1 {intoContainer.ContentsPreposition}to $2.",
                        emptier, emptier, Parent, intoContainer.Parent)).Append(playerEmote));
            }
        }

        foreach (IGameItem item in contents)
        {
            item.ContainedIn = null;
            if (intoContainer != null)
            {
                if (intoContainer.CanPut(item))
                {
                    intoContainer.Put(emptier, item);
                }
                else if (location != null)
                {
                    location.Insert(item);
                    emptier?.OutputHandler.Handle(new EmoteOutput(new Emote(
                            "@ cannot put $1 into $2, so #0 set|sets it down on the ground.", emptier, emptier, item,
                            intoContainer.Parent)));
                }
                else
                {
                    item.Delete();
                }

                continue;
            }

            if (location != null)
            {
                location.Insert(item);
            }
            else
            {
                item.Delete();
            }
        }

        Changed = true;
    }

    #endregion

    public override bool OverridesMaterial => OverridenMaterial != null;

    public override ISolid OverridenMaterial => Model.CorpseMaterial(_decayPoints);
}
