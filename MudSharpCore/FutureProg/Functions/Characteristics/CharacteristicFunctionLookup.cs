#nullable enable

using MudSharp.Form.Characteristics;

namespace MudSharp.FutureProg.Functions.Characteristics;

internal static class CharacteristicFunctionLookup
{
	internal static readonly ProgVariableTypes[] DefinitionTypes =
	[
		ProgVariableTypes.Number, ProgVariableTypes.Text, ProgVariableTypes.CharacteristicDefinition
	];

	internal static ICharacteristicDefinition? Definition(IFunction function, IFuturemud gameworld)
	{
		return function.Result?.GetObject switch
		{
			ICharacteristicDefinition definition => definition,
			decimal id => gameworld.Characteristics.Get((long)id),
			string name => gameworld.Characteristics.GetByName(name),
			_ => null
		};
	}
}
