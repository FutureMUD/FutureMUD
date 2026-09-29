#nullable enable

using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Effects;
using MudSharp.Events;
using MudSharp.Framework;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CombatTargetRetentionTests
{
	[DataTestMethod]
	[DataRow(CharacterState.Unconscious, false, true)]
	[DataRow(CharacterState.Paralysed, false, true)]
	[DataRow(CharacterState.Awake, false, true)]
	[DataRow(CharacterState.Unconscious, true, false)]
	[DataRow(CharacterState.Awake, true, false)]
	[DataRow(CharacterState.Dead, false, false)]
	public void CheckCombatStatus_TargetedButUnableToAcquire_UsesCombatLeaveRules(
		CharacterState state, bool canLeave, bool remains)
	{
		var victim = TargetlessCharacter.Create(state);
		var attacker = new Mock<ICharacter>();
		attacker.SetupGet(x => x.CombatTarget).Returns(victim);
		var combat = new Mock<ICombat>();
		combat.SetupGet(x => x.Combatants).Returns([victim, attacker.Object]);
		combat.Setup(x => x.CanFreelyLeaveCombat(victim)).Returns(canLeave);
		typeof(PerceiverItem).GetField("_combat", BindingFlags.Instance | BindingFlags.NonPublic)!
			.SetValue(victim, combat.Object);

		Assert.AreEqual(remains, victim.CheckCombatStatus());
		Assert.IsNull(victim.CombatTarget);
	}

	[TestMethod]
	public void CheckCombatStatus_UnopposedUnconsciousVictim_CanLeave()
	{
		var victim = TargetlessCharacter.Create(CharacterState.Unconscious);
		var combat = new Mock<ICombat>();
		combat.SetupGet(x => x.Combatants).Returns([victim]);
		typeof(PerceiverItem).GetField("_combat", BindingFlags.Instance | BindingFlags.NonPublic)!
			.SetValue(victim, combat.Object);
		Assert.IsFalse(victim.CheckCombatStatus());
	}

	private sealed class TargetlessCharacter : MudSharp.Character.Character
	{
		private TargetlessCharacter() : base(null!, null!, true) { }

		public static TargetlessCharacter Create(CharacterState state)
		{
			var character = TestObjectFactory.CreateUninitialized<TargetlessCharacter>();
			character.Body = new Mock<IBody>().Object;
			typeof(PerceivedItem).GetProperty(nameof(EffectHandler))!
				.SetValue(character, new EffectHandler(character));
			typeof(MudSharp.Character.Character).GetField("_state", BindingFlags.Instance | BindingFlags.NonPublic)!
				.SetValue(character, state);
			return character;
		}

		// An able but fully controlled victim cannot re-engage either. Exercise the real acquisition
		// and status methods while isolating the independent engagement permission and event systems.
		public override bool CanEngage(IPerceiver target) => false;
		public override bool HandleEvent(EventType type, params dynamic[] arguments) => false;
		public override bool MeleeRange { get; set; }
	}
}
