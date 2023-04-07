using System;
using MudSharp.Framework;

namespace MudSharp.Character.Name;

public static class PersonalNameExtensions
{
	public static string DefaultChargenBlurb(this NameUsage usage)
	{
		switch (usage)
		{
			case NameUsage.BirthName:
				return
					"A birth name is generally given to an individual by their parents at their birth. It will usually be the primary name by which they are addressed in most informal or semi-formal contexts. In some cultures, it's common for a child's birth name to be the same as their same-gender parent.";
			case NameUsage.Dimunative:
				return
					"A dimunative is an alternative form of a person's given name that is used by those very close to the individual in private contexts. Depending on the culture this may be more or less appropriate to use in non-family informal contexts.";
			case NameUsage.Nickname:
				return
					"A nickname is an additional name given to someone by their peers or enemies that happens to stick around. Sometimes these may be flattering or they may be pejorative. In some cases an individual may even wear the pejorative nickname as a kind of badge of pride.";
			case NameUsage.MiddleName:
				return
					"Middle names are used in some cultures to differentiate individuals with the same given name and surname. They may also be used to honour relatives, draw parallels to religious figures or just because parents couldn't decide between several names.";
			case NameUsage.AdultName:
				return
					"An adult name is a name given to an individual upon reaching adulthood. This may or may not involve some kind of coming of age ritual which could potentially influence the name that is given to that individual. From that point on, they are typically known by the adult name.";
			case NameUsage.ChildName:
				return
					"A child name is a name given to a child upon reaching a certain age (possibly after surviving an infant mortality period), but is typically only used until they become an adult at which point they are given a new name.";
			case NameUsage.Surname:
				return
					"A surname is a name inherited from one or sometimes both of the child's parents and is also known as a family name or sometimes a last name. Most typically inherited verbatim from the father, a surname serves the function of identifying related individuals in society.";
			case NameUsage.Patronym:
				return
					"A patronym is a name that is derived from the name of a person's father. There are typically cultural rules about how these names should be formed and they may differ between male and female children of the same father.";
			case NameUsage.Matronym:
				return
					"A matronym is a name that is derived from the name of a person's mother. There are typically cultural rules about how these names should be formed and they may differ between male and female children of the same mother.";
			case NameUsage.FamilyGroupName:
				return
					"A family group name is a name that is shared amongst a group of related families. For example, it may indicate membership of a tribe or common descent from some revered ancestor.";
			case NameUsage.GenerationName:
				return
					"A generation name is a name that is shared amongst all offspring of a particular generation in a family, such as all offspring of a particular marriage.";
			case NameUsage.RegnalName:
				return
					"A regnal name is an alternative name adopted by a ruler upon coronation. Typically they are thereafter known by their regnal name in all contexts.";
			case NameUsage.SacredName:
				return
					"A sacred name is a name given to an individual that is only used in certain sacred contexts such as during religious rituals or perhaps when in a particular holy or religious site.";
			case NameUsage.Toponym:
				return
					"A toponym is a type of name that refers to the place where a person originates from. Different cultures have different rules about how to form the toponym.";
			case NameUsage.OwnerName:
				return
					"An owner name is a name that represents your owner. It may be their actual name or surname, or it might be some related word. Either way, it marks you as owned by someone else.";
		}

		throw new ArgumentOutOfRangeException("Unknown NameUsage type " + usage.DescribeEnum());
	}
}