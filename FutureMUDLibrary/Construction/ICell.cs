using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using MudSharp.Celestial;
using MudSharp.Character;
using MudSharp.Climate;
using MudSharp.Combat;
using MudSharp.Construction.Boundary;
using MudSharp.Economy;
using MudSharp.Effects;
using MudSharp.Form.Audio;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.Magic;
using MudSharp.Movement;
using MudSharp.RPG.Checks;
using MudSharp.Work.Foraging;
using MudSharp.Work.Projects;

namespace MudSharp.Construction
{

    public delegate void CellProposedForDeletionDelegate(ICell cell, ProposalRejectionResponse response);

    public interface ICell : ILocation, IFutureProgVariable, IHaveMagicResource, IHaveTags
    {
        /// <summary>
        /// If a cell is temporary, it may disappear at any time.
        /// </summary>
        bool Temporary { get; }
        IEnumerable<ICell> Surrounds { get; }
        IRoom Room { get; }
        IZone Zone { get; }
        IShard Shard { get; }
        IEnumerable<IArea> Areas { get; }

        ICellOverlay CurrentOverlay { get; }
        IEnumerable<ICellOverlay> Overlays { get; }

        IForagableProfile ForagableProfile { get; set; }

        IFluid Atmosphere { get; }

        IEnumerable<IRangedCover> LocalCover { get; }

        bool SafeQuit => CurrentOverlay.SafeQuit;

        CellOutdoorsType OutdoorsType(IPerceiver voyeur);

        void Login(ICharacter loginCharacter);

        IEnumerable<ICellExit> ExitsFor(IPerceiver voyeur, bool ignoreLayers = false);
        ITerrain Terrain(IPerceiver voyeur);
        string ExitStrings(IPerceiver voyeur, ICellOverlay overlay, bool colour = true);
        string GetFriendlyReference(IPerceiver voyeur);
        ICellExit GetExit(CardinalDirection direction, IPerceiver voyeur);
        ICellExit GetExit(string direction, string target, IPerceiver voyeur);
        ICellExit GetExitKeyword(string direction, IPerceiver voyeur);
        ICellExit GetExitTo(ICell otherCell, IPerceiver voyeur, bool ignoreLayers = false);
        void ResolveMovement(IMovement move);
        void RegisterMovement(IMovement move);
        IHearingProfile HearingProfile(IPerceiver voyeur);
        IEditableCellOverlay GetOrCreateOverlay(ICellOverlayPackage package);
        ICellOverlay GetOverlay(ICellOverlayPackage package);
        bool SetCurrentOverlay(ICellOverlayPackage package);
        void AddOverlay(IEditableCellOverlay overlay);
        void RemoveOverlay(long id);

        TimeOfDay CurrentTimeOfDay { get; }
        double CurrentIllumination(IPerceiver voyeur);
        Difficulty SpotDifficulty(IPerceiver spotter);
        [CanBeNull]IWeatherEvent CurrentWeather(IPerceiver voyeur);
        [CanBeNull]IWeatherController WeatherController { get; }
        double CurrentTemperature(IPerceiver voyeur);
        ISeason CurrentSeason(IPerceiver voyeur);

        bool IsExitVisible(IPerceiver voyeur, ICellExit exit, PerceptionTypes type, 
            PerceiveIgnoreFlags flags = PerceiveIgnoreFlags.None);
        int LoadItems(IEnumerable<Models.GameItem> items);
        double GetForagableYield(string foragableType);
        void ConsumeYieldFor(IForagable foragable);
        void ConsumeYield(string foragableType, double yield);
        IEnumerable<string> ForagableTypes { get; }
        IEnumerable<IRangedCover> GetCoverFor(IPerceiver voyeur);

        bool CanGet(IGameItem item, ICharacter getter);
        string WhyCannotGet(IGameItem item, ICharacter getter);

        bool CanGetAccess(IGameItem item, ICharacter getter);
        string WhyCannotGetAccess(IGameItem item, ICharacter getter);


        void PostLoadTasks(MudSharp.Models.Cell cell);
        void Destroy(ICell fallbackCell);
        Action DestroyWithDatabaseAction(ICell fallbackCell);

        void AreaAdded(IArea area);
        void AreaRemoved(IArea area);

        IPermanentShop Shop { get; set; }

        double EstimatedDirectDistanceTo(ICell otherCell);

        IEnumerable<ILocalProject> LocalProjects { get; }
        void AddProject(ILocalProject project);
        void RemoveProject(ILocalProject project);

        void OnExitsInitialised();
        ICellOverlay GetOverlayFor(IPerceiver voyeur);

        /// <summary>
        /// Determines whether this cell acts like a water cell (i.e. swimming required), optionally specifying a layer at which the swim should be checked
        /// </summary>
        /// <param name="referenceLayer">A layer to check if it counts as a swim layer</param>
        /// <returns>True if the specified layer (and by implication all lower layers) is a swim layer</returns>
        bool IsSwimmingLayer(RoomLayer referenceLayer = RoomLayer.GroundLevel);

        bool IsUnderwaterLayer(RoomLayer referenceLayer);
        (bool Truth, IEnumerable<string> Errors) ProposeDelete();
        event CellProposedForDeletionDelegate CellProposedForDeletion;
		event EventHandler CellRequestsDeletion;
	}
}