# FutureMUD Patch Notes Authoring And Publishing Guide

## Purpose
This document defines how FutureMUD public patch notes are written and published on the FutureMUD website.

These posts are public-facing patch notes, not exhaustive engineering changelogs. The job is to tell builders, admins, and interested players what changed, why it matters, and whether they need to do anything about it.

The canonical published copy lives at [futuremud.com/patch-notes](https://futuremud.com/patch-notes). Discord is a discussion and announcement channel: it can link to a published note and provide a short summary, but it is not the authoritative archive and should not contain the only copy of release information.

The same overall style applies to all public-facing FutureMUD products:

- `Engine`
- `Database Seeder`
- `Discord Bot`
- `Terrain Planner`
- `Terrain API`

Website and publishing-platform changes can also have their own patch notes when they materially affect downloads, documentation, installation, or other public workflows.

## Publishing Channels

### Canonical Channel: FutureMUD Website

Every public patch note is a Markdown file in `FutureMUD.Web/Content/PatchNotes`. Once merged to `master`, it is deployed with the website and appears in two places:

- `/patch-notes` shows the publication date, title, summary, and tags.
- `/patch-notes/{slug}` shows the full note.

Patch-note detail pages are also included in the website sitemap. The website does not currently provide a separate patch-note RSS feed; `/news/feed.xml` is for news and project announcements, not product patch notes.

### Supporting Channel: Discord

Discord remains the right place to announce a release, discuss its effects, answer questions, collect bug reports, and provide support. A Discord announcement should normally contain:

- the product and version;
- one or two sentences describing the most important changes or warnings;
- a link to the canonical website patch note;
- a link to the relevant download page when useful.

Do not maintain a divergent long-form Discord copy. If a published note needs correction, update the repository Markdown and redeploy the website, then point Discord readers to the corrected page.

### News Versus Patch Notes

Use `FutureMUD.Web/Content/News` for project announcements, engineering updates, guides, or broader context. Use `FutureMUD.Web/Content/PatchNotes` for concrete product, release, website, or publishing changes. A major release can have both: the news post tells the story, while the patch note records what changed and what users must do.

## Website Content Contract

### File Location And Name

Create one file per published change set in `FutureMUD.Web/Content/PatchNotes` using:

```text
YYYY-MM-DD-stable-slug.md
```

The filename must use lowercase ASCII letters, numbers, and hyphens after the date. The date becomes the publication date and the part after `YYYY-MM-DD-` becomes the public URL slug. Renaming a published file changes its URL, so treat the filename as stable once it is public.

Files that do not match this pattern are not published. This is why the directory's `README.md` can live beside the content without appearing on the website.

### Required Front Matter

Every patch note must begin with terminated front matter containing all four fields:

```yaml
---
title: Engine 2.0.0
summary: A concise public summary of the release.
date: 2026-07-16
tags: engine, release
---
```

- `title` is the page heading and the link text on the patch-note index. Include the public product name and version for a product release.
- `summary` appears on both the index and the detail page. Write it as a useful standalone sentence, not as a duplicate heading or a teaser with missing context.
- `date` must be an ISO date in `YYYY-MM-DD` form and must exactly match the filename date.
- `tags` is a comma-separated, non-empty list. Use stable lowercase audience or product terms such as `engine`, `seeder`, `discord-bot`, `terrain-planner`, `terrain-api`, `website`, `publishing`, `security`, or `release`.

The index is sorted by publication date descending, then by slug. Do not use a future date merely to force a note to the top.

### Supported Markdown And Safety

The website deliberately supports a small, safe Markdown subset:

- headings using `#` through `######`;
- paragraphs;
- unordered lists using `- `;
- fenced code blocks using triple backticks;
- `**strong text**`;
- inline code using backticks;
- links using `[label](https://example.com)` or safe relative URLs.

Raw HTML is encoded rather than rendered. Unsafe link schemes are rejected. Do not rely on tables, images, blockquotes, nested lists, task lists, embedded media, or other extended Markdown features unless the renderer and its tests are deliberately expanded first.

Because the website page already renders the title, summary, date, and tags from front matter, do not repeat the title as the first body heading. Start the body with an operational note, a short introductory paragraph, a section heading, or the change list.

### Links And Durable References

Prefer links that will remain valid after the release:

- `/downloads` or a stable versioned download URL;
- `/getting-started` for installation prerequisites;
- `/docs/...` for published engine reference material;
- `/license` for licensing terms;
- the public GitHub repository when source provenance matters;
- the FutureMUD Discord invite for support and discussion.

Do not link to local files, temporary workflow artifacts, private administration pages, draft pull requests, or Discord messages as the only source of required upgrade information.

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
### Put The Product And Version In The Page Title

Set the front-matter `title` to the product name users recognise and its exact version number. The website renders this as the page heading, so do not repeat it at the start of the body.

Typical title values:

- `Engine 1.56.0`
- `Database Seeder 2.4.0`
- `Discord Bot 1.8.0`
- `Terrain Planner 1.1.0`
- `Terrain API 1.0.1`

Keep the title simple and prominent. Use the product's established public name and the exact three-part release version.

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

1. Confirm the public product name, exact three-part version, release tag, supported runtimes, and expected download availability.
2. Decide whether the content is a patch note, a news post, or both. Concrete shipped changes belong in patch notes.
3. Gather the changes and mark them as player-facing, builder-facing, admin-facing, operator-facing, or internal-only.
4. Pull breaking changes, upgrade requirements, compatibility notes, and warnings to the top.
5. Decide whether the release needs a simple list or a few meaningful sections.
6. Group tiny related fixes and rewrite technical implementation details as practical outcomes.
7. Choose a stable dated filename and write complete front matter. The summary must make sense on the index without the body.
8. Write the body using only the supported safe Markdown subset. Do not repeat the front-matter title.
9. Check links, command names, product names, version numbers, and any claimed download availability against the actual release.
10. Cut internal detail that users do not need and check that operational actions are explicit.
11. Run the website tests and review the rendered index and detail page before merging.
12. After deployment, verify the public URL and then announce it through Discord or other community channels.

## Repository Publishing Workflow

1. Add the note beneath `FutureMUD.Web/Content/PatchNotes` on a normal review branch.
2. Run:

   ```powershell
   dotnet test FutureMUD.Web.Tests/FutureMUD.Web.Tests.csproj -c Release -m:1 -p:RestoreBuildInParallel=false -p:NuGetAudit=false
   ```

3. Open a pull request that includes the note and any related website changes. Review the filename, front matter, summary, rendered structure, links, product version, and operational claims.
4. Coordinate with the product release workflow. Product tags publish binaries and generated documentation; merging a patch note publishes website content. These are separate workflows. Do not say a download is available until the product release has been promoted and verified on `/downloads`.
5. Merge the patch-note pull request to `master`. Changes below `FutureMUD.Web/**` trigger `.github/workflows/deploy-website.yml`, which restores, tests, publishes the Linux website, atomically activates it on the server, and checks `/health/ready`.
6. Confirm the workflow succeeded, then verify both `/patch-notes` and `/patch-notes/{slug}` on the public website. Check the displayed title, summary, date, tags, body structure, links, and any referenced download.
7. Publish the short Discord announcement with a link to the verified website note.

Do not edit patch-note files directly on the production server. Repository Markdown is the source of truth, and the deployment workflow is the supported publishing channel.

### Corrections After Publication

For a typo or clarification that does not change the release meaning, update the existing Markdown file without renaming it and redeploy. For a material correction—especially a missed warning, incorrect compatibility statement, or unavailable download—update the website note promptly and post a Discord correction linking to it. Preserve the original publication date unless the change represents a genuinely new release or change set.

## Reusable Templates

These are website-ready skeletons. Replace the example date, slug, version, summary, and tags with real values.

### Small Product Release

Filename: `2026-07-20-engine-2-0-1.md`

```markdown
---
title: Engine 2.0.1
summary: Fixes several builder workflow issues and improves pathfinding around locked exits.
date: 2026-07-20
tags: engine, release
---
No additional setup is required for this update.

- Fixed a bug where ...
- Added ...
- Improved ...
- Changed ... so that ...
```

### Larger Release With Operational Notes

Filename: `2026-08-03-engine-2-1-0.md`

```markdown
---
title: Engine 2.1.0
summary: Adds the example system, expands builder controls, and requires an updated .NET runtime.
date: 2026-08-03
tags: engine, release, upgrade
---
**Upgrade note:** Install the required .NET runtime before replacing the previous Engine build.

## Bug Fixes

- Fixed ...
- Improved ...

## Minor Features

- Added ...
- Changed ...

## Major New Feature: Example System

Implemented a new system for ... This lets builders, admins, or players ... It still has some room for future improvement, but it is ready to use now.
```

### Multi-Product Change Set

Filename: `2026-08-10-august-product-updates.md`

```markdown
---
title: August 2026 product updates
summary: Updates the Engine, Database Seeder, and Discord Bot with coordinated fixes and new tooling.
date: 2026-08-10
tags: engine, seeder, discord-bot, release
---
## Engine 2.2.0

- Fixed ...
- Added ...

## Database Seeder 2.5.0

- Added ...
- Improved ...

## Discord Bot 1.9.0

- Fixed ...
- Added ...
```

### Website Or Publishing Change

Filename: `2026-08-12-download-workflow-update.md`

```markdown
---
title: Download workflow update
summary: Improves release verification and clarifies supported runtime downloads.
date: 2026-08-12
tags: website, publishing
---
- Added ...
- Changed ... so that ...
- Updated the download guidance for ...
```

### Discord Announcement

```text
Engine 2.0.1 is now available.

This release fixes several builder workflow issues and improves pathfinding around locked exits. No additional setup is required.

Patch notes: https://futuremud.com/patch-notes/engine-2-0-1
Downloads: https://futuremud.com/downloads
```

Keep the Discord announcement short. The website note is the canonical full record.

## Do And Don't

### Do

- Do keep the emphasis on what changed for the user.
- Do make the front-matter summary useful when read by itself on the patch-note index.
- Do mention warnings, uncertainty, costs, compatibility, or rollout caveats when they matter.
- Do coordinate claims about downloadable releases with the separately promoted product release.
- Do use stable filenames, dates, tags, product names, and version numbers.
- Do use only the Markdown features that the website renderer supports.
- Do summarise internal work in broad user-relevant terms.
- Do use sections when a release is large enough to benefit from them.
- Do keep multi-product releases clearly separated by product heading.
- Do verify the canonical website page before linking it from Discord.

### Don't

- Don't treat a Discord post as the canonical or only patch note.
- Don't duplicate the front-matter title as the first body heading.
- Don't turn the post into a technical diff.
- Don't explain refactors in depth unless the audience needs that detail.
- Don't force section headings onto very small releases.
- Don't mix different product updates together without clear headings.
- Don't pad the post with every tiny fix when a grouped summary is clearer.
- Don't rely on raw HTML, tables, images, nested lists, or other unsupported Markdown.
- Don't rename a published file casually, because that changes its public URL.
- Don't claim that a release is downloadable until the version is visible and verified on the website.
- Don't edit the production server as a substitute for merging repository content.

## Final Sense Check

Before merging and announcing, quickly check:

- Does the filename follow `YYYY-MM-DD-stable-slug.md`, and is the slug suitable as a permanent public URL?
- Are `title`, `summary`, `date`, and `tags` present, and does the date match the filename?
- Does the front-matter title clearly identify the product and version when appropriate?
- Does the summary make sense on the index without reading the body?
- Is any upgrade note, warning, compatibility issue, or caveat visible immediately?
- Would a builder, admin, player, or operator understand why each item matters?
- Have internal-only details been simplified enough?
- If there are multiple products, are they clearly separated?
- Does the body avoid repeating information already rendered by the page header?
- Does the note use only supported Markdown and durable public links?
- Has the relevant product release been promoted and verified if the note says it is available?
- Did the website tests and deployment workflow pass?
- Are the index and detail URLs correct on the live website?
- Does any Discord announcement link back to that canonical page?
- Does the note sound like a person explaining a release rather than a changelog export?

If the answer to those questions is yes, the patch note is ready to publish.
