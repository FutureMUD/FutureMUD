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
using MudSharp.Magic.Environment;
using MudSharp.Movement;
using MudSharp.RPG.Checks;
using MudSharp.Work.Agriculture;
using MudSharp.Work.Foraging;
using MudSharp.Work.Projects;
using System;
using System.Collections.Generic;

#nullable enable annotations

namespace MudSharp.Construction
{

    public delegate void RoomProposedForDeletionDelegate(IRoom room, ProposalRejectionResponse response);

    public interface IRoom : ILocation, IProgVariable, IHaveMagicResource, IHaveTags, IRoomLiquidSurface
    {
		/// <summary>Optional global builder identifier, independent of the numeric ID and overlay name.</summary>
		string? UniqueName { get; }
		bool TrySetUniqueName(string? value, out string error);

		/// <summary>
		/// The spatial model used by this cell. Existing cell implementations remain ordinary
		/// unless they expose a route definition.
		/// </summary>
		RoomSpatialType SpatialType => RouteDefinition is null
			? RoomSpatialType.Ordinary
			: RoomSpatialType.LinearRoute;

		/// <summary>
		/// The optional one-dimensional spatial definition for a linear route cell.
		/// </summary>
		IRouteRoomDefinition? RouteDefinition => null;

#nullable restore annotations

        /// <summary>
        /// If a cell is temporary, it may disappear at any time.
        /// </summary>
        bool Temporary { get; }
        IEnumerable<IRoom> Surrounds { get; }
        /// <summary>Stored ownership, independent of hosted exterior projection.</summary>
        IZone OwningZone { get; }
        IEnumerable<IArea> OwningAreas { get; }
        (int X, int Y, int Z) StoredCoordinates { get; }
        int? X { get; }
        int? Y { get; }
        int? Z { get; }
        void SetCoordinates(int x, int y, int z);
        void SetNewZone(IZone zone);
        void AddArea(IArea area);
        void RemoveArea(IArea area);
        IZone Zone { get; }
        IShard Shard { get; }
        IEnumerable<IArea> Areas { get; }

        IRoomOverlay CurrentOverlay { get; }
        IEnumerable<IRoomOverlay> Overlays { get; }

        IForagableProfile ForagableProfile { get; set; }
        /// <summary>Whether an effective forage profile exists, without synchronising its yield pools.</summary>
        bool HasForagableProfile { get; }
        IAgricultureField AgricultureField { get; }

        IFluid Atmosphere { get; }

        IEnumerable<IRangedCover> LocalCover { get; }

        bool SafeQuit => CurrentOverlay.SafeQuit;

        RoomOutdoorsType OutdoorsType(IPerceiver voyeur);

        void Login(ICharacter loginCharacter);

        IEnumerable<IRoomExit> ExitsFor(IPerceiver voyeur, bool ignoreLayers = false);
        ITerrain Terrain(IPerceiver voyeur);
        string ExitStrings(IPerceiver voyeur, IRoomOverlay overlay, bool colour = true);
        string GetFriendlyReference(IPerceiver voyeur);
        IRoomExit GetExit(CardinalDirection direction, IPerceiver voyeur);
        IRoomExit GetExit(string direction, string target, IPerceiver voyeur);
        IRoomExit GetExitKeyword(string direction, IPerceiver voyeur);
        IRoomExit GetExitTo(IRoom otherRoom, IPerceiver voyeur, bool ignoreLayers = false);
        void ResolveMovement(IMovement move);
        void RegisterMovement(IMovement move);
        string ProcessedFullDescription(IPerceiver voyeur, PerceiveIgnoreFlags flags, IRoomOverlay overlay);
        IHearingProfile HearingProfile(IPerceiver voyeur);
        IEditableRoomOverlay GetOrCreateOverlay(IRoomOverlayPackage package);
        IRoomOverlay GetOverlay(IRoomOverlayPackage package);
        bool SetCurrentOverlay(IRoomOverlayPackage package);
        void AddOverlay(IEditableRoomOverlay overlay);
        void RemoveOverlay(long id);

        TimeOfDay CurrentTimeOfDay { get; }
        double CurrentIllumination(IPerceiver voyeur);
        Difficulty SpotDifficulty(IPerceiver spotter);
        [CanBeNull] IWeatherEvent CurrentWeather(IPerceiver voyeur);
        [CanBeNull] new IWeatherController WeatherController { get; }
        double CurrentTemperature(IPerceiver voyeur);
        ISeason CurrentSeason(IPerceiver voyeur);

        bool IsExitVisible(IPerceiver voyeur, IRoomExit exit, PerceptionTypes type,
            PerceiveIgnoreFlags flags = PerceiveIgnoreFlags.None);
        int LoadItems(IEnumerable<Models.GameItem> items);
        double GetForagableYield(string foragableType);
        /// <summary>
        /// Projects the current effective profile's yield without saving, recovering yield, or changing subscriptions.
        /// Returns false when the profile or configured yield key is unavailable. New keys project their maximum;
        /// existing depleted keys retain their balance, clamped to the current maximum.
        /// </summary>
        bool TryPeekForagableYield(string foragableType, out double yield);
		/// <summary>Purely observes stock and the effective forage-profile lifecycle used by a planned debit.</summary>
		bool TryPeekForagableYield(string foragableType, out NativeForageYieldSnapshot snapshot);
        /// <summary>Synchronises forage pools at an owning configuration or mutation boundary.</summary>
        void SynchroniseForagableProfile();
        bool CanConsumeYield(string foragableType, double yield);
        bool TryConsumeYield(string foragableType, double yield);
		/// <summary>Exactly compare-and-applies a previously observed native forage debit.</summary>
		bool TryConsumeYield(NativeForageYieldSnapshot expected, double yield, out string reason);
		/// <summary>Validates and consumes distinct forage keys atomically under their one cell owner.</summary>
		bool TryConsumeYieldBatch(IReadOnlyList<NativeForageDebitRequest> requests, out string reason);
        void ConsumeYieldFor(IForagable foragable);
        void ConsumeYield(string foragableType, double yield);
        IEnumerable<string> ForagableTypes { get; }
        IEnumerable<IRangedCover> GetCoverFor(IPerceiver voyeur);

        bool CanGet(IGameItem item, ICharacter getter);
        string WhyCannotGet(IGameItem item, ICharacter getter);

        bool CanGetAccess(IGameItem item, ICharacter getter);
        string WhyCannotGetAccess(IGameItem item, ICharacter getter);


        void PostLoadTasks(MudSharp.Models.Room room);
        void Destroy(IRoom fallbackRoom);
        Action DestroyWithDatabaseAction(IRoom fallbackRoom);

        void AreaAdded(IArea area);
        void AreaRemoved(IArea area);

        IPermanentShop Shop { get; set; }

        double EstimatedDirectDistanceTo(IRoom otherRoom);

        IEnumerable<ILocalProject> LocalProjects { get; }
        void AddProject(ILocalProject project);
        void RemoveProject(ILocalProject project);

        void OnExitsInitialised();
        IRoomOverlay GetOverlayFor(IPerceiver voyeur);

        /// <summary>
        /// Determines whether this cell acts like a water cell (i.e. swimming required), optionally specifying a layer at which the swim should be checked
        /// </summary>
        /// <param name="referenceLayer">A layer to check if it counts as a swim layer</param>
        /// <returns>True if the specified layer (and by implication all lower layers) is a swim layer</returns>
        bool IsSwimmingLayer(RoomLayer referenceLayer = RoomLayer.GroundLevel);

        bool IsUnderwaterLayer(RoomLayer referenceLayer);
        (bool Truth, IEnumerable<string> Errors) ProposeDelete();
        event RoomProposedForDeletionDelegate RoomProposedForDeletion;
        event EventHandler RoomRequestsDeletion;
        event RoomEchoEvent OnRoomEcho;
        event RoomEmoteEchoEvent OnRoomEmoteEcho;
        void CheckFallExitStatus();
        IEnumerable<ITrack> Tracks { get; }
        void AddTrack(ITrack track);
        void RemoveTrack(ITrack track);
        void InitialiseTracks(IReadOnlyCollectionDictionary<IRoom, ITrack> tracks);

        /// <summary>
        /// Use this to send an AudioOutput to all perceivers in the same cell, adjacent layers, and surrounding cells with an audio volume drop off
        /// </summary>
        /// <param name="audioText">The text to echo. Use {0} for the direction ("from above", "far to the east", etc) and {1} for the volume ("very loud", "quiet", etc)</param>
        /// <param name="volume">The volume to echo. Drops off one per room</param>
        /// <param name="source">The source for the emote</param>
        /// <param name="originalLayer">The original layer that the sound emanates from</param>
        /// <param name="ignoreOriginLayer">If true, doesn't echo the original layer/room combo. Otherwise includes this location as well</param>
        void HandleAudioEcho(string audioText, AudioVolume volume, IPerceiver source, RoomLayer originalLayer,
            bool ignoreOriginLayer = true);

        /// <summary>
        /// Sends an audio echo and exposes its builder-facing category through the NoiseEmitted event.
        /// </summary>
        void HandleAudioEcho(string audioText, AudioVolume volume, IPerceiver source, RoomLayer originalLayer,
            bool ignoreOriginLayer, string noiseType)
        {
            HandleAudioEcho(audioText, volume, source, originalLayer, ignoreOriginLayer);
        }

        /// <summary>
        /// Emits structured received noise using a finite propagation budget independent of AudioVolume.
        /// </summary>
        void HandleAudioEcho(string audioText, AudioVolume volume, double propagationBudget,
            AudioPropagationMode propagationMode, IPerceiver source, RoomLayer originalLayer,
            bool ignoreOriginLayer, string noiseType);
    }
}
