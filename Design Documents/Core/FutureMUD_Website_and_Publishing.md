# FutureMUD Website and Publishing

## Purpose

`FutureMUD.Web` is the public project home for the FutureMUD engine. It replaces the engine-specific About and Downloads material previously hosted by LabMUD, while LabMUD remains an independent game site. The website is a stateless ASP.NET Core Razor Pages application targeting .NET 10. It has no accounts, CMS, analytics database, or connection to a game database.

## Public content model

Authored pages live in `FutureMUD.Web/Content/Pages`. News lives in `FutureMUD.Web/Content/News` and uses `YYYY-MM-DD-slug.md` filenames. Front matter requires a title for every document and additionally requires summary, ISO publication date, and comma-separated tags for news. The renderer implements the supported Markdown subset after HTML encoding; raw HTML and non-HTTP link schemes are never emitted.

Public routes cover the home page, About, Getting Started, Downloads, news and RSS, command documentation, FutureProg functions/types/collection extensions, item components, sitemap, and liveness/readiness probes. The four historical generated HTML filenames and the historical engine About and Downloads paths issue permanent redirects.

## Documentation catalogue

The versioned contract is in `FutureMUDLibrary/Documentation`. Schema version 1 contains engine/source metadata and five code-backed families:

- command help, including command words, module, permission, audience, administrator text, and conditional variants;
- grouped FutureProg functions and overloads, including parameters, return type, category, contexts, and help;
- FutureProg variable dot-reference properties;
- collection extension functions;
- item-component type blurbs and builder help.

`MudSharp --export-documentation <output-path> --source-revision <sha>` runs before normal boot setup. It initializes static registries and the component registration manager, but does not read connection configuration, create boot markers, start TCP listeners, or connect to MySQL. Lists and overloads are sorted, slugs derive from names rather than list positions, and the output file is replaced atomically. A fixed timestamp and revision produce deterministic content. The debug legacy HTML writers consume this same catalogue.

Database-authored help and seeder-profile-specific content are intentionally excluded. The website reads `/var/lib/futuremud-web/documentation/live/catalogue.json`, validates the schema, and reloads it when the file changes. Help text is treated as plain text: MXP tags and ANSI escape instructions are removed, FutureMUD colour markers are converted to a fixed CSS allowlist, and all other characters are HTML encoded.

## Product and release contract

`FutureMUD.Web/Configuration/release-products.json` is the central product manifest. It owns tag prefixes, project/version sources, runtime matrices, framework-dependent packaging, archive names, test projects, and the Engine documentation requirement. Stable tags must carry an exact three-part project version:

- `engine-vX.Y.Z`
- `seeder-vX.Y.Z`
- `discordbot-vX.Y.Z`
- `terrainplanner-vX.Y.Z`
- `terrainapi-vX.Y.Z`

The release workflow publishes Windows x64 and the declared Linux x64/ARM64 variants. macOS is not advertised. Matrix jobs test and package independently; the final job downloads all archives, exercises the complete upload/promote flow against a temporary website store, then uses the production bearer token.

## Publishing protocol and storage

Publishing endpoints are under `/api/publishing/v1`. The create operation is idempotent for product, version, and source commit. An upload reports received chunks and accepts 32 MiB chunks with exact `Content-Range` and SHA-256 `Digest` headers. Duplicate identical chunks are harmless; conflicting chunks, ranges, sizes, file names, products, runtimes, and hashes are rejected. Completion assembles to a private candidate and verifies both byte count and full SHA-256 before validation.

The bearer token itself is never stored. Production configuration supplies its hexadecimal SHA-256, request authentication hashes the presented value, and comparison uses `CryptographicOperations.FixedTimeEquals`. Publishing also has an application rate limiter, Nginx rate limit, fixed filename/identifier allowlists, a 4 GiB per-artifact ceiling, and a preflight free-space reserve.

Storage is rooted at `/var/lib/futuremud-web`:

```
staging/{uploadId}
releases/live/{product}
releases/previous/{product}
documentation/live
```

Promotion renames directories on the same filesystem. The old live product becomes the private rollback slot, while only the new release manifest is addressable by download routes. A failed rename restores the old live directory. Startup repairs an interrupted `.moving` directory. Drafts and rollback slots expire after 24 hours; only the latest public version is served.

Versioned downloads enable byte ranges, attachment disposition, strong SHA-derived ETags, immutable one-year caching, publication time, and checksum response headers. `.sha256` endpoints provide verification text. `/downloads/{product}/latest/{runtime}` is a no-store redirect to the versioned archive.

## Hosting and operations

Kestrel listens only on `127.0.0.1:5070` under a hardened systemd unit. Nginx terminates strict TLS, supplies forwarded headers, compression, canonical host redirects, a 34 MiB request limit, and disabled request buffering for publishing. Cloudflare proxies both hostnames using strict origin TLS and does not cache publishing or health routes.

Website deployment is independent of product publishing. Changes to the web project, Markdown, or shared documentation contract run the website tests, publish `linux-x64`, transfer through a restricted SSH account, atomically switch `/opt/futuremud-web/current`, restart, health-check, and roll back the symlink on failure. Secrets remain in GitHub environments and `/etc/futuremud-web/environment`; logs go to journald and never include authorization headers.
