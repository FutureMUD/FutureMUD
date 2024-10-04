using System.Collections.Generic;
using System.Linq;
using System.Text;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.Work.Projects.ConcreteTypes;

public class ActiveLocalProject : ActiveProject, ILocalProject
{
	public ActiveLocalProject(IProject project, ICharacter owner) : base(project)
	{
		_characterOwner = owner;
		_characterOwnerId = owner.Id;
		Location = owner.Location;
		Location.AddProject(this);
		project.OnStartProg?.Execute(this);
	}

	public ActiveLocalProject(MudSharp.Models.ActiveProject project, IFuturemud gameworld) : base(project, gameworld)
	{
		_characterOwnerId = project.CharacterId ?? 0L;
		Location = Gameworld.Cells.Get(project.CellId ?? 0);
		Location.AddProject(this);
	}

	public override void Cancel(ICharacter actor)
	{
		actor.OutputHandler.Handle(new EmoteOutput(new Emote(
			$"@ cancel|cancels the {ProjectDefinition.Name.Colour(Telnet.Cyan)} local project.", actor, actor)));
		ProjectDefinition.OnCancelProg?.Execute(this);
		Location.RemoveProject(this);
		Delete();
	}

	private bool CheckForProjectCompletion()
	{
		if (_labourProgress.All(x => x.Value >= x.Key.TotalProgressRequired) &&
		    _materialProgress.All(x => x.Value >= x.Key.QuantityRequired))
		{
			foreach (var action in CurrentPhase.CompletionActions)
			{
				action.CompleteAction(this);
			}

			var nextPhase =
				ProjectDefinition.Phases.ElementAtOrDefault(ProjectDefinition.Phases.ToList().IndexOf(CurrentPhase) +
				                                            1);
			if (nextPhase != null)
			{
				CurrentPhase = nextPhase;
				_labourProgress.Clear();
				_materialProgress.Clear();
				_activeLabour.Clear();
				Changed = true;
				Location.Handle(
					$"The {ProjectDefinition.Name.Colour(Telnet.Cyan)} local project has entered the {CurrentPhase.Name.ColourBold(Telnet.White)} phase.");
				// TODO - queueing multiple jobs
				return true;
			}

			Location.Handle(
				$"The {ProjectDefinition.Name.Colour(Telnet.Cyan)} local project has been completed.");
			ProjectDefinition.OnFinishProg?.Execute(this);
			Location.RemoveProject(this);
			Delete();
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
			Location.Handle(
				$"The {labour.Name.ColourValue()} labour requirement of the {ProjectDefinition.Name.Colour(Telnet.Cyan)} local project is now complete.");
			foreach (var (ch, _) in _activeLabour.Where(x => x.Labour == labour))
			{
				ch.OutputHandler.Send(
					$"You have finished your work on the {labour.Name.ColourValue()} task of the {ProjectDefinition.Name.Colour(Telnet.Cyan)} project in {Location.HowSeen(ch)}.");
				ch.CurrentProject = (null, null);
			}

			_activeLabour.RemoveAll(x => x.Labour == labour);
			return CheckForProjectCompletion();
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
			Location.Handle(
				$"The requirement for {material.Name.ColourValue()} with the {ProjectDefinition.Name.Colour(Telnet.Cyan)} local project is now complete.");
			CheckForProjectCompletion();
		}
	}

	public override void Join(ICharacter actor, IProjectLabourRequirement labour)
	{
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
	}

	protected override void DatabaseInsert(MudSharp.Models.ActiveProject project)
	{
		project.CellId = Location?.Id;
	}

	public IEnumerable<(ICharacter Character, IProjectLabourRequirement Role)> Workers => _activeLabour;

	public override string ProjectsCommandOutput(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.Append($"[{Id.ToString("N0", actor)}] ");
		sb.Append(ProjectDefinition.Name.Colour(Telnet.Cyan));
		sb.Append(" - Phase ");
		sb.Append(CurrentPhase.PhaseNumber.ToString("N0", actor).ColourValue());
		sb.Append(" - ");
		sb.Append(
			$"{CurrentPhase.LabourRequirements.Sum(x => x.HoursRemaining(this)).ToString("N2", actor).ColourValue()} hours of work remain");
		if (CurrentPhase.MaterialRequirements.Any())
		{
			sb.Append(
				$", materials {(CurrentPhase.MaterialRequirements.Where(x => x.IsMandatoryForProjectCompletion).Sum(x => MaterialProgress[x]) / CurrentPhase.MaterialRequirements.Where(x => x.IsMandatoryForProjectCompletion).Sum(x => x.QuantityRequired)).ToString("P0", actor).ColourValue()} complete");
		}

		return sb.ToString();
	}
}