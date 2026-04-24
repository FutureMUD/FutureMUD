using MudSharp.Body;
using MudSharp.Body.PartProtos;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.Magic;
using MudSharp.Planes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MudSharp.Health;



public class Drug : SaveableItem, IDrug
{
    private void DoDatabaseInsert()
    {
        using (new FMDB())
        {
            Models.Drug dbitem = new()
            {
                Name = Name,
                IntensityPerGram = IntensityPerGram,
                RelativeMetabolisationRate = RelativeMetabolisationRate,
                DrugVectors = (int)DrugVectors
            };
            foreach (KeyValuePair<DrugType, (double Multiplier, DrugAdditionalInfo ExtraInfo)> item in DrugTypeMulipliers)
            {
                Models.DrugIntensity dbmult = new()
                {
                    DrugType = (int)item.Key,
                    RelativeIntensity = item.Value.Multiplier,
                    AdditionalEffects = item.Value.ExtraInfo.DatabaseString
                };
                dbitem.DrugsIntensities.Add(dbmult);
            }
            FMDB.Context.Drugs.Add(dbitem);
            FMDB.Context.SaveChanges();
            _id = dbitem.Id;
        }
    }

    public Drug(Drug rhs, string newName)
    {
        Gameworld = rhs.Gameworld;
        _name = newName;
        DrugVectors = rhs.DrugVectors;
        IntensityPerGram = rhs.IntensityPerGram;
        RelativeMetabolisationRate = rhs.RelativeMetabolisationRate;
        foreach (KeyValuePair<DrugType, (double Multiplier, DrugAdditionalInfo ExtraInfo)> item in DrugTypeMulipliers)
        {
            DrugTypeMulipliers[item.Key] = (item.Value.Multiplier, AdditionalInfoFor(item.Key, item.Value.ExtraInfo.DatabaseString));
        }

        DoDatabaseInsert();
    }

    public Drug(IFuturemud gameworld, string name)
    {
        Gameworld = gameworld;
        _name = name;
        DrugVectors = DrugVector.Injected;
        IntensityPerGram = 1.0;
        RelativeMetabolisationRate = 1.0;

        DoDatabaseInsert();
    }

    public Drug(MudSharp.Models.Drug drug, IFuturemud gameworld)
    {
        Gameworld = gameworld;
        _id = drug.Id;
        _name = drug.Name;
        DrugVectors = (DrugVector)drug.DrugVectors;
        IntensityPerGram = drug.IntensityPerGram;
        RelativeMetabolisationRate = drug.RelativeMetabolisationRate;
        foreach (Models.DrugIntensity item in drug.DrugsIntensities)
        {
            DrugTypeMulipliers[(DrugType)item.DrugType] = (item.RelativeIntensity, AdditionalInfoFor((DrugType)item.DrugType, item.AdditionalEffects));
        }
    }

    public IDrug Clone(string newName)
    {
        return new Drug(this, newName);
    }

    #region Overrides of Item

    public override string FrameworkItemType => "Drug";

    #endregion

    #region Overrides of SaveableItem

    /// <summary>Tells the object to perform whatever save action it needs to do</summary>
    public override void Save()
    {
        Models.Drug dbitem = FMDB.Context.Drugs.Find(Id);
        dbitem.Name = Name;
        dbitem.DrugVectors = (int)DrugVectors;
        dbitem.IntensityPerGram = IntensityPerGram;
        dbitem.RelativeMetabolisationRate = RelativeMetabolisationRate;
        FMDB.Context.DrugsIntensities.RemoveRange(dbitem.DrugsIntensities);
        foreach (KeyValuePair<DrugType, (double Multiplier, DrugAdditionalInfo ExtraInfo)> item in DrugTypeMulipliers)
        {
            Models.DrugIntensity dbmult = new()
            {
                DrugType = (int)item.Key,
                RelativeIntensity = item.Value.Multiplier,
                AdditionalEffects = item.Value.ExtraInfo?.DatabaseString
            };
            dbitem.DrugsIntensities.Add(dbmult);
        }

        Changed = false;
    }

    #endregion

    #region Implementation of IDrug

    public IEnumerable<DrugType> DrugTypes => DrugTypeMulipliers.Select(x => x.Key).AsEnumerable();
    public DrugVector DrugVectors { get; set; }
    public double IntensityPerGram { get; set; }
    public double RelativeMetabolisationRate { get; set; }
    public Dictionary<DrugType, (double Multiplier, DrugAdditionalInfo ExtraInfo)> DrugTypeMulipliers { get; } = new();

    public T AdditionalInfoFor<T>(DrugType type) where T : DrugAdditionalInfo
    {
        return (T)DrugTypeMulipliers.ValueOrDefault(type, default).ExtraInfo;
    }

    private DrugAdditionalInfo AdditionalInfoFor(DrugType type, string extra)
    {
        switch (type)
        {
            case DrugType.NeutraliseDrugEffect:
                return new NeutraliseDrugAdditionalInfo
                {
                    NeutralisedTypes = extra
                                       .Split(' ')
                                       .Select(int.Parse)
                                       .Cast<DrugType>()
                                       .ToList()
                };
            case DrugType.BodypartDamage:
                return new BodypartDamageAdditionalInfo
                {
                    BodypartTypes = extra
                                    .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                                    .Select(int.Parse)
                                    .Cast<BodypartTypeEnum>()
                                    .ToList()
                };
            case DrugType.HealingRate:
                return new HealingRateAdditionalInfo
                {
                    HealingRateIntensity = double.Parse(extra.Split(" ")[0]),
                    HealingDifficultyIntensity = double.Parse(extra.Split(" ")[1])
                };
            case DrugType.MagicAbility:
                return new MagicAbilityAdditionalInfo
                {
                    MagicCapabilityIds = extra
                                             .Split(' ')
                                             .Select(long.Parse)
                                             .ToList()
                };
            case DrugType.OrganFunction:
                return new OrganFunctionAdditionalInfo
                {
                    OrganTypes = extra
                                        .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                                        .Select(int.Parse)
                                        .Cast<BodypartTypeEnum>()
                                        .ToList()
                };
            case DrugType.PlanarState:
                {
                    var parts = (extra ?? string.Empty).Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    return new PlanarStateAdditionalInfo
                    {
                        State = parts.ElementAtOrDefault(0) ?? "noncorporeal",
                        PlaneId = long.TryParse(parts.ElementAtOrDefault(1), out var planeId) ? planeId : 0,
                        VisibleToDefaultPlane = bool.TryParse(parts.ElementAtOrDefault(2), out var visible) && visible
                    };
                }
            case DrugType.NeutraliseSpecificDrug:
                return new NeutraliseSpecificDrugAdditionalInfo()
                {
                    NeutralisedIds = extra
                                         .Split(' ')
                     .Select(long.Parse)
                     .ToList()
                };
            default:
                return null;
        }
    }

    public const string HelpText = @"You can use the following options with this command:

	#3name <name>#0 - renames this drug
	#3intensity <%>#0 - sets the intensity of this drug per gram
	#3metabolism <%>#0 - sets the relative metabolism of this drug relative to others of its type
	#3inhaled|ingested|injected|touch#0 - toggles the specific vector
	#3type intensity <which> <%>#0 - adds a drug intensity of a particular effect

The following options require a matching intensity for the type before using them:

	#3type damage <bodypart type>#0 - toggles bodypart damage
	#3type magic <which>#0 - toggles a specific magic capability effect
	#3type neutralise <type>#0 - toggles neutralising a drug type
	#3type neutralisespecific <drug>#0 - toggles neutralising a specific drug
	#3type planar <corporeal|noncorporeal> [plane] [visible]#0 - configures planar state
	#3type healingrate <rate%> <difficulty%> - sets healing rate / difficulty bonuses";

    /// <inheritdoc />
    public bool BuildingCommand(ICharacter actor, StringStack command)
    {
        switch (command.PopForSwitch())
        {
            case "name":
                return BuildingCommandName(actor, command);
            case "intensity":
                return BuildingCommandIntensity(actor, command);
            case "metabolism":
                return BuildingCommandMetabolism(actor, command);
            case "inhaled":
                return BuildingCommandInhaled(actor);
            case "injected":
                return BuildingCommandInjected(actor);
            case "ingested":
                return BuildingCommandIngested(actor);
            case "touch":
                return BuildingCommandTouch(actor);
            case "type":
                return BuildingCommandType(actor, command);
            default:
                actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
                return false;
        }
    }

    private bool BuildingCommandType(ICharacter actor, StringStack command)
    {
        switch (command.PopForSwitch())
        {
            case "intensity":
                return BuildingCommandTypeIntensity(actor, command);
            case "healingrate":
                return BuildingCommandHealingRate(actor, command);
            case "magic":
            case "magiccapability":
            case "magicability":
            case "capability":
            case "ability":
                return BuildingCommandMagicCapability(actor, command);
            case "neutralise":
                return BuildingCommandNeutralise(actor, command);
            case "neutralisespecific":
                return BuildingCommandNeutraliseSpecific(actor, command);
            case "damage":
                return BuildingCommandDamage(actor, command);
            case "organfunction":
                return BuildingCommandOrganFunction(actor, command);
            case "planar":
            case "plane":
            case "corporeality":
                return BuildingCommandPlanar(actor, command);
            default:
                return false;
        }
    }

    private bool BuildingCommandPlanar(ICharacter actor, StringStack command)
    {
        if (!DrugTypeMulipliers.ContainsKey(DrugType.PlanarState))
        {
            actor.OutputHandler.Send("This drug does not contain a planar state effect intensity. You must set that first.");
            return false;
        }

        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Do you want this drug to apply a corporeal or noncorporeal state?");
            return false;
        }

        var state = command.PopSpeech().ToLowerInvariant();
        if (!state.EqualToAny("corporeal", "manifest", "manifested", "noncorporeal", "incorporeal", "dissipate", "dissipated"))
        {
            actor.OutputHandler.Send("The planar state must be corporeal or noncorporeal.");
            return false;
        }

        var plane = actor.Gameworld.DefaultPlane;
        var visible = false;
        while (!command.IsFinished)
        {
            var token = command.PopSpeech();
            if (token.EqualToAny("visible", "seen", "manifest"))
            {
                visible = true;
                continue;
            }

            plane = actor.Gameworld.Planes.GetByIdOrName(token);
            if (plane is null)
            {
                actor.OutputHandler.Send($"There is no plane identified by {token.ColourCommand()}.");
                return false;
            }
        }

        DrugTypeMulipliers[DrugType.PlanarState] = (DrugTypeMulipliers[DrugType.PlanarState].Multiplier,
            new PlanarStateAdditionalInfo
            {
                State = state,
                PlaneId = plane?.Id ?? 0,
                VisibleToDefaultPlane = visible
            });
        Changed = true;
        actor.OutputHandler.Send($"This drug now applies a {state.ColourValue()} planar state on {(plane?.Name ?? "the default plane").ColourName()}.");
        return true;
    }

    private bool BuildingCommandDamage(ICharacter actor, StringStack command)
    {
        if (!DrugTypeMulipliers.ContainsKey(DrugType.BodypartDamage))
        {
            actor.OutputHandler.Send("This drug does not contain a bodypart damage effect intensity. You must set that first.");
            return false;
        }

        if (command.IsFinished)
        {
            actor.OutputHandler.Send($"Which bodypart type do you want to toggle damage to? The valid choices are {Enum.GetValues<BodypartTypeEnum>().ListToColouredString()}.");
            return false;
        }

        if (!command.SafeRemainingArgument.TryParseEnum<BodypartTypeEnum>(out BodypartTypeEnum value))
        {
            actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid bodypart type. The valid choices are {Enum.GetValues<BodypartTypeEnum>().ListToColouredString()}.");
            return false;
        }

        BodypartDamageAdditionalInfo extra = (BodypartDamageAdditionalInfo)DrugTypeMulipliers[DrugType.BodypartDamage].ExtraInfo;
        if (extra.BodypartTypes.Contains(value))
        {
            extra.BodypartTypes.Remove(value);
            actor.OutputHandler.Send($"This drug will no longer damage bodyparts of type {value.DescribeEnum().ColourValue()}.");
        }
        else
        {
            extra.BodypartTypes.Add(value);
            actor.OutputHandler.Send($"This drug will now damage bodyparts of type {value.DescribeEnum().ColourValue()}.");
        }
        Changed = true;
        return true;
    }

    private bool BuildingCommandOrganFunction(ICharacter actor, StringStack command)
    {
        if (!DrugTypeMulipliers.ContainsKey(DrugType.OrganFunction))
        {
            actor.OutputHandler.Send("This drug does not contain an organ function effect intensity. You must set that first.");
            return false;
        }

        if (command.IsFinished)
        {
            actor.OutputHandler.Send($"Which organ type do you want to toggle? The valid choices are {Enum.GetValues<BodypartTypeEnum>().ListToColouredString()}.");
            return false;
        }

        if (!command.SafeRemainingArgument.TryParseEnum<BodypartTypeEnum>(out BodypartTypeEnum value))
        {
            actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid organ type. The valid choices are {Enum.GetValues<BodypartTypeEnum>().ListToColouredString()}.");
            return false;
        }

        OrganFunctionAdditionalInfo extra = (OrganFunctionAdditionalInfo)DrugTypeMulipliers[DrugType.OrganFunction].ExtraInfo;
        if (extra.OrganTypes.Contains(value))
        {
            extra.OrganTypes.Remove(value);
            actor.OutputHandler.Send($"This drug will no longer affect organs of type {value.DescribeEnum().ColourValue()}.");
        }
        else
        {
            extra.OrganTypes.Add(value);
            actor.OutputHandler.Send($"This drug will now affect organs of type {value.DescribeEnum().ColourValue()}.");
        }
        Changed = true;
        return true;
    }

    private bool BuildingCommandNeutraliseSpecific(ICharacter actor, StringStack command)
    {
        if (!DrugTypeMulipliers.ContainsKey(DrugType.NeutraliseSpecificDrug))
        {
            actor.OutputHandler.Send("This drug does not contain a neutralise specific drug effect intensity. You must set that first.");
            return false;
        }

        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which specific drug do you want to toggle neutralisation for this drug?");
            return false;
        }

        IDrug drug = Gameworld.Drugs.GetByIdOrName(command.SafeRemainingArgument);
        if (drug is null)
        {
            actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid drug.");
            return false;
        }

        if (drug == this)
        {
            actor.OutputHandler.Send("A drug cannot neutralise itself.");
            return false;
        }

        NeutraliseSpecificDrugAdditionalInfo extra = AdditionalInfoFor<NeutraliseSpecificDrugAdditionalInfo>(DrugType.NeutraliseSpecificDrug);
        if (extra.NeutralisedIds.Contains(drug.Id))
        {
            extra.NeutralisedIds.Remove(drug.Id);
            actor.OutputHandler.Send($"This drug no longer neutralises the drug {drug.Name.ColourValue()}.");
        }
        else
        {
            extra.NeutralisedIds.Add(drug.Id);
            actor.OutputHandler.Send($"This drug now neutralises the drug {drug.Name.ColourValue()}.");
        }

        Changed = true;
        return true;
    }

    private bool BuildingCommandNeutralise(ICharacter actor, StringStack command)
    {
        if (!DrugTypeMulipliers.ContainsKey(DrugType.NeutraliseDrugEffect))
        {
            actor.OutputHandler.Send("This drug does not contain a neutralise drug effect intensity. You must set that first.");
            return false;
        }

        if (command.IsFinished)
        {
            actor.OutputHandler.Send($"Which drug type do you want to toggle neutralisation of? The valid choices are {Enum.GetValues<DrugType>().ListToColouredString()}.");
            return false;
        }

        if (!command.SafeRemainingArgument.TryParseEnum<DrugType>(out DrugType value))
        {
            actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid drug type. The valid choices are {Enum.GetValues<DrugType>().ListToColouredString()}.");
            return false;
        }

        NeutraliseDrugAdditionalInfo extra = (NeutraliseDrugAdditionalInfo)DrugTypeMulipliers[DrugType.NeutraliseDrugEffect].ExtraInfo;
        if (extra.NeutralisedTypes.Contains(value))
        {
            extra.NeutralisedTypes.Remove(value);
            actor.OutputHandler.Send($"This drug will no longer neutralise drug effects of type {value.DescribeEnum().ColourValue()}.");
        }
        else
        {
            extra.NeutralisedTypes.Add(value);
            actor.OutputHandler.Send($"This drug will now neutralise drug effects of type {value.DescribeEnum().ColourValue()}.");
        }
        Changed = true;
        return true;
    }

    private bool BuildingCommandMagicCapability(ICharacter actor, StringStack command)
    {
        if (!DrugTypeMulipliers.ContainsKey(DrugType.MagicAbility))
        {
            actor.OutputHandler.Send("This drug does not contain a magic ability effect intensity. You must set that first.");
            return false;
        }

        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which magic capability do you want to toggle for this drug?");
            return false;
        }

        IMagicCapability capability = Gameworld.MagicCapabilities.GetByIdOrName(command.SafeRemainingArgument);
        if (capability is null)
        {
            actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid magic capability.");
            return false;
        }

        MagicAbilityAdditionalInfo extra = AdditionalInfoFor<MagicAbilityAdditionalInfo>(DrugType.MagicAbility);
        if (extra.MagicCapabilityIds.Contains(capability.Id))
        {
            extra.MagicCapabilityIds.Remove(capability.Id);
            actor.OutputHandler.Send($"This drug no longer provides the magic capability {capability.Name.Colour(capability.School.PowerListColour)}.");
        }
        else
        {
            extra.MagicCapabilityIds.Add(capability.Id);
            actor.OutputHandler.Send($"This drug now provides the magic capability {capability.Name.Colour(capability.School.PowerListColour)}.");
        }

        Changed = true;
        return true;
    }

    private bool BuildingCommandHealingRate(ICharacter actor, StringStack command)
    {
        if (!DrugTypeMulipliers.ContainsKey(DrugType.HealingRate))
        {
            actor.OutputHandler.Send("This drug does not contain a healing rate effect intensity. You must set that first.");
            return false;
        }

        if (command.IsFinished)
        {
            actor.OutputHandler.Send("What should be the healing rate increase?");
            return false;
        }

        if (!command.PopSpeech().TryParsePercentage(actor.Account.Culture, out double rate) || rate < 0.0)
        {
            actor.OutputHandler.Send($"The text {command.Last.ColourCommand()} is not a valid percentage.");
            return false;
        }

        if (command.IsFinished)
        {
            actor.OutputHandler.Send("What should be the healing difficulty bonus?");
            return false;
        }

        if (!command.PopSpeech().TryParsePercentage(actor.Account.Culture, out double bonus))
        {
            actor.OutputHandler.Send($"The text {command.Last.ColourCommand()} is not a valid percentage.");
            return false;
        }

        HealingRateAdditionalInfo extra = (HealingRateAdditionalInfo)DrugTypeMulipliers[DrugType.HealingRate].ExtraInfo;
        extra.HealingRateIntensity = rate;
        extra.HealingDifficultyIntensity = bonus;
        Changed = true;
        actor.OutputHandler.Send($"This drug will now have a healing rate bonus of {rate.ToBonusPercentageString(actor)} and difficulty bonus of {bonus.ToBonusPercentageString(actor)} per intensity.");
        return true;
    }

    private bool BuildingCommandTypeIntensity(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send($"Which drug type effect do you want to change the intensity of? The valid choices are {Enum.GetValues<DrugType>().ListToColouredString()}.");
            return false;
        }

        if (!command.PopSpeech().TryParseEnum<DrugType>(out DrugType type))
        {
            actor.OutputHandler.Send($"The text {command.Last.ColourCommand()} is not a valid drug type. The valid choices are {Enum.GetValues<DrugType>().ListToColouredString()}.");
            return false;
        }

        if (command.IsFinished)
        {
            actor.OutputHandler.Send("What should be the intensity of that drug effect?");
            return false;
        }

        if (!command.SafeRemainingArgument.TryParsePercentage(actor.Account.Culture, out double value))
        {
            actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid percentage.");
            return false;
        }

        if (value <= 0.0)
        {
            DrugTypeMulipliers.Remove(type);
            Changed = true;
            actor.OutputHandler.Send($"This drug will not have any effects of type {type.DescribeEnum().ColourValue()}.");
            return true;
        }

        if (DrugTypeMulipliers.ContainsKey(type))
        {
            DrugTypeMulipliers[type] = (value, DrugTypeMulipliers[type].ExtraInfo);
        }
        else
        {
            switch (type)
            {
                case DrugType.NeutraliseDrugEffect:
                    DrugTypeMulipliers[type] = (value, new NeutraliseDrugAdditionalInfo { NeutralisedTypes = [] });
                    break;
                case DrugType.NeutraliseSpecificDrug:
                    DrugTypeMulipliers[type] = (value, new NeutraliseSpecificDrugAdditionalInfo() { NeutralisedIds = [] });
                    break;
                case DrugType.BodypartDamage:
                    DrugTypeMulipliers[type] = (value, new BodypartDamageAdditionalInfo() { BodypartTypes = [] });
                    break;
                case DrugType.HealingRate:
                    DrugTypeMulipliers[type] = (value, new HealingRateAdditionalInfo() { HealingDifficultyIntensity = 0.0, HealingRateIntensity = 0.0 });
                    break;
                case DrugType.MagicAbility:
                    DrugTypeMulipliers[type] = (value, new MagicAbilityAdditionalInfo { MagicCapabilityIds = [] });
                    break;
                case DrugType.PlanarState:
                    DrugTypeMulipliers[type] = (value, new PlanarStateAdditionalInfo
                    {
                        State = "noncorporeal",
                        PlaneId = Gameworld.DefaultPlane?.Id ?? 0,
                        VisibleToDefaultPlane = false
                    });
                    break;
                default:
                    DrugTypeMulipliers[type] = (value, null);
                    break;
            }
        }

        Changed = true;
        actor.OutputHandler.Send($"The intensity of the {type.DescribeEnum()} drug type is now {value.ToStringP2Colour(actor)}.");
        return true;
    }

    private bool BuildingCommandTouch(ICharacter actor)
    {
        if (!DrugVectors.HasFlag(DrugVector.Touched))
        {
            DrugVectors |= DrugVector.Touched;
        }
        else
        {
            DrugVectors &= ~DrugVector.Touched;
        }

        Changed = true;
        actor.OutputHandler.Send($"This drug can {DrugVectors.HasFlag(DrugVector.Touched).NowNoLonger()} be absorbed by touch contact.");
        return true;
    }

    private bool BuildingCommandIngested(ICharacter actor)
    {
        if (!DrugVectors.HasFlag(DrugVector.Ingested))
        {
            DrugVectors |= DrugVector.Ingested;
        }
        else
        {
            DrugVectors &= ~DrugVector.Ingested;
        }
        Changed = true;
        actor.OutputHandler.Send($"This drug can {DrugVectors.HasFlag(DrugVector.Ingested).NowNoLonger()} be ingested.");
        return true;
    }

    private bool BuildingCommandInjected(ICharacter actor)
    {
        if (!DrugVectors.HasFlag(DrugVector.Injected))
        {
            DrugVectors |= DrugVector.Injected;
        }
        else
        {
            DrugVectors &= ~DrugVector.Injected;
        }
        Changed = true;
        actor.OutputHandler.Send($"This drug can {DrugVectors.HasFlag(DrugVector.Injected).NowNoLonger()} be injected.");
        return true;
    }

    private bool BuildingCommandInhaled(ICharacter actor)
    {
        if (!DrugVectors.HasFlag(DrugVector.Inhaled))
        {
            DrugVectors |= DrugVector.Inhaled;
        }
        else
        {
            DrugVectors &= ~DrugVector.Inhaled;
        }

        Changed = true;
        actor.OutputHandler.Send($"This drug can {DrugVectors.HasFlag(DrugVector.Inhaled).NowNoLonger()} be inhaled.");
        return true;
    }

    private bool BuildingCommandMetabolism(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send($"What should be the relative metabolism of this drug per gram?");
            return false;
        }

        if (!command.SafeRemainingArgument.TryParsePercentage(actor.Account.Culture, out double value))
        {
            actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid percentage.");
            return false;
        }

        RelativeMetabolisationRate = value;
        Changed = true;
        actor.OutputHandler.Send($"This drug now has a relative metabolism of {RelativeMetabolisationRate.ToStringP2Colour(actor)}.");
        return true;
    }

    private bool BuildingCommandIntensity(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send($"What should be the relative intensity of this drug per gram?");
            return false;
        }

        if (!command.SafeRemainingArgument.TryParsePercentage(actor.Account.Culture, out double value))
        {
            actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid percentage.");
            return false;
        }

        IntensityPerGram = value;
        Changed = true;
        actor.OutputHandler.Send($"This drug now has a relative intensity per gram of {IntensityPerGram.ToStringP2Colour(actor)}.");
        return true;
    }

    private bool BuildingCommandName(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("What name do you want to give to this drug?");
            return false;
        }

        string name = command.SafeRemainingArgument;
        if (Gameworld.Drugs.Any(x => x.Name.EqualTo(name)))
        {
            actor.OutputHandler.Send($"There is already a drug called {name.ColourName()}. Names must be unique.");
            return false;
        }

        actor.OutputHandler.Send($"You rename the drug {_name.ColourName()} to {name.ColourName()}.");
        _name = name;
        Changed = true;
        return true;
    }

    public virtual string Show(ICharacter voyeur)
    {
        StringBuilder sb = new();
        sb.AppendLine($"Drug #{Id:N0} - {Name}");
        sb.AppendLine($"Intensity per Gram: {IntensityPerGram.ToStringP2Colour(voyeur)}");
        sb.AppendLine($"Relative Metabolisation Rate: {RelativeMetabolisationRate.ToStringP2Colour(voyeur)}");
        sb.AppendLine($"Vectors: {DrugVectors.Describe().ColourValue()}");
        sb.AppendLine();
        sb.AppendLine("Effect Types:");
        sb.AppendLine();
        foreach (KeyValuePair<DrugType, (double Multiplier, DrugAdditionalInfo ExtraInfo)> effect in DrugTypeMulipliers)
        {
            sb.Append($"\t{effect.Key.DescribeEnum().ColourValue()} @ {effect.Value.Multiplier.ToStringP2Colour(voyeur)}");
            switch (effect.Key)
            {
                case DrugType.NeutraliseDrugEffect:
                    sb.AppendLine($" - {((NeutraliseDrugAdditionalInfo)effect.Value.ExtraInfo).NeutralisedTypes.ListToColouredString()}");
                    break;
                case DrugType.NeutraliseSpecificDrug:
                    sb.AppendLine($" - {((NeutraliseSpecificDrugAdditionalInfo)effect.Value.ExtraInfo).NeutralisedIds.SelectNotNull(x => Gameworld.Drugs.Get(x)).Select(x => x.Name).ListToColouredString()}");
                    break;
                case DrugType.HealingRate:
                    HealingRateAdditionalInfo extra = (HealingRateAdditionalInfo)effect.Value.ExtraInfo;
                    sb.AppendLine($" - Rate {extra.HealingRateIntensity.ToBonusPercentageString(voyeur)}, Difficulty Bonus {extra.HealingDifficultyIntensity.ToBonusPercentageString(voyeur)}");
                    break;
                case DrugType.BodypartDamage:
                    sb.AppendLine($" - {((BodypartDamageAdditionalInfo)effect.Value.ExtraInfo).BodypartTypes.ListToColouredString()}");
                    break;
                case DrugType.MagicAbility:
                    sb.AppendLine($" - {((MagicAbilityAdditionalInfo)effect.Value.ExtraInfo).MagicCapabilityIds.SelectNotNull(x => Gameworld.MagicCapabilities.Get(x)).Select(x => x.Name.Colour(x.School.PowerListColour)).ListToString()}");
                    break;
                case DrugType.OrganFunction:
                    sb.AppendLine($" - {((OrganFunctionAdditionalInfo)effect.Value.ExtraInfo).OrganTypes.ListToColouredString()}");
                    break;
                case DrugType.PlanarState:
                    PlanarStateAdditionalInfo planar = (PlanarStateAdditionalInfo)effect.Value.ExtraInfo;
                    sb.AppendLine($" - {planar.State.ColourValue()} on {(Gameworld.Planes.Get(planar.PlaneId)?.Name ?? "default").ColourName()}{(planar.VisibleToDefaultPlane ? ", visible to default plane".ColourCommand() : string.Empty)}");
                    break;
                default:
                    sb.AppendLine();
                    break;
            }
        }

        return sb.ToString();
    }

    public string DescribeEffect(DrugType type, IPerceiver voyeur)
    {
        switch (type)
        {
            case DrugType.NeutraliseDrugEffect:
                List<string> types = AdditionalInfoFor<NeutraliseDrugAdditionalInfo>(DrugType.NeutraliseDrugEffect)
                            .NeutralisedTypes
                            .Select(x => x.DescribeEnum().ColourValue())
                            .ToList();
                return $"Neutralising {types.ListToString()} @ {IntensityForType(type).ToString("N4", voyeur)}";
            case DrugType.BodypartDamage:
                return
                    $"Damaging {AdditionalInfoFor<BodypartDamageAdditionalInfo>(DrugType.BodypartDamage).BodypartTypes.Select(x => x.DescribeEnum().Pluralise().ColourValue()).ListToString()} @ {IntensityForType(type).ToString("N4", voyeur)}";
            case DrugType.HealingRate:
                HealingRateAdditionalInfo split = AdditionalInfoFor<HealingRateAdditionalInfo>(DrugType.HealingRate);
                return
                    $"HealingRate Mult ({split.HealingRateIntensity.ToString("N4", voyeur)}) Diff ({split.HealingDifficultyIntensity.ToString("N4", voyeur)}) @ {IntensityForType(type).ToString("N4", voyeur)}";
            case DrugType.MagicAbility:
                List<IMagicCapability> capabilities = AdditionalInfoFor<MagicAbilityAdditionalInfo>(DrugType.MagicAbility)
                                                   .MagicCapabilityIds
                                                   .SelectNotNull(x => Gameworld.MagicCapabilities.Get(x))
                                                   .ToList();
                return
                        $"MagicAbility of {capabilities.Select(x => x.Name.Colour(x.School.PowerListColour)).ListToString()} @ {IntensityForType(type).ToString("N4", voyeur)}";
            case DrugType.OrganFunction:
                return
                        $"OrganFunction affecting {AdditionalInfoFor<OrganFunctionAdditionalInfo>(DrugType.OrganFunction).OrganTypes.Select(x => x.DescribeEnum().Pluralise().ColourValue()).ListToString()} @ {IntensityForType(type).ToString("N4", voyeur)}";
            case DrugType.VisionImpairment:
                return $"VisionImpairment @ {IntensityForType(type).ToString("N4", voyeur)}";
            case DrugType.ThermalImbalance:
                return $"ThermalImbalance @ {IntensityForType(type).ToString("N4", voyeur)}";
            case DrugType.PlanarState:
                PlanarStateAdditionalInfo planar = AdditionalInfoFor<PlanarStateAdditionalInfo>(DrugType.PlanarState);
                return
                    $"PlanarState {planar.State} on {Gameworld.Planes.Get(planar.PlaneId)?.Name ?? "default"} @ {IntensityForType(type).ToString("N4", voyeur)}";
        }

        return $"{type.DescribeEnum()} @ {IntensityForType(type).ToString("N4", voyeur)}";
    }

    public double IntensityForType(DrugType type)
    {
        return DrugTypeMulipliers.ValueOrDefault(type, default).Multiplier * IntensityPerGram;
    }

    #endregion

    #region IFutureProgVariable Implementation

    public ProgVariableTypes Type => ProgVariableTypes.Drug;
    public object GetObject => this;

    public IProgVariable GetProperty(string property)
    {
        switch (property.ToLowerInvariant())
        {
            case "id":
                return new NumberVariable(Id);
            case "name":
                return new TextVariable(Name);
            case "intensitypergram":
                return new NumberVariable(IntensityPerGram);
            case "metabolisationrate":
            case "metabolizationrate":
                return new NumberVariable(RelativeMetabolisationRate);
            case "vectors":
                return new CollectionVariable(
                    DrugVectors.GetFlags().OfType<DrugVector>().Select(x => new TextVariable(x.Describe())).ToList(),
                    ProgVariableTypes.Text);
            case "types":
                return new CollectionVariable(DrugTypes.Select(x => new TextVariable(x.DescribeEnum())).ToList(),
                    ProgVariableTypes.Text);
            case "itensities":
                return new CollectionVariable(DrugTypes.Select(x => new NumberVariable(IntensityForType(x))).ToList(),
                    ProgVariableTypes.Number);
        }

        throw new NotImplementedException();
    }

    private static IReadOnlyDictionary<string, ProgVariableTypes> DotReferenceHandler()
    {
        return new Dictionary<string, ProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
        {
            { "id", ProgVariableTypes.Number },
            { "name", ProgVariableTypes.Text },
            { "intensitypergram", ProgVariableTypes.Number },
            { "metabolisationrate", ProgVariableTypes.Text | ProgVariableTypes.Collection },
            { "metabolizationrate", ProgVariableTypes.Number },
            { "vectors", ProgVariableTypes.Text | ProgVariableTypes.Collection },
            { "types", ProgVariableTypes.Text | ProgVariableTypes.Collection },
            { "intensities", ProgVariableTypes.Number | ProgVariableTypes.Collection }
        };
    }

    private static IReadOnlyDictionary<string, string> DotReferenceHelp()
    {
        return new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
        {
            { "id", "The unique database id of the drug" },
            { "name", "The name of the drug" },
            {
                "intensitypergram",
                "The hard-coded intensity of the drug per gram ingested. Has different effects per drug type"
            },
            { "metabolisationrate", "The relative rate at which the drug is metabolised (removed) by the body" },
            { "metabolizationrate", "An alias for the metabolisationrate property" },
            { "vectors", "The potential vectors for the drug affecting someone" },
            { "types", "The drug effect types contained in this drug" },
            { "intensities", "The intensities for each drug type, ordered the same way as the 'types' property" }
        };
    }

    public static void RegisterFutureProgCompiler()
    {
        ProgVariable.RegisterDotReferenceCompileInfo(ProgVariableTypes.Drug, DotReferenceHandler(),
            DotReferenceHelp());
    }

    #endregion
}
