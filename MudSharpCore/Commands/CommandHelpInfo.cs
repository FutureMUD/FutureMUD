using MudSharp.CharacterCreation;
using MudSharp.Commands.Modules;
using MudSharp.Accounts;
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
        CommandName = helpName;
        Aliases = new[] { helpName };
        PermissionRequired = adminOnly ? PermissionLevel.JuniorAdmin : PermissionLevel.Any;
    }

    public CommandHelpInfo(string helpName, string defaultHelp, AutoHelp autoHelp, string adminHelp, bool adminOnly,
        Func<ICharacter, bool> conditionalHelpPredicate, string conditionalHelpText)
        : this(helpName, defaultHelp, autoHelp, adminHelp, adminOnly)
    {
        AddConditionalHelp(conditionalHelpPredicate, conditionalHelpText);
    }

    public IReadOnlyList<string> Examples { get; private set; } = Array.Empty<string>();
    public string ExamplePrerequisites { get; private set; } = string.Empty;

    public CommandHelpInfo WithExamples(IEnumerable<string> examples, string prerequisites)
    {
        Examples = examples.ToArray();
        ExamplePrerequisites = prerequisites;
        if (Examples.Count == 0) return this;
        var section = "\n\nExamples (" + prerequisites + "):\n" +
                      string.Join("\n", Examples.Select(x => x.ColourCommand()));
        DefaultHelp += section;
        AdminHelp += section;
        return this;
    }

    public string HelpName { get; protected set; }
    public string DefaultHelp { get; protected set; }
    public string AdminHelp { get; protected set; }
    public AutoHelp AutoHelpSetting { get; protected set; }
    public bool AdminOnly { get; protected set; }

	public string CommandName { get; private set; }
	public IReadOnlyList<string> Aliases { get; private set; }
	public string Subsystem { get; private set; } = "Built-In";
	public PermissionLevel PermissionRequired { get; private set; }
	public string Capability { get; private set; } = "Command";
	public string Summary { get; private set; } = string.Empty;

	public CommandHelpInfo WithMetadata(string commandName, IEnumerable<string> aliases, string subsystem,
		PermissionLevel permission, string capability = "Command", string summary = null)
	{
		CommandName = commandName;
		Aliases = aliases.Distinct(StringComparer.InvariantCultureIgnoreCase).ToArray();
		Subsystem = subsystem;
		PermissionRequired = permission;
		Capability = capability;
		Summary = summary ?? DefaultHelp?.SubstituteANSIColour().StripANSIColour()
			.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault()?.Trim() ?? string.Empty;
		return this;
	}

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
    IEnumerable<string> IHelpInformation.Keywords => Aliases.Concat(new[] { HelpName, CommandName, Subsystem, Capability })
        .Distinct(StringComparer.InvariantCultureIgnoreCase);

    /// <inheritdoc />
    string IHelpInformation.Category => "Commands";

    /// <inheritdoc />
    string IHelpInformation.Subcategory => Subsystem;

    /// <inheritdoc />
    string IHelpInformation.TagLine => string.IsNullOrEmpty(Summary) ? $"Help for the {HelpName} command" : Summary;

    /// <inheritdoc />
    string IHelpInformation.PublicText => DefaultHelp;

    /// <inheritdoc />
    string IHelpInformation.LastEditedBy => "System";

    /// <inheritdoc />
    DateTime IHelpInformation.LastEditedDate => DateTime.MinValue; // Source edit date is not recorded.

    /// <inheritdoc />
    IFutureProg IHelpInformation.Rule => Futuremud.Games.First().AlwaysTrueProg;

    /// <inheritdoc />
    IEnumerable<Tuple<IFutureProg, string>> IHelpInformation.AdditionalTexts => Enumerable.Empty<Tuple<IFutureProg, string>>();

    /// <inheritdoc />
    bool IHelpInformation.CanView(ICharacter actor)
    {
        return !AdminOnly || actor.IsAdministrator();
    }

    /// <inheritdoc />
    string IHelpInformation.DisplayHelpFile(ICharacter actor)
    {
        IHelpInformation ihi = this;
        StringBuilder sb = new();
        sb.AppendLine($"Help on {HelpName.TitleCase()}".GetLineWithTitleInner(actor, Telnet.Cyan, Telnet.BoldWhite));
        sb.AppendLine();
        sb.AppendLine($"Category: {ihi.Category.TitleCase().ColourName()}");
        sb.AppendLine($"Subcategory: {ihi.Subcategory.TitleCase().ColourName()}");
        sb.AppendLine($"Keywords: {ihi.Keywords.ListToColouredStringOr()}");
        sb.AppendLine($"Command: {CommandName.ColourCommand()} | Capability: {Capability.ColourName()} | Access: {PermissionRequired.DescribeEnum().ColourName()}".Wrap(actor.InnerLineFormatLength));
        sb.AppendLine($"Tagline: {ihi.TagLine.ProperSentences()}".Wrap(actor.InnerLineFormatLength));
        sb.AppendLine();
        sb.AppendLine(HelpTextFor(actor).SubstituteANSIColour().Wrap(actor.InnerLineFormatLength));


        foreach (Tuple<IFutureProg, string> addition in ihi.AdditionalTexts.Where(x => x.Item1?.ExecuteBool(actor) == true))
        {
            sb.AppendLine();
            sb.AppendLine(addition.Item2.Wrap(actor.InnerLineFormatLength));
        }

        sb.AppendLine();
        sb.AppendLine(
            "Source: built-in command help shipped with the engine. Source edit date is not recorded.");

        return sb.ToString();
    }

    /// <inheritdoc />
    string IHelpInformation.DisplayHelpFile(IChargen chargen)
    {
        IHelpInformation ihi = this;
        StringBuilder sb = new();
        sb.AppendLine($"Help on {HelpName.TitleCase()}".GetLineWithTitleInner(chargen, Telnet.Cyan, Telnet.BoldWhite));
        sb.AppendLine();
        sb.AppendLine($"Category: {ihi.Category.TitleCase().ColourName()}");
        sb.AppendLine($"Subcategory: {ihi.Subcategory.TitleCase().ColourName()}");
        sb.AppendLine($"Keywords: {ihi.Keywords.ListToColouredStringOr()}");
        sb.AppendLine($"Tagline: {ihi.TagLine.ProperSentences()}".Wrap(chargen.Account.InnerLineFormatLength));
        sb.AppendLine();
        sb.AppendLine(ihi.PublicText.SubstituteANSIColour().Wrap(chargen.Account.InnerLineFormatLength));
        sb.AppendLine();
        sb.AppendLine(
            "Source: built-in command help shipped with the engine. Source edit date is not recorded.");

        return sb.ToString();
    }
}
