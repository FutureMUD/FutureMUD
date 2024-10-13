using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MudSharp.Framework;
using MudSharp.TimeAndDate;

namespace MudSharp.Community.Boards
{
    public interface IBoardPost : IFrameworkItem {
        string Title { get; }
        string Text { get; }
        long? AuthorId { get; }
        string AuthorName { get; }
        DateTime PostTime { get; }
        bool AuthorIsCharacter { get; }
        [CanBeNull] MudDateTime InGameDateTime { get; }
        string AuthorShortDescription { get; }
        string AuthorFullDescription { get; }
        void Delete();
    }
}
