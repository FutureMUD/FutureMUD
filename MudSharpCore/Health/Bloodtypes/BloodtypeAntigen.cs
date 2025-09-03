using System.Linq;
using System.Text;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.PerceptionEngine;

namespace MudSharp.Health.Bloodtypes;

#nullable enable
public class BloodtypeAntigen : SaveableItem, IBloodtypeAntigen
{
    public BloodtypeAntigen(MudSharp.Models.BloodtypeAntigen dbitem, IFuturemud gameworld)
    {
        Gameworld = gameworld;
        _id = dbitem.Id;
        _name = dbitem.Name;
    }

    public BloodtypeAntigen(IFuturemud gameworld, string name)
    {
        Gameworld = gameworld;
        _name = name;
        using (new FMDB())
        {
            var dbitem = new MudSharp.Models.BloodtypeAntigen { Name = name };
            FMDB.Context.BloodtypeAntigens.Add(dbitem);
            FMDB.Context.SaveChanges();
            _id = dbitem.Id;
        }
    }

    public override string FrameworkItemType => "BloodtypeAntigen";

    public override void Save()
    {
        var dbitem = FMDB.Context.BloodtypeAntigens.Find(Id);
        dbitem.Name = Name;
        Changed = false;
    }

    public const string HelpText = @"You can use the following options with this command:

name <name> - renames this blood antigen";

    public bool BuildingCommand(ICharacter actor, StringStack command)
    {
        switch (command.PopForSwitch())
        {
            case "name":
                return BuildingCommandName(actor, command);
        }

        actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
        return false;
    }

    private bool BuildingCommandName(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("What new name should this blood antigen have?");
            return false;
        }

        var name = command.SafeRemainingArgument.TitleCase();
        if (Gameworld.BloodtypeAntigens.Any(x => x.Name.EqualTo(name)))
        {
            actor.OutputHandler.Send($"There is already a blood antigen named {name.ColourName()}.");
            return false;
        }

        _name = name;
        Changed = true;
        actor.OutputHandler.Send($"This blood antigen is now called {name.ColourName()}.");
        return true;
    }

    public string Show(ICharacter actor)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Blood Antigen #{Id.ToStringN0(actor)} - {Name}".GetLineWithTitleInner(actor, Telnet.Cyan, Telnet.BoldWhite));
        return sb.ToString();
    }
}

