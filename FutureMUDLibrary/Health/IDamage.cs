using MudSharp.Body;
using MudSharp.Character;
using MudSharp.GameItems;
using MudSharp.RPG.Checks;

namespace MudSharp.Health
{
    public interface IDamage
    {
		/// <summary>Optional runtime-only physical mutation fence; ordinary damage has none.</summary>
		IDamageMutationGuard MutationGuard => null;
		/// <summary>Optional invocation-local native wound measurement; never persisted.</summary>
		MudSharp.Magic.IMagicVitalTransferDamage VitalTransfer => null;
		/// <summary>The body receiving this injury, which may differ from its controller's active body.</summary>
		IBody TargetBody => null;
		MudSharp.Form.Material.ExposureDamageContext ExposureContext => null;
        DamageType DamageType { get; }
        double DamageAmount { get; }
        double PainAmount { get; }
        double ShockAmount { get; }
        double StunAmount { get; }
        IBodypart Bodypart { get; }
        ICharacter ActorOrigin { get; }
        IGameItem ToolOrigin { get; }
        IGameItem LodgableItem { get; }
        double AngleOfIncidentRadians { get; }
        Outcome PenetrationOutcome { get; }
    }
}
