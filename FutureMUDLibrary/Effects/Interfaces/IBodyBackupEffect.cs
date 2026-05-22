#nullable enable

using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.GameItems.Interfaces;

namespace MudSharp.Effects.Interfaces;

public interface IBodyBackupEffect : IEffectSubtype
{
	long BackupBodyId { get; }
	long DestinationCellId { get; }
	ICell? DestinationCell { get; }
	RoomLayer DestinationLayer { get; }
	int Priority { get; }
	BodyRemainsContext RemainsContext { get; }
	bool ConsumeOnUse { get; }
	string SourceDescription { get; }
	string OldLocationEcho { get; }
	string NewLocationEcho { get; }
	string SelfEcho { get; }
	void ConsumeBackup(ICharacter character);
}
