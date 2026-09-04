
using MudSharp.GameItems;

namespace MudSharp.FutureProg.Functions.Outfits;

internal sealed class LoadOutfitTemplate : BuiltInFunction
{
	private readonly IFuturemud _gameworld;
	private readonly bool _useName;
	private readonly bool _useOverride;
	private readonly bool _useLoadArguments;

	private LoadOutfitTemplate(IList<IFunction> parameterFunctions, IFuturemud gameworld, bool useName, bool useOverride, bool useLoadArguments)
		: base(parameterFunctions)
	{
		_gameworld = gameworld;
		_useName = useName;
		_useOverride = useOverride;
		_useLoadArguments = useLoadArguments;
	}

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.Outfit;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var directTemplate = ParameterFunctions[0].Result.GetObject as IOutfitTemplate;
		var template = directTemplate ??
		               (_useName
			               ? _gameworld.OutfitTemplates.GetByIdOrName(ParameterFunctions[0].Result.GetObject?.ToString() ?? string.Empty)
			               : _gameworld.OutfitTemplates.Get((long)(decimal)ParameterFunctions[0].Result.GetObject));
		if (template is null)
		{
			ErrorMessage = "There was no such outfit template.";
			return StatementResult.Error;
		}

		var target = ParameterFunctions[1].Result.GetObject as ICharacter;
		if (target is null)
		{
			ErrorMessage = "Null target in LoadOutfitTemplate.";
			return StatementResult.Error;
		}

		var overrideName = _useOverride
			? ParameterFunctions[2].Result.GetObject?.ToString()
			: null;
		var loadArguments = _useLoadArguments
			? ParameterFunctions[3].Result.GetObject?.ToString()
			: null;

		try
		{
			Result = template.Materialise(target, overrideName, loadArguments);
			return StatementResult.Normal;
		}
		catch (Exception ex)
		{
			ErrorMessage = ex.Message;
			return StatementResult.Error;
		}
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"loadoutfittemplate",
			new[] { ProgVariableTypes.Number, ProgVariableTypes.Character },
			(pars, gameworld) => new LoadOutfitTemplate(pars, gameworld, useName: false, useOverride: false, useLoadArguments: false),
			new List<string> { "templateId", "target" },
			new List<string>
			{
				"The ID of the outfit template to load",
				"The character who should receive the created outfit and items"
			},
			"Loads all the items in an outfit template, places them around the target character according to the template, attaches a normal outfit to the target, and returns that outfit.",
			"Outfits",
			ProgVariableTypes.Outfit
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"loadoutfittemplate",
			new[] { ProgVariableTypes.Text, ProgVariableTypes.Character },
			(pars, gameworld) => new LoadOutfitTemplate(pars, gameworld, useName: true, useOverride: false, useLoadArguments: false),
			new List<string> { "templateName", "target" },
			new List<string>
			{
				"The name or ID of the outfit template to load",
				"The character who should receive the created outfit and items"
			},
			"Loads all the items in an outfit template, places them around the target character according to the template, attaches a normal outfit to the target, and returns that outfit.",
			"Outfits",
			ProgVariableTypes.Outfit
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"loadoutfittemplate",
			new[] { ProgVariableTypes.Number, ProgVariableTypes.Character, ProgVariableTypes.Text },
			(pars, gameworld) => new LoadOutfitTemplate(pars, gameworld, useName: false, useOverride: true, useLoadArguments: false),
			new List<string> { "templateId", "target", "outfitName" },
			new List<string>
			{
				"The ID of the outfit template to load",
				"The character who should receive the created outfit and items",
				"The name to use for the created outfit"
			},
			"Loads all the items in an outfit template, places them around the target character according to the template, attaches a normal outfit to the target with the specified name, and returns that outfit.",
			"Outfits",
			ProgVariableTypes.Outfit
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"loadoutfittemplate",
			new[] { ProgVariableTypes.Text, ProgVariableTypes.Character, ProgVariableTypes.Text },
			(pars, gameworld) => new LoadOutfitTemplate(pars, gameworld, useName: true, useOverride: true, useLoadArguments: false),
			new List<string> { "templateName", "target", "outfitName" },
			new List<string>
			{
				"The name or ID of the outfit template to load",
				"The character who should receive the created outfit and items",
				"The name to use for the created outfit"
			},
			"Loads all the items in an outfit template, places them around the target character according to the template, attaches a normal outfit to the target with the specified name, and returns that outfit.",
			"Outfits",
			ProgVariableTypes.Outfit
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"loadoutfittemplate",
			new[] { ProgVariableTypes.Number, ProgVariableTypes.Character, ProgVariableTypes.Text, ProgVariableTypes.Text },
			(pars, gameworld) => new LoadOutfitTemplate(pars, gameworld, useName: false, useOverride: true, useLoadArguments: true),
			new List<string> { "templateId", "target", "outfitName", "loadArgs" },
			new List<string>
			{
				"The ID of the outfit template to load",
				"The character who should receive the created outfit and items",
				"The name to use for the created outfit, or blank to use the template name",
				"Additional item load arguments to apply to every created item"
			},
			"Loads all the items in an outfit template, applies the extra load arguments to every created item after any per-item arguments, places them around the target character according to the template, attaches a normal outfit to the target with the specified name, and returns that outfit.",
			"Outfits",
			ProgVariableTypes.Outfit
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"loadoutfittemplate",
			new[] { ProgVariableTypes.Text, ProgVariableTypes.Character, ProgVariableTypes.Text, ProgVariableTypes.Text },
			(pars, gameworld) => new LoadOutfitTemplate(pars, gameworld, useName: true, useOverride: true, useLoadArguments: true),
			new List<string> { "templateName", "target", "outfitName", "loadArgs" },
			new List<string>
			{
				"The name or ID of the outfit template to load",
				"The character who should receive the created outfit and items",
				"The name to use for the created outfit, or blank to use the template name",
				"Additional item load arguments to apply to every created item"
			},
			"Loads all the items in an outfit template, applies the extra load arguments to every created item after any per-item arguments, places them around the target character according to the template, attaches a normal outfit to the target with the specified name, and returns that outfit.",
			"Outfits",
			ProgVariableTypes.Outfit
		));

		RegisterTypedTemplate(useOverride: false, useLoadArguments: false);
		RegisterTypedTemplate(useOverride: true, useLoadArguments: false);
		RegisterTypedTemplate(useOverride: true, useLoadArguments: true);
	}

	private static void RegisterTypedTemplate(bool useOverride, bool useLoadArguments)
	{
		var parameterTypes = useLoadArguments
			? new[] { ProgVariableTypes.OutfitTemplate, ProgVariableTypes.Character, ProgVariableTypes.Text, ProgVariableTypes.Text }
			: useOverride
				? new[] { ProgVariableTypes.OutfitTemplate, ProgVariableTypes.Character, ProgVariableTypes.Text }
				: new[] { ProgVariableTypes.OutfitTemplate, ProgVariableTypes.Character };
		var parameterNames = useLoadArguments
			? new List<string> { "template", "target", "outfitName", "loadArgs" }
			: useOverride
				? new List<string> { "template", "target", "outfitName" }
				: new List<string> { "template", "target" };
		var parameterHelp = useLoadArguments
			? new List<string>
			{
				"The resolved outfit template to load", "The character who should receive the created outfit and items",
				"The name to use for the created outfit", "Additional item load arguments to apply to every created item"
			}
			: useOverride
				? new List<string>
				{
					"The resolved outfit template to load", "The character who should receive the created outfit and items",
					"The name to use for the created outfit"
				}
				: new List<string>
				{
					"The resolved outfit template to load", "The character who should receive the created outfit and items"
				};

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"loadoutfittemplate",
			parameterTypes,
			(pars, gameworld) => new LoadOutfitTemplate(pars, gameworld, useName: false, useOverride, useLoadArguments),
			parameterNames,
			parameterHelp,
			"Loads all items in a resolved outfit template onto the target and returns the created outfit.",
			"Outfits",
			ProgVariableTypes.Outfit));
	}
}
