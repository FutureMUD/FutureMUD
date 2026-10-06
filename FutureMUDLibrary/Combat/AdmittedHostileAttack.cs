#nullable enable
using System;
using MudSharp.Character;
using MudSharp.Framework;

namespace MudSharp.Combat;

/// <summary>An execution-local admitted attack, not a queued intention or persisted grant.</summary>
public sealed record AdmittedHostileAttack(IPerceiver Attacker, ICharacter Recipient, Guid OperationIdentity);
