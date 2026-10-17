#nullable enable

using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Models;
using MudSharp.RPG.Checks;

namespace MudSharp.Framework;

public partial class Futuremud
{
	internal static CheckTemplate CreatePeekCheckTemplate(FuturemudDatabaseContext db, CheckType type)
	{
		// A missing check in an established world is unbound (formula 0), with no implicit skill grant.
		// The reveal template must never inherit the generic fallback's improvement/branching behavior.
		var reveal = type == CheckType.PeekRevealCheck;
		var name = reveal ? "Peek Reveal Check" : "Peek Attempt Check";
		var templates = db.CheckTemplates.Include(x => x.CheckTemplateDifficulties).ToList();
		bool Safe(CheckTemplate candidate) => candidate.CheckMethod == "Standard" && candidate.ImproveTraits == !reveal &&
			!candidate.CanBranchIfTraitMissing && candidate.FailIfTraitMissingMode == 1 && Enum.GetValues<Difficulty>().All(x =>
				candidate.CheckTemplateDifficulties.Any(y => y.Difficulty == (int)x));
		var existing = templates.FirstOrDefault(x => x.Name == name && Safe(x));
		if (existing is not null) return existing;
		// Preserve a builder-owned same-name policy. Reuse a safe prior fallback, otherwise choose a free name.
		var fallback = templates.FirstOrDefault(x => x.Name.StartsWith(name + " Upgrade", StringComparison.Ordinal) && Safe(x));
		if (fallback is not null) return fallback;
		var selectedName = name;
		for (var suffix = 1; templates.Any(x => x.Name == selectedName); suffix++)
			selectedName = suffix == 1 ? name + " Upgrade" : $"{name} Upgrade {suffix}";

		int[] modifiers = [100, 75, 50, 33, 15, 0, -10, -20, -33, -50, -100];
		var template = new CheckTemplate
		{
			Name = selectedName, CheckMethod = "Standard", ImproveTraits = !reveal,
			CanBranchIfTraitMissing = false, FailIfTraitMissingMode = 1
		};
		foreach (var difficulty in Enum.GetValues<Difficulty>())
			template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{
				Difficulty = (int)difficulty, Modifier = modifiers[(int)difficulty]
			});
		db.CheckTemplates.Add(template);
		return template;
	}
}
