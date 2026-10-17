# Armageddon S3 Native Special Mechanics

This document records the native runtime boundary for the bounded Ember Seed, Restorative Mud and Sown Sickness content. Catalogue rows and invocation templates are separate builder-authored content; prototypes never choose an invocation grade or invent a spell identifier.

## Ember Seed

Use the `ArmageddonEmberSeed` component type (`armageddonemberseed` builder alias) with prototype XML `<Definition damagePerGrade="3" />`. `damagePerGrade` must be finite, positive and at most 100. A single activation binds from the real `SpellLifecycleOrigin`: per-output lifecycle ID, native spell ID, grade, temporary-cleanup mode, creator identity and absolute deadline. The binding also records the exact creator instance. An ordinary item, copied item, stale binding, expired item, permanent item, or unavailable/mismatched creator refuses activation.

Applying the item activates it. A serializable transaction confirms the component's spent state against its exact active item lifecycle before damage. An uncertain commit produces no damage and holds further activation until reload. It snapshots the colocated living, non-stasis characters and their exact bodies, spatial locations and external parts, rechecking them after visibility/engagement callbacks. Burning damage is `damagePerGrade * bound grade` per eligible character. Parent deletion follows the blast. An interruption can consume the blast before damage resolves; it cannot replay damage after reload.

## Restorative Mud

Use the existing native `TopicalCream` component and its touched-vector delivery path, with a builder profile such as `<NativeHealingDose key="arm.item.restorative_mud" grams="1" budgetPerGrade="10" />`. `grams` is a physical dose in grams. The component total weight must represent exactly that physical mass after conversion through `BaseWeightToKilograms`; the builder profile command sets the quantity accordingly. The S4 item binding stores the real lifecycle/spell identity, grade, creator identity and instance, reference dose, per-grade budget and spent state. Unbound or copied instances cannot provide a dose.

One exact full dose must be applied once to an uncovered bodypart with an existing wound. The same native component-consumption transaction confirms the spent state before carrier delivery or wound changes. After carrier callbacks the original actor/body, location, part accessibility and wound membership are rechecked. Total direct `CurrentDamage` reduction across wounds on that bodypart is capped at `budgetPerGrade * bound grade`; the cream's existing drug and prog features are disallowed for this special profile. Spent cold loading sets remaining mass directly without deleting the parent during construction. Prototype and binding XML numbers use invariant XML parsing. This is direct wound-damage reduction; S5 carrier/pharmacology qualification remains a separate gate.

## Sown Sickness

Sown Sickness is native infection kind 5, created through `Infection.LoadNewInfection` and the existing `SpellDiseaseEffect` attribution flow. Its grade is clamped to 1 through 7 from native intensity, with no spontaneous transmission. Its attached `ThermalImbalance` symptom supplies Very Mild Hyperthermia at grades 1–3, Mild at 4–6 and Moderate at 7. Native thermal penalties compose with an additional 10% stamina expenditure and 5% regeneration reduction per grade. Ambient exposure and fire/drug heat continue to update their separate ambient effect. Infection removal removes only its exact symptom, preserving ordinary exposure effects. Antibiotics use the native infection treatment path; this adds no independent disease-profile persistence layer.
