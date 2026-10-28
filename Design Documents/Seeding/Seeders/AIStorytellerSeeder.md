# AIStorytellerSeeder

## Purpose and installation

The optional starter pack installs Example Narrative Steward, three sample FutureProgs, and twelve reference documents. The `install` yes/no question, account prerequisite and seeder order are unchanged. An explicit seeder rerun is the only stock modernization entry point; server startup does not rewrite saved settings.

New stock uses `gpt-6-luna`/`none` for attention, `gpt-6.1-terra`/`low` for events and `gpt-6.1-terra`/`medium` for time updates. Output budgets are 256/4,096/4,096. Execution options enable fixed five-second routine-event batching, a 120-second deadline and stable-prefix caching on compatible models. The sample retains its heartbeat subscriptions and existing operational pause default. These models require operator account access; the seeder does not contact OpenAI.

## Ownership and upgrades

`SeederManagedRecords` entries use owner `AIStorytellerSeeder`, module `starter-pack`, entity type and the original source name as stable key. Logical IDs resolve renamed managed records. Storytellers, progs and documents have separate field baselines, with prog parameter signatures reconciled as one field and reference restrictions preserved independently.

A same-name legacy row is adopted only when all mapped scalar fields and, for progs, its ordered parameter signature match the historical source definition. Line-ending differences alone are accepted. Legacy storyteller adoption also requires verified linked sample prog ownership. Matching names alone never establish ownership. Unknown/customized legacy rows remain unclaimed and unchanged, with a report explaining why. Duplicate identities, competing claims and missing managed rows fail the transaction rather than guessing replacements.

After adoption, field-level reconciliation updates fields still equal to the previous stock baseline and preserves builder edits, including names, prompts, models, subscriptions, pause state, tool XML, prog bodies/parameters and reference visibility. The desired stock baseline is saved separately from preserved overrides so later reruns cannot reinterpret those overrides as stock. Fresh installation writes current definitions directly. Historical source strings exist only to recognize legacy records; unsupported focus-tool guidance is removed from current stock output.

## Authoring and verification

Keep reference identity, inline content, readiness check and `EnsureReferenceDocument` call together in the owning seeder. For changed stock prose, retain its prior definition as an explicit historical adoption candidate where legacy migration is required. Do not overwrite ambiguous same-name content or adopt a live builder value as a new stock baseline.

Focused regressions live in `DatabaseSeeder Unit Tests/AIStorytellerSeederTests.cs`: fresh/repeated installation, legacy adoption, line endings, customized legacy preservation, managed renames/edits and linked-prog ownership. In-memory tests establish reconciliation behavior but do not establish native MySQL rollback or live gameplay. See [runtime contract and opt-in live evaluation](../../AI/AI_Storyteller_Design.md), [repeatability strategy](../DatabaseSeeder_Repeatability_Strategy.md), and [verification map](../../../.codex/references/verification-and-docs.md).
