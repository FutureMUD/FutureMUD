
using MudSharp.Construction;
using System.IO;

namespace MudSharp.Work.Projects.ConcreteTypes;

public class ActiveLocalProject : ActiveProject, ILocalProject
{
    public ActiveLocalProject(IProject project, ICharacter owner) : base(project)
    {
        _characterOwner = owner.Identity as ICharacter ?? owner;
        _characterOwnerId = CharacterInstanceIdentityComparer.IdentityId(owner);
        Location = owner.Location;
		RoomLayer = owner.RoomLayer;
		RoutePositionMetres = RouteSpatialService.Instance.GetEffectiveLocation(owner).RoutePositionMetres;
		LocalProjectSpatialRules.ValidateLoadedSite(Location, RoomLayer, RoutePositionMetres, Id);
        Location.AddProject(this);
        project.OnStartProg?.Execute(this);
    }

    public ActiveLocalProject(MudSharp.Models.ActiveProject project, IFuturemud gameworld) : base(project, gameworld)
    {
        _characterOwnerId = project.CharacterId ?? 0L;
        Location = Gameworld.Cells.Get(project.CellId ?? 0);
		if (Location is null)
		{
			throw new InvalidDataException(
				$"Active local project #{project.Id} refers to missing cell #{project.CellId?.ToString() ?? "null"}.");
		}

		RoomLayer = (RoomLayer)project.RoomLayer;
		RoutePositionMetres = (double?)project.RoutePosition;
		LocalProjectSpatialRules.ValidateLoadedSite(Location, RoomLayer, RoutePositionMetres, project.Id);
        Location.AddProject(this);
    }

	public RoomLayer RoomLayer { get; }
	public double? RoutePositionMetres { get; }
	public SpatialLocation SpatialLocation => new(Location, RoomLayer, RoutePositionMetres);

	public bool IsAtProjectSite(ICharacter character)
	{
		return LocalProjectSpatialRules.IsAtSite(SpatialLocation, character);
	}

	private IReadOnlyCollection<ICharacter> CharactersAtProjectSite()
	{
		return LocalProjectSpatialRules.CharactersAtSite(SpatialLocation);
	}

	public void HandleAtProjectSite(string text)
	{
		LocalProjectSpatialRules.HandleAtSite(SpatialLocation, text);
	}

    public override void Cancel(ICharacter actor)
    {
		var locationCharacters = CharactersAtProjectSite();
        actor.OutputHandler.Handle(new EmoteOutput(new Emote(
            $"@ cancel|cancels the {ProjectDefinition.Name.Colour(Telnet.Cyan)} local project.", actor, actor)));
        ProjectDefinition.OnCancelProg?.Execute(this);
        ClearWorkersFromProject();
        Location.RemoveProject(this);
        Delete();
        TryJoinQueuedProjectLabourFor(locationCharacters);
    }

    private void ClearWorkersFromProject()
    {
        foreach ((ICharacter character, _) in _activeLabour.ToList())
        {
            if (character.CurrentProject.Project == this)
            {
                character.CurrentProject = (null, null);
            }
        }

        _activeLabour.Clear();
    }

    private bool CheckForProjectCompletion()
    {
        if (AreCurrentPhaseCompletionRequirementsMet())
        {
            foreach (IProjectAction action in OrderedCompletionActions())
            {
                action.CompleteAction(this);
            }

            IProjectPhase nextPhase =
                ProjectDefinition.Phases.ElementAtOrDefault(ProjectDefinition.Phases.ToList().IndexOf(CurrentPhase) +
                                                            1);
            if (nextPhase != null)
            {
                CurrentPhase = nextPhase;
                _labourProgress.Clear();
                _materialProgress.Clear();
                _labourPaymentRates.Clear();
                _materialPaymentRates.Clear();
                ClearWorkersFromProject();
                Changed = true;
				HandleAtProjectSite(
                    $"The {ProjectDefinition.Name.Colour(Telnet.Cyan)} local project has entered the {CurrentPhase.Name.ColourBold(Telnet.White)} phase.");
				TryJoinQueuedProjectLabourFor(CharactersAtProjectSite());
                return true;
            }

			var locationCharacters = CharactersAtProjectSite();
			HandleAtProjectSite(
                $"The {ProjectDefinition.Name.Colour(Telnet.Cyan)} local project has been completed.");
            ProjectDefinition.OnFinishProg?.Execute(this);
            ClearWorkersFromProject();
            Location.RemoveProject(this);
            Delete();
            TryJoinQueuedProjectLabourFor(locationCharacters);
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
			HandleAtProjectSite(
                $"The {labour.Name.ColourValue()} labour requirement of the {ProjectDefinition.Name.Colour(Telnet.Cyan)} local project is now complete.");
            foreach ((ICharacter ch, IProjectLabourRequirement _) in _activeLabour.Where(x => x.Labour == labour))
            {
                ch.OutputHandler.Send(
                    $"You have finished your work on the {labour.Name.ColourValue()} task of the {ProjectDefinition.Name.Colour(Telnet.Cyan)} project in {Location.HowSeen(ch)}.");
                ch.CurrentProject = (null, null);
            }

            _activeLabour.RemoveAll(x => x.Labour == labour);
            var changedProjectState = CheckForProjectCompletion();
            if (!changedProjectState)
            {
				TryJoinQueuedProjectLabourFor(CharactersAtProjectSite());
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
			HandleAtProjectSite(
                $"The requirement for {material.Name.ColourValue()} with the {ProjectDefinition.Name.Colour(Telnet.Cyan)} local project is now complete.");
            CheckForProjectCompletion();
        }
    }

    public override void Join(ICharacter actor, IProjectLabourRequirement labour)
    {
		if (!IsAtProjectSite(actor))
		{
			actor.OutputHandler.Send("You are too far away from that local project to work on it.");
			return;
		}

        actor.CurrentProject.Project?.Leave(actor);
        _activeLabour.Add((actor, labour));
        actor.CurrentProject = (this, labour);
        actor.OutputHandler.Handle(new EmoteOutput(
            new Emote(
                $"@ begin|begins working on the {labour.Name.ColourValue()} task of the {ProjectDefinition.Name.Colour(Telnet.Cyan)} local project.",
                actor, actor), flags: OutputFlags.SuppressObscured));
    }

    public override void Leave(ICharacter actor)
    {
        _activeLabour.RemoveAll(x => x.Character == actor);
        actor.CurrentProject = (null, null);
        actor.OutputHandler.Handle(new EmoteOutput(new Emote(
            $"@ stop|stops all work on the {ProjectDefinition.Name.Colour(Telnet.Cyan)} local project.", actor,
            actor)));
		TryJoinQueuedProjectLabourFor(CharactersAtProjectSite().Where(x => x != actor).ToList());
    }

	public override void DoProjectsTick()
	{
		foreach (var worker in _activeLabour
			         .Where(x => !IsAtProjectSite(x.Character))
			         .ToList())
		{
			_activeLabour.Remove(worker);
			if (worker.Character.CurrentProject.Project == this)
			{
				worker.Character.CurrentProject = (null, null);
			}

			worker.Character.OutputHandler.Send(
				$"You stop working on the {worker.Labour.Name.ColourName()} task because you have moved away from the local project.");
		}

		base.DoProjectsTick();
	}

    protected override void DatabaseInsert(MudSharp.Models.ActiveProject project)
    {
        project.CharacterId = _characterOwnerId;
        project.CellId = Location?.Id;
		project.RoomLayer = (int)RoomLayer;
		project.RoutePosition = RoutePositionMetres.HasValue
			? Math.Round((decimal)RoutePositionMetres.Value, 3, MidpointRounding.AwayFromZero)
			: null;
    }

    public IEnumerable<(ICharacter Character, IProjectLabourRequirement Role)> Workers => _activeLabour;

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
