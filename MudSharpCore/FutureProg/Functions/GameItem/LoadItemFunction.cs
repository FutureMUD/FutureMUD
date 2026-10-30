using MudSharp.Events;
using MudSharp.Form.Characteristics;
using MudSharp.Framework.Revision;
using MudSharp.GameItems;
using MudSharp.GameItems.Prototypes;
using MudSharp.Character;

namespace MudSharp.FutureProg.Functions.GameItem;

internal class LoadItemFunction : BuiltInFunction
{
    private readonly IFuturemud _gameworld;
    private readonly bool _useCharacterContext;

    public LoadItemFunction(IList<IFunction> parameters, IFuturemud gameworld, bool usequantity, bool useparamstring, bool useUniqueName, bool useCharacterContext = false)
        : base(parameters)
    {
        _gameworld = gameworld;
        UseQuantity = usequantity;
        UseParamsString = useparamstring;
        UseUniqueName = useUniqueName;
        _useCharacterContext = useCharacterContext;
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

        var directPrototype = ParameterFunctions[0].Result.GetObject as IGameItemProto;
        var inputText = directPrototype?.Name ??
                        (UseUniqueName
                            ? (string)ParameterFunctions[0].Result.GetObject
                            : ((long)(decimal)ParameterFunctions[0].Result.GetObject).ToString());
        IGameItemProto proto = directPrototype ??
                              (UseUniqueName
                                  ? _gameworld.ItemProtos.FindByUniqueName(inputText)
                                  : _gameworld.ItemProtos.Get(long.Parse(inputText)));
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
            var paramIndex = _useCharacterContext ? 2 : UseQuantity ? 2 : 1;
            paramString = (string)ParameterFunctions[paramIndex].Result.GetObject;
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

        ICharacter? loader = _useCharacterContext ? ParameterFunctions[1].Result.GetObject as ICharacter : null;
        if (_useCharacterContext && loader is null)
        {
            ErrorMessage = "The loaditem character argument must be a character.";
            return StatementResult.Error;
        }

        IGameItem newItem;
        try
        {
            newItem = proto.CreateNew(loader, null, quantity, paramString).First();
        }
        catch (ArgumentException error)
        {
            ErrorMessage = error.Message;
            return StatementResult.Error;
        }
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

        RegisterTypedPrototype(false, false);
        RegisterTypedPrototype(true, false);
        RegisterTypedPrototype(false, true);
        RegisterTypedPrototype(true, true);
        RegisterContextLoadItem(ProgVariableTypes.Number, false);
        RegisterContextLoadItem(ProgVariableTypes.Text, true);
        RegisterContextLoadItem(ProgVariableTypes.ItemPrototype, false);
    }

    private static void RegisterContextLoadItem(ProgVariableTypes prototypeType, bool useUniqueName)
    {
        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "loaditem",
            new[] { prototypeType, ProgVariableTypes.Character, ProgVariableTypes.Text },
            (pars, gameworld) => new LoadItemFunction(pars, gameworld, false, true, useUniqueName, true),
            new List<string> { useUniqueName ? "uniqueName" : "prototype", "character", "loadArguments" },
            new List<string>
            {
                useUniqueName ? "The unique name of the item prototype to load" : "The ID or resolved item prototype to load",
                "The character used to resolve target-relative wearable sizing arguments",
                "The item variable and wearable sizing arguments"
            },
            "Loads a new item with the supplied character as the load-string target context. The load string can contain `wearsize=10`, `wearsize=universal`, or `wearsize=\"for self\"` / `wearsize=\"for <person>\"`. It does not put the item anywhere.",
            "Items",
            ProgVariableTypes.Item));
    }

    private static void RegisterTypedPrototype(bool useQuantity, bool useParameters)
    {
        var parameterTypes = useQuantity
            ? useParameters
                ? new[] { ProgVariableTypes.ItemPrototype, ProgVariableTypes.Number, ProgVariableTypes.Text }
                : new[] { ProgVariableTypes.ItemPrototype, ProgVariableTypes.Number }
            : useParameters
                ? new[] { ProgVariableTypes.ItemPrototype, ProgVariableTypes.Text }
                : new[] { ProgVariableTypes.ItemPrototype };
        var parameterNames = useQuantity
            ? useParameters ? new List<string> { "prototype", "quantity", "variables" } : new List<string> { "prototype", "quantity" }
            : useParameters ? new List<string> { "prototype", "variables" } : new List<string> { "prototype" };
        var parameterHelp = useQuantity
            ? useParameters
                ? new List<string>
                {
                    "The resolved item prototype to load", "The stack quantity when applicable",
                    "Default values for registered item variables"
                }
                : new List<string> { "The resolved item prototype to load", "The stack quantity when applicable" }
            : useParameters
                ? new List<string> { "The resolved item prototype to load", "Default values for registered item variables" }
                : new List<string> { "The resolved item prototype to load" };

        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "loaditem",
            parameterTypes,
            (pars, gameworld) => new LoadItemFunction(pars, gameworld, useQuantity, useParameters, false),
            parameterNames,
            parameterHelp,
            "Loads a new item from a resolved item prototype. It does not put the item anywhere.",
            "Items",
            ProgVariableTypes.Item));
    }
}
