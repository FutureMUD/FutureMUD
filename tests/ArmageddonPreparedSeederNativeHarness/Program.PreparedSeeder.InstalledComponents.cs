#nullable enable
using System.Reflection;
using System.Security.Cryptography;
using System.Text.Json;
using System.Xml.Linq;
using Moq;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Prototypes;
using Db = MudSharp.Models;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	// Builder alias is proglight; the native database loader persists the spaced type.
	private const string InstalledLightDatabaseType = "Prog Light";
	private static string InstalledLightDefinition() => new XElement("Definition", new XElement("IlluminationProvided", 40)).ToString();
	private static string InstalledWearableDefinition(long profile) => new XElement("Definition",
		new XElement("Profiles", new XAttribute("Default", profile), new XElement("Profile", profile)),
		new XElement("WearableProg", 0), new XElement("WhyCannotWearProg", 0)).ToString();

	private static object[] ValidateInstalledComponents(IEnumerable<Db.GameItemComponentProto> rows, long profileId)
	{
		// Actual registration, constructors, readiness and native XML saving are exercised.
		// External catalogue references are bounded fixtures; this is not a MUD boot certificate.
		var world = new Mock<IFuturemud>();
		var profile = new Mock<IWearProfile>(); profile.SetupGet(x => x.Id).Returns(profileId);
		var profiles = new All<IWearProfile>(); profiles.Add(profile.Object);
		world.SetupGet(x => x.WearProfiles).Returns(profiles);
		world.SetupGet(x => x.FutureProgs).Returns(new All<IFutureProg>());
		world.SetupGet(x => x.SaveManager).Returns(new Mock<ISaveManager>().Object);
		var manager = new GameItemComponentManager();
		var validations = new List<object>();
		var loaded = new List<IGameItemComponentProto>();
		foreach (var row in rows)
		{
			var component = manager.GetProto(row, world.Object);
			Require(component is not null, $"Installed dependency {row.Id} has unregistered native type '{row.Type}'.");
			Require(component!.CanSubmit(), $"Installed dependency {row.Id} is not ready: {component.WhyCannotSubmit()}");
			Require(component.TypeDescription == row.Type, $"Installed dependency {row.Id} used an alias rather than its canonical database type.");
			var serializer = component.GetType().GetMethod("SaveToXml", BindingFlags.Instance | BindingFlags.NonPublic)!;
			var saved = (string)serializer.Invoke(component, null)!;
			var reload = manager.GetProto(new Db.GameItemComponentProto { Id = row.Id, RevisionNumber = row.RevisionNumber,
				Name = row.Name, Description = row.Description, Type = row.Type, Definition = saved, EditableItem = row.EditableItem }, world.Object);
			Require(reload is not null && reload.CanSubmit() && reload.GetType() == component.GetType(), $"Installed dependency {row.Id} failed native XML reload/readiness.");
			if (row.Type == InstalledLightDatabaseType)
				Require(component is ProgLightGameItemComponentProto { IlluminationProvided: 40 } && reload is ProgLightGameItemComponentProto { IlluminationProvided: 40 }, "Installed light lost its configured illumination.");
			if (row.Type == "Wearable")
				Require(component is WearableGameItemComponentProto wearable && reload is WearableGameItemComponentProto restored &&
					wearable.DefaultProfile?.Id == profileId && restored.DefaultProfile?.Id == profileId && wearable.Profiles.Count() == 1 && restored.Profiles.Count() == 1 &&
					wearable.WearableProg is null && restored.WearableProg is null && wearable.WhyCannotWearProg is null && restored.WhyCannotWearProg is null,
					"Installed wearable lost its exact profile or introduced a wear script.");
			loaded.Add(component);
			validations.Add(new { row.Id, row.RevisionNumber, row.Type, row.Name, definition = row.Definition, native_saved_definition = saved,
				runtime_type = component.GetType().FullName, registered = true, ready = true, native_xml_roundtrip = true });
		}
		Require(!GameItemComponentPrototypeExclusivity.FindConflicts(loaded).Any() && !GameItemComponentPrototypeRequirements.FindMissingRequirements(loaded).Any(),
			"Installed light component composition violates native exclusivity or sibling requirements.");
		return validations.ToArray();
	}

	private static int PreparedInstalledComponentPreflight()
	{
		Db.GameItemComponentProto Row(long id, string type, string definition) => new() { Id = id, RevisionNumber = 0, Type = type,
			Name = "Installed component preflight " + type, Description = "No database or player writes", Definition = definition,
			EditableItem = new() { BuilderDate = DateTime.UtcNow, RevisionStatus = (int)RevisionStatus.Current } };
		var rows = new[] { Row(1, "Holdable", "<Definition/>"), Row(2, "Wearable", InstalledWearableDefinition(1)), Row(3, InstalledLightDatabaseType, InstalledLightDefinition()) };
		var validations = ValidateInstalledComponents(rows, 1);
		Require(new GameItemComponentManager().GetProto(Row(4, "ProgLight", InstalledLightDefinition()), new Mock<IFuturemud>().Object) is null,
			"The guessed alias unexpectedly became a production registration.");
		Console.WriteLine("ARMPREP-installed-components=passed " + JsonSerializer.Serialize(new { status = "PASS", validations,
			canonical_light_type = InstalledLightDatabaseType, guessed_type_rejected = true, database_started = false, mud_started = false,
			limits = "Native registry/XML/readiness with bounded external catalogue fixtures; actual full-world readiness remains required.",
			assembly = typeof(GNHProgram).Assembly.Location, assembly_sha256 = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(typeof(GNHProgram).Assembly.Location))) }));
		return 0;
	}
}
