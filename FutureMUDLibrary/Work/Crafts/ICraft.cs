using System;
using System.Collections.Generic;
using MudSharp.Accounts;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;
using MudSharp.GameItems.Interfaces;
using MudSharp.RPG.Checks;

namespace MudSharp.Work.Crafts
{
	public interface ICraft : IEditableRevisableItem
	{
		string Blurb { get; }
		/// <summary>
		/// e.g. crafting a sword
		/// </summary>
		string ActionDescription { get; }
		string Category { get; }
		string ActiveCraftItemSDesc { get; }
		IEnumerable<ICraftTool> Tools { get; }
		IEnumerable<ICraftInput> Inputs { get; }
		IEnumerable<ICraftProduct> Products { get; }
		IEnumerable<ICraftProduct> FailProducts { get; }
		Outcome FailThreshold { get; }
		ITraitDefinition CheckTrait { get; }
		Difficulty CheckDifficulty { get; }
		bool IsPracticalCheck { get; }
		IEnumerable<string> PhaseEchoes { get; }
		IEnumerable<TimeSpan> PhaseLengths { get; }
		ITraitExpression QualityFormula { get; }
		IFutureProg AppearInCraftsListProg { get; }
		IFutureProg CanUseProg { get; }
		IFutureProg WhyCannotUseProg { get; }
		IFutureProg OnUseProgStart { get; }
		IFutureProg OnUseProgComplete { get; }
		IFutureProg OnUseProgCancel { get; }
		bool AppearInCraftsList(ICharacter actor);
		(bool Success, string Error) CanDoCraft(ICharacter character, IActiveCraftGameItemComponent component, bool allowStartOnly, bool ignoreToolAndMaterialFailure);
		string GetMaterialPreview(ICharacter character);
		void BeginCraft(ICharacter character);
		void PauseCraft(ICharacter character, IActiveCraftGameItemComponent component, IActiveCraftEffect effect);
		void CancelCraft(ICharacter character, IActiveCraftGameItemComponent component);
		(bool Success, string Error) CanResumeCraft(ICharacter character, IActiveCraftGameItemComponent active);
		void ResumeCraft(ICharacter character, IActiveCraftGameItemComponent active);
		string DisplayCraft(ICharacter character);
		bool HandleCraftPhase(ICharacter character, IActiveCraftEffect effect, IActiveCraftGameItemComponent component, int phase);
		TimeSpan DurationForPhase(int phase);
		int LastPhase { get; }
		void CalculateCraftIsValid();
		ICraft Clone(IAccount originator, string newName);
		bool CraftIsValid { get; }
	}
}
