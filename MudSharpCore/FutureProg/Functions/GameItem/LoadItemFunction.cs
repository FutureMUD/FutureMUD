using MudSharp.Events;
using MudSharp.Form.Characteristics;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.FutureProg.Functions.GameItem;

internal class LoadItemFunction : BuiltInFunction
{
    private readonly IFuturemud _gameworld;

    public LoadItemFunction(IList<IFunction> parameters, IFuturemud gameworld, bool usequantity, bool useparamstring, bool useUniqueName)
        : base(parameters)
    {
        _gameworld = gameworld;
        UseQuantity = usequantity;
        UseParamsString = useparamstring;
        UseUniqueName = useUniqueName;
    }

    public bool UseQuantity { get; set; }
    public bool UseParamsString { get; set; }
    public bool UseUniqueName { get; set; }

    public override ProgVariableTypes ReturnType
    {
        get => ProgVariableTypes.Item;
        protected set { }
    }

    public override StatementResult Execute(IVariableSpace variables)
    {
        if (base.Execute(variables) == StatementResult.Error)
        {
            return StatementResult.Error;
        }

        var inputText = UseUniqueName
            ? (string)ParameterFunctions[0].Result.GetObject
            : ((long)(decimal)ParameterFunctions[0].Result.GetObject).ToString();
        IGameItemProto proto = UseUniqueName
            ? _gameworld.ItemProtos.FindByUniqueName(inputText)
            : _gameworld.ItemProtos.Get(long.Parse(inputText));
        if (proto == null)
        {
            ErrorMessage = "There was no prototype " + inputText;
            return StatementResult.Error;
        }

        if (proto.Status != RevisionStatus.Current)
        {
            ErrorMessage = "Prototype " + inputText + " is not approved for use.";
            return StatementResult.Error;
        }

        if (proto.Components.Any(x => x.PreventManualLoad))
        {
            ErrorMessage = $"Prototype {inputText} contains components that prevent manual loading.";
            return StatementResult.Error;
        }

        string paramString = "";
        if (UseParamsString)
        {
            paramString =
                (string)
                (UseQuantity
                    ? ParameterFunctions[2].Result.GetObject
                    : ParameterFunctions[1].Result.GetObject);
        }

        int quantity = 1;
        if (UseQuantity && proto.IsItemType<StackableGameItemComponentProto>())
        {
            quantity = (int)(decimal)ParameterFunctions[1].Result.GetObject;
        }

        if (quantity < 1)
        {
            return StatementResult.Normal;
        }

        IGameItem newItem = proto.CreateNew(null, null, quantity, paramString).First();
        _gameworld.Add(newItem);
        newItem.HandleEvent(EventType.ItemFinishedLoading, newItem);
        newItem.Login();
        Result = newItem;
        return StatementResult.Normal;
    }

    public static void RegisterFunctionCompiler()
    {
        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "loaditem",
            new[] { ProgVariableTypes.Number },
            (pars, gameworld) => new LoadItemFunction(pars, gameworld, false, false, false),
            new List<string> { "id" },
            new List<string> { "The ID of the item prototype that you want to load" },
            "This function loads a new item into the game based on the ID that you supply. It does not put the item anywhere, so you must then insert it somewhere like in a room or a character's inventory.",
            "Items",
            ProgVariableTypes.Item
        ));

        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "loaditem",
            new[] { ProgVariableTypes.Number, ProgVariableTypes.Number },
            (pars, gameworld) => new LoadItemFunction(pars, gameworld, true, false, false),
            new List<string> { "id", "quantity" },
            new List<string>
            {
                "The ID of the item prototype that you want to load",
                "The quantity of the item you want to load. If this item is not stackable, this input is ignored."
            },
            "This function loads a new item into the game based on the ID that you supply. It does not put the item anywhere, so you must then insert it somewhere like in a room or a character's inventory.",
            "Items",
            ProgVariableTypes.Item
        ));

        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "loaditem",
            new[] { ProgVariableTypes.Number, ProgVariableTypes.Text },
            (pars, gameworld) => new LoadItemFunction(pars, gameworld, false, true, false),
            new List<string> { "id", "variables" },
            new List<string>
            {
                "The ID of the item prototype that you want to load",
                "The default values for any variables on this item. This syntax is as per using the ITEM LOAD command in game, which means the general syntax is varname=value or varname=\"longer value\", with multiple variables separated by spaces"
            },
            "This function loads a new item into the game based on the ID that you supply. It does not put the item anywhere, so you must then insert it somewhere like in a room or a character's inventory.",
            "Items",
            ProgVariableTypes.Item
        ));

        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "loaditem",
            new[] { ProgVariableTypes.Number, ProgVariableTypes.Number, ProgVariableTypes.Text },
            (pars, gameworld) => new LoadItemFunction(pars, gameworld, true, true, false),
            new List<string> { "id", "quantity", "variables" },
            new List<string>
            {
                "The ID of the item prototype that you want to load",
                "The quantity of the item you want to load. If this item is not stackable, this input is ignored",
                "The default values for any variables on this item. This syntax is as per using the ITEM LOAD command in game, which means the general syntax is varname=value or varname=\"longer value\", with multiple variables separated by spaces"
            },
            "This function loads a new item into the game based on the ID that you supply. It does not put the item anywhere, so you must then insert it somewhere like in a room or a character's inventory.",
            "Items",
            ProgVariableTypes.Item
        ));

        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "loaditem",
            new[] { ProgVariableTypes.Text },
            (pars, gameworld) => new LoadItemFunction(pars, gameworld, false, false, true),
            new List<string> { "uniqueName" },
            new List<string> { "The unique name of the item prototype that you want to load" },
            "This function loads a new item into the game based on the unique name that you supply. It does not put the item anywhere, so you must then insert it somewhere like in a room or a character's inventory.",
            "Items",
            ProgVariableTypes.Item
        ));

        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "loaditem",
            new[] { ProgVariableTypes.Text, ProgVariableTypes.Number },
            (pars, gameworld) => new LoadItemFunction(pars, gameworld, true, false, true),
            new List<string> { "uniqueName", "quantity" },
            new List<string>
            {
                "The unique name of the item prototype that you want to load",
                "The quantity of the item you want to load. If this item is not stackable, this input is ignored."
            },
            "This function loads a new item into the game based on the unique name that you supply. It does not put the item anywhere, so you must then insert it somewhere like in a room or a character's inventory.",
            "Items",
            ProgVariableTypes.Item
        ));

        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "loaditem",
            new[] { ProgVariableTypes.Text, ProgVariableTypes.Text },
            (pars, gameworld) => new LoadItemFunction(pars, gameworld, false, true, true),
            new List<string> { "uniqueName", "variables" },
            new List<string>
            {
                "The unique name of the item prototype that you want to load",
                "The default values for any variables on this item. This syntax is as per using the ITEM LOAD command in game, which means the general syntax is varname=value or varname=\"longer value\", with multiple variables separated by spaces"
            },
            "This function loads a new item into the game based on the unique name that you supply. It does not put the item anywhere, so you must then insert it somewhere like in a room or a character's inventory.",
            "Items",
            ProgVariableTypes.Item
        ));

        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "loaditem",
            new[] { ProgVariableTypes.Text, ProgVariableTypes.Number, ProgVariableTypes.Text },
            (pars, gameworld) => new LoadItemFunction(pars, gameworld, true, true, true),
            new List<string> { "uniqueName", "quantity", "variables" },
            new List<string>
            {
                "The unique name of the item prototype that you want to load",
                "The quantity of the item you want to load. If this item is not stackable, this input is ignored",
                "The default values for any variables on this item. This syntax is as per using the ITEM LOAD command in game, which means the general syntax is varname=value or varname=\"longer value\", with multiple variables separated by spaces"
            },
            "This function loads a new item into the game based on the unique name that you supply. It does not put the item anywhere, so you must then insert it somewhere like in a room or a character's inventory.",
            "Items",
            ProgVariableTypes.Item
        ));
    }
}
