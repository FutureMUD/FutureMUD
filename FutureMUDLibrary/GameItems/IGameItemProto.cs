using System;
using System.Collections.Generic;
using MudSharp.Character;
using MudSharp.Form.Material;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;
using MudSharp.Health;

namespace MudSharp.GameItems {
	public interface IGameItemProto : IEditableRevisableItem, IHaveTags {
		IHealthStrategy HealthStrategy { get; }
		string ShortDescription { get; }
		string FullDescription { get; }
		SizeCategory Size { get; }
		ISolid Material { get; }
		ItemQuality BaseItemQuality { get; }
		double Weight { get; }

		bool HighPriority { get; }
		ANSIColour CustomColour { get; }
		bool PermitPlayerSkins { get; }
		bool IsHiddenFromPlayers { get; }
		string EditHeaderColour(ICharacter voyeur);


		IEnumerable<(IFutureProg Prog, string? ShortDescription, string? FullDescription, string? FullDescriptionAddendum)> ExtraDescriptions { get; }

		IEnumerable<IGameItemComponentProto> Components { get; }
		
		bool OverridesLongDescription { get; }
		string LongDescription { get; }
		IReadOnlyDictionary<string, string> DefaultVariables { get; }
		IGameItemGroup ItemGroup { get; }
		bool Morphs { get; }
		TimeSpan MorphTimeSpan { get; }
		string MorphEmote { get; }
		decimal CostInBaseCurrency { get; }
		IGameItem LoadDestroyedItem(IGameItem originalItem);
		IGameItem LoadMorphedItem(IGameItem originalItem);
		bool IsItemType<T>() where T : IGameItemComponentProto;
		T GetItemType<T>() where T : IGameItemComponentProto;
		IGameItem CreateNew(ICharacter loader = null);

		bool CheckForComponentPrototypeUpdates();
		IGameItemProto Clone(ICharacter builder);
	}
}