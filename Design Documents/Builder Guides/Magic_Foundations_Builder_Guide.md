---
slug: magic-foundations
category: Magic
summary: Build a school of magic, grant access, fund and target a spell, and sustain a power through the lamplighters' Kindle and Read the Weave abilities.
---

# Magic Foundations

A lamplighter leaves a pool of light in a street and walks away. Later, an inspector opens her magical senses and recognises the enchantment as she passes through. Both belong to the same school. The light has a location and an expiry time. The sense belongs to the inspector and draws on her reserves for as long as she maintains it. Those differences tell you what to build.

Start with [powers and spells](#when-to-build-a-power-or-a-spell) and [the anatomy of a spell](#the-anatomy-of-a-spell), or go straight to [building the lamplighters’ school](#building-the-lamplighters-school).

## What makes someone a magician?

| Builder object | Decision it represents | What changes in play |
| --- | --- | --- |
| School | What family does this magic belong to? Choose a name, a player command verb, an adjective and a colour. | The school verb gives practitioners a place to find and use both powers and spells. Choose a distinctive single word that players can type. |
| Capability | Who has access, and on what terms? A `skilllevel` capability belongs to one school and specifies a concentration trait, regenerators and rules for inherent powers. | A single capability can admit an apprentice and unlock additional powers as a trait improves. A character can hold several capabilities. |
| Grant | How does a particular person acquire the capability? A character merit of type `Magic Capability` can grant one or more capabilities. | The school becomes available to its practitioner. A competency in the character's skill list alone does not confer membership. |
| Trait and check | What quality is being measured? The skill or attribute supplies a value. The check, its difficulty and the required outcome decide what that value achieves on this attempt. | Meeting a skill threshold can unlock an ability today while its activation can still fail tonight. Casting, resistance and power activation can use different checks. |
| Knowledge | Which spells has this practitioner learned? In an ordinary skill-level school each spell has a Boolean known-spell prog, accepting a character or a character and spell. | An eligible practitioner sees ready, known castable spells under the school verb. A school can teach its first spell to every member and use a quest or lesson to unlock the next. |

Separating the grant, the unlock and the roll gives each a clear purpose. Joining the guild can confer membership. Reaching a skill value can qualify someone for an inspector's technique. Neither event guarantees success when they put the technique to use. See the [skills and checks guide](Skills_Checks_and_Progression_Builder_Guide.md) for choosing traits, difficulties and outcomes.

### Who pays, how much, and when?

A **resource** is a named balance on a holder. Define its capacity, then decide whether characters should start with it and how much they receive. Starting amount, current balance and maximum are three different numbers. A newly created simple resource has a zero cap. Giving it an impressive name and attaching a generator will still leave it unable to accumulate anything until you set its cap.

A **regenerator** changes the balance over time. A linear regenerator can give each holder a fixed amount per minute. A state regenerator can vary the yield with conditions. A capability can attach a regenerator to its practitioners. This also provides a route for a character who joins the school after character creation to acquire the pool and begin recharging it. For reserves that belong to land, see [environmental magic](Environmental_Magic_Builder_Guide.md).

For a spell, a cost expression answers “how much does this cast spend?” For a sustained power, distinguish the cost of invoking a verb from upkeep per minute. **Concentration** expresses how much magical attention a character is using. It is separate from resource expenditure. A timed enchantment left in the world and a perception mode occupying the caster's attention can therefore feel quite different even when both draw on the same resource.

## When to build a power or a spell

| Choose | When it fits | How you author and grant it |
| --- | --- | --- |
| **Spell** | You can describe the ability as an invocation that finds a recipient, resolves a cast and applies one or more effects. The effects can be instantaneous or last independently afterwards. Examples include lighting a room, enchanting an item or protecting a party. | Choose a trigger, compose target and caster effects, then set costs, checks, descriptions and duration. For the ordinary casting route, pair school access with the spell's knowledge rule. Players use `<schoolverb> cast`. |
| **Power** | An existing power type already implements the interaction you want, such as entering and leaving a perceptual mode, connecting two minds, or sensing something over a distance. Its own commands and continuing behaviour are part of what the player learns. | Choose the type, configure the verbs and options it offers, and grant the power through a capability's trait rule. Each type supplies its own behaviour and activation check. Players invoke its verb directly under the school. |

Duration alone is a poor way to choose. A spell can leave a lasting effect and a power can perform a single action. Ask what the player does **after** the first command. If they leave a light behind to expire on its own, a room-targeted spell expresses the whole action. If they keep a sense open, examine several things, pay upkeep and deliberately close the sense, a sustained power already owns that sequence. Mixing the two gives the school one vocabulary and one economy while preserving both interactions.

## The anatomy of a spell

### 1. The trigger determines the invocation and its target

A trigger is the rule that starts this spell and supplies its casting context. Ordinary casting triggers parse the player's chosen strength and any remaining arguments. Each has its own permitted power range, target selection and options. Here **spell power** means a strength such as `Weak` or `Standard`. It is distinct from a named ability authored in the power editor.

| Trigger | What it supplies | A good reason to choose it |
| --- | --- | --- |
| `self` | The caster as the primary character target. | A personal charm whose target-side effect always belongs to its user. |
| `room` | The room the caster occupies. The player supplies strength and need not name anything. | Kindle should keep illuminating the street when its caster leaves. |
| `character` or `item` | A character or item selected according to that trigger's rules. A filter prog can narrow eligibility. | A charm intended for a person, or a glow meant to travel with a particular object. |
| `party` | A group of character recipients, resolved member by member. | A blessing cast once over companions. A ward or resistance can spare one member while the others are affected. |
| `characterexit` | A primary character and a named extra parameter, `exit`, for effects that consume it. | An effect needs to know both **who** it acts on and **which way** it acts. The exit is additional context, not a second character target. |

Distinguish three conditions. The **known prog** decides who has learned the spell. The **trigger** decides how a particular invocation finds legal targets. A **resistance check** lets a legal target contest an otherwise successful cast. For example, a character trigger's `filterprog` can admit only guild members. Its Boolean prog takes the candidate target, optionally followed by the caster. Someone excluded by that rule never gets as far as a resistance roll. Some triggers are invoked by other systems, such as a spell attached to a hit or a substance. Use `magic spell triggers` and `magic spell triggerhelp <type>` to inspect the supported contracts.

### 2. Decide what happens to each recipient

The spell editor keeps two separately numbered lists. **Target effects** (`effect`) apply to the recipient supplied by the trigger. **Caster effects** (`castereffect`) apply to the person who cast the spell. Choose a template that can act on the actual recipient. `roomlight` needs a room. `glow` can attach to a character or item. Use `magic spell effects` and `magic spell effecthelp <type>` to inspect templates and target compatibility.

A **self trigger** sends the caster down the ordinary target path. A **caster effect** gives the caster a consequence alongside the spell's primary result. Changing Kindle's trigger to `self` would deprive its room light of a room. Keeping the room trigger and adding a caster-side glow can illuminate the street **and** leave the lamplighter's fingertips shining. For a nonempty party, the caster list runs once after at least one member resolves successfully. It does not run once per companion. If every member is blocked or resists, the caster effects do not run.

### 3. Set the price, the roll and the lifetime

1. **Before commitment**, the engine checks applicable lockouts, resource affordability, material requirements and any required target. A missing component, an unaffordable cost or a required target that cannot be supplied can prevent the cast without charging it. Material plans can require free hands or a held item as well as something to consume.
2. **At commitment**, the cast spends its costs, executes its material plan and applies configured lockouts. The ordinary spell then rolls `CastSpellCheck` with its casting trait and difficulty. Set the minimum passing outcome explicitly. A committed attempt that fails the roll still costs the caster, and runs neither effect list.
3. **On a successful cast**, the trigger's targets enter resolution. Wards can intervene. If the spell has a resistance trait and difficulty, an eligible resisting character compares a `ResistMagicSpellCheck` against the cast. The original cost is paid once for the invocation, including on a partially resisted group cast.
4. **For continuing effects**, give the spell a duration expression. Ordinary spell duration is measured in seconds. Resource costs and duration each refer to a *named trait expression* or its ID. Costs can use the selected `power` and whether the primary target is the caster (`self`). A duration can also reflect the outcome. A fixed value is a good first choice when you want players to learn how long they can rely on an effect.

Write the cast emote to describe the attempt that succeeded at the casting stage. Put target-specific consequences in the target emote or effect description, because a ward or resistance may still intervene. Give a failed roll its own line. A trigger that can return no target also needs a null-target emote for readiness. `magic spell show` reports a building error when required pieces are missing. These editors update live definitions, so decide when to open the known-spell rule to players.

## Building the lamplighters’ school

The guild teaches every apprentice **Kindle**. At 50 in **Lamplore**, an experienced member gains **Read the Weave** and can inspect the work of the evening patrol. A guild member can hold 20 points of **Wicklight** and recovers one point per minute. Kindle costs three points. Opening the sense costs one point, and maintaining it costs two per minute. A quiet walk gives the lamplighter time to recover; keeping the sense open draws the reserve down.

The commands assume a world with the normal skill and check setup and the stock `AlwaysTrue` prog. Use an account with the relevant building and character administration permissions. Enter commands one line at a time. In the few blocks that open a text editor, the final `@` on its own line submits the text. Use a playing character for the apprentice: administrators have access to every capability, so their school list cannot tell you what joining the guild granted.

### Establish the school and its reserve

If your world already has a suitable casting skill with a cap of at least 50, keep its existing settings and substitute its name for Lamplore throughout. For a new guild skill, create a cap of 70. This puts the inspector's threshold within reach while leaving room to improve afterwards.

```text
te new Guild Lamplore Ceiling
te set formula 70
skill edit new Lamplore
skill set group Guildcraft
skill set expression Guild Lamplore Ceiling
magic school edit new Lamplighters lamplight lamplit yellow
magic school show
```

`Lamplighters` is the name other builders select. `lamplight` is what the apprentice types. `lamplit` describes the school's magic. Next create a constant cap prog. The cap slot can be called for different kinds of holder; this constant accepts any parameters and ignores them. Enter the toggle once on the new prog.

```text
prog edit new WicklightCapacity
prog set return number
prog set anyparameters
prog set text
return 20
@
prog edit close
magic resource edit new simple Wicklight Wick
magic resource set cap WicklightCapacity
magic regenerator edit new linear Wicklight WickRecovery
magic regenerator set amount 1
magic capability edit new skilllevel Wickkeeper Lamplighters Lamplore
magic capability set regenerator WickRecovery
magic capability set conpoints 1
magic capability show
```

Leave the new resource's starting rules at their defaults. When a member gains the capability, its generator creates a missing balance at zero and begins filling it. That is how the example reserves Wicklight for inducted members without giving every new character a full pool. The one concentration point is enough to account for one sustained reading. Adding a regenerator to this capability is a toggle; on an existing school, inspect the capability before repeating it.

### Build Kindle

The guild wants the street to stay lit for five minutes whether a master or apprentice casts the spell. First create two reusable trait expressions. Their names make the units clear when someone later inspects the spell.

```text
te new Kindle Wicklight Cost
te set formula 3
te new Kindle Duration Seconds
te set formula 300
magic spell edit new Kindle Lamplighters
magic spell set blurb Leave a warm guild light burning in this room.
magic spell set description
Lift a hand to kindle a stationary light. It burns for five minutes after you leave. Use lamplight cast kindle standard.
@
magic spell set trait Lamplore
magic spell set difficulty Easy
magic spell set threshold MinorPass
magic spell set castemote $0 lift|lifts a palm and a bead of amber light rises from it.
magic spell set failcastemote A spark gutters out in the palm of $0.
magic spell set trigger new room
magic spell set trigger set minpower Weak
magic spell set trigger set maxpower Strong
magic spell set effect add roomlight
magic spell set effect 1 light 10
magic spell set effect 1 desc A bead of {0} amber light hangs overhead.
magic spell set effect 1 colour yellow
magic spell set cost Wicklight Kindle Wicklight Cost
magic spell set duration Kindle Duration Seconds
magic spell show
```

Read the trigger, target effect, cost, duration and casting threshold together. At `Standard` strength (numeric power 5), the room gains **50 lux**. The `{0}` in its addendum comes from the world's light description for the spell's contribution. A bright square and a dark alley both gain the same 50 lux, but the difference is easier to see in the alley. In either place the charge is three Wicklight because the cost expression is constant. The room trigger supplies the location itself, so the player types a strength and can leave out a target name.

Give every ordinary practitioner of this school knowledge of Kindle once its definition is ready:

```text
magic spell set prog AlwaysTrue
magic spell show
```

A new spell starts with `AlwaysFalse`. Setting `AlwaysTrue` opens this school's ordinary knowledge gate; characters outside the school still need a route to it. For a later guild secret, use a Boolean prog that reads the practitioner's lesson or quest progress. The knowledge rule should answer a question without consuming anything: the game also asks it when listing spells and displaying help. [FutureProg builder guide](FutureProg_Builder_Guide.md)

**Optional guild custom: the lamplighter's seal.** If the guild wants a visible sign of who recently kindled a light, add a small caster effect to the selected Kindle spell:

```text
magic spell set castereffect add glow
magic spell set castereffect 1 glow 1
magic spell set castereffect 1 sdesc (with glowing fingertips)
magic spell set castereffect 1 desc A faint amber glow clings to the fingertips.
magic spell show
```

These are two independent recipients on the same successful cast. When the lamplighter enters the next room, their faint personal glow travels with them. The stronger overhead light stays where the spell was cast. Both use Kindle's duration. To keep the basic guild design, omit this custom or remove just the personal glow with `magic spell set castereffect remove 1`. The target list has its own effect 1.

### Build Read the Weave

An inspector wants to look at several rooms and enchanted items on one round. Use the `magicksense` power type. While active, it adds magical perception to the viewer. An ordinary look at a room or item with qualifying magical effects can then describe its auras. The player's commands will be `lamplight readweave` and `lamplight releaseweave`.

```text
magic power edit new magicksense Lamplighters "Read the Weave" Lamplore
magic power set begin readweave
magic power set end releaseweave
magic power set blurb Attend to the auras of nearby enchantments.
magic power set help
Use lamplight readweave to open your senses, then look at the street or examine an item. Use lamplight releaseweave when your inspection is complete. Opening costs one Wicklight and each minute costs two more.
@
magic power set difficulty Normal
magic power set threshold MinorPass
magic power set beginemote $0 pause|pauses to study the light.
magic power set endemote $0 blink|blinks and look|looks away.
magic power set failemote The threads of light slip from focus.
magic power set cost readweave Wicklight 1
magic power set sustaincost Wicklight 2
magic power set concentration 1
magic power set duration 0
magic power show
```

The type uses `MagicTelepathyCheck` with the selected Lamplore trait for activation. This skill roll is separate from the 50-point unlock. `duration 0` allows the reading to continue while sustained. The activation cost is spent when the sense opens successfully. If upkeep can no longer be paid, the sense ends. Leave the release verb free so the inspector can always choose to stop spending Wicklight.

The stock `magicksense` type starts with psionic classification and durable psionic traces enabled. This guild treats its reading as ordinary magic. On the **new** power, switch each setting off once and inspect the result; they are independent toggles. If your setting treats the guild as psychics, keep the defaults. Then add the power to the existing capability:

```text
magic power set psionic
magic power set trace
magic power show
magic capability edit Wickkeeper
magic capability set power "Read the Weave" Lamplore 50
magic capability show
```

Members with Lamplore below 50 can still cast Kindle. At 50 the same capability includes Read the Weave among their inherent powers. Giving the spell a higher casting difficulty would change the chance of kindling a light; it would do nothing to confer or withhold the inspector's sense.

### Induct a lamplighter and use both abilities

Create a merit to carry the capability. Here `Mara` stands for the visible playing character being inducted. The skill award represents her apprenticeship; in a campaign the same skill can come from character creation or lessons.

```text
merit edit new "Magic Capability" Lamplighters Oath
merit set capability Wickkeeper
merit show
skill level Mara Lamplore 30
givemerit Mara Lamplighters Oath
```

As Mara, use `lamplight` to read the reserve and concentration, then `lamplight spells` and `lamplight spell Kindle` to read what she has learned. Once there are at least three points in the reserve, she can light the street:

```text
lamplight cast kindle standard
look
```

The cast requires no lamp item. Someone arriving a moment later can see the overhead light even after Mara leaves. If the street has a light-dependent passage in its authored description, the added lux can also move it into a new light band. To write both versions before the first patrol, use the [room preview example](Time_Skies_and_Weather_Builder_Guide.md#preview-the-ferry-landing-in-different-conditions).

When Mara qualifies as an inspector, ordinary skill progression can take her to 50. To award that qualification directly, staff can use `skill level Mara Lamplore 50`. With some Wicklight in reserve, Mara can now inspect an active Kindle:

```text
lamplight powers
lamplight help "Read the Weave"
lamplight readweave
look
lamplight releaseweave
```

Anyone with ordinary sight can see the light. A successful reading gives Mara the additional aura information for applicable magical effects on what she examines. A plain, unenchanted object has no aura to report. Releasing the sense ends Mara's enhanced perception immediately; Kindle continues until its own five minutes have elapsed.

## When the result surprises you

| What the builder or player sees | Where to look |
| --- | --- |
| The administrator can use `lamplight` but the apprentice cannot. | Inspect the merit on the playing character, the merit's capability list, and the capability's school. The skill and the merit are separate grants. |
| The school works, but Kindle is absent from its spell list. | Read `magic spell show Kindle` for a building error. Check its school, castable trigger and known prog. A missing duration on a continuing effect can keep the spell off the list even when the knowledge prog allows it. |
| The reserve is empty after induction. | A newly attached generator starts a missing pool at zero. Inspect the capability's regenerators, the recovery amount and the resource's cap. A zero cap keeps a pool empty across recovery ticks. |
| Wicklight was spent and the street is dark. | A failed committed cast spends three points. Distinguish the failure emote from a successful cast later blocked by a ward. On a successful light, check the effect list and selected strength. `Insignificant` is numeric zero, which is why Kindle sets a minimum. |
| Read the Weave appears under powers but casting it by name fails. | Use the begin verb shown in power help. The spell command and the power's verb have separate dispatch. If the sense drops mid-round, compare the per-minute upkeep with the reserve and its recovery. |

For a school where characters prepare spells, spend slots or cast from scrolls, continue with [Vancian magic](Vancian_Magic_Builder_Guide.md). For terrain-bound reserves and the consequences of drawing on a place, continue with [environmental magic](Environmental_Magic_Builder_Guide.md).
