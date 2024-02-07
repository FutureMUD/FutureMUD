using System;
using System.Collections.Generic;
using MudSharp.Character;
using MudSharp.CharacterCreation;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;

namespace MudSharp.Help {
    public interface IHelpfile : IFrameworkItem, ISaveable {
        IEnumerable<string> Keywords { get; }

        /// <summary>
        ///     The help category to which this helpfile belongs
        /// </summary>
        string Category { get; }

        /// <summary>
        ///     The Subcategory to which this helpfile belongs
        /// </summary>
        string Subcategory { get; }

        /// <summary>
        ///     A short one-line summary of the content of the article, designed to be shown in help searches
        /// </summary>
        string TagLine { get; }

        /// <summary>
        ///     The public text of this helpfile, which all can see
        /// </summary>
        string PublicText { get; }

        /// <summary>
        ///     The account name of the person who last edited this helpfile
        /// </summary>
        string LastEditedBy { get; }

        /// <summary>
        ///     The date on which this helpfile was last edited
        /// </summary>
        DateTime LastEditedDate { get; }

        /// <summary>
        ///     Contains rules about who may view this helpfile. Only people who meet all of the rules (if there are any) may view
        ///     the helpfile.
        /// </summary>
        IFutureProg Rule { get; }

        /// <summary>
        ///     Additional fragments of text that will be displayed after the helpfile if certain rules are met
        /// </summary>
        IEnumerable<Tuple<IFutureProg, string>> AdditionalTexts { get; }

        IEditableHelpfile GetEditableHelpfile { get; }

        /// <summary>
        ///     Determines whether or not the specified character can view the helpfile at all
        /// </summary>
        /// <param name="actor">The actor enquiring</param>
        /// <returns>True if they can view the helpfile</returns>
        bool CanView(ICharacter actor);

        /// <summary>
        ///     Displays the helpfile for the actor
        /// </summary>
        /// <param name="actor">The actor enquiring</param>
        /// <returns>The text of the helpfile</returns>
        string DisplayHelpFile(ICharacter actor);

        /// <summary>
        ///     Displays the helpfile for the chargen
        /// </summary>
        /// <param name="chargen">The chargen enquiring</param>
        /// <returns>The text of the helpfile</returns>
        string DisplayHelpFile(IChargen chargen);

        void Delete();
        void DeleteExtraText(int index);
        void ReorderExtraText(int oldIndex, int newIndex);
    }
}