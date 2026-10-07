using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using System.Collections.Generic;

namespace MudSharp.Construction.Autobuilder
{
    public interface IAutobuilderRoom : ISaveable, IEditableItem
    {
        IRoom CreateRoom(ICharacter builder, ITerrain specifiedTerrain, bool deferDescription, params string[] tags);
        IRoom CreateRoom(ICharacter builder, ITerrain specifiedTerrain, bool deferDescription,
            IReadOnlyCollection<ITag> frameworkTags, params string[] tags) =>
            CreateRoom(builder, specifiedTerrain, deferDescription, tags);
        IAutobuilderRoom Clone(string newName);
        void RedescribeRoom(IRoom room, params string[] tags);
        string ShowCommandByline { get; }
    }
}
