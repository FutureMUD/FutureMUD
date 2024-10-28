using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;
using MudSharp.Commands.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.CharacterCreation;
using MudSharp.FutureProg;
using MudSharp.Help;
using System.Diagnostics;
using System.Xml.Linq;
using System.Numerics;

namespace MudSharp.Commands;

public class CommandHelpInfo : ICommandHelpInfo
{
	/// <inheritdoc />
	public override string ToString()
	{
		return $"Help {HelpName}";
	}

	public CommandHelpInfo(string helpName, string defaultHelp, AutoHelp autoHelp, string adminHelp, bool adminOnly)
	{
		HelpName = helpName;
		DefaultHelp = defaultHelp;
		AutoHelpSetting = autoHelp;
		AdminHelp = adminHelp;
		AdminOnly = adminOnly;
	}

	public string HelpName { get; protected set; }
	public string DefaultHelp { get; protected set; }
	public string AdminHelp { get; protected set; }
	public AutoHelp AutoHelpSetting { get; protected set; }
	public bool AdminOnly { get; protected set; }



	//Try to find and send defaultHelp from the attribute, or as a backup look up a matching helpfile
	private bool ShowHelp(ICharacter argument, IOutputHandler outputHandler)
	{
		if (!string.IsNullOrEmpty(AdminHelp) && argument.IsAdministrator())
		{
			
			outputHandler.Send(
				$"{$"Help on {HelpName.TitleCase()}".GetLineWithTitleInner(argument, Telnet.Cyan, Telnet.BoldWhite)}\n\n{AdminHelp.SubstituteANSIColour()}");
			return true;
		}

		//Try to show the default help as backup
		if (DefaultHelp.Length > 0)
		{
			outputHandler.Send(
				$"{$"Help on {HelpName.TitleCase()}".GetLineWithTitleInner(argument, Telnet.Cyan, Telnet.BoldWhite)}\n\n{DefaultHelp.SubstituteANSIColour()}");
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

	string IHelpInformation.HelpName => HelpName;

	/// <inheritdoc />
	IEnumerable<string> IHelpInformation.Keywords => [HelpName];

	/// <inheritdoc />
	string IHelpInformation.Category => "Commands";

	/// <inheritdoc />
	string IHelpInformation.Subcategory => "Built-In";

	/// <inheritdoc />
	string IHelpInformation.TagLine => $"Automatically generated help for the {HelpName} command";

	/// <inheritdoc />
	string IHelpInformation.PublicText => DefaultHelp;

	/// <inheritdoc />
	string IHelpInformation.LastEditedBy => "System";

	/// <inheritdoc />
	DateTime IHelpInformation.LastEditedDate => DateTime.UtcNow;

	/// <inheritdoc />
	IFutureProg IHelpInformation.Rule => Futuremud.Games.First().AlwaysTrueProg;

	/// <inheritdoc />
	IEnumerable<Tuple<IFutureProg, string>> IHelpInformation.AdditionalTexts => Enumerable.Empty<Tuple<IFutureProg, string>>();

	/// <inheritdoc />
	bool IHelpInformation.CanView(ICharacter actor)
	{
		return true;
	}

	/// <inheritdoc />
	string IHelpInformation.DisplayHelpFile(ICharacter actor)
	{
		IHelpInformation ihi = this;
		var sb = new StringBuilder();
		sb.AppendLine($"Help on {HelpName.TitleCase()}".GetLineWithTitleInner(actor, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine($"Category: {ihi.Category.TitleCase().ColourValue()}");
		sb.AppendLine($"Subcategory: {ihi.Subcategory.TitleCase().ColourValue()}");
		sb.AppendLine($"Keywords: {ihi.Keywords.ListToColouredStringOr()}");
		sb.AppendLine($"Tagline: {ihi.TagLine.ProperSentences().ColourCommand()}");
		sb.AppendLine();
		if (actor.IsAdministrator() && !string.IsNullOrEmpty(AdminHelp))
		{
			sb.AppendLine(AdminHelp.SubstituteANSIColour().Wrap(actor.InnerLineFormatLength));
		}
		else
		{
			sb.AppendLine(ihi.PublicText.SubstituteANSIColour().Wrap(actor.InnerLineFormatLength));
		}
		

		foreach (var addition in ihi.AdditionalTexts.Where(x => x.Item1?.ExecuteBool(actor) == true))
		{
			sb.AppendLine();
			sb.AppendLine(addition.Item2.Wrap(actor.InnerLineFormatLength));
		}

		sb.AppendLine();
		sb.AppendLine(
			$"Lasted edited by {ihi.LastEditedBy.Proper().Colour(Telnet.Green)} on {ihi.LastEditedDate.GetLocalDateString(actor).Colour(Telnet.Green)}.");

		return sb.ToString();
	}

	/// <inheritdoc />
	string IHelpInformation.DisplayHelpFile(IChargen chargen)
	{
		IHelpInformation ihi = this;
		var sb = new StringBuilder();
		sb.AppendLine($"Help on {HelpName.TitleCase()}".GetLineWithTitleInner(chargen, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine($"Category: {ihi.Category.TitleCase().ColourValue()}");
		sb.AppendLine($"Subcategory: {ihi.Subcategory.TitleCase().ColourValue()}");
		sb.AppendLine($"Keywords: {ihi.Keywords.ListToColouredStringOr()}");
		sb.AppendLine($"Tagline: {ihi.TagLine.ProperSentences().ColourCommand()}");
		sb.AppendLine();
		sb.AppendLine(ihi.PublicText.SubstituteANSIColour().Wrap(chargen.Account.InnerLineFormatLength));
		sb.AppendLine();
		sb.AppendLine(
			$"Lasted edited by {ihi.LastEditedBy.Proper().Colour(Telnet.Green)} on {ihi.LastEditedDate.GetLocalDateString(chargen.Account).Colour(Telnet.Green)}.");

		return sb.ToString();
	}
}