#nullable enable

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private const string VitalReceiptBaseCompatibility = "inert-vital-receipt-and-recorded-native-base-compilers-20261013";
	private const string DamageInterfacePath = "FutureMUDLibrary/Health/IDamage.cs";
	private const string BaseDamageInterfaceHash = "407A6C10C4EC149CE8E3DC57EC47EB58DAB4D6A327E2B505E54A596C120A4004";
	private const string VitalDamageInterfaceHash = "DC4FDBB63B7569EA3D80243250A4905E31A8A8854ADB37C0A7B3DFB31F0DD5F4";
	// Captured full-source evidence proves these base compiler inputs at the original capture.
	// They are now explicit inputs for all new descriptors. The sealed original stays unchanged.
	private static readonly BaseSource[] BaseCompilerPins =
	[
		new("MudSharpCore/Celestial/Authored/CompiledAuthoredCelestial.cs", "6B50D2054922A21B3037F88513465C8A2BBA94EDB0DEDFB8D78044252821AC45"),
		new("MudSharpCore/Framework/OfflineProgCompilation.cs", "7408E0390824FF31CF165452F1F577736593753DDFE83CD5E4C2EA3AFE2546F7")
	];
	private static readonly BaseSource[] ReviewedS4SourceAdditions =
	[
		new("FutureMUDLibrary/Magic/IMagicVitalTransferDamage.cs", "ED326A0A1B9B5DCCA00E0F095D570CF2C443313BF6A2F2A2BD12048B6A494D7E")
	];

	private static bool CompatibleBaseSources(BaseWorldFixture receipt, IReadOnlyList<BaseSource> current, out string? rule)
	{
		rule = null;
		if (receipt.BaseSourceSha256 != BaseValueHash(receipt.BaseSources)) return false;
		if (receipt.BaseSourceSha256 == BaseValueHash(current)) return true;
		if (receipt.BaseSources.Select(x => x.Path).Distinct(StringComparer.Ordinal).Count() != receipt.BaseSources.Count ||
			current.Select(x => x.Path).Distinct(StringComparer.Ordinal).Count() != current.Count) return false;
		var projected = current.ToDictionary(x => x.Path, StringComparer.Ordinal);
		var recorded = receipt.BaseSources.ToDictionary(x => x.Path, StringComparer.Ordinal);
		foreach (var addition in BaseCompilerPins.Concat(ReviewedS4SourceAdditions))
		{
			if (recorded.ContainsKey(addition.Path)) continue;
			if (!projected.TryGetValue(addition.Path, out var present) || present.Sha256 != addition.Sha256) return false;
			projected.Remove(addition.Path);
		}
		if (!recorded.TryGetValue(DamageInterfacePath, out var oldDamage) || oldDamage.Sha256 != BaseDamageInterfaceHash ||
			!projected.TryGetValue(DamageInterfacePath, out var newDamage) || newDamage.Sha256 != VitalDamageInterfaceHash) return false;
		// The exact reviewed change adds only a default-null, invocation-local receipt property.
		// No health/schema subtree is excluded; every other input still compares byte-for-byte.
		projected[DamageInterfacePath] = oldDamage;
		var compatible = BaseValueHash(projected.Values.OrderBy(x => x.Path, StringComparer.Ordinal).ToArray()) == receipt.BaseSourceSha256;
		if (compatible) rule = VitalReceiptBaseCompatibility;
		return compatible;
	}
}
