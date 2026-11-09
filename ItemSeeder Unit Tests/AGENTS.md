# Scope

This project contains tests owned by the standalone Item Seeder product, including item catalogue generation, manifest ownership, repeatability, source inventory and item-specific installation behaviour.

Apply the [seeder test policy](../Design%20Documents/Seeding/Seeder_Test_Policy.md). Keep functional compiler, binding, ownership and installation coverage. Remove catalogue name/count/value snapshots and source/document task checklists; retain catalogue-dependent compatibility checks only with a concrete risk-based justification.

Tests that inspect shared database foundations may reference Database Seeder for fixture source and prerequisite behavior. Keep the fast suite suitable for normal Item Seeder CI. Put comprehensive catalogue capture, install and rerun matrices under the existing integration and extended categories.
