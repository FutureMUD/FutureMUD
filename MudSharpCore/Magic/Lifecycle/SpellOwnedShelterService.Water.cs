#nullable enable

using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Effects.Concrete;
using MudSharp.Form.Material;
using MudSharp.Framework.Revision;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;

namespace MudSharp.Magic.Lifecycle;

public sealed partial class SpellOwnedShelterService
{
	private readonly Dictionary<Guid, WaterTransfer> _pendingWater = new();

	private string? WaterAdmissionError(SpellShelterConfiguration c, int grade)
	{
		var prototype = _world.ItemProtos.Get(c.WaterPrototypeId);
		if (prototype is not GameItemProto native || prototype.Status != RevisionStatus.Current || prototype.PreventManualLoad ||
			prototype.Morphs || native.OnLoadProgs.Any() || prototype.Components.Count() != 1 ||
			prototype.Components.Single() is not LiquidContainerGameItemComponentProto container ||
			container.Closable || container.AdjustQuantityProg is not null || container.DefaultLiquid is not null ||
			!double.IsFinite(container.LiquidCapacity) || container.LiquidCapacity <= 0 ||
			_world.DefaultHooks.Any(x => x.PerceivableType.EqualTo("GameItem")))
			return "Bind an approved immovable, open, unscripted finite liquid container with no default refill or other components.";
		var amount = c.LitresPerGrade * grade / _world.UnitManager.BaseFluidToLitres;
		if (_world.Liquids.Get(c.LiquidId) is null || !double.IsFinite(c.LitresPerGrade) || c.LitresPerGrade <= 0 ||
			!double.IsFinite(amount) || amount <= 0 || amount > container.LiquidCapacity)
			return "Bind a real liquid and a finite grade-scaled volume within the selected container's capacity.";
		if (PuddleGameItemComponentProto.ItemPrototype is not { } puddle || !ReferenceEquals(puddle.Gameworld, _world))
			return "Native puddle persistence is required to conserve remaining water when the haven disappears.";
		return null;
	}

	private GameItem PrepareWater(SpellShelterConfiguration c, ICharacter caster, int grade)
	{
		var item = new GameItem(_world.ItemProtos.Get(c.WaterPrototypeId), caster, ItemQuality.Standard, deferSpellInitialisation: true);
		item.GetItemType<ILiquidContainer>().LiquidMixture = new LiquidMixture(_world.Liquids.Get(c.LiquidId),
			c.LitresPerGrade * grade / _world.UnitManager.BaseFluidToLitres, _world);
		return item;
	}

	/// <summary>Remaining liquid (including foreign additions) moves once; no initial quantity is recreated.</summary>
	private WaterTransfer? RetireWater(FuturemudDatabaseContext context, SpellOwnedLifecycle life, SpatialLocation destination)
	{
		var claim = life.Entities.SingleOrDefault(x => x.Kind == SpellOwnedEntityKind.GameItem);
		if (claim is null || context.GameItems.Find(claim.Id) is not { } row) return null;
		var source = _world.TryGetItem(claim.Id, true) as GameItem;
		var roomId = life.Entities.Single(x => x.Kind == SpellOwnedEntityKind.Room).Id;
		RequireSupplyPersistence(context, claim.Id, roomId);
		if (source is null || !HasDirectSupplyCustody(source, _world.Rooms.Get(roomId)) || source.Components.Count() != 1 ||
			source.Components.Single() is not LiquidContainerGameItemComponent container || container.Locks.Any() ||
			source.Effects.Any(x => x is not SurfaceContaminationEffect) || source.Hooks.Any() || source.DeepItems.Skip(1).Any() || source.AttachedAndConnectedItems.Any() ||
			source.LodgedItems.Any() || source.Wounds.Any())
			throw new InvalidOperationException("The finite supply has an unadapted custody/dependency; retain its room and material.");
		WaterTransfer? result = null;
		if (container.LiquidMixture is { IsEmpty: false } mixture)
		{
			var puddle = new GameItem(PuddleGameItemComponentProto.ItemPrototype, null, ItemQuality.Standard, deferSpellInitialisation: true);
			puddle.GetItemType<ILiquidContainer>().LiquidMixture = mixture.Clone();
			puddle.MoveTo(destination, noSave: true);
			var puddleRow = (Models.GameItem)puddle.DatabaseInsert();
			var components = puddle.Components.Cast<GameItemComponent>()
				.Select(x => (x, (Models.GameItemComponent)x.DatabaseInsert())).ToArray();
			context.RoomsGameItems.Add(new Models.RoomsGameItems { GameItem = puddleRow, RoomId = destination.Room.Id });
			result = new WaterTransfer(puddle, puddleRow, components);
		}
		context.GameItems.Remove(row);
		return result;
	}

	internal static bool HasDirectSupplyCustody(IGameItem source, IRoom? room) => room is not null &&
		!source.Deleted && source.InInventoryOf is null && source.ContainedIn is null &&
		ReferenceEquals(source.Location, room) && room.GameItems.Any(x => ReferenceEquals(x, source));

	internal static void RequireSupplyPersistence(FuturemudDatabaseContext context, long itemId, long roomId)
	{
		var row = context.GameItems.Find(itemId) ?? throw new InvalidOperationException("The claimed finite supply is missing; retain its topology for recovery.");
		var rooms = context.RoomsGameItems.Where(x => x.GameItemId == itemId).Select(x => x.RoomId).ToArray();
		if (row.ContainerId is not null || rooms.Length != 1 || rooms[0] != roomId)
			throw new InvalidOperationException("The finite supply has conflicting persisted custody; retain all custodians.");
		foreach (var entity in context.Model.GetEntityTypes())
		foreach (var key in entity.GetForeignKeys().Where(x => x.PrincipalEntityType.ClrType == typeof(Models.GameItem)))
		{
			if (entity.ClrType == typeof(Models.RoomsGameItems) || entity.ClrType == typeof(Models.GameItemComponent) ||
				entity.ClrType == typeof(Models.GameItemMagicResource)) continue;
			if (key.Properties.Count != 1 || key.PrincipalKey.Properties.Single().Name != "Id")
				throw new InvalidOperationException($"Unsupported declared supply foreign key: {entity.ClrType.Name}.");
			if (HasDeclaredReference(context, entity.ClrType, key.Properties[0].Name, key.Properties[0].ClrType, itemId))
				throw new InvalidOperationException($"Foreign {entity.ClrType.Name}.{key.Properties[0].Name} references the finite supply.");
		}
	}

	private sealed record WaterTransfer(GameItem Item, Models.GameItem Row,
		IReadOnlyList<(GameItemComponent Component, Models.GameItemComponent Row)> Components)
	{
		public void Activate(IFuturemud world, SpatialLocation destination)
		{
			Item.ActivateCommittedOrdinaryItem(Row, Components);
			if (!world.Items.Any(x => ReferenceEquals(x, Item))) world.Add(Item);
			if (!destination.Room.GameItems.Contains(Item)) destination.Room.Insert(Item, true);
			Item.Login();
		}
	}
}
