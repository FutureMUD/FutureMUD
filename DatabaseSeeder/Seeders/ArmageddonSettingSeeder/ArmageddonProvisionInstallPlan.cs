#nullable enable
using System.Collections.Generic;

namespace DatabaseSeeder.Seeders;

public sealed record ArmageddonFoodPrototype(long Id, int Revision);
public sealed record ArmageddonFoodProfile(int Order, long Predicate, IReadOnlyList<ArmageddonFoodPrototype> Foods);
public sealed record ArmageddonWineRecipe(int Order, long Predicate, long Liquid);

/// <summary>Existing native content selections only; no identities, nutrition or player state are invented.</summary>
public sealed record ArmageddonProvisionInstallPlan(bool Install, long School, long Resource, long AlwaysFalseProg,
	long MealSkill, long WineSkill, IReadOnlyList<ArmageddonFoodProfile> FoodProfiles,
	long Wine, IReadOnlyList<ArmageddonWineRecipe> WineRecipes, long? WineBonusPlane = null);
