#nullable enable

using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private void SeedMedievalDoorsLocksAndStrongboxes()
	{
		CreateItem(
			"medieval_door_shared_bamboo_screen_hanging",
			"screen",
			"a bamboo doorway screen",
			null,
			"This bamboo doorway screen is a large, workmanlike screen built from split bamboo. Thin slats are bound into a flat screen, with a top rail for hanging and a lightly weighted lower edge. The gaps are narrow but still visible. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.Large,
			ItemQuality.Standard,
			3000.0,
			36.0m,
			true,
			false,
			"bamboo",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Door_Bad_Large"
			],
			null,
			null,
			null,
			null,
			"Bamboo screen-like doorway barrier with weak door behaviour."
		);

		CreateItem(
			"medieval_door_shared_barn_door",
			"door",
			"a broad barn door",
			null,
			"This broad barn door is a very large, workmanlike door built from pine boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.VeryLarge,
			ItemQuality.Standard,
			62000.0,
			130.0m,
			true,
			false,
			"pine",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Door_Normal_VeryLarge"
			],
			null,
			null,
			null,
			null,
			"Broad plank door for barns and granaries."
		);

		CreateItem(
			"medieval_door_shared_bathhouse_screen_door",
			"screen",
			"a bathhouse screen door",
			null,
			"This bathhouse screen door is a large, workmanlike screen built from cedar boards. Thin slats are bound into a flat screen, with a top rail for hanging and a lightly weighted lower edge. The gaps are narrow but still visible. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.Large,
			ItemQuality.Standard,
			17000.0,
			52.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Door_Bad_Large"
			],
			null,
			null,
			null,
			null,
			"Light screen-like door for baths, washrooms, and changing spaces."
		);

		CreateItem(
			"medieval_door_shared_boarded_yard_gate",
			"gate",
			"a boarded yard gate",
			null,
			"This boarded yard gate is a very large, workmanlike gate built from oak boards. Vertical pales sit inside a braced frame, leaving open gaps between the timbers. The latch side is squared to meet a post cleanly. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.VeryLarge,
			ItemQuality.Standard,
			52000.0,
			110.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Gate_Normal_VeryLarge"
			],
			null,
			null,
			null,
			null,
			"Common yard gate; gate behaviour can be seen and fired through."
		);

		CreateItem(
			"medieval_door_shared_bone_bead_doorway_hanging",
			"hanging",
			"a bone bead doorway hanging",
			null,
			"This bone bead doorway hanging is a large, workmanlike hanging worked from bone. Strings of beads hang from a narrow header, leaving small gaps between each strand. The lower ends are uneven from movement through the passage. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.Large,
			ItemQuality.Standard,
			2800.0,
			38.0m,
			true,
			false,
			"bone",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Door_Bad_Large"
			],
			null,
			null,
			null,
			null,
			"Beaded doorway hanging that marks a passage without making a secure barrier."
		);

		CreateItem(
			"medieval_door_shared_carpet_doorway_hanging",
			"hanging",
			"a carpet doorway hanging",
			null,
			"This carpet doorway hanging is a large, well-made hanging made from woven wool. Heavy fabric hangs from a reinforced top edge, falling in a single soft sheet. The lower hem is weighted so it settles back after being pushed aside. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.Large,
			ItemQuality.Good,
			5200.0,
			70.0m,
			true,
			false,
			"wool",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Luxury Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Door_Bad_Large"
			],
			null,
			null,
			null,
			null,
			"Heavy woven doorway hanging used as a soft barrier."
		);

		CreateItem(
			"medieval_door_shared_cart_gate",
			"gate",
			"a cart-yard gate",
			null,
			"This cart-yard gate is a very large, workmanlike gate built from oak boards. Vertical pales sit inside a braced frame, leaving open gaps between the timbers. The latch side is squared to meet a post cleanly. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.VeryLarge,
			ItemQuality.Standard,
			64000.0,
			135.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Gate_Normal_VeryLarge"
			],
			null,
			null,
			null,
			null,
			"Wide yard gate suitable for carts and wagons."
		);

		CreateItem(
			"medieval_door_shared_cellar_door",
			"door",
			"a braced cellar door",
			null,
			"This braced cellar door is a large, well-made door built from oak boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.Large,
			ItemQuality.Good,
			42000.0,
			96.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Door_Secure_Large"
			],
			null,
			null,
			null,
			null,
			"Secure cellar door, smashable and uninstallable only from the hinge side by component behaviour."
		);

		CreateItem(
			"medieval_door_shared_cellar_grate",
			"grate",
			"a cellar stair grate",
			null,
			"This cellar stair grate is a large, well-made grate worked from wrought iron. Iron bars cross the frame in a regular grid, leaving open spaces between them. The hinge side is heavier than the closing side. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.Large,
			ItemQuality.Good,
			52000.0,
			210.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Gate_Secure_Large"
			],
			null,
			null,
			null,
			null,
			"Secure barred grate for cellar stairs or storage entries."
		);

		CreateItem(
			"medieval_door_shared_counting_house_grille",
			"grate",
			"a counting-house grille",
			null,
			"This counting-house grille is a large, well-made grate worked from wrought iron. Iron bars cross the frame in a regular grid, leaving open spaces between them. The hinge side is heavier than the closing side. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.Large,
			ItemQuality.Good,
			50000.0,
			230.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Gate_Lockable_Secure_Large"
			],
			null,
			null,
			null,
			null,
			"Lockable barred grille for counting rooms, pay windows, or secure counters."
		);

		CreateItem(
			"medieval_door_shared_courtyard_grille_gate",
			"gate",
			"a barred courtyard gate",
			null,
			"This barred courtyard gate is a very large, well-made gate worked from wrought iron. Vertical pales sit inside a braced frame, leaving open gaps between the timbers. The latch side is squared to meet a post cleanly. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.VeryLarge,
			ItemQuality.Good,
			86000.0,
			360.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Gate_Tough_VeryLarge"
			],
			null,
			null,
			null,
			null,
			"Very large barred gate for courtyards and enclosed compounds."
		);

		CreateItem(
			"medieval_door_shared_felt_doorway_hanging",
			"hanging",
			"a felt doorway hanging",
			null,
			"This felt doorway hanging is a large, workmanlike hanging made from pressed felt. Heavy fabric hangs from a reinforced top edge, falling in a single soft sheet. The lower hem is weighted so it settles back after being pushed aside. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.Large,
			ItemQuality.Standard,
			3200.0,
			28.0m,
			true,
			false,
			"felt",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Door_Bad_Large"
			],
			null,
			null,
			null,
			null,
			"Thicker felt hanging for drafts and privacy, not a secure door."
		);

		CreateItem(
			"medieval_door_shared_fur_doorway_hanging",
			"hanging",
			"a fur doorway hanging",
			null,
			"This fur doorway hanging is a large, workmanlike hanging made from dressed fur. Heavy fabric hangs from a reinforced top edge, falling in a single soft sheet. The lower hem is weighted so it settles back after being pushed aside. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.Large,
			ItemQuality.Standard,
			4200.0,
			44.0m,
			true,
			false,
			"fur",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Door_Bad_Large"
			],
			null,
			null,
			null,
			null,
			"Warm fur doorway hanging for cold interiors and drafty doors."
		);

		CreateItem(
			"medieval_door_shared_garden_gate",
			"gate",
			"a garden gate",
			null,
			"This garden gate is a large, workmanlike gate built from willow boards. Vertical pales sit inside a braced frame, leaving open gaps between the timbers. The latch side is squared to meet a post cleanly. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.Large,
			ItemQuality.Standard,
			18000.0,
			28.0m,
			true,
			false,
			"willow",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Gate_Bad_Large"
			],
			null,
			null,
			null,
			null,
			"Light garden or orchard gate with weak gate behaviour."
		);

		CreateItem(
			"medieval_door_shared_granary_door",
			"door",
			"a raised granary door",
			null,
			"This raised granary door is a large, workmanlike door built from oak boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.Large,
			ItemQuality.Standard,
			35000.0,
			82.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Door_Secure_Large"
			],
			null,
			null,
			null,
			null,
			"Secure granary door designed for food stores and raised storehouses."
		);

		CreateItem(
			"medieval_door_shared_heavy_hall_door",
			"door",
			"a heavy hall door",
			null,
			"This heavy hall door is a large, well-made door built from oak boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.Large,
			ItemQuality.Good,
			50000.0,
			150.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Door_Tough_Large"
			],
			null,
			null,
			null,
			null,
			"Heavy plank door for halls, courts, and substantial households."
		);

		CreateItem(
			"medieval_door_shared_inn_chamber_door",
			"door",
			"an inn chamber door",
			null,
			"This inn chamber door is a large, workmanlike door built from beech boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.Large,
			ItemQuality.Standard,
			25500.0,
			68.0m,
			true,
			false,
			"beech",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Door_Normal_Large"
			],
			null,
			null,
			null,
			null,
			"Modest interior door for inn chambers or rented rooms."
		);

		CreateItem(
			"medieval_door_shared_interior_chamber_door",
			"door",
			"an interior chamber door",
			null,
			"This interior chamber door is a large, workmanlike door built from beech boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.Large,
			ItemQuality.Standard,
			24000.0,
			54.0m,
			true,
			false,
			"beech",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Door_Normal_Large"
			],
			null,
			null,
			null,
			null,
			"Lighter door for bedchambers, storerooms, and inner rooms."
		);

		CreateItem(
			"medieval_door_shared_ironbound_door",
			"door",
			"an iron-bound door",
			null,
			"This iron-bound door is a large, well-made door built from oak boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.Large,
			ItemQuality.Good,
			57000.0,
			190.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Door_Tough_Large"
			],
			null,
			null,
			null,
			null,
			"Oak door strengthened with visible iron strapping and no built-in lock behaviour."
		);

		CreateItem(
			"medieval_door_shared_leather_strip_hanging",
			"hanging",
			"a leather strip doorway hanging",
			null,
			"This leather strip doorway hanging is a large, workmanlike hanging made from worked leather. Heavy fabric hangs from a reinforced top edge, falling in a single soft sheet. The lower hem is weighted so it settles back after being pushed aside. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.Large,
			ItemQuality.Standard,
			3600.0,
			42.0m,
			true,
			false,
			"leather",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Door_Bad_Large"
			],
			null,
			null,
			null,
			null,
			"Strip-hanging for workshops, stables, or storerooms."
		);

		CreateItem(
			"medieval_door_shared_linen_doorway_curtain",
			"curtain",
			"a linen doorway curtain",
			null,
			"This linen doorway curtain is a large, workmanlike curtain made from woven linen. Heavy fabric hangs from a reinforced top edge, falling in a single soft sheet. The lower hem is weighted so it settles back after being pushed aside. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.Large,
			ItemQuality.Standard,
			1500.0,
			18.0m,
			true,
			false,
			"linen",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Door_Bad_Large"
			],
			null,
			null,
			null,
			null,
			"Light doorway curtain with weak door behaviour."
		);

		CreateItem(
			"medieval_door_shared_livestock_pen_gate",
			"gate",
			"a livestock pen gate",
			null,
			"This livestock pen gate is a large, workmanlike gate built from ash boards. Vertical pales sit inside a braced frame, leaving open gaps between the timbers. The latch side is squared to meet a post cleanly. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.Large,
			ItemQuality.Standard,
			22000.0,
			40.0m,
			true,
			false,
			"ash",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Gate_Normal_Large"
			],
			null,
			null,
			null,
			null,
			"Simple gate for pens, folds, and stockyards."
		);

		CreateItem(
			"medieval_door_shared_lockable_barn_door",
			"door",
			"a lockable barn door",
			null,
			"This lockable barn door is a very large, workmanlike door built from pine boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.VeryLarge,
			ItemQuality.Standard,
			66000.0,
			190.0m,
			true,
			false,
			"pine",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Door_Lockable_Normal_VeryLarge"
			],
			null,
			null,
			null,
			null,
			"Large barn door with built-in lock behaviour."
		);

		CreateItem(
			"medieval_door_shared_lockable_cart_gate",
			"gate",
			"a lockable cart-yard gate",
			null,
			"This lockable cart-yard gate is a very large, well-made gate built from oak boards. Vertical pales sit inside a braced frame, leaving open gaps between the timbers. The latch side is squared to meet a post cleanly. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.VeryLarge,
			ItemQuality.Good,
			70000.0,
			205.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Gate_Lockable_Normal_VeryLarge"
			],
			null,
			null,
			null,
			null,
			"Wide cart gate with built-in lock behaviour."
		);

		CreateItem(
			"medieval_door_shared_lockable_cellar_door",
			"door",
			"a lockable braced cellar door",
			null,
			"This lockable braced cellar door is a large, well-made door built from oak boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.Large,
			ItemQuality.Good,
			45000.0,
			150.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Door_Lockable_Secure_Large"
			],
			null,
			null,
			null,
			null,
			"Cellar door with built-in lock and hinge-side security behaviour."
		);

		CreateItem(
			"medieval_door_shared_lockable_chamber_door",
			"door",
			"a lockable chamber door",
			null,
			"This lockable chamber door is a large, well-made door built from beech boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.Large,
			ItemQuality.Good,
			26500.0,
			92.0m,
			true,
			false,
			"beech",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Door_Lockable_Normal_Large"
			],
			null,
			null,
			null,
			null,
			"Inner-room door with built-in lock behaviour."
		);

		CreateItem(
			"medieval_door_shared_lockable_cottage_door",
			"door",
			"a lockable cottage door",
			null,
			"This lockable cottage door is a large, workmanlike door built from pine boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.Large,
			ItemQuality.Standard,
			30500.0,
			88.0m,
			true,
			false,
			"pine",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Door_Lockable_Normal_Large"
			],
			null,
			null,
			null,
			null,
			"Plain cottage-scale door with built-in lock behaviour."
		);

		CreateItem(
			"medieval_door_shared_lockable_courtyard_grille_gate",
			"gate",
			"a lockable barred courtyard gate",
			null,
			"This lockable barred courtyard gate is a very large, well-made gate worked from wrought iron. Vertical pales sit inside a braced frame, leaving open gaps between the timbers. The latch side is squared to meet a post cleanly. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.VeryLarge,
			ItemQuality.Good,
			92000.0,
			450.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Gate_Lockable_Tough_VeryLarge"
			],
			null,
			null,
			null,
			null,
			"Lockable very large barred gate for courtyards and compounds."
		);

		CreateItem(
			"medieval_door_shared_lockable_granary_door",
			"door",
			"a lockable granary door",
			null,
			"This lockable granary door is a large, well-made door built from oak boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.Large,
			ItemQuality.Good,
			38000.0,
			136.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Door_Lockable_Secure_Large"
			],
			null,
			null,
			null,
			null,
			"Granary door with built-in lock behaviour."
		);

		CreateItem(
			"medieval_door_shared_lockable_hall_door",
			"door",
			"a lockable heavy hall door",
			null,
			"This lockable heavy hall door is a large, well-made door built from oak boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.Large,
			ItemQuality.Good,
			53500.0,
			210.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Door_Lockable_Tough_Large"
			],
			null,
			null,
			null,
			null,
			"Tough hall door with built-in lock behaviour."
		);

		CreateItem(
			"medieval_door_shared_lockable_inn_chamber_door",
			"door",
			"a lockable inn chamber door",
			null,
			"This lockable inn chamber door is a large, well-made door built from beech boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.Large,
			ItemQuality.Good,
			28000.0,
			110.0m,
			true,
			false,
			"beech",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Door_Lockable_Normal_Large"
			],
			null,
			null,
			null,
			null,
			"Inn chamber door with built-in lock behaviour."
		);

		CreateItem(
			"medieval_door_shared_lockable_ironbound_door",
			"door",
			"a lockable iron-bound door",
			null,
			"This lockable iron-bound door is a large, well-made door built from oak boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.Large,
			ItemQuality.Good,
			60500.0,
			255.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Door_Lockable_Tough_Large"
			],
			null,
			null,
			null,
			null,
			"Iron-bound door with built-in lock behaviour."
		);

		CreateItem(
			"medieval_door_shared_lockable_mill_door",
			"door",
			"a lockable millhouse door",
			null,
			"This lockable millhouse door is a large, well-made door built from beech boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.Large,
			ItemQuality.Good,
			39000.0,
			132.0m,
			true,
			false,
			"beech",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Door_Lockable_Normal_Large"
			],
			null,
			null,
			null,
			null,
			"Millhouse or work-building door with built-in lock behaviour."
		);

		CreateItem(
			"medieval_door_shared_lockable_palisade_gate",
			"gate",
			"a lockable palisade gate",
			null,
			"This lockable palisade gate is a very large, well-made gate built from oak boards. Vertical pales sit inside a braced frame, leaving open gaps between the timbers. The latch side is squared to meet a post cleanly. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.VeryLarge,
			ItemQuality.Good,
			76000.0,
			240.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Gate_Lockable_Secure_VeryLarge"
			],
			null,
			null,
			null,
			null,
			"Large palisade gate with built-in lock behaviour."
		);

		CreateItem(
			"medieval_door_shared_lockable_pantry_door",
			"door",
			"a lockable pantry door",
			null,
			"This lockable pantry door is a large, workmanlike door built from ash boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.Large,
			ItemQuality.Standard,
			25000.0,
			86.0m,
			true,
			false,
			"ash",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Door_Lockable_Normal_Large"
			],
			null,
			null,
			null,
			null,
			"Pantry or buttery door with built-in lock behaviour."
		);

		CreateItem(
			"medieval_door_shared_lockable_plank_household_door",
			"door",
			"a lockable plank household door",
			null,
			"This lockable plank household door is a large, well-made door built from oak boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.Large,
			ItemQuality.Good,
			39000.0,
			132.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Door_Lockable_Normal_Large"
			],
			null,
			null,
			null,
			null,
			"Household door with built-in lock behaviour for houses, shops, or private chambers."
		);

		CreateItem(
			"medieval_door_shared_lockable_portcullis_grate",
			"grate",
			"a lockable portcullis-like grate",
			null,
			"This lockable portcullis-like grate is a huge, well-made grate worked from wrought iron. Iron bars cross the frame in a regular grid, leaving open spaces between them. The hinge side is heavier than the closing side. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.Huge,
			ItemQuality.Good,
			168000.0,
			740.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Gate_Lockable_Tough_Huge"
			],
			null,
			null,
			null,
			null,
			"Huge lockable barred gate profile for strong entrances."
		);

		CreateItem(
			"medieval_door_shared_lockable_postern_gate",
			"gate",
			"a lockable postern gate",
			null,
			"This lockable postern gate is a very large, well-made gate built from oak boards. Vertical pales sit inside a braced frame, leaving open gaps between the timbers. The latch side is squared to meet a post cleanly. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.VeryLarge,
			ItemQuality.Good,
			70000.0,
			255.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Gate_Lockable_Secure_VeryLarge"
			],
			null,
			null,
			null,
			null,
			"Postern gate with built-in lock behaviour."
		);

		CreateItem(
			"medieval_door_shared_lockable_shop_door",
			"door",
			"a lockable shopfront door",
			null,
			"This lockable shopfront door is a large, well-made door built from elm boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.Large,
			ItemQuality.Good,
			37000.0,
			126.0m,
			true,
			false,
			"elm",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Door_Lockable_Normal_Large"
			],
			null,
			null,
			null,
			null,
			"Shop or workshop door with built-in lock behaviour."
		);

		CreateItem(
			"medieval_door_shared_lockable_stable_door",
			"door",
			"a lockable stable door",
			null,
			"This lockable stable door is a large, well-made door built from oak boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.Large,
			ItemQuality.Good,
			41500.0,
			142.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Door_Lockable_Normal_Large"
			],
			null,
			null,
			null,
			null,
			"Stable or byre door with built-in lock behaviour."
		);

		CreateItem(
			"medieval_door_shared_lockable_store_room_door",
			"door",
			"a lockable storeroom door",
			null,
			"This lockable storeroom door is a large, well-made door built from oak boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.Large,
			ItemQuality.Good,
			40000.0,
			142.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Door_Lockable_Secure_Large"
			],
			null,
			null,
			null,
			null,
			"Storeroom door with built-in locking behaviour."
		);

		CreateItem(
			"medieval_door_shared_lockable_tower_stair_door",
			"door",
			"a lockable tower stair door",
			null,
			"This lockable tower stair door is a large, well-made door built from oak boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.Large,
			ItemQuality.Good,
			48000.0,
			185.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Door_Lockable_Secure_Large"
			],
			null,
			null,
			null,
			null,
			"Tower or stair door with built-in lock behaviour."
		);

		CreateItem(
			"medieval_door_shared_lockable_town_gate_leaf",
			"gate",
			"a lockable town-gate leaf",
			null,
			"This lockable town-gate leaf is a huge, well-made gate built from oak boards. Vertical pales sit inside a braced frame, leaving open gaps between the timbers. The latch side is squared to meet a post cleanly. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.Huge,
			ItemQuality.Good,
			142000.0,
			540.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Gate_Lockable_Tough_Huge"
			],
			null,
			null,
			null,
			null,
			"Huge built-in-lock gate leaf for large entrances."
		);

		CreateItem(
			"medieval_door_shared_lockable_warehouse_door",
			"door",
			"a lockable warehouse door",
			null,
			"This lockable warehouse door is a very large, well-made door built from oak boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.VeryLarge,
			ItemQuality.Good,
			67500.0,
			225.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Door_Lockable_Normal_VeryLarge"
			],
			null,
			null,
			null,
			null,
			"Large warehouse door with built-in lock behaviour."
		);

		CreateItem(
			"medieval_door_shared_lockable_wicket_gate",
			"gate",
			"a lockable wicket gate",
			null,
			"This lockable wicket gate is a large, well-made gate built from ash boards. Vertical pales sit inside a braced frame, leaving open gaps between the timbers. The latch side is squared to meet a post cleanly. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.Large,
			ItemQuality.Good,
			29000.0,
			88.0m,
			true,
			false,
			"ash",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Gate_Lockable_Normal_Large"
			],
			null,
			null,
			null,
			null,
			"Small lockable gate for yards, gardens, and side entries."
		);

		CreateItem(
			"medieval_door_shared_lockable_yard_gate",
			"gate",
			"a lockable yard gate",
			null,
			"This lockable yard gate is a very large, well-made gate built from oak boards. Vertical pales sit inside a braced frame, leaving open gaps between the timbers. The latch side is squared to meet a post cleanly. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.VeryLarge,
			ItemQuality.Good,
			56000.0,
			170.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Gate_Lockable_Secure_VeryLarge"
			],
			null,
			null,
			null,
			null,
			"Built-in-lock yard gate with hinge-side security behaviour."
		);

		CreateItem(
			"medieval_door_shared_mill_door",
			"door",
			"a millhouse door",
			null,
			"This millhouse door is a large, workmanlike door built from beech boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.Large,
			ItemQuality.Standard,
			36000.0,
			82.0m,
			true,
			false,
			"beech",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Door_Normal_Large"
			],
			null,
			null,
			null,
			null,
			"Working door for a millhouse, brewhouse, or comparable industrial building."
		);

		CreateItem(
			"medieval_door_shared_palisade_gate",
			"gate",
			"a palisade gate",
			null,
			"This palisade gate is a very large, workmanlike gate built from oak boards. Vertical pales sit inside a braced frame, leaving open gaps between the timbers. The latch side is squared to meet a post cleanly. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.VeryLarge,
			ItemQuality.Standard,
			70000.0,
			160.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Gate_Normal_VeryLarge"
			],
			null,
			null,
			null,
			null,
			"Large timber gate for palisades, livestock yards, and enclosed compounds."
		);

		CreateItem(
			"medieval_door_shared_pantry_door",
			"door",
			"a narrow pantry door",
			null,
			"This narrow pantry door is a large, workmanlike door built from ash boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.Large,
			ItemQuality.Standard,
			23000.0,
			50.0m,
			true,
			false,
			"ash",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Door_Normal_Large"
			],
			null,
			null,
			null,
			null,
			"Practical household pantry or buttery door without built-in lock behaviour."
		);

		CreateItem(
			"medieval_door_shared_plank_household_door",
			"door",
			"a plank household door",
			null,
			"This plank household door is a large, workmanlike door built from oak boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.Large,
			ItemQuality.Standard,
			36000.0,
			80.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Door_Normal_Large"
			],
			null,
			null,
			null,
			null,
			"Common exterior household door with board-and-batten construction and no built-in lock behaviour."
		);

		CreateItem(
			"medieval_door_shared_portcullis_grate",
			"grate",
			"a portcullis-like grate",
			null,
			"This portcullis-like grate is a huge, well-made grate worked from wrought iron. Iron bars cross the frame in a regular grid, leaving open spaces between them. The hinge side is heavier than the closing side. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.Huge,
			ItemQuality.Good,
			160000.0,
			620.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Gate_Tough_Huge"
			],
			null,
			null,
			null,
			null,
			"Huge barred gate profile that can be seen and fired through."
		);

		CreateItem(
			"medieval_door_shared_postern_gate",
			"gate",
			"a postern gate",
			null,
			"This postern gate is a very large, well-made gate built from oak boards. Vertical pales sit inside a braced frame, leaving open gaps between the timbers. The latch side is squared to meet a post cleanly. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.VeryLarge,
			ItemQuality.Good,
			65000.0,
			175.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Gate_Secure_VeryLarge"
			],
			null,
			null,
			null,
			null,
			"Secure small outer gate for side passages and wall openings."
		);

		CreateItem(
			"medieval_door_shared_poultry_yard_gate",
			"gate",
			"a poultry yard gate",
			null,
			"This poultry yard gate is a large, plain gate built from willow boards. Vertical pales sit inside a braced frame, leaving open gaps between the timbers. The latch side is squared to meet a post cleanly. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.Large,
			ItemQuality.Poor,
			12000.0,
			14.0m,
			true,
			false,
			"willow",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Gate_Bad_Large"
			],
			null,
			null,
			null,
			null,
			"Light weak gate for poultry yards and small garden enclosures."
		);

		CreateItem(
			"medieval_door_shared_safe_room_door",
			"door",
			"a safe-room door",
			null,
			"This safe-room door is a large, finely made door built from oak boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.Large,
			ItemQuality.VeryGood,
			70000.0,
			310.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Door_Lockable_Tough_Large"
			],
			null,
			null,
			null,
			null,
			"Very sturdy lockable door for treasuries, record rooms, and strong rooms."
		);

		CreateItem(
			"medieval_door_shared_shop_door",
			"door",
			"a shopfront plank door",
			null,
			"This shopfront plank door is a large, workmanlike door built from elm boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.Large,
			ItemQuality.Standard,
			34000.0,
			76.0m,
			true,
			false,
			"elm",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Door_Normal_Large"
			],
			null,
			null,
			null,
			null,
			"Plain shopfront or workshop door without built-in lock behaviour."
		);

		CreateItem(
			"medieval_door_shared_simple_cottage_door",
			"door",
			"a simple cottage door",
			null,
			"This simple cottage door is a large, workmanlike door built from pine boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.Large,
			ItemQuality.Standard,
			28000.0,
			52.0m,
			true,
			false,
			"pine",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Door_Normal_Large"
			],
			null,
			null,
			null,
			null,
			"Plain plank door for cottages, sheds, and modest outbuildings."
		);

		CreateItem(
			"medieval_door_shared_stable_door",
			"door",
			"a stable door",
			null,
			"This stable door is a large, workmanlike door built from oak boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.Large,
			ItemQuality.Standard,
			38000.0,
			90.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Door_Normal_Large"
			],
			null,
			null,
			null,
			null,
			"Plain stable or byre door without built-in lock behaviour."
		);

		CreateItem(
			"medieval_door_shared_stairwell_grate",
			"grate",
			"a stairwell security grate",
			null,
			"This stairwell security grate is a large, well-made grate worked from wrought iron. Iron bars cross the frame in a regular grid, leaving open spaces between them. The hinge side is heavier than the closing side. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.Large,
			ItemQuality.Good,
			48000.0,
			205.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Gate_Secure_Large"
			],
			null,
			null,
			null,
			null,
			"Secure iron grate for stairwells, cellars, or narrow passages."
		);

		CreateItem(
			"medieval_door_shared_store_room_door",
			"door",
			"a storeroom door",
			null,
			"This storeroom door is a large, workmanlike door built from oak boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.Large,
			ItemQuality.Standard,
			37000.0,
			88.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Door_Secure_Large"
			],
			null,
			null,
			null,
			null,
			"Secure storeroom door intended for pantries, workshops, and storage rooms."
		);

		CreateItem(
			"medieval_door_shared_tower_stair_door",
			"door",
			"a tower stair door",
			null,
			"This tower stair door is a large, well-made door built from oak boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.Large,
			ItemQuality.Good,
			45000.0,
			125.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Door_Secure_Large"
			],
			null,
			null,
			null,
			null,
			"Narrow secure door suitable for towers, stair passages, and defended inner rooms."
		);

		CreateItem(
			"medieval_door_shared_town_gate_leaf",
			"gate",
			"a town-gate leaf",
			null,
			"This town-gate leaf is a huge, well-made gate built from oak boards. Vertical pales sit inside a braced frame, leaving open gaps between the timbers. The latch side is squared to meet a post cleanly. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.Huge,
			ItemQuality.Good,
			135000.0,
			420.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Gate_Tough_Huge"
			],
			null,
			null,
			null,
			null,
			"Huge tough gate leaf for town, castle, or compound entrances."
		);

		CreateItem(
			"medieval_door_shared_treasure_room_door",
			"door",
			"a treasure-room door",
			null,
			"This treasure-room door is a large, finely made door built from oak boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.Large,
			ItemQuality.VeryGood,
			76000.0,
			360.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Door_Lockable_Tough_Large"
			],
			null,
			null,
			null,
			null,
			"Heavy built-in-lock door for elite or institutional secure storage rooms."
		);

		CreateItem(
			"medieval_door_shared_warehouse_door",
			"door",
			"a broad warehouse door",
			null,
			"This broad warehouse door is a very large, workmanlike door built from oak boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.VeryLarge,
			ItemQuality.Standard,
			62000.0,
			150.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Door_Normal_VeryLarge"
			],
			null,
			null,
			null,
			null,
			"Broad movable warehouse door for stores, barns, and industrial buildings."
		);

		CreateItem(
			"medieval_door_shared_wicket_gate",
			"gate",
			"a narrow wicket gate",
			null,
			"This narrow wicket gate is a large, workmanlike gate built from ash boards. Vertical pales sit inside a braced frame, leaving open gaps between the timbers. The latch side is squared to meet a post cleanly. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.Large,
			ItemQuality.Standard,
			26000.0,
			52.0m,
			true,
			false,
			"ash",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Gate_Normal_Large"
			],
			null,
			null,
			null,
			null,
			"Small pass-through gate for yards, gardens, and fenced closes."
		);

		CreateItem(
			"medieval_door_shared_window_grate",
			"grate",
			"a barred window grate",
			null,
			"This barred window grate is a large, workmanlike grate worked from wrought iron. Iron bars cross the frame in a regular grid, leaving open spaces between them. The hinge side is heavier than the closing side. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.Large,
			ItemQuality.Standard,
			30000.0,
			120.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Gate_Normal_Large"
			],
			null,
			null,
			null,
			null,
			"Portable barred grate profile for large window or service openings."
		);

		CreateItem(
			"medieval_door_shared_wool_doorway_hanging",
			"hanging",
			"a wool doorway hanging",
			null,
			"This wool doorway hanging is a large, workmanlike hanging made from woven wool. Heavy fabric hangs from a reinforced top edge, falling in a single soft sheet. The lower hem is weighted so it settles back after being pushed aside. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.Large,
			ItemQuality.Standard,
			2500.0,
			24.0m,
			true,
			false,
			"wool",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Door_Bad_Large"
			],
			null,
			null,
			null,
			null,
			"Soft doorway barrier; weak door behaviour represents a non-secure hanging."
		);

		CreateItem(
			"medieval_door_shared_wrought_iron_grate",
			"grate",
			"a wrought iron barred grate",
			null,
			"This wrought iron barred grate is a large, well-made grate worked from wrought iron. Iron bars cross the frame in a regular grid, leaving open spaces between them. The hinge side is heavier than the closing side. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.Large,
			ItemQuality.Good,
			45000.0,
			190.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Gate_Tough_Large"
			],
			null,
			null,
			null,
			null,
			"Barred grate using gate behaviour so sight and ranged fire can pass through."
		);

		CreateItem(
			"medieval_door_religious_religious_chapel_door",
			"door",
			"a chapel side door",
			null,
			"This chapel side door is a large, well-made door built from oak boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.Large,
			ItemQuality.Good,
			43000.0,
			135.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Door_Normal_Large"
			],
			null,
			null,
			null,
			null,
			"Sturdy side door suitable for chapels, shrines, and small sanctuaries."
		);

		CreateItem(
			"medieval_door_religious_religious_lockable_chapel_door",
			"door",
			"a lockable chapel side door",
			null,
			"This lockable chapel side door is a large, well-made door built from oak boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.Large,
			ItemQuality.Good,
			46000.0,
			195.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Door_Lockable_Normal_Large"
			],
			null,
			null,
			null,
			null,
			"Side door for a sanctuary or religious building with built-in lock behaviour."
		);

		CreateItem(
			"medieval_door_religious_religious_screen_gate",
			"gate",
			"a sanctuary screen gate",
			null,
			"This sanctuary screen gate is a large, well-made gate worked from wrought iron. Thin slats are bound into a flat screen, with a top rail for hanging and a lightly weighted lower edge. The gaps are narrow but still visible. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.Large,
			ItemQuality.Good,
			40000.0,
			220.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Gate_Lockable_Secure_Large"
			],
			null,
			null,
			null,
			null,
			"Lockable open gate for separating sanctuary, choir, or inner precinct spaces."
		);

		CreateItem(
			"medieval_door_religious_religious_vestry_door",
			"door",
			"a vestry store door",
			null,
			"This vestry store door is a large, well-made door built from oak boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.Large,
			ItemQuality.Good,
			41000.0,
			145.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Door_Lockable_Secure_Large"
			],
			null,
			null,
			null,
			null,
			"Secure built-in-lock door for vestment rooms, scripture storage, or treasury-adjacent rooms."
		);

		CreateItem(
			"medieval_door_northern_north_sea_northern_boathouse_door",
			"door",
			"a pine boathouse door",
			null,
			"This pine boathouse door is a very large, workmanlike door built from pine boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.VeryLarge,
			ItemQuality.Standard,
			54000.0,
			118.0m,
			true,
			false,
			"pine",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Door_Normal_VeryLarge"
			],
			null,
			null,
			null,
			null,
			"Wide timber door for boathouses and shore sheds."
		);

		CreateItem(
			"medieval_door_northern_north_sea_northern_forge_grate",
			"grate",
			"a forge-yard iron grate",
			null,
			"This forge-yard iron grate is a large, well-made grate worked from wrought iron. Iron bars cross the frame in a regular grid, leaving open spaces between them. The hinge side is heavier than the closing side. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.Large,
			ItemQuality.Good,
			56000.0,
			230.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Gate_Tough_Large"
			],
			null,
			null,
			null,
			null,
			"Rugged iron grate for forge yards and workshops."
		);

		CreateItem(
			"medieval_door_northern_north_sea_northern_fur_door_flap",
			"flap",
			"a heavy fur door flap",
			null,
			"This heavy fur door flap is a large, workmanlike flap made from dressed fur. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.Large,
			ItemQuality.Standard,
			5200.0,
			55.0m,
			true,
			false,
			"fur",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Door_Bad_Large"
			],
			null,
			null,
			null,
			null,
			"Cold-weather door flap with weak door behaviour."
		);

		CreateItem(
			"medieval_door_northern_north_sea_northern_lockable_palisade_gate",
			"gate",
			"a lockable palisade gate",
			null,
			"This lockable palisade gate is a very large, well-made gate built from oak boards. Vertical pales sit inside a braced frame, leaving open gaps between the timbers. The latch side is squared to meet a post cleanly. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.VeryLarge,
			ItemQuality.Good,
			84000.0,
			260.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Gate_Lockable_Secure_VeryLarge"
			],
			null,
			null,
			null,
			null,
			"Large lockable palisade gate."
		);

		CreateItem(
			"medieval_door_northern_north_sea_northern_lockable_stave_door",
			"door",
			"a lockable stave-built door",
			null,
			"This lockable stave-built door is a large, well-made door built from pine boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.Large,
			ItemQuality.Good,
			36000.0,
			124.0m,
			true,
			false,
			"pine",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Door_Lockable_Normal_Large"
			],
			null,
			null,
			null,
			null,
			"Stave-built household door with built-in lock behaviour."
		);

		CreateItem(
			"medieval_door_northern_north_sea_northern_longhouse_hall_door",
			"door",
			"a longhouse hall door",
			null,
			"This longhouse hall door is a large, well-made door built from oak boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.Large,
			ItemQuality.Good,
			52000.0,
			165.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Door_Tough_Large"
			],
			null,
			null,
			null,
			null,
			"Heavy hall door for longhouses and large timber halls."
		);

		CreateItem(
			"medieval_door_northern_north_sea_northern_palisade_gate",
			"gate",
			"a spiked palisade gate",
			null,
			"This spiked palisade gate is a very large, workmanlike gate built from oak boards. Vertical pales sit inside a braced frame, leaving open gaps between the timbers. The latch side is squared to meet a post cleanly. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.VeryLarge,
			ItemQuality.Standard,
			78000.0,
			180.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Gate_Normal_VeryLarge"
			],
			null,
			null,
			null,
			null,
			"Large timber gate for palisaded yards and enclosures."
		);

		CreateItem(
			"medieval_door_northern_north_sea_northern_stave_house_door",
			"door",
			"a stave-built house door",
			null,
			"This stave-built house door is a large, workmanlike door built from pine boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.Large,
			ItemQuality.Standard,
			33000.0,
			74.0m,
			true,
			false,
			"pine",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Door_Normal_Large"
			],
			null,
			null,
			null,
			null,
			"Stave-built northern household door."
		);

		CreateItem(
			"medieval_door_western_european_western_arched_oak_door",
			"door",
			"an arched oak door",
			null,
			"This arched oak door is a large, well-made door built from oak boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.Large,
			ItemQuality.Good,
			47000.0,
			145.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Door_Normal_Large"
			],
			null,
			null,
			null,
			null,
			"Arched plank door suitable for stone or timber-framed buildings."
		);

		CreateItem(
			"medieval_door_western_european_western_castle_postern",
			"gate",
			"a castle postern gate",
			null,
			"This castle postern gate is a very large, well-made gate built from oak boards. Vertical pales sit inside a braced frame, leaving open gaps between the timbers. The latch side is squared to meet a post cleanly. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.VeryLarge,
			ItemQuality.Good,
			90000.0,
			340.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Gate_Tough_VeryLarge"
			],
			null,
			null,
			null,
			null,
			"Heavy side gate suitable for fortified walls and closes."
		);

		CreateItem(
			"medieval_door_western_european_western_churchyard_grille",
			"grate",
			"a churchyard iron grille",
			null,
			"This churchyard iron grille is a very large, well-made grate worked from wrought iron. Iron bars cross the frame in a regular grid, leaving open spaces between them. The hinge side is heavier than the closing side. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.VeryLarge,
			ItemQuality.Good,
			78000.0,
			330.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Gate_Tough_VeryLarge"
			],
			null,
			null,
			null,
			null,
			"Large iron grille usable in churchyards, cloisters, or enclosed courts."
		);

		CreateItem(
			"medieval_door_western_european_western_half_timber_shop_door",
			"door",
			"a half-timber shop door",
			null,
			"This half-timber shop door is a large, workmanlike door built from elm boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.Large,
			ItemQuality.Standard,
			34000.0,
			82.0m,
			true,
			false,
			"elm",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Door_Normal_Large"
			],
			null,
			null,
			null,
			null,
			"Shop door for high-street, market, or craft-front buildings."
		);

		CreateItem(
			"medieval_door_western_european_western_lockable_arched_oak_door",
			"door",
			"a lockable arched oak door",
			null,
			"This lockable arched oak door is a large, well-made door built from oak boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.Large,
			ItemQuality.Good,
			50500.0,
			205.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Door_Lockable_Normal_Large"
			],
			null,
			null,
			null,
			null,
			"Arched oak door with built-in lock behaviour."
		);

		CreateItem(
			"medieval_door_western_european_western_lockable_manor_gate",
			"gate",
			"a lockable manorial gate",
			null,
			"This lockable manorial gate is a very large, well-made gate built from oak boards. Vertical pales sit inside a braced frame, leaving open gaps between the timbers. The latch side is squared to meet a post cleanly. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.VeryLarge,
			ItemQuality.Good,
			82000.0,
			310.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Gate_Lockable_Secure_VeryLarge"
			],
			null,
			null,
			null,
			null,
			"Built-in-lock gate for manorial yards and compounds."
		);

		CreateItem(
			"medieval_door_western_european_western_manor_gate",
			"gate",
			"a manorial yard gate",
			null,
			"This manorial yard gate is a very large, well-made gate built from oak boards. Vertical pales sit inside a braced frame, leaving open gaps between the timbers. The latch side is squared to meet a post cleanly. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.VeryLarge,
			ItemQuality.Good,
			76000.0,
			220.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Gate_Secure_VeryLarge"
			],
			null,
			null,
			null,
			null,
			"Stout manorial yard gate without built-in lock behaviour."
		);

		CreateItem(
			"medieval_door_western_european_western_vault_door",
			"door",
			"a stone-vault strong door",
			null,
			"This stone-vault strong door is a large, finely made door built from oak boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.Large,
			ItemQuality.VeryGood,
			76000.0,
			360.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Door_Lockable_Tough_Large"
			],
			null,
			null,
			null,
			null,
			"Heavy lockable door for vaults, strongrooms, and record chambers."
		);

		CreateItem(
			"medieval_door_mediterranean_mediterranean_bead_door_hanging",
			"hanging",
			"a shell-bead doorway hanging",
			null,
			"This shell-bead doorway hanging is a large, workmanlike hanging worked from bone. Strings of beads hang from a narrow header, leaving small gaps between each strand. The lower ends are uneven from movement through the passage. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.Large,
			ItemQuality.Standard,
			2600.0,
			36.0m,
			true,
			false,
			"bone",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Door_Bad_Large"
			],
			null,
			null,
			null,
			null,
			"Beaded doorway hanging for warm interiors and shaded rooms."
		);

		CreateItem(
			"medieval_door_mediterranean_mediterranean_bronze_grille",
			"grate",
			"a bronze barred grille",
			null,
			"This bronze barred grille is a large, well-made grate worked from bronze. Iron bars cross the frame in a regular grid, leaving open spaces between them. The hinge side is heavier than the closing side. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.Large,
			ItemQuality.Good,
			42000.0,
			260.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Gate_Tough_Large"
			],
			null,
			null,
			null,
			null,
			"Bronze grille for courtyards, store rooms, or service openings."
		);

		CreateItem(
			"medieval_door_mediterranean_mediterranean_bronze_studded_door",
			"door",
			"a bronze-studded door",
			null,
			"This bronze-studded door is a large, well-made door built from cypress boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.Large,
			ItemQuality.Good,
			50000.0,
			220.0m,
			true,
			false,
			"cypress",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Door_Tough_Large"
			],
			null,
			null,
			null,
			null,
			"Sturdy cypress door with visible bronze studs and strapping."
		);

		CreateItem(
			"medieval_door_mediterranean_mediterranean_cypress_courtyard_door",
			"door",
			"a cypress courtyard door",
			null,
			"This cypress courtyard door is a large, well-made door built from cypress boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.Large,
			ItemQuality.Good,
			39000.0,
			130.0m,
			true,
			false,
			"cypress",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Door_Normal_Large"
			],
			null,
			null,
			null,
			null,
			"Cypress-plank door for courtyards and warm-climate houses."
		);

		CreateItem(
			"medieval_door_mediterranean_mediterranean_lockable_cypress_door",
			"door",
			"a lockable cypress courtyard door",
			null,
			"This lockable cypress courtyard door is a large, well-made door built from cypress boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.Large,
			ItemQuality.Good,
			42000.0,
			190.0m,
			true,
			false,
			"cypress",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Door_Lockable_Normal_Large"
			],
			null,
			null,
			null,
			null,
			"Cypress courtyard door with built-in lock behaviour."
		);

		CreateItem(
			"medieval_door_mediterranean_mediterranean_lockable_villa_gate",
			"gate",
			"a lockable villa gate",
			null,
			"This lockable villa gate is a very large, well-made gate built from cypress boards. Vertical pales sit inside a braced frame, leaving open gaps between the timbers. The latch side is squared to meet a post cleanly. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.VeryLarge,
			ItemQuality.Good,
			67500.0,
			300.0m,
			true,
			false,
			"cypress",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Gate_Lockable_Normal_VeryLarge"
			],
			null,
			null,
			null,
			null,
			"Courtyard gate with built-in lock behaviour."
		);

		CreateItem(
			"medieval_door_mediterranean_mediterranean_villa_gate",
			"gate",
			"a villa courtyard gate",
			null,
			"This villa courtyard gate is a very large, well-made gate built from cypress boards. Vertical pales sit inside a braced frame, leaving open gaps between the timbers. The latch side is squared to meet a post cleanly. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.VeryLarge,
			ItemQuality.Good,
			62000.0,
			210.0m,
			true,
			false,
			"cypress",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Gate_Normal_VeryLarge"
			],
			null,
			null,
			null,
			null,
			"Courtyard gate for villa, bath, or urban compound settings."
		);

		CreateItem(
			"medieval_door_mediterranean_mediterranean_warehouse_gate",
			"gate",
			"a harbourside warehouse gate",
			null,
			"This harbourside warehouse gate is a very large, workmanlike gate built from oak boards. Vertical pales sit inside a braced frame, leaving open gaps between the timbers. The latch side is squared to meet a post cleanly. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.VeryLarge,
			ItemQuality.Standard,
			72000.0,
			175.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Gate_Secure_VeryLarge"
			],
			null,
			null,
			null,
			null,
			"Large gate for harbourside stores and warehouse yards."
		);

		CreateItem(
			"medieval_door_islamicate_urban_islamicate_bazaar_grille",
			"grate",
			"a bazaar shop grille",
			null,
			"This bazaar shop grille is a large, well-made grate worked from wrought iron. Iron bars cross the frame in a regular grid, leaving open spaces between them. The hinge side is heavier than the closing side. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.Large,
			ItemQuality.Good,
			48000.0,
			225.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Gate_Lockable_Secure_Large"
			],
			null,
			null,
			null,
			null,
			"Lockable barred grille for market stalls and shops."
		);

		CreateItem(
			"medieval_door_islamicate_urban_islamicate_bronze_studded_gate",
			"gate",
			"a bronze-studded compound gate",
			null,
			"This bronze-studded compound gate is a very large, well-made gate built from cedar boards. Vertical pales sit inside a braced frame, leaving open gaps between the timbers. The latch side is squared to meet a post cleanly. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.VeryLarge,
			ItemQuality.Good,
			76000.0,
			285.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Gate_Tough_VeryLarge"
			],
			null,
			null,
			null,
			null,
			"Large compound gate with bronze-studded visual treatment."
		);

		CreateItem(
			"medieval_door_islamicate_urban_islamicate_carved_cedar_door",
			"door",
			"a carved cedar courtyard door",
			null,
			"This carved cedar courtyard door is a large, well-made door built from cedar boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.Large,
			ItemQuality.Good,
			39000.0,
			155.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Door_Normal_Large"
			],
			null,
			null,
			null,
			null,
			"Carved cedar door for urban houses, courtyards, and workshops."
		);

		CreateItem(
			"medieval_door_islamicate_urban_islamicate_cotton_door_curtain",
			"curtain",
			"a cotton doorway curtain",
			null,
			"This cotton doorway curtain is a large, workmanlike curtain made from woven cotton. Heavy fabric hangs from a reinforced top edge, falling in a single soft sheet. The lower hem is weighted so it settles back after being pushed aside. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.Large,
			ItemQuality.Standard,
			1500.0,
			22.0m,
			true,
			false,
			"cotton",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Door_Bad_Large"
			],
			null,
			null,
			null,
			null,
			"Light cotton doorway barrier for interiors and screened passages."
		);

		CreateItem(
			"medieval_door_islamicate_urban_islamicate_lattice_screen_door",
			"screen",
			"a carved lattice screen door",
			null,
			"This carved lattice screen door is a large, well-made screen built from cedar boards. Thin slats are bound into a flat screen, with a top rail for hanging and a lightly weighted lower edge. The gaps are narrow but still visible. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.Large,
			ItemQuality.Good,
			19000.0,
			110.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Gate_Normal_Large"
			],
			null,
			null,
			null,
			null,
			"Open lattice screen using gate behaviour so sight and missiles can pass through."
		);

		CreateItem(
			"medieval_door_islamicate_urban_islamicate_lockable_cedar_door",
			"door",
			"a lockable carved cedar door",
			null,
			"This lockable carved cedar door is a large, well-made door built from cedar boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.Large,
			ItemQuality.Good,
			42000.0,
			215.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Door_Lockable_Normal_Large"
			],
			null,
			null,
			null,
			null,
			"Carved cedar door with built-in lock behaviour."
		);

		CreateItem(
			"medieval_door_islamicate_urban_islamicate_lockable_compound_gate",
			"gate",
			"a lockable compound gate",
			null,
			"This lockable compound gate is a very large, well-made gate built from cedar boards. Vertical pales sit inside a braced frame, leaving open gaps between the timbers. The latch side is squared to meet a post cleanly. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.VeryLarge,
			ItemQuality.Good,
			82000.0,
			380.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Gate_Lockable_Tough_VeryLarge"
			],
			null,
			null,
			null,
			null,
			"Compound gate with built-in lock behaviour."
		);

		CreateItem(
			"medieval_door_islamicate_urban_islamicate_storehouse_door",
			"door",
			"a cedar storehouse door",
			null,
			"This cedar storehouse door is a large, well-made door built from cedar boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.Large,
			ItemQuality.Good,
			43000.0,
			145.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Door_Secure_Large"
			],
			null,
			null,
			null,
			null,
			"Secure storehouse door without built-in lock behaviour."
		);

		CreateItem(
			"medieval_door_south_asian_south_asian_bamboo_screen_door",
			"screen",
			"a woven bamboo screen door",
			null,
			"This woven bamboo screen door is a large, workmanlike screen built from split bamboo. Thin slats are bound into a flat screen, with a top rail for hanging and a lightly weighted lower edge. The gaps are narrow but still visible. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.Large,
			ItemQuality.Standard,
			6500.0,
			40.0m,
			true,
			false,
			"bamboo",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Door_Bad_Large"
			],
			null,
			null,
			null,
			null,
			"Light bamboo screen for shaded domestic passages."
		);

		CreateItem(
			"medieval_door_south_asian_south_asian_bazaar_grille",
			"grate",
			"an iron bazaar grille",
			null,
			"This iron bazaar grille is a large, well-made grate worked from wrought iron. Iron bars cross the frame in a regular grid, leaving open spaces between them. The hinge side is heavier than the closing side. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.Large,
			ItemQuality.Good,
			50000.0,
			225.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Gate_Lockable_Secure_Large"
			],
			null,
			null,
			null,
			null,
			"Lockable open grille for bazaars, shops, and store counters."
		);

		CreateItem(
			"medieval_door_south_asian_south_asian_carved_teak_gate",
			"gate",
			"a carved teak compound gate",
			null,
			"This carved teak compound gate is a very large, well-made gate built from teak boards. Vertical pales sit inside a braced frame, leaving open gaps between the timbers. The latch side is squared to meet a post cleanly. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.VeryLarge,
			ItemQuality.Good,
			88000.0,
			360.0m,
			true,
			false,
			"teak",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Gate_Tough_VeryLarge"
			],
			null,
			null,
			null,
			null,
			"Carved teak compound gate without built-in lock behaviour."
		);

		CreateItem(
			"medieval_door_south_asian_south_asian_cotton_door_cloth",
			"hanging",
			"a cotton doorway cloth",
			null,
			"This cotton doorway cloth is a large, workmanlike hanging made from woven cotton. Heavy fabric hangs from a reinforced top edge, falling in a single soft sheet. The lower hem is weighted so it settles back after being pushed aside. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.Large,
			ItemQuality.Standard,
			1400.0,
			18.0m,
			true,
			false,
			"cotton",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Door_Bad_Large"
			],
			null,
			null,
			null,
			null,
			"Simple cloth doorway barrier for warm interiors."
		);

		CreateItem(
			"medieval_door_south_asian_south_asian_lockable_teak_door",
			"door",
			"a lockable teak courtyard door",
			null,
			"This lockable teak courtyard door is a large, well-made door built from teak boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.Large,
			ItemQuality.Good,
			48500.0,
			260.0m,
			true,
			false,
			"teak",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Door_Lockable_Normal_Large"
			],
			null,
			null,
			null,
			null,
			"Teak courtyard door with built-in lock behaviour."
		);

		CreateItem(
			"medieval_door_south_asian_south_asian_lockable_teak_gate",
			"gate",
			"a lockable carved teak gate",
			null,
			"This lockable carved teak gate is a very large, well-made gate built from teak boards. Vertical pales sit inside a braced frame, leaving open gaps between the timbers. The latch side is squared to meet a post cleanly. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.VeryLarge,
			ItemQuality.Good,
			95000.0,
			470.0m,
			true,
			false,
			"teak",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Gate_Lockable_Tough_VeryLarge"
			],
			null,
			null,
			null,
			null,
			"Carved teak compound gate with built-in lock behaviour."
		);

		CreateItem(
			"medieval_door_south_asian_south_asian_stable_gate",
			"gate",
			"a teak stable gate",
			null,
			"This teak stable gate is a very large, workmanlike gate built from teak boards. Vertical pales sit inside a braced frame, leaving open gaps between the timbers. The latch side is squared to meet a post cleanly. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.VeryLarge,
			ItemQuality.Standard,
			76000.0,
			210.0m,
			true,
			false,
			"teak",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Gate_Normal_VeryLarge"
			],
			null,
			null,
			null,
			null,
			"Large teak gate for stables, compounds, or courtyards."
		);

		CreateItem(
			"medieval_door_south_asian_south_asian_teak_courtyard_door",
			"door",
			"a teak courtyard door",
			null,
			"This teak courtyard door is a large, well-made door built from teak boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.Large,
			ItemQuality.Good,
			45000.0,
			190.0m,
			true,
			false,
			"teak",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Door_Normal_Large"
			],
			null,
			null,
			null,
			null,
			"Strong teak door for courtyard houses and compounds."
		);

		CreateItem(
			"medieval_door_east_asian_east_asian_bamboo_lattice_gate",
			"gate",
			"a bamboo lattice gate",
			null,
			"This bamboo lattice gate is a large, workmanlike gate built from split bamboo. Vertical pales sit inside a braced frame, leaving open gaps between the timbers. The latch side is squared to meet a post cleanly. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.Large,
			ItemQuality.Standard,
			11000.0,
			48.0m,
			true,
			false,
			"bamboo",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Gate_Normal_Large"
			],
			null,
			null,
			null,
			null,
			"Open bamboo lattice gate using see-and-fire-through gate behaviour."
		);

		CreateItem(
			"medieval_door_east_asian_east_asian_compound_gate",
			"gate",
			"a timber compound gate",
			null,
			"This timber compound gate is a very large, well-made gate built from cypress boards. Vertical pales sit inside a braced frame, leaving open gaps between the timbers. The latch side is squared to meet a post cleanly. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.VeryLarge,
			ItemQuality.Good,
			78000.0,
			260.0m,
			true,
			false,
			"cypress",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Gate_Normal_VeryLarge"
			],
			null,
			null,
			null,
			null,
			"Large timber gate for compounds and yards."
		);

		CreateItem(
			"medieval_door_east_asian_east_asian_iron_grille",
			"grate",
			"an iron lattice grille",
			null,
			"This iron lattice grille is a large, well-made grate worked from wrought iron. Iron bars cross the frame in a regular grid, leaving open spaces between them. The hinge side is heavier than the closing side. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.Large,
			ItemQuality.Good,
			42000.0,
			205.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Gate_Tough_Large"
			],
			null,
			null,
			null,
			null,
			"Open iron grille using gate behaviour."
		);

		CreateItem(
			"medieval_door_east_asian_east_asian_lockable_panel_door",
			"door",
			"a lockable pine panel door",
			null,
			"This lockable pine panel door is a large, well-made door built from pine boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.Large,
			ItemQuality.Good,
			28000.0,
			120.0m,
			true,
			false,
			"pine",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Door_Lockable_Normal_Large"
			],
			null,
			null,
			null,
			null,
			"Paneled pine door with built-in lock behaviour."
		);

		CreateItem(
			"medieval_door_east_asian_east_asian_lockable_storehouse_door",
			"door",
			"a lockable storehouse door",
			null,
			"This lockable storehouse door is a large, well-made door built from cypress boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.Large,
			ItemQuality.Good,
			50000.0,
			215.0m,
			true,
			false,
			"cypress",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Door_Lockable_Secure_Large"
			],
			null,
			null,
			null,
			null,
			"Storehouse door with built-in lock behaviour."
		);

		CreateItem(
			"medieval_door_east_asian_east_asian_paper_screen_door",
			"screen",
			"a paper screen door",
			null,
			"This paper screen door is a large, workmanlike screen made from layered paper. Thin slats are bound into a flat screen, with a top rail for hanging and a lightly weighted lower edge. The gaps are narrow but still visible. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.Large,
			ItemQuality.Standard,
			2200.0,
			32.0m,
			true,
			false,
			"paper",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Door_Bad_Large"
			],
			null,
			null,
			null,
			null,
			"Delicate screen-door prototype with weak door behaviour."
		);

		CreateItem(
			"medieval_door_east_asian_east_asian_pine_panel_door",
			"door",
			"a pine panel door",
			null,
			"This pine panel door is a large, workmanlike door built from pine boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.Large,
			ItemQuality.Standard,
			25000.0,
			70.0m,
			true,
			false,
			"pine",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Door_Normal_Large"
			],
			null,
			null,
			null,
			null,
			"Light paneled wooden door for domestic interiors and shops."
		);

		CreateItem(
			"medieval_door_east_asian_east_asian_storehouse_door",
			"door",
			"a heavy storehouse door",
			null,
			"This heavy storehouse door is a large, well-made door built from cypress boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.Large,
			ItemQuality.Good,
			46000.0,
			150.0m,
			true,
			false,
			"cypress",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Door_Secure_Large"
			],
			null,
			null,
			null,
			null,
			"Secure storehouse door for warehouses, compounds, or estate storage."
		);

		CreateItem(
			"medieval_door_steppe_and_caravan_steppe_caravanserai_gate",
			"gate",
			"a caravanserai yard gate",
			null,
			"This caravanserai yard gate is a very large, well-made gate built from oak boards. Vertical pales sit inside a braced frame, leaving open gaps between the timbers. The latch side is squared to meet a post cleanly. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.VeryLarge,
			ItemQuality.Good,
			84000.0,
			280.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Gate_Secure_VeryLarge"
			],
			null,
			null,
			null,
			null,
			"Large secure gate for caravan yards and enclosed compounds."
		);

		CreateItem(
			"medieval_door_steppe_and_caravan_steppe_corral_gate",
			"gate",
			"a horse-corral gate",
			null,
			"This horse-corral gate is a very large, workmanlike gate built from ash boards. Vertical pales sit inside a braced frame, leaving open gaps between the timbers. The latch side is squared to meet a post cleanly. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.VeryLarge,
			ItemQuality.Standard,
			52000.0,
			105.0m,
			true,
			false,
			"ash",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Gate_Normal_VeryLarge"
			],
			null,
			null,
			null,
			null,
			"Broad gate for horse corrals and animal yards."
		);

		CreateItem(
			"medieval_door_steppe_and_caravan_steppe_felt_door_flap",
			"flap",
			"a felt tent door flap",
			null,
			"This felt tent door flap is a large, workmanlike flap made from pressed felt. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.Large,
			ItemQuality.Standard,
			2800.0,
			24.0m,
			true,
			false,
			"felt",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Door_Bad_Large"
			],
			null,
			null,
			null,
			null,
			"Felt door flap for tents, wagons, or temporary shelters."
		);

		CreateItem(
			"medieval_door_steppe_and_caravan_steppe_iron_barrier_grate",
			"grate",
			"a wrought iron barrier grate",
			null,
			"This wrought iron barrier grate is a large, well-made grate worked from wrought iron. Iron bars cross the frame in a regular grid, leaving open spaces between them. The hinge side is heavier than the closing side. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.Large,
			ItemQuality.Good,
			52000.0,
			230.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Gate_Tough_Large"
			],
			null,
			null,
			null,
			null,
			"Open iron barrier for stores, yards, and secure passages."
		);

		CreateItem(
			"medieval_door_steppe_and_caravan_steppe_lattice_door_panel",
			"screen",
			"a wooden lattice door panel",
			null,
			"This wooden lattice door panel is a large, workmanlike screen built from ash boards. Heavy fabric hangs from a reinforced top edge, falling in a single soft sheet. The lower hem is weighted so it settles back after being pushed aside. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.Large,
			ItemQuality.Standard,
			15000.0,
			50.0m,
			true,
			false,
			"ash",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Gate_Normal_Large"
			],
			null,
			null,
			null,
			null,
			"Light lattice barrier represented with gate behaviour."
		);

		CreateItem(
			"medieval_door_steppe_and_caravan_steppe_leather_tent_flap",
			"flap",
			"a leather tent door flap",
			null,
			"This leather tent door flap is a large, workmanlike flap made from worked leather. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.Large,
			ItemQuality.Standard,
			3600.0,
			42.0m,
			true,
			false,
			"leather",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Door_Bad_Large"
			],
			null,
			null,
			null,
			null,
			"Leather door flap for tents and portable camps."
		);

		CreateItem(
			"medieval_door_steppe_and_caravan_steppe_lockable_caravanserai_gate",
			"gate",
			"a lockable caravanserai gate",
			null,
			"This lockable caravanserai gate is a very large, well-made gate built from oak boards. Vertical pales sit inside a braced frame, leaving open gaps between the timbers. The latch side is squared to meet a post cleanly. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.VeryLarge,
			ItemQuality.Good,
			90000.0,
			380.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Gate_Lockable_Secure_VeryLarge"
			],
			null,
			null,
			null,
			null,
			"Caravanserai gate with built-in lock behaviour."
		);

		CreateItem(
			"medieval_door_steppe_and_caravan_steppe_wagon_gate",
			"gate",
			"a plank wagon-yard gate",
			null,
			"This plank wagon-yard gate is a very large, workmanlike gate built from pine boards. Vertical pales sit inside a braced frame, leaving open gaps between the timbers. The latch side is squared to meet a post cleanly. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
			SizeCategory.VeryLarge,
			ItemQuality.Standard,
			58000.0,
			125.0m,
			true,
			false,
			"pine",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Construction Materials / Worked Timber"
			],
			[
				"Holdable",
				"Destroyable_Door",
				"Gate_Normal_VeryLarge"
			],
			null,
			null,
			null,
			null,
			"Wide gate for wagon yards and caravan enclosures."
		);

		CreateItem(
			"medieval_shared_bar_keeper_brackets",
			"bracket",
			"iron locking-bar brackets",
			null,
			"These iron locking-bar brackets are small, workmanlike bracket worked from wrought iron. Holes and bent edges show exactly how the hardware fixes against timber. The working face is plain, with bright wear around the contact points. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Small,
			ItemQuality.Standard,
			1100.0,
			18.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			"Inert brackets for a removable bar; they do not provide lock behaviour alone."
		);

		CreateItem(
			"medieval_shared_barrel_bit_key",
			"key",
			"a barrel-bit warded key",
			null,
			"This barrel-bit warded key is a tiny, workmanlike key worked from wrought iron. A shaped bow forms the handle, with a straight shaft and cut bit at the end. The bit shows simple wards and rubbed working edges. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			70.0,
			5.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Warded_Key"
			],
			null,
			null,
			null,
			null,
			"Key with a visibly thick bit and simple ward cuts."
		);

		CreateItem(
			"medieval_shared_brass_keyring",
			"keyring",
			"a brass keyring",
			null,
			"This brass keyring is a tiny, well-made keyring worked from brass. A split in the circular ring gives keys a clear point for loading. The surface is polished where metal has rubbed against metal. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			95.0,
			14.0m,
			true,
			false,
			"brass",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Keyring_Small"
			],
			null,
			null,
			null,
			null,
			"Small keyring made in a finer visible material."
		);

		CreateItem(
			"medieval_shared_broad_coffer_lock",
			"lock",
			"a broad coffer lock",
			null,
			"This broad coffer lock is a small, workmanlike lock worked from wrought iron. The lock has a flat faceplate, a visible keyhole, and a heavy shackle fitting. The back is thicker where the warded mechanism sits inside. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Small,
			ItemQuality.Standard,
			1100.0,
			30.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Warded_Lock_Normal"
			],
			null,
			null,
			null,
			null,
			"Broad lock face suitable for larger coffer lids and household storage chests."
		);

		CreateItem(
			"medieval_shared_bronze_keyring",
			"keyring",
			"a bronze keyring",
			null,
			"This bronze keyring is a tiny, workmanlike keyring worked from bronze. A split in the circular ring gives keys a clear point for loading. The surface is polished where metal has rubbed against metal. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			105.0,
			10.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Keyring_Small"
			],
			null,
			null,
			null,
			null,
			"Bronze keyring variant for household key bundles."
		);

		CreateItem(
			"medieval_shared_bronze_warded_key",
			"key",
			"a bronze warded key",
			null,
			"This bronze warded key is a tiny, workmanlike key worked from bronze. A shaped bow forms the handle, with a straight shaft and cut bit at the end. The bit shows simple wards and rubbed working edges. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			60.0,
			12.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Warded_Key"
			],
			null,
			null,
			null,
			null,
			"Bronze key variant for households where bronze fittings are preferred."
		);

		CreateItem(
			"medieval_shared_casket_hook_latch",
			"latch",
			"a small casket hook latch",
			null,
			"This small casket hook latch is a very small, workmanlike latch worked from brass. A curved hook meets a small eye plate, with screw holes visible on both leaves. The hook end is smoothed where it has been lifted by hand. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			120.0,
			12.0m,
			true,
			false,
			"brass",
			[
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Latch_Container_Hook"
			],
			null,
			null,
			null,
			null,
			"Small hook-and-eye latch for caskets, small cupboards, and light compartment doors."
		);

		CreateItem(
			"medieval_shared_chest_hasp_set",
			"hasp",
			"an iron chest hasp latch",
			null,
			"This iron chest hasp latch is a very small, workmanlike hasp worked from wrought iron. A hinged hasp crosses over a staple plate, with the fastening point clearly visible at the front. The hinge knuckle is rubbed bright from movement. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			620.0,
			12.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Latch_Container_Hasp"
			],
			null,
			null,
			null,
			null,
			"Functional hasp latch set for chests, coffers, trunks, and similar storage containers."
		);

		CreateItem(
			"medieval_shared_coffer_hasp_latch",
			"latch",
			"a stout coffer hasp latch",
			null,
			"This stout coffer hasp latch is a small, workmanlike latch worked from wrought iron. A hinged hasp crosses over a staple plate, with the fastening point clearly visible at the front. The hinge knuckle is rubbed bright from movement. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Small,
			ItemQuality.Standard,
			740.0,
			20.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Latch_Container_Hasp"
			],
			null,
			null,
			null,
			null,
			"Stouter hasp latch for a coffer, strong chest, or merchant storage box."
		);

		CreateItem(
			"medieval_shared_compact_box_lock",
			"lock",
			"a compact iron box lock",
			null,
			"This compact iron box lock is a very small, workmanlike lock worked from wrought iron. The lock has a flat faceplate, a visible keyhole, and a heavy shackle fitting. The back is thicker where the warded mechanism sits inside. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			760.0,
			20.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Warded_Lock_Normal"
			],
			null,
			null,
			null,
			null,
			"Compact lock profile for small boxes, tool chests, or travelling cases."
		);

		CreateItem(
			"medieval_shared_crude_casket_lock",
			"lock",
			"a crude little casket lock",
			null,
			"This crude little casket lock is a tiny, plain lock worked from wrought iron. The lock has a flat faceplate, a visible keyhole, and a heavy shackle fitting. The back is thicker where the warded mechanism sits inside. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Tiny,
			ItemQuality.Poor,
			320.0,
			8.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Warded_Lock_Bad"
			],
			null,
			null,
			null,
			null,
			"Small low-grade warded lock for caskets, small boxes, or similar fitted storage."
		);

		CreateItem(
			"medieval_shared_crude_one_sided_latch",
			"latch",
			"a crude one-sided latch",
			null,
			"This crude one-sided latch is a very small, plain latch worked from wrought iron. A hinged hasp crosses over a staple plate, with the fastening point clearly visible at the front. The hinge knuckle is rubbed bright from movement. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.VerySmall,
			ItemQuality.Poor,
			560.0,
			5.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Latch_Bad"
			],
			null,
			null,
			null,
			null,
			"Low-grade one-sided latch for simple closures."
		);

		CreateItem(
			"medieval_shared_cupboard_latch",
			"latch",
			"a small cupboard latch",
			null,
			"This small cupboard latch is a very small, workmanlike latch worked from wrought iron. A curved hook meets a small eye plate, with screw holes visible on both leaves. The hook end is smoothed where it has been lifted by hand. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			380.0,
			8.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Latch_Container_Hook"
			],
			null,
			null,
			null,
			null,
			"Small hook-and-eye latch for cupboards, shutters, light hatches, and similar one-sided closures."
		);

		CreateItem(
			"medieval_shared_cupboard_lock",
			"lock",
			"an iron cupboard lock",
			null,
			"This iron cupboard lock is a very small, workmanlike lock worked from wrought iron. The lock has a flat faceplate, a visible keyhole, and a heavy shackle fitting. The back is thicker where the warded mechanism sits inside. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			720.0,
			20.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Warded_Lock_Normal"
			],
			null,
			null,
			null,
			null,
			"Loose lock intended for cupboard, hutch, or cabinet installation."
		);

		CreateItem(
			"medieval_shared_door_hasp_set",
			"hasp",
			"an iron door hasp set",
			null,
			"This iron door hasp set is a small, workmanlike hasp worked from wrought iron. A hinged hasp crosses over a staple plate, with the fastening point clearly visible at the front. The hinge knuckle is rubbed bright from movement. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Small,
			ItemQuality.Standard,
			900.0,
			16.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			"Inert door hasp hardware intended to work with a separate loose lock."
		);

		CreateItem(
			"medieval_shared_door_latch",
			"latch",
			"an iron door latch",
			null,
			"This iron door latch is a very small, workmanlike latch worked from wrought iron. A straight sliding bar passes through two keepers, with a worn grip at one side. The metal is thickest where the bar meets the receiving plate. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			720.0,
			14.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Latch_Door_Bar"
			],
			null,
			null,
			null,
			null,
			"Heavy sliding bar latch for one-sided door closure."
		);

		CreateItem(
			"medieval_shared_drawbolt_latch",
			"latch",
			"an iron drawbolt latch",
			null,
			"This iron drawbolt latch is a very small, workmanlike latch worked from wrought iron. A straight sliding bar passes through two keepers, with a worn grip at one side. The metal is thickest where the bar meets the receiving plate. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			840.0,
			16.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Latch_Door_Bar"
			],
			null,
			null,
			null,
			null,
			"Drawbolt-style one-sided door or shutter latch."
		);

		CreateItem(
			"medieval_shared_drop_bar_latch",
			"latch",
			"a drop-bar iron latch",
			null,
			"This drop-bar iron latch is a small, well-made latch worked from wrought iron. A heavy drop bar rests between two deep keepers, with a battered lift point near the centre. The ends are squared to sit firmly in their sockets. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Small,
			ItemQuality.Good,
			1300.0,
			24.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Latch_Gate_DropBar"
			],
			null,
			null,
			null,
			null,
			"Weighty drop-bar latch for yard gates, stable gates, and palisade gates."
		);

		CreateItem(
			"medieval_shared_excellent_bronze_warded_lock",
			"lock",
			"an excellent bronze warded lock",
			null,
			"This excellent bronze warded lock is a very small, finely made lock worked from bronze. The lock has a flat faceplate, a visible keyhole, and a heavy shackle fitting. The back is thicker where the warded mechanism sits inside. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.VerySmall,
			ItemQuality.VeryGood,
			880.0,
			120.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Warded_Lock_Excellent"
			],
			null,
			null,
			null,
			null,
			"High-grade bronze lock for elite furniture, treasuries, or guarded storage."
		);

		CreateItem(
			"medieval_shared_fine_brass_warded_key",
			"key",
			"a fine brass warded key",
			null,
			"This fine brass warded key is a tiny, well-made key worked from brass. A shaped bow forms the handle, with a straight shaft and cut bit at the end. The bit shows simple wards and rubbed working edges. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			55.0,
			18.0m,
			true,
			false,
			"brass",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Warded_Key"
			],
			null,
			null,
			null,
			null,
			"Fine key form with cleaner shaping and higher-status material."
		);

		CreateItem(
			"medieval_shared_fine_brass_warded_lock",
			"lock",
			"a fine brass warded lock",
			null,
			"This fine brass warded lock is a very small, well-made lock worked from brass. The lock has a flat faceplate, a visible keyhole, and a heavy shackle fitting. The back is thicker where the warded mechanism sits inside. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			820.0,
			70.0m,
			true,
			false,
			"brass",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Warded_Lock_Good"
			],
			null,
			null,
			null,
			null,
			"Fine decorative lock whose behaviour remains a simple warded lock."
		);

		CreateItem(
			"medieval_shared_fine_chamber_latch",
			"latch",
			"a fine brass chamber latch",
			null,
			"This fine brass chamber latch is a very small, well-made latch worked from brass. A straight sliding bar passes through two keepers, with a worn grip at one side. The metal is thickest where the bar meets the receiving plate. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			480.0,
			30.0m,
			true,
			false,
			"brass",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Latch_Door_Bar"
			],
			null,
			null,
			null,
			null,
			"Better-made one-sided chamber-door latch with bright brass presentation."
		);

		CreateItem(
			"medieval_shared_fresh_replacement_key",
			"key",
			"a fresh-cut replacement key",
			null,
			"This fresh-cut replacement key is a tiny, workmanlike key worked from wrought iron. A shaped bow forms the handle, with a straight shaft and cut bit at the end. The bit shows simple wards and rubbed working edges. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			60.0,
			8.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Warded_Key"
			],
			null,
			null,
			null,
			null,
			"Freshly shaped key blank cut into an ordinary warded key profile."
		);

		CreateItem(
			"medieval_shared_hatch_hook_latch",
			"latch",
			"an iron hatch hook latch",
			null,
			"This iron hatch hook latch is a small, workmanlike latch worked from wrought iron. A curved hook meets a small eye plate, with screw holes visible on both leaves. The hook end is smoothed where it has been lifted by hand. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Small,
			ItemQuality.Standard,
			300.0,
			10.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Latch_Container_Hook"
			],
			null,
			null,
			null,
			null,
			"Hook latch for a small hatch, shuttered service opening, or light container lid."
		);

		CreateItem(
			"medieval_shared_heavy_gate_key",
			"key",
			"a heavy iron gate key",
			null,
			"This heavy iron gate key is a very small, workmanlike key worked from wrought iron. A shaped bow forms the handle, with a straight shaft and cut bit at the end. The bit shows simple wards and rubbed working edges. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			150.0,
			9.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Warded_Key"
			],
			null,
			null,
			null,
			null,
			"Large key form suitable for gates or exterior locks."
		);

		CreateItem(
			"medieval_shared_heavy_gate_lock",
			"lock",
			"a heavy iron gate lock",
			null,
			"This heavy iron gate lock is a small, well-made lock worked from wrought iron. The lock has a flat faceplate, a visible keyhole, and a heavy shackle fitting. The back is thicker where the warded mechanism sits inside. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Small,
			ItemQuality.Good,
			2400.0,
			58.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Warded_Lock_Good"
			],
			null,
			null,
			null,
			null,
			"Stouter loose warded lock suited to yard gates, pens, and exterior barriers."
		);

		CreateItem(
			"medieval_shared_heavy_steward_keyring",
			"keyring",
			"a heavy steward's keyring",
			null,
			"This heavy steward's keyring is a very small, well-made keyring worked from wrought iron. A split in the circular ring gives keys a clear point for loading. The surface is polished where metal has rubbed against metal. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			300.0,
			18.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Keyring_Large"
			],
			null,
			null,
			null,
			null,
			"Large, sturdy keyring for a steward, gatekeeper, storekeeper, or head servant."
		);

		CreateItem(
			"medieval_shared_hook_latch",
			"latch",
			"a hook-and-eye latch",
			null,
			"This hook-and-eye latch is a very small, workmanlike latch worked from wrought iron. A curved hook meets a small eye plate, with screw holes visible on both leaves. The hook end is smoothed where it has been lifted by hand. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			420.0,
			8.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Latch_Container_Hook"
			],
			null,
			null,
			null,
			null,
			"Small hook-and-eye latch for light lids, cupboard leaves, and small hatches."
		);

		CreateItem(
			"medieval_shared_household_door_key",
			"key",
			"a large household door key",
			null,
			"This large household door key is a very small, workmanlike key worked from wrought iron. A shaped bow forms the handle, with a straight shaft and cut bit at the end. The bit shows simple wards and rubbed working edges. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			90.0,
			7.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Warded_Key"
			],
			null,
			null,
			null,
			null,
			"Larger key silhouette for ordinary household door locks."
		);

		CreateItem(
			"medieval_shared_household_door_lock",
			"lock",
			"an iron household door lock",
			null,
			"This iron household door lock is a small, workmanlike lock worked from wrought iron. The lock has a flat faceplate, a visible keyhole, and a heavy shackle fitting. The back is thicker where the warded mechanism sits inside. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Small,
			ItemQuality.Standard,
			1500.0,
			35.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Warded_Lock_Normal"
			],
			null,
			null,
			null,
			null,
			"Loose lock hardware for ordinary doors; built-in lockable doors use door components instead."
		);

		CreateItem(
			"medieval_shared_iron_hasp",
			"hasp",
			"an iron hasp latch",
			null,
			"This iron hasp latch is a very small, workmanlike hasp worked from wrought iron. A hinged hasp crosses over a staple plate, with the fastening point clearly visible at the front. The hinge knuckle is rubbed bright from movement. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			420.0,
			8.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Latch_Container_Hasp"
			],
			null,
			null,
			null,
			null,
			"Functional hasp latch for a chest, trunk, coffer, or other lockable container surface."
		);

		CreateItem(
			"medieval_shared_iron_hasp_lock",
			"lock",
			"an iron hasp lock",
			null,
			"This iron hasp lock is a very small, workmanlike lock worked from wrought iron. The lock has a flat faceplate, a visible keyhole, and a heavy shackle fitting. The back is thicker where the warded mechanism sits inside. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			980.0,
			24.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Warded_Lock_Normal"
			],
			null,
			null,
			null,
			null,
			"Loose lock intended to work with a visible hasp or staple fitting."
		);

		CreateItem(
			"medieval_shared_iron_strike_plate",
			"plate",
			"an iron strike plate",
			null,
			"This iron strike plate is a very small, workmanlike plate worked from wrought iron. Holes and bent edges show exactly how the hardware fixes against timber. The working face is plain, with bright wear around the contact points. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			360.0,
			6.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			"Passive receiving plate for a bolt, latch, or lock tongue."
		);

		CreateItem(
			"medieval_shared_iron_warded_padlock",
			"lock",
			"an iron warded padlock",
			null,
			"This iron warded padlock is a very small, workmanlike lock worked from wrought iron. The lock has a flat faceplate, a visible keyhole, and a heavy shackle fitting. The back is thicker where the warded mechanism sits inside. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			950.0,
			24.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Warded_Lock_Normal"
			],
			null,
			null,
			null,
			null,
			"Padlock-like warded lock for hasps, chains, and temporary closures."
		);

		CreateItem(
			"medieval_shared_keyhole_escutcheon",
			"escutcheon",
			"a keyhole escutcheon plate",
			null,
			"This keyhole escutcheon plate is a tiny, workmanlike escutcheon worked from brass. Holes and bent edges show exactly how the hardware fixes against timber. The working face is plain, with bright wear around the contact points. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			120.0,
			7.0m,
			true,
			false,
			"brass",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			"Small protective keyhole surround for furniture or door hardware."
		);

		CreateItem(
			"medieval_shared_large_keeper_keyring",
			"keyring",
			"a large keeper's keyring",
			null,
			"This large keeper's keyring is a very small, workmanlike keyring worked from wrought iron. A split in the circular ring gives keys a clear point for loading. The surface is polished where metal has rubbed against metal. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			240.0,
			12.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Keyring_Large"
			],
			null,
			null,
			null,
			null,
			"Large keyring component for many household, yard, or storehouse keys."
		);

		CreateItem(
			"medieval_shared_large_keepers_key",
			"key",
			"a large keeper's key",
			null,
			"This large keeper's key is a very small, well-made key worked from wrought iron. A shaped bow forms the handle, with a straight shaft and cut bit at the end. The bit shows simple wards and rubbed working edges. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			125.0,
			15.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Warded_Key"
			],
			null,
			null,
			null,
			null,
			"Large key silhouette for a custodian, steward, or household keeper."
		);

		CreateItem(
			"medieval_shared_linked_key_chain",
			"keyring",
			"an iron linked key chain",
			null,
			"This iron linked key chain is a very small, workmanlike keyring worked from wrought iron. A split in the circular ring gives keys a clear point for loading. The surface is polished where metal has rubbed against metal. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			180.0,
			9.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Keyring_Large"
			],
			null,
			null,
			null,
			null,
			"Chain-like keyring form using the large keyring component."
		);

		CreateItem(
			"medieval_shared_lock_plate",
			"plate",
			"an iron lock plate",
			null,
			"This iron lock plate is a very small, workmanlike plate worked from wrought iron. Holes and bent edges show exactly how the hardware fixes against timber. The working face is plain, with bright wear around the contact points. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			500.0,
			9.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			"Decorative or protective lock plate; no locking behaviour by itself."
		);

		CreateItem(
			"medieval_shared_lock_straps",
			"strap",
			"a pair of iron lock straps",
			null,
			"This pair of iron lock straps is very small, workmanlike strap worked from wrought iron. Holes and bent edges show exactly how the hardware fixes against timber. The working face is plain, with bright wear around the contact points. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			700.0,
			12.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			"Reinforcing straps for lock plates, chests, and doors; no independent lock behaviour."
		);

		CreateItem(
			"medieval_shared_long_warded_key",
			"key",
			"a long iron warded key",
			null,
			"This long iron warded key is a very small, workmanlike key worked from wrought iron. A shaped bow forms the handle, with a straight shaft and cut bit at the end. The bit shows simple wards and rubbed working edges. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			95.0,
			6.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Warded_Key"
			],
			null,
			null,
			null,
			null,
			"Longer key form for deeper lock plates, large chests, or door hardware."
		);

		CreateItem(
			"medieval_shared_loop_bow_brass_key",
			"key",
			"a loop-bowed brass key",
			null,
			"This loop-bowed brass key is a tiny, workmanlike key worked from brass. A shaped bow forms the handle, with a straight shaft and cut bit at the end. The bit shows simple wards and rubbed working edges. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			45.0,
			8.0m,
			true,
			false,
			"brass",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Warded_Key"
			],
			null,
			null,
			null,
			null,
			"Brass key variant with a broad loop bow; behaviour remains a warded key."
		);

		CreateItem(
			"medieval_shared_master_cut_warded_lock",
			"lock",
			"a master-cut warded lock",
			null,
			"This master-cut warded lock is a very small, excellent lock worked from wrought iron. The lock has a flat faceplate, a visible keyhole, and a heavy shackle fitting. The back is thicker where the warded mechanism sits inside. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.VerySmall,
			ItemQuality.Excellent,
			950.0,
			200.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Warded_Lock_Master"
			],
			null,
			null,
			null,
			null,
			"Exceptional loose warded lock for high-security or prestige use."
		);

		CreateItem(
			"medieval_shared_palisade_gate_dropbar",
			"latch",
			"a palisade gate drop-bar",
			null,
			"This palisade gate drop-bar is a large, workmanlike latch worked from wrought iron. A heavy drop bar rests between two deep keepers, with a battered lift point near the centre. The ends are squared to sit firmly in their sockets. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Large,
			ItemQuality.Standard,
			6200.0,
			46.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Latch_Gate_DropBar"
			],
			null,
			null,
			null,
			null,
			"Weighty drop-bar latch for palisade gates, yard gates, and fortified outer barriers."
		);

		CreateItem(
			"medieval_shared_plain_warded_key",
			"key",
			"a plain iron warded key",
			null,
			"This plain iron warded key is a tiny, workmanlike key worked from wrought iron. A shaped bow forms the handle, with a straight shaft and cut bit at the end. The bit shows simple wards and rubbed working edges. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			70.0,
			4.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Warded_Key"
			],
			null,
			null,
			null,
			null,
			"General-purpose key for the warded mechanical lock family."
		);

		CreateItem(
			"medieval_shared_plain_warded_lock",
			"lock",
			"a plain iron warded lock",
			null,
			"This plain iron warded lock is a very small, workmanlike lock worked from wrought iron. The lock has a flat faceplate, a visible keyhole, and a heavy shackle fitting. The back is thicker where the warded mechanism sits inside. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			900.0,
			22.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Warded_Lock_Normal"
			],
			null,
			null,
			null,
			null,
			"General-purpose pre-modern warded lock for household use."
		);

		CreateItem(
			"medieval_shared_portcullis_pawl",
			"pawl",
			"a portcullis winch pawl",
			null,
			"This portcullis winch pawl is a medium-sized, well-made pawl worked from wrought iron. A toothed pawl sits on a pivot plate, with a short handle for lifting it clear. The striking edge is chipped from repeated catching. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Normal,
			ItemQuality.Good,
			2600.0,
			72.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Latch_Portcullis_Pawl"
			],
			null,
			null,
			null,
			null,
			"Winch pawl or brake latch for holding a portcullis, grate, or similar heavy barrier in place."
		);

		CreateItem(
			"medieval_shared_reinforced_iron_latch",
			"latch",
			"a reinforced iron latch",
			null,
			"This reinforced iron latch is a small, finely made latch worked from wrought iron. A hinged hasp crosses over a staple plate, with the fastening point clearly visible at the front. The hinge knuckle is rubbed bright from movement. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Small,
			ItemQuality.VeryGood,
			1500.0,
			42.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Latch_Excellent"
			],
			null,
			null,
			null,
			null,
			"High-grade one-sided latch with heavier plates and reinforcement."
		);

		CreateItem(
			"medieval_shared_repaired_warded_lock",
			"lock",
			"a visibly repaired warded lock",
			null,
			"This visibly repaired warded lock is a very small, plain lock worked from wrought iron. The lock has a flat faceplate, a visible keyhole, and a heavy shackle fitting. The back is thicker where the warded mechanism sits inside. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.VerySmall,
			ItemQuality.Substandard,
			880.0,
			10.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Warded_Lock_Bad"
			],
			null,
			null,
			null,
			null,
			"Serviceable but rough lock with visible patched plates and replacement rivets."
		);

		CreateItem(
			"medieval_shared_ring_bow_key",
			"key",
			"a ring-bowed iron key",
			null,
			"This ring-bowed iron key is a tiny, workmanlike key worked from wrought iron. A shaped bow forms the handle, with a straight shaft and cut bit at the end. The bit shows simple wards and rubbed working edges. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			75.0,
			5.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Warded_Key"
			],
			null,
			null,
			null,
			null,
			"Key with a simple ring-shaped bow for tying, hanging, or carrying on a keyring."
		);

		CreateItem(
			"medieval_shared_rust_spotted_warded_lock",
			"lock",
			"a rust-spotted warded lock",
			null,
			"This rust-spotted warded lock is a very small, plain lock worked from wrought iron. The lock has a flat faceplate, a visible keyhole, and a heavy shackle fitting. The back is thicker where the warded mechanism sits inside. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.VerySmall,
			ItemQuality.Poor,
			820.0,
			6.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Warded_Lock_Terrible"
			],
			null,
			null,
			null,
			null,
			"Loose warded lock in visibly poor condition; not tied to any particular door or container."
		);

		CreateItem(
			"medieval_shared_rusted_one_sided_latch",
			"latch",
			"a rusted one-sided latch",
			null,
			"This rusted one-sided latch is a very small, plain latch worked from wrought iron. A hinged hasp crosses over a staple plate, with the fastening point clearly visible at the front. The hinge knuckle is rubbed bright from movement. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.VerySmall,
			ItemQuality.Poor,
			520.0,
			3.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Latch_Terrible"
			],
			null,
			null,
			null,
			null,
			"Rough one-sided latch in poor condition."
		);

		CreateItem(
			"medieval_shared_shop_shutter_bar_latch",
			"latch",
			"a shop-shutter bar latch",
			null,
			"This shop-shutter bar latch is a medium-sized, workmanlike latch worked from wrought iron. A straight sliding bar passes through two keepers, with a worn grip at one side. The metal is thickest where the bar meets the receiving plate. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1800.0,
			26.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Latch_Door_Bar"
			],
			null,
			null,
			null,
			null,
			"Heavy sliding bar latch for shop shutters, booth doors, and other broad hinged panels."
		);

		CreateItem(
			"medieval_shared_shop_shutter_lock",
			"lock",
			"a shop-shutter warded lock",
			null,
			"This shop-shutter warded lock is a very small, workmanlike lock worked from wrought iron. The lock has a flat faceplate, a visible keyhole, and a heavy shackle fitting. The back is thicker where the warded mechanism sits inside. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			950.0,
			25.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Warded_Lock_Normal"
			],
			null,
			null,
			null,
			null,
			"Loose lock for shutters, stalls, booths, and small shopfront closures."
		);

		CreateItem(
			"medieval_shared_shutter_latch",
			"latch",
			"an iron shutter latch",
			null,
			"This iron shutter latch is a very small, workmanlike latch worked from wrought iron. A straight sliding bar passes through two keepers, with a worn grip at one side. The metal is thickest where the bar meets the receiving plate. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			520.0,
			10.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Latch_Door_Bar"
			],
			null,
			null,
			null,
			null,
			"Sliding bar latch for interior shutters and door-like panels."
		);

		CreateItem(
			"medieval_shared_simple_iron_latch",
			"latch",
			"a simple iron latch",
			null,
			"This simple iron latch is a very small, workmanlike latch worked from wrought iron. A hinged hasp crosses over a staple plate, with the fastening point clearly visible at the front. The hinge knuckle is rubbed bright from movement. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			620.0,
			12.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Latch_Normal"
			],
			null,
			null,
			null,
			null,
			"Ordinary one-sided latch for doors, cupboards, and shutters."
		);

		CreateItem(
			"medieval_shared_simple_key_hoop",
			"keyring",
			"a simple iron key hoop",
			null,
			"This simple iron key hoop is a tiny, workmanlike keyring worked from wrought iron. A split in the circular ring gives keys a clear point for loading. The surface is polished where metal has rubbed against metal. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			85.0,
			4.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Keyring_Small"
			],
			null,
			null,
			null,
			null,
			"Plain hoop-style keyring for a small set of keys."
		);

		CreateItem(
			"medieval_shared_sliding_bolt_latch",
			"latch",
			"a sliding bolt latch",
			null,
			"This sliding bolt latch is a very small, workmanlike latch worked from wrought iron. A straight sliding bar passes through two keepers, with a worn grip at one side. The metal is thickest where the bar meets the receiving plate. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			760.0,
			15.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Latch_Door_Bar"
			],
			null,
			null,
			null,
			null,
			"Sliding bolt latch for doors, shutters, and internal barriers."
		);

		CreateItem(
			"medieval_shared_small_chamber_lock",
			"lock",
			"a small brass chamber lock",
			null,
			"This small brass chamber lock is a very small, well-made lock worked from brass. The lock has a flat faceplate, a visible keyhole, and a heavy shackle fitting. The back is thicker where the warded mechanism sits inside. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			620.0,
			55.0m,
			true,
			false,
			"brass",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Warded_Lock_Good"
			],
			null,
			null,
			null,
			null,
			"Cleaner, finer lock suited to private chambers, cabinets, or elite household boxes."
		);

		CreateItem(
			"medieval_shared_small_chest_key",
			"key",
			"a small iron chest key",
			null,
			"This small iron chest key is a tiny, workmanlike key worked from wrought iron. A shaped bow forms the handle, with a straight shaft and cut bit at the end. The bit shows simple wards and rubbed working edges. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			55.0,
			4.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Warded_Key"
			],
			null,
			null,
			null,
			null,
			"Ordinary key profile for chest and coffer locks."
		);

		CreateItem(
			"medieval_shared_small_chest_lock",
			"lock",
			"a small iron chest lock",
			null,
			"This small iron chest lock is a very small, workmanlike lock worked from wrought iron. The lock has a flat faceplate, a visible keyhole, and a heavy shackle fitting. The back is thicker where the warded mechanism sits inside. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			650.0,
			18.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Warded_Lock_Normal"
			],
			null,
			null,
			null,
			null,
			"Ordinary loose warded lock sized for a chest or compact coffer."
		);

		CreateItem(
			"medieval_shared_small_iron_keyring",
			"keyring",
			"a small iron keyring",
			null,
			"This small iron keyring is a tiny, workmanlike keyring worked from wrought iron. A split in the circular ring gives keys a clear point for loading. The surface is polished where metal has rubbed against metal. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			110.0,
			5.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Keyring_Small"
			],
			null,
			null,
			null,
			null,
			"Small keyring component for a handful of keys."
		);

		CreateItem(
			"medieval_shared_stable_door_latch",
			"latch",
			"a stable-door iron latch",
			null,
			"This stable-door iron latch is a small, workmanlike latch worked from wrought iron. A straight sliding bar passes through two keepers, with a worn grip at one side. The metal is thickest where the bar meets the receiving plate. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Small,
			ItemQuality.Standard,
			950.0,
			16.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Latch_Door_Bar"
			],
			null,
			null,
			null,
			null,
			"Sturdy one-sided sliding bar latch for stable doors."
		);

		CreateItem(
			"medieval_shared_staple_plate",
			"staple",
			"an iron staple plate",
			null,
			"This iron staple plate is a very small, workmanlike staple worked from wrought iron. Holes and bent edges show exactly how the hardware fixes against timber. The working face is plain, with bright wear around the contact points. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			360.0,
			6.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			"Inert staple plate or catch for a hasp, chain, or lock fitting."
		);

		CreateItem(
			"medieval_shared_storehouse_key",
			"key",
			"a stout storehouse key",
			null,
			"This stout storehouse key is a very small, workmanlike key worked from wrought iron. A shaped bow forms the handle, with a straight shaft and cut bit at the end. The bit shows simple wards and rubbed working edges. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			125.0,
			8.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Warded_Key"
			],
			null,
			null,
			null,
			null,
			"Stout warded key for storeroom, granary, or warehouse locks."
		);

		CreateItem(
			"medieval_shared_storehouse_lock",
			"lock",
			"a stout storehouse lock",
			null,
			"This stout storehouse lock is a small, well-made lock worked from wrought iron. The lock has a flat faceplate, a visible keyhole, and a heavy shackle fitting. The back is thicker where the warded mechanism sits inside. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Small,
			ItemQuality.Good,
			1900.0,
			52.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Warded_Lock_Good"
			],
			null,
			null,
			null,
			null,
			"Sturdy lock profile for storerooms, granaries, warehouses, or strong cupboards."
		);

		CreateItem(
			"medieval_shared_strongbox_lock",
			"lock",
			"a good iron strongbox lock",
			null,
			"This good iron strongbox lock is a very small, well-made lock worked from wrought iron. The lock has a flat faceplate, a visible keyhole, and a heavy shackle fitting. The back is thicker where the warded mechanism sits inside. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			1050.0,
			45.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Warded_Lock_Good"
			],
			null,
			null,
			null,
			null,
			"Better-made loose lock for a strongbox or money chest."
		);

		CreateItem(
			"medieval_shared_tag_holed_warded_key",
			"key",
			"a tag-holed warded key",
			null,
			"This tag-holed warded key is a tiny, workmanlike key worked from wrought iron. A shaped bow forms the handle, with a straight shaft and cut bit at the end. The bit shows simple wards and rubbed working edges. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			65.0,
			5.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Warded_Key"
			],
			null,
			null,
			null,
			null,
			"Key with a pierced bow for tying on a tag, cord, or keyring; no writing is assumed."
		);

		CreateItem(
			"medieval_shared_tiny_casket_key",
			"key",
			"a tiny warded casket key",
			null,
			"This tiny warded casket key is a tiny, workmanlike key worked from wrought iron. A shaped bow forms the handle, with a straight shaft and cut bit at the end. The bit shows simple wards and rubbed working edges. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			35.0,
			3.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Warded_Key"
			],
			null,
			null,
			null,
			null,
			"Small key form for caskets and miniature locks; mechanical key family remains warded."
		);

		CreateItem(
			"medieval_shared_trunk_hasp_latch",
			"latch",
			"an iron trunk hasp latch",
			null,
			"This iron trunk hasp latch is a small, workmanlike latch worked from wrought iron. A hinged hasp crosses over a staple plate, with the fastening point clearly visible at the front. The hinge knuckle is rubbed bright from movement. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Small,
			ItemQuality.Standard,
			680.0,
			18.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Latch_Container_Hasp"
			],
			null,
			null,
			null,
			null,
			"Functional hasp latch sized for trunks, blanket chests, and travelling storage boxes."
		);

		CreateItem(
			"medieval_shared_winch_brake_pawl",
			"pawl",
			"a heavy winch brake pawl",
			null,
			"This heavy winch brake pawl is a medium-sized, well-made pawl worked from wrought iron. A toothed pawl sits on a pivot plate, with a short handle for lifting it clear. The striking edge is chipped from repeated catching. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Normal,
			ItemQuality.Good,
			2300.0,
			64.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Latch_Portcullis_Pawl"
			],
			null,
			null,
			null,
			null,
			"Heavy pawl for a hoist, winch, gate lift, or portcullis-like mechanism."
		);

		CreateItem(
			"medieval_shared_worn_old_warded_key",
			"key",
			"a worn old warded key",
			null,
			"This worn old warded key is a tiny, plain key worked from wrought iron. A shaped bow forms the handle, with a straight shaft and cut bit at the end. The bit shows simple wards and rubbed working edges. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Tiny,
			ItemQuality.Poor,
			45.0,
			1.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Warded_Key"
			],
			null,
			null,
			null,
			null,
			"Old key form with softened edges; no guaranteed special compatibility beyond the warded key component."
		);

		CreateItem(
			"medieval_shared_yard_gate_latch",
			"latch",
			"a yard-gate iron latch",
			null,
			"This yard-gate iron latch is a small, well-made latch worked from wrought iron. A heavy drop bar rests between two deep keepers, with a battered lift point near the centre. The ends are squared to sit firmly in their sockets. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Small,
			ItemQuality.Good,
			1200.0,
			24.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Latch_Gate_DropBar"
			],
			null,
			null,
			null,
			null,
			"Gate drop-bar latch for yards, byres, pens, and outer service gates."
		);

		CreateItem(
			"medieval_northern_ship_chest_lock",
			"lock",
			"an iron ship-chest lock",
			null,
			"This iron ship-chest lock is a very small, well-made lock worked from wrought iron. The lock has a flat faceplate, a visible keyhole, and a heavy shackle fitting. The back is thicker where the warded mechanism sits inside. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			1200.0,
			42.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Warded_Lock_Good"
			],
			null,
			null,
			null,
			null,
			"Regional-form addition for sea chests, travel trunks, and damp-weather storage."
		);

		CreateItem(
			"medieval_northern_stave_door_latch",
			"latch",
			"a stave-door iron latch",
			null,
			"This stave-door iron latch is a small, well-made latch worked from wrought iron. A straight sliding bar passes through two keepers, with a worn grip at one side. The metal is thickest where the bar meets the receiving plate. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Small,
			ItemQuality.Good,
			1200.0,
			20.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Latch_Door_Bar"
			],
			null,
			null,
			null,
			null,
			"Sturdy one-sided bar latch for plank or stave-built doors."
		);

		CreateItem(
			"medieval_western_hall_keeper_keyring",
			"keyring",
			"a hall-keeper's keyring",
			null,
			"This hall-keeper's keyring is a very small, well-made keyring worked from wrought iron. A split in the circular ring gives keys a clear point for loading. The surface is polished where metal has rubbed against metal. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			260.0,
			20.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Keyring_Large"
			],
			null,
			null,
			null,
			null,
			"Large keyring form for hall, store, and chamber key management."
		);

		CreateItem(
			"medieval_western_moulded_chamber_lock",
			"lock",
			"a moulded brass chamber lock",
			null,
			"This moulded brass chamber lock is a very small, well-made lock worked from brass. The lock has a flat faceplate, a visible keyhole, and a heavy shackle fitting. The back is thicker where the warded mechanism sits inside. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			850.0,
			72.0m,
			true,
			false,
			"brass",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Warded_Lock_Good"
			],
			null,
			null,
			null,
			null,
			"Fine domestic chamber-lock form for urban or manor interiors."
		);

		CreateItem(
			"medieval_mediterranean_bronze_box_lock",
			"lock",
			"a bronze box lock",
			null,
			"This bronze box lock is a very small, workmanlike lock worked from bronze. The lock has a flat faceplate, a visible keyhole, and a heavy shackle fitting. The back is thicker where the warded mechanism sits inside. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			760.0,
			34.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Warded_Lock_Normal"
			],
			null,
			null,
			null,
			null,
			"Bronze-fitted box lock for Mediterranean-facing household storage."
		);

		CreateItem(
			"medieval_mediterranean_bronze_ring_bow_key",
			"key",
			"a bronze ring-bow key",
			null,
			"This bronze ring-bow key is a tiny, workmanlike key worked from bronze. A shaped bow forms the handle, with a straight shaft and cut bit at the end. The bit shows simple wards and rubbed working edges. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			58.0,
			12.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Warded_Key"
			],
			null,
			null,
			null,
			null,
			"Bronze key form with a clear ring bow for tying or hanging."
		);

		CreateItem(
			"medieval_islamicate_brass_scrollwork_lock",
			"lock",
			"a brass scrollwork warded lock",
			null,
			"This brass scrollwork warded lock is a very small, well-made lock worked from brass. The lock has a flat faceplate, a visible keyhole, and a heavy shackle fitting. The back is thicker where the warded mechanism sits inside. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			860.0,
			80.0m,
			true,
			false,
			"brass",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Warded_Lock_Good"
			],
			null,
			null,
			null,
			null,
			"Decorative brass lock form for urban chests, cabinets, or storerooms."
		);

		CreateItem(
			"medieval_islamicate_courtyard_latch",
			"latch",
			"a brass courtyard latch",
			null,
			"This brass courtyard latch is a very small, well-made latch worked from brass. A heavy drop bar rests between two deep keepers, with a battered lift point near the centre. The ends are squared to sit firmly in their sockets. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			650.0,
			35.0m,
			true,
			false,
			"brass",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Latch_Gate_DropBar"
			],
			null,
			null,
			null,
			null,
			"Drop-bar style latch for a courtyard gate or heavy outer screen."
		);

		CreateItem(
			"medieval_south_asian_brass_courtyard_lock",
			"lock",
			"a brass courtyard lock",
			null,
			"This brass courtyard lock is a very small, well-made lock worked from brass. The lock has a flat faceplate, a visible keyhole, and a heavy shackle fitting. The back is thicker where the warded mechanism sits inside. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			920.0,
			70.0m,
			true,
			false,
			"brass",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Warded_Lock_Good"
			],
			null,
			null,
			null,
			null,
			"Brass lock form for courtyard gates, strong cupboards, or household boxes."
		);

		CreateItem(
			"medieval_south_asian_broad_loop_key",
			"key",
			"a broad-loop brass key",
			null,
			"This broad-loop brass key is a tiny, well-made key worked from brass. A shaped bow forms the handle, with a straight shaft and cut bit at the end. The bit shows simple wards and rubbed working edges. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			60.0,
			16.0m,
			true,
			false,
			"brass",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Warded_Key"
			],
			null,
			null,
			null,
			null,
			"Broad-loop brass key silhouette for regional household locks."
		);

		CreateItem(
			"medieval_east_asian_bronze_cabinet_lock",
			"lock",
			"a bronze cabinet lock",
			null,
			"This bronze cabinet lock is a very small, workmanlike lock worked from bronze. The lock has a flat faceplate, a visible keyhole, and a heavy shackle fitting. The back is thicker where the warded mechanism sits inside. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			720.0,
			34.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Warded_Lock_Normal"
			],
			null,
			null,
			null,
			null,
			"Compact bronze lock form for cabinet, chest, or document-storage furniture."
		);

		CreateItem(
			"medieval_east_asian_pierced_keyhole_plate",
			"escutcheon",
			"a pierced copper keyhole plate",
			null,
			"This pierced copper keyhole plate is a tiny, workmanlike escutcheon worked from copper. Holes and bent edges show exactly how the hardware fixes against timber. The working face is plain, with bright wear around the contact points. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			95.0,
			9.0m,
			true,
			false,
			"copper",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			"Decorative keyhole plate; passive hardware without lock behaviour."
		);

		CreateItem(
			"medieval_steppe_pack_chest_key",
			"key",
			"a stout pack-chest key",
			null,
			"This stout pack-chest key is a very small, workmanlike key worked from wrought iron. A shaped bow forms the handle, with a straight shaft and cut bit at the end. The bit shows simple wards and rubbed working edges. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			95.0,
			8.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Warded_Key"
			],
			null,
			null,
			null,
			null,
			"Stout warded key form for portable chest and travel storage locks."
		);

		CreateItem(
			"medieval_steppe_travel_coffer_lock",
			"lock",
			"a travel-coffer iron lock",
			null,
			"This travel-coffer iron lock is a very small, well-made lock worked from wrought iron. The lock has a flat faceplate, a visible keyhole, and a heavy shackle fitting. The back is thicker where the warded mechanism sits inside. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			1050.0,
			42.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Construction Materials",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Warded_Lock_Good"
			],
			null,
			null,
			null,
			null,
			"Robust loose lock for portable coffers, packs, wagons, and travelling storage."
		);
	}
}
