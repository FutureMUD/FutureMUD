#nullable enable

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Health;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmourDamageProvenanceTests
{
	[TestMethod]
	[DataRow("natural", false)]
	[DataRow("spell", false)]
	[DataRow("worn", false)]
	[DataRow("worn", true)]
	public void ArmourPackets_RetainExactActorAndWeaponWithoutChangingMagnitudes(string route, bool passive)
	{
		var world = new Mock<IFuturemud>();
		var owner = new Mock<ICharacter>();
		var check = new Mock<ICheck>();
		check.Setup(x => x.Check(owner.Object, Difficulty.Normal, (IPerceivable)null!, (IUseTrait)null!, 0.0,
			TraitUseType.Practical, It.IsAny<(string Parameter, object value)[]>()))
			.Returns(CheckOutcome.SimpleOutcome(CheckType.PenetrationDefenseCheck, Outcome.MajorPass));
		world.Setup(x => x.GetCheck(CheckType.PenetrationDefenseCheck)).Returns(check.Object);
		var type = new ArmourType(new MudSharp.Models.ArmourType
		{
			Id = 17, Name = "Isolated transparent armour", Definition = "<Definition/>"
		}, world.Object);
		var actor = Mock.Of<ICharacter>();
		var tool = Mock.Of<IGameItem>(x => x.Id == 31);
		var incoming = new Damage
		{
			ActorOrigin = actor, ToolOrigin = tool, DamageType = DamageType.Burning,
			DamageAmount = 4.0, PainAmount = 3.0, StunAmount = 2.0,
			AngleOfIncidentRadians = Math.PI * 0.5, PenetrationOutcome = Outcome.MajorFail
		};
		var solid = Mock.Of<ISolid>();
		var wounds = new List<IWound>();
		var packets = new List<IDamage>();
		switch (route)
		{
			case "natural":
				var natural = type.AbsorbDamage(incoming, ItemQuality.Standard, solid, owner.Object, ref wounds);
				packets.Add(natural.SufferedDamage); packets.Add(natural.PassThroughDamage);
				break;
			case "spell":
				var magical = type.AbsorbDamageViaSpell(incoming, solid, ItemQuality.Standard, owner.Object, passive);
				packets.Add(magical.PassedOn); packets.Add(magical.Absorbed);
				break;
			case "worn":
				var item = new Mock<IGameItem>();
				item.SetupGet(x => x.Material).Returns(solid); item.SetupGet(x => x.Quality).Returns(ItemQuality.Standard);
				item.Setup(x => x.SufferDamage(It.IsAny<IDamage>())).Callback<IDamage>(packets.Add).Returns(Array.Empty<IWound>());
				item.Setup(x => x.PassiveSufferDamage(It.IsAny<IDamage>())).Callback<IDamage>(packets.Add).Returns(Array.Empty<IWound>());
				var armour = Mock.Of<IArmour>(x => x.Parent == item.Object);
				packets.Add(type.AbsorbDamage(incoming, armour, owner.Object, ref wounds, passive));
				break;
		}
		Assert.AreEqual(2, packets.Count);
		foreach (var packet in packets)
		{
			Assert.IsNotNull(packet);
			Assert.AreSame(actor, packet.ActorOrigin);
			Assert.AreSame(tool, packet.ToolOrigin);
			Assert.AreEqual(DamageType.Burning, packet.DamageType);
			Assert.AreEqual(4.0, packet.DamageAmount, 0.000001);
			Assert.AreEqual(3.0, packet.PainAmount, 0.000001);
			Assert.AreEqual(2.0, packet.StunAmount, 0.000001);
		}
	}
}
