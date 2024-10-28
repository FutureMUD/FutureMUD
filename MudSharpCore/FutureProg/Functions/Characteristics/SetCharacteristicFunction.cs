using MudSharp.Form.Characteristics;
using MudSharp.Framework;
using MudSharp.FutureProg.Variables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.FutureProg.Functions.Characteristics;

internal class SetCharacteristicFunction : BuiltInFunction
{
	private IFuturemud _gameworld;

	private SetCharacteristicFunction(IList<IFunction> parameters, IFuturemud gameworld) : base(parameters)
	{
		_gameworld = gameworld;
	}

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.Boolean;
		protected set => base.ReturnType = value;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"setcharacteristic",
				new[]
				{
					ProgVariableTypes.Character, ProgVariableTypes.Number, ProgVariableTypes.Number
				},
				(pars, gameworld) => new SetCharacteristicFunction(pars, gameworld),
				new List<string> { "character", "definition", "value" },
				new List<string>
				{
					"The character whose characteristics you want to set",
					"The ID number of the characteristic definition you want to use",
					"The ID number of the characteristic value that you want to set"
				},
				"Sets the intrinsic characteristic value for the characteristic definition on the supplied target. Returns true if successful.",
				"Characteristics",
				ProgVariableTypes.Boolean
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"setcharacteristic",
				new[]
				{
					ProgVariableTypes.Character, ProgVariableTypes.Text, ProgVariableTypes.Number
				},
				(pars, gameworld) => new SetCharacteristicFunction(pars, gameworld),
				new List<string> { "character", "definition", "value" },
				new List<string>
				{
					"The character whose characteristics you want to set",
					"The name of the characteristic definition you want to use",
					"The ID number of the characteristic value that you want to set"
				},
				"Sets the intrinsic characteristic value for the characteristic definition on the supplied target. Returns true if successful.",
				"Characteristics",
				ProgVariableTypes.Boolean
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"setcharacteristic",
				new[]
				{
					ProgVariableTypes.Character, ProgVariableTypes.Number, ProgVariableTypes.Text
				},
				(pars, gameworld) => new SetCharacteristicFunction(pars, gameworld),
				new List<string> { "character", "definition", "value" },
				new List<string>
				{
					"The character whose characteristics you want to set",
					"The ID number of the characteristic definition you want to use",
					"The name of the characteristic value that you want to set"
				},
				"Sets the intrinsic characteristic value for the characteristic definition on the supplied target. Returns true if successful.",
				"Characteristics",
				ProgVariableTypes.Boolean
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"setcharacteristic",
				new[]
				{
					ProgVariableTypes.Character, ProgVariableTypes.Text, ProgVariableTypes.Text
				},
				(pars, gameworld) => new SetCharacteristicFunction(pars, gameworld),
				new List<string> { "character", "definition", "value" },
				new List<string>
				{
					"The character whose characteristics you want to set",
					"The name of the characteristic definition you want to use",
					"The name of the characteristic value that you want to set"
				},
				"Sets the intrinsic characteristic value for the characteristic definition on the supplied target. Returns true if successful.",
				"Characteristics",
				ProgVariableTypes.Boolean
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"setcharacteristic",
				new[]
				{
					ProgVariableTypes.Item, ProgVariableTypes.Number, ProgVariableTypes.Number
				},
				(pars, gameworld) => new SetCharacteristicFunction(pars, gameworld),
				new List<string> { "item", "definition", "value" },
				new List<string>
				{
					"The item whose characteristics you want to set",
					"The ID number of the characteristic definition you want to use",
					"The ID number of the characteristic value that you want to set"
				},
				"Sets the intrinsic characteristic value for the characteristic definition on the supplied target. Returns true if successful.",
				"Characteristics",
				ProgVariableTypes.Boolean
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"setcharacteristic",
				new[]
				{
					ProgVariableTypes.Item, ProgVariableTypes.Text, ProgVariableTypes.Number
				},
				(pars, gameworld) => new SetCharacteristicFunction(pars, gameworld),
				new List<string> { "item", "definition", "value" },
				new List<string>
				{
					"The item whose characteristics you want to set",
					"The name of the characteristic definition you want to use",
					"The ID number of the characteristic value that you want to set"
				},
				"Sets the intrinsic characteristic value for the characteristic definition on the supplied target. Returns true if successful.",
				"Characteristics",
				ProgVariableTypes.Boolean
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"setcharacteristic",
				new[]
				{
					ProgVariableTypes.Item, ProgVariableTypes.Number, ProgVariableTypes.Text
				},
				(pars, gameworld) => new SetCharacteristicFunction(pars, gameworld),
				new List<string> { "item", "definition", "value" },
				new List<string>
				{
					"The item whose characteristics you want to set",
					"The ID number of the characteristic definition you want to use",
					"The name of the characteristic value that you want to set"
				},
				"Sets the intrinsic characteristic value for the characteristic definition on the supplied target. Returns true if successful.",
				"Characteristics",
				ProgVariableTypes.Boolean
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"setcharacteristic",
				new[]
				{
					ProgVariableTypes.Item, ProgVariableTypes.Text, ProgVariableTypes.Text
				},
				(pars, gameworld) => new SetCharacteristicFunction(pars, gameworld),
				new List<string> { "item", "definition", "value" },
				new List<string>
				{
					"The item whose characteristics you want to set",
					"The name of the characteristic definition you want to use",
					"The name of the characteristic value that you want to set"
				},
				"Sets the intrinsic characteristic value for the characteristic definition on the supplied target. Returns true if successful.",
				"Characteristics",
				ProgVariableTypes.Boolean
			)
		);
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		if (ParameterFunctions[0]?.Result is not IHaveCharacteristics target)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		var definition = ParameterFunctions[1].ReturnType.CompatibleWith(ProgVariableTypes.Text)
			? _gameworld.Characteristics.GetByName(ParameterFunctions[1].Result?.GetObject as string ?? "")
			: _gameworld.Characteristics.Get((long)(ParameterFunctions[1].Result?.GetObject as decimal? ?? 0.0M));
		if (definition == null)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		var value = ParameterFunctions[2].ReturnType.CompatibleWith(ProgVariableTypes.Text)
			? _gameworld.CharacteristicValues.GetByName(ParameterFunctions[2].Result?.GetObject as string ?? "")
			: _gameworld.CharacteristicValues.Get((long)(ParameterFunctions[2].Result?.GetObject as decimal? ?? 0.0M));
		if (value == null || !definition.IsValue(value))
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		target.SetCharacteristic(definition, value);
		Result = new BooleanVariable(true);
		return StatementResult.Normal;
	}
}