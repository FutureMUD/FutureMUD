#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Accounts;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Commands.Modules;
using MudSharp.Commands;
using MudSharp.Commands.Trees;
using MudSharp.Communication;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Form.Material;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.GameItems.Interfaces;
using MudSharp.Magic;
using MudSharp.Magic.SpellEffects;
using MudSharp.Magic.SpellTriggers;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Checks;
using MudSharp.Traps;
using MudSharp.Work.Agriculture;
using MudSharp.Work.Crafts;
using MudSharp.Work.Crafts.Inputs;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CommandExecutionSecurityTests
{
	[TestInitialize]
	public void TestInitialize()
	{
		SpellTriggerFactory.SetupFactory();
		SpellEffectFactory.SetupFactory();
	}

	[TestMethod]
	public void CommandExecutionSecurity_CanForceTarget_AppliesAdminRankRules()
	{
		var actor = Character(PermissionLevel.Admin).Object;
		var lowerAdmin = Character(PermissionLevel.JuniorAdmin).Object;
		var sameAdmin = Character(PermissionLevel.Admin).Object;
		var higherAdmin = Character(PermissionLevel.SeniorAdmin).Object;
		var player = Character(PermissionLevel.Player).Object;
		var founder = Character(PermissionLevel.Founder).Object;

		Assert.IsTrue(CommandExecutionGuards.CanForceTarget(actor, lowerAdmin));
		Assert.IsTrue(CommandExecutionGuards.CanForceTarget(actor, player));
		Assert.IsFalse(CommandExecutionGuards.CanForceTarget(actor, sameAdmin));
		Assert.IsFalse(CommandExecutionGuards.CanForceTarget(actor, higherAdmin));
		Assert.IsTrue(CommandExecutionGuards.CanForceTarget(founder, higherAdmin));
		Assert.IsTrue(CommandExecutionGuards.CanForceTarget(founder, sameAdmin));
	}

	[TestMethod]
	public void CommandExecutionSecurity_CanUseAsTarget_BlocksAdminsExceptForFounders()
	{
		var actor = Character(PermissionLevel.Admin).Object;
		var founder = Character(PermissionLevel.Founder).Object;
		var juniorAdmin = Character(PermissionLevel.JuniorAdmin).Object;
		var guide = Character(PermissionLevel.Guide).Object;
		var player = Character(PermissionLevel.Player).Object;

		Assert.IsFalse(CommandExecutionGuards.CanUseAsTarget(actor, juniorAdmin));
		Assert.IsTrue(CommandExecutionGuards.CanUseAsTarget(actor, guide));
		Assert.IsTrue(CommandExecutionGuards.CanUseAsTarget(actor, player));
		Assert.IsTrue(CommandExecutionGuards.CanUseAsTarget(founder, juniorAdmin));
	}

	[TestMethod]
	public void CommandExecutionSecurity_ExecuteForcedCommand_DowngradesStaffPcAndRestoresOnSuccess()
	{
		var target = CharacterWithMutablePermission(PermissionLevel.HighAdmin, true, out var permission);
		PermissionLevel? observedPermission = null;
		target.Setup(x => x.ExecuteCommand("look"))
		      .Callback(() => observedPermission = permission.Value)
		      .Returns(true);

		var result = CommandExecutionGuards.ExecuteForcedCommand(target.Object, "look");

		Assert.IsTrue(result);
		Assert.AreEqual(PermissionLevel.Player, observedPermission);
		Assert.AreEqual(PermissionLevel.HighAdmin, permission.Value);
		target.Verify(x => x.ChangePermissionLevel(PermissionLevel.Player), Times.Once);
		target.Verify(x => x.ChangePermissionLevel(PermissionLevel.HighAdmin), Times.Once);
	}

	[TestMethod]
	public void CommandExecutionSecurity_ExecuteForcedCommand_RestoresStaffPcAfterException()
	{
		var target = CharacterWithMutablePermission(PermissionLevel.SeniorAdmin, true, out var permission);
		target.Setup(x => x.ExecuteCommand("explode"))
		      .Throws(new InvalidOperationException("Command failed."));

		Assert.ThrowsException<InvalidOperationException>(() =>
			CommandExecutionGuards.ExecuteForcedCommand(target.Object, "explode"));

		Assert.AreEqual(PermissionLevel.SeniorAdmin, permission.Value);
		target.Verify(x => x.ChangePermissionLevel(PermissionLevel.Player), Times.Once);
		target.Verify(x => x.ChangePermissionLevel(PermissionLevel.SeniorAdmin), Times.Once);
	}

	[TestMethod]
	public void CommandExecutionSecurity_ExecuteForcedCommand_DoesNotDowngradePlayersOrNpcs()
	{
		var player = CharacterWithMutablePermission(PermissionLevel.Player, true, out var playerPermission);
		var npc = CharacterWithMutablePermission(PermissionLevel.NPC, false, out var npcPermission);
		PermissionLevel? observedPlayerPermission = null;
		PermissionLevel? observedNpcPermission = null;
		player.Setup(x => x.ExecuteCommand("look"))
		      .Callback(() => observedPlayerPermission = playerPermission.Value)
		      .Returns(true);
		npc.Setup(x => x.ExecuteCommand("look"))
		   .Callback(() => observedNpcPermission = npcPermission.Value)
		   .Returns(true);

		CommandExecutionGuards.ExecuteForcedCommand(player.Object, "look");
		CommandExecutionGuards.ExecuteForcedCommand(npc.Object, "look");

		Assert.AreEqual(PermissionLevel.Player, observedPlayerPermission);
		Assert.AreEqual(PermissionLevel.NPC, observedNpcPermission);
		player.Verify(x => x.ChangePermissionLevel(It.IsAny<PermissionLevel>()), Times.Never);
		npc.Verify(x => x.ChangePermissionLevel(It.IsAny<PermissionLevel>()), Times.Never);
	}

	[TestMethod]
	public void CommandExecutionSecurity_ForceCommandEffect_ExecutesStaffPcInMortalMode()
	{
		var gameworld = new Mock<IFuturemud>();
		var spell = new Mock<IMagicSpell>();
		spell.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		var effect = SpellEffectFactory.LoadEffect(new XElement("Effect",
			new XAttribute("type", "forcecommand"),
			new XElement("Command", new XCData("look"))), spell.Object);
		var target = CharacterWithMutablePermission(PermissionLevel.HighAdmin, true, out var permission);
		target.Setup(x => x.AffectedBy<IIgnoreForceEffect>()).Returns(false);
		var commands = new Mock<ICharacterCommandManager>();
		commands.Setup(x => x.LocateCommand(target.Object, ref It.Ref<string>.IsAny))
		        .Returns((IExecutable<ICharacter>)null!);
		var commandTree = new Mock<ICharacterCommandTree>();
		commandTree.SetupGet(x => x.Commands).Returns(commands.Object);
		target.SetupGet(x => x.CommandTree).Returns(commandTree.Object);
		PermissionLevel? observedPermission = null;
		target.Setup(x => x.ExecuteCommand("look"))
		      .Callback(() => observedPermission = permission.Value)
		      .Returns(true);

		effect.GetOrApplyEffect(Character(PermissionLevel.Player).Object, target.Object, OpposedOutcomeDegree.None,
			SpellPower.Insignificant, new Mock<IMagicSpellEffectParent>().Object, []);

		Assert.AreEqual(PermissionLevel.Player, observedPermission);
		Assert.AreEqual(PermissionLevel.HighAdmin, permission.Value);
		target.Verify(x => x.ChangePermissionLevel(PermissionLevel.Player), Times.Once);
		target.Verify(x => x.ChangePermissionLevel(PermissionLevel.HighAdmin), Times.Once);
	}

	[TestMethod]
	public void CommandExecutionSecurity_ForceCommandEffect_StillRespectsIgnoreForceEffect()
	{
		var spell = new Mock<IMagicSpell>();
		spell.SetupGet(x => x.Gameworld).Returns(new Mock<IFuturemud>().Object);
		var effect = SpellEffectFactory.LoadEffect(new XElement("Effect",
			new XAttribute("type", "forcecommand"),
			new XElement("Command", new XCData("look"))), spell.Object);
		var target = CharacterWithMutablePermission(PermissionLevel.HighAdmin, true, out _);
		target.Setup(x => x.AffectedBy<IIgnoreForceEffect>()).Returns(true);

		effect.GetOrApplyEffect(Character(PermissionLevel.Player).Object, target.Object, OpposedOutcomeDegree.None,
			SpellPower.Insignificant, new Mock<IMagicSpellEffectParent>().Object, []);

		target.Verify(x => x.ExecuteCommand(It.IsAny<string>()), Times.Never);
		target.Verify(x => x.ChangePermissionLevel(It.IsAny<PermissionLevel>()), Times.Never);
	}

	[TestMethod]
	public void CommandExecutionSecurity_ForceTargetResolver_UsesCorrectWorldCollections()
	{
		var all = Character(PermissionLevel.Player).Object;
		var player = Character(PermissionLevel.Player).Object;
		var npc = Character(PermissionLevel.NPC, false).Object;
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.Actors).Returns(Repository([all]).Object);
		gameworld.SetupGet(x => x.Characters).Returns(Repository([player]).Object);
		gameworld.SetupGet(x => x.NPCs).Returns(Repository([npc]).Object);

		Assert.AreSame(all, ForceTargetResolver.Resolve(gameworld.Object, ForceTargetScope.All).Single());
		Assert.AreSame(player, ForceTargetResolver.Resolve(gameworld.Object, ForceTargetScope.Players).Single());
		Assert.AreSame(npc, ForceTargetResolver.Resolve(gameworld.Object, ForceTargetScope.Npcs).Single());
	}
	[TestMethod]
	public void LiteracyRead_SealedTargetRequiresManipulationBeforeBreakingSeal()
	{
		var actor = Character(PermissionLevel.Player);
		var output = new Mock<IOutputHandler>();
		output.Setup(x => x.Send(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
		      .Returns(true);
		actor.SetupGet(x => x.OutputHandler).Returns(output.Object);

		var target = new Mock<IGameItem>();
		var readable = new Mock<IReadable>();
		var sealable = new Mock<ISealable>();
		sealable.SetupGet(x => x.IsSealed).Returns(true);
		target.Setup(x => x.GetItemType<IReadable>()).Returns(readable.Object);
		target.Setup(x => x.GetItemType<IOpenable>()).Returns((IOpenable)null!);
		target.Setup(x => x.GetItemType<ISealable>()).Returns(sealable.Object);
		actor.Setup(x => x.TargetItem("letter")).Returns(target.Object);
		actor.Setup(x => x.CanManipulateItem(target.Object)).Returns((false, "You cannot reach that."));

		InvokeLiteracyCommand("Read", actor.Object, "read letter");

		sealable.Verify(x => x.BreakSeal(It.IsAny<ICharacter>(), It.IsAny<string>()), Times.Never);
		output.Verify(x => x.Send("You cannot reach that.", true, false), Times.Once);
	}

	[TestMethod]
	public void CraftListCommands_EmptyQuotedFilter_DoesNotThrow()
	{
		var actor = Character(PermissionLevel.Player);
		var output = OutputHandler();
		actor.SetupGet(x => x.OutputHandler).Returns(output.Object);
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.Crafts).Returns(RevisableRepository(Array.Empty<ICraft>()).Object);
		actor.SetupGet(x => x.Gameworld).Returns(gameworld.Object);

		InvokeStatic(typeof(CraftModule), "Crafts", actor.Object, "crafts \"\"");
		InvokeStatic(typeof(CraftModule), "CookList", actor.Object, new StringStack("\"\""));
	}

	[TestMethod]
	public void ConditionRepairInput_DoesNotSatisfyConsumedGameItemContract()
	{
		Assert.IsFalse(typeof(ICraftInputConsumesGameItem).IsAssignableFrom(typeof(ConditionRepairInput)));
	}

	[TestMethod]
	public void FieldHerdDrive_BlockedExit_DoesNotMoveHerd()
	{
		var actor = Character(PermissionLevel.Player);
		var output = OutputHandler();
		actor.SetupGet(x => x.OutputHandler).Returns(output.Object);
		var sourceField = new Mock<IAgricultureField>();
		var destinationField = new Mock<IAgricultureField>();
		var sourceCell = new Mock<ICell>();
		var destinationCell = new Mock<ICell>();
		var exit = new Mock<ICellExit>();
		var herd = new Mock<IAgricultureHerdDefinition>();
		herd.SetupGet(x => x.Id).Returns(1L);
		herd.SetupGet(x => x.Name).Returns("cattle");
		var herds = new Mock<IUneditableAll<IAgricultureHerdDefinition>>();
		herds.Setup(x => x.GetByIdOrName("cattle", It.IsAny<bool>())).Returns(herd.Object);
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.AgricultureHerdDefinitions).Returns(herds.Object);

		sourceCell.SetupGet(x => x.AgricultureField).Returns(sourceField.Object);
		sourceCell.Setup(x => x.GetExitKeyword("north", actor.Object)).Returns(exit.Object);
		destinationCell.SetupGet(x => x.AgricultureField).Returns(destinationField.Object);
		exit.SetupGet(x => x.Destination).Returns(destinationCell.Object);
		actor.SetupGet(x => x.Location).Returns(sourceCell.Object);
		actor.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		actor.Setup(x => x.CanCross(exit.Object)).Returns((false, (IEmoteOutput)null!));

		InvokeStatic(typeof(AgricultureModule), "FieldHerdDrive", actor.Object, new StringStack("cattle north 1"));

		sourceField.Verify(x => x.DriveHerdTo(It.IsAny<IAgricultureField>(), It.IsAny<IAgricultureHerdDefinition>(),
			It.IsAny<int>(), actor.Object, out It.Ref<string>.IsAny), Times.Never);
		output.Verify(x => x.Send("You cannot drive the herd through that exit.", true, false), Times.Once);
	}

	[TestMethod]
	public void FillGas_SourceCannotBeManipulated_DoesNotDrainSource()
	{
		var actor = Character(PermissionLevel.Player);
		var output = OutputHandler();
		actor.SetupGet(x => x.OutputHandler).Returns(output.Object);
		var gas = new Mock<IGas>();
		gas.Setup(x => x.GasCountAs(gas.Object)).Returns(true);
		var targetContainer = new Mock<IGasContainer>();
		targetContainer.SetupProperty(x => x.Gas);
		targetContainer.SetupProperty(x => x.GasVolumeAtOneAtmosphere, 0.0);
		targetContainer.SetupGet(x => x.GasCapacityAtOneAtmosphere).Returns(10.0);
		var sourceContainer = new Mock<IGasContainer>();
		sourceContainer.SetupProperty(x => x.Gas, gas.Object);
		sourceContainer.SetupProperty(x => x.GasVolumeAtOneAtmosphere, 5.0);
		sourceContainer.SetupGet(x => x.GasCapacityAtOneAtmosphere).Returns(10.0);
		var target = new Mock<IGameItem>();
		target.Setup(x => x.GetItemType<IGasContainer>()).Returns(targetContainer.Object);
		target.Setup(x => x.HowSeen(actor.Object, It.IsAny<bool>(), It.IsAny<DescriptionType>(), It.IsAny<bool>(),
			It.IsAny<PerceiveIgnoreFlags>())).Returns("vial");
		var source = new Mock<IGameItem>();
		source.Setup(x => x.GetItemType<IGasContainer>()).Returns(sourceContainer.Object);
		source.Setup(x => x.HowSeen(actor.Object, It.IsAny<bool>(), It.IsAny<DescriptionType>(), It.IsAny<bool>(),
			It.IsAny<PerceiveIgnoreFlags>())).Returns("source");
		actor.Setup(x => x.TargetHeldItem("vial")).Returns(target.Object);
		actor.Setup(x => x.TargetItem("source")).Returns(source.Object);
		actor.Setup(x => x.CanManipulateItem(target.Object)).Returns((true, string.Empty));
		actor.Setup(x => x.CanManipulateItem(source.Object)).Returns((false, "You cannot reach that."));

		InvokeStatic(typeof(ManipulationModule), "FillGas", actor.Object, "fillgas vial source");

		Assert.AreEqual(5.0, sourceContainer.Object.GasVolumeAtOneAtmosphere, 0.0001);
		Assert.AreEqual(0.0, targetContainer.Object.GasVolumeAtOneAtmosphere, 0.0001);
		output.Verify(x => x.Send("You cannot reach that.", true, false), Times.Once);
	}

	[TestMethod]
	public void TrapLay_RoomComponentWithoutManipulationPermission_IsRejected()
	{
		var actor = Character(PermissionLevel.Player);
		var body = new Mock<IBody>();
		var cell = new Mock<ICell>();
		actor.SetupGet(x => x.Body).Returns(body.Object);
		actor.SetupGet(x => x.Location).Returns(cell.Object);
		var item = new Mock<IGameItem>();
		item.SetupGet(x => x.Id).Returns(1L);
		item.SetupGet(x => x.Location).Returns(cell.Object);
		item.SetupGet(x => x.Effects).Returns([]);
		actor.Setup(x => x.CanManipulateItem(item.Object)).Returns((false, "You cannot reach that."));

		var result = InvokeStatic<(bool Truth, string Message)>(typeof(TrapModule), "CanUsePhysicalTrapItem",
			actor.Object, item.Object);

		Assert.IsFalse(result.Truth);
		Assert.AreEqual("You cannot reach that.", result.Message);
		actor.Verify(x => x.CanManipulateItem(item.Object), Times.Once);
	}

	[TestMethod]
	public void TrapLay_HeldDroppableComponent_IsAllowed()
	{
		var actor = Character(PermissionLevel.Player);
		var body = new Mock<IBody>();
		var cell = new Mock<ICell>();
		actor.SetupGet(x => x.Body).Returns(body.Object);
		actor.SetupGet(x => x.Location).Returns(cell.Object);
		var item = new Mock<IGameItem>();
		item.SetupGet(x => x.Id).Returns(1L);
		item.SetupGet(x => x.InInventoryOf).Returns(body.Object);
		item.SetupGet(x => x.Effects).Returns([]);
		body.SetupGet(x => x.ItemsInHands).Returns([item.Object]);
		body.Setup(x => x.CanDrop(item.Object, 0)).Returns(true);
		actor.Setup(x => x.CanManipulateItem(item.Object)).Returns((true, string.Empty));

		var result = InvokeStatic<(bool Truth, string Message)>(typeof(TrapModule), "CanUsePhysicalTrapItem",
			actor.Object, item.Object);

		Assert.IsTrue(result.Truth, result.Message);
		Assert.AreEqual(string.Empty, result.Message);
		body.Verify(x => x.CanDrop(item.Object, 0), Times.Once);
		body.Verify(x => x.CanRemoveItem(It.IsAny<IGameItem>(), It.IsAny<ItemCanGetIgnore>()), Times.Never);
	}

	[TestMethod]
	public void TrapLay_HeldComponentThatCannotBeDropped_IsRejected()
	{
		var actor = Character(PermissionLevel.Player);
		var body = new Mock<IBody>();
		var cell = new Mock<ICell>();
		actor.SetupGet(x => x.Body).Returns(body.Object);
		actor.SetupGet(x => x.Location).Returns(cell.Object);
		var item = new Mock<IGameItem>();
		item.SetupGet(x => x.Id).Returns(1L);
		item.SetupGet(x => x.InInventoryOf).Returns(body.Object);
		item.SetupGet(x => x.Effects).Returns([]);
		body.SetupGet(x => x.ItemsInHands).Returns([item.Object]);
		body.Setup(x => x.CanDrop(item.Object, 0)).Returns(false);
		body.Setup(x => x.WhyCannotDrop(item.Object, 0)).Returns("You cannot drop that component.");
		actor.Setup(x => x.CanManipulateItem(item.Object)).Returns((true, string.Empty));

		var result = InvokeStatic<(bool Truth, string Message)>(typeof(TrapModule), "CanUsePhysicalTrapItem",
			actor.Object, item.Object);

		Assert.IsFalse(result.Truth);
		Assert.AreEqual("You cannot drop that component.", result.Message);
		body.Verify(x => x.CanDrop(item.Object, 0), Times.Once);
	}

	[TestMethod]
	public void TrapLay_AutomaticComponentCandidates_UseHeldItemsBeforeLooseRoomItems()
	{
		var actor = Character(PermissionLevel.Player);
		var body = new Mock<IBody>();
		var cell = new Mock<ICell>();
		actor.SetupGet(x => x.Body).Returns(body.Object);
		actor.SetupGet(x => x.Location).Returns(cell.Object);
		actor.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		var held = new Mock<IGameItem>();
		held.SetupGet(x => x.Id).Returns(1L);
		held.SetupGet(x => x.InInventoryOf).Returns(body.Object);
		held.SetupGet(x => x.Effects).Returns([]);
		var loose = new Mock<IGameItem>();
		loose.SetupGet(x => x.Id).Returns(2L);
		loose.SetupGet(x => x.Location).Returns(cell.Object);
		loose.SetupGet(x => x.Effects).Returns([]);
		body.SetupGet(x => x.ItemsInHands).Returns([held.Object]);
		body.Setup(x => x.CanDrop(held.Object, 0)).Returns(true);
		cell.Setup(x => x.LayerGameItems(RoomLayer.GroundLevel)).Returns([loose.Object]);
		actor.Setup(x => x.CanManipulateItem(held.Object)).Returns((true, string.Empty));
		actor.Setup(x => x.CanManipulateItem(loose.Object)).Returns((true, string.Empty));

		var candidates = InvokeStatic<List<IGameItem>>(typeof(TrapModule), "GetTrapComponentCandidates",
			actor.Object, cell.Object, new List<IGameItem>());

		CollectionAssert.AreEqual(new List<IGameItem> { held.Object, loose.Object }, candidates);
	}

	[TestMethod]
	public void TrapLay_ComponentPlacement_UsesAnInRoomInventoryPlan()
	{
		var actor = Character(PermissionLevel.Player);
		var gameworld = new Mock<IFuturemud>();
		var item = new Mock<IGameItem>();
		actor.SetupGet(x => x.Gameworld).Returns(gameworld.Object);

		var plan = InvokeStatic<IInventoryPlan>(typeof(TrapModule), "CreateTrapComponentInventoryPlan",
			actor.Object, new List<IGameItem> { item.Object });

		Assert.IsInstanceOfType(plan, typeof(InventoryPlan));
		var action = ((InventoryPlan)plan).Template.FirstPhase.Actions.Single();
		Assert.IsInstanceOfType(action, typeof(InventoryPlanActionDrop));
		Assert.AreEqual(DesiredItemState.InRoom, action.DesiredState);
	}

	[TestMethod]
	public void TrapLay_HeldNonComponentAnchor_IsIncludedInPlacementPlan()
	{
		var actor = Character(PermissionLevel.Player);
		var body = new Mock<IBody>();
		var anchor = new Mock<IGameItem>();
		var component = new Mock<IGameItem>();
		actor.SetupGet(x => x.Body).Returns(body.Object);
		body.SetupGet(x => x.ItemsInHands).Returns([anchor.Object]);

		var placementItems = InvokeStatic<List<IGameItem>>(typeof(TrapModule), "GetTrapPlacementItems",
			actor.Object, anchor.Object, new List<IGameItem> { component.Object });

		CollectionAssert.AreEqual(new List<IGameItem> { component.Object, anchor.Object }, placementItems);
	}

	[TestMethod]
	public void TrapLay_HeldAnchorThatIsAComponent_IsPlacedOnlyOnce()
	{
		var actor = Character(PermissionLevel.Player);
		var body = new Mock<IBody>();
		var anchor = new Mock<IGameItem>();
		actor.SetupGet(x => x.Body).Returns(body.Object);
		body.SetupGet(x => x.ItemsInHands).Returns([anchor.Object]);

		var placementItems = InvokeStatic<List<IGameItem>>(typeof(TrapModule), "GetTrapPlacementItems",
			actor.Object, anchor.Object, new List<IGameItem> { anchor.Object });

		Assert.AreEqual(1, placementItems.Count);
		Assert.AreSame(anchor.Object, placementItems.Single());
	}

	[TestMethod]
	public void TrapLay_ItemInAnotherInventory_IsRejected()
	{
		var actor = Character(PermissionLevel.Player);
		var body = new Mock<IBody>();
		var otherBody = new Mock<IBody>();
		var cell = new Mock<ICell>();
		actor.SetupGet(x => x.Body).Returns(body.Object);
		actor.SetupGet(x => x.Location).Returns(cell.Object);
		var item = new Mock<IGameItem>();
		item.SetupGet(x => x.Id).Returns(1L);
		item.SetupGet(x => x.InInventoryOf).Returns(otherBody.Object);
		item.SetupGet(x => x.Effects).Returns([]);

		var result = InvokeStatic<(bool Truth, string Message)>(typeof(TrapModule), "CanUsePhysicalTrapItem",
			actor.Object, item.Object);

		Assert.IsFalse(result.Truth);
		StringAssert.Contains(result.Message, "You must be holding");
		actor.Verify(x => x.CanManipulateItem(It.IsAny<IGameItem>()), Times.Never);
	}

	[TestMethod]
	public void TrapLay_ComponentParser_UsesHeldPreferredLocalResolver()
	{
		var actor = Character(PermissionLevel.Player);
		var cell = new Mock<ICell>();
		var item = new Mock<IGameItem>();
		actor.Setup(x => x.TargetLocalOrHeldItem("wire")).Returns(item.Object);

		InvokeStatic(typeof(TrapModule), "ParseSuppliedComponents", actor.Object,
			new StringStack("using wire"), cell.Object, null!);

		actor.Verify(x => x.TargetLocalOrHeldItem("wire"), Times.Once);
		actor.Verify(x => x.TargetItem(It.IsAny<string>()), Times.Never);
	}

	[TestMethod]
	public void TrapLay_HeldComponent_IsRemovedFromInventoryWhenInstalled()
	{
		var body = new Mock<IBody>();
		var item = new Mock<IGameItem>();
		item.SetupGet(x => x.InInventoryOf).Returns(body.Object);

		TrapEffect.DetachInstalledComponent(item.Object);

		body.Verify(x => x.Take(item.Object), Times.Once);
		item.Verify(x => x.Get(null), Times.Once);
	}

	[TestMethod]
	public void TrapLay_RoomComponent_IsExtractedWhenInstalled()
	{
		var cell = new Mock<ICell>();
		var item = new Mock<IGameItem>();
		item.SetupGet(x => x.Location).Returns(cell.Object);

		TrapEffect.DetachInstalledComponent(item.Object);

		cell.Verify(x => x.Extract(item.Object), Times.Once);
		item.Verify(x => x.Get(null), Times.Once);
	}

	[TestMethod]
	public void TrapComponentReservation_UsesTrapAnchorAsSpatialHost()
	{
		var item = new Mock<IGameItem>();
		var anchor = new Mock<ICell>();
		var reservation = new TrapComponentReservationEffect(item.Object, Guid.NewGuid(), anchor.Object,
			RoomLayer.InTrees, 4_500.0);

		Assert.AreSame(anchor.Object, ((IProvideItemSpatialHostEffect)reservation).SpatialHost);
		Assert.AreSame(anchor.Object, GameItem.ResolveEffectSpatialHost([reservation], item.Object));
		Assert.AreEqual(RoomLayer.InTrees, reservation.SpatialLayer);
		Assert.AreEqual(4_500.0, reservation.SpatialRoutePositionMetres);
	}

	[TestMethod]
	public void TrapComponentRecovery_RestoresCapturedSpatialLocation()
	{
		var routeDefinition = new Mock<IRouteCellDefinition>();
		routeDefinition.SetupGet(x => x.LengthMetres).Returns(10_000.0);
		var cell = new Mock<ICell>();
		cell.SetupGet(x => x.RouteDefinition).Returns(routeDefinition.Object);
		var item = new Mock<IGameItem>();
		item.SetupProperty(x => x.RoomLayer);
		var location = new SpatialLocation(cell.Object, RoomLayer.InAir, 4_500.0);

		TrapEffect.RestoreInstalledComponent(item.Object, location);

		Assert.AreEqual(RoomLayer.InAir, item.Object.RoomLayer);
		item.Verify(x => x.MoveTo(location, null, false), Times.Once);
		cell.Verify(x => x.Insert(item.Object, true), Times.Once);
	}

	[TestMethod]
	public void Arm_KnownManipulableDisarmedTrap_RearmsTrap()
	{
		var actor = Character(PermissionLevel.Player);
		var output = OutputHandler();
		var target = new Mock<IGameItem>();
		var template = new Mock<ITrapTemplate>();
		template.SetupGet(x => x.Id).Returns(10L);
		template.SetupGet(x => x.RevisionNumber).Returns(1);
		template.SetupGet(x => x.Charges).Returns(1);
		target.SetupGet(x => x.Gameworld).Returns(new Mock<IFuturemud>().Object);
		var trap = new TrapEffect(target.Object, template.Object);
		Assert.IsTrue(trap.Disarm());
		var knowledge = new TrapKnowledgeEffect(actor.Object, trap.InstanceId, trap.TemplateId, trap.TemplateRevisionNumber);
		actor.SetupGet(x => x.OutputHandler).Returns(output.Object);
		actor.Setup(x => x.TargetItem("box")).Returns(target.Object);
		actor.Setup(x => x.CanManipulateItem(target.Object)).Returns((true, string.Empty));
		actor.Setup(x => x.EffectsOfType<TrapKnowledgeEffect>(It.IsAny<Predicate<TrapKnowledgeEffect>>()))
			.Returns([knowledge]);
		target.Setup(x => x.EffectsOfType<TrapEffect>(It.IsAny<Predicate<TrapEffect>>())).Returns([trap]);

		InvokeStatic(typeof(ManipulationModule), "Arm", actor.Object, "arm box");

		Assert.AreEqual(TrapState.Armed, trap.State);
		actor.Verify(x => x.CanManipulateItem(target.Object), Times.Once);
	}

	[TestMethod]
	public void Arm_TrapWithoutManipulationPermission_DoesNotRearm()
	{
		var actor = Character(PermissionLevel.Player);
		var output = OutputHandler();
		var target = new Mock<IGameItem>();
		var template = new Mock<ITrapTemplate>();
		template.SetupGet(x => x.Id).Returns(10L);
		template.SetupGet(x => x.RevisionNumber).Returns(1);
		template.SetupGet(x => x.Charges).Returns(1);
		target.SetupGet(x => x.Gameworld).Returns(new Mock<IFuturemud>().Object);
		var trap = new TrapEffect(target.Object, template.Object);
		Assert.IsTrue(trap.Disarm());
		actor.SetupGet(x => x.OutputHandler).Returns(output.Object);
		actor.Setup(x => x.TargetItem("box")).Returns(target.Object);
		actor.Setup(x => x.CanManipulateItem(target.Object)).Returns((false, "You cannot reach that."));
		target.Setup(x => x.EffectsOfType<TrapEffect>(It.IsAny<Predicate<TrapEffect>>())).Returns([trap]);

		InvokeStatic(typeof(ManipulationModule), "Arm", actor.Object, "arm box");

		Assert.AreEqual(TrapState.Disarmed, trap.State);
		output.Verify(x => x.Send("You cannot reach that.", true, false), Times.Once);
	}

	[TestMethod]
	public void Arm_UnknownTrap_DoesNotRevealOrRearmTrap()
	{
		var actor = Character(PermissionLevel.Player);
		var output = OutputHandler();
		var target = new Mock<IGameItem>();
		var template = new Mock<ITrapTemplate>();
		template.SetupGet(x => x.Id).Returns(10L);
		template.SetupGet(x => x.RevisionNumber).Returns(1);
		template.SetupGet(x => x.Charges).Returns(1);
		target.SetupGet(x => x.Gameworld).Returns(new Mock<IFuturemud>().Object);
		target.Setup(x => x.HowSeen(actor.Object, true, It.IsAny<DescriptionType>(), It.IsAny<bool>(),
			It.IsAny<PerceiveIgnoreFlags>())).Returns("a box");
		var trap = new TrapEffect(target.Object, template.Object);
		Assert.IsTrue(trap.Disarm());
		actor.SetupGet(x => x.OutputHandler).Returns(output.Object);
		actor.Setup(x => x.TargetItem("box")).Returns(target.Object);
		actor.Setup(x => x.CanManipulateItem(target.Object)).Returns((true, string.Empty));
		actor.Setup(x => x.EffectsOfType<TrapKnowledgeEffect>(It.IsAny<Predicate<TrapKnowledgeEffect>>()))
			.Returns([]);
		target.Setup(x => x.EffectsOfType<TrapEffect>(It.IsAny<Predicate<TrapEffect>>())).Returns([trap]);

		InvokeStatic(typeof(ManipulationModule), "Arm", actor.Object, "arm box");

		Assert.AreEqual(TrapState.Disarmed, trap.State);
		output.Verify(x => x.Send("a box does not have an armable explosive trigger.", true, false), Times.Once);
	}

	[TestMethod]
	public void Arm_ActiveKnownTrap_DoesNotBypassItsLifecycleState()
	{
		var actor = Character(PermissionLevel.Player);
		var output = OutputHandler();
		var target = new Mock<IGameItem>();
		var template = new Mock<ITrapTemplate>();
		template.SetupGet(x => x.Id).Returns(10L);
		template.SetupGet(x => x.RevisionNumber).Returns(1);
		template.SetupGet(x => x.Charges).Returns(1);
		target.SetupGet(x => x.Gameworld).Returns(new Mock<IFuturemud>().Object);
		var trap = new TrapEffect(target.Object, template.Object);
		var knowledge = new TrapKnowledgeEffect(actor.Object, trap.InstanceId, trap.TemplateId, trap.TemplateRevisionNumber);
		actor.SetupGet(x => x.OutputHandler).Returns(output.Object);
		actor.Setup(x => x.TargetItem("box")).Returns(target.Object);
		actor.Setup(x => x.CanManipulateItem(target.Object)).Returns((true, string.Empty));
		actor.Setup(x => x.EffectsOfType<TrapKnowledgeEffect>(It.IsAny<Predicate<TrapKnowledgeEffect>>()))
			.Returns([knowledge]);
		target.Setup(x => x.EffectsOfType<TrapEffect>(It.IsAny<Predicate<TrapEffect>>())).Returns([trap]);

		InvokeStatic(typeof(ManipulationModule), "Arm", actor.Object, "arm box");

		Assert.AreEqual(TrapState.Armed, trap.State);
		output.Verify(x => x.Send("Only an unarmed or disarmed trap can be armed this way.", true, false), Times.Once);
	}

	[TestMethod]
	public void Arm_OrdinaryExplosiveTrigger_RemainsAvailable()
	{
		var actor = Character(PermissionLevel.Player);
		var target = new Mock<IGameItem>();
		var trigger = new Mock<IArmableExplosiveTrigger>();
		actor.Setup(x => x.TargetItem("charge")).Returns(target.Object);
		actor.Setup(x => x.CanManipulateItem(target.Object)).Returns((true, string.Empty));
		target.Setup(x => x.EffectsOfType<TrapEffect>(It.IsAny<Predicate<TrapEffect>>())).Returns([]);
		target.Setup(x => x.GetItemType<IArmableExplosiveTrigger>()).Returns(trigger.Object);
		trigger.Setup(x => x.CanArm(actor.Object, string.Empty)).Returns(true);

		InvokeStatic(typeof(ManipulationModule), "Arm", actor.Object, "arm charge");

		trigger.Verify(x => x.Arm(actor.Object, string.Empty, It.IsAny<IEmote>()), Times.Once);
	}

	[TestMethod]
	public void ExportCraftCsvCell_QuotesAndNeutralisesSpreadsheetFormulae()
	{
		Assert.AreEqual("\"'=1+1\"", SpreadsheetSafeCsv.EncodeCell("=1+1"));
		Assert.AreEqual("\"text, with \"\"quotes\"\"\"", SpreadsheetSafeCsv.EncodeCell("text, with \"quotes\""));
		Assert.AreEqual("\"\"", SpreadsheetSafeCsv.EncodeCell(null));
	}

	private static Mock<ICharacter> Character(PermissionLevel permissionLevel, bool isPlayerCharacter = true)
	{
		var character = new Mock<ICharacter>();
		character.SetupGet(x => x.PermissionLevel).Returns(permissionLevel);
		character.SetupGet(x => x.IsPlayerCharacter).Returns(isPlayerCharacter);
		return character;
	}

	private static Mock<IOutputHandler> OutputHandler()
	{
		var output = new Mock<IOutputHandler>();
		output.Setup(x => x.Send(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>())).Returns(true);
		output.Setup(x => x.Send(It.IsAny<IOutput>(), It.IsAny<bool>(), It.IsAny<bool>())).Returns(true);
		return output;
	}

	private static void InvokeStatic(Type type, string methodName, params object[] arguments)
	{
		var method = type.GetMethod(methodName, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)!;
		method.Invoke(null, arguments);
	}

	private static T InvokeStatic<T>(Type type, string methodName, params object[] arguments)
	{
		var method = type.GetMethod(methodName, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)!;
		return (T)method.Invoke(null, arguments)!;
	}

	private static Mock<IUneditableRevisableAll<T>> RevisableRepository<T>(IEnumerable<T> items) where T : class, IRevisableItem
	{
		var list = items.ToList();
		var mock = new Mock<IUneditableRevisableAll<T>>();
		mock.As<IEnumerable<T>>()
		    .Setup(x => x.GetEnumerator())
		    .Returns(() => list.GetEnumerator());
		return mock;
	}

	private static Mock<IUneditableAll<T>> Repository<T>(IEnumerable<T> items) where T : class, IFrameworkItem
	{
		var values = items.ToList();
		var repository = new Mock<IUneditableAll<T>>();
		repository.As<IEnumerable<T>>()
		          .Setup(x => x.GetEnumerator())
		          .Returns(() => values.GetEnumerator());
		return repository;
	}
	private static Mock<ICharacter> CharacterWithMutablePermission(PermissionLevel startingPermission,
		bool isPlayerCharacter, out MutablePermission permission)
	{
		permission = new MutablePermission(startingPermission);
		var capturedPermission = permission;
		var character = new Mock<ICharacter>();
		character.SetupGet(x => x.PermissionLevel).Returns(() => capturedPermission.Value);
		character.SetupGet(x => x.IsPlayerCharacter).Returns(isPlayerCharacter);
		character.Setup(x => x.ChangePermissionLevel(It.IsAny<PermissionLevel>()))
		         .Callback<PermissionLevel>(value => capturedPermission.Value = value);
		return character;
	}

	private static void InvokeLiteracyCommand(string methodName, ICharacter actor, string command)
	{
		var method = typeof(LiteracyModule).GetMethod(methodName, BindingFlags.Static | BindingFlags.NonPublic);
		Assert.IsNotNull(method, $"Could not find LiteracyModule.{methodName}.");
		method.Invoke(null, new object[] { actor, command });
	}

	private sealed class MutablePermission(PermissionLevel value)
	{
		public PermissionLevel Value { get; set; } = value;
	}
}
