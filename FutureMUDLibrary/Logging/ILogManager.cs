using MudSharp.Character;
using MudSharp.Framework;

namespace MudSharp.Logging
{
    public interface ILogManager : IHaveFuturemud
    {
        void LogCharacterCommand(ICharacter character, string command);
        void InstallLogger(ICustomLogger logger);
        void CustomLogEntry(LogEntryType type, params object[] arguments);
        void FlushLog();
    }
}