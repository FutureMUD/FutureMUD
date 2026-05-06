using MudSharp.Communication.Language;
using MudSharp.Form.Colour;
using MudSharp.Framework;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.FutureProg.Functions.GameItem;

internal class AddPrintedWritingFunction : BuiltInFunction
{
	private readonly IFuturemud _gameworld;
	private readonly bool _useAdvancedOptions;

	public AddPrintedWritingFunction(IList<IFunction> parameters, IFuturemud gameworld, bool useAdvancedOptions)
		: base(parameters)
	{
		_gameworld = gameworld;
		_useAdvancedOptions = useAdvancedOptions;
	}

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.Boolean;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var item = ParameterFunctions[0].Result.GetObject as IGameItem;
		var book = item?.GetItemType<BookGameItemComponent>();
		if (book is null)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		var page = (int)(decimal)ParameterFunctions[1].Result.GetObject;
		var text = (string)ParameterFunctions[2].Result.GetObject;
		var language = ParameterFunctions[3].Result.GetObject as ILanguage;
		var script = ParameterFunctions[4].Result.GetObject as IScript;
		var provenance = (string)ParameterFunctions[5].Result.GetObject;
		if (language is null || script is null)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		var colour = _gameworld.Colours.Get(_gameworld.GetStaticLong("DefaultWritingColourInText"));
		if (colour is null)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		var style = WritingStyleDescriptors.MachinePrinted;
		if (_useAdvancedOptions)
		{
			if (!TryGetColour((string)ParameterFunctions[6].Result.GetObject, out colour) ||
			    !TryGetStyle((string)ParameterFunctions[7].Result.GetObject, out style))
			{
				Result = new BooleanVariable(false);
				return StatementResult.Normal;
			}
		}

		var writing = new PrintedWriting(_gameworld, text, language, script, provenance, style, colour);
		if (!book.CanAddWriting(writing, page))
		{
			_gameworld.SaveManager.Abort(writing);
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		Result = new BooleanVariable(book.AddWriting(writing, page));
		return StatementResult.Normal;
	}

	private bool TryGetColour(string text, out IColour colour)
	{
		colour = _gameworld.Colours.Get(_gameworld.GetStaticLong("DefaultWritingColourInText"));
		if (string.IsNullOrWhiteSpace(text))
		{
			return colour is not null;
		}

		colour = long.TryParse(text, out var value)
			? _gameworld.Colours.Get(value)
			: _gameworld.Colours.GetByName(text);
		return colour is not null;
	}

	private static bool TryGetStyle(string text, out WritingStyleDescriptors style)
	{
		style = WritingStyleDescriptors.MachinePrinted;
		if (string.IsNullOrWhiteSpace(text))
		{
			return true;
		}

		var parsed = WritingStyleDescriptors.None.Parse(text);
		if (parsed != WritingStyleDescriptors.None)
		{
			style = parsed;
			return true;
		}

		var result = WritingStyleDescriptors.None;
		foreach (var item in text.Split(',', '|', '+').Select(x => x.Trim()).Where(x => x.Length > 0))
		{
			parsed = WritingStyleDescriptors.None.Parse(item);
			if (parsed == WritingStyleDescriptors.None)
			{
				return false;
			}

			result |= parsed;
		}

		if (result == WritingStyleDescriptors.None)
		{
			return false;
		}

		style = result;
		return true;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"addprintedwriting",
			new[]
			{
				ProgVariableTypes.Item, ProgVariableTypes.Number, ProgVariableTypes.Text, ProgVariableTypes.Language,
				ProgVariableTypes.Script, ProgVariableTypes.Text
			},
			(pars, gameworld) => new AddPrintedWritingFunction(pars, gameworld, false),
			new List<string> { "item", "page", "text", "language", "script", "provenance" },
			new List<string>
			{
				"The book item to receive the printed writing",
				"The page number to write to",
				"The printed text to add",
				"The language of the printed text",
				"The script of the printed text",
				"The publisher, source, or provenance text to display for this writing"
			},
			"Adds a new piece of provenance-aware printed writing to a specific page of a book, returning false if the target is not a book, the page is invalid, or the text will not fit.",
			"Items",
			ProgVariableTypes.Boolean
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"addprintedwriting",
			new[]
			{
				ProgVariableTypes.Item, ProgVariableTypes.Number, ProgVariableTypes.Text, ProgVariableTypes.Language,
				ProgVariableTypes.Script, ProgVariableTypes.Text, ProgVariableTypes.Text, ProgVariableTypes.Text
			},
			(pars, gameworld) => new AddPrintedWritingFunction(pars, gameworld, true),
			new List<string> { "item", "page", "text", "language", "script", "provenance", "colour", "style" },
			new List<string>
			{
				"The book item to receive the printed writing",
				"The page number to write to",
				"The printed text to add",
				"The language of the printed text",
				"The script of the printed text",
				"The publisher, source, or provenance text to display for this writing",
				"The colour name or ID for the printed text",
				"The writing style descriptor, or multiple descriptors separated by commas, pipes, or plus signs"
			},
			"Adds a new piece of provenance-aware printed writing with explicit colour and style to a specific page of a book, returning false if validation fails.",
			"Items",
			ProgVariableTypes.Boolean
		));
	}
}

internal class CopyWritingToFunction : BuiltInFunction
{
	public CopyWritingToFunction(IList<IFunction> parameters)
		: base(parameters)
	{
	}

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.Boolean;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var item = ParameterFunctions[0].Result.GetObject as IGameItem;
		var book = item?.GetItemType<BookGameItemComponent>();
		var page = (int)(decimal)ParameterFunctions[1].Result.GetObject;
		var writing = ParameterFunctions[2].Result.GetObject as IWriting;
		if (book is null || writing is null || !book.CanAddWriting(writing, page))
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		var copy = writing.Copy();
		var result = book.AddWriting(copy, page);
		if (!result)
		{
			copy.Gameworld.SaveManager.Abort(copy);
		}

		Result = new BooleanVariable(result);
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"copywritingto",
			new[] { ProgVariableTypes.Item, ProgVariableTypes.Number, ProgVariableTypes.Writing },
			(pars, _) => new CopyWritingToFunction(pars),
			new List<string> { "item", "page", "writing" },
			new List<string>
			{
				"The book item to receive the writing copy",
				"The page number to copy to",
				"The existing writing to copy"
			},
			"Copies an existing writing onto a specific page of a book, returning false if the target is not a book, the page is invalid, or the writing will not fit.",
			"Items",
			ProgVariableTypes.Boolean
		));
	}
}

internal class SetBookTitleFunction : BuiltInFunction
{
	public SetBookTitleFunction(IList<IFunction> parameters)
		: base(parameters)
	{
	}

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.Boolean;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var item = ParameterFunctions[0].Result.GetObject as IGameItem;
		var book = item?.GetItemType<BookGameItemComponent>();
		if (book is null)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		book.Title = (string)ParameterFunctions[1].Result.GetObject;
		book.Changed = true;
		Result = new BooleanVariable(true);
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"setbooktitle",
			new[] { ProgVariableTypes.Item, ProgVariableTypes.Text },
			(pars, _) => new SetBookTitleFunction(pars),
			new List<string> { "item", "title" },
			new List<string>
			{
				"The book item whose title should be changed",
				"The new title to apply"
			},
			"Sets the title on a book item and returns false if the supplied item is not a book.",
			"Items",
			ProgVariableTypes.Boolean
		));
	}
}
