using MudSharp.GameItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DatabaseSeeder.Seeders
{
    public partial class ItemSeeder
    {
        private static readonly EraOutfitSlotSpec[] AntiquityClothingSlotDefinitions = [];

        private static readonly EraCultureSpec[] AntiquityEraCultureSpecs =
        [
            new("hellenic", "Hellenic", "Hellenic"),
            new("egyptian", "Egyptian", "Egyptian"),
            new("roman", "Roman", "Roman"),
            new("celtic", "Celtic", "Celtic"),
            new("germanic", "Germanic", "Germanic"),
            new("kushite", "Kushite", "Kushite"),
            new("punic", "Punic", "Punic"),
            new("persian", "Persian", "Persian"),
            new("etruscan", "Etruscan", "Etruscan"),
            new("anatolian", "Anatolian", "Anatolian"),
            new("scythian_sarmatian", "Scythian-Sarmatian", "Scythian-Sarmatian")
        ];

        private static readonly EraSeederConfiguration AntiquityEraConfiguration = new(
            "antiquity",
            "Eras / Antiquity",
            "Eras / Antiquity / Cultures",
            null,
            "antiquity",
            [
                "Market / Clothing / Simple Clothing",
                "Market / Clothing / Standard Clothing",
                "Market / Clothing / Luxury Clothing",
                "Market / Clothing / Footwear"
            ],
            new EraVariableColourPolicy(
                "Variable_FineColour",
                "Variable_2FineColour",
                ["$colour"],
                ["$colour1", "$colour2"],
                ["papyrus", "bronze", "silver", "gold", "stone", "fired clay"]),
            [
                "Ancient Textile Production",
                "Ancient Hide and Leatherworking",
                "Ancient Common Clothing Crafting"
            ],
            ["Tailoring", "Leathermaking", "Jewellery", "Crafting"],
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["garment_cloth"] = "Garment Cloth",
                ["spun_yarn"] = "Spun Yarn",
                ["leather_panel"] = "Prepared Leather Panel",
                ["leather_sole"] = "Cut Leather Sole",
                ["bead_stock"] = "Bead Stock"
            },
            AntiquityClothingSlotDefinitions,
            false,
            false,
            false);

        internal static (string EraKey, string EraRootTag, string CultureTagRoot, string? StatusOrSocialRoleTagRoot, bool CompleteOutfitCataloguesRequired, bool GenericBaselineWardrobeGenerationAllowed, bool PlayerFacingDescriptionsMayIncludeCultureNames, IReadOnlyCollection<string> CultureKeys) AntiquityEraConfigurationForTesting =>
            (AntiquityEraConfiguration.EraKey,
                AntiquityEraConfiguration.EraRootTag,
                AntiquityEraConfiguration.CultureTagRoot,
                AntiquityEraConfiguration.StatusOrSocialRoleTagRoot,
                AntiquityEraConfiguration.CompleteOutfitCataloguesRequired,
                AntiquityEraConfiguration.GenericBaselineWardrobeGenerationAllowed,
                AntiquityEraConfiguration.PlayerFacingDescriptionsMayIncludeCultureNames,
                AntiquityEraCultureSpecs.Select(x => x.Key).ToArray());

        private void SeedAntiquityClothing()
        {
            #region Antiquity Textile Tools
            InstallCatalogueItem(Historical01Stock.AntiquityBronzeTextileShears());

            InstallCatalogueItem(Historical01Stock.AntiquityBronzeSewingNeedle());

            InstallCatalogueItem(Historical01Stock.AntiquityRettingTrough());

            InstallCatalogueItem(Historical01Stock.AntiquityFlaxBreak());

            InstallCatalogueItem(Historical01Stock.AntiquityFibreHackle());

            InstallCatalogueItem(Historical01Stock.AntiquityDropSpindle());

            InstallCatalogueItem(Historical01Stock.AntiquityDistaff());

            InstallCatalogueItem(Historical01Stock.AntiquityWarpWeightedLoom());

            InstallCatalogueItem(Historical01Stock.AntiquityClayLoomWeight());

            InstallCatalogueItem(Historical01Stock.AntiquityWeavingShuttle());

            InstallCatalogueItem(Historical01Stock.AntiquityWeaversSword());

            InstallCatalogueItem(Historical01Stock.AntiquityDyeVat());

            InstallCatalogueItem(Historical01Stock.AntiquityMordantCauldron());

            InstallCatalogueItem(Historical01Stock.AntiquityFullersTrough());

            InstallCatalogueItem(Historical01Stock.AntiquityFullersMallet());

            InstallCatalogueItem(Historical01Stock.AntiquityTenterFrame());

            #endregion

            #region Antiquity Leatherworking Tools
            InstallCatalogueItem(Historical01Stock.AntiquityBronzeAwlPunch());

            InstallCatalogueItem(Historical01Stock.AntiquityOakStitchingPony());

            InstallCatalogueItem(Historical01Stock.AntiquityBronzeEdgeBeveller());

            InstallCatalogueItem(Historical01Stock.AntiquityBronzeLeatherGouge());

            InstallCatalogueItem(Historical01Stock.AntiquityBronzeLeatherCreaser());

            InstallCatalogueItem(Historical01Stock.AntiquityOakShoeLast());

            InstallCatalogueItem(Historical01Stock.AntiquityBronzeLeatherWaxPot());

            InstallCatalogueItem(Historical01Stock.AntiquityBronzeHideScraper());

            InstallCatalogueItem(Historical01Stock.AntiquityOakTanningBeam());

            InstallCatalogueItem(Historical01Stock.AntiquityOakTanningRack());

            InstallCatalogueItem(Historical01Stock.AntiquityOakTanningPaddle());

            InstallCatalogueItem(Historical01Stock.AntiquityBronzeDehairingKnife());

            InstallCatalogueItem(Historical01Stock.AntiquityOakBrainTanningBucket());

            #endregion

            #region Common Antiquity
            InstallCatalogueItem(Historical01Stock.AntiquityLinenLoincloth());

            InstallCatalogueItem(Historical01Stock.AntiquityLinenBreastband());

            InstallCatalogueItem(Historical01Stock.AntiquityPlainLeatherBelt());

            InstallCatalogueItem(Historical01Stock.AntiquityBronzeBuckledLeatherBelt());

            InstallCatalogueItem(Historical01Stock.AntiquityPlainLeatherSandals());

            InstallCatalogueItem(Historical01Stock.AntiquityFineLeatherSandals());

            InstallCatalogueItem(Historical01Stock.AntiquitySoftLeatherShoes());

            InstallCatalogueItem(Historical01Stock.AntiquityAnkleLeatherBoots());

            InstallCatalogueItem(Historical01Stock.AntiquitySimpleWovenSash());

            InstallCatalogueItem(Historical01Stock.AntiquityFineWovenSash());

            InstallCatalogueItem(Historical01Stock.AntiquityPapyrusSandals());

            InstallCatalogueItem(Historical01Stock.AntiquityFinePapyrusSandals());

            InstallCatalogueItem(Historical01Stock.AntiquityLowStrappedLeatherShoes());

            InstallCatalogueItem(Historical01Stock.AntiquitySoftLeatherRidingBoots());

            InstallCatalogueItem(Historical01Stock.AntiquityWrappedLinenHeadcloth());

            InstallCatalogueItem(Historical01Stock.AntiquityFineLinenHeadcloth());

            InstallCatalogueItem(Historical01Stock.AntiquityLinenShoulderShawl());

            InstallCatalogueItem(Historical01Stock.AntiquityFineLinenShoulderShawl());

            InstallCatalogueItem(Historical01Stock.AntiquityFrontKnottedGirdle());

            InstallCatalogueItem(Historical01Stock.AntiquityFineFrontKnottedGirdle());

            InstallCatalogueItem(Historical01Stock.AntiquityConicalFeltCap());

            InstallCatalogueItem(Historical01Stock.AntiquityFineConicalFeltCap());

            InstallCatalogueItem(Historical01Stock.AntiquityRoundedFeltCap());

            InstallCatalogueItem(Historical01Stock.AntiquityTallKyrbasia());

            InstallCatalogueItem(Historical01Stock.AntiquityFlutedFeltHat());

            InstallCatalogueItem(Historical01Stock.AntiquityGlassUsekhCollar());
            #endregion

            #region Celtic Antiquity
            InstallCatalogueItem(Historical01Stock.AntiquitySleevedCommonWoolTunic());

            InstallCatalogueItem(Historical01Stock.AntiquityWoolBraccae());

            InstallCatalogueItem(Historical01Stock.AntiquityRectangularWoolCloak());

            InstallCatalogueItem(Historical01Stock.AntiquityFineBorderedWoolTunic());

            InstallCatalogueItem(Historical01Stock.AntiquityFineWoolBraccae());

            InstallCatalogueItem(Historical01Stock.AntiquityFineCheckedWoolCloak());

            InstallCatalogueItem(Historical01Stock.AntiquityLongSleevedWoolTunic());

            InstallCatalogueItem(Historical01Stock.AntiquityWoolWrapSkirt());

            InstallCatalogueItem(Historical01Stock.AntiquityBroadWoolMantle());

            InstallCatalogueItem(Historical01Stock.AntiquityFineSleevedWoolGown());

            InstallCatalogueItem(Historical01Stock.AntiquityFineBorderedWoolMantle());

            InstallCatalogueItem(Historical01Stock.AntiquityLinenShoulderVeil());

            InstallCatalogueItem(Historical01Stock.AntiquityHeavyHoodedWoolCloak());

            InstallCatalogueItem(Historical01Stock.AntiquityWoolWorkSmock());

            InstallCatalogueItem(Historical01Stock.AntiquityFineRitualWoolMantle());

            InstallCatalogueItem(Historical01Stock.AntiquityWoolWinterMittens());

            InstallCatalogueItem(Historical01Stock.AntiquityCheckedWoolLegWraps());
            #endregion

            #region Germanic Antiquity
            InstallCatalogueItem(Historical01Stock.AntiquityStraightWoolTunic());

            InstallCatalogueItem(Historical01Stock.AntiquityNarrowWoolTrousers());

            InstallCatalogueItem(Historical01Stock.AntiquityHeavyWoolCloak());

            InstallCatalogueItem(Historical01Stock.AntiquityFineBandedWoolTunic());

            InstallCatalogueItem(Historical01Stock.AntiquityFineTaperedWoolTrousers());

            InstallCatalogueItem(Historical01Stock.AntiquityFurLinedWoolCloak());

            InstallCatalogueItem(Historical01Stock.AntiquityLongStraightWoolTunic());

            InstallCatalogueItem(Historical01Stock.AntiquityOverlappingWoolSkirt());

            InstallCatalogueItem(Historical01Stock.AntiquityCheckedWoolScarf());

            InstallCatalogueItem(Historical01Stock.AntiquityWoollySkinCape());

            InstallCatalogueItem(Historical01Stock.AntiquityFineLongWoolGown());

            InstallCatalogueItem(Historical01Stock.AntiquityFineHeavyWoolMantle());

            InstallCatalogueItem(Historical01Stock.AntiquityLinenHeadVeil());

            InstallCatalogueItem(Historical01Stock.AntiquityHeavyFeltHood());

            InstallCatalogueItem(Historical01Stock.AntiquityFineSeeressWoolMantle());

            InstallCatalogueItem(Historical01Stock.AntiquityFeltHandMitts());

            InstallCatalogueItem(Historical01Stock.AntiquityWoolFootwraps());

            InstallCatalogueItem(Historical01Stock.AntiquityWoolEarBand());
            #endregion

            #region Italic Antiquity
            InstallCatalogueItem(Historical01Stock.AntiquityKneeLengthWoolTunica());

            InstallCatalogueItem(Historical01Stock.AntiquityWoolTravelMantle());

            InstallCatalogueItem(Historical01Stock.AntiquityFineLinenTunica());

            InstallCatalogueItem(Historical01Stock.AntiquityWoolToga());

            InstallCatalogueItem(Historical01Stock.AntiquityLongWoolTunica());

            InstallCatalogueItem(Historical01Stock.AntiquityWoolPalla());

            InstallCatalogueItem(Historical01Stock.AntiquityFineLongLinenTunica());

            InstallCatalogueItem(Historical01Stock.AntiquityWoolStola());

            InstallCatalogueItem(Historical01Stock.AntiquityFineWoolPalla());

            InstallCatalogueItem(Historical01Stock.AntiquityHoodedWoolPaenula());

            InstallCatalogueItem(Historical01Stock.AntiquityFineTogaPraetexta());

            InstallCatalogueItem(Historical01Stock.AntiquityWoolUdones());

            InstallCatalogueItem(Historical01Stock.AntiquityWoolHandWraps());

            InstallCatalogueItem(Historical01Stock.AntiquityRomanWoolFocale());

            InstallCatalogueItem(Historical01Stock.AntiquityRomanWoolSagum());

            InstallCatalogueItem(Historical01Stock.AntiquityItalicWhiteWoolInfula());

            InstallCatalogueItem(Historical01Stock.AntiquityEgyptianSpottedHidePriestMantle());
            #endregion

            #region Hellenic Antiquity
            InstallCatalogueItem(Historical01Stock.AntiquityShortWoolChiton());

            InstallCatalogueItem(Historical01Stock.AntiquityWoolHimation());

            InstallCatalogueItem(Historical01Stock.AntiquityFineLinenChiton());

            InstallCatalogueItem(Historical01Stock.AntiquityFineWoolHimation());

            InstallCatalogueItem(Historical01Stock.AntiquityShortWoolChlamys());

            InstallCatalogueItem(Historical01Stock.AntiquityFullLengthWoolPeplos());

            InstallCatalogueItem(Historical01Stock.AntiquityFullWoolHimation());

            InstallCatalogueItem(Historical01Stock.AntiquityFineLongLinenChiton());

            InstallCatalogueItem(Historical01Stock.AntiquityFineFullWoolHimation());

            InstallCatalogueItem(Historical01Stock.AntiquityLightLinenHeadVeil());

            InstallCatalogueItem(Historical01Stock.AntiquityWoolExomis());

            InstallCatalogueItem(Historical01Stock.AntiquityHeavyWoolZeira());

            InstallCatalogueItem(Historical01Stock.AntiquityFineRitualWoolPeplos());

            InstallCatalogueItem(Historical01Stock.AntiquityWoolTravelLegWraps());

            InstallCatalogueItem(Historical01Stock.AntiquityWoolHandWrapsHellenic());

            #endregion

            #region Punic Antiquity
            InstallCatalogueItem(Historical01Stock.AntiquityShortFittedLinenTunic());

            InstallCatalogueItem(Historical01Stock.AntiquityPatternedLinenWaistcloth());

            InstallCatalogueItem(Historical01Stock.AntiquityShortSleevedLinenOverblouse());

            InstallCatalogueItem(Historical01Stock.AntiquityLongLinenInnerRobe());

            InstallCatalogueItem(Historical01Stock.AntiquityOneShoulderWoolMantle());

            InstallCatalogueItem(Historical01Stock.AntiquityLongFoldedLinenRobe());

            InstallCatalogueItem(Historical01Stock.AntiquityLooseLinenHood());

            InstallCatalogueItem(Historical01Stock.AntiquityFineFullLinenGown());

            InstallCatalogueItem(Historical01Stock.AntiquityLeftShoulderOverdrape());

            InstallCatalogueItem(Historical01Stock.AntiquityStarBorderedLinenRobe());

            InstallCatalogueItem(Historical01Stock.AntiquitySailorWoolWindCloak());

            InstallCatalogueItem(Historical01Stock.AntiquityFineTempleLinenVeil());
            #endregion

            #region Persian Antiquity
            InstallCatalogueItem(Historical01Stock.AntiquitySarapisWoolTunic());

            InstallCatalogueItem(Historical01Stock.AntiquityFineSarapisLinenTunic());

            InstallCatalogueItem(Historical01Stock.AntiquityAnaxyridesWoolTrousers());

            InstallCatalogueItem(Historical01Stock.AntiquityFinePatternedAnaxyrides());

            InstallCatalogueItem(Historical01Stock.AntiquityWoolKandys());

            InstallCatalogueItem(Historical01Stock.AntiquityFineWoolKandys());

            InstallCatalogueItem(Historical01Stock.AntiquityPleatedCourtRobe());

            InstallCatalogueItem(Historical01Stock.AntiquityFinePleatedCourtGown());

            InstallCatalogueItem(Historical01Stock.AntiquityWideClothBelt());

            InstallCatalogueItem(Historical01Stock.AntiquityFineWideClothBelt());

            InstallCatalogueItem(Historical01Stock.AntiquityFullHeadAndNeckVeil());

            InstallCatalogueItem(Historical01Stock.AntiquityHeavyFeltRidingCoat());

            InstallCatalogueItem(Historical01Stock.AntiquityFineMagianLinenRobe());

            InstallCatalogueItem(Historical01Stock.AntiquityFeltRidingMittens());

            InstallCatalogueItem(Historical01Stock.AntiquityWoolBootLiners());
            #endregion

            #region Other Antiquity
            InstallCatalogueItem(Historical01Stock.AdjacentAntiquityPointedLeatherShoes());

            InstallCatalogueItem(Historical01Stock.AdjacentAntiquityFinePointedLeatherShoes());

            InstallCatalogueItem(Historical01Stock.AdjacentAntiquityShortSleevedLinenTunic());

            InstallCatalogueItem(Historical01Stock.AdjacentAntiquityBorderedWoolTunic());

            InstallCatalogueItem(Historical01Stock.AdjacentAntiquityCurvedTebenna());

            InstallCatalogueItem(Historical01Stock.AdjacentAntiquityFineCurvedTebenna());

            InstallCatalogueItem(Historical01Stock.AdjacentAntiquityWrappedLinenSkirt());

            InstallCatalogueItem(Historical01Stock.AdjacentAntiquityRectangularShoulderCloak());

            InstallCatalogueItem(Historical01Stock.AdjacentAntiquityFittedLinenGown());

            InstallCatalogueItem(Historical01Stock.AdjacentAntiquityLinenHeadMantle());

            InstallCatalogueItem(Historical01Stock.AdjacentAntiquityWoolCraftsmanTunic());

            InstallCatalogueItem(Historical01Stock.AdjacentAntiquityPriestlyLinenVeil());

            InstallCatalogueItem(Historical01Stock.AdjacentAntiquityBeltedWoolTunic());

            InstallCatalogueItem(Historical01Stock.AdjacentAntiquityFineBandedTunic());

            InstallCatalogueItem(Historical01Stock.AdjacentAntiquityBandedLegWraps());

            InstallCatalogueItem(Historical01Stock.AdjacentAntiquityHoodedWoolCloak());

            InstallCatalogueItem(Historical01Stock.AdjacentAntiquityForwardPointingFeltCap());

            InstallCatalogueItem(Historical01Stock.AdjacentAntiquityShortWoolCape());

            InstallCatalogueItem(Historical01Stock.AdjacentAntiquityFinePatternedWoolRobe());

            InstallCatalogueItem(Historical01Stock.AdjacentAntiquityFringedWoolMantle());

            InstallCatalogueItem(Historical01Stock.AdjacentAntiquityWoolWrappedSkirt());

            InstallCatalogueItem(Historical01Stock.AdjacentAntiquityFineRectangularVeil());

            InstallCatalogueItem(Historical01Stock.AdjacentAntiquityHeavyFeltWinterCap());

            InstallCatalogueItem(Historical01Stock.AdjacentAntiquityPriestlyBandedWoolRobe());

            InstallCatalogueItem(Historical01Stock.AdjacentAntiquityFeltHandMitts());

            InstallCatalogueItem(Historical01Stock.AdjacentAntiquityWoolNeckWrap());

            InstallCatalogueItem(Historical01Stock.AdjacentAntiquitySoftRidingBoots());

            InstallCatalogueItem(Historical01Stock.AdjacentAntiquityFeltRidingCap());

            InstallCatalogueItem(Historical01Stock.AdjacentAntiquityTallFeltCap());

            InstallCatalogueItem(Historical01Stock.AdjacentAntiquityRidingTunic());

            InstallCatalogueItem(Historical01Stock.AdjacentAntiquityWoolRidingTrousers());

            InstallCatalogueItem(Historical01Stock.AdjacentAntiquityPatternedRidingTrousers());

            InstallCatalogueItem(Historical01Stock.AdjacentAntiquityOpenRidingCaftan());

            InstallCatalogueItem(Historical01Stock.AdjacentAntiquityFurTrimmedCaftan());

            InstallCatalogueItem(Historical01Stock.AdjacentAntiquitySplitRidingSkirt());

            InstallCatalogueItem(Historical01Stock.AdjacentAntiquityLongFeltCoat());

            InstallCatalogueItem(Historical01Stock.AdjacentAntiquityHorseherderWoolTunic());

            InstallCatalogueItem(Historical01Stock.AdjacentAntiquityRitualFeltCap());

            InstallCatalogueItem(Historical01Stock.AdjacentAntiquityFeltMittens());

            InstallCatalogueItem(Historical01Stock.AdjacentAntiquityFeltBootLiners());

            InstallCatalogueItem(Historical01Stock.AdjacentAntiquityNarrowLinenKilt());

            InstallCatalogueItem(Historical01Stock.AdjacentAntiquityLinenShoulderCloth());

            InstallCatalogueItem(Historical01Stock.AdjacentAntiquityCottonWrapSkirt());

            InstallCatalogueItem(Historical01Stock.AdjacentAntiquitySleevelessLinenTunic());

            InstallCatalogueItem(Historical01Stock.AdjacentAntiquityFringedLinenRobe());

            InstallCatalogueItem(Historical01Stock.AdjacentAntiquityTasseledLinenShawl());

            InstallCatalogueItem(Historical01Stock.AdjacentAntiquityTallLinenHeaddress());

            InstallCatalogueItem(Historical01Stock.AdjacentAntiquityBeadedLinenGirdle());

            InstallCatalogueItem(Historical01Stock.AdjacentAntiquityCottonDrapedDress());

            InstallCatalogueItem(Historical01Stock.AdjacentAntiquityLinenBeadApron());

            InstallCatalogueItem(Historical01Stock.AdjacentAntiquityPlainCottonHeadcloth());

            InstallCatalogueItem(Historical01Stock.AdjacentAntiquityLinenScribeKilt());

            InstallCatalogueItem(Historical01Stock.AdjacentAntiquityTempleLinenShift());

            InstallCatalogueItem(Historical01Stock.AdjacentAntiquityCottonFieldWrap());

            InstallCatalogueItem(Historical01Stock.AdjacentAntiquityFineRitualCottonMantle());

            InstallCatalogueItem(Historical01Stock.AdjacentAntiquityFineLinenSandals());
            #endregion
        }

        private void SeedAntiquityJewellery()
        {
            InstallCatalogueItem(Historical01Stock.JewellerySimpleBronzeTorc());

            InstallCatalogueItem(Historical01Stock.JewelleryAmberDiscNecklace());

            InstallCatalogueItem(Historical01Stock.JewelleryBronzeSpiralBracelet());

            InstallCatalogueItem(Historical01Stock.JewelleryBronzeCoilRing());

            InstallCatalogueItem(Historical01Stock.JewelleryBronzeHoopEarrings());

            InstallCatalogueItem(Historical01Stock.JewelleryCopperAnkleRings());

            InstallCatalogueItem(Historical01Stock.JewelleryPenannularBronzeBrooch());

            InstallCatalogueItem(Historical01Stock.JewelleryHeavyGoldTorc());

            InstallCatalogueItem(Historical01Stock.JewelleryRedInlaidSilverTorc());

            InstallCatalogueItem(Historical01Stock.JewelleryGoldBossedCirclet());

            InstallCatalogueItem(Historical01Stock.JewelleryGoldRibbonBracelet());

            InstallCatalogueItem(Historical01Stock.JewelleryGoldGarnetRing());

            InstallCatalogueItem(Historical01Stock.JewelleryGoldAmberDropEarrings());

            InstallCatalogueItem(Historical01Stock.JewelleryGoldSpiralAnklets());

            InstallCatalogueItem(Historical01Stock.JewelleryBronzeBeadHeadband());

            InstallCatalogueItem(Historical01Stock.JewelleryBoneAndAmberNecklace());

            InstallCatalogueItem(Historical01Stock.JewelleryIronWireBracelet());

            InstallCatalogueItem(Historical01Stock.JewelleryBronzeWireRing());

            InstallCatalogueItem(Historical01Stock.JewellerySmallBronzeWireEarrings());

            InstallCatalogueItem(Historical01Stock.JewelleryBronzeAnkleHoops());

            InstallCatalogueItem(Historical01Stock.JewelleryBronzeDressFibula());

            InstallCatalogueItem(Historical01Stock.JewellerySilverGiltBrowBand());

            InstallCatalogueItem(Historical01Stock.JewelleryGoldFoilAmberNecklace());

            InstallCatalogueItem(Historical01Stock.JewelleryGoldSnakeHeadArmlet());

            InstallCatalogueItem(Historical01Stock.JewelleryGoldSnakeHeadRing());

            InstallCatalogueItem(Historical01Stock.JewelleryGoldAmberEarrings());

            InstallCatalogueItem(Historical01Stock.JewellerySilverBellAnklets());

            InstallCatalogueItem(Historical01Stock.JewelleryAngularMarkedSilverFibula());

            InstallCatalogueItem(Historical01Stock.JewelleryPlainBronzeBrowBand());

            InstallCatalogueItem(Historical01Stock.JewelleryBlueMelonBeadNecklace());

            InstallCatalogueItem(Historical01Stock.JewelleryCarvedIvoryBangle());

            InstallCatalogueItem(Historical01Stock.JewelleryIronSignetRing());

            InstallCatalogueItem(Historical01Stock.JewelleryBronzeWireDropEarrings());

            InstallCatalogueItem(Historical01Stock.JewelleryBronzeChainAnklets());

            InstallCatalogueItem(Historical01Stock.JewelleryBronzeCrossbarFibula());

            InstallCatalogueItem(Historical01Stock.JewelleryGoldLeafDiadem());

            InstallCatalogueItem(Historical01Stock.JewelleryGoldPearlNecklace());

            InstallCatalogueItem(Historical01Stock.JewelleryGoldOpenworkBracelets());

            InstallCatalogueItem(Historical01Stock.JewelleryGoldEmeraldRing());

            InstallCatalogueItem(Historical01Stock.JewelleryGoldPearlEarrings());

            InstallCatalogueItem(Historical01Stock.JewelleryGoldPearlAnklets());

            InstallCatalogueItem(Historical01Stock.JewelleryGoldCarnelianSignetRing());

            InstallCatalogueItem(Historical01Stock.JewelleryBronzeRosetteHeadband());

            InstallCatalogueItem(Historical01Stock.JewelleryGlassTubeNecklace());

            InstallCatalogueItem(Historical01Stock.JewelleryBronzePairBracelets());

            InstallCatalogueItem(Historical01Stock.JewelleryBronzeSealRing());

            InstallCatalogueItem(Historical01Stock.JewelleryBronzeDiscEarrings());

            InstallCatalogueItem(Historical01Stock.JewelleryBronzeLinkAnklets());

            InstallCatalogueItem(Historical01Stock.JewelleryBronzeCloakPin());

            InstallCatalogueItem(Historical01Stock.JewelleryGoldGarnetDiadem());

            InstallCatalogueItem(Historical01Stock.JewelleryGoldStrapNecklace());

            InstallCatalogueItem(Historical01Stock.JewelleryGoldSerpentBracelets());

            InstallCatalogueItem(Historical01Stock.JewelleryGoldSerpentRing());

            InstallCatalogueItem(Historical01Stock.JewelleryGoldBoatPendantEarrings());

            InstallCatalogueItem(Historical01Stock.JewelleryGoldRosetteAnklets());

            InstallCatalogueItem(Historical01Stock.JewelleryGoldFloralBrooch());

            InstallCatalogueItem(Historical01Stock.JewelleryShellDiscHeadband());

            InstallCatalogueItem(Historical01Stock.JewelleryBlueGlassEyeBeadNecklace());

            InstallCatalogueItem(Historical01Stock.JewelleryGlassBeadBracelet());

            InstallCatalogueItem(Historical01Stock.JewelleryCopperScarabRing());

            InstallCatalogueItem(Historical01Stock.JewelleryBronzeBallEarrings());

            InstallCatalogueItem(Historical01Stock.JewelleryShellBeadAnklets());

            InstallCatalogueItem(Historical01Stock.JewelleryBlueGlassHeadAmulet());

            InstallCatalogueItem(Historical01Stock.JewelleryGoldRosetteSeaDiadem());

            InstallCatalogueItem(Historical01Stock.JewelleryGoldFiligraneGlassNecklace());

            InstallCatalogueItem(Historical01Stock.JewelleryGoldCarnelianBracelet());

            InstallCatalogueItem(Historical01Stock.JewelleryGoldScarabSignetRing());

            InstallCatalogueItem(Historical01Stock.JewelleryGoldCageBallEarrings());

            InstallCatalogueItem(Historical01Stock.JewelleryGoldRosetteSeaAnklets());

            InstallCatalogueItem(Historical01Stock.JewelleryGoldGlassHeadEarrings());

            InstallCatalogueItem(Historical01Stock.JewelleryTurquoiseBeadHeadband());

            InstallCatalogueItem(Historical01Stock.JewelleryAgateBeadNecklace());

            InstallCatalogueItem(Historical01Stock.JewelleryBronzeRoundedArmlet());

            InstallCatalogueItem(Historical01Stock.JewelleryBronzeSquareSealRing());

            InstallCatalogueItem(Historical01Stock.JewelleryBronzeDropLoopEarrings());

            InstallCatalogueItem(Historical01Stock.JewelleryBronzeHeavyAnklets());

            InstallCatalogueItem(Historical01Stock.JewelleryCopperLionBrooch());

            InstallCatalogueItem(Historical01Stock.JewelleryGoldRosettePlateDiadem());

            InstallCatalogueItem(Historical01Stock.JewelleryGoldLapisNecklace());

            InstallCatalogueItem(Historical01Stock.JewelleryGoldGriffinArmlet());

            InstallCatalogueItem(Historical01Stock.JewelleryGoldCarnelianSealRing());

            InstallCatalogueItem(Historical01Stock.JewelleryGoldCrescentEarrings());

            InstallCatalogueItem(Historical01Stock.JewelleryGoldTurquoiseAnklets());

            InstallCatalogueItem(Historical01Stock.JewelleryGoldLionGirdle());

            InstallCatalogueItem(Historical01Stock.JewelleryBlueGlazedCeramicHeadband());

            InstallCatalogueItem(Historical01Stock.JewelleryShellAndBoneNecklace());

            InstallCatalogueItem(Historical01Stock.JewelleryBlueGlazedCeramicBracelets());

            InstallCatalogueItem(Historical01Stock.JewelleryBoneScarabRing());

            InstallCatalogueItem(Historical01Stock.JewelleryCopperWireEarrings());

            InstallCatalogueItem(Historical01Stock.JewelleryCopperAnkleRingsValley());

            InstallCatalogueItem(Historical01Stock.JewelleryCowrieShellGirdle());

            InstallCatalogueItem(Historical01Stock.JewelleryGoldCobraFrontlet());

            InstallCatalogueItem(Historical01Stock.JewelleryGoldUsekhCollar());

            InstallCatalogueItem(Historical01Stock.JewelleryGoldFalconBracelets());

            InstallCatalogueItem(Historical01Stock.JewelleryGoldLapisScarabRing());

            InstallCatalogueItem(Historical01Stock.JewelleryGoldCarnelianEarrings());

            InstallCatalogueItem(Historical01Stock.JewelleryGoldCarnelianAnklets());

            InstallCatalogueItem(Historical01Stock.JewelleryElectrumWingedPectoral());

            InstallCatalogueItem(Historical01Stock.JewelleryBronzeGranuleHeadband());

            InstallCatalogueItem(Historical01Stock.JewelleryGlassPendantNecklace());

            InstallCatalogueItem(Historical01Stock.JewelleryBronzeDiscBracelet());

            InstallCatalogueItem(Historical01Stock.JewelleryBronzeCarnelianRing());

            InstallCatalogueItem(Historical01Stock.JewelleryBronzeLobedEarrings());

            InstallCatalogueItem(Historical01Stock.JewelleryShellLacedAnklets());

            InstallCatalogueItem(Historical01Stock.JewelleryBronzeWingedFibula());

            InstallCatalogueItem(Historical01Stock.JewelleryBoneDressPin());

            InstallCatalogueItem(Historical01Stock.JewelleryGoldPalmetteDiadem());

            InstallCatalogueItem(Historical01Stock.JewelleryGoldGlassPendantNecklace());

            InstallCatalogueItem(Historical01Stock.JewelleryGoldCarnelianBracelets());

            InstallCatalogueItem(Historical01Stock.JewelleryGoldScarabRing());

            InstallCatalogueItem(Historical01Stock.JewelleryGoldQuartzDiscEarrings());

            InstallCatalogueItem(Historical01Stock.JewelleryGranulatedGoldAnklets());

            InstallCatalogueItem(Historical01Stock.JewelleryGoldSphinxFibula());

            InstallCatalogueItem(Historical01Stock.JewelleryGoldScarabDressPin());

            InstallCatalogueItem(Historical01Stock.JewelleryBronzeDiscBrowband());

            InstallCatalogueItem(Historical01Stock.JewelleryWoodenBeadNecklace());

            InstallCatalogueItem(Historical01Stock.JewelleryBronzeOpenBracelet());

            InstallCatalogueItem(Historical01Stock.JewelleryCopperSealRing());

            InstallCatalogueItem(Historical01Stock.JewelleryBronzeHollowEarrings());

            InstallCatalogueItem(Historical01Stock.JewelleryCopperBellAnklets());

            InstallCatalogueItem(Historical01Stock.JewelleryBronzeStuddedFibula());

            InstallCatalogueItem(Historical01Stock.JewelleryBoneBeltPlaque());

            InstallCatalogueItem(Historical01Stock.JewelleryElectrumRosetteDiadem());

            InstallCatalogueItem(Historical01Stock.JewelleryGoldAgateNecklace());

            InstallCatalogueItem(Historical01Stock.JewelleryElectrumSpiralBracelets());

            InstallCatalogueItem(Historical01Stock.JewelleryGoldLionSealRing());

            InstallCatalogueItem(Historical01Stock.JewelleryGoldTurquoiseDropEarrings());

            InstallCatalogueItem(Historical01Stock.JewelleryElectrumAnkleRings());

            InstallCatalogueItem(Historical01Stock.JewelleryGoldStuddedFibula());

            InstallCatalogueItem(Historical01Stock.JewelleryGoldIvoryBeltPlaque());

            InstallCatalogueItem(Historical01Stock.JewelleryBronzeAnimalHeadband());

            InstallCatalogueItem(Historical01Stock.JewelleryBoneToothNecklace());

            InstallCatalogueItem(Historical01Stock.JewelleryBronzeOpenArmlet());

            InstallCatalogueItem(Historical01Stock.JewelleryBronzeAnimalRing());

            InstallCatalogueItem(Historical01Stock.JewelleryBronzeLoopEarrings());

            InstallCatalogueItem(Historical01Stock.JewelleryBronzeTaperedAnklets());

            InstallCatalogueItem(Historical01Stock.JewelleryBronzeAnimalBeltPlaques());

            InstallCatalogueItem(Historical01Stock.JewelleryBronzeAnimalCloakPin());

            InstallCatalogueItem(Historical01Stock.JewelleryGoldAnimalPlaqueHeadband());

            InstallCatalogueItem(Historical01Stock.JewelleryGoldTurquoisePendantNecklace());

            InstallCatalogueItem(Historical01Stock.JewelleryGoldGriffinArmlets());

            InstallCatalogueItem(Historical01Stock.JewelleryGoldStagRing());

            InstallCatalogueItem(Historical01Stock.JewelleryGoldBirdDropEarrings());

            InstallCatalogueItem(Historical01Stock.JewelleryGoldInlaidTurquoiseAnklets());

            InstallCatalogueItem(Historical01Stock.JewelleryGoldAnimalBeltPlaques());

            InstallCatalogueItem(Historical01Stock.JewelleryGoldAnimalCloakPin());

            InstallCatalogueItem(Historical01Stock.JewelleryShellAndBoneBrowband());

            InstallCatalogueItem(Historical01Stock.JewelleryShellCarnelianNecklace());

            InstallCatalogueItem(Historical01Stock.JewelleryCopperRamBracelet());

            InstallCatalogueItem(Historical01Stock.JewelleryCopperRamRing());

            InstallCatalogueItem(Historical01Stock.JewelleryCopperHoopEarrings());

            InstallCatalogueItem(Historical01Stock.JewelleryShellCopperAnklets());

            InstallCatalogueItem(Historical01Stock.JewelleryBlueGlassRamAmulet());

            InstallCatalogueItem(Historical01Stock.JewelleryBoneHairPin());

            InstallCatalogueItem(Historical01Stock.JewelleryGoldRamHeadDiadem());

            InstallCatalogueItem(Historical01Stock.JewelleryGoldCarnelianBroadCollar());

            InstallCatalogueItem(Historical01Stock.JewelleryGoldLapisBracelets());

            InstallCatalogueItem(Historical01Stock.JewelleryGoldJasperRing());

            InstallCatalogueItem(Historical01Stock.JewelleryGoldRamHeadEarrings());

            InstallCatalogueItem(Historical01Stock.JewelleryGoldCarnelianBeadAnklets());

            InstallCatalogueItem(Historical01Stock.JewelleryGoldRamHeadPectoral());

            InstallCatalogueItem(Historical01Stock.JewelleryGoldLapisHairPin());
        }

        private void SeedAntiquityArmour()
        {
            #region Celtic
            InstallCatalogueItem(Historical01Stock.AntiquityCelticPeakedBronzeHelmet());

            InstallCatalogueItem(Historical01Stock.AntiquityCelticCrestedBronzeHelmet());

            InstallCatalogueItem(Historical01Stock.AntiquityCelticRivetedIronMailShirt());

            InstallCatalogueItem(Historical01Stock.AntiquityCelticFineRivetedMailShirt());

            InstallCatalogueItem(Historical01Stock.AntiquityCelticDyedLeatherScaleVest());

            InstallCatalogueItem(Historical01Stock.AntiquityCelticBronzeStuddedLeatherBelt());

            InstallCatalogueItem(Historical01Stock.AntiquityCelticLeatherWarBracers());

            InstallCatalogueItem(Historical01Stock.AntiquityCelticFurLinedWarBoots());

            #endregion

            #region Germanic
            InstallCatalogueItem(Historical01Stock.AntiquityGermanicIronBandedHelmet());

            InstallCatalogueItem(Historical01Stock.AntiquityGermanicDecoratedSpangenhelm());

            InstallCatalogueItem(Historical01Stock.AntiquityGermanicIronMailCoat());

            InstallCatalogueItem(Historical01Stock.AntiquityGermanicPaddedWoolUndercoat());

            InstallCatalogueItem(Historical01Stock.AntiquityGermanicBroadLeatherWarBelt());

            InstallCatalogueItem(Historical01Stock.AntiquityGermanicHideWarBracers());

            InstallCatalogueItem(Historical01Stock.AntiquityGermanicFurCuffedHighBoots());

            #endregion

            #region Etruscan
            InstallCatalogueItem(Historical01Stock.AntiquityEtruscanCheekGuardedBronzeHelmet());

            InstallCatalogueItem(Historical01Stock.AntiquityEtruscanAnatomicalBronzeCuirass());

            InstallCatalogueItem(Historical01Stock.AntiquityEtruscanBronzeLeftGreave());

            InstallCatalogueItem(Historical01Stock.AntiquityEtruscanPairBronzeGreaves());

            InstallCatalogueItem(Historical01Stock.AntiquityEtruscanRoundBronzePectoral());

            InstallCatalogueItem(Historical01Stock.AntiquityEtruscanSilverInlaidBronzeCuirass());

            InstallCatalogueItem(Historical01Stock.AntiquityEtruscanPterugesLeatherGirdle());

            InstallCatalogueItem(Historical01Stock.AntiquityEtruscanBronzeStuddedSandals());

            #endregion

            #region Italic/Roman
            InstallCatalogueItem(Historical01Stock.AntiquityRomanMontefortinoBronzeHelmet());

            InstallCatalogueItem(Historical01Stock.AntiquityRomanCrestedMontefortinoHelmet());

            InstallCatalogueItem(Historical01Stock.AntiquityRomanCoolusBronzeHelmet());

            InstallCatalogueItem(Historical01Stock.AntiquityRomanImperialGallicIronHelmet());

            InstallCatalogueItem(Historical01Stock.AntiquityRomanPlumedIronOfficerHelmet());

            InstallCatalogueItem(Historical01Stock.AntiquityRomanIntercisaIronRidgeHelmet());

            InstallCatalogueItem(Historical01Stock.AntiquityRomanBronzePectoralPlate());

            InstallCatalogueItem(Historical01Stock.AntiquityRomanTripleDiscBronzeCuirass());

            InstallCatalogueItem(Historical01Stock.AntiquityRomanLoricaHamataMailShirt());

            InstallCatalogueItem(Historical01Stock.AntiquityRomanLoricaHamataFineMailShirt());

            InstallCatalogueItem(Historical01Stock.AntiquityRomanLoricaSquamataScaleCuirass());

            InstallCatalogueItem(Historical01Stock.AntiquityRomanLoricaSquamataGildedCuirass());

            InstallCatalogueItem(Historical01Stock.AntiquityRomanLoricaSegmentataBandedCuirass());

            InstallCatalogueItem(Historical01Stock.AntiquityRomanPaddedSubarmalis());

            InstallCatalogueItem(Historical01Stock.AntiquityRomanFinePaddedSubarmalis());

            InstallCatalogueItem(Historical01Stock.AntiquityRomanPlatedMilitaryBelt());

            InstallCatalogueItem(Historical01Stock.AntiquityRomanApronedMilitaryBelt());

            InstallCatalogueItem(Historical01Stock.AntiquityRomanBronzeGreaves());

            InstallCatalogueItem(Historical01Stock.AntiquityRomanSegmentedIronArmguard());

            InstallCatalogueItem(Historical01Stock.AntiquityRomanReinforcedCaligae());

            InstallCatalogueItem(Historical01Stock.AntiquityRomanLateMailCoat());

            InstallCatalogueItem(Historical01Stock.AntiquityRomanLateScaleCoat());

            InstallCatalogueItem(Historical01Stock.AntiquityRomanLeatherFieldBoots());

            InstallCatalogueItem(Historical01Stock.AntiquityRomanBronzeMuscleCuirass());

            #endregion

            #region Hellenic
            InstallCatalogueItem(Historical01Stock.AntiquityHellenicCorinthianBronzeHelmet());

            InstallCatalogueItem(Historical01Stock.AntiquityHellenicCrestedBronzeHelmet());

            InstallCatalogueItem(Historical01Stock.AntiquityHellenicBronzeBellCuirass());

            InstallCatalogueItem(Historical01Stock.AntiquityHellenicBronzeMuscleCuirass());

            InstallCatalogueItem(Historical01Stock.AntiquityHellenicLinenLinothorax());

            InstallCatalogueItem(Historical01Stock.AntiquityHellenicFinePaintedLinothorax());

            InstallCatalogueItem(Historical01Stock.AntiquityHellenicBronzeGreaves());

            InstallCatalogueItem(Historical01Stock.AntiquityHellenicLeatherPterugesGirdle());

            InstallCatalogueItem(Historical01Stock.AntiquityHellenicBronzeVambraces());

            InstallCatalogueItem(Historical01Stock.AntiquityHellenicBronzeStuddedSandals());

            #endregion

            #region Phoenician/Punic
            InstallCatalogueItem(Historical01Stock.AntiquityPunicConicalBronzeHelmet());

            InstallCatalogueItem(Historical01Stock.AntiquityPunicCrestedBronzeHelmet());

            InstallCatalogueItem(Historical01Stock.AntiquityPunicLinenTubeCuirass());

            InstallCatalogueItem(Historical01Stock.AntiquityPunicFinePaintedCuirass());

            InstallCatalogueItem(Historical01Stock.AntiquityPunicIronMailCuirass());

            InstallCatalogueItem(Historical01Stock.AntiquityPunicGildedScaleCuirass());

            InstallCatalogueItem(Historical01Stock.AntiquityPunicBronzeGreaves());

            InstallCatalogueItem(Historical01Stock.AntiquityPunicBronzeStuddedGirdle());

            #endregion

            #region Persian
            InstallCatalogueItem(Historical01Stock.AntiquityPersianIronScaleCorselet());

            InstallCatalogueItem(Historical01Stock.AntiquityPersianBronzeScaleCorselet());

            InstallCatalogueItem(Historical01Stock.AntiquityPersianPaddedRidingCoat());

            InstallCatalogueItem(Historical01Stock.AntiquityPersianIronHelmetWithAventail());

            InstallCatalogueItem(Historical01Stock.AntiquityPersianFeltArmouredKyrbasia());

            InstallCatalogueItem(Historical01Stock.AntiquityPersianScaleAnaxyrides());

            InstallCatalogueItem(Historical01Stock.AntiquityPersianLamellarVambraces());

            InstallCatalogueItem(Historical01Stock.AntiquityPersianLamellarGreaves());

            InstallCatalogueItem(Historical01Stock.AntiquityPersianSoftRidingBoots());

            #endregion

            #region Egyptian
            InstallCatalogueItem(Historical01Stock.AntiquityEgyptianPaddedLinenWarCap());

            InstallCatalogueItem(Historical01Stock.AntiquityEgyptianBlueWarCrownCap());

            InstallCatalogueItem(Historical01Stock.AntiquityEgyptianLinenCorselet());

            InstallCatalogueItem(Historical01Stock.AntiquityEgyptianLeatherScaleCuirass());

            InstallCatalogueItem(Historical01Stock.AntiquityEgyptianBronzeScaleCuirass());

            InstallCatalogueItem(Historical01Stock.AntiquityEgyptianScaleKiltGuard());

            InstallCatalogueItem(Historical01Stock.AntiquityEgyptianLeatherArcherBracer());

            #endregion

            #region Anatolian
            InstallCatalogueItem(Historical01Stock.AntiquityAnatolianForwardCurvedBronzeHelmet());

            InstallCatalogueItem(Historical01Stock.AntiquityAnatolianIonianBronzeHelmet());

            InstallCatalogueItem(Historical01Stock.AntiquityAnatolianBronzeChestCuirass());

            InstallCatalogueItem(Historical01Stock.AntiquityAnatolianLinenArrowCuirass());

            InstallCatalogueItem(Historical01Stock.AntiquityAnatolianBronzeGreaves());

            InstallCatalogueItem(Historical01Stock.AntiquityAnatolianLeatherCavalryBoots());

            InstallCatalogueItem(Historical01Stock.AntiquityAnatolianFineWingedBronzeHelmet());

            InstallCatalogueItem(Historical01Stock.AntiquityAnatolianFinePaintedLinenCuirass());

            #endregion

            #region Scythian/Sarmatian
            InstallCatalogueItem(Historical01Stock.AntiquityScythianLeatherScaleCorselet());

            InstallCatalogueItem(Historical01Stock.AntiquityScythianConicalScaleCap());

            InstallCatalogueItem(Historical01Stock.AntiquityScythianFeltPaddedCap());

            InstallCatalogueItem(Historical01Stock.AntiquityScythianScaleTrousers());

            InstallCatalogueItem(Historical01Stock.AntiquityScythianHighRidingBoots());

            InstallCatalogueItem(Historical01Stock.AntiquitySarmatianIronScaleCuirass());

            InstallCatalogueItem(Historical01Stock.AntiquitySarmatianLamellarCuirass());

            InstallCatalogueItem(Historical01Stock.AntiquitySarmatianIronLaminarArmguards());

            InstallCatalogueItem(Historical01Stock.AntiquitySarmatianLamellarGreaves());

            InstallCatalogueItem(Historical01Stock.AntiquitySarmatianMailAventailHelmet());

            #endregion

            #region Nubian/Kushite
            InstallCatalogueItem(Historical01Stock.AntiquityKushitePaddedLinenArcherCap());

            InstallCatalogueItem(Historical01Stock.AntiquityKushiteReinforcedLinenCorselet());

            InstallCatalogueItem(Historical01Stock.AntiquityKushiteLeatherScaleBreastguard());

            InstallCatalogueItem(Historical01Stock.AntiquityKushiteBronzeScaleRoyalCuirass());

            InstallCatalogueItem(Historical01Stock.AntiquityKushiteLeatherArcherBracer());

            InstallCatalogueItem(Historical01Stock.AntiquityKushiteLeatherKiltGuard());

            InstallCatalogueItem(Historical01Stock.AntiquityKushiteSandArmouredSandals());

            #endregion
        }

        private void SeedAntiquityContainers()
        {
            #region Dry Containers
            InstallCatalogueItem(Historical01Stock.AntiquityOpenWillowCarryingBasket());

            InstallCatalogueItem(Historical01Stock.AntiquityLiddedWillowStorageBasket());

            InstallCatalogueItem(Historical01Stock.AntiquityCoarseHempGrainSack());

            InstallCatalogueItem(Historical01Stock.AntiquitySmallLinenSpiceSachet());

            InstallCatalogueItem(Historical01Stock.AntiquityRopeCordedTradeBundle());

            InstallCatalogueItem(Historical01Stock.AntiquityOakStaveDryCask());

            InstallCatalogueItem(Historical01Stock.AntiquityTallHoopedTradeBarrel());

            InstallCatalogueItem(Historical01Stock.AntiquityCheckedWoolMarketBag());

            InstallCatalogueItem(Historical01Stock.AntiquitySmokedHideMeatBag());

            InstallCatalogueItem(Historical01Stock.AntiquityTerracottaDryAmphora());

            InstallCatalogueItem(Historical01Stock.AntiquitySmallEarthenwareAmphoriskos());

            InstallCatalogueItem(Historical01Stock.AntiquityLiddedCeramicPyxis());

            InstallCatalogueItem(Historical01Stock.AntiquityBronzeCistaBox());

            InstallCatalogueItem(Historical01Stock.AntiquityCypressCapsaScrollBox());

            InstallCatalogueItem(Historical01Stock.AntiquityWaxTabletLocker());

            InstallCatalogueItem(Historical01Stock.AntiquityPaintedLarnaxChest());

            InstallCatalogueItem(Historical01Stock.AntiquityTallNarrowTradeAmphora());

            InstallCatalogueItem(Historical01Stock.AntiquityCedarTradeCasket());

            InstallCatalogueItem(Historical01Stock.AntiquityStackedSampleCase());

            InstallCatalogueItem(Historical01Stock.AntiquityLeatherDocumentCase());

            InstallCatalogueItem(Historical01Stock.AntiquityCedarScribeBox());

            InstallCatalogueItem(Historical01Stock.AntiquityWalnutGrainChest());

            InstallCatalogueItem(Historical01Stock.AntiquityPaintedBoxwoodCoffer());

            InstallCatalogueItem(Historical01Stock.AntiquityCedarCosmeticBox());

            InstallCatalogueItem(Historical01Stock.AntiquityPapyrusLiddedBasket());

            InstallCatalogueItem(Historical01Stock.AntiquityAlabasterKohlBox());

            InstallCatalogueItem(Historical01Stock.AntiquityAcaciaLinenChest());

            InstallCatalogueItem(Historical01Stock.AntiquityPlaitedPapyrusMarketBasket());

            InstallCatalogueItem(Historical01Stock.AntiquityEbonyInlaidCasket());

            InstallCatalogueItem(Historical01Stock.AntiquityFeltNomadBundle());

            InstallCatalogueItem(Historical01Stock.AntiquityLeatherMirrorCase());

            InstallCatalogueItem(Historical01Stock.AntiquityBirchbarkStorageBox());

            InstallCatalogueItem(Historical01Stock.AntiquityFlatWoodenCarryingTray());

            InstallCatalogueItem(Historical01Stock.AntiquityShallowOfferingBasket());

            InstallCatalogueItem(Historical01Stock.AntiquityBroadLinenStorageBag());

            InstallCatalogueItem(Historical01Stock.AntiquityIvoryChipJewelBox());

            InstallCatalogueItem(Historical01Stock.AntiquityPlainLeatherBeltPouch());

            InstallCatalogueItem(Historical01Stock.AntiquityLinenDrawstringPurse());

            InstallCatalogueItem(Historical01Stock.AntiquityHempShoulderTote());

            InstallCatalogueItem(Historical01Stock.AntiquityCanvasTravelPack());

            InstallCatalogueItem(Historical01Stock.AntiquityDoubleStrapTravelPack());

            InstallCatalogueItem(Historical01Stock.AntiquityWoolSlingBundle());

            InstallCatalogueItem(Historical01Stock.AntiquityCheckedWoolBeltPouch());

            InstallCatalogueItem(Historical01Stock.AntiquityFurLinedForagerBag());

            InstallCatalogueItem(Historical01Stock.AntiquityDeerLeatherGameBag());

            InstallCatalogueItem(Historical01Stock.AntiquityScrollCapsaSling());

            InstallCatalogueItem(Historical01Stock.AntiquityFoldedTabletWallet());

            InstallCatalogueItem(Historical01Stock.AntiquityRoundCoinPurse());

            InstallCatalogueItem(Historical01Stock.AntiquityBrightLinenTradePouch());

            InstallCatalogueItem(Historical01Stock.AntiquityPapyrusAmuletPouch());

            InstallCatalogueItem(Historical01Stock.AntiquityBeadedKohlPouch());

            InstallCatalogueItem(Historical01Stock.AntiquityPlaitedPapyrusShoulderBag());

            InstallCatalogueItem(Historical01Stock.AntiquityLeatherDispatchSatchel());

            InstallCatalogueItem(Historical01Stock.AntiquityFeltRidingPack());

            InstallCatalogueItem(Historical01Stock.AntiquityWideBeltDocumentPouch());

            InstallCatalogueItem(Historical01Stock.AntiquityWoolTabletSatchel());

            InstallCatalogueItem(Historical01Stock.AntiquitySteppeSaddlebagPack());

            InstallCatalogueItem(Historical01Stock.AntiquitySteppeGorytosCase());

            InstallCatalogueItem(Historical01Stock.AntiquityFeltMirrorPouch());

            InstallCatalogueItem(Historical01Stock.AntiquityFurProvisionPouch());

            InstallCatalogueItem(Historical01Stock.AntiquityLowOpenShelf());

            InstallCatalogueItem(Historical01Stock.AntiquityWideTradeShelves());

            InstallCatalogueItem(Historical01Stock.AntiquityScrollPigeonholeShelves());

            InstallCatalogueItem(Historical01Stock.AntiquityPotteryDisplayStand());

            InstallCatalogueItem(Historical01Stock.AntiquityFoldedTextileStand());

            InstallCatalogueItem(Historical01Stock.AntiquityLowMarketCounter());

            InstallCatalogueItem(Historical01Stock.AntiquitySmallOfferingTable());

            InstallCatalogueItem(Historical01Stock.AntiquityLongStoreTable());

            InstallCatalogueItem(Historical01Stock.AntiquityCypressStorageCupboard());

            InstallCatalogueItem(Historical01Stock.AntiquityPortableGarmentCabinet());

            InstallCatalogueItem(Historical01Stock.AntiquityAshArmourStand());

            InstallCatalogueItem(Historical01Stock.AntiquityOakWeaponRack());

            InstallCatalogueItem(Historical01Stock.AntiquityGlassFrontDisplayCase());

            InstallCatalogueItem(Historical01Stock.AntiquityAcaciaLinenStand());

            InstallCatalogueItem(Historical01Stock.AntiquityCedarCanopicChest());

            InstallCatalogueItem(Historical01Stock.AntiquityCedarDisplayHutch());

            InstallCatalogueItem(Historical01Stock.AntiquityCedarDocumentCabinet());

            InstallCatalogueItem(Historical01Stock.AntiquityWalnutSideboard());

            InstallCatalogueItem(Historical01Stock.AntiquityBirchCollapsibleRack());

            InstallCatalogueItem(Historical01Stock.AntiquityPapyrusGoodsStand());

            InstallCatalogueItem(Historical01Stock.AntiquityBronzeDisplayStand());

            InstallCatalogueItem(Historical01Stock.AntiquityOakTradeCabinet());

            #endregion

            #region Liquid Containers
            InstallCatalogueItem(Historical01Stock.AntiquityLiquidTerracottaWaterJar());

            InstallCatalogueItem(Historical01Stock.AntiquityLiquidPitchLinedStorageJar());

            InstallCatalogueItem(Historical01Stock.AntiquityLiquidSealedOilAmphora());

            InstallCatalogueItem(Historical01Stock.AntiquityLiquidCommonTradeAmphora());

            InstallCatalogueItem(Historical01Stock.AntiquityLiquidSmallAmphoriskos());

            InstallCatalogueItem(Historical01Stock.AntiquityLiquidCedarStopperedWineJar());

            InstallCatalogueItem(Historical01Stock.AntiquityLiquidOakWetCask());

            InstallCatalogueItem(Historical01Stock.AntiquityLiquidSmallCooperedRundlet());

            InstallCatalogueItem(Historical01Stock.AntiquityLiquidHoopedTradeBarrel());

            InstallCatalogueItem(Historical01Stock.AntiquityLiquidOakHogshead());

            InstallCatalogueItem(Historical01Stock.AntiquityLiquidAshAleCask());

            InstallCatalogueItem(Historical01Stock.AntiquityLiquidBronzeSitulaPail());

            InstallCatalogueItem(Historical01Stock.AntiquityLiquidBuccheroWinePitcher());

            InstallCatalogueItem(Historical01Stock.AntiquityLiquidBuccheroWaterHydria());

            InstallCatalogueItem(Historical01Stock.AntiquityLiquidDresselWineAmphora());

            InstallCatalogueItem(Historical01Stock.AntiquityLiquidSauceAmphora());

            InstallCatalogueItem(Historical01Stock.AntiquityLiquidRedSlippedServingJug());

            InstallCatalogueItem(Historical01Stock.AntiquityLiquidHydriaWaterJar());

            InstallCatalogueItem(Historical01Stock.AntiquityLiquidOinochoeWineJug());

            InstallCatalogueItem(Historical01Stock.AntiquityLiquidWhiteGroundLekythos());

            InstallCatalogueItem(Historical01Stock.AntiquityLiquidRoundAryballos());

            InstallCatalogueItem(Historical01Stock.AntiquityLiquidSlenderAlabastron());

            InstallCatalogueItem(Historical01Stock.AntiquityLiquidBroadMixingKrater());

            InstallCatalogueItem(Historical01Stock.AntiquityLiquidNarrowPunicAmphora());

            InstallCatalogueItem(Historical01Stock.AntiquityLiquidPunicOilFlask());

            InstallCatalogueItem(Historical01Stock.AntiquityLiquidCedarStopperedMerchantJar());

            InstallCatalogueItem(Historical01Stock.AntiquityLiquidGlazedWineEwer());

            InstallCatalogueItem(Historical01Stock.AntiquityLiquidBronzeCaravanWaterJar());

            InstallCatalogueItem(Historical01Stock.AntiquityLiquidCalcitePerfumeJar());

            InstallCatalogueItem(Historical01Stock.AntiquityLiquidRedClayWaterJar());

            InstallCatalogueItem(Historical01Stock.AntiquityLiquidBlueGlazedUnguentJar());

            InstallCatalogueItem(Historical01Stock.AntiquityLiquidRedSlippedAnatolianPitcher());

            InstallCatalogueItem(Historical01Stock.AntiquityLiquidAnatolianStagRhyton());

            InstallCatalogueItem(Historical01Stock.AntiquityLiquidSteppeMilkSkin());

            InstallCatalogueItem(Historical01Stock.AntiquityLiquidBlackToppedWaterJar());

            InstallCatalogueItem(Historical01Stock.AntiquityLiquidMeroiticPaintedBeerJar());

            InstallCatalogueItem(Historical01Stock.AntiquityLiquidPlainLeatherWaterskin());

            InstallCatalogueItem(Historical01Stock.AntiquityLiquidWideMouthWaterskin());

            InstallCatalogueItem(Historical01Stock.AntiquityLiquidLinenSlungClayCanteen());

            InstallCatalogueItem(Historical01Stock.AntiquityLiquidBronzeBeltFlask());

            InstallCatalogueItem(Historical01Stock.AntiquityLiquidGlassPerfumePendant());

            InstallCatalogueItem(Historical01Stock.AntiquityLiquidLeatherBeltOilFlask());

            InstallCatalogueItem(Historical01Stock.AntiquityLiquidHideAleSkin());

            InstallCatalogueItem(Historical01Stock.AntiquityLiquidBirchStopperedMeadSkin());

            InstallCatalogueItem(Historical01Stock.AntiquityLiquidBronzeBeltAryballos());

            InstallCatalogueItem(Historical01Stock.AntiquityLiquidSoldierShoulderCanteen());

            InstallCatalogueItem(Historical01Stock.AntiquityLiquidAthleteOilAryballos());

            InstallCatalogueItem(Historical01Stock.AntiquityLiquidSlungLekythosFlask());

            InstallCatalogueItem(Historical01Stock.AntiquityLiquidPurpleGlassPerfumeFlask());

            InstallCatalogueItem(Historical01Stock.AntiquityLiquidSailorWaterSkin());

            InstallCatalogueItem(Historical01Stock.AntiquityLiquidCaravanWaterskin());

            InstallCatalogueItem(Historical01Stock.AntiquityLiquidSilverTippedBeltFlask());

            InstallCatalogueItem(Historical01Stock.AntiquityLiquidReedWrappedCanteen());

            InstallCatalogueItem(Historical01Stock.AntiquityLiquidAlabasterUnguentPendant());

            InstallCatalogueItem(Historical01Stock.AntiquityLiquidPapyrusSlungWaterJar());

            InstallCatalogueItem(Historical01Stock.AntiquityLiquidPaintedKushiteCanteen());

            InstallCatalogueItem(Historical01Stock.AntiquityLiquidFeltCoveredRidingCanteen());

            InstallCatalogueItem(Historical01Stock.AntiquityLiquidSaddleWaterskin());

            InstallCatalogueItem(Historical01Stock.AntiquityLiquidAnatolianBeltOilFlask());

            InstallCatalogueItem(Historical01Stock.AntiquityLiquidSteppeKumisSkin());

            InstallCatalogueItem(Historical01Stock.AntiquityLiquidSimpleClayDrinkingCup());

            InstallCatalogueItem(Historical01Stock.AntiquityLiquidCarvedWoodenCup());

            InstallCatalogueItem(Historical01Stock.AntiquityLiquidBronzeLibationBowl());

            InstallCatalogueItem(Historical01Stock.AntiquityLiquidSmallGlassUnguentBottle());

            InstallCatalogueItem(Historical01Stock.AntiquityLiquidPlainPouringJug());

            InstallCatalogueItem(Historical01Stock.AntiquityLiquidBroadTableKrater());

            InstallCatalogueItem(Historical01Stock.AntiquityLiquidLowWashBasin());

            InstallCatalogueItem(Historical01Stock.AntiquityLiquidBronzeFeastCauldron());

            InstallCatalogueItem(Historical01Stock.AntiquityLiquidHornDrinkingCup());

            InstallCatalogueItem(Historical01Stock.AntiquityLiquidBuccheroChalice());

            InstallCatalogueItem(Historical01Stock.AntiquityLiquidRedSlippedCup());

            InstallCatalogueItem(Historical01Stock.AntiquityLiquidShallowKylix());

            InstallCatalogueItem(Historical01Stock.AntiquityLiquidTallKantharos());

            InstallCatalogueItem(Historical01Stock.AntiquityLiquidGlassAlabastron());

            InstallCatalogueItem(Historical01Stock.AntiquityLiquidPaintedPunicCup());

            InstallCatalogueItem(Historical01Stock.AntiquityLiquidSilverCourtCup());

            InstallCatalogueItem(Historical01Stock.AntiquityLiquidAnimalHeadedRhyton());

            InstallCatalogueItem(Historical01Stock.AntiquityLiquidBlueGlassCosmeticBottle());

            InstallCatalogueItem(Historical01Stock.AntiquityLiquidBronzeHandwashingBasin());

            InstallCatalogueItem(Historical01Stock.AntiquityLiquidAnatolianLibationCup());

            InstallCatalogueItem(Historical01Stock.AntiquityLiquidSteppeBronzeKettle());

            InstallCatalogueItem(Historical01Stock.AntiquityLiquidTooledLeatherFlask());

            InstallCatalogueItem(Historical01Stock.AntiquityLiquidKermaBeaker());

            InstallCatalogueItem(Historical01Stock.AntiquityLiquidPaintedKushiteLibationBowl());

            #endregion

            #region Tableware
            InstallCatalogueItem(Historical01Stock.AntiquityTablewareCelticHornCup());

            InstallCatalogueItem(Historical01Stock.AntiquityTablewareCelticBronzeFeastingCup());

            InstallCatalogueItem(Historical01Stock.AntiquityTablewareCelticWoodenAleCup());

            InstallCatalogueItem(Historical01Stock.AntiquityTablewareGermanicWoodenBeaker());

            InstallCatalogueItem(Historical01Stock.AntiquityTablewareGermanicHornBeaker());

            InstallCatalogueItem(Historical01Stock.AntiquityTablewareEtruscanBuccheroChalice());

            InstallCatalogueItem(Historical01Stock.AntiquityTablewareEtruscanBuccheroKantharos());

            InstallCatalogueItem(Historical01Stock.AntiquityTablewareEtruscanBronzeLadleCup());

            InstallCatalogueItem(Historical01Stock.AntiquityTablewareItalicPlainCeramicPoculum());

            InstallCatalogueItem(Historical01Stock.AntiquityTablewareItalicTerraSigillataCup());

            InstallCatalogueItem(Historical01Stock.AntiquityTablewareItalicSilverWineCup());

            InstallCatalogueItem(Historical01Stock.AntiquityTablewareHellenicKylix());

            InstallCatalogueItem(Historical01Stock.AntiquityTablewareHellenicSkyphos());

            InstallCatalogueItem(Historical01Stock.AntiquityTablewareHellenicKantharos());

            InstallCatalogueItem(Historical01Stock.AntiquityTablewareHellenicPhiale());

            InstallCatalogueItem(Historical01Stock.AntiquityTablewarePunicRedSlipCup());

            InstallCatalogueItem(Historical01Stock.AntiquityTablewarePunicGlassCup());

            InstallCatalogueItem(Historical01Stock.AntiquityTablewarePunicBronzeLibationBowl());

            InstallCatalogueItem(Historical01Stock.AntiquityTablewarePersianLotiformCup());

            InstallCatalogueItem(Historical01Stock.AntiquityTablewarePersianBronzeRhyton());

            InstallCatalogueItem(Historical01Stock.AntiquityTablewarePersianShallowGoldBowl());

            InstallCatalogueItem(Historical01Stock.AntiquityTablewareEgyptianBlueGlassGoblet());

            InstallCatalogueItem(Historical01Stock.AntiquityTablewareEgyptianBakedClayBeerMug());

            InstallCatalogueItem(Historical01Stock.AntiquityTablewareEgyptianAlabasterCup());

            InstallCatalogueItem(Historical01Stock.AntiquityTablewareAnatolianAnimalRhyton());

            InstallCatalogueItem(Historical01Stock.AntiquityTablewareAnatolianBronzeCup());

            InstallCatalogueItem(Historical01Stock.AntiquityTablewareAnatolianRedBurnishedGoblet());

            InstallCatalogueItem(Historical01Stock.AntiquityTablewareScythianWoodenCup());

            InstallCatalogueItem(Historical01Stock.AntiquityTablewareScythianGoldCup());

            InstallCatalogueItem(Historical01Stock.AntiquityTablewareScythianLeatherTravelCup());

            InstallCatalogueItem(Historical01Stock.AntiquityTablewareNubianBlackBurnishedCup());

            InstallCatalogueItem(Historical01Stock.AntiquityTablewareNubianRedBlackGoblet());

            InstallCatalogueItem(Historical01Stock.AntiquityTablewareNubianBronzeDrinkingBowl());

            InstallCatalogueItem(Historical01Stock.AntiquityTablewareCelticWoodenTrencher());

            InstallCatalogueItem(Historical01Stock.AntiquityTablewareCelticBronzeServingBowl());

            InstallCatalogueItem(Historical01Stock.AntiquityTablewareGermanicWoodenFoodBowl());

            InstallCatalogueItem(Historical01Stock.AntiquityTablewareGermanicBreadTrencher());

            InstallCatalogueItem(Historical01Stock.AntiquityTablewareEtruscanBuccheroDish());

            InstallCatalogueItem(Historical01Stock.AntiquityTablewareEtruscanBronzeStemmedBowl());

            InstallCatalogueItem(Historical01Stock.AntiquityTablewareItalicTerraSigillataPlate());

            InstallCatalogueItem(Historical01Stock.AntiquityTablewareItalicPlainEarthenwarePlate());

            InstallCatalogueItem(Historical01Stock.AntiquityTablewareItalicSamianBowl());

            InstallCatalogueItem(Historical01Stock.AntiquityTablewareHellenicPinaxPlate());

            InstallCatalogueItem(Historical01Stock.AntiquityTablewareHellenicShallowMezeDish());

            InstallCatalogueItem(Historical01Stock.AntiquityTablewarePunicRedSlipBowl());

            InstallCatalogueItem(Historical01Stock.AntiquityTablewarePunicShallowFishPlate());

            InstallCatalogueItem(Historical01Stock.AntiquityTablewarePersianSilverFootedBowl());

            InstallCatalogueItem(Historical01Stock.AntiquityTablewarePersianBronzeDish());

            InstallCatalogueItem(Historical01Stock.AntiquityTablewareEgyptianBreadPlate());

            InstallCatalogueItem(Historical01Stock.AntiquityTablewareEgyptianAlabasterOfferingBowl());

            InstallCatalogueItem(Historical01Stock.AntiquityTablewareAnatolianRedBurnishedBowl());

            InstallCatalogueItem(Historical01Stock.AntiquityTablewareAnatolianBronzePlatterBowl());

            InstallCatalogueItem(Historical01Stock.AntiquityTablewareScythianWoodenMeatPlatter());

            InstallCatalogueItem(Historical01Stock.AntiquityTablewareScythianGoldOfferingDish());

            InstallCatalogueItem(Historical01Stock.AntiquityTablewareNubianBlackBurnishedBowl());

            InstallCatalogueItem(Historical01Stock.AntiquityTablewareNubianRedwarePlate());

            InstallCatalogueItem(Historical01Stock.AntiquityTablewareCelticBronzeFeastTray());

            InstallCatalogueItem(Historical01Stock.AntiquityTablewareCelticOakServingBoard());

            InstallCatalogueItem(Historical01Stock.AntiquityTablewareGermanicWoodenServingTray());

            InstallCatalogueItem(Historical01Stock.AntiquityTablewareEtruscanBronzeDisplayTray());

            InstallCatalogueItem(Historical01Stock.AntiquityTablewareEtruscanBuccheroPlatter());

            InstallCatalogueItem(Historical01Stock.AntiquityTablewareItalicLanxPlatter());

            InstallCatalogueItem(Historical01Stock.AntiquityTablewareItalicTerraSigillataServingDish());

            InstallCatalogueItem(Historical01Stock.AntiquityTablewareHellenicPaintedServingPinax());

            InstallCatalogueItem(Historical01Stock.AntiquityTablewareHellenicBronzeCupTray());

            InstallCatalogueItem(Historical01Stock.AntiquityTablewarePunicRedSlipServingPlatter());

            InstallCatalogueItem(Historical01Stock.AntiquityTablewarePunicBronzeTradeTray());

            InstallCatalogueItem(Historical01Stock.AntiquityTablewarePersianSilverCharger());

            InstallCatalogueItem(Historical01Stock.AntiquityTablewarePersianBronzeBanquetTray());

            InstallCatalogueItem(Historical01Stock.AntiquityTablewareEgyptianPalmwoodOfferingTray());

            InstallCatalogueItem(Historical01Stock.AntiquityTablewareEgyptianAlabasterServingStand());

            InstallCatalogueItem(Historical01Stock.AntiquityTablewareAnatolianBronzeCupStand());

            InstallCatalogueItem(Historical01Stock.AntiquityTablewareAnatolianPaintedCeramicPlatter());

            InstallCatalogueItem(Historical01Stock.AntiquityTablewareScythianHideLinedWoodenTray());

            InstallCatalogueItem(Historical01Stock.AntiquityTablewareScythianGoldMountPlatter());

            InstallCatalogueItem(Historical01Stock.AntiquityTablewareNubianBlackwareServingPlatter());

            InstallCatalogueItem(Historical01Stock.AntiquityTablewareNubianBronzeOfferingTray());
            #endregion
        }

        private void SeedAntiquityDoorsAndLocks()
        {
            InstallCatalogueItem(Historical01Stock.AntiquityOakHouseDoor());

            InstallCatalogueItem(Historical01Stock.AntiquityLockableOakHouseDoor());

            InstallCatalogueItem(Historical01Stock.AntiquityCedarPanelDoor());

            InstallCatalogueItem(Historical01Stock.AntiquityLockableCedarPanelDoor());

            InstallCatalogueItem(Historical01Stock.AntiquityAcaciaCourtyardDoor());

            InstallCatalogueItem(Historical01Stock.AntiquityLockableAcaciaCourtyardDoor());

            InstallCatalogueItem(Historical01Stock.AntiquityStuddedPlankDoor());

            InstallCatalogueItem(Historical01Stock.AntiquityLockableStuddedPlankDoor());

            InstallCatalogueItem(Historical01Stock.AntiquityCypressShrineDoor());

            InstallCatalogueItem(Historical01Stock.AntiquityLockableCypressShrineDoor());

            InstallCatalogueItem(Historical01Stock.AntiquityBronzeCladCivicDoor());

            InstallCatalogueItem(Historical01Stock.AntiquityLockableBronzeCladCivicDoor());

            InstallCatalogueItem(Historical01Stock.AntiquityPlankedCourtyardGate());

            InstallCatalogueItem(Historical01Stock.AntiquityLockablePlankedCourtyardGate());

            InstallCatalogueItem(Historical01Stock.AntiquityPalisadeCampGate());

            InstallCatalogueItem(Historical01Stock.AntiquityLockablePalisadeCampGate());

            InstallCatalogueItem(Historical01Stock.AntiquityFortifiedCompoundGate());

            InstallCatalogueItem(Historical01Stock.AntiquityLockableFortifiedCompoundGate());

            InstallCatalogueItem(Historical01Stock.AntiquityCityGateLeaf());

            InstallCatalogueItem(Historical01Stock.AntiquityLockableCityGateLeaf());

            InstallCatalogueItem(Historical01Stock.AntiquityFurDoorHanging());

            InstallCatalogueItem(Historical01Stock.AntiquityWovenWoolDoorCurtain());

            InstallCatalogueItem(Historical01Stock.AntiquityHeavyLinenDoorCloth());

            InstallCatalogueItem(Historical01Stock.AntiquityPatternedCarpetDoorHanging());

            InstallCatalogueItem(Historical01Stock.AntiquityBeadDoorHanging());

            InstallCatalogueItem(Historical01Stock.AntiquityReedDoorScreen());

            InstallCatalogueItem(Historical01Stock.AntiquityFeltTentDoorFlap());

            InstallCatalogueItem(Historical01Stock.AntiquityLeatherTentDoorFlap());

            InstallCatalogueItem(Historical01Stock.AntiquityPapyrusLashedLightDoor());

            InstallCatalogueItem(Historical01Stock.AntiquityWoodenBarredWicket());

            InstallCatalogueItem(Historical01Stock.AntiquityLockableWoodenBarredWicket());

            InstallCatalogueItem(Historical01Stock.AntiquityBronzeLatticedGate());

            InstallCatalogueItem(Historical01Stock.AntiquityLockableBronzeLatticedGate());

            InstallCatalogueItem(Historical01Stock.AntiquityWroughtIronBarredGate());

            InstallCatalogueItem(Historical01Stock.AntiquityLockableWroughtIronBarredGate());

            InstallCatalogueItem(Historical01Stock.AntiquityBronzeShrineGrille());

            InstallCatalogueItem(Historical01Stock.AntiquityLockableBronzeShrineGrille());

            InstallCatalogueItem(Historical01Stock.AntiquityCitySallyPortGrate());

            InstallCatalogueItem(Historical01Stock.AntiquityLockableCitySallyPortGrate());

            InstallCatalogueItem(Historical01Stock.AntiquityMassiveBarredGateway());

            InstallCatalogueItem(Historical01Stock.AntiquityLockableMassiveBarredGateway());

            InstallCatalogueItem(Historical01Stock.AntiquityWoodenPinLock());

            InstallCatalogueItem(Historical01Stock.AntiquityWoodenPinLockKey());

            InstallCatalogueItem(Historical01Stock.AntiquityBronzeWardedDoorLock());

            InstallCatalogueItem(Historical01Stock.AntiquityBronzeWardedKey());

            InstallCatalogueItem(Historical01Stock.AntiquityWroughtIronWardedDoorLock());

            InstallCatalogueItem(Historical01Stock.AntiquityWroughtIronWardedKey());

            InstallCatalogueItem(Historical01Stock.AntiquitySmallCasketTumblerLock());

            InstallCatalogueItem(Historical01Stock.AntiquitySmallCasketLiftKey());

            InstallCatalogueItem(Historical01Stock.AntiquityLargeGateBarLock());

            InstallCatalogueItem(Historical01Stock.AntiquityLargeGateKey());

            InstallCatalogueItem(Historical01Stock.AntiquityPoorIronSlideLock());

            InstallCatalogueItem(Historical01Stock.AntiquityPlainSlideKey());

            InstallCatalogueItem(Historical01Stock.AntiquityFineBronzeTumblerLock());

            InstallCatalogueItem(Historical01Stock.AntiquityFineBronzeLiftKey());

            InstallCatalogueItem(Historical01Stock.AntiquityBronzeLeverDoorLock());

            InstallCatalogueItem(Historical01Stock.AntiquityTwoToothedBronzeLeverKey());

            InstallCatalogueItem(Historical01Stock.AntiquityWroughtIronGateLeverLock());

            InstallCatalogueItem(Historical01Stock.AntiquityLongIronGateKey());

            InstallCatalogueItem(Historical01Stock.AntiquityTempleBarLeverLock());

            InstallCatalogueItem(Historical01Stock.AntiquityCeremonialTempleKey());

            InstallCatalogueItem(Historical01Stock.AntiquityWoodenCrossbarLatch());

            InstallCatalogueItem(Historical01Stock.AntiquityWroughtIronDrawLatch());

            InstallCatalogueItem(Historical01Stock.AntiquityBronzeShrineLatch());

            InstallCatalogueItem(Historical01Stock.AntiquitySmallBronzeKeyring());

            InstallCatalogueItem(Historical01Stock.AntiquityLargeIronKeyring());

        }

        private void SeedAntiquityRepairKits()
        {
            InstallCatalogueItem(Historical01Stock.AntiquityTextileRepairKit());

            InstallCatalogueItem(Historical01Stock.AntiquityLeatherRepairKit());

            InstallCatalogueItem(Historical01Stock.AntiquityWoodRepairKit());

            InstallCatalogueItem(Historical01Stock.AntiquityMetalRepairKit());

            InstallCatalogueItem(Historical01Stock.AntiquityStoneRepairKit());

            InstallCatalogueItem(Historical01Stock.AntiquityCeramicRepairKit());

            InstallCatalogueItem(Historical01Stock.AntiquityHardOrganicRepairKit());

            InstallCatalogueItem(Historical01Stock.AntiquityFieldRepairBundle());
        }

        private void SeedAntiquityHouseholdFurniture()
        {
            InstallCatalogueItem(Historical01Stock.AntiquityPlainWoodenStool());

            InstallCatalogueItem(Historical01Stock.AntiquityRushSeatedStool());

            InstallCatalogueItem(Historical01Stock.AntiquityThreeLeggedCampStool());

            InstallCatalogueItem(Historical01Stock.AntiquityFoldingCuruleStool());

            InstallCatalogueItem(Historical01Stock.AntiquityLowBoxStool());

            InstallCatalogueItem(Historical01Stock.AntiquityCarvedAnimalLeggedStool());

            InstallCatalogueItem(Historical01Stock.AntiquityPlainPlankBench());

            InstallCatalogueItem(Historical01Stock.AntiquityLongHallBench());

            InstallCatalogueItem(Historical01Stock.AntiquityCarvedHouseholdBench());

            InstallCatalogueItem(Historical01Stock.AntiquityLowBackedChair());

            InstallCatalogueItem(Historical01Stock.AntiquityCurvedBackKlismosChair());

            InstallCatalogueItem(Historical01Stock.AntiquityHighBackEliteChair());

            InstallCatalogueItem(Historical01Stock.AntiquityLowDiningCouch());

            InstallCatalogueItem(Historical01Stock.AntiquityFineBanquetCouch());

            InstallCatalogueItem(Historical01Stock.AntiquityPaddedDaybed());

            InstallCatalogueItem(Historical01Stock.AntiquitySmallLowTable());

            InstallCatalogueItem(Historical01Stock.AntiquityPlainTrestleTable());

            InstallCatalogueItem(Historical01Stock.AntiquityLargeFeastingTable());

            InstallCatalogueItem(Historical01Stock.AntiquityRoundThreeLeggedTable());

            InstallCatalogueItem(Historical01Stock.AntiquityBronzeFootedTable());

            InstallCatalogueItem(Historical01Stock.AntiquityInlaidLuxuryTable());

            InstallCatalogueItem(Historical01Stock.AntiquityPlainSideTable());

            InstallCatalogueItem(Historical01Stock.AntiquityPaintedSideTable());

            InstallCatalogueItem(Historical01Stock.AntiquityBronzeTripodStand());

            InstallCatalogueItem(Historical01Stock.AntiquityPlainWritingTable());

            InstallCatalogueItem(Historical01Stock.AntiquitySlopedWritingDesk());

            InstallCatalogueItem(Historical01Stock.AntiquityDraweredDeskChest());

            InstallCatalogueItem(Historical01Stock.AntiquityLowDisplayStand());

            InstallCatalogueItem(Historical01Stock.AntiquitySimpleSleepingMat());

            InstallCatalogueItem(Historical01Stock.AntiquityWovenReedPallet());

            InstallCatalogueItem(Historical01Stock.AntiquityWoollenBedroll());

            InstallCatalogueItem(Historical01Stock.AntiquityHideSleepingRoll());

            InstallCatalogueItem(Historical01Stock.AntiquityPlainWoodenBed());

            InstallCatalogueItem(Historical01Stock.AntiquityRopeStrungBed());

            InstallCatalogueItem(Historical01Stock.AntiquityCarvedEliteBed());

            InstallCatalogueItem(Historical01Stock.AntiquityWoodenHeadrest());

            InstallCatalogueItem(Historical01Stock.AntiquityFineCarvedHeadrest());

            InstallCatalogueItem(Historical01Stock.AntiquityLinenBolster());

            InstallCatalogueItem(Historical01Stock.AntiquityWoolSleepingBlanket());

            InstallCatalogueItem(Historical01Stock.AntiquityFinePatternedCoverlet());

            InstallCatalogueItem(Historical01Stock.AntiquityPlainStorageChest());

            InstallCatalogueItem(Historical01Stock.AntiquityBronzeBoundChest());

            InstallCatalogueItem(Historical01Stock.AntiquityLowClothingCoffer());

            InstallCatalogueItem(Historical01Stock.AntiquitySmallHouseholdCupboard());

            InstallCatalogueItem(Historical01Stock.AntiquityLargeStorageCupboard());

            InstallCatalogueItem(Historical01Stock.AntiquityOpenWallShelf());

            InstallCatalogueItem(Historical01Stock.AntiquityNarrowDisplayShelves());

            InstallCatalogueItem(Historical01Stock.AntiquityWideHouseholdShelves());

            InstallCatalogueItem(Historical01Stock.AntiquityDocumentShelves());

            InstallCatalogueItem(Historical01Stock.AntiquityLowSideboard());

            InstallCatalogueItem(Historical01Stock.AntiquityGlassDisplayCase());

            InstallCatalogueItem(Historical01Stock.AntiquityPortableLacqueredBox());

            InstallCatalogueItem(Historical01Stock.AntiquitySimpleClayOilLamp());

            InstallCatalogueItem(Historical01Stock.AntiquityMouldedTerracottaLamp());

            InstallCatalogueItem(Historical01Stock.AntiquityBronzeOilLamp());

            InstallCatalogueItem(Historical01Stock.AntiquityHangingBronzeLamp());

            InstallCatalogueItem(Historical01Stock.AntiquityTallLampStand());

            InstallCatalogueItem(Historical01Stock.AntiquityPlainWaxCandle());

            InstallCatalogueItem(Historical01Stock.AntiquityFineBeeswaxCandle());

            InstallCatalogueItem(Historical01Stock.AntiquityPitchSoakedTorch());

            InstallCatalogueItem(Historical01Stock.AntiquityLongBurningTorch());

            InstallCatalogueItem(Historical01Stock.AntiquityBronzeBrazier());

            InstallCatalogueItem(Historical01Stock.AntiquityIronFireBasket());

            InstallCatalogueItem(Historical01Stock.AntiquityEarthenwareBrazier());

            InstallCatalogueItem(Historical01Stock.AntiquityPortableIncenseBurner());

            InstallCatalogueItem(Historical01Stock.AntiquitySimpleWovenFloorMat());

            InstallCatalogueItem(Historical01Stock.AntiquityPlainWoolFloorRug());

            InstallCatalogueItem(Historical01Stock.AntiquityPatternedWoolCarpet());

            InstallCatalogueItem(Historical01Stock.AntiquityFeltTentCarpet());

            InstallCatalogueItem(Historical01Stock.AntiquityCheckedWoolHanging());

            InstallCatalogueItem(Historical01Stock.AntiquityFineEmbroideredHanging());

            InstallCatalogueItem(Historical01Stock.AntiquityLinenDoorCurtain());

            InstallCatalogueItem(Historical01Stock.AntiquityHeavyWoolCurtain());

            InstallCatalogueItem(Historical01Stock.AntiquityLeatherFloorCover());

            InstallCatalogueItem(Historical01Stock.AntiquityPlainLinenCushion());

            InstallCatalogueItem(Historical01Stock.AntiquityStuffedWoolCushion());

            InstallCatalogueItem(Historical01Stock.AntiquityFinePatternedCushion());

            InstallCatalogueItem(Historical01Stock.AntiquityFurThrow());

            InstallCatalogueItem(Historical01Stock.AntiquityFineBorderedTablecloth());

            InstallCatalogueItem(Historical01Stock.AntiquitySmallPaintedPlaque());

            InstallCatalogueItem(Historical01Stock.AntiquityHouseholdTerracottaFigurine());

            InstallCatalogueItem(Historical01Stock.AntiquityBronzeHouseholdFigurine());

            InstallCatalogueItem(Historical01Stock.AntiquityAlabasterDisplayBowl());

            InstallCatalogueItem(Historical01Stock.AntiquityGlassDisplayBowl());

            InstallCatalogueItem(Historical01Stock.AntiquityBronzeMirrorStand());

            InstallCatalogueItem(Historical01Stock.AntiquityIvoryInlaidBox());

            InstallCatalogueItem(Historical01Stock.AntiquityPaintedHouseholdShrineStand());

            InstallCatalogueItem(Historical01Stock.AntiquityCarvedWoodenScreen());

            InstallCatalogueItem(Historical01Stock.AntiquitySimplePartitionScreen());
        }

        private void SeedAntiquityWeaponsShieldsAccessories()
        {
            #region Common Weapons and Accessories
            InstallCatalogueItem(Historical01Stock.AntiquityWeaponCommonBronzeUtilityKnife());

            InstallCatalogueItem(Historical01Stock.AntiquityWeaponCommonIronUtilityKnife());

            InstallCatalogueItem(Historical01Stock.AntiquityWeaponCommonBronzeDagger());

            InstallCatalogueItem(Historical01Stock.AntiquityWeaponCommonIronDagger());

            InstallCatalogueItem(Historical01Stock.AntiquityWeaponCommonHardwoodClub());

            InstallCatalogueItem(Historical01Stock.AntiquityWeaponCommonHardwoodQuarterstaff());

            InstallCatalogueItem(Historical01Stock.AntiquityWeaponCommonShortSelfBow());

            InstallCatalogueItem(Historical01Stock.AntiquityWeaponCommonSimpleSling());

            InstallCatalogueItem(Historical01Stock.AntiquityWeaponCommonStaffSling());

            InstallCatalogueItem(Historical01Stock.AntiquityAmmoCommonFieldPointArrow());

            InstallCatalogueItem(Historical01Stock.AntiquityAmmoCommonBroadheadArrow());

            InstallCatalogueItem(Historical01Stock.AntiquityAmmoCommonTargetArrow());

            InstallCatalogueItem(Historical01Stock.AntiquityAmmoCommonFieldPointBolt());

            InstallCatalogueItem(Historical01Stock.AntiquityAmmoCommonBroadheadBolt());

            InstallCatalogueItem(Historical01Stock.AntiquityAmmoCommonStoneSlingBullet());

            InstallCatalogueItem(Historical01Stock.AntiquityAmmoCommonLeadSlingBullet());

            InstallCatalogueItem(Historical01Stock.AntiquityShieldCommonRoundHideShield());

            InstallCatalogueItem(Historical01Stock.AntiquityShieldCommonRoundWoodenShield());

            InstallCatalogueItem(Historical01Stock.AntiquityShieldCommonWickerShield());

            InstallCatalogueItem(Historical01Stock.AntiquityShieldCommonSmallBossedShield());

            InstallCatalogueItem(Historical01Stock.AntiquityAccessoryCommonLeatherArrowQuiver());

            InstallCatalogueItem(Historical01Stock.AntiquityAccessoryCommonTallArrowQuiver());

            InstallCatalogueItem(Historical01Stock.AntiquityAccessoryCommonSlingBulletPouch());

            InstallCatalogueItem(Historical01Stock.AntiquityAccessoryCommonLeadBulletPouch());

            InstallCatalogueItem(Historical01Stock.AntiquityAccessoryCommonSmallKnifeSheath());

            InstallCatalogueItem(Historical01Stock.AntiquityAccessoryCommonDaggerSheath());

            InstallCatalogueItem(Historical01Stock.AntiquityAccessoryCommonStraightSwordScabbard());

            InstallCatalogueItem(Historical01Stock.AntiquityAccessoryCommonLongSwordScabbard());

            InstallCatalogueItem(Historical01Stock.AntiquityAccessoryCommonLargeWeaponSling());

            InstallCatalogueItem(Historical01Stock.AntiquityAccessoryCommonJavelinQuiver());

            InstallCatalogueItem(Historical01Stock.AntiquityAccessoryCommonBroadWeaponBelt());

            #endregion

            #region Celtic
            InstallCatalogueItem(Historical01Stock.AntiquityWeaponCelticLongIronSlashingSword());

            InstallCatalogueItem(Historical01Stock.AntiquityWeaponCelticFineLongIronSword());

            InstallCatalogueItem(Historical01Stock.AntiquityWeaponCelticLeafBladedSpear());

            InstallCatalogueItem(Historical01Stock.AntiquityWeaponCelticLightThrowingJavelin());

            InstallCatalogueItem(Historical01Stock.AntiquityWeaponCelticIronWarAxe());

            InstallCatalogueItem(Historical01Stock.AntiquityShieldCelticLongOvalPaintedShield());

            InstallCatalogueItem(Historical01Stock.AntiquityShieldCelticRoundBossedShield());

            InstallCatalogueItem(Historical01Stock.AntiquityAccessoryCelticDecoratedSwordScabbard());

            InstallCatalogueItem(Historical01Stock.AntiquityAccessoryCelticBroadBaldric());

            #endregion

            #region Germanic
            InstallCatalogueItem(Historical01Stock.AntiquityWeaponGermanicFrameaSpear());

            InstallCatalogueItem(Historical01Stock.AntiquityWeaponGermanicBarbedAngonJavelin());

            InstallCatalogueItem(Historical01Stock.AntiquityWeaponGermanicBroadSeax());

            InstallCatalogueItem(Historical01Stock.AntiquityWeaponGermanicThrowingAxe());

            InstallCatalogueItem(Historical01Stock.AntiquityWeaponGermanicLongWarSword());

            InstallCatalogueItem(Historical01Stock.AntiquityShieldGermanicRoundLindenShield());

            InstallCatalogueItem(Historical01Stock.AntiquityShieldGermanicPaintedRoundShield());

            InstallCatalogueItem(Historical01Stock.AntiquityAccessoryGermanicSeaxSheath());

            InstallCatalogueItem(Historical01Stock.AntiquityAccessoryGermanicAxeBeltLoop());

            #endregion

            #region Italic / Roman
            InstallCatalogueItem(Historical01Stock.AntiquityWeaponRomanEarlyHastaSpear());

            InstallCatalogueItem(Historical01Stock.AntiquityWeaponRomanRepublicanGladius());

            InstallCatalogueItem(Historical01Stock.AntiquityWeaponRomanMainzGladius());

            InstallCatalogueItem(Historical01Stock.AntiquityWeaponRomanPompeiiGladius());

            InstallCatalogueItem(Historical01Stock.AntiquityWeaponRomanLateSpatha());

            InstallCatalogueItem(Historical01Stock.AntiquityWeaponRomanPugioDagger());

            InstallCatalogueItem(Historical01Stock.AntiquityWeaponRomanVerutumJavelin());

            InstallCatalogueItem(Historical01Stock.AntiquityWeaponRomanHeavyPilum());

            InstallCatalogueItem(Historical01Stock.AntiquityWeaponRomanWeightedPlumbata());

            InstallCatalogueItem(Historical01Stock.AntiquityWeaponRomanDolabra());

            InstallCatalogueItem(Historical01Stock.AntiquityWeaponRomanContusLance());

            InstallCatalogueItem(Historical01Stock.AntiquityWeaponRomanCompositeBow());

            InstallCatalogueItem(Historical01Stock.AntiquityShieldRomanRoundClipeus());

            InstallCatalogueItem(Historical01Stock.AntiquityShieldRomanRepublicanOvalScutum());

            InstallCatalogueItem(Historical01Stock.AntiquityShieldRomanCurvedRectangularScutum());

            InstallCatalogueItem(Historical01Stock.AntiquityShieldRomanAuxiliaryOvalShield());

            InstallCatalogueItem(Historical01Stock.AntiquityShieldRomanParma());

            InstallCatalogueItem(Historical01Stock.AntiquityShieldRomanLateOvalShield());

            InstallCatalogueItem(Historical01Stock.AntiquityAccessoryRomanGladiusScabbard());

            InstallCatalogueItem(Historical01Stock.AntiquityAccessoryRomanPugioSheath());

            InstallCatalogueItem(Historical01Stock.AntiquityAccessoryRomanSpathaScabbard());

            InstallCatalogueItem(Historical01Stock.AntiquityAccessoryRomanPilumCarryingSleeve());

            InstallCatalogueItem(Historical01Stock.AntiquityAccessoryRomanPlumbataPouch());

            InstallCatalogueItem(Historical01Stock.AntiquityAccessoryRomanMilitaryWeaponBelt());

            #endregion

            #region Hellenic
            InstallCatalogueItem(Historical01Stock.AntiquityWeaponHellenicDorySpear());

            InstallCatalogueItem(Historical01Stock.AntiquityWeaponHellenicXiphos());

            InstallCatalogueItem(Historical01Stock.AntiquityWeaponHellenicKopis());

            InstallCatalogueItem(Historical01Stock.AntiquityWeaponHellenicSarissaPike());

            InstallCatalogueItem(Historical01Stock.AntiquityWeaponHellenicLightJavelin());

            InstallCatalogueItem(Historical01Stock.AntiquityWeaponHellenicBellyDrawnBow());

            InstallCatalogueItem(Historical01Stock.AntiquityWeaponHellenicShortWarBow());

            InstallCatalogueItem(Historical01Stock.AntiquityShieldHellenicAspis());

            InstallCatalogueItem(Historical01Stock.AntiquityShieldHellenicPelta());

            InstallCatalogueItem(Historical01Stock.AntiquityShieldHellenicThureos());

            InstallCatalogueItem(Historical01Stock.AntiquityAccessoryHellenicXiphosScabbard());

            InstallCatalogueItem(Historical01Stock.AntiquityAccessoryHellenicKopisSheath());

            InstallCatalogueItem(Historical01Stock.AntiquityAccessoryHellenicBoltQuiver());

            #endregion

            #region Phoenician / Punic
            InstallCatalogueItem(Historical01Stock.AntiquityWeaponPunicSocketedInfantrySpear());

            InstallCatalogueItem(Historical01Stock.AntiquityWeaponPunicShortStraightSword());

            InstallCatalogueItem(Historical01Stock.AntiquityWeaponPunicCurvedMercenarySword());

            InstallCatalogueItem(Historical01Stock.AntiquityWeaponPunicLightJavelin());

            InstallCatalogueItem(Historical01Stock.AntiquityWeaponPunicHeavyIronJavelin());

            InstallCatalogueItem(Historical01Stock.AntiquityWeaponPunicBalearicSling());

            InstallCatalogueItem(Historical01Stock.AntiquityWeaponPunicCompositeBow());

            InstallCatalogueItem(Historical01Stock.AntiquityShieldPunicRoundHideShield());

            InstallCatalogueItem(Historical01Stock.AntiquityShieldPunicOvalInfantryShield());

            InstallCatalogueItem(Historical01Stock.AntiquityShieldPunicWickerLightShield());

            InstallCatalogueItem(Historical01Stock.AntiquityAccessoryPunicJavelinQuiver());

            InstallCatalogueItem(Historical01Stock.AntiquityAccessoryPunicShortSwordSheath());

            #endregion

            #region Persian
            InstallCatalogueItem(Historical01Stock.AntiquityWeaponPersianAkinakes());

            InstallCatalogueItem(Historical01Stock.AntiquityWeaponPersianSagarisAxe());

            InstallCatalogueItem(Historical01Stock.AntiquityWeaponPersianLongSpear());

            InstallCatalogueItem(Historical01Stock.AntiquityWeaponPersianLightJavelin());

            InstallCatalogueItem(Historical01Stock.AntiquityWeaponPersianCompositeBow());

            InstallCatalogueItem(Historical01Stock.AntiquityAmmoPersianReedArrow());

            InstallCatalogueItem(Historical01Stock.AntiquityShieldPersianWickerSpara());

            InstallCatalogueItem(Historical01Stock.AntiquityShieldPersianRoundDhal());

            InstallCatalogueItem(Historical01Stock.AntiquityAccessoryPersianGorytosQuiver());

            InstallCatalogueItem(Historical01Stock.AntiquityAccessoryPersianAkinakesScabbard());

            InstallCatalogueItem(Historical01Stock.AntiquityAccessoryPersianBowcaseStrap());

            InstallCatalogueItem(Historical01Stock.AntiquityAccessoryPersianArrowPouch());

            #endregion

            #region Egyptian
            InstallCatalogueItem(Historical01Stock.AntiquityWeaponEgyptianKhopesh());

            InstallCatalogueItem(Historical01Stock.AntiquityWeaponEgyptianBronzeWarAxe());

            InstallCatalogueItem(Historical01Stock.AntiquityWeaponEgyptianStoneMace());

            InstallCatalogueItem(Historical01Stock.AntiquityWeaponEgyptianBronzeMace());

            InstallCatalogueItem(Historical01Stock.AntiquityWeaponEgyptianShortSpear());

            InstallCatalogueItem(Historical01Stock.AntiquityWeaponEgyptianLightJavelin());

            InstallCatalogueItem(Historical01Stock.AntiquityWeaponEgyptianSimpleSelfBow());

            InstallCatalogueItem(Historical01Stock.AntiquityWeaponEgyptianCompositeBow());

            InstallCatalogueItem(Historical01Stock.AntiquityAmmoEgyptianReedArrow());

            InstallCatalogueItem(Historical01Stock.AntiquityShieldEgyptianHideShield());

            InstallCatalogueItem(Historical01Stock.AntiquityShieldEgyptianPapyrusShield());

            InstallCatalogueItem(Historical01Stock.AntiquityAccessoryEgyptianKhopeshSheath());

            InstallCatalogueItem(Historical01Stock.AntiquityAccessoryEgyptianLinenArrowQuiver());

            #endregion

            #region Etruscan
            InstallCatalogueItem(Historical01Stock.AntiquityWeaponEtruscanBronzeTippedSpear());

            InstallCatalogueItem(Historical01Stock.AntiquityWeaponEtruscanLightJavelin());

            InstallCatalogueItem(Historical01Stock.AntiquityWeaponEtruscanShortSword());

            InstallCatalogueItem(Historical01Stock.AntiquityWeaponEtruscanCurvedSword());

            InstallCatalogueItem(Historical01Stock.AntiquityWeaponEtruscanBronzeDagger());

            InstallCatalogueItem(Historical01Stock.AntiquityShieldEtruscanRoundBronzeShield());

            InstallCatalogueItem(Historical01Stock.AntiquityShieldEtruscanLargeAspis());

            InstallCatalogueItem(Historical01Stock.AntiquityAccessoryEtruscanBronzeFittedScabbard());

            InstallCatalogueItem(Historical01Stock.AntiquityAccessoryEtruscanSpearSling());

            InstallCatalogueItem(Historical01Stock.AntiquityAccessoryEtruscanDaggerSheath());

            #endregion

            #region Anatolian
            InstallCatalogueItem(Historical01Stock.AntiquityWeaponAnatolianBronzeSpear());

            InstallCatalogueItem(Historical01Stock.AntiquityWeaponAnatolianCrescentAxe());

            InstallCatalogueItem(Historical01Stock.AntiquityWeaponAnatolianCurvedShortSword());

            InstallCatalogueItem(Historical01Stock.AntiquityWeaponAnatolianLongDagger());

            InstallCatalogueItem(Historical01Stock.AntiquityWeaponAnatolianLightJavelin());

            InstallCatalogueItem(Historical01Stock.AntiquityWeaponAnatolianCompositeBow());

            InstallCatalogueItem(Historical01Stock.AntiquityShieldAnatolianTallHideShield());

            InstallCatalogueItem(Historical01Stock.AntiquityShieldAnatolianWickerPelta());

            InstallCatalogueItem(Historical01Stock.AntiquityAccessoryAnatolianArrowQuiver());

            InstallCatalogueItem(Historical01Stock.AntiquityAccessoryAnatolianCurvedSwordSheath());

            #endregion

            #region Scythian / Sarmatian
            InstallCatalogueItem(Historical01Stock.AntiquityWeaponScythianAkinakes());

            InstallCatalogueItem(Historical01Stock.AntiquityWeaponScythianSagaris());

            InstallCatalogueItem(Historical01Stock.AntiquityWeaponSarmatianKontosLance());

            InstallCatalogueItem(Historical01Stock.AntiquityWeaponScythianLightJavelin());

            InstallCatalogueItem(Historical01Stock.AntiquityWeaponScythianCompositeBow());

            InstallCatalogueItem(Historical01Stock.AntiquityAmmoScythianBarbedArrow());

            InstallCatalogueItem(Historical01Stock.AntiquityAccessoryScythianGorytos());

            InstallCatalogueItem(Historical01Stock.AntiquityShieldScythianSmallHideShield());

            InstallCatalogueItem(Historical01Stock.AntiquityShieldSarmatianRoundTarge());

            InstallCatalogueItem(Historical01Stock.AntiquityAccessoryScythianAxeLoop());

            InstallCatalogueItem(Historical01Stock.AntiquityAccessoryScythianAkinakesScabbard());

            #endregion

            #region Nubian / Kushite
            InstallCatalogueItem(Historical01Stock.AntiquityWeaponKushiteLongSelfBow());

            InstallCatalogueItem(Historical01Stock.AntiquityWeaponKushiteCompactBow());

            InstallCatalogueItem(Historical01Stock.AntiquityAmmoKushiteCaneArrow());

            InstallCatalogueItem(Historical01Stock.AntiquityAmmoKushiteBarbedArrow());

            InstallCatalogueItem(Historical01Stock.AntiquityWeaponKushiteIronSpear());

            InstallCatalogueItem(Historical01Stock.AntiquityWeaponKushiteLightJavelin());

            InstallCatalogueItem(Historical01Stock.AntiquityWeaponKushiteHandAxe());

            InstallCatalogueItem(Historical01Stock.AntiquityWeaponKushiteShortSword());

            InstallCatalogueItem(Historical01Stock.AntiquityWeaponKushiteIronDagger());

            InstallCatalogueItem(Historical01Stock.AntiquityShieldKushiteLargeOxhideShield());

            InstallCatalogueItem(Historical01Stock.AntiquityShieldKushiteWickerHideShield());

            InstallCatalogueItem(Historical01Stock.AntiquityAccessoryKushiteTallArrowQuiver());

            InstallCatalogueItem(Historical01Stock.AntiquityAccessoryKushiteBowCarryingSling());

            #endregion
        }
    }
}
