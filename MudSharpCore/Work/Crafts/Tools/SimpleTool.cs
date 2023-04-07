using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.GameItems;

namespace MudSharp.Work.Crafts.Tools;

public class SimpleTool : BaseTool
{
	protected SimpleTool(Models.CraftTool tool, ICraft craft, IFuturemud gameworld) : base(tool, craft, gameworld)
	{
		var root = XElement.Parse(tool.Definition);
		TargetItemId = long.Parse(root.Element("TargetItemId").Value);
	}

	protected SimpleTool(ICraft craft, IFuturemud gameworld) : base(craft, gameworld)
	{
	}

	public long TargetItemId { get; set; }

	#region Overrides of BaseTool

	public override bool IsTool(IGameItem item)
	{
		return item?.Prototype.Id == TargetItemId;
	}

	public override double ToolFitness(IGameItem item)
	{
		return 1.0;
	}

	public override string ToolType => "SimpleTool";

	protected override string SaveDefinition()
	{
		return new XElement("Definition", new XElement("TargetItemId", TargetItemId)).ToString();
	}

	public override string BuilderHelpString =>
		$"{base.BuilderHelpString}\n\titem <id|name> - the item to target for this tool";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "item":
			case "tool":
			case "target":
				return BuildingCommandItem(actor, command);
		}

		return base.BuildingCommand(actor, command);
	}

	private bool BuildingCommandItem(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which item should this tool target?");
			return false;
		}

		var item = long.TryParse(command.SafeRemainingArgument, out var value)
			? Gameworld.ItemProtos.Get(value)
			: Gameworld.ItemProtos.GetByName(command.SafeRemainingArgument);
		if (item == null)
		{
			actor.OutputHandler.Send("There is no such item prototype.");
			return false;
		}

		TargetItemId = item.Id;
		ToolChanged = true;
		actor.OutputHandler.Send(
			$"This tool will now target item prototype #{TargetItemId.ToString("N0", actor)} ({item.ShortDescription.ColourObject()}).");
		return true;
	}

	public override bool IsValid()
	{
		return TargetItemId != 0;
	}

	public override string WhyNotValid()
	{
		return "You must first set a target item for this tool.";
	}

	#endregion

	public static void RegisterCraftTool()
	{
		CraftToolFactory.RegisterCraftToolType("SimpleTool",
			(input, craft, game) => new SimpleTool(input, craft, game));
		CraftToolFactory.RegisterCraftToolTypeForBuilders("simple", (craft, game) => new SimpleTool(craft, game));
	}

	public override string Name => Gameworld.ItemProtos.Get(TargetItemId)?.ShortDescription ??
	                               "an unspecified item".Colour(Telnet.Red);

	public override string HowSeen(IPerceiver voyeur)
	{
		return Gameworld.ItemProtos.Get(TargetItemId)?.ShortDescription ?? "an unspecified item".Colour(Telnet.Red);
	}
}