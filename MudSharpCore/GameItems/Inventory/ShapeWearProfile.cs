using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;

namespace MudSharp.GameItems.Inventory;

public class ShapeWearProfile : WearProfile
{
	public readonly Dictionary<IBodypartShape, Tuple<int, IWearlocProfile>> ShapeCounts =
		new();

	public ShapeWearProfile(MudSharp.Models.WearProfile profile, IFuturemud game)
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
		LoadWearlocsFromXml(XElement.Parse(profile.WearlocProfiles), game);
	}

	public override string FrameworkItemType => "ShapeWearProfile";
	public override string Type => "Shape";

	public override Dictionary<IWear, IWearlocProfile> AllProfiles
	{
		get
		{
			var result = new Dictionary<IWear, IWearlocProfile>();
			foreach (var shape in ShapeCounts)
			{
				var parts =
					DesignedBody.AllBodyparts.Where(x => x.Shape == shape.Key)
					            .OfType<IWear>()
					            .Take(shape.Value.Item1)
					            .ToList();
				foreach (var part in parts)
				{
					result.Add(part, shape.Value.Item2);
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
			string.Format(actor, "Name: {0}", Name),
			string.Format(actor, "Body: {0} (#{1})", DesignedBody?.Name.TitleCase() ?? "None",
				DesignedBody?.Id ?? -1),
			string.Format(actor, "Type: {0}", Type)
		}.ArrangeStringsOntoLines(3, (uint)actor.Account.LineFormatLength));
		sb.Append(new[]
		{
			string.Format(actor, "Inventory: {0:N0}", WearStringInventory),
			string.Format(actor, "Wear: {0}|{1} {2}", WearAction1st, WearAction3rd, WearAffix),
			string.Format(actor, "Require Empty: {0}", RequireContainerIsEmpty)
		}.ArrangeStringsOntoLines(3, (uint)actor.Account.LineFormatLength));
		sb.AppendLine($"Description:\n\n{Description.Wrap(80, "\t")}");
		sb.AppendLine();
		sb.Append(
			StringUtilities.GetTextTable(
				from location in ShapeCounts
				select new[]
				{
					location.Key.Name.TitleCase(),
					location.Value.Item1.ToString("N0", actor),
					location.Value.Item2.Mandatory ? "True" : "False",
					location.Value.Item2.PreventsRemoval ? "True" : "False",
					location.Value.Item2.Transparent ? "True" : "False",
					location.Value.Item2.NoArmour ? "True" : "False",
					location.Value.Item2.HidesSeveredBodyparts ? "True" : "False"
				},
				new[]
					{ "Shape", "Min", "Mandatory", "Prevent Removal", "Transparent", "No Armour", "Covers Sever" },
				actor.Account.LineFormatLength,
				colour: Telnet.Green
			)
		);
		return sb.ToString();
	}

	private void LoadWearlocsFromXml(XElement root, IFuturemud game)
	{
		foreach (var shape in root.Elements("Shape"))
		{
			ShapeCounts.Add(
				long.TryParse(shape.Attribute("ShapeId").Value, out var value)
					? game.BodypartShapes.Get(value)
					: game.BodypartShapes.FirstOrDefault(
						x =>
							x.Name.Equals(shape.Attribute("ShapeId").Value,
								StringComparison.InvariantCultureIgnoreCase)),
				Tuple.Create(Convert.ToInt32(shape.Attribute("Count").Value),
					(IWearlocProfile)new WearlocProfile(shape)));
		}
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
		dbitem.WearlocProfiles = new XElement("Profiles",
			from item in ShapeCounts
			select new XElement("Shape",
				new XAttribute("ShapeId", item.Key.Id),
				new XAttribute("Count", item.Value.Item1),
				new XAttribute("Transparent", item.Value.Item2.Transparent),
				new XAttribute("NoArmour", item.Value.Item2.NoArmour),
				new XAttribute("PreventsRemoval", item.Value.Item2.PreventsRemoval),
				new XAttribute("Mandatory", item.Value.Item2.Mandatory),
				new XAttribute("HidesSevered", item.Value.Item2.HidesSeveredBodyparts)
			)
		).ToString();
		Changed = false;
	}

	public override Dictionary<IWear, IWearlocProfile> Profile(IBody body)
	{
		var result = new Dictionary<IWear, IWearlocProfile>();
		var locCounts = body.WornItemCounts;
		foreach (var shape in ShapeCounts)
		{
			var locs = locCounts.Where(x => x.Key.Shape == shape.Key);
			if (locs.Count() < shape.Value.Item1 && shape.Value.Item2.Mandatory)
			{
				return null;
			}

			var final = locs.OrderBy(x => x.First()).Take(shape.Value.Item1);
			foreach (var loc in final)
			{
				result.Add(loc.Key, shape.Value.Item2);
			}
		}

		return result;
	}

	public override bool CompatibleWith(IWearProfile otherProfile)
	{
		if (otherProfile is ShapeWearProfile shapeProfile)
		{
			return shapeProfile.ShapeCounts.All(x =>
				!x.Value.Item2.Mandatory || ShapeCounts.Any(y => y.Key == x.Key && y.Value.Item1 <= x.Value.Item1));
		}

		return otherProfile.AllProfiles.All(x => !x.Value.Mandatory || ShapeCounts.Any(y => y.Key == x.Key.Shape));
	}

	#region Building Commands

	public override void BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.Pop().ToLowerInvariant())
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
			actor.Send("Which shape do you want to set a parameter for?");
			return;
		}

		var shape = long.TryParse(command.PopSpeech(), out var value)
			? ShapeCounts.Select(x => x.Key).FirstOrDefault(x => x.Id == value)
			: ShapeCounts.Select(x => x.Key)
			             .FirstOrDefault(x => x.Name.Equals(command.Last, StringComparison.InvariantCultureIgnoreCase));

		if (shape == null)
		{
			actor.Send("There is no such bodypart shape on this profile for you to set any parameter of.");
			return;
		}

		bool bValue;
		var whichCommand = command.Pop().ToLowerInvariant();
		switch (whichCommand)
		{
			case "count":
				if (command.IsFinished)
				{
					actor.Send("How many of this shape should the wear profile require?");
					return;
				}

				if (!int.TryParse(command.Pop(), out var iValue) || iValue < 1)
				{
					actor.Send("You must enter a valid number of shapes for this wear profile to require.");
					return;
				}

				ShapeCounts[shape] = new Tuple<int, IWearlocProfile>(iValue, ShapeCounts[shape].Item2);
				break;
			case "transparent":
				if (command.IsFinished)
				{
					ShapeCounts[shape].Item2.Transparent = !ShapeCounts[shape].Item2.Transparent;
					break;
				}

				if (!bool.TryParse(command.Pop(), out bValue))
				{
					actor.Send(
						"You can either leave the final argument blank to toggle, or you can use true or false.");
					return;
				}

				ShapeCounts[shape].Item2.Transparent = bValue;
				break;
			case "noarmour":
				if (command.IsFinished)
				{
					ShapeCounts[shape].Item2.NoArmour = !ShapeCounts[shape].Item2.NoArmour;
					break;
				}

				if (!bool.TryParse(command.Pop(), out bValue))
				{
					actor.Send(
						"You can either leave the final argument blank to toggle, or you can use true or false.");
					return;
				}

				ShapeCounts[shape].Item2.NoArmour = bValue;
				break;
			case "preventremoval":
			case "preventsremoval":
				if (command.IsFinished)
				{
					ShapeCounts[shape].Item2.PreventsRemoval = !ShapeCounts[shape].Item2.PreventsRemoval;
					break;
				}

				if (!bool.TryParse(command.Pop(), out bValue))
				{
					actor.Send(
						"You can either leave the final argument blank to toggle, or you can use true or false.");
					return;
				}

				ShapeCounts[shape].Item2.PreventsRemoval = bValue;
				break;
			case "mandatory":
				if (command.IsFinished)
				{
					ShapeCounts[shape].Item2.Mandatory = !ShapeCounts[shape].Item2.Mandatory;
					break;
				}

				if (!bool.TryParse(command.Pop(), out bValue))
				{
					actor.Send(
						"You can either leave the final argument blank to toggle, or you can use true or false.");
					return;
				}

				ShapeCounts[shape].Item2.Mandatory = bValue;
				break;
			case "hidessevered":
				if (command.IsFinished)
				{
					ShapeCounts[shape].Item2.HidesSeveredBodyparts = !ShapeCounts[shape].Item2.HidesSeveredBodyparts;
					break;
				}

				if (!bool.TryParse(command.Pop(), out bValue))
				{
					actor.Send(
						"You can either leave the final argument blank to toggle, or you can use true or false.");
					return;
				}

				ShapeCounts[shape].Item2.HidesSeveredBodyparts = bValue;
				break;
			default:
				actor.Send(
					$"Which parameter do you want to edit for the {shape.Name.Colour(Telnet.Green)} shape? Valid choices are count, transparent, noarmour, preventremoval, mandatory and hidessevered.");
				return;
		}

		Changed = true;
		switch (whichCommand)
		{
			case "count":
				actor.Send(
					$"Wear Profile {Id:N0} ({Name}) with the {shape.Name.Colour(Telnet.Green)} shape will now require {ShapeCounts[shape].Item1:N0} of that shape.");
				break;
			case "transparent":
				actor.Send(
					$"Wear Profile {Id:N0} ({Name}) with the {shape.Name.Colour(Telnet.Green)} shape will {(ShapeCounts[shape].Item2.Transparent ? "now" : "no longer")} be transparent.");
				break;
			case "noarmour":
				actor.Send(
					$"Wear Profile {Id:N0} ({Name}) with the {shape.Name.Colour(Telnet.Green)} shape will {(ShapeCounts[shape].Item2.NoArmour ? "now" : "no longer")} count as armour.");
				break;
			case "preventremoval":
			case "preventsremoval":
				actor.Send(
					$"Wear Profile {Id:N0} ({Name}) with the {shape.Name.Colour(Telnet.Green)} shape will {(ShapeCounts[shape].Item2.PreventsRemoval ? "now" : "no longer")} prevent removal.");
				break;
			case "mandatory":
				actor.Send(
					$"Wear Profile {Id:N0} ({Name}) with the {shape.Name.Colour(Telnet.Green)} shape will {(ShapeCounts[shape].Item2.Mandatory ? "now" : "no longer")} be mandatory.");
				break;
			case "hidessevered":
				actor.Send(
					$"Wear Profile {Id:N0} ({Name}) with the {shape.Name.Colour(Telnet.Green)} shape will {(ShapeCounts[shape].Item2.HidesSeveredBodyparts ? "now" : "no longer")} hide severed bodyparts.");
				break;
		}
	}

	private void BuildingCommandRemove(ICharacter actor, StringStack command)
	{
		if (DesignedBody == null)
		{
			actor.Send(
				"You must first select a body design before you can do anything with individual bodypart shapes.");
			return;
		}

		if (command.IsFinished)
		{
			actor.Send("Which bodypart shape do you want to remove?");
			return;
		}

		var part = long.TryParse(command.PopSpeech(), out var value)
			? ShapeCounts.Select(x => x.Key).FirstOrDefault(x => x.Id == value)
			: ShapeCounts.Select(x => x.Key)
			             .FirstOrDefault(x => x.Name.Equals(command.Last, StringComparison.InvariantCultureIgnoreCase));

		if (part == null)
		{
			actor.Send("There is no such bodypart shape on this profile for you to remove.");
			return;
		}

		ShapeCounts.Remove(part);
		Changed = true;
		actor.Send(
			$"Wear Profile {Id:N0} ({Name}) no longer has a role to play on the {part.Name.Colour(Telnet.Green)} shape.");
	}

	private void BuildingCommandAdd(ICharacter actor, StringStack command)
	{
		if (DesignedBody == null)
		{
			actor.Send(
				"You must first select a body design before you can do anything with individual bodypart shapes.");
			return;
		}

		if (command.IsFinished)
		{
			actor.Send("Which bodypart shape do you want to add?");
			return;
		}

		var part = long.TryParse(command.PopSpeech(), out var value)
			? actor.Gameworld.BodypartShapes.FirstOrDefault(x => x.Id == value)
			: actor.Gameworld.BodypartShapes.FirstOrDefault(
				x => x.Name.Equals(command.Last, StringComparison.InvariantCultureIgnoreCase));

		if (part == null)
		{
			actor.Send("There is no such bodypart shape for you to add.");
			return;
		}

		if (ShapeCounts.ContainsKey(part))
		{
			actor.Send($"Wear Profile {Id:N0} ({Name}) already contains the {part.Name.Colour(Telnet.Green)} shape");
			return;
		}

		ShapeCounts[part] = Tuple.Create(1, (IWearlocProfile)new WearlocProfile());
		Changed = true;
		actor.Send(
			$"Wear Profile {Id:N0} ({Name}) now has a removal-preventing, non-mandatory, non-transparent, armour-allowing, non-sever covering role at the {part.Name.Colour(Telnet.Green)} shape.");
	}

	protected override IEnumerable<string> BuildingOptions => new[] { "add", "remove", "set" };

	#endregion
}