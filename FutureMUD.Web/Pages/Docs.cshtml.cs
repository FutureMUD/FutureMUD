#nullable enable

using FutureMUD.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;
using MudSharp.Documentation;

namespace FutureMUD.Web.Pages;

public sealed class DocsModel : PageModel
{
	private static readonly object DocumentationCacheLock = new();
	private const int PageSize = 100;
	private const int MaximumPage = 500;
	private const int MaximumQueryLength = 200;
	private const int MaximumFilterLength = 100;
	private const int MaximumFilterOptions = 256;
	private readonly DocumentationService _documentation;
	private readonly IMemoryCache _memoryCache;

	public DocsModel(DocumentationService documentation, IMemoryCache memoryCache)
	{
		_documentation = documentation;
		_memoryCache = memoryCache;
	}

	public string Title { get; private set; } = string.Empty;
	public string CollectionUrl { get; private set; } = string.Empty;
	public string SectionCssClass { get; private set; } = string.Empty;
	public string SectionDescription { get; private set; } = string.Empty;
	public string EngineVersion { get; private set; } = string.Empty;
	public string SourceRevision { get; private set; } = string.Empty;
	public string Query { get; private set; } = string.Empty;
	public string Audience { get; private set; } = string.Empty;
	public string Category { get; private set; } = string.Empty;
	public IReadOnlyList<string> Audiences { get; private set; } = [];
	public IReadOnlyList<string> Categories { get; private set; } = [];
	public IReadOnlyList<DocumentationEntry> Entries { get; private set; } = [];
	public DocumentationEntry? Detail { get; private set; }
	public CommandDocumentationDetail? CommandDetail { get; private set; }
	public ProgFunctionDocumentationDetail? FunctionDetail { get; private set; }
	public ProgTypeDocumentationDetail? TypeDetail { get; private set; }
	public CollectionExtensionDocumentationDetail? CollectionDetail { get; private set; }
	public ItemComponentDocumentationDetail? ItemComponentDetail { get; private set; }
	public int TotalResults { get; private set; }
	public int CurrentPage { get; private set; } = 1;
	public int TotalPages { get; private set; } = 1;
	public int FirstResult => TotalResults == 0 ? 0 : (CurrentPage - 1) * PageSize + 1;
	public int LastResult => Math.Min(TotalResults, CurrentPage * PageSize);
	public bool HasPreviousPage => CurrentPage > 1;
	public bool HasNextPage => CurrentPage < TotalPages;

	public IActionResult OnGet(
		string section,
		string? collection,
		string? slug,
		string? q,
		string? audience,
		string? category,
		int pageNumber = 1)
	{
		if (section == "commands" && collection is not null && slug is null)
		{
			slug = collection;
			collection = null;
		}

		var catalogue = _documentation.GetCatalogue();
		EngineVersion = catalogue.EngineVersion;
		SourceRevision = catalogue.SourceRevision;
		if (!ConfigureSection(section, collection))
		{
			return NotFound();
		}
		var projection = GetCachedProjection(catalogue, section, collection);

		if (!string.IsNullOrWhiteSpace(slug))
		{
			if (!projection.EntriesBySlug.TryGetValue(slug, out var detail))
			{
				return NotFound();
			}
			Detail = detail;
			ConfigureDetail(catalogue, section, collection, slug);
			return Page();
		}

		Query = NormaliseInput(q, MaximumQueryLength);
		Audience = NormaliseInput(audience, MaximumFilterLength);
		Category = NormaliseInput(category, MaximumFilterLength);
		Audiences = projection.Audiences;
		Categories = projection.Categories;

		var matching = projection.Entries
			.Where(entry => Query.Length == 0 ||
				entry.Title.Contains(Query, StringComparison.OrdinalIgnoreCase) ||
				entry.Summary.Contains(Query, StringComparison.OrdinalIgnoreCase) ||
				entry.SearchText.Contains(Query, StringComparison.OrdinalIgnoreCase))
			.Where(entry => Audience.Length == 0 || entry.Audience.Equals(Audience, StringComparison.OrdinalIgnoreCase))
			.Where(entry => Category.Length == 0 || entry.Category.Equals(Category, StringComparison.OrdinalIgnoreCase));
		TotalResults = matching.Count();
		TotalPages = Math.Clamp((TotalResults + PageSize - 1) / PageSize, 1, MaximumPage);
		CurrentPage = Math.Clamp(pageNumber, 1, TotalPages);
		Entries = matching
			.Skip((CurrentPage - 1) * PageSize)
			.Take(PageSize)
			.ToList();
		return Page();
	}

	public string GetPageUrl(int page) =>
		$"{CollectionUrl}?q={Uri.EscapeDataString(Query)}&audience={Uri.EscapeDataString(Audience)}&category={Uri.EscapeDataString(Category)}&pageNumber={page}";

	private static string NormaliseInput(string? value, int maximumLength)
	{
		var trimmed = value?.Trim() ?? string.Empty;
		return trimmed.Length <= maximumLength ? trimmed : trimmed[..maximumLength];
	}

	private bool ConfigureSection(string section, string? collection)
	{
		(Title, CollectionUrl, SectionCssClass, SectionDescription) = (section, collection) switch
		{
			("commands", null) => ("Command reference", "/docs/commands", "commands", "Player, builder and staff command help generated from the engine."),
			("futureprog", "functions") => ("FutureProg functions", "/docs/futureprog/functions", "functions", "Built-in FutureProg functions with overloads, types, contexts and parameter help."),
			("futureprog", "types") => ("FutureProg types", "/docs/futureprog/types", "types", "FutureProg variable types and their available dot-reference properties."),
			("futureprog", "collections") => ("Collection extensions", "/docs/futureprog/collections", "collections", "Query, filter, aggregate and transform functions for FutureProg collections."),
			("items", "components") => ("Item components", "/docs/items/components", "components", "Source-backed item-component blurbs and builder help."),
			_ => (string.Empty, string.Empty, string.Empty, string.Empty)
		};
		return Title.Length > 0;
	}

	private DocumentationProjection GetCachedProjection(
		DocumentationCatalogue catalogue,
		string section,
		string? collection)
	{
		var key = new DocumentationCacheKey(catalogue, section, collection ?? string.Empty);
		if (_memoryCache.TryGetValue(key, out DocumentationProjection? projection) && projection is not null)
		{
			return projection;
		}

		lock (DocumentationCacheLock)
		{
			if (_memoryCache.TryGetValue(key, out projection) && projection is not null)
			{
				return projection;
			}

			var entries = SelectEntries(catalogue, section, collection)
				?? throw new InvalidOperationException("The configured documentation section is not supported.");
			var entriesBySlug = new Dictionary<string, DocumentationEntry>(StringComparer.Ordinal);
			foreach (var entry in entries)
			{
				entriesBySlug.TryAdd(entry.Slug, entry);
			}
			entries.Sort((left, right) =>
			{
				var titleComparison = StringComparer.OrdinalIgnoreCase.Compare(left.Title, right.Title);
				return titleComparison != 0
					? titleComparison
					: StringComparer.Ordinal.Compare(left.Slug, right.Slug);
			});
			projection = new DocumentationProjection(
				entries,
				entriesBySlug,
				entries
					.Select(entry => entry.Audience)
					.Where(value => !string.IsNullOrWhiteSpace(value))
					.Distinct(StringComparer.OrdinalIgnoreCase)
					.OrderBy(value => value, StringComparer.OrdinalIgnoreCase)
					.Take(MaximumFilterOptions)
					.ToList(),
				entries
					.Select(entry => entry.Category)
					.Where(value => !string.IsNullOrWhiteSpace(value))
					.Distinct(StringComparer.OrdinalIgnoreCase)
					.OrderBy(value => value, StringComparer.OrdinalIgnoreCase)
					.Take(MaximumFilterOptions)
					.ToList());
			_memoryCache.Set(
				key,
				projection,
				new MemoryCacheEntryOptions()
					.SetSize(EstimateCacheSize(entries))
					.SetSlidingExpiration(TimeSpan.FromMinutes(30))
					.SetAbsoluteExpiration(TimeSpan.FromHours(6)));
			return projection;
		}
	}

	private static long EstimateCacheSize(IReadOnlyList<DocumentationEntry> entries)
	{
		long characters = 0;
		foreach (var entry in entries)
		{
			characters += entry.Slug.Length + entry.Title.Length + entry.Category.Length +
				entry.Audience.Length + entry.Summary.Length + entry.SummaryHtml.Length + entry.SearchText.Length;
		}
		return Math.Max(1, entries.Count + (characters + 1023) / 1024);
	}

	private List<DocumentationEntry>? SelectEntries(DocumentationCatalogue catalogue, string section, string? collection)
	{
		if (section == "commands" && collection is null)
		{
			return catalogue.Commands.Select(command => new DocumentationEntry(
				command.Slug,
				command.Name,
				command.Module,
				command.Audience,
				string.Join(", ", command.CommandWords),
				BuildCommandSearchText(command))).ToList();
		}

		if (section == "futureprog" && collection == "functions")
		{
			return catalogue.ProgFunctions.Select(function => new DocumentationEntry(
				function.Slug,
				function.Name,
				function.Category,
				string.Join(", ", function.Overloads.SelectMany(overload => overload.Contexts).Distinct()),
				$"{function.Overloads.Count} overload(s)",
				BuildFunctionSearchText(function))).ToList();
		}

		if (section == "futureprog" && collection == "types")
		{
			return catalogue.ProgTypes.Select(type => new DocumentationEntry(
				type.Slug,
				type.Name,
				"type",
				string.Empty,
				$"{type.Properties.Count} dot-reference properties",
				string.Join('\n', type.Properties.Select(property => $"{property.Name} {property.Type} {property.Help}")))).ToList();
		}

		if (section == "futureprog" && collection == "collections")
		{
			return catalogue.CollectionExtensions.Select(extension => new DocumentationEntry(
				extension.Slug,
				extension.Name,
				"collection",
				string.Join(", ", extension.Contexts),
				$"Returns {extension.ReturnType}",
				$"{extension.Name} {extension.ReturnType} {string.Join(' ', extension.Contexts)} {extension.Help}")).ToList();
		}

		if (section == "items" && collection == "components")
		{
			return catalogue.ItemComponents.Select(component => new DocumentationEntry(
				component.Slug,
				component.Name,
				"item component",
				"builder",
				component.Blurb,
				$"{component.Blurb}\n{component.BuilderHelp}")).ToList();
		}

		return null;
	}

	private void ConfigureDetail(
		DocumentationCatalogue catalogue,
		string section,
		string? collection,
		string slug)
	{
		if (section == "commands" && collection is null)
		{
			var command = catalogue.Commands.First(command => command.Slug == slug);
			CommandDetail = new CommandDocumentationDetail(
				command.CommandWords,
				command.PermissionLevel,
				TextMarkupService.ToSafeHtml(command.DefaultHelp),
				command.AdminHelp == command.DefaultHelp
					? null
					: TextMarkupService.ToSafeHtml(command.AdminHelp),
				command.ConditionalHelp
					.Select(help => new ConditionalCommandHelpDetail(
						help.Condition,
						TextMarkupService.ToSafeHtml(help.Help)))
					.ToList());
			return;
		}

		if (section == "futureprog" && collection == "functions")
		{
			var function = catalogue.ProgFunctions.First(function => function.Slug == slug);
			FunctionDetail = new ProgFunctionDocumentationDetail(function.Overloads
				.Select((overload, index) => new ProgFunctionOverloadDetail(
					index + 1,
					overload.ReturnType,
					overload.Contexts,
					TextMarkupService.ToSafeHtml(
						string.IsNullOrWhiteSpace(overload.GeneralHelp)
							? overload.Help
							: overload.GeneralHelp),
					overload.Parameters
						.Select(parameter => new ProgFunctionParameterDetail(
							parameter.Name,
							parameter.Type,
							TextMarkupService.ToSafeHtml(parameter.Help)))
						.ToList()))
				.ToList());
			return;
		}

		if (section == "futureprog" && collection == "types")
		{
			var type = catalogue.ProgTypes.First(type => type.Slug == slug);
			TypeDetail = new ProgTypeDocumentationDetail(type.Properties
				.Select(property => new ProgTypePropertyDetail(
					property.Name,
					property.Type,
					TextMarkupService.ToSafeHtml(property.Help)))
				.ToList());
			return;
		}

		if (section == "futureprog" && collection == "collections")
		{
			var extension = catalogue.CollectionExtensions.First(extension => extension.Slug == slug);
			CollectionDetail = new CollectionExtensionDocumentationDetail(
				extension.ReturnType,
				extension.Contexts,
				TextMarkupService.ToSafeHtml(extension.Help));
			return;
		}

		if (section == "items" && collection == "components")
		{
			var component = catalogue.ItemComponents.First(component => component.Slug == slug);
			ItemComponentDetail = new ItemComponentDocumentationDetail(
				TextMarkupService.ToSafeHtml(component.Blurb),
				TextMarkupService.ToSafeHtml(component.BuilderHelp));
		}
	}

	private static string BuildCommandSearchText(CommandHelpDocument command) =>
		$"{command.DefaultHelp}\n{command.AdminHelp}\n" +
		string.Join('\n', command.ConditionalHelp.Select(help => $"{help.Condition} {help.Help}"));

	private static string BuildFunctionSearchText(ProgFunctionDocument function) =>
		string.Join('\n', function.Overloads.Select(overload =>
			$"{overload.ReturnType} {function.Name} " +
			$"{string.Join(' ', overload.Contexts)} " +
			$"{overload.GeneralHelp} {overload.Help} " +
			string.Join(' ', overload.Parameters.Select(parameter =>
				$"{parameter.Type} {parameter.Name} {parameter.Help}"))));
}

file sealed record DocumentationCacheKey(
	DocumentationCatalogue Catalogue,
	string Section,
	string Collection);

internal sealed record DocumentationProjection(
	IReadOnlyList<DocumentationEntry> Entries,
	IReadOnlyDictionary<string, DocumentationEntry> EntriesBySlug,
	IReadOnlyList<string> Audiences,
	IReadOnlyList<string> Categories);

public sealed record DocumentationEntry(
	string Slug,
	string Title,
	string Category,
	string Audience,
	string Summary,
	string SearchText)
{
	public string SummaryHtml { get; } = TextMarkupService.ToSafeHtml(Summary);
}

public sealed record CommandDocumentationDetail(
	IReadOnlyList<string> CommandWords,
	string PermissionLevel,
	string DefaultHelpHtml,
	string? AdminHelpHtml,
	IReadOnlyList<ConditionalCommandHelpDetail> ConditionalHelp);

public sealed record ConditionalCommandHelpDetail(
	string Condition,
	string HelpHtml);

public sealed record ProgFunctionDocumentationDetail(
	IReadOnlyList<ProgFunctionOverloadDetail> Overloads);

public sealed record ProgFunctionOverloadDetail(
	int Number,
	string ReturnType,
	IReadOnlyList<string> Contexts,
	string HelpHtml,
	IReadOnlyList<ProgFunctionParameterDetail> Parameters);

public sealed record ProgFunctionParameterDetail(
	string Name,
	string Type,
	string HelpHtml);

public sealed record ProgTypeDocumentationDetail(
	IReadOnlyList<ProgTypePropertyDetail> Properties);

public sealed record ProgTypePropertyDetail(
	string Name,
	string Type,
	string HelpHtml);

public sealed record CollectionExtensionDocumentationDetail(
	string ReturnType,
	IReadOnlyList<string> Contexts,
	string HelpHtml);

public sealed record ItemComponentDocumentationDetail(
	string BlurbHtml,
	string BuilderHelpHtml);
