using System.Linq;
using System.Text;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework;

namespace MudSharp.Work.Projects.ConcreteTypes;

public class ActivePersonalProject : ActiveProject, IPersonalProject
{
	public ActivePersonalProject(MudSharp.Models.ActiveProject project, IFuturemud gameworld) : base(project, gameworld)
	{
		_characterOwnerId = project.CharacterId ?? 0L;
	}

	public ActivePersonalProject(IProject project, ICharacter owner) : base(project)
	{
		_characterOwner = owner;
		_characterOwnerId = owner.Id;
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
		CharacterOwner.RemovePersonalProject(this);
		Delete();
	}

	private bool CheckForProjectCompletion(bool alreadyWorkingOnProject)
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
				Changed = true;
				if (CharacterOwner.Location?.Characters.Contains(CharacterOwner) == true)
				{
					CharacterOwner.OutputHandler.Send(
						$"Your {ProjectDefinition.Name.Colour(Telnet.Cyan)} personal project has entered the {CurrentPhase.Name.ColourBold(Telnet.White)} phase.");
				}

				var newLabour =
					CurrentPhase.LabourRequirements.FirstOrDefault(x => x.CharacterIsQualified(CharacterOwner));
				if (alreadyWorkingOnProject && newLabour != null)
				{
					_activeLabour.Add((CharacterOwner, newLabour));
					CharacterOwner.CurrentProject = (this, newLabour);
					if (CharacterOwner.Location?.Characters.Contains(CharacterOwner) == true)
					{
						CharacterOwner.OutputHandler.Send(
							$"You begin working on the {newLabour.Name.ColourValue()} task of your {ProjectDefinition.Name.Colour(Telnet.Cyan)} personal project.");
					}
				}

				return true;
			}

			if (CharacterOwner.Location?.Characters.Contains(CharacterOwner) == true)
			{
				CharacterOwner.OutputHandler.Send(
					$"Your {ProjectDefinition.Name.Colour(Telnet.Cyan)} personal project has been completed.");
			}

			ProjectDefinition.OnFinishProg?.Execute(this);
			CharacterOwner.RemovePersonalProject(this);
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
			if (CharacterOwner.Location?.Characters.Contains(CharacterOwner) == true)
			{
				CharacterOwner.OutputHandler.Send(
					$"You have finished your work on the {labour.Name.ColourValue()} task of your {ProjectDefinition.Name.Colour(Telnet.Cyan)} personal project.");
			}

			CharacterOwner.CurrentProject = (null, null);
			_activeLabour.Clear();
			return CheckForProjectCompletion(true);
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

			CheckForProjectCompletion(ActiveLabour.Any(x => x.Character == CharacterOwner));
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