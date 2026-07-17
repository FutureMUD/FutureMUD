#nullable enable

using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FutureMUD.Web.Tests;

[TestClass]
public sealed class WebsiteProjectBoundaryTests
{
	private const string DocumentationSource = "../FutureMUDLibrary/Documentation/DocumentationCatalogue.cs";
	private const string DocumentationLink = "Documentation/DocumentationCatalogue.cs";

	[TestMethod]
	public void WebsiteProject_HasNoProjectReferences()
	{
		var projectPath = FindWebsiteProject();
		var document = XDocument.Load(projectPath);
		var projectReferences = document
			.Descendants()
			.Where(element => element.Name.LocalName == "ProjectReference")
			.ToList();

		Assert.AreEqual(
			0,
			projectReferences.Count,
			$"The website must remain independent of the engine project graph. Found: {string.Join(", ", projectReferences.Select(x => x.Attribute("Include")?.Value))}");
	}

	[TestMethod]
	public void WebsiteProject_LinksCanonicalDocumentationContract()
	{
		var projectPath = FindWebsiteProject();
		var document = XDocument.Load(projectPath);
		var linkedContracts = document
			.Descendants()
			.Where(element => element.Name.LocalName == "Compile")
			.Where(element => PathsEqual(element.Attribute("Include")?.Value, DocumentationSource))
			.ToList();

		Assert.AreEqual(1, linkedContracts.Count, "The website must compile the canonical documentation transport contract exactly once.");
		Assert.IsTrue(
			PathsEqual(linkedContracts[0].Attribute("Link")?.Value, DocumentationLink),
			$"The documentation transport contract must be linked as {DocumentationLink}.");

		var projectDirectory = Path.GetDirectoryName(projectPath)!;
		var linkedSource = Path.GetFullPath(Path.Combine(projectDirectory, DocumentationSource));
		Assert.IsTrue(File.Exists(linkedSource), $"The linked documentation transport contract does not exist at {linkedSource}.");
	}

	[TestMethod]
	public void DeploymentHelper_AssignsReleaseOwnershipBeforeRestrictiveModes()
	{
		var projectDirectory = Path.GetDirectoryName(FindWebsiteProject())!;
		var helperPath = Path.Combine(projectDirectory, "Deployment", "deploy-futuremud-web");
		var helper = File.ReadAllText(helperPath);
		var ownership = helper.IndexOf("chown -R root:futuremud-web \"$release\"", StringComparison.Ordinal);
		var directoryModes = helper.IndexOf("find \"$release\" -type d -exec chmod 0750", StringComparison.Ordinal);
		var fileModes = helper.IndexOf("find \"$release\" -type f -exec chmod 0640", StringComparison.Ordinal);

		Assert.IsTrue(ownership >= 0, "The deployment helper must make extracted content readable by the website service group.");
		Assert.IsTrue(ownership < directoryModes && ownership < fileModes,
			"Release ownership must be assigned before the final restrictive directory and file modes are applied.");
	}

	[TestMethod]
	public void DeploymentHelper_UsesNumericTarOwnersForStableSizeParsing()
	{
		var projectDirectory = Path.GetDirectoryName(FindWebsiteProject())!;
		var helperPath = Path.Combine(projectDirectory, "Deployment", "deploy-futuremud-web");
		var helper = File.ReadAllText(helperPath);

		StringAssert.Contains(
			helper,
			"tar -tzf \"$archive\" --numeric-owner --quoting-style=escape",
			"The path listing must not render archive-controlled owner or group names.");
		StringAssert.Contains(
			helper,
			"tar -tvzf \"$archive\" --numeric-owner --quoting-style=escape",
			"The verbose listing must keep the parsed size in a stable field even when archive owner metadata contains whitespace.");
	}

	private static bool PathsEqual(string? actual, string expected)
	{
		return string.Equals(
			actual?.Replace('\\', '/'),
			expected,
			StringComparison.OrdinalIgnoreCase);
	}

	private static string FindWebsiteProject()
	{
		var directory = new DirectoryInfo(AppContext.BaseDirectory);
		while (directory is not null)
		{
			var projectPath = Path.Combine(directory.FullName, "FutureMUD.Web", "FutureMUD.Web.csproj");
			if (File.Exists(projectPath))
			{
				return projectPath;
			}

			directory = directory.Parent;
		}

		throw new AssertFailedException($"Could not locate FutureMUD.Web.csproj from {AppContext.BaseDirectory}.");
	}
}
