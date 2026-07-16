#nullable enable

using System.Net;
using System.Text;

namespace FutureMUD.Web.Services;

public static class SiteMetadataEndpoints
{
	public static IEndpointRouteBuilder MapSiteMetadata(this IEndpointRouteBuilder endpoints)
	{
		endpoints.MapGet("/news/feed.xml", (MarkdownContentService content) =>
		{
			var items = content.GetNews().Take(20);
			var builder = new StringBuilder("<?xml version=\"1.0\" encoding=\"utf-8\"?><rss version=\"2.0\"><channel><title>FutureMUD News</title><link>https://futuremud.com/news</link><description>FutureMUD project news</description>");
			foreach (var item in items)
			{
				builder.Append("<item><title>").Append(WebUtility.HtmlEncode(item.Title)).Append("</title><link>https://futuremud.com/news/")
					.Append(Uri.EscapeDataString(item.Slug)).Append("</link><guid>https://futuremud.com/news/").Append(Uri.EscapeDataString(item.Slug))
					.Append("</guid><pubDate>").Append(item.PublishedAt?.UtcDateTime.ToString("R")).Append("</pubDate><description>")
					.Append(WebUtility.HtmlEncode(item.Summary)).Append("</description></item>");
			}
			builder.Append("</channel></rss>");
			return Results.Text(builder.ToString(), "application/rss+xml", Encoding.UTF8);
		});

		endpoints.MapGet("/sitemap.xml", (MarkdownContentService content) =>
		{
			var routes = new[] { "", "about", "license", "downloads", "getting-started", "news", "patch-notes", "docs/commands", "docs/futureprog/functions", "docs/futureprog/types", "docs/futureprog/collections", "docs/items/components" }
				.Concat(content.GetNews().Select(item => $"news/{item.Slug}"))
				.Concat(content.GetPatchNotes().Select(item => $"patch-notes/{item.Slug}"));
			var xml = "<?xml version=\"1.0\" encoding=\"utf-8\"?><urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">" +
				string.Concat(routes.Select(route => $"<url><loc>https://futuremud.com/{WebUtility.HtmlEncode(route)}</loc></url>")) + "</urlset>";
			return Results.Text(xml, "application/xml", Encoding.UTF8);
		});
		return endpoints;
	}
}
