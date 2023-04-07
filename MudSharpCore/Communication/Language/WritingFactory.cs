using System;
using MudSharp.Models;
using MudSharp.Framework;

namespace MudSharp.Communication.Language;

public static class WritingFactory
{
	public static IWriting LoadWriting(Writing writing, IFuturemud gameworld)
	{
		switch (writing.WritingType)
		{
			case "simple":
				return new SimpleWriting(writing, gameworld);
			default:
				throw new ApplicationException(
					$"Unknown writing type in WritingFactory.LoadWriting: {writing.WritingType}");
		}
	}
}