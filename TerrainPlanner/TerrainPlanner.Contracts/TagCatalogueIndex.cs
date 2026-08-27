namespace TerrainPlanner.Contracts;

public sealed class TagCatalogueIndex
{
	private TagCatalogueIndex(IReadOnlyDictionary<long, TagCatalogueItem> byId)
	{
		ById = byId;
	}

	public static TagCatalogueIndex Empty { get; } = Create([]);

	public IReadOnlyDictionary<long, TagCatalogueItem> ById { get; }

	public static TagCatalogueIndex Create(IEnumerable<TagCatalogueItem> tags)
	{
		ArgumentNullException.ThrowIfNull(tags);

		var byId = tags
			.GroupBy(tag => tag.Id)
			.ToDictionary(group => group.Key, group => group.First());

		return new TagCatalogueIndex(byId);
	}
}
