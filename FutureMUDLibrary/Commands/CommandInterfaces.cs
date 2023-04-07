using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Commands.Modules;
using MudSharp.Commands.Socials;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;

namespace MudSharp.Commands {
    public interface IExecutable : IReportCommands {
        delegate void CommandMethodDelegate(string playerInput);

        string Name { get; }

        PermissionLevel PermissionRequired { get; }

        string FailedToFindCommand { get; }

        bool ConsumesUsedCommand { get; }

        CharacterState AllowedStates { get; }

        bool Execute(string playerInput, CharacterState state = CharacterState.Any, PermissionLevel permissionLevel = PermissionLevel.Any,
            IOutputHandler outputHandler = null);
    }

    public interface IExecutable<in T> : IReportCommands {
        delegate void CommandGenericMethodDelegate(T argument, string playerInput);
        string Name { get; }

        PermissionLevel PermissionRequired { get; }

        CommandDisplayOptions DisplayOptions { get; }

        string FailedToFindCommand { get; }

        bool ConsumesUsedCommand { get; }

        CharacterState AllowedStates { get; }

        string DelayType { get; }

        string DelayErrorMessage { get; }
        string DelayExceptionType { get; }

        bool NoHideCommand { get; }

        bool NoCombatCommand { get; }

        bool NoMovementCommand { get; }
        
        bool NoMeleeCombatCommand { get; }

        bool MustBeAnEnforcer { get; }

        ICommandHelpInfo HelpInfo { get; }
        Func<object,string,bool> AppearInCommandsListCondition { get; }

        string ModuleName { get; }

        bool Execute(T argument, string playerInput, CharacterState state = CharacterState.Any,
            PermissionLevel permissionLevel = PermissionLevel.Any, IOutputHandler outputHandler = null);
    }

    public interface ICommandManager : IExecutable {
        PermissionLevel HasPermissionUpTo { get; }
        IReadOnlyDictionary<string, IExecutable> Commands {get;}
        void Add(string entry, CommandMethodDelegate method, CharacterState state = CharacterState.Any,
            PermissionLevel permissionRequired = PermissionLevel.Any);

        void Add(IEnumerable<string> entries, CommandMethodDelegate method, CharacterState state = CharacterState.Any,
            PermissionLevel permissionRequired = PermissionLevel.Any);

        void Add(string entry, IExecutable command);
        void Add(IEnumerable<string> entries, IExecutable command);
        void AddFrom(ICommandManager other);
        void Override(string entry, IExecutable command);
        void Override(string[] entries, IExecutable command);
        bool Remove(string entry, IExecutable command);
        bool Remove(IEnumerable<string> entries, IExecutable command);
    }

    public interface ICommandManager<T> : IExecutable<T> {
        PermissionLevel HasPermissionUpTo { get; }
        IReadOnlyDictionary<string, IExecutable<T>> TCommands {get;}
        void Add(string entry, CommandGenericMethodDelegate method, CharacterState state = CharacterState.Any,
            PermissionLevel permissionRequired = PermissionLevel.Any);

        void Add(IEnumerable<string> entries, CommandGenericMethodDelegate method, CharacterState state = CharacterState.Any,
            PermissionLevel permissionRequired = PermissionLevel.Any);

        void Add(string entry, IExecutable<T> command);
        void Add(IEnumerable<string> entries, IExecutable<T> command);
        void AddFrom(ICommandManager<T> other);
        void Override(string entry, IExecutable<T> command);
        void Override(string[] entries, IExecutable<T> command);
        bool Remove(string entry, IExecutable<T> command);
        bool Remove(IEnumerable<string> entries, IExecutable<T> command);
        IExecutable<T> LocateCommand(T reference, ref string input);
    }

    

    public interface ICharacterCommandManager : ICommandManager<ICharacter>
    {
        IEnumerable<ISocial> Socials { get; set; }
        IEnumerable<ICommandHelpInfo> CommandHelpInfos { get; }
        void AddSocials(IEnumerable<ISocial> socials);

        bool Execute(ICharacter argument, IExecutable<ICharacter> command, string playerInput, CharacterState state = CharacterState.Any,
            PermissionLevel permissionLevel = PermissionLevel.Any, IOutputHandler outputHandler = null);

        IEnumerable<string> ReportCommands(PermissionLevel authority, ICharacter reference);
        IEnumerable<IGrouping<string, string>> ReportCommandsInGroups(PermissionLevel authority, ICharacter reference);

        void Add(string entry, IEnumerable<string> entries, IExecutable<ICharacter>.CommandGenericMethodDelegate commandGenericMethod,
            CharacterState state = CharacterState.Any,
            PermissionLevel permissionRequired = PermissionLevel.Any,
            CommandDisplayOptions options = CommandDisplayOptions.None);

        void Clear();
        IEnumerable<string> ReportCommands(PermissionLevel minimumAuthority, PermissionLevel maximumAuthority, ICharacter reference);
    }
}