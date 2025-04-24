using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using ExpressionEngine;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects.Interfaces;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Health;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.SpellEffects;

public class CreateItemEffect : IMagicSpellEffectTemplate
{
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("createitem", (root, spell) => new CreateItemEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("createitem", BuilderFactory,
			"Loads a new item into a room, character inventory or container",
			HelpText,
			true,
			true,
			SpellTriggerFactory.MagicTriggerTypes.Where(x => IsCompatibleWithTrigger(SpellTriggerFactory.BuilderInfoForType(x).TargetTypes)).ToArray());
	}

	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands,
		IMagicSpell spell)
	{
		return (new CreateItemEffect(new XElement("Effect",
			new XAttribute("type", "createitem"),
			new XElement("ItemQuality", new XCData("base")),
			new XElement("ItemPrototypeId", 0),
			new XElement("ItemSkinId", 0),
			new XElement("Quantity", 1),
			new XElement("LoadString", new XCData(""))
		), spell), string.Empty);
	}

	protected CreateItemEffect(XElement root, IMagicSpell spell)
	{
		Spell = spell;
		ItemQuality = new Expression(root.Element("ItemQuality").Value);
		_itemPrototypeId = long.Parse(root.Element("ItemPrototypeId").Value);
		_itemSkinId = long.Parse(root.Element("ItemSkinId").Value);
		Quantity = int.Parse(root.Element("Quantity").Value);
		LoadString = root.Element("LoadString").Value;
	}
	public IFuturemud Gameworld => Spell.Gameworld;

	public IMagicSpell Spell { get; }

	public Expression ItemQuality { get; private set; }

	private long _itemPrototypeId;

	public IGameItemProto ItemPrototype => Gameworld.ItemProtos.Get(_itemPrototypeId);

	private long _itemSkinId;

	public IGameItemSkin ItemSkin => Gameworld.ItemSkins.Get(_itemSkinId);

	public int Quantity { get; private set; }

	public string LoadString { get; private set; }

	public XElement SaveToXml()
	{
		return new XElement("Effect",
			new XAttribute("type", "createitem"),
			new XElement("ItemQuality", new XCData(ItemQuality.OriginalExpression)),
			new XElement("ItemPrototypeId", _itemPrototypeId),
			new XElement("ItemSkinId", _itemSkinId),
			new XElement("Quantity", Quantity),
			new XElement("LoadString", new XCData(LoadString))
		);
	}

	public bool IsInstantaneous => true;
	public bool RequiresTarget => true;

	public bool IsCompatibleWithTrigger(IMagicTrigger types) => IsCompatibleWithTrigger(types.TargetTypes);
public static bool IsCompatibleWithTrigger(string types)
	{
		switch (types)
		{
			case "item":
			case "items":
			case "character":
			case "characters":
			case "perceivable":
			case "perceivables":
			case "room":
			case "rooms":
				return true;
			default:
				return false;
		}
	}

	public IMagicSpellEffect GetOrApplyEffect(ICharacter caster, IPerceivable target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
	{
		var prototype = ItemPrototype;
		if (prototype is null)
		{
			return null;
		}

		var skin = ItemSkin;

		var quality = (ItemQuality)(int)Math.Floor(ItemQuality.EvaluateDoubleWith(
			("base", (int)prototype.BaseItemQuality),
			("power", (int)power),
			("outcome", (int)outcome)
		));

		if (target is ICharacter tch)
		{
			var items = prototype.CreateNew(caster, skin, Quantity, LoadString);
			foreach (var item in items)
			{
				if (tch.Body.CanGet(item, 0))
				{
					tch.Body.Get(item, silent: true);
				}
				else
				{
					tch.Location.Insert(item, true);
				}
				item.HandleEvent(EventType.ItemFinishedLoading, item);
				item.Login();
			}

			return null;
		}

		if (target is ICell cell)
		{
			var items = prototype.CreateNew(caster, skin, Quantity, LoadString);
			foreach (var item in items)
			{
				cell.Insert(item, true);
				item.HandleEvent(EventType.ItemFinishedLoading, item);
				item.Login();
			}

			return null;
		}

		if (target is IGameItem gitem)
		{
			var container = gitem.GetItemType<IContainer>();
			var sheathe = gitem.GetItemType<ISheath>();
			var belt = gitem.GetItemType<IBelt>();
			var items = prototype.CreateNew(caster, skin, Quantity, LoadString);
			var location = gitem.TrueLocations.FirstOrDefault() ?? caster.Location;
			foreach (var item in items)
			{
				if (container is not null && container.CanPut(item))
				{
					container.Put(null, item, false);
					item.HandleEvent(EventType.ItemFinishedLoading, item);
					item.Login();
					continue;
				}

				if (sheathe is not null && sheathe.CanSheath(item))
				{
					sheathe.Content = item.GetItemType<IWieldable>();
					item.HandleEvent(EventType.ItemFinishedLoading, item);
					item.Login();
					continue;
				}

				if (belt is not null && item.GetItemType<IBeltable>() is IBeltable beltable && belt.CanAttachBeltable(beltable) == IBeltCanAttachBeltableResult.Success)
				{
					belt.AddConnectedItem(beltable);
					item.HandleEvent(EventType.ItemFinishedLoading, item);
					item.Login();
					continue;
				}

				location.Insert(item, true);
				item.HandleEvent(EventType.ItemFinishedLoading, item);
				item.Login();
			}

			return null;

		}
		return null;
	}

	public IMagicSpellEffectTemplate Clone()
	{
		return new CreateItemEffect(SaveToXml(), Spell);
	}

	#region Implementation of IEditableItem

	public const string HelpText = @"You can use the following options with this effect:

	#3item <proto>#0 - sets the item proto to be loaded
	#3quantity <##>#0 - sets the quantity of item to be loaded
	#3skin <which>#0 - sets the item skin for the item to be loaded
	#3skin none#0 - clears the item skin
	#3load <text>#0 - sets the load argument (same as #3item load <item>#0 command)
	#3load none#0 - clears the load argument
	#3quality <formula>#0 - sets the formula for item quality. See below for possible parameters.

Parameters for quality formula:

	#6base#0 - the base quality of the item to be loaded
	#6power#0 - the power of the spell 0 (Insignificant) to 10 (Recklessly Powerful)
	#6outcome#0 - the outcome of the skill check 0 (Marginal) to 5 (Total)";
	public string Show(ICharacter actor)
	{
		return
			$"CreateItem - {Quantity.ToStringN0(actor)}x {ItemPrototype?.EditHeaderColour(actor) ?? "nothing".ColourError()}, Quality: {ItemQuality.OriginalExpression.ColourCommand()}, Load String: {LoadString.ColourCommand()}";
	}

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "item":
				return BuildingCommandItem(actor, command);
			case "quantity":
				return BuildingCommandQuantity(actor, command);
			case "skin":
				return BuildingCommandSkin(actor, command);
			case "quality":
				return BuildingCommandQuality(actor, command);
			case "load":
				return BuildingCommandLoad(actor, command);
		}

		actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
		return false;
	}

	private bool BuildingCommandLoad(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must either specify a load string or use #3none#0 to clear it.".SubstituteANSIColour());
			return false;
		}

		if (command.SafeRemainingArgument.EqualTo("none"))
		{
			LoadString = string.Empty;
			Spell.Changed = true;
			actor.OutputHandler.Send($"The item will no longer use a load string.");
			return true;
		}

		LoadString = command.SafeRemainingArgument;
		Spell.Changed = true;
		actor.OutputHandler.Send($"The item will now use the following load string: {LoadString.ColourCommand()}");
		return true;
	}

	private bool BuildingCommandQuality(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What should be the new quality formula?");
			return false;
		}

		var formula = new Expression(command.SafeRemainingArgument);
		if (formula.HasErrors())
		{
			actor.OutputHandler.Send(formula.Error);
			return false;
		}

		ItemQuality = formula;
		Spell.Changed = true;
		actor.OutputHandler.Send($"The formula for item quality is now {formula.OriginalExpression.ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandSkin(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must either specify a skin or use #3none#0 to clear it.".SubstituteANSIColour());
			return false;
		}

		if (command.SafeRemainingArgument.EqualTo("none"))
		{
			_itemSkinId = 0;
			Spell.Changed = true;
			actor.OutputHandler.Send($"The item will no longer use an item skin.");
			return true;
		}

		var skin = Gameworld.ItemSkins.GetByIdOrName(command.SafeRemainingArgument);
		if (skin is null)
		{
			actor.OutputHandler.Send($"There is no item skin identified by the text {command.SafeRemainingArgument.ColourCommand()}.");
			return false;
		}

		if (skin.Status != RevisionStatus.Current)
		{
			actor.OutputHandler.Send($"The skin {skin.EditHeader()} is not approved for use.");
			return false;
		}

		if (skin.ItemProto.Id != _itemPrototypeId)
		{
			actor.OutputHandler.Send($"The skin {skin.EditHeader()} is not designed for the item prototype that this effect loads.");
			return false;
		}

		_itemSkinId = skin.Id;
		Spell.Changed = true;
		actor.OutputHandler.Send($"This item will now load with the skin {skin.EditHeader()}.");
		return true;
	}

	private bool BuildingCommandItem(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which item prototype should this spell effect load?");
			return false;
		}

		var proto = Gameworld.ItemProtos.GetByIdOrName(command.SafeRemainingArgument);
		if (proto is null)
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid item prototype.");
			return false;
		}

		if (proto.Status != RevisionStatus.Current)
		{
			actor.OutputHandler.Send($"The item prototype {proto.EditHeaderColour(actor)} is not approved for use.");
			return false;
		}

		if (proto.PreventManualLoad)
		{
			actor.OutputHandler.Send($"The item prototype {proto.EditHeaderColour(actor)} should not be manually loaded.");
			return false;
		}

		_itemPrototypeId = proto.Id;
		_itemSkinId = 0;
		Spell.Changed = true;
		actor.OutputHandler.Send($"This spell effect will now load the item {proto.EditHeaderColour(actor)}.");
		return true;
	}

	private bool BuildingCommandQuantity(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("How many of the item should be loaded?");
			return false;
		}

		if (!int.TryParse(command.SafeRemainingArgument, out var value) || value <= 0)
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid number greater than zero.");
			return false;
		}

		Quantity = value;
		Spell.Changed = true;
		actor.OutputHandler.Send($"This effect will now load {value.ToStringN0Colour(actor)} of the item.");
		return true;
	}

	#endregion
}