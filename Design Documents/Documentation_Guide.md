# Documentation Guide

## Document roles

Each system has a master guide describing its supported behavior, architecture, persistence and limits. Builder/player guides explain practical use. Reference catalogues preserve content, identities and source evidence. A roadmap/proposal describes outstanding design; a verification record describes an executed or explicitly unexecuted check at a stated revision.

When an implementation plan is complete, transfer lasting requirements, invariants, compatibility decisions and remaining gaps into the owning master guide, update inbound links, then delete the task list. Keep dated receipts when they contain unique evidence, failures or limitations. Do not preserve obsolete API sketches as current contracts or delete an unfinished requirement because most of its plan shipped. Git history retains the removed planning prose.

Do not call a system complete from a checklist, source marker, test-project presence or old release count. Current source determines whether a feature exists; tests and live qualification determine what has actually been verified. Preserve deliberate exclusions and separate proposed behavior from delivered behavior within mixed design documents.

## Names and placement

- Use readable `Title_Case_With_Underscores.md` filenames and the owning thematic directory. Keep common acronyms consistent, such as `AI`, `NPC` and `API`.
- Use `_System.md` or an established `_System_Overview.md` for master contracts, `_Builder_Guide.md`/`_Player_Guide.md` for practical guides, `_Roadmap.md`/`_Proposal.md` for unfinished work and `_Verification_YYYYMMDD.md` for new dated evidence.
- Retain established descriptive names when they are already unambiguous; do not rename stable catalogue/export/resource identities merely for cosmetic uniformity.
- Group optional setting integration beneath its setting. Generic engine magic belongs in `Magic/`; Armageddon-specific magic belongs in `Armageddon/Magic/`. Vehicles belong in `World/`.
- Keep current contracts, outstanding proposals and dated evidence visibly separated in each area's `README.md`. Use a `Proposals/` or `Verification/` subdirectory when a large family needs one.
- Use UTF-8 and repository-relative Markdown links. Encode spaces in link targets or use angle brackets. After moving a file, resolve its outgoing links from its new directory and update all inbound documentation, tool and instruction references.

## Publishing builder guides

[Builder guides](https://futuremud.com/docs/builder-guides) are for people and agents working inside a running FutureMUD game. Put each public guide directly in `Design Documents/Builder Guides/` as a UTF-8 `.md` file. Placement in that folder opts the guide into publication. Keep engine and seeder implementation instructions, proposals and verification records in their thematic directories. The thematic index should link to the guide's canonical source.

Begin the file with the following front matter, then a single Markdown title. The title appears in the results list and at the top of the page.

```markdown
---
slug: room-descriptions
category: World building
summary: Write room descriptions that respond to light, weather and viewer knowledge.
---

# Room descriptions
```

Use a unique slug of lowercase letters and digits with single hyphens between words. Keep the slug stable so links continue to work after a title or filename change. Reuse an existing category where it fits. The site searches the title, summary and body, and filters by category. Relative links to other public guides open their site pages. Links to supporting files elsewhere in the repository open the corresponding GitHub paths. Use ordinary headings, lists, tables and fenced command examples. The guide renderer escapes raw HTML.

Explain the builder's mental model and prerequisites, then show the relevant command syntax and its revision, activation and permission rules. Walk through a concrete setup with expected results, show how it connects to other systems, and document common failures and how to diagnose them. Link to the command or FutureProg reference for exhaustive syntax. See the [builder guide coverage review](Building/Builder_Guide_Coverage_20261118.md) for the current backlog.

Build and run `FutureMUD.Web.Tests` when adding or changing guides. The website build embeds every guide in the publication folder and the `Deploy FutureMUD website` action picks up changes after they reach `master`. Removing a guide from the folder removes its page in the next deployment. See the [website publishing contract](Core/FutureMUD_Website_and_Publishing.md#builder-guide-publication).

## Historical and generated material

Published news/patch notes, source research, original corpora and generated/embedded payloads have different lifecycles from implementation plans. Preserve dated claims as history; correct a known factual overstatement explicitly. An ignored `.artifacts` path is local evidence and should be labelled as such, not presented as a downloadable repository file.

CultureSeeder's retained handoff directories contain runtime JSON resources and research/catalogue inputs. Their names are compatibility exceptions, not instructions to repeat the completed handoff. Generated era/admission/catalogue assets retain their owning scripts and stable identities.

## Verification for documentation changes

Review changed behavior claims against the owning source/contracts. Validate local links, moved paths, index coverage, UTF-8 and the diff. Check tool/project references when moving documentation used by scripts or tests. Run code tests when an executable contract changes; prose-only consolidation does not certify gameplay, installer reruns, SMTP, MySQL or release deployment.

The [16 November 2026 suite review](Core/Documentation_Review_20261116.md) records this consolidation's source baseline, retired plans and validation limits.
