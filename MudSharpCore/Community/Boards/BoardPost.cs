using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Character.Name;
using MudSharp.Database;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.Models;
using MudSharp.TimeAndDate;

namespace MudSharp.Community.Boards;

public class BoardPost : LateInitialisingItem, IBoardPost
{
	public BoardPost(MudSharp.Models.BoardPost post, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = post.Id;
		IdInitialised = true;
		Board = gameworld.Boards.Get(post.BoardId);
		Title = post.Title;
		Text = post.Content;
		PostTime = post.PostTime;
		AuthorId = post.AuthorId;
		AuthorIsCharacter = post.AuthorIsCharacter;
		InGameDateTime = !string.IsNullOrEmpty(post.InGameDateTime)
			? new MudDateTime(post.InGameDateTime, Gameworld)
			: null;
		AuthorFullDescription = post.AuthorFullDescription;
		AuthorShortDescription = post.AuthorShortDescription;
		_authorName = post.AuthorName;
	}

	public BoardPost(IBoard board, IAccount account, string title, string text, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		Board = board;
		AuthorId = account?.Id;
		_authorName = account?.Name ?? "System";
		Title = title;
		Text = text;
		PostTime = DateTime.UtcNow;
		InGameDateTime = Board.Calendar?.CurrentDateTime;
		AuthorIsCharacter = false;
		Gameworld.SaveManager.AddInitialisation(this);
	}

	public BoardPost(IBoard board, ICharacter author, string title, string text, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		Board = board;
		AuthorId = author?.Id;
		_authorName = author?.CurrentName.GetName(NameStyle.FullName) ?? "System";
		AuthorShortDescription =
			author?.HowSeen(author, colour: false,
				flags: PerceiveIgnoreFlags.IgnoreCanSee | PerceiveIgnoreFlags.IgnoreSelf);
		AuthorFullDescription =
			author?.Body.LookText(author, false);
		Title = title;
		Text = text;
		PostTime = DateTime.UtcNow;
		InGameDateTime = Board.Calendar?.CurrentDateTime ?? author?.Location.DateTime();
		AuthorIsCharacter = true;
		Gameworld.SaveManager.AddInitialisation(this);
	}

	#region Overrides of Item

	public override string FrameworkItemType => "BoardPost";

	#endregion

	#region Implementation of IBoardPost

	public IBoard Board { get; set; }
	public string Text { get; set; }
	public long? AuthorId { get; set; }

	public string AuthorShortDescription { get; set; }
	public string AuthorFullDescription { get; set; }

	private string _authorName;

	public string AuthorName
	{
		get
		{
			if (!AuthorId.HasValue)
			{
				return "System";
			}

			if (_authorName == null)
			{
				if (AuthorIsCharacter)
				{
					_authorName = Gameworld.TryGetCharacter(AuthorId.Value, true).PersonalName
					                       .GetName(NameStyle.FullName);
				}
				else
				{
					using (new FMDB())
					{
						_authorName = FMDB.Context.Accounts.FirstOrDefault(x => x.Id == AuthorId.Value)?.Name ??
						              "System";
					}
				}

				Changed = true;
			}

			return _authorName;
		}
	}

	public string Title { get; set; }

	public DateTime PostTime { get; set; }
	public bool AuthorIsCharacter { get; set; }
	public MudDateTime InGameDateTime { get; set; }

	#endregion

	#region Overrides of LateInitialisingItem

	public override void Save()
	{
		var dbitem = FMDB.Context.BoardPosts.Find(Id);
		if (dbitem != null)
		{
			dbitem.Content = Text;
			dbitem.AuthorId = AuthorId;
			dbitem.Title = Title;
			dbitem.InGameDateTime = InGameDateTime?.GetDateTimeString();
			dbitem.AuthorIsCharacter = AuthorIsCharacter;
			dbitem.PostTime = PostTime;
			dbitem.AuthorName = AuthorName;
			dbitem.AuthorFullDescription = AuthorFullDescription;
			dbitem.AuthorShortDescription = AuthorShortDescription;
		}

		Changed = false;
	}

	public override object DatabaseInsert()
	{
		var dbitem = new Models.BoardPost();
		FMDB.Context.BoardPosts.Add(dbitem);
		dbitem.Title = Title;
		dbitem.Content = Text;
		dbitem.AuthorId = AuthorId;
		dbitem.BoardId = Board.Id;
		dbitem.PostTime = PostTime;
		dbitem.AuthorIsCharacter = AuthorIsCharacter;
		dbitem.InGameDateTime = InGameDateTime?.GetDateTimeString();
		dbitem.AuthorFullDescription = AuthorFullDescription;
		dbitem.AuthorShortDescription = AuthorShortDescription;
		dbitem.AuthorName = AuthorName;
		return dbitem;
	}

	public override void SetIDFromDatabase(object dbitem)
	{
		var dbpost = (MudSharp.Models.BoardPost)dbitem;
		_id = dbpost.Id;
	}

	#endregion
}