using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Framework;

namespace MudSharp_Unit_Tests;

[TestClass]
public class MultipleNamesExtensionsTests
{
    private class MultiNameStub : IHaveMultipleNames
    {
        public long Id { get; init; }
        public string FrameworkItemType => "Stub";
        public IEnumerable<string> Names { get; init; } = Enumerable.Empty<string>();
        public string Name => Names.First();
    }

    private readonly List<MultiNameStub> _items = new()
    {
        new MultiNameStub { Id = 1, Names = new[] { "alpha", "first alias" } },
        new MultiNameStub { Id = 2, Names = new[] { "beta", "second alias" } }
    };

    [TestMethod]
    public void GetByIdOrNames_ReturnsById()
    {
        Assert.AreSame(_items[1], _items.GetByIdOrNames("2"));
    }

    [TestMethod]
    public void GetByIdOrNames_ReturnsByExactName()
    {
        Assert.AreSame(_items[0], _items.GetByIdOrNames("first alias"));
    }

    [TestMethod]
    public void GetByIdOrNames_ReturnsByAbbreviation()
    {
        Assert.AreSame(_items[1], _items.GetByIdOrNames("bet"));
    }

    [TestMethod]
    public void GetByIdOrNames_ReturnsNullWhenNotFound()
    {
        Assert.IsNull(_items.GetByIdOrNames("gamma"));
    }
}

