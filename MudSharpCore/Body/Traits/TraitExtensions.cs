using System;

namespace MudSharp.Body.Traits;

public static class TraitExtensions
{
	public static string Describe(this TraitUseType type)
	{
		switch (type)
		{
			case TraitUseType.Theoretical:
				return "Theoretical";
			case TraitUseType.Practical:
				return "Practical";
			default:
				throw new ArgumentOutOfRangeException(nameof(type), type, null);
		}
	}
}