using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class FutureProgsParameter
    {
        public long FutureProgId { get; set; }
        public int ParameterIndex { get; set; }
        public long ParameterType { get; set; }
        public string ParameterName { get; set; }

        public virtual FutureProg FutureProg { get; set; }
    }
}
