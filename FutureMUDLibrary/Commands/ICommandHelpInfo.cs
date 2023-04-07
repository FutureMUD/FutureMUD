using MudSharp.Character;
using MudSharp.PerceptionEngine;

namespace MudSharp.Commands
{
    
    public enum AutoHelp
    {
        Never,              // Never do autohelp, for now it means the attribute does nothing
        NoArg,              // If the user enters no arguments, show the help then exit
        HelpArg,            // If the user enters 'help' or '?' as the only argument, show the help then exit
        HelpArgOrNoArg,     // If the user enters 'help' or '?' or no argument, show the help then exit
    }

    public interface ICommandHelpInfo
    {
        string HelpName { get; }
        string DefaultHelp { get; }
        string AdminHelp { get; }
        AutoHelp AutoHelpSetting { get; }
        bool CheckHelp(ICharacter argument, string playerInput, IOutputHandler outputHandler);
    }
}