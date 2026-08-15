#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Framework;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class PerceivableItemExtensionsTests
{
	[TestMethod]
	public void GetIndividualPerceivables_FlattensMixedSinglesAndGroupsInOrder()
	{
		var single = new Mock<IPerceivable>();
		single.SetupGet(x => x.IsSingleEntity).Returns(true);
		var liquidDummy = new Mock<IPerceivable>();
		liquidDummy.SetupGet(x => x.IsSingleEntity).Returns(true);
		var firstMember = new Mock<IPerceivable>().Object;
		var secondMember = new Mock<IPerceivable>().Object;
		var group = new Mock<IPerceivableGroup>();
		group.SetupGet(x => x.IsSingleEntity).Returns(false);
		group.SetupGet(x => x.Members).Returns([firstMember, secondMember]);

		var result = new IPerceivable[] { single.Object, liquidDummy.Object, group.Object }
			.GetIndividualPerceivables()
			.ToArray();

		CollectionAssert.AreEqual(
			new IPerceivable[] { single.Object, liquidDummy.Object, firstMember, secondMember },
			result);
	}
}
