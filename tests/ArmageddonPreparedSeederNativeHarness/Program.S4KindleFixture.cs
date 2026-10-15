#nullable enable
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Db = MudSharp.Models;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private static void ReconcileS4Kindle(TestDatabase database, string inputPath)
	{
		const string key = "arm.spell.kindle_enchantment";
		string PreservedRows()
		{
			using var db=NewIndependentContext(database.ConnectionString);
			var spell=db.SeederManagedRecords.Single(x => x.StableKey==key).LogicalId;
			var prog=db.SeederManagedRecords.Single(x => x.StableKey==key+".eligibility").LogicalId;
			return JsonSerializer.Serialize(new {
				spells=db.MagicSpells.AsNoTracking().Where(x => x.Id!=spell).OrderBy(x => x.Id).ToArray().Select(row => db.Entry(row).Properties.ToDictionary(property => property.Metadata.Name, property => property.CurrentValue)).ToArray(),
				progs=db.FutureProgs.AsNoTracking().Where(x => x.Id!=prog).OrderBy(x => x.Id).ToArray().Select(row => db.Entry(row).Properties.ToDictionary(property => property.Metadata.Name, property => property.CurrentValue)).ToArray(),
				claims=db.SeederManagedRecords.AsNoTracking().Where(x => !x.StableKey.StartsWith(key) &&
					x.StableKey!="arm.binding.trait.kindle_wisdom" && x.StableKey!="arm.binding.trait.kindle_endurance").OrderBy(x => x.Id).ToArray().Select(row => db.Entry(row).Properties.ToDictionary(property => property.Metadata.Name, property => property.CurrentValue)).ToArray() });
		}
		var tables=S4TableChecksums(database);var preserved=PreservedRows();long original;
		using(var db=NewIndependentContext(database.ConnectionString)) original=db.SeederManagedRecords.Single(x => x.StableKey==key).LogicalId!.Value;
		var result=ArmageddonStockSpellInstaller.Install(() => NewIndependentContext(database.ConnectionString));
		Require(result.Status==ArmageddonInstallStatus.Completed,"Kindle normal owned reconciliation failed: "+string.Join(";",result.Messages));
		using(var db=NewIndependentContext(database.ConnectionString))
		{
			Require(db.SeederManagedRecords.Single(x => x.StableKey==key).LogicalId==original,"Kindle changed owned spell identity.");
			var xml=XElement.Parse(db.MagicSpells.Single(x => x.Id==original).Definition);
			Require((string?)xml.Element("Trigger")?.Attribute("type")=="itemchoice" && (string?)xml.Element("Effects")?.Elements().Single().Attribute("type")=="itemempower","Kindle did not reconcile to selected item enchantment.");
		}
		var after=S4TableChecksums(database);var changed=tables.Keys.Where(x => tables[x]!=after[x]).Order().ToArray();
		Require(changed.All(x => x.Equals("MagicSpells",StringComparison.OrdinalIgnoreCase) || x.Equals("FutureProgs",StringComparison.OrdinalIgnoreCase) || x.Equals("SeederManagedRecords",StringComparison.OrdinalIgnoreCase)) && preserved==PreservedRows(),"Kindle reconciliation changed unrelated rows/tables: "+string.Join(",",changed));
		var checksum=PreparedDatabaseChecksum(database);
		result=ArmageddonStockSpellInstaller.Install(() => NewIndependentContext(database.ConnectionString));
		Require(result.Status==ArmageddonInstallStatus.Completed && checksum==PreparedDatabaseChecksum(database),"Kindle reconciled rerun changed database.");
		var input=JsonNode.Parse(File.ReadAllText(inputPath))!;
		input["kindle_reconciliation"]=JsonSerializer.SerializeToNode(new { spell=original, changed_tables=changed, unrelated_rows_preserved=true, rerun_noop=true, checksum });
		File.WriteAllText(inputPath,input.ToJsonString());
	}

	private static void PrepareS4KindleProp(MudSharp.Database.FuturemudDatabaseContext db, JsonObject input, long source)
	{
		var original=db.GameItemProtos.AsNoTracking().Include(x => x.EditableItem).Include(x => x.GameItemProtosGameItemComponentProtos).Single(x => x.Id==source);
		var component=new Db.GameItemComponentProto { Id=db.GameItemComponentProtos.Max(x => x.Id)+1,RevisionNumber=0,
			Name="S4 private wieldable prop",Description="Private ordinary native wieldable prop.",Type="Wieldable",Definition="<Definition><CanWieldProg>0</CanWieldProg><WhyCannotWieldProg>0</WhyCannotWieldProg></Definition>",
			EditableItem=new() { RevisionStatus=0 } };
		db.Entry(component.EditableItem).CurrentValues.SetValues(original.EditableItem);component.EditableItem.Id=0;
		db.Add(component);db.SaveChanges();
		var proto=new Db.GameItemProto();db.Entry(proto).CurrentValues.SetValues(original);
		proto.Id=db.GameItemProtos.Max(x => x.Id)+1;proto.RevisionNumber=0;proto.Name="s4kindleprop";proto.UniqueName="s4kindleprop";proto.Keywords="s4kindleprop prop";
		proto.ShortDescription="a plain S4 kindle prop";proto.FullDescription="A private ordinary wieldable prop for isolated native acceptance.";
		proto.EditableItem=new();db.Entry(proto.EditableItem).CurrentValues.SetValues(original.EditableItem);proto.EditableItem.Id=0;proto.EditableItemId=0;
		db.Add(proto);db.SaveChanges();
		foreach(var row in original.GameItemProtosGameItemComponentProtos)
			proto.GameItemProtosGameItemComponentProtos.Add(new() { GameItemProto=proto,GameItemComponentProtoId=row.GameItemComponentProtoId,GameItemComponentRevision=row.GameItemComponentRevision });
		proto.GameItemProtosGameItemComponentProtos.Add(new() { GameItemProto=proto,GameItemComponentProtoId=component.Id,GameItemComponentRevision=0 });db.SaveChanges();
		input["family_acceptance"]!["kindle_prototype"]=proto.Id;
		var resource=input["family_acceptance"]!["routes"]![0]!["resource"]!.GetValue<long>();
		input["family_acceptance"]!["kindle_resource_name"]=db.MagicResources.Single(x => x.Id==resource).Name;
	}
}
