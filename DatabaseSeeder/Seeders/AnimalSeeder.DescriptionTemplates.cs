#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder.Seeders;

public partial class AnimalSeeder
{
	private static AnimalDescriptionPack MammalPack(
		string babyShort,
		string juvenileMaleShort,
		string juvenileFemaleShort,
		string adultMaleShort,
		string adultFemaleShort,
		string appearance,
		string feature,
		string behaviour,
		string habitat)
	{
		return BuildPack(
			babyShort,
			babyShort,
			juvenileMaleShort,
			juvenileFemaleShort,
			adultMaleShort,
			adultFemaleShort,
			$"This is a &male {babyShort}. {appearance} Even so, it is clearly too young to fend for itself and stays close to its mother or den.",
			$"This is {juvenileMaleShort}, not yet fully grown. {appearance} {behaviour} It has not yet filled out into the heavier frame of a mature adult.",
			$"This is {juvenileFemaleShort}, not yet fully grown. {appearance} {behaviour} It has not yet filled out into the heavier frame of a mature adult.",
			$"This is &a_an[&age] {adultMaleShort}. {appearance} {feature} {behaviour} It looks well adapted to life in {habitat}.",
			$"This mature {adultMaleShort} is fully grown and carries itself with quiet confidence. {feature} {appearance} It is plainly an animal built for {habitat}.",
			$"This is &a_an[&age] {adultFemaleShort}. {appearance} {feature} {behaviour} It looks well adapted to life in {habitat}.",
			$"This mature {adultFemaleShort} is fully grown and alert. {feature} {appearance} It is plainly an animal built for {habitat}."
		);
	}

	private static AnimalDescriptionPack BirdPack(
		string babyShort,
		string juvenileShort,
		string adultShort,
		string appearance,
		string feature,
		string behaviour,
		string habitat)
	{
		return BuildPack(
			babyShort,
			babyShort,
			juvenileShort,
			juvenileShort,
			adultShort,
			adultShort,
			$"This is a {babyShort}, still downy and ungainly. {appearance} It relies upon older birds for warmth, food and safety.",
			$"This is {juvenileShort}, not yet in full plumage. {appearance} {behaviour} It has begun to grow into the lines of an adult, but is still plainly immature.",
			$"This is {juvenileShort}, not yet in full plumage. {appearance} {behaviour} It has begun to grow into the lines of an adult, but is still plainly immature.",
			$"This is &a_an[&age] {adultShort}. {appearance} {feature} {behaviour} It looks entirely at home in {habitat}.",
			$"This mature {adultShort} looks fully feathered and well seasoned. {feature} {appearance} Every line of its body suggests a life spent in {habitat}.",
			$"This is &a_an[&age] {adultShort}. {appearance} {feature} {behaviour} It looks entirely at home in {habitat}.",
			$"This mature {adultShort} looks fully feathered and well seasoned. {feature} {appearance} Every line of its body suggests a life spent in {habitat}."
		);
	}

	private static AnimalDescriptionPack SerpentPack(
		string babyShort,
		string juvenileShort,
		string adultShort,
		string appearance,
		string feature,
		string behaviour,
		string habitat)
	{
		return BuildPack(
			babyShort,
			babyShort,
			juvenileShort,
			juvenileShort,
			adultShort,
			adultShort,
			$"This is a {babyShort}, little more than a hatchling. {appearance} It looks delicate, but there is already something watchful and cold-blooded in its stillness.",
			$"This is {juvenileShort}, not yet at full size. {appearance} {behaviour} It is leaner and smaller than a mature adult, but already every bit a serpent.",
			$"This is {juvenileShort}, not yet at full size. {appearance} {behaviour} It is leaner and smaller than a mature adult, but already every bit a serpent.",
			$"This is &a_an[&age] {adultShort}. {appearance} {feature} {behaviour} It looks superbly adapted to life in {habitat}.",
			$"This mature {adultShort} coils with calm assurance. {feature} {appearance} It is the sort of reptile that belongs in {habitat}.",
			$"This is &a_an[&age] {adultShort}. {appearance} {feature} {behaviour} It looks superbly adapted to life in {habitat}.",
			$"This mature {adultShort} coils with calm assurance. {feature} {appearance} It is the sort of reptile that belongs in {habitat}."
		);
	}

	private static AnimalDescriptionPack AquaticPack(
		string babyShort,
		string juvenileShort,
		string adultShort,
		string appearance,
		string feature,
		string behaviour,
		string habitat)
	{
		return BuildPack(
			babyShort,
			babyShort,
			juvenileShort,
			juvenileShort,
			adultShort,
			adultShort,
			$"This is a {babyShort}. {appearance} It is still immature and has not yet grown into the full form of its species.",
			$"This is {juvenileShort}, not yet fully mature. {appearance} {behaviour} It already moves like a creature built for {habitat}.",
			$"This is {juvenileShort}, not yet fully mature. {appearance} {behaviour} It already moves like a creature built for {habitat}.",
			$"This is &a_an[&age] {adultShort}. {appearance} {feature} {behaviour} It appears perfectly suited to {habitat}.",
			$"This mature {adultShort} has the self-assured look of a fully grown aquatic animal. {feature} {appearance} It belongs in {habitat}.",
			$"This is &a_an[&age] {adultShort}. {appearance} {feature} {behaviour} It appears perfectly suited to {habitat}.",
			$"This mature {adultShort} has the self-assured look of a fully grown aquatic animal. {feature} {appearance} It belongs in {habitat}."
		);
	}

	private static AnimalDescriptionPack InsectPack(
		string babyShort,
		string juvenileShort,
		string adultShort,
		string appearance,
		string feature,
		string behaviour,
		string habitat)
	{
		return BuildPack(
			babyShort,
			babyShort,
			juvenileShort,
			juvenileShort,
			adultShort,
			adultShort,
			$"This is a {babyShort}. {appearance} Even at this stage it looks more alien than soft or helpless.",
			$"This is {juvenileShort}, not yet fully developed. {appearance} {behaviour} It is still growing toward the hard, complete form of an adult.",
			$"This is {juvenileShort}, not yet fully developed. {appearance} {behaviour} It is still growing toward the hard, complete form of an adult.",
			$"This is &a_an[&age] {adultShort}. {appearance} {feature} {behaviour} It is unmistakably a creature of {habitat}.",
			$"This mature {adultShort} looks efficient, hard and purposeful. {feature} {appearance} Its form is entirely suited to {habitat}.",
			$"This is &a_an[&age] {adultShort}. {appearance} {feature} {behaviour} It is unmistakably a creature of {habitat}.",
			$"This mature {adultShort} looks efficient, hard and purposeful. {feature} {appearance} Its form is entirely suited to {habitat}."
		);
	}

	private static AnimalDescriptionPack ReptilePack(
		string babyShort,
		string juvenileShort,
		string adultShort,
		string appearance,
		string feature,
		string behaviour,
		string habitat)
	{
		return BuildPack(
			babyShort,
			babyShort,
			juvenileShort,
			juvenileShort,
			adultShort,
			adultShort,
			$"This is a {babyShort}. {appearance} It is young, small and still growing into the tougher lines of its adult form.",
			$"This is {juvenileShort}, not yet full grown. {appearance} {behaviour} It already has the patient stillness of a cold-blooded hunter.",
			$"This is {juvenileShort}, not yet full grown. {appearance} {behaviour} It already has the patient stillness of a cold-blooded hunter.",
			$"This is &a_an[&age] {adultShort}. {appearance} {feature} {behaviour} It seems entirely at home in {habitat}.",
			$"This mature {adultShort} is thick with muscle and scales. {feature} {appearance} It is plainly a creature shaped by {habitat}.",
			$"This is &a_an[&age] {adultShort}. {appearance} {feature} {behaviour} It seems entirely at home in {habitat}.",
			$"This mature {adultShort} is thick with muscle and scales. {feature} {appearance} It is plainly a creature shaped by {habitat}."
		);
	}

	private static AnimalDescriptionPack BuildPack(
		string babyMaleShort,
		string babyFemaleShort,
		string juvenileMaleShort,
		string juvenileFemaleShort,
		string adultMaleShort,
		string adultFemaleShort,
		string babyMaleFull,
		string juvenileMaleFull,
		string juvenileFemaleFull,
		string adultMalePrimaryFull,
		string adultMaleSecondaryFull,
		string adultFemalePrimaryFull,
		string adultFemaleSecondaryFull)
	{
		return new AnimalDescriptionPack(
			[new AnimalDescriptionVariant(babyMaleShort, babyMaleFull)],
			[new AnimalDescriptionVariant(babyFemaleShort, babyMaleFull)],
			[new AnimalDescriptionVariant(juvenileMaleShort, juvenileMaleFull)],
			[new AnimalDescriptionVariant(juvenileFemaleShort, juvenileFemaleFull)],
			[
				new AnimalDescriptionVariant(adultMaleShort, adultMalePrimaryFull),
				new AnimalDescriptionVariant(adultMaleShort, adultMaleSecondaryFull)
			],
			[
				new AnimalDescriptionVariant(adultFemaleShort, adultFemalePrimaryFull),
				new AnimalDescriptionVariant(adultFemaleShort, adultFemaleSecondaryFull)
			]
		);
	}
}
