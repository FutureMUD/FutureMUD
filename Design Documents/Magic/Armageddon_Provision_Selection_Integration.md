# Provision selection integration contract

This dependency is local to the provision lane on `codex/armageddon-five-stock-spells`, descended from exact local base `1ad24d636925da64e7a82ffa858b0af470e56b0a`. It follows the separately reviewed five-stock series ending `a905bb0615457d19db1b6a5f1a2f33be7ade50a3`. It does not integrate the independently reviewed charged-device or combat branches.

## Shared edits allocated by the coordinator

`Prepare` accepts an optional original casting copy. Both live preparations in `Cast` receive the initial `prepared.Spell`. The new `MagicCastingService.PreparedSelection.cs` partial checks spell ID, selected grade, profile version, separate target/caster effect counts, exact type, index and serialized effect configuration before transferring an invocation-local token. Existing template validation still runs. Initial validation records the token's recipient before any payment. Tokens require one unchanged recipient; these provision stocks are ordinary single room/item target spells.

During integration preserve the device lane's separately added pre-Paying admission and payment wrapper in `MagicCastingService.Execution.cs`, and its other allocated `Prepare` checks. This lane does not contain them. There are no shared native harness dispatch changes and no central ledger changes. The dedicated provision project links the existing harness and the previous dedicated five-stock partials unchanged, then adds its own entrypoint and UUID-isolated runner.

## Frozen choice and live admission

Optional `FoodProfiles` and `Recipes` are versioned XML with orders 1–32, boolean(character) predicates and an explicit `always` fallback last. Every configured predicate and output must resolve, including nonselected candidates. Configured malformed XML is retained and fails closed on reload/clone. No configured profiles means the original fixed prototype/liquid behavior.

First matching profile wins. Food draws independently from that profile's approved pool once per selected grade. A private immutable token carries that original output sequence or recipe, recipient reference, physical actor frame, profile order and authored/candidate metadata stamp. Fresh casting copies adopt it before validation. Predicate callbacks still execute on live state; failure, nonboolean return, changed first match, callback movement or authored metadata drift refuses admission rather than selecting a replacement. Revalidation never draws food again. An advisory quote is a separate preparation; a new explicit Cast selects its own choices.

Application uses the original casting copy and rechecks live profile admission. Existing paid-failure reporting applies to refusal before mutation. Exceptions after a real first output retain the existing durable `NeedsReview` receipt and quarantine: no replay, automatic retry, refund or invented completion. Consumed/reviewed origin IDs refuse before preparation.

Independent review found that food confirmation originally preceded eligibility and lifetime callbacks. The correction runs all those callbacks before the final food confirmation, including permanent and cached-lifetime exits. That confirmation is the last callback-bearing admission in the item adapter; physical frame and profile/policy metadata checks follow every profile callback. The choice remains frozen. `ProvisionFoodAdmissionCastingTests` drives full Cast through the actual food adapter with seven counted draws and callbacks mutating only the third preparation; temporary/permanent/cached-lifetime eligibility changes and lifetime movement all refuse before Paying, debit or item creation. The dedicated native fixture repeats final eligibility and lifetime callback drift against real domain/SQL state. Qualification before this correction remains prior history.

## Regression coverage and limits

`PreparedSelectionCastingTests` exercises actual full Cast, both repeated Prepare calls, one choice, a prepayment admission change, and partial mutation with durable uncertainty, origin replay refusal and staff acknowledgement without refund. `ProvisionProfileTests` exercises ordered predicates, explicit fallback, compile/signature/missing/null/failed predicate refusal, callback context changes, malformed XML preservation, original recipient binding and live first-match change after token adoption. Existing operation-reporting tests retain their assertions and now pass arrays/selected liquid to the extended private application constructors.

Native provision qualification is recorded separately with final source and assembly fingerprints. Unit fixtures do not substitute for native persistence; native fixtures use actual domain objects and SQL with controlled world/check catalogues, not a full live Telnet/login session. The old five-stock receipt is immutable history and is not a receipt for this dependency.
