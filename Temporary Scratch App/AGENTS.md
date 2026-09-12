# Temporary Scratch App instructions

Inherits [repository instructions](../AGENTS.md).

This console project runs snippets, prototypes, data manipulations and utility code using references to `FutureMUDLibrary` and other solution projects, without booting a complete game.

It is not the repository's regression-test suite. Put durable automated tests in the owning test project, not here. Keep temporary utility/prototype code separate from production engine behaviour. Before running a data-manipulation snippet, verify its input/output and database target; the availability of this scratch host is not permission to mutate live data.
