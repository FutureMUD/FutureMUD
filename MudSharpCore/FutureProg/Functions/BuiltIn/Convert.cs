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
				new[] { ProgVariableTypes.MagicResourceHaver, ProgVariableTypes.Character },
				(pars, gameworld) => new ConvertFunction(pars, gameworld),
				new[] { "from", "to" },
				new[]
				{
					"The variable that you want to convert. Can be null.",
					"A dummy variable of the type that you want to convert to."
				},
				"This function allows you to convert a variable from one type to another type.",
				"Conversion",
				ProgVariableTypes.Character
			)
		);
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"convert",
				new[] { ProgVariableTypes.MagicResourceHaver, ProgVariableTypes.Toon },
				(pars, gameworld) => new ConvertFunction(pars, gameworld),
				new[] { "from", "to" },
				new[]
				{
					"The variable that you want to convert. Can be null.",
					"A dummy variable of the type that you want to convert to."
				},
				"This function allows you to convert a variable from one type to another type.",
				"Conversion",
				ProgVariableTypes.Toon
			)
		);
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"convert",
				new[] { ProgVariableTypes.MagicResourceHaver, ProgVariableTypes.Item },
				(pars, gameworld) => new ConvertFunction(pars, gameworld),
				new[] { "from", "to" },
				new[]
				{
					"The variable that you want to convert. Can be null.",
					"A dummy variable of the type that you want to convert to."
				},
				"This function allows you to convert a variable from one type to another type.",
				"Conversion",
				ProgVariableTypes.Item
			)
		);
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"convert",
				new[] { ProgVariableTypes.MagicResourceHaver, ProgVariableTypes.Location },
				(pars, gameworld) => new ConvertFunction(pars, gameworld),
				new[] { "from", "to" },
				new[]
				{
					"The variable that you want to convert. Can be null.",
					"A dummy variable of the type that you want to convert to."
				},
				"This function allows you to convert a variable from one type to another type.",
				"Conversion",
				ProgVariableTypes.Location
			)
		);

		// Perceivers
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"convert",
				new[] { ProgVariableTypes.Perceiver, ProgVariableTypes.Character },
				(pars, gameworld) => new ConvertFunction(pars, gameworld),
				new[] { "from", "to" },
				new[]
				{
					"The variable that you want to convert. Can be null.",
					"A dummy variable of the type that you want to convert to."
				},
				"This function allows you to convert a variable from one type to another type.",
				"Conversion",
				ProgVariableTypes.Character
			)
		);
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"convert",
				new[] { ProgVariableTypes.Perceiver, ProgVariableTypes.Item },
				(pars, gameworld) => new ConvertFunction(pars, gameworld),
				new[] { "from", "to" },
				new[]
				{
					"The variable that you want to convert. Can be null.",
					"A dummy variable of the type that you want to convert to."
				},
				"This function allows you to convert a variable from one type to another type.",
				"Conversion",
				ProgVariableTypes.Item
			)
		);

		// Perceivables
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"convert",
				new[] { ProgVariableTypes.Perceivable, ProgVariableTypes.Character },
				(pars, gameworld) => new ConvertFunction(pars, gameworld),
				new[] { "from", "to" },
				new[]
				{
					"The variable that you want to convert. Can be null.",
					"A dummy variable of the type that you want to convert to."
				},
				"This function allows you to convert a variable from one type to another type.",
				"Conversion",
				ProgVariableTypes.Character
			)
		);
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"convert",
				new[] { ProgVariableTypes.Perceivable, ProgVariableTypes.Item },
				(pars, gameworld) => new ConvertFunction(pars, gameworld),
				new[] { "from", "to" },
				new[]
				{
					"The variable that you want to convert. Can be null.",
					"A dummy variable of the type that you want to convert to."
				},
				"This function allows you to convert a variable from one type to another type.",
				"Conversion",
				ProgVariableTypes.Item
			)
		);
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"convert",
				new[] { ProgVariableTypes.Perceivable, ProgVariableTypes.Location },
				(pars, gameworld) => new ConvertFunction(pars, gameworld),
				new[] { "from", "to" },
				new[]
				{
					"The variable that you want to convert. Can be null.",
					"A dummy variable of the type that you want to convert to."
				},
				"This function allows you to convert a variable from one type to another type.",
				"Conversion",
				ProgVariableTypes.Location
			)
		);
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"convert",
				new[] { ProgVariableTypes.Perceivable, ProgVariableTypes.Zone },
				(pars, gameworld) => new ConvertFunction(pars, gameworld),
				new[] { "from", "to" },
				new[]
				{
					"The variable that you want to convert. Can be null.",
					"A dummy variable of the type that you want to convert to."
				},
				"This function allows you to convert a variable from one type to another type.",
				"Conversion",
				ProgVariableTypes.Zone
			)
		);
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"convert",
				new[] { ProgVariableTypes.Perceivable, ProgVariableTypes.Shard },
				(pars, gameworld) => new ConvertFunction(pars, gameworld),
				new[] { "from", "to" },
				new[]
				{
					"The variable that you want to convert. Can be null.",
					"A dummy variable of the type that you want to convert to."
				},
				"This function allows you to convert a variable from one type to another type.",
				"Conversion",
				ProgVariableTypes.Shard
			)
		);

		// Reference Types
		foreach (var type in ProgVariableTypes.ReferenceType.GetSingleFlags())
		{
			FutureProg.RegisterBuiltInFunctionCompiler(
				new FunctionCompilerInformation(
					"convert",
					new[] { ProgVariableTypes.ReferenceType, type },
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
				new[] { ProgVariableTypes.ReferenceType, ProgVariableTypes.Perceivable },
				(pars, gameworld) => new ConvertFunction(pars, gameworld),
				new[] { "from", "to" },
				new[]
				{
					"The variable that you want to convert. Can be null.",
					"A dummy variable of the type that you want to convert to."
				},
				"This function allows you to convert a variable from one type to another type.",
				"Conversion",
				ProgVariableTypes.Perceivable
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"convert",
				new[] { ProgVariableTypes.ReferenceType, ProgVariableTypes.Perceiver },
				(pars, gameworld) => new ConvertFunction(pars, gameworld),
				new[] { "from", "to" },
				new[]
				{
					"The variable that you want to convert. Can be null.",
					"A dummy variable of the type that you want to convert to."
				},
				"This function allows you to convert a variable from one type to another type.",
				"Conversion",
				ProgVariableTypes.Perceiver
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"convert",
				new[] { ProgVariableTypes.ReferenceType, ProgVariableTypes.Toon },
				(pars, gameworld) => new ConvertFunction(pars, gameworld),
				new[] { "from", "to" },
				new[]
				{
					"The variable that you want to convert. Can be null.",
					"A dummy variable of the type that you want to convert to."
				},
				"This function allows you to convert a variable from one type to another type.",
				"Conversion",
				ProgVariableTypes.Toon
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"convert",
				new[] { ProgVariableTypes.ReferenceType, ProgVariableTypes.MagicResourceHaver },
				(pars, gameworld) => new ConvertFunction(pars, gameworld),
				new[] { "from", "to" },
				new[]
				{
					"The variable that you want to convert. Can be null.",
					"A dummy variable of the type that you want to convert to."
				},
				"This function allows you to convert a variable from one type to another type.",
				"Conversion",
				ProgVariableTypes.MagicResourceHaver
			)
		);

		// All Types
		foreach (var type in ProgVariableTypes.Anything.GetSingleFlags())
		{
			FutureProg.RegisterBuiltInFunctionCompiler(
				new FunctionCompilerInformation(
					"convert",
					new[] { ProgVariableTypes.Anything, type },
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

	public override ProgVariableTypes ReturnType
	{
		get => ParameterFunctions.ElementAtOrDefault(1)?.ReturnType ?? ProgVariableTypes.Anything;
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
			case ProgVariableTypes.MagicResourceHaver:
			case ProgVariableTypes.Perceivable:
			case ProgVariableTypes.Perceiver:
			case ProgVariableTypes.Anything:
			case ProgVariableTypes.ReferenceType:
				Result = result;
				return StatementResult.Normal;
		}

		Result = new NullVariable(ReturnType);
		return StatementResult.Normal;
	}
}