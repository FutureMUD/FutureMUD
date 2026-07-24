using MudSharp.Accounts;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;

#nullable enable

namespace MudSharp.GameItems.Prototypes;

public class BayonetAttachmentGameItemComponentProto : GameItemComponentProto, IBayonetAttachmentPrototype
{
	protected BayonetAttachmentGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "BayonetAttachment")
	{
		Style = BayonetAttachmentStyle.Socket;
		MinimumBore = 0.45;
		MaximumBore = 0.8;
	}

	protected BayonetAttachmentGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto,
		IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	public override string TypeDescription => "BayonetAttachment";
	public BayonetAttachmentStyle Style { get; private set; }
	public double MinimumBore { get; private set; }
	public double MaximumBore { get; private set; }
	public bool BlocksFiring => Style == BayonetAttachmentStyle.Plug;

	protected override void LoadFromXml(XElement root)
	{
		Style = root.Element("Style")?.Value.TryParseEnum<BayonetAttachmentStyle>(out var style) == true
			? style
			: BayonetAttachmentStyle.Socket;
		MinimumBore = double.TryParse(root.Element("MinimumBore")?.Value, out var minimumBore)
			? minimumBore
			: 0.45;
		MaximumBore = double.TryParse(root.Element("MaximumBore")?.Value, out var maximumBore)
			? maximumBore
			: 0.8;
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("Style", Style),
			new XElement("MinimumBore", MinimumBore),
			new XElement("MaximumBore", MaximumBore)).ToString();
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter? loader = null, bool temporary = false)
	{
		return new BayonetAttachmentGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new BayonetAttachmentGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new BayonetAttachmentGameItemComponentProto(proto, gameworld));
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("bayonetattachment", true,
			(gameworld, account) => new BayonetAttachmentGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("bayonet attachment", false,
			(gameworld, account) => new BayonetAttachmentGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("BayonetAttachment",
			(proto, gameworld) => new BayonetAttachmentGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"BayonetAttachment",
			"Makes an item a musket bayonet attachment",
			BuildingHelpText);
	}

	private const string BuildingHelpText =
		@"You can use the following options with this component:

	#3name <name>#0 - sets the name of the component
	#3desc <desc>#0 - sets the description of the component
	#3style <plug|socket|sword>#0 - sets the attachment style; plug bayonets block firing
	#3bore <minimum> <maximum>#0 - sets the compatible musket-bore range in inches";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "style":
			case "type":
				return BuildingCommandStyle(actor, command);
			case "bore":
			case "bores":
			case "range":
				return BuildingCommandBore(actor, command);
			default:
				return base.BuildingCommand(actor, command.GetUndo());
		}
	}

	private bool BuildingCommandStyle(ICharacter actor, StringStack command)
	{
		if (command.IsFinished ||
		    !command.SafeRemainingArgument.TryParseEnum<BayonetAttachmentStyle>(out var style))
		{
			actor.OutputHandler.Send("You must specify plug, socket or sword.");
			return false;
		}

		Style = style;
		Changed = true;
		actor.OutputHandler.Send(
			$"This is now a {Style.DescribeEnum().ColourName()} bayonet and {(BlocksFiring ? "blocks" : "permits")} firing while mounted.");
		return true;
	}

	private bool BuildingCommandBore(ICharacter actor, StringStack command)
	{
		var minimumText = command.PopSpeech();
		var maximumText = command.PopSpeech();
		if (!double.TryParse(minimumText, out var minimum) ||
		    !double.TryParse(maximumText, out var maximum) ||
		    minimum <= 0.0 ||
		    maximum < minimum)
		{
			actor.OutputHandler.Send(
				"You must specify positive minimum and maximum bore sizes, with the maximum no smaller than the minimum.");
			return false;
		}

		MinimumBore = minimum;
		MaximumBore = maximum;
		Changed = true;
		actor.OutputHandler.Send(
			$"This bayonet now fits muskets from {MinimumBore.ToString("N2", actor).ColourValue()} to {MaximumBore.ToString("N2", actor).ColourValue()} inches bore.");
		return true;
	}

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return
			$"{"Bayonet Attachment Item Component".ColourName()} (#{Id.ToString("N0", actor)}r{RevisionNumber.ToString("N0", actor)}, {Name})\n\nStyle: {Style.DescribeEnum().ColourName()}\nCompatible Bore: {MinimumBore.ToString("N2", actor).ColourValue()}-{MaximumBore.ToString("N2", actor).ColourValue()} inches\nBlocks Firing: {BlocksFiring.ToColouredString()}";
	}
}
