using System;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Effects.Interfaces;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems.Decorators;
using MudSharp.GameItems;

namespace MudSharp.Effects.Concrete;

public class ResidueContamination : Effect, ICleanableEffect, ISDescAdditionEffect, IDescriptionAdditionEffect,
	IEffectAddsWeight
{
	private static IStackDecorator _stackDecorator;
	private double _weight;

	public ResidueContamination(IPerceivable owner, ISolid material, ILiquid originalLiquid, double weight,
		IFutureProg applicabilityProg = null) : base(owner, applicabilityProg)
	{
		Material = material;
		OriginalLiquid = originalLiquid;
		Weight = weight;
	}

	public ResidueContamination(XElement effect, IPerceivable owner) : base(effect, owner)
	{
		LoadFromXml(effect.Element("Effect"));
	}

	public static IStackDecorator StackDecorator => _stackDecorator ??= Futuremud.Games.First()
		.StackDecorators.Get(
			long.Parse(
				Futuremud.Games.First()
				         .GetStaticConfiguration("ResidueStackDecorator")));

	public ISolid Material { get; set; }
	public ILiquid OriginalLiquid { get; set; }

	public double Weight
	{
		get { return _weight; }
		set
		{
#if DEBUG
			if (double.IsNaN(value))
			{
				Console.WriteLine("NaN weight!");
			}
#endif
			_weight = value;
			Changed = true;
		}
	}

	private void LoadFromXml(XElement root)
	{
		Material = Gameworld.Materials.Get(long.Parse(root.Element("Material")?.Value ?? "0"));
		OriginalLiquid = Gameworld.Liquids.Get(long.Parse(root.Element("Liquid")?.Value ?? "0"));
		_weight = double.Parse(root.Element("Weight")?.Value ?? "0");
		if (double.IsNaN(Weight))
		{
			_weight = double.Epsilon;
		}
	}

	public static void InitialiseEffectType()
	{
		RegisterFactory("ResidueContamination", (effect, owner) => new ResidueContamination(effect, owner));
	}

	#region Overrides of Effect

	public override string Describe(IPerceiver voyeur)
	{
		return $"Residue contamination with {Material.Name} {Weight:N4}kg";
	}

	protected override string SpecificEffectType { get; } = "ResidueContamination";

	public override IEffect NewEffectOnItemMorph(IGameItem oldItem, IGameItem newItem)
	{
		if (oldItem == Owner)
		{
			return new ResidueContamination(newItem, Material, OriginalLiquid, Weight, ApplicabilityProg);
		}

		return null;
	}

	public override bool SavingEffect { get; } = true;

	protected override XElement SaveDefinition()
	{
		return
			new XElement("Effect", new XElement("Material", Material.Id),
				new XElement("Liquid", OriginalLiquid?.Id ?? 0), new XElement("Weight", Weight));
	}

	#endregion

	#region Implementation of ICleanableEffect

	public TimeSpan BaseCleanTime
		=> TimeSpan.FromSeconds(Gameworld.GetStaticDouble("BaseCleanTime"));

	public ITag CleaningToolTag
		=> Gameworld.Tags.Get(Gameworld.GetStaticLong("ResidueCleaningToolTag"));

	public ILiquid LiquidRequired => Material.Solvent ?? OriginalLiquid?.Solvent;

	public double LiquidAmountConsumed
	{
		get => Weight * Material.SolventRatio;
		set => Weight = value / Material.SolventRatio;
	}

	public string EmoteBeginClean => Gameworld.GetStaticString("EmoteBeginClean");
	public string EmoteStopClean => Gameworld.GetStaticString("EmoteStopClean");
	public string EmoteFinishClean => Gameworld.GetStaticString("EmoteFinishClean");
	public string PromptStatusLine => Gameworld.GetStaticString("CleanPromptStatusLine");

	/// <summary>
	/// Performs a small amount of cleaning with a liquid.
	/// </summary>
	/// <param name="liquid">A liquid being used to clean</param>
	/// <param name="amount">The amount of liquid being used</param>
	/// <returns>True if the effect is now totally cleaned</returns>
	public bool CleanWithLiquid(LiquidMixture liquid, double amount)
	{
		if (liquid?.CountsAs(LiquidRequired).Truth == true)
		{
			Weight -= amount / Material.SolventRatio;
			Changed = true;
		}

		return Weight <= 0.0;
	}

	#endregion

	#region Implementation of ISDescAdditionEffect

	public string AddendumText => Material.ResidueSdesc;

	public string GetAddendumText(bool colour)
	{
		return colour && Colour != null ? AddendumText.Colour(Colour) : AddendumText;
	}

	public string AdditionalText =>
		string.Format(
			Material.ResidueDesc ?? $"It is covered in a layer of {Material.MaterialDescription.ToLowerInvariant()}",
			StackDecorator.Describe("", "", Weight));

	public string GetAdditionalText(IPerceiver voyeur, bool colour)
	{
		return colour && Colour != null ? AdditionalText.Colour(Colour) : AdditionalText;
	}

	public bool PlayerSet { get; } = false;
	public ANSIColour Colour => Material.ResidueColour;

	public override bool PreventsItemFromMerging(IGameItem effectOwnerItem, IGameItem targetItem)
	{
		return true;
	}

	#endregion

	#region Implementation of IEffectAddsWeight

	public double AddedWeight => Weight;

	#endregion
}