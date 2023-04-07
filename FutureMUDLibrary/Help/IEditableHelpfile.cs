using System;
using System.Collections.Generic;
using MudSharp.FutureProg;

namespace MudSharp.Help {
    public interface IEditableHelpfile : IHelpfile {
        new List<string> Keywords { get; }

        /// <summary>
        ///     The help category to which this helpfile belongs
        /// </summary>
        new string Category { get; set; }

        /// <summary>
        ///     The Subcategory to which this helpfile belongs
        /// </summary>
        new string Subcategory { get; set; }

        /// <summary>
        ///     A short one-line summary of the content of the article, designed to be shown in help searches
        /// </summary>
        new string TagLine { get; set; }

        /// <summary>
        ///     The public text of this helpfile, which all can see
        /// </summary>
        new string PublicText { get; set; }

        /// <summary>
        ///     Contains rules about who may view this helpfile. Only people who meet all of the rules (if there are any) may view
        ///     the helpfile.
        /// </summary>
        new IFutureProg Rule { get; set; }

        /// <summary>
        ///     Additional fragments of text that will be displayed after the helpfile if certain rules are met
        /// </summary>
        new List<Tuple<IFutureProg, string>> AdditionalTexts { get; }

        /// <summary>
        ///     The account name of the person who last edited this helpfile
        /// </summary>
        new string LastEditedBy { get; set; }

        /// <summary>
        ///     The date on which this helpfile was last edited
        /// </summary>
        new DateTime LastEditedDate { get; set; }

        void SetName(string name);
    }
}