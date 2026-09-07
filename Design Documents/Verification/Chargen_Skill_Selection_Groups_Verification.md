# Chargen skill-selection groups implementation report

Verified 7 September 2026 against checkout `879a84824673ebd718122d1e09339b36b31172a5`, rather than assuming the inherited `f06a9eff0e14b19f7ca41b0372738ec3a7078c8b` baseline. Changes are uncommitted.

## Delivered feature

Persistent, editable generic groups use real character skill traits, stable keys, revisions, ordered membership, pick bounds, compiled eligibility/member progs and disabled-by-default creation/cloning. Soft retirement preserves identity. Attributes and invalid members/progs fail closed.

`SkillGroupResolver` and `SkillGroupAllocation.IsFeasible` provide shared eligibility, explicit NewOnly/CountKnown ownership and unique allocation with global matching. Required shortages block; optional choices cannot strand another required group. `SkillGroupScreen.Wrap` integrates Mandatory Free -> Group Picks -> Open Skills into `SkillPickerScreen`, `SkillCostPickerScreen`, `SkillSkipperScreen` and `SkillBoostSkipperScreen`. Skippers still require group decisions. Suggestions run after groups.

`ChargenSkillClaims` persists mandatory, independent, recoverable ordinary and per-group claims; existing boost selections remain separate. Explicit ordinary-to-group allocation suppresses only the base purchase charge. Repricing derives from claims, so repeated display/reconnect cannot refund twice; removing group ownership restores the original ordinary claim. The actual cost-storyboard regression verifies 16 points -> 6 points -> 16 points, retaining the 6-point boost. Background/definition edits and application load/submission/approval revalidate claims.

`ChargenSkillSelectionGroupSeeder.Upsert` accepts a stable-key definition, already-resolved traits and compiled prog references. Three-way reconciliation preserves builder edits. Integration tests install equivalent fixtures both before and after ChargenSeeder.

`GeneratedSkillGroupAdapter.Apply` uses the same resolver with explicit optional policy and deterministic saved choices. Variable NPC templates opt in through `skillgroups <seed> <decline|fill>`; existing templates remain off and hand-authored values remain authoritative.

## Builder demonstration

See [the builder and integration guide](../Characters/Chargen_Skill_Selection_Groups.md) for full syntax. A non-language fixture creates Household Training with Hearthcraft, Mending and Yardwork, uses `chargenskillgroup set picks 2`, assigns a compiled Boolean(chargen) eligibility prog, validates, then enables it. The applicant explicitly picks exactly two distinct skills. These names are demonstration fixtures, not a new production content pack.

Builder management includes `list`, `show`, `create`, `clone`, `edit`, `set name`, the `set description` editor, `set member add/remove`, `set eligibility`, `set membereligibility`, `set picks`, `set existing`, `set order`, `set enabled`, `retire` and `validate`. Group-phase commands are `groups`, `group`, `pick`, `unpick`, `help`, `done`, `skip` and `back`.

## Changed files and persistence

| Area | Files |
|---|---|
| Library | `CharacterCreation/IChargenSkillSelectionGroup.cs`, `ChargenSkillClaims.cs`, `SkillGroupAllocation.cs`, `IChargen.cs`; `Framework/IFuturemud.cs` |
| Runtime | `CharacterCreation/ChargenSkillSelectionGroup.cs`, `SkillGroupResolver.cs`, `Chargen.SkillGroups.cs`, `Chargen.cs`; `Screens/SkillGroupScreen.cs` and all four existing skill screens |
| Builder/loading | `Commands/Modules/ChargenModule.SkillGroups.cs`, `ChargenModule.cs`; `Framework/FuturemudLoaders.cs`, `FuturemudVariables.cs` |
| Generated templates | `NPC/Templates/GeneratedSkillGroupAdapter.cs`, `VariableNPCTemplate.cs`, `SimpleCharacterTemplate.cs` |
| Database | `Models/ChargenSkillSelectionGroup.cs`; `Database/FuturemudDatabaseContext.SkillSelectionGroups.cs`, `FuturemudDatabaseContextConfiguring.cs`; generated migration `20260907085537_ChargenSkillSelectionGroups.cs`, matching designer and model snapshot |
| Seeder | `Seeders/Utilities/Chargen/ChargenSkillSelectionGroupSeeder.cs`, `BlankDatabaseSnapshot.sql`, `BlankDatabaseSnapshot.manifest.json` |
| Tests | `SkillGroupAllocationTests.cs` in library tests; `ChargenSkillSelectionGroupTests.cs` and `ChargenSkillSelectionGroupIntegrationTests.cs` in core tests; `ChargenSkillSelectionGroupSeederTests.cs` in seeder tests; `ChargenSkillSelectionGroupModelTests.cs` in database tests |
| Documentation | New feature guide and this report; character creation runtime, builder, seeder and documentation index; design-document root index |

The migration adds definitions and ordered membership with unique stable keys, composite membership keys and restrictive trait/prog relationships. It seeds no universally enabled demonstration groups. The blank snapshot includes the reviewed EF-generated idempotent delta from `20260905072550_PsychicWitnessMemory`, with table-name casing aligned to the existing snapshot.

## Executed verification

```powershell
dotnet restore MudSharp.sln -m:1 -p:RestoreBuildInParallel=false -p:NuGetAudit=false
scripts/test-unit.ps1
dotnet test "MudSharpCore Unit Tests/MudSharpCore Unit Tests.csproj" --no-restore -m:1 -p:UseSharedCompilation=false "-p:NoWarn=NU1902%3BNU1510" --filter FullyQualifiedName~ChargenSkillSelectionGroup
dotnet ef migrations has-pending-model-changes --project MudsharpDatabaseLibrary/MudsharpDatabaseLibrary.csproj --startup-project MudSharpCore/MudSharpCore.csproj --no-build
git diff --check
```

Restore succeeded. The final normal fast suite passed **4,497 tests**, zero failures/skips: library 472, expression 23, seeder 766, core 3,041, database 42, Discord 22, converter 43, web 54 and terrain 34. After the last builder validation output change, the focused core run rebuilt successfully and passed **36 tests**, zero failures/skips. Builds report existing nullable warnings. EF reports no pending model changes. Whitespace checks pass.

SG01-SG20-labelled tests cover non-language exact counts; eligibility; four-screen sequencing; real base/boost pricing; explicit known credits; overlap ownership and dead ends; optional shortages; builder persistence/create/clone/retirement; compiled, wrong-signature, failing and missing progs/traits; real chargen XML reconstruction; background/revision changes; delayed suggestions; three-way seeding and both storyboard installation orders; opt-in native variable-template generation; and existing Latin-1/full-name rendering. The shared allocator additionally compares all 512 three-by-three candidate pools against exhaustive assignment and tests multi-slot reassignment.

## Compatibility and execution limitations

No eligible groups with no existing group state use the original screen path. Existing generated templates remain opted out. Game-configured starting values, caps, boosts and trait resource rules remain authoritative. No CultureSeeder redesign, historical curriculum, production demo groups or encoder changes are included. No known implementation requirement was intentionally deferred.

Live MySQL migration application and clean snapshot import remain unverified. The full snapshot refresh attempt failed with a Windows TLS/SSPI credential error; the documented reviewed-delta fallback was used instead. Database integration tests use EF InMemory plus relational model assertions, which do not establish live MySQL execution. Reconnect coverage reconstructs real chargen XML in tests; a running telnet client/server session and interactive builder editor/confirmation flows were not exercised. These are execution limitations, not claimed live validation.
