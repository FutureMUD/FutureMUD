namespace MudSharp.Framework {
    /// <summary>
    ///     An IProposal is designed to be partnered with an "Accept" effect type, for actions that a player must accept (such
    ///     as a clan invitation)
    /// </summary>
    public interface IProposal : IKeyworded {
        /// <summary>
        ///     Signals to the proposal that it has been accepted by the Supplicant, and it should take action to resolve the
        ///     Proposition.
        /// </summary>
        /// <param name="message">Any additional text passed in by the ACCEPT command</param>
        void Accept(string message = "");

        /// <summary>
        ///     Signals to the proposal that it has been rejected by the Supplicant, and it should take action to cancel the
        ///     Proposition.
        /// </summary>
        /// <param name="message">Any additional text passed in by the DECLINE command</param>
        void Reject(string message = "");

        /// <summary>
        ///     Signals to the proposal that it has timed out, and it should take action to cancel the proposition.
        /// </summary>
        void Expire();

        /// <summary>
        ///     Asks for a one line description of the proposal, as seen by the individual supplied
        /// </summary>
        /// <param name="voyeur">The voyeur for the description</param>
        /// <returns>A string describing the proposal</returns>
        string Describe(IPerceiver voyeur);
    }
}