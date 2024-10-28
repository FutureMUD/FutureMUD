using MudSharp.Form.Characteristics;
using MudSharp.Framework;
using MudSharp.FutureProg.Variables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.FutureProg.Functions.Characteristics;

internal class CharacteristicValueFunction : BuiltInFunction
{
	private bool _returnIdOfCharacteristic;
	private IFuturemud _gameworld;

	private CharacteristicValueFunction(IList<IFunction> parameters, bool id, IFuturemud gameworld) : base(parameters)
	{
		_returnIdOfCharacteristic = id;
		_gameworld = gameworld;
	}

	public override ProgVariableTypes ReturnType
	{
		get => _returnIdOfCharacteristic ? ProgVariableTypes.Number : ProgVariableTypes.Text;
		protected set => base.ReturnType = value;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"characteristicvalue",
				new[]
				{
					ProgVariableTypes.Character, ProgVariableTypes.Number
				},
				(pars, gameworld) => new CharacteristicValueFunction(pars, false, gameworld),
				new List<string> { "character", "characteristic" },
				new List<string>
				{
					"The character whose characteristics you are interested in probing",
					"The ID Number of the characteristic definition you want to probe the value of"
				},
				"This function returns the name of the supplied character's intrinsic characteristic for the supplied definition. E.g. If you supplied the ID number of the eyecolour characteristic, you might get the 'emerald green' as a return value.",
				"Characteristics",
				ProgVariableTypes.Text
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"characteristicvalue",
				new[]
				{
					ProgVariableTypes.Item, ProgVariableTypes.Number
				},
				(pars, gameworld) => new CharacteristicValueFunction(pars, false, gameworld),
				new List<string> { "item", "characteristic" },
				new List<string>
				{
					"The item whose characteristics you are interested in probing",
					"The ID Number of the characteristic definition you want to probe the value of"
				},
				"This function returns the name of the supplied items's intrinsic characteristic for the supplied definition. E.g. If you supplied the ID number of the colour characteristic, you might get the 'hot pink' as a return value.",
				"Characteristics",
				ProgVariableTypes.Text
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"characteristicvalue",
				new[]
				{
					ProgVariableTypes.Character, ProgVariableTypes.Text
				},
				(pars, gameworld) => new CharacteristicValueFunction(pars, false, gameworld),
				new List<string> { "character", "characteristic" },
				new List<string>
				{
					"The character whose characteristics you are interested in probing",
					"The name of the characteristic definition you want to probe the value of"
				},
				"This function returns the name of the supplied character's intrinsic characteristic for the supplied definition. E.g. If you supplied the ID number of the eyecolour characteristic, you might get the 'emerald green' as a return value.",
				"Characteristics",
				ProgVariableTypes.Text
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"characteristicvalue",
				new[]
				{
					ProgVariableTypes.Item, ProgVariableTypes.Text
				},
				(pars, gameworld) => new CharacteristicValueFunction(pars, false, gameworld),
				new List<string> { "item", "characteristic" },
				new List<string>
				{
					"The item whose characteristics you are interested in probing",
					"The name of the characteristic definition you want to probe the value of"
				},
				"This function returns the name of the supplied items's intrinsic characteristic for the supplied definition. E.g. If you supplied the ID number of the colour characteristic, you might get the 'hot pink' as a return value.",
				"Characteristics",
				ProgVariableTypes.Text
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"characteristicid",
				new[]
				{
					ProgVariableTypes.Character, ProgVariableTypes.Number
				},
				(pars, gameworld) => new CharacteristicValueFunction(pars, true, gameworld),
				new List<string> { "character", "characteristic" },
				new List<string>
				{
					"The character whose characteristics you are interested in probing",
					"The ID Number of the characteristic definition you want to probe the value of"
				},
				"This function returns the ID of the supplied character's intrinsic characteristic for the supplied definition. E.g. If you supplied the ID number of the colour characteristic, you might get the ID 435 as a return value.",
				"Characteristics",
				ProgVariableTypes.Text
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"characteristicid",
				new[]
				{
					ProgVariableTypes.Item, ProgVariableTypes.Number
				},
				(pars, gameworld) => new CharacteristicValueFunction(pars, true, gameworld),
				new List<string> { "item", "characteristic" },
				new List<string>
				{
					"The item whose characteristics you are interested in probing",
					"The ID Number of the characteristic definition you want to probe the value of"
				},
				"This function returns the ID of the supplied items's intrinsic characteristic for the supplied definition. E.g. If you supplied the ID number of the colour characteristic, you might get the ID 435 as a return value.",
				"Characteristics",
				ProgVariableTypes.Text
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"characteristicid",
				new[]
				{
					ProgVariableTypes.Character, ProgVariableTypes.Text
				},
				(pars, gameworld) => new CharacteristicValueFunction(pars, true, gameworld),
				new List<string> { "character", "characteristic" },
				new List<string>
				{
					"The character whose characteristics you are interested in probing",
					"The name of the characteristic definition you want to probe the value of"
				},
				"This function returns the ID of the supplied character's intrinsic characteristic for the supplied definition. E.g. If you supplied the ID number of the colour characteristic, you might get the ID 435 as a return value.",
				"Characteristics",
				ProgVariableTypes.Text
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"characteristicid",
				new[]
				{
					ProgVariableTypes.Item, ProgVariableTypes.Text
				},
				(pars, gameworld) => new CharacteristicValueFunction(pars, true, gameworld),
				new List<string> { "item", "characteristic" },
				new List<string>
				{
					"The item whose characteristics you are interested in probing",
					"The name of the characteristic definition you want to probe the value of"
				},
				"This function returns the ID of the supplied items's intrinsic characteristic for the supplied definition. E.g. If you supplied the ID number of the colour characteristic, you might get the ID 435 as a return value.",
				"Characteristics",
				ProgVariableTypes.Text
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
			Result = _returnIdOfCharacteristic ? (IProgVariable)new NumberVariable(0) : new TextVariable("");
			return StatementResult.Normal;
		}

		var definition = ParameterFunctions[1].ReturnType.CompatibleWith(ProgVariableTypes.Text)
			? _gameworld.Characteristics.GetByName(ParameterFunctions[1].Result?.GetObject as string ?? "")
			: _gameworld.Characteristics.Get((long)(ParameterFunctions[1].Result?.GetObject as decimal? ?? 0.0M));
		if (definition == null)
		{
			Result = _returnIdOfCharacteristic ? (IProgVariable)new NumberVariable(0) : new TextVariable("");
			return StatementResult.Normal;
		}

		var value = target.GetCharacteristic(definition, null);
		Result = _returnIdOfCharacteristic
			? (IProgVariable)new NumberVariable(value?.Id ?? 0L)
			: new TextVariable(value?.Name ?? "");
		return StatementResult.Normal;
	}
}