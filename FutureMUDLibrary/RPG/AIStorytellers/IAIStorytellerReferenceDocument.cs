using System;
using System.Collections.Generic;
using System.Text;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;

namespace MudSharp.RPG.AIStorytellers;

public interface IAIStorytellerReferenceDocument : IFrameworkItem, ISaveable, IEditableItem
{
	public string FolderName { get; }
	public string Keywords { get; }
	public string DocumentContents { get; }
	public bool ReturnForSearch(string searchterm);
}
