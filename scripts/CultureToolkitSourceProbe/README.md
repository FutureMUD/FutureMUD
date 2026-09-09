# Culture toolkit verification probe

This executable records C# source evaluation, content imports and integration fixtures. It does not certify historical evidence. Build with single-node MSBuild:

```powershell
dotnet build scripts/CultureToolkitSourceProbe/CultureToolkitSourceProbe.csproj -m:1 -p:UseSharedCompilation=false
```

Invoke the resulting DLL with one of the following modes. Paths are relative to the working directory. Output JSON always states whether its IDs are isolated fixture IDs or actual MySQL IDs.

| Arguments | Scope |
|---|---|
| `--archived-names <corpus> <report>` | No MySQL. Import and rerun all retained historical naming from the immutable archive; compare profile payloads and reviewed presentation changes. |
| `--archived-languages <corpus> <report>` | No MySQL. Five isolated language/script/intelligibility imports and reruns. |
| `<report>` | Read-only installed prerequisites; evaluate the four retained source modules in isolated InMemory contexts. |
| `--install-fixtures <report>` | Five eras in both Culture/Chargen orders; full-content entrypoint followed by rerun. |
| `--optional-fixtures <report>` | Five eras times seven partial names/languages/heritage combinations. |
| `--round2-optional <report>` | Five eras with names-only, languages-only and heritage-without-languages, each followed by a rerun. |
| `--round2-live-graphs <successful-live-receipt> <report>` | Read-only actual MySQL script membership and generated acquisition trait predicates; verifies their equality after a clean committed rerun. |
| `--round2-builder-rerun <successful-live-receipt> <report>` | Two full reruns with builder script, name and accent edits in the named round-two disposable MySQL database; rollback afterward. Also verifies caller-transaction rollback after an intentional prog compilation failure. |
| `--upgrade-fixture <report>` | Retained combined Dark Ages/Medieval source, then Medieval toolkit upgrade and rerun; report preserved baseline ambiguities. |
| `--live-import <database> <era> <report>` | Create a verified-absent local disposable database, apply migrations, execute stock prerequisite and Culture seeders, then commit a stable Culture rerun. |
| `--live-resume <database> <era> <report>` | Resume only a failed Culture import whose exact receipt proves the stock prerequisites completed. No reset. |
| `--live-rerun <database> <era> <report>` | Verify another stable Culture rerun only when the exact successful disposable-import receipt already exists. |
| `--live-receipt <database> <report>` | Read-only live prog contracts, managed identities and the named native-pipeline NPC fixture's persisted traits/accents. |

Modes using installed prerequisites require `FUTUREMUD_SOURCE_PROBE_CONNECTION` in the process environment. Never put credentials into tracked command files, command-line arguments or receipts. Prerequisite reads require the installed stock Human Admin ethnicity, attributes, Literacy and supporting definitions. All ordinary fixture writes stay in explicitly isolated InMemory stores.

Live modes are deliberately restricted to local development databases prefixed `futuremud_culture_live_2145_`. Fresh imports refuse an existing target, and the database name must fit the migration lock's length limit. They exercise the typed Debug Medieval profile's selected CoreData, Time, Attribute, SkillPackage, Human, Chargen and Culture steps through the shared executor; they are not a claim of executing every seeder in a complete Debug replay profile.

The telnet checks were executed separately with the `futuremud-mud-tester` skill against the disposable imported world. Their transcripts and the distinctions between input validation, C#, MySQL and telnet execution are recorded in [the implementation record](../../Design%20Documents/Verification/CultureSeeder_Redesign_Implementation.md).
