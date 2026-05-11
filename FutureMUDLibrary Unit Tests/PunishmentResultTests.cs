#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.RPG.Law;
using MudSharp.TimeAndDate;

namespace MudSharp_Unit_Tests;

[TestClass]
public class PunishmentResultTests
{
	[TestMethod]
	public void OperatorPlus_PreservesExecutionFlag()
	{
		var combined = new PunishmentResult
		{
			Execution = true
		} + new PunishmentResult
		{
			CustodialSentence = MudTimeSpan.FromDays(1)
		};

		Assert.IsTrue(combined.Execution);
		Assert.AreEqual(1.0, combined.CustodialSentence.TotalDays);
	}
}
