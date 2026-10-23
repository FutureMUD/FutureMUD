#nullable enable

using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private void SeedMedievalClothing()
	{
		SeedMedievalItemsReusedByRenaissanceOutfits();

		InstallCatalogueItem(RepresentativeCatalogue.Braies());

		InstallCatalogueItem(Historical03Stock.MedievalLinenUndertunic());

		InstallCatalogueItem(Historical03Stock.MedievalLinenChemise());

		InstallCatalogueItem(Historical03Stock.MedievalLinenBreastband());

		InstallCatalogueItem(Historical03Stock.MedievalPlainLeatherBelt());

		InstallCatalogueItem(Historical03Stock.MedievalIronBuckledLeatherBelt());

		InstallCatalogueItem(Historical03Stock.MedievalSimpleWovenSash());

		InstallCatalogueItem(Historical03Stock.MedievalFineWovenGirdle());

		InstallCatalogueItem(Historical03Stock.MedievalFineLeatherShoes());

		InstallCatalogueItem(Historical03Stock.MedievalAnkleLeatherBoots());

		InstallCatalogueItem(Historical03Stock.MedievalHighLeatherBoots());

		InstallCatalogueItem(Historical03Stock.MedievalPlainLeatherSandals());

		InstallCatalogueItem(Historical03Stock.MedievalWoolHose());

		InstallCatalogueItem(Historical03Stock.MedievalWoolChausses());

		InstallCatalogueItem(Historical03Stock.MedievalNarrowWoolTrousers());

		InstallCatalogueItem(Historical03Stock.MedievalWoolTunic());

		InstallCatalogueItem(Historical03Stock.MedievalLongWoolTunic());

		InstallCatalogueItem(Historical03Stock.MedievalHeavyWoolCloak());

		InstallCatalogueItem(Historical03Stock.MedievalSimpleWoolMantle());

		InstallCatalogueItem(Historical03Stock.MedievalFurLinedCloak());

		InstallCatalogueItem(Historical03Stock.MedievalLinenCoif());

		InstallCatalogueItem(Historical03Stock.MedievalLinenVeil());

		CreateHistoricalClothingItem(FindHistoricalClothingSource("medieval_linen_wimple")!);

		InstallCatalogueItem(Historical03Stock.MedievalWoolHood());

		InstallCatalogueItem(Historical03Stock.MedievalFeltCap());

		InstallCatalogueItem(Historical03Stock.MedievalRectangularWoolCloak());

		InstallCatalogueItem(Historical03Stock.MedievalBroochedWoolMantle());

		InstallCatalogueItem(Historical03Stock.MedievalTabletBandedWoolTunic());

		InstallCatalogueItem(Historical03Stock.MedievalStraightWoolGown());

		InstallCatalogueItem(Historical03Stock.MedievalPinnedWoolGown());

		InstallCatalogueItem(Historical03Stock.MedievalFineBorderedWoolGown());

		InstallCatalogueItem(Historical03Stock.MedievalWoolLegWraps());

		InstallCatalogueItem(Historical03Stock.MedievalLinenSaerk());

		InstallCatalogueItem(Historical03Stock.MedievalWoolHangerok());

		InstallCatalogueItem(Historical03Stock.MedievalFineWoolHangerok());

		InstallCatalogueItem(Historical03Stock.MedievalCheckedWoolMantle());

		InstallCatalogueItem(Historical03Stock.MedievalFineCheckedWoolMantle());

		InstallCatalogueItem(Historical03Stock.MedievalGaelicLeine());

		InstallCatalogueItem(Historical03Stock.MedievalFineBorderedLeine());

		InstallCatalogueItem(Historical03Stock.MedievalWoolBrat());

		InstallCatalogueItem(Historical03Stock.MedievalFineBorderedBrat());

		InstallCatalogueItem(Historical03Stock.MedievalWoolTrews());

		InstallCatalogueItem(Historical03Stock.MedievalFrankishKneeTunic());

		InstallCatalogueItem(Historical03Stock.MedievalFineFrankishTunic());

		InstallCatalogueItem(Historical03Stock.MedievalCarolingianCloak());

		InstallCatalogueItem(Historical03Stock.MedievalNormanSplitTunic());

		InstallCatalogueItem(Historical03Stock.MedievalFineSplitRidingTunic());

		InstallCatalogueItem(Historical03Stock.MedievalCapetianCotte());

		InstallCatalogueItem(Historical03Stock.MedievalFineWoolCotte());

		InstallCatalogueItem(Historical03Stock.MedievalLinenChainse());

		InstallCatalogueItem(Historical03Stock.MedievalWoolKirtle());

		InstallCatalogueItem(Historical03Stock.MedievalFineWoolKirtle());

		InstallCatalogueItem(Historical03Stock.MedievalFineBliaut());

		InstallCatalogueItem(Historical03Stock.MedievalSimpleSurcoat());

		InstallCatalogueItem(Historical03Stock.MedievalFineSilkSurcoat());

		InstallCatalogueItem(Historical03Stock.MedievalHalfCircleMantle());

		InstallCatalogueItem(Historical03Stock.MedievalFurTrimmedMantle());

		InstallCatalogueItem(Historical03Stock.MedievalGugelHood());

		InstallCatalogueItem(Historical03Stock.MedievalBarbetteVeil());

		InstallCatalogueItem(Historical03Stock.MedievalFineLinenVeil());

		InstallCatalogueItem(Historical03Stock.MedievalChristianIberianCamisa());

		InstallCatalogueItem(Historical03Stock.MedievalWoolSaya());

		InstallCatalogueItem(Historical03Stock.MedievalFineWoolSaya());

		InstallCatalogueItem(Historical03Stock.MedievalIberianPellote());

		InstallCatalogueItem(Historical03Stock.MedievalFineSilkPellote());

		InstallCatalogueItem(Historical03Stock.MedievalIberianManto());

		InstallCatalogueItem(Historical03Stock.MedievalFineIberianManto());

		InstallCatalogueItem(Historical03Stock.MedievalLinenToca());

		InstallCatalogueItem(Historical03Stock.MedievalLinenKamision());

		InstallCatalogueItem(Historical03Stock.MedievalWoolDalmatic());

		InstallCatalogueItem(Historical03Stock.MedievalSilkDalmatic());

		InstallCatalogueItem(Historical03Stock.MedievalSkaramangion());

		InstallCatalogueItem(Historical03Stock.MedievalSagion());

		InstallCatalogueItem(Historical03Stock.MedievalTablionCloak());

		InstallCatalogueItem(Historical03Stock.MedievalLorosSash());

		InstallCatalogueItem(Historical03Stock.MedievalMaforion());

		InstallCatalogueItem(Historical03Stock.MedievalFineMaforion());

		InstallCatalogueItem(Historical03Stock.MedievalSoftCampagiShoes());

		InstallCatalogueItem(Historical03Stock.MedievalLinenQamis());

		InstallCatalogueItem(Historical03Stock.MedievalCottonQamis());

		InstallCatalogueItem(Historical03Stock.MedievalFineSilkQamis());

		InstallCatalogueItem(Historical03Stock.MedievalCottonSirwal());

		InstallCatalogueItem(Historical03Stock.MedievalFineSilkSirwal());

		InstallCatalogueItem(Historical03Stock.MedievalWoolJubba());

		InstallCatalogueItem(Historical03Stock.MedievalFineTirazRobe());

		InstallCatalogueItem(Historical03Stock.MedievalLinenIzar());

		InstallCatalogueItem(Historical03Stock.MedievalWoolBurnous());

		InstallCatalogueItem(Historical03Stock.MedievalFineBurnous());

		InstallCatalogueItem(Historical03Stock.MedievalWrappedCottonTurban());

		InstallCatalogueItem(Historical03Stock.MedievalFineSilkTurban());

		InstallCatalogueItem(Historical03Stock.MedievalCottonHeadVeil());

		InstallCatalogueItem(Historical03Stock.MedievalFineSilkVeil());

		InstallCatalogueItem(Historical03Stock.MedievalSoftLeatherSlippers());

		InstallCatalogueItem(Historical03Stock.MedievalSeljukCaftan());

		InstallCatalogueItem(Historical03Stock.MedievalFineSeljukCaftan());

		InstallCatalogueItem(Historical03Stock.MedievalFeltTurkicCap());

		InstallCatalogueItem(Historical03Stock.MedievalMagyarRidingCoat());

		InstallCatalogueItem(Historical03Stock.MedievalFineMagyarCoat());

		InstallCatalogueItem(Historical03Stock.MedievalSteppeRidingCaftan());

		InstallCatalogueItem(Historical03Stock.MedievalFineSteppeCaftan());

		InstallCatalogueItem(Historical03Stock.MedievalSteppeFeltBoots());

		InstallCatalogueItem(Historical03Stock.MedievalSteppeFeltCap());

		InstallCatalogueItem(Historical03Stock.MedievalSteppeSilkBelt());

		InstallCatalogueItem(Historical03Stock.MedievalWoolDeel());

		InstallCatalogueItem(Historical03Stock.MedievalFineSilkDeel());

		InstallCatalogueItem(Historical03Stock.MedievalRusLinenRubakha());

		InstallCatalogueItem(Historical03Stock.MedievalFineRusRubakha());

		InstallCatalogueItem(Historical03Stock.MedievalRusWoolPorty());

		InstallCatalogueItem(Historical03Stock.MedievalRusOnuchi());

		InstallCatalogueItem(Historical03Stock.MedievalRusPoneva());

		InstallCatalogueItem(Historical03Stock.MedievalRusSleevelessOvergown());

		InstallCatalogueItem(Historical03Stock.MedievalRusShuba());

		InstallCatalogueItem(Historical03Stock.MedievalRusFurHat());

		InstallCatalogueItem(Historical03Stock.MedievalCottonDhoti());

		InstallCatalogueItem(Historical03Stock.MedievalFineCottonDhoti());

		InstallCatalogueItem(Historical03Stock.MedievalCottonUttariya());

		InstallCatalogueItem(Historical03Stock.MedievalFineSilkUttariya());

		InstallCatalogueItem(Historical03Stock.MedievalCottonTurban());

		InstallCatalogueItem(Historical03Stock.MedievalFineSilkTurbanIndian());

		CreateHistoricalClothingItem(FindHistoricalClothingSource("medieval_cotton_sari")!);

		CreateHistoricalClothingItem(FindHistoricalClothingSource("medieval_fine_silk_sari")!);

		CreateHistoricalClothingItem(FindHistoricalClothingSource("medieval_short_cotton_bodice")!);

		InstallCatalogueItem(Historical03Stock.MedievalFineSilkBodice());

		InstallCatalogueItem(Historical03Stock.MedievalCottonLowerCloth());

		InstallCatalogueItem(Historical03Stock.MedievalSouthIndianVeshti());

		InstallCatalogueItem(Historical03Stock.MedievalSilkBorderedVeshti());

		InstallCatalogueItem(Historical03Stock.MedievalSouthIndianAngavastram());

		InstallCatalogueItem(Historical03Stock.MedievalFineAngavastram());

		InstallCatalogueItem(Historical03Stock.MedievalCottonWrapSkirt());

		InstallCatalogueItem(Historical03Stock.MedievalFineSilkWrapSkirt());

		InstallCatalogueItem(Historical03Stock.MedievalSongCrossCollarRobe());

		InstallCatalogueItem(Historical03Stock.MedievalSongRoundCollarRobe());

		InstallCatalogueItem(Historical03Stock.MedievalSongBeizi());

		InstallCatalogueItem(Historical03Stock.MedievalSongRuJacket());

		InstallCatalogueItem(Historical03Stock.MedievalSongChangqunSkirt());

		InstallCatalogueItem(Historical03Stock.MedievalSongTrousers());

		InstallCatalogueItem(Historical03Stock.MedievalSongClothShoes());

		InstallCatalogueItem(Historical03Stock.MedievalSongOfficialCap());

		InstallCatalogueItem(Historical03Stock.MedievalSongHeadscarf());

		InstallCatalogueItem(Historical03Stock.MedievalGoryeoCrossCollarJacket());

		InstallCatalogueItem(Historical03Stock.MedievalGoryeoBaji());

		InstallCatalogueItem(Historical03Stock.MedievalGoryeoChima());

		InstallCatalogueItem(Historical03Stock.MedievalGoryeoPoRobe());

		InstallCatalogueItem(Historical03Stock.MedievalGoryeoSilkOverrobe());

		InstallCatalogueItem(Historical03Stock.MedievalGoryeoHeadcloth());

		InstallCatalogueItem(Historical03Stock.MedievalGoryeoFeltHat());

		InstallCatalogueItem(Historical03Stock.MedievalJapaneseKosode());

		InstallCatalogueItem(Historical03Stock.MedievalJapaneseHakama());

		InstallCatalogueItem(Historical03Stock.MedievalJapaneseSuikan());

		InstallCatalogueItem(Historical03Stock.MedievalJapaneseHitatare());

		InstallCatalogueItem(Historical03Stock.MedievalJapaneseUchigi());

		InstallCatalogueItem(Historical03Stock.MedievalJapaneseKariginu());

		InstallCatalogueItem(Historical03Stock.MedievalJapaneseEboshi());

		InstallCatalogueItem(Historical03Stock.MedievalJapaneseHempSandals());

		InstallCatalogueItem(Historical03Stock.MedievalJapaneseHeadcloth());

		InstallCatalogueItem(Historical03Stock.MedievalWoolOverTunic());

		InstallCatalogueItem(Historical03Stock.MedievalLinedWoolHose());

		InstallCatalogueItem(Historical03Stock.MedievalWoolFootwraps());

		InstallCatalogueItem(Historical03Stock.MedievalWoolMittens());

		InstallCatalogueItem(Historical03Stock.MedievalLeatherWinterMittens());

		InstallCatalogueItem(Historical03Stock.MedievalLinedLeatherBoots());

		InstallCatalogueItem(Historical03Stock.MedievalHeavyHoodedWoolCloak());

		InstallCatalogueItem(Historical03Stock.MedievalFurFacedWinterCloak());

		InstallCatalogueItem(Historical03Stock.MedievalWaxedSkinCape());

		InstallCatalogueItem(Historical03Stock.MedievalHeavyCheckedWoolMantle());

		InstallCatalogueItem(Historical03Stock.MedievalHeavyWoolBrat());

		InstallCatalogueItem(Historical03Stock.MedievalFurTrimmedBrat());

		InstallCatalogueItem(Historical03Stock.MedievalWoolCowlHood());

		InstallCatalogueItem(Historical03Stock.MedievalHeavyWoolSurcoat());

		InstallCatalogueItem(Historical03Stock.MedievalLinedWoolManto());

		InstallCatalogueItem(Historical03Stock.MedievalWoolPaenula());

		InstallCatalogueItem(Historical03Stock.MedievalHeavySagion());

		InstallCatalogueItem(Historical03Stock.MedievalHeavyWoolBurnous());

		InstallCatalogueItem(Historical03Stock.MedievalPaddedWoolJubba());

		InstallCatalogueItem(Historical03Stock.MedievalWoolIzarWrap());

		InstallCatalogueItem(Historical03Stock.MedievalFurLinedCaftan());

		InstallCatalogueItem(Historical03Stock.MedievalFeltWinterDeel());

		InstallCatalogueItem(Historical03Stock.MedievalQuiltedWinterTrousers());

		InstallCatalogueItem(Historical03Stock.MedievalFurEarflapHat());

		InstallCatalogueItem(Historical03Stock.MedievalFeltStockings());

		InstallCatalogueItem(Historical03Stock.MedievalWoolShawl());

		InstallCatalogueItem(Historical03Stock.MedievalThickCottonShawl());

		InstallCatalogueItem(Historical03Stock.MedievalFineGoatHairShawl());

		InstallCatalogueItem(Historical03Stock.MedievalHeavyCottonAngavastram());

		InstallCatalogueItem(Historical03Stock.MedievalPaddedSilkOverrobe());

		InstallCatalogueItem(Historical03Stock.MedievalHeavyBeiziOverrobe());

		InstallCatalogueItem(Historical03Stock.MedievalPaddedPoRobe());

		InstallCatalogueItem(Historical03Stock.MedievalLinedKosode());

		InstallCatalogueItem(Historical03Stock.MedievalLayeredUchigiSet());

		InstallCatalogueItem(Historical03Stock.MedievalLinenWorkApron());

		InstallCatalogueItem(Historical03Stock.MedievalHempWorkApron());

		CreateHistoricalClothingItem(FindHistoricalClothingSource("medieval_leather_smith_apron")!);

		InstallCatalogueItem(Historical03Stock.MedievalLinenSleeveGuards());

		InstallCatalogueItem(Historical03Stock.MedievalLeatherWorkGloves());

		InstallCatalogueItem(Historical03Stock.MedievalCleanLinenCoif());

		InstallCatalogueItem(Historical03Stock.MedievalLinenWorkCap());

		InstallCatalogueItem(Historical03Stock.MedievalWideWorkSash());

		InstallCatalogueItem(Historical03Stock.MedievalCanvasWorkTabard());

		InstallCatalogueItem(Historical03Stock.MedievalDrabDyersApron());

		InstallCatalogueItem(Historical03Stock.MedievalShortWoolWorkTunic());

		InstallCatalogueItem(Historical03Stock.MedievalCleanTownWoolTunic());

		InstallCatalogueItem(Historical03Stock.MedievalTuckedWoolWorkGown());

		InstallCatalogueItem(Historical03Stock.MedievalLinenMarketVeil());

		InstallCatalogueItem(Historical03Stock.MedievalBroochedMarketMantle());

		InstallCatalogueItem(Historical03Stock.MedievalLeatherTraderBelt());

		InstallCatalogueItem(Historical03Stock.MedievalWoolMarketHeadcloth());

		InstallCatalogueItem(Historical03Stock.MedievalTownLeine());

		InstallCatalogueItem(Historical03Stock.MedievalWorkWoolCotte());

		InstallCatalogueItem(Historical03Stock.MedievalTownWorkKirtle());

		InstallCatalogueItem(Historical03Stock.MedievalSleevelessWorkSurcoat());

		InstallCatalogueItem(Historical03Stock.MedievalPlainWoolShopGown());

		InstallCatalogueItem(Historical03Stock.MedievalClerkWoolGown());

		InstallCatalogueItem(Historical03Stock.MedievalNeatWoolTownHood());

		InstallCatalogueItem(Historical03Stock.MedievalMerchantsWoolHood());

		InstallCatalogueItem(Historical03Stock.MedievalLinenShopWimple());

		InstallCatalogueItem(Historical03Stock.MedievalLinenBakerCap());

		InstallCatalogueItem(Historical03Stock.MedievalLightLinenWorkCamisa());

		InstallCatalogueItem(Historical03Stock.MedievalCottonMarketToca());

		InstallCatalogueItem(Historical03Stock.MedievalWoolWorkPellote());

		InstallCatalogueItem(Historical03Stock.MedievalPlainWorkDalmatic());

		InstallCatalogueItem(Historical03Stock.MedievalShortWorkSagion());

		InstallCatalogueItem(Historical03Stock.MedievalLinenShopMaforion());

		InstallCatalogueItem(Historical03Stock.MedievalShortCottonWorkQamis());

		InstallCatalogueItem(Historical03Stock.MedievalLinenWorkSirwal());

		InstallCatalogueItem(Historical03Stock.MedievalLinenShopTurban());

		InstallCatalogueItem(Historical03Stock.MedievalCottonWaistApron());

		InstallCatalogueItem(Historical03Stock.MedievalSleevelessWorkJubba());

		InstallCatalogueItem(Historical03Stock.MedievalBazaarMerchantJubba());

		InstallCatalogueItem(Historical03Stock.MedievalScribeLinenSleeves());

		InstallCatalogueItem(Historical03Stock.MedievalMarketIzarWrap());

		InstallCatalogueItem(Historical03Stock.MedievalShortWorkCaftan());

		InstallCatalogueItem(Historical03Stock.MedievalRusTownRubakha());

		InstallCatalogueItem(Historical03Stock.MedievalTuckedPonevaWorkSkirt());

		InstallCatalogueItem(Historical03Stock.MedievalFurriersLeatherApron());

		InstallCatalogueItem(Historical03Stock.MedievalFeltMarketCap());

		InstallCatalogueItem(Historical03Stock.MedievalWideLeatherWorkBelt());

		InstallCatalogueItem(Historical03Stock.MedievalMountedTraderCoat());

		InstallCatalogueItem(Historical03Stock.MedievalShortCottonWorkDhoti());

		InstallCatalogueItem(Historical03Stock.MedievalCottonWorkWaistcloth());

		InstallCatalogueItem(Historical03Stock.MedievalWeaversCottonShouldercloth());

		InstallCatalogueItem(Historical03Stock.MedievalMarketCottonTurban());

		InstallCatalogueItem(Historical03Stock.MedievalTradeBorderedUttariya());

		InstallCatalogueItem(Historical03Stock.MedievalSouthIndianWorkVeshti());

		InstallCatalogueItem(Historical03Stock.MedievalCottonBazaarSari());

		InstallCatalogueItem(Historical03Stock.MedievalSongWorkShanJacket());

		InstallCatalogueItem(Historical03Stock.MedievalSongShortWorkTrousers());

		InstallCatalogueItem(Historical03Stock.MedievalSongShopkeeperRobe());

		InstallCatalogueItem(Historical03Stock.MedievalSongMerchantHeadscarf());

		InstallCatalogueItem(Historical03Stock.MedievalSongCottonWorkApron());

		InstallCatalogueItem(Historical03Stock.MedievalGoryeoWorkJeogori());

		InstallCatalogueItem(Historical03Stock.MedievalGoryeoWorkBaji());

		InstallCatalogueItem(Historical03Stock.MedievalGoryeoMarketChima());

		InstallCatalogueItem(Historical03Stock.MedievalGoryeoShopPo());

		InstallCatalogueItem(Historical03Stock.MedievalJapaneseWorkKosode());

		InstallCatalogueItem(Historical03Stock.MedievalJapaneseWorkHakama());

		InstallCatalogueItem(Historical03Stock.MedievalJapaneseHempWorkApron());

		InstallCatalogueItem(Historical03Stock.MedievalJapaneseYumakiWrap());

		InstallCatalogueItem(Historical03Stock.MedievalLatinBlackMonasticHabit());

		InstallCatalogueItem(Historical03Stock.MedievalLatinBlackScapular());

		InstallCatalogueItem(Historical03Stock.MedievalLatinBlackCowl());

		InstallCatalogueItem(Historical03Stock.MedievalLatinWhiteMonasticTunic());

		InstallCatalogueItem(Historical03Stock.MedievalLatinBlackOverScapular());

		InstallCatalogueItem(Historical03Stock.MedievalLatinWhiteCarthusianHabit());

		InstallCatalogueItem(Historical03Stock.MedievalLatinCanonBlackHabit());

		InstallCatalogueItem(Historical03Stock.MedievalLatinWhiteRochet());

		InstallCatalogueItem(Historical03Stock.MedievalLatinFriarGreyHabit());

		InstallCatalogueItem(Historical03Stock.MedievalLatinRopeCincture());

		InstallCatalogueItem(Historical03Stock.MedievalLatinWhiteFriarHabit());

		InstallCatalogueItem(Historical03Stock.MedievalLatinBlackFriarCappa());

		InstallCatalogueItem(Historical03Stock.MedievalLatinBrownFriarHabit());

		InstallCatalogueItem(Historical03Stock.MedievalLatinWhiteFriarMantle());

		InstallCatalogueItem(Historical03Stock.MedievalLatinSimpleNunHabit());

		InstallCatalogueItem(Historical03Stock.MedievalLatinNunWimpleVeil());

		InstallCatalogueItem(Historical03Stock.MedievalLatinPoorClareGreyHabit());

		InstallCatalogueItem(Historical03Stock.MedievalLatinPoorClareVeil());

		InstallCatalogueItem(Historical03Stock.MedievalLatinBlackClericalGown());

		InstallCatalogueItem(Historical03Stock.MedievalLatinProcessionalCope());

		InstallCatalogueItem(Historical03Stock.MedievalLatinBishopMitre());

		InstallCatalogueItem(Historical03Stock.MedievalLatinBishopGloves());

		InstallCatalogueItem(Historical03Stock.MedievalLatinArchbishopPallium());

		CreateHistoricalClothingItem(FindHistoricalClothingSource("medieval_eastern_black_riassa")!);

		InstallCatalogueItem(Historical03Stock.MedievalEasternMonasticMantle());

		InstallCatalogueItem(Historical03Stock.MedievalEasternBlackKlobuk());

		InstallCatalogueItem(Historical03Stock.MedievalEasternWomensMonasticVeil());

		CreateHistoricalClothingItem(FindHistoricalClothingSource("medieval_eastern_sticharion")!);

		InstallCatalogueItem(Historical03Stock.MedievalEasternOrarion());

		CreateHistoricalClothingItem(FindHistoricalClothingSource("medieval_eastern_epitrachelion")!);

		CreateHistoricalClothingItem(FindHistoricalClothingSource("medieval_eastern_epimanikia")!);

		CreateHistoricalClothingItem(FindHistoricalClothingSource("medieval_eastern_phelonion")!);

		InstallCatalogueItem(Historical03Stock.MedievalEasternSakkos());

		InstallCatalogueItem(Historical03Stock.MedievalEasternOmophorion());

		CreateHistoricalClothingItem(FindHistoricalClothingSource("medieval_eastern_kamilavkion")!);

		CreateHistoricalClothingItem(FindHistoricalClothingSource("medieval_islamic_plain_imam_qamis")!);

		InstallCatalogueItem(Historical03Stock.MedievalIslamicPlainImamTurban());

		InstallCatalogueItem(Historical03Stock.MedievalIslamicScholarsJubba());

		InstallCatalogueItem(Historical03Stock.MedievalIslamicTaylasan());

		InstallCatalogueItem(Historical03Stock.MedievalIslamicQadiTurban());

		InstallCatalogueItem(Historical03Stock.MedievalSufiPatchedKhirqa());

		InstallCatalogueItem(Historical03Stock.MedievalSufiWoolSash());

		InstallCatalogueItem(Historical03Stock.MedievalSufiFeltCap());

		InstallCatalogueItem(Historical03Stock.MedievalSufiCoarseWoolCloak());

		CreateHistoricalClothingItem(FindHistoricalClothingSource("medieval_jewish_tallit_gadol")!);

		CreateHistoricalClothingItem(FindHistoricalClothingSource("medieval_jewish_tallit_katan")!);

		CreateHistoricalClothingItem(FindHistoricalClothingSource("medieval_jewish_skullcap")!);

		InstallCatalogueItem(Historical03Stock.MedievalJewishScholarsRobe());

		CreateHistoricalClothingItem(FindHistoricalClothingSource("medieval_hindu_white_priest_dhoti")!);

		InstallCatalogueItem(Historical03Stock.MedievalHinduWhiteUttariya());

		CreateHistoricalClothingItem(FindHistoricalClothingSource("medieval_hindu_kaupina")!);

		InstallCatalogueItem(Historical03Stock.MedievalHinduOchreAsceticWrap());

		InstallCatalogueItem(Historical03Stock.MedievalHinduOchreShoulderCloth());

		CreateHistoricalClothingItem(FindHistoricalClothingSource("medieval_jain_white_ascetic_robe")!);

		CreateHistoricalClothingItem(FindHistoricalClothingSource("medieval_jain_white_shoulder_wrap")!);

		InstallCatalogueItem(Historical03Stock.MedievalBuddhistPatchedKasaya());

		InstallCatalogueItem(Historical03Stock.MedievalBuddhistNunRobe());

		InstallCatalogueItem(Historical03Stock.MedievalBuddhistBlackMonasticRobe());

		CreateHistoricalClothingItem(FindHistoricalClothingSource("medieval_daoist_cross_collar_robe")!);

		CreateHistoricalClothingItem(FindHistoricalClothingSource("medieval_daoist_cloud_sleeved_robe")!);

		CreateHistoricalClothingItem(FindHistoricalClothingSource("medieval_daoist_ritual_cap")!);

		CreateHistoricalClothingItem(FindHistoricalClothingSource("medieval_shinto_white_joe_robe")!);

		CreateHistoricalClothingItem(FindHistoricalClothingSource("medieval_shinto_priest_hakama")!);

		CreateHistoricalClothingItem(FindHistoricalClothingSource("medieval_shinto_priest_eboshi")!);

		CreateHistoricalClothingItem(FindHistoricalClothingSource("medieval_shinto_miko_white_kosode")!);

		CreateHistoricalClothingItem(FindHistoricalClothingSource("medieval_shinto_miko_red_hakama")!);

		InstallCatalogueItem(Historical03Stock.MedievalSteppeRitualFeltCoat());

		InstallCatalogueItem(Historical03Stock.MedievalSteppeRitualFurCap());

		InstallCatalogueItem(Historical03Stock.MedievalSteppeRitualSash());

		InstallCatalogueItem(Historical03Stock.MedievalAnnularBronzeBrooch());

		InstallCatalogueItem(Historical03Stock.MedievalPenannularBronzeBrooch());

		InstallCatalogueItem(Historical03Stock.MedievalPairOvalBrooches());

		InstallCatalogueItem(Historical03Stock.MedievalPlainBronzeCloakPin());

		InstallCatalogueItem(Historical03Stock.MedievalIronCloakPin());

		InstallCatalogueItem(Historical03Stock.MedievalBronzeMountedLeatherBelt());

		InstallCatalogueItem(Historical03Stock.MedievalSilverMountedLeatherBelt());

		CreateHistoricalClothingItem(FindHistoricalClothingSource("medieval_tablet_woven_garters")!);

		InstallCatalogueItem(Historical03Stock.MedievalWoolLegTies());

		InstallCatalogueItem(Historical03Stock.MedievalLinenHeadband());

		InstallCatalogueItem(Historical03Stock.MedievalSilkHairRibbon());

		InstallCatalogueItem(Historical03Stock.MedievalSilkRobeSash());

		InstallCatalogueItem(Historical03Stock.MedievalWovenKosodeSash());

		InstallCatalogueItem(Historical03Stock.MedievalHempWaistCord());

		InstallCatalogueItem(Historical03Stock.MedievalCottonWaistCord());

		InstallCatalogueItem(Historical03Stock.MedievalGlassBeadNecklace());

		InstallCatalogueItem(Historical03Stock.MedievalAmberBeadNecklace());

		InstallCatalogueItem(Historical03Stock.MedievalShellBeadNecklace());

		InstallCatalogueItem(Historical03Stock.MedievalBronzeArmlet());

		InstallCatalogueItem(Historical03Stock.MedievalBronzeBracelets());

		InstallCatalogueItem(Historical03Stock.MedievalBronzeBangles());

		InstallCatalogueItem(Historical03Stock.MedievalBronzeAnklets());

		InstallCatalogueItem(Historical03Stock.MedievalSilverEarrings());

		InstallCatalogueItem(Historical03Stock.MedievalSimpleBronzeRing());

		InstallCatalogueItem(Historical03Stock.MedievalFineSilverRing());

		InstallCatalogueItem(Historical03Stock.MedievalBoneHairpin());

		InstallCatalogueItem(Historical03Stock.MedievalBronzeHairpin());

		InstallCatalogueItem(Historical03Stock.MedievalSilverHairpins());

		InstallCatalogueItem(Historical03Stock.MedievalTabletWovenBelt());

		InstallCatalogueItem(Historical03Stock.MedievalDecoratedLeatherGirdle());

		InstallCatalogueItem(Historical03Stock.MedievalTravelWoolCloak());

		InstallCatalogueItem(Historical03Stock.MedievalBroadBrimFeltHat());

		InstallCatalogueItem(Historical03Stock.MedievalTravelWoolGaiters());

		InstallCatalogueItem(Historical03Stock.MedievalOiledLinenRainCape());

		InstallCatalogueItem(Historical03Stock.MedievalShepherdsHoodedCloak());

		InstallCatalogueItem(Historical03Stock.MedievalRoughFeltHerderHat());

		InstallCatalogueItem(Historical03Stock.MedievalFisherHeadcloth());

		InstallCatalogueItem(Historical03Stock.MedievalShortBoatmansTunic());

		InstallCatalogueItem(Historical03Stock.MedievalRopeSoledHempSandals());

		InstallCatalogueItem(Historical03Stock.MedievalLeatherHawkingGlove());

		InstallCatalogueItem(Historical03Stock.MedievalRoughWoolGaiters());

		InstallCatalogueItem(Historical03Stock.MedievalPatchedWoolCloak());

		InstallCatalogueItem(Historical03Stock.MedievalStablehandWoolCap());

		InstallCatalogueItem(Historical03Stock.MedievalCartDriverWoolCoat());

		InstallCatalogueItem(Historical03Stock.MedievalPlainServiceGown());

		InstallCatalogueItem(Historical03Stock.MedievalTuckedWorkKirtle());

		InstallCatalogueItem(Historical03Stock.MedievalLargeLinenWorkApron());

		InstallCatalogueItem(Historical03Stock.MedievalLinenLaundryHeadcloth());

		InstallCatalogueItem(Historical03Stock.MedievalShortServiceTunic());

		InstallCatalogueItem(Historical03Stock.MedievalHouseholdWorkSash());

		InstallCatalogueItem(Historical03Stock.MedievalKitchenSleeveTies());

		InstallCatalogueItem(Historical03Stock.MedievalScullionLinenCoif());

		InstallCatalogueItem(Historical03Stock.MedievalLeatherStableApron());

		InstallCatalogueItem(Historical03Stock.MedievalInnWorkersWoolHood());

		InstallCatalogueItem(Historical03Stock.MedievalStudentWoolGown());

		InstallCatalogueItem(Historical03Stock.MedievalMasterScholarGown());

		InstallCatalogueItem(Historical03Stock.MedievalPhysicianWoolGown());

		InstallCatalogueItem(Historical03Stock.MedievalNotaryWoolGown());

		InstallCatalogueItem(Historical03Stock.MedievalLinedScholarsHood());

		InstallCatalogueItem(Historical03Stock.MedievalPlainBlackSkullcap());

		InstallCatalogueItem(Historical03Stock.MedievalSoberLectureMantle());

		InstallCatalogueItem(Historical03Stock.MedievalFineScholarsCap());

		InstallCatalogueItem(Historical03Stock.MedievalChildWoolTunic());

		InstallCatalogueItem(Historical03Stock.MedievalChildLinenShift());

		InstallCatalogueItem(Historical03Stock.MedievalChildLeatherShoes());

		InstallCatalogueItem(Historical03Stock.MedievalChildWoolCloak());

		InstallCatalogueItem(Historical03Stock.MedievalApprenticeWorkTunic());

		InstallCatalogueItem(Historical03Stock.MedievalApprenticeLinenCap());

		InstallCatalogueItem(Historical03Stock.MedievalChildSilkCourtRobe());

		InstallCatalogueItem(Historical03Stock.MedievalApprenticeCanvasApron());
	}
	private void SeedMedievalItemsReusedByRenaissanceOutfits()
	{
		InstallCatalogueItem(Historical03Stock.MedievalSoftLeatherShoes());

		InstallCatalogueItem(Historical03Stock.MedievalLatinWhiteAlb());

		CreateHistoricalClothingItem(FindHistoricalClothingSource("medieval_latin_amice")!);

		CreateHistoricalClothingItem(FindHistoricalClothingSource("medieval_latin_linen_cincture")!);

		CreateHistoricalClothingItem(FindHistoricalClothingSource("medieval_latin_stole")!);

		CreateHistoricalClothingItem(FindHistoricalClothingSource("medieval_latin_maniple")!);

		InstallCatalogueItem(Historical03Stock.MedievalLatinChasuble());

		CreateHistoricalClothingItem(FindHistoricalClothingSource("medieval_latin_dalmatic_vestment")!);

		InstallCatalogueItem(Historical03Stock.MedievalBuddhistFormalKesa());

		InstallCatalogueItem(Historical03Stock.MedievalBuddhistPlainUnderrobe());

		InstallCatalogueItem(Historical03Stock.MedievalBuddhistTravellingMantle());

		InstallCatalogueItem(Historical03Stock.MedievalHempSandals());
	}
}
