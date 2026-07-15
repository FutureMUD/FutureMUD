#nullable enable

using System;
using System.Collections.Generic;

namespace MudSharp.Documentation;

/// <summary>
/// A database-independent snapshot of the documentation embedded in a FutureMUD engine build.
/// </summary>
public sealed class DocumentationCatalogue
{
	public const int CurrentSchemaVersion = 1;

	public int SchemaVersion { get; init; } = CurrentSchemaVersion;
	public string EngineVersion { get; init; } = string.Empty;
	public string SourceRevision { get; init; } = string.Empty;
	public DateTimeOffset GeneratedAtUtc { get; init; }
	public IReadOnlyList<CommandHelpDocument> Commands { get; init; } = [];
	public IReadOnlyList<ProgFunctionDocument> ProgFunctions { get; init; } = [];
	public IReadOnlyList<ProgTypeDocument> ProgTypes { get; init; } = [];
	public IReadOnlyList<CollectionExtensionDocument> CollectionExtensions { get; init; } = [];
	public IReadOnlyList<ItemComponentHelpDocument> ItemComponents { get; init; } = [];
}

public sealed class CommandHelpDocument
{
	public string Slug { get; init; } = string.Empty;
	public string Name { get; init; } = string.Empty;
	public string Module { get; init; } = string.Empty;
	public string PermissionLevel { get; init; } = string.Empty;
	public string Audience { get; init; } = string.Empty;
	public IReadOnlyList<string> CommandWords { get; init; } = [];
	public string DefaultHelp { get; init; } = string.Empty;
	public string AdminHelp { get; init; } = string.Empty;
	public IReadOnlyList<ConditionalCommandHelpDocument> ConditionalHelp { get; init; } = [];
}

public sealed class ConditionalCommandHelpDocument
{
	public string Condition { get; init; } = string.Empty;
	public string Help { get; init; } = string.Empty;
}

public sealed class ProgFunctionDocument
{
	public string Slug { get; init; } = string.Empty;
	public string Name { get; init; } = string.Empty;
	public string Category { get; init; } = string.Empty;
	public IReadOnlyList<ProgFunctionOverloadDocument> Overloads { get; init; } = [];
}

public sealed class ProgFunctionOverloadDocument
{
	public IReadOnlyList<ProgFunctionParameterDocument> Parameters { get; init; } = [];
	public string ReturnType { get; init; } = string.Empty;
	public IReadOnlyList<string> Contexts { get; init; } = [];
	public string Help { get; init; } = string.Empty;
}

public sealed class ProgFunctionParameterDocument
{
	public string Name { get; init; } = string.Empty;
	public string Type { get; init; } = string.Empty;
}

public sealed class ProgTypeDocument
{
	public string Slug { get; init; } = string.Empty;
	public string Name { get; init; } = string.Empty;
	public IReadOnlyList<ProgTypePropertyDocument> Properties { get; init; } = [];
}

public sealed class ProgTypePropertyDocument
{
	public string Name { get; init; } = string.Empty;
	public string Type { get; init; } = string.Empty;
	public string Help { get; init; } = string.Empty;
}

public sealed class CollectionExtensionDocument
{
	public string Slug { get; init; } = string.Empty;
	public string Name { get; init; } = string.Empty;
	public string ReturnType { get; init; } = string.Empty;
	public IReadOnlyList<string> Contexts { get; init; } = [];
	public string Help { get; init; } = string.Empty;
}

public sealed class ItemComponentHelpDocument
{
	public string Slug { get; init; } = string.Empty;
	public string Name { get; init; } = string.Empty;
	public string Blurb { get; init; } = string.Empty;
	public string BuilderHelp { get; init; } = string.Empty;
}
