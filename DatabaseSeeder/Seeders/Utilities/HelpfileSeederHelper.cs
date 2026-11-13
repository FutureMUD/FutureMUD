#nullable enable

using System;
using MudSharp.Database;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders.Utilities;

public static class HelpfileSeederHelper
{
	/// <summary>Finds or creates a named help topic. The caller decides which fields to update and when to save.</summary>
	public static Helpfile EnsureHelpfile(FuturemudDatabaseContext context, string name, Action<Helpfile> update)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(name);
		ArgumentNullException.ThrowIfNull(update);
		var helpfile = SeederRepeatabilityHelper.EnsureNamedEntity(context.Helpfiles, name, x => x.Name, () =>
		{
			var created = new Helpfile();
			context.Helpfiles.Add(created);
			return created;
		});
		helpfile.Name = name;
		update(helpfile);
		return helpfile;
	}
}
