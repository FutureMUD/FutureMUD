using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Combat;
using MudSharp.Commands.Modules;

namespace MudSharp_Unit_Tests;

[TestClass]
public class GameModuleNaturalAttackTests
{
	[TestMethod]
	public void NaturalAttackTypesForDisplay_IncludesPushbackUnarmed()
	{
		CollectionAssert.Contains(GameModule.NaturalAttackTypesForDisplay,
			BuiltInCombatMoveType.PushbackUnarmed);
	}
}
