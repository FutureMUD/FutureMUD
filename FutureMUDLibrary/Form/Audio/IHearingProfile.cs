using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.RPG.Checks;

namespace MudSharp.Form.Audio {
    /// <summary>
    ///     An IHearingProfile supplies information about how difficult it is to hear a particular sound at a given location,
    ///     volume and proximity
    /// </summary>
    public interface IHearingProfile : IFrameworkItem {
        /// <summary>
        ///     What this HearingProfile shows up as in the survey command
        /// </summary>
        string SurveyDescription { get; }

        Difficulty AudioDifficulty(ILocation location, AudioVolume volume, Proximity proximity);

        /// <summary>
        ///     Gets the current profile for the specified location (e.g. weekday hearing profile returns down to simple)
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        IHearingProfile CurrentProfile(ILocation location);
    }
}