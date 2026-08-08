#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ClinchEffectTests
{
	[TestMethod]
	public void ClincherLeavesCombat_ExpiresEffect()
	{
		var (effect, clincher, target) = CreateFixture();

		clincher.Raise(x => x.OnLeaveCombat += null!, clincher.Object);

		clincher.Verify(x => x.RemoveEffect(effect, true), Times.Once);
		target.Verify(x => x.RemoveEffect(It.IsAny<IEffect>(), It.IsAny<bool>()), Times.Never);
	}

	[TestMethod]
	public void TargetLeavesCombat_ExpiresEffect()
	{
		var (effect, clincher, target) = CreateFixture();

		target.Raise(x => x.OnLeaveCombat += null!, target.Object);

		clincher.Verify(x => x.RemoveEffect(effect, true), Times.Once);
	}

	[TestMethod]
	public void UnrelatedParticipantLeavesCombat_DoesNotExpireEffect()
	{
		var (effect, clincher, _) = CreateFixture();
		var unrelated = new Mock<ICharacter>();

		unrelated.Raise(x => x.OnLeaveCombat += null!, unrelated.Object);

		clincher.Verify(x => x.RemoveEffect(effect, true), Times.Never);
	}

	[TestMethod]
	public void OneTargetLeavesCombat_OtherHolderEffectRemains()
	{
		var combat = new Mock<ICombat>();
		var holder = CreateCharacter(combat.Object);
		var firstTarget = CreateCharacter(combat.Object);
		var secondTarget = CreateCharacter(combat.Object);
		var firstEffect = new ClinchEffect(holder.Object, firstTarget.Object);
		var secondEffect = new ClinchEffect(holder.Object, secondTarget.Object);

		firstTarget.Raise(x => x.OnLeaveCombat += null!, firstTarget.Object);

		holder.Verify(x => x.RemoveEffect(firstEffect, true), Times.Once);
		holder.Verify(x => x.RemoveEffect(secondEffect, true), Times.Never);
	}

	private static (ClinchEffect Effect, Mock<ICharacter> Clincher, Mock<ICharacter> Target) CreateFixture()
	{
		var combat = new Mock<ICombat>();
		var clincher = CreateCharacter(combat.Object);
		var target = CreateCharacter(combat.Object);
		return (new ClinchEffect(clincher.Object, target.Object), clincher, target);
	}

	private static Mock<ICharacter> CreateCharacter(ICombat combat)
	{
		var character = new Mock<ICharacter>();
		character.SetupGet(x => x.Combat).Returns(combat);
		return character;
	}
}
