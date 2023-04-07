using MudSharp.Accounts;
using MudSharp.Character;

namespace MudSharp.Commands.Trees {
    public interface ICommandTree<T> {
        PermissionLevel PermissionLevel { get; }

        ICommandManager<T> Commands { get; }
    }

    public interface ICommandTree<T, out U> {
        PermissionLevel PermissionLevel { get; }

        ICommandManager<T> Commands { get; }

        U Execute(T subject);
    }

    public interface ICharacterCommandTree : ICommandTree<ICharacter> {
        new ICharacterCommandManager Commands { get; }
    }
}