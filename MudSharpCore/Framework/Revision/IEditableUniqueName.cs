#nullable enable

namespace MudSharp.Framework.Revision;

internal interface IEditableUniqueName : IEditableRevisableItem
{
	string? UniqueName { get; }

	void SetUniqueNameFromValidatedBulkRename(string? uniqueName);
}
