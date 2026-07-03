using MudSharp.Framework;
using MudSharp.Framework.Save;
using System.Collections.Generic;

#nullable enable

namespace MudSharp.Communication;

public interface IWritingCollectionEntry
{
	int Page { get; }
	int Order { get; }
	ICanBeRead Readable { get; }
}

public interface IWritingCollection : IFrameworkItem, ISaveable
{
	string Description { get; }
	string DefaultTitle { get; }
	IEnumerable<IWritingCollectionEntry> Entries { get; }
	int PageCount { get; }
	int DocumentLengthForPage(int page);
}
