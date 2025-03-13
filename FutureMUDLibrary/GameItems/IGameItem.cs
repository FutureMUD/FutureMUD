using System;
using System.Collections.Generic;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Climate;
using MudSharp.Construction;
using MudSharp.Form.Characteristics;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.Health;
using MudSharp.Magic;
using MudSharp.PerceptionEngine;

namespace MudSharp.GameItems {
	public interface IGameItem : IMortalPerceiver, IHaveMagicResource, IHaveCharacteristics, IHaveTags, IHaveABody,
		IEquatable<IGameItem> {
		IEnumerable<IGameItemComponent> Components { get; }

		/// <summary>
		///     An enumerable value that indicates the overall quality of this IGameItem
		/// </summary>
		ItemQuality Quality { get; set; }
		ItemQuality RawQuality { get; }

		IGameItemProto Prototype { get; }
		SizeCategory Size { get; }
		ISolid Material { get; set; }
		double Buoyancy(double fluidDensity);
		IGameItemGroup ItemGroup { get; }
		/// <summary>
		/// This is a percentage condition between 0.0 (destroyed) and 1.0 (perfect) that represents non-damage condition, i.e. wear and tear or maintenance
		/// </summary>
		double Condition { get; set; }

		/// <summary>
		/// This is a percentage condition between 0.0 (destroyed) and 1.0 (undamaged) that represents damage as a percentage of hitpoints
		/// </summary>
		double DamageCondition { get; }


		bool DesignedForOffhandUse { get; }

		/// <summary>
		///     Game Items are often not simply "in" a location but may be indirectly in one (e.g. in inventory) or even more (e.g.
		///     installed doors). This will return all cells they are actually in.
		/// </summary>
		IEnumerable<ICell> TrueLocations { get; }

		/// <summary>
		/// This version of the TrueLocations property is designed to be used internally to avoid infinite loops when grabbing TrueLocations of things that are connected to other things. You should consider using the property version unless you know what you are doing.
		/// </summary>
		/// <param name="itemsConsidered"></param>
		/// <returns></returns>
		IEnumerable<ICell> TrueLocationsExcept(List<IGameItem> itemsConsidered);

		/// <summary>
		///     This is an item that for one reason or other this item is "In", whether that be sheath, container, etc.
		/// </summary>
		IGameItem ContainedIn { get; set; }

		/// <summary>
		/// A collection of all items, including the item itself, that are contained within this item. This will recursively go down as many layers as it has to.
		/// </summary>
		IEnumerable<IGameItem> DeepItems { get; }

		/// <summary>
		/// A collection of all items, including the item itself that are contained within this item. It will not go down further layers.
		/// </summary>
		IEnumerable<IGameItem> ShallowItems { get; }

		/// <summary>
		/// A collection of all items, including the item itself that are contained within this item IF they can be removed
		/// </summary>
		IEnumerable<IGameItem> ShallowAccessibleItems(ICharacter potentialGetter);

		/// <summary>
		/// This returns an IPerceivable (which may be this item) that represents the perceivable "thing" that is actually in the room, for purposes of working out proximity of this item irrespestive of whether it is sitting in the room, being carried, in a container, attached to something etc.
		/// </summary>
		IPerceivable LocationLevelPerceivable { get; }

		/// <summary>
		/// Sets the ContainedIn Property but short circuits saving the item, for load-time setting of this property
		/// </summary>
		/// <param name="item"></param>
		void LoadTimeSetContainedIn(IGameItem item);

		IBody InInventoryOf { get; }

		bool IsInInventory(IBody body);

		bool Deleted { get; }

		bool Destroyed { get; }

		bool HighPriority { get; }

		bool CanBeBundled { get; }

		void LoadPosition(Models.GameItem item);

		/// <summary>
		///     Called when the item is required to non-permanent leave the game, for example, when it leaves in the inventory of a
		///     quitting player
		/// </summary>
		void Quit();

		/// <summary>
		///     Called when the item is to leave the game permenantly. Handles all database deletion and gameworld removal.
		/// </summary>
		void Delete();

		void Login();

		/// <summary>
		///     Returns a value indicating whether this IGameItem contains an IGameItemComponent of the specified type
		/// </summary>
		/// <typeparam name="T">Any type derived from IGameItemComponent</typeparam>
		/// <returns>True if this IGameItem contains any IGameItemComponents of Type T</returns>
		bool IsItemType<T>() where T : IGameItemComponent;

		/// <summary>
		///     Returns the IGameItemComponent of the specified Type T deriving from IGameItemComponent
		/// </summary>
		/// <typeparam name="T">Any type derived from IGameItemComponent</typeparam>
		/// <returns>Any IGameItemComponent of Type T that this IGameItem contains</returns>
		T GetItemType<T>() where T : IGameItemComponent;

		IEnumerable<T> GetItemTypes<T>() where T : IGameItemComponent;

		/// <summary>
		///     Indicates whether this game item can move to another room, whether in the inventory of someone who is moving, or
		///     when being dragged
		/// </summary>
		/// <returns></returns>
		bool PreventsMovement();

		/// <summary>
		///     Indicates why an item cannot be moved if it cannot be moved
		/// </summary>
		/// <param name="mover"></param>
		/// <returns></returns>
		string WhyPreventsMovement(ICharacter mover);

		/// <summary>
		///     Called when an item has been forcefully moved (such as admin teleportation or spell)
		/// </summary>
		void ForceMove();

		bool AllowReposition();
		string WhyCannotReposition();
		bool CanMerge(IGameItem otherItem);
		void Merge(IGameItem otherItem);
		void Handle(string text, OutputRange range = OutputRange.Personal);
		void Handle(IOutput output, OutputRange range = OutputRange.Personal);

		/// <summary>
		///     This function is called at the end of a batch of loading for tasks that require other objects to be fully loaded
		///     and ready
		/// </summary>
		void FinaliseLoadTimeTasks();

		/// <summary>
		///     This function attempts to swap an existing item with a new item, leaving the item itself to take care of the
		///     details of what that means.
		///     This is mainly designed to be used with item morphs, corpses, etc
		/// </summary>
		/// <param name="existingItem">An existing item that has ContainedIn set to this item</param>
		/// <param name="newItem">The new item to swap in</param>
		/// <returns>True is a swap was made</returns>
		bool SwapInPlace(IGameItem existingItem, IGameItem newItem);

		/// <summary>
		///     Forcefully "takes" a contained item, leaving the components to take care of the details of what that means. Caller
		///     assumes responsibility for the item from there.
		/// </summary>
		/// <param name="item">The item to be taken</param>
		void Take(IGameItem item);

		event ConnectedEvent OnConnected;
		event ConnectedEvent OnDisconnected;
		event PerceivableEvent OnRemovedFromLocation;
		event InventoryChangeEvent OnInventoryChange;
		void InvokeInventoryChange(InventoryState oldState, InventoryState newState);

		void ConnectedItem(IConnectable other, ConnectorType type);
		void DisconnectedItem(IConnectable other, ConnectorType type);

		bool CheckPrototypeForUpdate();

		IEnumerable<IWound> PassiveSufferDamageViaContainedItem(IExplosiveDamage damage, Proximity proximity, Facing facing, IGameItem source);

		/// <summary>
		/// Whether or not we should warn the purger before purging this item
		/// </summary>
		bool WarnBeforePurge { get; }

		void ExposeToLiquid(LiquidMixture mixture, IBodypart part, LiquidExposureDirection direction);
		ItemSaturationLevel SaturationLevel { get; }
		ItemSaturationLevel SaturationLevelForLiquid(LiquidInstance instance);
		ItemSaturationLevel SaturationLevelForLiquid(double total);
		(double Coating, double Absorb) LiquidAbsorbtionAmounts { get; }
		void ExposeToPrecipitation(PrecipitationLevel level, ILiquid liquid);

		#region Inventory

		/// <summary>
		///     Whether or not the IGameItem can be "gotten" based on IGameItem properties only
		/// </summary>
		/// <returns>ItemGetResponse indicating ability to be gotten</returns>
		ItemGetResponse CanGet(ItemCanGetIgnore ignoreFlags = ItemCanGetIgnore.None);

		/// <summary>
		///     Whether or not the IGameItem can be "gotten" in the specified quantity based on IGameItem properties only
		/// </summary>
		/// <param name="quantity">The quantity of the item desired to get</param>
		/// <returns>ItemGetResponse indicating ability to be gotten</returns>
		ItemGetResponse CanGet(int quantity, ItemCanGetIgnore ignoreFlags = ItemCanGetIgnore.None);

		/// <summary>
		///     Get the IGameItem to the specified person's inventory. Handles IGameItem side only.
		/// </summary>
		/// <param name="getter">The IBody that is "Getting" the item</param>
		/// <returns>The IGameItem that is being gotten</returns>
		IGameItem Get(IBody getter);

		/// <summary>
		///     Get the IGameItem to the specified person's inventory in the specified quantity. Handles IGameItem side only.
		/// </summary>
		/// <param name="getter">The IBody that is "Getting" the item</param>
		/// <param name="quantity">The quantity to get</param>
		/// <returns>The IGameItem that is being gotten</returns>
		IGameItem Get(IBody getter, int quantity);

		/// <summary>
		///     Specifies whether the whole IGameItem drops when the specified quantity argument is given, or whether it splits
		/// </summary>
		/// <param name="quantity">The quantity against which to test</param>
		/// <returns>True if the whole IGameItem drops, false if it splits</returns>
		bool DropsWhole(int quantity);

		bool DropsWholeByWeight(double weight);

		IGameItem DropByWeight(ICell location, double weight);
		IGameItem GetByWeight(IBody getter, double weight);
		IGameItem PeekSplitByWeight(double weight);

		/// <summary>
		///     Drop the IGameItem to the specified ILocation. Handles IGameItem side only.
		/// </summary>
		/// <param name="location"></param>
		/// <returns></returns>
		IGameItem Drop(ICell location);

		/// <summary>
		///     Drop the IGameItem to the specified ILocation in the specified quantity. Handles IGameItem side only.
		/// </summary>
		/// <param name="location"></param>
		/// <param name="quantity"></param>
		/// <returns></returns>
		IGameItem Drop(ICell location, int quantity);

		IGameItem PeekSplit(int quantity);

		int Quantity { get; }

		IEnumerable<IGameItem> AttachedAndConnectedItems { get; }

		IEnumerable<IGameItem> LodgedItems { get; }
		IEnumerable<IGameItem> AttachedItems { get; }
		IEnumerable<ConnectorType> Connections { get; }
		IEnumerable<Tuple<ConnectorType, IConnectable>> ConnectedItems { get; }
		IEnumerable<ConnectorType> FreeConnections { get; }

		string Evaluate(ICharacter actor);

		/// <summary>
		/// Creates a new item that is a copy of this item, including similar copies of all contained items
		/// </summary>
		/// <returns></returns>
		IGameItem DeepCopy(bool addToGameworld);

		#endregion

		DateTime MorphTime { get; set; }
		TimeSpan? CachedMorphTime { get; }
		void StartMorphTimer();
		void EndMorphTimer();

		IGameItemSkin Skin { get; set; }
	}
}