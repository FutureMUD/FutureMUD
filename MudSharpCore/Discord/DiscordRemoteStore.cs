#nullable enable

using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Models;

namespace MudSharp.Discord;

internal interface IDiscordRemoteStore
{
	DiscordAccountLink? FindLink(string userId);
	void SaveLink(DiscordAccountLink link);
	void RemoveLink(string userId);
	IReadOnlyList<DiscordHook> Hooks();
	void SaveHook(DiscordHook hook);
	DiscordRemoteInvocation? FindInvocation(string id);
	void SaveInvocation(DiscordRemoteInvocation invocation);
	IReadOnlyList<DiscordRemoteInvocation> RecentInvocations();
	bool HookInvokedSince(long hookId, DateTime cutoff);
	string? GetSetting(string key);
	void SetSetting(string key, string value);
	void Recover();
}

internal sealed class DiscordRemoteStore : IDiscordRemoteStore
{
	public DiscordAccountLink? FindLink(string userId)
	{
		using (new FMDB())
		{
			return FMDB.Context.DiscordAccountLinks.AsNoTracking().FirstOrDefault(x => x.DiscordUserId == userId);
		}
	}

	public void SaveLink(DiscordAccountLink link)
	{
		using (new FMDB())
		{
			var current = FMDB.Context.DiscordAccountLinks.Find(link.DiscordUserId);
			if (current is null)
			{
				FMDB.Context.DiscordAccountLinks.Add(link);
			}
			else
			{
				FMDB.Context.Entry(current).CurrentValues.SetValues(link);
			}
			FMDB.Context.SaveChanges();
		}
	}

	public void RemoveLink(string userId)
	{
		using (new FMDB())
		{
			var link = FMDB.Context.DiscordAccountLinks.Find(userId);
			if (link is not null)
			{
				FMDB.Context.DiscordAccountLinks.Remove(link);
			}
			foreach (var pending in FMDB.Context.DiscordRemoteInvocations
				         .Where(x => x.DiscordUserId == userId && x.Status == "confirmation"))
			{
				pending.Status = "cancelled";
				pending.ConfirmationId = null;
				pending.ResultText = "Account link revoked.";
				pending.UpdatedUtc = DateTime.UtcNow;
			}
			FMDB.Context.SaveChanges();
		}
	}

	public IReadOnlyList<DiscordHook> Hooks()
	{
		using (new FMDB())
		{
			return FMDB.Context.DiscordHooks.AsNoTracking().OrderBy(x => x.Key).ToList();
		}
	}

	public void SaveHook(DiscordHook hook)
	{
		using (new FMDB())
		{
			if (hook.Id == 0)
			{
				FMDB.Context.DiscordHooks.Add(hook);
			}
			else
			{
				var current = FMDB.Context.DiscordHooks.Find(hook.Id) ??
				              throw new ArgumentException("That Discord hook no longer exists.");
				FMDB.Context.Entry(current).CurrentValues.SetValues(hook);
			}
			FMDB.Context.SaveChanges();
		}
	}

	public DiscordRemoteInvocation? FindInvocation(string id)
	{
		using (new FMDB())
		{
			return FMDB.Context.DiscordRemoteInvocations.AsNoTracking().FirstOrDefault(x => x.Id == id);
		}
	}

	public void SaveInvocation(DiscordRemoteInvocation invocation)
	{
		using (new FMDB())
		{
			var current = FMDB.Context.DiscordRemoteInvocations.Find(invocation.Id);
			if (current is null)
			{
				FMDB.Context.DiscordRemoteInvocations.Add(invocation);
			}
			else
			{
				FMDB.Context.Entry(current).CurrentValues.SetValues(invocation);
			}
			FMDB.Context.SaveChanges();
		}
	}

	public IReadOnlyList<DiscordRemoteInvocation> RecentInvocations()
	{
		using (new FMDB())
		{
			return FMDB.Context.DiscordRemoteInvocations.AsNoTracking()
				.OrderByDescending(x => x.CreatedUtc).Take(200).ToList();
		}
	}

	public bool HookInvokedSince(long hookId, DateTime cutoff)
	{
		using (new FMDB())
		{
			return FMDB.Context.DiscordRemoteInvocations.AsNoTracking().Any(x => x.HookId == hookId &&
				x.UpdatedUtc > cutoff && (x.Status == "invoking" || x.Status == "completed" ||
				                         x.Status == "failed" || x.Status == "indeterminate"));
		}
	}

	public string? GetSetting(string key)
	{
		using (new FMDB())
		{
			return FMDB.Context.DiscordIntegrationSettings.AsNoTracking()
				.Where(x => x.Key == key).Select(x => x.Value).FirstOrDefault();
		}
	}

	public void SetSetting(string key, string value)
	{
		using (new FMDB())
		{
			var setting = FMDB.Context.DiscordIntegrationSettings.Find(key);
			if (setting is null)
			{
				FMDB.Context.DiscordIntegrationSettings.Add(new DiscordIntegrationSetting { Key = key, Value = value });
			}
			else
			{
				setting.Value = value;
			}
			FMDB.Context.SaveChanges();
		}
	}

	public void Recover()
	{
		using (new FMDB())
		{
			foreach (var invocation in FMDB.Context.DiscordRemoteInvocations
				         .Where(x => x.Status == "invoking" || x.Status == "queued"))
			{
				invocation.Status = "indeterminate";
				invocation.ResultText = "Execution was interrupted. Inspect world state; this action will not be replayed.";
				invocation.UpdatedUtc = DateTime.UtcNow;
			}
			FMDB.Context.SaveChanges();
			var cutoff = DateTime.UtcNow.AddDays(-30);
			// Retain identity/fingerprint tombstones. Deleting the key would allow an old mutation to run again.
			FMDB.Context.DiscordRemoteInvocations.Where(x => x.CreatedUtc < cutoff)
				.ExecuteUpdate(setters => setters
					.SetProperty(x => x.Arguments, "")
					.SetProperty(x => x.ResultText, "Detailed output expired under the 30-day retention policy. The recorded execution will not be repeated.")
					.SetProperty(x => x.ConfirmationId, (string?)null)
					.SetProperty(x => x.Status, x => x.Status == "confirmation" ? "expired" : x.Status));
		}
	}
}
