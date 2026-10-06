# AttributeSeeder

## Start here to add content

Edit [AttributeSeeder.cs](../../../DatabaseSeeder/Seeders/AttributeSeeder/AttributeSeeder.cs): **SeedAttributes, the selected shape branch; AttributeDescriberDefinitions.cs owns descriptive ranges**. This is an authored C# seam unless the data-source section below identifies linked/embedded inputs or generated outputs. Do not edit emitted database rows or exported inventories as a substitute for the source definition.

## Purpose, operator choices and prerequisites

Sets up attributes for your game.
Baseline availability: **Enabled**. The entrypoint is [AttributeSeeder.cs](../../../DatabaseSeeder/Seeders/AttributeSeeder/AttributeSeeder.cs); `SeederQuestions`, `SeedData` and `ShouldSeedData` define operator filtering/validation, execution and legacy readiness. No manual Program registration is needed.

Operator question IDs: `choice`, `decorator`. Exact accepted values, conditional visibility and defaults are defined in the linked `SeederQuestions`; the trace below explains the relevant choice path.

Metadata prerequisite descriptions from [SeederMetadataRegistry](../../../DatabaseSeeder/SeederMetadataRegistry.cs):

- The Core seeder must have created at least one account.

These predicates supplement concrete `ShouldSeedData` and helper preflights; declared ordering types are listed in the [dependency map](Dependency_Map.md).

## Inputs, representative trace and practical authoring guide

**Selection and real entry.** `SeederQuestions` defines answer key `choice`; accepted shape aliases are `soi`, `dnd`, `labmud`, `split`, `3stats`, `rpi`, `simple`, `arm`. The separate `decorator` question accepts `rpi`, `labmud`, `modern`, `raw`. `SeedData` (`AttributeSeeder.cs:76`) first checks `HasAttributeFoundation` (any `TraitDefinitions.Type==1`); if present it calls `ReconcileExistingAttributes`, otherwise starts the install transaction and enters `SeedAttributes`. For `choice=soi`, the “Strength” trait block at about line 573 creates `TraitDefinition` with type 1, group `Physical`, derived type 0, the created `improver.Id`, visible, a chargen blurb, branch multiplier 1.0, alias `str`, teach/learn difficulty 11, and decorator selected by `questionAnswers["decorator"]` (named “Strength Attribute” for LabMUD; otherwise the first decorator). It is explicitly added to `context.TraitDefinitions`. The existing branch also creates Constitution, Agility, Dexterity, and further traits; the stamina derived trait and decorators have separate linked blocks. `SeedData` dispatch is not a table-driven independent trait catalogue: definitions are repeated in the per-shape switch.

**Add-one recipe.** Adding a new *base attribute to every package shape* requires adding the `TraitDefinition` initializer to each relevant `choice` case in `SeedAttributes` (start at the `switch (questionAnswers["choice"].ToLowerInvariant())` around line 560; locate each case’s Strength block), set unique stable `Name` and `Alias`, `Type=1`, correct `TraitGroup`/`DerivedType`, the local `improver.Id`, `Hidden`, player-facing `ChargenBlurb`, `BranchMultiplier`, `TeachDifficulty`, `LearnDifficulty`, and decorator ID lookup consistent with the selected package/decorator. If it is only intended for SOI, add it in that branch only and document that scope. If introducing a decorator label, add its creation in the relevant decorator-building branch (there are direct `TraitDecorators.Add` writes earlier) and, if the decorator is descriptive, add it to [AttributeDescriberDefinitions.cs](../../../DatabaseSeeder/Seeders/AttributeSeeder/AttributeDescriberDefinitions.cs); if derived, also define correct `DerivedType`/expression and dependencies. The seeder's setup includes a pre-save before trait definitions and an improver/config reconciliation path. Adding only one trait row can leave no matching describer or expression for an intended derived stat. No new question is needed for a trait that is static stock in an existing choice; no reflection registration is needed.

**Ownership/failure.** Initial installation begins and commits a transaction at lines 83-85, but that code has no `using`, catch, or explicit rollback; provider disposal/failure behavior must not be over-described as a guaranteed explicit rollback. Existing attribute foundation takes a distinct reconciliation transaction (`ReconcileExistingAttributes`, around line 119), with metadata marking `Idempotent / FullReconcile` and stating it retains the selected shape. The live first-install gate is `ShouldSeedData`: no accounts => prerequisites unmet; any type-1 trait => `MayAlreadyBeInstalled`; otherwise ready. The repeatability metadata includes only Core dependency and account prerequisite. `DatabaseSeeder Unit Tests/AttributeDescriberSeederTests.cs` is relevant to describer mappings, but this is not evidence by itself of package shape behaviors; tests not run.

The `choice` filter itself is conditioned on `!HasAttributeFoundation(context)`, as is `decorator`, so these answers are not re-asked for an existing type-1 trait foundation. They are therefore installation-shape choices, not update selectors. Decorator `labmud` is semantically special in the initializer: it attempts an attribute-specific decorator by exact name, while other choices intentionally use `context.TraitDecorators.First()`. Before adding a trait, inspect all branch variants and the selected decorator stock set; otherwise behavior varies between packages even where the stat label appears identical. At a minimum a test for the chosen branch should check persisted name, alias, type, decorator, improver, plus any derived-expression references. Main initial path also creates/saves decorators and improver before trait rows; EF identity values are expected from those earlier saves. The emitted trait's numeric and textual values both affect runtime/chargen, so the `ChargenBlurb` and teach/learn difficulties should be treated as required authored fields rather than copied defaults without intent.

**Worked new installer option: `example-soi`.** To add an attribute *package option* rather than one stat, start with the SOI package. Add a clearly described `example-soi` line to the `choice` prompt and validator switch, then give it an explicit `SeedAttributes` switch route (initially a shared case with `soi` if its shape is deliberately identical). If it has a distinct shape, separate that branch and provide all trait/decorator/derived-stamina references using the existing branch pattern. Audit every `choice` comparison, including decorator and stamina logic; extending only the final trait switch can leave mismatched support definitions. Check `SeederQuestionRegistry` answer handling and related replay fixtures before exposing the choice. `choice` is hidden after a type-1 foundation exists, and `ReconcileExistingAttributes` only ensures the Non-Improving improver and MaximumStamina prog/configuration: it does not install newly authored attributes or migrate an existing world's shape. Verify the new choice on a fresh fixture and test preservation on an existing foundation. This is an illustrative future recipe, not a new option shipped by this task.

## Declared rerun and ownership contract

Metadata declares `Idempotent / FullReconcile`.

Reruns retain the selected attribute shape and reconcile its stock traits, decorators, improver and expressions.

Those are metadata declarations. The source trace above identifies actual lookup/field/transaction behavior; matching names alone do not prove field ownership or preservation. [The shared executor](README.md#shared-execution-contract) handles exceptions and answer persistence but supplies no cross-seeder transaction.

## Verification and related references

This guide was checked by static source reading; its addition recipe was not executed. No build, database or runtime result is claimed. For a future content change, add focused source/loader/serialization or selected-path first-install/rerun regression checks as appropriate, following the [verification map](../../../.codex/references/verification-and-docs.md). A source invariant is not proof of installed gameplay behavior.

See [repeatability strategy](../DatabaseSeeder_Repeatability_Strategy.md), [coverage and reviewed evidence](Coverage_and_Evidence.md), and the relevant linked test/source references above.
