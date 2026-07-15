#nullable enable

using FutureMUD.Web.Publishing;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FutureMUD.Web.Pages;

public sealed class DownloadsModel : PageModel
{
	private readonly ReleaseProductCatalogue _catalogue;
	private readonly ReleaseStore _store;

	public DownloadsModel(ReleaseProductCatalogue catalogue, ReleaseStore store)
	{
		_catalogue = catalogue;
		_store = store;
	}

	public IReadOnlyList<DownloadProduct> Products { get; private set; } = [];

	public async Task OnGetAsync(CancellationToken cancellationToken)
	{
		var products = new List<DownloadProduct>();
		foreach (var product in _catalogue.Products.OrderBy(item => item.PublicName, StringComparer.Ordinal))
		{
			products.Add(new DownloadProduct(product, await _store.GetLiveReleaseAsync(product.Id, cancellationToken)));
		}
		Products = products;
	}
}

public sealed record DownloadProduct(ReleaseProductDefinition Product, PublicRelease? Release);
