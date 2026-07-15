#nullable enable

using System.Text.Json;

namespace FutureMUD.Web.Publishing;

public sealed class ReleaseProductCatalogue
{
	private readonly IReadOnlyDictionary<string, ReleaseProductDefinition> _products;

	public ReleaseProductCatalogue(IWebHostEnvironment environment)
	{
		var path = Path.Combine(environment.ContentRootPath, "Configuration", "release-products.json");
		using var stream = File.OpenRead(path);
		var manifest = JsonSerializer.Deserialize<ReleaseProductManifest>(stream, new JsonSerializerOptions(JsonSerializerDefaults.Web))
			?? throw new InvalidDataException("The release product manifest is invalid.");
		_products = manifest.Products.ToDictionary(product => product.Id, StringComparer.Ordinal);
	}

	public IReadOnlyCollection<ReleaseProductDefinition> Products => _products.Values.ToList();
	public bool TryGet(string id, out ReleaseProductDefinition definition) => _products.TryGetValue(id, out definition!);
}
