#nullable enable

using System.Globalization;
using System.Reflection;
using System.Text.Json;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Framework.Revision;
using MudSharp.GameItems;
using MudSharp.Magic;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using GameItemComponentProto = MudSharp.Models.GameItemComponentProto;
using GameItemProto = MudSharp.Models.GameItemProto;

internal static class PocketFixtures
{
	internal static void Run(DbContextOptions<FuturemudDatabaseContext> options, string receipt)
	{
		using var db = new FuturemudDatabaseContext(options);
		using var transaction = db.Database.BeginTransaction();
		var token = Path.GetFileName(Path.GetDirectoryName(receipt))!.Split('-').Last();
		var keywordSuffix = string.Concat(token.Select(c => (char)('a' + int.Parse(c.ToString(), NumberStyles.HexNumber))));
		var seed = db.GameItemProtos.Include(x => x.EditableItem).Single(x => x.UniqueName == "QA N18 Effigy");
		var hold = db.GameItemComponentProtos.Include(x => x.EditableItem).First(x => x.Type == "Holdable" && x.EditableItem.RevisionStatus == (int)RevisionStatus.Current);
		var mass = 1.0 / double.Parse(db.StaticConfigurations.Single(x => x.SettingName == "BaseWeightUOMToKilograms").Definition, System.Globalization.CultureInfo.InvariantCulture);
		GameItemComponentProto Component(string type)
		{
			var name = "QA N20 " + token + " " + type;
			if (db.GameItemComponentProtos.SingleOrDefault(x => x.Name == name) is { } existing) return existing;
			var component = new GameItemComponentProto { Id = db.GameItemComponentProtos.Max(x => x.Id) + 1, RevisionNumber = 0,
				Name = name, Description = "Disposable N20 custody fixture.", Type = type,
				Definition = new XElement("Definition", new XAttribute("Weight", 100 * mass), new XAttribute("MaxSize", (int)SizeCategory.Normal),
					new XAttribute("Closable", true), new XAttribute("Transparent", true), new XAttribute("OnceOnly", false), new XAttribute("Preposition", "in")).ToString(),
				EditableItem = Copy(seed.EditableItem) };
			component.EditableItem.Id = 0; db.GameItemComponentProtos.Add(component); db.SaveChanges(); return component;
		}
		var container = Component("Container"); var pocket = Component("FoldedPocket");
		GameItemProto Item(string label, double kilograms, GameItemComponentProto? extra = null)
		{
			var name = "QA N20 " + token + " " + label;
			if (db.GameItemProtos.SingleOrDefault(x => x.UniqueName == name) is { } existing) return existing;
			var item = Copy(seed); item.Id = db.GameItemProtos.Max(x => x.Id) + 1; item.RevisionNumber = 0;
			item.Name = name; item.UniqueName = name; item.Keywords = "qan" + label.ToLowerInvariant() + keywordSuffix;
			item.ShortDescription = "a " + item.Keywords + " fixture"; item.FullDescription = "A disposable native N20 fixture.";
			item.Weight = kilograms * mass; item.Size = (int)SizeCategory.Small;
			item.EditableItemId = 0; item.EditableItem = Copy(seed.EditableItem); item.EditableItem.Id = 0;
			foreach (var c in extra is null ? new[] { hold } : new[] { hold, extra })
				item.GameItemProtosGameItemComponentProtos.Add(new() { GameItemComponentProtoId = c.Id, GameItemComponentRevision = c.RevisionNumber, GameItemProto = item });
			db.GameItemProtos.Add(item); db.SaveChanges(); return item;
		}
		var prototypes = new Dictionary<string, long> { ["pocket"] = Item("Pocket", .1, pocket).Id, ["inner"] = Item("Inner", .1, pocket).Id, ["focus"] = Item("Focus", .1).Id,
			["bag"] = Item("Bag", 1, container).Id, ["outer"] = Item("Outer", 1, container).Id,
			["child"] = Item("Child", 1).Id, ["full"] = Item("Full", 2).Id, ["overflow"] = Item("Overflow", 1).Id };
		transaction.Commit();
		File.WriteAllText(receipt, JsonSerializer.Serialize(new { Status = "PASS", Prototypes = prototypes, Keywords = prototypes.Keys.ToDictionary(x => x, x => "qan" + x + keywordSuffix), Capacity = 2 * mass }, new JsonSerializerOptions { WriteIndented = true }));
	}
	private static T Copy<T>(T source) where T : class, new()
	{
		var row = new T();
		foreach (var p in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(x => x.CanWrite && (x.PropertyType.IsValueType || x.PropertyType == typeof(string)))) p.SetValue(row, p.GetValue(source));
		return row;
	}
}
