#nullable enable

using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.GameItem;

internal sealed class SetItemPermissionFunction(IList<IFunction> parameters) : BuiltInFunction(parameters)
{
	public override ProgVariableTypes ReturnType { get => ProgVariableTypes.Boolean; protected set { } }
	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error) return StatementResult.Error;
		var item = ParameterFunctions[0].Result?.GetObject as IGameItem;
		var text = ParameterFunctions[1].Result?.GetObject as string;
		var permitted = ParameterFunctions[2].Result?.GetObject as bool?;
		var components = item?.GetItemTypes<IItemInteractionPermissions>().ToArray() ?? [];
		if (permitted is null || !Enum.TryParse<ItemInteractionRestriction>(text, true, out var action) ||
			action == ItemInteractionRestriction.None || !Enum.IsDefined(action) || components.Length == 0)
		{ Result = new BooleanVariable(false); return StatementResult.Normal; }
		foreach (var component in components) component.SetPermission(action, permitted.Value);
		Result = new BooleanVariable(true); return StatementResult.Normal;
	}
	public static void RegisterFunctionCompiler() => FutureProg.RegisterBuiltInFunctionCompiler(new("setitempermission",
		[ProgVariableTypes.Item, ProgVariableTypes.Text, ProgVariableTypes.Boolean], (parameters, _) => new SetItemPermissionFunction(parameters),
		["item", "interaction", "permitted"], ["The item with an Item Permissions component.", "Pickup, Drop, Give, Put, Sheathe or RoomListing.", "True permits the interaction; false restricts it."],
		"Updates and saves an item's interaction permission without changing its prototype. Returns false for invalid inputs or an item without the component.", "Items", ProgVariableTypes.Boolean));
}
