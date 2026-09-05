using MudSharp.Form.Characteristics;
using MudSharp.CharacterCreation;
using MudSharp.FutureProg.Variables;
using System.Text.RegularExpressions;

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

        var target = ParameterFunctions[0].Result?.GetObject;
        if (target == null)
        {
            Result = new NullVariable(ProgVariableTypes.Text);
            return StatementResult.Normal;
        }

        if (target is not IHaveCharacteristics && target is not IChargen)
        {
            Result = new NullVariable(ProgVariableTypes.Text);
            return StatementResult.Normal;
        }

        var ihc = target as IHaveCharacteristics;
        if (ParameterFunctions[1].ReturnType == ProgVariableTypes.CharacteristicDefinition)
        {
            var definition = ParameterFunctions[1].Result?.GetObject as ICharacteristicDefinition;
            var viewer = IgnorePerceiver ? null : ParameterFunctions[2].Result?.GetObject as IPerceiver;
            if (target is IChargen typedChargen)
            {
                var selected = typedChargen.SelectedCharacteristics.FirstOrDefault(x => x.Item1 == definition).Item2;
                Result = selected is null ? new NullVariable(ProgVariableTypes.Text) : new TextVariable(selected.GetValue);
                return StatementResult.Normal;
            }

            Result = definition is null
                ? new NullVariable(ProgVariableTypes.Text)
                : new TextVariable(ihc.DescribeCharacteristic(definition, viewer, CharacteristicDescriptionType.Normal));
            return StatementResult.Normal;
        }

        string targetDefinition = ParameterFunctions[1].Result?.GetObject?.ToString() ?? "";

        IPerceiver perceiver = ParameterFunctions.Count == 3 ? (IPerceiver)ParameterFunctions[2].Result?.GetObject : default;

        if (string.IsNullOrEmpty(targetDefinition))
        {
            Result = new NullVariable(ProgVariableTypes.Text);
            return StatementResult.Normal;
        }


        if (target is IChargen chargen)
        {
            CharacteristicDescriptionType type;
            Regex regex = new("(?<base>.+)(?<modifier>basic|fancy)", RegexOptions.IgnoreCase);
            if (regex.IsMatch(targetDefinition))
            {
                Match match = regex.Match(targetDefinition);
                type = match.Groups["modifier"].Value.Equals("basic", StringComparison.InvariantCultureIgnoreCase)
                    ? CharacteristicDescriptionType.Basic
                    : CharacteristicDescriptionType.Fancy;
                targetDefinition = match.Groups["base"].Value;
            }
            else
            {
                type = CharacteristicDescriptionType.Normal;
            }

            (ICharacteristicDefinition, ICharacteristicValue) definition =
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
            Tuple<ICharacteristicDefinition, CharacteristicDescriptionType> definition = ihc.GetCharacteristicDefinition(targetDefinition);
            if (definition?.Item1 is null)
            {
                Result = new NullVariable(ProgVariableTypes.Text);
                return StatementResult.Normal;
            }

            string result = ihc.DescribeCharacteristic(definition.Item1, IgnorePerceiver ? null : perceiver,
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
		foreach (var real in new[] { false, true })
		{
			FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
				real ? "getrealcharacteristic" : "getcharacteristic",
				real ? [ProgVariableTypes.Item | ProgVariableTypes.Toon, ProgVariableTypes.CharacteristicDefinition]
					: [ProgVariableTypes.Item | ProgVariableTypes.Toon, ProgVariableTypes.CharacteristicDefinition, ProgVariableTypes.Perceiver],
				(pars, world) => new GetCharacteristicFunction(pars, world, real),
				real ? ["target", "definition"] : ["target", "definition", "perceiver"],
				real ? ["The character, chargen or item to query.", "The resolved definition to describe."]
					: ["The character, chargen or item to query.", "The resolved definition to describe.", "The viewer whose perception applies."],
				"Returns the normal description of the characteristic. The real variant ignores obscurers; use the text definition overload for basic/fancy description modifiers.",
				"Characteristics", ProgVariableTypes.Text));
		}

    }
}