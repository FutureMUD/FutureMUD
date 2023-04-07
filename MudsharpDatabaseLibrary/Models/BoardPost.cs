using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class BoardPost
    {
        public long Id { get; set; }
        public long BoardId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public long? AuthorId { get; set; }
        public DateTime PostTime { get; set; }
        public bool AuthorIsCharacter { get; set; }
        public string InGameDateTime { get; set; }
        public virtual Board Board { get; set; }
        public string AuthorName { get; set; }
        public string AuthorShortDescription { get; set; }
        public string AuthorFullDescription { get; set; }
    }
}
