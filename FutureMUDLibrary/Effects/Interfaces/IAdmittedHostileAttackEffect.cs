#nullable enable
using MudSharp.Combat;

namespace MudSharp.Effects.Interfaces;

public interface IAdmittedHostileAttackEffect : IEffectSubtype
{
	void OnAdmittedHostileAttack(AdmittedHostileAttack attack);
}
