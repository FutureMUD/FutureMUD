using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MudSharp.Form.Characteristics;
using MudSharp.Framework;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.BuiltIn;

internal class GetCharacteristicFunction : BuiltInFunction
{
	private GetCharacteristicFunction(IList<IFunction> parameters, IFuturemud gameworld, bool ignoreObscurers)
		: base(parameters)
	{
		Gameworld = gameworld;
		IgnorePerceiver = ignoreObscurers;
	}

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.Text;
		protected set { }
	}

	private IFuturemud Gameworld { get; set; }
	private bool IgnorePerceiver { get; }

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var target = ParameterFunctions[0].Result;
		if (target == null)
		{
			Result = new NullVariable(ProgVariableTypes.Text);
			return StatementResult.Normal;
		}

		if (target is not IHaveCharacteristics ihc)
		{
			Result = new NullVariable(ProgVariableTypes.Text);
			return StatementResult.Normal;
		}

		var targetDefinition = ParameterFunctions[1].Result?.GetObject.ToString() ?? "";

		var perceiver = ParameterFunctions.Count == 3 ? (IPerceiver)ParameterFunctions[2].Result?.GetObject : default;

		if (string.IsNullOrEmpty(targetDefinition))
		{
			Result = new NullVariable(ProgVariableTypes.Text);
			return StatementResult.Normal;
		}


		if (target is CharacterCreation.Chargen chargen)
		{
			CharacteristicDescriptionType type;
			var regex = new Regex("(?<base>.+)(?<modifier>basic|fancy)", RegexOptions.IgnoreCase);
			if (regex.IsMatch(targetDefinition))
			{
				var match = regex.Match(targetDefinition);
				type = match.Groups["modifier"].Value.Equals("basic", StringComparison.InvariantCultureIgnoreCase)
					? CharacteristicDescriptionType.Basic
					: CharacteristicDescriptionType.Fancy;
				targetDefinition = match.Groups["base"].Value;
			}
			else
			{
				type = CharacteristicDescriptionType.Normal;
			}

			var definition =
				chargen.SelectedCharacteristics.FirstOrDefault(
					x => x.Item1.Name.Equals(targetDefinition, StringComparison.InvariantCultureIgnoreCase));
			if (definition.Item1 == null)
			{
				Result = new NullVariable(ProgVariableTypes.Text);
				return StatementResult.Normal;
			}

			switch (type)
			{
				case CharacteristicDescriptionType.Normal:
					Result = new TextVariable(definition.Item2.GetValue);
					break;
				case CharacteristicDescriptionType.Fancy:
					Result = new TextVariable(definition.Item2.GetFancyValue);
					break;
				case CharacteristicDescriptionType.Basic:
					Result = new TextVariable(definition.Item2.GetBasicValue);
					break;
			}

			return StatementResult.Normal;
		}
		else
		{
			var definition = ihc.GetCharacteristicDefinition(targetDefinition);
			var result = ihc.DescribeCharacteristic(definition.Item1, IgnorePerceiver ? null : perceiver,
				definition.Item2);
			Result = new TextVariable(result);
			return StatementResult.Normal;
		}
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"getcharacteristic",
				new[]
				{
					ProgVariableTypes.Item | ProgVariableTypes.Toon, ProgVariableTypes.Text,
					ProgVariableTypes.Perceiver
				},
				(pars, gameworld) => new GetCharacteristicFunction(pars, gameworld, false),
				new List<string> { "thing", "target", "perceiver" },
				new List<string>
				{
					"The thing whose characteristics you want to interrogate",
					"The name of the characteristic you want, including <name>fancy or <name>basic forms",
					"The perceiver through whose perspective these characteristics should be interpreted"
				},
				"This function allows you to return the text value of a specified characteristic of a character, chargen or item. It returns the same result as if you had used $name in a description.",
				"Characteristics",
				ProgVariableTypes.Text
			)
		);
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"getrealcharacteristic",
				new[] { ProgVariableTypes.Item | ProgVariableTypes.Toon, ProgVariableTypes.Text },
				(pars, gameworld) => new GetCharacteristicFunction(pars, gameworld, true),
				new List<string> { "thing", "target" },
				new List<string>
				{
					"The thing whose characteristics you want to interrogate",
					"The name of the characteristic you want, including <name>fancy or <name>basic forms"
				},
				"This function allows you to return the text value of a specified characteristic of a character, chargen or item. It returns the same result as if you had used $name in a description. This version returns the true value of a characteristic, unhindered by the lens of a perceiver.",
				"Characteristics",
				ProgVariableTypes.Text
			)
		);
	}
}