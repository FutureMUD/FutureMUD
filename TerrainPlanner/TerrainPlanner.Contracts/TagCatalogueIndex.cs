namespace TerrainPlanner.Contracts;

public sealed class TagCatalogueIndex
{
	private readonly HashSet<string> _ambiguousShortNames;

	private TagCatalogueIndex(
		IReadOnlyDictionary<long, TagCatalogueItem> byId,
		IReadOnlyDictionary<string, TagCatalogueItem> byShortName,
		HashSet<string> ambiguousShortNames)
	{
		ById = byId;
		ByShortName = byShortName;
		_ambiguousShortNames = ambiguousShortNames;
	}

	public static TagCatalogueIndex Empty { get; } = Create([]);

	public IReadOnlyDictionary<long, TagCatalogueItem> ById { get; }
	public IReadOnlyDictionary<string, TagCatalogueItem> ByShortName { get; }

	public static TagCatalogueIndex Create(IEnumerable<TagCatalogueItem> tags)
	{
		ArgumentNullException.ThrowIfNull(tags);

		var byId = tags
			.GroupBy(tag => tag.Id)
			.ToDictionary(group => group.Key, group => group.First());
		var ambiguousShortNames = byId.Values
			.GroupBy(tag => tag.ShortName, StringComparer.OrdinalIgnoreCase)
			.Where(group => group.Count() > 1)
			.Select(group => group.Key)
			.ToHashSet(StringComparer.OrdinalIgnoreCase);
		var byShortName = byId.Values
			.Where(tag => FeatureMaskUnavailableReason(tag, ambiguousShortNames) is null)
			.ToDictionary(tag => tag.ShortName, StringComparer.OrdinalIgnoreCase);

		return new TagCatalogueIndex(byId, byShortName, ambiguousShortNames);
	}

	public bool IsAvailableForFeatureMask(TagCatalogueItem tag) =>
		FeatureMaskUnavailableReason(tag) is null;

	public string? FeatureMaskUnavailableReason(TagCatalogueItem tag) =>
		FeatureMaskUnavailableReason(tag, _ambiguousShortNames);

	private static string? FeatureMaskUnavailableReason(TagCatalogueItem tag,
		IReadOnlySet<string> ambiguousShortNames)
	{
		if (string.IsNullOrWhiteSpace(tag.ShortName))
		{
			return "This tag has no short name for an autobuilder feature mask.";
		}

		if (tag.ShortName.Contains(',') || tag.ShortName.Contains('|') ||
			tag.ShortName.Contains('\r') || tag.ShortName.Contains('\n'))
		{
			return "This tag's short name contains an autobuilder feature-mask delimiter.";
		}

		return ambiguousShortNames.Contains(tag.ShortName)
			? $"The short name '{tag.ShortName}' is shared by multiple tags."
			: null;
	}
}
