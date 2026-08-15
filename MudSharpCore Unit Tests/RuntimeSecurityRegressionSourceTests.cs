#nullable enable

using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MudSharp_Unit_Tests;

[TestClass]
public class RuntimeSecurityRegressionSourceTests
{
	[TestMethod]
	public void OfferingReceiver_LiquidEchoUsesLiteralTokenReplacement()
	{
		var prototypeSource = ReadSource("MudSharpCore", "GameItems", "Prototypes",
			"OfferingReceiverGameItemComponentProto.cs");
		var runtimeSource = ReadSource("MudSharpCore", "GameItems", "Components",
			"OfferingReceiverGameItemComponent.cs");

		StringAssert.Contains(prototypeSource,
			"text.Replace(\"{0}\", \"some liquid\", StringComparison.Ordinal)");
		StringAssert.Contains(runtimeSource,
			"_prototype.LiquidAcceptEcho.Replace(\"{0}\", offered.ColouredLiquidDescription");
		Assert.IsFalse(prototypeSource.Contains("string.Format(System.Globalization.CultureInfo.InvariantCulture, text",
			StringComparison.Ordinal));
		Assert.IsFalse(runtimeSource.Contains("string.Format(System.Globalization.CultureInfo.InvariantCulture, _prototype.LiquidAcceptEcho",
			StringComparison.Ordinal));
		Assert.AreEqual("some liquid {1}",
			"{0} {1}".Replace("{0}", "some liquid", StringComparison.Ordinal));
		Assert.AreEqual("some liquid {",
			"{0} {".Replace("{0}", "some liquid", StringComparison.Ordinal));
	}

	[TestMethod]
	public void LockingCashRegister_KeylessAdminPathDoesNotDereferenceKey()
	{
		var source = ReadSource("MudSharpCore", "GameItems", "Components",
			"LockingCashRegisterGameItemComponent.cs");
		var emitStart = source.IndexOf("private void EmitLockChange", StringComparison.Ordinal);
		var installStart = source.IndexOf("public void InstallLock", emitStart, StringComparison.Ordinal);

		Assert.IsTrue(emitStart >= 0);
		Assert.IsTrue(installStart > emitStart);
		var emitSource = source[emitStart..installStart];
		StringAssert.Contains(emitSource, "if (actor is not null && key is not null)");
		StringAssert.Contains(emitSource, "key?.Parent");
		StringAssert.Contains(emitSource, "_prototype.LockEmoteNoActor");
	}

	[TestMethod]
	public void Undo_RequiresColocationBeforeRemovingRestraint()
	{
		var source = ReadSource("MudSharpCore", "Commands", "Modules", "InventoryModule.cs");
		var undoStart = source.IndexOf("protected static void Undo", StringComparison.Ordinal);
		var unwieldStart = source.IndexOf("[PlayerCommand(\"Unwield\"", undoStart, StringComparison.Ordinal);

		Assert.IsTrue(undoStart >= 0);
		Assert.IsTrue(unwieldStart > undoStart);
		var undoSource = source[undoStart..unwieldStart];
		var proximityGuard = undoSource.IndexOf("if (!actor.ColocatedWith(target))", StringComparison.Ordinal);
		var restraintRemoval = undoSource.IndexOf("target.Body.Take(item);", StringComparison.Ordinal);
		Assert.IsTrue(proximityGuard >= 0);
		Assert.IsTrue(restraintRemoval > proximityGuard);
	}

	[TestMethod]
	public void ManipulationCommands_PhysicalActionsRequireImmediateReach()
	{
		var source = ReadSource("MudSharpCore", "Commands", "Modules", "ManipulationModule.cs");

		var drag = Slice(source, "[PlayerCommand(\"Drag\"", "[PlayerCommand(\"Struggle\"");
		StringAssert.Contains(drag, "if (!actor.ColocatedWith(help))");
		StringAssert.Contains(drag, "if (!actor.ColocatedWith(target))");

		var apply = Slice(source, "[PlayerCommand(\"Apply\"", "public const string DipHelpText");
		StringAssert.Contains(apply, "if (!character.ColocatedWith(targetCharacter))");

		var inject = Slice(source, "[PlayerCommand(\"Inject\"", "[PlayerCommand(\"Feed\"");
		StringAssert.Contains(inject, "if (!character.ColocatedWith(targetCharacter))");

		var feed = Slice(source, "[PlayerCommand(\"Feed\"", "[PlayerCommand(\"Eat\"");
		StringAssert.Contains(feed, "if (!character.ColocatedWith(target))");

		var fill = Slice(source, "[PlayerCommand(\"Fill\"", "[PlayerCommand(\"FillGas\"");
		StringAssert.Contains(fill, "if (!character.ColocatedWith(containerOwner))");
		StringAssert.Contains(fill, "character.CanManipulateItem(container)");

		var spill = Slice(source, "[PlayerCommand(\"Spill\"", "[PlayerCommand(\"Smoke\"");
		StringAssert.Contains(spill, "if (!character.ColocatedWith(charTarget))");
		StringAssert.Contains(spill, "character.CanManipulateItem(target)");

		var light = Slice(source, "[PlayerCommand(\"Light\"", "[PlayerCommand(\"Extinguish\"");
		StringAssert.Contains(light, "character.CanManipulateItem(ignitionItem)");

		var extinguish = Slice(source, "[PlayerCommand(\"Extinguish\"", "[PlayerCommand(\"Knock\"");
		StringAssert.Contains(extinguish, "character.CanManipulateItem(target)");

		var install = Slice(source, "[PlayerCommand(\"Install\"", "private const string UninstallHelpText");
		StringAssert.Contains(install, "character.CanManipulateItem(targetItem)");

		var uninstall = Slice(source, "[PlayerCommand(\"Uninstall\"", "[PlayerCommand(\"Junk\"");
		StringAssert.Contains(uninstall, "character.CanManipulateItem(targetItem)");
		StringAssert.Contains(uninstall, "character.CanManipulateItem(lockableItem)");

		var attachProsthetic = Slice(source, "private static bool CanAttachProsthetic",
			"[PlayerCommand(\"Detach\"");
		StringAssert.Contains(attachProsthetic, "if (!actor.ColocatedWith(target))");

		var attach = Slice(source, "[PlayerCommand(\"Attach\"", "[PlayerCommand(\"Recover\"");
		StringAssert.Contains(attach, "actor.CanManipulateItem(musket.Parent)");

		var detachProsthetic = Slice(source, "protected static void DetachProsthetic",
			"[PlayerCommand(\"Close\"");
		StringAssert.Contains(detachProsthetic, "if (!actor.ColocatedWith(target))");

		var detach = Slice(source, "[PlayerCommand(\"Detach\"", "private static void DetachFirearmAttachment");
		StringAssert.Contains(detach, "actor.CanManipulateItem(targetBelt)");

		var connect = Slice(source, "[PlayerCommand(\"Connect\"", "[PlayerCommand(\"Disconnect\"");
		StringAssert.Contains(connect, "if (!actor.ColocatedWith(targetActor))");

		var disconnect = Slice(source, "[PlayerCommand(\"Disconnect\"", "[PlayerCommand(\"Select\"");
		StringAssert.Contains(disconnect, "if (!actor.ColocatedWith(targetActor))");

		var roll = SliceToEnd(source, "[PlayerCommand(\"Roll\"");
		StringAssert.Contains(roll, "actor.CanManipulateItem(surfaceTarget)");

		var open = Slice(source, "[PlayerCommand(\"Open\"", "[PlayerCommand(\"Attach\"");
		StringAssert.Contains(open, "actor.CanManipulateItem(openable.Parent)");

		var close = Slice(source, "[PlayerCommand(\"Close\"", "[PlayerCommand(\"Lock\"");
		StringAssert.Contains(close, "actor.CanManipulateItem(openable.Parent)");

		var characterSource = ReadSource("MudSharpCore", "Character", "Character.cs");
		var canStyle = Slice(characterSource, "public bool CanStyle", "public bool Style");
		StringAssert.Contains(canStyle, "if (!target.ColocatedWith(this))");
	}

	private static string Slice(string source, string startMarker, string endMarker)
	{
		var start = source.IndexOf(startMarker, StringComparison.Ordinal);
		var end = source.IndexOf(endMarker, start, StringComparison.Ordinal);
		Assert.IsTrue(start >= 0, $"Missing source marker: {startMarker}");
		Assert.IsTrue(end > start, $"Missing source marker after {startMarker}: {endMarker}");
		return source[start..end];
	}

	private static string SliceToEnd(string source, string startMarker)
	{
		var start = source.IndexOf(startMarker, StringComparison.Ordinal);
		Assert.IsTrue(start >= 0, $"Missing source marker: {startMarker}");
		return source[start..];
	}

	private static string ReadSource(params string[] parts)
	{
		return File.ReadAllText(Path.GetFullPath(Path.Combine(
			AppContext.BaseDirectory,
			"..",
			"..",
			"..",
			"..",
			Path.Combine(parts))));
	}
}
