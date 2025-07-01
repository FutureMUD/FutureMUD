using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MC = MudSharp.Framework.CollectionExtensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CollectionExtensionsMoreTests
{
    [TestMethod]
    public void RemoveAllKeys_RemovesMatchingKeys()
    {
        var dict = new Dictionary<int, string> { {1,"a"}, {2,"b"}, {3,"c"} };
        MC.RemoveAllKeys<Dictionary<int,string>,int,string>(dict, k => k%2==0);
        CollectionAssert.AreEquivalent(new[] {1,3}, dict.Keys.ToList());
    }

    [TestMethod]
    public void SumAndConcatVariants_Basic()
    {
        var decimals = new[] {1.5m, 2.5m};
        var (d1,d2,d3) = decimals.Sum3(x=>x, x=>x*2, x=>x*3);
        Assert.AreEqual(4.0m,d1);
        Assert.AreEqual(8.0m,d2);
        Assert.AreEqual(12.0m,d3);

        var longs = new long[]{1,2,3};
        var (l1,l2) = longs.Sum2(x=>x,x=>x*2);
        Assert.AreEqual(6,l1);
        Assert.AreEqual(12,l2);

        IEnumerable<int> list = new List<int>{1,2};
        var result = list.ConcatIfNotNull(3).ToList();
        CollectionAssert.AreEqual(new[]{1,2,3}, result);
        var result2 = MC.ConcatIfNotNull<int?>(list.Cast<int?>(), null).ToList();
        CollectionAssert.AreEqual(new[]{1,2}, result2);
    }

    [TestMethod]
    public void ExceptMethods_Work()
    {
        var src = new[]{1,2,3,2};
        CollectionAssert.AreEqual(new[]{1,3}, src.Except(2).ToList());

        var srcStr = new[]{"a","b","c"};
        CollectionAssert.AreEqual(new[]{"a","c"}, srcStr.ExceptCovariant<string,object>("b").ToList());
    }

    [TestMethod]
    public void Plus_ValueOrDefault_WhereMaxMin()
    {
        var list = new[]{1,3,2};
        CollectionAssert.AreEqual(new[]{1,3,2,4}, list.Plus(4).ToList());

        var dict = new Dictionary<string,int>{{"a",1}};
        Assert.AreEqual(1, dict.ValueOrDefault<string,int,string>("a",0));
        Assert.AreEqual(5, dict.ValueOrDefault<string,int,string?>(null,5));

        var tuples = new[]{(Id:1,Score:5),(Id:2,Score:3),(Id:3,Score:5)};
        var max = tuples.WhereMax(x=>x.Score).Select(x=>x.Id).ToList();
        CollectionAssert.AreEqual(new[]{1,3}, max);
        var min = tuples.WhereMin(x=>x.Score).Select(x=>x.Id).Single();
        Assert.AreEqual(2,min);
    }

    [TestMethod]
    public void NullUtilities_AndMinCount()
    {
        var items = new string?[]{"a",null,"b"};
        CollectionAssert.AreEqual(new[]{"a","b"}, items.WhereNotNull(x=>x).ToList());
        CollectionAssert.AreEqual(new[]{"A","B"}, items.SelectNotNull(x=>x?.ToUpper()).ToList());

        Assert.IsTrue(new[]{1,2,3}.MinCountOrAll(x=>x>0,2));
        Assert.IsTrue(new[]{1,2}.MinCountOrAll(x=>x>0,5));
        Assert.IsFalse(new[]{1,2,3}.MinCountOrAll(x=>x>1,3));

        Assert.IsTrue(0.IsDefault());
        Assert.IsFalse(5.IsDefault());

        var slist = new List<string>{"a"};
        slist.AddNotNull("b");
        slist.AddNotNull<string?>(null);
        CollectionAssert.AreEqual(new[]{"a","b"}, slist);
    }

    [TestMethod]
    public void ArrayAdjacents_Functionality()
    {
        var grid = new int[3,3];
        int val = 0;
        for(int i=0;i<3;i++)
            for(int j=0;j<3;j++)
                grid[i,j]=val++;
        var visited = new List<int>();
        grid.ApplyActionToAdjacents(1,1,x=>visited.Add(x));
        CollectionAssert.AreEquivalent(new[]{0,1,2,3,5,6,7,8}, visited);

        visited.Clear();
        grid.ApplyActionToAdjacentsWithInfo(1,1,(val,dir,x,y)=>visited.Add(val));
        CollectionAssert.AreEquivalent(new[]{0,1,2,3,5,6,7,8}, visited);

        visited.Clear();
        grid.ApplyActionToAdjacentsWithDirection(1,1,(val,dir)=>visited.Add(val));
        CollectionAssert.AreEquivalent(new[]{0,1,2,3,5,6,7,8}, visited);

        var count = grid.ApplyFunctionToAdjacentsReturnCount(1,1,v=>v>4);
        Assert.AreEqual(4,count);
        count = grid.ApplyFunctionToAdjacentsReturnCountWithDirection(1,1,(v,d)=>v%2==0);
        Assert.AreEqual(4,count);

        Assert.AreEqual((1,0), grid.GetCoordsOfElement(3));
    }

    [TestMethod]
    public void FrameworkItemLookups_Work()
    {
        var items = new[]{ new FrameworkItemStub{Name="Alpha",Id=1}, new FrameworkItemStub{Name="Alphabet",Id=2} };
        Assert.AreEqual(1, items.GetByNameOrAbbreviation("alpha")?.Id);
        Assert.AreEqual(1, items.Get(1)?.Id);
        Assert.AreEqual(2, items.GetByIdOrName("2")?.Id);
        Assert.AreEqual(1, items.GetByIdOrOrder("#1")?.Id);
        Assert.AreEqual(1, items.OrdinalPositionOf(items[0]));
    }

    [TestMethod]
    public void RevisableLookups_Work()
    {
        var items = new[]{
            new RevisableItemStub{Name="Alpha",Id=1,RevisionNumber=1,Status=RevisionStatus.Current},
            new RevisableItemStub{Name="Alpha",Id=1,RevisionNumber=2,Status=RevisionStatus.PendingRevision}
        };
        Assert.AreEqual(2, items.GetByIdOrNameRevisableForEditing("Alpha")?.RevisionNumber);
        Assert.AreEqual(1, items.GetByIdOrNameRevisable("Alpha")?.RevisionNumber);
        Assert.AreEqual(1, items.GetByRevisableId("1")?.RevisionNumber);
        Assert.AreEqual(1, items.GetById("1")?.Id);
    }
}
