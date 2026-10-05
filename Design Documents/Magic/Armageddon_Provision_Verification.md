# Provision stock verification checkpoint

Sustain Meal and Draw Wine are ordinary editable stock spells with ordered native food/liquid profiles. Their immutable prepared selections survive both live admission copies without rerolling. The food adapter confirms its selection and physical actor frame after every eligibility/lifetime callback, including permanent and cached-lifetime branches; late drift refuses before payment or output creation.

## Local source commits

- `c7ebddb7e9b66eb9b219b6ca150357f1112c9a08`: optional prepared-selection contract and provision adapter profiles.
- `074ed3e97dfedaf25dbd4f345328b84b29883f46`: two editable stock definitions, dedicated tests and native entrypoint.
- `2c0b0e367d735dfb4757d26fecea795abf5b4e22`: final food callback-order correction and actual-adapter tests.

Final execution qualified source commit `2c0b0e367d735dfb4757d26fecea795abf5b4e22`. The receipt and this note form a subsequent documentation-only checkpoint. The earlier five-stock receipt remains unchanged.

## Final evidence

| Check | Result | Local evidence |
| --- | --- | --- |
| Aggregate, all ten test projects | 7,337 passed; zero failed/skipped; complete counts | `.artifacts/test-runs/provision-final-units-02/20261005T081948Z-baae46c18c94/summary.json` |
| Relevant cases extracted from that same aggregate | 92 passed | `.artifacts/test-runs/provision-final-units-02/provision-focus.json` |
| Dedicated native build | Zero errors; 140 warnings | `.artifacts/test-runs/five-stock-reporting-history/provision-native-build-11.log` |
| Fresh native provision, five-stock and four-stock modes | All eleven stock spells passed; owned databases and servers cleaned up | `.artifacts/test-runs/provision-final-native-02/summary.json` |
| Native source and assembly stability | 6,985 input files and complete DLL manifest unchanged | `.artifacts/test-runs/provision-final-native-02/{source,assemblies}-{before,after}.json` |

Aggregate source fingerprint: `defcd761d2fc8739f94693f671c994b84b4ca31d4bdfc156381e45affdda4aab`.

Dedicated harness DLL SHA256: `7817cc8ae7e9e6d2cdd80e516b7b6b0d4640ed1b6d1b2d5f0c8b6cb0cd4225c9`.

`Armageddon_Provision_Verification_Receipt.json` binds 97 relevant source files to raw SHA256 and committed Git blobs, 20 assemblies to SHA256, and 28 evidence artifacts to SHA256. Full native source/DLL manifests and all ten aggregate TRX files are included. Earlier diagnostic runs are retained and do not substitute for this final packet.

## Review and integration limits

Parent independent review and integration remain pending. Preserve the separately owned device admission/payment hooks when integrating the allocated casting preparation changes. The production stock-builder hook is included; shared native harness dispatch and central ledgers are untouched. The existing five-utility factory APIs remain unchanged by this provision batch.

Native acceptance uses real domain objects and SQL with controlled surrounding world/checks and targeted mutation callbacks; it is not full Telnet/login acceptance. Historical conditional precedence and nominal food clock were recovered. Exact historical food/liquid statistics and native identity correspondence remain unavailable and require explicit builder-authored mappings. Wine safety and source-unit volume mapping, saved food quantities and absolute item deadlines are documented engine adaptations. See `Armageddon_Provision_Stock_Integration.md` and `Armageddon_Provision_Selection_Integration.md` for the recovered source rules and integration boundaries.
