using System;

namespace MudSharp.Work.Butchering;

public static class ButcheryExtensions
{
	/// <summary>
	/// Returns the string representation of the verb, e.g. Butcher, Salvage etc
	/// </summary>
	/// <param name="verb"></param>
	/// <param name="properCase"></param>
	/// <returns></returns>
	public static string Describe(this ButcheryVerb verb, bool properCase = true)
	{
		switch (verb)
		{
			case ButcheryVerb.Butcher:
				return properCase ? "Butcher" : "butcher";
			case ButcheryVerb.Salvage:
				return properCase ? "Salvage" : "salvage";
		}

		throw new ApplicationException("Unknown ButcheryVerb in ButcheryExtensions.Describe");
	}

	/// <summary>
	/// Returns the gerund of the verb's action, e.g. butchering, salvaging, etc
	/// </summary>
	/// <param name="verb"></param>
	/// <returns></returns>
	public static string DescribeGerund(this ButcheryVerb verb)
	{
		switch (verb)
		{
			case ButcheryVerb.Butcher:
				return "butchering";
			case ButcheryVerb.Salvage:
				return "salvaging";
		}

		throw new ApplicationException("Unknown ButcheryVerb in ButcheryExtensions.DescribeGerund");
	}
}