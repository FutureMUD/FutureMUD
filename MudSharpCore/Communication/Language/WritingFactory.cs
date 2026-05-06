using MudSharp.Framework;
using MudSharp.Models;
using System;

namespace MudSharp.Communication.Language;

public static class WritingFactory
{
    public static IWriting LoadWriting(Writing writing, IFuturemud gameworld)
    {
        switch (writing.WritingType)
        {
            case "simple":
                return new SimpleWriting(writing, gameworld);
            case "composite":
                return new CompositeWriting(writing, gameworld);
            case "printed":
                return new PrintedWriting(writing, gameworld);
            default:
                throw new ApplicationException(
                    $"Unknown writing type in WritingFactory.LoadWriting: {writing.WritingType}");
        }
    }
}
