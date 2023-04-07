using MudSharp.Framework;

namespace MudSharp.Communication.Language.Scramblers {
    public interface ILanguageScrambler {
        string Name { get; }

        string Description { get; }

        /// <summary>
        ///     Whether or not the Scrambler is static and controlled by the GameWorld, or instanced onto a Character.
        /// </summary>
        bool Instanced { get; }

        string Scramble(string input, double obscuredratio);
        string Scramble(ExplodedString input, double obscuredratio);
        string Scramble(ExplodedString input, double shortbiasedobscuredratio, double longbiasedobscuredratio);
    }
}