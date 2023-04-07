using System;

namespace MudSharp.Framework.Save {
    public delegate void LateInitialisingItemDelegate(ILateInitialisingItem item);

    public enum InitialisationPhase {
        First,
        Second,
        AfterFirstDatabaseHit
    }

    public interface ILateInitialisingItem : ISaveable, IFrameworkItem {
        bool IdHasBeenRegistered { get; }
        InitialisationPhase InitialisationPhase { get; }
        event LateInitialisingItemDelegate IdRegistered;
        object DatabaseInsert();
        void SetIDFromDatabase(object dbitem);
        Action InitialiseItem();
        void SetNoSave(bool value);
        bool GetNoSave();
    }
}