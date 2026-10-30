#nullable enable

using System.Collections.Generic;

namespace MudSharp.GameItems;

public sealed record KeywordDescription(string Keywords, string Description);

public interface IKeywordDescriptionProvider
{
	IReadOnlyCollection<KeywordDescription> KeywordDescriptions { get; }
}
