using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.VisualBasic;
using MudSharp.Accounts;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Climate;
using MudSharp.Combat;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Units;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.GameItems.Inventory;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using System.Management;
using MudSharp.Form.Material;

namespace MudSharp.GameItems.Prototypes;
public class MusketGameItemComponentProto : GameItemComponentProto
{
	public static ISolid GunpowderMaterial => Futuremud.Games.First().Materials.Get(Futuremud.Games.First().GetStaticLong("GunpowderMaterialId"));

	public static ITag WadItemTag => Futuremud.Games.First().Tags.Get(Futuremud.Games.First().GetStaticLong("WadItemTagId"));
	public static ITag RamrodTag => Futuremud.Games.First().Tags.Get(Futuremud.Games.First().GetStaticLong("MusketRamrodTag"));
	public static ITag MusketUnjammingToolTag => Futuremud.Games.First().Tags.Get(Futuremud.Games.First().GetStaticLong("MusketUnjammingToolTag"));

	public override string TypeDescription => "Musket";

	#region Constructors
	protected MusketGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(gameworld, originator, "Musket")
	{
		MeleeWeaponType = gameworld.WeaponTypes.Get(gameworld.GetStaticLong("DefaultMusketMeleeWeaponType"));
		RangedWeaponType = gameworld.RangedWeaponTypes.FirstOrDefault(x => x.RangedWeaponType == Combat.RangedWeaponType.Musket);
		
		LoadEmoteBall = "$0 push|pushes $2$?3| wrapped in $3||$ into $1.";
		LoadEmoteCartridge = "$0 bite|bites open $2 and pour|pours a small amount of the charge into the priming pan of $1, close|closes the frizzen, rest|rests the whole gun butt-first on the ground and shove|shoves the cartridge down into the barrel.";
		LoadEmotePowder = "$0 pour|pours a small amount of charge into the priming pan of $1, close|closes the frizzen, rest|rests the whole gun butt-first on the ground and pour|pours $2 down the barrel.";
		LoadEmoteClean = "$0 push|pushes the worm-end of $2 into the barrel of $1 and remove|removes any remaining debris from previous shots.";
		LoadEmoteRamrod = "$0 push|pushes the ram-end of the $1 into the barrel of $1, tamping down on the loaded shot.";
		LoadEmoteTap = "$0 hit|hits the butt-end of $1 on the ground, using gravity to force the shot into place.";
		ReadyEmote = "$0 pull|pulls the cock into the fire position on $1.";
		UnloadEmote = "$0 empty|empties and discard|discards the contents of the barrel of $1.";
		UnreadyEmote = "$0 gently lower|lowers the cock of $1 out of the firing position.";
		StartUnjamEmote = "$0 push|pushes the worm-end of $2 into the barrel of $1, laboriously attempting to remove the jam.";
		FinishUnjamEmote = "$0 successfully dislodge|dislodges the blockage in the barrel of $1, and discard|discards the debris.";
		FailUnjamEmote = "$0 fail|fails to dislodge the debris in the barrel of $1, but continue|continues with &his efforts.";
		FireEmote = "@ squeeze|squeezes the trigger on $2 and it lets off a thundering blast towards $1.";
		FireEmoteJam = "@ squeeze|squeezes the trigger on $2 and it lets off a thundering blast towards $1, with a larger than usual kick.";
		FireEmoteMisfire = "@ squeeze|squeezes the trigger on $2 and it misfires, discharging its ammunition harmlessly and ineffectively with a thunderous blast of smoke.";
		FireEmoteCatastrophy = "@ squeeze|squeezes the trigger on $2 and it catastrophically explodes in &0's face!";
		
		PowderVolumePerShot = 6.5;
		BarrelBore = 0.8;
		MisfireChance = new TraitExpression("Max(0, sqrt(max(0, 70-operate))*0.075 + taploaded*0.1 + skipclean*0.1 + if(precipitation<3,precipitation,pow(precipitation,1.5))+(5-gunquality)*0.03+cartridgeused*(5-cartridgequality)*0.03+(1-sqrt(condition))*0.5-wadused*0.2+wetpowder*0.5)", gameworld);
		JamChance = new TraitExpression("0.1+cartridgeused*0.1+(1-condition)*0.5+taploaded*0.3", gameworld);
		CatastrophyDamageFormula = new ExpressionEngine.Expression("10+1d40");
	}

	protected MusketGameItemComponentProto(Models.GameItemComponentProto proto, IFuturemud gameworld) : base(proto, gameworld)
	{
	}

	protected override void LoadFromXml(XElement root)
	{
		LoadEmoteBall = root.Element("LoadEmoteBall").Value;
		LoadEmoteCartridge = root.Element("LoadEmoteCartridge").Value;
		LoadEmotePowder = root.Element("LoadEmotePowder").Value;
		LoadEmoteClean = root.Element("LoadEmoteClean").Value;
		LoadEmoteRamrod = root.Element("LoadEmoteRamrod").Value;
		LoadEmoteTap = root.Element("LoadEmoteTap").Value;

		ReadyEmote = root.Element("ReadyEmote").Value;
		UnloadEmote = root.Element("UnloadEmote").Value;
		UnreadyEmote = root.Element("UnreadyEmote").Value;
		StartUnjamEmote = root.Element("StartUnjamEmote")?.Value ?? "$0 push|pushes the worm-end of $2 into the barrel of $1, laboriously attempting to remove the jam.";
		FinishUnjamEmote = root.Element("FinishUnjamEmote")?.Value ?? "$0 successfully dislodge|dislodges the blockage in the barrel of $1, and discard|discards the debris.";
		FailUnjamEmote = root.Element("FailUnjamEmote")?.Value ?? "$0 fail|fails to dislodge the debris in the barrel of $1, but continue|continues with &his efforts.";

		FireEmote = root.Element("FireEmote").Value;
		FireEmoteJam = root.Element("FireEmoteJam").Value;
		FireEmoteMisfire = root.Element("FireEmoteMisfire").Value;
		FireEmoteCatastrophy = root.Element("FireEmoteCatastrophy").Value;

		_powderVolumePerShot = double.Parse(root.Element("PowderVolumePerShot").Value);
		_barrelBore = double.Parse(root.Element("BarrelBore").Value);

		MisfireChance = new TraitExpression(root.Element("MisfireChance").Value, Gameworld);
		JamChance = new TraitExpression(root.Element("JamChance").Value, Gameworld);
		CatastrophyDamageFormula = new ExpressionEngine.Expression(root.Element("CatastrophyDamageFormula").Value);


		_rangedWeaponType = Gameworld.RangedWeaponTypes.Get(long.Parse(root.Element("RangedWeaponType").Value));
		var element = root.Element("MeleeWeaponType");
		if (element != null)
		{
			MeleeWeaponType = Gameworld.WeaponTypes.Get(long.Parse(element.Value));
		}
		else
		{
			MeleeWeaponType = Gameworld.WeaponTypes.Get(Gameworld.GetStaticLong("DefaultMusketMeleeWeaponType"));
		}

		CalculateLoadTemplates();
	}
	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("LoadEmoteClean", new XCData(LoadEmoteClean)),
			new XElement("LoadEmoteCartridge", new XCData(LoadEmoteCartridge)),
			new XElement("LoadEmotePowder", new XCData(LoadEmotePowder)),
			new XElement("LoadEmoteBall", new XCData(LoadEmoteBall)),
			new XElement("LoadEmoteRamrod", new XCData(LoadEmoteRamrod)),
			new XElement("LoadEmoteTap", new XCData(LoadEmoteTap)),
			new XElement("LoadEmoteClean", new XCData(LoadEmoteClean)),
			new XElement("ReadyEmote", new XCData(ReadyEmote)),
			new XElement("UnloadEmote", new XCData(UnloadEmote)),
			new XElement("UnreadyEmote", new XCData(UnreadyEmote)),
			new XElement("StartUnjamEmote", new XCData(StartUnjamEmote)),
			new XElement("FailUnjamEmote", new XCData(FailUnjamEmote)),
			new XElement("FinishUnjamEmote", new XCData(FinishUnjamEmote)),
			new XElement("FireEmote", new XCData(FireEmote)),
			new XElement("FireEmoteJam", new XCData(FireEmoteJam)),
			new XElement("FireEmoteMisfire", new XCData(FireEmoteMisfire)),
			new XElement("FireEmoteCatastrophy", new XCData(FireEmoteCatastrophy)),
			new XElement("RangedWeaponType", RangedWeaponType?.Id ?? 0),
			new XElement("MeleeWeaponType", MeleeWeaponType?.Id ?? 0),
			new XElement("PowderVolumePerShot", PowderVolumePerShot),
			new XElement("BarrelBore", BarrelBore),
			new XElement("MisfireChance", MisfireChance.OriginalFormulaText),
			new XElement("JamChance", JamChance.OriginalFormulaText),
			new XElement("CatastrophyDamageFormula", CatastrophyDamageFormula.OriginalExpression)
		).ToString();
	}

	#endregion

	#region Component Instance Initialising Functions
	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new MusketGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(Models.GameItemComponent component, IGameItem parent)
	{
		return new MusketGameItemComponent(component, this, parent);
	}
	#endregion

	#region Initialisation Tasks
	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("Musket".ToLowerInvariant(), true, (gameworld, account) => new MusketGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Musket", (proto, gameworld) => new MusketGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"Musket",
			$"A musket is a muzzle-loaded firearm which uses either a wheellock or a flintlock to fire",
			BuildingHelpText
		);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator, (proto, gameworld) => new MusketGameItemComponentProto(proto, gameworld));
	}
	#endregion

	#region Core Properties

	private void CalculateLoadTemplates()
	{
		LoadTemplateClean = new InventoryPlanTemplate(Gameworld, new[]
			{
				new InventoryPlanPhaseTemplate(1, new[]
				{
					InventoryPlanAction.LoadAction(Gameworld, DesiredItemState.Held, Gameworld.GetStaticLong("MusketCleaningRamrodTag"), 0, null, null, 1, originalReference: "ramrod"),
					InventoryPlanAction.LoadAction(Gameworld, DesiredItemState.Held, 0, 0,
						item => item.GetItemType<IRangedWeapon>()?.Prototype == this, null, originalReference: "musket")
				})
			});
		LoadTemplateLoadPowder = new InventoryPlanTemplate(Gameworld, new[]
		{
				new InventoryPlanPhaseTemplate(1, new[]
				{
					new InventoryPlanActionConsumeCommodity(Gameworld, PowderVolumePerShot, GunpowderMaterial, 0, null, null)
					{
						OriginalReference = "gunpowder"
					},
					InventoryPlanAction.LoadAction(Gameworld, DesiredItemState.Held, 0, 0,
						item => item.GetItemType<IRangedWeapon>()?.Prototype == this, null, originalReference: "musket")
				})
			});
		LoadTemplateLoadBall = new InventoryPlanTemplate(Gameworld, new[]
		{
				new InventoryPlanPhaseTemplate(1, new[]
				{
					InventoryPlanAction.LoadAction(Gameworld, DesiredItemState.Held, 0, 0, item =>
					{
						var ball = item.GetItemType<MusketBallGameItemComponent>();
						if (ball is null)
						{
							return false;
						}

						if (ball.BulletBore > BarrelBore)
						{
							return false;
						}

						return true;
					}, null, 1, originalReference: "ball", fitnessscorer: item =>
					{
						var ball = item.GetItemType<MusketBallGameItemComponent>();
						if (ball is null)
						{
							return 0.0;
						}

						return int.MaxValue - BarrelBore + ball.BulletBore;
					}),
					InventoryPlanAction.LoadAction(Gameworld, DesiredItemState.Held, 0, 0,
						item => item.GetItemType<IRangedWeapon>()?.Prototype == this, null, originalReference: "musket")
				})
			});
		LoadTemplateLoadCartridge = new InventoryPlanTemplate(Gameworld, new[]
		{
				new InventoryPlanPhaseTemplate(1, new[]
				{
					InventoryPlanAction.LoadAction(Gameworld, DesiredItemState.Held, 0, 0, item =>
					{
						var cartridge = item.GetItemType<MusketCartridgeGameItemComponent>();
						if (cartridge is null)
						{
							return false;
						}

						if (cartridge.BulletBore > BarrelBore)
						{
							return false;
						}

						return true;
					}, null, 1, originalReference: "cartridge", fitnessscorer: item =>
					{
						var cartridge = item.GetItemType<MusketCartridgeGameItemComponent>();
						if (cartridge is null)
						{
							return 0.0;
						}

						return int.MaxValue - BarrelBore + cartridge.BulletBore;
					}),
					InventoryPlanAction.LoadAction(Gameworld, DesiredItemState.Held, 0, 0,
						item => item.GetItemType<IRangedWeapon>()?.Prototype == this, null, originalReference: "musket")
				})
			});
		LoadTemplateLoadRamrod = new InventoryPlanTemplate(Gameworld, new[]
		{
				new InventoryPlanPhaseTemplate(1, new[]
				{
					InventoryPlanAction.LoadAction(Gameworld, DesiredItemState.Held, Gameworld.GetStaticLong("MusketRamrodTag"), 0, null, null, 1, originalReference: "ramrod"),
					InventoryPlanAction.LoadAction(Gameworld, DesiredItemState.Held, 0, 0,
						item => item.GetItemType<IRangedWeapon>()?.Prototype == this, null, originalReference: "musket")
				})
			});
		LoadTemplateFinishLoading = new InventoryPlanTemplate(Gameworld, new[]
		{
			new InventoryPlanPhaseTemplate(1, new[]
			{
				InventoryPlanAction.LoadAction(Gameworld, DesiredItemState.Wielded, 0, 0,
					item => item.GetItemType<IRangedWeapon>()?.Prototype == this, null, originalReference: "musket")
			})
		});
		UnjamTemplate = new InventoryPlanTemplate(Gameworld, new[]
		{
			new InventoryPlanPhaseTemplate(1, new[]
			{
				InventoryPlanAction.LoadAction(Gameworld, DesiredItemState.Held, Gameworld.GetStaticLong("MusketUnjammingToolTag"), 0, null, null, 1, originalReference: "ramrod"),
				InventoryPlanAction.LoadAction(Gameworld, DesiredItemState.Held, 0, 0,
					item => item.GetItemType<IRangedWeapon>()?.Prototype == this, null, originalReference: "musket")
			})
		});
	}

	private IRangedWeaponType _rangedWeaponType;

	public IRangedWeaponType RangedWeaponType
	{
		get => _rangedWeaponType;
		set
		{
			_rangedWeaponType = value;
			CalculateLoadTemplates();
		}
	}

	private IWeaponType _meleeWeaponType;

	public IWeaponType MeleeWeaponType
	{
		get => _meleeWeaponType;
		set
		{
			_meleeWeaponType = value;
		}
	}

	public IInventoryPlanTemplate LoadTemplateClean { get; set; }
	public IInventoryPlanTemplate LoadTemplateLoadPowder { get; set; }
	public IInventoryPlanTemplate LoadTemplateLoadBall { get; set; }
	public IInventoryPlanTemplate LoadTemplateLoadCartridge { get; set; }
	public IInventoryPlanTemplate LoadTemplateLoadRamrod { get; set; }
	public IInventoryPlanTemplate LoadTemplateFinishLoading { get; set; }
	public IInventoryPlanTemplate UnjamTemplate { get; set; }

	public string LoadEmoteClean { get; set; }
	public string LoadEmoteCartridge { get; set; }
	public string LoadEmotePowder { get; set; }
	public string LoadEmoteBall { get; set; }
	public string LoadEmoteRamrod { get; set; }
	public string LoadEmoteTap { get; set; }

	public string ReadyEmote { get; set; }

	public string UnloadEmote { get; set; }

	public string UnreadyEmote { get; set; }
	public string StartUnjamEmote { get; set; }
	public string FinishUnjamEmote { get; set; }
	public string FailUnjamEmote { get; set; }
	public string FireEmote { get; set; }
	public string FireEmoteJam { get; set; }
	public string FireEmoteMisfire { get; set; }
	public string FireEmoteCatastrophy { get; set; }

	private double _powderVolumePerShot;

	public double PowderVolumePerShot
	{
		get => _powderVolumePerShot;
		set
		{
			_powderVolumePerShot = value;
			CalculateLoadTemplates();
		}
	}

	private double _barrelBore;

	public double BarrelBore
	{
		get => _barrelBore;
		set
		{
			_barrelBore = value;
			CalculateLoadTemplates();
		}
	}

	public ITraitExpression MisfireChance { get; set; }
	public ITraitExpression JamChance { get; set; }
	public ExpressionEngine.Expression CatastrophyDamageFormula { get; set; }

	#endregion

	public const string BuildingHelpText = @"You can use the following options:

	#3name <name>#0 - sets the name of the component
	#3desc <desc>#0 - sets the description of the component
	#3ranged <ranged type>#0 - sets the ranged weapon type for this component. See #3show ranges#0 for a list.
	#3ejectonfire#0 - toggles whether casings are ejected on fire or on ready
	#3load <emote>#0 - sets the emote for loading this weapon. $0 is the loader, $1 is the gun, $2 is the clip.
	#3unload <emote>#0 - sets the emote for unloading this weapon. $0 is the loader, $1 is the gun, $2 is the clip.
	#3ready <emote>#0 - sets the emote for readying this gun. $0 is the loader, $1 is the gun.
	#3unready <emote>#0 - sets the emote for unreadying this gun. $0 is the loader, $1 is the gun and $2 is the chambered round.
	#3unreadyempty <emote>#0 - sets the emote for unreadying this gun when there is no chambered round. $0 is the loader, $1 is the gun.
	#3fire <emote>#0 - sets the emote for firing the gun. $0 is the firer, $1 is the target, $2 is the gun.
	#3fireempty <emote>#0 - sets the emote for firing the gun when it is empty. $0 is the firer, $1 is the target, $2 is the gun.";

	public override string ShowBuildingHelp => BuildingHelpText;

	#region Building Commands

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "ranged":
			case "ranged type":
			case "rangedtype":
			case "type":
				return BuildingCommandType(actor, command);
			case "loadclean":
				return BuildingCommandLoadEmoteClean(actor, command);
			case "loadcartridge":
				return BuildingCommandLoadEmoteCartridge(actor, command);
			case "loadpowder":
				return BuildingCommandLoadEmotePowder(actor, command);
			case "loadball":
				return BuildingCommandLoadEmoteBall(actor, command);
			case "loadramrod":
				return BuildingCommandLoadEmoteRamrod(actor, command);
			case "loadtap":
				return BuildingCommandLoadEmoteTap(actor, command);
			case "unload":
				return BuildingCommandUnloadEmote(actor, command);
			case "ready":
				return BuildingCommandReadyEmote(actor, command);
			case "unready":
				return BuildingCommandUnreadyEmote(actor, command);
			case "unjamstart":
			case "unjambegin":
				return BuildingCommandStartUnjamEmote(actor, command);
			case "unjamfinish":
			case "unjamend":
			case "unjam":
				return BuildingCommandFinishUnjamEmote(actor, command);
			case "unjamfail":
				return BuildingCommandFailUnjamEmote(actor, command);
			case "fire":
				return BuildingCommandFireEmote(actor, command);
			case "firejam":
				return BuildingCommandFireEmoteJam(actor, command);
			case "firemisfire":
				return BuildingCommandFireEmoteMisfire(actor, command);
			case "firecatasrophy":
				return BuildingCommandFireEmoteCatastrophy(actor, command);
			case "melee":
			case "meleetype":
			case "melee type":
			case "melee_type":
				return BuildingCommand_Melee(actor, command);
			case "powder":
				return BuildingCommandPowder(actor, command);
			case "misfire":
				return BuildingCommandMisfire(actor, command);
			case "jam":
				return BuildingCommandJam(actor, command);
			case "catastrophy":
				return BuildingCommandCatastrophyDamage(actor, command);
			case "bore":
				return BuildingCommandBore(actor, command);
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	private bool BuildingCommandStartUnjamEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What emote do you want to set for starting to unjam the musket?");
			actor.OutputHandler.Send("Hint: $0 is the loader, $1 is the musket, $2 is the ramrod.".Colour(Telnet.Yellow));
			return false;
		}

		var emoteText = command.SafeRemainingArgument.Fullstop();
		var emote = new Emote(emoteText, new DummyPerceiver(), new DummyPerceivable(), new DummyPerceivable(), new DummyPerceivable());
		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return false;
		}

		StartUnjamEmote = emoteText;
		Changed = true;
		actor.OutputHandler.Send($"The following emote will now be used when starting to unjam this musket:\n\n{emoteText.ColourCommand()}\n");
		return true;
	}

	private bool BuildingCommandFinishUnjamEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What emote do you want to set for finishing unjamming the musket?");
			actor.OutputHandler.Send("Hint: $0 is the loader, $1 is the musket, $2 is the ramrod.".Colour(Telnet.Yellow));
			return false;
		}

		var emoteText = command.SafeRemainingArgument.Fullstop();
		var emote = new Emote(emoteText, new DummyPerceiver(), new DummyPerceivable(), new DummyPerceivable(), new DummyPerceivable());
		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return false;
		}

		FinishUnjamEmote = emoteText;
		Changed = true;
		actor.OutputHandler.Send($"The following emote will now be used when finishing unjamming this musket:\n\n{emoteText.ColourCommand()}\n");
		return true;
	}

	private bool BuildingCommandFailUnjamEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What emote do you want to set for failing to unjam the musket?");
			actor.OutputHandler.Send("Hint: $0 is the loader, $1 is the musket, $2 is the ramrod.".Colour(Telnet.Yellow));
			return false;
		}

		var emoteText = command.SafeRemainingArgument.Fullstop();
		var emote = new Emote(emoteText, new DummyPerceiver(), new DummyPerceivable(), new DummyPerceivable(), new DummyPerceivable());
		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return false;
		}

		FailUnjamEmote = emoteText;
		Changed = true;
		actor.OutputHandler.Send($"The following emote will now be used when failing to unjam this musket:\n\n{emoteText.ColourCommand()}\n");
		return true;
	}

	private bool BuildingCommandBore(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What should be the bore (or gauge) or the balls this musket takes?");
			return false;
		}

		if (!double.TryParse(command.SafeRemainingArgument, out var value) || value <= 0.0)
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid bore.");
			return false;
		}

		BarrelBore = value;
		Changed = true;
		actor.OutputHandler.Send($"This musket now has a barrel bore of {value.ToStringN2Colour(actor)} and requires musket balls that match.");
		return true;
	}

	private bool BuildingCommandMisfire(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What should be the formula for a misfire for this gun?");
			return false;
		}

		var te = new TraitExpression(command.SafeRemainingArgument, Gameworld);
		if (te.HasErrors())
		{
			actor.OutputHandler.Send(te.Error);
			return false;
		}

		MisfireChance = te;
		Changed = true;
		actor.OutputHandler.Send($"The misfire chance for this musket is now a percentage defined by the formula {command.SafeRemainingArgument.ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandJam(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What should be the formula for a jam for this gun?");
			return false;
		}

		var te = new TraitExpression(command.SafeRemainingArgument, Gameworld);
		if (te.HasErrors())
		{
			actor.OutputHandler.Send(te.Error);
			return false;
		}

		JamChance = te;
		Changed = true;
		actor.OutputHandler.Send($"The jam chance when misfiring for this musket is now a percentage defined by the formula {command.SafeRemainingArgument.ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandCatastrophyDamage(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What should be the damage done by this weapon when it explodes catastrophically?");
			return false;
		}

		var expr = new ExpressionEngine.Expression(command.SafeRemainingArgument);
		if (expr.HasErrors())
		{
			actor.OutputHandler.Send(expr.Error);
			return false;
		}

		CatastrophyDamageFormula = expr;
		Changed = true;
		actor.OutputHandler.Send($"The catastrophic explosion damage from this musket is now given by the formula {command.SafeRemainingArgument.ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandPowder(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("How much powder should be required per shot?");
			return false;
		}

		if (!Gameworld.UnitManager.TryGetBaseUnits(command.SafeRemainingArgument, UnitType.Mass, out var value) || value <= 0.0)
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid mass of gunpowder.");
			return false;
		}

		PowderVolumePerShot = value;
		Changed = true;
		actor.OutputHandler.Send($"This musket now requires {Gameworld.UnitManager.DescribeExact(value, UnitType.Mass, actor).ColourValue()} of gunpowder per shot.");
		return true;
	}

	private bool BuildingCommand_Melee(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("Which melee weapon type do you want to set for this component?");
			return false;
		}

		var type = long.TryParse(command.PopSpeech(), out var value)
			? actor.Gameworld.WeaponTypes.Get(value)
			: actor.Gameworld.WeaponTypes.GetByName(command.Last);
		if (type == null)
		{
			actor.Send("There is no such melee weapon type.");
			return false;
		}

		MeleeWeaponType = type;
		Changed = true;
		actor.Send(
			$"This component will now use the melee weapon type {MeleeWeaponType.Name.TitleCase().Colour(Telnet.Green)}.");
		return true;
	}

	private bool BuildingCommandLoadEmoteClean(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What emote do you want to set for the cleaning loading step?");
			actor.OutputHandler.Send("Hint: $0 is the loader, $1 is the musket, $2 is the ramrod.".Colour(Telnet.Yellow));
			return false;
		}

		var emoteText = command.SafeRemainingArgument.Fullstop();
		var emote = new Emote(emoteText, new DummyPerceiver(), new DummyPerceivable(), new DummyPerceivable(), new DummyPerceivable());
		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return false;
		}

		LoadEmoteClean = emoteText;
		Changed = true;
		actor.OutputHandler.Send($"The following emote will now be used when this musket is cleaned during the loading steps:\n\n{emoteText.ColourCommand()}\n");
		return true;
	}

	private bool BuildingCommandLoadEmotePowder(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What emote do you want to set for the powder loading step?");
			actor.OutputHandler.Send("Hint: $0 is the loader, $1 is the musket, $2 is the powder, $3 is the powder container.".Colour(Telnet.Yellow));
			return false;
		}

		var emoteText = command.SafeRemainingArgument.Fullstop();
		var emote = new Emote(emoteText, new DummyPerceiver(), new DummyPerceivable(), new DummyPerceivable(), new DummyPerceivable(), new DummyPerceivable());
		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return false;
		}

		LoadEmotePowder = emoteText;
		Changed = true;
		actor.OutputHandler.Send($"The following emote will now be used for the powder loading step of this musket:\n\n{emoteText.ColourCommand()}\n");
		return true;
	}

	private bool BuildingCommandLoadEmoteBall(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What emote do you want to set for the ball loading step?");
			actor.OutputHandler.Send("Hint: $0 is the loader, $1 is the musket, $2 is the ball, $3 is the wad (but can be null).".Colour(Telnet.Yellow));
			return false;
		}

		var emoteText = command.SafeRemainingArgument.Fullstop();
		var emote = new Emote(emoteText, new DummyPerceiver(), new DummyPerceivable(), new DummyPerceivable(), new DummyPerceivable(), null);
		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return false;
		}

		LoadEmoteBall = emoteText;
		Changed = true;
		actor.OutputHandler.Send($"The following emote will now be used for the ball loading step of this musket:\n\n{emoteText.ColourCommand()}\n");
		return true;
	}

	private bool BuildingCommandLoadEmoteCartridge(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What emote do you want to set for the cartridge loading step?");
			actor.OutputHandler.Send("Hint: $0 is the loader, $1 is the musket, $2 is the cartridge.".Colour(Telnet.Yellow));
			return false;
		}

		var emoteText = command.SafeRemainingArgument.Fullstop();
		var emote = new Emote(emoteText, new DummyPerceiver(), new DummyPerceivable(), new DummyPerceivable(), new DummyPerceivable());
		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return false;
		}

		LoadEmoteCartridge = emoteText;
		Changed = true;
		actor.OutputHandler.Send($"The following emote will now be used for the cartridge loading step of this musket:\n\n{emoteText.ColourCommand()}\n");
		return true;
	}

	private bool BuildingCommandLoadEmoteRamrod(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What emote do you want to set for the ramrod loading step?");
			actor.OutputHandler.Send("Hint: $0 is the loader, $1 is the musket, $2 is the ramrod.".Colour(Telnet.Yellow));
			return false;
		}

		var emoteText = command.SafeRemainingArgument.Fullstop();
		var emote = new Emote(emoteText, new DummyPerceiver(), new DummyPerceivable(), new DummyPerceivable(), new DummyPerceivable());
		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return false;
		}

		LoadEmoteRamrod = emoteText;
		Changed = true;
		actor.OutputHandler.Send($"The following emote will now be used for the ramrod loading step of this musket:\n\n{emoteText.ColourCommand()}\n");
		return true;
	}

	private bool BuildingCommandLoadEmoteTap(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What emote do you want to set for the tap loading step?");
			actor.OutputHandler.Send("Hint: $0 is the loader, $1 is the musket".Colour(Telnet.Yellow));
			return false;
		}

		var emoteText = command.SafeRemainingArgument.Fullstop();
		var emote = new Emote(emoteText, new DummyPerceiver(), new DummyPerceivable(), new DummyPerceivable());
		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return false;
		}

		LoadEmoteTap = emoteText;
		Changed = true;
		actor.OutputHandler.Send($"The following emote will now be used for the tap loading step of this musket:\n\n{emoteText.ColourCommand()}\n");
		return true;
	}

	private bool BuildingCommandReadyEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What emote do you want to set for when people ready this gun?");
			actor.Send("Hint: $0 is the loader, $1 is the gun.".Colour(Telnet.Yellow));
			return false;
		}

		ReadyEmote = command.RemainingArgument.Fullstop();
		Changed = true;
		actor.Send($"The following emote will now be used when this gun is readied:\n\n{ReadyEmote}\n");
		return true;
	}

	private bool BuildingCommandUnloadEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What emote do you want to set for when people unload a clip from this gun?");
			actor.Send("Hint: $0 is the loader, $1 is the gun, $2 is the clip.".Colour(Telnet.Yellow));
			return false;
		}

		UnloadEmote = command.RemainingArgument.Fullstop();
		Changed = true;
		actor.Send($"The following emote will now be used when this gun is unloaded:\n\n{UnloadEmote}\n");
		return true;
	}

	private bool BuildingCommandUnreadyEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What emote do you want to set for when people empty the chamber on this gun?");
			actor.Send("Hint: $0 is the loader, $1 is the gun, $2 is the round in the chamber.".Colour(Telnet.Yellow));
			return false;
		}

		UnreadyEmote = command.RemainingArgument.Fullstop();
		Changed = true;
		actor.Send(
			$"The following emote will now be used when the chamber of this gun is emptied:\n\n{UnreadyEmote}\n");
		return true;
	}

	private bool BuildingCommandFireEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What emote do you want to set for when people fire this musket?");
			actor.Send("Hint: $0 is the loader, $1 is the musket, $2 is the target.".Colour(Telnet.Yellow));
			return false;
		}

		var emoteText = command.SafeRemainingArgument.Fullstop();
		var emote = new Emote(emoteText, new DummyPerceiver(), new DummyPerceivable(), new DummyPerceivable(), new DummyPerceivable(), new DummyPerceivable());
		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return false;
		}

		FireEmote = emoteText;
		Changed = true;
		actor.Send($"The following emote will now be used when this musket is fired:\n\n{emoteText.ColourCommand()}\n");
		return true;
	}

	private bool BuildingCommandFireEmoteJam(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What emote do you want to set for when people fire this musket but it jams?");
			actor.Send("Hint: $0 is the loader, $1 is the musket, $2 is the target.".Colour(Telnet.Yellow));
			return false;
		}

		var emoteText = command.SafeRemainingArgument.Fullstop();
		var emote = new Emote(emoteText, new DummyPerceiver(), new DummyPerceivable(), new DummyPerceivable(), new DummyPerceivable(), new DummyPerceivable());
		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return false;
		}

		FireEmoteJam = emoteText;
		Changed = true;
		actor.Send($"The following emote will now be used when this musket is fired but jams:\n\n{emoteText.ColourCommand()}\n");
		return true;
	}

	private bool BuildingCommandFireEmoteMisfire(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What emote do you want to set for when people fire this musket but it misfires?");
			actor.Send("Hint: $0 is the loader, $1 is the musket, $2 is the target.".Colour(Telnet.Yellow));
			return false;
		}

		var emoteText = command.SafeRemainingArgument.Fullstop();
		var emote = new Emote(emoteText, new DummyPerceiver(), new DummyPerceivable(), new DummyPerceivable(), new DummyPerceivable(), new DummyPerceivable());
		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return false;
		}

		FireEmoteMisfire = emoteText;
		Changed = true;
		actor.Send($"The following emote will now be used when this musket is fired but misfires:\n\n{emoteText.ColourCommand()}\n");
		return true;
	}

	private bool BuildingCommandFireEmoteCatastrophy(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What emote do you want to set for when people fire this musket and get a catastrophic failure?");
			actor.Send("Hint: $0 is the loader, $1 is the musket, $2 is the target.".Colour(Telnet.Yellow));
			return false;
		}

		var emoteText = command.SafeRemainingArgument.Fullstop();
		var emote = new Emote(emoteText, new DummyPerceiver(), new DummyPerceivable(), new DummyPerceivable(), new DummyPerceivable(), new DummyPerceivable());
		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return false;
		}

		FireEmoteCatastrophy = emoteText;
		Changed = true;
		actor.Send($"The following emote will now be used when this musket is fired with a catastrophic failure:\n\n{emoteText.ColourCommand()}\n");
		return true;
	}

	private bool BuildingCommandType(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send(
				$"What Ranged Weapon Type do you want to use for this musket? See {"show ranged".Colour(Telnet.Yellow)} for a list of ranged weapon types.");
			return false;
		}

		var type = long.TryParse(command.PopSpeech(), out var value)
			? actor.Gameworld.RangedWeaponTypes.Get(value)
			: actor.Gameworld.RangedWeaponTypes.GetByName(command.Last);
		if (type == null)
		{
			actor.Send("There is no such ranged weapon type.");
			return false;
		}

		if (type.RangedWeaponType != Combat.RangedWeaponType.Musket)
		{
			actor.Send("You can only give muskets a ranged weapon type that is suitable for them.");
			return false;
		}

		RangedWeaponType = type;
		actor.Send(
			$"This musket will now be of type {RangedWeaponType.Name.TitleCase().Colour(Telnet.Green)}.");
		Changed = true;
		return true;
	}

	#endregion

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor, @"{0} (#{1:N0}r{2:N0}, {3})

This item is a muzzle-loading musket and uses the {4} ranged weapon type.
It has a barrel bore of {5} and uses {6} of gunpowder per shot.
It is also a melee weapon with type {7}.
Misfire formula: {8}
Jam formula: {9}
Catastrophy Damage: {10}

Load Emote (Clean): {11}
Load Emote (Powder): {12}
Load Emote (Ball): {13}
Load Emote (Cartridge): {14}
Load Emote (Ramrod): {15}
Load Emote (Tap): {16}
Unload Emote: {17}
Ready Emote: {18}
Unready Emote: {19}
Start Unjam Emote: {20}
Finish Unjam Emote: {21}
Fail Unjam Emote: {22}
Fire Emote: {23}
Fire Emote (Misfire): {24}
Fire Emote (Jam): {25}
Fire Emote (Catasrophy): {26}",
			"Musket Game Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name,
			RangedWeaponType.Name.ColourValue(),
			$"{BarrelBore.ToString("N2", actor)}in".ColourValue(),
			Gameworld.UnitManager.DescribeExact(PowderVolumePerShot, UnitType.Mass, actor).ColourValue(),
			MeleeWeaponType.Name.ColourValue(),
			MisfireChance.OriginalFormulaText.ColourCommand(),
			JamChance.OriginalFormulaText.ColourCommand(),
			CatastrophyDamageFormula.OriginalExpression.ColourCommand(),
			LoadEmoteClean.ColourCommand(),
			LoadEmotePowder.ColourCommand(),
			LoadEmoteBall.ColourCommand(),
			LoadEmoteCartridge.ColourCommand(),
			LoadEmoteRamrod.ColourCommand(),
			LoadEmoteTap.ColourCommand(),
			UnloadEmote.ColourCommand(),
			ReadyEmote.ColourCommand(),
			UnreadyEmote.ColourCommand(),
			StartUnjamEmote.ColourCommand(),
			FinishUnjamEmote.ColourCommand(),
			FailUnjamEmote.ColourCommand(),
			FireEmote.ColourCommand(),
			FireEmoteMisfire.ColourCommand(),
			FireEmoteJam.ColourCommand(),
			FireEmoteCatastrophy.ColourCommand()
			);
	}
}
