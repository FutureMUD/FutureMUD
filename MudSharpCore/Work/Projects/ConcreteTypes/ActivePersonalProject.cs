using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MudSharp.Work.Projects.ConcreteTypes;

public class ActivePersonalProject : ActiveProject, IPersonalProject
{
    public ActivePersonalProject(MudSharp.Models.ActiveProject project, IFuturemud gameworld) : base(project, gameworld)
    {
        _characterOwnerId = project.CharacterId ?? 0L;
    }

    public ActivePersonalProject(IProject project, ICharacter owner) : base(project)
    {
        _characterOwner = owner.Identity as ICharacter ?? owner;
        _characterOwnerId = CharacterInstanceIdentityComparer.IdentityId(owner);
        project.OnStartProg?.Execute(this);
    }

    protected override void DatabaseInsert(MudSharp.Models.ActiveProject project)
    {
        project.CharacterId = _characterOwnerId;
    }

    public override ICell Location
    {
        get => CharacterOwner.Location;
        protected init { }
    }

    public override void Cancel(ICharacter actor)
    {
        actor.OutputHandler.Send($"You cancel your personal project '{Name.Colour(Telnet.Cyan)}'.");
        ProjectDefinition.OnCancelProg?.Execute(this);
        var workers = ClearWorkersFromProject();
        CharacterOwner.RemovePersonalProject(this);
        Delete();
        foreach (var worker in workers.DefaultIfEmpty(CharacterOwner))
        {
            worker.TryJoinQueuedProjectLabour();
        }
    }

    private List<ICharacter> ClearWorkersFromProject()
    {
        var workers = _activeLabour.Select(x => x.Character).DistinctBy(x => x.InstanceId).ToList();
        foreach (var worker in workers.Where(x => x.CurrentProject.Project == this))
        {
            worker.CurrentProject = (null, null);
        }

        _activeLabour.Clear();
        return workers;
    }

    private List<ICharacter> ClearWorkersFromLabour(IProjectLabourRequirement labour)
    {
        var workers = _activeLabour
                      .Where(x => x.Labour == labour)
                      .Select(x => x.Character)
                      .DistinctBy(x => x.InstanceId)
                      .ToList();
        foreach (var worker in workers.Where(x => x.CurrentProject.Project == this))
        {
            worker.CurrentProject = (null, null);
        }

        _activeLabour.RemoveAll(x => x.Labour == labour);
        return workers;
    }

    private bool CheckForProjectCompletion(IEnumerable<ICharacter> workersToResume)
    {
        if (AreCurrentPhaseCompletionRequirementsMet())
        {
            var resumeList = workersToResume.DistinctBy(x => x.InstanceId).ToList();
            foreach (IProjectAction action in OrderedCompletionActions())
            {
                action.CompleteAction(this);
            }

            IProjectPhase nextPhase =
                ProjectDefinition.Phases.ElementAtOrDefault(ProjectDefinition.Phases.ToList().IndexOf(CurrentPhase) +
                                                            1);
            if (nextPhase != null)
            {
                ClearWorkersFromProject();
                CurrentPhase = nextPhase;
                _labourProgress.Clear();
                _materialProgress.Clear();
                _labourPaymentRates.Clear();
                _materialPaymentRates.Clear();
                Changed = true;
                if (CharacterOwner.Location?.Characters.Contains(CharacterOwner) == true)
                {
                    CharacterOwner.OutputHandler.Send(
                        $"Your {ProjectDefinition.Name.Colour(Telnet.Cyan)} personal project has entered the {CurrentPhase.Name.ColourBold(Telnet.White)} phase.");
                }

                foreach (var worker in resumeList)
                {
                    var newLabour =
                        CurrentPhase.LabourRequirements
                                    .FirstOrDefault(x =>
                                        x.CharacterIsQualified(worker) &&
                                        _activeLabour.Count(y => y.Labour == x) < x.MaximumSimultaneousWorkers);
                    if (newLabour == null)
                    {
                        continue;
                    }

                    _activeLabour.Add((worker, newLabour));
                    worker.CurrentProject = (this, newLabour);
                    if (worker.Location?.Characters.Contains(worker) == true)
                    {
                        worker.OutputHandler.Send(
                            $"You begin working on the {newLabour.Name.ColourValue()} task of your {ProjectDefinition.Name.Colour(Telnet.Cyan)} personal project.");
                    }
                }

                foreach (var worker in resumeList.Where(x => x.CurrentProject.Project == null).DefaultIfEmpty(CharacterOwner))
                {
                    worker.TryJoinQueuedProjectLabour();
                }

                return true;
            }

            if (CharacterOwner.Location?.Characters.Contains(CharacterOwner) == true)
            {
                CharacterOwner.OutputHandler.Send(
                    $"Your {ProjectDefinition.Name.Colour(Telnet.Cyan)} personal project has been completed.");
            }

            ProjectDefinition.OnFinishProg?.Execute(this);
            var workers = ClearWorkersFromProject();
            CharacterOwner.RemovePersonalProject(this);
            Delete();
            foreach (var worker in workers.DefaultIfEmpty(CharacterOwner))
            {
                worker.TryJoinQueuedProjectLabour();
            }
            return true;
        }

        return false;
    }

    public override bool FulfilLabour(IProjectLabourRequirement labour, double progress)
    {
        _labourProgress[labour] += progress;
        Changed = true;
        if (_labourProgress[labour] >= labour.TotalProgressRequired)
        {
            _labourProgress[labour] = labour.TotalProgressRequired;
            var workers = ClearWorkersFromLabour(labour);
            foreach (var worker in workers.DefaultIfEmpty(CharacterOwner))
            {
                if (worker.Location?.Characters.Contains(worker) == true)
                {
                    worker.OutputHandler.Send(
                        $"You have finished your work on the {labour.Name.ColourValue()} task of your {ProjectDefinition.Name.Colour(Telnet.Cyan)} personal project.");
                }
            }

            var changedProjectState = CheckForProjectCompletion(workers);
            if (!changedProjectState)
            {
                foreach (var worker in workers.DefaultIfEmpty(CharacterOwner))
                {
                    worker.TryJoinQueuedProjectLabour();
                }
            }

            return changedProjectState;
        }

        return false;
    }

    public override void FulfilMaterial(IProjectMaterialRequirement material, double progress)
    {
        _materialProgress[material] += progress;
        Changed = true;
        if (_materialProgress[material] >= material.QuantityRequired)
        {
            _materialProgress[material] = material.QuantityRequired;
            if (CharacterOwner.Location?.Characters.Contains(CharacterOwner) == true)
            {
                CharacterOwner.OutputHandler.Send(
                    $"The requirement for {material.Name.ColourValue()} with your {ProjectDefinition.Name.Colour(Telnet.Cyan)} personal project is now complete.");
            }

            CheckForProjectCompletion(ActiveLabour.Select(x => x.Character));
        }
    }

    public override void Join(ICharacter actor, IProjectLabourRequirement labour)
    {
        actor.CurrentProject.Project?.Leave(actor);
        _activeLabour.Add((actor, labour));
        actor.CurrentProject = (this, labour);
        actor.OutputHandler.Send(
            $"You begin working on the {labour.Name.ColourValue()} task of your {ProjectDefinition.Name.Colour(Telnet.Cyan)} personal project.");
    }

    public override void Leave(ICharacter actor)
    {
        _activeLabour.RemoveAll(x => x.Character == actor);
        actor.CurrentProject = (null, null);
        actor.OutputHandler.Send(
            $"You stop all work on your {ProjectDefinition.Name.Colour(Telnet.Cyan)} personal project.");
    }

    public override string ProjectsCommandOutput(ICharacter actor)
    {
        StringBuilder sb = new();
        sb.Append($"[{Id.ToString("N0", actor)}] ");
        sb.Append(ProjectDefinition.Name.Colour(Telnet.Cyan));
        sb.Append(" - Phase ");
        sb.Append(CurrentPhase.PhaseNumber.ToString("N0", actor).ColourValue());
        sb.Append(" - ");
        sb.Append(
            $"{CurrentPhase.LabourRequirements.Sum(x => x.HoursRemaining(this)).ToString("N2", actor).ColourValue()} hours of work remain");
        var mandatoryMaterialCompletion = MandatoryMaterialCompletionRatio();
        if (mandatoryMaterialCompletion.HasValue)
        {
            sb.Append(
                $", materials {mandatoryMaterialCompletion.Value.ToString("P0", actor).ColourValue()} complete");
        }
        if (HasSatisfiedButJoinableLabour(actor))
        {
            sb.Append(", satisfied but joinable".ColourCommand());
        }

        return sb.ToString();
    }
}
