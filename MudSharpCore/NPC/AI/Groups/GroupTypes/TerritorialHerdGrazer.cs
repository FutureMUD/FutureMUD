using MudSharp.Celestial;

namespace MudSharp.NPC.AI.Groups.GroupTypes;

public class TerritorialHerdGrazer : NeutralHerdGrazers, IEditableGroupAIType
{
    public new static void RegisterGroupAIType()
    {
        GroupAITypeFactory.RegisterGroupAIType("territorialherdgrazer", DatabaseLoader, BuilderLoader);
    }

    private static IGroupAIType DatabaseLoader(XElement root, IFuturemud gameworld)
    {
        return new TerritorialHerdGrazer(root, gameworld);
    }

    private static (IGroupAIType Type, string Error) BuilderLoader(string builderArgs, IFuturemud gameworld)
    {
        StringStack ss = new(builderArgs);
        if (ss.IsFinished)
        {
            return (null, "You must supply a dominant gender.");
        }

        if (!Utilities.TryParseEnum<Gender>(ss.PopSpeech(), out Gender gender))
        {
            return (null, $"The supplied value '{ss.Last}' is not a valid gender.");
        }

        if (ss.IsFinished || !ss.PopSpeech().TryParsePercentage(out double confidence))
        {
            return (null,
                "You must supply a percentage confidence level that determines how often they will posture versus flee from threats when not aggressive.");
        }

        if (ss.IsFinished || !ss.PopSpeech().TryParsePercentage(out double aggression))
        {
            return (null,
                "You must supply a percentage aggression level that determines how often they will posture versus attack threats.");
        }

        (bool success, string error, IEnumerable<TimeOfDay> activeTimes) = ParseBuilderArgument(ss.PopSpeech().ToLowerInvariant());
        if (!success)
        {
            return (null, error);
        }

        return (new TerritorialHerdGrazer(gender, activeTimes, aggression, confidence, gameworld), string.Empty);
    }

    public double Aggression { get; protected set; }

    public IFutureProg IsAggressiveProg { get; protected set; }

    protected TerritorialHerdGrazer(Gender dominantGender, IEnumerable<TimeOfDay> activeTimesOfDay, double aggression,
        double confidence, IFuturemud gameworld) : base(dominantGender, activeTimesOfDay, confidence, gameworld)
    {
        Aggression = aggression;
		IsAggressiveProg = gameworld.AlwaysFalseProg;
    }

    protected TerritorialHerdGrazer(XElement root, IFuturemud gameworld) : base(root, gameworld)
    {
        Confidence = double.Parse(root.Element("Confidence").Value);
        Aggression = double.Parse(root.Element("Aggression").Value);
		IsAggressiveProg = gameworld.FutureProgs.Get(long.Parse(root.Element("IsAggressiveProg")?.Value ?? "0")) ??
			gameworld.AlwaysFalseProg;
    }

    public override string Name
    {
        get
        {
            if (DominantGender == Gender.Indeterminate)
            {
                return $"Egalitarian {GroupActivityTimeDescription} Territorial Grazers";
            }

            return $"{DominantGender.DescribeEnum()}-Dominant {GroupActivityTimeDescription} Territorial Grazers";
        }
    }

    public override XElement SaveToXml()
    {
        return new XElement("GroupType",
            new XAttribute("typename", "territorialherdgrazer"),
            new XElement("ActiveTimes",
                from time in ActiveTimesOfDay
                select new XElement("Time", (int)time)
            ),
            new XElement("Confidence", Confidence),
            new XElement("Aggression", Aggression),
            new XElement("IsAggressiveProg", IsAggressiveProg?.Id ?? 0),
            new XElement("Gender", (short)DominantGender)
        );
    }

	public override bool ConsidersThreat(ICharacter ch, IGroupAI group, GroupAlertness alertness)
	{
		return IsAggressiveProg.ExecuteBool(false, ch) || base.ConsidersThreat(ch, group, alertness);
	}

	protected override void EvaluateAlertLevel(IGroupAI group)
	{
		base.EvaluateAlertLevel(group);
		if (group.GroupMembers
			.SelectMany(x => x.Location.LayerCharacters(x.RoomLayer))
			.OfType<ICharacter>()
			.Any(x => !group.GroupMembers.ContainsPhysicalInstance(x) && IsAggressiveProg.ExecuteBool(false, x)))
		{
			group.Alertness = GroupAlertness.Aggressive;
			group.CurrentAction = GroupAction.AttackThreats;
		}
	}

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		if (!command.PopForSwitch().EqualToAny("aggressive", "aggression", "aggressiveprog"))
		{
			actor.OutputHandler.Send("You can set #3aggressive <prog>#0 for this territorial group type.".SubstituteANSIColour());
			return false;
		}

		IFutureProg prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			ProgVariableTypes.Boolean, new[] { ProgVariableTypes.Character }).LookupProg();
		if (prog is null)
		{
			return false;
		}

		IsAggressiveProg = prog;
		actor.OutputHandler.Send($"This territorial group will now use {prog.MXPClickableFunctionName()} for aggression.");
		return true;
	}

	public string Show(ICharacter actor)
	{
		return $"Territorial Aggression: {Aggression.ToString("P2", actor).ColourValue()}\nAggressive Prog: {IsAggressiveProg?.MXPClickableFunctionName() ?? "None".ColourError()}";
	}
}
