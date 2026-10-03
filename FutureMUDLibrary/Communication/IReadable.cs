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
        long? AuthorId => Author?.Id;
        ArchivedCharacterIdentity ArchivedAuthor => null;
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
        public static string AuthorName(this ICanBeRead readable,
            MudSharp.Character.Name.NameStyle style = MudSharp.Character.Name.NameStyle.FullName)
        {
            var author = readable.Author;
            if (author is not null) return author.PersonalName?.GetName(style) ?? author.Name;
            var archived = readable.ArchivedAuthor;
            return (style == MudSharp.Character.Name.NameStyle.FullName ? archived?.FullName : null) ?? archived?.DisplayName ??
                (readable.AuthorId is > 0 ? $"Unknown author #{readable.AuthorId}" : "Printed/Anonymous");
        }

        public static ICanBeRead CopyReadable(this ICanBeRead readable)
        {
            return readable ?? throw new ArgumentNullException(nameof(readable));
        }
    }
}
