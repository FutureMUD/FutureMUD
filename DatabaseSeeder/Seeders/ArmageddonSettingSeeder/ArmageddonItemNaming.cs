#nullable enable
using System.Linq;
using MudSharp.Database;

namespace DatabaseSeeder.Seeders;

/// <summary>Checks item catalogue labels independently of shared primary nouns.</summary>
internal static class ArmageddonItemNaming
{
	internal static bool LabelCollision(FuturemudDatabaseContext db, long self, string name)
	{
		var normalised = name.ToLowerInvariant();
		return db.GameItemProtos.Any(x => x.Id != self &&
			((x.UniqueName != null && x.UniqueName.ToLower() == normalised) ||
			 ((x.UniqueName == null || x.UniqueName == "") && x.Name.ToLower() == normalised)));
	}
}
