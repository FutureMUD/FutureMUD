#nullable enable
namespace MudSharp.Magic.Casting;

public sealed partial class MagicCastingService
{
    private static void ReusePreparedSelections(MagicSpell copy, MagicSpell source,
        ICharacter actor, IEnumerable<IPerceivable> targets)
    {
        if (copy.Id != source.Id || copy.InvocationGrade != source.InvocationGrade ||
            copy.GradeProfile?.Version != source.GradeProfile?.Version)
            throw new InvalidOperationException("Prepared selection spell identity or version changed.");
        Pair(copy.SpellEffects.ToArray(), source.SpellEffects.ToArray(), targets);
        Pair(copy.CasterSpellEffects.ToArray(), source.CasterSpellEffects.ToArray(), [actor]);
        void Pair(IMagicSpellEffectTemplate[] fresh, IMagicSpellEffectTemplate[] original, IEnumerable<IPerceivable> recipients)
        {
            if (fresh.Length != original.Length) throw new InvalidOperationException("Prepared selection effect count changed.");
            for (var index = 0; index < fresh.Length; ++index)
            {
                if (fresh[index].GetType() != original[index].GetType() ||
                    !XNode.DeepEquals(fresh[index].SaveToXml(), original[index].SaveToXml()))
                    throw new InvalidOperationException("Prepared selection effect type, side, index or configuration changed.");
                if (original[index] is not IMagicSpellEffectPreparedSelection prepared) continue;
                foreach (var recipient in recipients)
                {
                    var token = prepared.CapturePreparedSelection(actor, recipient);
                    if (token is null) continue;
                    string? error = null;
                    if (fresh[index] is not IMagicSpellEffectPreparedSelection receiver ||
                        !receiver.TryReusePreparedSelection(token, actor, recipient, out error))
                        throw new InvalidOperationException("Prepared selection admission changed: " + error);
                }
            }
        }
    }
}
