using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable
namespace MudSharp.RPG.Hints
{
	public interface INewPlayerHint : ISaveable, IEditableItem
	{
		string Text { get; }
		IFutureProg? FilterProg { get; }
		int Priority { get; }
		bool CanRepeat { get; }
	}
}
