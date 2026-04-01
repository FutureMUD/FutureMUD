using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Framework;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ValueRangeTests
{
    [TestMethod]
    public void CompareTo_OrdersByMinimumThenMaximum()
    {
        var a = new ValueRange { MinimumValue = 0.0, MaximumValue = 1.0 };
        var b = new ValueRange { MinimumValue = 1.0, MaximumValue = 2.0 };
        var c = new ValueRange { MinimumValue = 0.0, MaximumValue = 2.0 };

        Assert.IsTrue(a.CompareTo(b) < 0, "Range A should come before range B");
        Assert.IsTrue(b.CompareTo(a) > 0, "Range B should come after range A");
        Assert.IsTrue(a.CompareTo(c) < 0, "Range A should come before range C by max");
        Assert.IsTrue(c.CompareTo(a) > 0, "Range C should come after range A by max");
        Assert.AreEqual(0, a.CompareTo(a), "Range should compare equal to itself");
    }

    [TestMethod]
    public void IComparableCompareTo_WithDoubleArgument()
    {
        var range = new ValueRange { MinimumValue = 10.0, MaximumValue = 20.0 };
        IComparable comp = range;

        Assert.AreEqual(-1, comp.CompareTo(5.0), "Value below range should return -1");
        Assert.AreEqual(0, comp.CompareTo(10.0), "Value equal to minimum should return 0");
        Assert.AreEqual(0, comp.CompareTo(15.0), "Value inside range should return 0");
        Assert.AreEqual(1, comp.CompareTo(20.0), "Value equal to maximum should return 1");
        Assert.AreEqual(1, comp.CompareTo(25.0), "Value above range should return 1");
    }

    [TestMethod]
    public void CompareToDouble_Boundaries()
    {
        var range = new ValueRange { MinimumValue = -5.0, MaximumValue = 5.0 };
        var comp = (IComparable<double>)range;

        Assert.AreEqual(-1, comp.CompareTo(-5.1), "Below minimum should return -1");
        Assert.AreEqual(0, comp.CompareTo(-5.0), "Equal to minimum should return 0");
        Assert.AreEqual(0, comp.CompareTo(0.0), "Inside range should return 0");
        Assert.AreEqual(1, comp.CompareTo(5.0), "Equal to maximum should return 1");
        Assert.AreEqual(1, comp.CompareTo(5.1), "Above maximum should return 1");
    }
}
