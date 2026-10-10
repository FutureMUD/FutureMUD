using MudSharp.Framework;
using System.Collections.Generic;
using System;

namespace MudSharp.Construction.Boundary
{
    public interface IExitManager
    {
        IPathfindingService PathfindingService { get; }
		ISpatialPathfinder SpatialPathfinder => throw new NotSupportedException(
			"This exit manager does not provide hybrid spatial pathfinding.");

        /// <summary>
        ///     Retrieves the correct exit for the specified room
        /// </summary>
        /// <param name="room">The room for which this exit is being retrieved</param>
        /// <param name="direction">A CardinalDirection to retrieve the exit for.</param>
        /// <param name="overlay">An optional parameter specifying the overlay to use. If not specified, uses the current overlay</param>
        /// <returns>The appropriate IRoomExit if found, or null if not</returns>
        IRoomExit GetExit(IRoom room, CardinalDirection direction, IPerceiver voyeur);

        /// <summary>
        ///     Retrieves the correct exit for the specified room
        /// </summary>
        /// <param name="room">The room for which this exit is being retrieved</param>
        /// <param name="verb">The verb used to initiate the movement, e.g. "north", or "enter"</param>
        /// <param name="target">The target of the verb, e.g. "shop" in "enter shop"</param>
        /// <param name="voyeur">The person for whom the room exit is being retrieved</param>
        /// <param name="overlay">An optional parameter specifying the overlay to use. If not specified, uses the current overlay</param>
        /// <returns>The appropriate IRoomExit if found, or null if not</returns>
        IRoomExit GetExit(IRoom room, string verb, string target, IPerceiver voyeur, IRoomOverlay overlay = null);

        /// <summary>
        ///     Retrieves the correct exit for the specified room by target exit keyword
        /// </summary>
        /// <param name="room">The room for which this exit is being retrieved</param>
        /// <param name="verb">The verb used to target the exit, e.g. "north" or "tavern"</param>
        /// <param name="voyeur">The person for whom the room exit is being retrieved</param>
        /// <param name="overlay">An optional parameter specifying the overlay to use. If not specified, uses the current overlay</param>
        /// <returns>The appropriate IRoomExit if found, or null if not</returns>
        IRoomExit GetExitKeyword(IRoom room, string keyword, IPerceiver voyeur, IRoomOverlay overlay = null);

        /// <summary>
        ///     Retrieves all exits for the specified room and overlay combination
        /// </summary>
        /// <param name="room">The room for which to request exit information</param>
        /// <param name="overlay">An optional parameter specifying the overlay to use. If not specified, uses the current overlay</param>
        /// <returns>An IEnumerable of all the IRoomExit for this room and overlay</returns>
        IEnumerable<IRoomExit> GetExitsFor(IRoom room, IRoomOverlay overlay = null, RoomLayer? layer = null);

        /// <summary>
        /// Retrieves all exits for the specified room and overlay combination
        /// </summary>
        /// <param name="room">The room for which to request the exit information</param>
        /// <param name="package">The overlay package for which you want to get exits</param>
        /// <returns>An IEnumerable of all the IRoomExits for this room and overlay package</returns>
        IEnumerable<IRoomExit> GetExitsFor(IRoom room, IRoomOverlayPackage package, RoomLayer? layer = null);

        /// <summary>
        ///     Returns all possible IRoomExits for the specified IRoom. This requires that all overlays for the room will be
        ///     initialised.
        /// </summary>
        /// <param name="room">The IRoom for which to return all IRoomExits</param>
        /// <returns>An IEnumerable containing all IRoomExits for all IRoomOverlays for this IRoom</returns>
        IEnumerable<IRoomExit> GetAllExits(IRoom room);

        IExit GetExitByID(long id);
        IEnumerable<IExit> TransientExits { get; }
		event Action<ITransientExit> TransientExitRegistered;
		event Action<ITransientExit, ITransientExit> TransientExitReplaced;
		event Action<ITransientExit> TransientExitUnregistered;
        void RegisterTransientExit(IExit exit);
		bool ReplaceTransientExit(IExit existingExit, IExit replacementExit);
        void UnregisterTransientExit(IExit exit);

        /// <summary>
        ///     This function is called by the RoomOverlay when it changes which exits it uses. It ensures that the ExitManager
        ///     updates its list of exits for that overlay.
        /// </summary>
        /// <param name="room"></param>
        /// <param name="overlay"></param>
        void UpdateRoomOverlayExits(IRoom room, IRoomOverlay overlay);

        void PreloadCriticalExits();
        void DeleteRoom(IRoom room);
		/// <summary>Release only explicitly committed/deleted exits and the removed room's cached topology.</summary>
		void ForgetCommittedTopology(long roomId, IReadOnlyCollection<long> exitIds) =>
			throw new NotSupportedException("This exit manager has no exact committed-topology release adapter.");
        void InitialiseRoom(IRoom room, IRoomOverlay overlay);
    }
}
