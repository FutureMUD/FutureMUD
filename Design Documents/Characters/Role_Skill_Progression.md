# Role skill progression

The reusable **Role Skill Progression** character merit composes a primary role's prerequisite trees with a secondary role's associations. Rules bind numeric role and skill identities; renaming either does not change membership. Builders can replace validated `<Rules>` XML with `merit set rules <Rules>...</Rules>`.

Applicability also governs initial grants. Its prog must accept a chargen/toon or no parameters; a constant false profile supplies no initial skills or floors. An incompatible character-only predicate blocks initial projection with a configuration error rather than silently granting disabled skills. Use role availability to impose character-only admission rules.

Each `Rule` specifies `role`, `trait`, `primary`, `opening`, `cap`, optional `parent`, `threshold`, and `bonus`. Primary prerequisites must belong to the same role; duplicate memberships, cycles and invalid numeric values are rejected. One primary and one secondary membership may claim each skill. Native cap expressions remain the authority for improvement caps; configure those consistently with the merit.

With both memberships, the primary opening and cap take priority. Add the secondary bonus once, clamped to that cap. A shared child keeps its primary prerequisite and is absent at creation unless an independent authorized grant already supplies it; an already-known child receives its composed floor once during creation. Without a primary membership, the secondary opening and cap apply immediately. This replaces additive role trait adjustments for governed skills; do not add a second positive adjustment for them.

The skill screens project roots and standalone grants into mandatory claims, including worlds without skill-selection groups. Changes to selected roles recompute this projection; it does not become an independent award. Final submission and approval recompute opening floors and max-merge them with culture/independent claims.

After all new-character roles and merits are attached, creation snapshots deferred overlap opening floors into a permanent saving effect. The canonical character's successful `AddTrait` applies its pending floor in the initial native trait insert, then consumes the claim before language callbacks. Explicit builder/FutureProg grants retain their existing authority. Ordinary learning and role branching consult the selected child's prerequisite plus its normal `LearnableProg`; the adapter supplies no new permission for unrelated skills or heard languages. Role branches are checked on skill use even at a casting ceiling.

The effect never creates new claims on login or installer rerun. On reload it retires pending claims whose skills already exist, without awarding values. This covers a crash after the adjusted skill insert commits but before the effect save. Native removal flushes the save queue before deleting the skill, so normal removal/reacquisition cannot repeat a consumed bonus. Empty effects retain the creation-time initialization marker.

These contracts apply to new characters with the configured merit. Adding or changing a merit on an existing character does not enroll that character for historical bonuses. Existing-world migration requires an explicit separate plan.

Focused coverage includes shared-root clamps, shared-child prerequisites, standalone projection, role resets, unchanged independent claims, exact writing exceptions, builder edits/removed links and restart claim retirement. A complete installed/gameplay certificate is recorded separately in the owning setting's evidence ledger.
