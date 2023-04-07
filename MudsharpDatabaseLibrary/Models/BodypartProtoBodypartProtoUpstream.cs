using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class BodypartProtoBodypartProtoUpstream
    {
        public long Child { get; set; }
        public long Parent { get; set; }

        public virtual BodypartProto ChildNavigation { get; set; }
        public virtual BodypartProto ParentNavigation { get; set; }
    }
}
