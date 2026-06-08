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
using MudSharp.Character;
using MudSharp.Commands.Modules;
using MudSharp.Commands;
using MudSharp.Commands.Trees;
using MudSharp.Communication;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Interfaces;
using MudSharp.Form.Material;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Magic;
using MudSharp.Magic.SpellEffects;
using MudSharp.Magic.SpellTriggers;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Checks;
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
	public void CommandExecutionSecurity_ForceFanoutUsesCorrectWorldCollections()
	{
		var source = File.ReadAllText(GetSourcePath("MudSharpCore", "Commands", "Modules", "StorytellerModule.cs"));
		var allBranch = Slice(source, "targetText.Equals(\"all\"", "targetText.Equals(\"players\"");
		var playersBranch = Slice(source, "targetText.Equals(\"players\"", "targetText.Equals(\"npcs\"");
		var npcsBranch = Slice(source, "targetText.Equals(\"npcs\"", "targetText.Equals(\"here\"");

		StringAssert.Contains(allBranch, "character.Gameworld.Actors");
		StringAssert.Contains(playersBranch, "character.Gameworld.Characters");
		StringAssert.Contains(npcsBranch, "character.Gameworld.NPCs");
		StringAssert.Contains(allBranch, "CommandExecutionGuards.CanForceTarget(character, x)");
		StringAssert.Contains(playersBranch, "CommandExecutionGuards.CanForceTarget(character, x)");
		StringAssert.Contains(npcsBranch, "CommandExecutionGuards.CanForceTarget(character, x)");
	}

	[TestMethod]
	public void CommandExecutionSecurity_AuthoredIndirectCommandPathsUseSecureHelper()
	{
		var guardedSources = new[]
		{
			("MudSharpCore", "FutureProg", "Statements", "Force.cs"),
			("MudSharpCore", "FutureProg", "Statements", "Delay.cs"),
			("MudSharpCore", "Effects", "Concrete", "Dreaming.cs"),
			("MudSharpCore", "Framework", "Scheduling", "Schedules.cs"),
			("MudSharpCore", "Magic", "SpellEffects", "MagicPhase3Effects.cs")
		};

		foreach (var parts in guardedSources)
		{
			var source = File.ReadAllText(GetSourcePath(parts.Item1, parts.Item2, parts.Item3, parts.Item4));
			Assert.IsTrue(source.Contains("CommandExecutionGuards.", StringComparison.Ordinal),
				$"{parts.Item4} should use CommandExecutionGuards for authored forced commands.");
		}

		var hookSource = File.ReadAllText(GetSourcePath("MudSharpCore", "Events", "Hooks", "CommandHook.cs"));
		StringAssert.Contains(hookSource, "if (target is ICharacter character)");
		StringAssert.Contains(hookSource, "CommandExecutionGuards.ExecuteForcedCommand(character, command);");
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
	public void ExportCraftCsvCell_QuotesAndNeutralisesSpreadsheetFormulae()
	{
		var method = typeof(ImplementorModule).GetMethod("EncodeCraftExportCsvCell",
			BindingFlags.Static | BindingFlags.NonPublic)!;

		Assert.AreEqual("\"'=1+1\"", method.Invoke(null, ["=1+1"]));
		Assert.AreEqual("\"text, with \"\"quotes\"\"\"", method.Invoke(null, ["text, with \"quotes\""]));
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

	private static Mock<IUneditableRevisableAll<T>> RevisableRepository<T>(IEnumerable<T> items) where T : class, IRevisableItem
	{
		var list = items.ToList();
		var mock = new Mock<IUneditableRevisableAll<T>>();
		mock.As<IEnumerable<T>>()
		    .Setup(x => x.GetEnumerator())
		    .Returns(() => list.GetEnumerator());
		return mock;
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

	private static string Slice(string source, string startText, string endText)
	{
		var start = source.IndexOf(startText, StringComparison.Ordinal);
		Assert.IsTrue(start >= 0, $"Could not find source marker {startText}.");
		var end = source.IndexOf(endText, start, StringComparison.Ordinal);
		Assert.IsTrue(end > start, $"Could not find source marker {endText} after {startText}.");
		return source[start..end];
	}

	private static string GetSourcePath(params string[] parts)
	{
		return Path.GetFullPath(Path.Combine(
			AppContext.BaseDirectory,
			"..",
			"..",
			"..",
			"..",
			Path.Combine(parts)));
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
