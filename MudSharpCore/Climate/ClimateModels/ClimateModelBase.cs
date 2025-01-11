using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Celestial;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Save;

namespace MudSharp.Climate.ClimateModels;

public abstract class ClimateModelBase : SaveableItem, IClimateModel
{
	#region Overrides of Item

	public sealed override string FrameworkItemType => "ClimateModel";

	#endregion

	#region Implementation of IClimateModel

	public abstract IWeatherEvent HandleWeatherTick(IWeatherEvent currentWeather, ISeason currentSeason,
		TimeOfDay currentTime, int consecutiveUnchangedPeriods);

	/// <summary>
	/// The number of in-character minutes between checking for weather changes
	/// </summary>
	public int MinuteProcessingInterval { get; protected set; }

	/// <summary>
	/// The minimum number of in-character minutes between flavour echoes for current weather being sent
	/// </summary>
	public int MinimumMinutesBetweenFlavourEchoes { get; protected set; }

	/// <summary>
	/// The chance (0.0-1.0) of a flavour echo being issued every in-character minute
	/// </summary>
	public double MinuteFlavourEchoChance { get; protected set; }

	public abstract IEnumerable<IWeatherEvent> PermittedTransitions(IWeatherEvent currentEvent, ISeason currentSeason,
		TimeOfDay timeOfDay);

	public abstract IClimateModel Clone(string name);

	public abstract IEnumerable<ISeason> Seasons { get; }
	public abstract IEnumerable<IWeatherEvent> WeatherEvents { get; }

	#endregion

	#region Implementation of IEditableItem

	/// <inheritdoc />
	public abstract bool BuildingCommand(ICharacter actor, StringStack command);

	/// <inheritdoc />
	public abstract string Show(ICharacter actor);

	#endregion
}