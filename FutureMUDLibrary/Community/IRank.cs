using System;
using System.Collections.Generic;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.GameItems;

namespace MudSharp.Community {
    public interface IRank : IFrameworkItem, ISaveable, IFutureProgVariable {
        IClan Clan { get; }

        IEnumerable<string> Abbreviations { get; }

        List<Tuple<IFutureProg, string>> AbbreviationsAndProgs { get; }

        IEnumerable<string> Titles { get; }

        List<Tuple<IFutureProg, string>> TitlesAndProgs { get; }

        List<IPaygrade> Paygrades { get; }

        ClanPrivilegeType Privileges { get; set; }

        IGameItemProto InsigniaGameItem { get; set; }

        ClanFameType FameType {get;set;}

        int RankNumber { get; set; }

        /// <summary>
        ///     E.g. Enlisted, Commissioned. Different permissions required to promote/demote to different rank path
        /// </summary>
        string RankPath { get; set; }

        /// <summary>
        ///     Formal Abbreviation when used as a form of address, e.g. Cpt. for Captain
        /// </summary>
        /// <param name="character">The character in question. If null, returns only default values</param>
        /// <returns>An abbreviation for the rank</returns>
        string Abbreviation(ICharacter character);

        /// <summary>
        ///     Formal name of rank when used as a form of address, e.g. Major General
        /// </summary>
        string Title(ICharacter character);

        void SetName(string newName);
    }
}