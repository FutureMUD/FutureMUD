using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Functions;
using MudSharp.FutureProg.Variables;

namespace MudSharp_Unit_Tests;

[TestClass]
public class FutureProgFunctionDocumentationTests
{
	[TestMethod]
	public void RegisterFunctionCompiler_AllBuiltInFunctionTypes_ExposeStaticRegistration()
	{
		var missing = Futuremud.GetAllTypes()
			.Where(x => x.IsSubclassOf(typeof(BuiltInFunction)) && !x.IsAbstract)
			.Where(x => x.GetMethod("RegisterFunctionCompiler", BindingFlags.Public | BindingFlags.Static) is null)
			.Select(x => x.FullName)
			.OrderBy(x => x)
			.ToList();

		Assert.AreEqual(
			0,
			missing.Count,
			$"Built-in FutureProg functions missing public static RegisterFunctionCompiler():{Environment.NewLine}{string.Join(Environment.NewLine, missing)}"
		);
	}

	[TestMethod]
	public void BuiltInFunctionMetadata_AllRegisteredFunctions_AreFullyDocumented()
	{
		FutureProgTestBootstrap.EnsureInitialised();

		var failures = FutureProg.GetFunctionCompilerInformations()
			.SelectMany(ValidateFunction)
			.ToList();

		Assert.AreEqual(
			0,
			failures.Count,
			$"Built-in FutureProg function documentation gaps:{Environment.NewLine}{string.Join(Environment.NewLine, failures.Take(100))}"
		);
	}

	[TestMethod]
	public void BuiltInFunctionMetadata_DeclaredReturnTypes_MatchCompiledFunctions()
	{
		FutureProgTestBootstrap.EnsureInitialised();

		var failures = FutureProg.GetFunctionCompilerInformations()
			.Select(ValidateDeclaredReturnType)
			.Where(x => !string.IsNullOrWhiteSpace(x))
			.ToList();

		Assert.AreEqual(
			0,
			failures.Count,
			$"Built-in FutureProg function return metadata mismatches:{Environment.NewLine}{string.Join(Environment.NewLine, failures.Take(100))}"
		);
	}

	private static IEnumerable<string> ValidateFunction(FunctionCompilerInformation function)
	{
		var signature = $"{function.FunctionName}({string.Join(", ", function.Parameters.Select(x => x.Describe()))})";
		var parameters = function.Parameters.ToList();
		var parameterNames = function.ParameterNames?.ToList();
		var parameterHelp = function.ParameterHelp?.ToList();

		if (string.IsNullOrWhiteSpace(function.FunctionName))
		{
			yield return $"{signature}: missing function name";
		}

		if (string.IsNullOrWhiteSpace(function.FunctionHelp))
		{
			yield return $"{signature}: missing function help";
		}

		if (string.IsNullOrWhiteSpace(function.Category) || function.Category.EqualTo("Uncategorised"))
		{
			yield return $"{signature}: missing category";
		}

		if (function.ReturnType == ProgVariableTypes.Error)
		{
			yield return $"{signature}: missing return type";
		}

		if (parameterNames is null || parameterNames.Count != parameters.Count)
		{
			yield return $"{signature}: parameter name count does not match parameter count";
		}
		else
		{
			foreach (var name in parameterNames.Where(string.IsNullOrWhiteSpace))
			{
				yield return $"{signature}: blank parameter name";
			}

			foreach (var duplicate in parameterNames.GroupBy(x => x, StringComparer.InvariantCultureIgnoreCase).Where(x => x.Count() > 1))
			{
				yield return $"{signature}: duplicate parameter name {duplicate.Key}";
			}
		}

		if (parameterHelp is null || parameterHelp.Count != parameters.Count)
		{
			yield return $"{signature}: parameter help count does not match parameter count";
		}
		else
		{
			foreach (var help in parameterHelp.Where(string.IsNullOrWhiteSpace))
			{
				yield return $"{signature}: blank parameter help";
			}
		}
	}

	private static string ValidateDeclaredReturnType(FunctionCompilerInformation function)
	{
		var signature = $"{function.FunctionName}({string.Join(", ", function.Parameters.Select(x => x.Describe()))})";
		if (function.FunctionName.EqualTo("getregister"))
		{
			return null;
		}

		if (function.ReturnType == ProgVariableTypes.Anything)
		{
			return null;
		}

		try
		{
			var parameters = function.Parameters
				.Select(x => new ReturnTypeOnlyFunction(x))
				.Cast<IFunction>()
				.ToList();
			var compiled = function.CompilerFunction(parameters, FutureProgTestBootstrap.Gameworld);
			if (!compiled.ReturnType.CompatibleWith(function.ReturnType) &&
			    !function.ReturnType.CompatibleWith(compiled.ReturnType))
			{
				return $"{signature}: registered return type {function.ReturnType.Describe()} but compiled function returns {compiled.ReturnType.Describe()}";
			}
		}
		catch (Exception ex)
		{
			return $"{signature}: compiler threw {ex.GetType().Name}: {ex.Message}";
		}

		return null;
	}

	private sealed class ReturnTypeOnlyFunction : IFunction
	{
		public ReturnTypeOnlyFunction(ProgVariableTypes returnType)
		{
			ReturnType = returnType;
			Result = returnType.CompatibleWith(ProgVariableTypes.Number)
				? new NumberVariable(1.0M)
				: returnType.CompatibleWith(ProgVariableTypes.Text)
					? new TextVariable("text")
					: returnType.CompatibleWith(ProgVariableTypes.Boolean)
						? new BooleanVariable(false)
						: null;
		}

		public IProgVariable Result { get; }
		public ProgVariableTypes ReturnType { get; }
		public string ErrorMessage => string.Empty;
		public StatementResult ExpectedResult => StatementResult.Normal;

		public StatementResult Execute(IVariableSpace variables)
		{
			return StatementResult.Normal;
		}

		public bool IsReturnOrContainsReturnOnAllBranches()
		{
			return false;
		}
	}
}
