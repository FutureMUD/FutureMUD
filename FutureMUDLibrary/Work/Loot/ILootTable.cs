#nullable enable

using MudSharp.Framework.Revision;

namespace MudSharp.Work.Loot;

public interface ILootTable : IEditableRevisableItem
{
	int AlgorithmVersion { get; }
	LootTableDefinition Definition { get; }
	string DefinitionHash { get; }
}
