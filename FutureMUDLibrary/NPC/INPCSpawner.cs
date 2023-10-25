using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;

namespace MudSharp.NPC
{
	#nullable enable
	public enum SpawnStrategy
	{
		Simple,
		OpenTerritory,
		Multi
	}

	public interface INPCSpawner : ISaveable, IEditableItem
	{
		bool IsActive { get; }
		void CheckSpawn();
		SpawnStrategy SpawnStrategy { get; }
	}
}
