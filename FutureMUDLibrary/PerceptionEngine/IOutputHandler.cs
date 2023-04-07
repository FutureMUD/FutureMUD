using MudSharp.Framework;
using System.Linq;

namespace MudSharp.PerceptionEngine {
    public interface IOutputHandler {
        /// <summary>
        ///     The Perceiver bound to this handler. Returns null if the underlying handler does not handle individual perceivers.
        /// </summary>
        IPerceiver Perceiver { get; }

        /// <summary>
        ///     Whether or not the underlying handler has any buffered output.
        /// </summary>
        bool HasBufferedOutput { get; }

        /// <summary>
        ///     The current buffered output, if any, on the underlying handler.
        /// </summary>
        string BufferedOutput { get; }

        /// <summary>
        ///     If true, tells the OutputHandler to ignore all output sent to it until further notice
        /// </summary>
        bool QuietMode { get; set; }

        /// <summary>
        ///     Set the Perceiver.
        /// </summary>
        /// <returns>Whether or not a Perceiver was set.</returns>
        bool Register(IPerceiver perceiver);

        /// <summary>
        ///     Passes a string to the handler.
        /// </summary>
        /// <returns>True if the string was handled in some way, false if not.</returns>
        bool Send(string text, bool newline = true, bool nopage = false);

        /// <summary>
        ///     Passes an Output instance to the handler.
        /// </summary>
        /// <returns>True if the string was handled in some way, false if not.</returns>
        bool Send(IOutput output, bool newline = true, bool nopage = false);

        /// <summary>
        ///     The user has entered the MORE command, and it should be handled
        /// </summary>
        void More();

        /// <summary>
        ///     Flushes any underlying buffered output.
        /// </summary>
        void Flush();

        /// <summary>
        ///     Sends just the prompt without any other texts
        /// </summary>
        /// <returns></returns>
        bool SendPrompt();
    }
}