#nullable enable

using System;
using System.Collections.Generic;
using CultureInfo = System.Globalization.CultureInfo;
using System.Linq;
using System.Text;
using MudSharp.FutureProg;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

internal sealed partial class ArmageddonWorldfileWriter
{
	private const string PostalCourierRewardRegister = "armw_postal_courier_reward";
	private const string PostalCourierParcelRegister = "armw_postal_courier_parcel";

	private void InstallPostalCouriers(PostalOffice[] offices, IReadOnlyDictionary<int, Room> custodyRooms, double sourceWeightUnit)
	{
		EnsurePostalRegister(ProgVariableTypes.Item, PostalCourierRewardRegister, ProgVariableTypes.Number, "<var>0</var>");
		EnsurePostalRegister(ProgVariableTypes.Item, PostalCourierParcelRegister, ProgVariableTypes.Number, "<var>0</var>");
		var kilograms = (sourceWeightUnit * 100).ToString("R", CultureInfo.InvariantCulture);
		var pickup = new StringBuilder(VendorAdmission + "\n");
		pickup.AppendLine("if (@args.Count(word, true) != 2 or lowercase(getindex(@args, 0)) != \"mail\")\nreturn false\nend if\nif (not(@ArmageddonWorldfileClan28Leader(@actor)))\nsend(@actor, \"Only an appointed House Kohmar leader may take courier consignments.\")\nreturn true\nend if\nvar office = 0\nvar destination = 0\nvar priceRate = 0\nvar days = 0\nvar reliability = 0\nvar query = lowercase(getindex(@args, 1))");
		AppendPostalSourceOffice(pickup, offices);
		for (var i = 0; i < offices.Length; i++)
			pickup.AppendLine($"if (@query == \"{offices[i].City}\" or @query == \"{offices[i].Post}\")\ndestination = {i + 1}\nend if");
		AppendPostalRouteMatrix(pickup, offices);
		pickup.AppendLine("if (@office == 0 or @destination == 0 or @office == @destination)\nsend(@actor, \"Name another postal office: allanak, tuluk, luir or redstorm.\")\nreturn true\nend if\nvar storage = tolocation(0)");
		for (var i = 0; i < offices.Length; i++)
			pickup.AppendLine($"if (@office == {i + 1})\nstorage = tolocation({custodyRooms[offices[i].NpcVnum].Id})\nend if");
		pickup.AppendLine($"var pending = @storage.items.Where(parcel, getregister(@parcel, \"{PostalParcelDestinationRegister}\") == @destination and getregister(@parcel, \"{PostalParcelInTransitRegister}\") == 0)\nif (collectioncount(@pending) == 0)\nsend(@actor, \"There are no outgoing parcels for that office.\")\nreturn true\nend if\nvar first = collectionfirst(@pending)\nvar recipient = getregister(@first, \"{PostalParcelRecipientRegister}\")\nvar weight = 0\nvar previousReward = 0\nvar delivered = false\nforeach (parcel in @pending)\nif (getregister(@parcel, \"{PostalParcelRecipientRegister}\") == @recipient)");
		pickup.AppendLine($"var nextWeight = @weight + floor(max(0, @parcel.weight) / {kilograms})\nvar nextReward = floor(3 * @priceRate * @nextWeight / 4)\nvar reward = @nextReward - @previousReward\nsetregister @parcel \"{PostalCourierRewardRegister}\" @reward\nsetregister @parcel \"{PostalCourierParcelRegister}\" @parcel.id\nsetregister @parcel \"{PostalParcelInTransitRegister}\" 3\nif (purchaseitemdelivery(@actor, @seller, @parcel, @storage, tocurrency({_currency.Id}), 0, true))\nweight = @nextWeight\npreviousReward = @nextReward\ndelivered = true\nsend(@actor, \"Courier parcel: \" + @parcel.name + \" for \" + getregister(@parcel, \"{PostalParcelAddressRegister}\") + \" at \" + @query + \". Delivery pays \" + @reward + \" bits.\")\nelse\nsetregister @parcel \"{PostalParcelInTransitRegister}\" 0\nsetregister @parcel \"{PostalCourierRewardRegister}\" 0\nsetregister @parcel \"{PostalCourierParcelRegister}\" 0\nend if\nend if\nend foreach\nif (not(@delivered))\nsend(@actor, \"The postmaster cannot hand over those parcels right now.\")\nend if\nreturn true");
		var pickupProg = NpcServiceProg("ArmageddonWorldfilePostalCourierPickup", pickup.ToString());
		InstallNpcInputHook("Postal Courier Pickup", "get", pickupProg, offices.Select(x => x.NpcVnum).ToArray());

		var delivery = new StringBuilder(VendorAdmission + "\n");
		delivery.AppendLine("if (@args.Count(word, true) != 3 or lowercase(getindex(@args, 1)) != \"at\" or targetcharacter(@actor, getindex(@args, 2)) != @seller)\nreturn false\nend if\nif (not(@ArmageddonWorldfileClan28Leader(@actor)))\nsend(@actor, \"Only an appointed House Kohmar leader may complete courier consignments.\")\nreturn true\nend if\nvar parcel = targetitem(@actor, @actor.inventory, getindex(@args, 0))\nif (isnull(@parcel))\nsend(@actor, \"You must be holding the courier parcel.\")\nreturn true\nend if\nvar office = 0");
		AppendPostalSourceOffice(delivery, offices);
		delivery.AppendLine($"if (@office == 0 or getregister(@parcel, \"{PostalParcelDestinationRegister}\") != @office or getregister(@parcel, \"{PostalParcelInTransitRegister}\") != 3 or getregister(@parcel, \"{PostalCourierParcelRegister}\") != @parcel.id)\nsend(@actor, \"That is not an outstanding courier parcel for this office.\")\nreturn true\nend if\nvar storage = tolocation(0)");
		for (var i = 0; i < offices.Length; i++)
			delivery.AppendLine($"if (@office == {i + 1})\nstorage = tolocation({custodyRooms[offices[i].NpcVnum].Id})\nend if");
		delivery.AppendLine($"var reward = getregister(@parcel, \"{PostalCourierRewardRegister}\")\nif (not(purchasehelditemcustody(@actor, @seller, @parcel, @storage, tocurrency({_currency.Id}), 0)))\nsend(@actor, \"The postmaster cannot receive that parcel right now.\")\nreturn true\nend if\nsetregister @parcel \"{PostalCourierRewardRegister}\" 0\nsetregister @parcel \"{PostalCourierParcelRegister}\" 0\nsetregister @parcel \"{PostalParcelInTransitRegister}\" 2\nsetregister @parcel \"{PostalParcelArrivalRegister}\" now()\nsetregister @parcel \"{PostalParcelExpiryRegister}\" (now() + timespanfromseconds(40 * 5400))\nif (@reward > 0)\ngivecurrency(tocurrency({_currency.Id}), @reward, @actor, false)\nend if\nsend(@actor, \"You deliver the courier parcel and receive \" + @reward + \" bits.\")\nreturn true");
		var deliveryProg = NpcServiceProg("ArmageddonWorldfilePostalCourierDelivery", delivery.ToString());
		InstallNpcInputHook("Postal Courier Delivery", "deliver", deliveryProg, offices.Select(x => x.NpcVnum).ToArray());
		Record("npc-service.physical-post-couriers", "NativePostalCourierProgs", null, null, new
		{
			SourceClan = 28, Pickup = pickupProg.Id, Delivery = deliveryProg.Id,
			Disposition = "Initially unfilled House Kohmar Leader appointment; no Bram exception. One recipient's outgoing route batch per pickup, actual parcels with address/destination instructions. Source aggregate carried-weight reward uses native mass and source integer truncation; cumulative allocation preserves the batch's 75-percent integer reward. Approved adaptation pays only on successful destination delivery. Original parcel ID and state prevent copied or repeated rewards."
		});
	}
}
