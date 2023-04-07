using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Construction;
using MudSharp.GameItems;

namespace MudSharp.Health;

public class ExplosiveDamage : IExplosiveDamage
{
	public ExplosiveDamage(IEnumerable<IDamage> damages, double elevation, SizeCategory explosionSize,
		Proximity maximumProximity, bool explodingFromInside = false, IGameItem internalExplosionSource = null)
	{
		ReferenceDamages = damages;
		Elevation = elevation;
		ExplosionSize = explosionSize;
		ExplodingFromInside = explodingFromInside;
		InternalExplosionSource = internalExplosionSource;
		MaximumProximity = maximumProximity;
	}

	public ExplosiveDamage(IExplosiveDamage rhs)
	{
		ReferenceDamages = rhs.ReferenceDamages.Select(x => new Damage(x)).ToList();
		Elevation = rhs.Elevation;
		ExplosionSize = rhs.ExplosionSize;
		ExplodingFromInside = rhs.ExplodingFromInside;
		InternalExplosionSource = rhs.InternalExplosionSource;
		MaximumProximity = rhs.MaximumProximity;
	}

	public ExplosiveDamage(IExplosiveDamage rhs, double damageMultiplier)
	{
		ReferenceDamages = rhs.ReferenceDamages.Select(x => new Damage(x, damageMultiplier)).ToList();
		Elevation = rhs.Elevation;
		ExplosionSize = rhs.ExplosionSize;
		ExplodingFromInside = rhs.ExplodingFromInside;
		InternalExplosionSource = rhs.InternalExplosionSource;
		MaximumProximity = rhs.MaximumProximity;
	}

	public ExplosiveDamage(IExplosiveDamage rhs, double damageMultiplier, IGameItem newInternalExplosionSource)
	{
		ReferenceDamages = rhs.ReferenceDamages.Select(x => new Damage(x, damageMultiplier)).ToList();
		Elevation = rhs.Elevation;
		ExplosionSize = rhs.ExplosionSize;
		ExplodingFromInside = newInternalExplosionSource != null;
		InternalExplosionSource = newInternalExplosionSource;
		MaximumProximity = rhs.MaximumProximity;
	}

	public IEnumerable<IDamage> ReferenceDamages { get; }
	public double Elevation { get; }
	public SizeCategory ExplosionSize { get; }
	public bool ExplodingFromInside { get; }
	public IGameItem InternalExplosionSource { get; }
	public Proximity MaximumProximity { get; }
}