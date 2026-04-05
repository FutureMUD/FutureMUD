using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.Logging;

public class LogManager : ILogManager
{
    private readonly Queue<CharacterCommand> _commandQueue = new();

    private readonly bool _logNpcCommands;

    public LogManager(IFuturemud gameworld)
    {
        Gameworld = gameworld;
        _logNpcCommands = Gameworld.GetStaticBool("LogNPCCommands");
    }

    #region IHaveFuturemud Members

    public IFuturemud Gameworld { get; }

    #endregion

    public void LogCharacterCommand(ICharacter character, string command)
    {
        if (character == null)
        {
            return;
        }

        if (!_logNpcCommands && !character.IsPlayerCharacter)
        {
            return;
        }

        _commandQueue.Enqueue(new CharacterCommand
        {
            AccountId = character.Account?.Id == 0 ? default : character.Account?.Id,
            CharacterId = character.Id,
            CellId = character.Location?.Id ?? 0,
            Command = command,
            Time = DateTime.UtcNow,
            IsPlayerCharacter = character.IsPlayerCharacter
        });
    }

    private readonly List<ICustomLogger> _customLoggers = new();

    public void InstallLogger(ICustomLogger logger)
    {
        _customLoggers.Add(logger);
    }

    public void CustomLogEntry(LogEntryType type, params object[] arguments)
    {
        foreach (ICustomLogger logger in _customLoggers)
        {
            logger.HandleLog(type, arguments);
        }
    }

    public void FlushLog()
    {
        if (!_commandQueue.Any())
        {
            return;
        }

        using (new FMDB())
        {
            while (_commandQueue.Any())
            {
                CharacterCommand command = _commandQueue.Dequeue();
                if (command == null || command.CellId == 0)
                {
                    continue;
                }

                CharacterLog dbitem = new();
                FMDB.Context.CharacterLogs.Add(dbitem);
                dbitem.AccountId = command.AccountId;
                dbitem.CharacterId = command.CharacterId;
                dbitem.CellId = command.CellId;
                dbitem.Command = command.Command;
                dbitem.IsPlayerCharacter = command.IsPlayerCharacter;
                dbitem.Time = command.Time;
            }

            FMDB.Context.SaveChanges();
        }

        foreach (ICustomLogger logger in _customLoggers)
        {
            logger.SaveLog();
        }
    }

    internal class CharacterCommand
    {
        public long CharacterId { get; init; }
        public long CellId { get; init; }
        public long? AccountId { get; init; }
        public string Command { get; init; }
        public DateTime Time { get; init; }
        public bool IsPlayerCharacter { get; init; }
    }
}