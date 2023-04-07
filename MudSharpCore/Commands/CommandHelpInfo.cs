using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;
using MudSharp.Commands.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Commands;

public class CommandHelpInfo : ICommandHelpInfo
{
	public CommandHelpInfo(string helpName, string defaultHelp, AutoHelp autoHelp, string adminHelp)
	{
		HelpName = helpName;
		DefaultHelp = defaultHelp;
		AutoHelpSetting = autoHelp;
		AdminHelp = adminHelp;
	}

	public string HelpName { get; protected set; }
	public string DefaultHelp { get; protected set; }
	public string AdminHelp { get; protected set; }
	public AutoHelp AutoHelpSetting { get; protected set; }

	//Try to find and send defaultHelp from the attribute, or as a backup look up a matching helpfile
	private bool ShowHelp(ICharacter argument, IOutputHandler outputHandler)
	{
		if (!string.IsNullOrEmpty(AdminHelp) && argument.IsAdministrator())
		{
			outputHandler.Send(
				$"{$"Command Help for the {HelpName} command:".Colour(Telnet.Cyan)}\n\n{AdminHelp.SubstituteANSIColour()}");
			return true;
		}

		//Try to show the default help as backup
		if (DefaultHelp.Length > 0)
		{
			outputHandler.Send(
				$"{$"Command Help for the {HelpName} command:".Colour(Telnet.Cyan)}\n\n{DefaultHelp.SubstituteANSIColour()}");
			return true;
		}

		if (HelpName.Length > 0)
			//Try to show a helpfile since we seem to have one designated
		{
			if (HelpModule.HelpExactMatch(argument, HelpName, false))
			{
				return true;
			}
		}

		return false;
	}

	//Check to see if we should display help
	//Return TRUE if help was displayed, FALSE if if no help was displayed
	public bool CheckHelp(ICharacter argument, string playerInput, IOutputHandler outputHandler)
	{
		if (outputHandler == null)
		{
			return false;
		}

		var ss = new StringStack(playerInput.RemoveFirstWord());

		if (ss.IsFinished && (AutoHelpSetting == AutoHelp.NoArg || AutoHelpSetting == AutoHelp.HelpArgOrNoArg))
		{
			return ShowHelp(argument, outputHandler);
		}

		var firstArg = ss.Pop();
		if ((AutoHelpSetting == AutoHelp.HelpArg || AutoHelpSetting == AutoHelp.HelpArgOrNoArg) &&
		    ss.IsFinished && (firstArg == "help" || firstArg == "?"))
		{
			return ShowHelp(argument, outputHandler);
		}

		return false;
	}
}