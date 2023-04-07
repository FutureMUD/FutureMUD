using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class Board
    {
        public Board()
        {
            BoardPosts = new HashSet<BoardPost>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public bool ShowOnLogin { get; set; }
        public long? CalendarId { get; set; }

        public virtual Calendar Calendar { get; set; }
        public virtual ICollection<BoardPost> BoardPosts { get; set; }
    }
}
