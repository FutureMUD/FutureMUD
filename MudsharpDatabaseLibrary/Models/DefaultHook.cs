using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class DefaultHook
    {
        public long HookId { get; set; }
        public string PerceivableType { get; set; }
        public long FutureProgId { get; set; }

        public virtual FutureProg FutureProg { get; set; }
        public virtual Hooks Hook { get; set; }
    }
}
