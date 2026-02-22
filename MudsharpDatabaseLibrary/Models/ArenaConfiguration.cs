using System;
using System.Collections.Generic;

namespace MudSharp.Models;

public class ArenaCombatantClass
{
	public ArenaCombatantClass()
	{
		ArenaEventTypeSideAllowedClasses = new HashSet<ArenaEventTypeSideAllowedClass>();
		ArenaSignups = new HashSet<ArenaSignup>();
		ArenaRatings = new HashSet<ArenaRating>();
	}

	public long Id { get; set; }
	public long ArenaId { get; set; }
	public string Name { get; set; }
	public string Description { get; set; }
	public long EligibilityProgId { get; set; }
	public long? AdminNpcLoaderProgId { get; set; }
	public bool ResurrectNpcOnDeath { get; set; }
	public string DefaultStageNameTemplate { get; set; }
	public string DefaultSignatureColour { get; set; }

	public virtual Arena Arena { get; set; }
	public virtual FutureProg EligibilityProg { get; set; }
	public virtual FutureProg AdminNpcLoaderProg { get; set; }
	public virtual ICollection<ArenaEventTypeSideAllowedClass> ArenaEventTypeSideAllowedClasses { get; set; }
	public virtual ICollection<ArenaSignup> ArenaSignups { get; set; }
	public virtual ICollection<ArenaRating> ArenaRatings { get; set; }
}

public class ArenaEventType
{
	public ArenaEventType()
	{
		ArenaEventTypeSides = new HashSet<ArenaEventTypeSide>();
		ArenaEvents = new HashSet<ArenaEvent>();
	}

	public long Id { get; set; }
	public long ArenaId { get; set; }
	public string Name { get; set; }
	public bool BringYourOwn { get; set; }
	public int RegistrationDurationSeconds { get; set; }
	public int PreparationDurationSeconds { get; set; }
	public int? TimeLimitSeconds { get; set; }
	public int? AutoScheduleIntervalSeconds { get; set; }
	public DateTime? AutoScheduleReferenceTime { get; set; }
	public int BettingModel { get; set; }
	public int EliminationMode { get; set; }
	public bool AllowSurrender { get; set; }
	public decimal AppearanceFee { get; set; }
	public decimal VictoryFee { get; set; }
	public long? IntroProgId { get; set; }
	public long? ScoringProgId { get; set; }
	public long? ResolutionOverrideProgId { get; set; }

	public virtual Arena Arena { get; set; }
	public virtual FutureProg IntroProg { get; set; }
	public virtual FutureProg ScoringProg { get; set; }
	public virtual FutureProg ResolutionOverrideProg { get; set; }
	public virtual ICollection<ArenaEventTypeSide> ArenaEventTypeSides { get; set; }
	public virtual ICollection<ArenaEvent> ArenaEvents { get; set; }
}

public class ArenaEventTypeSide
{
	public ArenaEventTypeSide()
	{
		ArenaEventTypeSideAllowedClasses = new HashSet<ArenaEventTypeSideAllowedClass>();
	}

	public long Id { get; set; }
	public long ArenaEventTypeId { get; set; }
	public int Index { get; set; }
	public int Capacity { get; set; }
	public int Policy { get; set; }
	public bool AllowNpcSignup { get; set; }
	public bool AutoFillNpc { get; set; }
	public long? OutfitProgId { get; set; }
	public long? NpcLoaderProgId { get; set; }

	public virtual ArenaEventType ArenaEventType { get; set; }
	public virtual FutureProg OutfitProg { get; set; }
	public virtual FutureProg NpcLoaderProg { get; set; }
	public virtual ICollection<ArenaEventTypeSideAllowedClass> ArenaEventTypeSideAllowedClasses { get; set; }
}

public class ArenaEventTypeSideAllowedClass
{
	public long ArenaEventTypeSideId { get; set; }
	public long ArenaCombatantClassId { get; set; }

	public virtual ArenaEventTypeSide ArenaEventTypeSide { get; set; }
	public virtual ArenaCombatantClass ArenaCombatantClass { get; set; }
}
