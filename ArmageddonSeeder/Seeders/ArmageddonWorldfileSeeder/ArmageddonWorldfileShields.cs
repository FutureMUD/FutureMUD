#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Framework.Revision;
using MudSharp.Models;
using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

internal sealed partial class ArmageddonWorldfileWriter
{
	private readonly Dictionary<string, GameItemComponentProto> _shieldComponents = new(StringComparer.Ordinal);

	internal static string? SourceShieldType(ArmWorldItem source)
	{
		if (source.ItemType != 9) return null;
		var text = source.Name + " " + source.ShortDescription;
		bool Has(string word) => Regex.IsMatch(text, @"\b" + word + @"\b", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
		if (Has("buckler")) return "Buckler";
		if (!Has("shield")) return null;
		if (Has("tower")) return "Tower";
		if (Has("kite")) return "Kite";
		if (Has("heater")) return "Heater";
		return "Round";
	}

	private static GameItemComponentProto? NativeShieldComponent(FuturemudDatabaseContext db, string type)
	{
		var shields = db.ShieldTypes.Where(x => x.Name == type).Select(x => x.Id).ToArray();
		if (shields.Length != 1) return null;
		var shield = shields[0];
		// Prefer the original native prototype, using its current approved revision. The stock
		// component carries the native wielding, shield-bash and maintenance defaults unchanged.
		return db.GameItemComponentProtos.Include(x => x.EditableItem)
			.Where(x => x.Type == "Shield" && x.EditableItem.RevisionStatus == (int)RevisionStatus.Current)
			.OrderBy(x => x.Id).ThenByDescending(x => x.RevisionNumber).AsEnumerable()
			.FirstOrDefault(x => (long?)XElement.Parse(x.Definition).Element("ShieldType") == shield);
	}

	internal static IEnumerable<string> ShieldBindingErrors(FuturemudDatabaseContext db, ArmageddonWorldfilePackage source)
	{
		foreach (var type in source.Items.Select(SourceShieldType).Where(x => x is not null).Distinct())
			if (NativeShieldComponent(db, type!) is null)
				yield return $"Combat must supply the native {type} shield type and an approved Shield component.";
	}

	private GameItemComponentProto GetShieldComponent(string type)
	{
		if (_shieldComponents.TryGetValue(type, out var component)) return component;
		component = NativeShieldComponent(_db, type) ?? throw new InvalidOperationException($"Missing approved native {type} Shield component.");
		return _shieldComponents[type] = component;
	}

	internal static bool HasShieldComponent(GameItemProto prototype) =>
		prototype.GameItemProtosGameItemComponentProtos.Any(x => x.GameItemComponent.Type == "Shield");

	internal static OutfitTemplateItemPlacement ResetShieldPlacement(ArmWorldReset reset, GameItemProto prototype, OutfitTemplateItemPlacement placement) =>
		reset.Opcode == "E" && reset.Arguments[3] is 16 or 17 or 19 && HasShieldComponent(prototype)
			? OutfitTemplateItemPlacement.Wielded : placement;
}
