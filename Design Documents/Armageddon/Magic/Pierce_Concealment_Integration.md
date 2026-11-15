# Pierce Concealment integration and source contract

Pierce Concealment is authored and reviewable, but is not cleared for native completion. Two existing shared contracts fail the dedicated acceptance gate: character-owned magical blindness is ignored by `Body.CanSee`, and exclusive parent replacement leaves the old detection child attached. This lane changes neither shared casting/payment nor body/control perception. The entry-specific receipt records the failures separately from passing independent checks.

## Recovered source and deliberate adaptations

The authoritative completion brief is Library file `libfile_a4606cd0097081918d6c9d9e220e6da0`, `Armageddon_Magic_Completion_Implementation_Brief.md`. The historical `codedump.c` is `libfile_befb4ae78d1c8191aaa2640c49912d9d`. Both were read through supported Library reads; no cloud filesystem path or denied transfer was assumed. Relevant source locations are `spell_detect_invisibility` at lines 84574–84608 and `cast_detect_invisibility` at 217927–217967. The selected Sorcerer source-tree roster supplies opening 30, cap 90, minimum energy 7 and acquisition parent Sense Enchantment at skill 80; the executable source comment mentions Whiran/Sorcerer membership but does not supply that roster policy.

Historical rules:

- The recipient gains invisible perception. A missing recipient returns without applying the effect. The caster's Silt sector refuses the effect. There is no consumed component in this source function.
- Duration is `5 * grade` game hours; repeated applications accumulate to 48 hours through `stack_spell_affect`. Eyes tingle on application.
- Ordinary spell delivery requires the selected character. Potion delivery targets the caster; scroll delivery falls back to the caster; wand delivery refuses objects or a missing character. Staff/room wrappers visit eligible non-immortals with their own ethereal comparison.

Native adaptations:

- The editable stock uses ordinary character/self delivery and the current `detectinvisible` effect. Area and device wrappers are separate delivery policies and are not added here.
- The established 600-seconds-per-game-hour conversion gives `3000*grade`: grade one 3,000 seconds and grade seven 21,000 seconds. Native exclusive replacement deliberately replaces historical accumulation and its 48-hour cap. Replacement currently fails child cleanup; this is a blocker, not a second accumulation policy.
- Silt refusal is an editable support Prog comparing the caster's current terrain name to `silt`, evaluated through existing fail-closed target admission before payment. Builders can change that terrain mapping. Native perception grants `VisualMagical`, with no new ethereal perception, topology or blindness cure.
- Existing relative grades, efficiency, practice, speech/hand requirements and paid casting APIs are reused. Opening 30/cap 90 is an explicit capability entry; acquisition, including source parent Sense Enchantment 80, remains separately authored. Low/high native costs are 7/50 under the existing grade efficiency formula.

The earlier catalogue proposal's `B4*grade` duration, generic focus acquisition and proposed component categories are not historical rules and are not carried into this stock. The source criminal check has no new implementation in this lane; ordinary engine casting and delivery policy remain authoritative. Historical sector IDs and portable wrappers are not silently invented.

## Existing-interface operation reporting

`DetectInvisibleEffect` now implements existing `IMagicSpellEffectOperation` in a separate partial. Legacy `GetOrApplyEffect` still constructs and returns an unattached native child. Configured operation calls use existing parent ownership and character attachment APIs, returning no child to the outer pipeline so it cannot attach twice.

`Applied` requires both actual receiver retention and parent ownership. A repeated call using an already-retained child on the same parent returns `NoChange`; proved non-retention with unchanged effect references returns `NoChange`; unrelated or uncertain mutation returns `Unknown`. Invalid receiver/parent ownership is rejected. An attachment exception preserves ownership only when the receiver retained the child, allowing the existing exception finaliser to bound its lifetime. No casting-service, payment, operation schema, global interface or detection perception implementation changes are included.

Tests exercise paid reporting, ordinary mastery only for `Applied`, no-op/unknown, attachment failure before/after retention, quarantine/no replay, factory compatibility and refresh. The refresh unit fixture now honours the native removal-action flag and records the existing orphan-child contract; it does not fake successful cleanup.

## Required shared integration before clearance

1. `MudSharpCore/Body/Implementations/BodyPerception.cs:188` tests body-local `AffectedBy<IBlindnessEffect>()`. The existing blindness adapter in `Magic/SpellEffects/BlindnessEffect.cs:78` attaches its native `SpellBlindnessEffect` to the character. Native room-item visibility remains allowed with that character-owned effect, both before and after invisibility detection. Body-owned blindness still blocks it. The control/perception owner must choose and repair combined effect applicability; `Body.CombinedEffectsOfType<T>` already combines body and actor effects. This lane does not change the contract.
2. `MudSharpCore/Magic/MagicSpell.cs:1826` removes an old exclusive `MagicSpellParent` without firing its removal action. Native `EffectHandler.RemoveEffect` consequently unschedules/removes the parent but retains its unscheduled detection child. The casting/lifecycle owner must review parent-child removal semantics and ensure exclusive replacement removes the old child. No blanket removal semantics or casting finaliser change is made here.

The dedicated native entrypoint retains both failing assertions as boolean acceptance gates, logs the exact failures, and exits 1 even when independent checks pass. After the exclusive failure is captured, `firstHigh.RemovalEffect()` is fixture cleanup to permit independent ward/mastery/restart checks; it is not production code or evidence of repaired refresh. Fresh-process expiry qualifies the persisted current parent/child, not live orphan cleanup. Preserve this distinction during parent review.

## Integration hooks and scope

The only existing stock-builder dispatch change is `TryCreatePerceptionStockSpell` plus its help-name addition in `EditableItemHelperMagic.Stock.cs`. The new factory reuses `ArmageddonUtilityStock.Create/Definition` without changing that installer-owned API. The new native project links prior gathering/five-stock/provision partials read-only, owns its entrypoint and runner, and can run their regression modes without editing shared dispatch.

No central progress ledger, repertoire table, installer, charged carrier, charm/NPC AI, combat-authority file, historical receipt, primary checkout, remote branch or deployment is changed. Each native invocation owns a unique MySQL instance, endpoint, datadir, process/output directory and disposable database, verified before connection and cleanup. Native tests use real character/body/item/resource/effect/SQL paths with controlled world catalogues, checks and clock; they do not establish full Telnet/login or setting-specific acquisition.
