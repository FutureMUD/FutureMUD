#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Magic;
using MudSharp.Magic.Casting;
using MudSharp.Magic.Lifecycle;
using MudSharp.Magic.SpellEffects;
using MudSharp.NPC.Templates;

namespace MudSharp_Unit_Tests;

public partial class SpellOwnedNpcAdmissionTests
{
	[DataTestMethod]
	[DataRow("0", "true")]
	[DataRow("129", "true")]
	[DataRow("rand(1,7)", "true")]
	[DataRow("grade", "invalid")]
	public void Cast_InvalidCountOrProtection_RefusesBeforePaymentAndPreservesMalformedPolicy(string count, string guard)
	{
		var f = new MagicCastingFixture(); var spell = Configure(f, Template(f.World.Object)); PrepareSpatial(f);
		var xml = spell.SpellEffects.Single().SaveToXml();
		xml.Element("Lifecycle")!.SetElementValue("Count", count);
		xml.Element("Lifecycle")!.SetElementValue("GuardCaster", guard);
		var effect = new CreateNPCEffect(xml, spell);
		var effects = (List<IMagicSpellEffectTemplate>)typeof(MagicSpell).GetField("_spellEffects", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(spell)!;
		effects[0] = effect;
		Assert.IsNotNull(((CreateNPCEffect)effect.Clone()).DefinitionError);
		Assert.IsTrue(XNode.DeepEquals(xml, effect.SaveToXml()));
		var result = f.Service.Cast(new(f.Actor.Object, f.Earth.Id, spell.Id, 3, true, ""));
		Assert.AreEqual(MagicCastingStatus.Refused, result.Status, result.Message);
		Assert.IsNull(result.OperationId);
		Assert.IsTrue(f.Balances.Values.All(x => x == 100));
		Assert.AreEqual(0, f.Rolls); Assert.AreEqual(0, f.Store.Operations.Count); Assert.AreEqual(0, f.Flushes);
	}

	[DataTestMethod]
	[DataRow("count grade")]
	[DataRow("count 2")]
	[DataRow("guardcaster")]
	public void PreparedNpc_CreationPolicyChangedAfterConfirmation_InvalidatesRawFence(string command)
	{
		var f = new MagicCastingFixture(); var spell = Configure(f, Template(f.World.Object)); PrepareSpatial(f);
		var effect = (CreateNPCEffect)Copy(f, spell).SpellEffects.Single();
		var token = (IMagicSpellEffectPreparedSelectionRawToken)effect.CapturePreparedSelection(f.Actor.Object, f.Actor.Object.Location)!;
		Assert.IsTrue(effect.TryConfirmPreparedSelection(f.Actor.Object, f.Actor.Object.Location, out var error), error);
		Assert.IsTrue(effect.BuildingCommand(f.Actor.Object, new StringStack(command)));
		Assert.IsFalse(token.IsCurrent);
	}

	[DataTestMethod]
	[DataRow(true)]
	[DataRow(false)]
	public void Cast_FirstGuardianOnLoadQuitsOrUnloadsCreator_StopsGroupWithoutRefundOrReplay(bool stasis)
	{
		var f = new MagicCastingFixture(); var template = Template(f.World.Object);
		var spell = Configure(f, template); PrepareSpatial(f);
		Assert.IsTrue(((CreateNPCEffect)spell.SpellEffects.Single()).BuildingCommand(f.Actor.Object, new StringStack("count 2")));
		var loaded = true;
		f.World.SetupGet(x => x.Actors).Returns(MagicCastingFixture.Collection(() => loaded ? new[] { f.Actor.Object } : Array.Empty<ICharacter>()));
		var npc = new Mock<ICharacter>(); var npcState = CharacterState.Awake;
		npc.SetupGet(x => x.State).Returns(() => npcState);
		f.Actor.Setup(x => x.Quit(false)).Callback(() =>
		{
			loaded = false;
			if (stasis) f.Actor.SetupGet(x => x.State).Returns(CharacterState.Stasis);
		});
		var onLoad = new Mock<IFutureProg>();
		onLoad.Setup(x => x.Execute(It.IsAny<object[]>())).Callback(() =>
		{
			f.Actor.Object.Quit();
			// The first NPC also dies in the callback: later native login work is unnecessary.
			npcState = CharacterState.Dead;
		});
		template.OnLoadProg = onLoad.Object;
		var native = new Mock<ISpellOwnedNpcService>();
		native.Setup(x => x.Create(template, It.IsAny<SpatialLocation>(), It.IsAny<SpellLifecycleOrigin>())).Returns(npc.Object);
		f.World.SetupGet(x => x.SpellOwnedNpcs).Returns(native.Object);
		var result = f.Service.Cast(new(f.Actor.Object, f.Earth.Id, spell.Id, 3, true, ""));
		Assert.AreEqual(MagicCastingStatus.NeedsReview, result.Status, result.Message);
		native.Verify(x => x.Create(template, It.IsAny<SpatialLocation>(), It.IsAny<SpellLifecycleOrigin>()), Times.Once);
		onLoad.Verify(x => x.Execute(It.IsAny<object[]>()), Times.Once);
		// Grade three is one above this fixture's controlled grade: 15 * 1.5, paid once.
		Assert.AreEqual(77.5, f.Balances[f.Resources[1]]);
		Assert.AreEqual(1, f.Store.Operations.Count);
		Assert.AreEqual("NeedsReview", f.Store.Operations[result.OperationId!.Value].Stage);
	}
}
