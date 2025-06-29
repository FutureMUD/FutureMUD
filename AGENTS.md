# Instructions for Codex

To compile the project locally or in automated checks:

1. Run `scripts/setup.sh` once to install the .NET 9 SDK in `~/.dotnet`.
2. Use `scripts/test.sh` to build the main engine project.

`test.sh` sets `DOTNET_EnableWindowsTargeting` so the build succeeds on Linux.
