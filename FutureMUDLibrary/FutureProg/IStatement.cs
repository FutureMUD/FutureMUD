namespace MudSharp.FutureProg {
    public enum StatementResult {
        /// <summary>
        ///     The Statement executed as expected and did not alter the program flow
        /// </summary>
        Normal,

        /// <summary>
        ///     The Statement has requested to break the current loop
        /// </summary>
        Break,

        /// <summary>
        ///     The Statement has requested to skip to the next iteration of the current loop
        /// </summary>
        Continue,

        /// <summary>
        ///     The Statement is returning the FuturePROG
        /// </summary>
        Return,

        /// <summary>
        ///     The statement did not successfully execute and had an error
        /// </summary>
        Error
    }

    public interface IStatement {
        /// <summary>
        ///     If the Statement Reports an Error, this should contain the error message
        /// </summary>
        string ErrorMessage { get; }

        /// <summary>
        ///     Asks the statement what StatementResult it would expect to return if it executed successfully.
        /// </summary>
        StatementResult ExpectedResult { get; }

        /// <summary>
        ///     Executes the Statement with the supplied variable space, and indicates whether the script has ended
        /// </summary>
        /// <param name="variables">The variable space at the time of execution of the statement</param>
        /// <returns>A StatementResult indicating the result of the statement</returns>
        StatementResult Execute(IVariableSpace variables);

        bool IsReturnOrContainsReturnOnAllBranches();
    }
}