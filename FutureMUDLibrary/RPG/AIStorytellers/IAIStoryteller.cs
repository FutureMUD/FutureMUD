using System;
using System.Collections.Generic;
using System.Text;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;

namespace MudSharp.RPG.AIStorytellers;

public interface IAIStoryteller : IFrameworkItem, ISaveable, IEditableItem
{
	/// <summary>
	/// A description of the purpose and function of this AI Storyteller
	/// </summary>
	string Description { get; }

	void SubscribeEvents();
	void UnsubscribeEvents();
	void Delete();
	void Pause();
	void Unpause();
}
