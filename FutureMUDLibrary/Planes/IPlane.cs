using MudSharp.Framework;
using MudSharp.Framework.Revision;
using System.Collections.Generic;

namespace MudSharp.Planes;

public interface IPlane : IEditableItem, IKeywordedItem, IHaveMultipleNames
{
	IEnumerable<string> Aliases { get; }
	string Description { get; }
	int DisplayOrder { get; }
	bool IsDefault { get; }
}
