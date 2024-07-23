using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Models;
using MudSharp.Framework;
using MudSharp.RPG.Merits.Interfaces;

namespace MudSharp.RPG.Merits.CharacterMerits;

public class MyopiaMerit : CharacterMeritBase, IMyopiaMerit
{
	public bool CorrectedByGlasses { get; protected set; }

	protected MyopiaMerit(Merit merit, IFuturemud gameworld) : base(merit, gameworld)
	{
		LoadFromXml(XElement.Parse(merit.Definition));
	}

	protected MyopiaMerit(){}

	protected MyopiaMerit(IFuturemud gameworld, string name) : base(gameworld, name, "Myopia", "@ have|has bad eyesight")
	{
		DoDatabaseInsert();
	}

	/// <inheritdoc />
	protected override XElement SaveSubtypeDefinition(XElement root)
	{
		root.Add(new XAttribute("glasses", CorrectedByGlasses));
		return root;
	}

	private void LoadFromXml(XElement root)
	{
		CorrectedByGlasses = bool.Parse(root.Attribute("glasses")?.Value ?? "false");
	}

	public static void RegisterMeritInitialiser()
	{
		MeritFactory.RegisterMeritInitialiser("Myopia",
			(merit, gameworld) => new MyopiaMerit(merit, gameworld));
		MeritFactory.RegisterBuilderMeritInitialiser("Myopia", (gameworld, name) => new MyopiaMerit(gameworld, name));
		MeritFactory.RegisterMeritHelp("Myopia", "Makes it difficult to see things at range", new MyopiaMerit().HelpText);
	}

	/// <inheritdoc />
	protected override string SubtypeHelp => $@"{base.SubtypeHelp}
	#3glasses#0 - toggles being able to be corrected with glasses";

	/// <inheritdoc />
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "glasses":
				return BuildingCommandGlasses(actor);
		}
		return base.BuildingCommand(actor, command.GetUndo());
	}

	private bool BuildingCommandGlasses(ICharacter actor)
	{
		CorrectedByGlasses = !CorrectedByGlasses;
		Changed = true;
		actor.OutputHandler.Send($"This merit will {CorrectedByGlasses.NowNoLonger()} be able to be corrected by glasses.");
		return true;
	}

	/// <inheritdoc />
	protected override void SubtypeShow(ICharacter actor, StringBuilder sb)
	{
		sb.AppendLine($"Corrected by Glasses: {CorrectedByGlasses.ToColouredString()}");
	}
}