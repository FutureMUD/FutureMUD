# Manual Pilot contract - 19 October 2026

Direct recovered evidence: supplied `C:\Users\luker\Downloads\codedump.c`, SHA-256 `e44778f6a5fb9fd02db5c3ff78d4c468b93ae6c854a058d5af769dc0a95cd59f`; `cmd_pilot` at lines 113236 onward. This source was re-read during implementation. The earlier finding that ordinary Pilot proficiency was unverified is now resolved at source level, not at gameplay-acceptance level.

Room-scale wagon path (113503-113526): initial uniform 1..60 roll against Pilot; failure requests learning (Merchant3/other1); a second 1..60 roll at the pre-learning proficiency determines recovery. Recovery success returns without movement. A second failure adjusts the chosen direction by -1/0/+1 with four-heading wrap. Destination/closed/wagon/environment checks follow (113527-113566). Furniture/skimmer path (113355-113374) instead uses a 25% random cardinal deviation after failure. The source first attempts automatic missing-skill acquisition at5 using a wisdom learning chance (113284-113296).

Implemented native adaptation:

- One reusable configurable manual room-exit hook; off by default for other worlds, explicitly Pilot-bound and on for a fresh Armageddon skill package. No charge-only substitution.
- Uniform 1..60 initial and recovery rolls. Failure alone calls the configured native skill improver once; fixed source Merchant gain amounts become the approved native improvement model.
- One common two-stage policy for native room-exit vehicles, including ItemScale; this intentionally uses the wagon model rather than furniture's separate 25% policy. Eight native compass headings replace the four-heading index. Chosen exits must remain visible, safe and vehicle-ready; missing/ambiguous headings yield no progress.
- Missing skills are refused, with no wisdom-based auto-grant. This deliberate entitlement boundary prevents ordinary controls bypassing the approved Class/Subclass trees.
- Control/readiness precedes rolls. Safe selection does not spend propulsion; normal movement commits once and revalidates. Direction Sense remains a separate navigation sequence. RouteRoom/compiled routes are outside this room-exit hook.

This closes the direct-source investigation and adds functional manual proficiency. Required v1 acceptance still needs representative native manual control, failure/recovery/deviation and vehicle Direction Sense in the combined Setting world. Managed tests are recorded in the implementation ledger; they do not certify installed gameplay.
