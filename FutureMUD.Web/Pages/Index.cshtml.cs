#nullable enable

using FutureMUD.Web.Publishing;
using FutureMUD.Web.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FutureMUD.Web.Pages;

public sealed class IndexModel : PageModel
{
	private readonly MarkdownContentService _content;
	private readonly ReleaseProductCatalogue _products;
	private readonly ReleaseStore _releases;

	public IndexModel(MarkdownContentService content, ReleaseProductCatalogue products, ReleaseStore releases)
	{
		_content = content;
		_products = products;
		_releases = releases;
	}

	public IReadOnlyList<ContentDocument> News { get; private set; } = [];
	public IReadOnlyList<PublicRelease> Releases { get; private set; } = [];

	public async Task OnGetAsync(CancellationToken cancellationToken)
	{
		News = _content.GetNews();
		var releases = new List<PublicRelease>();
		foreach (var product in _products.Products)
		{
			var release = await _releases.GetLiveReleaseAsync(product.Id, cancellationToken);
			if (release is not null)
			{
				releases.Add(release);
			}
		}
		Releases = releases.OrderBy(release => release.Product, StringComparer.Ordinal).ToList();
	}
}
