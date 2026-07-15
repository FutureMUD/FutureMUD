#nullable enable

using FutureMUD.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MudSharp.Documentation;
using System.Net;

namespace FutureMUD.Web.Pages;

public sealed class DocsModel : PageModel
{
	private readonly DocumentationService _documentation;
	public DocsModel(DocumentationService documentation) => _documentation = documentation;

	public string Title { get; private set; } = string.Empty;
	public string CollectionUrl { get; private set; } = string.Empty;
	public string EngineVersion { get; private set; } = string.Empty;
	public string SourceRevision { get; private set; } = string.Empty;
	public string Query { get; private set; } = string.Empty;
	public string Audience { get; private set; } = string.Empty;
	public string Category { get; private set; } = string.Empty;
	public IReadOnlyList<string> Audiences { get; private set; } = [];
	public IReadOnlyList<string> Categories { get; private set; } = [];
	public IReadOnlyList<DocumentationEntry> Entries { get; private set; } = [];
	public DocumentationEntry? Detail { get; private set; }

	public IActionResult OnGet(string section, string? collection, string? slug, string? q, string? audience, string? category)
	{
		if (section == "commands" && collection is not null && slug is null)
		{
			slug = collection;
			collection = null;
		}

		var catalogue = _documentation.GetCatalogue();
		EngineVersion = catalogue.EngineVersion;
		SourceRevision = catalogue.SourceRevision;
		var all = SelectEntries(catalogue, section, collection);
		if (all is null)
		{
			return NotFound();
		}

		if (!string.IsNullOrWhiteSpace(slug))
		{
			Detail = all.FirstOrDefault(entry => entry.Slug.Equals(slug, StringComparison.Ordinal));
			return Detail is null ? NotFound() : Page();
		}

		Query = q?.Trim() ?? string.Empty;
		Audience = audience?.Trim() ?? string.Empty;
		Category = category?.Trim() ?? string.Empty;
		Audiences = all.Select(entry => entry.Audience).Where(value => !string.IsNullOrWhiteSpace(value)).Distinct(StringComparer.OrdinalIgnoreCase).Order().ToList();
		Categories = all.Select(entry => entry.Category).Where(value => !string.IsNullOrWhiteSpace(value)).Distinct(StringComparer.OrdinalIgnoreCase).Order().ToList();
		Entries = all
			.Where(entry => string.IsNullOrWhiteSpace(Query) || $"{entry.Title} {entry.Summary} {entry.SearchText}".Contains(Query, StringComparison.OrdinalIgnoreCase))
			.Where(entry => string.IsNullOrWhiteSpace(Audience) || entry.Audience.Equals(Audience, StringComparison.OrdinalIgnoreCase))
			.Where(entry => string.IsNullOrWhiteSpace(Category) || entry.Category.Equals(Category, StringComparison.OrdinalIgnoreCase))
			.OrderBy(entry => entry.Title, StringComparer.OrdinalIgnoreCase)
			.ToList();
		return Page();
	}

	private List<DocumentationEntry>? SelectEntries(DocumentationCatalogue catalogue, string section, string? collection)
	{
		if (section == "commands" && collection is null)
		{
			Title = "Command reference";
			CollectionUrl = "/docs/commands";
			return catalogue.Commands.Select(command => new DocumentationEntry(
				command.Slug,
				command.Name,
				command.Module,
				command.Audience,
				string.Join(", ", command.CommandWords),
				command.DefaultHelp,
				TextMarkupService.ToSafeHtml(BuildCommandHelp(command)))).ToList();
		}

		if (section == "futureprog" && collection == "functions")
		{
			Title = "FutureProg functions";
			CollectionUrl = "/docs/futureprog/functions";
			return catalogue.ProgFunctions.Select(function => new DocumentationEntry(
				function.Slug,
				function.Name,
				function.Category,
				string.Join(", ", function.Overloads.SelectMany(overload => overload.Contexts).Distinct()),
				$"{function.Overloads.Count} overload(s)",
				string.Join('\n', function.Overloads.Select(overload => overload.Help)),
				TextMarkupService.ToSafeHtml(BuildFunctionHelp(function)))).ToList();
		}

		if (section == "futureprog" && collection == "types")
		{
			Title = "FutureProg types";
			CollectionUrl = "/docs/futureprog/types";
			return catalogue.ProgTypes.Select(type => new DocumentationEntry(
				type.Slug,
				type.Name,
				"type",
				string.Empty,
				$"{type.Properties.Count} dot-reference properties",
				string.Join('\n', type.Properties.Select(property => property.Help)),
				TextMarkupService.ToSafeHtml(BuildTypeHelp(type)))).ToList();
		}

		if (section == "futureprog" && collection == "collections")
		{
			Title = "Collection extensions";
			CollectionUrl = "/docs/futureprog/collections";
			return catalogue.CollectionExtensions.Select(extension => new DocumentationEntry(
				extension.Slug,
				extension.Name,
				"collection",
				string.Join(", ", extension.Contexts),
				extension.ReturnType,
				extension.Help,
				TextMarkupService.ToSafeHtml($"Returns: {extension.ReturnType}\n\n{extension.Help}"))).ToList();
		}

		if (section == "items" && collection == "components")
		{
			Title = "Item components";
			CollectionUrl = "/docs/items/components";
			return catalogue.ItemComponents.Select(component => new DocumentationEntry(
				component.Slug,
				component.Name,
				"item component",
				"builder",
				component.Blurb,
				component.BuilderHelp,
				TextMarkupService.ToSafeHtml($"{component.Blurb}\n\n{component.BuilderHelp}"))).ToList();
		}

		return null;
	}

	private static string BuildCommandHelp(CommandHelpDocument command) =>
		$"Command words: {string.Join(", ", command.CommandWords)}\nPermission: {command.PermissionLevel}\n\n{command.DefaultHelp}" +
		(command.AdminHelp == command.DefaultHelp ? string.Empty : $"\n\nAdministrator variant:\n{command.AdminHelp}") +
		string.Concat(command.ConditionalHelp.Select(help => $"\n\nConditional variant ({help.Condition}):\n{help.Help}"));

	private static string BuildFunctionHelp(ProgFunctionDocument function) => string.Join("\n\n", function.Overloads.Select(overload =>
		$"{overload.ReturnType} {function.Name}({string.Join(", ", overload.Parameters.Select(parameter => $"{parameter.Type} {parameter.Name}"))})\nContexts: {string.Join(", ", overload.Contexts)}\n{overload.Help}"));

	private static string BuildTypeHelp(ProgTypeDocument type) => string.Join("\n\n", type.Properties.Select(property => $"{property.Name} → {property.Type}\n{property.Help}"));
}

public sealed record DocumentationEntry(
	string Slug,
	string Title,
	string Category,
	string Audience,
	string Summary,
	string SearchText,
	string SafeHtml);
