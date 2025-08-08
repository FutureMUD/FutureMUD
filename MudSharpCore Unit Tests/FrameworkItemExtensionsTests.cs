using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Framework;

namespace MudSharp_Unit_Tests;

[TestClass]
public class FrameworkItemExtensionsTests
{
    [TestMethod]
    public void FrameworkItemEquals_TrueWhenItemIdAndTypeMatch()
    {
        var item = new FrameworkItemStub { Id = 1 };
        Assert.IsTrue(item.FrameworkItemEquals(1, "Stub"));
    }

    [TestMethod]
    public void FrameworkItemEquals_FalseWhenIdDiffers()
    {
        var item = new FrameworkItemStub { Id = 1 };
        Assert.IsFalse(item.FrameworkItemEquals(2, "Stub"));
    }

    [TestMethod]
    public void FrameworkItemEquals_FalseWhenTypeDiffers()
    {
        var item = new FrameworkItemStub { Id = 1 };
        Assert.IsFalse(item.FrameworkItemEquals(1, "Other"));
    }

    [TestMethod]
    public void FrameworkItemEquals_TrueWhenItemNullAndIdNull()
    {
        IFrameworkItem item = null;
        Assert.IsTrue(item.FrameworkItemEquals(null, "Stub"));
    }

    [TestMethod]
    public void FrameworkItemEquals_FalseWhenItemNullAndIdNonNull()
    {
        IFrameworkItem item = null;
        Assert.IsFalse(item.FrameworkItemEquals(1, "Stub"));
    }
}
