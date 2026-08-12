#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Commands.Modules;
using MudSharp.Framework;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace MudSharp_Unit_Tests;

[TestClass]
public class EngineUpdateTests
{
	[TestMethod]
	public void EngineUpdateBinariesPath_DefaultConfiguration_PreservesExistingBinariesDirectory()
	{
		Assert.AreEqual(
			"Binaries",
			DefaultStaticSettings.DefaultStaticConfigurations[StaffModule.EngineUpdateBinariesPathStaticConfiguration]);
	}

	[TestMethod]
	public void ResolveEngineUpdateBinariesPath_RelativeConfiguration_UsesApplicationDirectory()
	{
		var applicationDirectory = Path.Combine(Path.GetTempPath(), "futuremud-update-tests", "application");

		Assert.AreEqual(
			Path.GetFullPath(Path.Combine(applicationDirectory, "Update Staging")),
			StaffModule.ResolveEngineUpdateBinariesPath("Update Staging", applicationDirectory));
	}

	[TestMethod]
	public void ResolveEngineUpdateBinariesPath_AbsoluteConfiguration_UsesConfiguredDirectory()
	{
		var configuredDirectory = Path.Combine(Path.GetTempPath(), "futuremud-update-tests", "external-staging");

		Assert.AreEqual(
			Path.GetFullPath(configuredDirectory),
			StaffModule.ResolveEngineUpdateBinariesPath(configuredDirectory, AppContext.BaseDirectory));
	}

	[TestMethod]
	public void ResolveEngineUpdateBinariesPath_EmptyConfiguration_Throws()
	{
		Assert.ThrowsException<InvalidDataException>(() =>
			StaffModule.ResolveEngineUpdateBinariesPath(" ", AppContext.BaseDirectory));
	}

	[TestMethod]
	public void EngineUpdateDownloadUrl_UsesFutureMudLatestRuntimeRoutes()
	{
		Assert.AreEqual(
			"https://futuremud.com/downloads/engine/latest/win-x64",
			StaffModule.EngineUpdateDownloadUrl(true, false, Architecture.X64));
		Assert.AreEqual(
			"https://futuremud.com/downloads/engine/latest/linux-x64",
			StaffModule.EngineUpdateDownloadUrl(false, true, Architecture.X64));
		Assert.AreEqual(
			"https://futuremud.com/downloads/engine/latest/linux-arm64",
			StaffModule.EngineUpdateDownloadUrl(false, true, Architecture.Arm64));
	}

	[TestMethod]
	public void EngineUpdateDownloadUrl_UnsupportedRuntime_Throws()
	{
		Assert.ThrowsException<PlatformNotSupportedException>(() =>
			StaffModule.EngineUpdateDownloadUrl(false, false, Architecture.X64));
	}

	[TestMethod]
	public void ResolveEngineUpdateEntryPath_ValidChild_ReturnsPathBelowExtractionRoot()
	{
		var root = Path.Combine(Path.GetTempPath(), "futuremud-update-tests");
		var destination = StaffModule.ResolveEngineUpdateEntryPath(root, "content/MudSharp");

		Assert.AreEqual(
			Path.GetFullPath(Path.Combine(root, "content", "MudSharp")),
			destination);
	}

	[TestMethod]
	public void ResolveEngineUpdateEntryPath_ParentTraversal_Throws()
	{
		var root = Path.Combine(Path.GetTempPath(), "futuremud-update-tests");

		Assert.ThrowsException<InvalidDataException>(() =>
			StaffModule.ResolveEngineUpdateEntryPath(root, "../outside.txt"));
	}

	[TestMethod]
	public void ResolveEngineUpdateEntryPath_AbsolutePath_Throws()
	{
		var root = Path.Combine(Path.GetTempPath(), "futuremud-update-tests");
		var outside = Path.GetFullPath(Path.Combine(root, "..", "outside.txt"));

		Assert.ThrowsException<InvalidDataException>(() =>
			StaffModule.ResolveEngineUpdateEntryPath(root, outside));
	}
}
