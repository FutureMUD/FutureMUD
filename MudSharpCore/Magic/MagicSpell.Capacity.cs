using System.Runtime.ExceptionServices;
using MudSharp.Body;
using MudSharp.Magic.Casting;

#nullable enable
namespace MudSharp.Magic;

public partial class MagicSpell
{
	/// <summary>Reconciles each canonical owner's capacity once after all target and caster effects resolve.</summary>
	private sealed class SpellCapacityBatch : IDisposable
	{
		private readonly Dictionary<MudSharp.Character.Character, IDisposable> _scopes = [];

		public void Include(IPerceivable target)
		{
			var actor = target as ICharacter ?? (target as IBody)?.Actor;
			if (actor is null || MagicCastingService.Owner(actor) is not MudSharp.Character.Character owner ||
				_scopes.ContainsKey(owner)) return;
			_scopes.Add(owner, owner.DeferCastingCapacityReconciliationForMutation());
		}

		public void Dispose()
		{
			ExceptionDispatchInfo? failure = null;
			foreach (var scope in _scopes.Values)
			{
				try { scope.Dispose(); }
				catch (Exception ex) { failure ??= ExceptionDispatchInfo.Capture(ex); }
			}
			_scopes.Clear();
			failure?.Throw();
		}
	}
}
