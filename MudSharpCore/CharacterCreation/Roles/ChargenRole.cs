using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MudSharp.Accounts;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.CharacterCreation.Resources;
using MudSharp.Database;
using MudSharp.Economy.Currency;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.Models;
using MudSharp.RPG.Merits;

namespace MudSharp.CharacterCreation.Roles;

internal class ChargenRole : SaveableItem, IChargenRole
{
	public ChargenRole(IAccount originator, ChargenRoleType type, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		using (new FMDB())
		{
			var dbitem = new Models.ChargenRole();
			FMDB.Context.ChargenRoles.Add(dbitem);
			dbitem.Name = "An unnamed role";
			dbitem.ChargenBlurb =
				"This role has not been described. You almost certainly should not be selecting it.";
			dbitem.Expired = false;
			dbitem.PosterId = originator.Id;
			dbitem.MaximumNumberAlive = 0;
			dbitem.MaximumNumberTotal = 0;
			dbitem.Type = (int)type;
			FMDB.Context.SaveChanges();
			LoadFromDatabase(dbitem);
		}
	}

	public ChargenRole(MudSharp.Models.ChargenRole role, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		LoadFromDatabase(role);
	}

	public override void Save()
	{
		using (new FMDB())
		{
			var dbitem = FMDB.Context.ChargenRoles.Find(Id);
			dbitem.Name = Name;
			dbitem.ChargenBlurb = ChargenBlurb;
			dbitem.PosterId = PosterId;
			dbitem.AvailabilityProgId = AvailabilityProg?.Id;
			dbitem.MaximumNumberAlive = MaximumNumberAlive;
			dbitem.MaximumNumberTotal = MaximumNumberTotal;

			FMDB.Context.ChargenAdvicesChargenRoles.RemoveRange(dbitem.ChargenAdvicesChargenRoles);
			foreach (var item in _chargenAdvices)
			{
				dbitem.ChargenAdvicesChargenRoles.Add(new ChargenAdvicesChargenRoles
					{ ChargenAdviceId = item.Id, ChargenRole = dbitem });
			}

			FMDB.Context.ChargenRolesApprovers.RemoveRange(dbitem.ChargenRolesApprovers);
			foreach (var account in RequiredApprovers)
			{
				dbitem.ChargenRolesApprovers.Add(
					new ChargenRolesApprovers
					{
						Approver = FMDB.Context.Accounts.FirstOrDefault(x => x.Name == account),
						ChargenRole = dbitem
					});
			}

			FMDB.Context.ChargenRolesCosts.RemoveRange(dbitem.ChargenRolesCosts);
			foreach (var item in Costs)
			{
				var newItem = new ChargenRolesCost();
				FMDB.Context.ChargenRolesCosts.Add(newItem);
				newItem.ChargenRole = dbitem;
				newItem.ChargenResourceId = item.Key.Id;
				newItem.Amount = item.Value;
			}

			FMDB.Context.ChargenRolesCurrencies.RemoveRange(dbitem.ChargenRolesCurrencies);
			foreach (var item in StartingCurrency)
			{
				var newItem = new ChargenRolesCurrency();
				FMDB.Context.ChargenRolesCurrencies.Add(newItem);
				newItem.ChargenRole = dbitem;
				newItem.CurrencyId = item.Key.Id;
				newItem.Amount = item.Value;
			}

			FMDB.Context.ChargenRolesTraits.RemoveRange(dbitem.ChargenRolesTraits);
			foreach (var item in TraitAdjustments)
			{
				var newItem = new ChargenRolesTrait();
				FMDB.Context.ChargenRolesTraits.Add(newItem);
				newItem.ChargenRole = dbitem;
				newItem.TraitId = item.Key.Id;
				newItem.Amount = item.Value.amount;
				newItem.GiveIfDoesntHave = item.Value.giveIfMissing;
			}

			FMDB.Context.ChargenRolesClanMemberships.RemoveRange(dbitem.ChargenRolesClanMemberships);
			foreach (var item in ClanMemberships)
			{
				var newItem = new ChargenRolesClanMemberships();
				FMDB.Context.ChargenRolesClanMemberships.Add(newItem);
				newItem.ChargenRole = dbitem;
				newItem.ClanId = item.Clan.Id;
				newItem.RankId = item.Rank.Id;
				newItem.PaygradeId = item.Paygrade?.Id;
				foreach (var appointment in item.Appointments)
				{
					var newAppointment = new ChargenRolesClanMembershipsAppointments();
					FMDB.Context.ChargenRolesClanMembershipsAppointments.Add(newAppointment);
					newAppointment.ChargenRolesClanMembership = newItem;
					newAppointment.AppointmentId = appointment.Id;
				}
			}

			FMDB.Context.SaveChanges();
		}

		Changed = false;
	}

	public override string FrameworkItemType => "ChargenRole";

	private void LoadFromDatabase(MudSharp.Models.ChargenRole role)
	{
		_id = role.Id;
		_name = role.Name;
		RoleType = (ChargenRoleType)role.Type;
		ChargenBlurb = role.ChargenBlurb;
		PosterId = role.PosterId;
		RequiredApprovers = role.ChargenRolesApprovers.Select(x => x.Approver.Name).ToList();
		MaximumNumberAlive = role.MaximumNumberAlive;
		MaximumNumberTotal = role.MaximumNumberTotal;
		AvailabilityProg = Gameworld.FutureProgs.Get(role.AvailabilityProgId ?? 0);
		_costs.AddRange(role.ChargenRolesCosts.Select(x =>
			new ChargenResourceCost(
				Gameworld.ChargenResources.Get(x.ChargenResourceId) ?? throw new InvalidOperationException(),
				x.RequirementOnly)));
		TraitAdjustments = role.ChargenRolesTraits.ToDictionary(x => Gameworld.Traits.Get(x.TraitId),
			x => (x.Amount, x.GiveIfDoesntHave));
		StartingCurrency = role.ChargenRolesCurrencies.ToDictionary(x => Gameworld.Currencies.Get(x.CurrencyId),
			x => x.Amount);
		_additionalMerits = role.ChargenRolesMerits.Select(x => Gameworld.Merits.Get(x.MeritId)).ToList();
		ClanMemberships =
			role.ChargenRolesClanMemberships.Select(x => (IRoleClanMembership)new RoleClanMembership(x, Gameworld))
			    .ToList();
		Expired = role.Expired;
		foreach (var item in role.ChargenAdvicesChargenRoles)
		{
			_chargenAdvices.Add(Gameworld.ChargenAdvices.Get(item.ChargenAdviceId));
		}
	}

	#region IChargenRole Members

	public ChargenRoleType RoleType { get; set; }

	public string Poster
	{
		get
		{
			using (new FMDB())
			{
				var dbaccount = FMDB.Context.Accounts.Find(PosterId);
				return dbaccount?.Name ?? "System";
			}
		}
	}

	public long PosterId { get; set; }

	public List<string> RequiredApprovers { get; set; }

	public int MaximumNumberAlive { get; set; }

	public int MaximumNumberTotal { get; set; }

	public string ChargenBlurb { get; set; }

	public IFutureProg AvailabilityProg { get; set; }

	private List<ChargenResourceCost> _costs = new();

	public Dictionary<IChargenResource, int> Costs =>
		_costs.Where(x => !x.RequirementOnly).ToDictionary(x => x.Resource, x => x.Amount);

	public Dictionary<IChargenResource, int> Requirements =>
		_costs.Where(x => x.RequirementOnly).ToDictionary(x => x.Resource, x => x.Amount);

	public int ResourceCost(IChargenResource resource)
	{
		return Costs.ValueOrDefault(resource, 0);
	}

	public bool ChargenAvailable(ICharacterTemplate template)
	{
		return _costs.Where(x => x.RequirementOnly)
		             .All(x => template.Account.AccountResources.ValueOrDefault(x.Resource, 0) >= x.Amount) &&
		       ((bool?)AvailabilityProg?.Execute(template) ?? true);
	}

	public Dictionary<ITraitDefinition, (double amount, bool giveIfMissing)> TraitAdjustments { get; set; }

	public Dictionary<ICurrency, decimal> StartingCurrency { get; set; }

	private List<IMerit> _additionalMerits;
	public IEnumerable<IMerit> AdditionalMerits => _additionalMerits;

	public List<IRoleClanMembership> ClanMemberships { get; set; }

	public bool Expired { get; set; }

	public void SetName(string name)
	{
		_name = name;
	}

	public PermissionLevel MinimumPermissionToApprove { get; set; }

	public PermissionLevel MinimumPermissionToView { get; set; }

	private readonly List<IChargenAdvice> _chargenAdvices = new();

	public IEnumerable<IChargenAdvice> ChargenAdvices => _chargenAdvices;

	public bool ToggleAdvice(IChargenAdvice advice)
	{
		Changed = true;
		if (_chargenAdvices.Contains(advice))
		{
			_chargenAdvices.Remove(advice);
			return false;
		}

		_chargenAdvices.Add(advice);
		return true;
	}

	public string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine(string.Format(actor, "Role #{0:N0}".Colour(Telnet.Cyan), Id));
		sb.AppendLine();
		sb.AppendLine(string.Format(actor, "Name: {0}", Name));
		sb.Append(new[]
		{
			$"Type: {RoleType.ToString().Colour(Telnet.Green)}",
			$"Current: {(Expired ? "no".Colour(Telnet.Red) : "yes".Colour(Telnet.Green))}"
		}.ArrangeStringsOntoLines(2, (uint)actor.Account.LineFormatLength));
		sb.Append(new[]
		{
			$"Creator: {Poster.Proper().Colour(Telnet.Green)}",
			$"Approvers: {RequiredApprovers.Select(x => x.Colour(Telnet.Green)).ListToString()}"
		}.ArrangeStringsOntoLines(2, (uint)actor.Account.LineFormatLength));
		sb.Append(new[]
		{
			$"Approval Permission: {MinimumPermissionToApprove.Describe().Colour(Telnet.Green)}",
			$"View Permission: {MinimumPermissionToView.Describe().Colour(Telnet.Green)}"
		}.ArrangeStringsOntoLines(2, (uint)actor.Account.LineFormatLength));
		sb.Append(new[]
		{
			$"Max Alive: {(MaximumNumberAlive == 0 ? "unlimited" : MaximumNumberAlive.ToString("N0", actor)).Colour(Telnet.Green)}",
			$"Max Total: {(MaximumNumberTotal == 0 ? "unlimited" : MaximumNumberTotal.ToString("N0", actor)).Colour(Telnet.Green)}"
		}.ArrangeStringsOntoLines(2, (uint)actor.Account.LineFormatLength));
		sb.AppendLine(
			$"Availability Prog: {(AvailabilityProg != null ? string.Format("{0} (#{1:N0})".FluentTagMXP("send", $"href='show futureprog {AvailabilityProg.Id}'"), AvailabilityProg.FunctionName, AvailabilityProg.Id) : "None".Colour(Telnet.Red))}");
		sb.AppendLine();
		sb.AppendLine("Blurb:");
		sb.AppendLine(ChargenBlurb.SubstituteANSIColour().Wrap(80, "\t"));

		if (Costs.Any())
		{
			sb.AppendLine();
			sb.AppendLine(
				$"Costs: {Costs.Select(x => string.Format(actor, "{0:N0} {1}", x.Value, (x.Value == 1 ? x.Key.Name : x.Key.PluralName).TitleCase())).ListToString()}");
		}

		if (ClanMemberships.Any())
		{
			sb.AppendLine();
			sb.AppendLine("Clan Memberships:");
			sb.AppendLine();
			foreach (var clan in ClanMemberships)
			{
				sb.AppendLine(
					$"Clan: {clan.Clan.FullName.TitleCase().Colour(Telnet.Green)} Rank: {clan.Rank.Name.TitleCase().Colour(Telnet.Green)} Paygrade: {(clan.Paygrade != null ? clan.Paygrade.Name.TitleCase().Colour(Telnet.Green) : "None".Colour(Telnet.Red))} Appointments: {(clan.Appointments.Any() ? clan.Appointments.Select(x => x.Name.TitleCase().Colour(Telnet.Green)).ListToString() : "None".Colour(Telnet.Red))}");
			}
		}

		if (TraitAdjustments.Any())
		{
			sb.AppendLine();
			sb.AppendLine("Trait Adjustments:");
			sb.AppendLine();
			foreach (var adjust in TraitAdjustments)
			{
				sb.AppendLine(
					$"Trait: {adjust.Key.Name.TitleCase().Colour(Telnet.Green)} Adjustment: {adjust.Value.amount.ToString("N2", actor).Colour(Telnet.Green)}{(adjust.Value.giveIfMissing ? "" : " (boost only)")}");
			}
		}

		if (StartingCurrency.Any())
		{
			sb.AppendLine();
			sb.AppendLine("Starting Currency:");
			sb.AppendLine();
			foreach (var item in StartingCurrency)
			{
				sb.AppendLine(
					$"Currency: {item.Key.Name.TitleCase().Colour(Telnet.Green)} Amount: {item.Key.Describe(item.Value, CurrencyDescriptionPatternType.Short).Colour(Telnet.Green)}");
			}
		}

		return sb.ToString();
	}

	#endregion

	#region IFutureProgVariableType Implementation

	FutureProgVariableTypes IFutureProgVariable.Type => FutureProgVariableTypes.Role;

	public object GetObject => this;

	private static IReadOnlyDictionary<string, FutureProgVariableTypes> DotReferenceHandler()
	{
		return new Dictionary<string, FutureProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", FutureProgVariableTypes.Number },
			{ "name", FutureProgVariableTypes.Text },
			{ "roletype", FutureProgVariableTypes.Number }
		};
	}

	private new static IReadOnlyDictionary<string, string> DotReferenceHelp()
	{
		return new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", "" },
			{ "name", "" },
			{ "roletype", "" }
		};
	}

	public static void RegisterFutureProgCompiler()
	{
		FutureProgVariable.RegisterDotReferenceCompileInfo(FutureProgVariableTypes.Role, DotReferenceHandler(),
			DotReferenceHelp());
	}

	public IFutureProgVariable GetProperty(string property)
	{
		switch (property.ToLowerInvariant())
		{
			case "id":
				return new NumberVariable(Id);
			case "name":
				return new TextVariable(Name);
			case "roletype":
				return new NumberVariable((int)RoleType);
		}

		throw new NotImplementedException();
	}

	#endregion
}