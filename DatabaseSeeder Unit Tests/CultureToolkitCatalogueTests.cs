#nullable enable

using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using DatabaseSeeder.Seeders.CultureToolkit;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CultureToolkitCatalogueTests
{

	[TestMethod]
	public void UnrelatedResourcesCannotHideMissingRequiredInputs()
	{
		foreach (var missing in CultureToolkitCatalogue.RequiredDocuments)
		{
			var names = CultureToolkitCatalogue.RequiredDocuments.Where(x => x != missing).Append("data.unrelated.json");
			StringAssert.Contains(Assert.ThrowsExactly<InvalidDataException>(() => CultureToolkitCatalogue.ValidateRequiredDocuments(names)).Message, missing);
		}
	}
}
