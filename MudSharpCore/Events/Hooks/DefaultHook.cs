using System;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.FutureProg;
using System.Linq;

namespace MudSharp.Events.Hooks;

public class DefaultHook : IDefaultHook
{
	public DefaultHook(MudSharp.Models.DefaultHook hook, IFuturemud gameworld)
	{
		PerceivableType = hook.PerceivableType;
		EligibilityProg = gameworld.FutureProgs.Get(hook.FutureProgId);
		Hook = gameworld.Hooks.Get(hook.HookId);
	}

	public DefaultHook(string perceivableType, IFutureProg eligibilityProg, IHook hook)
	{
		PerceivableType = perceivableType;
		EligibilityProg = eligibilityProg;
		Hook = hook;
		using (new FMDB())
		{
			FMDB.Context.DefaultHooks.Add(new Models.DefaultHook
			{
				FutureProgId = eligibilityProg.Id,
				HookId = hook.Id,
				PerceivableType = perceivableType
			});
			FMDB.Context.SaveChanges();
		}
	}

	public string PerceivableType { get; protected set; }

	public IFutureProg EligibilityProg { get; protected set; }

	#region IDefaultHook Members

	public bool Applies(IFutureProgVariable item, string type)
	{
		// Note: The main reason for splitting off this parameter is for creating a character from a Character Template, where the character itself is not loaded at that point and so you want to be able to check against the Character Template in your progs, but hook the Character that is generated.
		return item != null && type.Equals(PerceivableType, StringComparison.InvariantCultureIgnoreCase) &&
		       ((bool?)EligibilityProg.Execute(item) ?? false);
	}

	public IHook Hook { get; protected set; }

	public void Delete()
	{
		using (new FMDB())
		{
			Futuremud.Games.First().SaveManager.Flush();
			var dbitem = FMDB.Context.DefaultHooks.Find(Hook.Id, PerceivableType, EligibilityProg.Id);
			if (dbitem != null)
			{
				FMDB.Context.DefaultHooks.Remove(dbitem);
			}

			switch (PerceivableType)
			{
				case "GameItem":
					FMDB.Context.HooksPerceivables.RemoveRange(
						FMDB.Context.HooksPerceivables.Where(x => x.HookId == Hook.Id && x.GameItemId.HasValue));
					break;
				case "Character":
					FMDB.Context.HooksPerceivables.RemoveRange(
						FMDB.Context.HooksPerceivables.Where(x => x.HookId == Hook.Id && x.CharacterId.HasValue));
					break;
				case "Cell":
					FMDB.Context.HooksPerceivables.RemoveRange(
						FMDB.Context.HooksPerceivables.Where(x => x.HookId == Hook.Id && x.CellId.HasValue));
					break;
			}

			FMDB.Context.SaveChanges();
		}
	}

	#endregion
}