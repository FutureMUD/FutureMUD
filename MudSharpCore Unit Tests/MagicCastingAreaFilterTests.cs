#nullable enable
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Magic;
using MudSharp.Magic.Casting;
namespace MudSharp_Unit_Tests;
public partial class MagicCastingAreaTests
{
	private delegate bool TargetPolicyExecutor(out object result,object[] arguments);
	[DataTestMethod][DataRow("compile")][DataRow("null")][DataRow("failed")][DataRow("missing")]
	public void AreaTargetPolicy_InvalidationRefusesWithoutPaymentOrDamage(string failure)
	{
		var a=new AreaFixture();a.Target();var prog=CastingTargetFilterTests.Policy(72,failure);
		a.F.World.SetupGet(x=>x.FutureProgs).Returns(MagicCastingFixture.Collection(()=>failure=="missing"?[]:new[]{prog.Object}));
		CastingTargetFilterTests.SetFilter(a.F.Spell,"character",72);
		var before=a.F.Balances[a.F.Resources[1]];var writes=a.F.Store.Writes;
		var result=a.Service.Cast(a.Intent());Assert.AreEqual(MagicCastingStatus.Refused,result.Status,result.Message);
		Assert.AreEqual(before,a.F.Balances[a.F.Resources[1]]);Assert.AreEqual(writes,a.F.Store.Writes);
		Assert.AreEqual(0,a.Damage.Count);Assert.AreEqual(0,a.AreaDraws);Assert.IsNull(result.OperationId);
		if(failure is "null" or "failed")prog.Verify(x=>x.ExecuteWithStatus(out It.Ref<object>.IsAny,It.IsAny<object[]>()),Times.AtLeastOnce());
	}
}
