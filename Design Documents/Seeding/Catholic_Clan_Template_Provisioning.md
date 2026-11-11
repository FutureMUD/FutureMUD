# Catholic clan template provisioning

The Catholic catalogue implements the locked 10 November 2026 handover against PRs 850, 853, 854 and 858. The complete field-level definitions, eight alternative profiles, research links and stated simplifications are preserved in `DatabaseSeeder/Seeders/ClanSeeder/Catholic/CatholicTemplates.json`. Logical keys are authoring metadata, not database columns.

## Standing and office

Church, diocesan and parish ranks record affiliation and ordination: Catechumen, Lay Member, Deacon, Priest and Bishop. Pope, Cardinal, Archbishop, Metropolitan and Parish Priest are appointments. Religious ranks record formation/profession; religious superiors and councils are appointments. Losing an office never lowers standing. Membership and ordination are deliberately recorded separately in every institution, with no automatic inheritance or synchronisation.

Religious governors may record local formation/profession promotions and rank-path changes. This does not ordain anyone or grant demotion/expulsion. Ecclesial governors do not receive ordination promotion/demotion privileges. Finite positive office caps are adjustable game ceilings, not historical size claims. No salary or assets are installed.

## Create and stage

Inspect with `clan templates` and `clan view <template>`, then:

```text
clan create staged catholicchurch mychurch "Church of the Setting"
prog execute CatholicClanStatus self mychurch
prog execute CatholicClanExport self mychurch
```

The generic admin-only `staged` copy mode retains `IsTemplate=true`, with no founder membership and no running ballots. Profiled templates require this generic staging workflow and refuse the ordinary live-copy mode. Generated candidate/voting progs fail closed while staged. The existing world `CreatedClan` hook still runs once during creation. Stock templates remain unchanged. A copy of an already provisioned Catholic clan stages afresh: external bindings must be deliberately selected again. It copies configuration, not memberships or election history.

The six `CatholicClanStatus`, `CatholicClanBind`, `CatholicClanVariant`, `CatholicClanActivate`, `CatholicClanPreflight` and `CatholicClanExport` progs are seeded content, invoked through the existing administrator `prog execute` command. The engine has no Catholic-specific command, builtin or catalogue.

The private `ClanTemplate_<ID>_Profile` prog comment retains the source key, copy ID, rank/office key-to-ID mappings, selected variants, live bindings and generated prog IDs. `export` displays that authoring record for the world's setup records. The profile also carries the declarative standing/electorate rules and variant overrides authored by the seeder. The runtime evaluates generic membership, minimum-rank, exact-rank and office conditions. The provisioning profile itself uses no additional schema. The optional forms-of-address feature uses its own `ClanFormsOfAddress` schema migration. Do not hand-edit or delete generated metadata/progs as a substitute for provisioning.

## Bind and activate

Provision the Church first, followed by dioceses/orders, then provinces and houses. Use actual live clan IDs and, for a selected canonical superior, an actual office ID:

```text
prog execute CatholicClanActivate self mychurch
clan create staged catholicdiocese mydiocese "Diocese of the Setting"
prog execute CatholicClanBind self mydiocese "church <ChurchID>"
prog execute CatholicClanStatus self mydiocese
prog execute CatholicClanActivate self mydiocese
```

Other templates require the roles shown by `status`: `diocese`, `order`, `province`, `mother_abbey`, `mother_authority` or `canonical_superior`. For the last role:

```text
prog execute CatholicClanBind self myabbey "canonical_superior <liveClanID> <officeID>"
```

For a custom canonical authority without its own provisioning profile, append its Church ID explicitly in the quoted bind arguments: `"canonical_superior <liveClanID> <officeID> <ChurchID>"`. This records a world authoring fact rather than inferring affiliation from a name or location. Activation retains the resolved authority office IDs in the exported binding record, including fixed offices such as the Pope or provincial superior; an explicitly supplied office must match that role's semantic office.

The chosen jurisdiction is a world input. A nearby bishop or shared order name does not establish authority. Church identity must agree across parish/diocese and religious order/province/house networks. Template authorities, missing offices, self-confirmation and cyclic dependencies are rejected. A correctly bound vacant office is valid and leaves results awaiting its holder.

Activation validates mapped objects and relationships, binds exact confirming offices, installs declared external reservations only for nonelected seats, and generates/compiles deterministic progs. Only then does the native template-to-live setter start elections. No additional scheduler is used. Review world creation-hook effects and deliberately establish intended memberships/ranks before initial appointments; do not invent incumbents to suppress vacancies.

This tool handles initial staged setup only. It refuses activated records or ballot history. Do not bypass provisioning by toggling `clan set template`: an unactivated record still fails eligibility, but native live timers can start.

## Appointment and standing preflight

Before ordinary, confirmer-direct or external appointment:

```text
prog execute CatholicClanPreflight self <clan> "<office-key> <characterID>"
```

Preflight requires a living primary character, existing active local membership, already-sufficient local rank/profession, active membership of the bound Church, and any additional Church standing or qualifying office. Camerlengo requires Cardinal; Metropolitan requires the local Ordinary and Church Bishop standing; most named clerical religious superiors require Church Priest standing. A non-ordained abbess remains non-ordained in the Church.

On success use the authorised native route: `clan appoint` locally or `clan vassal appoint` externally. The latter records the Appointees ledger; adding a local office alone does not establish liege removal protection. Native automatic induction/promotion through external appointment is not an ordination/profession ceremony. The preflight requires standing already to exist.

Nomination progs enforce complete candidate conditions. Voting uses the electorate conditions without clerical candidate restrictions, with one vote per eligible primary identity. Generated progs resolve mapped rank objects using current runtime ordinals, not the catalogue's seed numbers, and recheck live binding/configuration identities on every evaluation. Raw administrative appointment commands are not intercepted.

Before changing required rank, profession, Cardinal/Canon status, Church membership or authority, resolve the affected candidature/result. Player withdrawal exists only during Nomination. A pending confirmed slate can be vetoed through the native election command. Administrators must settle unconfirmed ballots such as the papacy before changing qualifying standing. No late-withdraw player command or unattended final-install qualification recheck is supplied. Native below-minimum elected promotion remains an engine fallback, not proof of Catholic qualification.

For later governance changes, resolve affected ballots through supported native operations, reconcile external appointees/reservations, then deliberately review the retained mapping and regenerate affected progs through a content maintenance change. The initial bind/activate progs do not overwrite live governance or erase history. Stale bindings make generated predicates fail closed. This initial tool does not automate live rebinding or arbitrary new eligibility policies.

## Alternative profiles

Apply before activation with `prog execute CatholicClanVariant self <clan> "<key>"`. `CatholicClanStatus` lists applicable keys; repeated selection is rejected. These are deliberate overrides, not additional stock templates:

| Key | Effect |
| --- | --- |
| `historical_minor_orders` | Adds minor cleric/subdeacon standing; lowers Church Cardinal minimum to minor cleric. |
| `papacy_manual_conclave` | Disables native papal election; staff record conclave, acceptance and required ordination before installation. |
| `diocesan_chapter_election` | Replaces the Ordinary reservation with an indefinite chapter election confirmed by the actual Pope; other episcopal reservations remain. |
| `religious_confirmed_intervention` | Grants the external confirmer both direct appointment and dismissal for elected superiors. |
| `female_monastic_house` | Retitles the relevant house roles and replaces superior priesthood qualification with Church membership. |
| `nonclerical_religious_superior` | Retains profession but replaces superior priesthood qualification with Church membership. |
| `voluntary_immediate_episcopal_release` | Enables immediate resignation for ecclesial offices. |
| `religious_recall_game_variant` | Enables elected religious-office recall with separate professed-member initiation/voting progs. |

Core recall remains disabled. The variant uses strictly more than half the weighted votes cast, without quorum: no votes and exactly half fail. Native recall requires its configured confirmer and every applicable liege approval. Never add reservations to elected seats merely to exercise recall.

## Governance abstractions

The papacy uses secret, indefinite plurality voting by active Cardinals, with Bishop-ranked Cardinals as candidates and the native earlier-JoinDate tie break. It is not a two-thirds conclave; the dormant 0.5 recall threshold has no ordinary-election meaning. Cardinal grants no universal personnel/treasury authority. Camerlengo does not automatically become acting Pope.

The Dean is elected indefinitely by Cathedral Canons, requires Priest standing and is confirmed by the exact local Ordinary. That immediate parent may remove the Dean; confirmation does not permit direct replacement. Religious superiors are confirm-only by default. Four-seat religious councils are advisory: they do not implement consent/veto or gate voting. Worlds may reduce the positive cap before activation.

Religious elections use active professed members of that specific institution, a compact-game franchise. Indefinite terms are gameplay choices, not universal historical claims. Franciscan Guardians, Augustinian local Priors and dependent Benedictine Priors use external appointment; Dominican/Carmelite local Priors use provincial confirmation. Cistercian/Carthusian central elections and Premonstratensian central confirmation are explicit simplifications. Separately cloned mother houses and central offices do not synchronise automatically. Metropolitan dignity creates no automatic suffragan authority.

Pope and religious/support offices use the specified immediate-resignation convenience. Ecclesial offices with resignation disabled use an authored request/acceptance followed by a distinct authorised remover; there is no native request queue. Self-dismissal protection, external appointee provenance, exact-office confirmation, parent dismissal and supported office deletion remain the native mechanisms described in [clan elections and external control](../Economy/Clan_Elections_and_External_Control.md).

## Courtesy styles and ordination

The templates include optional direct forms of address through the generic [clan address system](../Economy/Clan_Forms_of_Address_Design.md). Clergy and selected offices supply editable English styles such as Father, Your Excellency, Your Eminence and Your Holiness. Religious profession stays independent of ordination: a male religious member can be addressed as Brother or Father according to their current rank in the explicitly bound Church. Female-house variants adjust the corresponding rules to Sister and selected Mother styles. These are configurable gameplay conventions, not universal historical protocol.

Cloning copies all ordered rules and office address priorities with independent rule IDs. Bound address predicates are regenerated for the new clone; they follow that clone's Church binding. No Catholic-specific engine functions or commands are added.

Administrators can inspect and customise a deployed clan:

```text
clan edit myorder
clan set rank "Professed Religious" address list
clan set appointment "Abbot" address list
clan set addresslook
clan address myorder none <subject>
editstaticstring ClanFormOfAddressLookFormat
```

Normal NPC progs use `ClanFormOfAddress(speaker, subject, clan)`, or the rank/appointment-specific alternatives. A null speaker tries public outsider rules first and then public lower-rank rules. The shared-clan full-look hint requires the MUD-wide `ClanSimilaritiesShowInLook` configuration and the clan's own look switch. `{address}`, `{clan}`, `{subjective}`, `{objective}` and `{possessive}` are supported in the MUD-wide static format string.

A rerun performs a one-time additive address upgrade for older stock-owned template profiles. It preserves existing custom address rows, titles and governance; later reruns do not restore builder-cleared rules. Live clones are left for deliberate builder editing or fresh deployment.
