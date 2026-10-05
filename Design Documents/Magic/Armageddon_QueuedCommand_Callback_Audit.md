# Queued command internal callback audit

Baseline: `1ad24d636925da64e7a82ffa858b0af470e56b0a`. Parent review rejected that checkpoint (P2): outer CombatBase gates did not revalidate after internal ResolveMove defender responses. Its original receipt and closing proofs are preserved as historical evidence, not comprehensive clearance.

## Bounded correction

- RetrieveItemMove, StandMove, ChangePositionMove and RepositionMove snapshot response targets and recheck bound authority and exact physical combat participation after each internal response and before inventory/posture mutation. Retrieval also checks after item-effect removal. Sticky rejection returns Irrelevant without action stamina.
- Actual move/CombatAction regressions cover expiry, revocation, combat departure and combat replacement for all four paths, plus valid direct controls (20 cases).
- MagicDefenseMove.Select snapshots candidate effects before executable eligibility Progs and excludes effects removed by those callbacks. A regression covers removal with mundane fallback. Actual native departure exposed and reproduces the former enumeration exception.
- A controlled native fixture uses paid Raise Servitor stock, real orders/ChooseMove/RetrieveItemMove/CombatAction/Body.Get, and actual Character.ResponseToMove -> MagicDefensePower.CanDefend eligibility callbacks. Outer response stays valid; internal expiry/departure refuses retrieval. Fixture enables the archive race's defence, removes fixture power costs/vision requirements, and uses deterministic combat recovery. Eligibility is controlled; successful magical attack defence and full production boot/Telnet are not claimed.

## Remaining OPEN callbacks

| Surface | Boundary before mutation | Remaining work |
| --- | --- | --- |
| MeleeWeaponAttack, NaturalAttackMove, MagicPowerAttackMove | Second ResponseToMove after beaten ward (`:154`, `:141`, `:73` at baseline) | Revalidate before subsequent damage; preserve ward cleanup on refusal. |
| StrangleAttack | Internal response before grapple extension (`:53`, mutation `:292/:300`) | Bind/revalidate the continuation before grapple mutation. |
| ChargeToMeleeMove | Owned behemoth child response/resolution (`:369-371`) | Propagate parent authority to the owned child and revalidate its continuation. |
| ManualCombatCommandResolver / SelectedCombatAction | Authored IsUsableBy (`:46`) before CombatTarget assignment (`:59`); selected factory only gates before construction | Revalidate factory policy callbacks before target mutation. |
| ChargeToMeleeMove | Authored usability checks (`:70/:328`) before MeleeRange updates (`:97-100`) | Revalidate before actor/target combat mutation. |
| CombatMessageManager -> CombatMessage.Applies | Message Progs (`:132/:167`) before auxiliary effects, breakout changes and firing | Audit/guard callers including AuxiliaryMove, BreakoutMove, RangedWeaponAttackBase and melee/natural damage paths. |
| MagicDefenseMove.Revalidate/TryDefend -> MagicDefensePower.CanDefend | Eligibility Progs (`:109-110`) invoked inside attack resolution | Revalidate attacker authority before later position/damage mutation, including failed-defence continuations. Candidate snapshotting alone does not resolve this. |

Source references above identify the audited baseline; later edits may shift line numbers. These are implementation blockers to comprehensive queued callback clearance, not new product approvals.

## Boundaries and unqualified paths

MoveToMeleeMove countershots (`:69/:85`) belong to independent defenders. They must not automatically inherit the attacking commander's authority. FireAndAdvanceToMeleeMove (`:64-65`) was an autonomous-strategy path outside the selected/manual factory scope.

No explicit internal defender/authoring callback was observed before the straightforward Draw/Wield/Wear/Remove/Load/Ready native operations. This does not establish coverage inside their underlying body/component calls. Arbitrary callbacks, concurrency, autonomous balance and full installed-world acceptance remain unqualified.

The whole Armageddon plan remains open: seven phases, 16 approved decisions, 154 candidates, 82 Sorcerer plus 12 support source rows and 25 native acceptance scenarios. N13 PC charm remains not_run; all eight feature packages, devices, installer and final native/release gates remain required. This correction stops at a local commit for parent independent review; no publication, merge, deployment or shared database access is authorized.
