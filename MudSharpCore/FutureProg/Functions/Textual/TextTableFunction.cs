using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Framework;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.Textual;

internal class TextTableFunction : BuiltInFunction
{
	public TextTableFunction(IList<IFunction> functions, bool blackandwhite)
		: base(functions)
	{
		BlackAndWhite = blackandwhite;
	}

	public bool BlackAndWhite { get; set; }

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"texttable",
			new[]
			{
				ProgVariableTypes.Collection | ProgVariableTypes.Text,
				ProgVariableTypes.Collection | ProgVariableTypes.Text, ProgVariableTypes.Number
			},
			(pars, gameworld) =>
				new TextTableFunction(pars, false),
			new List<string> { "headers", "values", "width" },
			new List<string>
			{
				"A collection of text values to use as the headers of the table",
				"A collection of text values, each of which is a row, with columns separated by the tab (\\t) character",
				"The width of the table"
			},
			"This function displays a coloured 'text table', essentially an ASCII representation of a table. The number of columns in the header MUST match the number of columns in each row.",
			"Text",
			ProgVariableTypes.Text
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"texttable",
			new[]
			{
				ProgVariableTypes.Text, ProgVariableTypes.Collection | ProgVariableTypes.Text,
				ProgVariableTypes.Number
			},
			(pars, gameworld) =>
				new TextTableFunction(pars, false),
			new List<string> { "headers", "values", "width" },
			new List<string>
			{
				"Text with values separated by tab characters (\\t) to use as the headers of the table",
				"A collection of text values, each of which is a row, with columns separated by the tab (\\t) character",
				"The width of the table"
			},
			"This function displays a coloured 'text table', essentially an ASCII representation of a table. The number of columns in the header MUST match the number of columns in each row.",
			"Text",
			ProgVariableTypes.Text
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"bwtexttable",
			new[]
			{
				ProgVariableTypes.Collection | ProgVariableTypes.Text,
				ProgVariableTypes.Collection | ProgVariableTypes.Text, ProgVariableTypes.Number
			},
			(pars, gameworld) =>
				new TextTableFunction(pars, true),
			new List<string> { "headers", "values", "width" },
			new List<string>
			{
				"A collection of text values to use as the headers of the table",
				"A collection of text values, each of which is a row, with columns separated by the tab (\\t) character",
				"The width of the table"
			},
			"This function displays a plain 'text table', essentially an ASCII representation of a table. The number of columns in the header MUST match the number of columns in each row.",
			"Text",
			ProgVariableTypes.Text
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"bwtexttable",
			new[]
			{
				ProgVariableTypes.Text, ProgVariableTypes.Collection | ProgVariableTypes.Text,
				ProgVariableTypes.Number
			},
			(pars, gameworld) =>
				new TextTableFunction(pars, true),
			new List<string> { "headers", "values", "width" },
			new List<string>
			{
				"Text with values separated by tab characters (\\t) to use as the headers of the table",
				"A collection of text values, each of which is a row, with columns separated by the tab (\\t) character",
				"The width of the table"
			},
			"This function displays a plain 'text table', essentially an ASCII representation of a table. The number of columns in the header MUST match the number of columns in each row.",
			"Text",
			ProgVariableTypes.Text
		));
	}

	#region Overrides of Function

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.Text;
		protected set { }
	}

	#region Overrides of BuiltInFunction

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		List<string> headers;
		if (ParameterFunctions[0].ReturnType == (ProgVariableTypes.Collection | ProgVariableTypes.Text))
		{
			headers =
				((IList)ParameterFunctions[0].Result.GetObject).OfType<object>().Select(x => x.ToString()).ToList();
		}
		else
		{
			headers =
				ParameterFunctions[0].Result.GetObject.ToString()
				                     .Split(new[] { '\t' }, StringSplitOptions.None)
				                     .ToList();
		}

		var body =
			((IList)ParameterFunctions[1].Result.GetObject).OfType<object>()
			                                               .Select(x => x.ToString())
			                                               .Select(x =>
				                                               x.Split(new[] { '\t' },
					                                               StringSplitOptions.None).ToList())
			                                               .ToList();
		var linelength = (int)(decimal)ParameterFunctions[2].Result.GetObject;
		Result =
			new TextVariable(StringUtilities.GetTextTable(body, headers, linelength, !BlackAndWhite,
				BlackAndWhite ? null : Telnet.Green));
		return StatementResult.Normal;
	}

	#endregion

	#endregion
}