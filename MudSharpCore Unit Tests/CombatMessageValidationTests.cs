#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Combat;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CombatMessageValidationTests
{
	[TestMethod]
	public void VetEmote_WeaponPushback_AcceptsWeaponMessageTokens()
	{
		var result = CombatMessage.VetEmote(BuiltInCombatMoveType.Pushback,
			"@ brace|braces $2 across $1 and shove|shoves hard to force &1 back.");

		Assert.IsTrue(result.IsValid, result.HelpText);
	}

	[TestMethod]
	[DataRow(BuiltInCombatMoveType.PushbackUnarmed)]
	[DataRow(BuiltInCombatMoveType.PushbackClinch)]
	public void VetEmote_UnarmedPushbacks_AcceptAttackerAndDefender(BuiltInCombatMoveType moveType)
	{
		var result = CombatMessage.VetEmote(moveType, "@ shove|shoves $1 backward.");

		Assert.IsTrue(result.IsValid, result.HelpText);
	}
}
