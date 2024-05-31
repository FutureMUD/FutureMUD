using System;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;

namespace MudSharp.Work.Crafts.Tools;

public class TagTool : BaseTool
{
	protected TagTool(Models.CraftTool tool, ICraft craft, IFuturemud gameworld) : base(tool, craft, gameworld)
	{
		var root = XElement.Parse(tool.Definition);
		TargetItemTag = gameworld.Tags.Get(long.Parse(root.Element("TargetItemTag").Value));
	}

	protected TagTool(ICraft craft, IFuturemud gameworld) : base(craft, gameworld)
	{
	}

	public ITag TargetItemTag { get; set; }

	/// <inheritdoc />
	public override bool RefersToTag(ITag tag)
	{
		return TargetItemTag?.IsA(tag) == true;
	}

	#region Overrides of BaseTool

	public override bool IsTool(IGameItem item)
	{
		return item.GetItemType<IToolItem>()?.CountAsTool(TargetItemTag) ??
		       item?.Tags.Any(x => x.IsA(TargetItemTag)) == true;
	}

	public override double ToolFitness(IGameItem item)
	{
		var tool = item.GetItemType<IToolItem>();
		if (tool == null)
		{
			return 1.0;
		}

		if (tool.ToolTimeMultiplier(TargetItemTag) <= 0.0)
		{
			return 0.0;
		}

		return 1.0 / tool.ToolTimeMultiplier(TargetItemTag);
	}

	public override void UseTool(IGameItem item, TimeSpan phaseLength, bool hasFailed)
	{
		if (!UseToolDuration)
		{
			return;
		}
		item.GetItemType<IToolItem>()?.UseTool(TargetItemTag, phaseLength);
	}

	public override double PhaseLengthMultiplier(IGameItem item)
	{
		return item.GetItemType<IToolItem>()?.ToolTimeMultiplier(TargetItemTag) ?? 1.0;
	}

	public override string ToolType => "TagTool";

	protected override string SaveDefinition()
	{
		return new XElement("Definition", new XElement("TargetItemTag", TargetItemTag?.Id ?? 0)).ToString();
	}

	#endregion

	public static void RegisterCraftTool()
	{
		CraftToolFactory.RegisterCraftToolType("TagTool", (input, craft, game) => new TagTool(input, craft, game));
		CraftToolFactory.RegisterCraftToolTypeForBuilders("tag", (craft, game) => new TagTool(craft, game));
	}

	public override string Name
	{
		get
		{
			if (TargetItemTag == null)
			{
				return "an item with an unspecified tag".Colour(Telnet.Red);
			}

			return $"an item with the {TargetItemTag?.Name.Colour(Telnet.Cyan)} tag";
		}
	}

	public override string HowSeen(IPerceiver voyeur)
	{
		if (TargetItemTag == null)
		{
			return "an item with an unspecified tag".Colour(Telnet.Red);
		}

		return $"an item with the {TargetItemTag?.Name.Colour(Telnet.Cyan)} tag";
	}

	public override bool IsValid()
	{
		return TargetItemTag != null;
	}

	public override string WhyNotValid()
	{
		return "You must first set a target item tag.";
	}

	public override string BuilderHelpString =>
		$"{base.BuilderHelpString}\n\t#3tag <id|name>#0 - sets the target tag required for this tool";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "tag":
			case "item":
				return BuildingCommandTag(actor, command);
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	private bool BuildingCommandTag(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which tag do you want items to have to satisfy this tool?");
			return false;
		}

		var matchedtags = actor.Gameworld.Tags.FindMatchingTags(command.SafeRemainingArgument);
		if (matchedtags.Count == 0)
		{
			actor.OutputHandler.Send("There is no such tag.");
			return false;
		}

		if (matchedtags.Count > 1)
		{
			actor.OutputHandler.Send(
				$"Your text matched multiple tags. Please specify one of the following tags:\n\n{matchedtags.Select(x => $"\t[{x.Id.ToString("N0", actor)}] {x.FullName.ColourName()}").ListToLines()}");
			return false;
		}

		var tag = matchedtags.Single();

		TargetItemTag = tag;
		ToolChanged = true;
		actor.OutputHandler.Send($"This tool will now target items with the {tag.FullName.Colour(Telnet.Cyan)} tag.");
		return true;
	}
}