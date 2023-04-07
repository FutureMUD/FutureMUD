using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.FutureProg.Variables;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.FutureProg.Functions.BuiltIn;

internal class ConvertFunction : BuiltInFunction
{
	public IFuturemud Gameworld { get; set; }

	#region Static Initialisation

	public static void RegisterFunctionCompiler()
	{
		// MRHs
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"convert",
				new[] { FutureProgVariableTypes.MagicResourceHaver, FutureProgVariableTypes.Character },
				(pars, gameworld) => new ConvertFunction(pars, gameworld),
				new[] { "from", "to" },
				new[]
				{
					"The variable that you want to convert. Can be null.",
					"A dummy variable of the type that you want to convert to."
				},
				"This function allows you to convert a variable from one type to another type.",
				"Conversion",
				FutureProgVariableTypes.Character
			)
		);
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"convert",
				new[] { FutureProgVariableTypes.MagicResourceHaver, FutureProgVariableTypes.Toon },
				(pars, gameworld) => new ConvertFunction(pars, gameworld),
				new[] { "from", "to" },
				new[]
				{
					"The variable that you want to convert. Can be null.",
					"A dummy variable of the type that you want to convert to."
				},
				"This function allows you to convert a variable from one type to another type.",
				"Conversion",
				FutureProgVariableTypes.Toon
			)
		);
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"convert",
				new[] { FutureProgVariableTypes.MagicResourceHaver, FutureProgVariableTypes.Item },
				(pars, gameworld) => new ConvertFunction(pars, gameworld),
				new[] { "from", "to" },
				new[]
				{
					"The variable that you want to convert. Can be null.",
					"A dummy variable of the type that you want to convert to."
				},
				"This function allows you to convert a variable from one type to another type.",
				"Conversion",
				FutureProgVariableTypes.Item
			)
		);
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"convert",
				new[] { FutureProgVariableTypes.MagicResourceHaver, FutureProgVariableTypes.Location },
				(pars, gameworld) => new ConvertFunction(pars, gameworld),
				new[] { "from", "to" },
				new[]
				{
					"The variable that you want to convert. Can be null.",
					"A dummy variable of the type that you want to convert to."
				},
				"This function allows you to convert a variable from one type to another type.",
				"Conversion",
				FutureProgVariableTypes.Location
			)
		);

		// Perceivers
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"convert",
				new[] { FutureProgVariableTypes.Perceiver, FutureProgVariableTypes.Character },
				(pars, gameworld) => new ConvertFunction(pars, gameworld),
				new[] { "from", "to" },
				new[]
				{
					"The variable that you want to convert. Can be null.",
					"A dummy variable of the type that you want to convert to."
				},
				"This function allows you to convert a variable from one type to another type.",
				"Conversion",
				FutureProgVariableTypes.Character
			)
		);
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"convert",
				new[] { FutureProgVariableTypes.Perceiver, FutureProgVariableTypes.Item },
				(pars, gameworld) => new ConvertFunction(pars, gameworld),
				new[] { "from", "to" },
				new[]
				{
					"The variable that you want to convert. Can be null.",
					"A dummy variable of the type that you want to convert to."
				},
				"This function allows you to convert a variable from one type to another type.",
				"Conversion",
				FutureProgVariableTypes.Item
			)
		);

		// Perceivables
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"convert",
				new[] { FutureProgVariableTypes.Perceivable, FutureProgVariableTypes.Character },
				(pars, gameworld) => new ConvertFunction(pars, gameworld),
				new[] { "from", "to" },
				new[]
				{
					"The variable that you want to convert. Can be null.",
					"A dummy variable of the type that you want to convert to."
				},
				"This function allows you to convert a variable from one type to another type.",
				"Conversion",
				FutureProgVariableTypes.Character
			)
		);
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"convert",
				new[] { FutureProgVariableTypes.Perceivable, FutureProgVariableTypes.Item },
				(pars, gameworld) => new ConvertFunction(pars, gameworld),
				new[] { "from", "to" },
				new[]
				{
					"The variable that you want to convert. Can be null.",
					"A dummy variable of the type that you want to convert to."
				},
				"This function allows you to convert a variable from one type to another type.",
				"Conversion",
				FutureProgVariableTypes.Item
			)
		);
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"convert",
				new[] { FutureProgVariableTypes.Perceivable, FutureProgVariableTypes.Location },
				(pars, gameworld) => new ConvertFunction(pars, gameworld),
				new[] { "from", "to" },
				new[]
				{
					"The variable that you want to convert. Can be null.",
					"A dummy variable of the type that you want to convert to."
				},
				"This function allows you to convert a variable from one type to another type.",
				"Conversion",
				FutureProgVariableTypes.Location
			)
		);
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"convert",
				new[] { FutureProgVariableTypes.Perceivable, FutureProgVariableTypes.Zone },
				(pars, gameworld) => new ConvertFunction(pars, gameworld),
				new[] { "from", "to" },
				new[]
				{
					"The variable that you want to convert. Can be null.",
					"A dummy variable of the type that you want to convert to."
				},
				"This function allows you to convert a variable from one type to another type.",
				"Conversion",
				FutureProgVariableTypes.Zone
			)
		);
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"convert",
				new[] { FutureProgVariableTypes.Perceivable, FutureProgVariableTypes.Shard },
				(pars, gameworld) => new ConvertFunction(pars, gameworld),
				new[] { "from", "to" },
				new[]
				{
					"The variable that you want to convert. Can be null.",
					"A dummy variable of the type that you want to convert to."
				},
				"This function allows you to convert a variable from one type to another type.",
				"Conversion",
				FutureProgVariableTypes.Shard
			)
		);

		// Reference Types
		foreach (var type in FutureProgVariableTypes.ReferenceType.GetSingleFlags())
		{
			FutureProg.RegisterBuiltInFunctionCompiler(
				new FunctionCompilerInformation(
					"convert",
					new[] { FutureProgVariableTypes.ReferenceType, type },
					(pars, gameworld) => new ConvertFunction(pars, gameworld),
					new[] { "from", "to" },
					new[]
					{
						"The variable that you want to convert. Can be null.",
						"A dummy variable of the type that you want to convert to."
					},
					"This function allows you to convert a variable from one type to another type.",
					"Conversion",
					type
				)
			);
		}

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"convert",
				new[] { FutureProgVariableTypes.ReferenceType, FutureProgVariableTypes.Perceivable },
				(pars, gameworld) => new ConvertFunction(pars, gameworld),
				new[] { "from", "to" },
				new[]
				{
					"The variable that you want to convert. Can be null.",
					"A dummy variable of the type that you want to convert to."
				},
				"This function allows you to convert a variable from one type to another type.",
				"Conversion",
				FutureProgVariableTypes.Perceivable
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"convert",
				new[] { FutureProgVariableTypes.ReferenceType, FutureProgVariableTypes.Perceiver },
				(pars, gameworld) => new ConvertFunction(pars, gameworld),
				new[] { "from", "to" },
				new[]
				{
					"The variable that you want to convert. Can be null.",
					"A dummy variable of the type that you want to convert to."
				},
				"This function allows you to convert a variable from one type to another type.",
				"Conversion",
				FutureProgVariableTypes.Perceiver
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"convert",
				new[] { FutureProgVariableTypes.ReferenceType, FutureProgVariableTypes.Toon },
				(pars, gameworld) => new ConvertFunction(pars, gameworld),
				new[] { "from", "to" },
				new[]
				{
					"The variable that you want to convert. Can be null.",
					"A dummy variable of the type that you want to convert to."
				},
				"This function allows you to convert a variable from one type to another type.",
				"Conversion",
				FutureProgVariableTypes.Toon
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"convert",
				new[] { FutureProgVariableTypes.ReferenceType, FutureProgVariableTypes.MagicResourceHaver },
				(pars, gameworld) => new ConvertFunction(pars, gameworld),
				new[] { "from", "to" },
				new[]
				{
					"The variable that you want to convert. Can be null.",
					"A dummy variable of the type that you want to convert to."
				},
				"This function allows you to convert a variable from one type to another type.",
				"Conversion",
				FutureProgVariableTypes.MagicResourceHaver
			)
		);

		// All Types
		foreach (var type in FutureProgVariableTypes.Anything.GetSingleFlags())
		{
			FutureProg.RegisterBuiltInFunctionCompiler(
				new FunctionCompilerInformation(
					"convert",
					new[] { FutureProgVariableTypes.Anything, type },
					(pars, gameworld) => new ConvertFunction(pars, gameworld),
					new[] { "from", "to" },
					new[]
					{
						"The variable that you want to convert. Can be null.",
						"A dummy variable of the type that you want to convert to."
					},
					"This function allows you to convert a variable from one type to another type.",
					"Conversion",
					type
				)
			);
		}
	}

	#endregion

	#region Constructors

	protected ConvertFunction(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions)
	{
		Gameworld = gameworld;
	}

	#endregion

	public override FutureProgVariableTypes ReturnType
	{
		get => ParameterFunctions.ElementAtOrDefault(1)?.ReturnType ?? FutureProgVariableTypes.Anything;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			Result = new NullVariable(ReturnType);
			return StatementResult.Error;
		}

		var result = FutureProg.GetVariable(ReturnType, ParameterFunctions[0].Result?.GetObject);
		if (!result.Type.CompatibleWith(ReturnType))
		{
			Result = new NullVariable(ReturnType);
			return StatementResult.Normal;
		}

		switch (ParameterFunctions[0].ReturnType)
		{
			case FutureProgVariableTypes.MagicResourceHaver:
			case FutureProgVariableTypes.Perceivable:
			case FutureProgVariableTypes.Perceiver:
			case FutureProgVariableTypes.Anything:
			case FutureProgVariableTypes.ReferenceType:
				Result = result;
				return StatementResult.Normal;
		}

		Result = new NullVariable(ReturnType);
		return StatementResult.Normal;
	}
}