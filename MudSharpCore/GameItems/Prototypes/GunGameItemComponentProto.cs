using System;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.PerceptionEngine;

namespace MudSharp.GameItems.Prototypes;

public class GunGameItemComponentProto : FirearmBaseGameItemComponentProto
{
	public override string TypeDescription => "Gun";

	#region Constructors

	protected GunGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(gameworld, originator, "Gun")
	{
		LoadEmote = "@ insert|inserts $2 into $1 and it clicks into place.";
		ReadyEmote = "@ rack|racks the slide on $1, and it clicks back into place.";
		UnloadEmote = "@ hit|hits the eject button on $1 and $2 is ejected.";
		UnreadyEmote = "@ open|opens the slide on $1 and work|works out $2 from the chamber.";
		UnreadyEmoteNoChamberedRound = "@ open|opens the slide on $1, but there is no round in the chamber.";
		FireEmote = "@ squeeze|squeezes the trigger on $1 and it fires a round with an extremely loud bang.";
		FireEmoteNoChamberedRound = "@ squeeze|squeezes the trigger on $1, and nothing happens except a quiet click.";
		ClipType = Gameworld.GetStaticConfiguration("DefaultGunClipType");
		MeleeWeaponType = gameworld.WeaponTypes.Get(gameworld.GetStaticLong("DefaultGunMeleeWeaponType"));
	}

	protected GunGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld) : base(
		proto, gameworld)
	{
	}

	protected override void LoadFromXml(XElement root)
	{
		base.LoadFromXml(root);
		// TODO - anything else
		RecalculateInventoryPlans();
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("LoadEmote", new XCData(LoadEmote)),
			new XElement("ReadyEmote", new XCData(ReadyEmote)),
			new XElement("UnloadEmote", new XCData(UnloadEmote)),
			new XElement("UnreadyEmote", new XCData(UnreadyEmote)),
			new XElement("UnreadyEmoteNoChamberedRound", new XCData(UnreadyEmoteNoChamberedRound)),
			new XElement("FireEmote", new XCData(FireEmote)),
			new XElement("FireEmoteNoChamberedRound", new XCData(FireEmoteNoChamberedRound)),
			new XElement("RangedWeaponType", RangedWeaponType?.Id ?? 0),
			new XElement("ClipType", new XCData(ClipType)),
			new XElement("MeleeWeaponType", MeleeWeaponType?.Id ?? 0),
			new XElement("CanWieldProg", CanWieldProg?.Id ?? 0),
			new XElement("WhyCannotWieldProg", WhyCannotWieldProg?.Id ?? 0)
		).ToString();
	}

	#endregion

	#region Component Instance Initialising Functions

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new GunGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new GunGameItemComponent(component, this, parent);
	}

	#endregion

	#region Initialisation Tasks

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("Gun".ToLowerInvariant(), true,
			(gameworld, account) => new GunGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Gun", (proto, gameworld) => new GunGameItemComponentProto(proto, gameworld));

		manager.AddTypeHelpInfo(
			"Gun",
			$"Makes an item a {"[ranged weapon]".Colour(Telnet.BoldCyan)} with removable-magazine semi-automatic gun mechanics",
			$@"{BuildingHelpText}
	#3clip <type>#0 - sets the type of clip that fits in this gun"
		);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator, (proto, gameworld) => new GunGameItemComponentProto(proto, gameworld));
	}

	#endregion

	public override string ShowBuildingHelp =>
		$@"{BuildingHelpText}
	#3clip <type>#0 - sets the type of clip that fits in this gun";

	#region Building Commands

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "clip":
				return BuildingCommandClip(actor, command);
			default:
				return base.BuildingCommand(actor, command.GetUndo());
		}
	}

    private bool BuildingCommandClip(ICharacter actor, StringStack command)
	{
        var types = Gameworld.ItemProtos.SelectNotNull(x => x.GetItemType<AmmoClipGameItemComponentProto>())
                             .Where(x => x.Status == RevisionStatus.Current).Select(x => x.ClipType).Distinct()
                             .OrderBy(x => x).ToList();
        if (command.IsFinished)
        {
            actor.Send("What clip type (form factor) should this ammo holder have?");
            if (types.Any())
            {
                actor.OutputHandler.Send(
                    $"Hint: the following form factors exist currently:\n{types.Select(x => x.ColourValue()).ListToString("\t", "\n", twoItemJoiner: "\n", conjunction: "")}");
            }

            return false;
        }

        var type = command.SafeRemainingArgument;
        ClipType = types.FirstOrDefault(x => x.EqualTo(type)) ?? type;
        Changed = true;
        actor.Send($"This gun will now take a clip type (form factor) of {ClipType.ColourValue()}.");
        if (types.All(x => !x.EqualTo(ClipType)))
        {
            actor.OutputHandler.Send("Warning: There have not been any clips of this type before. Check to see if the name is a typo.".ColourError());
        }

        return true;
    }

    #endregion

    #region Overrides of FirearmBaseGameItemComponentProto

    /// <inheritdoc />
    protected override void RecalculateInventoryPlans()
	{
		LoadTemplate = new InventoryPlanTemplate(Gameworld, new[]
		{
			new InventoryPlanPhaseTemplate(1, new[]
			{
				InventoryPlanAction.LoadAction(Gameworld, DesiredItemState.Held, 0, 0, item =>
				{
					var ammo = item.GetItemType<IAmmoClip>();
					if (ammo == null)
					{
						return false;
					}

					if (ammo.ClipType != ClipType ||
					    ammo.SpecificAmmoGrade != RangedWeaponType.SpecificAmmunitionGrade)
					{
						return false;
					}

					return true;
				}, null, 1, originalReference: "loaditem"),
				InventoryPlanAction.LoadAction(Gameworld, DesiredItemState.Held, 0, 0,
					item => item.GetItemType<IRangedWeapon>()?.Prototype == this, null)
			})
		});

		LoadTemplateIgnoreEmpty = new InventoryPlanTemplate(Gameworld, new[]
		{
			new InventoryPlanPhaseTemplate(1, new[]
			{
				InventoryPlanAction.LoadAction(Gameworld, DesiredItemState.Held, 0, 0, item =>
				{
					var ammo = item.GetItemType<IAmmoClip>();
					if (ammo == null)
					{
						return false;
					}

					if (ammo.ClipType != ClipType ||
					    ammo.SpecificAmmoGrade != RangedWeaponType.SpecificAmmunitionGrade)
					{
						return false;
					}

					if (!ammo.Contents.Any(x =>
						    x.GetItemType<IAmmo>()?.AmmoType.SpecificType
						     .EqualTo(RangedWeaponType.SpecificAmmunitionGrade) ?? false))
					{
						return false;
					}

					return true;
				}, null, 1, originalReference: "loaditem"),
				InventoryPlanAction.LoadAction(Gameworld, DesiredItemState.Held, 0, 0,
					item => item.GetItemType<IRangedWeapon>()?.Prototype == this, null)
			})
		});
	}

	#endregion

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(
			actor,
			@"{0} (#{1:N0}r{2:N0}, {3})

This is a modern firearm of type {4} and melee type {13}.
The CanWield prog is {14} and the WhyCannotWield prog is {15}.

Fire: {5}
FireEmpty: {6}
Load: {7}
Unload: {8}
Ready: {9}
Unready: {10}
UnreadyEmpty: {11}
Clip Type: {12}",
			"Gun Game Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name,
			RangedWeaponType?.Name.Colour(Telnet.Green) ?? "None".Colour(Telnet.Red),
			FireEmote.Colour(Telnet.Cyan),
			FireEmoteNoChamberedRound.Colour(Telnet.Cyan),
			LoadEmote.Colour(Telnet.Cyan),
			UnloadEmote.Colour(Telnet.Cyan),
			ReadyEmote.Colour(Telnet.Cyan),
			UnreadyEmote.Colour(Telnet.Cyan),
			UnreadyEmoteNoChamberedRound.Colour(Telnet.Cyan),
			ClipType.Colour(Telnet.Green),
			MeleeWeaponType?.Name.TitleCase().Colour(Telnet.Green) ?? "None".Colour(Telnet.Red),
			CanWieldProg?.MXPClickableFunctionName() ?? "None".ColourError(),
			WhyCannotWieldProg?.MXPClickableFunctionName() ?? "None".ColourError()
		);
	}
}