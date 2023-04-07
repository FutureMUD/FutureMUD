using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;

namespace MudSharp.Construction.Autobuilder
{
    public interface IAutobuilderRoom : ISaveable, IEditableItem
    {
        ICell CreateRoom(ICharacter builder, ITerrain specifiedTerrain, bool deferDescription, params string[] tags);
        IAutobuilderRoom Clone(string newName);
        void RedescribeRoom(ICell cell, params string[] tags);
        string ShowCommandByline { get; }
    }
}