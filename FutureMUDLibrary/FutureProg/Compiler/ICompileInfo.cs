using System.Collections.Generic;

namespace MudSharp.FutureProg.Compiler {
    public interface ICompileInfo {
        /// <summary>
        ///     Indicates whether the Statement compiled successfully
        /// </summary>
        bool IsError { get; }

        /// <summary>
        ///     Indicates whether the next compiled statement was a comment
        /// </summary>
        bool IsComment { get; }

        /// <summary>
        ///     The error message associated with any compilation error
        /// </summary>
        string ErrorMessage { get; }

        /// <summary>
        ///     The line number of any error
        /// </summary>
        int ErrorLineNumber { get; }

        /// <summary>
        ///     A successfully compiled IStatement
        /// </summary>
        IStatement CompiledStatement { get; }

        /// <summary>
        ///     The VariableSpace at the conclusion of the compilation of this statement
        /// </summary>
        IDictionary<string, ProgVariableTypes> VariableSpace { get; }

        /// <summary>
        ///     The lines remaining at the conclusion of the compilation of this statement
        /// </summary>
        IEnumerable<string> RemainingLines { get; }

        /// <summary>
        ///     The line number of the first line in this statement
        /// </summary>
        int BeginningLineNumber { get; }

        /// <summary>
        ///     The line number of the last line in this statement
        /// </summary>
        int EndingLineNumber { get; }
    }
}