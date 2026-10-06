# Armageddon representative firearm authority checkpoint

Checkpoint 15 builds on `dc1ef576080613fb23255e977a6e64ac1001da25`. One real manual-cycle internal-magazine firearm now passes native Load, Ready, Unready, selected ranged CombatAction and Unload, with exact ammunition identity, title/custody, stamina, condition and wound accounting.

Native acceptance exposed two persistence defects. The DB constructor did not restore loaded rounds' containment, and Unready left the chamber's saved definition unchanged. The existing raw load helper now restores legitimate children using original DB containment evidence without overwriting a returned body-held round, including gun-first historical stale-chamber reload. Unready now clears and marks the exact captured slot changed at the detach commit boundary, before receipt callbacks; revocation/replacement controls retain the item and current slot.

The retained managed baseline passed 1 / failed 3. Final focused Core passed 278/0/0, including seven constructor reload rows and three actual Unready controls. Final unfiltered Core passed 5,147/0/0 with stable source. Full Fast was NOT_RUN for this finite Core checkpoint; checkpoint14's cleared 7,503/0/0 is historical evidence only.

Six native cases and nineteen fresh-process readers passed: ordered success, queued expiry, precommit component-policy expiry, postcommit independent wound reaction, ordered miss and direct success. Accepted shots pay exactly 3 stamina and 0.01 condition once. Precommit refusal preserves exact chamber/magazine and pays nothing. Only hits install native wounds; the real postcommit wound callback's independent trait 60 survives. Reader stages cover ordinary unready, loaded chamber, shot/unload and one historical stale chamber, asserting actual body/item/component reload plus selected persisted stamina/trait/wound rows. Explicit gun-first loading preserves the returned round's actual body custody.

The [verification receipt](Armageddon_FirearmAuthority_Verification.json) retains exact case results, failed runs, ownership-checked disposable database/server cleanup, aligned 13-assembly source fingerprints and immutable copied binaries/dependencies before subsequent builds. All 23,338 earlier artifacts and 482 task files remain preserved; primary master 34eb remains clean. The full131071-byte brief is now locally materialized with native NTFS Library identity/version verification; its old read receipt remains unchanged.

This qualifies two ordinary nonstack rounds with no casing or separate projectile prototype. Shot/cover/recovery checks and race/encumbrance are controlled fixture dependencies. The postcommit hook is Body.OnWounded after wounds install; it does not qualify an actual defender countershot. Readers do not claim animated-actor reload, order replay or Telnet. No other lane was integrated; no publication, merge, deployment or shared/production DB occurred.

Mandatory remaining gates:

- Compact defended/failing or autonomous melee control: checkpoint14 qualifies a declared successful attack against a helpless target only.
- Actual independent defender ResponseToMove/countershot after original attacker expiry, with projectile/custody/wound controls. The current postcommit Body.OnWounded trait write does not qualify countershot.
- Representative native vehicle boarding: actual membership/slot/stamina, refused ordered attempt, independent/autonomous compatibility.
- Concrete container Get/Give/component Unload noncurrency merge conservation, absorbed cleanup, claim/expiry and cold reload; reachable evidenced whole/split/merge ammo, old casing/next-round and Musket/Artillery dependencies remain bounded separate gates.
- Separate mandatory release work: PC/NPC charm controller/reload/body switching; possession/reference-safe teardown and borrowed bodies; stock guard/rider/tracker/courier relationships; installed non-admin/per-spell native acceptance and whole-plan release checks.
- Existing separate lanes: structural cleanup, route-aware corpse restoration, Calm shared hooks and stock integration remain open.
