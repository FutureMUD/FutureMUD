#nullable enable

using MudSharp.Body;
using MudSharp.Database;
using MudSharp.Effects.Interfaces;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using System.Linq;

namespace MudSharp.Character;

public partial class Character
{
	private bool HasPhysicalReferenceToRetiredBody(long bodyId, IGameItem? excludingReference)
	{
		return Gameworld.Items
		                .Where(x => !ReferenceEquals(x, excludingReference))
		                .Where(x => !x.Deleted)
		                .Select(x => x.GetItemType<IButcherable>())
		                .Where(x => x is not null)
		                .Any(x => x!.OriginalBodyId == bodyId);
	}

	private bool HasLiveRuntimeReferenceToRetiredBody(IBody body)
	{
		if (body.Id == 0 || CurrentBody.Id == body.Id)
		{
			return true;
		}

		if (_forms.Any(x => x.Body.Id == body.Id) ||
		    _formSources.Any(x => x.Body.Id == body.Id))
		{
			return true;
		}

		return EffectsOfType<IBodyBackupEffect>()
		       .Any(x => x.BackupBodyId == body.Id);
	}

	private bool DeleteRetiredBodyDatabaseState(IBody body)
	{
		using (new FMDB())
		{
			var dbCharacter = FMDB.Context.Characters.Find(Id);
			if (dbCharacter is null)
			{
				return false;
			}

			if (dbCharacter.BodyId == body.Id)
			{
				if (CurrentBody.Id == body.Id)
				{
					return false;
				}

				dbCharacter.BodyId = CurrentBody.Id;
			}

			if (FMDB.Context.Characters.Any(x => x.Id != Id && x.BodyId == body.Id))
			{
				return false;
			}

			var formRows = FMDB.Context.CharacterBodies
			                      .Where(x => x.BodyId == body.Id)
			                      .ToList();
			if (formRows.Any(x => x.CharacterId != Id))
			{
				return false;
			}

			var sourceRows = FMDB.Context.CharacterBodySources
			                        .Where(x => x.BodyId == body.Id)
			                        .ToList();
			if (sourceRows.Any(x => x.CharacterId != Id))
			{
				return false;
			}

			FMDB.Context.CharacterBodies.RemoveRange(formRows);
			FMDB.Context.CharacterBodySources.RemoveRange(sourceRows);

			var dbBody = FMDB.Context.Bodies.Find(body.Id);
			if (dbBody is null)
			{
				return false;
			}

			FMDB.Context.Bodies.Remove(dbBody);
			FMDB.Context.SaveChanges();
			return true;
		}
	}

	public bool TryCleanupRetiredBody(IBody body, IGameItem? excludingReference = null)
	{
		if (body is null ||
		    HasLiveRuntimeReferenceToRetiredBody(body) ||
		    HasPhysicalReferenceToRetiredBody(body.Id, excludingReference))
		{
			return false;
		}

		foreach (var item in body.AllItems.ToList())
		{
			if (!item.Deleted)
			{
				item.Delete();
			}
		}

		Gameworld.SaveManager.Abort(body);
		if (!DeleteRetiredBodyDatabaseState(body))
		{
			return false;
		}

		Gameworld.EffectScheduler.Destroy(body);
		Gameworld.Scheduler.Destroy(body);
		Gameworld.Destroy(body);
		return true;
	}
}
