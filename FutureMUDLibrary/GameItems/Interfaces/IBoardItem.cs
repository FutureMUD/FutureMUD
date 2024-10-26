using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Community.Boards;
using MudSharp.FutureProg;

namespace MudSharp.GameItems.Interfaces
{
	public interface IBoardItem : IGameItemComponent
	{
		IBoard Board { get; }
		IFutureProg CanViewBoard { get;}
		IFutureProg CanPostToBoard { get; }
		string CantViewBoardEcho { get; }
		string CantPostToBoardEcho { get; }
		bool ShowAuthorName { get; }
		bool ShowAuthorShortDescription { get; }
		bool ShowAuthorDescription { get; }
	}
}
