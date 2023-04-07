using System;
using System.Collections.Generic;
using MudSharp.CharacterCreation;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;

namespace MudSharp.Character.Name
{
    public enum NameStyle {
        GivenOnly = 0,
        SimpleFull = 1,
        FullName = 2,
        Affectionate = 3,
        SurnameOnly = 4,
        FullWithNickname = 5
    }

    public interface INameCulture : IEditableItem, ISaveable, IXmlSavable
    {
	    
        IEnumerable<NameCultureElement> NameCultureElements { get; }
        IEnumerable<IRandomNameProfile> RandomNameProfiles { get; }
        Tuple<string, List<NameUsage>> NamePattern(NameStyle style);
        IPersonalName GetPersonalName(string pattern, bool nonSaving = false);
    }
}