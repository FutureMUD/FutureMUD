# Culture seeder catalogue and evidence assets

The completed implementation/task handoff has been consolidated into the [CultureSeeder master guide](../Seeders/CultureSeeder.md). This directory retains source catalogues, research and historical acceptance references.

Do not delete the original redesign's `data/` or `research/` payloads: the shared seeder project embeds them at their existing paths. Catalogue identity, naming evidence and ownership/rerun rules belong in the master guide. Historical receipts are indexed under [Verification](../../Verification/README.md).

`python tools/validate_handoff.py` checks the retained authored data and the historical manifest hashes for `data/` and `research/`. It reports which original authoring-document/tool hashes were not applied. The immutable `MANIFEST.sha256` still describes the old complete handoff; `--verify-original-package` checks that original bundle, including its now-retired plans, and is intended for an original extracted archive rather than this consolidated directory. Neither mode proves engine or database acceptance.
