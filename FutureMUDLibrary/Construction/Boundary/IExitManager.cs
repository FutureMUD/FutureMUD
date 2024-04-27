using System.Collections.Generic;
using MudSharp.Framework;

namespace MudSharp.Construction.Boundary {
    public interface IExitManager {
        /// <summary>
        ///     Retrieves the correct exit for the specified cell
        /// </summary>
        /// <param name="cell">The cell for which this exit is being retrieved</param>
        /// <param name="direction">A CardinalDirection to retrieve the exit for.</param>
        /// <param name="overlay">An optional parameter specifying the overlay to use. If not specified, uses the current overlay</param>
        /// <returns>The appropriate ICellExit if found, or null if not</returns>
        ICellExit GetExit(ICell cell, CardinalDirection direction, IPerceiver voyeur);

        /// <summary>
        ///     Retrieves the correct exit for the specified cell
        /// </summary>
        /// <param name="cell">The cell for which this exit is being retrieved</param>
        /// <param name="verb">The verb used to initiate the movement, e.g. "north", or "enter"</param>
        /// <param name="target">The target of the verb, e.g. "shop" in "enter shop"</param>
        /// <param name="voyeur">The person for whom the cell exit is being retrieved</param>
        /// <param name="overlay">An optional parameter specifying the overlay to use. If not specified, uses the current overlay</param>
        /// <returns>The appropriate ICellExit if found, or null if not</returns>
        ICellExit GetExit(ICell cell, string verb, string target, IPerceiver voyeur, ICellOverlay overlay = null);

        /// <summary>
        ///     Retrieves the correct exit for the specified cell by target exit keyword
        /// </summary>
        /// <param name="cell">The cell for which this exit is being retrieved</param>
        /// <param name="verb">The verb used to target the exit, e.g. "north" or "tavern"</param>
        /// <param name="voyeur">The person for whom the cell exit is being retrieved</param>
        /// <param name="overlay">An optional parameter specifying the overlay to use. If not specified, uses the current overlay</param>
        /// <returns>The appropriate ICellExit if found, or null if not</returns>
        ICellExit GetExitKeyword(ICell cell, string keyword, IPerceiver voyeur, ICellOverlay overlay = null);

        /// <summary>
        ///     Retrieves all exits for the specified cell and overlay combination
        /// </summary>
        /// <param name="cell">The cell for which to request exit information</param>
        /// <param name="overlay">An optional parameter specifying the overlay to use. If not specified, uses the current overlay</param>
        /// <returns>An IEnumerable of all the ICellExit for this cell and overlay</returns>
        IEnumerable<ICellExit> GetExitsFor(ICell cell, ICellOverlay overlay = null, RoomLayer? layer = null);

        /// <summary>
        /// Retrieves all exits for the specified cell and overlay combination
        /// </summary>
        /// <param name="cell">The cell for which to request the exit information</param>
        /// <param name="package">The overlay package for which you want to get exits</param>
        /// <returns>An IEnumerable of all the ICellExits for this cell and overlay package</returns>
        IEnumerable<ICellExit> GetExitsFor(ICell cell, ICellOverlayPackage package, RoomLayer? layer = null);

        /// <summary>
        ///     Returns all possible ICellExits for the specified ICell. This requires that all overlays for the cell will be
        ///     initialised.
        /// </summary>
        /// <param name="cell">The ICell for which to return all ICellExits</param>
        /// <returns>An IEnumerable containing all ICellExits for all ICellOverlays for this ICell</returns>
        IEnumerable<ICellExit> GetAllExits(ICell cell);

        IExit GetExitByID(long id);

        /// <summary>
        ///     This function is called by the CellOverlay when it changes which exits it uses. It ensures that the ExitManager
        ///     updates its list of exits for that overlay.
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="overlay"></param>
        void UpdateCellOverlayExits(ICell cell, ICellOverlay overlay);

        void PreloadCriticalExits();
        void DeleteCell(ICell cell);
        void InitialiseCell(ICell cell, ICellOverlay overlay);
    }
}