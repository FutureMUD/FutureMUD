using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Prototypes;
public class MusketBallGameItemComponentProto : AmmunitionGameItemComponentProto
{
	public override string TypeDescription => "MusketBall";

	#region Constructors
	protected MusketBallGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(gameworld, originator, "MusketBall")
	{
		AmmoType = gameworld.AmmunitionTypes.FirstOrDefault(x => x.RangedWeaponTypes.Contains(RangedWeaponType.Musket));
		BulletBore = 0.8;
	}

	protected MusketBallGameItemComponentProto(Models.GameItemComponentProto proto, IFuturemud gameworld) : base(proto, gameworld)
	{
	}

	protected override void LoadFromXml(XElement root)
	{
		BulletBore = double.Parse(root.Element("BulletBore").Value);
		base.LoadFromXml(root);
	}
	#endregion

	#region Saving
	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("AmmoType", AmmoType?.Id ?? 0),
			new XElement("BulletProto", BulletProto?.Id ?? 0),
			new XElement("BulletBore", BulletBore)
			).ToString();
	}
	#endregion

	public override bool CanSubmit()
	{
		return AmmoType != null && base.CanSubmit();
	}

	public override string WhyCannotSubmit()
	{
		if (AmmoType == null)
		{
			return "You must first give this item an ammo type.";
		}

		return base.WhyCannotSubmit();
	}

	#region Component Instance Initialising Functions
	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new MusketBallGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(Models.GameItemComponent component, IGameItem parent)
	{
		return new MusketBallGameItemComponent(component, this, parent);
	}
	#endregion

	#region Initialisation Tasks
	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("MusketBall".ToLowerInvariant(), true, (gameworld, account) => new MusketBallGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("MusketBall", (proto, gameworld) => new MusketBallGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"MusketBall",
			$"A musket ball that is used with gunpowder to fire a musket",
			BuildingHelpText
		);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator, (proto, gameworld) => new MusketBallGameItemComponentProto(proto, gameworld));
	}
	#endregion

	#region Building Commands

	private const string BuildingHelpText = $@"You can use the following options with this component:

	#3<name>#0 - sets the name of the component
	#3desc <desc>#0 - sets the description of the component
	#3ammo <ammo>#0 - sets the ammo grade, for example, #20.8in Bore Musket#0.
	#3bullet <proto>#0 - sets the bullet proto for this round, which will be loaded when fired.
	#3bore <inches>#0 - sets the bore of the bullet, historically 0.3-0.8in";
	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "type":
			case "ammo":
			case "ammunition":
				return BuildingCommandAmmunition(actor, command);
			case "bore":
			case "gauge":
				return BuildingCommandBore(actor, command);
			default:
				return base.BuildingCommand(actor, command.GetUndo());
		}
	}

	private bool BuildingCommandAmmunition(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What ammunition type do you want this component to be for?");
			return false;
		}

		var ammo = long.TryParse(command.PopSpeech(), out var value)
			? Gameworld.AmmunitionTypes.Get(value)
			: Gameworld.AmmunitionTypes.GetByName(command.Last);
		if (ammo == null)
		{
			actor.Send("There is no such ammunition type.");
			return false;
		}

		if (!ammo.RangedWeaponTypes.Contains(RangedWeaponType.Musket))
		{
			actor.OutputHandler.Send("You can only select ammunition types that are made for muskets.");
			return false;
		}

		AmmoType = ammo;
		Changed = true;
		actor.Send($"This ammunition is now of type {AmmoType.Name.Colour(Telnet.Cyan)}.");
		return true;
	}

	private bool BuildingCommandBore(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What should be the bore (or gauge) of the ball?");
			return false;
		}

		if (!double.TryParse(command.SafeRemainingArgument, out var value) || value <= 0.0)
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid bore.");
			return false;
		}

		BulletBore = value;
		Changed = true;
		actor.OutputHandler.Send($"This ball now has a bore of {value.ToStringN2Colour(actor)}.");
		return true;
	}
	#endregion

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor, "{0} (#{1:N0}r{2:N0}, {3})\r\n\r\nThis is a musket ball of type {4} with a bore of {5}in.{6}",
			"MusketBall Game Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name,
			AmmoType != null ? $"{AmmoType.Name} (#{AmmoType.Id:N0})" : "None".Colour(Telnet.Red),
			BulletBore.ToStringN2Colour(actor),
			AmmoType?.RangedWeaponTypes.Contains(RangedWeaponType.ModernFirearm) ?? false
				? $"\nWhen fired, it loads prototype  {(BulletProto != null ? BulletProtoId.ToString(actor).Colour(Telnet.Green) : "None".Colour(Telnet.Red))} as a bullet."
				: ""
			);
	}

	public double BulletBore { get; set; }
}
