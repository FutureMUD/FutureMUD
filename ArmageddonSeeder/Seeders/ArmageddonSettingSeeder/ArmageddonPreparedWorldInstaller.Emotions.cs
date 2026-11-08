#nullable enable
using System;
using System.Collections.Generic;
using MudSharp.Database;

namespace DatabaseSeeder.Seeders;

public static partial class ArmageddonPreparedWorldInstaller
{
	public const string EmotionalReadiness = "Optional Emotions requires unambiguous or explicitly selected Fury attribute, attribute units, native terrains, seven Calm saves and authored boolean(target, caster) eligibility progs. Null preserves owned definitions. Native per-spell/restart acceptance remains unrun; the full package stays disabled.";
	private static void ValidateEmotions(FuturemudDatabaseContext db, ArmageddonPreparedWorldBindings bindings, List<string> errors)
	{
		try { _ = ArmageddonEmotionalInstaller.PreservedSpells(db, bindings.Emotions is null ? null : bindings.Utilities.School); }
		catch (Exception error) when (error is InvalidOperationException or System.Xml.XmlException or ArgumentException)
		{ errors.Add(error.Message); }
		if (bindings.Emotions is not null) errors.AddRange(ArmageddonEmotionalInstaller.ValidateMappings(db, bindings.Emotions));
	}
}
