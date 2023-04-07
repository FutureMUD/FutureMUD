using System;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Checks;

namespace MudSharp.GameItems.Interfaces {
    public enum DoorState {
        Uninstalled = 0,
        Open,
        Closed
    }

    public delegate void DoorEvent(IDoor door);

    public interface IDoor : IOpenable, ILockable {
        IExit InstalledExit { get; set; }
        DoorState State { get; set; }
        bool CanPlayersUninstall { get; }
        bool CanPlayersSmash { get; }
        Difficulty UninstallDifficultyHingeSide { get; }
        Difficulty UninstallDifficultyNotHingeSide { get; }
        Difficulty SmashDifficulty { get; }
        ICell HingeCell { get; set; }
        ICell OpenDirectionCell { get; set; }
        ITraitDefinition UninstallTrait { get; }
        bool CanFireThrough { get; }
        string InstalledExitDescription(IPerceiver perceiver);
        bool CanCross(IBody body);
        bool CanSeeThrough(IBody body);
        void Knock(ICharacter actor, IEmote playerEmote = null);
        event DoorEvent OnRemovedFromExit;
        event DoorEvent OnChangeCanFireThrough;
    }

    public static class DoorStateExtensions {
        public static string Describe(this DoorState state) {
            switch (state) {
                case DoorState.Uninstalled:
                    return "Uninstalled";
                case DoorState.Open:
                    return "Open";
                case DoorState.Closed:
                    return "Closed";
                default:
                    throw new NotSupportedException();
            }
        }
    }
}