using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Framework;

namespace MudSharp.Construction.Autobuilder;

public class AutobuilderTerrainParameter : IAutobuilderParameter
{
	public AutobuilderTerrainParameter(string parameterName, string missingErrorMessage, bool isOptional,
		IFuturemud gameworld)
	{
		ParameterName = parameterName;
		MissingErrorMessage = missingErrorMessage;
		IsOptional = isOptional;
		Gameworld = gameworld;
	}

	#region Implementation of IAutobuilderParameter

	public bool IsOptional { get; set; }
	public string TypeName => "Terrain Type";
	public string ParameterName { get; set; }
	public string MissingErrorMessage { get; set; }

	public bool IsValidArgument(string argument, object[] previousArguments)
	{
		return long.TryParse(argument, out var value)
			? Gameworld.Terrains.Has(value)
			: Gameworld.Terrains.Has(argument);
	}

	public string WhyIsNotValidArgument(string argument, object[] previousArguments)
	{
		if (long.TryParse(argument, out _))
		{
			return
				$"There is no terrain type with the ID number {argument.Colour(Telnet.Red)} to satisfy parameter {ParameterName.Colour(Telnet.Cyan)}.";
		}

		return
			$"There is no terrain type with the name {argument.Colour(Telnet.Red)} to satisfy parameter {ParameterName.Colour(Telnet.Cyan)}.";
	}

	public object GetArgument(string argument)
	{
		return long.TryParse(argument, out var value)
			? Gameworld.Terrains.Get(value)
			: Gameworld.Terrains.GetByName(argument);
	}

	#endregion

	#region Implementation of IHaveFuturemud

	public IFuturemud Gameworld { get; }

	#endregion
}