#nullable enable

using System.Collections.Generic;
using System.Linq;
using MudSharp.Character;
using MudSharp.Commands.Helpers;
using MudSharp.Construction;
using MudSharp.Economy.Property;
using MudSharp.Framework;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;

namespace MudSharp.FutureProg.Functions.GameItem;

internal sealed class HostOwnershipFunction : BuiltInFunction
{
	private enum HostOwnershipMode
	{
		Query,
		Set,
		DeepSet
	}

	private readonly IFuturemud _gameworld;
	private readonly HostOwnershipMode _mode;

	private HostOwnershipFunction(IList<IFunction> parameters, IFuturemud gameworld, HostOwnershipMode mode)
		: base(parameters)
	{
		_gameworld = gameworld;
		_mode = mode;
	}

	public override ProgVariableTypes ReturnType { get; protected set; } = ProgVariableTypes.Boolean;

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		if (ParameterFunctions[0].Result?.GetObject is not IGameItem item)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		var hostType = ParameterFunctions[1].Result?.GetObject as string ?? string.Empty;
		var identifier = ParameterFunctions[2].Result?.GetObject as string ?? string.Empty;
		var host = new EmploymentHostResolver().Resolve(_gameworld, hostType, identifier, out _);
		if (host is null)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		if (_mode == HostOwnershipMode.Query)
		{
			Result = new BooleanVariable(item.IsOwnedBy(host));
			return StatementResult.Normal;
		}

		var items = _mode == HostOwnershipMode.DeepSet
			? OwnershipFunctionHelpers.EnumerateDeepOwnedItems(item)
			: [item];
		ItemOwnershipService.AssignOwner(items, host);
		Result = new BooleanVariable(true);
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		Register("ishostowner", HostOwnershipMode.Query,
			"Returns true when the item is owned by the selected live employment host.");
		Register("sethostownership", HostOwnershipMode.Set,
			"Sets the item owner to the selected live employment host.");
		Register("deepsethostownership", HostOwnershipMode.DeepSet,
			"Sets the item and its contained item tree to the selected live employment host.");
	}

	private static void Register(string name, HostOwnershipMode mode, string help)
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			name,
			[ProgVariableTypes.Item, ProgVariableTypes.Text, ProgVariableTypes.Text],
			(pars, gameworld) => new HostOwnershipFunction(pars, gameworld, mode),
			["item", "host type", "host identifier"],
			["The item", "The employment host type", "The host ID or name"],
			help,
			"Items",
			ProgVariableTypes.Boolean));
	}
}

internal sealed class PrivatePropertyFunction : BuiltInFunction
{
	private enum PrivatePropertyMode
	{
		IsPrivate,
		IsAuthorised,
		WouldTrespass,
		Reason
	}

	private readonly PrivatePropertyMode _mode;

	private PrivatePropertyFunction(IList<IFunction> parameters, PrivatePropertyMode mode) : base(parameters)
	{
		_mode = mode;
		ReturnType = mode == PrivatePropertyMode.Reason ? ProgVariableTypes.Text : ProgVariableTypes.Boolean;
	}

	public override ProgVariableTypes ReturnType { get; protected set; }

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		if (ParameterFunctions[0].Result?.GetObject is not ICell cell)
		{
			Result = _mode == PrivatePropertyMode.Reason
				? new TextVariable(string.Empty)
				: new BooleanVariable(false);
			return StatementResult.Normal;
		}

		if (_mode == PrivatePropertyMode.IsPrivate)
		{
			Result = new BooleanVariable(PrivatePropertyAccessService.EffectFor(cell) is not null);
			return StatementResult.Normal;
		}

		if (ParameterFunctions[1].Result?.GetObject is not ICharacter character)
		{
			Result = _mode == PrivatePropertyMode.Reason
				? new TextVariable(string.Empty)
				: new BooleanVariable(false);
			return StatementResult.Normal;
		}

		var access = PrivatePropertyAccessService.Evaluate(cell, character);
		Result = _mode switch
		{
			PrivatePropertyMode.IsAuthorised => new BooleanVariable(access.IsAuthorised),
			PrivatePropertyMode.WouldTrespass => new BooleanVariable(access.IsPrivateProperty && !access.IsAuthorised),
			PrivatePropertyMode.Reason => new TextVariable(access.Explanation),
			_ => new BooleanVariable(access.IsPrivateProperty)
		};
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"isprivateproperty", [ProgVariableTypes.Location],
			(pars, _) => new PrivatePropertyFunction(pars, PrivatePropertyMode.IsPrivate),
			["location"], ["The location to test"],
			"Returns true if the location has a private-property controller.", "Locations",
			ProgVariableTypes.Boolean));
		Register("isprivatepropertyauthorised", PrivatePropertyMode.IsAuthorised, ProgVariableTypes.Boolean,
			"Returns true if the character is authorised for the private location; non-private locations also return true.");
		Register("wouldtrespass", PrivatePropertyMode.WouldTrespass, ProgVariableTypes.Boolean,
			"Returns true if the character lacks access to the private location, independent of local criminal law.");
		Register("privatepropertyaccessreason", PrivatePropertyMode.Reason, ProgVariableTypes.Text,
			"Returns the private-property access decision explanation.");
	}

	private static void Register(string name, PrivatePropertyMode mode, ProgVariableTypes returnType, string help)
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			name, [ProgVariableTypes.Location, ProgVariableTypes.Character],
			(pars, _) => new PrivatePropertyFunction(pars, mode),
			["location", "character"], ["The location to test", "The character to test"],
			help, "Locations", returnType));
	}
}
