using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;

namespace MudSharp.GameItems.Inventory;

/// <summary>
///     A DirectWearProfile is an implementation of IWearProfile that looks directly for specific wear locations when
///     finding matches
/// </summary>
public class DirectWearProfile : WearProfile
{
	public DirectWearProfile(MudSharp.Models.WearProfile profile, IFuturemud game)
	{
		_id = profile.Id;
		_name = profile.Name;
		Gameworld = game;
		DesignedBody = game.BodyPrototypes.Get(profile.BodyPrototypeId);
		Description = profile.Description;
		WearStringInventory = profile.WearStringInventory;
		WearAction1st = profile.WearAction1st;
		WearAction3rd = profile.WearAction3rd;
		WearAffix = profile.WearAffix;
		RequireContainerIsEmpty = profile.RequireContainerIsEmpty;
		try
		{
			LoadWearlocsFromXml(XElement.Parse(profile.WearlocProfiles));
		}
		catch (Exception e)
		{
			throw new ApplicationException(
				$"Exception in LoadWearlocsFromXml for DirectWearProfile {Id:N0} ({Name}) - {e.Message}");
		}
	}

	protected DirectWearProfile(DirectWearProfile rhs, string name)
	{
		Gameworld = rhs.Gameworld;
		_name = name;
		DesignedBody = rhs.DesignedBody;
		Description = rhs.Description;
		WearAction1st = rhs.WearAction1st;
		WearAction3rd = rhs.WearAction3rd;
		WearStringInventory = rhs.WearStringInventory;
		WearAffix = rhs.WearAffix;
		RequireContainerIsEmpty = rhs.RequireContainerIsEmpty;
		using (new FMDB())
		{
			var dbitem = new Models.WearProfile
			{
				Name = Name,
				Description = Description,
				BodyPrototypeId = DesignedBody.Id,
				WearAction1st = WearAction1st,
				WearAction3rd = WearAction3rd,
				WearAffix = WearAffix,
				WearStringInventory = WearStringInventory,
				RequireContainerIsEmpty = RequireContainerIsEmpty,
				WearlocProfiles = SaveDefinition().ToString()
			};
			FMDB.Context.WearProfiles.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
			LoadWearlocsFromXml(XElement.Parse(dbitem.WearlocProfiles));
		}
	}

	public override IWearProfile Clone(string newName)
	{
		return new DirectWearProfile(this, newName);
	}

	public override string FrameworkItemType => "DirectWearProfile";

	public Dictionary<IBodypart, IWearlocProfile> Locations { get; protected set; }

	public override string Type => "Direct";

	public override Dictionary<IWear, IWearlocProfile> AllProfiles
	{
		get
		{
			var result = new Dictionary<IWear, IWearlocProfile>();
			foreach (var loc in Locations)
			{
				if (loc.Key is IWear key)
				{
					result.Add(key, loc.Value);
				}
			}

			return result;
		}
	}

	public override string ShowTo(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine(string.Format(actor, "Wear Profile #{0:N0}", Id).Colour(Telnet.Cyan));
		sb.AppendLine();
		sb.Append(new[]
		{
			string.Format(actor, "Name: {0}", Name.ColourValue()),
			string.Format(actor, "Body: {0} (#{1})", DesignedBody?.Name.TitleCase().ColourValue() ?? "None".ColourError(),
				DesignedBody?.Id ?? -1),
			string.Format(actor, "Type: {0}", Type.ColourValue())
		}.ArrangeStringsOntoLines(3, (uint)actor.Account.LineFormatLength));
		sb.Append(new[]
		{
			string.Format(actor, "Inventory: {0:N0}", WearStringInventory.ColourValue()),
			string.Format(actor, "Wear: #2{0}|{1} {2}#0".SubstituteANSIColour(), WearAction1st, WearAction3rd, WearAffix),
			string.Format(actor, "Require Empty: {0}", RequireContainerIsEmpty.ToColouredString())
		}.ArrangeStringsOntoLines(3, (uint)actor.Account.LineFormatLength));
		sb.AppendLine($"Description:\n\n{Description.Wrap(80, "\t").ColourCommand()}");
		sb.AppendLine();
		sb.Append(
			StringUtilities.GetTextTable(
				from location in Locations
				select new[]
				{
					location.Key.FullDescription().TitleCase(),
					location.Value.Mandatory ? "True" : "False",
					location.Value.PreventsRemoval ? "True" : "False",
					location.Value.Transparent ? "True" : "False",
					location.Value.NoArmour ? "True" : "False",
					location.Value.HidesSeveredBodyparts ? "True" : "False"
				},
				new[] { "Bodypart", "Mandatory", "Prevent Removal", "Transparent", "No Armour", "Covers Sever" },
				actor,
				colour: Telnet.Green
			)
		);
		return sb.ToString();
	}

	private void LoadWearlocsFromXml(XElement root)
	{
		try
		{
			Locations = new Dictionary<IBodypart, IWearlocProfile>();
			foreach (var loc in root.Elements("Profile"))
			{
				Locations.Add(
					long.TryParse(loc.Attribute("Bodypart").Value, out var value)
						? DesignedBody.AllExternalBodyparts.FirstOrDefault(x => x.Id == value)
						: DesignedBody.AllExternalBodyparts.FirstOrDefault(
							x =>
								x.Name.Equals(loc.Attribute("Bodypart").Value,
									StringComparison.InvariantCultureIgnoreCase)), new WearlocProfile(loc));
			}
		}
		catch (Exception e)
		{
			throw new ApplicationException(
				$"Exception in LoadWearlocsFromXml for DirectWearProfile {Id:N0} ({Name}) - {e.Message}");
		}
	}

	protected XElement SaveDefinition()
	{
		return new XElement("Profiles",
			from item in Locations
			select new XElement("Profile",
				new XAttribute("Bodypart", item.Key.Id),
				new XAttribute("Transparent", item.Value.Transparent),
				new XAttribute("NoArmour", item.Value.NoArmour),
				new XAttribute("PreventsRemoval", item.Value.PreventsRemoval),
				new XAttribute("Mandatory", item.Value.Mandatory),
				new XAttribute("HidesSevered", item.Value.HidesSeveredBodyparts)
			)
		);
	}

	public override void Save()
	{
		var dbitem = FMDB.Context.WearProfiles.Find(Id);
		dbitem.Description = Description;
		dbitem.Name = Name;
		dbitem.BodyPrototypeId = DesignedBody?.Id ?? 0;
		dbitem.WearStringInventory = WearStringInventory;
		dbitem.WearAction1st = WearAction1st;
		dbitem.WearAction3rd = WearAction3rd;
		dbitem.WearAffix = WearAffix;
		dbitem.RequireContainerIsEmpty = RequireContainerIsEmpty;
		dbitem.WearlocProfiles = SaveDefinition().ToString();
		Changed = false;
	}

	public override Dictionary<IWear, IWearlocProfile> Profile(IBody body)
	{
		var result = new Dictionary<IWear, IWearlocProfile>();
		foreach (var loc in Locations)
		{
			var targetloc = body.WearLocs.FirstOrDefault(x => x.CountsAs(loc.Key));
			if (targetloc == null)
			{
				if (loc.Value.Mandatory)
				{
					return null;
				}

				continue;
			}

			result.Add(targetloc, loc.Value);
		}

		return result;
	}

	public override bool CompatibleWith(IWearProfile otherProfile)
	{
		if (otherProfile is ShapeWearProfile shapeProfile)
		{
			return shapeProfile.ShapeCounts.All(x =>
				!x.Value.Item2.Mandatory || AllProfiles.Count(y => y.Key.Shape == x.Key) >= x.Value.Item1);
		}

		return otherProfile.AllProfiles.All(x => !x.Value.Mandatory || AllProfiles.Any(y => y.Key.CountsAs(x.Key)));
	}

	#region Building Commands

	/// <inheritdoc />
	protected override string SubtypeBuildingHelp => @"	#3add <part>#0 - adds a body part to the definition
	#3remove <part>#0 - removes a body part from the definition
	#3set <part> transparent#0 - toggles the cover at the part being transparent
	#3set <part> noarmour#0 - toggles the cover at the part not providing armour
	#3set <part> preventsremoval#0 - toggles the cover at the part preventing covered items being removed
	#3set <part> mandatory#0 - toggles the cover at the part being mandatory
	#3set <part> hidessevered#0 - toggles the cover at the part hiding severed body parts";

	public override void BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "add":
			case "new":
				BuildingCommandAdd(actor, command);
				return;
			case "remove":
			case "delete":
			case "del":
			case "rem":
				BuildingCommandRemove(actor, command);
				return;
			case "set":
			case "edit":
				BuildingCommandSet(actor, command);
				return;
			default:
				base.BuildingCommand(actor, command);
				return;
		}
	}

	private void BuildingCommandSet(ICharacter actor, StringStack command)
	{
		if (DesignedBody == null)
		{
			actor.Send("You must first select a body design before you can do anything with individual bodyparts.");
			return;
		}

		if (command.IsFinished)
		{
			actor.Send("Which bodypart do you want to set a parameter of?");
			return;
		}

		var part = long.TryParse(command.PopSpeech(), out var value)
			? Locations.Select(x => x.Key).FirstOrDefault(x => x.Id == value)
			: Locations.Select(x => x.Key)
			           .FirstOrDefault(
				           x => x.FullDescription()
				                 .Equals(command.Last, StringComparison.InvariantCultureIgnoreCase)) ??
			  Locations.Select(x => x.Key)
			           .FirstOrDefault(x => x.Name.Equals(command.Last, StringComparison.InvariantCultureIgnoreCase));

		if (part == null)
		{
			actor.Send("There is no such bodypart on this profile for you to set any parameter of.");
			return;
		}

		bool bValue;
		var whichCommand = command.Pop().ToLowerInvariant();
		switch (whichCommand)
		{
			case "transparent":
				if (command.IsFinished)
				{
					Locations[part].Transparent = !Locations[part].Transparent;
					break;
				}

				if (!bool.TryParse(command.Pop(), out bValue))
				{
					actor.Send(
						"You can either leave the final argument blank to toggle, or you can use true or false.");
					return;
				}

				Locations[part].Transparent = bValue;
				break;
			case "noarmour":
			case "noarmor":
			case "armor":
			case "armour":
				if (command.IsFinished)
				{
					Locations[part].NoArmour = !Locations[part].NoArmour;
					break;
				}

				if (!bool.TryParse(command.Pop(), out bValue))
				{
					actor.Send(
						"You can either leave the final argument blank to toggle, or you can use true or false.");
					return;
				}

				Locations[part].NoArmour = bValue;
				break;
			case "preventremoval":
			case "preventsremoval":
				if (command.IsFinished)
				{
					Locations[part].PreventsRemoval = !Locations[part].PreventsRemoval;
					break;
				}

				if (!bool.TryParse(command.Pop(), out bValue))
				{
					actor.Send(
						"You can either leave the final argument blank to toggle, or you can use true or false.");
					return;
				}

				Locations[part].PreventsRemoval = bValue;
				break;
			case "mandatory":
				if (command.IsFinished)
				{
					Locations[part].Mandatory = !Locations[part].Mandatory;
					break;
				}

				if (!bool.TryParse(command.Pop(), out bValue))
				{
					actor.Send(
						"You can either leave the final argument blank to toggle, or you can use true or false.");
					return;
				}

				Locations[part].Mandatory = bValue;
				break;
			case "hidessevered":
			case "hide":
			case "hidesevered":
			case "hidesever":
			case "hidessever":
			case "sever":
				if (command.IsFinished)
				{
					Locations[part].HidesSeveredBodyparts = !Locations[part].HidesSeveredBodyparts;
					break;
				}

				if (!bool.TryParse(command.Pop(), out bValue))
				{
					actor.Send(
						"You can either leave the final argument blank to toggle, or you can use true or false.");
					return;
				}

				Locations[part].HidesSeveredBodyparts = bValue;
				break;
			default:
				actor.Send(
					$"Which parameter do you want to edit for the {part.FullDescription().Colour(Telnet.Green)} part? Valid choices are transparent, noarmour, preventremoval, mandatory and hidessevered.");
				return;
		}

		Changed = true;
		switch (whichCommand)
		{
			case "transparent":
				actor.Send(
					$"Wear Profile {Id:N0} ({Name}) with the {part.FullDescription().Colour(Telnet.Green)} part will {(Locations[part].Transparent ? "now" : "no longer")} be transparent.");
				break;
			case "noarmour":
				actor.Send(
					$"Wear Profile {Id:N0} ({Name}) with the {part.FullDescription().Colour(Telnet.Green)} part will {(Locations[part].NoArmour ? "now" : "no longer")} count as armour.");
				break;
			case "preventremoval":
			case "preventsremoval":
				actor.Send(
					$"Wear Profile {Id:N0} ({Name}) with the {part.FullDescription().Colour(Telnet.Green)} part will {(Locations[part].PreventsRemoval ? "now" : "no longer")} prevent removal.");
				break;
			case "mandatory":
				actor.Send(
					$"Wear Profile {Id:N0} ({Name}) with the {part.FullDescription().Colour(Telnet.Green)} part will {(Locations[part].Mandatory ? "now" : "no longer")} be mandatory.");
				break;
			case "hidessevered":
				actor.Send(
					$"Wear Profile {Id:N0} ({Name}) with the {part.FullDescription().Colour(Telnet.Green)} part will {(Locations[part].HidesSeveredBodyparts ? "now" : "no longer")} hide severed bodyparts.");
				break;
		}
	}

	private void BuildingCommandRemove(ICharacter actor, StringStack command)
	{
		if (DesignedBody == null)
		{
			actor.Send("You must first select a body design before you can do anything with individual bodyparts.");
			return;
		}

		if (command.IsFinished)
		{
			actor.Send("Which bodypart do you want to remove?");
			return;
		}

		var part = long.TryParse(command.PopSpeech(), out var value)
			? Locations.Select(x => x.Key).FirstOrDefault(x => x.Id == value)
			: Locations.Select(x => x.Key)
			           .FirstOrDefault(
				           x => x.FullDescription()
				                 .Equals(command.Last, StringComparison.InvariantCultureIgnoreCase)) ??
			  Locations.Select(x => x.Key)
			           .FirstOrDefault(x => x.Name.Equals(command.Last, StringComparison.InvariantCultureIgnoreCase));

		if (part == null)
		{
			actor.Send("There is no such bodypart on this profile for you to remove.");
			return;
		}

		Locations.Remove(part);
		Changed = true;
		actor.Send(
			$"Wear Profile {Id:N0} ({Name}) no longer has a role to play on the {part.FullDescription().Colour(Telnet.Green)} part.");
	}

	private void BuildingCommandAdd(ICharacter actor, StringStack command)
	{
		if (DesignedBody == null)
		{
			actor.Send("You must first select a body design before you can do anything with individual bodyparts.");
			return;
		}

		if (command.IsFinished)
		{
			actor.Send("Which bodypart do you want to set a parameter of?");
			return;
		}

		var part = long.TryParse(command.PopSpeech(), out var value)
			? DesignedBody.AllBodyparts.FirstOrDefault(x => x.Id == value)
			: DesignedBody.AllBodyparts.FirstOrDefault(
				  x => x.FullDescription().Equals(command.Last, StringComparison.InvariantCultureIgnoreCase)) ??
			  DesignedBody.AllBodyparts.FirstOrDefault(
				  x => x.Name.Equals(command.Last, StringComparison.InvariantCultureIgnoreCase));

		if (part == null)
		{
			actor.Send("There is no such bodypart for you to add.");
			return;
		}

		if (Locations.ContainsKey(part))
		{
			actor.Send(
				$"Wear Profile {Id:N0} ({Name}) already contains the {part.FullDescription().Colour(Telnet.Green)} part");
			return;
		}

		Locations[part] = new WearlocProfile();
		Changed = true;
		actor.Send(
			$"Wear Profile {Id:N0} ({Name}) now has a removal-preventing, non-mandatory, non-transparent, armour-allowing, non-sever covering role at the {part.FullDescription().Colour(Telnet.Green)} part.");
	}

	#endregion
}