#nullable enable

using MudSharp.Database;
using MudSharp.Models;

namespace MudSharp.Community;

internal static class ClanAddressPersistence
{
	public static ClanAddressRule Load(RankFormOfAddress row) => new()
	{
		Id = row.Id, Text = row.Text, Relations = (ClanAddressRelation)row.Relations,
		IsPublic = row.IsPublic, FutureProgId = row.FutureProgId, TemplateKey = row.TemplateKey
	};

	public static ClanAddressRule Load(AppointmentFormOfAddress row) => new()
	{
		Id = row.Id, Text = row.Text, Relations = (ClanAddressRelation)row.Relations,
		IsPublic = row.IsPublic, FutureProgId = row.FutureProgId, TemplateKey = row.TemplateKey
	};

	public static RankFormOfAddress RankRow(ClanAddressRule rule, int order) => new()
	{
		Id = rule.Id, Text = rule.Text, Relations = (int)rule.Relations, IsPublic = rule.IsPublic,
		FutureProgId = rule.FutureProgId, TemplateKey = rule.TemplateKey, Order = order
	};

	public static AppointmentFormOfAddress AppointmentRow(ClanAddressRule rule, int order) => new()
	{
		Id = rule.Id, Text = rule.Text, Relations = (int)rule.Relations, IsPublic = rule.IsPublic,
		FutureProgId = rule.FutureProgId, TemplateKey = rule.TemplateKey, Order = order
	};

	public static void Save(Models.Rank owner, IReadOnlyList<ClanAddressRule> rules)
	{
		foreach (var row in owner.FormsOfAddress.Where(x => rules.All(y => y.Id != x.Id)).ToList())
			FMDB.Context.RankFormsOfAddress.Remove(row);
		for (var i = 0; i < rules.Count; i++)
		{
			var rule = rules[i];
			var row = owner.FormsOfAddress.SingleOrDefault(x => x.Id == rule.Id);
			if (row is null) owner.FormsOfAddress.Add(RankRow(rule, i));
			else
			{
				row.Order = i; row.Text = rule.Text; row.Relations = (int)rule.Relations;
				row.IsPublic = rule.IsPublic; row.FutureProgId = rule.FutureProgId; row.TemplateKey = rule.TemplateKey;
			}
		}
	}

	public static void Save(Models.Appointment owner, IReadOnlyList<ClanAddressRule> rules)
	{
		foreach (var row in owner.FormsOfAddress.Where(x => rules.All(y => y.Id != x.Id)).ToList())
			FMDB.Context.AppointmentFormsOfAddress.Remove(row);
		for (var i = 0; i < rules.Count; i++)
		{
			var rule = rules[i];
			var row = owner.FormsOfAddress.SingleOrDefault(x => x.Id == rule.Id);
			if (row is null) owner.FormsOfAddress.Add(AppointmentRow(rule, i));
			else
			{
				row.Order = i; row.Text = rule.Text; row.Relations = (int)rule.Relations;
				row.IsPublic = rule.IsPublic; row.FutureProgId = rule.FutureProgId; row.TemplateKey = rule.TemplateKey;
			}
		}
	}
}
