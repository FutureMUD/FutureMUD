using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.TimeAndDate.Date;

namespace MudSharp.Community.Boards
{
    public interface IBoard : IFrameworkItem {
        bool DisplayOnLogin { get; }
        ICalendar Calendar { get; }

        void MakeNewPost(IAccount author, string title, string text);
        void MakeNewPost(ICharacter author, string title, string text);

        IEnumerable<IBoardPost> Posts { get; }
    }
}
