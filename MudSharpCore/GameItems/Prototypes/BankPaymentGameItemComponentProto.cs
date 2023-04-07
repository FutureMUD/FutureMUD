using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MudSharp.GameItems.Prototypes;

public class BankPaymentGameItemComponentProto : GameItemComponentProto
{
	public override string TypeDescription => "BankPayment";

	public int MaximumUses { get; protected set; }

	#region Constructors

	protected BankPaymentGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(gameworld, originator,
		"BankPayment")
	{
	}

	protected BankPaymentGameItemComponentProto(Models.GameItemComponentProto proto, IFuturemud gameworld) : base(proto,
		gameworld)
	{
	}

	protected override void LoadFromXml(XElement root)
	{
		MaximumUses = int.Parse(root.Element("Uses").Value);
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("Uses", MaximumUses)
		).ToString();
	}

	#endregion

	#region Component Instance Initialising Functions

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new BankPaymentGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(Models.GameItemComponent component, IGameItem parent)
	{
		return new BankPaymentGameItemComponent(component, this, parent);
	}

	#endregion

	#region Initialisation Tasks

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("BankPayment".ToLowerInvariant(), true,
			(gameworld, account) => new BankPaymentGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("BankPayment",
			(proto, gameworld) => new BankPaymentGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"BankPayment",
			$"Lets the item be used for {"[Payment]".Colour(Telnet.BoldYellow)} from a bank",
			BuildingHelpText
		);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new BankPaymentGameItemComponentProto(proto, gameworld));
	}

	#endregion

	#region Building Commands

	private const string BuildingHelpText = @"You can use the following options with this component:

	<name> - sets the name of the component
	desc <desc> - sets the description of the component
	uses <#> - sets the number of uses (0 is unlimited)";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant().CollapseString())
		{
			default:
				return base.BuildingCommand(actor, command);
			case "uses":
				return BuildingCommandUses(actor, command);
		}
	}

	private bool BuildingCommandUses(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !int.TryParse(command.SafeRemainingArgument, out var value) || value < 0)
		{
			actor.OutputHandler.Send(
				"How many uses should this item have before it is used up? Use 0 for unlimited (e.g. swipe card)");
			return false;
		}

		MaximumUses = value;
		Changed = true;
		if (value == 0)
		{
			actor.OutputHandler.Send("This item can now be used an unlimited number of times.");
		}
		else if (value == 1)
		{
			actor.OutputHandler.Send("This item can now only be used a single time.");
		}
		else
		{
			actor.OutputHandler.Send(
				$"This item can now be used {value.ToString("N0", actor).ColourValue()} times.");
		}

		return true;
	}

	#endregion

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor,
			"{0} (#{1:N0}r{2:N0}, {3})\r\n\r\nThis item can be used to make payments with a bank account. It can be used {4}.",
			"BankPayment Game Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name,
			(MaximumUses switch
			{
				0 => "unlimited times", 1 => $"{1.ToString("N0", actor)} time",
				_ => $"{MaximumUses.ToString("N0", actor)} times"
			}).ColourValue()
		);
	}
}