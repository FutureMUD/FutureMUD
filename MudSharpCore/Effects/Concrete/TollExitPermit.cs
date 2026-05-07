#nullable enable annotations

using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp.Effects.Concrete;

public class TollExitPermit : Effect
{
	private readonly Dictionary<long, string> _permittedCharacterDescriptions = new();

	public TollExitPermit(ICharacter owner, ICellExit exit, bool moveAside, string resetEmote) : base(owner)
	{
		CharacterOwner = owner;
		ExitId = exit.Exit.Id;
		GuardCellId = exit.Origin.Id;
		MoveAside = moveAside;
		ResetEmote = resetEmote;
	}

	protected TollExitPermit(XElement effect, IPerceivable owner) : base(effect, owner)
	{
		CharacterOwner = (ICharacter)owner;
		var root = effect.Element("Element");
		ExitId = long.Parse(root.Element("ExitId")?.Value ?? "0");
		GuardCellId = long.Parse(root.Element("GuardCellId")?.Value ?? "0");
		MoveAside = bool.Parse(root.Element("MoveAside")?.Value ?? "false");
		ResetEmote = root.Element("ResetEmote")?.Value ?? string.Empty;

		foreach (var item in root.Element("Permitted")?.Elements("Character") ?? Enumerable.Empty<XElement>())
		{
			_permittedCharacterDescriptions[long.Parse(item.Attribute("id")?.Value ?? "0")] = item.Value;
		}
	}

	public ICharacter CharacterOwner { get; }
	public long ExitId { get; }
	public long GuardCellId { get; }
	public bool MoveAside { get; }
	public string ResetEmote { get; }
	public IEnumerable<long> PermittedCharacterIds => _permittedCharacterDescriptions.Keys;

	public void AddPermittedCharacter(ICharacter character)
	{
		_permittedCharacterDescriptions[character.Id] = character.HowSeen(CharacterOwner, colour: false);
		Changed = true;
	}

	public void AddPermittedCharacters(IEnumerable<ICharacter> characters)
	{
		foreach (var character in characters)
		{
			AddPermittedCharacter(character);
		}
	}

	private ICell? GuardCell => Gameworld.Cells.Get(GuardCellId);

	private ICellExit? GuardExit
	{
		get
		{
			var cell = GuardCell;
			if (cell is null)
			{
				return null;
			}

			return Gameworld.ExitManager.GetExitByID(ExitId)?.CellExitFor(cell);
		}
	}

	protected override XElement SaveDefinition()
	{
		return new XElement("Element",
			new XElement("ExitId", ExitId),
			new XElement("GuardCellId", GuardCellId),
			new XElement("MoveAside", MoveAside),
			new XElement("ResetEmote", new XCData(ResetEmote ?? string.Empty)),
			new XElement("Permitted",
				from item in _permittedCharacterDescriptions
				select new XElement("Character",
					new XAttribute("id", item.Key),
					new XCData(item.Value)
				)
			)
		);
	}

	public static void InitialiseEffectType()
	{
		RegisterFactory("TollExitPermit", (effect, owner) => new TollExitPermit(effect, owner));
	}

	public override string Describe(IPerceiver voyeur)
	{
		var exit = GuardExit;
		var exitText = exit is null ? "an unknown exit" : $"the exit to {exit.OutboundDirectionDescription}";
		return MoveAside
			? $"Holding {exitText} open for paid toll traffic"
			: $"Permitting paid toll traffic through {exitText}";
	}

	protected override string SpecificEffectType => "TollExitPermit";

	public override bool SavingEffect => true;

	public override void ExpireEffect()
	{
		var exit = GuardExit;
		if (exit is not null && CharacterOwner.Location == GuardCell)
		{
			if (MoveAside)
			{
				if (!CharacterOwner.AffectedBy<IGuardExitEffect>(x => x.Exit?.Exit.Id == ExitId))
				{
					CharacterOwner.AddEffect(new GuardingExit(CharacterOwner, exit, false));
				}
			}
			else
			{
				foreach (var guard in CharacterOwner
					.EffectsOfType<IGuardExitEffect>(x => x.Exit?.Exit.Id == ExitId)
					.ToList())
				{
					foreach (var id in _permittedCharacterDescriptions.Keys)
					{
						guard.RemoveExemption(id);
					}
				}
			}

			if (!string.IsNullOrWhiteSpace(ResetEmote))
			{
				var emote = new Emote(ResetEmote, CharacterOwner, CharacterOwner);
				if (emote.Valid)
				{
					CharacterOwner.OutputHandler.Handle(new EmoteOutput(emote, flags: OutputFlags.SuppressObscured));
				}
			}
		}

		Owner.RemoveEffect(this, true);
	}
}
