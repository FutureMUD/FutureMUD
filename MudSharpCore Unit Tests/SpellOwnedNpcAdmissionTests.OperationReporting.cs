#nullable enable

using System;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.Magic;
using MudSharp.Magic.Casting;
using MudSharp.Magic.Lifecycle;

namespace MudSharp_Unit_Tests;

public partial class SpellOwnedNpcAdmissionTests
{
	[TestMethod]
	[DataRow(1, 1)]
	[DataRow(3, 3)]
	public void PaidNativeCreation_CompletedGroupReportsAppliedWithoutRetainedEffect(int count, int grade)
	{
		var f = new MagicCastingFixture();
		var template = Template(f.World.Object);
		var spell = Configure(f, template);
		PrepareSpatial(f);
		Assert.IsTrue(spell.SpellEffects.Single().BuildingCommand(f.Actor.Object,
			new StringStack($"count {count}")));
		var npc = new Mock<ICharacter> { DefaultValue = DefaultValue.Mock };
		npc.SetupGet(x => x.State).Returns(CharacterState.Awake);
		npc.SetupGet(x => x.Location).Returns(f.Actor.Object.Location);
		npc.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		var native = new Mock<ISpellOwnedNpcService>();
		native.Setup(x => x.Create(template, It.IsAny<SpatialLocation>(), It.IsAny<SpellLifecycleOrigin>()))
			.Returns(npc.Object);
		f.World.SetupGet(x => x.SpellOwnedNpcs).Returns(native.Object);

		var result = f.Service.Cast(new(f.Actor.Object, f.Earth.Id, spell.Id, grade, grade > 2, ""));

		Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		native.Verify(x => x.Create(template, It.IsAny<SpatialLocation>(), It.IsAny<SpellLifecycleOrigin>()), Times.Exactly(count));
		var operation = f.Store.Operations[result.OperationId!.Value];
		Assert.AreEqual("Completed", operation.Stage);
		Assert.AreEqual("true", XElement.Parse(operation.Definition).Attribute("applied")?.Value);
		Assert.AreEqual(grade == 1 ? 95.0 : 77.5, f.Balances[f.Resources[1]]);
		Assert.AreEqual(grade == 3 ? 3 : 2, f.Store.Acquisition(100, spell.Id)!.ControlledGrade);
		Assert.AreEqual(grade == 3 ? 1 : 0, f.Samples);
		Assert.AreEqual(1, f.Store.Operations.Count);
	}
	[TestMethod]
	public void PaidNativeCreation_SecondCreationFails_HoldsOnceWithoutMasteryOrReplay()
	{
		var f = new MagicCastingFixture();
		var template = Template(f.World.Object);
		var spell = Configure(f, template);
		PrepareSpatial(f);
		Assert.IsTrue(spell.SpellEffects.Single().BuildingCommand(f.Actor.Object, new StringStack("count 3")));
		var npc = new Mock<ICharacter> { DefaultValue = DefaultValue.Mock };
		npc.SetupGet(x => x.State).Returns(CharacterState.Awake);
		npc.SetupGet(x => x.Location).Returns(f.Actor.Object.Location);
		var calls = 0;
		var native = new Mock<ISpellOwnedNpcService>();
		native.Setup(x => x.Create(template, It.IsAny<SpatialLocation>(), It.IsAny<SpellLifecycleOrigin>()))
			.Returns(() =>
			{
				if (++calls == 2) throw new InvalidOperationException("second native creation failed");
				return npc.Object;
			});
		f.World.SetupGet(x => x.SpellOwnedNpcs).Returns(native.Object);
		var intent = new MagicCastingIntent(f.Actor.Object, f.Earth.Id, spell.Id, 3, true, "");

		var result = f.Service.Cast(intent);

		Assert.AreEqual(MagicCastingStatus.NeedsReview, result.Status, result.Message);
		StringAssert.Contains(result.Message, "second native creation failed");
		Assert.AreEqual(2, calls);
		native.Verify(x => x.Create(template, It.IsAny<SpatialLocation>(), It.IsAny<SpellLifecycleOrigin>()), Times.Exactly(2));
		Assert.AreEqual(77.5, f.Balances[f.Resources[1]]);
		Assert.AreEqual(2, f.Store.Acquisition(100, spell.Id)!.ControlledGrade);
		Assert.AreEqual(0, f.Samples);
		var operation = f.Store.Operations[result.OperationId!.Value];
		Assert.AreEqual("NeedsReview", operation.Stage);
		Assert.AreNotEqual("true", XElement.Parse(operation.Definition).Attribute("applied")?.Value);
		Assert.AreEqual(MagicCastingStatus.Refused, f.Service.Cast(intent).Status);
		Assert.AreEqual(1, f.Store.Operations.Count);
		Assert.AreEqual(77.5, f.Balances[f.Resources[1]]);
	}

}
