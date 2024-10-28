using MudSharp.Character;
using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.FutureProg;
#nullable enable
internal class ProgLookupFromBuilderInputMultipleReturnTypes
{
	public IFuturemud Gameworld { get; }
	public ICharacter? Character { get; }
	public string BuilderInput { get; }
	private readonly List<ProgVariableTypes> _targetReturnTypes = new();
	private readonly List<IEnumerable<ProgVariableTypes>> _parameters = new();

	public ProgLookupFromBuilderInputMultipleReturnTypes(ICharacter character, string builderInput, ProgVariableTypes targetReturnType, IEnumerable<ProgVariableTypes> parameters)
	{
		Gameworld = character.Gameworld;
		BuilderInput = builderInput;
		_targetReturnTypes.Add(targetReturnType);
		_parameters.Add(parameters);
		Character = character;
	}

	public ProgLookupFromBuilderInputMultipleReturnTypes(ICharacter character, string builderInput, ProgVariableTypes targetReturnType, IEnumerable<IEnumerable<ProgVariableTypes>> parameters)
	{
		Gameworld = character.Gameworld;
		BuilderInput = builderInput;
		_targetReturnTypes.Add(targetReturnType);
		_parameters.AddRange(parameters);
		Character = character;
	}

	public ProgLookupFromBuilderInputMultipleReturnTypes(ICharacter character, string builderInput, IEnumerable<ProgVariableTypes> targetReturnTypes, IEnumerable<ProgVariableTypes> parameters)
	{
		Gameworld = character.Gameworld;
		BuilderInput = builderInput;
		_targetReturnTypes.AddRange(targetReturnTypes);
		_parameters.Add(parameters);
		Character = character;
	}

	public ProgLookupFromBuilderInputMultipleReturnTypes(ICharacter character, string builderInput, IEnumerable<ProgVariableTypes> targetReturnTypes, IEnumerable<IEnumerable<ProgVariableTypes>> parameters)
	{
		Gameworld = character.Gameworld;
		BuilderInput = builderInput;
		_targetReturnTypes.AddRange(targetReturnTypes);
		_parameters.AddRange(parameters);
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

		var match = false;
		foreach (var returnType in _targetReturnTypes)
		{
			if (prog.ReturnType.CompatibleWith(returnType))
			{
				match = true;
				break;
			}
		}

		if (!match)
		{
			Character?.OutputHandler.Send(
				$"You must specify a prog that returns a {_targetReturnTypes.Select(x => x.Describe()).ListToColouredStringOr(Telnet.Cyan)} value, whereas {prog.MXPClickableFunctionName()} returns {prog.ReturnType.Describe().ColourName()}.");
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
					$@"The prog {prog.MXPClickableFunctionName()} does not have the correct parameters.
The parameters of {prog.MXPClickableFunctionName()} are {prog.Parameters.Select(x => x.Describe().ColourName()).ListToString()}.
You can select progs with one of the following combinations of parameters:

{_parameters.Select(x => $"\t{x.Select(y => y.Describe().ColourName()).ListToString()}").ListToLines()}");
			}

			return null;
		}

		return prog;
	}
}

internal class ProgLookupFromBuilderInput
{
	public IFuturemud Gameworld { get; }
	public ICharacter? Character { get; }
	public string BuilderInput { get; }
	public ProgVariableTypes TargetReturnType { get; }
	private readonly List<IEnumerable<ProgVariableTypes>> _parameters = new();

	public ProgLookupFromBuilderInput(IFuturemud gameworld, ICharacter? character, string builderInput,
		ProgVariableTypes targetReturnType, IEnumerable<IEnumerable<ProgVariableTypes>> parameters)
	{
		Gameworld = gameworld;
		BuilderInput = builderInput;
		TargetReturnType = targetReturnType;
		_parameters.AddRange(parameters);
		Character = character;
	}

	public ProgLookupFromBuilderInput(IFuturemud gameworld, ICharacter character, string builderInput,
		ProgVariableTypes targetReturnType, IEnumerable<ProgVariableTypes> parameters)
	{
		Gameworld = gameworld;
		BuilderInput = builderInput;
		TargetReturnType = targetReturnType;
		_parameters.Add(parameters);
		Character = character;
	}

	public ProgLookupFromBuilderInput(ICharacter character, string builderInput, ProgVariableTypes targetReturnType, IEnumerable<ProgVariableTypes> parameters) :
		this(character.Gameworld, character, builderInput, targetReturnType, parameters)
	{
	}

	public ProgLookupFromBuilderInput(ICharacter character, string builderInput, ProgVariableTypes targetReturnType, IEnumerable<IEnumerable<ProgVariableTypes>> parameters) :
		this(character.Gameworld, character, builderInput, targetReturnType, parameters)
	{
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
					$@"The prog {prog.MXPClickableFunctionName()} does not have the correct parameters.
The parameters of {prog.MXPClickableFunctionName()} are {prog.Parameters.Select(x => x.Describe().ColourName()).ListToString()}.
You can select progs with the following combinations of parameters:{_parameters.Select(x => $"\t{x.Select(y => y.Describe().ColourName()).ListToString()}").ListToLines()}");
			}

			return null;
		}

		return prog;
	}
}