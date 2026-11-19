#nullable enable

using Markdig;
using Markdig.Extensions.AutoIdentifiers;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using System.Text.RegularExpressions;

namespace FutureMUD.Web.Services;

public sealed partial class BuilderGuideCatalogue
{
	public const string Route = "/docs/builder-guides";
	internal const string ResourcePrefix = "BuilderGuides.";
	private static readonly Uri RepositoryGuideDirectory =
		new("https://github.com/FutureMUD/FutureMUD/blob/master/Design%20Documents/Builder%20Guides/");
	private static readonly MarkdownPipeline Pipeline = new MarkdownPipelineBuilder()
		.DisableHtml()
		.UsePipeTables()
		.UseAutoIdentifiers(AutoIdentifierOptions.GitHub)
		.Build();
	private readonly Lazy<IReadOnlyDictionary<string, BuilderGuide>> _guides = new(LoadEmbeddedGuides);

	internal IReadOnlyDictionary<string, BuilderGuide> Guides => _guides.Value;

	private static IReadOnlyDictionary<string, BuilderGuide> LoadEmbeddedGuides()
	{
		var assembly = typeof(BuilderGuideCatalogue).Assembly;
		return Load(assembly.GetManifestResourceNames()
			.Where(name => name.StartsWith(ResourcePrefix, StringComparison.Ordinal))
			.Select(name =>
			{
				using var reader = new StreamReader(assembly.GetManifestResourceStream(name)!);
				return (name[ResourcePrefix.Length..], reader.ReadToEnd());
			}));
	}

	internal static IReadOnlyDictionary<string, BuilderGuide> Load(IEnumerable<(string FileName, string Text)> sources)
	{
		var guides = new Dictionary<string, BuilderGuide>(StringComparer.Ordinal);
		var slugBySourcePath = new Dictionary<string, string>(StringComparer.Ordinal);
		foreach (var (fileName, text) in sources)
		{
			var (metadata, markdown) = MarkdownContentService.ParseFrontMatter(text);
			var slug = metadata.GetValueOrDefault("slug");
			var category = metadata.GetValueOrDefault("category");
			var summary = metadata.GetValueOrDefault("summary");
			var body = markdown.TrimStart();
			var titleEnd = body.IndexOf('\n');
			if (slug is null || !SlugPattern().IsMatch(slug) ||
				string.IsNullOrWhiteSpace(category) || category.Length > 100 || string.IsNullOrWhiteSpace(summary) ||
				titleEnd < 3 || !body.StartsWith("# ", StringComparison.Ordinal) || string.IsNullOrWhiteSpace(body[2..titleEnd]))
			{
				throw new InvalidDataException($"Builder guide '{fileName}' requires a URL slug, category, summary and Markdown title.");
			}

			var guide = new BuilderGuide(slug, body[2..titleEnd].Trim(), category, summary, body[(titleEnd + 1)..]);
			if (!guides.TryAdd(slug, guide))
			{
				throw new InvalidDataException($"Builder guide '{fileName}' reuses the slug '{slug}'.");
			}

			var source = new Uri(RepositoryGuideDirectory, Uri.EscapeDataString(fileName));
			slugBySourcePath.Add(source.AbsolutePath, slug);
		}

		return guides.ToDictionary(entry => entry.Key,
			entry => entry.Value with { Html = Render(entry.Value.Markdown, slugBySourcePath) },
			StringComparer.Ordinal);
	}

	private static string Render(string markdown, IReadOnlyDictionary<string, string> slugBySourcePath)
	{
		var document = Markdown.Parse(markdown, Pipeline);
		foreach (var link in document.Descendants<LinkInline>())
		{
			link.Url = ResolveLink(link.Url, slugBySourcePath);
		}
		foreach (var link in document.Descendants<AutolinkInline>().ToList())
		{
			if (!MarkdownContentService.IsSafeLinkTarget(link.IsEmail ? $"mailto:{link.Url}" : link.Url))
			{
				link.ReplaceBy(new LiteralInline(link.Url));
			}
		}
		return document.ToHtml(Pipeline);
	}

	private static string ResolveLink(string? target, IReadOnlyDictionary<string, string> slugBySourcePath)
	{
		if (target is null || target.Contains('\\') || !MarkdownContentService.IsSafeLinkTarget(target))
		{
			return "#";
		}
		if (target.StartsWith('#') || target.StartsWith('/') || target.StartsWith('?') || Uri.TryCreate(target, UriKind.Absolute, out _))
		{
			return target;
		}
		if (!Uri.TryCreate(RepositoryGuideDirectory, target, out var source))
		{
			return "#";
		}
		return slugBySourcePath.TryGetValue(source.AbsolutePath, out var slug)
			? $"{Route}/{slug}{source.Query}{source.Fragment}"
			: source.AbsoluteUri;
	}

	[GeneratedRegex("\\A[a-z0-9]+(?:-[a-z0-9]+)*\\z", RegexOptions.CultureInvariant)]
	private static partial Regex SlugPattern();
}

internal sealed record BuilderGuide(string Slug, string Title, string Category, string Summary, string Markdown)
{
	public string Html { get; init; } = string.Empty;
}
