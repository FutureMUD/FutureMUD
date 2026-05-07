# Item Authoring Guidelines

Items in FutureMUD are composed of Item Components that give them functionality. For example, the ability to be worn is given by an IWearable component.

## Core Properties

### Description Properties

- Items have a name which is typically the one-word description of what sort of item it is. You can think of it as the "noun". "A sharp iron longsword" might have a name of "sword" or "longsword". It is used to display the contents of sheathes and group items together in bundles for example.

- Items have a "Short Description" or "SDesc" that is how they are referred to in emotes and echoes. These almost always start with "a" or "an" and are typically 3-6 words long. Some examples could be "a sharp iron longsword", "a steaming-hot meat pie", "an enormous mahogony table", or "a cerulean blue cotton blouse".

- Items have a "Full Description" or "Desc" that is text that is shown when the item is looked at. This is a longer description, usually 3-4 sentences and rarely more than a paragraph that explains what the item looks at in detail. Typically these focus on the shape, colour, style or specific details of the item.

- Items may have a "Long Description" or "LDesc" that overrides the appearance of the item when it is shown as part of the room contents. If none is specified, the default format is typically of the form "<sdesc> is here.". Some examples of how this might be used could be "A series of eggshell-white cupboards are installed along the western wall". These kinds of long descriptions are usually only used when the item is not "holdable" and is designed to be a permanent or semi-permanent fixture of a room.

- If the item has a Variable item component attached to it, all three of the above descriptions can have the value of the variable substituted into them with a markup syntax. This is typically $varname, and sometimes $varnamebasic or $varnamefancy. These variables are usually something like colour but can be used for all sorts of things, such as shapes, designs, motifs, anything really. The details of what the plain form and basic/fancy form mean vary by variable type, but each has an assumed grammatical place. 

- All three of the descriptions can have writing blocks that will be substituted by the viewer depending on their skills. See the [Writing Blocks](#Writing-Blocks) section for details.

- All three of the descriptions can have trait check blocks that will be substitued by the viewer depending on their skills. See the [Trait Checks](#Trait-Checks) section for details.

### Physical Properties

- All items have a weight, which is stored in grams. This is the weight of the item itself; for example a container adds the weight of its content, so its weight property is its own inherent weight.

- All items have a primary material, which is from a defined list of solid materials. Although in reality most real world items would be made from more than one material, the primary material is used as an abstraction. It tends to be used to determine things like buoyancy (used to determine if an item will float or sink), as well as hardness (used in determining damage attribution when two things hit each other) and can be targeted by spells, crafts etc. Sometimes there are generic versions of material and specific versions, like "Wood" as well as "Pine", "Oak" etc. The more specific form should be preferred if it exists, otherwise fall back to the general form.

- All items have a quality. This is the inherent, most common quality of the item and can be impacted after the fact by condition, magic, damage etc. This property represents the quality absent those modifications. See [Quality Values](#quality-values) for a complete list.

- All items have a size. This is an abstraction that gives cues to certain systems in the engine about how big and bulky an item is. Many other things in the world also have sizes or size limits using the same values. See [Size Values](#size-values) for a complete list.

- Items can have an inherent base cost. While this price can be overriden when the item is sold and can also be modified by many factors, this is the default base cost for the item. The specific values are MUD context dependent because 1 "base currency unit" will mean different things for different MUDs. It is not mandatory for an item to have a cost.

### Writing Blocks

Writing blocks let item descriptions include readable text that depends on language, script, and skill.

General form:

```text
writing{language,script,minskill=<value>,style=<style>,colour=<colour>}{readable text}{unreadable text}
```

`skill` and `minskill` are both accepted. `colour` and `color` are both accepted.

Examples:

```text
writing{English,Latin,minskill=30}{The label reads "Staff Only."}{You cannot read the text on the label.}
writing{Aelvish,Runes,skill=60,colour=green}{The lintel names the old gate.}{Green runes mark the lintel.}
```

Which languages and scripts are available will be dependent on the MUD and need to be confirmed.

### Trait Checks

Trait checks let descriptions show different text based on a character trait threshold.

General Form:

```text
check{Skill Name,value}{value if true}{value if false}
```

```text
check{Herbalism,35}{ You know that these berries can be used to make a lethal poison.}{}
check{Foraging,50}{You notice edible leaves growing near the wall.}{The wall is lined with climbing plants.}
```

Which traits/skills are available to test against will be dependent on the MUD and need to be confirmed.

### Quality Values

The complete list of qualities and their associated numeric values are as follows:

- Terrible = 0,
- ExtremelyBad = 1,
- Bad = 2,
- Poor = 3,
- Substandard = 4,
- Standard = 5,
- Good = 6,
- VeryGood = 7,
- Great = 8,
- Excellent = 9,
- Heroic = 10,
- Legendary = 11

### Size Values

The complete list of sizes, their associated numeric values and examples of things that would be that size are as follows:

- Nanoscopic = 0,
- Microscopic = 1,
- Miniscule = 2,
- Tiny = 3,
- VerySmall = 4 -> 
- Small = 5 -> underwear, hats, bags, daggers, shoes, phones
- Normal = 6 -> shirts, dresses, backpacks, swords, shields, computers
- Large = 7,
- VeryLarge = 8,
- Huge = 9,
- Enormous = 10,
- Gigantic = 11,
- Titanic = 12

