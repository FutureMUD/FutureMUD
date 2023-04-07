using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;

namespace MudSharp.Construction.Autobuilder.Rooms
{
    public interface IAutobuilderRandomDescriptionElement : IXmlSavable
    {
        double Weight { get; }
        bool Applies(ITerrain terrain, IEnumerable<string> tags);
        (string RoomName, string DescriptionText) TextForTags(ITerrain terrain, IEnumerable<string> tags);
        IEnumerable<string> Tags { get; }
        bool MandatoryIfValid { get; }
        int MandatoryPosition { get; }

        /// <summary>
        /// Executes a building command based on player input
        /// </summary>
        /// <param name="actor">The avatar of the player doing the command</param>
        /// <param name="command">The command they wish to execute</param>
        /// <returns>Returns true if the command was valid and anything was changed. If nothing was changed or the command was invalid, it returns false</returns>
        bool BuildingCommand(ICharacter actor, StringStack command);

        /// <summary>
        /// Shows a builder-specific output representing the IEditableItem
        /// </summary>
        /// <param name="actor">The avatar of the player who wants to view the IEditableItem</param>
        /// <returns>A string representing the item textually</returns>
        string Show(ICharacter actor);

        IAutobuilderRandomDescriptionElement Clone();
    }
}
