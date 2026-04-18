#nullable enable

using System.Collections.Generic;
using MudSharp.Community.Boards;

namespace MudSharp.Computers;

public sealed class ComputerHostedBoardInfo
{
	public long BoardId { get; init; }
	public string BoardName { get; init; } = string.Empty;
	public int PostCount { get; init; }
}

public interface IComputerBoardService
{
	bool IsBoardsServiceEnabled(IComputerHost host);
	bool SetBoardsServiceEnabled(IComputerHost host, bool enabled, out string error);
	IEnumerable<ComputerHostedBoardInfo> GetHostedBoardDetails(IComputerHost host);
	IEnumerable<IBoard> GetHostedBoards(IComputerHost host);
	IBoard? ResolveHostedBoard(IComputerHost host, string identifier, out string error);
	IEnumerable<IBoardPost> GetPosts(IComputerHost host, IBoard board);
	IBoardPost? ReadPost(IComputerHost host, IBoard board, long postId, out string error);
	bool CreatePost(IComputerHost host, IComputerNetworkAccount account, IBoard board, string title, string text,
		out string error);
	bool DeletePost(IComputerHost host, IComputerNetworkAccount account, IBoard board, long postId, out string error);
	IEnumerable<string> GetAdvertisedServiceDetails(IComputerHost host, string applicationId);
}
