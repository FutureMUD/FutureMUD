using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Framework;

namespace MudSharp.Construction.Autobuilder;

public class AutobuilderCustomParameter : IAutobuilderParameter, IHaveFuturemud
{
	#region Implementation of IAutobuilderParameter

	public bool IsOptional { get; init; }
	public string TypeName { get; init; }
	public string ParameterName { get; init; }
	public string MissingErrorMessage { get; init; }

	public Func<string, IFuturemud, object[], bool> IsValidArgumentFunction { get; init; }
	public Func<string, IFuturemud, object[], string> WhyIsNotValidArgumentFunction { get; init; }
	public Func<string, IFuturemud, object> GetArgumentFunction { get; init; }

	public bool IsValidArgument(string argument, object[] previousArguments)
	{
		return IsValidArgumentFunction(argument, Gameworld, previousArguments);
	}

	public string WhyIsNotValidArgument(string argument, object[] previousArguments)
	{
		return WhyIsNotValidArgumentFunction(argument, Gameworld, previousArguments);
	}

	public object GetArgument(string argument)
	{
		return GetArgumentFunction(argument, Gameworld);
	}

	#endregion

	#region Implementation of IHaveFuturemud

	public IFuturemud Gameworld { get; init; }

	#endregion
}