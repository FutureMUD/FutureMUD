#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MudSharp.FutureProg;
using MudSharp.Models;
using MudSharp.TimeAndDate.Intervals;

namespace DatabaseSeeder.Seeders;

internal sealed partial class ArmageddonWorldfileWriter
{
	internal sealed record PostalOffice(int NpcVnum, int RoomVnum, string Post, string City);
	internal sealed record PostalRoute(int DeliveryDays, int ReliabilityPercent, int BitsPerSourceWeightUnit);

	internal static readonly IReadOnlyList<PostalOffice> SourcePostalOffices =
	[
		new(50033, 50937, "trader", "allanak"),
		new(20115, 28028, "flint", "tuluk"),
		new(64007, 64027, "luir", "luir"),
		new(48001, 48061, "redstorm", "redstorm")
	];

	// Indices follow SourcePostalOffices; these are the source mail.c table's four
	// destination offices (the fifth source entry is its non-office transit bucket).
	internal static readonly PostalRoute[,] SourcePostalRoutes =
	{
		{ new(0, 100, 1), new(5, 90, 10), new(4, 90, 8), new(2, 98, 5) },
		{ new(5, 90, 10), new(0, 100, 1), new(4, 90, 8), new(6, 88, 12) },
		{ new(4, 92, 8), new(4, 92, 8), new(0, 100, 1), new(5, 90, 10) },
		{ new(2, 98, 5), new(8, 88, 12), new(5, 90, 10), new(0, 100, 1) }
	};

	private const string PostalAddressDictionaryRegister = "armw_postal_addresses";
	private const string PostalAddressOfficeDictionaryRegister = "armw_postal_address_offices";
	private const string PostalCharacterAddressRegister = "armw_postal_address";
	private const string PostalParcelAddressRegister = "armw_postal_address_text";
	private const string PostalCharacterOfficeRegister = "armw_postal_office";
	private const string PostalParcelRecipientRegister = "armw_postal_recipient";
	private const string PostalParcelDestinationRegister = "armw_postal_destination";
	private const string PostalParcelSenderRegister = "armw_postal_sender";
	private const string PostalParcelArrivalRegister = "armw_postal_arrival";
	private const string PostalParcelExpiryRegister = "armw_postal_expiry";
	private const string PostalParcelInTransitRegister = "armw_postal_transit";
	private const string PostalDeliveryCursorRegister = "armw_postal_delivery_cursor";

	private void InstallPostal()
	{
		var offices = SourcePostalOffices.Where(x => _npcs.ContainsKey(x.NpcVnum) && _rooms.ContainsKey(x.RoomVnum) &&
			_source.Npcs.Any(n => n.Vnum == x.NpcVnum) && _source.Resets.Any(r => r.Opcode == "M" &&
				r.ReferenceIssues.Length == 0 && r.Arguments.Length > 3 && r.Arguments[1] == x.NpcVnum && r.Arguments[3] == x.RoomVnum))
			.ToArray();
		if (offices.Length == 0) return;
		if (offices.Length != SourcePostalOffices.Count)
			throw new InvalidOperationException("Physical postal service requires all four source offices and their retained reset placements.");

		var addressRoom = _rooms[SourcePostalOffices[0].RoomVnum];
		EnsurePostalRegister(ProgVariableTypes.Location, PostalAddressDictionaryRegister,
			ProgVariableTypes.Dictionary | ProgVariableTypes.Number, "<var/>");
		EnsurePostalRegister(ProgVariableTypes.Location, PostalAddressOfficeDictionaryRegister,
			ProgVariableTypes.Dictionary | ProgVariableTypes.Number, "<var/>");
		EnsurePostalRegister(ProgVariableTypes.Location, PostalDeliveryCursorRegister, ProgVariableTypes.Number, "<var>0</var>");
		EnsurePostalRegister(ProgVariableTypes.Character, PostalCharacterAddressRegister, ProgVariableTypes.Text, "<var></var>");
		EnsurePostalRegister(ProgVariableTypes.Character, PostalCharacterOfficeRegister, ProgVariableTypes.Number, "<var>0</var>");
		EnsurePostalRegister(ProgVariableTypes.Item, PostalParcelAddressRegister, ProgVariableTypes.Text, "<var></var>");
		EnsurePostalRegister(ProgVariableTypes.Item, PostalParcelRecipientRegister, ProgVariableTypes.Number, "<var>0</var>");
		EnsurePostalRegister(ProgVariableTypes.Item, PostalParcelDestinationRegister, ProgVariableTypes.Number, "<var>0</var>");
		EnsurePostalRegister(ProgVariableTypes.Item, PostalParcelSenderRegister, ProgVariableTypes.Number, "<var>0</var>");
		EnsurePostalRegister(ProgVariableTypes.Item, PostalParcelArrivalRegister, ProgVariableTypes.DateTime, "<var>0001-01-01T00:00:00.0000000Z</var>");
		EnsurePostalRegister(ProgVariableTypes.Item, PostalParcelExpiryRegister, ProgVariableTypes.DateTime, "<var>0001-01-01T00:00:00.0000000Z</var>");
		EnsurePostalRegister(ProgVariableTypes.Item, PostalParcelInTransitRegister, ProgVariableTypes.Number, "<var>0</var>");

		var custodyRooms = offices.ToDictionary(x => x.NpcVnum, x => CreateAuxiliaryRoom(
			_source.Rooms.Single(r => r.Vnum == x.RoomVnum).Zone,
			$"postal-custody.Vnum{x.NpcVnum}.Room{x.RoomVnum}", $"{x.City} Post Office Custody Room",
			"A secured receiving room holds parcels until their registered recipients collect them."));
		var kg = _db.UnitsOfMeasure.Single(x => x.Id == _settings.NativeBindings[ArmageddonNativeBinding.Kilogram]).BaseMultiplier;
		var sourceWeightUnits = 0.01 * kg;
		var registrationBody = new StringBuilder(VendorAdmission + "\n")
			.AppendLine("var office = 0");
		foreach (var (office, index) in offices.Select((x, i) => (x, i)))
			registrationBody.AppendLine($"if (@seller.location == tolocation({_rooms[office.RoomVnum].Id}))\noffice = {index + 1}\nend if");
		registrationBody.AppendLine("if (@office == 0)\nreturn false\nend if\nif (@args.Count(word, true) != 1)\nsend(@actor, \"Register one short, single-word postal address, or use 'address off' to remove it.\")\nreturn true\nend if\nvar address = lowercase(getindex(@args, 0))\nvar addresses = getregister(tolocation(" + addressRoom.Id + "), \"" + PostalAddressDictionaryRegister + "\")\nvar addressOffices = getregister(tolocation(" + addressRoom.Id + "), \"" + PostalAddressOfficeDictionaryRegister + "\")\nvar oldAddress = getregister(@actor, \"" + PostalCharacterAddressRegister + "\")");
		registrationBody.AppendLine("if (@address == \"off\")\nif (@oldAddress != \"\" and dictionarycontainskey(@addresses, @oldAddress) and dictionaryget(@addresses, @oldAddress) == @actor.identityid)\naddresses = dictionarywithoutkey(@addresses, @oldAddress)\naddressOffices = dictionarywithoutkey(@addressOffices, @oldAddress)\nend if\nsetregister tolocation(" + addressRoom.Id + ") \"" + PostalAddressDictionaryRegister + "\" @addresses\nsetregister tolocation(" + addressRoom.Id + ") \"" + PostalAddressOfficeDictionaryRegister + "\" @addressOffices\nsetregister @actor \"" + PostalCharacterAddressRegister + "\" \"\"\nsetregister @actor \"" + PostalCharacterOfficeRegister + "\" 0\nsend(@actor, \"Your postal address is removed.\")\nreturn true\nend if");
		registrationBody.AppendLine("if (dictionarycontainskey(@addresses, @address) and dictionaryget(@addresses, @address) != @actor.identityid)\nsend(@actor, \"That postal address is already registered. Choose another.\")\nreturn true\nend if\nif (@oldAddress != \"\" and @oldAddress != @address and dictionarycontainskey(@addresses, @oldAddress) and dictionaryget(@addresses, @oldAddress) == @actor.identityid)\naddresses = dictionarywithoutkey(@addresses, @oldAddress)\naddressOffices = dictionarywithoutkey(@addressOffices, @oldAddress)\nend if\nset(@addresses, @address, @actor.identityid)\nset(@addressOffices, @address, @office)\nsetregister tolocation(" + addressRoom.Id + ") \"" + PostalAddressDictionaryRegister + "\" @addresses\nsetregister tolocation(" + addressRoom.Id + ") \"" + PostalAddressOfficeDictionaryRegister + "\" @addressOffices\nsetregister @actor \"" + PostalCharacterAddressRegister + "\" @address\nsetregister @actor \"" + PostalCharacterOfficeRegister + "\" @office\nsend(@actor, \"Your registered postal address is \" + @address + \" at the \" + @seller.name + \" office.\")\nreturn true");
		var registration = Prog("ArmageddonWorldfilePostalAddress", registrationBody.ToString(), ProgVariableTypes.Boolean,
			("actor", ProgVariableTypes.Character), ("npc", ProgVariableTypes.Perceivable), ("command", ProgVariableTypes.Text),
			("args", ProgVariableTypes.Text | ProgVariableTypes.Collection));
		InstallNpcInputHook("Postal Address Registration", "address", registration, offices.Select(x => x.NpcVnum).ToArray());

		var assess = new StringBuilder(VendorAdmission + "\n")
			.AppendLine("if (@args.Count(word, true) != 2)\nreturn false\nend if\nvar item = targetitem(@actor, @actor.inventory, getindex(@args, 0))\nvar address = lowercase(getindex(@args, 1))\nif (isnull(@item))\nreturn false\nend if\nvar office = 0\nvar destination = 0\nvar priceRate = 0\nvar days = 0\nvar reliability = 0\nvar addresses = getregister(tolocation(" + addressRoom.Id + "), \"" + PostalAddressDictionaryRegister + "\")\nvar addressOffices = getregister(tolocation(" + addressRoom.Id + "), \"" + PostalAddressOfficeDictionaryRegister + "\")\nif (not(dictionarycontainskey(@addresses, @address)))\nsend(@actor, \"That postal address is not registered.\")\nreturn true\nend if\ndestination = dictionaryget(@addressOffices, @address)\nvar recipient = dictionaryget(@addresses, @address)");
		AppendPostalSourceOffice(assess, offices);
		AppendPostalRouteMatrix(assess, offices);
		assess.AppendLine($"if (@office == 0 or @destination < 1 or @destination > 4)\nreturn false\nend if\nvar weightUnits = floor(max(0, itemprototype(@item.proto).weight) / {sourceWeightUnits.ToString("R", System.Globalization.CultureInfo.InvariantCulture)} + 0.5)\nvar price = @priceRate * (@weightUnits + 1)\nvar arrival = @days\nif (@office != @destination)\narrival = @arrival + random(0, 2)\nend if\nsend(@actor, \"Sending \" + @item.name + \" to \" + @address + \" will cost \" + @price + \" bits and arrive in about \" + @arrival + \" mud days. The source route delivery reliability is \" + @reliability + \" percent.\")\nreturn true");
		var assessment = Prog("ArmageddonWorldfilePostalAssess", assess.ToString(), ProgVariableTypes.Boolean,
			("actor", ProgVariableTypes.Character), ("npc", ProgVariableTypes.Perceivable), ("command", ProgVariableTypes.Text),
			("args", ProgVariableTypes.Text | ProgVariableTypes.Collection));
		InstallNpcInputHook("Postal Assessment", "assess", assessment, offices.Select(x => x.NpcVnum).ToArray());

		var send = new StringBuilder(VendorAdmission + "\n")
			.AppendLine("if (@args.Count(word, true) != 2)\nreturn false\nend if\nvar item = targetitem(@actor, @actor.inventory, getindex(@args, 0))\nvar address = lowercase(getindex(@args, 1))\nif (isnull(@item))\nreturn false\nend if\nvar office = 0\nvar destination = 0\nvar priceRate = 0\nvar days = 0\nvar reliability = 0\nvar addresses = getregister(tolocation(" + addressRoom.Id + "), \"" + PostalAddressDictionaryRegister + "\")\nvar addressOffices = getregister(tolocation(" + addressRoom.Id + "), \"" + PostalAddressOfficeDictionaryRegister + "\")\nif (not(dictionarycontainskey(@addresses, @address)))\nsend(@actor, \"That postal address is not registered.\")\nreturn true\nend if\ndestination = dictionaryget(@addressOffices, @address)\nvar recipient = dictionaryget(@addresses, @address)");
		AppendPostalSourceOffice(send, offices);
		AppendPostalRouteMatrix(send, offices);
		send.AppendLine($"if (@office == 0 or @destination < 1 or @destination > 4)\nreturn false\nend if\nvar weightUnits = floor(max(0, itemprototype(@item.proto).weight) / {sourceWeightUnits.ToString("R", System.Globalization.CultureInfo.InvariantCulture)} + 0.5)\nvar price = @priceRate * (@weightUnits + 1)\nvar transitDays = 0\nif (@office != @destination)\ntransitDays = @days + random(0, 2)\nend if\nvar arrival = now() + timespanfromseconds(@transitDays * 5400)\nvar expiry = @arrival + timespanfromseconds(40 * 5400)\nvar custody = tolocation(0)");
		foreach (var (office, index) in offices.Select((x, i) => (x, i)))
			send.AppendLine($"if (@destination == {index + 1})\ncustody = tolocation({custodyRooms[office.NpcVnum].Id})\nend if");
		send.AppendLine($"var originCustody = tolocation(0)");
		foreach (var (office, index) in offices.Select((x, i) => (x, i)))
			send.AppendLine($"if (@office == {index + 1})\noriginCustody = tolocation({custodyRooms[office.NpcVnum].Id})\nend if");
		send.AppendLine($"""
if (getregister(@item, "{PostalParcelRecipientRegister}") > 0)
	send(@actor, "That parcel already belongs to an outstanding postal consignment.")
	return true
end if
setregister @item "{PostalParcelAddressRegister}" @address
setregister @item "{PostalParcelRecipientRegister}" @recipient
setregister @item "{PostalParcelDestinationRegister}" @destination
setregister @item "{PostalParcelSenderRegister}" @actor.identityid
setregister @item "{PostalParcelArrivalRegister}" @arrival
setregister @item "{PostalParcelExpiryRegister}" @arrival
setregister @item "{PostalParcelInTransitRegister}" 0
if (@office == @destination)
	setregister @item "{PostalParcelExpiryRegister}" (@arrival + timespanfromseconds(40 * 5400))
	setregister @item "{PostalParcelInTransitRegister}" 2
end if
if (not(purchasehelditemcustody(@actor, @seller, @item, @originCustody, tocurrency({_currency.Id}), @price)))
	setregister @item "{PostalParcelAddressRegister}" ""
	setregister @item "{PostalParcelRecipientRegister}" 0
	setregister @item "{PostalParcelDestinationRegister}" 0
	setregister @item "{PostalParcelSenderRegister}" 0
	setregister @item "{PostalParcelArrivalRegister}" now()
	setregister @item "{PostalParcelExpiryRegister}" now()
	setregister @item "{PostalParcelInTransitRegister}" 0
	send(@actor, "The postmaster cannot accept that parcel or settle the postage.")
	return true
end if
send(@actor, "The postmaster accepts your parcel for the registered address " + @address + ".")
sendloc(@actor.location, "$0 hands a parcel to $1 for registered post.", @actor, @seller)
return true
""");
		var sending = Prog("ArmageddonWorldfilePostalSend", send.ToString(), ProgVariableTypes.Boolean,
			("actor", ProgVariableTypes.Character), ("npc", ProgVariableTypes.Perceivable), ("command", ProgVariableTypes.Text),
			("args", ProgVariableTypes.Text | ProgVariableTypes.Collection));
		InstallNpcInputHook("Postal Send", "mail", sending, offices.Select(x => x.NpcVnum).ToArray());
		InstallPostalCollection(offices, custodyRooms);
		InstallPostalDeliverySchedule(offices, custodyRooms);
		InstallPostalCouriers(offices, custodyRooms, sourceWeightUnits);

		Record("npc-service.physical-post", "NativePhysicalPostalService", null, null, new
		{
			Offices = offices, CustodyRooms = custodyRooms.ToDictionary(x => x.Key, x => x.Value.UniqueName),
			SourceRouteMatrix = offices.SelectMany((origin, from) => offices.Select((destination, to) => new { Origin = origin.Post, Destination = destination.Post, Route = SourcePostalRoutes[from, to] })).ToArray(), AddressIdentity = "CharacterIdentityId, stored in a central ordinary-room numeric dictionary with a separate destination-office dictionary; character address is removable/re-registerable.",
			Postage = "Source item weight is stored as source integer hundredths of a kilogram. The native item prototype's own weight (excluding contents, as source calc_object_weight does) is converted to source units and rounded to the nearest source integer; fee = source route sid rate * (weight + 1).",
			Arrival = "Cross-office items enter transit at the next source delivery pulse; a single reliability roll per outgoing origin/destination batch either moves the whole pending batch into transit or deletes it as source does. Each parcel retains its own speed + 0-2 mud-day arrival time. Source mail dates use the Zalanthan time clock: nine hours of 600 real seconds per day (5400 real seconds per mail-calendar day). Receiving-bin expiry is 40 mud days after arrival.",
			Disposition = "Native unconnected custody rooms hold real parcels and nested contents. Only the four source postmasters are attached; orphan NPC 20053 receives no invented office. Registration, assessment, outgoing postage, recipient pickup and source-clock batch transit are installed. Clan 28 leaders use the separate source courier hook; Bram receives no exception."
		});
		_counts["postal offices"] = offices.Length;
		_counts["postal custody rooms"] = custodyRooms.Count;
		Flush();
	}

	private void AppendPostalSourceOffice(StringBuilder body, PostalOffice[] offices)
	{
		foreach (var (office, index) in offices.Select((x, i) => (x, i)))
			body.AppendLine($"if (@seller.location == tolocation({_rooms[office.RoomVnum].Id}))\noffice = {index + 1}\nend if");
	}

	private void AppendPostalRouteMatrix(StringBuilder body, PostalOffice[] offices)
	{
		for (var from = 0; from < offices.Length; from++)
		for (var to = 0; to < offices.Length; to++)
		{
			var route = SourcePostalRoutes[from, to];
			body.AppendLine($"if (@office == {from + 1} and @destination == {to + 1})\ndays = {route.DeliveryDays}\nreliability = {route.ReliabilityPercent}\npriceRate = {route.BitsPerSourceWeightUnit}\nend if");
		}
	}

	private void EnsurePostalRegister(ProgVariableTypes ownerType, string name, ProgVariableTypes valueType, string defaultValue)
	{
		var owner = ownerType.ToStorageString();
		var definitions = _db.VariableDefinitions.Where(x => x.OwnerTypeDefinition == owner && x.Property == name).ToArray();
		if (definitions.Length > 1) throw new InvalidOperationException($"Postal register {name} is ambiguous.");
		if (definitions.Length == 1)
		{
			if (definitions[0].ContainedTypeDefinition != valueType.ToStorageString())
				throw new InvalidOperationException($"Postal register {name} already exists with an incompatible type.");
		}
		else
		{
			_db.VariableDefinitions.Add(new VariableDefinition
			{
				OwnerTypeDefinition = owner, Property = name, ContainedTypeDefinition = valueType.ToStorageString()
			});
		}
		var defaults = _db.VariableDefaults.Where(x => x.OwnerTypeDefinition == owner && x.Property == name).ToArray();
		if (defaults.Length > 1) throw new InvalidOperationException($"Postal register {name} has ambiguous defaults.");
		if (defaults.Length == 0)
			_db.VariableDefaults.Add(new VariableDefault { OwnerTypeDefinition = owner, Property = name, DefaultValue = defaultValue });
	}

	private void InstallPostalCollection(PostalOffice[] offices, IReadOnlyDictionary<int, Room> custodyRooms)
	{
		foreach (var office in offices)
		{
			var body = new StringBuilder(VendorAdmission + "\n");
			body.AppendLine("if (@args.Count(word, true) != 1 or lowercase(getindex(@args, 0)) != \"mail\")\nreturn false\nend if");
			body.AppendLine($"var room = tolocation({custodyRooms[office.NpcVnum].Id})\nvar found = false\nvar received = false\nvar expired = false");
			body.AppendLine($"foreach (parcel in @room.items)\nif (getregister(@parcel, \"{PostalParcelRecipientRegister}\") == @actor.identityid and getregister(@parcel, \"{PostalParcelInTransitRegister}\") == 2)\nfound = true");
			body.AppendLine($"var expiry = getregister(@parcel, \"{PostalParcelExpiryRegister}\")\nif (@expiry <= now())\ndeleteitem(@parcel)\nexpired = true\nelseif (getregister(@parcel, \"{PostalParcelArrivalRegister}\") <= now())");
			body.AppendLine($"if (purchaseitemdelivery(@actor, @seller, @parcel, @room, tocurrency({_currency.Id}), 0))\nsilentget(@actor, @parcel)\nsetregister @parcel \"{PostalParcelRecipientRegister}\" 0\nsetregister @parcel \"{PostalParcelDestinationRegister}\" 0\nsetregister @parcel \"{PostalParcelSenderRegister}\" 0\nsetregister @parcel \"{PostalParcelInTransitRegister}\" 0\nreceived = true\nend if\nend if\nend if\nend foreach");
			body.AppendLine("if (@received)\nsend(@actor, \"You collect the parcels addressed to you.\")\nreturn true\nend if\nif (@expired)\nsend(@actor, \"Some unclaimed parcels for your address had expired.\")\nreturn true\nend if\nif (@found)\nsend(@actor, \"Your parcels have not arrived yet.\")\nreturn true\nend if\nsend(@actor, \"There is no mail waiting for your registered address here.\")\nreturn true");
			var prog = Prog("ArmageddonWorldfilePostalCollect" + office.City, body.ToString(), ProgVariableTypes.Boolean,
				("actor", ProgVariableTypes.Character), ("npc", ProgVariableTypes.Perceivable), ("command", ProgVariableTypes.Text),
				("args", ProgVariableTypes.Text | ProgVariableTypes.Collection));
			InstallNpcInputHook("Postal Collection " + office.City, "get", prog, [office.NpcVnum]);
		}
	}

	private void InstallPostalDeliverySchedule(PostalOffice[] offices, IReadOnlyDictionary<int, Room> custodyRooms)
	{
		var transit = CreateAuxiliaryRoom(_source.Rooms.Single(x => x.Vnum == offices[0].RoomVnum).Zone,
			"postal-transit", "Postal Transit Store", "A secured store for parcels travelling between post offices.");
		var dispatchRoom = _rooms[offices[0].RoomVnum];
		var routeTickOrder = new (int From, int To)[]
		{
			(2, 1), (3, 1), (4, 1), (1, 2), (2, 2), (3, 2), (4, 2), (1, 3),
			(2, 3), (3, 3), (4, 3), (1, 4), (2, 4), (3, 4), (4, 4), (1, 1)
		};
		var body = new StringBuilder($"var transit = tolocation({transit.Id})\nvar dispatchRoom = tolocation({dispatchRoom.Id})\n");
		body.AppendLine($"var cursor = getregister(@dispatchRoom, \"{PostalDeliveryCursorRegister}\")");
		body.AppendLine("if (@cursor < 0 or @cursor > 15)\ncursor = 0\nend if\nvar routeFrom = 0\nvar routeTo = 0\nvar routeReliability = 0");
		foreach (var (pair, index) in routeTickOrder.Select((x, i) => (x, i)))
		{
			var route = SourcePostalRoutes[pair.From - 1, pair.To - 1];
			body.AppendLine($"if (@cursor == {index})\nrouteFrom = {pair.From}\nrouteTo = {pair.To}\nrouteReliability = {route.ReliabilityPercent}\nend if");
		}
		body.AppendLine("var nextCursor = @cursor + 1\nif (@nextCursor > 15)\nnextCursor = 0\nend if");
		body.AppendLine($"setregister @dispatchRoom \"{PostalDeliveryCursorRegister}\" @nextCursor\nvar origin = tolocation(0)");
		foreach (var (office, index) in offices.Select((x, i) => (x, i)))
			body.AppendLine($"if (@routeFrom == {index + 1})\norigin = tolocation({custodyRooms[office.NpcVnum].Id})\nend if");
		body.AppendLine($"var pending = @origin.items.Where(parcel, getregister(@parcel, \"{PostalParcelDestinationRegister}\") == @routeTo and getregister(@parcel, \"{PostalParcelInTransitRegister}\") == 0)");
		body.AppendLine($"if (@routeFrom != @routeTo and collectioncount(@pending) > 0)\nif (random(1, 100) <= @routeReliability)\nforeach (parcel in @pending)\nif (put(@parcel, @transit))\nsetregister @parcel \"{PostalParcelInTransitRegister}\" 1\nend if\nend foreach\nelse\nforeach (parcel in @pending)\ndeleteitem(@parcel)\nend foreach\nend if\nend if");
		body.AppendLine($"foreach (parcel in @transit.items)\nif (getregister(@parcel, \"{PostalParcelArrivalRegister}\") <= now())\nvar destination = getregister(@parcel, \"{PostalParcelDestinationRegister}\")");
		foreach (var (office, index) in offices.Select((x, i) => (x, i)))
			body.AppendLine($"if (@destination == {index + 1})\nif (put(@parcel, tolocation({custodyRooms[office.NpcVnum].Id})))\nsetregister @parcel \"{PostalParcelExpiryRegister}\" (now() + timespanfromseconds(40 * 5400))\nsetregister @parcel \"{PostalParcelInTransitRegister}\" 2\nend if\nend if");
		body.AppendLine("end if\nend foreach");
		foreach (var office in offices)
			body.AppendLine($"foreach (parcel in tolocation({custodyRooms[office.NpcVnum].Id}).items)\nif (getregister(@parcel, \"{PostalParcelInTransitRegister}\") == 2 and getregister(@parcel, \"{PostalParcelRecipientRegister}\") > 0 and getregister(@parcel, \"{PostalParcelExpiryRegister}\") <= now())\ndeleteitem(@parcel)\nend if\nend foreach");
		body.AppendLine("return true");
		var controller = Prog("ArmageddonWorldfilePostalDelivery", body.ToString(), ProgVariableTypes.Boolean);

		var calendar = _db.Calendars.Single(x => x.Id == _db.Cultures.Single(c => c.Id ==
			_settings.NativeBindings[ArmageddonNativeBinding.Culture]).PrimaryCalendarId);
		var clock = _db.Clocks.Single(x => x.Id == calendar.FeedClockId);
		var clockDefinition = System.Xml.Linq.XElement.Parse(clock.Definition);
		var secondsPerMinute = (int)clockDefinition.Element("SecondsPerMinute")!;
		var rate = SourceLawClockRate();
		var minutes = checked((int)Math.Max(1, Math.Ceiling(101.25 * rate / secondsPerMinute)));
		var timezone = _db.Timezones.Single(x => x.Id == clock.PrimaryTimezoneId);
		var schedule = new ProgSchedule
		{
			Id = Next<ProgSchedule>(), Name = "Armageddon physical post transit", IntervalType = (int)IntervalType.Minutely,
			IntervalModifier = minutes, FutureProgId = controller.Id,
			ReferenceDate = $"{calendar.Id}_{calendar.Date}_{clock.Id}_{timezone.Name} {clock.Hours}:{clock.Minutes}:{clock.Seconds}",
			ReferenceTime = string.Empty
		};
		_db.ProgSchedules.Add(schedule);
		Record("npc-service.physical-post-delivery", nameof(ProgSchedule), schedule.Id, null,
			new { TransitRoom = transit.UniqueName, ScheduleMinutes = minutes, SourcePulseSeconds = 101.25,
				Reliability = "One cyclic origin/destination route is processed per source delivery pulse, one reliability roll applies to that route's whole pending batch, and failed batches are lost. Transit parcels are never rerolled.",
				Disposition = "Source cadence is approximated to the nearest native clock minute. Zalanthan mail calendar days are 5400 real seconds; 40-day expiry applies after arrival." });
	}
}
