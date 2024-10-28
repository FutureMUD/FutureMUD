using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Economy.Currency;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems.Interfaces;

namespace MudSharp.GameItems;

public partial class GameItem
{
	#region IFutureProgVariable Members

	private static IReadOnlyDictionary<string, ProgVariableTypes> DotReferenceHandler()
	{
		return new Dictionary<string, ProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", ProgVariableTypes.Number },
			{ "effects", ProgVariableTypes.Collection | ProgVariableTypes.Effect },
			{ "name", ProgVariableTypes.Text },
			{ "type", ProgVariableTypes.Text },
			{ "proto", ProgVariableTypes.Number },
			{ "quantity", ProgVariableTypes.Number },
			{ "holder", ProgVariableTypes.Character },
			{ "wearer", ProgVariableTypes.Character },
			{ "weight", ProgVariableTypes.Number },
			{ "contents", ProgVariableTypes.Collection | ProgVariableTypes.Item },
			{ "container", ProgVariableTypes.Item },
			{ "iscontainer", ProgVariableTypes.Boolean },
			{ "isopenable", ProgVariableTypes.Boolean },
			{ "iscurrency", ProgVariableTypes.Boolean },
			{ "islock", ProgVariableTypes.Boolean },
			{ "islockable", ProgVariableTypes.Boolean },
			{ "iskey", ProgVariableTypes.Boolean },
			{ "istable", ProgVariableTypes.Boolean },
			{ "ischair", ProgVariableTypes.Boolean },
			{ "isdoor", ProgVariableTypes.Boolean },
			{ "isbelt", ProgVariableTypes.Boolean },
			{ "isbeltable", ProgVariableTypes.Boolean },
			{ "iswearable", ProgVariableTypes.Boolean },
			{ "iswieldable", ProgVariableTypes.Boolean },
			{ "isholdable", ProgVariableTypes.Boolean },
			{ "issheath", ProgVariableTypes.Boolean },
			{ "islightable", ProgVariableTypes.Boolean },
			{ "ispowered", ProgVariableTypes.Boolean },
			{ "ison", ProgVariableTypes.Boolean },
			{ "iscover", ProgVariableTypes.Boolean },
			{ "iscorpse", ProgVariableTypes.Boolean },
			{ "isweapon", ProgVariableTypes.Boolean },
			{ "ismeleeweapon", ProgVariableTypes.Boolean },
			{ "israngedweapon", ProgVariableTypes.Boolean },
			{ "providingcover", ProgVariableTypes.Boolean },
			{ "lit", ProgVariableTypes.Boolean },
			{ "open", ProgVariableTypes.Boolean },
			{ "locked", ProgVariableTypes.Boolean },
			{ "locks", ProgVariableTypes.Collection | ProgVariableTypes.Item },
			{ "corpsecharacter", ProgVariableTypes.Character },
			{ "location", ProgVariableTypes.Location },
			{ "tags", ProgVariableTypes.Collection | ProgVariableTypes.Text },
			{ "iscommodity", ProgVariableTypes.Boolean },
			{ "material", ProgVariableTypes.Material },
			{ "isgridconnectable", ProgVariableTypes.Boolean },
			{ "iselectricgridconnectable", ProgVariableTypes.Boolean },
			{ "grid", ProgVariableTypes.Number },
			{ "layer", ProgVariableTypes.Text },
			{ "isfood", ProgVariableTypes.Boolean },
			{ "isliquidcontainer", ProgVariableTypes.Boolean },
			{ "variables", ProgVariableTypes.Dictionary | ProgVariableTypes.Text}
		};
	}

	private static IReadOnlyDictionary<string, string> DotReferenceHelp()
	{
		return new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "proto", "The ID number of the proto this item was loaded from" },
			{ "quantity", "The quantity of this item in the stack. 1 if not stacked" },
			{ "holder", "The character who is holding this item, if any" },
			{ "wearer", "The character who is wearing this item, if any" },
			{ "weight", "The weight of this item in base units" },
			{ "contents", "A collection of the item contents of this item" },
			{ "container", "The item that this item is contained in, if any" },
			{ "iscontainer", "True if the item is a container" },
			{ "isopenable", "True is the item can be opened and closed" },
			{ "iscurrency", "True if the item is a currency pile" },
			{ "islock", "True if the item is a lock" },
			{ "islockable", "True if the item can be locked and unlocked" },
			{ "iskey", "True if the item is a key" },
			{ "istable", "True if the item is a table" },
			{ "ischair", "True if the item is a chair" },
			{ "isdoor", "True if the item is a door" },
			{ "isbelt", "True if the item is a belt" },
			{ "isbeltable", "True if the item can be attached to a belt" },
			{ "iswearable", "True if the item can be worn" },
			{ "iswieldable", "True if the item can be wielded" },
			{ "isholdable", "True if the item can be held" },
			{ "issheath", "True if the item is a weapon sheath" },
			{ "islightable", "True if the item can be lit with the light command" },
			{ "ispowered", "True if the item is electrically powered" },
			{ "ison", "True if the item is on" },
			{ "iscover", "True if the item can be used as ranged cover" },
			{ "iscorpse", "True if the item is a corpse" },
			{ "isweapon", "True if the item is a weapon of any kind" },
			{ "ismeleeweapon", "True if the item is a melee weapon" },
			{ "israngedweapon", "True if the item is a ranged weapon" },
			{ "providingcover", "True if the item is currently providing cover" },
			{ "lit", "True if the item is currently lit (e.g. by the light command)" },
			{ "open", "True if the item is currently open" },
			{ "locked", "True if the item is currently locked" },
			{ "locks", "A collection of the locks on this item" },
			{ "corpsecharacter", "If the item is a corpse, this is the original character" },
			{ "location", "The room that the item is in" },
			{ "tags", "A collection of the tags that this item has" },
			{ "iscommodity", "True if the item is a commodity pile" },
			{ "material", "The primary material that this item is made from" },
			{ "isgridconnectable", "True if this item can be connected to a grid" },
			{ "iselectricgridconnectable", "True if this item can be connected to an electrical grid" },
			{ "grid", "The grid that this item is connected to, if any" },
			{ "layer", "A text description of the layer this item is currently in" },
			{ "isfood", "True if the item is food" },
			{ "isliquidcontainer", "True if the item is a liquid container" },
			{ "variables", "Returns a dictionary of variable names and variable values"}
		};
	}

	private static IReadOnlyDictionary<string, ProgVariableTypes> TaggedDotReferenceHandler()
	{
		return new Dictionary<string, ProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "tags", ProgVariableTypes.Collection | ProgVariableTypes.Text }
		};
	}

	private static IReadOnlyDictionary<string, string> TaggedDotReferenceHelp()
	{
		return new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "tags", "A collection of the tags that this thing has" }
		};
	}

	public new static void RegisterFutureProgCompiler()
	{
		ProgVariable.RegisterDotReferenceCompileInfo(ProgVariableTypes.Item, DotReferenceHandler(),
			DotReferenceHelp());
		ProgVariable.RegisterDotReferenceCompileInfo(ProgVariableTypes.Tagged, TaggedDotReferenceHandler(),
			TaggedDotReferenceHelp());
	}

	public override IProgVariable GetProperty(string property)
	{
		switch (property.ToLowerInvariant())
		{
			case "proto":
				return new NumberVariable(Prototype.Id);

			case "quantity":
				return new NumberVariable(Quantity);

			case "holder":
				var holdable = GetItemType<IHoldable>();
				return holdable?.HeldBy?.Actor;

			case "wearer":
				var wearable = GetItemType<IWearable>();
				return wearable?.WornBy?.Actor;

			case "weight":
				return new NumberVariable(Weight);

			case "contents":
				return new CollectionVariable(
					IsItemType<IContainer>() ? GetItemType<IContainer>().Contents.ToList() : new List<IGameItem>(),
					ProgVariableTypes.Item
				);

			case "container":
				return ContainedIn;

			case "iscontainer":
				return new BooleanVariable(IsItemType<IContainer>());

			case "iscorpse":
				return new BooleanVariable(IsItemType<ICorpse>());

			case "isopenable":
				return new BooleanVariable(IsItemType<IOpenable>());

			case "iscurrency":
				return new BooleanVariable(IsItemType<ICurrencyPile>());

			case "islock":
				return new BooleanVariable(IsItemType<ILock>());

			case "islockable":
				return new BooleanVariable(IsItemType<ILockable>());

			case "iskey":
				return new BooleanVariable(IsItemType<IKey>());

			case "istable":
				return new BooleanVariable(IsItemType<ITable>());

			case "ischair":
				return new BooleanVariable(IsItemType<IChair>());

			case "isdoor":
				return new BooleanVariable(IsItemType<IDoor>());

			case "isbelt":
				return new BooleanVariable(IsItemType<IBelt>());

			case "isbeltable":
				return new BooleanVariable(IsItemType<IBeltable>());

			case "iswieldable":
				return new BooleanVariable(IsItemType<IWieldable>());

			case "iswearable":
				return new BooleanVariable(IsItemType<IWearable>());

			case "isholdable":
				return new BooleanVariable(IsItemType<IHoldable>());

			case "issheath":
				return new BooleanVariable(IsItemType<ISheath>());

			case "islightable":
				return new BooleanVariable(IsItemType<ILightable>());

			case "ispowered":
				return new BooleanVariable(GetItemType<IProducePower>()?.ProducingPower ?? false);

			case "ison":
				return new BooleanVariable(GetItemType<IOnOff>()?.SwitchedOn ?? false);

			case "iscover":
				return new BooleanVariable(IsItemType<IProvideCover>());

			case "isweapon":
				return new BooleanVariable(IsItemType<IRangedWeapon>() || IsItemType<IMeleeWeapon>());
			case "ismeleeweapon":
				return new BooleanVariable(IsItemType<IMeleeWeapon>());
			case "israngedweapon":
				return new BooleanVariable(IsItemType<IRangedWeapon>());

			case "providingcover":
				return new BooleanVariable(GetItemType<IProvideCover>()?.Cover != null);

			case "lit":
				var lightable = GetItemType<ILightable>();
				return new BooleanVariable(lightable?.Lit == true);

			case "open":
				var openable = GetItemType<IOpenable>();
				return new BooleanVariable(openable?.IsOpen == true);

			case "locked":
				var lockable = GetItemType<ILockable>();
				var theLock = GetItemType<ILock>();
				return new BooleanVariable(lockable?.Locks.Any(x => x.IsLocked) == true ||
				                           theLock?.IsLocked == true);

			case "locks":
				lockable = GetItemType<ILockable>();
				return new CollectionVariable(lockable?.Locks.ToList() ?? new List<ILock>(),
					ProgVariableTypes.Item);
			case "corpsecharacter":
				return GetItemType<ICorpse>()?.OriginalCharacter;
			case "location":
				return Location;
			case "tags":
				return new CollectionVariable(Tags.Select(x => x.Name).ToList(), ProgVariableTypes.Text);
			case "iscommodity":
				return new BooleanVariable(IsItemType<ICommodity>());
			case "material":
				return Material;
			case "isgridconnectable":
				return new BooleanVariable(IsItemType<ICanConnectToGrid>());
			case "iselectricgridconnectable":
				return new BooleanVariable(IsItemType<ICanConnectToElectricalGrid>());
			case "grid":
				return new NumberVariable(GetItemType<ICanConnectToGrid>()?.Grid?.Id ?? 0);
			case "layer":
				return new TextVariable(RoomLayer.DescribeEnum());
			case "isfood":
				return new BooleanVariable(IsItemType<IEdible>());
			case "isliquidcontainer":
				return new BooleanVariable(IsItemType<ILiquidContainer>());
			case "variables":
				var dict = new Dictionary<string, IProgVariable>(StringComparer.InvariantCultureIgnoreCase);
				var variable = GetItemType<IVariable>();
				if (variable is not null)
				{
					foreach (var item in variable.CharacteristicDefinitions)
					{
						dict[item.Name.ToLowerInvariant()] = new TextVariable(variable.GetCharacteristic(item).GetValue.ToLowerInvariant());
					}
				}
				return new DictionaryVariable(dict, ProgVariableTypes.Text);
			default:
				return base.GetProperty(property);
		}
	}

	public override ProgVariableTypes Type => ProgVariableTypes.Item;

	#endregion
}