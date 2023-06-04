using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Castle.Components.DictionaryAdapter;
using MudSharp.Celestial;
using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.Construction;
using MudSharp.Effects.Concrete;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.FutureProg;

namespace MudSharp.NPC.AI.Groups.GroupTypes;

public class FamilyPredatorGroup : PredatorGroupBase
{
	public static void RegisterGroupAIType()
	{
		GroupAITypeFactory.RegisterGroupAIType("familypredator", DatabaseLoader, BuilderLoader);
	}

	private static IGroupAIType DatabaseLoader(XElement root, IFuturemud gameworld)
	{
		return new FamilyPredatorGroup(root, gameworld);
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

		var (success, error, activeTimes) = ParseBuilderArgument(ss.PopSpeech().ToLowerInvariant());
		if (!success)
		{
			return (null, error);
		}

		return (new FamilyPredatorGroup(gender, activeTimes, gameworld), string.Empty);
	}

	/// <inheritdoc />
	public FamilyPredatorGroup(Gender dominantGender, IEnumerable<TimeOfDay> activeTimes, IFuturemud gameworld) : base(
		dominantGender, activeTimes, gameworld)
	{
	}

	/// <inheritdoc />
	public FamilyPredatorGroup(XElement root, IFuturemud gameworld) : base(root, gameworld)
	{
	}

	#region Overrides of GroupAIType

	/// <inheritdoc />
	public override string Name
	{
		get
		{
			if (DominantGender == Gender.Indeterminate)
			{
				return $"Egalitarian {GroupActivityTimeDescription} Family Predators";
			}

			return $"{DominantGender.DescribeEnum()}-Dominant {GroupActivityTimeDescription} Family Predators";
		}
	}

	protected class FamilyPredatorData : PredatorGroupData
	{
		public List<IRace> UntrustedRaces { get; } = new();
		public ICell HomeLocation { get; set; }

		public FamilyPredatorData(IFuturemud gameworld) : base(gameworld)
		{
		}

		public FamilyPredatorData(XElement root, IFuturemud gameworld) : base(root, gameworld)
		{
			foreach (var item in root.Element("UntrustedRaces").Elements())
			{
				var race = gameworld.Races.Get(long.Parse(item.Value));
				if (race == null)
				{
					continue;
				}

				UntrustedRaces.Add(race);
			}
		}

		public override XElement SaveToXml()
		{
			var item = base.SaveToXml();
			item.Add(new XElement("UntrustedRaces", from race in UntrustedRaces select new XElement("Race", race.Id)));
			return item;
		}

		public override string ShowText(ICharacter voyeur)
		{
			var sb = new StringBuilder();
			sb.Append(base.ShowText(voyeur));
			sb.AppendLine(
				$"Untrusted Races: {UntrustedRaces.Select(x => x.Name.ColourValue()).ListToCommaSeparatedValues()}");
			sb.AppendLine(
				$"Home Location: {HomeLocation?.GetFriendlyReference(voyeur).ColourName() ?? "None".Colour(Telnet.Red)}");
			return sb.ToString();
		}

		#region Overrides of PredatorGroupData

		/// <inheritdoc />
		protected override string ShowTextBeforeTerritory(ICharacter voyeur)
		{
			return
				$@"Untrusted Races: {UntrustedRaces.Select(x => x.Name.ColourValue()).ListToCommaSeparatedValues()}
Home Location: {HomeLocation?.GetFriendlyReference(voyeur).ColourName() ?? "None".Colour(Telnet.Red)}";
		}

		#endregion
	}

	/// <inheritdoc />
	public override void HandleMinuteTick(IGroupAI herd)
	{
		throw new NotImplementedException();
	}

	/// <inheritdoc />
	public override IGroupTypeData LoadData(XElement root, IFuturemud gameworld)
	{
		throw new NotImplementedException();
	}

	/// <inheritdoc />
	public override IGroupTypeData GetInitialData(IFuturemud gameworld)
	{
		throw new NotImplementedException();
	}

	/// <inheritdoc />
	public override XElement SaveToXml()
	{
		throw new NotImplementedException();
	}

	public override bool ConsidersThreat(ICharacter ch, IGroupAI group, GroupAlertness alertness)
	{
		var data = (FamilyPredatorData)group.Data;
		if (alertness > GroupAlertness.NotAlert && data.UntrustedRaces.Any(x => x.SameRace(ch.Race)))
		{
			return true;
		}

		return false;
	}

	protected override void GatherStragglers(IGroupAI group, PredatorGroupData data)
	{
		if (!group.GroupMembers.Any())
		{
			return;
		}

		var (main, stragglers, _) = GetGroups(group, data);
		if (!stragglers.Any())
		{
			return;
		}

		var targetLocation = (main.First().Location, main.First().RoomLayer);
		foreach (var ch in stragglers)
		{
			if (!ch.CouldMove(false, null).Success)
			{
				continue;
			}

			if (ch.AffectedBy<FollowingPath>())
			{
				ch.CombinedEffectsOfType<FollowingPath>().First().FollowPathAction();
				continue;
			}

			if (ch.Location == targetLocation.Location)
			{
				PathIndividualToLayer(ch, targetLocation.RoomLayer);
				continue;
			}

			PathIndividualToLocation(ch, group, targetLocation.Location);
		}
	}

	protected override bool HandleTenSecondAction(IGroupAI group, PredatorGroupData data,
		GroupAction groupCurrentAction)
	{
		throw new NotImplementedException();
	}

	/// <inheritdoc />
	protected override void HandleFindFood(IGroupAI group, PredatorGroupData data)
	{
		throw new NotImplementedException();
	}
	

	#endregion
}