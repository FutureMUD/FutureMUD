#nullable enable

using System;
using MudSharp.FutureProg;
using MudSharp.Framework.Save;

namespace MudSharp.Arenas;

public sealed class ArenaCombatantClass : SaveableItem, ICombatantClass
{
	public ArenaCombatantClass(MudSharp.Models.ArenaCombatantClass model, CombatArena arena)
	{
		Gameworld = arena.Gameworld;
		Arena = arena;
		_id = model.Id;
		_name = model.Name;
		EligibilityProg = Gameworld.FutureProgs.Get(model.EligibilityProgId);
		AdminNpcLoaderProg = model.AdminNpcLoaderProgId.HasValue
			? Gameworld.FutureProgs.Get(model.AdminNpcLoaderProgId.Value)
			: null;
		ResurrectNpcOnDeath = model.ResurrectNpcOnDeath;
		DefaultStageNameTemplate = string.IsNullOrWhiteSpace(model.DefaultStageNameTemplate)
			? null
			: model.DefaultStageNameTemplate;
		DefaultSignatureColour = string.IsNullOrWhiteSpace(model.DefaultSignatureColour)
			? null
			: model.DefaultSignatureColour;
	}

	public CombatArena Arena { get; }
	public IFutureProg EligibilityProg { get; private set; }
	public IFutureProg? AdminNpcLoaderProg { get; private set; }
	public bool ResurrectNpcOnDeath { get; }
	public string? DefaultStageNameTemplate { get; }
	public string? DefaultSignatureColour { get; }

	public override string FrameworkItemType => "ArenaCombatantClass";

	public override void Save()
	{
		Changed = false;
	}

}
