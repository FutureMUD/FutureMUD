#nullable enable

using MudSharp.Character;
using MudSharp.Magic;

namespace MudSharp.Effects.Interfaces;

public interface IMindContactConcealmentEffect : IEffectSubtype
{
	string UnknownIdentityDescription { get; }
	int AuditDifficultyStages { get; }
	bool ConcealsIdentityFrom(ICharacter source, ICharacter observer, IMagicSchool school);
}
