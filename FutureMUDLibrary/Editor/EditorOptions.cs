using System;

namespace MudSharp.Editor {
    [Flags]
    public enum EditorOptions {
        /// <summary>
        ///     All default options
        /// </summary>
        None = 0,

        /// <summary>
        ///     If this option is selected, the editor will not permit cancellation
        /// </summary>
        DenyCancel = 1 << 0,

        /// <summary>
        ///     If this option is selected, the editor can submit even completely empty
        /// </summary>
        PermitEmpty = 1 << 1
    }
}