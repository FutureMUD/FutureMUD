#nullable enable

using MudSharp.Accounts;
using MudSharp.Commands.Modules;
using MudSharp.Documentation;
using MudSharp.FutureProg.Functions;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text.Json;
using System.Threading;

namespace MudSharp.Documentation.Export;

public static class DocumentationCatalogueExporter
{
	private static readonly object InitialisationLock = new();
	private static bool _futureProgInitialised;

	public static DocumentationCatalogue CreateCatalogue(
		IFuturemud? gameworld,
		string sourceRevision,
		DateTimeOffset? generatedAtUtc = null)
	{
		EnsureFutureProgInitialised();
		var assembly = Assembly.GetExecutingAssembly();
		return new DocumentationCatalogue
		{
			EngineVersion = assembly.GetName().Version?.ToString(3) ?? "0.0.0",
			SourceRevision = sourceRevision,
			GeneratedAtUtc = generatedAtUtc ?? DateTimeOffset.UtcNow,
			Commands = ExportCommands(assembly),
			ProgFunctions = ExportFunctions(gameworld),
			ProgTypes = ExportTypes(),
			CollectionExtensions = ExportCollectionExtensions(),
			ItemComponents = ExportItemComponents()
		};
	}

	public static async Task ExportAsync(
		string outputPath,
		string sourceRevision,
		CancellationToken cancellationToken = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(outputPath);
		ArgumentException.ThrowIfNullOrWhiteSpace(sourceRevision);
		if (sourceRevision.Length != 40 || sourceRevision.Any(character => !Uri.IsHexDigit(character)))
		{
			throw new ArgumentException("The source revision must be a complete 40-character hexadecimal commit SHA.", nameof(sourceRevision));
		}

		using var gameworld = new Futuremud(null!);
		var catalogue = CreateCatalogue(gameworld, sourceRevision);
		var fullPath = Path.GetFullPath(outputPath);
		Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
		var temporaryPath = $"{fullPath}.{Guid.NewGuid():N}.tmp";
		try
		{
			await using (var stream = new FileStream(
				temporaryPath,
				FileMode.CreateNew,
				FileAccess.Write,
				FileShare.None,
				81920,
				FileOptions.Asynchronous | FileOptions.WriteThrough))
			{
				await JsonSerializer.SerializeAsync(stream, catalogue, JsonOptions, cancellationToken);
				await stream.FlushAsync(cancellationToken);
			}

			File.Move(temporaryPath, fullPath, true);
		}
		finally
		{
			if (File.Exists(temporaryPath))
			{
				File.Delete(temporaryPath);
			}
		}
	}

	public static void WriteLegacyHtml(IFuturemud gameworld, string outputDirectory, string sourceRevision = "development")
	{
		var catalogue = CreateCatalogue(gameworld, sourceRevision);
		Directory.CreateDirectory(outputDirectory);
		WriteLegacyFunctions(catalogue, Path.Combine(outputDirectory, "ProgFunctionsAlphabetically.html"), false);
		WriteLegacyFunctions(catalogue, Path.Combine(outputDirectory, "ProgFunctionsByCategory.html"), true);
		WriteLegacyTypes(catalogue, Path.Combine(outputDirectory, "ProgTypeHelps.html"));
		WriteLegacyCollections(catalogue, Path.Combine(outputDirectory, "ProgCollectionHelps.html"));
	}

	private static JsonSerializerOptions JsonOptions { get; } = new(JsonSerializerDefaults.Web)
	{
		WriteIndented = true
	};

	private static void EnsureFutureProgInitialised()
	{
		lock (InitialisationLock)
		{
			if (_futureProgInitialised || FutureProg.FutureProg.GetFunctionCompilerInformations().Any())
			{
				_futureProgInitialised = true;
				return;
			}

			FutureProg.FutureProg.Initialise();
			_futureProgInitialised = true;
		}
	}

	private static IReadOnlyList<CommandHelpDocument> ExportCommands(Assembly assembly)
	{
		return assembly.GetTypes()
			.Where(type => !type.IsAbstract && type.Namespace?.StartsWith("MudSharp.Commands.Modules", StringComparison.Ordinal) == true)
			.SelectMany(type => type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
				.Select(method => (Type: type, Method: method)))
			.Select(item => new
			{
				item.Type,
				PlayerCommand = item.Method.GetCustomAttribute<PlayerCommand>(),
				Help = item.Method.GetCustomAttribute<HelpInfo>(),
				Permission = item.Method.GetCustomAttribute<CommandPermission>(),
				Conditional = item.Method.GetCustomAttributes<ConditionalHelpInfo>().ToList()
			})
			.Where(item => item.PlayerCommand is not null && item.Help is not null)
			.Select(item =>
			{
				var permission = item.Permission?.PermissionLevel ?? PermissionLevel.Any;
				var module = item.Type.Name.EndsWith("Module", StringComparison.Ordinal)
					? item.Type.Name[..^"Module".Length]
					: item.Type.Name;
				return new CommandHelpDocument
				{
					Slug = CreateSlug($"{module}-{item.PlayerCommand!.Name}"),
					Name = item.PlayerCommand.Name,
					Module = module,
					PermissionLevel = permission.ToString(),
					Audience = GetCommandAudience(module, permission, item.Conditional.Count > 0),
					CommandWords = item.PlayerCommand.CommandWords
						.Select(word => word.ToLowerInvariant())
						.Distinct(StringComparer.Ordinal)
						.OrderBy(word => word, StringComparer.Ordinal)
						.ToList(),
					DefaultHelp = item.Help!.DefaultHelp ?? string.Empty,
					AdminHelp = item.Help.AdminHelp ?? item.Help.DefaultHelp ?? string.Empty,
					ConditionalHelp = item.Conditional
						.OrderBy(help => help.PredicateMethodName, StringComparer.Ordinal)
						.Select(help => new ConditionalCommandHelpDocument
						{
							Condition = help.PredicateMethodName,
							Help = help.HelpText ?? string.Empty
						})
						.ToList()
				};
			})
			.OrderBy(document => document.Name, StringComparer.OrdinalIgnoreCase)
			.ThenBy(document => document.Module, StringComparer.OrdinalIgnoreCase)
			.ThenBy(document => document.PermissionLevel, StringComparer.Ordinal)
			.ToList();
	}

	private static string GetCommandAudience(string module, PermissionLevel permission, bool conditional)
	{
		if (permission >= PermissionLevel.JuniorAdmin)
		{
			return module.Contains("Builder", StringComparison.OrdinalIgnoreCase) ? "builder" : "admin";
		}

		return conditional ? "conditional" : "player";
	}

	private static IReadOnlyList<ProgFunctionDocument> ExportFunctions(IFuturemud? gameworld)
	{
		return FutureProg.FutureProg.GetFunctionCompilerInformations()
			.GroupBy(info => info.FunctionName, StringComparer.OrdinalIgnoreCase)
			.Select(group => new ProgFunctionDocument
			{
				Slug = CreateSlug(group.Key),
				Name = group.Key,
				Category = group.Select(info => info.Category)
					.FirstOrDefault(category => !string.IsNullOrWhiteSpace(category)) ?? "Uncategorised",
				Overloads = group
					.OrderBy(info => string.Join("|", info.Parameters.Select(parameter => parameter.Describe())), StringComparer.Ordinal)
					.Select(info => ExportOverload(info, gameworld))
					.ToList()
			})
			.OrderBy(document => document.Name, StringComparer.OrdinalIgnoreCase)
			.ToList();
	}

	private static ProgFunctionOverloadDocument ExportOverload(FunctionCompilerInformation info, IFuturemud? gameworld)
	{
		var parameterTypes = info.Parameters.ToList();
		var parameterNames = info.ParameterNames?.ToList() ?? [];
		var parameterHelp = info.ParameterHelp?.ToList() ?? [];
		var parameters = parameterTypes.Select((type, index) => new ProgFunctionParameterDocument
		{
			Name = index < parameterNames.Count && !string.IsNullOrWhiteSpace(parameterNames[index])
				? parameterNames[index]
				: $"parameter{index + 1}",
			Type = type.Describe(),
			Help = index < parameterHelp.Count ? parameterHelp[index] ?? string.Empty : string.Empty
		}).ToList();
		var generalHelp = info.FunctionHelp ?? "This function has no general help information.";
		var combinedHelp = new StringBuilder(generalHelp);
		foreach (var parameter in parameters.Where(parameter => !string.IsNullOrWhiteSpace(parameter.Help)))
		{
			combinedHelp.AppendLine().Append(parameter.Name).Append(": ").Append(parameter.Help);
		}

		return new ProgFunctionOverloadDocument
		{
			Parameters = parameters,
			ReturnType = ResolveReturnType(info, gameworld),
			Contexts = info.AllowedContexts
				.Select(context => context.Describe())
				.OrderBy(context => context, StringComparer.Ordinal)
				.ToList(),
			GeneralHelp = generalHelp,
			Help = combinedHelp.ToString()
		};
	}

	private static string ResolveReturnType(FunctionCompilerInformation info, IFuturemud? gameworld)
	{
		if (info.ReturnType != ProgVariableTypes.Error)
		{
			return info.ReturnType.Describe();
		}

		try
		{
			var parameters = info.Parameters
				.Select(type => (IFunction)new DocumentationFunction(type))
				.ToList();
			return info.CompilerFunction(parameters, gameworld!).ReturnType.Describe();
		}
		catch
		{
			return "unknown";
		}
	}

	private static IReadOnlyList<ProgTypeDocument> ExportTypes()
	{
		return ProgVariable.DotReferenceCompileInfos
			.Select(entry => new ProgTypeDocument
			{
				Slug = CreateSlug(entry.Key.Describe()),
				Name = entry.Key.Describe(),
				Properties = entry.Value.PropertyTypeMap
					.OrderBy(property => property.Key, StringComparer.OrdinalIgnoreCase)
					.Select(property => new ProgTypePropertyDocument
					{
						Name = property.Key,
						Type = property.Value.Describe(),
						Help = entry.Value.PropertyHelpInfo.GetValueOrDefault(property.Key, string.Empty)
					})
					.ToList()
			})
			.OrderBy(document => document.Name, StringComparer.OrdinalIgnoreCase)
			.ToList();
	}

	private static IReadOnlyList<CollectionExtensionDocument> ExportCollectionExtensions()
	{
		return CollectionExtensionFunction.FunctionCompilerInformations
			.Select(info => new CollectionExtensionDocument
			{
				Slug = CreateSlug(info.FunctionName),
				Name = info.FunctionName,
				ReturnType = info.FunctionReturnInfo ?? string.Empty,
				Contexts = ["standard futureprog", "computer function", "computer program"],
				Help = info.FunctionHelp ?? string.Empty
			})
			.OrderBy(document => document.Name, StringComparer.OrdinalIgnoreCase)
			.ToList();
	}

	private static IReadOnlyList<ItemComponentHelpDocument> ExportItemComponents()
	{
		var manager = new GameItemComponentManager();
		return manager.TypeHelpInfo
			.Select(info => new ItemComponentHelpDocument
			{
				Slug = CreateSlug(info.Name),
				Name = info.Name,
				Blurb = info.Blurb ?? string.Empty,
				BuilderHelp = info.Help ?? string.Empty
			})
			.OrderBy(document => document.Name, StringComparer.OrdinalIgnoreCase)
			.ToList();
	}

	public static string CreateSlug(string value)
	{
		var builder = new StringBuilder();
		var pendingSeparator = false;
		foreach (var character in value.Trim().ToLowerInvariant())
		{
			if (char.IsLetterOrDigit(character))
			{
				if (pendingSeparator && builder.Length > 0)
				{
					builder.Append('-');
				}
				builder.Append(character);
				pendingSeparator = false;
				continue;
			}

			pendingSeparator = true;
		}
		return builder.ToString();
	}

	private static void WriteLegacyFunctions(DocumentationCatalogue catalogue, string path, bool groupByCategory)
	{
		using var writer = new StreamWriter(path, false, Encoding.UTF8);
		WriteLegacyHeader(writer, "FutureMUD Function Reference");
		if (groupByCategory)
		{
			foreach (var category in catalogue.ProgFunctions
				.GroupBy(function => function.Category)
				.OrderBy(group => group.Key, StringComparer.OrdinalIgnoreCase))
			{
				writer.WriteLine($"<details open><summary>{Encode(category.Key)}</summary>");
				foreach (var function in category.OrderBy(function => function.Name, StringComparer.OrdinalIgnoreCase))
				{
					WriteLegacyFunction(writer, function);
				}
				writer.WriteLine("</details>");
			}
		}
		else
		{
			foreach (var function in catalogue.ProgFunctions.OrderBy(function => function.Name, StringComparer.OrdinalIgnoreCase))
			{
				WriteLegacyFunction(writer, function);
			}
		}
		writer.WriteLine("</main></body></html>");
	}

	private static void WriteLegacyFunction(TextWriter writer, ProgFunctionDocument function)
	{
		writer.WriteLine($"<section><h2>{Encode(function.Name)}</h2><p>{Encode(function.Category)}</p>");
		foreach (var overload in function.Overloads)
		{
			writer.WriteLine($"<h3>{Encode(overload.ReturnType)} {Encode(function.Name)}({Encode(string.Join(", ", overload.Parameters.Select(parameter => $"{parameter.Type} {parameter.Name}")))})</h3>");
			writer.WriteLine($"<p>{Encode(string.IsNullOrWhiteSpace(overload.GeneralHelp) ? overload.Help : overload.GeneralHelp)}</p>");
			if (overload.Parameters.Count > 0)
			{
				writer.WriteLine("<table><thead><tr><th>Parameter</th><th>Type</th><th>Help</th></tr></thead><tbody>");
				foreach (var parameter in overload.Parameters)
				{
					writer.WriteLine($"<tr><td>{Encode(parameter.Name)}</td><td>{Encode(parameter.Type)}</td><td>{Encode(parameter.Help)}</td></tr>");
				}
				writer.WriteLine("</tbody></table>");
			}
		}
		writer.WriteLine("</section>");
	}

	private static void WriteLegacyTypes(DocumentationCatalogue catalogue, string path)
	{
		using var writer = new StreamWriter(path, false, Encoding.UTF8);
		WriteLegacyHeader(writer, "FutureMUD Type Help Reference");
		foreach (var type in catalogue.ProgTypes)
		{
			writer.WriteLine($"<details><summary>{Encode(type.Name)}</summary><table><thead><tr><th>Property</th><th>Type</th><th>Help</th></tr></thead><tbody>");
			foreach (var property in type.Properties)
			{
				writer.WriteLine($"<tr><td>{Encode(property.Name)}</td><td>{Encode(property.Type)}</td><td>{Encode(property.Help)}</td></tr>");
			}
			writer.WriteLine("</tbody></table></details>");
		}
		writer.WriteLine("</main></body></html>");
	}

	private static void WriteLegacyCollections(DocumentationCatalogue catalogue, string path)
	{
		using var writer = new StreamWriter(path, false, Encoding.UTF8);
		WriteLegacyHeader(writer, "FutureMUD Collection Extension Reference");
		foreach (var extension in catalogue.CollectionExtensions)
		{
			writer.WriteLine($"<details><summary>{Encode(extension.Name)}</summary><pre>{Encode(extension.Help)}</pre></details>");
		}
		writer.WriteLine("</main></body></html>");
	}

	private static void WriteLegacyHeader(TextWriter writer, string title)
	{
		writer.WriteLine("<!doctype html><html lang=\"en\"><head><meta charset=\"utf-8\"><meta name=\"viewport\" content=\"width=device-width,initial-scale=1\">");
		writer.WriteLine($"<title>{Encode(title)}</title><style>body{{font-family:system-ui;max-width:80rem;margin:auto;padding:1rem}}pre{{white-space:pre-wrap}}table{{border-collapse:collapse}}td,th{{border:1px solid;padding:.35rem}}</style></head><body><main>");
		writer.WriteLine($"<h1>{Encode(title)}</h1>");
	}

	private static string Encode(string value) => WebUtility.HtmlEncode(value);

	private sealed class DocumentationFunction : IFunction
	{
		public DocumentationFunction(ProgVariableTypes returnType)
		{
			ReturnType = returnType;
			Result = new NullVariable(returnType);
		}

		public IProgVariable Result { get; }
		public ProgVariableTypes ReturnType { get; }
		public string ErrorMessage => string.Empty;
		public StatementResult ExpectedResult => StatementResult.Normal;
		public StatementResult Execute(IVariableSpace variables) => StatementResult.Normal;
		public bool IsReturnOrContainsReturnOnAllBranches() => false;
	}
}
