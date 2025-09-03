using System.Collections.Generic;
using System.Linq;
using System.Text;
using MudSharp.Character;
using MudSharp.CharacterCreation;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Merits.Interfaces;

namespace MudSharp.Health.Bloodtypes;

#nullable enable
public class PopulationBloodModel : SaveableItem, IPopulationBloodModel
{
    private readonly List<(IBloodtype Bloodtype, double Weight)> _bloodTypes = new();

    public PopulationBloodModel(MudSharp.Models.PopulationBloodModel model, IFuturemud gameworld)
    {
        Gameworld = gameworld;
        _id = model.Id;
        _name = model.Name;
        foreach (var type in model.PopulationBloodModelsBloodtypes)
        {
            _bloodTypes.Add((gameworld.Bloodtypes.Get(type.BloodtypeId), type.Weight));
        }

        if (_bloodTypes.Any())
        {
            BloodModel = gameworld.BloodModels.First(x => x.Bloodtypes.Any(y => _bloodTypes.Any(z => z.Bloodtype == y)));
        }
    }

    public PopulationBloodModel(IFuturemud gameworld, string name)
    {
        Gameworld = gameworld;
        _name = name;
        using (new FMDB())
        {
            var dbitem = new MudSharp.Models.PopulationBloodModel { Name = name };
            FMDB.Context.PopulationBloodModels.Add(dbitem);
            FMDB.Context.SaveChanges();
            _id = dbitem.Id;
        }
    }

    public override string FrameworkItemType => "PopulationBloodModel";

    public override void Save()
    {
        var dbitem = FMDB.Context.PopulationBloodModels.Find(Id);
        dbitem.Name = Name;
        FMDB.Context.PopulationBloodModelsBloodtypes.RemoveRange(dbitem.PopulationBloodModelsBloodtypes);
        foreach (var (bloodtype, weight) in _bloodTypes)
        {
            dbitem.PopulationBloodModelsBloodtypes.Add(new MudSharp.Models.PopulationBloodModelsBloodtype
            {
                PopulationBloodModelId = Id,
                BloodtypeId = bloodtype.Id,
                Weight = weight
            });
        }

        Changed = false;
    }

    public IEnumerable<(IBloodtype Bloodtype, double Weight)> BloodTypes => _bloodTypes;

    public IBloodModel? BloodModel { get; private set; }

    public IBloodtype GetBloodType(ICharacterTemplate character)
    {
        if (character?.SelectedMerits.OfType<IFixedBloodTypeMerit>().Any() == true)
        {
            var bloodtype = character.SelectedMerits.OfType<IFixedBloodTypeMerit>().First().Bloodtype;
            if (_bloodTypes.Any(x => x.Bloodtype == bloodtype))
            {
                return bloodtype;
            }
        }

        return _bloodTypes.GetWeightedRandom(x => x.Weight).Bloodtype;
    }

    public const string HelpText = @"You can use the following options with this command:

name <name> - renames this population blood model
type <bloodtype> <weight> - adds or sets a blood type with a relative weight
remove <bloodtype> - removes a blood type from this model";

    public bool BuildingCommand(ICharacter actor, StringStack command)
    {
        switch (command.PopForSwitch())
        {
            case "name":
                return BuildingCommandName(actor, command);
            case "type":
            case "bloodtype":
                return BuildingCommandType(actor, command);
            case "remove":
            case "delete":
                return BuildingCommandRemove(actor, command);
        }

        actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
        return false;
    }

    private bool BuildingCommandName(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("What new name should this population blood model have?");
            return false;
        }

        var name = command.SafeRemainingArgument.TitleCase();
        if (Gameworld.PopulationBloodModels.Any(x => x.Name.EqualTo(name)))
        {
            actor.OutputHandler.Send($"There is already a population blood model named {name.ColourName()}.");
            return false;
        }

        _name = name;
        Changed = true;
        actor.OutputHandler.Send($"This population blood model is now called {name.ColourName()}.");
        return true;
    }

    private bool BuildingCommandType(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which blood type and weight?");
            return false;
        }

        var bt = Gameworld.Bloodtypes.GetByIdOrName(command.PopSpeech());
        if (bt is null)
        {
            actor.OutputHandler.Send("That is not a valid blood type.");
            return false;
        }

        if (command.IsFinished || !double.TryParse(command.SafeRemainingArgument, out var weight) || weight <= 0)
        {
            actor.OutputHandler.Send("You must supply a positive weight.");
            return false;
        }

        if (BloodModel is null)
        {
            BloodModel = Gameworld.BloodModels.First(x => x.Bloodtypes.Contains(bt));
        }
        else if (!BloodModel.Bloodtypes.Contains(bt))
        {
            actor.OutputHandler.Send("That blood type does not belong to this model's blood model.");
            return false;
        }

        _bloodTypes.RemoveAll(x => x.Bloodtype == bt);
        _bloodTypes.Add((bt, weight));
        Changed = true;
        actor.OutputHandler.Send($"This model now has {bt.Name.ColourValue()} with weight {weight.ToStringN2Colour(actor)}.");
        return true;
    }

    private bool BuildingCommandRemove(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which blood type should be removed?");
            return false;
        }

        var bt = Gameworld.Bloodtypes.GetByIdOrName(command.SafeRemainingArgument);
        if (bt is null)
        {
            actor.OutputHandler.Send("That is not a valid blood type.");
            return false;
        }

        var removed = _bloodTypes.RemoveAll(x => x.Bloodtype == bt);
        if (removed == 0)
        {
            actor.OutputHandler.Send("That blood type is not part of this model.");
            return false;
        }

        Changed = true;
        if (_bloodTypes.Count == 0)
        {
            BloodModel = null;
        }

        actor.OutputHandler.Send($"This model no longer includes {bt.Name.ColourValue()}.");
        return true;
    }

    public string Show(ICharacter actor)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Population Blood Model #{Id.ToStringN0(actor)} - {Name}".GetLineWithTitleInner(actor, Telnet.Cyan, Telnet.BoldWhite));
        sb.AppendLine();
        sb.AppendLine($"Blood Model: {BloodModel?.Name.ColourValue() ?? "None"}");
        var total = _bloodTypes.Sum(x => x.Weight);
        foreach (var (blood, weight) in _bloodTypes.OrderByDescending(x => x.Weight))
        {
            sb.AppendLine($"{blood.Name.ColourValue(),-20} {(weight / total).ToStringP2Colour(actor)}");
        }

        return sb.ToString();
    }
}

