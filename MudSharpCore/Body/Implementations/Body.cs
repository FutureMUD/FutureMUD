using Microsoft.EntityFrameworkCore;
using MudSharp.Body.Disfigurements;
using MudSharp.Body.Needs;
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Body.Traits;
using MudSharp.Body.Traits.Subtypes;
using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.Character.Name;
using MudSharp.CharacterCreation;
using MudSharp.CharacterCreation.Roles;
using MudSharp.Climate;
using MudSharp.Combat;
using MudSharp.Communication.Language;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Database;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Events;
using MudSharp.Form.Characteristics;
using MudSharp.Form.Material;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.Health;
using MudSharp.Health.Breathing;
using MudSharp.Health.Infections;
using MudSharp.Models;
using MudSharp.Movement;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Merits;
using MudSharp.RPG.Merits.CharacterMerits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Attribute = MudSharp.Body.Traits.Subtypes.Attribute;
using TraitDefinition = MudSharp.Body.Traits.TraitDefinition;

namespace MudSharp.Body.Implementations;

public partial class Body : PerceiverItem, IBody
{
    protected double _height;

    protected double _weight;

    public Body(IFuturemud gameworld, ICharacter character, ICharacterTemplate template)
    {
        _noSave = true;
        Gameworld = gameworld;
        _height = template.SelectedHeight;
        _weight = template.SelectedWeight;
        Actor = character;
        Race = template.SelectedRace;
        _breathingStrategy = Race.BreathingStrategy;
        Ethnicity = template.SelectedEthnicity;
        Prototype = template.SelectedRace.BaseBody;
        Gender = Gendering.Get(template.SelectedGender);
        Handedness = template.Handedness;
        _shortDescription = template.SelectedSdesc;
        _fullDescription = template.SelectedFullDesc;
        _shortDescriptionPattern =
            template.SelectedEntityDescriptionPatterns.FirstOrDefault(
                x => x.Type == EntityDescriptionType.ShortDescription);
        _fullDescriptionPattern =
            template.SelectedEntityDescriptionPatterns.FirstOrDefault(
                x => x.Type == EntityDescriptionType.FullDescription);

        foreach (IBodypart part in template.MissingBodyparts)
        {
            _severedRoots.Add(part);
        }

        foreach (IGameItemProto prosthetic in template.SelectedProstheses)
        {
            IGameItem item = prosthetic.CreateNew(null);
            IProsthetic pitem = item.GetItemType<IProsthetic>();
            if (pitem == null)
            {
                continue;
            }

            _prosthetics.Add(pitem);
        }

        foreach (KeyValuePair<IPositionState, IMoveSpeed> speed in Prototype.DefaultSpeeds)
        {
            CurrentSpeeds[speed.Key] = speed.Value;
        }

        foreach (ITrait attribute in template.SelectedAttributes)
        {
            _traits.Add(TraitFactory.LoadAttribute((IAttributeDefinition)attribute.Definition, this, attribute.Value));
        }

        foreach (IAttributeDefinition attribute in Race.Attributes.Where(x =>
                     !template.SelectedAttributes.Select(y => y.Definition).Contains(x)))
        {
            _traits.Add(TraitFactory.LoadAttribute(attribute, this, 10.0));
        }

        foreach ((ICharacteristicDefinition, ICharacteristicValue) characteristic in template.SelectedCharacteristics)
        {
            DefaultCharacteristicValues[characteristic.Item1] = characteristic.Item2;
        }

        List<ComboMerit> comboMerits = new();
        foreach (ICharacterMerit merit in template.SelectedMerits)
        {
            if (merit is ComboMerit cm)
            {
                comboMerits.Add(cm);
            }

            if (merit.MeritScope != MeritScope.Body)
            {
                continue;
            }

            _merits.Add(merit);
        }

        foreach (IChargenRole role in template.SelectedRoles)
        {
            foreach (IMerit merit in role.AdditionalMerits)
            {
                if (Merits.Contains(merit))
                {
                    continue;
                }

                if (merit is ComboMerit cm)
                {
                    comboMerits.Add(cm);
                }

                if (merit.MeritScope != MeritScope.Body)
                {
                    continue;
                }

                _merits.Add(merit);
            }
        }

        foreach (ComboMerit merit in comboMerits)
        {
            foreach (ICharacterMerit included in merit.CharacterMerits.Where(x => x.MeritScope == MeritScope.Character))
            {
                _merits.Add(included);
            }
        }

        foreach (ISelectedTattoo tattoo in template.SelectedTattoos)
        {
            _tattoos.Add(new Tattoo(tattoo.Tattoo, Gameworld, Actor, 100, tattoo.Bodypart,
                Actor.Culture.PrimaryCalendar.CurrentDateTime, tattoo.TextValues.Values));
        }

        foreach ((IDisfigurementTemplate Disfigurement, IBodypart Bodypart) disfigurement in template.SelectedDisfigurements)
        {
            if (disfigurement.Disfigurement is ITattooTemplate tt)
            {
                if (template.SelectedTattoos.Any(x => x.Tattoo == tt && x.Bodypart == disfigurement.Bodypart))
                {
                    continue;
                }

                _tattoos.Add(new Tattoo(tt, Gameworld, Actor, 100, disfigurement.Bodypart,
                    Actor.Culture.PrimaryCalendar.CurrentDateTime));
                continue;
            }

            throw new ApplicationException("Unknown Disfigurement Type in Body constructor.");
        }

        foreach (var scar in template.SelectedScars)
        {
            _scars.Add(new Scar(scar, Gameworld, Actor.Race));
        }

        Bloodtype = template.SelectedEthnicity.PopulationBloodModel?.GetBloodType(template);
        RecalculatePartsAndOrgans();
        RecalculateItemHelpers();

        PositionState = PositionStanding.Instance;
        _noSave = false;
    }

    public Body(MudSharp.Models.Body body, IFuturemud gameworld, ICharacter actor)
    {
        Prototype = gameworld.BodyPrototypes.Get(body.BodyPrototypeId);
        Gameworld = gameworld;
        Actor = actor;
        LoadFromDatabase(body);
    }

    public override string FrameworkItemType => "Body";

    public IBodyPrototype Prototype { get; protected set; }

    public ICharacter Actor { get; set; }
    public Alignment Handedness { get; set; }

    public IController Controller { get; protected set; }

    public IRace Race { get; protected set; }

    public IEthnicity Ethnicity { get; protected set; }

    public int AgeInYears => Actor.AgeInYears;
    public AgeCategory AgeCategory => Actor.AgeCategory;

    public bool Listening => throw new NotImplementedException();

    public IEnumerable<IEntityDescriptionPattern> EntityDescriptionPatterns
    {
        get
        {
            List<IEntityDescriptionPattern> patterns = new();
            if (_shortDescriptionPattern != null)
            {
                patterns.Add(_shortDescriptionPattern);
            }

            if (_fullDescriptionPattern != null)
            {
                patterns.Add(_fullDescriptionPattern);
            }

            return patterns;
        }
    }

    public static void RegisterPerceivableType(IFuturemud gameworld)
    {
        gameworld.RegisterPerceivableType("Body",
            id => gameworld.Characters.SelectMany(x => x.Bodies).FirstOrDefault(x => x.Id == id));
    }

    public (string ShortDescription, string FullDescription) GetRawDescriptions =>
        (_shortDescription, _fullDescription);

    public void SetFullDescription(string description)
    {
        _fullDescription = description;
        _fullDescriptionPattern = null;
        Changed = true;
    }

    public void SetShortDescription(string description)
    {
        _shortDescription = description;
        _shortDescriptionPattern = null;
        Changed = true;
    }

    public override ICell Location
    {
        get => Actor.Location;
        protected set => MoveTo(value, RoomLayer);
    }

    public override event LocatableEvent OnLocationChanged
    {
        add => Actor.OnLocationChanged += value;
        remove => Actor.OnLocationChanged -= value;
    }

    public override event LocatableEvent OnLocationChangedIntentionally
    {
        add => Actor.OnLocationChangedIntentionally += value;
        remove => Actor.OnLocationChangedIntentionally -= value;
    }

    public override void MoveTo(ICell location, RoomLayer layer, ICellExit exit = null, bool noSave = false)
    {
        Actor.MoveTo(location, layer, exit, noSave);
    }

    public override bool HandleEvent(EventType type, params dynamic[] arguments)
    {
        return Actor.HandleEvent(type, arguments);
    }

    #region Overrides of LateKeywordedInitialisingItem

    #region Overrides of LateKeywordedInitialisingItem

    public override InitialisationPhase InitialisationPhase => InitialisationPhase.First;

    #endregion

    #endregion

    public override void SetIDFromDatabase(object dbitem)
    {
        _id = ((MudSharp.Models.Body)dbitem).Id;
        if (Prosthetics.Any())
        {
            ProstheticsChanged = true;
        }
    }

    public override object DatabaseInsert()
    {
        Models.Body dbitem = new()
        {
            Height = Height,
            Weight = Weight,
            RaceId = Race.Id,
            Gender = (short)Gender.Enum,
            DominantHandAlignment = (int)Handedness,
            EthnicityId = Ethnicity.Id,
            BodyPrototypeId = Prototype.Id,
            Position = PositionState.Id,
            CurrentBloodVolume = CurrentBloodVolumeLitres,
            ShortDescription = _shortDescription,
            ShortDescriptionPatternId = _shortDescriptionPattern?.Id,
            FullDescription = _fullDescription,
            FullDescriptionPatternId = _fullDescriptionPattern?.Id,
            EffectData = SaveEffects().ToString(),
            Tattoos = SaveTattoos(),
            Scars = SaveScars(),
            HealthStrategyId = HealthStrategy?.Id
        };
        foreach (IBodypart item in _severedRoots)
        {
            dbitem.BodiesSeveredParts.Add(new BodiesSeveredParts { Bodies = dbitem, BodypartProtoId = item.Id });
        }

        foreach (ITrait trait in _traits)
        {
            Models.Trait dbtrait = new();
            FMDB.Context.Traits.Add(dbtrait);
            dbtrait.Body = dbitem;
            dbtrait.TraitDefinitionId = trait.Definition.Id;
            dbtrait.Value = trait.Value;
        }

        foreach (KeyValuePair<ICharacteristicDefinition, ICharacteristicValue> characteristic in DefaultCharacteristicValues)
        {
            Characteristic dbchar = new();
            FMDB.Context.Characteristics.Add(dbchar);
            dbchar.Body = dbitem;
            dbchar.CharacteristicId = characteristic.Value.Id;
            dbchar.Type = (int)characteristic.Key.Id;
        }

        foreach (IMerit merit in _merits)
        {
            PerceiverMerit dbmerit = new();
            FMDB.Context.PerceiverMerits.Add(dbmerit);
            dbmerit.MeritId = merit.Id;
            dbmerit.Body = dbitem;
        }

        FMDB.Context.Bodies.Add(dbitem);

        return dbitem;
    }

    protected override void ReleaseEvents()
    {
        base.ReleaseEvents();
        OnRemoveWound = null;
        OnWounded = null;
        OnExertionChanged = null;
        OnHeal = null;
    }

    public void Login()
    {
        _breathingStrategy = Race.BreathingStrategy;
        CalculateOrganFunctions(true);
        ScheduleCachedEffects();
        CheckDrugTick();
        StartHealthTick(true);
        InitialiseStamina();
        StartStaminaTick();
        CheckConsequences();
        CheckHealthStatus();
        foreach (IGameItem item in AllItems)
        {
            item.Login();
        }
    }

    public void Quit()
    {
        EffectsChanged = true;
        if (Changed)
        {
            Gameworld.SaveManager.Flush();
        }

        foreach (IGameItem item in AllItems)
        {
            item.Quit();
        }

        EndStaminaTick(true);
        EndDrugTick();
        EndHealthTick();
        CacheScheduledEffects();
        Gameworld.EffectScheduler.Destroy(this, true);
        Gameworld.Scheduler.Destroy(this);
        Gameworld.Destroy(this);
        PerceivableQuit();
        _breathingStrategy = new NonBreather();
    }

    public string ReportCondition()
    {
        // TODO
        throw new NotImplementedException();
    }

    public IBodypart GetTargetBodypart(string target)
    {
        return Bodyparts.GetFromItemListByKeyword(target, this);
    }

    public IBodypart GetTargetBodypart(string target, Alignment alignment, Orientation orientation)
    {
        return
            Bodyparts.Where(x => x.Alignment == alignment && x.Orientation == orientation)
                     .GetFromItemListByKeyword(target, this);
    }

    /// <summary>
    /// This function can return bones, organs and bodyparts
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public IBodypart GetTargetPart(string target)
    {
        return Bodyparts.Concat(Organs).Concat(Bones).GetFromItemListByKeyword(target, this);
    }

    public IBone GetTargetBone(string target)
    {
        return Bones.GetFromItemListByKeyword(target, this);
    }

    /// <summary>
    ///     Weight in base units
    /// </summary>
    public double Weight
    {
        get => _weight;
        set
        {
            _weight = value;
            Changed = true;
        }
    }

    /// <summary>
    ///     Height in base units
    /// </summary>
    public double Height
    {
        get => _height;
        set
        {
            _height = value;
            Changed = true;
        }
    }

    public void Register(IController controller)
    {
        Controller = controller;
    }

    public void ActivateForCharacter()
    {
        Controller = Actor?.CharacterController;
        _breathingStrategy = Race.BreathingStrategy;
    }

    public void SuspendForCharacter()
    {
        EndStaminaTick(true);
        EndDrugTick();
        EndHealthTick();
        CacheScheduledEffects();
        _breathingStrategy = new NonBreather();
        Controller = null;
    }

    public void DestroyBody()
    {
        Quit();
    }

    public override void Register(IOutputHandler handler)
    {
    }

    public override IOutputHandler OutputHandler
    {
        get => Actor.OutputHandler;
        protected set { }
    }

    public IPersonalName PersonalName
    {
        get => Actor.PersonalName;
        set => Actor.PersonalName = value;
    }

    public IList<IPersonalName> Aliases => Actor.Aliases;

    public IPersonalName CurrentName
    {
        get => Actor.CurrentName;
        set => Actor.CurrentName = value;
    }

    public bool NamesChanged
    {
        get => Actor.NamesChanged;
        set => Actor.NamesChanged = value;
    }

    public override bool IsSelf(IPerceivable other)
    {
        return base.IsSelf(other) || (other as ICharacter)?.Body == this;
    }

    public override IEnumerable<IPositionState> ValidPositions => Prototype.ValidPositions;

    public override int LineFormatLength => Actor.LineFormatLength;

    public override int InnerLineFormatLength => Actor.InnerLineFormatLength;

    public override double DefensiveAdvantage
    {
        get => Actor.DefensiveAdvantage;
        set => Actor.DefensiveAdvantage = value;
    }

    public override double OffensiveAdvantage
    {
        get => Actor.OffensiveAdvantage;
        set => Actor.OffensiveAdvantage = value;
    }

    public override DefenseType PreferredDefenseType
    {
        get => Actor.PreferredDefenseType;
        set => Actor.PreferredDefenseType = value;
    }

    public override double GetBonusForDefendersFromTargeting()
    {
        return Actor.GetBonusForDefendersFromTargeting();
    }

    public override double GetDefensiveAdvantagePenaltyFromTargeting()
    {
        return Actor.GetDefensiveAdvantagePenaltyFromTargeting();
    }

    public override bool CheckCombatStatus()
    {
        return Actor.CheckCombatStatus();
    }

    public override IPerceiver CombatTarget
    {
        get => Actor.CombatTarget;

        set => Actor.CombatTarget = value;
    }

    public override ICombatMove ResponseToMove(ICombatMove move, IPerceiver assailant)
    {
        return Actor.ResponseToMove(move, assailant);
    }

    public double GetCurrentBonusLevel()
    {
        // TODO - things that would affect bonus
        return
            HealthStrategy.WoundPenaltyFor(Actor) +
            NeedsModel.Status.GeneralBonusLevel()
            ;
    }

    public double GetPhysicalBonusLevel()
    {
        double bonus = 0.0;
        Dictionary<IGameItem, WearableItemCoverStatus> cover = GetAllItemsCoverStatus(true);
        foreach (KeyValuePair<IGameItem, WearableItemCoverStatus> item in cover)
        {
            IArmour armour = item.Key.GetItemType<IArmour>();
            if (armour is null || !armour.ApplyArmourPenalties)
            {
                continue;
            }

            if (item.Value == WearableItemCoverStatus.Uncovered)
            {
                bonus += armour.ArmourType.BaseDifficultyBonus;
                continue;
            }

            bonus += armour.ArmourType.StackedDifficultyBonus;
        }

        return bonus;
    }

    #region IFutureProgVariable Implementation

    public override ProgVariableTypes Type => ProgVariableTypes.Error;

    #endregion

    public override string ToString()
    {
        return $"Body {Id} - {HowSeen(this, colour: false, flags: PerceiveIgnoreFlags.IgnoreSelf)}";
    }

    protected void Initialise()
    {
    }

    private void LoadFromDatabase(MudSharp.Models.Body body)
    {
        _noSave = true;
        _loading = true;
        Race = Gameworld.Races.Get(body.RaceId);
        Ethnicity = Gameworld.Ethnicities.Get(body.EthnicityId);
        HeldBreathTime = TimeSpan.FromSeconds(body.HeldBreathLength);
        _healthStrategy = Gameworld.HealthStrategies.Get(body.HealthStrategyId ?? 0);
        _breathingStrategy = new NonBreather(); // This is set during Login
        _shortDescription = body.ShortDescription;
        _fullDescription = body.FullDescription;
        _shortDescriptionPattern = Gameworld.EntityDescriptionPatterns.Get(body.ShortDescriptionPatternId ?? 0);
        _fullDescriptionPattern = Gameworld.EntityDescriptionPatterns.Get(body.FullDescriptionPatternId ?? 0);
        Bloodtype = Gameworld.Bloodtypes.Get(body.BloodtypeId ?? 0);
        Gender = Gendering.Get((Gender)body.Gender);
        Handedness = (Alignment)body.DominantHandAlignment;
        _id = body.Id;
        IdInitialised = true;
        _height = body.Height;
        _weight = body.Weight;

        foreach (Characteristic characteristic in body.Characteristics)
        {
            DefaultCharacteristicValues.Add(Gameworld.Characteristics.Get(characteristic.Type),
                Gameworld.CharacteristicValues.Get(characteristic.CharacteristicId));
        }

        foreach (PerceiverMerit merit in body.PerceiverMerits)
        {
            _merits.Add(Gameworld.Merits.Get(merit.MeritId));
        }

        foreach (Models.Trait trait in body.Traits
                                           .Where(x => x.TraitDefinition.OwnerScope == (int)TraitOwnerScope.Body)
                                           .Where(x => x.TraitDefinition.Type == (int)TraitType.Attribute)
                                           .ToList())
        {
            _traits.Add(TraitDefinition.LoadTrait(trait, Gameworld, this));
        }

        foreach (Models.Trait trait in body.Traits
                                           .Where(x => x.TraitDefinition.OwnerScope == (int)TraitOwnerScope.Body)
                                           .Where(x => x.TraitDefinition.Type != (int)TraitType.Attribute)
                                           .ToList())
        {
            _traits.Add(TraitDefinition.LoadTrait(trait, Gameworld, this));
        }

        _traits.ForEach(x => x.Initialise(this));

        foreach (KeyValuePair<IPositionState, IMoveSpeed> speed in Prototype.DefaultSpeeds)
        {
            CurrentSpeeds[speed.Key] = speed.Value;
        }

        foreach (Wound wound in body.Wounds.ToList())
        {
            IWound newWound = WoundFactory.LoadWound(wound, Actor, Gameworld);
            _wounds.Add(newWound);
            if (wound.Infections.Any())
            {
                newWound.Infection = Health.Infections.Infection.LoadInfection(wound.Infections.First(), this);
            }
        }

        foreach (IGameItem item in Wounds.SelectNotNull(x => x.Lodged))
        {
            item.FinaliseLoadTimeTasks();
        }

        foreach (BodiesSeveredParts part in body.BodiesSeveredParts)
        {
            _severedRoots.Add(Prototype.AllBodypartsBonesAndOrgans.First(x => x.Id == part.BodypartProtoId));
        }

        foreach (BodiesProsthetics item in body.BodiesProsthetics)
        {
            IGameItem gitem = Gameworld.TryGetItem(item.Prosthetic, true);
            IProsthetic prosthetic = gitem?.GetItemType<IProsthetic>();
            if (prosthetic != null)
            {
                gitem.FinaliseLoadTimeTasks();
                _prosthetics.Add(prosthetic);
                prosthetic.InstallProsthetic(this);
            }
        }

        LoadImplants(body);

        RecalculatePartsAndOrgans();

        foreach (Models.Infection item in body.Infections)
        {
            IInfection infection = Health.Infections.Infection.LoadInfection(item, this);

            //If the wound is not null, then this infection belongs to the wound instead
            //of the body part. Let its pointer expire, we don't need to hold onto it
            if (infection.Wound == null)
            {
                if (infection.Bodypart != null)
                {
                    _partInfections.Add(infection);
                }
            }
        }

        foreach (BodyDrugDose drug in body.BodiesDrugDoses)
        {
            if (drug.Active)
            {
                _activeDrugDosages.Add(new DrugDosage
                {
                    Drug = Gameworld.Drugs.Get(drug.DrugId),
                    Grams = drug.Grams,
                    OriginalVector = (DrugVector)drug.OriginalVector
                });
            }
            else
            {
                _latentDrugDosages.Add(new DrugDosage
                {
                    Drug = Gameworld.Drugs.Get(drug.DrugId),
                    Grams = drug.Grams,
                    OriginalVector = (DrugVector)drug.OriginalVector
                });
            }
        }

        CurrentStamina = body.CurrentStamina;
        CurrentBloodVolumeLitres = body.CurrentBloodVolume;
        // TotalBloodVolume depends on Character, so Character is responsible for setting it
        LoadEffects(XElement.Parse(body.EffectData.IfNullOrWhiteSpace("<Effects/>")));
        _noSave = false;

        if (Bloodtype == null && Ethnicity.PopulationBloodModel != null)
        {
            Bloodtype = Ethnicity.PopulationBloodModel.GetBloodType(null);
            Changed = true;
        }

        LoadTattoos(body);
        LoadScars(body);
        _loading = false;
    }

    protected void SetupStandardParts()
    {
        _holdlocs =
            Bodyparts.OfType<IGrab>()
                     .Where(
                         x =>
                             SeveredRoots.All(y => !x.DownstreamOfPart(y)) ||
                             Prosthetics.Any(y => x.DownstreamOfPart(y.TargetBodypart) && y.Functional))
                     .ToList();
        _wieldLocs =
            Bodyparts.OfType<IWield>()
                     .Where(
                         x =>
                             SeveredRoots.All(y => !x.DownstreamOfPart(y)) ||
                             Prosthetics.Any(y => x.DownstreamOfPart(y.TargetBodypart) && y.Functional))
                     .ToList();
        _wearlocs =
            Bodyparts.OfType<IWear>()
                     .Where(
                         x =>
                             SeveredRoots.All(y => !x.DownstreamOfPart(y)) ||
                             Prosthetics.Any(y => x.DownstreamOfPart(y.TargetBodypart) && y.Functional))
                     .ToList();
        _limbs = Prototype.Limbs.Where(x => x.Parts.Any(y => Bodyparts.Contains(y))).ToList();
    }

    #region ISaveable Members

    public override void Save()
    {
        try
        {
            Models.Body dbentity = FMDB.Context.Bodies.Find(Id);
            dbentity.Height = Height;
            dbentity.Weight = Weight;
            dbentity.Position = PositionState.Id;
            dbentity.CurrentBloodVolume = CurrentBloodVolumeLitres;
            dbentity.BloodtypeId = Bloodtype?.Id;
            dbentity.FullDescription = _fullDescription;
            dbentity.ShortDescription = _shortDescription;
            dbentity.FullDescriptionPatternId = _fullDescriptionPattern?.Id;
            dbentity.ShortDescriptionPatternId = _shortDescriptionPattern?.Id;
            dbentity.HeldBreathLength = (int)HeldBreathTime.TotalSeconds;
            dbentity.Gender = (short)Gender.Enum;
            dbentity.DominantHandAlignment = (int)Handedness;
            dbentity.HealthStrategyId = HealthStrategy?.Id;

            if (InventoryChanged)
            {
                SaveInventory(dbentity);
            }

            if (StaminaChanged)
            {
                dbentity.CurrentStamina = CurrentStamina;
                StaminaChanged = false;
            }

            if (MeritsChanged)
            {
                SaveMerits(dbentity);
            }

            if (BodypartsChanged)
            {
                SaveBodyparts(dbentity);
            }

            if (DrugsChanged)
            {
                SaveDrugs(dbentity);
            }

            if (CharacteristicsChanged)
            {
                SaveCharacteristics(dbentity);
            }

            if (NeedsChanged)
            {
                NeedsChanged = false;
            }

            if (ProstheticsChanged)
            {
                SaveProsthetics(dbentity);
            }

            if (ImplantsChanged)
            {
                SaveImplants(dbentity);
            }

            if (TattoosChanged)
            {
                dbentity.Tattoos = SaveTattoos();
                TattoosChanged = false;
            }

            if (ScarsChanged)
            {
                dbentity.Scars = SaveScars();
                ScarsChanged = false;
            }

            if (EffectsChanged)
            {
                dbentity.EffectData = SaveEffects().ToString();
                EffectsChanged = false;
            }

            base.Save();
        }
        catch (DbUpdateException e)
        {
            Console.WriteLine($"DbUpdateException in Body Save {Id}: {e}");
            Gameworld.SystemMessage($"DbUpdateException in Body Save {Id}: {e}", true);
            throw;
        }

        Changed = false;
    }

    private void SaveBodyparts(MudSharp.Models.Body body)
    {
        FMDB.Context.BodiesSeveredParts.RemoveRange(body.BodiesSeveredParts);
        foreach (IBodypart item in _severedRoots)
        {
            body.BodiesSeveredParts.Add(new BodiesSeveredParts { Bodies = body, BodypartProtoId = item.Id });
        }

        FMDB.Context.SaveChanges();
        BodypartsChanged = false;
    }

    #endregion

    public string DebugInfo()
    {
        StringBuilder sb = new();
        sb.AppendLine($"Body ID {Id}");
        sb.AppendLine();
        sb.AppendLine($"Changed: {Changed}");
        sb.AppendLine($"BodypartsChanged: {BodypartsChanged}");
        sb.AppendLine($"DrugsChanged: {DrugsChanged}");
        sb.AppendLine($"CharacteristicsChanged: {CharacteristicsChanged}");
        sb.AppendLine($"InventoryChanged: {InventoryChanged}");
        sb.AppendLine($"MeritsChanged: {MeritsChanged}");
        sb.AppendLine($"NeedsChanged: {NeedsChanged}");
        sb.AppendLine($"StaminaChanged: {StaminaChanged}");
        sb.AppendLine($"PositionChanged: {PositionChanged}");
        sb.AppendLine($"ProstheticsChanged: {ProstheticsChanged}");
        sb.AppendLine();
        sb.AppendLine($"Stamina Tick (10s): {_tenSecondStaminaActive}");
        sb.AppendLine($"Stamina Tick (1m): {_minuteStaminaActive}");
        sb.AppendLine($"Health Tick: {_healthTickActive}");
        sb.AppendLine($"Drug Tick: {_drugTickOn}");


        return sb.ToString();
    }

    public bool CanRead(IWriting writing)
    {
        return Actor.CanRead(writing);
    }

    public string WhyCannotRead(IWriting writing)
    {
        return Actor.WhyCannotRead(writing);
    }

    public string GetWritingHeader(IWriting writing)
    {
        return Actor.GetWritingHeader(writing);
    }

    public bool Read(IWriting writing)
    {
        return Actor.Read(writing);
    }

    public bool CanWrite()
    {
        return Actor.CanWrite();
    }

    public bool CanIdentifyLanguage(ILanguage language)
    {
        return Actor.CanIdentifyLanguage(language);
    }

    public bool IsLiterate => Actor.IsLiterate;

    private bool _loading;

    public IEnumerable<T> CombinedEffectsOfType<T>() where T : class, IEffect
    {
        return EffectHandler.EffectsOfType<T>().Concat(Actor.EffectsOfType<T>());
    }

    public (double Coating, double Absorb) LiquidAbsorbtionAmounts =>
        // Coating amount is based on ~0.1L for a "normal" sized person and loosely tied to surface area
        (Math.Pow((int)Size, 2) * 0.0012 / Gameworld.UnitManager.BaseFluidToLitres, 0.0);

    public (double Coating, double Absorb) LiquidAbsorbtionAmountsForBodyparts(IEnumerable<IExternalBodypart> parts)
    {
        (double coating, double _) = LiquidAbsorbtionAmounts;
        return (coating * parts.Sum(x => x.RelativeHitChance) / Bodyparts.OfType<IExternalBodypart>().Sum(x => x.RelativeHitChance), 0.0);
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

    public ItemSaturationLevel SaturationLevelForLiquid(IEnumerable<IExternalBodypart> bodyparts)
    {
        (double coating, double absorb) = LiquidAbsorbtionAmountsForBodyparts(bodyparts);
        IEnumerable<BodyLiquidContamination> effects = EffectsOfType<BodyLiquidContamination>().Where(x => x.Bodyparts.Any(y => bodyparts.Contains(y)));
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

    private void ConsolidateLiquidContaminationEffects(IEnumerable<IExternalBodypart> parts)
    {
        List<BodyLiquidContamination> effects = EffectsOfType<BodyLiquidContamination>()
                      .Where(x => x.Bodyparts.Any(y => parts.Contains(y)))
                      .ToList();
        if (effects.Count <= 1)
        {
            return;
        }

        BodyLiquidContamination first = effects.First();
        effects = effects.Skip(1).ToList();
        foreach (BodyLiquidContamination effect in effects)
        {
            first.Bodyparts.AddRange(effect.Bodyparts);
        }

        first.Bodyparts = first.Bodyparts.Distinct().ToList();
        foreach (BodyLiquidContamination effect in effects)
        {
            first.AddLiquid(effect.ContaminatingLiquid);
            EffectHandler.RemoveEffect(effect, true);
        }
    }

    public void ExposeToLiquid(LiquidMixture mixture, IEnumerable<IExternalBodypart> parts, LiquidExposureDirection direction)
    {
        if (mixture.TotalVolume <= 0)
        {
            return;
        }

        if (direction == LiquidExposureDirection.Irrelevant)
        {
            foreach (IExternalBodypart part in parts)
            {
                LiquidMixture localMixture = mixture.Clone();
                localMixture.SetLiquidVolume(localMixture.TotalVolume * part.RelativeHitChance / parts.Sum(x => x.RelativeHitChance));
                IGameItem item = WornItemsFor(part).LastOrDefault();
                item?.ExposeToLiquid(localMixture, part, LiquidExposureDirection.FromOnTop);

                if (localMixture.IsEmpty)
                {
                    continue;
                }

                ExposeToLiquid(localMixture, part, LiquidExposureDirection.FromOnTop);
            }

            ConsolidateLiquidContaminationEffects(parts);
            return;
        }

        ConsolidateLiquidContaminationEffects(parts);
        BodyLiquidContamination effect = BodyLiquidContamination.CreateOrMergeEffect(Actor, mixture.Clone(), parts);
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

        (double coating, double _) = LiquidAbsorbtionAmountsForBodyparts(parts);
        if (effect.ContaminatingLiquid.TotalVolume > coating)
        {
            double excess = effect.ContaminatingLiquid.TotalVolume - coating;
            LiquidMixture excessMixture = effect.ContaminatingLiquid.RemoveLiquidVolume(excess);
            PuddleGameItemComponentProto.TopUpOrCreateNewPuddle(excessMixture, Location, RoomLayer, Actor);
        }

        mixture.SetLiquidVolume(0.0);
    }

    public void ExposeToLiquid(LiquidMixture mixture, IBodypart part, LiquidExposureDirection direction)
    {
        if (mixture.TotalVolume <= 0 || part is not IExternalBodypart ebp)
        {
            return;
        }

        if (direction == LiquidExposureDirection.Irrelevant)
        {
            IGameItem item = WornItemsFor(part).LastOrDefault();
            item?.ExposeToLiquid(mixture, part, LiquidExposureDirection.FromOnTop);

            foreach (IGameItem held in HeldOrWieldedItemsFor(part))
            {
                held.ExposeToLiquid(mixture, part, LiquidExposureDirection.FromOnTop);
            }

            if (mixture.TotalVolume <= 0)
            {
                return;
            }
        }

        BodyLiquidContamination effect = BodyLiquidContamination.CreateOrMergeEffect(Actor, mixture.Clone(), new[] { ebp });

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

        (double coating, double _) = LiquidAbsorbtionAmountsForBodyparts(new[] { ebp });
        if (effect.ContaminatingLiquid.TotalVolume > coating)
        {
            double excess = effect.ContaminatingLiquid.TotalVolume - coating;
            LiquidMixture excessMixture = effect.ContaminatingLiquid.RemoveLiquidVolume(excess);
            PuddleGameItemComponentProto.TopUpOrCreateNewPuddle(excessMixture, Location, RoomLayer, Actor);
        }

        mixture.SetLiquidVolume(0.0);
    }

    double LiquidVolumeFromPrecipitation(PrecipitationLevel level)
    {
        return Gameworld.GetStaticDouble($"PrecipitationAmountPerItemSize{Size.DescribeEnum()}{level.DescribeEnum()}");
    }

    public void ExposeToPrecipitation(PrecipitationLevel level, ILiquid liquid)
    {
        LiquidMixture mixture = new(liquid, LiquidVolumeFromPrecipitation(level), Gameworld);
        List<IExternalBodypart> externalParts = Bodyparts.OfType<IExternalBodypart>().ToList();
        double sum = externalParts.Sum(x => x.RelativeHitChance);
        foreach (IExternalBodypart bodypart in Bodyparts.OfType<IExternalBodypart>())
        {
            ExposeToLiquid(mixture.Clone(mixture.TotalVolume * bodypart.RelativeHitChance / sum), bodypart, LiquidExposureDirection.Irrelevant);
        }

        ConsolidateLiquidContaminationEffects(ExposedBodyparts.OfType<IExternalBodypart>());
    }
}
