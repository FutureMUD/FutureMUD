using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.RPG.Checks;
using System;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MudSharp.Work.Projects.LabourRequirements;

public class SupervisionProjectLabour : ProjectLabourBase
{
    public SupervisionProjectLabour(Models.ProjectLabourRequirement labour, IFuturemud gameworld) : base(labour,
        gameworld)
    {
        XElement root = XElement.Parse(labour.Definition);
        MultiplierForOtherLabours =
            double.TryParse(root.Element("MultiplierForOtherLabours")?.Value, out var value) ? value : 1.0;
        TraitScaledMultiplier = bool.TryParse(root.Element("TraitScaledMultiplier")?.Value, out var scaled) && scaled;
        IsMandatoryForProjectCompletion = false;
    }

    public SupervisionProjectLabour(ProjectLabourBase rhs, IProjectPhase newPhase) : base(rhs, newPhase, "supervision")
    {
        MultiplierForOtherLabours = rhs is SupervisionProjectLabour supervision
            ? supervision.MultiplierForOtherLabours
            : 1.0;
        TraitScaledMultiplier = rhs is SupervisionProjectLabour scaled && scaled.TraitScaledMultiplier;
        IsMandatoryForProjectCompletion = false;
    }

    public SupervisionProjectLabour(IFuturemud gameworld, IProjectPhase phase, string name) : base(gameworld, phase,
        "supervision", name)
    {
        MultiplierForOtherLabours = 1.0;
        TraitScaledMultiplier = false;
        IsMandatoryForProjectCompletion = false;
    }

    public override IProjectLabourRequirement Duplicate(IProjectPhase newPhase)
    {
        return new SupervisionProjectLabour(this, newPhase);
    }

    public double MultiplierForOtherLabours { get; protected set; }

    public bool TraitScaledMultiplier { get; protected set; }

    public override double ProgressMultiplierForOtherLabourPerPercentageComplete(IProjectLabourRequirement other,
        IActiveProject project)
    {
        var supervisors = project.ActiveLabour
                                 .Where(x => x.Labour == this)
                                 .Select(x => x.Character)
                                 .Where(x => x != null)
                                 .ToList();
        if (!supervisors.Any())
        {
            return 1.0;
        }

        if (TraitScaledMultiplier && RequiredTrait != null)
        {
            return supervisors.Max(ScaledMultiplierFor);
        }

        return MultiplierForOtherLabours;
    }

    private double ScaledMultiplierFor(ICharacter actor)
    {
        var target = Math.Clamp(
            Gameworld.GetCheck(CheckType.ProjectLabourCheck).TargetNumber(actor, TraitCheckDifficulty, RequiredTrait),
            0.0,
            100.0);
        return 1.0 + (MultiplierForOtherLabours - 1.0) * target / 100.0;
    }

    public override double HourlyProgress(ICharacter actor, bool previewOnly = false)
    {
        if (!previewOnly && TraitScaledMultiplier && RequiredTrait != null)
        {
            Gameworld.GetCheck(CheckType.ProjectLabourCheck).Check(actor, TraitCheckDifficulty, RequiredTrait);
        }

        return previewOnly ? double.Epsilon : 0.0;
    }

    public override double TotalProgressRequiredForDisplay => double.PositiveInfinity;

    public override double HoursRemaining(IActiveProject project)
    {
        return double.PositiveInfinity;
    }

    protected override XElement SaveDefinition()
    {
        var root = base.SaveDefinition();
        root.Add(new XElement("MultiplierForOtherLabours", MultiplierForOtherLabours));
        root.Add(new XElement("TraitScaledMultiplier", TraitScaledMultiplier));
        return root;
    }

    public override string Show(ICharacter actor)
    {
        var scaleText = TraitScaledMultiplier ? " skill-scaled" : string.Empty;
        if (RequiredTrait == null)
        {
            return $"Supervisory labour ({MultiplierForOtherLabours.ToString("P2", actor).ColourValue()}{scaleText} Multiplier)";
        }

        return
            $"Supervisory labour of {RequiredTrait.Name.ColourValue()}(>={MinimumTraitValue.ToString("N2", actor).ColourValue()})@{TraitCheckDifficulty.Describe().ColourValue()}({MultiplierForOtherLabours.ToString("P2", actor).ColourValue()}{scaleText} Multiplier)";
    }

    public override string ShowToPlayer(ICharacter actor)
    {
        if (RequiredTrait == null)
        {
            return $"{$"[Super]".Colour(Telnet.Yellow)} {Name.Colour(Telnet.Cyan)}";
        }

        return
            $"{$"[Super]".Colour(Telnet.Yellow)} {Name.Colour(Telnet.Cyan)} ({RequiredTrait.Name.ColourValue()}(>={RequiredTrait.Decorator.Decorate(MinimumTraitValue)})@{TraitCheckDifficulty.Describe().ColourValue()})";
    }

    protected override bool BuildingCommandShow(ICharacter actor, StringStack command, IProjectPhase phase)
    {
        StringBuilder sb = new();
        sb.AppendLine(
            $"Supervision Project Labour {Id.ToString("N0", actor).ColourValue()} - {Name.Colour(Telnet.Cyan)}");
        sb.AppendLine($"Phase {phase.PhaseNumber.ToString("N0", actor).ColourValue()}");
        sb.AppendLine($"Impact on Others: {MultiplierForOtherLabours.ToString("P2", actor).ColourValue()}");
        sb.AppendLine($"Skill-Scaled Multiplier: {TraitScaledMultiplier.ToColouredString()}");
        sb.AppendLine($"Maximum Workers: {MaximumSimultaneousWorkers.ToString("N0", actor).ColourValue()}");
        sb.AppendLine(
            $"IsQualifiedProg: {IsQualifiedProg?.MXPClickableFunctionNameWithId() ?? "None".Colour(Telnet.Red)}");
        sb.AppendLine($"Tested Trait: {RequiredTrait?.Name.Colour(Telnet.Cyan) ?? "None".Colour(Telnet.Red)}");
        sb.AppendLine($"Trait Difficulty: {TraitCheckDifficulty.Describe().ColourValue()}");
        sb.AppendLine($"Minimum Trait Value: {MinimumTraitValue.ToString("N2", actor).ColourValue()}");
        foreach (ILabourImpact impact in LabourImpacts)
        {
            sb.Append($"\t{impact.Name.ColourName()}: ");
            sb.AppendLine(impact.ShowFull(actor));
        }

        actor.OutputHandler.Send(sb.ToString());
        return true;
    }

    public override bool BuildingCommand(ICharacter actor, StringStack command, IProjectPhase phase)
    {
        switch (command.PopSpeech().ToLowerInvariant())
        {
            case "mandatory":
                actor.OutputHandler.Send("Supervisory labours are always non-mandatory.");
                return false;
            case "multiplier":
            case "mult":
                return BuildingCommandMultiplier(actor, command, phase);
            case "skillmultiplier":
            case "skill multiplier":
            case "skill_multiplier":
            case "traitmultiplier":
            case "trait multiplier":
            case "trait_multiplier":
            case "scaled":
                return BuildingCommandTraitScaledMultiplier(actor);
        }

        return base.BuildingCommand(actor, new StringStack($"\"{command.Last}\" {command.RemainingArgument}"), phase);
    }

    private bool BuildingCommandTraitScaledMultiplier(ICharacter actor)
    {
        TraitScaledMultiplier = !TraitScaledMultiplier;
        Changed = true;
        actor.OutputHandler.Send(
            $"This supervisory labour will {(TraitScaledMultiplier ? "now" : "no longer")} scale its multiplier by the supervisor's trait check target.");
        return true;
    }

    private bool BuildingCommandMultiplier(ICharacter actor, StringStack command, IProjectPhase phase)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send(
                "What multiplier should those working on this labour make to the efforts of those they are supervising?");
            return false;
        }

        if (!NumberUtilities.TryParsePercentage(command.PopSpeech(), out double value))
        {
            actor.OutputHandler.Send("That is not a valid percentage.");
            return false;
        }

        MultiplierForOtherLabours = value;
        Changed = true;
        actor.OutputHandler.Send(
            $"Those working on this project labour will now add a {MultiplierForOtherLabours.ToString("P2", actor).ColourValue()} multiplier to the efforts of others.");
        if (MultiplierForOtherLabours < 1.0)
        {
            actor.OutputHandler.Send(
                "Warning: Did you really intend for this to HINDER others (<100%)? ".Colour(Telnet.Red));
        }

        return true;
    }
}
