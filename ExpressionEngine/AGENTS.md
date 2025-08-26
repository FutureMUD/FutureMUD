# Scope

This document defines the **specific rules for Project ExpressionEngine**.  
It inherits from the [Solution-Level AGENTS.md](../AGENTS.md).

## Inheritance

* Rules here **extend or override** solution-level rules.
* Precedence: **Module > Project > Solution**.

## Purpose of Project
Provides NCalc-based expression parsing and evaluation for user-supplied formulas used throughout the engine.

## Key Architectural Principles
* Exposes `IExpression` to abstract expression evaluation.
* `Expression` wraps NCalc and adds custom functions such as `rand`, `drand`, `dice`, and `not`.
* Parameters are passed via a dictionary; enums are converted to numeric values before evaluation.
* Errors are logged and published through the `ExpressionError` event so callers can respond.
* Random behaviour uses a shared `RandomInstance`; consider thread safety when extending functionality.

## Notes

* All modules inherit both the solution-level and project-level rules unless explicitly overridden.
