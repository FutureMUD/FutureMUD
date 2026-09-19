# Gathering native-persistence acceptance harness

This narrow disposable-database harness covers native persistence acceptance for health-priced Self gathering and native organic field accounting. It is not part of the normal test suite or a general test platform.

From the repository root, build and run it with:

```powershell
dotnet build 'Temporary Scratch App\GatheringNativePersistenceHarness\GatheringNativePersistenceHarness.csproj' -c Debug --no-restore -m:1
& 'Temporary Scratch App\GatheringNativePersistenceHarness\Run-IsolatedAcceptance.ps1'
```

The runner starts a newly initialised MySQL instance on a loopback-only random port. It creates fresh, separately owned `futuremud_gather_gc_<timestamp>_<random>` and `futuremud_land_<timestamp>_<random>` databases after checking that each name is unused, imports the supported blank snapshot, and passes the isolated server connection only through `FUTUREMUD_GATHERING_TEST_CONNECTION`. The loopback-only disposable server uses no TLS because it has no trusted certificate. The harness never reads normal game configuration or chooses an existing database.

It executes the production `MagicGatheringService`, `SimpleLivingHealthStrategy`, native `SimpleOrganicWound` persistence and receipt store. GC-P01 uses no `OnGathered` callback and creates a wound; GC-P02 starts from a persisted cellular wound at 7 damage, 11 pain and 13 stun and requires the independent reader to observe 10, 15 and 18; GC-P03 launches a separate reader process that reconstructs the character, body and receipt service without a save-manager flush, logout, shutdown, healing tick or resource regeneration.

The land run first covers Y-T02 with production environmental-profile builder creation, organic authoring, `SaveManager` persistence, reload, clone, independent clone editing and a second reload. It then executes production `AgricultureField` accounting and lifecycle methods, `SaveManager`, EF/MySQL persistence, `AgricultureFieldInput` reservation, annual harvest, native herd grazing and a separate reader process. Y-P01 observes the conservative whole debit and prepaid fraction independently, then reconstructs the field and spends the remainder exactly once. Y-P02 replaces a crop, runs and reloads native ticks, and verifies saved lifecycle/progress. Y-P03 interleaves fractional plans with real craft, harvest and grazing consumers so stale plans cannot reuse physical stock. A provider trigger also forces one field checkpoint to fail and proves rollback plus `SaveManager` retry before the trigger is removed. The harness substitutes only unrelated world catalogues, deterministic weather and the ecological scalar factor; real coordinator damage/repair behavior is covered by the owning environmental integration tests (Y-P04).

The output records the base revision and dirty/clean worktree state, MySQL version, generated database name, fixture and operation IDs, expected/observed channels, and cleanup result. Cleanup first verifies the temporary server's data directory and the database ownership marker; it removes only resources created by that run.
