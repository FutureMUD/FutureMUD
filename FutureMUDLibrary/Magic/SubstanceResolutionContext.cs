using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.RPG.Checks;

#nullable enable
namespace MudSharp.Magic;

public sealed record SubstanceResolutionContext(IPerceivable Target, IMagicalSubstance Substance,
	double Dose, ICharacter? ResponsibleActor = null, OpposedOutcomeDegree Outcome = OpposedOutcomeDegree.Marginal);
