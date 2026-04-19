#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Community.Boards;
using MudSharp.Framework;

namespace MudSharp.Computers;

public sealed class ComputerBoardService : IComputerBoardService
{
	private readonly IFuturemud _gameworld;

	public ComputerBoardService(IFuturemud gameworld)
	{
		_gameworld = gameworld;
	}

	public bool IsBoardsServiceEnabled(IComputerHost host)
	{
		return host.IsNetworkServiceEnabled("boards");
	}

	public bool SetBoardsServiceEnabled(IComputerHost host, bool enabled, out string error)
	{
		return host.SetNetworkServiceEnabled("boards", enabled, out error);
	}

	public IEnumerable<ComputerHostedBoardInfo> GetHostedBoardDetails(IComputerHost host)
	{
		return GetHostedBoards(host)
			.OrderBy(x => x.Name)
			.ThenBy(x => x.Id)
			.Select(x => new ComputerHostedBoardInfo
			{
				BoardId = x.Id,
				BoardName = x.Name,
				PostCount = x.Posts.Count()
			})
			.ToList();
	}

	public IEnumerable<IBoard> GetHostedBoards(IComputerHost host)
	{
		return host.HostedBoardIds
			.Distinct()
			.Select(id => _gameworld.Boards.Get(id))
			.Where(x => x is not null)
			.Cast<IBoard>()
			.OrderBy(x => x.Name)
			.ThenBy(x => x.Id)
			.ToList();
	}

	public IBoard? ResolveHostedBoard(IComputerHost host, string identifier, out string error)
	{
		error = string.Empty;
		var boards = GetHostedBoards(host).ToList();
		if (!boards.Any())
		{
			error = $"{host.Name.ColourName()} is not currently exposing any network boards.";
			return null;
		}

		if (string.IsNullOrWhiteSpace(identifier))
		{
			error = "You must specify which hosted board you mean.";
			return null;
		}

		if (long.TryParse(identifier, out var boardId))
		{
			var idMatch = boards.FirstOrDefault(x => x.Id == boardId);
			if (idMatch is not null)
			{
				return idMatch;
			}
		}

		var exact = boards
			.Where(x => x.Name.Equals(identifier, StringComparison.InvariantCultureIgnoreCase))
			.ToList();
		if (exact.Count == 1)
		{
			return exact.Single();
		}

		if (exact.Count > 1)
		{
			error = $"More than one hosted board on {host.Name.ColourName()} matches {identifier.ColourCommand()}.";
			return null;
		}

		var partial = boards
			.Where(x => x.Name.StartsWith(identifier, StringComparison.InvariantCultureIgnoreCase))
			.ToList();
		if (partial.Count == 1)
		{
			return partial.Single();
		}

		if (partial.Count > 1)
		{
			error = $"More than one hosted board on {host.Name.ColourName()} begins with {identifier.ColourCommand()}.";
			return null;
		}

		error = $"{host.Name.ColourName()} is not exposing any board matching {identifier.ColourCommand()}.";
		return null;
	}

	public IEnumerable<IBoardPost> GetPosts(IComputerHost host, IBoard board)
	{
		if (!IsHostedBoard(host, board))
		{
			return Enumerable.Empty<IBoardPost>();
		}

		return board.Posts
			.OrderByDescending(x => x.PostTime)
			.ThenByDescending(x => x.Id)
			.ToList();
	}

	public IBoardPost? ReadPost(IComputerHost host, IBoard board, long postId, out string error)
	{
		error = string.Empty;
		if (!IsHostedBoard(host, board))
		{
			error = $"{host.Name.ColourName()} is not exposing the board {board.Name.ColourName()}.";
			return null;
		}

		var post = board.Posts.FirstOrDefault(x => x.Id == postId);
		if (post is not null)
		{
			return post;
		}

		error = $"{board.Name.ColourName()} does not have any post with id {postId.ToString("N0").ColourValue()}.";
		return null;
	}

	public bool CreatePost(IComputerHost host, IComputerNetworkAccount account, IBoard board, string title, string text,
		out string error)
	{
		error = string.Empty;
		if (!IsHostedBoard(host, board))
		{
			error = $"{host.Name.ColourName()} is not exposing the board {board.Name.ColourName()}.";
			return false;
		}

		if (!ValidateAuthenticatedAccount(host, account, out error))
		{
			return false;
		}

		if (string.IsNullOrWhiteSpace(title))
		{
			error = "You must specify a title for the new board post.";
			return false;
		}

		if (title.Length > 200)
		{
			error = "Board post titles must be 200 characters or fewer.";
			return false;
		}

		if (text.Length > 5000)
		{
			error = "Board posts must be under 5000 characters in length.";
			return false;
		}

		board.MakeNewPost(account.Address, title.Trim().TitleCase(), text);
		return true;
	}

	public bool DeletePost(IComputerHost host, IComputerNetworkAccount account, IBoard board, long postId, out string error)
	{
		error = string.Empty;
		if (!IsHostedBoard(host, board))
		{
			error = $"{host.Name.ColourName()} is not exposing the board {board.Name.ColourName()}.";
			return false;
		}

		if (!ValidateAuthenticatedAccount(host, account, out error))
		{
			return false;
		}

		var post = board.Posts.FirstOrDefault(x => x.Id == postId);
		if (post is null)
		{
			error = $"{board.Name.ColourName()} does not have any post with id {postId.ToString("N0").ColourValue()}.";
			return false;
		}

		if (post.AuthorId.HasValue || !post.AuthorName.EqualTo(account.Address))
		{
			error = $"Only the author {account.Address.ColourName()} can delete that network board post.";
			return false;
		}

		board.DeletePost(post);
		return true;
	}

	public IEnumerable<string> GetAdvertisedServiceDetails(IComputerHost host, string applicationId)
	{
		if (!applicationId.EqualTo("boards") || !host.IsNetworkServiceEnabled("boards"))
		{
			return Enumerable.Empty<string>();
		}

		var boards = GetHostedBoards(host)
			.Select(x => x.Name)
			.OrderBy(x => x)
			.ToList();
		if (!boards.Any())
		{
			return Enumerable.Empty<string>();
		}

		if (boards.Count <= 3)
		{
			return boards;
		}

		return boards.Take(3)
			.Concat(new[] { $"{boards.Count:N0} boards total" })
			.ToList();
	}

	private bool ValidateAuthenticatedAccount(IComputerHost host, IComputerNetworkAccount account, out string error)
	{
		error = string.Empty;
		if (!account.Enabled)
		{
			error = $"{account.Address.ColourName()} is not currently enabled for network services.";
			return false;
		}

		var hostedDomains = _gameworld.ComputerNetworkIdentityService.GetHostedDomains(host)
			.Where(x => x.Enabled)
			.Select(x => x.DomainName)
			.ToHashSet(StringComparer.InvariantCultureIgnoreCase);
		if (hostedDomains.Contains(account.DomainName))
		{
			return true;
		}

		error = $"{host.Name.ColourName()} does not accept board logins for {account.Address.ColourName()}.";
		return false;
	}

	private static bool IsHostedBoard(IComputerHost host, IBoard board)
	{
		return host.HostedBoardIds.Contains(board.Id);
	}
}
