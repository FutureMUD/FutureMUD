using MudSharp.CharacterCreation;
using MudSharp.Commands.Modules;
using MudSharp.Help;
using System.Diagnostics;
using System.Numerics;

namespace MudSharp.Commands;

public class CommandHelpInfo : ICommandHelpInfo
{
    private readonly List<(Func<ICharacter, bool> Predicate, string HelpText)> _conditionalHelpTexts = new();

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

    public CommandHelpInfo(string helpName, string defaultHelp, AutoHelp autoHelp, string adminHelp, bool adminOnly,
        Func<ICharacter, bool> conditionalHelpPredicate, string conditionalHelpText)
        : this(helpName, defaultHelp, autoHelp, adminHelp, adminOnly)
    {
        AddConditionalHelp(conditionalHelpPredicate, conditionalHelpText);
    }

    public string HelpName { get; protected set; }
    public string DefaultHelp { get; protected set; }
    public string AdminHelp { get; protected set; }
    public AutoHelp AutoHelpSetting { get; protected set; }
    public bool AdminOnly { get; protected set; }

    public CommandHelpInfo AddConditionalHelp(Func<ICharacter, bool> predicate, string helpText)
    {
        if (predicate == null)
        {
            throw new ArgumentNullException(nameof(predicate));
        }

        if (helpText == null)
        {
            throw new ArgumentNullException(nameof(helpText));
        }

        _conditionalHelpTexts.Add((predicate, helpText));
        return this;
    }

    public string HelpTextFor(ICharacter actor)
    {
        if (!string.IsNullOrEmpty(AdminHelp) && actor.IsAdministrator())
        {
            return AdminHelp;
        }

        foreach (var (predicate, helpText) in _conditionalHelpTexts)
        {
            if (predicate(actor))
            {
                return helpText;
            }
        }

        return DefaultHelp ?? string.Empty;
    }



    //Try to find and send defaultHelp from the attribute, or as a backup look up a matching helpfile
    private bool ShowHelp(ICharacter argument, IOutputHandler outputHandler)
    {
        var helpText = HelpTextFor(argument);
        if (!string.IsNullOrEmpty(helpText))
        {
            outputHandler.Send(
                $"{$"Help on {HelpName.TitleCase()}".GetLineWithTitleInner(argument, Telnet.Cyan, Telnet.BoldWhite)}\n\n{helpText.SubstituteANSIColour()}");
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

        StringStack ss = new(playerInput.RemoveFirstWord());

        if (ss.IsFinished && (AutoHelpSetting == AutoHelp.NoArg || AutoHelpSetting == AutoHelp.HelpArgOrNoArg))
        {
            return ShowHelp(argument, outputHandler);
        }

        string firstArg = ss.PopSpeech();
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
        StringBuilder sb = new();
        sb.AppendLine($"Help on {HelpName.TitleCase()}".GetLineWithTitleInner(actor, Telnet.Cyan, Telnet.BoldWhite));
        sb.AppendLine();
        sb.AppendLine($"Category: {ihi.Category.TitleCase().ColourValue()}");
        sb.AppendLine($"Subcategory: {ihi.Subcategory.TitleCase().ColourValue()}");
        sb.AppendLine($"Keywords: {ihi.Keywords.ListToColouredStringOr()}");
        sb.AppendLine($"Tagline: {ihi.TagLine.ProperSentences().ColourCommand()}");
        sb.AppendLine();
        sb.AppendLine(HelpTextFor(actor).SubstituteANSIColour().Wrap(actor.InnerLineFormatLength));


        foreach (Tuple<IFutureProg, string> addition in ihi.AdditionalTexts.Where(x => x.Item1?.ExecuteBool(actor) == true))
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
        StringBuilder sb = new();
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
