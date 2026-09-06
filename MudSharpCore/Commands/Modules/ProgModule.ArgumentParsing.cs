#nullable enable

using MudSharp.Communication.Language;
using MudSharp.Effects.Concrete;
using MudSharp.Economy;
using MudSharp.Form.Material;
using MudSharp.Framework.Units;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;

namespace MudSharp.Commands.Modules;

internal partial class ProgModule
{
	private const string ProgArgumentHelp = @"Manual FutureProg values are shared by #3prog execute#0, #3register default#0, #3setregister#0, #3cell set register#0 and computer-program execution.

Use quotes for names or text containing spaces, and parentheses to group a compound argument. #3""""#0 is empty text; #3()#0 is an empty container. Unclosed groups and missing or extra prog arguments are errors.

Numbers, dates and timespans use your locale. Booleans are #3true#0 or #3false#0. Reference types and personal names accept explicit #3null#0. A missing lookup never silently becomes null. Text #3null#0 remains the literal word.

Most world definitions accept IDs or names. Characters and items use normal targeting; #3self#0 selects your character, and locations accept #3here#0, an ID or a room reference. Additional selectors are:
	#3signedvariety#0: global ID, or #3(<language> <variety>)#0
	#3personalname#0: #3(<nameculture> <complete name>)#0
	#3effect#0: #3(<owner> <index>)#0, using a one-based index into the owner's effects
	#3trap#0: #3(<owner> <index>)#0, using a one-based index into only the owner's traps
	#3outfit#0: #3(<character> <outfit name>)#0
	#3outfititem#0: #3(<character> <outfit name> <item ID>)#0
	#3liquidmixture#0: #3(<liquid> <volume>)#0, for example #3(water 250ml)#0, or #3empty#0
	#3bankaccount#0: global account ID or #3bankcode:accountnumber#0 in your gameworld (including inactive accounts)

Qualify union types with a compatible concrete type: #3(liquid water)#0 for Material, #3(terrain forest)#0 for Tagged, or #3(number 5)#0 for ValueType. Perceivable, Perceiver, MagicResourceHaver and Tagged also accept compatible local targets. Toon accepts a character target or #3*<chargen ID>#0.

Container examples (the variable or parameter supplies the element type):
	Text collection: #3(""first value"" ""second value"")#0
	Number dictionary: #3(first 1 second 2)#0
	Text collection dictionary: #3(group (""first value"" second) empty ())#0

Dictionary keys are case-sensitive and must be unique. Quote keys containing spaces. Group each collection-dictionary value with parentheses; nested groups can also contain scoped selectors. Runtime-scoped types can be used for prog arguments but remain unavailable as persistent register values. Void, Error, Anything, Literal and modifier-only types are not input values.";

	private static (object? result, bool success) ArgumentError(ICharacter actor, string error)
	{
		actor.OutputHandler.Send(error);
		return (null, false);
	}

	/// <summary>Reads one command argument, retaining quotes inside balanced parenthesised groups.</summary>
	internal static bool TryPopArgument(ref StringStack input, out string argument, out string error)
	{
		argument = string.Empty;
		error = string.Empty;
		if (input.IsFinished)
		{
			error = "You must supply a value.";
			return false;
		}

		var text = input.RemainingArgument;
		if (text[0] != '(' && text[0] != '"' && text[0] != '“')
		{
			argument = input.PopSpeech();
			return true;
		}

		var grouped = text[0] == '(';
		var depth = 0;
		var quote = '\0';
		for (var i = 0; i < text.Length; i++)
		{
			var ch = text[i];
			if (quote != '\0')
			{
				if (ch != quote)
				{
					continue;
				}
				quote = '\0';
				if (grouped)
				{
					continue;
				}
			}
			else if (ch is '"' or '“')
			{
				quote = ch == '“' ? '”' : '"';
				continue;
			}
			else if (ch == '(')
			{
				depth++;
				continue;
			}
			else if (ch != ')' || --depth != 0)
			{
				continue;
			}

			if (i + 1 < text.Length && !char.IsWhiteSpace(text[i + 1]))
			{
				error = "Separate arguments with a space after closing quotes or parentheses.";
				return false;
			}

			argument = text[1..i];
			input = new StringStack(text[(i + 1)..].TrimStart());
			return true;
		}

		error = "Close all quotes and parentheses in the argument.";
		return false;
	}

	/// <summary>Reads a final value without destroying quotes belonging to container elements or scoped selectors.</summary>
	internal static (object? result, bool success) GetArgumentFromRemainingInput(ProgVariableTypes type,
		StringStack input, int parameter, ICharacter actor)
	{
		if (input.IsFinished)
		{
			return ArgumentError(actor, "You must supply a value.");
		}

		var structured = !type.IsExactType || type == ProgVariableTypes.PersonalName ||
		                 type == ProgVariableTypes.SignedVariety || type == ProgVariableTypes.Effect ||
		                 type == ProgVariableTypes.Trap || type == ProgVariableTypes.Outfit ||
		                 type == ProgVariableTypes.OutfitItem || type == ProgVariableTypes.LiquidMixture;
		var text = input.RemainingArgument;
		if (text.StartsWith('(') && structured || !structured && (text.StartsWith('"') || text.StartsWith('“')))
		{
			if (!TryPopArgument(ref input, out text, out var error))
			{
				return ArgumentError(actor, error);
			}
			if (!input.IsFinished)
			{
				return ArgumentError(actor, "Put the entire value inside the quotes or parentheses.");
			}
		}

		return GetArgument(type, text, parameter, actor);
	}

	private static (object? result, bool success) ResolveContainerArgument(ProgVariableTypes type,
		string text, int parameter, ICharacter actor)
	{
		var modifiers = (type.IsCollection ? 1 : 0) + (type.IsDictionary ? 1 : 0) + (type.IsCollectionDictionary ? 1 : 0);
		var elementType = type.WithoutContainerModifiers();
		if (modifiers != 1 || elementType == ProgVariableTypes.Void || elementType == ProgVariableTypes.Error)
		{
			return ArgumentError(actor, "Specify exactly one container modifier and a valid element type.");
		}

		var input = new StringStack(text.Trim());
		var collection = new List<object?>();
		var dictionary = new Dictionary<string, object?>();
		var collectionDictionary = new CollectionDictionary<string, object?>();
		while (!input.IsFinished)
		{
			if (!TryPopArgument(ref input, out var token, out var error))
			{
				return ArgumentError(actor, error);
			}

			string? key = null;
			if (!type.IsCollection)
			{
				key = token;
				if (dictionary.ContainsKey(key))
				{
					return ArgumentError(actor, $"The key {key.ColourCommand()} appears more than once.");
				}
				dictionary.Add(key, null);
				if (!TryPopArgument(ref input, out token, out error))
				{
					return ArgumentError(actor, $"Supply a value for key {key.ColourCommand()}. {error}");
				}
			}

			var (value, success) = GetArgument(type.IsCollectionDictionary ? elementType | ProgVariableTypes.Collection : elementType,
				token, parameter, actor);
			if (!success)
			{
				return (null, false);
			}

			if (type.IsCollection)
			{
				collection.Add(value);
			}
			else if (type.IsDictionary)
			{
				dictionary[key!] = value;
			}
			else
			{
				collectionDictionary.AddRange(key!, (List<object?>)value!);
			}
		}

		if (type.IsCollection) return (collection, true);
		if (type.IsDictionary) return (dictionary, true);
		return (collectionDictionary, true);
	}

	private static (object? result, bool success) ResolveUnionArgument(ProgVariableTypes type,
		string text, int parameter, ICharacter actor)
	{
		// Explicit types disambiguate unions such as Material and Tagged, and permit primitive union values.
		var input = new StringStack(text);
		var exactType = FutureProg.FutureProg.GetTypeByName(input.PopSpeech());
		if (!input.IsFinished && exactType.IsExactType && exactType != ProgVariableTypes.Void && exactType != ProgVariableTypes.Error)
		{
			if (!exactType.CompatibleWith(type))
			{
				return ArgumentError(actor, $"{exactType.Describe()} is not compatible with {type.Describe()}.");
			}
			var (value, success) = GetArgumentFromRemainingInput(exactType, input, parameter, actor);
			return success ? (FutureProg.FutureProg.GetVariable(exactType, value), true) : (null, false);
		}

		if (type == ProgVariableTypes.Toon)
		{
			return text.StartsWith('*')
				? GetArgument(ProgVariableTypes.Chargen, text[1..], parameter, actor)
				: GetArgument(ProgVariableTypes.Character, text, parameter, actor);
		}

		if (type == ProgVariableTypes.Perceivable || type == ProgVariableTypes.Perceiver ||
		    type == ProgVariableTypes.MagicResourceHaver || type == ProgVariableTypes.Tagged)
		{
			IPerceivable? target = text.EqualTo("here") ? actor.Location : text.EqualTo("self") ? actor : actor.Target(text);
			if (target is null || !target.Type.CompatibleWith(type))
			{
				return ArgumentError(actor, $"There is no compatible {type.Describe()} target. You can also use (<type> <target>)." );
			}
			return (target, true);
		}

		return ArgumentError(actor, $"Specify a concrete type followed by its value for {type.Describe()}, for example (number 5) or (liquid water).");
	}

	private static bool TryReadScopedArguments(string text, int count, ICharacter actor, out List<string> arguments)
	{
		arguments = new List<string>();
		var input = new StringStack(text);
		while (!input.IsFinished)
		{
			if (!TryPopArgument(ref input, out var value, out var error))
			{
				actor.OutputHandler.Send(error);
				return false;
			}
			arguments.Add(value);
		}
		if (arguments.Count == count) return true;
		actor.OutputHandler.Send($"This selector requires {count.ToString("N0", actor)} arguments. Quote names containing spaces.");
		return false;
	}

	private static (object? result, bool success) ResolveSignedVarietyArgument(string text, int parameter, ICharacter actor)
	{
		if (long.TryParse(text, out var id))
		{
			return ResolveFrameworkItemArgument(actor, actor.Gameworld.SignedLanguages.SelectMany(x => x.Varieties).Get(id),
				"signed variety", string.Empty);
		}
		if (!TryReadScopedArguments(text, 2, actor, out var args)) return (null, false);
		var language = actor.Gameworld.SignedLanguages.GetByIdOrName(args[0]);
		return ResolveFrameworkItemArgument(actor, language?.Varieties.GetByIdOrName(args[1]), "signed variety", string.Empty);
	}

	private static (object? result, bool success) ResolveEffectArgument(ProgVariableTypes type, string text,
		int parameter, ICharacter actor)
	{
		if (!TryReadScopedArguments(text, 2, actor, out var args)) return (null, false);
		var (owner, success) = GetArgument(ProgVariableTypes.Perceivable, args[0], parameter, actor);
		if (!success) return (null, false);
		if (owner is not IPerceivable perceivable || !int.TryParse(args[1], out var index) || index < 1)
		{
			return ArgumentError(actor, "Specify a non-null owner and a positive, one-based effect or trap index.");
		}

		if (type == ProgVariableTypes.Trap)
		{
			var trap = perceivable.Effects.OfType<TrapEffect>().ElementAtOrDefault(index - 1);
			return trap is null ? ArgumentError(actor, "There is no trap at that index on the owner.") : (new TrapVariable(trap), true);
		}
		var effect = perceivable.Effects.ElementAtOrDefault(index - 1);
		return effect is null ? ArgumentError(actor, "There is no effect at that index on the owner.") : (effect, true);
	}

	private static (object? result, bool success) ResolveOutfitArgument(ProgVariableTypes type, string text,
		int parameter, ICharacter actor)
	{
		if (!TryReadScopedArguments(text, type == ProgVariableTypes.Outfit ? 2 : 3, actor, out var args)) return (null, false);
		var (owner, success) = GetArgument(ProgVariableTypes.Character, args[0], parameter, actor);
		if (!success) return (null, false);
		var outfit = (owner as ICharacter)?.Outfits.FirstOrDefault(x => x.Name.EqualTo(args[1]));
		if (outfit is null) return ArgumentError(actor, "There is no such outfit on that character.");
		if (type == ProgVariableTypes.Outfit) return (outfit, true);
		if (!long.TryParse(args[2], out var id)) return ArgumentError(actor, "Specify the outfit item's ID.");
		var item = outfit.Items.FirstOrDefault(x => x.Id == id);
		return item is null ? ArgumentError(actor, "There is no such item in that outfit.") : (item, true);
	}

	private static (object? result, bool success) ResolveLiquidMixtureArgument(string text, int parameter, ICharacter actor)
	{
		if (text.EqualTo("empty")) return (LiquidMixture.CreateEmpty(actor.Gameworld), true);
		var input = new StringStack(text);
		if (!TryPopArgument(ref input, out var liquidText, out var error)) return ArgumentError(actor, error);
		var liquid = actor.Gameworld.Liquids.GetByIdOrName(liquidText);
		if (liquid is null) return ArgumentError(actor, "There is no such liquid. Use <liquid> <volume>, or empty.");
		if (!actor.Gameworld.UnitManager.TryGetBaseUnits(input.SafeRemainingArgument, UnitType.FluidVolume, actor, out var volume) ||
		    !double.IsFinite(volume) || volume <= 0.0)
		{
			return ArgumentError(actor, "Specify a positive liquid volume, for example (water 250ml).");
		}
		return (new LiquidMixture(liquid, volume, actor.Gameworld), true);
	}

	private static (object? result, bool success) ResolveBankAccountArgument(string text, ICharacter actor)
	{
		if (long.TryParse(text, out var id))
		{
			return ResolveFrameworkItemArgument(actor, actor.Gameworld.BankAccounts.Get(id), "bank account", string.Empty);
		}

		var separator = text.IndexOf(':');
		if (separator <= 0 || !int.TryParse(text[(separator + 1)..], out var number) || number <= 0)
		{
			return ArgumentError(actor, "Specify a bank account ID or bankcode:accountnumber.");
		}

		var bankText = text[..separator];
		var bank = actor.Gameworld.Banks.GetByIdOrName(bankText);
		if (bank is null)
		{
			var banks = actor.Gameworld.Banks.Where(x => x.Code.EqualTo(bankText)).ToList();
			if (banks.Count == 0)
			{
				banks = actor.Gameworld.Banks.Where(x => x.Code.StartsWith(bankText, StringComparison.OrdinalIgnoreCase)).ToList();
			}
			if (banks.Count != 1) return ArgumentError(actor, "Specify an unambiguous bank name, ID or code.");
			bank = banks[0];
		}

		return ResolveFrameworkItemArgument(actor, bank.BankAccounts.FirstOrDefault(x => x.AccountNumber == number), "bank account", string.Empty);
	}
}
