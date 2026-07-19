#nullable enable

namespace MudSharp.Character;

internal enum BodyFormDeletionBlocker
{
	None,
	CurrentForm,
	EmbodiedInstance,
	BodyBackup,
	PhysicalReference
}

internal static class BodyFormLifecyclePolicy
{
	public static BodyFormDeletionBlocker GetDeletionBlocker(bool isCurrentForm, bool hasEmbodiedInstance,
		bool hasBodyBackup, bool hasPhysicalReference)
	{
		return GetDeletionBlocker(
			isCurrentForm,
			() => hasEmbodiedInstance,
			() => hasBodyBackup,
			() => hasPhysicalReference);
	}

	public static BodyFormDeletionBlocker GetDeletionBlocker(bool isCurrentForm, Func<bool> hasEmbodiedInstance,
		Func<bool> hasBodyBackup, Func<bool> hasPhysicalReference)
	{
		if (isCurrentForm)
		{
			return BodyFormDeletionBlocker.CurrentForm;
		}

		if (hasEmbodiedInstance())
		{
			return BodyFormDeletionBlocker.EmbodiedInstance;
		}

		if (hasBodyBackup())
		{
			return BodyFormDeletionBlocker.BodyBackup;
		}

		return hasPhysicalReference()
			? BodyFormDeletionBlocker.PhysicalReference
			: BodyFormDeletionBlocker.None;
	}

	public static string GetDeletionFailureMessage(BodyFormDeletionBlocker blocker)
	{
		return blocker switch
		{
			BodyFormDeletionBlocker.CurrentForm => "You cannot delete the current body form.",
			BodyFormDeletionBlocker.EmbodiedInstance => "You cannot delete a form while it has a live embodied instance.",
			BodyFormDeletionBlocker.BodyBackup => "You cannot delete a form while it is referenced by a body backup effect.",
			BodyFormDeletionBlocker.PhysicalReference => "You cannot delete a form while corpses, remains, or other physical references still point at its body.",
			_ => string.Empty
		};
	}

	public static bool ShouldAttachApparentAge(bool alreadyHasMetadata)
	{
		return !alreadyHasMetadata;
	}

	public static double SelectProvisionedDimension(double candidate, double templateValue, double fallback)
	{
		if (double.IsFinite(candidate) && candidate > 0.0)
		{
			return candidate;
		}

		return double.IsFinite(templateValue) && templateValue > 0.0
			? templateValue
			: fallback;
	}
}
