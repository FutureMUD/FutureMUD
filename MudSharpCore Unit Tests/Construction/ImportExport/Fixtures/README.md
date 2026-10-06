# Frozen legacy spatial package fixtures

These JSON files were produced by the previously cleared expansion binary at commit `71a41999d291d3a7c70599e60c9e1a7145896130`, before the current v4 implementation was built. The source assembly SHA-256 was `BBC8DEF02B68E95BFADDBE2EB18F4DD31072A1FB39A039D93214328D6C746EB8`.

The tests pin the original canonical integrity hashes and verify them before normalization. Room IDs 100/101 differ from Cell IDs 200/201; duplicate coordinates, route geometry (v2/v3) and overlapping Areas (v3) are intentional. Do not regenerate fixtures using the implementation under test.

Malformed variants use frozen legacy DTOs only to produce correctly checksummed negative cases. These DTOs are wire compatibility records, not live Room entities or migration owners.
