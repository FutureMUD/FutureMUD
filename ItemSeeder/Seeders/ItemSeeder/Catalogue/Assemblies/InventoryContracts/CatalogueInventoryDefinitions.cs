#nullable enable

using System;
using System.Collections.Generic;

namespace DatabaseSeeder;

public sealed record ItemSeederManifestEntry(
	string EntityType,
	string StableKey,
	string Module,
	IReadOnlyCollection<string> EraAdmissions,
	IReadOnlyCollection<string> Dependencies,
	ItemSeederOwnershipPolicy OwnershipPolicy,
	string Fingerprint);

public sealed record ItemSeederManifestDocument(
	string ManifestVersion,
	string SourceFingerprint,
	DateTime GeneratedAtUtc,
	IReadOnlyCollection<ItemSeederManifestEntry> Entries);

