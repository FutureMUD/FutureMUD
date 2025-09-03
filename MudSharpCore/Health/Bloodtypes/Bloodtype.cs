using System.Collections.Generic;
using System.Linq;
using System.Text;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.PerceptionEngine;

namespace MudSharp.Health.Bloodtypes;

#nullable enable
public class Bloodtype : SaveableItem, IBloodtype
{
    public Bloodtype(MudSharp.Models.Bloodtype dbitem, IFuturemud gameworld)
    {
        Gameworld = gameworld;
        _id = dbitem.Id;
        _name = dbitem.Name;
        _antigens.AddRange(gameworld.BloodtypeAntigens.Where(x =>
            dbitem.BloodtypesBloodtypeAntigens.Any(y => y.BloodtypeAntigenId == x.Id)));
    }

    public Bloodtype(IFuturemud gameworld, string name)
    {
        Gameworld = gameworld;
        _name = name;
        using (new FMDB())
        {
            var dbitem = new MudSharp.Models.Bloodtype { Name = name };
            FMDB.Context.Bloodtypes.Add(dbitem);
            FMDB.Context.SaveChanges();
            _id = dbitem.Id;
        }
    }

    public override string FrameworkItemType => "Bloodtype";

    public override void Save()
    {
        var dbitem = FMDB.Context.Bloodtypes.Find(Id);
        dbitem.Name = Name;
        FMDB.Context.BloodtypesBloodtypeAntigens.RemoveRange(dbitem.BloodtypesBloodtypeAntigens);
        foreach (var antigen in _antigens)
        {
            dbitem.BloodtypesBloodtypeAntigens.Add(new MudSharp.Models.BloodtypesBloodtypeAntigens
            {
                BloodtypeId = Id,
                BloodtypeAntigenId = antigen.Id
            });
        }

        Changed = false;
    }

    private readonly List<IBloodtypeAntigen> _antigens = new();
    public IEnumerable<IBloodtypeAntigen> Antigens => _antigens;

    public bool IsCompatibleWithDonorBlood(IBloodtype donorBloodtype) =>
        donorBloodtype.Antigens.All(x => _antigens.Contains(x));

    public const string HelpText = @"You can use the following options with this command:

name <name> - renames this blood type
antigen <antigen> - toggles an antigen for this blood type";

    public bool BuildingCommand(ICharacter actor, StringStack command)
    {
        switch (command.PopForSwitch())
        {
            case "name":
                return BuildingCommandName(actor, command);
            case "antigen":
            case "ant":
                return BuildingCommandAntigen(actor, command);
        }

        actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
        return false;
    }

    private bool BuildingCommandName(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("What new name should this blood type have?");
            return false;
        }

        var name = command.SafeRemainingArgument.TitleCase();
        if (Gameworld.Bloodtypes.Any(x => x.Name.EqualTo(name)))
        {
            actor.OutputHandler.Send($"There is already a blood type named {name.ColourName()}.");
            return false;
        }

        _name = name;
        Changed = true;
        actor.OutputHandler.Send($"This blood type is now called {name.ColourName()}.");
        return true;
    }

    private bool BuildingCommandAntigen(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which antigen do you want to toggle?");
            return false;
        }

        var antigen = Gameworld.BloodtypeAntigens.GetByIdOrName(command.SafeRemainingArgument);
        if (antigen is null)
        {
            actor.OutputHandler.Send($"There is no blood antigen identified by {command.SafeRemainingArgument.ColourCommand()}.");
            return false;
        }

        if (_antigens.Contains(antigen))
        {
            _antigens.Remove(antigen);
            actor.OutputHandler.Send($"This blood type no longer has the {antigen.Name.ColourName()} antigen.");
        }
        else
        {
            _antigens.Add(antigen);
            actor.OutputHandler.Send($"This blood type now has the {antigen.Name.ColourName()} antigen.");
        }

        Changed = true;
        return true;
    }

    public string Show(ICharacter actor)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Blood Type #{Id.ToStringN0(actor)} - {Name}".GetLineWithTitleInner(actor, Telnet.Cyan, Telnet.BoldWhite));
        sb.AppendLine();
        sb.AppendLine($"Antigens: {_antigens.Select(x => x.Name.ColourValue()).ListToString()}");
        return sb.ToString();
    }
}

