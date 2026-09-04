#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Celestial;
using MudSharp.Construction.Grids;
using MudSharp.Form.Characteristics;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.NPC.Templates;
using MudSharp.Vehicles;
using MudSharp.Work.Agriculture;

namespace FutureMUDLibrary_Unit_Tests.FutureProg;

[TestClass]
public class PhaseOneProgTypeTests
{
	[TestMethod]
	public void PhaseOneTypes_ParseRoundTripAndParticipateInReferenceCollections()
	{
		foreach (var (type, token, kind) in PhaseOneTypes)
		{
			Assert.IsTrue(ProgVariableTypeRegistry.TryParse(token, out var parsed), token);
			Assert.AreEqual(type, parsed, token);
			Assert.IsTrue(ProgVariableTypeRegistry.TryParse(type.ToStorageString(), out var roundTripped), token);
			Assert.AreEqual(type, roundTripped, token);
			Assert.AreEqual(kind, type.ExactKind, token);
			Assert.IsTrue(ProgVariableTypes.CollectionItem.HasFlag(type), token);
			Assert.IsTrue(ProgVariableTypes.ReferenceType.HasFlag(type), token);
			Assert.IsTrue(ProgVariableTypes.Anything.HasFlag(type), token);
		}
	}

	[TestMethod]
	public void PhaseOneReferenceInterfaces_AreProgVariables()
	{
		foreach (var type in new[]
		         {
			         typeof(ITag),
			         typeof(IGameItemProto),
			         typeof(INPCTemplate),
			         typeof(IOutfitTemplate),
			         typeof(IVehicle),
			         typeof(ICelestialObject),
			         typeof(IGrid),
			         typeof(ICharacteristicDefinition),
			         typeof(ICharacteristicValue),
			         typeof(IAgricultureFieldProfile),
			         typeof(IAgricultureCropDefinition),
			         typeof(IAgricultureHerdDefinition),
			         typeof(IAgricultureWoodlandDefinition),
			         typeof(IAgricultureOperation)
		         })
		{
			Assert.IsTrue(typeof(IProgVariable).IsAssignableFrom(type), type.Name);
		}
	}

	private static readonly (ProgVariableTypes Type, string Token, ProgTypeKind Kind)[] PhaseOneTypes =
	[
		(ProgVariableTypes.Tag, "tag", ProgTypeKind.Tag),
		(ProgVariableTypes.ItemPrototype, "itemprototype", ProgTypeKind.ItemPrototype),
		(ProgVariableTypes.NPCTemplate, "npctemplate", ProgTypeKind.NPCTemplate),
		(ProgVariableTypes.OutfitTemplate, "outfittemplate", ProgTypeKind.OutfitTemplate),
		(ProgVariableTypes.Vehicle, "vehicle", ProgTypeKind.Vehicle),
		(ProgVariableTypes.CelestialObject, "celestialobject", ProgTypeKind.CelestialObject),
		(ProgVariableTypes.Grid, "grid", ProgTypeKind.Grid),
		(ProgVariableTypes.CharacteristicDefinition, "characteristicdefinition", ProgTypeKind.CharacteristicDefinition),
		(ProgVariableTypes.CharacteristicValue, "characteristicvalue", ProgTypeKind.CharacteristicValue),
		(ProgVariableTypes.AgricultureFieldProfile, "fieldprofile", ProgTypeKind.AgricultureFieldProfile),
		(ProgVariableTypes.AgricultureCropDefinition, "cropdefinition", ProgTypeKind.AgricultureCropDefinition),
		(ProgVariableTypes.AgricultureHerdDefinition, "herddefinition", ProgTypeKind.AgricultureHerdDefinition),
		(ProgVariableTypes.AgricultureWoodlandDefinition, "woodlanddefinition", ProgTypeKind.AgricultureWoodlandDefinition),
		(ProgVariableTypes.AgricultureOperation, "agricultureoperation", ProgTypeKind.AgricultureOperation)
	];
}
