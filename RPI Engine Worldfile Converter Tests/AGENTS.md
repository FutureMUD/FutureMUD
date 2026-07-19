# Scope

This document defines the specific rules for Project RPI Engine Worldfile Converter Tests.
It inherits from the [Solution-Level AGENTS.md](../AGENTS.md).

## Purpose of Project

Contains MSTest-based parser, transformation, validation, and fixture-corpus tests for the RPI Engine Worldfile Converter.

## Key Architectural Principles

* Treat supported legacy input shapes and emitted FutureMUD definitions as compatibility contracts.
* Prefer small synthetic inputs for edge cases and checked-in fixture corpora for representative end-to-end conversions.
* Keep the suite deterministic and independent of a live database.
* This suite is part of the default unit-test pass.
