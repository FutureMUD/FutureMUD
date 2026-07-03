using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.Communication.Language;
using MudSharp.GameItems.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Communication
{
    public interface ICanBeRead : IFrameworkItem, ISaveable
    {
        int DocumentLength { get; }
        ICharacter Author { get; }
        WritingImplementType ImplementType { get; }
        string ParseFor(ICharacter voyeur);
        string DescribeInLook(ICharacter voyeur);
    }

    public interface IReadableContentTemplate
    {
        int DocumentLength { get; }
        ICanBeRead CreateReadable(IFuturemud gameworld);
    }

    public interface IPageReadableContentTemplate : IReadableContentTemplate
    {
        int Page { get; }
        int Order { get; }
    }

    public interface IPageReadable
    {
        int Page { get; }
        int Order { get; }
        ICanBeRead Readable { get; }
    }

    public static class ReadableExtensions
    {
        public static ICanBeRead CopyReadable(this ICanBeRead readable)
        {
            return readable ?? throw new ArgumentNullException(nameof(readable));
        }
    }
}
