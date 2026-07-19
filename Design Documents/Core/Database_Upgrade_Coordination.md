# Database Upgrade Coordination

## Scope

`MudsharpDatabaseLibrary` coordinates startup migrations, pre-upgrade backups, failed-upgrade recovery, rollback state, blank snapshot import/export, and backup retention. The coordinator is designed so a process failure leaves enough state for the next startup to make a safe decision.

## Settings

`DatabaseBackupSettings.json` controls the backup directory, retained SQL backup count, automatic pre-migration backups, and automatic rollback recovery. Missing settings create the defaults. Invalid retained-count or directory values are repaired in memory. Malformed JSON is replaced on disk with usable defaults so one damaged configuration file does not permanently prevent startup.

## Upgrade lifecycle

`PrepareForStartup` validates the connection string, working directory, and executable type before doing work. It then:

1. loads and repairs backup settings;
2. examines any existing `db-upgrade-state.json`;
3. restores an earlier backup only when automatic recovery is enabled, the recorded database matches, and the recorded backup exists;
4. queries pending migrations;
5. creates and prunes a backup when enabled; and
6. writes pending migrations and target state before returning.

`ApplyPreparedMigrations` persists `MigrationAttempted = true` before invoking the migration service. A thrown migration therefore leaves an actionable state file. Explicit rollback records the error before restoring; a successful restore archives the state as `rolled-back`, while a restore failure leaves the failure state for the next startup. Successful completion removes the active state file.

Disabled backups remain supported. Pending migration state is still written, but no backup path is claimed and automatic restore is not attempted without one.

## Snapshot and retention behavior

Blank database snapshot import and export use unique temporary directories. The directories are removed in `finally` blocks after both success and exceptions. Export replaces the concrete database name with the maintained placeholder; import performs the reverse substitution before restore.

Backup pruning considers SQL files only, orders them newest first, and deletes entries beyond `RetainedBackupCount`.

## Verification contract

The normal database-library suite covers request validation, missing and damaged settings, migration progress, attempted-state ordering, matching and mismatched recovery, missing backups, disabled backups, successful and failed rollback, completion, snapshot cleanup on exceptions, and backup pruning. EF mapping checks remain limited to named compatibility requirements rather than model-wide snapshots.
