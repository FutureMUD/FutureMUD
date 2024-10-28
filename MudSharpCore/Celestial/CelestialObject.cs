using System;
using MudSharp.Construction;
using MudSharp.Effects;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;
using MudSharp.TimeAndDate.Time;

namespace MudSharp.Celestial;

public abstract class CelestialObject : PerceivedItem, ICelestialObject
{
	public override string FrameworkItemType => "Celestial";

	public static void RegisterPerceivableType(IFuturemud gameworld)
	{
		gameworld.RegisterPerceivableType("Celestial", id => gameworld.CelestialObjects.Get(id));
	}

	protected readonly CircularRange<string> AzimuthDescriptions = new();

	protected readonly CircularRange<string> ElevationDescriptions = new();

	public abstract double CurrentElevationAngle(GeographicCoordinate geography);
	public abstract double CurrentAzimuthAngle(GeographicCoordinate geography, double elevationAngle);
	public abstract double CurrentIllumination(GeographicCoordinate geography);
	public abstract CelestialInformation CurrentPosition(GeographicCoordinate geography);

	public abstract CelestialInformation ReturnNewCelestialInformation(ILocation location,
		CelestialInformation celestialStatus, GeographicCoordinate coordinate);

	public virtual string Describe(CelestialInformation info)
	{
		return
			$"{Name} {string.Format(ElevationDescriptions.Get(info.LastAscensionAngle), AzimuthDescriptions.Get(info.LastAzimuthAngle))}";
	}

	public override void Register(IOutputHandler handler)
	{
		// ???
	}

	#region ICelestialObject Members

	public PerceptionTypes PerceivableTypes => PerceptionTypes.AllVisual;

	public abstract bool CelestialAngleIsUsedToDetermineTimeOfDay { get; }

	public TimeOfDay CurrentTimeOfDay(GeographicCoordinate geography)
	{
		var position = CurrentPosition(geography);
		if (position == null)
		{
			return TimeOfDay.Night;
		}

		if (position.LastAscensionAngle > 0.0)
		{
			if (position.Direction == CelestialMoveDirection.Ascending)
			{
				return TimeOfDay.Morning;
			}

			return TimeOfDay.Afternoon;
		}

		// 12 degrees below horizon is a good general measure of the beginning of dawn or end of dusk
		if (position.LastAscensionAngle < 0.20944)
		{
			return TimeOfDay.Night;
		}

		if (position.Direction == CelestialMoveDirection.Ascending)
		{
			return TimeOfDay.Dawn;
		}

		return TimeOfDay.Dusk;
	}

	#endregion

	#region IFutureProgVariable Implementation

	public override ProgVariableTypes Type => ProgVariableTypes.Error;

	#endregion

	#region IObserve Members

	public bool CanSee(ILocateable target, PerceiveIgnoreFlags flags = PerceiveIgnoreFlags.None)
	{
		return false;
	}

	#endregion

	#region Update Events

	public event CelestialUpdateHandler MinuteUpdateEvent;

	/// <summary>
	///     This function negotiates with each zone subscribed to this sun and checks to see if any thresholds have been met to
	///     display sun-based triggers, i.e. dawn, dusk, twilight, etc.
	/// </summary>
	protected void UpdateZones()
	{
		MinuteUpdateEvent?.Invoke(this);
	}

	#endregion

	#region Current State Properties

	protected int _currentMinutesInCelestialYear;

	public int CurrentMinutesInCelestialYear
	{
		get => _currentMinutesInCelestialYear;
		protected set
		{
			_currentMinutesInCelestialYear = value;
			Changed = true;
		}
	}

	/// <summary>
	///     Not equal to calendar year. Celestial year is to keep track of great years.
	/// </summary>
	protected int _currentCelestialYear;

	public int CurrentCelestialYear
	{
		get => _currentCelestialYear;
		protected set
		{
			_currentCelestialYear = value;
			Changed = true;
		}
	}

	public abstract double CurrentCelestialDay { get; }

	public abstract double CelestialDaysPerYear { get; }

	#endregion

	#region Static Calculated Properties

	protected int _minutesPerYear;

	public int MinutesPerYear
	{
		get => _minutesPerYear;
		protected set => _minutesPerYear = value;
	}

	/// <summary>
	///     Used to make sure the time doesn't wander due to fractional minutes.
	/// </summary>
	protected double _minutesPerYearFraction;

	public double MinutesPerYearFraction
	{
		get => _minutesPerYearFraction;
		protected set => _minutesPerYearFraction = value;
	}

	protected double _yearsBetweenFractionBumps;

	public double YearsBetweenFractionBumps
	{
		get => _yearsBetweenFractionBumps;
		protected set => _yearsBetweenFractionBumps = value;
	}

	protected int _yearOfLastFractionBump;

	public int YearOfLastFractionBump
	{
		get => _yearOfLastFractionBump;
		protected set => _yearOfLastFractionBump = value;
	}

	#endregion

	#region Interface With Time

	protected IClock _clock;

	public IClock Clock
	{
		get => _clock;
		protected set
		{
			if (_clock != null)
			{
				_clock.MinutesUpdated -= _clock_MinutesUpdated;
			}

			_clock = value;

			if (_clock != null)
			{
				_clock.MinutesUpdated += _clock_MinutesUpdated;
			}
		}
	}

	private void _clock_MinutesUpdated()
	{
		AddMinutes();
	}

	public virtual void AddMinutes()
	{
		if (++CurrentMinutesInCelestialYear == MinutesPerYear)
		{
			CurrentMinutesInCelestialYear = 1;
			if ((++CurrentCelestialYear - YearOfLastFractionBump) / YearsBetweenFractionBumps >= 1.0)
			{
				YearOfLastFractionBump = CurrentCelestialYear;
				CurrentMinutesInCelestialYear++;
			}
		}

		UpdateZones();
	}


	/// <summary>
	///     Only to be used when making wholesale calendar dateshifts.
	/// </summary>
	/// <param name="numberOfMinutes"></param>
	public void AddMinutes(int numberOfMinutes)
	{
		/* Some commentary on this inefficient way of doing this:
		 * 
		 * This function is usually going to be called with a parameter of 1, when the clock advances 1 minute. It's easiest to handle that logic.
		 * 
		 * It only really gets a non-1 value when it has to move significantly because of the calendar - which means, for example that someone
		 * has decided to shift the calendar's date by some amount of time. This is a rare occurance that is generally a "head admin" level of
		 * privilege so I would rather optimise it for the former functionality.
		 * 
		 * Eventually this might get tidied up, but it's low priority.
		 * */

		for (var i = 0; i < numberOfMinutes; i++)
		{
			AddMinutes();
		}
	}

	#endregion
}