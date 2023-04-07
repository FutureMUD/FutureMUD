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

	public bool IsOptional { get; set; }
	public string TypeName { get; set; }
	public string ParameterName { get; set; }
	public string MissingErrorMessage { get; set; }

	public Func<string, IFuturemud, object[], bool> IsValidArgumentFunction { get; set; }
	public Func<string, IFuturemud, object[], string> WhyIsNotValidArgumentFunction { get; set; }
	public Func<string, IFuturemud, object> GetArgumentFunction { get; set; }

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

	public IFuturemud Gameworld { get; set; }

	#endregion
}