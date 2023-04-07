using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Form.Shape
{
    public enum Gender : short {
        /// <summary>
        ///     Gender is unknown or not able to be determined. Used for things that have a gender that is obscured.
        /// </summary>
        Indeterminate = 0,

        /// <summary>
        ///     Specifically of the neuter gender
        /// </summary>
        Neuter = 1,

        /// <summary>
        ///     Boys have a penis
        /// </summary>
        Male = 2,

        /// <summary>
        ///     Girls have a vagina
        /// </summary>
        Female = 3,

        /// <summary>
        ///     And some people have different thoughts on the matter altogether
        /// </summary>
        NonBinary = 4
    }
}
