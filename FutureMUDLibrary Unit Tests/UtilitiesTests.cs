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

	private enum ByteEnum : byte
	{
		Zero = 0,
		One = 1
	}

	private enum SByteEnum : sbyte
	{
		MinusOne = -1,
		Zero = 0
	}

	private enum ShortEnum : short
	{
		Zero = 0,
		Two = 2
	}

	private enum UShortEnum : ushort
	{
		Zero = 0,
		Two = 2
	}

	private enum UIntEnum : uint
	{
		Zero = 0,
		Two = 2
	}

	private enum ULongEnum : ulong
	{
		Zero = 0,
		Two = 2
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
	public void CreateList_CachesConstructor()
	{
		var list1 = (List<int>)Utilities.CreateList(typeof(int));
		var list2 = (List<int>)Utilities.CreateList(typeof(int));
		Assert.AreEqual(0, list1.Count);
		Assert.AreEqual(0, list2.Count);
	}

    [TestMethod]
    public void NowNoLonger_ReturnsCorrectStrings()
    {
        Assert.AreEqual("now", true.NowNoLonger());
        Assert.AreEqual("no longer", false.NowNoLonger());
    }

    [Flags]
    private enum FlagEnum
    {
        None = 0,
        One = 1,
        Two = 2,
        Four = 4
    }

    private enum CamelCaseEnum
    {
        SomeValue
    }

    private class InfoAttribute : Attribute
    {
        public string Name { get; }
        public InfoAttribute(string name) => Name = name;
    }

    private enum InfoEnum
    {
        [Info("Alpha")] Alpha,
        Beta
    }

    [TestMethod]
    public void In_ReturnsExpectedResults()
    {
        Assert.IsTrue(5.In(1, 3, 5));
        Assert.IsFalse(4.In(1, 2, 3));

        string value = null;
        Assert.IsTrue(value.In(null, "other"));
    }

    [TestMethod]
    public void AppendLineFormat_AppendsWithFormatProvider()
    {
        var sb = new StringBuilder();
        sb.AppendLineFormat("Hello {0}", "World");
        Assert.AreEqual($"Hello World{Environment.NewLine}", sb.ToString());

        sb.Clear();
        sb.AppendLineFormat(new System.Globalization.CultureInfo("fr-FR"), "Value {0:N1}", 12.5);
        Assert.AreEqual($"Value 12,5{Environment.NewLine}", sb.ToString());
    }

    [TestMethod]
    public void DescribeEnum_FlagCombination()
    {
        var combined = FlagEnum.One | FlagEnum.Two;
        Assert.AreEqual("One and Two", combined.DescribeEnum());

        Assert.AreEqual("Some Value", CamelCaseEnum.SomeValue.DescribeEnum(true));
    }

    [TestMethod]
    public void GetAttribute_ReturnsCustomAttribute()
    {
        var attr = InfoEnum.Alpha.GetAttribute<InfoAttribute>();
        Assert.IsNotNull(attr);
        Assert.AreEqual("Alpha", attr.Name);

        Assert.IsNull(InfoEnum.Beta.GetAttribute<InfoAttribute>());
    }

	[TestMethod]
	public void TryParseEnum_UnderlyingTypes_ParsesNumericValues()
	{
		Assert.IsTrue("1".TryParseEnum<ByteEnum>(out var byteValue));
		Assert.AreEqual(ByteEnum.One, byteValue);
		Assert.IsTrue("-1".TryParseEnum<SByteEnum>(out var sbyteValue));
		Assert.AreEqual(SByteEnum.MinusOne, sbyteValue);
		Assert.IsTrue("2".TryParseEnum<ShortEnum>(out var shortValue));
		Assert.AreEqual(ShortEnum.Two, shortValue);
		Assert.IsTrue("2".TryParseEnum<UShortEnum>(out var ushortValue));
		Assert.AreEqual(UShortEnum.Two, ushortValue);
		Assert.IsTrue("2".TryParseEnum<UIntEnum>(out var uintValue));
		Assert.AreEqual(UIntEnum.Two, uintValue);
		Assert.IsTrue("2".TryParseEnum<ULongEnum>(out var ulongValue));
		Assert.AreEqual(ULongEnum.Two, ulongValue);
		Assert.IsFalse("5".TryParseEnum<ByteEnum>(out _));
	}

	[TestMethod]
	public void SplitDelimiter_TrailingDelimiter_DoesNotAddEmptySegment()
	{
		var input = Encoding.ASCII.GetBytes("A,");
		var parts = input.SplitDelimiter(new byte[] { (byte)',' }, Utilities.ByteSplitOptions.DiscardDelimiter)
			.Select(b => Encoding.ASCII.GetString(b)).ToList();
		CollectionAssert.AreEqual(new List<string> { "A" }, parts);
	}
}
