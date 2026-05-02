using MudSharp.Accounts;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MudSharp.GameItems.Components;

public class CorpseGameItemComponent : GameItemComponent, ICorpse, ILazyLoadDuringIdleTime
{
    protected CorpseGameItemComponentProto _prototype;

    public override bool PreventsMerging(IGameItemComponent component)
    {
        // Corpses never merge
        return true;
    }

    public override bool WarnBeforePurge => OriginalBody.AllItems.Any();

    public override IGameItemComponentProto Prototype => _prototype;

    public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
    {
        return new CorpseGameItemComponent(this, newParent, temporary);
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
        if (flags.HasFlag(PerceiveIgnoreFlags.IgnoreLoadThings))
        {
            return "a corpse";
        }

        return Model.Describe(type, Decay, OriginalCharacter, OriginalBody, voyeur,
                   EatenWeight / (Model.EdiblePercentage * OriginalBody.Weight)) +
               (Skinned && type == DescriptionType.Short ? " (Skinned)".Colour(Telnet.Red) : "");
    }

    public override double ComponentWeight => OriginalBody.Weight + OriginalBody.ExternalItems.Sum(x => x.Weight) +
                OriginalBody.Implants.Sum(x => x.Parent.Weight) +
                OriginalBody.Prosthetics.Sum(x => x.Parent.Weight) - EatenWeight;

    public override double ComponentBuoyancy(double fluidDensity)
    {
        return (fluidDensity - 1.01) * (OriginalBody.Weight - EatenWeight) +
               OriginalBody.AllItems.Sum(x => x.Buoyancy(fluidDensity));
    }

    public override bool OverridesMaterial => OverridenMaterial != null;

    public override ISolid OverridenMaterial => Model.CorpseMaterial(_decayPoints);

    #region Overrides of GameItemComponent

    public override bool WrapFullDescription => false;

    #endregion

    protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
    {
        _prototype = (CorpseGameItemComponentProto)newProto;
    }

    protected override string SaveToXml()
    {
        return new XElement("Definition",
            new XElement("OriginalCharacter", _originalCharacterId),
            new XElement("OriginalBody", _originalBodyId),
            new XElement("Model", Model?.Id ?? 0),
            new XElement("DecayPoints", DecayPoints),
            new XElement("DecayState", (int)Decay),
            new XElement("EatenWeight", EatenWeight),
            new XElement("Skinned", Skinned),
            new XElement("ButcheredSubcategories",
                from item in ButcheredSubcategories
                select new XElement("Subcategory", item)
            ),
            new XElement("TimeOfDeath", new XText(TimeOfDeath.ToString(CultureInfo.InvariantCulture)))
        ).ToString();
    }

    private void LoadFromXml(XElement definition)
    {
        _originalCharacterId = long.Parse(definition.Element("OriginalCharacter").Value);
        _originalBodyId = long.Parse(definition.Element("OriginalBody")?.Value ?? "0");
        Model = Gameworld.CorpseModels.Get(long.Parse(definition.Element("Model").Value));
        DecayPoints = double.Parse(definition.Element("DecayPoints").Value);
        Decay = (DecayState)int.Parse(definition.Element("DecayState").Value);
        TimeOfDeath = DateTime.Parse(definition.Element("TimeOfDeath").Value, CultureInfo.InvariantCulture,
            DateTimeStyles.AssumeUniversal);
        EatenWeight = double.Parse(definition.Element("EatenWeight")?.Value ?? "0.0");
        _skinned = bool.Parse(definition.Element("Skinned")?.Value ?? "false");
        _butcheredSubcategories.AddRange(
            definition.Element("ButcheredSubcategories")?.Elements("Subcategory")
                      .Select(x => x.Value.NormaliseButcherySubcategory())
                      .Where(x => !string.IsNullOrWhiteSpace(x)) ?? Enumerable.Empty<string>());
    }

    #region Constructors

    public CorpseGameItemComponent(CorpseGameItemComponentProto proto, IGameItem parent, bool temporary = false)
        : base(parent, proto, temporary)
    {
        _prototype = proto;
        Decay = DecayState.Fresh;
        TimeOfDeath = DateTime.UtcNow;
        if (!temporary)
        {
            SetupDecayListener();
        }
    }

    public CorpseGameItemComponent(MudSharp.Models.GameItemComponent component, CorpseGameItemComponentProto proto,
        IGameItem parent) : base(component, parent)
    {
        _prototype = proto;
        _noSave = true;
        LoadFromXml(XElement.Parse(component.Definition));
        Gameworld.SaveManager.AddLazyLoad(this);
        _noSave = false;
        SetupDecayListener();
    }

    public CorpseGameItemComponent(CorpseGameItemComponent rhs, IGameItem newParent, bool temporary = false) : base(rhs,
        newParent, temporary)
    {
        _prototype = rhs._prototype;
        _originalCharacterId = rhs._originalCharacterId;
        _originalCharacter = rhs._originalCharacter;
        _originalBodyId = rhs._originalBodyId;
        _originalBody = rhs._originalBody;
        Model = rhs.Model;
        Decay = rhs.Decay;
        DecayPoints = rhs.DecayPoints;
        TimeOfDeath = rhs.TimeOfDeath;
        Skinned = rhs.Skinned;
        _butcheredSubcategories.AddRange(rhs._butcheredSubcategories);
        if (!temporary)
        {
            SetupDecayListener();
        }
    }

    private void SetupDecayListener()
    {
        Gameworld.HeartbeatManager.MinuteHeartbeat += HeartbeatManagerOnMinuteHeartbeat;
    }

    private void HeartbeatManagerOnMinuteHeartbeat()
    {
        if (Parent.AffectedBy<ICorpsePreservationEffect>())
        {
            return;
        }

        if (!Parent.TrueLocations.Any())
        {
            Console.WriteLine("Corpse did not have any true location.");
            return;
        }

        DecayPoints += Model.DecayRate(
            Parent.Location != null
                ? Parent.Location.CurrentOverlay.Terrain
                : Parent.TrueLocations.First().CurrentOverlay.Terrain
        );
    }

    public override void Delete()
    {
        OriginalCharacter.Corpse = null;
        base.Delete();
        Gameworld.HeartbeatManager.MinuteHeartbeat -= HeartbeatManagerOnMinuteHeartbeat;
        // If a corpse is deleted and its owner is still dead (i.e. hasn't been resurrected), delete the inventory
        if (OriginalCharacter.Status == CharacterStatus.Deceased)
        {
            foreach (IGameItem item in OriginalBody.ExternalItems.ToList())
            {
                item.Delete();
            }
        }
    }

    public override void Quit()
    {
        base.Quit();
        Gameworld.HeartbeatManager.MinuteHeartbeat -= HeartbeatManagerOnMinuteHeartbeat;
    }

    public override bool Take(IGameItem item)
    {
        if (OriginalBody.ExternalItems.Contains(item))
        {
            OriginalBody.Take(item);
            return true;
        }

        return false;
    }

    #endregion

    #region ICorpse Members

    private long _originalCharacterId;
    private ICharacter _originalCharacter;
    private long _originalBodyId;
    private IBody _originalBody;

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

    public double RemainingEdibleWeight => OriginalBody.Weight * Model.EdiblePercentage - EatenWeight;

    private void LoadOriginalReferences(bool viaSaveManager)
    {
        _originalCharacter = Gameworld.TryGetCharacter(_originalCharacterId, true);
        _originalCharacter?.Corpse = this;
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
            _originalCharacterId = value?.Id ?? 0;
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

    public DateTime TimeOfDeath { get; set; }

    private bool _skinned;

    public bool Skinned
    {
        get => _skinned;
        set
        {
            _skinned = value;
            Changed = true;
        }
    }

    public IEnumerable<IBodypart> Parts => OriginalBody.Bodyparts;

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
                     (string.IsNullOrEmpty(subcategory) || x.Subcategory.EqualTo(subcategory))))
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
                if (damageRatio >= item.DamagedThreshold || result.Outcome.In(Outcome.MajorFail, Outcome.Fail))
                {
                    if (item.DamagedProto != null)
                    {
                        LoadItem(item.DamagedProto, item.DamagedQuantity);
                        continue;
                    }
                }

                LoadItem(item.NormalProto, item.NormalQuantity);
            }

            foreach (IBodypart part in product.RequiredBodyparts)
            {
                foreach (IGameItem item in OriginalBody.AllItemsAtOrDownstreamOfPart(part).ToList())
                {
                    OriginalBody.Take(item);
                    butcher.Location.Insert(item);
                }
            }
        }

        if (productSB.Length > 0)
        {
            Parent.OutputHandler.Handle(
                new EmoteOutput(new Emote(productSB.ToString(), Parent, products.ToArray<IPerceivable>()), style: OutputStyle.NoNewLine));
        }

        if (!string.IsNullOrEmpty(subcategory))
        {
            _butcheredSubcategories.Add(subcategory.NormaliseButcherySubcategory());
            Changed = true;
            return false;
        }

        foreach (IGameItem item in OriginalBody.AllItems.ToList())
        {
            OriginalBody.Take(item);
            butcher.Location.Insert(item);
        }

        return true;
    }

    public void Skin(ICharacter skinner)
    {
        void LoadItem(IGameItemProto proto, int quantity)
        {
            if (proto.Components.Any(x => x is StackableGameItemComponentProto))
            {
                IGameItem newItem = proto.CreateNew(skinner);
                newItem.RoomLayer = Parent.RoomLayer;
                newItem.GetItemType<IStackable>().Quantity = quantity;
                Gameworld.Add(newItem);
                skinner.Location.Insert(newItem);
                newItem.HandleEvent(EventType.ItemFinishedLoading, newItem);
                newItem.Login();
                return;
            }

            for (int i = 0; i < quantity; i++)
            {
                IGameItem newItem = proto.CreateNew(skinner);
                newItem.RoomLayer = Parent.RoomLayer;
                Gameworld.Add(newItem);
                skinner.Location.Insert(newItem);
                newItem.HandleEvent(EventType.ItemFinishedLoading, newItem);
                newItem.Login();
            }
        }

        ICheck check = Gameworld.GetCheck(CheckType.SkinningCheck);
        Skinning effect = skinner.EffectsOfType<Skinning>().First();
        foreach (IButcheryProduct product in OriginalCharacter.Race.ButcheryProfile.Products.Where(x =>
                     x.IsPelt && x.AppliesTo(this) && x.CanProduce(skinner, Parent)))
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
                CheckOutcome result = check.Check(skinner, effect.CheckDifficulty, effect.Trait);
                if (damageRatio >= item.DamagedThreshold || result.Outcome.In(Outcome.MajorFail, Outcome.Fail))
                {
                    if (damageRatio >= item.DamagedThreshold)
                    {
                        if (item.DamagedProto != null)
                        {
                            LoadItem(item.DamagedProto, item.DamagedQuantity);
                            continue;
                        }
                    }
                }

                LoadItem(item.NormalProto, item.NormalQuantity);
            }
        }

        Skinned = true;
    }

    void ILazyLoadDuringIdleTime.DoLoad()
    {
        LoadOriginalReferences(true);
    }

    IBody IHaveABody.Body => OriginalBody;

    double IHaveWeight.Weight
    {
        set => OriginalBody.Weight = value;
        get => OriginalBody.Weight;
    }

    double IHaveHeight.Height
    {
        set => OriginalBody.Height = value;
        get => OriginalBody.Height;
    }

    Alignment IHaveABody.Handedness => OriginalBody.Handedness;
    #endregion

    #region IOverrideItemWoundBehaviour Implementation

    public IHealthStrategy HealthStrategy => OriginalBody.HealthStrategy;
    public IEnumerable<IWound> Wounds => OriginalBody.Wounds;

    public IEnumerable<IWound> VisibleWounds(IPerceiver voyeur, WoundExaminationType examinationType)
    {
        return OriginalBody.VisibleWounds(voyeur, examinationType);
    }

    public IEnumerable<IWound> SufferDamage(IDamage damage)
    {
        return OriginalBody.SufferDamage(damage);
    }

    public IEnumerable<IWound> PassiveSufferDamage(IDamage damage)
    {
        return OriginalBody.PassiveSufferDamage(damage);
    }

    public IEnumerable<IWound> PassiveSufferDamage(IExplosiveDamage damage, Proximity proximity, Facing facing)
    {
        return OriginalBody.PassiveSufferDamage(damage, proximity, facing);
    }

    public void ProcessPassiveWound(IWound wound)
    {
        OriginalBody.ProcessPassiveWound(wound);
    }

    public WoundSeverity GetSeverityFor(IWound wound)
    {
        return OriginalBody.GetSeverityFor(wound);
    }

    public double GetSeverityFloor(WoundSeverity severity, bool usePercentageModel = false)
    {
        return OriginalBody.GetSeverityFloor(severity, usePercentageModel);
    }

    public void EvaluateWounds()
    {
        OriginalBody.EvaluateWounds();
    }

    public void CureAllWounds()
    {
        OriginalBody.CureAllWounds();
    }

    public void StartHealthTick(bool initial = false)
    {
        OriginalBody.StartHealthTick(initial);
    }

    public void EndHealthTick()
    {
        OriginalBody.EndHealthTick();
    }

    public void AddWound(IWound wound)
    {
        OriginalBody.AddWound(wound);
    }

    public void AddWounds(IEnumerable<IWound> wounds)
    {
        OriginalBody.AddWounds(wounds);
    }

    #endregion
}
