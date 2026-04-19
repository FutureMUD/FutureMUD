using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.TimeAndDate.Date;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Community.Boards;

public class Board : FrameworkItem, IBoard, IHaveFuturemud
{
    public Board(MudSharp.Models.Board board, IFuturemud gameworld)
    {
        Gameworld = gameworld;
        _id = board.Id;
        _name = board.Name;
        DisplayOnLogin = board.ShowOnLogin;
        _calendarId = board.CalendarId;
        foreach (Models.BoardPost post in board.BoardPosts)
        {
            _posts.Add(new BoardPost(post, gameworld));
        }
    }

    #region Overrides of Item

    public override string FrameworkItemType => "Board";

    #endregion

    #region Implementation of IBoard

    public bool DisplayOnLogin { get; set; }
    private long? _calendarId;
    private ICalendar _calendar;

    public ICalendar Calendar => _calendar ??= Gameworld.Calendars.Get(_calendarId ?? 0);

    public void MakeNewPost(IAccount author, string title, string text)
    {
        _posts.Add(new BoardPost(this, author, title, text, Gameworld));
    }

    public void MakeNewPost(ICharacter author, string title, string text)
    {
        _posts.Add(new BoardPost(this, author, title, text, Gameworld));
    }

    public void MakeNewPost(string authorName, string title, string text)
    {
        _posts.Add(new BoardPost(this, authorName, title, text, Gameworld));
    }

    public void DeletePost(IBoardPost post)
    {
        _posts.Remove(post);
        post.Delete();
    }

    private readonly List<IBoardPost> _posts = new();
    public IEnumerable<IBoardPost> Posts => _posts;

    #endregion

    #region Implementation of IHaveFuturemud

    public IFuturemud Gameworld { get; set; }

    #endregion
}
