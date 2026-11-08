#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Prototypes;
using Db = MudSharp.Models;

namespace MudSharp_Unit_Tests;

internal static class ArmageddonNativeLightFixture
{
	internal static IReadOnlyList<Db.GameItemComponentProto> Components(long profileId)
	{
		var manager = new GameItemComponentManager();
		// Read the actual registry's canonical identity by prototype class, not a guessed type string.
		var audit = ((IEnumerable)typeof(GameItemComponentManager).GetProperty("RegistrationAuditEntries", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(manager)!).Cast<object>().ToArray();
		string Canonical(Type prototype)
		{
			var entry = audit.Single(x => (string)x.GetType().GetProperty("PrototypeClass")!.GetValue(x)! == prototype.FullName);
			return (string)entry.GetType().GetProperty("CanonicalDatabaseType")!.GetValue(entry)!;
		}
		var profile = new Mock<IWearProfile>(); profile.SetupGet(x => x.Id).Returns(profileId);
		var profiles = new All<IWearProfile>(); profiles.Add(profile.Object);
		var world = new Mock<IFuturemud>();
		world.SetupGet(x => x.WearProfiles).Returns(profiles);
		world.SetupGet(x => x.FutureProgs).Returns(new All<IFutureProg>());
		world.SetupGet(x => x.SaveManager).Returns(new Mock<ISaveManager>().Object);
		var seeds = new[]
		{
			(typeof(HoldableGameItemComponentProto), new XElement("Definition")),
			(typeof(WearableGameItemComponentProto), new XElement("Definition", new XElement("Profiles", new XAttribute("Default", profileId), new XElement("Profile", profileId)), new XElement("WearableProg", 0), new XElement("WhyCannotWearProg", 0))),
			(typeof(ProgLightGameItemComponentProto), new XElement("Definition", new XElement("IlluminationProvided", 40)))
		};
		return seeds.Select((seed, index) =>
		{
			var row = new Db.GameItemComponentProto { Id = index + 1, RevisionNumber = 0, Name = "selected " + seed.Item1.Name,
				Description = "Native registry serialized fixture", Type = Canonical(seed.Item1), Definition = seed.Item2.ToString(),
				EditableItem = new() { RevisionStatus = (int)RevisionStatus.Current } };
			var component = manager.GetProto(row, world.Object);
			Assert.IsNotNull(component); Assert.IsInstanceOfType(component, seed.Item1); Assert.IsTrue(component!.CanSubmit());
			Assert.AreEqual(row.Type, component.TypeDescription);
			row.Definition = (string)seed.Item1.GetMethod("SaveToXml", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(component, null)!;
			Assert.IsInstanceOfType(manager.GetProto(row, world.Object), seed.Item1);
			return row;
		}).ToArray();
	}
}
