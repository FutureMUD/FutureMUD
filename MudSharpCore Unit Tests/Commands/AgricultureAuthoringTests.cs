#nullable enable

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Commands.Modules;
using MudSharp.Framework;
using MudSharp.Form.Material;
using MudSharp.Work.Agriculture;

namespace MudSharp_Unit_Tests.Commands;

[TestClass]
[DoNotParallelize]
public class AgricultureAuthoringTests
{
	[TestMethod]
	public void CommodityEditors_ValidateIndexedChangesAndPersistCropSeedsAndWoodlandOutputs()
	{
		using var database = new InMemoryDatabaseTestScope();
		var world = BuilderWorkflowRegressionTests.World();
		var materials = new All<ISolid>(); materials.Add(Mock.Of<ISolid>(x => x.Id == 1 && x.Name == "plant matter" && x.Names == new[] { "plant matter" }));
		world.SetupGet(x => x.Materials).Returns(materials);
		var actorMock = BuilderWorkflowRegressionTests.Actor(world.Object);
		actorMock.Setup(x => x.IsAdministrator(MudSharp.Accounts.PermissionLevel.JuniorAdmin)).Returns(true);
		var actor = actorMock.Object;
		var cropRow = new MudSharp.Models.AgricultureCropDefinition { Id = 1, Name = "Crop", Category = "grain", Description = "A crop", Definition = "<Crop />" };
		var woodRow = new MudSharp.Models.AgricultureWoodlandDefinition { Id = 1, Name = "Wood", WoodlandType = "forest", Description = "A woodland", Definition = "<Woodland />" };
		database.Context.AgricultureCropDefinitions.Add(cropRow); database.Context.AgricultureWoodlandDefinitions.Add(woodRow); database.Context.SaveChanges();
		var crop = new AgricultureCropDefinition(cropRow, world.Object);
		var wood = new AgricultureWoodlandDefinition(woodRow, world.Object);
		var crops = new All<IAgricultureCropDefinition>(); crops.Add(crop);
		var woods = new All<IAgricultureWoodlandDefinition>(); woods.Add(wood);
		world.SetupGet(x => x.AgricultureCropDefinitions).Returns(crops);
		world.SetupGet(x => x.AgricultureWoodlandDefinitions).Returns(woods);
		foreach (var command in new[] { "Crop output add 100 \"plant matter\"", "Crop seeds add 5 \"plant matter\"", "Crop output set 1 125 \"plant matter\"", "Crop output set 1 NaN \"plant matter\"", "Crop output remove 2" })
			BuilderWorkflowRegressionTests.Invoke(typeof(AgricultureModule), "SetCrop", actor, new StringStack(command));
		BuilderWorkflowRegressionTests.Invoke(typeof(AgricultureModule), "SetWoodland", actor, new StringStack("Wood output add 250 \"plant matter\""));
		crop.Save(); wood.Save(); database.Context.SaveChanges(); database.Context.ChangeTracker.Clear();
		var loadedCrop = new AgricultureCropDefinition(database.Context.AgricultureCropDefinitions.Single(), world.Object);
		var loadedWood = new AgricultureWoodlandDefinition(database.Context.AgricultureWoodlandDefinitions.Single(), world.Object);
		Assert.AreEqual(125.0, loadedCrop.YieldOutputs.Single().BaseWeight);
		Assert.AreEqual(5.0, loadedCrop.SeedRequirements.Single().BaseWeight);
		Assert.AreEqual("plant matter", loadedCrop.YieldOutputs.Single().MaterialName);
		Assert.AreEqual(250.0, loadedWood.YieldOutputs.Single().BaseWeight);
	}

	[TestMethod]
	public void ApiaryEditor_SavesInstallTendingAndProductiveHarvestSettings()
	{
		using var database = new InMemoryDatabaseTestScope();
		var world = BuilderWorkflowRegressionTests.World();
		var materials = new All<ISolid>(); materials.Add(Mock.Of<ISolid>(x => x.Id == 1 && x.Name == "honey" && x.Names == new[] { "honey" }));
		world.SetupGet(x => x.Materials).Returns(materials);
		var actorMock = BuilderWorkflowRegressionTests.Actor(world.Object);
		actorMock.Setup(x => x.IsAdministrator(MudSharp.Accounts.PermissionLevel.JuniorAdmin)).Returns(true);
		var actor = actorMock.Object;
		var row = new MudSharp.Models.AgricultureOperation { Id = 1, Name = "Apiary", Description = "Harvest", OperationType = (int)AgricultureOperationType.HarvestApiary, Definition = "<Operation />" };
		database.Context.AgricultureOperations.Add(row); database.Context.SaveChanges();
		var operation = new AgricultureOperation(row, world.Object);
		var operations = new All<IAgricultureOperation>(); operations.Add(operation); world.SetupGet(x => x.AgricultureOperations).Returns(operations);
		foreach (var command in new[] { "Apiary apiaryinstall 3 2", "Apiary apiarytend 4 5 6", "Apiary apiaryyield 1.5 10 add 100 honey", "Apiary apiaryyield NaN 10 clear" })
			BuilderWorkflowRegressionTests.Invoke(typeof(AgricultureModule), "SetOperation", actor, new StringStack(command));
		operation.Save(); database.Context.SaveChanges(); database.Context.ChangeTracker.Clear();
		var loaded = new AgricultureOperation(database.Context.AgricultureOperations.Single(), world.Object);
		Assert.AreEqual(3, loaded.ApiaryInstallHiveCount);
		Assert.AreEqual(1.5, loaded.ApiaryYieldMultiplier);
		Assert.AreEqual(100.0, loaded.ApiaryYieldOutputs.Single().BaseWeight);
		Assert.AreEqual(6, loaded.ApiaryTendYieldDelta);
	}
}
