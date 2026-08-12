using MudSharp.Character;
using MudSharp.Work.Projects;

namespace MudSharp.RPG.Merits.Interfaces;

/// <summary>
/// Multiplies only the progress a character contributes during a funded project labour tick.
/// </summary>
public interface IProjectLabourContributionMerit : ICharacterMerit
{
	double ProjectLabourContributionMultiplier(ICharacter character, IActiveProject project);
}
