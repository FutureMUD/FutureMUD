using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using System;
using System.Collections.Generic;
using System.Text;

namespace MudSharp.RPG.AIStorytellers;

public interface IAIStorytellerReferenceDocument : IFrameworkItem, ISaveable, IEditableItem
{
    public string FolderName { get; }
    public string Keywords { get; }
    public string DocumentContents { get; }
    public bool ReturnForSearch(string searchterm);
    public bool IsVisibleTo(IAIStoryteller storyteller);
}
