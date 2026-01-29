using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Celestial;
using MudSharp.Character;
using MudSharp.Climate;
using MudSharp.Construction.Boundary;
using MudSharp.Events;
using MudSharp.Form.Audio;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Checks;
using MudSharp.TimeAndDate;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Time;

namespace MudSharp.Construction;

public abstract class Location : PerceivedItem, ILocation
{
	protected Location(IFuturemud gameworld)
	{
		Gameworld = gameworld;
	}

	#region Overrides of LateKeywordedInitialisingItem

	public override bool IdHasBeenRegistered => true;

	#endregion

	public override void Register(IOutputHandler handler)
	{
		// TODO?
	}

	public abstract CelestialInformation GetInfo(ICelestialObject celestial);

	public virtual MudTime Time(IClock whichClock)
	{
		return null;
	}

	public virtual MudDate Date(ICalendar whichCalendar)
	{
		return null;
	}

	public MudDateTime DateTime(ICalendar whichCalendar = null)
	{
		if (whichCalendar == null)
		{
			whichCalendar = Calendars.First();
		}

		return new MudDateTime(Date(whichCalendar), Time(whichCalendar.FeedClock), TimeZone(whichCalendar.FeedClock));
	}

	public virtual Difficulty LocalAudioDifficulty(IPerceiver perceiver, AudioVolume volume, Proximity proximity)
	{
		return Difficulty.Automatic;
	}

	public override object DatabaseInsert()
	{
		// Not required to do late initialisation
		return this;
	}

	public override void SetIDFromDatabase(object dbitem)
	{
		// Do nothing
	}

	#region ILocation Members

	protected readonly List<ICharacter> _characters = new();
	protected List<IGameItem> _gameItems = new();

	public IEnumerable<ICharacter> Characters => _characters;

	public IEnumerable<ICharacter> LayerCharacters(RoomLayer layer)
	{
		return _characters.Where(x => x.RoomLayer == layer);
	}

	public IEnumerable<IGameItem> GameItems => _gameItems;

	public IEnumerable<IGameItem> LayerGameItems(RoomLayer layer)
	{
		return _gameItems.Where(x => x.RoomLayer == layer);
	}

	public IEnumerable<IPerceivable> Perceivables => Characters.Cast<IPerceivable>().Concat(GameItems);

	public abstract IEnumerable<ICell> Cells { get; }

	public IEnumerable<IHandleEvents> EventHandlers
		=> Characters.Cast<IHandleEvents>().Concat(GameItems).Concat(new IHandleEvents[] { this });

	public virtual void Insert(IGameItem thing, bool newStack)
	{
		if (thing != null && !_gameItems.Contains(thing))
		{
			_gameItems.Add(thing);
		}
	}

	public virtual void Extract(IGameItem thing)
	{
		if (thing != null)
		{
			_gameItems.Remove(thing);
		}
	}

	public virtual void Enter(ICharacter movingCharacter, ICellExit exit = null, bool noSave = false,
		RoomLayer roomLayer = RoomLayer.GroundLevel)
	{
		if (_characters.Contains(movingCharacter))
		{
			return;
		}

		_characters.Add(movingCharacter);
	}

	protected void DoEnterEvent(ICharacter character)
	{
		OnCharacterEnters?.Invoke(character, this);
	}

	public virtual void Leave(ICharacter movingCharacter)
	{
		_characters.Remove(movingCharacter);
	}

	protected void DoLeaveEvent(ICharacter character)
	{
		OnCharacterLeaves?.Invoke(character, this);
	}

	public virtual IEnumerable<IClock> Clocks => throw new NotImplementedException();

	public virtual IEnumerable<ICalendar> Calendars => throw new NotImplementedException();

	public virtual IEnumerable<ICelestialObject> Celestials => throw new NotImplementedException();

	public abstract IMudTimeZone TimeZone(IClock whichClock);

	public virtual IWeatherController WeatherController
	{
		get => null;
		set
		{
			// NOOP
		}
	}

	public event CharacterMovementEvent OnCharacterEnters;
	public event CharacterMovementEvent OnCharacterLeaves;

	public virtual void HandleRoomEcho(string echo, RoomLayer? layer = null)
	{
		// NOOP
	}

	#endregion
}