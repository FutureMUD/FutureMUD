using System.Collections.Generic;
using MudSharp.Celestial;
using MudSharp.Character;
using MudSharp.Construction.Boundary;
using MudSharp.Events;
using MudSharp.Form.Audio;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.GameItems;
using MudSharp.RPG.Checks;
using MudSharp.TimeAndDate;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Time;

namespace MudSharp.Construction {
    

    public interface ILocation : IPerceivable, ISaveable {
        /// <summary>
        ///     Returns an IEnumerable of all IHandleEvent-implementing things present, including itself
        /// </summary>
        IEnumerable<IHandleEvents> EventHandlers { get; }

        IEnumerable<ICharacter> Characters { get; }

        IEnumerable<ICharacter> LayerCharacters(RoomLayer layer);

        IEnumerable<IGameItem> GameItems { get; }

        IEnumerable<IGameItem> LayerGameItems(RoomLayer layer);

        IEnumerable<IPerceivable> Perceivables { get; }
        IEnumerable<ICell> Cells { get; }

        IEnumerable<IClock> Clocks { get; }
        IEnumerable<ICalendar> Calendars { get; }
        IEnumerable<ICelestialObject> Celestials { get; }

        void Insert(IGameItem thing, bool newStack = false);
        void Extract(IGameItem thing);

        void Leave(ICharacter movingCharacter);
        void Enter(ICharacter movingCharacter, ICellExit exit = null, bool noSave = false, RoomLayer roomLayer = RoomLayer.GroundLevel);

        CelestialInformation GetInfo(ICelestialObject celestial);
        MudTime Time(IClock whichClock);
        MudDate Date(ICalendar whichCalendar);
        IMudTimeZone TimeZone(IClock whichClock);
        MudDateTime DateTime(ICalendar whichCalendar = null);

        Difficulty LocalAudioDifficulty(IPerceiver perceiver, AudioVolume volume, Proximity proximity);
        event CharacterMovementEvent OnCharacterEnters;
        event CharacterMovementEvent OnCharacterLeaves;
    }

    public delegate void CharacterMovementEvent(ICharacter character, ILocation location);
}