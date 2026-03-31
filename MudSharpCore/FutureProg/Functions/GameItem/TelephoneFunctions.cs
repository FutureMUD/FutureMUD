using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;

namespace MudSharp.FutureProg.Functions.GameItem;

internal class PhoneNumberFunction : BuiltInFunction
{
	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"phonenumber",
				new[] { ProgVariableTypes.Item },
				(pars, gameworld) => new PhoneNumberFunction(pars),
				["item"],
				["The item whose current phone number you want to retrieve"],
				"This function returns the active telephone number for a phone or telecom endpoint item, if any.",
				"Items",
				ProgVariableTypes.Text
			)
		);
	}

	protected PhoneNumberFunction(IList<IFunction> parameterFunctions) : base(parameterFunctions)
	{
	}

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.Text;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		if (ParameterFunctions[0].Result?.GetObject is not IGameItem item)
		{
			Result = new NullVariable(ProgVariableTypes.Text);
			return StatementResult.Normal;
		}

		var phoneNumber = item.GetItemType<ITelephone>()?.PhoneNumber ??
		                  item.GetItemType<ITelephoneNumberOwner>()?.PhoneNumber;
		Result = phoneNumber != null
			? new TextVariable(phoneNumber)
			: new NullVariable(ProgVariableTypes.Text);
		return StatementResult.Normal;
	}
}

internal class SetPhoneNumberFunction : BuiltInFunction
{
	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"setphonenumber",
				new[] { ProgVariableTypes.Item, ProgVariableTypes.Text },
				(pars, gameworld) => new SetPhoneNumberFunction(pars),
				new List<string> { "item", "number" },
				new List<string>
				{
					"The item whose number you want to set",
					"The number to assign, or an empty string to return to automatic assignment"
				},
				"This function sets the preferred telephone number for a phone or telecom endpoint. It returns true if the assignment succeeded.",
				"Items",
				ProgVariableTypes.Boolean
			)
		);
	}

	protected SetPhoneNumberFunction(IList<IFunction> parameterFunctions) : base(parameterFunctions)
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

		if (ParameterFunctions[0].Result?.GetObject is not IGameItem item)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		var owner = item.GetItemType<ITelephoneNumberOwner>() ?? item.GetItemType<ITelephone>()?.NumberOwner;
		if (owner == null)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		var text = ParameterFunctions[1].Result?.GetObject?.ToString() ?? string.Empty;
		if (string.IsNullOrWhiteSpace(text))
		{
			owner.PreferredNumber = null;
			Result = new BooleanVariable(true);
			return StatementResult.Normal;
		}

		var normalised = new string(text.Where(char.IsDigit).ToArray());
		if (string.IsNullOrWhiteSpace(normalised))
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		if (owner.TelecommunicationsGrid != null &&
		    !owner.TelecommunicationsGrid.RequestNumber(owner, normalised, owner.AllowSharedNumber, false))
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		owner.PreferredNumber = normalised;
		Result = new BooleanVariable(true);
		return StatementResult.Normal;
	}
}

internal class SetPhoneSharedNumberFunction : BuiltInFunction
{
	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"setphonesharednumber",
				new[] { ProgVariableTypes.Item, ProgVariableTypes.Boolean },
				(pars, gameworld) => new SetPhoneSharedNumberFunction(pars),
				new List<string> { "item", "shared" },
				new List<string>
				{
					"The item whose phone endpoint should allow or deny shared numbers",
					"Whether this endpoint should permit a number to be shared with other endpoints"
				},
				"This function toggles whether a phone or telecom endpoint permits shared-number assignment. It returns true if the item supported telephone numbering.",
				"Items",
				ProgVariableTypes.Boolean
			)
		);
	}

	protected SetPhoneSharedNumberFunction(IList<IFunction> parameterFunctions) : base(parameterFunctions)
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

		if (ParameterFunctions[0].Result?.GetObject is not IGameItem item)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		var owner = item.GetItemType<ITelephoneNumberOwner>() ?? item.GetItemType<ITelephone>()?.NumberOwner;
		if (owner == null)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		var shared = Convert.ToBoolean(ParameterFunctions[1].Result?.GetObject ?? false);
		owner.AllowSharedNumber = shared;
		Result = new BooleanVariable(true);
		return StatementResult.Normal;
	}
}
