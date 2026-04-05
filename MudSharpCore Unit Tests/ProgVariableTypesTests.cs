using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.FutureProg;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ProgVariableTypesTests
{
    [TestMethod]
    public void StorageString_LegacyType_RoundTrips()
    {
        ProgVariableTypes original = ProgVariableTypes.Character | ProgVariableTypes.Collection;
        string storage = original.ToStorageString();
        ProgVariableTypes reparsed = ProgVariableTypes.FromStorageString(storage);

        Assert.AreEqual(original, reparsed);
        Assert.AreEqual("v1:408", storage);
    }

    [TestMethod]
    public void StorageString_OverflowType_RoundTrips()
    {
        const string definition = "v1:800000000000000000";

        ProgVariableTypes parsed = ProgVariableTypes.FromStorageString(definition);

        Assert.AreEqual(definition, parsed.ToStorageString());
        Assert.AreEqual(ProgVariableTypeCode.Unknown, parsed.LegacyCode);
    }

    [TestMethod]
    public void ExactKind_AndElementKind_HandleModifiers()
    {
        ProgVariableTypes collectionType = ProgVariableTypes.Text | ProgVariableTypes.Collection;
        ProgVariableTypes literalType = ProgVariableTypes.Boolean | ProgVariableTypes.Literal;

        Assert.AreEqual(ProgTypeKind.Text, collectionType.ElementKind);
        Assert.AreEqual(ProgTypeKind.Unknown, collectionType.ExactKind);
        Assert.IsTrue(collectionType.IsCollection);
        Assert.IsFalse(collectionType.IsExactType);

        Assert.AreEqual(ProgTypeKind.Boolean, literalType.ExactKind);
        Assert.IsTrue(literalType.IsLiteral);
        Assert.IsTrue(literalType.IsExactType);
    }

    [TestMethod]
    public void Compatibility_AndAliases_WorkWithExistingMaskSemantics()
    {
        Assert.IsTrue((ProgVariableTypes.Character | ProgVariableTypes.Collection)
            .CompatibleWith(ProgVariableTypes.Collection));
        Assert.IsTrue(ProgVariableTypes.Character.CompatibleWith(ProgVariableTypes.Toon));
        Assert.IsTrue(ProgVariableTypes.Character.CompatibleWith(ProgVariableTypes.Perceivable));
        Assert.IsFalse(ProgVariableTypes.Text.CompatibleWith(ProgVariableTypes.Perceivable));
    }

    [TestMethod]
    public void TryParse_AcceptsFriendlyLegacyAndStorageFormats()
    {
        Assert.IsTrue(ProgVariableTypes.TryParse("Terrain", out ProgVariableTypes terrain));
        Assert.AreEqual(ProgVariableTypes.Terrain, terrain);

        Assert.IsTrue(ProgVariableTypes.TryParse("4398046511104", out ProgVariableTypes terrainFromLegacy));
        Assert.AreEqual(ProgVariableTypes.Terrain, terrainFromLegacy);

        Assert.IsTrue(ProgVariableTypes.TryParse("Text Collection", out ProgVariableTypes textCollection));
        Assert.AreEqual(ProgVariableTypes.Text | ProgVariableTypes.Collection, textCollection);

        Assert.IsTrue(ProgVariableTypes.TryParse("v1:408", out ProgVariableTypes storageType));
        Assert.AreEqual(ProgVariableTypes.Character | ProgVariableTypes.Collection, storageType);
    }

    [TestMethod]
    public void TryParse_RejectsNegativeLegacyValues()
    {
        Assert.IsFalse(ProgVariableTypes.TryParse("-1", out _));
    }
}
