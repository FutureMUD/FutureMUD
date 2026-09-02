#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using System;
using System.Collections.Generic;
using DbEditableItem = MudSharp.Models.EditableItem;
using DbGameItemComponent = MudSharp.Models.GameItemComponent;
using DbGameItemComponentProto = MudSharp.Models.GameItemComponentProto;

namespace MudSharp_Unit_Tests;

[TestClass]
public class PowerToolComponentTests
{
	[TestMethod]
	public void DurationAwarePowerContract_UsesWattsAndDurationAndPreservesCopyLoadPaths()
	{
		var gameworld = new Mock<IFuturemud>();
		var power = new Mock<IProducePower>();
		var toolTag = new Mock<ITag>();
		var parentTag = new Mock<ITag>();
		parentTag.Setup(x => x.IsA(toolTag.Object)).Returns(true);
		var parent = CreateParent(gameworld.Object, power.Object, parentTag.Object);
		var copyParent = CreateParent(gameworld.Object, power.Object, parentTag.Object);
		var proto = (PowerToolGameItemComponentProto)new GameItemComponentManager()
			.GetProto(CreatePrototype(), gameworld.Object)!;
		var component = (PowerToolGameItemComponent)proto.CreateNew(parent.Object, temporary: true);
		var usage = TimeSpan.FromMinutes(2);
		power.Setup(x => x.CanDrawdownSpike(800.0, usage)).Returns(true);

		component.OnPowerCutIn();

		Assert.IsTrue(component.CanUseTool(toolTag.Object, usage));
		component.UseTool(toolTag.Object, usage);
		Assert.IsTrue(parent.Object.Condition < 1.0);
		power.Verify(x => x.CanDrawdownSpike(800.0, usage), Times.Once);
		power.Verify(x => x.DrawdownSpike(800.0, usage), Times.Once);
		power.Verify(x => x.CanDrawdownSpike(It.IsAny<double>()), Times.Never);
		power.Verify(x => x.DrawdownSpike(It.IsAny<double>()), Times.Never);

		Assert.IsInstanceOfType(component.Copy(copyParent.Object, true), typeof(PowerToolGameItemComponent));
		Assert.IsInstanceOfType(proto.LoadComponent(
			new DbGameItemComponent { Id = 99, Definition = "<Definition />" }, copyParent.Object),
			typeof(PowerToolGameItemComponent));
	}

	[TestMethod]
	public void PowerBankDurationAwareSpike_EnforcesOutputAndDebitsWattHours()
	{
		var gameworld = new Mock<IFuturemud>();
		var parent = new Mock<IGameItem>();
		parent.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		var databasePrototype = CreatePrototype();
		databasePrototype.Type = "PowerBank";
		databasePrototype.Name = "PowerBank_USB_C_Standard";
		databasePrototype.Definition = "<Definition><Connectors /><CapacityInWattHours>40</CapacityInWattHours><MaximumInputInWatts>18</MaximumInputInWatts><MaximumOutputInWatts>18</MaximumOutputInWatts><ChargingEfficiency>0.9</ChargingEfficiency><InputConnectors /><OutputConnectors /></Definition>";
		var proto = (PowerBankGameItemComponentProto)new GameItemComponentManager()
			.GetProto(databasePrototype, gameworld.Object)!;
		var component = (PowerBankGameItemComponent)proto.CreateNew(parent.Object, temporary: true);

		Assert.IsTrue(component.CanDrawdownSpike(18.0, TimeSpan.FromHours(2)));
		Assert.IsTrue(component.DrawdownSpike(18.0, TimeSpan.FromHours(2)));
		Assert.AreEqual(4.0, component.WattHoursRemaining, 1.0e-9);
		Assert.IsFalse(component.CanDrawdownSpike(18.0, TimeSpan.FromMinutes(14)));
		Assert.IsFalse(component.CanDrawdownSpike(19.0, TimeSpan.FromSeconds(1)));
	}

	private static Mock<IGameItem> CreateParent(IFuturemud gameworld, IProducePower power, ITag tag)
	{
		var parent = new Mock<IGameItem>();
		parent.SetupGet(x => x.Gameworld).Returns(gameworld);
		parent.SetupGet(x => x.Tags).Returns(new[] { tag });
		parent.SetupProperty(x => x.Quality, ItemQuality.Standard);
		parent.SetupProperty(x => x.Condition, 1.0);
		parent.Setup(x => x.GetItemType<IProducePower>()).Returns(power);
		return parent;
	}

	private static DbGameItemComponentProto CreatePrototype()
	{
		return new DbGameItemComponentProto
		{
			Id = 7001,
			RevisionNumber = 1,
			Type = "PowerTool",
			Name = "PowerTool_Workshop",
			Description = "Workshop power tool",
			Definition = "<Definition><Wattage>800</Wattage><MultiplierReductionPerQuality>0.05</MultiplierReductionPerQuality><BaseMultiplier>1.0</BaseMultiplier><ToolDurabilitySecondsExpression>(1+quality) * 7200</ToolDurabilitySecondsExpression></Definition>",
			EditableItem = new DbEditableItem
			{
				Id = 7001,
				RevisionNumber = 1,
				RevisionStatus = (int)RevisionStatus.Current,
				BuilderAccountId = 1,
				BuilderDate = DateTime.UtcNow
			}
		};
	}
}
