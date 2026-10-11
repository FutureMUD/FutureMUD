#nullable enable

using System.Reflection;
using System.Text.Json.Nodes;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Models;

namespace FuryCalmSmokeWorld;

/// <summary>Private N23 routes and supplies in the existing owned N22 ordinary-player world.</summary>
internal static class RechargeFixtures
{
	internal static void Run(DbContextOptions<FuturemudDatabaseContext> options, string receipt)
	{
		DeviceFixtures.Run(options, receipt);
		var result = JsonNode.Parse(File.ReadAllText(receipt))!.AsObject();
		using var db = new FuturemudDatabaseContext(options);
		using var transaction = db.Database.BeginTransaction();
		var token = Path.GetFileName(Path.GetDirectoryName(receipt))!.Split('-').Last();
		var capability = db.MagicCapabilities.Single(x => x.Id == result["Capability"]!.GetValue<long>());
		var merit = db.Merits.Single(x => x.Id == result["Merit"]!.GetValue<long>());
		var mend = result["Mend"]!.GetValue<long>();
		var focus = result["Focus"]!.GetValue<long>();
		var trait = result["Trait"]!.GetValue<long>();
		(long Capability, long Merit) Route(string role, params long[] spells)
		{
			var row = Copy(capability); row.Id = 0; row.Name = "QA N23 " + token + " " + role;
			var root = XElement.Parse(capability.Definition);
			var casting = root.Element("Casting")!;
			casting.SetAttributeValue("identity", Guid.NewGuid());
			casting.SetAttributeValue("trait", trait);
			casting.SetAttributeValue("startingVersion", 1);
			casting.RemoveNodes();
			foreach (var spell in spells)
				casting.Add(new XElement("Admission", new XAttribute("key", Guid.NewGuid()),
					new XAttribute("spell", spell), new XAttribute("starting", false),
					new XAttribute("trait", trait), new XAttribute("min", 1), new XAttribute("max", 7)));
			row.Definition = root.ToString(); db.MagicCapabilities.Add(row); db.SaveChanges();
			var grant = Copy(merit); grant.Id = 0; grant.Name = row.Name + " caster";
			var grantRoot = XElement.Parse(merit.Definition);
			grantRoot.Element("Capabilities")!.ReplaceNodes(new XElement("Capability", row.Id));
			grant.Definition = grantRoot.ToString(); db.Merits.Add(grant); db.SaveChanges();
			return (row.Id, grant.Id);
		}
		var alternate = Route("alternate", mend, focus);
		var wrong = Route("incompatible", focus);
		var component = db.GameItemComponentProtos.Single(x => x.Id == result["PrivateComponent"]!.GetValue<long>() && x.RevisionNumber == 0);
		var deviceRoot = XElement.Parse(component.Definition);
		deviceRoot.Element("Seconds")!.Value = "60";
		deviceRoot.Add(new XElement("Spell", focus));
		var materialId = result["Material"]!.GetValue<long>();
		var materialTag = db.GameItemProtosTags.Single(x => x.GameItemProtoId == materialId).TagId;
		// This private item explicitly allows nearby supplies; stock and carried-only focus plans are unchanged.
		deviceRoot.Element("Plan")!.ReplaceWith(new XElement("Plan", new XElement("Phase", new XElement("Action",
			new XAttribute("state", "consumed"), new XAttribute("tag", materialTag), new XAttribute("secondtag", 0),
			new XAttribute("quantity", 1)))));
		component.Definition = deviceRoot.ToString();
		var interrupt = Copy(component); interrupt.Id = db.GameItemComponentProtos.Max(x => x.Id) + 1;
		interrupt.Name = "QA N23 " + token + " interruption"; interrupt.EditableItemId = 0;
		interrupt.EditableItem = Copy(db.EditableItems.Single(x => x.Id == component.EditableItemId)); interrupt.EditableItem.Id = 0;
		var interruptRoot = XElement.Parse(component.Definition);
		interruptRoot.Element("Seconds")!.Value = "60";
		interruptRoot.Element("Plan")!.ReplaceNodes(new XElement("Phase"));
		interrupt.Definition = interruptRoot.ToString(); db.GameItemComponentProtos.Add(interrupt); db.SaveChanges();
		var prototype = db.GameItemProtos.Include(x => x.EditableItem).Include(x => x.GameItemProtosGameItemComponentProtos)
			.Single(x => x.Id == result["PrivateDevice"]!.GetValue<long>() && x.RevisionNumber == 0);
		var item = Copy(prototype); item.Id = db.GameItemProtos.Max(x => x.Id) + 1;
		item.Name = "QA N23 " + token + " interruption"; item.UniqueName = item.Name;
		item.Keywords = "qan23interrupt"; item.ShortDescription = "a qan23interrupt fixture";
		item.EditableItemId = 0; item.EditableItem = Copy(prototype.EditableItem); item.EditableItem.Id = 0;
		foreach (var link in prototype.GameItemProtosGameItemComponentProtos)
			item.GameItemProtosGameItemComponentProtos.Add(new() { GameItemProto = item,
				GameItemComponentProtoId = link.GameItemComponentProtoId == component.Id ? interrupt.Id : link.GameItemComponentProtoId,
				GameItemComponentRevision = link.GameItemComponentRevision });
		db.GameItemProtos.Add(item); db.SaveChanges(); transaction.Commit();
		result["Scope"] = "N23 recharge; independent private fixtures, native ordinary accounts retained";
		result["AlternateCapability"] = alternate.Capability; result["AlternateMerit"] = alternate.Merit;
		result["WrongCapability"] = wrong.Capability; result["WrongMerit"] = wrong.Merit;
		result["InterruptDevice"] = item.Id; result["InterruptComponent"] = interrupt.Id;
		result["MaterialTag"] = materialTag; result["PrivateInitialDefinition"] = component.Definition;
		File.WriteAllText(receipt, result.ToJsonString(new System.Text.Json.JsonSerializerOptions { WriteIndented = true }));
	}

	private static T Copy<T>(T source) where T : class, new()
	{
		var row = new T();
		foreach (var property in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
			.Where(x => x.CanWrite && (x.PropertyType.IsValueType || x.PropertyType == typeof(string))))
			property.SetValue(row, property.GetValue(source));
		return row;
	}
}
