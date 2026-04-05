using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.NPC.Templates;
using System.Collections.Generic;

namespace MudSharp.FutureProg.Functions.BuiltIn;

internal class LoadNpcFunction : BuiltInFunction
{
    private readonly IFuturemud _gameworld;

    protected LoadNpcFunction(IList<IFunction> parameters, IFuturemud gameworld)
        : base(parameters)
    {
        _gameworld = gameworld;
    }

    public override ProgVariableTypes ReturnType
    {
        get => ProgVariableTypes.Character;
        protected set { }
    }

    public override StatementResult Execute(IVariableSpace variables)
    {
        if (base.Execute(variables) == StatementResult.Error)
        {
            return StatementResult.Error;
        }

        long vnum = (long)(decimal)(ParameterFunctions[0].Result?.GetObject ?? 0);
        INPCTemplate proto = _gameworld.NpcTemplates.Get(vnum);
        if (proto == null)
        {
            ErrorMessage = "There was no prototype " + vnum;
            return StatementResult.Error;
        }

        if (proto.Status != RevisionStatus.Current)
        {
            ErrorMessage = "Prototype " + vnum + " is not approved for use.";
            return StatementResult.Error;
        }

        if (ParameterFunctions[1].Result?.GetObject is not ICell location)
        {
            ErrorMessage = "Location cannot be null";
            return StatementResult.Error;
        }

        RoomLayer layer = RoomLayer.GroundLevel;
        if (ParameterFunctions.Count == 3)
        {
            string layerText = ParameterFunctions[2].Result?.GetObject?.ToString();
            if (string.IsNullOrEmpty(layerText))
            {
                ErrorMessage = "Invalid room layer specified";
                return StatementResult.Error;
            }

            if (!Utilities.TryParseEnum<RoomLayer>(layerText, out layer))
            {
                ErrorMessage = "Invalid room layer specified";
                return StatementResult.Error;
            }
        }

        ICharacter npc = proto.CreateNewCharacter(location);
        _gameworld.Add(npc, true);
        proto.OnLoadProg?.Execute(npc);
        npc.RoomLayer = layer;
        location.Login(npc);
        Result = npc;
        return StatementResult.Normal;
    }

    public static void RegisterFunctionCompiler()
    {
        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "loadnpc",
            new[] { ProgVariableTypes.Number, ProgVariableTypes.Location },
            (pars, gameworld) => new LoadNpcFunction(pars, gameworld),
            new List<string> { "Id", "Location" },
            new List<string> { "The Id of the NPC template to load", "The location into which they will be loaded" },
            "This function loads an NPC from a specified template into a location.",
            "NPCs",
            ProgVariableTypes.Character
        ));

        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "loadnpc",
            new[] { ProgVariableTypes.Number, ProgVariableTypes.Location, ProgVariableTypes.Text },
            (pars, gameworld) => new LoadNpcFunction(pars, gameworld),
            new List<string> { "Id", "Location", "Layer" },
            new List<string>
            {
                "The Id of the NPC template to load", "The location into which they will be loaded",
                "The layer into which to load the NPC"
            },
            "This function loads an NPC from a specified template into a location.",
            "NPCs",
            ProgVariableTypes.Character
        ));
    }
}