using MudSharp.Framework;

namespace MudSharp.Effects.Interfaces {
    public interface ISDescAdditionEffect : IEffectSubtype {
        string AddendumText { get; }
        string GetAddendumText(bool colour);
		bool DescriptionAdditionApplies(IPerceiver voyeur) => Applies(voyeur) && !string.IsNullOrEmpty(GetAddendumText(false));
	}
}