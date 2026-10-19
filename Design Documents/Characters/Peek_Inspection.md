# Peek covert inspection

`peek <character> [container]` is a read-only covert action in `StealthModule`. It requires the distinct character-owned Peek skill, including against unconscious, asleep or otherwise helpless targets. Ordinary Look and inventory manipulation permissions retain their existing behavior.

## Detection and disclosure

One Normal `PeekCheck` resolves the attempt and supplies at most one Peek learning opportunity. Conscious observers who can see the actor on the same layer oppose it with their ordinary `SpotStealthCheck`. Only actual noticers receive alerts; those alerts describe the attempted inspection and never name inventory items or the selected container. Peek does not call the crime/reporting path.

A major failure, abject failure or severe active interruption by an able, non-helpless target stops disclosure. Other failed attempts can still reveal belongings, independently of notice. A noticed attempt can succeed. A sleeping/unconscious target does not observe or interrupt, but conscious witnesses can still notice.

A broad attempt considers distinct worn, held and wielded items, including covered worn equipment. Visible, normally exposed equipment is always included after the eligibility gates; it does not disappear because a reveal roll failed. Non-unary body inventories suppress ordinary display of held items even though the external-item helper includes them; Peek requires a carried-item reveal check for those items and cannot target concealed containers there. Other eligible equipment uses an independent `PeekRevealCheck`. Broad Peek does not enumerate containers, internal implants or other inventory recursively.

A selected container must be exposed on the target, visible, open and accessible. Only its immediate eligible contents are considered, each with an independent reveal check. A transparent closed container is still closed. Failure to select a container uses the same message for missing, closed, concealed and inaccessible containers.

Lighting, ordinary item invisibility/concealment, location/layer and inventory-plane gates apply before disclosure. The target must also be colocated within ordinary inventory reach, including route-room distance rules. No ignore-hiding, ignore-darkness or ignore-perception flag is used. `IgnoreSpotting` prevents incidental sight checks from revealing hidden items or granting extra perception training during enumeration. Observer eligibility, container state, equipment exposure and disclosure eligibility are revalidated after checks, so check/learning callbacks cannot expose stale belongings or alert witnesses who have moved away. Observers also require sufficient light to see the actor.

## Configuration and installation

| Setting | Initial value | Meaning |
| --- | --- | --- |
| `PeekCoveredItemDifficulty` | `6` (Hard) | Revelation of covered worn equipment |
| `PeekCarriedItemDifficulty` | `5` (Normal) | Revelation of concealed held/wielded equipment in non-unary body inventories |
| `PeekContainerItemDifficulty` | `5` (Normal) | Revelation of immediate contents of the explicitly selected open container |

These are initial configurable content recommendations, not recovered historical numbers or separately approved difficulty decisions. Invalid values fall back to those defaults. Each valid attempt resolves immediately and installs a five-second, Peek-only command delay before running checks. Failed attempts also consume that delay; invalid syntax, targets or containers do not. The delay does not block other commands, movement or combat, and expiry performs no inspection. Existing general-action, movement and combat admission restrictions still apply when starting Peek. There is no public start emote or pending target action. Callback-driven changes during the immediate inspection still pass the disclosure revalidation gates.

The ordinary Skill Package seeds a distinct Peek skill in both simple and complex packages, using the selected native improver/cap scaffolding. `PeekCheck` is ID **236**; `PeekRevealCheck` is ID **237**. Both checks bind numerically to Peek, cannot branch it on use and fail if it is missing. The attempt uses an improving native Standard check. Revelation uses a non-improving, non-branching Standard check with an explicit `{noimprove,nobranch}` parameter. The runtime fails closed if that safety policy is changed.

Existing worlds receive disabled, zero-formula checks on startup, with a safe non-improving reveal template. No player gains the skill implicitly, and no existing player values are deleted or reset. `PeekSeederHelper.BindExistingSkill(context, traitId)` binds an explicitly selected character skill in an additive update. The helper preflights both bindings before mutation, replaces only unbound zero placeholders, creates new expression rows rather than editing shared ones, and preserves compatible existing bindings, IDs, formula additions, difficulty modifiers and settings on rerun. It refuses conflicting custom bindings for explicit builder review.

`ArmageddonNonMagicSkillManifest` records the early stable keys `arm.skill.peek`, `arm.check.peek.attempt` and `arm.check.peek.reveal`. `BindPeek` accepts the explicit trait ID resolved by the eventual full non-magic installer. Recovered source membership is Burglar/Pick-pocket. This action slice does not invent numeric role openings, caps, prerequisites or grants, or replace the existing skill package in an established world; those remain owned by the full Armageddon install/migration contract.

## Verification scope

The target must be an active room member and outside stasis. Immediate admission rejects detached targets before skill checks or inventory reads. The command retains no target for later resolution or expiry callbacks.

Managed regressions cover independent detection/disclosure, partial results, exposed and covered items, botches/interruption, alert recipients, helpless targets, closed/hidden/inaccessible containers, immediate contents, layers/planes/darkness, callback invalidation, no transfers, native single-opportunity learning and immediate resolution and the five-second Peek-only cooldown. Seeder tests cover stable reruns, preserved player values, safe placeholders, conflicting customization and source memberships. Native database/Telnet qualification requires a scheduled isolated world; managed checks alone do not establish that result.
