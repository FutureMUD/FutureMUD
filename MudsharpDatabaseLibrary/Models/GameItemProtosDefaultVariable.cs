using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class GameItemProtosDefaultVariable
    {
        public long GameItemProtoId { get; set; }
        public string VariableName { get; set; }
        public string VariableValue { get; set; }
        public int GameItemProtoRevNum { get; set; }

        public virtual GameItemProto GameItemProto { get; set; }
    }
}
