using MudSharp.Framework;

namespace MudSharp.Effects
{
    public interface IEffectHandler : IHaveEffects
    {
        IPerceivable Parent { get; }

		/// <summary>Reconciles dependent runtime inputs after an installed effect changes in place.</summary>
		void NotifyEffectUpdated(IEffect effect);
    }
}
