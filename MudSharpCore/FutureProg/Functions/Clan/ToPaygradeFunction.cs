﻿using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Community;
using MudSharp.Framework;

namespace MudSharp.FutureProg.Functions.Clan;

internal class ToPaygradeFunction : BuiltInFunction
{
	private readonly IFuturemud _gameworld;

	public ToPaygradeFunction(IList<IFunction> parameters, IFuturemud gameworld)
		: base(parameters)
	{
		_gameworld = gameworld;
	}

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.ClanPaygrade;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var clan = (IClan)ParameterFunctions.ElementAt(0).Result;
		if (clan == null)
		{
			ErrorMessage = "Clan Function in ToPaygrade returned null.";
			return StatementResult.Error;
		}

		Result = ParameterFunctions[0].ReturnType.CompatibleWith(ProgVariableTypes.Text)
			? clan.Paygrades.FirstOrDefault(
				  x =>
					  x.Name.Equals((string)ParameterFunctions.ElementAt(1).Result.GetObject,
						  StringComparison.InvariantCultureIgnoreCase)) ??
			  clan.Paygrades.FirstOrDefault(
				  x =>
					  x.Abbreviation.Equals((string)ParameterFunctions.ElementAt(1).Result.GetObject,
						  StringComparison.InvariantCultureIgnoreCase))
			: clan.Paygrades.FirstOrDefault(
				x => x.Id == (int)(decimal)ParameterFunctions.ElementAt(1).Result.GetObject);

		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"topaygrade",
			new[] { ProgVariableTypes.Clan, ProgVariableTypes.Number },
			(pars, gameworld) => new ToPaygradeFunction(pars, gameworld)
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"topaygrade",
			new[] { ProgVariableTypes.Clan, ProgVariableTypes.Text },
			(pars, gameworld) => new ToPaygradeFunction(pars, gameworld)
		));
	}
}