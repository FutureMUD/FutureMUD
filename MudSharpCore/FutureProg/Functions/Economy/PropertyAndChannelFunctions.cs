#nullable enable

using MudSharp.Character;
using MudSharp.Communication;
using MudSharp.Construction;
using MudSharp.Economy.Property;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.Economy;

internal sealed class PropertyQueryFunction : BuiltInFunction
{
	private enum PropertyQueryMode
	{
		ForLocation,
		IsOwner,
		IsLeaseholder,
		IsTenant
	}

	private readonly IFuturemud _gameworld;
	private readonly PropertyQueryMode _mode;

	private PropertyQueryFunction(IList<IFunction> parameters, IFuturemud gameworld, PropertyQueryMode mode)
		: base(parameters)
	{
		_gameworld = gameworld;
		_mode = mode;
	}

	public override ProgVariableTypes ReturnType => _mode == PropertyQueryMode.ForLocation
		? ProgVariableTypes.Property
		: ProgVariableTypes.Boolean;

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		if (_mode == PropertyQueryMode.ForLocation)
		{
			if (ParameterFunctions[0].Result?.GetObject is not ICell cell)
			{
				Result = new NullVariable(ProgVariableTypes.Property);
				return StatementResult.Normal;
			}

			Result = _gameworld.Properties.FirstOrDefault(x => x.PropertyLocations.Any(y => y.Id == cell.Id))
			         is IProgVariable locationProperty
				? locationProperty
				: new NullVariable(ProgVariableTypes.Property);
			return StatementResult.Normal;
		}

		if (ParameterFunctions[0].Result?.GetObject is not IProperty property ||
		    ParameterFunctions[1].Result?.GetObject is not ICharacter character)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		var value = _mode switch
		{
			PropertyQueryMode.IsOwner => property.IsAuthorisedOwner(character),
			PropertyQueryMode.IsLeaseholder => property.IsAuthorisedLeaseHolder(character),
			PropertyQueryMode.IsTenant => property.Lease?.IsTenant(character, false) == true,
			_ => false
		};
		Result = new BooleanVariable(value);
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"property",
			[ProgVariableTypes.Location],
			(parameters, gameworld) => new PropertyQueryFunction(parameters, gameworld, PropertyQueryMode.ForLocation),
			["location"],
			["The cell for which to find a property."],
			"Returns the property containing the location, or null if the location is not assigned to a property.",
			"Economy",
			ProgVariableTypes.Property));

		Register("ispropertyowner", PropertyQueryMode.IsOwner,
			"Returns true when the character is an authorised owner of the property.");
		Register("ispropertyleaseholder", PropertyQueryMode.IsLeaseholder,
			"Returns true when the character is an authorised leaseholder of the property.");
		Register("ispropertytenant", PropertyQueryMode.IsTenant,
			"Returns true when the character is a declared or indirect tenant of the property's active lease.");
	}

	private static void Register(string name, PropertyQueryMode mode, string help)
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			name,
			[ProgVariableTypes.Property, ProgVariableTypes.Character],
			(parameters, gameworld) => new PropertyQueryFunction(parameters, gameworld, mode),
			["property", "character"],
			["The property to inspect.", "The character to test."],
			help,
			"Economy",
			ProgVariableTypes.Boolean));
	}
}

internal sealed class SendChannelFunction : BuiltInFunction
{
	private SendChannelFunction(IList<IFunction> parameters) : base(parameters)
	{
	}

	public override ProgVariableTypes ReturnType => ProgVariableTypes.Void;

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		if (ParameterFunctions[0].Result?.GetObject is IChannel channel &&
		    ParameterFunctions[1].Result?.GetObject is ICharacter character &&
		    ParameterFunctions[2].Result?.GetObject is string message)
		{
			channel.Send(character, message);
		}

		Result = null;
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"sendchannel",
			[ProgVariableTypes.Channel, ProgVariableTypes.Character, ProgVariableTypes.Text],
			(parameters, _) => new SendChannelFunction(parameters),
			["channel", "character", "message"],
			["The channel to use.", "The character sending the message.", "The message to send."],
			"Sends a channel message using the normal channel speaker, membership, listener, and Discord rules. Returns no value.",
			"Communication",
			ProgVariableTypes.Void));
	}
}
