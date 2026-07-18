# FutureMUD Website and Publishing

## Purpose

`FutureMUD.Web` is the public project home for the FutureMUD engine. It replaces the engine-specific About and Downloads material previously hosted by LabMUD, while LabMUD remains an independent game site. The website is a stateless ASP.NET Core Razor Pages application targeting .NET 10. It has no accounts, CMS, analytics database, or connection to a game database.

## Public content model

Authored pages live in `FutureMUD.Web/Content/Pages`. News lives in `FutureMUD.Web/Content/News`, while release and website patch notes live in `FutureMUD.Web/Content/PatchNotes`; both dated collections use `YYYY-MM-DD-slug.md` filenames. Front matter requires a title for every document and additionally requires a summary, matching ISO publication date, and comma-separated tags for dated content. `Content/PatchNotes/README.md` documents the authoring workflow and is excluded by the dated filename allowlist. The renderer implements the supported Markdown subset after HTML encoding. Raw HTML is never emitted; links are restricted to relative, HTTP, HTTPS, and `mailto` targets after repeated entity canonicalisation. Control-character, browser-normalised, data, script, and network-path targets are rejected.

Public routes cover the home page, About, Licensing, Getting Started, Downloads, news and RSS, patch-note index/detail pages, command documentation, FutureProg functions/types/collection extensions, item components, sitemap, and liveness/readiness probes. The four historical generated HTML filenames and the historical engine About and Downloads paths issue permanent redirects.

The global header remains visible while the page scrolls. Its Documentation disclosure is the canonical discovery point for commands, FutureProg functions, FutureProg types, collection extensions, item components, and patch notes; it uses native disclosure semantics and remains keyboard- and touch-operable without JavaScript. Documentation pages repeat these destinations in a compact section navigation. On narrow screens the sticky header wraps without hiding the brand or clipping the disclosure, and anchored content accounts for the header height.

The home, About, Downloads, Licensing, and footer surfaces identify the CC BY-NC-ND 3.0 license, link to Discord support and the public GitHub repository, and explicitly distinguish public source visibility from open-source or open-contribution permission. Getting Started records the .NET 10, MySQL 8.0, email, storage, and memory baseline.

The homepage gameplay carousel uses original screenshots from `FutureMUD.Web/wwwroot/images/gameplay`. It progressively enhances a horizontally scrollable, keyboard-focusable list with previous/next controls, never auto-advances, and respects reduced-motion preferences. Add another optimized image and corresponding `figure[data-carousel-slide]` with meaningful alt text, caption, and updated slide numbering when expanding the carousel.

## Documentation catalogue

The versioned contract is in `FutureMUDLibrary/Documentation`. The website links the database-free contract source file directly rather than referencing the full engine library, so its deployment does not carry Entity Framework, MySQL, cryptography, or design-time dependencies. Schema version 1 contains engine/source metadata and five code-backed families:

- command help, including command words, module, permission, audience, administrator text, and conditional variants;
- grouped FutureProg functions and overloads, including parameter names, parameter types, optional parameter help, return type, category, contexts, and general help;
- FutureProg variable dot-reference properties;
- collection extension functions;
- item-component type blurbs and builder help.

`MudSharp --export-documentation <output-path> --source-revision <sha>` runs before normal boot setup. It initializes static registries and the component registration manager, but does not read connection configuration, create boot markers, start TCP listeners, or connect to MySQL. Lists and overloads are sorted, slugs derive from names rather than list positions, and the output file is replaced atomically. A fixed timestamp and revision produce deterministic content. The debug legacy HTML writers consume this same catalogue.

Database-authored help and seeder-profile-specific content are intentionally excluded. The website reads `/var/lib/futuremud-web/documentation/live/catalogue.json`, validates the schema, and reloads it when the file changes. Sanitised, sorted section projections, slug lookup, and filter options are cached per catalogue instance with bounded size and expiry. Public query/filter values are length-bounded; result pages render at most 100 entries and 256 values per filter, even for the accepted 50,000-entry catalogue ceiling. Help text is treated as plain text: MXP tags are removed, known FutureMUD foreground ANSI SGR sequences and colour markers are converted to a fixed CSS allowlist, unknown ANSI instructions are removed, and all other characters are HTML encoded. The complete `#0`-`#O` engine marker palette and canonical Telnet foreground/reset sequences are mapped; FutureProg function, type, variable, keyword, and text colours use the engine's exact dark-mode RGB values.

Detail pages preserve catalogue structure rather than flattening every family into one preformatted block. Command default, administrator, and conditional help render as separate compact variants; FutureProg overloads render as syntax-coloured signatures with context chips and parameter tables; type properties render in a property/return-type/help table; collection extensions show their calling shape and return contract; and item-component blurbs are separated from builder help. Schema-1 catalogues published before per-parameter help was added remain readable: their combined overload help is shown and the parameter table identifies descriptions as part of that function text. No schema-version bump is required for this additive field.

## Product and release contract

`FutureMUD.Web/Configuration/release-products.json` is the central product manifest. It owns tag prefixes, project/version sources, runtime matrices, framework-dependent and single-file packaging, archive names, test projects, and the Engine documentation requirement. `singleFile` and `includeNativeLibrariesForSelfExtract` are explicit per product: normal stable releases bundle managed and native dependencies into the app host, embed symbols, and deliberately disable trimming because FutureMUD uses reflection-heavy registries. Product content such as Markdown, SQL snapshots, and application settings remains alongside the executable when required. Stable tags must carry an exact three-part project version and point to the exact merged commit containing that version:

- `engine-vX.Y.Z`
- `seeder-vX.Y.Z`
- `discordbot-vX.Y.Z`
- `terrainplanner-vX.Y.Z`
- `terrainapi-vX.Y.Z`

The release workflow publishes Windows x64 and the declared Linux x64/ARM64 variants. macOS is not advertised. Matrix jobs test and package independently. Secretless jobs exercise the complete upload/promote flow against a temporary website store; a fresh production-environment job then downloads the verified workflow artifacts and runs only the publisher script captured from protected `master`. Release-tag input, manifest values, project paths, runtime matrices, and commit ancestry are independently allowlisted before outputs are produced. Releases are tag-driven only; the historical backfill workflow is a fixed tag-to-commit allowlist and is not a nightly or general-purpose publishing path.

The complete human and Codex procedure is documented in [FutureMUD Release Process](./FutureMUD_Release_Process.md).

## Publishing protocol and storage

Publishing endpoints are under `/api/publishing/v1`. The create operation is idempotent for product, version, and source commit. An upload reports received chunks and accepts 32 MiB chunks with exact `Content-Range` and SHA-256 `Digest` headers. Duplicate identical chunks are harmless; conflicting chunks, ranges, sizes, file names, products, runtimes, and hashes are rejected. Completion assembles to a private candidate and verifies both byte count and full SHA-256 before validation.

The bearer token itself is never stored. Production configuration supplies its hexadecimal SHA-256, startup validation rejects malformed hashes, request authentication hashes the presented value, and comparison uses `CryptographicOperations.FixedTimeEquals`. Routing and the client-address-partitioned application limiter run before publishing authentication, bounding wrong-bearer traffic without allowing one address to exhaust another address's quota. Authentication then runs before JSON model binding or request-body parsing and ignores unmatched publishing-prefix paths. Create metadata is capped at 64 KiB while artifact chunks retain the 32 MiB protocol limit. Publishing also has a separate Nginx rate limit, fixed filename/identifier allowlists, a 4 GiB per-binary-artifact ceiling, a 32 MiB documentation ceiling with nested-entry/string/total-text bounds, and mount-aware free-space checks before draft creation, chunk writes, assembly, and documentation activation. Security allowlists and `Content-Range` parsing use absolute whole-string anchors, so an otherwise valid identifier, version, digest, commit, filename, or range cannot carry a trailing line terminator. Log fields derived from requests or persisted manifests encode every Unicode line ending as visible `\n` text before reaching a provider.

Storage is rooted at `/var/lib/futuremud-web`:

```
staging/{uploadId}
releases/live/{product}
releases/previous/{product}
documentation/live
```

Promotion renames directories on the same filesystem. A process-wide mutation lock serialises creation, promotion, and cleanup across product-shared paths; ref-counted upload locks serialise draft mutations and retire after the last holder; a storage-allocation lock serialises every capacity check with its corresponding chunk, assembly, or documentation write. Versions must increase monotonically and a published version cannot be replaced, preserving the cache-integrity promise of immutable versioned URLs. The old live product becomes the private rollback slot, while only the new release manifest is addressable by download routes. Documentation is prepared before public mutation, request cancellation is no longer used once commit begins, and a write-through phase journal makes artifact, documentation, and draft-status recovery deterministic: startup rolls a prepared transaction back and completes a committed transaction. Stale partial assemblies are removed before retry capacity checks. This coordination assumes the deployed single website writer; multiple processes sharing a data root would require an operating-system, filesystem, or database lock. Drafts and rollback slots expire after 24 hours; rollback age begins when the old live directory is moved, and only the latest public version is served.

Versioned downloads enable byte ranges, attachment disposition, strong SHA-derived ETags, immutable one-year caching, publication time, and checksum response headers. A request filename is rejected if it contains a parent reference or either path separator; the selected product, version, and artifact must exactly match the validated persisted manifest; and the filesystem path is then reconstructed from that stored identity, normalized, and checked to remain below the product's live directory. `.sha256` endpoints provide verification text. `/downloads/{product}/latest/{runtime}` is a no-store redirect to the versioned archive.

The public Downloads page presents each product with one aligned release header and a responsive artifact grid. Runtime, archive size, download action, and checksum action stay grouped per artifact; desktop layouts use the available width while mobile layouts collapse without depending on anonymous nested-grid placement.

## Security model and trust boundaries

The threat model assumes an attacker knows the complete repository and deployment design. Security must therefore come from authenticated capabilities, strict validation, immutable revisions, least privilege, and host/edge controls rather than obscurity. The public application has no account, session, database, arbitrary upload, outbound-request, or raw-HTML surface. `AllowedHosts`, canonical redirects, no permissive CORS policy, attachment downloads, response hardening headers, and a restrictive content-security policy constrain the public HTTP surface.

Publishing is the principal application capability. A bearer holder can create, upload, validate, promote, and abandon releases for every product, so the production token must be generated from at least 256 bits of cryptographically secure randomness, stored only as a GitHub production-environment secret, and rotated after any suspected runner, maintainer, or host compromise. Authorization failures and promotions should be monitored. A future trust-boundary upgrade should replace or supplement the shared bearer with GitHub OIDC claim validation and signed release manifests/attestations; separate upload and promote capabilities are also desirable.

GitHub Actions treats repository/tag code as untrusted before production authorization. Action references are full commit SHAs; checkout credentials are not persisted; GitHub context values enter scripts only through environment variables; tag, commit ancestry, product, path, runtime, and manifest values are hard-allowlisted. Compilation, tests, packaging, and end-to-end smoke publishing run without production credentials. Fresh production jobs consume short-lived workflow artifacts and execute deployment/publishing logic captured from protected `master`. Product-level concurrency prevents routine and backfill publications racing each other. Repository rulesets, CODEOWNERS, release-tag restrictions, production-environment reviewers, secret scanning, and push protection remain required GitHub-side controls.

The website project must remain independent of the engine/database dependency graph. A regression test and CI dependency-boundary scan enforce the linked documentation-contract seam. Dependabot covers Actions and both website NuGet projects; the website security workflow performs low-severity NuGet audit enforcement, dependency review, Release tests, and CodeQL on pull requests, protected-branch changes, a weekly schedule, and manual dispatch.

Secret values, data-protection keys, production appsettings, environment files, private keys/certificates, and local infrastructure state are ignored beneath `FutureMUD.Web`. Production values belong in `/etc/futuremud-web/environment`, GitHub environment secrets, Cloudflare scoped tokens, or AWS IAM/SSM facilities. Neither repository configuration nor logs may contain a bearer token, private key, AWS credential, Cloudflare token, origin address intended to remain private, or authorization header.

## Hosting and operations

Kestrel listens only on `127.0.0.1:5070` under a hardened systemd unit. The service has an empty capability set, read-only system/home views, private devices/tmp, namespace/kernel/proc restrictions, and write access only to `/var/lib/futuremud-web`. The broad forwarded-header environment switch is deliberately absent: application code trusts only loopback Nginx.

Nginx terminates origin TLS 1.2/1.3, rejects unknown hosts, restores `CF-Connecting-IP` only from the versioned official Cloudflare CIDRs, overwrites the forwarded address, compresses public text, applies short public timeouts, and grants the publishing path its separate 34 MiB/long-upload policy. The default body limit is 1 MiB. Nginx hides upstream HSTS/cache fields and emits one consistent one-year `includeSubDomains` HSTS policy without preload. Cloudflare must proxy both hostnames using Full (strict), enforce minimum edge TLS 1.2, authenticate/restrict origin access, and bypass cache for publishing and health routes.

Website deployment is independent of product publishing. Changes to the web project, Markdown, or shared documentation contract run the website tests and publish `linux-x64` without secrets. A fresh production-environment job downloads the one-day workflow artifact, transfers it through a restricted SSH account, and invokes the root-owned helper with one exact commit SHA. The helper atomically claims the upload out of the deployment user's directory, seals and hashes a root-owned copy, bounds archive type/path/count/compressed/expanded sizes and free space, extracts to a new read-only release, atomically switches `/opt/futuremud-web/current`, restarts, health-checks, rolls back on failure or signal, and prunes old releases without deleting active/rollback targets. Secrets remain in GitHub environments and `/etc/futuremud-web/environment`; logs go to journald and never include authorization headers. The stronger planned architecture is GitHub OIDC plus S3/SSM so no public SSH or long-lived deploy key is required.


## In-engine updater

From Engine 2.0.0, `DEBUG UPDATE` downloads the no-store latest redirect for the host runtime (`/downloads/engine/latest/win-x64`, `/downloads/engine/latest/linux-x64`, or `/downloads/engine/latest/linux-arm64`) and fails closed on unsupported platforms. The command verifies the response SHA-256, rejects archive entries that escape the `Binaries` directory, and stages the release for the generated restart script to apply. It uses `AppContext.BaseDirectory`, which remains valid for single-file applications. Linux restart scripts copy staged files and restore the executable bit before launch.
