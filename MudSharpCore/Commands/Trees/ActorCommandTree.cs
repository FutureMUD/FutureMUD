using System.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Commands.Modules;
using MudSharp.Framework;

namespace MudSharp.Commands.Trees;

internal class ActorCommandTree : ICharacterCommandTree
{
	private PermissionLevel _permissionLevel;

	protected ActorCommandTree()
	{
	}

	public static ActorCommandTree Instance { get; } = new()
	{
		PermissionLevel = PermissionLevel.Player
	};

	public PermissionLevel PermissionLevel
	{
		get => _permissionLevel;
		init
		{
			_permissionLevel = value;
			ProcessCommands();
		}
	}

	public ICharacterCommandManager Commands { get; protected set; }

	#region ICommandTree<ICharacter> Members

	ICommandManager<ICharacter> ICommandTree<ICharacter>.Commands => Commands;

	#endregion

	protected virtual void ProcessCommands()
	{
		Commands = new CharacterCommandManager("", PermissionLevel);
		Commands.AddFrom(MovementModule.Instance.Commands);
		Commands.AddFrom(CharacterInformationModule.Instance.Commands);
		Commands.AddFrom(CommunicationsModule.Instance.Commands);
		Commands.AddFrom(CombatModule.Instance.Commands);
		Commands.AddFrom(GameModule.Instance.Commands);
		Commands.AddFrom(InventoryModule.Instance.Commands);
		Commands.AddFrom(PerceptionModule.Instance.Commands);
		Commands.AddFrom(PositionModule.Instance.Commands);
		Commands.AddFrom(TimeModule.Instance.Commands);
		Commands.AddFrom(ManipulationModule.Instance.Commands);
		Commands.AddFrom(HelpModule.Instance.Commands);
		Commands.AddFrom(EconomyModule.Instance.Commands);
		Commands.AddFrom(PropertyModule.Instance.Commands);
		Commands.AddFrom(ClanModule.Instance.Commands);
		Commands.AddFrom(StealthModule.Instance.Commands);
		Commands.AddFrom(HealthModule.Instance.Commands);
		Commands.AddFrom(ShowModule.Instance.Commands);
		Commands.AddFrom(LiteracyModule.Instance.Commands);
		Commands.AddFrom(CraftModule.Instance.Commands);
		Commands.AddFrom(LegalModule.Instance.Commands);
		Commands.AddFrom(CrimeModule.Instance.Commands);
		Commands.AddFrom(WeatherModule.Instance.Commands);
		Commands.AddFrom(HeritageBuilderModule.Instance.Commands);
		Commands.AddFrom(SharedModule.Instance.Commands);
	}
}