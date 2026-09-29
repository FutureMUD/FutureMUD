#nullable enable

using System;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Database;
using MudSharp.GameItems.Interfaces;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

/// <summary>Repairs the missing material map in original stock organic corpses without replacing authored maps.</summary>
internal static class OrganicCorpseMaterialSeederHelper
{
	public static void ApplyDefaults(FuturemudDatabaseContext context, CorpseModel model)
	{
		var definition = XElement.Parse(model.Definition);
		if (definition.Element("CorpseMaterials") is not null) return;
		var flesh = context.Materials.FirstOrDefault(x => x.Name == "flesh");
		var bonyFlesh = context.Materials.FirstOrDefault(x => x.Name == "bony flesh");
		var bone = context.Materials.FirstOrDefault(x => x.Name == "bone");
		if (flesh is null || bonyFlesh is null || bone is null) return;
		definition.Add(new XElement("CorpseMaterials", Enum.GetValues<DecayState>().Select(state =>
			new XElement("CorpseMaterial", new XAttribute("state", (int)state), state switch
			{
				DecayState.Skeletal => bone.Id,
				DecayState.Decayed or DecayState.HeavilyDecayed => bonyFlesh.Id,
				_ => flesh.Id
			}))));
		model.Definition = definition.ToString();
	}

	public static void EnsureStockModels(FuturemudDatabaseContext context)
	{
		foreach (var model in context.CorpseModels.Where(x => x.Type == "Standard" &&
			         (x.Name == "Organic Animal Corpse" || x.Name == "Organic Human Corpse")).ToList())
		{
			ApplyDefaults(context, model);
		}
	}
}
