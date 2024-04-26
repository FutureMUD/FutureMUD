using System;
using MudSharp.Framework;

namespace MudSharp.GameItems.Decorators;

public abstract class StackDecorator : FrameworkItem, IStackDecorator
{
	public static IStackDecorator LoadStackDecorator(MudSharp.Models.StackDecorator proto)
	{
		switch (proto.Type)
		{
			case "Suffix":
				return new SuffixStackDecorator(proto);
			case "Pile":
				return new PileDecorator(proto);
			case "Bites":
				return new BitesDecorator(proto);
			case "Range":
				return new RangeDecorator(proto);
			default:
				throw new NotImplementedException();
		}
	}

	#region IStackDecorator Members

	public abstract string Describe(string name, string description, double quantity);

	public string Description { get; protected init; }

	#endregion
}