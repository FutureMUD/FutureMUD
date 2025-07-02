using System.Xml.Linq;
using MudSharp.Models;
using MudSharp.Framework;
using MudSharp.RPG.Merits.Interfaces;
using MudSharp.Character;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.RPG.Merits.CharacterMerits;

public class ScatterChanceMerit : CharacterMeritBase, IScatterChanceMerit
{
    protected ScatterChanceMerit(Merit merit, IFuturemud gameworld) : base(merit, gameworld)
    {
        var definition = XElement.Parse(merit.Definition);
        ScatterMultiplier = double.Parse(definition.Element("Multiplier")?.Value ?? "1.0");
    }

    protected ScatterChanceMerit() { }

    protected ScatterChanceMerit(IFuturemud gameworld, string name) : base(gameworld, name, "Scatter Chance", "@ affect|affects the chance of scattered shots")
    {
        ScatterMultiplier = 1.0;
        DoDatabaseInsert();
    }

    public static void RegisterMeritInitialiser()
    {
        MeritFactory.RegisterMeritInitialiser("Scatter Chance", (merit, gameworld) => new ScatterChanceMerit(merit, gameworld));
        MeritFactory.RegisterBuilderMeritInitialiser("Scatter Chance", (gameworld, name) => new ScatterChanceMerit(gameworld, name));
        MeritFactory.RegisterMeritHelp("Scatter Chance", "Modifies how likely a ranged attack is to scatter", new ScatterChanceMerit().HelpText);
    }

    protected override XElement SaveSubtypeDefinition(XElement root)
    {
        root.Add(new XElement("Multiplier", ScatterMultiplier));
        return root;
    }

    protected override void SubtypeShow(ICharacter actor, System.Text.StringBuilder sb)
    {
        sb.AppendLine($"Scatter Multiplier: {ScatterMultiplier.ToString("P2", actor).ColourValue()}");
    }

    protected override string SubtypeHelp => $@"{base.SubtypeHelp}
        #3multiplier <%>#0 - sets the scatter multiplier";

    public override bool BuildingCommand(ICharacter actor, StringStack command)
    {
        switch (command.PopForSwitch())
        {
            case "multiplier":
            case "mult":
            case "mod":
                return BuildingCommandMultiplier(actor, command);
        }
        return base.BuildingCommand(actor, command.GetUndo());
    }

    private bool BuildingCommandMultiplier(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("What multiplier should be applied to scatter chance?");
            return false;
        }

        if (!command.SafeRemainingArgument.TryParsePercentage(actor.Account.Culture, out var value))
        {
            actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid percentage.");
            return false;
        }

        ScatterMultiplier = value;
        Changed = true;
        actor.OutputHandler.Send($"Scatter multiplier set to {ScatterMultiplier.ToString("P2", actor).ColourValue()}.");
        return true;
    }

    public double ScatterMultiplier { get; private set; }
}
