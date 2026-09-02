#nullable enable

using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace MudSharp_Unit_Tests;

[TestClass]
public class IndustrialisedPrerequisiteAuditTests
{
	private static string RepositoryRoot
	{
		get
		{
			var directory = new DirectoryInfo(AppContext.BaseDirectory);
			while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "FutureMUD.sln")) &&
			       !File.Exists(Path.Combine(directory.FullName, "MudSharp.sln")))
			{
				directory = directory.Parent;
			}

			return directory?.FullName ?? throw new AssertFailedException("Could not locate the repository root.");
		}
	}

	[TestMethod]
	public void ComponentTypeAndPrerequisiteAudits_ContainExactCurrentBaseline()
	{
		using var typeDocument = JsonDocument.Parse(File.ReadAllText(Path.Combine(RepositoryRoot,
			"Design Documents", "Data", "Item_Component_Types.json")));
		var typeRows = typeDocument.RootElement.EnumerateArray().ToList();
		Assert.AreEqual(244, typeRows.Count);
		Assert.AreEqual(244, typeRows
			.Select(x => x.GetProperty("Component Type Name").GetString())
			.Distinct(StringComparer.OrdinalIgnoreCase).Count());
		Assert.AreEqual(109, typeRows.Count(x => x.GetProperty("Technology").GetString() == "Modern"));
		Assert.AreEqual(18, typeRows.Count(x => x.GetProperty("Technology").GetString() == "Futuristic"));
		Assert.IsTrue(typeRows.All(x => x.GetProperty("Has Database Loader").GetBoolean()));
		Assert.IsTrue(typeRows.All(x => x.GetProperty("Has Help").GetBoolean()));

		var componentAuditRows = File.ReadAllLines(Path.Combine(RepositoryRoot, "Design Documents", "Seeding",
			"Industrialised_Component_Prerequisite_Audit.tsv"));
		Assert.AreEqual(245, componentAuditRows.Length);
		Assert.AreEqual(13, componentAuditRows[0].Split('\t').Length);

		var resourceAuditRows = File.ReadAllLines(Path.Combine(RepositoryRoot, "Design Documents", "Seeding",
			"Industrialised_Resource_Prerequisite_Audit.tsv"));
		Assert.IsTrue(resourceAuditRows.Length > 30);
		Assert.IsTrue(resourceAuditRows.Skip(1).All(x => x.Split('\t')[2] == "yes"));
	}

	[TestMethod]
	public void SeededComponentExport_ContainsClosedSourceAndIndustrialisedProfilesExactlyOnce()
	{
		using var document = JsonDocument.Parse(File.ReadAllText(Path.Combine(RepositoryRoot,
			"Design Documents", "Data", "Seeded_Item_Components.json")));
		var rows = document.RootElement.EnumerateArray()
			.Select(x => new
			{
				Name = x.GetProperty("Component Name").GetString()!,
				Type = x.GetProperty("Component Type").GetString()!
			})
			.ToList();
		Assert.AreEqual(rows.Count, rows.Select(x => x.Name).Distinct(StringComparer.OrdinalIgnoreCase).Count());
		foreach (var name in UsefulSeeder.IndustrialisedPrerequisiteComponentNamesForTesting)
		{
			Assert.AreEqual(1, rows.Count(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase)), name);
		}

		string[] recoveredSourceRows =
		[
			"Magazine_Compact_9mm", "Magazine_Service_9mm", "Magazine_SMG_9mm", "Magazine_STANAG_556",
			"Magazine_STANAG_762", "Magazine_Belt_556", "Magazine_Belt_762", "PinPull_Hand_Grenade",
			"Countdown_Plastic_Explosive", "ShopStall_Antiquity_OpenCounter",
			"MarketGoodWeight_Antiquity_StapleFood", "MeasuringInstrument_Antiquity_BalanceScale"
		];
		foreach (var name in recoveredSourceRows)
		{
			Assert.AreEqual(1, rows.Count(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase)), name);
		}
	}
}
