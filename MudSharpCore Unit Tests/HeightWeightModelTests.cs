using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MudSharp_Unit_Tests;

[TestClass]
public class HeightWeightModelTests
{
	[TestInitialize]
	public void Setup()
	{
	}

	[TestMethod]
	public void CalculateBMI_ValidInput_ReturnsCorrectBMI()
	{
		// Arrange
		double weight = 70000.0; // g
		double height = 175; // cm

		// Act
		double result = MudSharp.NPC.Templates.HeightWeightModel.GetBMI(height, weight);

		// Assert
		double expected = 22.8571; // Expected BMI value
		Assert.AreEqual(expected, result, 0.0001, "BMI calculation is incorrect");
	}
}