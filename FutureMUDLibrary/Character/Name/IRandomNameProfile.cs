using System.Collections.Generic;
using MudSharp.CharacterCreation;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;

namespace MudSharp.Character.Name
{
    public interface IRandomNameProfile : IFrameworkItem, ISaveable, IEditableItem
    {
        IReadOnlyDictionary<NameUsage, string> NameUsageDiceExpressions { get; }
        IReadOnlyDictionary<NameUsage, List<(string Value, int Weight)>> RandomNames { get; }
        INameCulture Culture { get; }
        Gender Gender { get; }
        bool IsCompatibleGender(Gender gender);
        bool IsReady { get; }
        IPersonalName GetRandomPersonalName(bool nonSaving = false);
        string GetRandomNameElement(NameUsage usage);
        bool UseForChargenNameSuggestions(ICharacterTemplate template);
	}
}