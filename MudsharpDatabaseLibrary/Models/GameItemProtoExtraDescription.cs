using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Models
{
    public class GameItemProtoExtraDescription
    {
        public GameItemProtoExtraDescription()
        {

        }

        public long GameItemProtoId { get; set; }
        public int GameItemProtoRevisionNumber { get; set; }
        public long ApplicabilityProgId { get; set; }
        public string? ShortDescription { get; set; }
        public string? FullDescription { get; set; }
        public string? FullDescriptionAddendum { get; set; }
        public int Priority {get;set;}

        public virtual GameItemProto GameItemProto { get; set; }
        public virtual FutureProg ApplicabilityProg { get; set; }
    }
}
