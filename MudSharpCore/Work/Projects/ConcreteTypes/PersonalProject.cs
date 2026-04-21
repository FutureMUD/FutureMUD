using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MudSharp.Work.Projects.ConcreteTypes;

public class PersonalProject : Project
{
    public PersonalProject(MudSharp.Models.Project project, IFuturemud gameworld) : base(project, gameworld)
    {
    }

    public PersonalProject(IAccount originator) : base(originator, "personal")
    {
    }

    public override IEnumerable<string> ProjectCatalogueColumns(ICharacter actor)
    {
        return new[]
        {
            Name,
            Tagline,
            $"{Phases.Sum(x => x.LabourRequirements.Sum(y => y.TotalProgressRequiredForDisplay)).ToString("N2", actor)} man-hours",
            "Yes"
        };
    }

    public override void InitiateProject(ICharacter actor)
    {
        actor.OutputHandler.Send($"You begin the {Name.Colour(Telnet.Cyan)} personal project.");
        ActivePersonalProject project = new(this, actor);
        Gameworld.Add(project);
        actor.AddPersonalProject(project);
    }

    protected override string WhyCannotCancelProjectInvariant(ICharacter actor, IActiveProject local)
    {
        if (Gameworld.ActiveJobs.Any(x => !x.IsJobComplete && x.ActiveProject == local))
        {
            return "You cannot cancel that personal project while an active job still depends on it.";
        }

        return string.Empty;
    }

    protected override string WhyCannotCancelProjectFallback(ICharacter actor, IActiveProject local)
    {
        return actor == local.CharacterOwner
            ? string.Empty
            : "Only the owner of that personal project can cancel it.";
    }

    public override string ShowToPlayer(ICharacter actor)
    {
        StringBuilder sb = new();
        sb.AppendLine($"Personal Project: {Name.Colour(Telnet.Cyan)}");
        sb.AppendLine($"Tagline: {Tagline.ColourCommand()}");
        sb.AppendLine(
            $"You currently {(CanInitiateProg.Execute<bool?>(actor) == true ? "can".Colour(Telnet.Green) : "cannot".Colour(Telnet.Red))} initiate this project.");
        foreach (IProjectPhase phase in Phases)
        {
            sb.AppendLine();
            sb.AppendLine($"Phase {phase.PhaseNumber.ToString("N0", actor)} - {phase.Description}".GetLineWithTitle(
                actor.LineFormatLength, actor.Account.UseUnicode, Telnet.Blue, Telnet.BoldMagenta));
            foreach (IProjectLabourRequirement labour in phase.LabourRequirements)
            {
                sb.AppendLine($"\t{labour.ShowToPlayer(actor)}");
                foreach (ILabourImpact impact in labour.LabourImpacts)
                {
                    sb.AppendLine($"\t\t* {impact.ShowToPlayer(actor)}");
                }
            }

            foreach (IProjectMaterialRequirement material in phase.MaterialRequirements)
            {
                sb.AppendLine($"\t[Material {material.Name.Colour(Telnet.Cyan)}] {material.ShowToPlayer(actor)}");
            }

            foreach (IProjectAction action in phase.CompletionActions)
            {
                sb.AppendLine($"\t[Action] {action.ShowToPlayer(actor)}");
            }
        }

        return sb.ToString();
    }

    protected override XElement SaveDefinition(XElement baseDefinition)
    {
        return baseDefinition;
    }

    public override string Show(ICharacter actor)
    {
        StringBuilder sb = new();
        sb.AppendLine($"Personal Project {Id.ToString("N0", actor)}r{RevisionNumber.ToString("N0", actor)} - {Name}");
        sb.AppendLine("Main".GetLineWithTitle(actor.LineFormatLength, actor.Account.UseUnicode, Telnet.Cyan,
            Telnet.BoldYellow));
        sb.AppendLine(
            $"AppearInListProg: {AppearInProjectListProg?.MXPClickableFunctionNameWithId() ?? "None".Colour(Telnet.Red)}");
        sb.AppendLine(
            $"CanInitiateProg: {CanInitiateProg?.MXPClickableFunctionNameWithId() ?? "None".Colour(Telnet.Red)}");
        sb.AppendLine(
            $"WhyCannotInitiateProg: {WhyCannotInitiateProg?.MXPClickableFunctionNameWithId() ?? "None".Colour(Telnet.Red)}");
        sb.AppendLine(
            $"CanCancelProg: {CanCancelProg?.MXPClickableFunctionNameWithId() ?? "None".Colour(Telnet.Red)}");
        sb.AppendLine(
            $"WhyCannotCancelProg: {WhyCannotCancelProg?.MXPClickableFunctionNameWithId() ?? "None".Colour(Telnet.Red)}");
        sb.AppendLine($"Appear in Job List: {AppearInJobsList.ToColouredString()}");
        sb.AppendLine($"Tagline: {Tagline}");
        sb.AppendLine("Phases".GetLineWithTitle(actor.LineFormatLength, actor.Account.UseUnicode, Telnet.Cyan,
            Telnet.BoldYellow));
        foreach (IProjectPhase phase in Phases)
        {
            sb.AppendLine();
            sb.AppendLine($"Phase {phase.PhaseNumber.ToString("N0", actor)}".GetLineWithTitle(actor.LineFormatLength,
                actor.Account.UseUnicode, Telnet.Blue, Telnet.BoldMagenta));
            sb.AppendLine($"Description: {phase.Description}");
            foreach (IProjectLabourRequirement labour in phase.LabourRequirements)
            {
                sb.AppendLine($"\t[LabourReq {labour.Name}] {labour.Show(actor)}");
                foreach (ILabourImpact impact in labour.LabourImpacts)
                {
                    sb.AppendLine($"\t\t[Impact {impact.Name}] {impact.ShowFull(actor)}");
                }
            }

            foreach (IProjectMaterialRequirement material in phase.MaterialRequirements)
            {
                sb.AppendLine($"\t[MaterialReq {material.Name}] {material.Show(actor)}");
            }

            foreach (IProjectAction action in phase.CompletionActions)
            {
                sb.AppendLine($"\t[Action {action.Name}] {action.Show(actor)}");
            }
        }

        return sb.ToString();
    }

    #region Overrides of Project

    protected override string HelpText => $"{base.HelpText}\n\t#3jobs#0 - toggles appearing in the jobs project list";

    public override bool BuildingCommand(ICharacter actor, StringStack command)
    {
        switch (command.PopForSwitch())
        {
            case "jobs":
                return BuildingCommandJobs(actor, command);
        }

        return base.BuildingCommand(actor, command.GetUndo());
    }

    private bool BuildingCommandJobs(ICharacter actor, StringStack command)
    {
        AppearInJobsList = !AppearInJobsList;
        Changed = true;
        actor.OutputHandler.Send(
            $"This project will {(AppearInJobsList ? "now" : "no longer")} appear in the list of personal projects that jobs can choose from.");
        return true;
    }

    #endregion

    public override IActiveProject LoadActiveProject(MudSharp.Models.ActiveProject project)
    {
        return new ActivePersonalProject(project, Gameworld);
    }
}
