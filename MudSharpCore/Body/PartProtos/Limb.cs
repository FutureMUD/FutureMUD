using System.Collections.Generic;
using System.Linq;
using MudSharp.Database;
using MudSharp.Framework;

namespace MudSharp.Body.PartProtos;

public class Limb : FrameworkItem, ILimb
{
	private readonly List<IBodypart> _parts = new();
	private readonly List<long> _spinalPartIds = new();
	private List<ISpineProto> _spinalBodyparts;

	public IFuturemud Gameworld { get; }

	public Limb(MudSharp.Models.Limb limb, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = limb.Id;
		_name = limb.Name;
		LimbType = (LimbType)limb.LimbType;
		Prototype = gameworld.BodyPrototypes.Get(limb.RootBodyId);
		RootBodypart = gameworld.BodypartPrototypes.Get(limb.RootBodypartId);
		LimbDamageThresholdMultiplier = limb.LimbDamageThresholdMultiplier;
		LimbPainThresholdMultiplier = limb.LimbPainThresholdMultiplier;
		foreach (var part in limb.LimbsBodypartProto)
		{
			_parts.Add(gameworld.BodypartPrototypes.Get(part.BodypartProtoId));
		}

		foreach (var part in limb.LimbsSpinalParts)
		{
			_spinalPartIds.Add(part.BodypartProtoId);
		}
	}

	public void AddBodypart(IBodypart part)
	{
		if (!_parts.Contains(part))
		{
			_parts.Add(part);
			if (part.IdHasBeenRegistered)
			{
				using (new FMDB())
				{
					var dblimb = FMDB.Context.Limbs.Find(Id);
					dblimb.LimbsBodypartProto.Add(FMDB.Context.LimbsBodypartProto.Find(part.Id, Id));
					FMDB.Context.SaveChanges();
				}
			}
		}
	}

	public void RemoveBodypart(IBodypart part)
	{
		if (_parts.Contains(part))
		{
			_parts.Remove(part);
			if (part.IdHasBeenRegistered)
			{
				using (new FMDB())
				{
					var dblimb = FMDB.Context.Limbs.Find(Id);
					dblimb.LimbsBodypartProto.Remove(FMDB.Context.LimbsBodypartProto.Find(part.Id, Id));
					FMDB.Context.SaveChanges();
				}
			}
		}
	}

	public override string FrameworkItemType => "Limb";

	public LimbType LimbType { get; set; }

	public IEnumerable<IBodypart> Parts => _parts;

	public IBodyPrototype Prototype { get; set; }

	public IBodypart RootBodypart { get; set; }

	public double LimbDamageThresholdMultiplier { get; set; }
	public double LimbPainThresholdMultiplier { get; set; }

	public IEnumerable<ISpineProto> SpineProtos
	{
		get
		{
			if (_spinalBodyparts == null)
			{
				_spinalBodyparts = _spinalPartIds.SelectNotNull(x => Gameworld.BodypartPrototypes.Get(x) as ISpineProto)
				                                 .ToList();
			}

			return _spinalBodyparts;
		}
	}

	#region Overrides of Object

	/// <summary>
	///     Returns a string that represents the current object.
	/// </summary>
	/// <returns>
	///     A string that represents the current object.
	/// </returns>
	public override string ToString()
	{
		return $"Limb {Id:N0} - {Name}";
	}

	#endregion
}