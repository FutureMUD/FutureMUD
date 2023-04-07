using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Celestial;
using MudSharp.Character;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.FutureProg;

namespace MudSharp.NPC.AI.Groups.GroupTypes;

public class TerritorialHerdGrazer : NeutralHerdGrazers
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
		var ss = new StringStack(builderArgs);
		if (ss.IsFinished)
		{
			return (null, "You must supply a dominant gender.");
		}

		if (!Utilities.TryParseEnum<Gender>(ss.PopSpeech(), out var gender))
		{
			return (null, $"The supplied value '{ss.Last}' is not a valid gender.");
		}

		if (ss.IsFinished || !ss.PopSpeech().TryParsePercentage(out var confidence))
		{
			return (null,
				"You must supply a percentage confidence level that determines how often they will posture versus flee from threats when not aggressive.");
		}

		if (ss.IsFinished || !ss.PopSpeech().TryParsePercentage(out var aggression))
		{
			return (null,
				"You must supply a percentage aggression level that determines how often they will posture versus attack threats.");
		}

		var (success, error, activeTimes) = ParseBuilderArgument(ss.PopSpeech().ToLowerInvariant());
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
	}

	protected TerritorialHerdGrazer(XElement root, IFuturemud gameworld) : base(root, gameworld)
	{
		Confidence = double.Parse(root.Element("Confidence").Value);
		Aggression = double.Parse(root.Element("Aggression").Value);
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
			new XElement("Gender", (short)DominantGender)
		);
	}
}