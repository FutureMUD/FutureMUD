using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Economy;
using MudSharp.Work.Projects;

namespace MudSharp.Character;

public partial class Character
{
	private void SaveProjects(MudSharp.Models.Character character)
	{
		character.CurrentProjectId = _currentProject.Project?.Id;
		character.CurrentProjectLabourId = _currentProject.Labour?.Id;
		character.CurrentProjectHours = CurrentProjectHours;
	}

	private void LoadProjects(MudSharp.Models.Character character)
	{
		_personalProjects.AddRange(character.ActiveProjects.Select(x => Gameworld.ActiveProjects.Get(x.Id)));
		var project = Gameworld.ActiveProjects.Get(character.CurrentProjectId ?? 0);
		_currentProject = (project,
			project?.CurrentPhase.LabourRequirements.First(x => x.Id == character.CurrentProjectLabourId));
		_currentProjectHours = character.CurrentProjectHours;
	}

	private readonly List<IActiveJob> _activeJobs = new();
	public IEnumerable<IActiveJob> ActiveJobs => _activeJobs;

	public void AddJob(IActiveJob job)
	{
		_activeJobs.Add(job);
		Changed = true;
	}

	public void RemoveJob(IActiveJob job)
	{
		_activeJobs.Remove(job);
		Changed = true;
	}

	private readonly List<IActiveProject> _personalProjects = new();
	public IEnumerable<IPersonalProject> PersonalProjects => _personalProjects.OfType<IPersonalProject>();

	private (IActiveProject Project, IProjectLabourRequirement Labour) _currentProject;

	public (IActiveProject Project, IProjectLabourRequirement Labour) CurrentProject
	{
		get => _currentProject;
		set
		{
			_currentProject = value;
			_currentProjectHours = 0.0;
			Changed = true;
		}
	}

	private double _currentProjectHours;

	public double CurrentProjectHours
	{
		get => _currentProjectHours;
		set
		{
			_currentProjectHours = value;
			Changed = true;
		}
	}

	public void AddPersonalProject(IActiveProject project)
	{
		_personalProjects.Add(project);
	}

	public void RemovePersonalProject(IActiveProject project)
	{
		_personalProjects.Remove(project);
		if (CurrentProject.Project == project)
		{
			CurrentProject = (null, null);
			CurrentProjectHours = 0.0;
		}
	}
}