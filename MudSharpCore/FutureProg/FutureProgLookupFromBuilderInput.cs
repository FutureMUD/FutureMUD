using MudSharp.Character;
using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.FutureProg;
#nullable enable
internal class FutureProgLookupFromBuilderInput
{
	public IFuturemud Gameworld { get; }
	public ICharacter? Character { get; }
	public string BuilderInput { get; }
	public FutureProgVariableTypes TargetReturnType { get; }
	private readonly List<IEnumerable<FutureProgVariableTypes>> _parameters = new();

	public FutureProgLookupFromBuilderInput(IFuturemud gameworld, ICharacter? character, string builderInput,
		FutureProgVariableTypes targetReturnType, IEnumerable<IEnumerable<FutureProgVariableTypes>> parameters)
	{
		Gameworld = gameworld;
		BuilderInput = builderInput;
		TargetReturnType = targetReturnType;
		_parameters.AddRange(parameters);
		Character = character;
	}

	public FutureProgLookupFromBuilderInput(IFuturemud gameworld, ICharacter character, string builderInput,
		FutureProgVariableTypes targetReturnType, IEnumerable<FutureProgVariableTypes> parameters)
	{
		Gameworld = gameworld;
		BuilderInput = builderInput;
		TargetReturnType = targetReturnType;
		_parameters.Add(parameters);
		Character = character;
	}

	/// <summary>
	/// This function will search for the prog based on the supplied criteria and echo to the admin any reason for failure.
	/// </summary>
	/// <returns>A matching prog, or null if none is found</returns>
	public IFutureProg? LookupProg()
	{
		var prog = Gameworld.FutureProgs.GetByIdOrName(BuilderInput);
		if (prog is null)
		{
			Character?.OutputHandler.Send($"There is no such prog identified by {BuilderInput.ColourCommand()}.");
			return null;
		}

		if (Character?.IsAdministrator() == false && !prog.Public)
		{
			Character?.OutputHandler.Send($"There is no such prog identified by {BuilderInput.ColourCommand()}.");
			return null;
		}

		if (!prog.ReturnType.CompatibleWith(TargetReturnType))
		{
			Character?.OutputHandler.Send(
				$"You must specify a prog that returns a {TargetReturnType.Describe().ColourName()} value, whereas {prog.MXPClickableFunctionName()} returns {prog.ReturnType.Describe().ColourName()}.");
			return null;
		}

		if (!_parameters.Any(x => prog.MatchesParameters(x)))
		{
			if (_parameters.Count == 1)
			{
				Character?.OutputHandler.Send(
					$@"You must specify a prog with parameters matching {_parameters.Single().Select(x => x.Describe().ColourName()).ListToString()}, whereas {prog.MXPClickableFunctionName()}'s parameters are {prog.Parameters.Select(x => x.Describe().ColourName()).ListToString()}.");
			}
			else
			{
				Character?.OutputHandler.Send(
					$@"The prog {prog.MXPClickableFunctionName()} does not have the correct paramaters.
The parameters of {prog.MXPClickableFunctionName()} are {prog.Parameters.Select(x => x.Describe().ColourName()).ListToString()}.
You can select progs with the following combinations of parameters:{_parameters.Select(x => $"\t{x.Select(y => y.Describe().ColourName()).ListToString()}").ListToLines()}");
			}

			return null;
		}

		return prog;
	}
}