# Publishing FutureMUD patch notes

Patch notes are repository-authored Markdown and deploy with the website. Create one file per published change set using this filename format:

```
YYYY-MM-DD-stable-slug.md
```

The filename date must match the `date` field. Every published file requires this front matter:

```
---
title: Engine 2.0.0
summary: A concise public summary of the release.
date: 2026-07-16
tags: engine, release
---
```

Write the body using the website's safe Markdown subset: headings, paragraphs, bullet lists, fenced code, strong text, inline code, and HTTP or HTTPS links. Raw HTML is encoded and unsafe link schemes are rejected.

Files that do not match the dated filename pattern, including this README, are not published. Run `FutureMUD.Web.Tests` before merging a patch note.
