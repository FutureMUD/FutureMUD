#nullable enable

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private const string S4BaseCompatibility = "inert-damage-hooks-and-s4-room-reference-classifications-20261013";
	private const string DamageInterfacePath = "FutureMUDLibrary/Health/IDamage.cs";
	private const string BaseDamageInterfaceHash = "407A6C10C4EC149CE8E3DC57EC47EB58DAB4D6A327E2B505E54A596C120A4004";
	private const string ReviewedDamageInterfaceHash = "D05A7CD804266CFCBFF5C4CDA568207FBB4AF76E62F22D6B1574113170050EA6";
	private const string RoomReferenceTypesPath = "MudsharpDatabaseLibrary/Database/RoomReferenceEffectTypes.cs";
	private const string BaseRoomReferenceTypesHash = "FD29553872E4369CDB807EEB62C6A23F52D85728241FD7E76D27FBF949E16E53";
	private const string ReviewedRoomReferenceTypesHash = "434DCE3A30E9D7D3173E2A66AAD41CC2380BC1B2814E12428F7222FFD0C7B160";
	// Captured full-source evidence proves these base compiler inputs at the original capture.
	// They are now explicit inputs for all new descriptors. The sealed original stays unchanged.
	private static readonly BaseSource[] BaseCompilerPins =
	[
		new("MudSharpCore/Celestial/Authored/CompiledAuthoredCelestial.cs", "6B50D2054922A21B3037F88513465C8A2BBA94EDB0DEDFB8D78044252821AC45"),
		new("MudSharpCore/Framework/OfflineProgCompilation.cs", "7408E0390824FF31CF165452F1F577736593753DDFE83CD5E4C2EA3AFE2546F7")
	];
	private static readonly BaseSource[] ReviewedS4SourceAdditions =
	[
		new("FutureMUDLibrary/Magic/IMagicVitalTransferDamage.cs", "ED326A0A1B9B5DCCA00E0F095D570CF2C443313BF6A2F2A2BD12048B6A494D7E"),
		new("FutureMUDLibrary/Health/IDamageMutationGuard.cs", "6C937050BDFF2490FC7B8B63AF166D094472CC5E2B261865ABEA072E9ACC3F9D"),
		new("FutureMUDLibrary/Effects/Interfaces/IExitBarrierTraversalEffect.cs", "E373823D741CC0D7868483AAF3CAA40D536CA717A968FC1103CFC9E6BE9B8011"),
		new("FutureMUDLibrary/Effects/Interfaces/IMagicInterdictionUseEffect.cs", "DCEFF3284A853030A6D48568790512C688455AAC4520A89DE3C4A50DEC3D2F9A")
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
			!projected.TryGetValue(DamageInterfacePath, out var newDamage) || newDamage.Sha256 != ReviewedDamageInterfaceHash) return false;
		// The exact reviewed changes add default-null invocation-local receipt/mutation hooks and inert interfaces.
		// The pre-Armageddon prefix performs no live damage, traversal or spell interdiction; schema, answers and restored content remain sealed.
		// No health/schema subtree is excluded; every other input still compares byte-for-byte.
		projected[DamageInterfacePath] = oldDamage;
		if (!recorded.TryGetValue(RoomReferenceTypesPath, out var oldRoomTypes) || oldRoomTypes.Sha256 != BaseRoomReferenceTypesHash ||
			!projected.TryGetValue(RoomReferenceTypesPath, out var newRoomTypes) || newRoomTypes.Sha256 != ReviewedRoomReferenceTypesHash) return false;
		// The two reviewed additions classify S4 serializers absent from the sealed base dump.
		projected[RoomReferenceTypesPath] = oldRoomTypes;
		var compatible = BaseValueHash(projected.Values.OrderBy(x => x.Path, StringComparer.Ordinal).ToArray()) == receipt.BaseSourceSha256;
		if (compatible) rule = S4BaseCompatibility;
		return compatible;
	}
}
