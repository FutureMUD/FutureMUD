using MySql.Data.MySqlClient;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private static void PrepareFullBootPrototypes(TestDatabase database, Action<string, string, Dictionary<string, string>> clone)
	{
		using var connection = database.OpenOwnedConnection();
		long Scalar(string sql) => Convert.ToInt64(new MySqlCommand(sql, connection).ExecuteScalar());
		void Sql(string sql) => new MySqlCommand(sql, connection).ExecuteNonQuery();
		const string stockWhere = "UniqueName='medieval_industry_stock_book_board_pair' AND EditableItemId IN(SELECT Id FROM editableitems WHERE RevisionStatus=4)";
		Require(Scalar("SELECT COUNT(*) FROM gameitemprotos WHERE " + stockWhere) == 1, "Expected one simple stock board prototype.");
		var terrain = Scalar("SELECT Id FROM terrains WHERE Name='Vehicle Interior'");
		for (var i = 0; i < 7; i++) Sql($"INSERT INTO editableitems(Id,RevisionNumber,RevisionStatus,BuilderAccountId,BuilderDate) VALUES({900000 + i},0,4,0,UTC_TIMESTAMP())");
		Sql($"""
			INSERT INTO vehicleprotos(Id,RevisionNumber,EditableItemId,Name,Description,VehicleScale) VALUES(900000,0,900000,'Cell lifecycle platform','Disposable room-scale retirement fixture.',2);
			INSERT INTO vehiclecompartmentprotos(Id,VehicleProtoId,VehicleProtoRevision,Name,Description,DisplayOrder,InteriorTerrainId,InteriorOutdoorsType) VALUES(900000,900000,0,'control','A control room.',0,{terrain},0),(900001,900000,0,'deckhouse','A deckhouse.',1,{terrain},0);
			INSERT INTO vehiclecompartmentlinkprotos(VehicleProtoId,VehicleProtoRevision,SourceVehicleCompartmentProtoId,DestinationVehicleCompartmentProtoId,OutboundDirection,InboundDirection,OutboundDescription,InboundDescription) VALUES(900000,0,900000,900001,'aft','forward','the deckhouse','the control room');
			INSERT INTO vehicleoccupantslotprotos(Id,VehicleProtoId,VehicleProtoRevision,VehicleCompartmentProtoId,Name,SlotType,Capacity,RequiredForMovement,ContributesToPropulsion,BoatStabilityDifficulty) VALUES(900000,900000,0,900000,'driver',1,1,0,0,5),(900001,900000,0,900001,'passenger',0,2,0,0,5);
			INSERT INTO vehiclecontrolstationprotos(VehicleProtoId,VehicleProtoRevision,VehicleOccupantSlotProtoId,Name,IsPrimary) VALUES(900000,0,900000,'controls',1);
			INSERT INTO vehiclemovementprofileprotos(VehicleProtoId,VehicleProtoRevision,Name,MovementType,MovementEnvironment,ExposesOccupantsToWater,IsDefault,RequiredPowerSpikeInWatts,MinimumEnginePowerInWatts,FuelVolumePerMove,RequiredInstalledRole,RequiresTowLinksClosed,RequiresAccessPointsClosed,RouteSpeedMetresPerSecond,RoutePropulsionMode,RouteFuelVolumePerMetre,RoutePowerDrawWatts,AutomaticOperationCapable) VALUES(900000,0,'ordinary',0,0,0,1,0,0,0,'',0,0,0,0,0,0,0);
			INSERT INTO vehicleaccesspointprotos(Id,VehicleProtoId,VehicleProtoRevision,VehicleCompartmentProtoId,Name,Description,AccessPointType,StartsOpen,MustBeClosedForMovement,DisplayOrder) VALUES(900000,900000,0,900001,'ramp','a loading ramp',2,1,0,0);
			INSERT INTO gameitemcomponentprotos(Id,RevisionNumber,EditableItemId,Name,Type,Description,Definition) VALUES
			(900000,0,900001,'Lifecycle exterior','Vehicle Exterior','Exterior fixture','<Definition><VehiclePrototypeId>900000</VehiclePrototypeId></Definition>'),
			(900001,0,900002,'Lifecycle access','Vehicle Access Point','Access fixture','<Definition><VehiclePrototypeId>900000</VehiclePrototypeId><AccessPointPrototypeId>900000</AccessPointPrototypeId></Definition>'),
			(900002,0,900003,'Lifecycle dwelling','Dwelling','Persisted fixture','<Definition><TemplateEntryCell>8101</TemplateEntryCell><DoorProto>0</DoorProto><DoorSize>7</DoorSize><EntranceKeyword>building</EntranceKeyword><EntranceDescription>the building</EntranceDescription></Definition>');
			""");
		for (var i = 0; i < 3; i++)
		{
			var name = new[] { "hull", "ramp", "dwelling" }[i];
			clone("gameitemprotos", stockWhere, new() { ["Id"] = (900000 + i).ToString(), ["RevisionNumber"] = "0", ["EditableItemId"] = (900004 + i).ToString(), ["Name"] = $"'Qualification {name}'", ["UniqueName"] = $"'acceptance_cell_lifecycle_{name}'", ["Keywords"] = $"'{name}'", ["ShortDescription"] = $"'a qualification {name}'", ["FullDescription"] = $"'A qualification {name}.'" });
		}
		Sql("INSERT INTO gameitemprotos_gameitemcomponentprotos(GameItemProtoId,GameItemProtoRevision,GameItemComponentProtoId,GameItemComponentRevision) VALUES(900000,0,900000,0),(900001,0,900001,0),(900002,0,900002,0); UPDATE vehicleprotos SET ExteriorItemProtoId=900000,ExteriorItemProtoRevision=0 WHERE Id=900000 AND RevisionNumber=0; UPDATE vehicleaccesspointprotos SET ProjectionItemProtoId=900001,ProjectionItemProtoRevision=0 WHERE Id=900000");
		Sql("INSERT INTO gameitems(Id,Quality,GameItemProtoId,GameItemProtoRevision,RoomLayer,`Condition`,MaterialId,Size,PositionId,PositionModifier,EffectData) SELECT 900000,5,900002,0,0,1,MaterialId,Size,1,0,'<Effects/>' FROM gameitemprotos WHERE Id=900002 AND RevisionNumber=0; INSERT INTO gameitemcomponents(GameItemId,GameItemComponentProtoId,GameItemComponentProtoRevision,Definition) VALUES(900000,900002,0,'<Definition><EntranceCell>8103</EntranceCell></Definition>'); INSERT INTO cells_gameitems(CellId,GameItemId) VALUES(8101,900000)");
	}
}
