using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Form.Characteristics;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.Form.Material;
using MudSharp.Form.Shape;
using MudSharp.PerceptionEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CommodityCharacteristicRequirementTests
{
	private sealed class TestCharacteristicDefinition : ICharacteristicDefinition
	{
		public TestCharacteristicDefinition(long id, string name)
		{
			Id = id;
			Name = name;
			Pattern = new Regex(name, RegexOptions.IgnoreCase);
		}

		public string Name { get; }
		public long Id { get; }
		public string FrameworkItemType => "CharacteristicDefinition";
		public Regex Pattern { get; }
		public string Description => Name;
		public CharacteristicType Type => CharacteristicType.Standard;
		public CharacterGenerationDisplayType ChargenDisplayType => CharacterGenerationDisplayType.DisplayAll;
		public ICharacteristicValue DefaultValue => null;
		public ICharacteristicDefinition Parent => null;
		public bool Changed { get; set; }
		public IFuturemud Gameworld => null;

		public bool IsValue(ICharacteristicValue value)
		{
			return value?.Definition == this;
		}

		public bool IsDefaultValue(ICharacteristicValue value)
		{
			return false;
		}

		public ICharacteristicValue GetRandomValue()
		{
			return null;
		}

		public void SetDefaultValue(ICharacteristicValue theDefault)
		{
		}

		public void BuildingCommand(MudSharp.Character.ICharacter actor, StringStack command)
		{
		}

		public string Show(MudSharp.Character.ICharacter actor)
		{
			return string.Empty;
		}

		public void Save()
		{
		}
	}

	private sealed class TestCharacteristicValue : ICharacteristicValue
	{
		public TestCharacteristicValue(long id, string name, ICharacteristicDefinition definition)
		{
			Id = id;
			Name = name;
			Definition = definition;
		}

		public string Name { get; }
		public long Id { get; }
		public string FrameworkItemType => "CharacteristicValue";
		public IFutureProg ChargenApplicabilityProg => null;
		public IFutureProg OngoingValidityProg => null;
		public ICharacteristicDefinition Definition { get; }
		public string GetValue => Name;
		public string GetBasicValue => Name;
		public string GetFancyValue => Name;
		public PluralisationType Pluralisation => PluralisationType.Singular;

		public void BuildingCommand(MudSharp.Character.ICharacter actor, StringStack command)
		{
		}

		public string Show(MudSharp.Character.ICharacter actor)
		{
			return string.Empty;
		}

		public ICharacteristicValue Clone(string newName)
		{
			return new TestCharacteristicValue(Id + 1000, newName, Definition);
		}
	}

	[TestMethod]
	public void Matches_Wildcard_AllowsCharacteristicBearingCommodity()
	{
		var definition = new TestCharacteristicDefinition(1, "colour");
		var red = new TestCharacteristicValue(10, "red", definition);
		var requirement = new CommodityCharacteristicRequirement();

		Assert.IsTrue(requirement.Matches(Commodity(new Dictionary<ICharacteristicDefinition, ICharacteristicValue>
		{
			[definition] = red
		})));
	}

	[TestMethod]
	public void Matches_None_RejectsCharacteristicBearingCommodity()
	{
		var definition = new TestCharacteristicDefinition(1, "colour");
		var red = new TestCharacteristicValue(10, "red", definition);
		var requirement = new CommodityCharacteristicRequirement();
		requirement.SetNone();

		Assert.IsFalse(requirement.Matches(Commodity(new Dictionary<ICharacteristicDefinition, ICharacteristicValue>
		{
			[definition] = red
		})));
		Assert.IsTrue(requirement.Matches(Commodity(new Dictionary<ICharacteristicDefinition, ICharacteristicValue>())));
	}

	[TestMethod]
	public void Matches_DefinitionAny_AllowsAnyValueAndExtraCharacteristics()
	{
		var colour = new TestCharacteristicDefinition(1, "colour");
		var weave = new TestCharacteristicDefinition(2, "weave");
		var red = new TestCharacteristicValue(10, "red", colour);
		var herringbone = new TestCharacteristicValue(20, "herringbone", weave);
		var requirement = new CommodityCharacteristicRequirement();
		requirement.SetRequirement(colour, null);

		Assert.IsTrue(requirement.Matches(Commodity(new Dictionary<ICharacteristicDefinition, ICharacteristicValue>
		{
			[colour] = red,
			[weave] = herringbone
		})));
	}

	[TestMethod]
	public void Matches_ExactValue_RequiresThatValue()
	{
		var colour = new TestCharacteristicDefinition(1, "colour");
		var red = new TestCharacteristicValue(10, "red", colour);
		var blue = new TestCharacteristicValue(11, "blue", colour);
		var requirement = new CommodityCharacteristicRequirement();
		requirement.SetRequirement(colour, red);

		Assert.IsTrue(requirement.Matches(Commodity(new Dictionary<ICharacteristicDefinition, ICharacteristicValue>
		{
			[colour] = red
		})));
		Assert.IsFalse(requirement.Matches(Commodity(new Dictionary<ICharacteristicDefinition, ICharacteristicValue>
		{
			[colour] = blue
		})));
	}

	[TestMethod]
	public void CommodityIdentityEqual_RequiresExactCharacteristicsAndIndirectFlag()
	{
		var material = new Mock<ISolid>().Object;
		var tag = new Mock<ITag>().Object;
		var colour = new TestCharacteristicDefinition(1, "colour");
		var weave = new TestCharacteristicDefinition(2, "weave");
		var red = new TestCharacteristicValue(10, "red", colour);
		var herringbone = new TestCharacteristicValue(20, "herringbone", weave);

		var lhs = Commodity(new Dictionary<ICharacteristicDefinition, ICharacteristicValue>
		{
			[colour] = red
		}, material, tag, false);
		var matching = Commodity(new Dictionary<ICharacteristicDefinition, ICharacteristicValue>
		{
			[colour] = red
		}, material, tag, false);
		var extraCharacteristic = Commodity(new Dictionary<ICharacteristicDefinition, ICharacteristicValue>
		{
			[colour] = red,
			[weave] = herringbone
		}, material, tag, false);
		var differentIndirectFlag = Commodity(new Dictionary<ICharacteristicDefinition, ICharacteristicValue>
		{
			[colour] = red
		}, material, tag, true);

		Assert.IsTrue(CommodityCharacteristicRequirement.CommodityIdentityEqual(lhs, matching));
		Assert.IsFalse(CommodityCharacteristicRequirement.CommodityIdentityEqual(lhs, extraCharacteristic));
		Assert.IsFalse(CommodityCharacteristicRequirement.CommodityIdentityEqual(lhs, differentIndirectFlag));
	}

	[TestMethod]
	public void LoadFromXml_MissingElement_DefaultsToWildcard()
	{
		var definition = new TestCharacteristicDefinition(1, "colour");
		var red = new TestCharacteristicValue(10, "red", definition);
		var requirement = new CommodityCharacteristicRequirement();

		requirement.LoadFromXml(null, Gameworld(new[] { definition }, new[] { red }));

		Assert.IsTrue(requirement.IsWildcard);
		Assert.IsTrue(requirement.Matches(Commodity(new Dictionary<ICharacteristicDefinition, ICharacteristicValue>
		{
			[definition] = red
		})));
	}

	[TestMethod]
	public void SaveAndLoad_RoundTripsSpecificRequirements()
	{
		var colour = new TestCharacteristicDefinition(1, "colour");
		var weave = new TestCharacteristicDefinition(2, "weave");
		var red = new TestCharacteristicValue(10, "red", colour);
		var herringbone = new TestCharacteristicValue(20, "herringbone", weave);
		var requirement = new CommodityCharacteristicRequirement();
		requirement.SetRequirement(colour, red);
		requirement.SetRequirement(weave, null);

		XElement xml = requirement.SaveToXml();
		var loaded = new CommodityCharacteristicRequirement();
		loaded.LoadFromXml(xml, Gameworld(new[] { colour, weave }, new[] { red, herringbone }));

		Assert.IsTrue(loaded.Matches(Commodity(new Dictionary<ICharacteristicDefinition, ICharacteristicValue>
		{
			[colour] = red,
			[weave] = herringbone
		})));
		Assert.IsFalse(loaded.Matches(Commodity(new Dictionary<ICharacteristicDefinition, ICharacteristicValue>
		{
			[colour] = red
		})));
	}

	[TestMethod]
	public void CommodityComponent_SaveAndLoad_RoundTripsCharacteristics()
	{
		var colour = new TestCharacteristicDefinition(1, "colour");
		var red = new TestCharacteristicValue(10, "red", colour);
		ISolid material = Material(100, "linen");
		IFuturemud gameworld = ComponentGameworld(material, new[] { colour }, new[] { red });
		CommodityGameItemComponentProto proto = CommodityProto(gameworld);
		IGameItem parent = Parent(gameworld);
		var component = new CommodityGameItemComponent(proto, parent, true)
		{
			Material = material,
			Weight = 250.0
		};
		component.SetCommodityCharacteristic(colour, red);

		string xml = SaveComponent(component);
		var loaded = new CommodityGameItemComponent(new MudSharp.Models.GameItemComponent
		{
			Id = 1,
			Definition = xml
		}, proto, parent);

		Assert.AreEqual(red, loaded.GetCommodityCharacteristic(colour));
		Assert.AreEqual(material, loaded.Material);
		Assert.AreEqual(250.0, loaded.Weight);
	}

	[TestMethod]
	public void CommodityComponent_LoadOldXml_DefaultsToNoCharacteristics()
	{
		var colour = new TestCharacteristicDefinition(1, "colour");
		var red = new TestCharacteristicValue(10, "red", colour);
		ISolid material = Material(100, "linen");
		IFuturemud gameworld = ComponentGameworld(material, new[] { colour }, new[] { red });
		CommodityGameItemComponentProto proto = CommodityProto(gameworld);
		IGameItem parent = Parent(gameworld);

		var loaded = new CommodityGameItemComponent(new MudSharp.Models.GameItemComponent
		{
			Id = 1,
			Definition = "<Definition><Material>100</Material><Weight>125</Weight><Tag>0</Tag></Definition>"
		}, proto, parent);

		Assert.AreEqual(0, loaded.CommodityCharacteristics.Count);
		Assert.IsNull(loaded.GetCommodityCharacteristic(colour));
		Assert.AreEqual(material, loaded.Material);
	}

	[TestMethod]
	public void CommodityComponent_SetRemoveAndClear_ValidateDefinitionValues()
	{
		var colour = new TestCharacteristicDefinition(1, "colour");
		var weave = new TestCharacteristicDefinition(2, "weave");
		var red = new TestCharacteristicValue(10, "red", colour);
		var herringbone = new TestCharacteristicValue(20, "herringbone", weave);
		ISolid material = Material(100, "linen");
		IFuturemud gameworld = ComponentGameworld(material, new[] { colour, weave }, new[] { red, herringbone });
		var component = new CommodityGameItemComponent(CommodityProto(gameworld), Parent(gameworld), true)
		{
			Material = material
		};

		Assert.IsFalse(component.SetCommodityCharacteristic(colour, herringbone));
		Assert.AreEqual(0, component.CommodityCharacteristics.Count);
		Assert.IsTrue(component.SetCommodityCharacteristic(colour, red));
		Assert.AreEqual(red, component.GetCommodityCharacteristic(colour));
		Assert.IsTrue(component.RemoveCommodityCharacteristic(colour));
		Assert.IsNull(component.GetCommodityCharacteristic(colour));
		component.SetCommodityCharacteristic(colour, red);
		component.ClearCommodityCharacteristics();
		Assert.AreEqual(0, component.CommodityCharacteristics.Count);
	}

	[TestMethod]
	public void CommodityComponent_Copy_PreservesCharacteristics()
	{
		var colour = new TestCharacteristicDefinition(1, "colour");
		var red = new TestCharacteristicValue(10, "red", colour);
		ISolid material = Material(100, "linen");
		IFuturemud gameworld = ComponentGameworld(material, new[] { colour }, new[] { red });
		var component = new CommodityGameItemComponent(CommodityProto(gameworld), Parent(gameworld), true)
		{
			Material = material,
			Weight = 100.0,
			UseIndirectQuantityDescription = true
		};
		component.SetCommodityCharacteristic(colour, red);

		var copy = (CommodityGameItemComponent)component.Copy(Parent(gameworld), true);

		Assert.AreEqual(red, copy.GetCommodityCharacteristic(colour));
		Assert.AreEqual(material, copy.Material);
		Assert.AreEqual(100.0, copy.Weight);
		Assert.IsTrue(copy.UseIndirectQuantityDescription);
	}

	[TestMethod]
	public void CommodityComponent_PreventsMerging_WhenCharacteristicsDiffer()
	{
		var colour = new TestCharacteristicDefinition(1, "colour");
		var red = new TestCharacteristicValue(10, "red", colour);
		var blue = new TestCharacteristicValue(11, "blue", colour);
		ISolid material = Material(100, "linen");
		IFuturemud gameworld = ComponentGameworld(material, new[] { colour }, new[] { red, blue });
		CommodityGameItemComponentProto proto = CommodityProto(gameworld);
		var lhs = new CommodityGameItemComponent(proto, Parent(gameworld), true)
		{
			Material = material
		};
		var rhs = new CommodityGameItemComponent(proto, Parent(gameworld), true)
		{
			Material = material
		};
		lhs.SetCommodityCharacteristic(colour, red);
		rhs.SetCommodityCharacteristic(colour, blue);

		Assert.IsTrue(lhs.PreventsMerging(rhs));
		rhs.SetCommodityCharacteristic(colour, red);
		Assert.IsFalse(lhs.PreventsMerging(rhs));
	}

	[TestMethod]
	public void CommodityComponent_Decorate_RendersCharacteristicsBeforeMaterialAndTag()
	{
		var colour = new TestCharacteristicDefinition(1, "colour");
		var red = new TestCharacteristicValue(10, "cherry red", colour);
		ISolid material = Material(100, "linen");
		ITag tag = Tag(200, "fabric bolt");
		IFuturemud gameworld = ComponentGameworld(material, new[] { colour }, new[] { red }, tag);
		var component = new CommodityGameItemComponent(CommodityProto(gameworld), Parent(gameworld), true)
		{
			Material = material,
			Weight = 100.0,
			Tag = tag,
			UseIndirectQuantityDescription = true
		};
		component.SetCommodityCharacteristic(colour, red);

		string description = component.Decorate(new Mock<IPerceiver>().Object, "", "", DescriptionType.Short, false, PerceiveIgnoreFlags.None);

		StringAssert.Contains(description, "cherry red linen fabric bolts");
	}

	private static ICommodity Commodity(
		IReadOnlyDictionary<ICharacteristicDefinition, ICharacteristicValue> characteristics,
		ISolid material = null,
		ITag tag = null,
		bool useIndirect = false)
	{
		var mock = new Mock<ICommodity>();
		mock.SetupGet(x => x.CommodityCharacteristics).Returns(characteristics);
		mock.SetupGet(x => x.Material).Returns(material);
		mock.SetupGet(x => x.Tag).Returns(tag);
		mock.SetupGet(x => x.UseIndirectQuantityDescription).Returns(useIndirect);
		mock.Setup(x => x.GetCommodityCharacteristic(It.IsAny<ICharacteristicDefinition>()))
		    .Returns((ICharacteristicDefinition definition) =>
			    characteristics.TryGetValue(definition, out ICharacteristicValue value) ? value : null);
		return mock.Object;
	}

	private static ISolid Material(long id, string name)
	{
		var mock = new Mock<ISolid>();
		mock.SetupGet(x => x.Id).Returns(id);
		mock.SetupGet(x => x.Name).Returns(name);
		mock.SetupGet(x => x.MaterialDescription).Returns(name);
		mock.SetupGet(x => x.ResidueColour).Returns(Telnet.Cyan);
		return mock.Object;
	}

	private static ITag Tag(long id, string name)
	{
		var mock = new Mock<ITag>();
		mock.SetupGet(x => x.Id).Returns(id);
		mock.SetupGet(x => x.Name).Returns(name);
		mock.SetupGet(x => x.FullName).Returns(name);
		mock.Setup(x => x.IsA(It.IsAny<ITag>())).Returns((ITag tag) => tag == mock.Object);
		return mock.Object;
	}

	private static IGameItem Parent(IFuturemud gameworld)
	{
		var mock = new Mock<IGameItem>();
		mock.SetupGet(x => x.Gameworld).Returns(gameworld);
		mock.SetupGet(x => x.Id).Returns(1);
		return mock.Object;
	}

	private static CommodityGameItemComponentProto CommodityProto(IFuturemud gameworld)
	{
		var dbProto = new MudSharp.Models.GameItemComponentProto
		{
			Id = 1,
			Name = "Commodity",
			Description = "Commodity",
			Definition = "",
			RevisionNumber = 1,
			EditableItem = new MudSharp.Models.EditableItem
			{
				RevisionNumber = 1,
				RevisionStatus = 1,
				BuilderDate = DateTime.UtcNow,
				BuilderAccountId = 1
			}
		};
		return (CommodityGameItemComponentProto)Activator.CreateInstance(
			typeof(CommodityGameItemComponentProto),
			BindingFlags.Instance | BindingFlags.NonPublic,
			null,
			new object[] { dbProto, gameworld },
			null);
	}

	private static string SaveComponent(CommodityGameItemComponent component)
	{
		return (string)typeof(CommodityGameItemComponent)
			.GetMethod("SaveToXml", BindingFlags.Instance | BindingFlags.NonPublic)
			.Invoke(component, Array.Empty<object>());
	}

	private static IFuturemud ComponentGameworld(
		ISolid material,
		IEnumerable<ICharacteristicDefinition> definitions,
		IEnumerable<ICharacteristicValue> values,
		ITag tag = null)
	{
		var mock = new Mock<IFuturemud>();
		mock.SetupGet(x => x.Materials).Returns(Repository(new[] { material }).Object);
		mock.SetupGet(x => x.Tags).Returns(Repository(tag is null ? Array.Empty<ITag>() : new[] { tag }).Object);
		mock.SetupGet(x => x.Characteristics).Returns(Repository(definitions).Object);
		mock.SetupGet(x => x.CharacteristicValues).Returns(Repository(values).Object);
		mock.Setup(x => x.GetStaticDouble(It.IsAny<string>())).Returns(1000000.0);
		return mock.Object;
	}

	private static IFuturemud Gameworld(IEnumerable<ICharacteristicDefinition> definitions, IEnumerable<ICharacteristicValue> values)
	{
		var mock = new Mock<IFuturemud>();
		mock.SetupGet(x => x.Characteristics).Returns(Repository(definitions).Object);
		mock.SetupGet(x => x.CharacteristicValues).Returns(Repository(values).Object);
		return mock.Object;
	}

	private static Mock<IUneditableAll<T>> Repository<T>(IEnumerable<T> items) where T : class, IFrameworkItem
	{
		var list = items.ToList();
		var mock = new Mock<IUneditableAll<T>>();
		mock.As<IEnumerable<T>>()
		    .Setup(x => x.GetEnumerator())
		    .Returns(() => list.GetEnumerator());
		mock.Setup(x => x.Get(It.IsAny<long>()))
		    .Returns((long id) => list.FirstOrDefault(x => x.Id == id));
		mock.Setup(x => x.GetByName(It.IsAny<string>()))
		    .Returns((string name) => list.FirstOrDefault(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)));
		mock.Setup(x => x.GetByIdOrName(It.IsAny<string>(), It.IsAny<bool>()))
		    .Returns((string text, bool _) =>
			    long.TryParse(text, out long id)
				    ? list.FirstOrDefault(x => x.Id == id)
				    : list.FirstOrDefault(x => x.Name.Equals(text, StringComparison.InvariantCultureIgnoreCase)));
		mock.SetupGet(x => x.Count).Returns(list.Count);
		return mock;
	}
}
