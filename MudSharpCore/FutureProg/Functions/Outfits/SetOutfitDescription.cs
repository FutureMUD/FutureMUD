using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;

namespace MudSharp.FutureProg.Functions.Outfits;

internal class SetOutfitDescription : BuiltInFunction
{
    public IFuturemud Gameworld { get; set; }

    #region Static Initialisation

    public static void RegisterFunctionCompiler()
    {
        FutureProg.RegisterBuiltInFunctionCompiler(
            new FunctionCompilerInformation(
                "setoutfitdescription",
                new[] { ProgVariableTypes.Outfit, ProgVariableTypes.Text },
                (pars, gameworld) => new SetOutfitDescription(pars, gameworld),
                [
                    "outfit",
                    "description"
                ],
                [
                    "The outfit to redescribe",
                    "The new description for the outfit"
                ],
                "This function redescribes an outfit. Returns the outfit parameter.",
                "Outfits",
                ProgVariableTypes.Outfit
            )
        );
    }

    #endregion

    #region Constructors

    protected SetOutfitDescription(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions)
    {
        Gameworld = gameworld;
    }

    #endregion

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

        IOutfit outfit = (IOutfit)ParameterFunctions[0].Result;
        if (outfit == null)
        {
            ErrorMessage = "Null outfit in SetOutfitDescription";
            return StatementResult.Error;
        }

        string newDesc = ParameterFunctions[1].Result.GetObject?.ToString();
        if (string.IsNullOrEmpty(newDesc))
        {
            ErrorMessage = "Invalid Description in SetOutfitDescription";
            return StatementResult.Error;
        }

        outfit.Description = newDesc;
        outfit.Owner.OutfitsChanged = true;
        Result = outfit;
        return StatementResult.Normal;
    }
}