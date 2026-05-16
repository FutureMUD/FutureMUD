# FutureMUD Patch Notes Authoring Guide

## Purpose
This document captures how FutureMUD public release notes are usually written for Discord.

These posts are public-facing patch notes, not exhaustive engineering changelogs. The job is to tell builders, admins, and interested players what changed, why it matters, and whether they need to do anything about it.

The same overall style applies to all public-facing FutureMUD products:

- `Engine`
- `Database Seeder`
- `Discord Bot`
- `Terrain Planner`
- `Terrain API`

## Core Voice And Tone
FutureMUD patch notes should sound like a developer talking directly to the people who use the products, not like marketing copy and not like a git diff.

The usual voice is:

- plainspoken and direct
- lightly conversational without becoming chatty
- not super technical unless the audience actually needs the technical detail
- candid about uncertainty, caveats, performance impact, costs, or future follow-up
- focused on the visible effect of a change rather than the internal implementation

Good patch notes in this style often sound like:

- "Fixed a bug where..."
- "Added a new..."
- "Changed the way..."
- "Implemented support for..."
- "Improved performance in..."

When useful, it is fine to add short asides in parentheses. Those asides should add context, personality, or a warning, not derail the note.

## What These Posts Are Trying To Do
Each post should quickly answer most of these questions:

- What product was updated?
- What version is it?
- Are there any required upgrades, compatibility notes, or warnings?
- What changed that a user, builder, or operator will actually notice?
- Are there any bigger new features worth calling out separately?
- Is there any follow-up information the audience should know, such as setup work, costs, or a plan for future refinement?

If a piece of work does not help answer one of those questions, it probably does not need much space in the patch notes.

## Structural Rules
### Start With The Product And Version
Begin with a heading that names the product the way users see it and includes the version number.

Typical heading patterns:

- `Engine Version 1.56.0`
- `Database Seeder v2.4.0`
- `Discord Bot v1.8.0`
- `Terrain Planner v0.9.0`
- `Terrain API v0.5.0`

Keep the heading simple and prominent. Do not bury the product name halfway through the post.

### Put Breaking Or Operational Notes Up Front
If there is any note that affects whether someone can safely update, put it immediately below the heading before the change list.

Typical examples:

- required runtime or framework upgrades
- breaking compatibility changes
- new setup steps
- hosting or deployment impacts
- cost caveats
- "this is only tentatively fixed" style caveats when honesty matters more than confidence

These notes should be short, plain, and explicit.

### Use The Smallest Structure That Fits
For a small release, a heading and a simple bullet list is usually enough.

For a bigger release, use sections such as:

- `Bug Fixes`
- `Minor Features`
- `Major New Feature`
- `Performance`
- `Operational Notes`

Only add sections when they help readability. Do not force a tiny update into a big formal structure if a short bullet list reads better.

### Use Paragraphs For Big Features
When a feature needs more than one sentence of explanation, switch from a single bullet to a short paragraph under a heading.

This is especially appropriate for:

- big new systems
- new products or major product capabilities
- features with non-obvious value
- things that need a warning, rollout note, or usage guidance

The goal is still brevity. The extra paragraph is there to explain why the feature matters, not to document every implementation detail.

## Content Selection Rules
### Definitely Include
Patch notes should usually include:

- user-facing bug fixes
- builder or admin tooling changes
- new commands, options, systems, or workflows
- visible behavioural changes
- notable performance improvements
- security or platform changes that matter to operators
- setup, upgrade, deployment, or hosting notes when relevant

### Usually Summarise Broadly
Internal work can be mentioned, but usually only at a high level.

Examples of acceptable broad phrasing:

- "Improved some internal performance in scheduler code"
- "Tidied up some internal handling around room echoes"
- "Refactored some of the terrain loading code to make future changes easier"

If the work was mainly refactoring, cleanup, or code movement, only go deeper when the audience needs to know the consequence.

### Usually Leave Out
Avoid spending much time on:

- purely internal renames
- low-level refactors with no practical effect
- implementation details that do not change usage or outcomes
- every tiny fix when a grouped summary is clearer

The patch notes should describe what changed for the user, not prove how much work went into the release.

### Group Small Changes
If there are several tiny fixes in one area, group them rather than making the post feel like a raw issue tracker dump.

For example, prefer:

- "Fixed a few issues with map display colours and terrain tags"

over several separate bullets for each tiny display correction unless those details matter to users.

## How Technical To Be
Default to explaining the result, not the mechanism.

Good:

- "Added a new admin command to inspect recent speech at a location"
- "Improved the way pathfinding AI handles locked doors"
- "Terrain Planner can now preserve more of the map display metadata when editing terrain"

Usually too technical unless needed:

- naming internal classes, methods, or modules
- describing refactor strategy
- explaining internal event flow or database structures
- listing internal abstractions that changed

It is still fine to mention command names, feature names, script function names, or product-facing identifiers when they are useful to builders and admins. Inline code formatting is appropriate there.

## Wording Conventions
### Prefer Clear Action Verbs
Lead with direct verbs such as:

- `Fixed`
- `Added`
- `Implemented`
- `Improved`
- `Changed`

Those verbs make the post easy to scan.

### Explain The Consequence
Whenever possible, finish the sentence with the practical result.

Useful patterns:

- "Fixed a bug where X was causing Y"
- "Changed the way X works so that Y"
- "Added support for X, which lets you Y"
- "Improved X performance when Y"

This is usually better than naming a system and assuming the reader already knows why it matters.

### Be Candid When Needed
Honesty is part of the style.

It is acceptable to say things like:

- the fix is tentative
- the feature still needs more optimisation
- there may be hosting or token costs
- more work is planned later

That candour makes the notes feel trustworthy and practical.

### Use Backticks Selectively
Use inline code formatting for:

- commands
- function names that builders or admins actually use
- config keys or technical identifiers that matter operationally

Do not wrap ordinary prose in backticks just because it is technical-sounding.

## Product Handling Rules
### Same Voice, Different Product Name
The writing voice stays the same across all public products. What changes is the product heading and the kinds of operational notes that matter.

### Engine
Engine notes usually focus on:

- gameplay behaviour
- builder and admin commands
- runtime fixes
- FutureProg additions
- new systems
- performance or platform upgrades

### Database Seeder
Seeder notes usually focus on:

- new seeded content
- expanded builder starting points
- seeder workflow fixes
- repeatability or resilience improvements
- new optional seed packages

### Discord Bot
Discord Bot notes usually focus on:

- new bot commands or integrations
- moderation or admin workflow changes
- reliability improvements
- message handling or connection fixes
- setup or permissions implications

### Terrain Planner
Terrain Planner notes usually focus on:

- editing workflow improvements
- map display or terrain metadata changes
- import or export behaviour
- usability improvements
- browser or client-side limitations when relevant

### Terrain API
Terrain API notes usually focus on:

- new endpoints or capabilities
- response or compatibility changes
- authentication or hosting considerations
- performance or reliability changes
- deployment notes when they matter

### Multi-Product Releases
When more than one product ships at once, do not mix everything into one combined bullet list.

Instead:

1. Start with the first product heading and its notes.
2. Add a separate heading for the next product.
3. Keep each product's notes self-contained.

That makes it clear which update belongs to which product.

## Authoring Method
When assembling patch notes for a release, use this process:

1. Gather the changes for the product being released.
2. Mark which ones are user-facing, admin-facing, builder-facing, operator-facing, or internal-only.
3. Pull any breaking changes, upgrade requirements, or warnings to the top.
4. Decide whether the release is small enough for a simple bullet list or large enough to need sections.
5. Group tiny related fixes together.
6. Rewrite technical changes into practical plain-English outcomes.
7. Give big features a short paragraph if they need context.
8. Do a last pass to cut internal detail that users do not need.

This method should produce notes that feel informative without feeling dense.

## Reusable Templates
These are skeletons, not rigid formats. Adjust as needed, but stay within the same general style.

### Small Engine Patch
```text
Engine Version 1.56.0

- Fixed a bug where ...
- Added ...
- Improved ...
- Changed ... so that ...
```

### Larger Engine Release With Categories
```text
Engine Version 1.57.0

Note: You will need to update to ... before using this version.

Bug Fixes:

- Fixed ...
- Fixed ...
- Improved ...

Minor Features:

- Added ...
- Added ...

Major New Feature: Example System

Implemented a new system for ... This lets builders/admins/players ... It still has some room for future improvement, but it is ready to use now.
```

### Seeder-Only Release
```text
Database Seeder v2.4.0

- Added new seeded ...
- Improved some of the default ...
- Changed the way the seeder handles ... so that it no longer ...
- Added support for seeding ...
```

### Discord Bot Release
```text
Discord Bot v1.8.0

- Added a new command for ...
- Fixed an issue where the bot would ...
- Improved reliability around ...
- Changed ... so that admins can ...
```

### Terrain Planner Release
```text
Terrain Planner v0.9.0

- Improved the workflow for ...
- Fixed an issue where terrain edits could ...
- Added support for ...
- Changed the way ... displays so that ...
```

### Terrain API Release
```text
Terrain API v0.5.0

Note: This update changes ... so make sure you also update ...

- Added a new endpoint for ...
- Improved performance for ...
- Fixed a bug where ...
- Changed ... so that clients now ...
```

### Combined Multi-Product Release Post
```text
Engine Version 1.58.0

- Fixed ...
- Added ...

Database Seeder v2.5.0

- Added ...
- Improved ...

Discord Bot v1.9.0

- Fixed ...
- Added ...
```

### Breaking Change Or Required Upgrade Note
```text
Engine Version 1.59.0

Note: You must update to ... in order to use this version going forward.

- Fixed ...
- Added ...
- Improved ...
```

## Do And Don't
### Do

- Do keep the emphasis on what changed for the user.
- Do mention warnings, uncertainty, costs, or rollout caveats when they matter.
- Do summarise internal work in broad user-relevant terms.
- Do use sections when a release is large enough to benefit from them.
- Do keep multi-product releases clearly separated by product heading.

### Don't

- Don't turn the post into a technical diff.
- Don't explain refactors in depth unless the audience needs that detail.
- Don't force section headings onto very small releases.
- Don't mix different product updates together without clear headings.
- Don't pad the post with every tiny fix when a grouped summary is clearer.

## Final Sense Check
Before posting, quickly check:

- Does the heading clearly identify the product and version?
- Is any upgrade note, warning, or caveat visible immediately?
- Would a builder, admin, or operator understand why each bullet matters?
- Have internal-only details been simplified enough?
- If there are multiple products, are they clearly separated?
- Does the post sound like a person explaining a release, rather than a changelog export?

If the answer to those questions is yes, the patch notes are probably in the right style.
