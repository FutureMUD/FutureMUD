#nullable enable

using FutureMUD.Web.Configuration;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace FutureMUD.Web.Services;

public sealed partial class MarkdownContentService
{
	private readonly string _contentRoot;

	public MarkdownContentService(IWebHostEnvironment environment, IOptions<FutureMudWebOptions> options)
	{
		_contentRoot = Path.GetFullPath(options.Value.ContentRoot, environment.ContentRootPath);
	}

	public ContentDocument GetPage(string slug)
	{
		if (!SafeSlugRegex().IsMatch(slug))
		{
			throw new FileNotFoundException();
		}

		return Parse(Path.Combine(_contentRoot, "Pages", $"{slug}.md"), false);
	}

	public IReadOnlyList<ContentDocument> GetNews() => GetDatedDocuments("News");

	public ContentDocument? GetNews(string slug) => GetNews()
		.FirstOrDefault(document => document.Slug.Equals(slug, StringComparison.OrdinalIgnoreCase));

	public IReadOnlyList<ContentDocument> GetPatchNotes() => GetDatedDocuments("PatchNotes");

	public ContentDocument? GetPatchNote(string slug) => GetPatchNotes()
		.FirstOrDefault(document => document.Slug.Equals(slug, StringComparison.OrdinalIgnoreCase));

	private IReadOnlyList<ContentDocument> GetDatedDocuments(string collection)
	{
		var directory = Path.Combine(_contentRoot, collection);
		if (!Directory.Exists(directory))
		{
			return [];
		}

		return Directory.EnumerateFiles(directory, "*.md", SearchOption.TopDirectoryOnly)
			.Where(path => DatedContentFileRegex().IsMatch(Path.GetFileName(path)))
			.Select(path => Parse(path, true))
			.OrderByDescending(document => document.PublishedAt)
			.ThenBy(document => document.Slug, StringComparer.Ordinal)
			.ToList();
	}

	public static string RenderMarkdown(string markdown)
	{
		var output = new StringBuilder();
		var paragraph = new List<string>();
		var inCode = false;
		var inList = false;

		void FlushParagraph()
		{
			if (paragraph.Count == 0)
			{
				return;
			}
			output.Append("<p>").Append(RenderInline(string.Join((char)0x0A, paragraph))).AppendLine("</p>");
			paragraph.Clear();
		}

		void CloseList()
		{
			if (!inList)
			{
				return;
			}
			output.AppendLine("</ul>");
			inList = false;
		}

		foreach (var rawLine in markdown.Replace("\r\n", "\n", StringComparison.Ordinal).Split('\n'))
		{
			if (rawLine.StartsWith("```", StringComparison.Ordinal))
			{
				FlushParagraph();
				CloseList();
				output.AppendLine(inCode ? "</code></pre>" : "<pre><code>");
				inCode = !inCode;
				continue;
			}

			if (inCode)
			{
				output.AppendLine(WebUtility.HtmlEncode(rawLine));
				continue;
			}

			if (string.IsNullOrWhiteSpace(rawLine))
			{
				FlushParagraph();
				CloseList();
				continue;
			}

			var heading = HeadingRegex().Match(rawLine);
			if (heading.Success)
			{
				FlushParagraph();
				CloseList();
				var level = heading.Groups[1].Value.Length;
				output.Append($"<h{level}>").Append(RenderInline(heading.Groups[2].Value)).AppendLine($"</h{level}>");
				continue;
			}

			if (rawLine.StartsWith("- ", StringComparison.Ordinal))
			{
				FlushParagraph();
				if (!inList)
				{
					output.AppendLine("<ul>");
					inList = true;
				}
				output.Append("<li>").Append(RenderInline(rawLine[2..])).AppendLine("</li>");
				continue;
			}

			paragraph.Add(rawLine.Trim());
		}

		FlushParagraph();
		CloseList();
		if (inCode)
		{
			output.AppendLine("</code></pre>");
		}
		return output.ToString();
	}

	private ContentDocument Parse(string path, bool dated)
	{
		if (!File.Exists(path))
		{
			throw new FileNotFoundException("Content was not found.", path);
		}

		var text = File.ReadAllText(path);
		var (metadata, markdown) = ParseFrontMatter(text);
		var fileName = Path.GetFileNameWithoutExtension(path);
		var slug = dated ? fileName[11..] : fileName;
		var title = metadata.GetValueOrDefault("title");
		if (string.IsNullOrWhiteSpace(title))
		{
			throw new InvalidDataException($"{path} is missing a title.");
		}

		DateTimeOffset? publishedAt = null;
		var tags = (metadata.GetValueOrDefault("tags") ?? string.Empty)
			.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
		if (dated)
		{
			var summary = metadata.GetValueOrDefault("summary");
			var dateText = metadata.GetValueOrDefault("date");
			if (string.IsNullOrWhiteSpace(summary) ||
				!DateTimeOffset.TryParseExact(dateText, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var parsedDate) ||
				tags.Length == 0 || !fileName.StartsWith($"{parsedDate:yyyy-MM-dd}-", StringComparison.Ordinal))
			{
				throw new InvalidDataException($"{path} requires summary, tags, and an ISO publication date matching its filename.");
			}
			publishedAt = parsedDate;
		}

		return new ContentDocument(
			slug,
			title,
			metadata.GetValueOrDefault("summary") ?? string.Empty,
			publishedAt,
			tags,
			RenderMarkdown(markdown));
	}

	private static (Dictionary<string, string> Metadata, string Markdown) ParseFrontMatter(string text)
	{
		var metadata = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
		var normalised = text.Replace("\r\n", "\n", StringComparison.Ordinal);
		if (!normalised.StartsWith("---\n", StringComparison.Ordinal))
		{
			return (metadata, normalised);
		}

		var end = normalised.IndexOf("\n---\n", 4, StringComparison.Ordinal);
		if (end < 0)
		{
			throw new InvalidDataException("Front matter is not terminated.");
		}
		foreach (var line in normalised[4..end].Split('\n'))
		{
			var separator = line.IndexOf(':');
			if (separator <= 0)
			{
				throw new InvalidDataException($"Invalid front matter line: {line}");
			}
			metadata[line[..separator].Trim()] = line[(separator + 1)..].Trim();
		}
		return (metadata, normalised[(end + 5)..]);
	}

	private static string RenderInline(string value)
	{
		var encoded = WebUtility.HtmlEncode(value);
		encoded = CodeRegex().Replace(encoded, "<code>$1</code>");
		encoded = StrongRegex().Replace(encoded, "<strong>$1</strong>");
		encoded = LinkRegex().Replace(encoded, match =>
		{
			var href = WebUtility.HtmlDecode(match.Groups[2].Value);
			if (!IsSafeLinkTarget(href))
			{
				return match.Groups[1].Value;
			}
			return $"<a href=\"{WebUtility.HtmlEncode(href)}\">{match.Groups[1].Value}</a>";
		});
		return encoded;
	}

	private static bool IsSafeLinkTarget(string href)
	{
		if (string.IsNullOrWhiteSpace(href))
		{
			return false;
		}

		var canonical = href;
		for (var pass = 0; pass < 8; pass++)
		{
			var decoded = WebUtility.HtmlDecode(canonical);
			if (decoded.Equals(canonical, StringComparison.Ordinal))
			{
				break;
			}
			if (pass == 7)
			{
				return false;
			}
			canonical = decoded;
		}

		if (string.IsNullOrWhiteSpace(canonical) ||
			!canonical.Equals(canonical.Trim(), StringComparison.Ordinal) ||
			canonical.Any(char.IsControl))
		{
			return false;
		}

		canonical = canonical.Replace((char)0x5C, '/');
		if (canonical.StartsWith("//", StringComparison.Ordinal))
		{
			return false;
		}

		if (SchemeRegex().IsMatch(canonical))
		{
			if (!Uri.TryCreate(canonical, UriKind.Absolute, out var absoluteUri))
			{
				return false;
			}

			return absoluteUri.Scheme.Equals(Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase) ||
				absoluteUri.Scheme.Equals(Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase) ||
				absoluteUri.Scheme.Equals(Uri.UriSchemeMailto, StringComparison.OrdinalIgnoreCase);
		}

		return Uri.TryCreate(canonical, UriKind.Relative, out _);
	}
	[GeneratedRegex("^[a-z0-9][a-z0-9-]*$", RegexOptions.CultureInvariant)]
	private static partial Regex SafeSlugRegex();
	[GeneratedRegex("^\\d{4}-\\d{2}-\\d{2}-[a-z0-9][a-z0-9-]*\\.md$", RegexOptions.CultureInvariant)]
	private static partial Regex DatedContentFileRegex();
	[GeneratedRegex("^(#{1,6})\\s+(.+)$", RegexOptions.CultureInvariant)]
	private static partial Regex HeadingRegex();
	[GeneratedRegex("`([^`]+)`", RegexOptions.CultureInvariant)]
	private static partial Regex CodeRegex();
	[GeneratedRegex("\\*\\*([^*]+)\\*\\*", RegexOptions.CultureInvariant)]
	private static partial Regex StrongRegex();
	[GeneratedRegex("\\[([^]]+)]\\(([^)]+)\\)", RegexOptions.CultureInvariant)]
	private static partial Regex LinkRegex();
	[GeneratedRegex("^[A-Za-z][A-Za-z0-9+.-]*:", RegexOptions.CultureInvariant)]
	private static partial Regex SchemeRegex();
}

public sealed record ContentDocument(
	string Slug,
	string Title,
	string Summary,
	DateTimeOffset? PublishedAt,
	IReadOnlyList<string> Tags,
	string Html);
