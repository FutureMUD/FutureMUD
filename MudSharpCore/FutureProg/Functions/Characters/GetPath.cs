using System.Collections.Generic;
using System.Linq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Framework;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.Characters;

internal class GetPath : BuiltInFunction
{
    public IFuturemud Gameworld { get; set; }
    #region Static Initialisation

    public static void RegisterFunctionCompiler()
    {
        FutureProg.RegisterBuiltInFunctionCompiler(
            new FunctionCompilerInformation(
                "getpath",
                [
                    ProgVariableTypes.Character, 
					ProgVariableTypes.Location
                ], // the parameters the function takes
                (pars, gameworld) => new GetPath(pars, gameworld, PathSearchMode.IncludeUnlockableDoors),
                [
                    "Character",
                    "Destination"
                ], // parameter names
                [
                    "The character who you want directions for",
                    "The destination room to path them to"
                ], // parameter help text
                "Returns a list of direction commands between the character and the target, if one exists.", // help text for the function,
                "Character", // the category to which this function belongs,
                ProgVariableTypes.Text | ProgVariableTypes.Collection // the return type of the function
            )
        );

        FutureProg.RegisterBuiltInFunctionCompiler(
            new FunctionCompilerInformation(
                "getpathignoredoors",
                [
                    ProgVariableTypes.Character,
                    ProgVariableTypes.Location
                ], // the parameters the function takes
                (pars, gameworld) => new GetPath(pars, gameworld, PathSearchMode.IgnoreDoors),
                [
                    "Character",
                    "Destination"
                ], // parameter names
                [
                    "The character who you want directions for",
                    "The destination room to path them to"
                ], // parameter help text
                "Returns a list of direction commands between the character and the target, if one exists, but ignores any doors (open, closed, locked or otherwise).", // help text for the function,
                "Character", // the category to which this function belongs,
                ProgVariableTypes.Text | ProgVariableTypes.Collection // the return type of the function
            )
        );

        FutureProg.RegisterBuiltInFunctionCompiler(
            new FunctionCompilerInformation(
                "getpathrespectdoors",
                [
                    ProgVariableTypes.Character,
                    ProgVariableTypes.Location
                ], // the parameters the function takes
                (pars, gameworld) => new GetPath(pars, gameworld, PathSearchMode.RespectClosedDoors),
                [
                    "Character",
                    "Destination"
                ], // parameter names
                [
                    "The character who you want directions for",
                    "The destination room to path them to"
                ], // parameter help text
                "Returns a list of direction commands between the character and the target, if one exists, but respects any closed doors as untraversable.", // help text for the function,
                "Character", // the category to which this function belongs,
                ProgVariableTypes.Text | ProgVariableTypes.Collection // the return type of the function
            )
        );

        FutureProg.RegisterBuiltInFunctionCompiler(
            new FunctionCompilerInformation(
                "getpathunlocked",
                [
                    ProgVariableTypes.Character,
                    ProgVariableTypes.Location
                ], // the parameters the function takes
                (pars, gameworld) => new GetPath(pars, gameworld, PathSearchMode.IncludeUnlockedDoors),
                [
                    "Character",
                    "Destination"
                ], // parameter names
                [
                    "The character who you want directions for",
                    "The destination room to path them to"
                ], // parameter help text
                "Returns a list of direction commands between the character and the target, if one exists, but respects any locked and closed doors as untraversable.", // help text for the function,
                "Character", // the category to which this function belongs,
                ProgVariableTypes.Text | ProgVariableTypes.Collection // the return type of the function
            )
        );
    }

    #endregion

	internal enum PathSearchMode
	{
		IncludeUnlockableDoors,
		IncludeUnlockedDoors,
		IgnoreDoors,
        RespectClosedDoors
    }

	public PathSearchMode SearchMode {get;}

    #region Constructors

    protected GetPath(IList<IFunction> parameterFunctions, IFuturemud gameworld, PathSearchMode searchMode) : base(
        parameterFunctions)
    {
        Gameworld = gameworld;
        SearchMode = searchMode;
    }

    #endregion

    public override ProgVariableTypes ReturnType
    {
        get => ProgVariableTypes.Text | ProgVariableTypes.Collection;
        protected set { }
    }

    public override StatementResult Execute(IVariableSpace variables)
    {
        if (base.Execute(variables) == StatementResult.Error)
        {
            return StatementResult.Error;
        }

        if (ParameterFunctions[0].Result is not ICharacter target)
        {
            Result = new BooleanVariable(false);
            return StatementResult.Normal;
        }

        if (ParameterFunctions[1].Result is not ICell location)
        {
            Result = new BooleanVariable(false);
            return StatementResult.Normal;
        }

		var path = target
			.PathBetween(location, 50, 
			SearchMode switch { 
				PathSearchMode.IgnoreDoors => PathSearch.PathIgnoreDoors(target),
				PathSearchMode.IncludeUnlockableDoors => PathSearch.PathIncludeUnlockableDoors(target),
				PathSearchMode.IncludeUnlockedDoors => PathSearch.PathIncludeUnlockedDoors(target),
				PathSearchMode.RespectClosedDoors => PathSearch.PathRespectClosedDoors(target),
                _ => PathSearch.PathIncludeUnlockableDoors(target)
			}
			)
            .Select(x =>
            {
                if (x.OutboundDirection != CardinalDirection.Unknown)
                {
                    return x.OutboundDirection.DescribeBrief();
                }

                return x is NonCardinalCellExit nc ? $"{nc.Verb} {nc.PrimaryKeyword}".ToLowerInvariant() : "??";
            })
            .ToList();

        Result = new CollectionVariable(path, ProgVariableTypes.Text);
        return StatementResult.Normal;
    }
}
