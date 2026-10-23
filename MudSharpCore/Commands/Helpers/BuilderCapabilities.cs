#nullable enable
using MudSharp.Framework;

namespace MudSharp.Commands.Helpers;

/// <summary>Capabilities derive from the delegates used to dispatch each operation.</summary>
internal sealed record BuilderCapabilities(bool Create, bool CanClone, bool Delete, bool CustomFilters, bool Revisions)
{
	public string Describe()
	{
		return $"Builder capabilities: new {Create.ToColouredString()}, clone {CanClone.ToColouredString()}, delete {Delete.ToColouredString()}, custom filters {CustomFilters.ToColouredString()}, revisions {Revisions.ToColouredString()}.\n" +
		       "Use capabilities to inspect supported operations. validate checks without activating; diff compares revisions where supported; undo reverses your last eligible set-name edit.\n" +
		       BuilderCatalogueOptions.Help.SubstituteANSIColour();
	}
}
