# Gathering native-persistence acceptance harness

This narrow disposable-database harness covers the three native persistence acceptance checks for health-priced Self gathering. It is not part of the normal test suite or a general test platform.

From the repository root, build and run it with:

```powershell
dotnet build 'Temporary Scratch App\GatheringNativePersistenceHarness\GatheringNativePersistenceHarness.csproj' -c Debug --no-restore -m:1
& 'Temporary Scratch App\GatheringNativePersistenceHarness\Run-IsolatedAcceptance.ps1'
```

The runner starts a newly initialised MySQL instance on a loopback-only random port, creates one fresh `futuremud_gather_gc_<timestamp>_<random>` database after checking that its name is unused, imports the supported blank snapshot, and passes its isolated connection only through `FUTUREMUD_GATHERING_TEST_CONNECTION`. It never reads normal game configuration or chooses an existing database.

It executes the production `MagicGatheringService`, `SimpleLivingHealthStrategy`, native `SimpleOrganicWound` persistence and receipt store. GC-P01 uses no `OnGathered` callback and creates a wound; GC-P02 starts from a persisted cellular wound at 7 damage, 11 pain and 13 stun and requires the independent reader to observe 10, 15 and 18; GC-P03 launches a separate reader process that reconstructs the character, body and receipt service without a save-manager flush, logout, shutdown, healing tick or resource regeneration.

The output records the base revision and dirty/clean worktree state, MySQL version, generated database name, fixture and operation IDs, expected/observed channels, and cleanup result. Cleanup first verifies the temporary server's data directory and the database ownership marker; it removes only resources created by that run.
