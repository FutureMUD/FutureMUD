#nullable enable

using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.GameItems.Inventory.Size;

namespace MudSharp.FutureProg.Functions.GameItem;

internal enum WearableSizingFunctionKind
{
	Scalar,
	Height,
	Weight,
	State,
	Snapshot,
	SetScalar,
	SetForCharacter,
	SetFromText,
	Fits
}

internal sealed class WearableSizingFunction(IList<IFunction> parameterFunctions, WearableSizingFunctionKind kind)
	: BuiltInFunction(parameterFunctions)
{
	public override ProgVariableTypes ReturnType
	{
		get => kind switch
		{
			WearableSizingFunctionKind.State or WearableSizingFunctionKind.Snapshot => ProgVariableTypes.Text,
			WearableSizingFunctionKind.Scalar or WearableSizingFunctionKind.Height or WearableSizingFunctionKind.Weight => ProgVariableTypes.Number,
			_ => ProgVariableTypes.Boolean
		};
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error) return StatementResult.Error;

		var item = ParameterFunctions[0].Result?.GetObject as IGameItem;
		var wearable = item?.GetItemType<IWearable>();
		if (kind is WearableSizingFunctionKind.Scalar or WearableSizingFunctionKind.Height or
			WearableSizingFunctionKind.Weight or WearableSizingFunctionKind.State or WearableSizingFunctionKind.Snapshot)
		{
			if (wearable is null)
			{
				Result = kind is WearableSizingFunctionKind.State or WearableSizingFunctionKind.Snapshot
					? new TextVariable(string.Empty)
					: new NumberVariable(0.0);
				return StatementResult.Normal;
			}

			var fit = wearable.Fit;
			Result = kind switch
			{
				WearableSizingFunctionKind.Scalar => new NumberVariable(fit.Scalar),
				WearableSizingFunctionKind.Height => new NumberVariable(fit.Height),
				WearableSizingFunctionKind.Weight => new NumberVariable(fit.Weight),
				WearableSizingFunctionKind.State => new TextVariable(fit.Kind.ToString()),
				_ => new TextVariable(WearableSizing.ToLoadValue(fit))
			};
			return StatementResult.Normal;
		}

		if (wearable is null)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		switch (kind)
		{
			case WearableSizingFunctionKind.SetScalar:
				if (ParameterFunctions[1].Result?.GetObject is not decimal scalarValue || scalarValue < 0 || !double.IsFinite((double)scalarValue))
				{
					Result = new BooleanVariable(false);
					return StatementResult.Normal;
				}

				wearable.SetFit(WearableFit.ForScalar((double)scalarValue));
				Result = new BooleanVariable(true);
				return StatementResult.Normal;
			case WearableSizingFunctionKind.SetForCharacter:
				if (ParameterFunctions[1].Result?.GetObject is not ICharacter character)
				{
					Result = new BooleanVariable(false);
					return StatementResult.Normal;
				}

				wearable.SetFit(wearable.FitFor(character.Body));
				Result = new BooleanVariable(true);
				return StatementResult.Normal;
			case WearableSizingFunctionKind.SetFromText:
				if (ParameterFunctions[1].Result?.GetObject is not string text ||
					!WearableSizing.TryParse(text, null, item!.Gameworld, out var fit, out _))
				{
					Result = new BooleanVariable(false);
					return StatementResult.Normal;
				}

				wearable.SetFit(fit);
				Result = new BooleanVariable(true);
				return StatementResult.Normal;
			case WearableSizingFunctionKind.Fits:
				Result = new BooleanVariable(ParameterFunctions[1].Result?.GetObject is ICharacter target && wearable.Fits(target.Body));
				return StatementResult.Normal;
			default:
				Result = new BooleanVariable(false);
				return StatementResult.Normal;
		}
	}

	public static void RegisterFunctionCompiler()
	{
		Register("wearablefitscalar", [], "Returns the scalar fit value, or zero for a non-wearable.", WearableSizingFunctionKind.Scalar, ProgVariableTypes.Number);
		Register("wearablefitheight", [], "Returns the garment fit height in base length units, or zero when unavailable.", WearableSizingFunctionKind.Height, ProgVariableTypes.Number);
		Register("wearablefitweight", [], "Returns the garment fit weight in base mass units, or zero when unavailable.", WearableSizingFunctionKind.Weight, ProgVariableTypes.Number);
		Register("wearablefitstate", [], "Returns the garment fit state name, or an empty string for a non-wearable.", WearableSizingFunctionKind.State, ProgVariableTypes.Text);
		Register("wearablefit", [], "Returns an item's fit as a reusable wearsize load-value snapshot, or an empty string for a non-wearable.", WearableSizingFunctionKind.Snapshot, ProgVariableTypes.Text);
		Register("setwearablefitscalar", [ProgVariableTypes.Number], "Sets a wearable item's scalar fit. Returns false for a non-wearable or invalid value.", WearableSizingFunctionKind.SetScalar, ProgVariableTypes.Boolean, "scalar");
		Register("setwearablefit", [ProgVariableTypes.Toon], "Sets a wearable item's fit to the current measurements of a character. Returns false for a non-wearable.", WearableSizingFunctionKind.SetForCharacter, ProgVariableTypes.Boolean, "character");
		Register("setwearablefit", [ProgVariableTypes.Text], "Sets a wearable item's fit from unfitted, universal, a scalar, or a native-unit measurements snapshot. Returns false for invalid input.", WearableSizingFunctionKind.SetFromText, ProgVariableTypes.Boolean, "fit");
		Register("wearablefits", [ProgVariableTypes.Toon], "Tests whether a wearable item fits a character.", WearableSizingFunctionKind.Fits, ProgVariableTypes.Boolean, "character");
	}

	private static void Register(string name, ProgVariableTypes[] extraParameters, string description,
		WearableSizingFunctionKind kind, ProgVariableTypes returnType, string? extraParameterName = null)
	{
		var parameterTypes = new[] { ProgVariableTypes.Item }.Concat(extraParameters).ToArray();
		var parameterNames = new[] { "item" }.Concat(extraParameterName is null ? [] : [extraParameterName]).ToList();
		var parameterDescriptions = new[] { "The wearable item to inspect or modify." }
			.Concat(extraParameterName is null ? [] : [extraParameterName == "scalar"
				? "The scalar fit value to set."
				: "The character whose body measurements are used."]).ToList();
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			name,
			parameterTypes,
			(pars, _) => new WearableSizingFunction(pars, kind),
			parameterNames,
			parameterDescriptions,
			description,
			"Items",
			returnType));
	}
}
