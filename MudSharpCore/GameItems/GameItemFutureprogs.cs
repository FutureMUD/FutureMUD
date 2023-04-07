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

	private static IReadOnlyDictionary<string, FutureProgVariableTypes> DotReferenceHandler()
	{
		return new Dictionary<string, FutureProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", FutureProgVariableTypes.Number },
			{ "effects", FutureProgVariableTypes.Collection | FutureProgVariableTypes.Effect },
			{ "name", FutureProgVariableTypes.Text },
			{ "type", FutureProgVariableTypes.Text },
			{ "proto", FutureProgVariableTypes.Number },
			{ "quantity", FutureProgVariableTypes.Number },
			{ "holder", FutureProgVariableTypes.Character },
			{ "wearer", FutureProgVariableTypes.Character },
			{ "weight", FutureProgVariableTypes.Number },
			{ "contents", FutureProgVariableTypes.Collection | FutureProgVariableTypes.Item },
			{ "container", FutureProgVariableTypes.Item },
			{ "iscontainer", FutureProgVariableTypes.Boolean },
			{ "isopenable", FutureProgVariableTypes.Boolean },
			{ "iscurrency", FutureProgVariableTypes.Boolean },
			{ "islock", FutureProgVariableTypes.Boolean },
			{ "islockable", FutureProgVariableTypes.Boolean },
			{ "iskey", FutureProgVariableTypes.Boolean },
			{ "istable", FutureProgVariableTypes.Boolean },
			{ "ischair", FutureProgVariableTypes.Boolean },
			{ "isdoor", FutureProgVariableTypes.Boolean },
			{ "isbelt", FutureProgVariableTypes.Boolean },
			{ "isbeltable", FutureProgVariableTypes.Boolean },
			{ "iswearable", FutureProgVariableTypes.Boolean },
			{ "iswieldable", FutureProgVariableTypes.Boolean },
			{ "isholdable", FutureProgVariableTypes.Boolean },
			{ "issheath", FutureProgVariableTypes.Boolean },
			{ "islightable", FutureProgVariableTypes.Boolean },
			{ "ispowered", FutureProgVariableTypes.Boolean },
			{ "ison", FutureProgVariableTypes.Boolean },
			{ "iscover", FutureProgVariableTypes.Boolean },
			{ "iscorpse", FutureProgVariableTypes.Boolean },
			{ "isweapon", FutureProgVariableTypes.Boolean },
			{ "ismeleeweapon", FutureProgVariableTypes.Boolean },
			{ "israngedweapon", FutureProgVariableTypes.Boolean },
			{ "providingcover", FutureProgVariableTypes.Boolean },
			{ "lit", FutureProgVariableTypes.Boolean },
			{ "open", FutureProgVariableTypes.Boolean },
			{ "locked", FutureProgVariableTypes.Boolean },
			{ "locks", FutureProgVariableTypes.Collection | FutureProgVariableTypes.Item },
			{ "corpsecharacter", FutureProgVariableTypes.Character },
			{ "location", FutureProgVariableTypes.Location },
			{ "tags", FutureProgVariableTypes.Collection | FutureProgVariableTypes.Text },
			{ "iscommodity", FutureProgVariableTypes.Boolean },
			{ "material", FutureProgVariableTypes.Material },
			{ "isgridconnectable", FutureProgVariableTypes.Boolean },
			{ "iselectricgridconnectable", FutureProgVariableTypes.Boolean },
			{ "grid", FutureProgVariableTypes.Number },
			{ "layer", FutureProgVariableTypes.Text }
		};
	}

	private static IReadOnlyDictionary<string, string> DotReferenceHelp()
	{
		return new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "proto", "" },
			{ "quantity", "" },
			{ "holder", "" },
			{ "wearer", "" },
			{ "weight", "" },
			{ "contents", "" },
			{ "container", "" },
			{ "iscontainer", "" },
			{ "isopenable", "" },
			{ "iscurrency", "" },
			{ "islock", "" },
			{ "islockable", "" },
			{ "iskey", "" },
			{ "istable", "" },
			{ "ischair", "" },
			{ "isdoor", "" },
			{ "isbelt", "" },
			{ "isbeltable", "" },
			{ "iswearable", "" },
			{ "iswieldable", "" },
			{ "isholdable", "" },
			{ "issheath", "" },
			{ "islightable", "" },
			{ "ispowered", "" },
			{ "ison", "" },
			{ "iscover", "" },
			{ "iscorpse", "" },
			{ "isweapon", "" },
			{ "ismeleeweapon", "" },
			{ "israngedweapon", "" },
			{ "providingcover", "" },
			{ "lit", "" },
			{ "open", "" },
			{ "locked", "" },
			{ "locks", "" },
			{ "corpsecharacter", "" },
			{ "location", "" },
			{ "tags", "" },
			{ "iscommodity", "" },
			{ "material", "" },
			{ "isgridconnectable", "" },
			{ "iselectricgridconnectable", "" },
			{ "grid", "" },
			{ "layer", "" }
		};
	}

	private static IReadOnlyDictionary<string, FutureProgVariableTypes> TaggedDotReferenceHandler()
	{
		return new Dictionary<string, FutureProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "tags", FutureProgVariableTypes.Collection | FutureProgVariableTypes.Text }
		};
	}

	private static IReadOnlyDictionary<string, string> TaggedDotReferenceHelp()
	{
		return new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "tags", "" }
		};
	}

	public new static void RegisterFutureProgCompiler()
	{
		FutureProgVariable.RegisterDotReferenceCompileInfo(FutureProgVariableTypes.Item, DotReferenceHandler(),
			DotReferenceHelp());
		FutureProgVariable.RegisterDotReferenceCompileInfo(FutureProgVariableTypes.Tagged, TaggedDotReferenceHandler(),
			TaggedDotReferenceHelp());
	}

	public override IFutureProgVariable GetProperty(string property)
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
					FutureProgVariableTypes.Item
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
					FutureProgVariableTypes.Item);
			case "corpsecharacter":
				return GetItemType<ICorpse>()?.OriginalCharacter;
			case "location":
				return Location;
			case "tags":
				return new CollectionVariable(Tags.Select(x => x.Name).ToList(), FutureProgVariableTypes.Text);
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
			default:
				return base.GetProperty(property);
		}
	}

	public override FutureProgVariableTypes Type => FutureProgVariableTypes.Item;

	#endregion
}