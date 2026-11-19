# Armageddon S4 qualification and reusable fixtures

Luke approved this strategy on 13 October 2026. It supersedes earlier instructions to run a separate complete native spell × grade × target matrix for every entry. Required identities, approved content and behaviour, ownership/custody fences and disabled availability/Release gates are unchanged. The coordinator continues implementation and integration in coherent dependency batches.

## Evidence layers

| Layer | Required evidence | Reuse boundary |
| --- | --- | --- |
| Definition | Cheap automated checks for every required entry: exact inventory, approved costs/grade policies, references, typed dependencies and ownership | Each installed definition must pass; a positive ID or factory name is insufficient |
| Shared contract | Executed tests of each distinct payment/refusal, targeting, resistance, lifetime and persistence implementation | Configurations may inherit these results only when the ledger names the implementation/version, receipt and applicable assumptions |
| Direct critical case | Focused executed checks of new runtime behaviour, bespoke adapters, boundary grades and grades where behaviour changes | Deliberate coverage of the changed behaviour; random samples do not replace these cases |
| Representative/sample | End-to-end gameplay examples and reproducible seeded-random samples across meaningful configurations | Record seed, selected configuration, covered dimensions and result; expand around failures and retain regression cases |

Code inspection identifies mechanisms, dependencies and test assumptions. It is not formal proof of behaviour. Tests establish their executed assertions and practical limits. An entry may satisfy S4 through its definition checks plus a documented combination of direct, inherited and sampled evidence. No automatic promotion occurs solely because the strategy changed. S5 progression/production/affordability and later readiness gates remain separate.

The entry ledger distinguishes direct, inherited and sampled evidence, missing assumptions and outstanding critical cases. Historical receipts retain their original scope; older exhaustive-native wording is historical and cannot reintroduce the superseded Cartesian-product requirement. Shared-contract evidence is invalidated by changes to the tested implementation or by a configuration that violates its assumptions. Repair and focused verification follow observed failures; successful broad suites are not repeated after every small edit.

## Database workflow

Use one verified immutable clean **pre-Armageddon-seeder base-world** snapshot for the applicable fixture. First inspect existing repository skills, backups and harness descriptors. A post-S3 catalogue dump is an installed fixture and is not the clean base. A blank schema snapshot is not a seeded base-world snapshot.

The canonical descriptor records the dump SHA-256, source schema/migration version, relevant base-seeder source/input fingerprint, exact seed answers and restoration procedure. Restore only into a fresh isolated disposable database with the normal ownership marker, process/job and cleanup fences. Never restore over a user gameplay database or mutate the canonical dump. Relevant schema/base-input changes invalidate the base; S4-only content changes do not require repeating unrelated base seeding.

Build a new clean base only when no compatible verified base exists, when relevant base/schema inputs change, or when specifically exercising fresh-install/seeder behaviour. Derive installed Armageddon fixtures from a clean restore. Reuse an installed fixture for independent checks when state isolation/reset is reliable; use deliberate fresh restores for rollback, resume or other tests requiring clean state. Keep source/dump/schema validation intact and record whether a run restores the base, restores an installed fixture, or validates a fresh install.

The latest continuation receipt records the selected descriptor and compatible hashes after discovery. Local private dumps are optional machine-specific evidence, not committed files or portable prerequisites. Scenarios, descriptors and recreation/restoration procedures must remain reproducible on another host.

## Current integration priorities

Complete required content and its defining adapters before consolidated acceptance. Reuse the existing canonical contribution reconciler and real S3 dependencies. The shelter adapter now consumes deterministic native autobuilder presentation, selected-grade occupancy and protected private topology creation; direct regressions cover its newly demonstrated presentation/callback/schema risks. Remaining creature/controller, utility, projection, pocket, bane and other content follows in dependency batches. Install the deferred foci/devices/recipes using real qualified allowlists. Draft PR #788 remains open; Release, admissions, Class availability and unqualified producer gates stay disabled, with no merge or deployment.
