using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MudSharp.Work.Projects.ConcreteTypes;

public class LocalProject : Project
{
    public LocalProject(MudSharp.Models.Project project, IFuturemud gameworld) : base(project, gameworld)
    {
    }

    public LocalProject(IAccount originator) : base(originator, "local")
    {
    }

    public override IEnumerable<string> ProjectCatalogueColumns(ICharacter actor)
    {
        return new[]
        {
            Name,
            Tagline,
            $"{Phases.Sum(x => x.LabourRequirements.Sum(y => y.TotalProgressRequiredForDisplay)).ToString("N2", actor)} man-hours",
            "No"
        };
    }

    public override string ShowToPlayer(ICharacter actor)
    {
        StringBuilder sb = new();
        sb.AppendLine($"Local Project: {Name.Colour(Telnet.Cyan)}");
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

    public override void InitiateProject(ICharacter actor)
    {
        actor.OutputHandler.Handle(
            new EmoteOutput(new Emote($"@ begin|begins the {Name.Colour(Telnet.Cyan)} local project.", actor, actor)));
        ActiveLocalProject project = new(this, actor);
        Gameworld.Add(project);
    }

    protected override string WhyCannotCancelProjectFallback(ICharacter actor, IActiveProject local)
    {
        return actor == local.CharacterOwner
            ? string.Empty
            : "Only the owner of that local project can cancel it.";
    }

    protected override XElement SaveDefinition(XElement baseDefinition)
    {
        return baseDefinition;
    }

    public override IActiveProject LoadActiveProject(MudSharp.Models.ActiveProject project)
    {
        return new ActiveLocalProject(project, Gameworld);
    }

    public override string Show(ICharacter actor)
    {
        StringBuilder sb = new();
        sb.AppendLine($"Local Project {Id.ToString("N0", actor)}r{RevisionNumber.ToString("N0", actor)} - {Name}");
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
}
