using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Framework;

namespace MudSharp_Unit_Tests;

[TestClass]
public class UtilitiesTests
{
    private enum SampleEnum
    {
        Alpha = 0,
        Beta = 1,
        Gamma = 2
    }

    [TestMethod]
    public void SplitDelimiter_PreserveDelimiter()
    {
        var input = Encoding.ASCII.GetBytes("A,B,C");
        var parts = input.SplitDelimiter(new byte[] { (byte)',' }, Utilities.ByteSplitOptions.PreserveDelimiter)
                          .Select(b => Encoding.ASCII.GetString(b)).ToList();
        CollectionAssert.AreEqual(new List<string> { "A,", "B,", "C" }, parts);
    }

    [TestMethod]
    public void SplitDelimiter_DiscardDelimiter()
    {
        var input = Encoding.ASCII.GetBytes("A,B,C");
        var parts = input.SplitDelimiter(new byte[] { (byte)',' }, Utilities.ByteSplitOptions.DiscardDelimiter)
                          .Select(b => Encoding.ASCII.GetString(b)).ToList();
        CollectionAssert.AreEqual(new List<string> { "A", "B", "C" }, parts);
    }

    [TestMethod]
    public void TryParseEnum_ValidAndInvalid()
    {
        Assert.IsTrue("Beta".TryParseEnum<SampleEnum>(out var result));
        Assert.AreEqual(SampleEnum.Beta, result);

        Assert.IsTrue("2".TryParseEnum<SampleEnum>(out result));
        Assert.AreEqual(SampleEnum.Gamma, result);

        Assert.IsFalse("Delta".TryParseEnum<SampleEnum>(out result));
        Assert.AreEqual(default(SampleEnum), result);
    }

    [TestMethod]
    public void ParseEnumWithDefault_ReturnsFallbackForInvalid()
    {
        var value = "Gamma".ParseEnumWithDefault(SampleEnum.Alpha);
        Assert.AreEqual(SampleEnum.Gamma, value);

        value = "Unknown".ParseEnumWithDefault(SampleEnum.Alpha);
        Assert.AreEqual(SampleEnum.Alpha, value);
    }

    [TestMethod]
    public void CreateList_Reflective()
    {
        var list = (List<string>)Utilities.CreateList(typeof(string));
        Assert.AreEqual(0, list.Count);
        list.Add("hello");
        Assert.AreEqual("hello", list[0]);
    }

    [TestMethod]
    public void NowNoLonger_ReturnsCorrectStrings()
    {
        Assert.AreEqual("now", true.NowNoLonger());
        Assert.AreEqual("no longer", false.NowNoLonger());
    }
}
