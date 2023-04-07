using MudSharp.Framework;
using MudSharp.RPG.Checks;

namespace MudSharp.Health {
    public interface IDamageSource {
        IDamage GetDamage(IPerceiver perceiverSource, OpposedOutcome opposedOutcome);
    }
}