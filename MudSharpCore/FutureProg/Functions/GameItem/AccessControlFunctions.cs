#nullable enable

using MudSharp.Character;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;

namespace MudSharp.FutureProg.Functions.GameItem;

internal enum AccessControlOperation
{
	KeypadCode,
	SetKeypadCode,
	BiometricAdd,
	BiometricRemove,
	BiometricClear,
	BiometricAllows,
	BiometricIds,
	KeycardAddCode,
	KeycardRemoveCode,
	KeycardClearCodes,
	KeycardHasCode,
	KeycardCodes,
	ReaderAddCode,
	ReaderRemoveCode,
	ReaderClearCodes,
	ReaderAcceptsCode,
	ReaderCodes
}

internal class AccessControlFunction : BuiltInFunction
{
	private readonly AccessControlOperation _operation;
	private readonly ProgVariableTypes _returnType;

	private AccessControlFunction(IList<IFunction> parameters, AccessControlOperation operation,
		ProgVariableTypes returnType) : base(parameters)
	{
		_operation = operation;
		_returnType = returnType;
	}

	public override ProgVariableTypes ReturnType
	{
		get => _returnType;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var item = ParameterFunctions[0].Result?.GetObject as IGameItem;
		var text = ParameterFunctions.Count > 1 ? ParameterFunctions[1].Result?.GetObject?.ToString() ?? string.Empty : string.Empty;
		var character = ParameterFunctions.Count > 1 ? ParameterFunctions[1].Result?.GetObject as ICharacter : null;
		Result = _operation switch
		{
			AccessControlOperation.KeypadCode => new TextVariable(item?.GetItemType<IKeypad>()?.Code ?? string.Empty),
			AccessControlOperation.SetKeypadCode => Boolean(item?.GetItemType<IKeypad>()?.TrySetCode(text, out _) == true),
			AccessControlOperation.BiometricAdd => Boolean(character is not null &&
				item?.GetItemType<IBiometricScanner>()?.AddAuthorisedPerson(character, out _) == true),
			AccessControlOperation.BiometricRemove => Boolean(character is not null && RemoveBiometric(item, character)),
			AccessControlOperation.BiometricClear => Boolean(item?.GetItemType<IBiometricScanner>()?.ClearAuthorisedPeople() == true),
			AccessControlOperation.BiometricAllows => Boolean(character is not null && AllowsBiometric(item, character)),
			AccessControlOperation.BiometricIds => BiometricIds(item),
			AccessControlOperation.KeycardAddCode => Boolean(item?.GetItemType<IKeycard>()?.AddCode(text, out _) == true),
			AccessControlOperation.KeycardRemoveCode => Boolean(item?.GetItemType<IKeycard>()?.RemoveCode(text, out _) == true),
			AccessControlOperation.KeycardClearCodes => Boolean(item?.GetItemType<IKeycard>()?.ClearCodes() == true),
			AccessControlOperation.KeycardHasCode => Boolean(item?.GetItemType<IKeycard>()?.HasCode(text) == true),
			AccessControlOperation.KeycardCodes => KeycardCodes(item),
			AccessControlOperation.ReaderAddCode => Boolean(item?.GetItemType<IKeycardScanner>()?.AddAcceptedCode(text, out _) == true),
			AccessControlOperation.ReaderRemoveCode => Boolean(item?.GetItemType<IKeycardScanner>()?.RemoveAcceptedCode(text, out _) == true),
			AccessControlOperation.ReaderClearCodes => Boolean(item?.GetItemType<IKeycardScanner>()?.ClearAcceptedCodes() == true),
			AccessControlOperation.ReaderAcceptsCode => Boolean(item?.GetItemType<IKeycardScanner>()?.AcceptsCode(text) == true),
			AccessControlOperation.ReaderCodes => ReaderCodes(item),
			_ => Boolean(false)
		};
		return StatementResult.Normal;
	}

	private static BooleanVariable Boolean(bool value) => new(value);

	private static bool RemoveBiometric(IGameItem? item, ICharacter character)
	{
		var scanner = item?.GetItemType<IBiometricScanner>();
		return scanner?.RemoveAuthorisedPerson(CharacterInstanceIdentityComparer.IdentityId(character), out _) == true;
	}

	private static bool AllowsBiometric(IGameItem? item, ICharacter character) =>
		item?.GetItemType<IBiometricScanner>()?.IsAuthorised(CharacterInstanceIdentityComparer.IdentityId(character)) == true;

	private static CollectionVariable BiometricIds(IGameItem? item) =>
		new((item?.GetItemType<IBiometricScanner>()?.AuthorisedPeople ?? [])
			.Select(x => (IProgVariable)new NumberVariable(x.CharacterId)).ToList(), ProgVariableTypes.Number);

	private static CollectionVariable KeycardCodes(IGameItem? item) =>
		new((item?.GetItemType<IKeycard>()?.Codes ?? [])
			.Select(x => (IProgVariable)new TextVariable(x)).ToList(), ProgVariableTypes.Text);

	private static CollectionVariable ReaderCodes(IGameItem? item) =>
		new((item?.GetItemType<IKeycardScanner>()?.AcceptedCodes ?? [])
			.Select(x => (IProgVariable)new TextVariable(x)).ToList(), ProgVariableTypes.Text);

	public static void RegisterFunctionCompiler()
	{
		Register("keypadcode", [ProgVariableTypes.Item], AccessControlOperation.KeypadCode,
			ProgVariableTypes.Text, ["item"], ["The item containing the keypad"],
			"Returns the keypad's current per-item code, or empty text for an incompatible item.");
		Register("setkeypadcode", [ProgVariableTypes.Item, ProgVariableTypes.Text], AccessControlOperation.SetKeypadCode,
			ProgVariableTypes.Boolean, ["item", "code"], ["The item containing the keypad", "The new numeric code"],
			"Changes a keypad's runtime code without emitting an access pulse.");
		RegisterCharacter("biometricadd", AccessControlOperation.BiometricAdd, "Adds a character identity to a biometric scanner.");
		RegisterCharacter("biometricremove", AccessControlOperation.BiometricRemove, "Removes a character identity from a biometric scanner.");
		Register("biometricclear", [ProgVariableTypes.Item], AccessControlOperation.BiometricClear,
			ProgVariableTypes.Boolean, ["item"], ["The biometric scanner item"], "Removes all identities from a biometric scanner.");
		RegisterCharacter("biometricallows", AccessControlOperation.BiometricAllows, "Returns whether a biometric scanner allows a character identity.");
		Register("biometricids", [ProgVariableTypes.Item], AccessControlOperation.BiometricIds,
			ProgVariableTypes.Number | ProgVariableTypes.Collection, ["item"], ["The biometric scanner item"],
			"Returns the stable identity IDs authorised by a biometric scanner.");
		RegisterCode("keycardaddcode", AccessControlOperation.KeycardAddCode, "Adds a case-sensitive code to a keycard.");
		RegisterCode("keycardremovecode", AccessControlOperation.KeycardRemoveCode, "Removes a case-sensitive code from a keycard.");
		Register("keycardclearcodes", [ProgVariableTypes.Item], AccessControlOperation.KeycardClearCodes,
			ProgVariableTypes.Boolean, ["item"], ["The keycard item"], "Removes every code from a keycard.");
		RegisterCode("keycardhascode", AccessControlOperation.KeycardHasCode, "Returns whether a keycard has a case-sensitive code.");
		Register("keycardcodes", [ProgVariableTypes.Item], AccessControlOperation.KeycardCodes,
			ProgVariableTypes.Text | ProgVariableTypes.Collection, ["item"], ["The keycard item"], "Returns all codes stored on a keycard.");
		RegisterCode("keycardreaderaddcode", AccessControlOperation.ReaderAddCode, "Adds a case-sensitive accepted code to a keycard reader.");
		RegisterCode("keycardreaderremovecode", AccessControlOperation.ReaderRemoveCode, "Removes an accepted code from a keycard reader.");
		Register("keycardreaderclearcodes", [ProgVariableTypes.Item], AccessControlOperation.ReaderClearCodes,
			ProgVariableTypes.Boolean, ["item"], ["The keycard reader item"], "Removes every accepted code from a keycard reader.");
		RegisterCode("keycardreaderacceptscode", AccessControlOperation.ReaderAcceptsCode, "Returns whether a keycard reader accepts a case-sensitive code.");
		Register("keycardreadercodes", [ProgVariableTypes.Item], AccessControlOperation.ReaderCodes,
			ProgVariableTypes.Text | ProgVariableTypes.Collection, ["item"], ["The keycard reader item"], "Returns all codes accepted by a keycard reader.");
	}

	private static void RegisterCharacter(string name, AccessControlOperation operation, string description) =>
		Register(name, [ProgVariableTypes.Item, ProgVariableTypes.Character], operation, ProgVariableTypes.Boolean,
			["item", "character"], ["The biometric scanner item", "The character identity"], description);

	private static void RegisterCode(string name, AccessControlOperation operation, string description) =>
		Register(name, [ProgVariableTypes.Item, ProgVariableTypes.Text], operation, ProgVariableTypes.Boolean,
			["item", "code"], ["The keycard or keycard reader item", "The case-sensitive code"], description);

	private static void Register(string name, ProgVariableTypes[] parameters, AccessControlOperation operation,
		ProgVariableTypes returnType, string[] parameterNames, string[] parameterDescriptions, string description)
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(name, parameters,
			(pars, gameworld) => new AccessControlFunction(pars, operation, returnType), parameterNames,
			parameterDescriptions, description, "Items", returnType));
	}
}
