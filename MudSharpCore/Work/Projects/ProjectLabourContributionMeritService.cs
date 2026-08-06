using MudSharp.RPG.Merits.Interfaces;

namespace MudSharp.Work.Projects;

public static class ProjectLabourContributionMeritService
{
	public static double MultiplierFor(ICharacter character, IActiveProject project)
	{
		var multiplier = 1.0;
		foreach (var merit in character.Merits.OfType<IProjectLabourContributionMerit>()
		             .Where(x => x.Applies(character)))
		{
			try
			{
				var result = merit.ProjectLabourContributionMultiplier(character, project);
				if (double.IsNaN(result) || double.IsInfinity(result))
				{
					character.Gameworld.SystemMessage(
						$"Project labour contribution merit {merit.Name} returned an invalid multiplier for {character.Name}; using 1.0.",
						true);
					continue;
				}

				multiplier *= Math.Max(0.0, result);
			}
			catch (Exception ex)
			{
				character.Gameworld.SystemMessage(
					$"Project labour contribution merit {merit.Name} failed for {character.Name}: {ex.Message}; using 1.0.",
					true);
			}
		}

		return multiplier;
	}
}
