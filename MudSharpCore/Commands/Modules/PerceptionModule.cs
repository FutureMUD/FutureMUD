using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Humanizer;
using MoreLinq;
using MudSharp.Accounts;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.Combat;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Concrete;
using MudSharp.Form.Material;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.Health;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;

namespace MudSharp.Commands.Modules;

internal class PerceptionModule : Module<ICharacter>
{
	private PerceptionModule()
		: base("Perception")
	{
		IsNecessary = true;
	}

	public static PerceptionModule Instance { get; } = new();

	[PlayerCommand("Exits", "exits")]
	[RequiredCharacterState(CharacterState.Conscious)]
	[HelpInfo("Exits", @"This command allows you to see what exits are present at the location you're in. The syntax is #3exits#0.", AutoHelp.HelpArg)]
	protected static void Exits(ICharacter actor, string input)
	{
		var exits = actor
		            .Location
		            .ExitsFor(actor)
		            .Where(x => actor.CanSee(actor.Location, x))
		            .ToList();
		var sb = new StringBuilder();
		sb.AppendLine($"Exits for {actor.Location.HowSeen(actor)}:");
		foreach (var exit in exits)
		{
			if (exit.Exit.Door?.IsOpen == false && !exit.Exit.Door.CanFireThrough && !actor.IsAdministrator())
			{
				sb.AppendLine($"\t{exit.DescribeFor(actor).ColourIfNotColoured(Telnet.Green)}");
				continue;
			}

			sb.AppendLine(
				$"\t{(actor.IsAdministrator() ? $"[#{exit.Exit.Id.ToString("N0", actor)}] " : "")}{exit.DescribeFor(actor).ColourIfNotColoured(Telnet.Green)} to {exit.Destination.HowSeen(actor)}{(actor.IsAdministrator() ? $" (room #{exit.Destination.Id.ToString("N0", actor)})" : "")}");
		}

		actor.OutputHandler.Send(sb.ToString());
	}

	[PlayerCommand("SurveyCover", "surveycover")]
	[RequiredCharacterState(CharacterState.Conscious)]
	[HelpInfo("SurveyCover", @"The surveycover command is used to determine what kinds of ranged cover are in the location so that you can take cover from ranged attacks. 

Cover can be classified as either hard or soft. Soft cover makes you harder to hit but ultimately doesn't provide any protection if beaten, whereas hardcover may be struck instead of you. Some kinds of cover can also allow you to move around within the room and stay in cover. Additionally, cover has a maximum position that you can be in when you use it. For example, you may need to be prone behind cover to benefit from it.

The syntax is simply #3surveycover#0.

See the #6cover#0 command for how to take cover. Your combat settings may also automatically move you into cover in a fight.", AutoHelp.HelpArg)]

	protected static void SurveyCover(ICharacter actor, string input)
	{
		InternalSurveyCover(actor, new StringStack(input.RemoveFirstWord()));
	}

	protected static void InternalSurveyCover(ICharacter actor, StringStack input)
	{
		var covers = actor.Location.GetCoverFor(actor);
		var coverItems =
			actor.Location.LayerGameItems(actor.RoomLayer).SelectNotNull(x => x.GetItemType<IProvideCover>())
			     .Where(x => actor.CanSee(x.Parent) && x.Cover != null)
			     .ToList();

		if (!covers.Any() && !coverItems.Any())
		{
			actor.Send("There is no cover available here. You'd better hope you don't get caught in a firefight!");
			return;
		}

		var sb = new StringBuilder();
		sb.AppendLine("There is the following cover available in this location:");
		sb.AppendLine();
		sb.AppendLine("From Terrain".ColourName());
		sb.AppendLine(StringUtilities.GetTextTable(
			from cover in covers select new List<string>
			{
				cover.Name,
				cover.CoverExtent.Describe(),
				cover.CoverType.Describe(),
				cover.HighestPositionState.Name,
				cover.CoverStaysWhileMoving.ToColouredString()
			},
			new List<string>
			{
				"Name",
				"Extent",
				"Type",
				"Highest Position",
				"Allow Move?"
			},
			actor
		));
		sb.AppendLine();
		sb.AppendLine("From Items".ColourName());
		sb.AppendLine(StringUtilities.GetTextTable(
			from ci in coverItems 
			let cover = ci.Cover
			select new List<string>
			{
				ci.Parent.HowSeen(actor),
				cover.Name,
				cover.CoverExtent.Describe(),
				cover.CoverType.Describe(),
				cover.HighestPositionState.Name,
				cover.CoverStaysWhileMoving.ToColouredString()
			},
			new List<string>
			{
				"Item",
				"Name",
				"Extent",
				"Type",
				"Highest Position",
				"Allow Move?"
			},
			actor
		));

		actor.Send(sb.ToString());
	}

	[PlayerCommand("Survey", "survey")]
	[RequiredCharacterState(CharacterState.Conscious)]
	[HelpInfo("Survey", @"The survey command is used to get detailed information about your surroundings, for example, what the terrain type is or how noisy it is.

The syntax is simply #3survey#0.", AutoHelp.HelpArg)]
	protected static void Survey(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		if (ss.Peek().Equals("cover", StringComparison.InvariantCultureIgnoreCase))
		{
			InternalSurveyCover(actor, ss);
			return;
		}

		var sb = new StringBuilder();
		sb.Append("You can tell the following things about ");
		sb.Append(actor.Location.HowSeen(actor));
		sb.AppendLine(":");
		sb.AppendLine();
		sb.AppendLine(actor.Location.SafeQuit
			? "It is permitted to quit in this location.".Colour(Telnet.Green)
			: "It is not permitted to quit in this location.".Colour(Telnet.Red));
		var terrain = actor.Location.Terrain(actor);
		sb.AppendLine($"The terrain here is {terrain.Name.TitleCase().ColourForegroundCustom(terrain.TerrainANSIColour)}.");
		if (actor.IsAdministrator())
		{
			sb.AppendLineFormat(actor, "This room is in zone {0} (#{1:N0})",
				actor.Location.Room.Zone.Name.Colour(Telnet.Green),
				actor.Location.Room.Zone.Id);
			sb.AppendLine($"Latitude: {actor.Location.Zone.Geography.Latitude.RadiansToDegrees().ToString("N6", actor).ColourValue()}");
			sb.AppendLine($"Longitude: {actor.Location.Zone.Geography.Longitude.RadiansToDegrees().ToString("N6", actor).ColourValue()}");
			sb.AppendLine($"Elevation: {actor.Gameworld.UnitManager.DescribeMostSignificant(actor.Location.Zone.Geography.Elevation / actor.Gameworld.UnitManager.BaseHeightToMetres, Framework.Units.UnitType.Length, actor).ColourValue()}");
		}

		sb.AppendLine($"This location type is {actor.Location.OutdoorsType(actor).Describe().Colour(Telnet.Green)}.");

		var legals = actor.Gameworld.LegalAuthorities
		                  .Where(x => x.PlayersKnowTheirCrimes && x.EnforcementZones.Contains(actor.Location.Zone))
		                  .ToList();
		if (legals.Any())
		{
			sb.AppendLine(
				$"This is a lawful zone in the {legals.Select(x => x.Name.ColourName()).Humanize()} enforcement {(legals.Count == 1 ? "regime" : "regimes")}");
		}

		var property = actor.Gameworld.Properties.FirstOrDefault(x => x.PropertyLocations.Contains(actor.Location));
		if (property != null)
		{
			sb.AppendLine($"This location is part of the property called {property.Name.ColourName()}.");
		}

		var bank = actor.Gameworld.Banks.FirstOrDefault(x => x.BranchLocations.Contains(actor.Location));
		if (bank != null)
		{
			sb.AppendLine($"This location is a branch of {bank.Name.ColourName()}.");
		}

		var auctionHouse = actor.Gameworld.AuctionHouses.FirstOrDefault(x => x.AuctionHouseCell == actor.Location);
		if (auctionHouse != null)
		{
			sb.AppendLine($"This location is an auction house called {auctionHouse.Name.ColourName()}.");
		}

		var ez = actor.Gameworld.EconomicZones.FirstOrDefault(x => x.ConveyancingCells.Contains(actor.Location));
		if (ez != null)
		{
			sb.AppendLine($"This location can be used to manage property in the {ez.Name.ColourName()} economic zone.");
		}
		var illumination = actor.Location.CurrentIllumination(actor);
		sb.AppendLine($"Light levels are {actor.Gameworld.LightModel.GetIlluminationDescription(illumination).Colour(Telnet.Green)}{(actor.IsAdministrator() ? $"{illumination.ToString("N3", actor)} lux".ColourValue().ParenthesesSpacePrior() : "")}.");
		sb.AppendLine($"The base difficulty is {terrain.HideDifficulty.DescribeColoured()} to hide here.");
		var spotDifficulty = terrain.SpotDifficulty.Lowest(actor.Gameworld.LightModel.GetSightDifficulty(illumination * actor.Race.IlluminationPerceptionMultiplier));
		sb.AppendLine($"The base difficulty is {spotDifficulty.DescribeColoured()} to spot hidden things here.");
		sb.AppendLine(actor.Location.HearingProfile(actor) != null
			? actor.Location.HearingProfile(actor).SurveyDescription
			: "There is nothing remarkable about the noise levels here.".Colour(Telnet.Yellow));
		if (actor.Location.OutdoorsType(actor) == CellOutdoorsType.Outdoors)
		{
			sb.AppendLine();
			if (actor.Location.CurrentWeather(actor)?.ObscuresViewOfSky == true)
			{
				sb.AppendLine("You can't see any details about the sky because of the bad weather.");
			}
			else
			{
				sb.AppendLine(actor.Location.Room.Zone.DescribeSky);
			}
		}

		if (actor.IsAdministrator())
		{
			if (actor.Location.ForagableProfile == null)
			{
				sb.AppendLine("There is no foragable profile here.");
			}
			else
			{
				sb.AppendLineFormat(actor, "The foragable profile here is {0} (#{1:N0})",
					actor.Location.ForagableProfile.Name, actor.Location.ForagableProfile.Id);
				foreach (var type in actor.Location.ForagableTypes)
				{
					var yield = actor.Location.GetForagableYield(type);
					if (yield > 0.0)
					{
						sb.AppendLineFormat(actor, "\tYield [{0}] = {1:N2}", type.Colour(Telnet.Green), yield);
					}
				}
				sb.AppendLine();
			}

			sb.AppendLine(actor.Location.Atmosphere != null
				? $"The atmosphere here consists of {actor.Location.Atmosphere.Name.Colour(actor.Location.Atmosphere.DisplayColour)}."
				: $"There is no atmosphere here!");

			sb.AppendLine(
				$"Infections caught here will be of type {terrain.PrimaryInfection.Describe().ColourValue()}, difficulty {terrain.InfectionVirulence.DescribeColoured()} with virulence {terrain.InfectionMultiplier.ToString("P3", actor).ColourValue()}.");
			sb.AppendLine();
			var territoryOwners = actor.Gameworld.NPCs.Where(x =>
				x.CombinedEffectsOfType<Territory>().Any(y => y.Cells.Contains(actor.Location))).ToList();
			sb.AppendLine("Territory Claimed By:");
			if (!territoryOwners.Any())
			{
				sb.AppendLine("\tNone");
			}
			else
			{
				foreach (var owner in territoryOwners)
				{
					sb.AppendLine(
						$"\t{owner.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreCanSee | PerceiveIgnoreFlags.IgnoreSelf)} (#{owner.Id.ToString("N0", actor)})");
				}
			}
		}
		else
		{
			foreach (var yield in actor.Race.EdibleForagableYields)
			{
				var amount = actor.Location.GetForagableYield(yield.YieldType);
				var bites = (int)(amount / yield.YieldPerBite);
				if (bites > 0.0)
				{
					sb.AppendLine(
						$"There is approximately {bites.ToString("N0", actor).Colour(Telnet.Green)} bite{(bites == 1 ? "" : "s")} worth of {yield.YieldType.Colour(Telnet.Cyan)} yield here.");
				}
			}
		}

		actor.OutputHandler.Send(sb.ToString());
	}

	[PlayerCommand("Vicinity", "vicinity")]
	[RequiredCharacterState(CharacterState.Conscious)]
	[HelpInfo("Vicinity", @"The vicinity command is used to get an idea of which items and characters are in your immediate vicinity or the immediate vicinity of a target. This is helpful because it might affect certain outcomes like who can hear you when you whisper, or who is in close proximity when area damage is taken.

The syntax is either:

	#3vicinity#0 - shows all of the things in your own vicinity
	#3vicinity <target>#0 - shows the vicinity information for the target", AutoHelp.HelpArg)]
	protected static void Vicinity(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		IPerceiver target = actor;
		if (!ss.IsFinished)
		{
			target = actor.TargetLocal(ss.SafeRemainingArgument) as IPerceiver;
			if (target is null)
			{
				actor.OutputHandler.Send("You don't see anything like that here.");
				return;
			}
		}
		var sb = new StringBuilder();
		sb.AppendLine($"The following things are in {(target == actor ? "your" : target.HowSeen(actor, type: DescriptionType.Possessive))} vicinity:");
		sb.AppendLine();
		foreach (var thing in target.Location.LayerCharacters(target.RoomLayer).Where(x => x.InVicinity(target) && actor.CanSee(x)))
		{
			sb.AppendLine(thing.HowSeen(actor));
		}

		foreach (var thing in target.Location.LayerGameItems(target.RoomLayer).Where(x => x.InVicinity(target) && actor.CanSee(x)))
		{
			sb.AppendLine(thing.HowSeen(actor));
		}

		foreach (
			var thing in
			actor.Location.LayerGameItems(actor.RoomLayer).SelectNotNull(x => x.GetItemType<ITable>())
			     .SelectMany(x => x.Chairs)
			     .Select(x => x.Parent)
			     .Where(x => x.InVicinity(actor) && actor.CanSee(x)))
		{
			sb.AppendLine(thing.HowSeen(actor));
		}

		actor.OutputHandler.Send(sb.ToString());
	}

	[PlayerCommand("Look", "look", "l", "lo", "loo")]
	[RequiredCharacterState(CharacterState.Conscious)]
	[HelpInfo("look",
		@"The look command is used to show you specific information about your character's surrounds, and has a few simple variants:

To view your general surroundings: #3look#0
To look at something in particular: #3look <thing>#0
To look inside something: #3look in <thing>#0
To look at something someone else has: #3look <person> <thing>#0
To look at a door installed in an exit: #3look <direction> <doorkeywords>#0
To look at a person's tattoos: #3look <person> tattoos [<bodypart>]#0
To look at a person's scars: #3look <person> scars [<bodypart>]
To look at graffiti in a location: #3look graffiti [<which>]#0
To look at graffiti on an object: #3look <item> graffiti [<which>]#0

The use of the look command is affected by various factors such as the ambient light level, the relative skill and attribute levels of you and the things you could potentially see, magical effects, and damage to your eyes.

See also: HELP EVALUATE, HELP SEARCH, HELP SCAN",
		AutoHelp.HelpArg)]
	protected static void Look(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		var arg = ss.PopSafe();
		if (arg.Length == 0)
		{
			actor.Body.Look();
			return;
		}

		var lookin = false;

		if (arg.EqualTo("in"))
		{
			arg = ss.PopSafe();
			if (arg.Length == 0)
			{
				actor.OutputHandler.Send("Look in what?");
				return;
			}

			lookin = true;
		}

		if (arg.EqualTo("graffiti"))
		{
			actor.Body.LookGraffiti(ss.SafeRemainingArgument);
			return;
		}

		var target = actor.Target(arg);

		if (target == null)
		{
			var localItems = actor.Location.LayerGameItems(actor.RoomLayer).Where(x => actor.CanSee(x)).ToList();
			var groups = actor.Location.LayerGameItems(actor.RoomLayer).Where(x => actor.CanSee(x))
			                  .SelectNotNull(x => x.ItemGroup)
			                  .Distinct()
			                  .ConcatIfNotNull(localItems.Count > 25 ? GameItemProto.TooManyItemsGroup : null)
			                  .ToList();
			var puddles = localItems.SelectNotNull(x => x.GetItemType<PuddleGameItemComponent>()).ToList();
			var bloodPuddles = puddles.Where(x => x.LiquidMixture.Instances.All(y => y is BloodLiquidInstance)).ToList();
			var commodityTag = actor.Gameworld.Tags.Get(actor.Gameworld.GetStaticLong("PuddleResidueTagId"));
			var residues = (commodityTag is not null ? localItems
			                                           .SelectNotNull(x => x.GetItemType<ICommodity>())
			                                           .Where(x => x.Tag == commodityTag) : Enumerable.Empty<ICommodity>()).ToList();

			if (puddles.Any())
			{
				groups.Add(PuddleGameItemComponentProto.PuddleGroup);
			}

			if (bloodPuddles.Any())
			{
				groups.Add(PuddleGameItemComponentProto.BloodGroup);
			}

			if (residues.Any())
			{
				groups.Add(PuddleGameItemComponentProto.ResidueGroup);
			}

			var targetGroup = groups.GetFromItemListByKeyword(arg, actor);
			if (targetGroup == null)
			{
				actor.OutputHandler.Send("You do not see that to look at.");
				return;
			}

			actor.OutputHandler.Send(targetGroup.LookDescription(actor,
				targetGroup switch
				{
					not null when GameItemProto.TooManyItemsGroup == targetGroup => localItems,
					not null when PuddleGameItemComponentProto.PuddleGroup == targetGroup => puddles.Select(x => x.Parent),
					not null when PuddleGameItemComponentProto.BloodGroup == targetGroup => bloodPuddles.Select(x => x.Parent),
					not null when PuddleGameItemComponentProto.ResidueGroup == targetGroup => residues.Select(x => x.Parent),
					_ => actor.Location.LayerGameItems(actor.RoomLayer).Where(x => x.ItemGroup == targetGroup)
				},
				actor.Location));
			return;
		}

		if (!ss.IsFinished)
		{
			var text = ss.PopSafe();
			// look <item> <lock>
			if (!lookin && ((target as IGameItem)?.IsItemType<ILockable>() ?? false))
			{
				var targetLockable = (target as IGameItem).GetItemType<ILockable>();
				var targetlock = targetLockable.Locks.Select(x => x.Parent).GetFromItemListByKeyword(text, actor);
				if (targetlock != null)
				{
					actor.Body.Look(targetlock);
					return;
				}
			}

			if (target is IMortalPerceiver mp && text.EqualTo("wounds"))
			{
				actor.Body.LookWounds(mp);
				return;
			}

			// look <character> <item>|tattoos|scars
			if (target is ICharacter targetActor)
			{
				if (text.EqualTo("tattoos"))
				{
					if (!ss.IsFinished)
					{
						var targetPart = targetActor.Body.GetTargetPart(ss.PopSpeech());
						if (targetPart == null || targetPart is IOrganProto || targetPart is IBone)
						{
							actor.OutputHandler.Send(
								$"{targetActor.HowSeen(actor, true)} does not have any such bodypart.");
							return;
						}

						actor.Body.LookTattoos(targetActor, targetPart);
						return;
					}

					actor.Body.LookTattoos(targetActor);
					return;
				}

				if (text.EqualTo("scars"))
				{
					if (!ss.IsFinished)
					{
						var targetPart = targetActor.Body.GetTargetPart(ss.PopSpeech());
						if (targetPart == null || targetPart is IOrganProto || targetPart is IBone)
						{
							actor.OutputHandler.Send(
								$"{targetActor.HowSeen(actor, true)} does not have any such bodypart.");
							return;
						}

						actor.Body.LookScars(targetActor, targetPart);
						return;
					}

					actor.Body.LookScars(targetActor);
					return;
				}

				if (lookin && !targetActor.WillingToPermitInventoryManipulation(actor))
				{
					actor.OutputHandler.Send(
						$"{targetActor.HowSeen(actor, true)} is not willing to let you interact with {targetActor.ApparentGender(actor).Possessive()} inventory.");
					return;
				}

				target = targetActor.Body.ExternalItemsForOtherActors.Where(x => actor.CanSee(x))
				                    .GetFromItemListByKeyword(text, actor);
				if (target == null)
				{
					actor.Send(
						$"{targetActor.HowSeen(actor, true)} {(targetActor == actor ? "do" : "does")} not have anything like that to look at.");
					return;
				}
			}
			else if (target is IGameItem item)
			{
				if (text.EqualTo("graffiti"))
				{
					actor.Body.LookGraffitiThing(item, ss.SafeRemainingArgument);
					return;
				}
			}
		}

		if (lookin)
		{
			actor.Body.LookIn(target);
		}
		else
		{
			actor.Body.Look(target);
		}
	}

	private static void DisplayForScan(ICharacter actor, IEnumerable<ICharacter> characters, StringBuilder sb,
		bool useLayerTags)
	{
		foreach (var move in characters.SelectNotNull(x => x.Movement).Distinct().Where(x => x.SeenBy(actor)))
		{
			sb.Append(move.Describe(actor).Colour(Telnet.Yellow));
			if (useLayerTags)
			{
				sb.Append(" ");
				sb.Append(move.CharacterMovers.First().RoomLayer.ColouredTag());
			}

			sb.AppendLine();
			foreach (var ch in move.CharacterMovers.Where(x => actor.CanSee(x)))
			{
				actor.SeeTarget(ch);
			}
		}

		foreach (var ch in characters.Where(x => x.Movement == null))
		{
			sb.Append(ch.HowSeen(actor, true, DescriptionType.Long));
			if (useLayerTags)
			{
				sb.Append(" ");
				sb.Append(ch.RoomLayer.ColouredTag());
			}

			sb.AppendLine();
			actor.SeeTarget(ch);
		}
	}

	private static void DisplayForScan(ICharacter actor, IEnumerable<IGameItem> items, StringBuilder sb,
		bool useLayerTags)
	{
		var layerItems = items.GroupBy(x => x.RoomLayer);
		foreach (var layerItem in layerItems)
		{
			if (layerItem.GroupBy(x => x.ItemGroup).Sum(x => x.Key != null ? 1 : x.Count()) > 25 &&
			    GameItemProto.TooManyItemsGroup != null)
			{
				sb.Append(GameItemProto.TooManyItemsGroup.Describe(actor, layerItem, layerItem.First().Location)
				                       .Fullstop().Colour(Telnet.Cyan));
				if (useLayerTags)
				{
					sb.Append(" ");
					sb.Append(layerItem.Key.ColouredTag());
				}

				sb.AppendLine();
			}
			else
			{
				foreach (var group in layerItem.GroupBy(x => x.ItemGroup).OrderBy(x => x.Key == null))
				{
					if (group.Key != null)
					{
						sb.Append(group.Key.Describe(actor, group.AsEnumerable(), group.First().Location).Fullstop()
						               .Colour(Telnet.Cyan));
						if (useLayerTags)
						{
							sb.Append(" ");
							sb.Append(group.First().RoomLayer.ColouredTag());
						}

						sb.AppendLine();
					}
					else
					{
						foreach (var item in group)
						{
							sb.Append(item.HowSeen(actor, true, DescriptionType.Long));
							if (useLayerTags)
							{
								sb.Append(" ");
								sb.Append(item.RoomLayer.ColouredTag());
							}

							sb.AppendLine();
						}
					}
				}
			}
		}

		foreach (var item in items)
		{
			actor.SeeTarget(item);
		}
	}

	[PlayerCommand("QuickScan", "quickscan", "qscan", "qs")]
	[RequiredCharacterState(CharacterState.Able)]
	[HelpInfo("quickscan", "Quickly scan adjacent rooms. Syntax: qs [<minsize>] [<specific exit>]", AutoHelp.HelpArg)]
	protected static void QuickScan(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		List<ICellExit> exits = null;
		var userSetSize = SizeCategory.Nanoscopic;
		var layers = actor.Location.Terrain(actor).TerrainLayers.ToList();
		if (ss.IsFinished)
		{
			exits = actor.Location.ExitsFor(actor).ToList();
		}
		else
		{
			var sizeArg = ss.PopSpeech();
			var exitArg = ss.IsFinished ? "" : ss.SafeRemainingArgument;
			var sizes = Enum.GetValues(typeof(SizeCategory)).OfType<SizeCategory>();
			var setSize = sizes.FirstOrDefault(x =>
				x.Describe().Equals(sizeArg, StringComparison.InvariantCultureIgnoreCase));
			if (setSize != SizeCategory.Nanoscopic)
			{
				userSetSize = setSize;
			}
			else
			{
				exitArg = sizeArg;
			}

			if (!exitArg.Equals(string.Empty))
			{
				exits = new[] { actor.Location.GetExitKeyword(exitArg, actor) }.ToList();
			}
			else
			{
				exits = actor.Location.ExitsFor(actor).ToList();
			}
		}

		if ((!exits.Any() && layers.Count <= 1) || exits.Any(x => x == null))
		{
			actor.Send(string.IsNullOrEmpty(ss.Last)
				? "There are no visible exits or other layers for you to scan."
				: "There is no such exit for you to scan.");
			return;
		}

		var check = actor.Gameworld.GetCheck(CheckType.QuickscanPerceptionCheck);
		var allOutcomes = check.CheckAgainstAllDifficulties(actor, Difficulty.Normal, null);
		var difficulty = actor.Location.SpotDifficulty(actor).StageDown(1);
		var outcome = allOutcomes[difficulty];
		var minSize = actor.CurrentContextualSize(SizeContext.None);
		switch (outcome.Outcome)
		{
			case Outcome.MinorPass:
				minSize = minSize.ChangeSize(-1);
				break;
			case Outcome.Pass:
				minSize = minSize.ChangeSize(-2);
				break;
			case Outcome.MajorPass:
				minSize = minSize.ChangeSize(-3);
				break;
		}

		if (minSize < userSetSize)
		{
			minSize = userSetSize;
		}

		var sb = new StringBuilder();
		var sameCellItems = actor.Location.GameItems.Where(x =>
			x.RoomLayer != actor.RoomLayer &&
			x.Size >= minSize &&
			actor.CanSee(x, PerceiveIgnoreFlags.IgnoreObscured)
		).ToList();
		var sameCellCharacters = actor.Location.Characters.Where(x =>
			x.RoomLayer != actor.RoomLayer &&
			x.CurrentContextualSize(SizeContext.Scan) >= minSize &&
			actor.CanSee(x, PerceiveIgnoreFlags.IgnoreObscured)
		).ToList();

		if (sameCellCharacters.Any() || sameCellItems.Any())
		{
			sb.AppendLine($"In the same location as you, you see:");
			DisplayForScan(actor, sameCellCharacters, sb, true);
			DisplayForScan(actor, sameCellItems, sb, true);
		}

		foreach (var exit in exits)
		{
			if (sb.Length > 0)
			{
				sb.AppendLine();
			}

			if (exit.Exit.Door?.IsOpen == false)
			{
				if (!exit.Exit.Door.CanSeeThrough(actor.Body))
				{
					sb.AppendLine(
						$"To {exit.OutboundDirectionDescription}, you can see nothing because of {exit.Exit.Door.Parent.HowSeen(actor)}.");
					continue;
				}

				if (!actor.Body.CanSee(exit.Destination))
				{
					sb.AppendLine($"To {exit.OutboundDirectionDescription}, you can see nothing.");
					continue;
				}

				sb.AppendLine(
					$"To {exit.OutboundDirectionDescription} in {exit.Destination.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreLayers)}, you can see the following through {exit.Exit.Door.Parent.HowSeen(actor)}:");
			}
			else if (!actor.Body.CanSee(exit.Destination))
			{
				sb.AppendLine($"To {exit.OutboundDirectionDescription}, you can see nothing.");
				continue;
			}
			else
			{
				sb.AppendLine(
					$"To {exit.OutboundDirectionDescription} in {exit.Destination.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreLayers)}, you can see the following:");
			}


			difficulty = exit.Destination.SpotDifficulty(actor);
			outcome = allOutcomes[difficulty];
			minSize = actor.CurrentContextualSize(SizeContext.None);
			switch (outcome.Outcome)
			{
				case Outcome.MinorPass:
					minSize = minSize.ChangeSize(-1);
					break;
				case Outcome.Pass:
					minSize = minSize.ChangeSize(-2);
					break;
				case Outcome.MajorPass:
					minSize = minSize.ChangeSize(-3);
					break;
			}

			if (minSize < userSetSize)
			{
				minSize = userSetSize;
			}

			var location = exit.Destination;
			var characters = location.Characters.Where(x =>
				x.RoomLayer.CanBeSeenFromLayer(actor.RoomLayer) &&
				x.CurrentContextualSize(SizeContext.Scan) >= minSize &&
				actor.CanSee(x, PerceiveIgnoreFlags.IgnoreObscured)).ToList();
			var items = location.GameItems.Where(x =>
				x.RoomLayer.CanBeSeenFromLayer(actor.RoomLayer) && x.Size > minSize &&
				actor.CanSee(x, PerceiveIgnoreFlags.IgnoreObscured)).ToList();
			if (!items.Any() && !characters.Any())
			{
				sb.AppendLine("You see nothing.");
				continue;
			}

			DisplayForScan(actor, characters, sb, true);
			DisplayForScan(actor, items, sb, true);
		}

		actor.OutputHandler.Send(sb.ToString(), false, true);
	}

	[PlayerCommand("Scan", "scan", "sca")]
	[RequiredCharacterState(CharacterState.Able)]
	[HelpInfo("scan", "Quickly scan a range of rooms. Syntax: scan [<minsize>] [<specific exit>]", AutoHelp.HelpArg)]
	[DelayBlock("general", "You must first stop {0} before you can do that.")]
	[NoMeleeCombatCommand]
	protected static void Scan(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		List<ICellExit> exits = null;
		var userSetSize = SizeCategory.Nanoscopic;
		if (ss.IsFinished)
		{
			exits = actor.Location.ExitsFor(actor).ToList();
		}
		else
		{
			var sizeArg = ss.PopSpeech();
			var exitArg = ss.IsFinished ? "" : ss.SafeRemainingArgument;
			var sizes = Enum.GetValues(typeof(SizeCategory)).OfType<SizeCategory>();
			var setSize = sizes.FirstOrDefault(x =>
				x.Describe().Equals(sizeArg, StringComparison.InvariantCultureIgnoreCase));
			if (setSize != SizeCategory.Nanoscopic)
			{
				userSetSize = setSize;
			}
			else
			{
				exitArg = sizeArg;
			}

			if (!exitArg.Equals(string.Empty))
			{
				exits = new[] { actor.Location.GetExitKeyword(exitArg, actor) }.ToList();
			}
			else
			{
				exits = actor.Location.ExitsFor(actor).ToList();
			}
		}

		if (exits.Any(x => x == null))
		{
			actor.OutputHandler.Send("There is no such exit for you to scan.");
			return;
		}

		if (!exits.Any() && actor.Location.Terrain(actor).TerrainLayers.Except(actor.RoomLayer)
		                         .All(x => !x.CanBeSeenFromLayer(actor.RoomLayer)))
		{
			actor.OutputHandler.Send("There are no visible exits or visible layers for you to scan.");
			return;
		}


		actor.OutputHandler.Handle(new EmoteOutput(new Emote("@ begin|begins to scan the horizon.", actor),
			flags: OutputFlags.Insigificant));

		var check = actor.Gameworld.GetCheck(CheckType.ScanPerceptionCheck);
		var allOutcomes = check.CheckAgainstAllDifficulties(actor, Difficulty.Normal, null);
		var difficulty = actor.Location.SpotDifficulty(actor);
		var outcome = allOutcomes[difficulty];
		var minSize = actor.CurrentContextualSize(SizeContext.None).ChangeSize(-2);
		switch (outcome.Outcome)
		{
			case Outcome.MinorPass:
				minSize = minSize.ChangeSize(-1);
				break;
			case Outcome.Pass:
				minSize = minSize.ChangeSize(-2);
				break;
			case Outcome.MajorPass:
				minSize = minSize.ChangeSize(-3);
				break;
		}

		if (minSize < userSetSize)
		{
			minSize = userSetSize;
		}

		var sameCellItems = actor.Location.GameItems.Where(x =>
			x.RoomLayer != actor.RoomLayer &&
			x.RoomLayer.CanBeSeenFromLayer(actor.RoomLayer) &&
			x.Size >= minSize &&
			actor.CanSee(x, PerceiveIgnoreFlags.IgnoreObscured)
		).ToList();
		var sameCellCharacters = actor.Location.Characters.Where(x =>
			x.RoomLayer != actor.RoomLayer &&
			x.RoomLayer.CanBeSeenFromLayer(actor.RoomLayer) &&
			x.CurrentContextualSize(SizeContext.Scan) >= minSize &&
			actor.CanSee(x, PerceiveIgnoreFlags.IgnoreObscured)
		).ToList();

		if (sameCellCharacters.Any() || sameCellItems.Any())
		{
			var isb = new StringBuilder();
			isb.AppendLine($"In the same location as you, you see:");
			DisplayForScan(actor, sameCellCharacters, isb, true);
			DisplayForScan(actor, sameCellItems, isb, true);
			actor.OutputHandler.Send(isb.ToString());
		}

		if (!exits.Any())
		{
			return;
		}

		actor.AddEffect(new Scanning(actor, from exit in exits
		                                    select new Action<IPerceivable>(perc =>
		                                    {
			                                    var cexit = exit;
			                                    if (cexit.Exit.Door?.IsOpen == false &&
			                                        !cexit.Exit.Door.CanSeeThrough(actor.Body))
			                                    {
				                                    actor.Send(
					                                    $"You can see nothing to the {cexit.OutboundDirectionDescription} because {cexit.Exit.Door.Parent.HowSeen(actor)} is closed.");
				                                    return;
			                                    }

			                                    actor.OutputHandler.Send(
				                                    new EmoteOutput(
					                                    new Emote(
						                                    $"@ scan|scans the distance towards {cexit.OutboundDirectionDescription}.",
						                                    actor)));

			                                    var cells =
				                                    cexit.Destination.CellsInVicinity(2, true, true,
					                                         Constants.CardinalDirections.Where(x =>
						                                                  !x.IsOpposingDirection(
							                                                  cexit.OutboundDirection))
					                                                  .ToList(), cexit.OutboundDirection)
				                                         .Except(actor.Location)
				                                         .Select(x => (Cell: x,
					                                         Path: actor.PathBetween(x, 5, false, true, true)))
				                                         .Where(x => x.Path.Count() > 0)
				                                         .OrderBy(x => x.Path.Count())
				                                         .ToList();
			                                    var sb = new StringBuilder();
			                                    foreach (var cell in cells)
			                                    {
				                                    var distance = cell.Path.Select(x => x.OutboundDirection)
				                                                       .DistanceAsCrowFlies();
				                                    difficulty = cell.Cell.SpotDifficulty(actor).StageUp(distance - 1);
				                                    outcome = allOutcomes[difficulty];
				                                    minSize = actor.CurrentContextualSize(SizeContext.None);
				                                    switch (outcome.Outcome)
				                                    {
					                                    case Outcome.MajorFail:
						                                    minSize = minSize.ChangeSize(2);
						                                    break;
					                                    case Outcome.Fail:
						                                    minSize = minSize.ChangeSize(1);
						                                    break;
					                                    case Outcome.MinorPass:
						                                    minSize = minSize.ChangeSize(-1);
						                                    break;
					                                    case Outcome.Pass:
						                                    minSize = minSize.ChangeSize(-2);
						                                    break;
					                                    case Outcome.MajorPass:
						                                    minSize = minSize.ChangeSize(-3);
						                                    break;
				                                    }

				                                    if (minSize < userSetSize)
				                                    {
					                                    minSize = userSetSize;
				                                    }

				                                    var majorDirection =
					                                    cell.Path.DescribeDirection().ToLowerInvariant();

				                                    var visibleItems = cell.Cell.GameItems.Where(
					                                                           x => x.RoomLayer.CanBeSeenFromLayer(
							                                                           actor.RoomLayer) &&
						                                                           x.Size >= minSize &&
						                                                           actor.CanSee(x,
							                                                           PerceiveIgnoreFlags
								                                                           .IgnoreObscured))
				                                                           .ToList();
				                                    var visibleCharacters = cell.Cell.Characters.Where(
						                                    x => x.RoomLayer.CanBeSeenFromLayer(actor.RoomLayer) &&
						                                         x.CurrentContextualSize(SizeContext.Scan) >= minSize &&
						                                         actor.CanSee(x, PerceiveIgnoreFlags.IgnoreObscured))
					                                    .ToList();

				                                    if (visibleCharacters.Any() || visibleItems.Any())
				                                    {
					                                    switch (cell.Path.Select(x => x.OutboundDirection)
					                                                .DistanceAsCrowFlies())
					                                    {
						                                    case 1:
							                                    sb.AppendLine(
								                                    $"Immediately to {majorDirection} in {cell.Cell.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreLayers)} you can see:");
							                                    break;
						                                    case 2:
							                                    sb.AppendLine(
								                                    $"Close by to {majorDirection} in {cell.Cell.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreLayers)} you can see:");
							                                    break;
						                                    case 3:
							                                    sb.AppendLine(
								                                    $"Far to {majorDirection} in {cell.Cell.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreLayers)} you can see:");
							                                    break;
						                                    case 4:
							                                    sb.AppendLine(
								                                    $"Very far to {majorDirection} in {cell.Cell.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreLayers)} you can see:");
							                                    break;
						                                    default:
							                                    sb.AppendLine(
								                                    $"Extremely far to {majorDirection} in {cell.Cell.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreLayers)} you can see:");
							                                    break;
					                                    }
				                                    }
				                                    else
				                                    {
					                                    continue;
				                                    }

				                                    DisplayForScan(actor, visibleCharacters, sb, true);
				                                    DisplayForScan(actor, visibleItems, sb, true);
			                                    }

			                                    if (sb.Length > 0)
			                                    {
				                                    actor.OutputHandler.Send(sb.ToString(), false, true);
			                                    }
			                                    else
			                                    {
				                                    actor.OutputHandler.Send(
					                                    "You can't see anything worth taking notice of.");
			                                    }
		                                    }),
			"scanning the horizon",
			new[] { "general", "scanning" },
			exits.Count,
			TimeSpan.FromSeconds(5)
		), TimeSpan.FromSeconds(5));
	}

	[PlayerCommand("Longscan", "longscan", "lscan", "ls")]
	[RequiredCharacterState(CharacterState.Able)]
	[HelpInfo("longscan", "Take time to scan around you thoroughly. Syntax: ls [<minsize>] [<specific exit>]",
		AutoHelp.HelpArg)]
	[DelayBlock("general", "You must first stop {0} before you can do that.")]
	[NoMeleeCombatCommand]
	protected static void LongScan(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		List<ICellExit> exits = null;
		var userSetSize = SizeCategory.Nanoscopic;
		if (ss.IsFinished)
		{
			exits = actor.Location.ExitsFor(actor).ToList();
		}
		else
		{
			var sizeArg = ss.Pop();
			var exitArg = ss.IsFinished ? "" : ss.SafeRemainingArgument;
			var sizes = Enum.GetValues(typeof(SizeCategory)).OfType<SizeCategory>();
			var setSize = sizes.FirstOrDefault(x =>
				x.Describe().Equals(sizeArg, StringComparison.InvariantCultureIgnoreCase));
			if (setSize != SizeCategory.Nanoscopic)
			{
				userSetSize = setSize;
			}
			else
			{
				exitArg = sizeArg;
			}

			if (!exitArg.Equals(string.Empty))
			{
				exits = new[] { actor.Location.GetExitKeyword(exitArg, actor) }.ToList();
			}
			else
			{
				exits = actor.Location.ExitsFor(actor).ToList();
			}
		}

		if (exits.Any(x => x == null))
		{
			actor.OutputHandler.Send("There is no such exit for you to scan.");
			return;
		}

		if (!exits.Any() && actor.Location.Terrain(actor).TerrainLayers.Except(actor.RoomLayer)
		                         .All(x => !x.CanBeSeenFromLayer(actor.RoomLayer)))
		{
			actor.OutputHandler.Send("There are no visible exits or visible layers for you to scan.");
			return;
		}

		var scanSpeedOutcome =
			actor.Gameworld.GetCheck(CheckType.LongscanPerceptionCheck).Check(actor, Difficulty.Normal);

		actor.OutputHandler.Handle(new EmoteOutput(new Emote("@ begin|begins to scan the horizon thoroughly.", actor),
			flags: OutputFlags.Insigificant));

		var check = actor.Gameworld.GetCheck(CheckType.LongscanPerceptionCheck);
		var allOutcomes = check.CheckAgainstAllDifficulties(actor, Difficulty.Normal, null);
		var difficulty = actor.Location.SpotDifficulty(actor);
		var outcome = allOutcomes[difficulty];
		var minSize = actor.CurrentContextualSize(SizeContext.None).ChangeSize(-3);
		switch (outcome.Outcome)
		{
			case Outcome.MinorPass:
				minSize = minSize.ChangeSize(-1);
				break;
			case Outcome.Pass:
				minSize = minSize.ChangeSize(-2);
				break;
			case Outcome.MajorPass:
				minSize = minSize.ChangeSize(-3);
				break;
		}

		if (minSize < userSetSize)
		{
			minSize = userSetSize;
		}

		var sameCellItems = actor.Location.GameItems.Where(x =>
			x.RoomLayer != actor.RoomLayer &&
			x.RoomLayer.CanBeSeenFromLayer(actor.RoomLayer) &&
			x.Size >= minSize &&
			actor.CanSee(x, PerceiveIgnoreFlags.IgnoreObscured)
		).ToList();
		var sameCellCharacters = actor.Location.Characters.Where(x =>
			x.RoomLayer != actor.RoomLayer &&
			x.RoomLayer.CanBeSeenFromLayer(actor.RoomLayer) &&
			x.CurrentContextualSize(SizeContext.Scan) >= minSize &&
			actor.CanSee(x, PerceiveIgnoreFlags.IgnoreObscured)
		).ToList();

		if (sameCellCharacters.Any() || sameCellItems.Any())
		{
			var isb = new StringBuilder();
			isb.AppendLine($"In the same location as you, you see:");
			DisplayForScan(actor, sameCellCharacters, isb, true);
			DisplayForScan(actor, sameCellItems, isb, true);
			actor.OutputHandler.Send(isb.ToString());
		}

		if (!exits.Any())
		{
			return;
		}

		actor.AddEffect(new Scanning(actor, from exit in exits
		                                    select new Action<IPerceivable>(perc =>
		                                    {
			                                    var cexit = exit;
			                                    if (cexit.Exit.Door?.IsOpen == false &&
			                                        !cexit.Exit.Door.CanSeeThrough(actor.Body))
			                                    {
				                                    actor.Send(
					                                    $"You can see nothing to the {cexit.OutboundDirectionDescription} because {cexit.Exit.Door.Parent.HowSeen(actor)} is closed.");
				                                    return;
			                                    }

			                                    actor.OutputHandler.Send(
				                                    new EmoteOutput(
					                                    new Emote(
						                                    $"@ scan|scans the distance towards {cexit.OutboundDirectionDescription}.",
						                                    actor)));

			                                    var range = actor.MaximumPerceptionRange;
			                                    var cells =
				                                    cexit.Destination.CellsInVicinity(range, true, true,
					                                         Constants.CardinalDirections.Where(x =>
						                                                  !x.IsOpposingDirection(
							                                                  cexit.OutboundDirection))
					                                                  .ToList(), cexit.OutboundDirection)
				                                         .Except(actor.Location)
				                                         .Select(x => (Cell: x,
					                                         Path: actor.PathBetween(x, range, false, true, true)))
				                                         .Where(x => x.Path.Count() > 0)
				                                         .OrderBy(x => x.Path.Count())
				                                         .ToList();
			                                    var sb = new StringBuilder();
			                                    foreach (var cell in cells)
			                                    {
				                                    var distance = cell.Path.Select(x => x.OutboundDirection)
				                                                       .DistanceAsCrowFlies();
				                                    difficulty = cell.Cell.SpotDifficulty(actor).StageUp(distance - 1);
				                                    outcome = allOutcomes[difficulty];
				                                    minSize = actor.CurrentContextualSize(SizeContext.None);
				                                    switch (outcome.Outcome)
				                                    {
					                                    case Outcome.MajorFail:
						                                    minSize = minSize.ChangeSize(2);
						                                    break;
					                                    case Outcome.Fail:
						                                    minSize = minSize.ChangeSize(1);
						                                    break;
					                                    case Outcome.MinorPass:
						                                    minSize = minSize.ChangeSize(-1);
						                                    break;
					                                    case Outcome.Pass:
						                                    minSize = minSize.ChangeSize(-2);
						                                    break;
					                                    case Outcome.MajorPass:
						                                    minSize = minSize.ChangeSize(-3);
						                                    break;
				                                    }

				                                    if (minSize < userSetSize)
				                                    {
					                                    minSize = userSetSize;
				                                    }

				                                    var majorDirection =
					                                    cell.Path.DescribeDirection().ToLowerInvariant();
				                                    var visibleItems = cell.Cell.GameItems.Where(
					                                                           x =>
						                                                           x.RoomLayer
							                                                           .CanBeSeenFromLayerForLongscan(
								                                                           actor.RoomLayer) &&
						                                                           x.Size >= minSize &&
						                                                           actor.CanSee(x,
							                                                           PerceiveIgnoreFlags
								                                                           .IgnoreObscured))
				                                                           .ToList();
				                                    var visibleCharacters = cell.Cell.Characters.Where(
						                                    x =>
							                                    x.RoomLayer.CanBeSeenFromLayerForLongscan(
								                                    actor.RoomLayer) &&
							                                    x.CurrentContextualSize(SizeContext.Scan) >= minSize &&
							                                    actor.CanSee(x, PerceiveIgnoreFlags.IgnoreObscured))
					                                    .ToList();

				                                    if (visibleCharacters.Any() || visibleItems.Any())
				                                    {
					                                    switch (cell.Path.Select(x => x.OutboundDirection)
					                                                .DistanceAsCrowFlies())
					                                    {
						                                    case 1:
							                                    sb.AppendLine(
								                                    $"Immediately to {majorDirection} in {cell.Cell.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreLayers)} you can see:");
							                                    break;
						                                    case 2:
							                                    sb.AppendLine(
								                                    $"Close by to {majorDirection} in {cell.Cell.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreLayers)} you can see:");
							                                    break;
						                                    case 3:
							                                    sb.AppendLine(
								                                    $"Far to {majorDirection} in {cell.Cell.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreLayers)} you can see:");
							                                    break;
						                                    case 4:
							                                    sb.AppendLine(
								                                    $"Very far to {majorDirection} in {cell.Cell.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreLayers)} you can see:");
							                                    break;
						                                    default:
							                                    sb.AppendLine(
								                                    $"Extremely far to {majorDirection} in {cell.Cell.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreLayers)} you can see:");
							                                    break;
					                                    }
				                                    }

				                                    DisplayForScan(actor, visibleCharacters, sb, true);
				                                    DisplayForScan(actor, visibleItems, sb, true);
			                                    }

			                                    if (sb.Length > 0)
			                                    {
				                                    actor.OutputHandler.Send(sb.ToString(), false, true);
			                                    }
			                                    else
			                                    {
				                                    actor.OutputHandler.Send(
					                                    "You can't see anything worth taking notice of.");
			                                    }
		                                    }),
			"scanning the horizon",
			new[] { "general", "scanning" },
			exits.Count,
			TimeSpan.FromSeconds(25 - 3 * scanSpeedOutcome.SuccessDegrees())
		), TimeSpan.FromSeconds(25 - 3 * scanSpeedOutcome.SuccessDegrees()));
	}

	[PlayerCommand("Watch", "watch")]
	[RequiredCharacterState(CharacterState.Conscious)]
	[NoMeleeCombatCommand]
	protected static void Watch(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		if (ss.Peek().EqualToAny("none", "off", "stop", "clear"))
		{
			if (actor.EffectsOfType<WatchMaster>().Any())
			{
				actor.RemoveAllEffects(x => x.IsEffectType<WatchMaster>());
				actor.OutputHandler.Send("You are no longer watching your surroundings.");
				return;
			}

			actor.OutputHandler.Send("You aren't watching anything in particular to stop.");
			return;
		}

		var exits = new List<ICellExit>();
		if (ss.Peek().EqualTo("all"))
		{
			exits.AddRange(actor.Location.ExitsFor(actor));
		}
		else
		{
			while (!ss.IsFinished)
			{
				var exit = actor.Location.GetExitKeyword(ss.PopSpeech(), actor);
				if (exit == null)
				{
					actor.OutputHandler.Send($"There is no exit with a keyword of {ss.Last.Colour(Telnet.Yellow)}.");
					return;
				}

				if (exit.Exit.Door?.IsOpen == false && !exit.Exit.Door.CanSeeThrough(actor.Body))
				{
					actor.OutputHandler.Send(
						$"You can't see through {exit.Exit.Door.Parent.HowSeen(actor)}, so you can't watch the location with the keyword {ss.Last.Colour(Telnet.Yellow)}.");
					return;
				}

				if (!exits.Contains(exit))
				{
					exits.Add(exit);
				}
			}
		}

		if (!exits.Any())
		{
			actor.OutputHandler.Send("You don't see any exits like that to watch.");
			return;
		}

		actor.RemoveAllEffects(x => x.IsEffectType<WatchMaster>());
		var effect = new WatchMaster(actor);
		actor.AddEffect(effect);
		foreach (var exit in exits)
		{
			effect.AddSpiedCell(exit.Destination);
		}

		actor.OutputHandler.Send(
			$"You begin watching the locations accessed via the exits to {exits.Select(x => x.OutboundDirectionDescription).ListToString()}.");
	}

	[PlayerCommand("Point", "point")]
	[RequiredCharacterState(CharacterState.Able)]
	[HelpInfo("point",
		"This command allows you to point out targets that you have sighted to everyone in the same location as you. The syntax is POINT <target> or POINT CURRENT to point at somebody you're targeting in combat. The targets are ordered in the same order that they appear in the TARGETS command for the purposes of working out the best keywords.",
		AutoHelp.HelpArgOrNoArg)]
	protected static void Point(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		var targetText = ss.PopSpeech();
		IMortalPerceiver target;
		if (targetText.EqualTo("current") && actor.CombatTarget != null)
		{
			target = actor.CombatTarget as IMortalPerceiver;
		}
		else
		{
			target = actor.SeenTargets.Where(x => actor.CanSee(x)).GetFromItemListByKeyword(targetText, actor);
		}

		if (target == null)
		{
			actor.OutputHandler.Send("You don't have eyes on any targets like that.");
			return;
		}

		PlayerEmote pemote = null;
		if (!ss.IsFinished)
		{
			pemote = new PlayerEmote(ss.SafeRemainingArgument, actor);
			if (!pemote.Valid)
			{
				actor.OutputHandler.Send(pemote.ErrorMessage);
				return;
			}
		}

		if (actor.ColocatedWith(target))
		{
			actor.OutputHandler.Handle(new MixedEmoteOutput(new Emote($"@ point|points out $1", actor, actor, target))
				.Append(pemote));
		}
		else if (actor.Location == target.Location)
		{
			actor.OutputHandler.Handle(
				new MixedEmoteOutput(new Emote($"@ point|points out $1, {target.RoomLayer.LocativeDescription()}",
					actor, actor, target)).Append(pemote));
		}
		else
		{
			var path = actor.PathBetween(target, 10, PathSearch.IncludeFireableDoors).ToList();
			if (!path.Any())
			{
				actor.OutputHandler.Send("You can no longer see that target.");
				actor.LoseTarget(target);
				return;
			}

			var distanceDescriptor = "extremely far";
			switch (path.Select(x => x.OutboundDirection).DistanceAsCrowFlies())
			{
				case 1:
					distanceDescriptor = "immediately";
					break;
				case 2:
					distanceDescriptor = "close by";
					break;
				case 3:
					distanceDescriptor = "far";
					break;
				case 4:
					distanceDescriptor = "very far";
					break;
			}

			actor.OutputHandler.Handle(new MixedEmoteOutput(new Emote(
				$"@ point|points out $1, {distanceDescriptor} to {path.DescribeDirection()} in $2", actor, actor,
				target, target.Location)).Append(pemote));
		}


		foreach (var person in actor.Location.LayerCharacters(actor.RoomLayer))
		{
			person.SeeTarget(target);
		}
	}

	[PlayerCommand("Tracks", "tracks")]
	[CommandPermission(PermissionLevel.Admin)]
	protected static void Tracks(ICharacter actor, string input)
	{
		// TODO - for debug only
		var tracks = actor.Location.Tracks.Where(x => x.RoomLayer == actor.RoomLayer).ToList();
		if (tracks.Count == 0)
		{
			actor.OutputHandler.Send("There are no tracks here.");
			return;
		}
		var sb = new StringBuilder();
		sb.AppendLine("There are the following tracks here:");
		sb.AppendLine();
		var i = 1;
		foreach (var track in tracks)
		{
			sb.AppendLine($"{i++.ToString("N0", actor)}) {track.DescribeForTracksCommand(actor)}");
		}

		actor.OutputHandler.Send(sb.ToString());
	}
}