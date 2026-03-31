#nullable enable

using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Framework;

namespace MudSharp_Unit_Tests;

[TestClass]
public class AllLookupAliasTests
{
	private sealed class MultiNameFrameworkItemStub : IFrameworkItem, IHaveMultipleNames
	{
		public long Id { get; init; }
		public string FrameworkItemType => "Stub";
		public IEnumerable<string> Names { get; init; } = Enumerable.Empty<string>();
		public string Name => Names.First();
	}

	[TestMethod]
	public void Get_ExactAliasMatch_ReturnsMatchingItem()
	{
		var all = new All<IFrameworkItem>();
		var item = new MultiNameFrameworkItemStub { Id = 1, Names = ["mild steel", "steel"] };
		all.Add(item);

		var results = all.Get("steel");

		Assert.AreEqual(1, results.Count);
		Assert.AreSame(item, results.Single());
		Assert.IsTrue(all.Has("steel"));
	}

	[TestMethod]
	public void GetByIdOrName_AbbreviatedAliasMatch_ReturnsMatchingItem()
	{
		var all = new All<IFrameworkItem>();
		var item = new MultiNameFrameworkItemStub { Id = 1, Names = ["high-density polyethylene", "hdpe"] };
		all.Add(item);

		var result = all.GetByIdOrName("hd");

		Assert.AreSame(item, result);
	}

	[TestMethod]
	public void GetByName_CanonicalNameLookup_StillWorks()
	{
		var all = new All<IFrameworkItem>();
		var item = new MultiNameFrameworkItemStub { Id = 1, Names = ["polycarbonate", "pc"] };
		all.Add(item);

		var result = all.GetByName("polycarbonate");

		Assert.AreSame(item, result);
	}

	[TestMethod]
	public void GetByIdOrName_NonMultiNameItem_BehaviourRemainsUnchanged()
	{
		var all = new All<IFrameworkItem>();
		var item = new FrameworkItemStub { Id = 1, Name = "delta" };
		all.Add(item);

		var result = all.GetByIdOrName("del");

		Assert.AreSame(item, result);
		Assert.IsFalse(all.Has("del"));
	}
}
