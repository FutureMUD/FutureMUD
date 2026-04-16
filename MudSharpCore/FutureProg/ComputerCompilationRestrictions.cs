#nullable enable

using MudSharp.Framework;
using MudSharp.FutureProg.Functions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.FutureProg;

internal static class ComputerCompilationRestrictions
{
	private static readonly HashSet<string> DisallowedFunctionCategories = new(StringComparer.InvariantCultureIgnoreCase)
	{
		"AI Storyteller",
		"Arena",
		"Character",
		"Characteristics",
		"Characters",
		"Chargen",
		"Clans",
		"Combat",
		"Crime",
		"Currency",
		"Disfigurements",
		"Echoes",
		"Effects",
		"Hooks",
		"Items",
		"Lookup",
		"Manipulation",
		"NPCs",
		"Outfits",
		"Perception",
		"Register",
		"Rooms",
		"Tags"
	};

	private static readonly HashSet<ProgVariableTypes> AllowedComputerBaseTypes =
	[
		ProgVariableTypes.Boolean,
		ProgVariableTypes.Number,
		ProgVariableTypes.Text,
		ProgVariableTypes.MudDateTime,
		ProgVariableTypes.TimeSpan
	];

	public static bool TryValidateTypeForContext(ProgVariableTypes type, FutureProgCompilationContext context,
		out string errorMessage)
	{
		if (IsTypeAllowedInContext(type, context))
		{
			errorMessage = string.Empty;
			return true;
		}

		errorMessage = $"Type {type.Describe()} is not supported in {context.Describe()} compilation.";
		return false;
	}

	public static bool IsTypeAllowedInContext(ProgVariableTypes type, FutureProgCompilationContext context)
	{
		if (!context.IsComputerContext())
		{
			return type != ProgVariableTypes.Error;
		}

		var workingType = type.WithoutLiteral();
		if (workingType == ProgVariableTypes.Void)
		{
			return true;
		}

		var hasCollection = workingType.HasFlag(ProgVariableTypes.Collection);
		var hasDictionary = workingType.HasFlag(ProgVariableTypes.Dictionary);
		var hasCollectionDictionary = workingType.HasFlag(ProgVariableTypes.CollectionDictionary);
		if ((hasCollection ? 1 : 0) + (hasDictionary ? 1 : 0) + (hasCollectionDictionary ? 1 : 0) > 1)
		{
			return false;
		}

		return AllowedComputerBaseTypes.Contains(workingType.WithoutContainerModifiers());
	}

	public static bool TryValidateBuiltInFunction(FunctionCompilerInformation compiler, IList<IFunction> parameters,
		FutureProgCompilationContext context, out string errorMessage)
	{
		if (!compiler.SupportsContext(context))
		{
			errorMessage = $"Built in function \"{compiler.FunctionName}\" is not available in {context.Describe()} compilation.";
			return false;
		}

		if (!context.IsComputerContext())
		{
			errorMessage = string.Empty;
			return true;
		}

		if (DisallowedFunctionCategories.Contains(compiler.Category))
		{
			errorMessage = $"Built in function \"{compiler.FunctionName}\" is not available in {context.Describe()} compilation.";
			return false;
		}

		if (compiler.FunctionName.EqualTo("null"))
		{
			if (parameters.FirstOrDefault() is ConstantFunction { Result.GetObject: string nullTypeName } &&
			    ProgVariableTypes.TryParse(nullTypeName, out var nullType) &&
			    IsTypeAllowedInContext(nullType, context))
			{
				errorMessage = string.Empty;
				return true;
			}

			errorMessage = $"Built in function \"{compiler.FunctionName}\" is not available in {context.Describe()} compilation.";
			return false;
		}

		if (!IsTypeAllowedInContext(compiler.ReturnType, context))
		{
			errorMessage = $"Built in function \"{compiler.FunctionName}\" returns {compiler.ReturnType.Describe()}, which is not supported in {context.Describe()} compilation.";
			return false;
		}

		if (parameters.Any(x => !IsTypeAllowedInContext(x.ReturnType, context)))
		{
			errorMessage = $"Built in function \"{compiler.FunctionName}\" uses parameter types that are not supported in {context.Describe()} compilation.";
			return false;
		}

		errorMessage = string.Empty;
		return true;
	}
}
