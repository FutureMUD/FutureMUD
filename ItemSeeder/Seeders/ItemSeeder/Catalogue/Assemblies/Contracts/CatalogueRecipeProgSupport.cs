#nullable enable

using MudSharp.FutureProg;
using System.Collections.Generic;

namespace DatabaseSeeder.Seeders;

public sealed record CatalogueRecipeProgParameter(ProgVariableTypes Type, string Name);
public sealed record CatalogueRecipeProgSupport(CatalogueMetadata Metadata, string Name, string Category,
	string Subcategory, ProgVariableTypes ReturnType, string Comment, string Text,
	IReadOnlyList<CatalogueRecipeProgParameter> Parameters);
