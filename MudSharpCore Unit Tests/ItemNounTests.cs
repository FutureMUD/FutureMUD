using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Commands.Helpers;
using MudSharp.Commands.Modules;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems;

#nullable enable

namespace MudSharp_Unit_Tests;

[TestClass]
public class ItemNounTests
{
	[TestMethod]
	public void BulkRename_DistinctPrototypesAndUntouchedNoun_AllowsSharedNouns()
	{
		var helper = EditableRevisableItemHelper.GameItemHelper;
		var items = new[] { Proto(1, "gate"), Proto(2, "hatch"), Proto(3, "door") };
		var plan = EditableItemNameBulkRenamePlanner.CreatePlan(
			items, "^(gate|hatch)$", "door", x => x.Id, x => x.RevisionNumber,
			x => x.Status, x => x.Name, helper.NameScopeKeyFunc, helper.TryNormaliseNameForBulkRename);

		Assert.IsTrue(plan.IsValid);
		Assert.IsTrue(plan.TryApply(helper.SetNameFromValidatedBulkRenameAction, out var count));
		Assert.AreEqual(2, count);
		foreach (var item in items)
		{
			Assert.AreEqual("door", item.Name);
		}
	}

	[TestMethod]
	[DataRow("123", true)]
	[DataRow(" door ", true)]
	[DataRow("   ", false)]
	public void NounValidation_UsesVocabularyRules(string noun, bool valid)
	{
		var result = EditableRevisableItemHelper.GameItemHelper.TryNormaliseNameForBulkRename(Proto(1, "gate"), noun);
		Assert.AreEqual(valid, result.IsValid);
		if (valid) Assert.AreEqual(noun.Trim(), result.Name);
	}

	[TestMethod]
	[DataRow("door", "door")]
	[DataRow("123", "123")]
	[DataRow("", "door copy")]
	public void CloneNoun_DoesNotRequireCatalogueUniqueness(string requested, string expected)
	{
		var method = typeof(ItemBuilderModule).GetMethod("TryGetItemCloneName", BindingFlags.NonPublic | BindingFlags.Static)!;
		// A strict actor proves noun validation does not query the world's existing names.
		object?[] args = { new Mock<ICharacter>(MockBehavior.Strict).Object, Proto(1, "door"), requested, null };
		Assert.AreEqual(true, method.Invoke(null, args));
		Assert.AreEqual(expected, args[3]);
	}

	[TestMethod]
	[DataRow("noun door")]
	[DataRow("name door")]
	public void SetNoun_ExistingPrototypeUsesSameNoun_AppliesChange(string command)
	{
		var target = Proto(1, "gate");
		var catalogue = new RevisableAll<IGameItemProto>();
		catalogue.Add(target);
		catalogue.Add(Proto(2, "door"));
		var world = new Mock<IFuturemud>();
		world.SetupGet(x => x.ItemProtos).Returns(catalogue);
		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.Gameworld).Returns(world.Object);
		actor.SetupGet(x => x.OutputHandler).Returns(new Mock<MudSharp.PerceptionEngine.IOutputHandler>().Object);
		var method = typeof(BaseBuilderModule).GetMethod("TrySetEditableRevisableItemName", BindingFlags.NonPublic | BindingFlags.Static)!;
		Assert.AreEqual(true, method.Invoke(null, new object[] { actor.Object, new StringStack(command), EditableRevisableItemHelper.GameItemHelper, target }));
		Assert.AreEqual("door", target.Name);
	}

	private static GameItemProto Proto(long id, string noun)
	{
		var proto = (GameItemProto)RuntimeHelpers.GetUninitializedObject(typeof(GameItemProto));
		typeof(MudSharp.Framework.FrameworkItem).GetField("_id", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(proto, id);
		proto.SetNoSave(true);
		proto.SetNameFromValidatedBulkRename(noun);
		return proto;
	}
}
