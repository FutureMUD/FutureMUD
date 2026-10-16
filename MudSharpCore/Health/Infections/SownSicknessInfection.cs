#nullable enable

using MudSharp.Body;
using MudSharp.Effects.Concrete;
using MudSharp.RPG.Checks;

namespace MudSharp.Health.Infections;

/// <summary>
/// A builder-authored, spell-contracted infection. It follows native infection progression and treatment,
/// while deliberately disabling both wound-to-wound and character-to-character spread.
/// </summary>
public sealed class SownSicknessInfection : SimpleInfection
{
	private SownSicknessSymptoms? _symptoms;
	public int Grade => GradeFromIntensity(Intensity);

	public static int GradeFromIntensity(double intensity)
	{
		return Math.Clamp((int)Math.Ceiling(double.IsFinite(intensity) ? intensity : 1.0), 1, 7);
	}

	public SownSicknessInfection(Difficulty virulenceDifficulty, double intensity, IBody owner, IWound? wound,
		IBodypart? bodypart, double virulence)
		: base(virulenceDifficulty, intensity, owner, wound!, bodypart!, virulence)
	{
	}

	public SownSicknessInfection(MudSharp.Models.Infection infection, IBody body, IWound? wound,
		IBodypart? bodypart)
		: base(infection, body, wound!, bodypart!)
	{
	}

	public override InfectionType InfectionType => InfectionType.SownSickness;

	protected override void HandleInfectionSideEffects()
	{
		base.HandleInfectionSideEffects();
		if (Owner.Actor is not ICharacter character)
		{
			return;
		}

		if (_symptoms is not null && character.EffectsOfType<SownSicknessSymptoms>().Contains(_symptoms))
		{
			if (_symptoms.Grade == Grade)
			{
				return;
			}

			character.RemoveEffect(_symptoms, true);
		}

		_symptoms = new SownSicknessSymptoms(character, this, Grade);
		character.AddEffect(_symptoms);
	}

	protected override void OnDelete()
	{
		base.OnDelete();
		if (_symptoms is not null && Owner.Actor is ICharacter character)
		{
			character.RemoveEffect(_symptoms, true);
			_symptoms = null;
		}
	}

	public override bool InfectionCanSpread() => false;

	public override void Spread(Outcome outcome)
	{
		// Sown sickness has no spontaneous spread vector. Only its bound spell effect creates it.
	}
}
