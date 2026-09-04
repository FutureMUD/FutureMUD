#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Celestial;
using MudSharp.Character;
using MudSharp.Commands.Modules;
using MudSharp.Communication;
using MudSharp.Communication.Language;
using MudSharp.Construction.Grids;
using MudSharp.Economy;
using MudSharp.Economy.Property;
using MudSharp.Form.Characteristics;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.NPC.Templates;
using MudSharp.PerceptionEngine;
using MudSharp.Vehicles;
using MudSharp.Work.Agriculture;

namespace MudSharp_Unit_Tests;

[TestClass]
public class VariableRegisterReferenceValueTests
{
	[TestMethod]
	public void SupportsPersistentStorage_AcceptsDurableReferencesAndRejectsRuntimeScopedValues()
	{
		Assert.IsTrue(VariableRegister.SupportsPersistentStorage(ProgVariableTypes.Market));
		Assert.IsTrue(VariableRegister.SupportsPersistentStorage(ProgVariableTypes.AgricultureField));
		Assert.IsTrue(VariableRegister.SupportsPersistentStorage(ProgVariableTypes.VehicleRoute));
		Assert.IsTrue(VariableRegister.SupportsPersistentStorage(ProgVariableTypes.SignedLanguage));
		Assert.IsTrue(VariableRegister.SupportsPersistentStorage(
			ProgVariableTypes.Market | ProgVariableTypes.Collection));
		foreach (var type in PhaseOneTypes)
		{
			Assert.IsTrue(VariableRegister.SupportsPersistentStorage(type), type.Describe());
		}
		foreach (var type in PhaseTwoTypes)
		{
			Assert.IsTrue(VariableRegister.SupportsPersistentStorage(type), type.Describe());
		}

		Assert.IsFalse(VariableRegister.SupportsPersistentStorage(ProgVariableTypes.Chargen));
		Assert.IsFalse(VariableRegister.SupportsPersistentStorage(ProgVariableTypes.Exit));
		Assert.IsFalse(VariableRegister.SupportsPersistentStorage(ProgVariableTypes.Effect));
		Assert.IsFalse(VariableRegister.SupportsPersistentStorage(ProgVariableTypes.Outfit));
		Assert.IsFalse(VariableRegister.SupportsPersistentStorage(ProgVariableTypes.OutfitItem));
		Assert.IsFalse(VariableRegister.SupportsPersistentStorage(ProgVariableTypes.Trap));
		Assert.IsFalse(VariableRegister.SupportsPersistentStorage(
			ProgVariableTypes.Outfit | ProgVariableTypes.Collection));
	}

	[TestMethod]
	public void ReferenceVariableValue_DurableProgTypes_ResolveFromTheirGameworldRegistries()
	{
		var gameworld = new Mock<IFuturemud>();
		var market = CreateFrameworkItem<IMarket>(101);
		var marketCategory = CreateFrameworkItem<IMarketCategory>(102);
		var field = CreateFrameworkItem<IAgricultureField>(103);
		var route = CreateFrameworkItem<IVehicleRoute>(104);
		var service = CreateFrameworkItem<IVehicleService>(105);
		var journey = CreateFrameworkItem<IVehicleJourney>(106);
		var skillPackage = CreateFrameworkItem<INPCSkillPackage>(107);
		var signedLanguage = CreateFrameworkItem<ISignedLanguage>(108);
		var signedVariety = CreateFrameworkItem<ISignedLanguageVariety>(109);

		var markets = new All<IMarket>();
		markets.Add(market.Object);
		gameworld.SetupGet(x => x.Markets).Returns(markets);

		var marketCategories = new All<IMarketCategory>();
		marketCategories.Add(marketCategory.Object);
		gameworld.SetupGet(x => x.MarketCategories).Returns(marketCategories);

		var fields = new All<IAgricultureField>();
		fields.Add(field.Object);
		gameworld.SetupGet(x => x.AgricultureFields).Returns(fields);

		var routes = new Mock<IUneditableRevisableAll<IVehicleRoute>>();
		routes.Setup(x => x.Get(route.Object.Id)).Returns(route.Object);
		gameworld.SetupGet(x => x.VehicleRoutes).Returns(routes.Object);

		var services = new All<IVehicleService>();
		services.Add(service.Object);
		gameworld.SetupGet(x => x.VehicleServices).Returns(services);

		var journeys = new All<IVehicleJourney>();
		journeys.Add(journey.Object);
		gameworld.SetupGet(x => x.VehicleJourneys).Returns(journeys);

		var skillPackages = new All<INPCSkillPackage>();
		skillPackages.Add(skillPackage.Object);
		gameworld.SetupGet(x => x.NpcSkillPackages).Returns(skillPackages);

		signedLanguage.SetupGet(x => x.Varieties).Returns([signedVariety.Object]);
		var signedLanguages = new All<ISignedLanguage>();
		signedLanguages.Add(signedLanguage.Object);
		gameworld.SetupGet(x => x.SignedLanguages).Returns(signedLanguages);

		Assert.AreSame(market.Object, Resolve(gameworld.Object, ProgVariableTypes.Market, market.Object.Id));
		Assert.AreSame(marketCategory.Object,
			Resolve(gameworld.Object, ProgVariableTypes.MarketCategory, marketCategory.Object.Id));
		Assert.AreSame(field.Object, Resolve(gameworld.Object, ProgVariableTypes.AgricultureField, field.Object.Id));
		Assert.AreSame(route.Object, Resolve(gameworld.Object, ProgVariableTypes.VehicleRoute, route.Object.Id));
		Assert.AreSame(service.Object, Resolve(gameworld.Object, ProgVariableTypes.VehicleService, service.Object.Id));
		Assert.AreSame(journey.Object, Resolve(gameworld.Object, ProgVariableTypes.VehicleJourney, journey.Object.Id));
		Assert.AreSame(skillPackage.Object,
			Resolve(gameworld.Object, ProgVariableTypes.NPCSkillPackage, skillPackage.Object.Id));
		Assert.AreSame(signedLanguage.Object,
			Resolve(gameworld.Object, ProgVariableTypes.SignedLanguage, signedLanguage.Object.Id));
		Assert.AreSame(signedVariety.Object,
			Resolve(gameworld.Object, ProgVariableTypes.SignedVariety, signedVariety.Object.Id));
	}

	[TestMethod]
	public void ReferenceVariableValue_PhaseOneProgTypes_ResolveFromTheirGameworldRegistries()
	{
		var gameworld = new Mock<IFuturemud>();
		var tag = CreateFrameworkItem<ITag>(201);
		var itemPrototype = CreateFrameworkItem<IGameItemProto>(202);
		var npcTemplate = CreateFrameworkItem<INPCTemplate>(203);
		var outfitTemplate = CreateFrameworkItem<IOutfitTemplate>(204);
		var vehicle = CreateFrameworkItem<IVehicle>(205);
		var celestial = CreateFrameworkItem<ICelestialObject>(206);
		var grid = CreateFrameworkItem<IGrid>(207);
		var characteristic = CreateFrameworkItem<ICharacteristicDefinition>(208);
		var characteristicValue = CreateFrameworkItem<ICharacteristicValue>(209);
		var fieldProfile = CreateFrameworkItem<IAgricultureFieldProfile>(210);
		var crop = CreateFrameworkItem<IAgricultureCropDefinition>(211);
		var herd = CreateFrameworkItem<IAgricultureHerdDefinition>(212);
		var woodland = CreateFrameworkItem<IAgricultureWoodlandDefinition>(213);
		var operation = CreateFrameworkItem<IAgricultureOperation>(214);

		gameworld.SetupGet(x => x.Tags).Returns(CreateRegistry(tag.Object).Object);
		gameworld.SetupGet(x => x.ItemProtos).Returns(CreateRevisableRegistry(itemPrototype.Object).Object);
		gameworld.SetupGet(x => x.NpcTemplates).Returns(CreateRevisableRegistry(npcTemplate.Object).Object);
		gameworld.SetupGet(x => x.OutfitTemplates).Returns(CreateRegistry(outfitTemplate.Object).Object);
		gameworld.SetupGet(x => x.Vehicles).Returns(CreateRegistry(vehicle.Object).Object);
		gameworld.SetupGet(x => x.CelestialObjects).Returns(CreateRegistry(celestial.Object).Object);
		gameworld.SetupGet(x => x.Grids).Returns(CreateRegistry(grid.Object).Object);
		gameworld.SetupGet(x => x.Characteristics).Returns(CreateRegistry(characteristic.Object).Object);
		gameworld.SetupGet(x => x.CharacteristicValues).Returns(CreateRegistry(characteristicValue.Object).Object);
		gameworld.SetupGet(x => x.AgricultureFieldProfiles).Returns(CreateRegistry(fieldProfile.Object).Object);
		gameworld.SetupGet(x => x.AgricultureCropDefinitions).Returns(CreateRegistry(crop.Object).Object);
		gameworld.SetupGet(x => x.AgricultureHerdDefinitions).Returns(CreateRegistry(herd.Object).Object);
		gameworld.SetupGet(x => x.AgricultureWoodlandDefinitions).Returns(CreateRegistry(woodland.Object).Object);
		gameworld.SetupGet(x => x.AgricultureOperations).Returns(CreateRegistry(operation.Object).Object);

		foreach (var (type, item) in new (ProgVariableTypes Type, IFrameworkItem Item)[]
		         {
			         (ProgVariableTypes.Tag, tag.Object),
			         (ProgVariableTypes.ItemPrototype, itemPrototype.Object),
			         (ProgVariableTypes.NPCTemplate, npcTemplate.Object),
			         (ProgVariableTypes.OutfitTemplate, outfitTemplate.Object),
			         (ProgVariableTypes.Vehicle, vehicle.Object),
			         (ProgVariableTypes.CelestialObject, celestial.Object),
			         (ProgVariableTypes.Grid, grid.Object),
			         (ProgVariableTypes.CharacteristicDefinition, characteristic.Object),
			         (ProgVariableTypes.CharacteristicValue, characteristicValue.Object),
			         (ProgVariableTypes.AgricultureFieldProfile, fieldProfile.Object),
			         (ProgVariableTypes.AgricultureCropDefinition, crop.Object),
			         (ProgVariableTypes.AgricultureHerdDefinition, herd.Object),
			         (ProgVariableTypes.AgricultureWoodlandDefinition, woodland.Object),
			         (ProgVariableTypes.AgricultureOperation, operation.Object)
		         })
		{
			Assert.AreSame(item, Resolve(gameworld.Object, type, item.Id), type.Describe());
		}

		var output = new Mock<IOutputHandler>();
		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		actor.SetupGet(x => x.OutputHandler).Returns(output.Object);
		foreach (var (type, item) in new (ProgVariableTypes Type, IFrameworkItem Item)[]
		         {
			         (ProgVariableTypes.Tag, tag.Object),
			         (ProgVariableTypes.ItemPrototype, itemPrototype.Object),
			         (ProgVariableTypes.NPCTemplate, npcTemplate.Object),
			         (ProgVariableTypes.OutfitTemplate, outfitTemplate.Object),
			         (ProgVariableTypes.Vehicle, vehicle.Object),
			         (ProgVariableTypes.CelestialObject, celestial.Object),
			         (ProgVariableTypes.Grid, grid.Object),
			         (ProgVariableTypes.CharacteristicDefinition, characteristic.Object),
			         (ProgVariableTypes.CharacteristicValue, characteristicValue.Object),
			         (ProgVariableTypes.AgricultureFieldProfile, fieldProfile.Object),
			         (ProgVariableTypes.AgricultureCropDefinition, crop.Object),
			         (ProgVariableTypes.AgricultureHerdDefinition, herd.Object),
			         (ProgVariableTypes.AgricultureWoodlandDefinition, woodland.Object),
			         (ProgVariableTypes.AgricultureOperation, operation.Object)
		         })
		{
			var (argument, success) = ProgModule.GetArgument(type, item.Id.ToString(), 1, actor.Object);
			Assert.IsTrue(success, type.Describe());
			Assert.AreSame(item, argument, type.Describe());
		}
	}

	[TestMethod]
	public void ReferenceVariableValue_PhaseTwoProgTypes_ResolveFromPropertyCollectionsAndGameworldRegistries()
	{
		var gameworld = new Mock<IFuturemud>();
		var property = CreateFrameworkItem<IProperty>(301);
		var propertyKey = CreateFrameworkItem<IPropertyKey>(302);
		var expiredPropertyLease = CreateFrameworkItem<IPropertyLease>(303);
		var propertyLease = CreateFrameworkItem<IPropertyLease>(304);
		var expiredPropertyLeaseOrder = CreateFrameworkItem<IPropertyLeaseOrder>(305);
		var propertyLeaseOrder = CreateFrameworkItem<IPropertyLeaseOrder>(306);
		var propertySaleOrder = CreateFrameworkItem<IPropertySaleOrder>(307);
		var economicZone = CreateFrameworkItem<IEconomicZone>(308);
		var channel = CreateFrameworkItem<IChannel>(309);

		property.SetupGet(x => x.PropertyKeys).Returns([propertyKey.Object]);
		property.SetupGet(x => x.ExpiredLeases).Returns([expiredPropertyLease.Object]);
		property.SetupGet(x => x.Lease).Returns(propertyLease.Object);
		property.SetupGet(x => x.ExpiredLeaseOrders).Returns([expiredPropertyLeaseOrder.Object]);
		property.SetupGet(x => x.LeaseOrder).Returns(propertyLeaseOrder.Object);
		property.SetupGet(x => x.SaleOrder).Returns(propertySaleOrder.Object);

		var properties = new All<IProperty>();
		properties.Add(property.Object);
		gameworld.SetupGet(x => x.Properties).Returns(properties);
		gameworld.SetupGet(x => x.EconomicZones).Returns(CreateRegistry(economicZone.Object).Object);
		gameworld.SetupGet(x => x.Channels).Returns(CreateRegistry(channel.Object).Object);

		foreach (var (type, item) in new (ProgVariableTypes Type, IFrameworkItem Item)[]
		         {
			         (ProgVariableTypes.Property, property.Object),
			         (ProgVariableTypes.PropertyKey, propertyKey.Object),
			         (ProgVariableTypes.PropertyLease, propertyLease.Object),
			         (ProgVariableTypes.PropertyLeaseOrder, propertyLeaseOrder.Object),
			         (ProgVariableTypes.PropertySaleOrder, propertySaleOrder.Object),
			         (ProgVariableTypes.EconomicZone, economicZone.Object),
			         (ProgVariableTypes.Channel, channel.Object)
		         })
		{
			Assert.AreSame(item, Resolve(gameworld.Object, type, item.Id), type.Describe());
		}
		Assert.AreSame(expiredPropertyLease.Object,
			Resolve(gameworld.Object, ProgVariableTypes.PropertyLease, expiredPropertyLease.Object.Id));
		Assert.AreSame(expiredPropertyLeaseOrder.Object,
			Resolve(gameworld.Object, ProgVariableTypes.PropertyLeaseOrder, expiredPropertyLeaseOrder.Object.Id));

		var output = new Mock<IOutputHandler>();
		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		actor.SetupGet(x => x.OutputHandler).Returns(output.Object);
		foreach (var (type, item) in new (ProgVariableTypes Type, IFrameworkItem Item)[]
		         {
			         (ProgVariableTypes.Property, property.Object),
			         (ProgVariableTypes.PropertyKey, propertyKey.Object),
			         (ProgVariableTypes.PropertyLease, propertyLease.Object),
			         (ProgVariableTypes.PropertyLeaseOrder, propertyLeaseOrder.Object),
			         (ProgVariableTypes.PropertySaleOrder, propertySaleOrder.Object),
			         (ProgVariableTypes.EconomicZone, economicZone.Object),
			         (ProgVariableTypes.Channel, channel.Object)
		         })
		{
			var (argument, success) = ProgModule.GetArgument(type, item.Id.ToString(), 1, actor.Object);
			Assert.IsTrue(success, type.Describe());
			Assert.AreSame(item, argument, type.Describe());
		}
	}

	private static Mock<T> CreateFrameworkItem<T>(long id) where T : class, IFrameworkItem
	{
		var item = new Mock<T>();
		item.SetupGet(x => x.Id).Returns(id);
		item.SetupGet(x => x.Name).Returns($"{typeof(T).Name} {id}");
		item.SetupGet(x => x.FrameworkItemType).Returns(typeof(T).Name);
		return item;
	}

	private static IProgVariable? Resolve(IFuturemud gameworld, ProgVariableTypes type, long id)
	{
		return new VariableRegister.ReferenceVariableValue { Type = type, ID = id }.GetVariable(gameworld);
	}

	private static Mock<IUneditableAll<T>> CreateRegistry<T>(T item) where T : class, IFrameworkItem
	{
		var registry = new Mock<IUneditableAll<T>>();
		registry.Setup(x => x.Get(item.Id)).Returns(item);
		registry.Setup(x => x.GetByIdOrName(It.IsAny<string>(), It.IsAny<bool>()))
			.Returns((string value, bool _) => value == item.Id.ToString() || value.EqualTo(item.Name) ? item : null);
		return registry;
	}

	private static Mock<IUneditableRevisableAll<T>> CreateRevisableRegistry<T>(T item)
		where T : class, IFrameworkItem, IRevisableItem
	{
		var registry = new Mock<IUneditableRevisableAll<T>>();
		registry.Setup(x => x.Get(item.Id)).Returns(item);
		registry.Setup(x => x.GetByIdOrName(It.IsAny<string>(), It.IsAny<bool>()))
			.Returns((string value, bool _) => value == item.Id.ToString() || value.EqualTo(item.Name) ? item : null);
		return registry;
	}

	private static readonly ProgVariableTypes[] PhaseOneTypes =
	[
		ProgVariableTypes.Tag,
		ProgVariableTypes.ItemPrototype,
		ProgVariableTypes.NPCTemplate,
		ProgVariableTypes.OutfitTemplate,
		ProgVariableTypes.Vehicle,
		ProgVariableTypes.CelestialObject,
		ProgVariableTypes.Grid,
		ProgVariableTypes.CharacteristicDefinition,
		ProgVariableTypes.CharacteristicValue,
		ProgVariableTypes.AgricultureFieldProfile,
		ProgVariableTypes.AgricultureCropDefinition,
		ProgVariableTypes.AgricultureHerdDefinition,
		ProgVariableTypes.AgricultureWoodlandDefinition,
		ProgVariableTypes.AgricultureOperation
	];

	private static readonly ProgVariableTypes[] PhaseTwoTypes =
	[
		ProgVariableTypes.Property,
		ProgVariableTypes.PropertyKey,
		ProgVariableTypes.PropertyLease,
		ProgVariableTypes.PropertyLeaseOrder,
		ProgVariableTypes.PropertySaleOrder,
		ProgVariableTypes.EconomicZone,
		ProgVariableTypes.Channel
	];
}
