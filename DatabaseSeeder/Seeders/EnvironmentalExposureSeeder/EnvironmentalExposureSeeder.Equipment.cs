using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Form.Material;
using MudSharp.Models;

#nullable enable

namespace DatabaseSeeder.Seeders;

public partial class EnvironmentalExposureSeeder
{
	private void InstallEquipmentMaterials(bool fantasy)
	{
		var template = _context.Materials.OrderBy(x => x.Id).First();
		Material? Add(string name, double liquid, double gas, double soak, double threshold)
		{
			var material = new Material();
			_context.Entry(material).CurrentValues.SetValues(template);
			material.Id = 0; material.Name = name; material.MaterialDescription = name;
			material.Density = 1200; material.Absorbency = liquid > 0 ? 0.1 : 0;
			material.HeatDamagePoint = threshold; material.SolventId = null; material.LiquidFormId = null;
			material.ExposureInfo = new MaterialExposureProperties { LiquidTransmission = liquid, GasTransmission = gas, ThermalTransmission = 0.5, SoakPerSecond = soak, ThermalSlope = 0.02, ThermalCap = 4 }.Save();
			var owned = Owned("material:" + name, "equipment", material, () => _context.Materials.FirstOrDefault(x => x.Name == name));
			if (owned is not null) Overlay("MaterialExposure", owned.Id, "material:" + owned.Id, owned.ExposureInfo, material.ExposureInfo, string.IsNullOrWhiteSpace(owned.ExposureInfo) || ReferenceEquals(owned, material), xml => owned.ExposureInfo = xml);
			return owned;
		}
		Add("exposure porous acid-resistant cloth", 1, 1, 0.1, 180);
		Add("exposure sealed susceptible rubber", 0, 0, 0, 100);
		Add("exposure compatible storage polymer", 0, 0, 0, 180);
		if (fantasy)
		{
			var bone = Add("exposure accursed bone", 0, 0, 0, 120);
			var tag = Owned("tag:accursed", "fantasy", new Tag { Name = "Exposure Accursed", ShouldSeeProgId = null },
				() => _context.Tags.FirstOrDefault(x => x.Name == "Exposure Accursed"));
			if (tag is not null && bone is not null)
			{
				var present = _context.MaterialsTags.Any(x => x.MaterialId == bone.Id && x.TagId == tag.Id);
				Overlay("MaterialAccursedTag", bone.Id, $"material:{bone.Id}:tag:{tag.Id}", present.ToString(), bool.TrueString, true,
					value => { if (bool.Parse(value) && !present) _context.MaterialsTags.Add(new MaterialsTags { MaterialId = bone.Id, TagId = tag.Id }); });
			}
			_context.SaveChanges();
		}
	}

	private void InstallEquipment()
	{
		var account = _context.Accounts.OrderBy(x => x.Id).FirstOrDefault();
		var components = _context.GameItemComponentProtos.Where(x => x.RevisionNumber == 0).ToList();
		GameItemComponentProto? Component(string name) => components.SingleOrDefault(x => x.Name == name);
		var hold = Component("Holdable"); var clothing = Component("Destroyable_Clothing"); var miscellaneous = Component("Destroyable_Misc");
		var wear = Component("Wear_Tunic"); var flask = Component("LContainer_Flask");
		if (account is null || hold is null || clothing is null || miscellaneous is null || wear is null || flask is null)
		{
			_notes.Add("Deferred demonstration equipment: install Human wear profiles and Useful item components (Holdable, Wear_Tunic, Destroyable_Clothing, Destroyable_Misc, LContainer_Flask), then rerun.");
			return;
		}
		Add("exposure_porous_tunic", "tunic", "an acid-resistant porous test tunic", "exposure porous acid-resistant cloth", [hold, clothing, wear]);
		Add("exposure_sealed_tunic", "tunic", "a sealed rubber test tunic", "exposure sealed susceptible rubber", [hold, clothing, wear]);
		if (_context.Materials.Any(x => x.Name == "cotton")) Add("exposure_absorbent_tunic", "tunic", "an absorbent cotton test tunic", "cotton", [hold, clothing, wear]);
		Add("exposure_compatible_flask", "flask", "a compatible polymer test flask", "exposure compatible storage polymer", [hold, miscellaneous, flask]);
		if (_context.Materials.Any(x => x.Name == "glass")) Add("exposure_incompatible_flask", "flask", "a glass corrosive-test flask", "glass", [hold, miscellaneous, flask]);
		if (!components.Any(x => x.Name == "Rebreather_Watertight")) _notes.Add("Supplied-air demo requires the optional modern Useful components; no breathing apparatus or filled supply was invented.");
		return;

		void Add(string key, string noun, string description, string materialName, IReadOnlyList<GameItemComponentProto> required)
		{
			var material = _context.Materials.SingleOrDefault(x => x.Name == materialName);
			if (material is null) { _notes.Add($"Deferred {key}: material {materialName} is unavailable."); return; }
			var desired = new GameItemProto
			{
				Id = (_context.GameItemProtos.Max(x => (long?)x.Id) ?? 0) + 1, RevisionNumber = 0, Name = noun, UniqueName = key,
				ShortDescription = description, FullDescription = description + ". This damageable stock example separates its material's susceptibility from liquid, gas and heat transmission.",
				Keywords = noun + " exposure test", MaterialId = material.Id, Size = 4, Weight = noun == "flask" ? 500 : 900, BaseItemQuality = 5,
				MorphEmote = "$0 decays into nothing.", BuilderNotes = "EnvironmentalExposureSeeder: demonstration equipment; no live instance or hazardous room created.",
				EditableItem = new EditableItem { RevisionNumber = 0, RevisionStatus = 4, BuilderAccountId = account.Id, ReviewerAccountId = account.Id,
					BuilderDate = DateTime.UtcNow, ReviewerDate = DateTime.UtcNow, BuilderComment = "Environmental exposure stock", ReviewerComment = "Environmental exposure stock" }
			};
			foreach (var component in required) desired.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos { GameItemProto = desired, GameItemComponent = component });
			Owned("item:" + key, "equipment", desired, () => _context.GameItemProtos.FirstOrDefault(x => x.UniqueName == key));
		}
	}
}
