using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DatabaseSeeder.Seeders
{
    public partial class UsefulSeeder
    {

        private DictionaryWithDefault<string, MudSharp.Models.Tag> _tags = new(StringComparer.OrdinalIgnoreCase);

        private void AddTag(FuturemudDatabaseContext context, string name, string parent)
        {
            if (_tags.Any(x => x.Key.Equals(name, StringComparison.InvariantCultureIgnoreCase)))
            {
                return;
            }

            Tag tag = new()
            {
                Name = name,
                Parent = _tags[parent]
            };
            _tags[name] = tag;
            context.Tags.Add(tag);
        }

        private void SeedTags(FuturemudDatabaseContext context, ICollection<string> errors)
        {
            _tags = context.Tags.ToDictionaryWithDefault(x => x.Name, x => x, StringComparer.OrdinalIgnoreCase);
            // Eras
            AddTag(context, "Era", "");
            AddTag(context, "Stone Age Era", "Era");
            AddTag(context, "Bronze Age Era", "Era");
            AddTag(context, "Iron Age Era", "Era");
            AddTag(context, "Antiquity Era", "Era");
            AddTag(context, "Dark Ages Era", "Era");
            AddTag(context, "Medieval Era", "Era");
            AddTag(context, "Renaissance Era", "Era");
            AddTag(context, "Colonial Era", "Era");
            AddTag(context, "Industrial Era", "Era");
            AddTag(context, "Modern Era", "Era");
            AddTag(context, "Nuclear Era", "Era");
            AddTag(context, "Information Age Era", "Era");
            AddTag(context, "Near Future Era", "Era");
            AddTag(context, "Far Future Era", "Era");

            // Functions
            AddTag(context, "Functions", "");

            AddTag(context, "Material Functions", "Functions");
            AddTag(context, "Kindling", "Material Functions");
            AddTag(context, "Firewood", "Material Functions");
            AddTag(context, "Meltable", "Material Functions");
            AddTag(context, "Salvagable Fabric", "Material Functions");
            AddTag(context, "Commoditisable", "Material Functions");
            AddTag(context, "Padding", "Material Functions");
            AddTag(context, "Hot Fire", "Material Functions");
            AddTag(context, "String", "Material Functions");
            AddTag(context, "Debris", "Material Functions");
            AddTag(context, "Tanning Agent", "Material Functions");
            AddTag(context, "Ore Deposit", "Material Functions");
            AddTag(context, "Ignition Source", "Material Functions");
            AddTag(context, "Fire", "Material Functions");
            AddTag(context, "Musket Wadding", "Material Functions");

            AddTag(context, "Repairing", "Functions");
            AddTag(context, "Sharpening", "Functions");

            // Clothing
            AddTag(context, "Worn Items", "Functions");
            AddTag(context, "Footwear", "Worn Items");
            AddTag(context, "Socks", "Worn Items");
            AddTag(context, "Legwear", "Worn Items");
            AddTag(context, "Bodywear", "Worn Items");
            AddTag(context, "Underwear", "Worn Items");
            AddTag(context, "Belts", "Worn Items");
            AddTag(context, "Gloves", "Worn Items");
            AddTag(context, "Hats", "Worn Items");
            AddTag(context, "Spectacles", "Worn Items");
            AddTag(context, "Goggles", "Worn Items");
            AddTag(context, "Scarves", "Worn Items");
            AddTag(context, "Headwear", "Worn Items");
            AddTag(context, "Fashion Accessories", "Worn Items");
            AddTag(context, "Jewellery", "Worn Items");
            AddTag(context, "Rings", "Jewellery");
            AddTag(context, "Necklaces", "Jewellery");
            AddTag(context, "Bracelets", "Jewellery");
            AddTag(context, "Anklets", "Jewellery");
            AddTag(context, "Earrings", "Jewellery");
            AddTag(context, "Piercings", "Jewellery");

            // Separation
            AddTag(context, "Separation", "Functions");
            AddTag(context, "Cutting", "Separation");
            AddTag(context, "Shearing", "Separation");
            AddTag(context, "Shaving", "Separation");
            AddTag(context, "Precision Cutting", "Cutting");
            AddTag(context, "Filleting", "Cutting");
            AddTag(context, "Wood Cutting", "Cutting");
            AddTag(context, "Metal Cutting", "Cutting");
            AddTag(context, "Stone Cutting", "Cutting");
            AddTag(context, "Knife", "Cutting");
            AddTag(context, "Scissors", "Shearing");
            AddTag(context, "Shears", "Shearing");
            AddTag(context, "Guillotine", "Shearing");
            AddTag(context, "Safety Razor", "Shaving");
            AddTag(context, "Razorblade", "Shaving");
            AddTag(context, "Crude", "Shaving");

            // Joining
            AddTag(context, "Joining", "Functions");
            AddTag(context, "Adhesion", "Joining");
            AddTag(context, "Clamping", "Joining");
            AddTag(context, "Fastening", "Joining");
            AddTag(context, "Crimping", "Joining");
            AddTag(context, "Welding", "Joining");
            AddTag(context, "Tie", "Joining");
            AddTag(context, "Pin", "Joining");
            AddTag(context, "Sewing", "Joining");
            AddTag(context, "Surgical Suturing", "Sewing");
            AddTag(context, "Glue", "Adhesion");
            AddTag(context, "Cement", "Adhesion");
            AddTag(context, "Joining Tape", "Adhesion");
            AddTag(context, "Nail", "Fastening");
            AddTag(context, "Bolt", "Fastening");
            AddTag(context, "Nut", "Fastening");
            AddTag(context, "Washer", "Fastening");
            AddTag(context, "Screw", "Fastening");
            AddTag(context, "Rivet", "Fastening");
            AddTag(context, "Clamp", "Clamping");
            AddTag(context, "Peg", "Clamping");
            AddTag(context, "Clip", "Clamping");
            AddTag(context, "Tie Strap", "Tie");
            AddTag(context, "Tie Wire", "Tie");
            AddTag(context, "Tie Rope", "Tie");
            AddTag(context, "Tie Band", "Tie");
            AddTag(context, "Wire Crimp", "Crimping");
            AddTag(context, "Cable Crimp", "Crimping");
            AddTag(context, "Metal Crimp", "Crimping");
            AddTag(context, "Jewellers Crimp", "Crimping");
            AddTag(context, "Soldering Wire", "Welding");
            AddTag(context, "Solderer", "Welding");
            AddTag(context, "Arc Welding Wire", "Welding");
            AddTag(context, "Arc Welder", "Welding");
            AddTag(context, "Flashbutt Welder", "Welding");
            AddTag(context, "Aluminothermic Welding Portion", "Welding");
            AddTag(context, "Aluminothermic Welding Crucible", "Welding");
            AddTag(context, "Braze Welding Rod", "Welding");
            AddTag(context, "Braze Welder", "Welding");
            AddTag(context, "Flux", "Welding");

            // Containers
            AddTag(context, "Container", "Functions");
            AddTag(context, "Porous Container", "Container");
            AddTag(context, "Watertight Container", "Container");
            AddTag(context, "Airtight Container", "Container");
            AddTag(context, "Open Container", "Container");

            // Tools
            AddTag(context, "Tools", "Functions");

            // Cleanup / Normalisation - Generic Tool Functions
            AddTag(context, "Fastening Tools", "Tools");
            AddTag(context, "Screwdriver", "Fastening Tools");
            AddTag(context, "Spanner", "Fastening Tools");
            AddTag(context, "Wrench", "Fastening Tools");
            AddTag(context, "Riveter", "Fastening Tools");
            AddTag(context, "Rivet Gun", "Fastening Tools");
            AddTag(context, "Stapler", "Fastening Tools");

            AddTag(context, "Striking Tools", "Tools");
            AddTag(context, "Hammer", "Striking Tools");
            AddTag(context, "Mallet", "Striking Tools");

            AddTag(context, "Gripping Tools", "Tools");
            AddTag(context, "Pliers", "Gripping Tools");

            AddTag(context, "Cutting and Shaping Tools", "Tools");
            AddTag(context, "Saw", "Cutting and Shaping Tools");
            AddTag(context, "Chisel", "Cutting and Shaping Tools");

            AddTag(context, "Finishing Tools", "Tools");
            AddTag(context, "Trowel", "Finishing Tools");

            AddTag(context, "Access Equipment", "Tools");
            AddTag(context, "Ladder", "Access Equipment");
            AddTag(context, "Stepladder", "Ladder");
            AddTag(context, "Extension Ladder", "Ladder");
            AddTag(context, "A-Frame Ladder", "Ladder");

            AddTag(context, "Layout Tools", "Tools");
            AddTag(context, "Stringline", "Layout Tools");
            AddTag(context, "Spirit Level", "Layout Tools");
            AddTag(context, "Straightedge", "Layout Tools");
            AddTag(context, "Ruler", "Layout Tools");

            // Cooking
            AddTag(context, "Cooking", "Tools");

            // Cooking Utensils
            AddTag(context, "Cooking Utensils", "Cooking");
            AddTag(context, "Spatula", "Cooking Utensils");
            AddTag(context, "Mixer", "Cooking Utensils");
            AddTag(context, "Egg Beater", "Cooking Utensils");
            AddTag(context, "Whisk", "Cooking Utensils");
            AddTag(context, "Rolling Pin", "Cooking Utensils");
            AddTag(context, "Piping Bag", "Cooking Utensils");
            AddTag(context, "Cooking Spoon", "Cooking Utensils");
            AddTag(context, "Stirrer", "Cooking Utensils");
            AddTag(context, "Cooking Tongs", "Cooking Utensils");
            AddTag(context, "Skewer", "Cooking Utensils");
            AddTag(context, "Potato Masher", "Cooking Utensils");
            AddTag(context, "Potato Ricer", "Cooking Utensils");
            AddTag(context, "Peeler", "Cooking Utensils");
            AddTag(context, "Swivel Peeler", "Peeler");
            AddTag(context, "Julienne Peeler", "Cooking Utensils");
            AddTag(context, "Paring Knife", "Cooking Utensils");
            AddTag(context, "Chopping Board", "Cooking Utensils");
            AddTag(context, "Pizza Peel", "Cooking Utensils");
            AddTag(context, "Skimmer Ladle", "Cooking Utensils");
            AddTag(context, "Cake Lifter", "Cooking Utensils");
            AddTag(context, "Ladle", "Cooking Utensils");
            AddTag(context, "Serving Spoon", "Cooking Utensils");
            AddTag(context, "Mortar and Pestle", "Cooking Utensils");
            AddTag(context, "Triturator", "Cooking Utensils");
            AddTag(context, "Can Opener", "Cooking Utensils");
            AddTag(context, "Colander", "Cooking Utensils");
            AddTag(context, "Measuring Cup", "Cooking Utensils");
            AddTag(context, "Cookie Cutter", "Cooking Utensils");
            AddTag(context, "Pizza Cutter", "Cooking Utensils");
            AddTag(context, "Pizza Stone", "Cooking Utensils");
            AddTag(context, "Mixing Bowl", "Cooking Utensils");
            AddTag(context, "Basting Brush", "Cooking Utensils");
            AddTag(context, "Sifter", "Cooking Utensils");
            AddTag(context, "Icing Comb", "Cooking Utensils");
            AddTag(context, "Icing Spatula", "Spatula");
            AddTag(context, "Turning Spatula", "Spatula");
            AddTag(context, "Scoop", "Cooking Utensils");
            AddTag(context, "Icecream Scoop", "Scoop");
            AddTag(context, "Bulk Scoop", "Scoop");
            AddTag(context, "Grater", "Cooking Utensils");
            AddTag(context, "Box Grater", "Grater");
            AddTag(context, "Hand Grater", "Grater");
            AddTag(context, "Zester", "Grater");
            AddTag(context, "Spiralizer", "Cooking Utensils");
            AddTag(context, "Bamboo Mat", "Cooking Utensils");
            AddTag(context, "Meat Tenderiser", "Cooking Utensils");
            AddTag(context, "Cooking Knife", "Cooking Utensils");
            AddTag(context, "Santoku Knife", "Cooking Knife");
            AddTag(context, "Carving Knife", "Cooking Knife");
            AddTag(context, "Chef's Knife", "Cooking Knife");
            AddTag(context, "Bread Knife", "Cooking Knife");
            AddTag(context, "Butter Knife", "Cooking Knife");
            AddTag(context, "Steak Knife", "Cooking Knife");
            AddTag(context, "Serrated Knife", "Cooking Knife");
            AddTag(context, "Filleting Knife", "Cooking Knife");
            AddTag(context, "Utility Knife", "Cooking Knife");
            AddTag(context, "Oyster Knife", "Cooking Knife");
            AddTag(context, "Carving Fork", "Cooking Utensils");
            AddTag(context, "Garlic Press", "Cooking Utensils");
            AddTag(context, "Juice Press", "Cooking Utensils");
            AddTag(context, "Cherry Pitter", "Cooking Utensils");
            AddTag(context, "Nut Cracker", "Cooking Utensils");
            AddTag(context, "Fish Scaler", "Cooking Utensils");
            AddTag(context, "Marinade Injector", "Cooking Utensils");
            AddTag(context, "Cooling Rack", "Cooking Utensils");
            AddTag(context, "Drying Rack", "Cooking Utensils");
            AddTag(context, "Honey Dipper", "Cooking Utensils");
            AddTag(context, "Fishbone Tweezers", "Cooking Utensils");
            AddTag(context, "Seafood Pick", "Cooking Utensils");
            AddTag(context, "Dough Scraper", "Cooking Utensils");
            AddTag(context, "Sponge Slicer", "Cooking Utensils");
            AddTag(context, "Kitchen Funnel", "Cooking Utensils");
            AddTag(context, "Cookware", "Cooking");
            AddTag(context, "Cooking Pot", "Cookware");
            AddTag(context, "Cooking Pan", "Cookware");
            AddTag(context, "Cooking Tray", "Cookware");
            AddTag(context, "Bakeware", "Cookware");
            AddTag(context, "Frypan", "Cooking Pan");
            AddTag(context, "Saucepan", "Cooking Pan");
            AddTag(context, "Skillet", "Cooking Pan");
            AddTag(context, "Saute Pan", "Cooking Pan");
            AddTag(context, "Paella Pan", "Cooking Pan");
            AddTag(context, "Wok", "Cooking Pan");
            AddTag(context, "Stock Pot", "Cooking Pot");
            AddTag(context, "Boiling Pot", "Cooking Pot");
            AddTag(context, "Stew Pot", "Cooking Pot");
            AddTag(context, "Cauldron", "Cooking Pot");
            AddTag(context, "Baking Sheet", "Cooking Tray");
            AddTag(context, "Baking Tray", "Cooking Tray");
            AddTag(context, "Patisserie Tray", "Cooking Tray");
            AddTag(context, "Roasting Pan", "Cooking Tray");
            AddTag(context, "Rack Roasting Pan", "Cooking Tray");
            AddTag(context, "Deep Roasting Pan", "Cooking Tray");
            AddTag(context, "Pizza Tray", "Cooking Tray");
            AddTag(context, "Cake Tin", "Bakeware");
            AddTag(context, "Round Cake Tin", "Cake Tin");
            AddTag(context, "Square Cake Tin", "Cake Tin");
            AddTag(context, "Rectangular Cake Tin", "Cake Tin");
            AddTag(context, "Quiche Tin", "Bakeware");
            AddTag(context, "Muffin Pan", "Bakeware");
            AddTag(context, "Loaf Pan", "Bakeware");

            // Cleaning
            AddTag(context, "Cleaning", "Tools");
            AddTag(context, "Broom", "Cleaning");
            AddTag(context, "Millet Broom", "Broom");
            AddTag(context, "Straw Broom", "Broom");
            AddTag(context, "Indoor Broom", "Broom");
            AddTag(context, "Outdoor Broom", "Broom");
            AddTag(context, "Shop Broom", "Broom");
            AddTag(context, "Dustpan and Broom", "Broom");
            AddTag(context, "Mop", "Cleaning");
            AddTag(context, "Brush", "Cleaning");
            AddTag(context, "Scrub Brush", "Brush");
            AddTag(context, "Grout Brush", "Brush");
            AddTag(context, "Toilet Brush", "Brush");
            AddTag(context, "Wirebrush", "Brush");
            AddTag(context, "Bottlebrush", "Brush");
            AddTag(context, "Stem Brush", "Brush");
            AddTag(context, "Drain Brush", "Brush");
            AddTag(context, "Duster", "Cleaning");
            AddTag(context, "Cleaning Rag", "Cleaning");
            AddTag(context, "Scourer", "Cleaning");
            AddTag(context, "Steel Wool", "Cleaning");
            AddTag(context, "Dishcloth", "Cleaning");
            AddTag(context, "Sponge", "Cleaning");
            AddTag(context, "Chamois", "Cleaning");
            AddTag(context, "Squeegee", "Cleaning");
            AddTag(context, "Soap", "Cleaning");

            // Digging
            AddTag(context, "Digging", "Tools");
            AddTag(context, "Hoe", "Digging");
            AddTag(context, "Mattock", "Digging");
            AddTag(context, "Shovel", "Digging");
            AddTag(context, "Spade", "Digging");

            // Construction Tools
            // Cleanup / Normalisation - Construction-Specific Tool Names
            AddTag(context, "Construction Tools", "Tools");
            AddTag(context, "Construction Spanner", "Construction Tools");
            AddTag(context, "Shifter Spanner", "Spanner");
            AddTag(context, "Construction Screwdriver", "Construction Tools");
            AddTag(context, "Construction Hammer", "Construction Tools");
            AddTag(context, "Construction Riveter", "Construction Tools");
            AddTag(context, "Construction Rivet Gun", "Construction Tools");
            AddTag(context, "Construction Stapler", "Construction Tools");
            AddTag(context, "Construction Mallet", "Construction Tools");
            AddTag(context, "Construction Wrench", "Construction Tools");
            AddTag(context, "Torque Wrench", "Wrench");
            AddTag(context, "Construction Saw", "Construction Tools");
            AddTag(context, "Construction Chisel", "Construction Tools");
            AddTag(context, "Construction Pliers", "Construction Tools");
            AddTag(context, "Construction Trowel", "Construction Tools");
            AddTag(context, "Construction Ruler", "Ruler");
            AddTag(context, "Wedge", "Construction Tools");
            AddTag(context, "Construction Ladder", "Construction Tools");
            AddTag(context, "Cement Mixer", "Construction Tools");
            AddTag(context, "Wheelbarrow", "Construction Tools");

            // Metalworking Tools
            AddTag(context, "Metalworking Tools", "Tools");
            AddTag(context, "Anvil", "Metalworking Tools");
            AddTag(context, "Forge", "Metalworking Tools");
            AddTag(context, "Bellows", "Metalworking Tools");
            AddTag(context, "Crucible", "Metalworking Tools");
            AddTag(context, "Forge Tongs", "Metalworking Tools");
            AddTag(context, "Forge Hammer", "Metalworking Tools");

            // Textilecraft Tools
            AddTag(context, "Textilecraft Tools", "Tools");
            AddTag(context, "Awl", "Textilecraft Tools");
            AddTag(context, "Burnisher", "Textilecraft Tools");
            AddTag(context, "Creaser", "Textilecraft Tools");
            AddTag(context, "Thread", "Textilecraft Tools");
            AddTag(context, "Sewing Needle", "Textilecraft Tools");
            AddTag(context, "Beading Needle", "Textilecraft Tools");
            AddTag(context, "Seam Ripper", "Textilecraft Tools");
            AddTag(context, "Fabric Pin", "Textilecraft Tools");
            AddTag(context, "Pinking Shears", "Textilecraft Tools");
            AddTag(context, "Tracer Wheel", "Textilecraft Tools");
            AddTag(context, "Sewing Machine", "Textilecraft Tools");
            AddTag(context, "Loom", "Textilecraft Tools");
            AddTag(context, "Knitting Needle", "Textilecraft Tools");
            AddTag(context, "Dress Form", "Textilecraft Tools");

            // Woodcrafting Tools
            AddTag(context, "Woodcrafting Tools", "Tools");
            AddTag(context, "Splitting Axe", "Woodcrafting Tools");
            AddTag(context, "Felling Axe", "Woodcrafting Tools");
            AddTag(context, "Tomahawk Axe", "Woodcrafting Tools");
            AddTag(context, "Lathe", "Woodcrafting Tools");
            AddTag(context, "Wood Chisel", "Woodcrafting Tools");
            AddTag(context, "Planer", "Woodcrafting Tools");
            AddTag(context, "Splitting Awl", "Woodcrafting Tools");
            AddTag(context, "Adze", "Woodcrafting Tools");
            AddTag(context, "Wood Auger", "Woodcrafting Tools");
            AddTag(context, "Saws", "Woodcrafting Tools");
            AddTag(context, "Bow Saw", "Saws");
            AddTag(context, "Hack Saw", "Saws");
            AddTag(context, "Hand Saw", "Saws");
            AddTag(context, "Fine Saw", "Saws");
            AddTag(context, "Crosscut Saw", "Saws");
            AddTag(context, "Pruning Saw", "Saws");
            AddTag(context, "Forest Saw", "Saws");
            AddTag(context, "Circular Saw", "Saws");
            AddTag(context, "Jig Saw", "Saws");
            AddTag(context, "Chain Saw", "Saws");
            AddTag(context, "Wood Clamp", "Woodcrafting Tools");
            AddTag(context, "Carving Drum Gauge", "Woodcrafting Tools");
            AddTag(context, "Carving Spoon", "Woodcrafting Tools");
            AddTag(context, "Wood File", "Woodcrafting Tools");
            AddTag(context, "Sandpaper", "Woodcrafting Tools");
            AddTag(context, "Rasp", "Woodcrafting Tools");
            AddTag(context, "Trammel", "Woodcrafting Tools");

            // Tattoos
            AddTag(context, "Tattooing Tools", "Tools");
            AddTag(context, "Tattooing Needle", "Tattooing Tools");

            // Leatherworking
            AddTag(context, "Leatherworking Tools", "Tools");
            AddTag(context, "Awl Punch", "Leatherworking Tools");
            AddTag(context, "Leather Stitching Pony", "Leatherworking Tools");
            AddTag(context, "Edge Beveller", "Leatherworking Tools");
            AddTag(context, "Leather Gouge", "Leatherworking Tools");
            AddTag(context, "Leather Creaser", "Leatherworking Tools");

            // Stoneworking
            AddTag(context, "Stoneworking Tools", "Tools");
            AddTag(context, "Stone Chisel", "Stoneworking Tools");
            AddTag(context, "Stone Mallet", "Stoneworking Tools");
            AddTag(context, "Plug and Feathers", "Stoneworking Tools");
            AddTag(context, "Bush Hammer", "Stoneworking Tools");

            // Spinning
            AddTag(context, "Spinning Tools", "Textilecraft Tools");
            AddTag(context, "Distaff", "Spinning Tools");
            AddTag(context, "Spindle", "Spinning Tools");
            AddTag(context, "Drop Spindle", "Spinning Tools");
            AddTag(context, "Spinner's Weights", "Spinning Tools");

            // Weaving
            AddTag(context, "Weaving Tools", "Textilecraft Tools");
            AddTag(context, "Hand Loom", "Weaving Tools");
            AddTag(context, "Tablet Weaving Cards", "Weaving Tools");
            AddTag(context, "Weaver's Sword", "Weaving Tools");
            AddTag(context, "Warping Board", "Weaving Tools");
            AddTag(context, "Shuttle", "Weaving Tools");
            AddTag(context, "Loom Weight", "Weaving Tools");
            AddTag(context, "Warp-Weighted Loom", "Weaving Tools");
            AddTag(context, "Weaver's Comb", "Weaving Tools");
            AddTag(context, "Heddle Rod", "Weaving Tools");
            AddTag(context, "Lease Rod", "Weaving Tools");
            AddTag(context, "Weaving Reed", "Weaving Tools");
            AddTag(context, "Beater Batten", "Weaving Tools");

            // Fletching
            AddTag(context, "Fletching Tools", "Woodcrafting Tools");
            AddTag(context, "Arrow Jig", "Fletching Tools");
            AddTag(context, "Fletching Clamp", "Fletching Tools");
            AddTag(context, "Shaft Straightener", "Fletching Tools");

            // Bowmaking
            AddTag(context, "Bowyer Tools", "Woodcrafting Tools");
            AddTag(context, "Bow Press", "Bowyer Tools");
            AddTag(context, "Tillering Stick", "Bowyer Tools");
            AddTag(context, "Bow Scale", "Bowyer Tools");

            // Papermaking
            AddTag(context, "Papermaking Tools", "Tools");
            AddTag(context, "Mould and Deckle", "Papermaking Tools");
            AddTag(context, "Press Felt", "Papermaking Tools");
            AddTag(context, "Hollander Beater", "Papermaking Tools");
            AddTag(context, "Rag Sorting Knife", "Papermaking Tools");
            AddTag(context, "Rag Beating Trough", "Papermaking Tools");
            AddTag(context, "Papermaker's Vat", "Papermaking Tools");
            AddTag(context, "Watermark Wire", "Papermaking Tools");
            AddTag(context, "Couching Blanket", "Papermaking Tools");
            AddTag(context, "Lay Press", "Papermaking Tools");
            AddTag(context, "Paper Sizing Brush", "Papermaking Tools");
            AddTag(context, "Gelatine Sizing Pot", "Papermaking Tools");
            AddTag(context, "Paper Burnishing Agate", "Papermaking Tools");

            // Glassblowing
            AddTag(context, "Glassblowing Tools", "Tools");
            AddTag(context, "Blowpipe", "Glassblowing Tools");
            AddTag(context, "Pontil Rod", "Glassblowing Tools");
            AddTag(context, "Marver Table", "Glassblowing Tools");
            AddTag(context, "Jacks", "Glassblowing Tools");
            AddTag(context, "Paper Pads", "Glassblowing Tools");
            AddTag(context, "Blocks", "Glassblowing Tools");
            AddTag(context, "Glassblower's Bench", "Glassblowing Tools");
            AddTag(context, "Glass Shears", "Glassblowing Tools");
            AddTag(context, "Optic Mold", "Glassblowing Tools");
            AddTag(context, "Snap Tool", "Glassblowing Tools");
            AddTag(context, "Punty Paddle", "Glassblowing Tools");
            AddTag(context, "Glory Hole", "Glassblowing Tools");
            AddTag(context, "Annealing Lehr", "Glassblowing Tools");

            // Locksmithing
            AddTag(context, "Locksmithing Tools", "Tools");
            AddTag(context, "Lockpick", "Locksmithing Tools");
            AddTag(context, "Torsion Wrench", "Locksmithing Tools");
            AddTag(context, "Locksmith's File", "Locksmithing Tools");
            AddTag(context, "Locksmith's Tweezers", "Locksmithing Tools");
            AddTag(context, "Locksmithing Jig", "Locksmithing Tools");
            AddTag(context, "Key Gauge", "Locksmithing Tools");
            AddTag(context, "Impressioning File", "Locksmithing Tools");
            AddTag(context, "Safe Dial Manipulator", "Locksmithing Tools");

            // Gunsmithing
            AddTag(context, "Gunsmithing Tools", "Tools");
            AddTag(context, "Barrel Reamer", "Gunsmithing Tools");
            AddTag(context, "Gun Drill", "Gunsmithing Tools");
            AddTag(context, "Bore Snake", "Gunsmithing Tools");
            AddTag(context, "Tamping Rod", "Gunsmithing Tools");
            AddTag(context, "Gun Vise", "Gunsmithing Tools");
            AddTag(context, "Mainspring Vise", "Gunsmithing Tools");
            AddTag(context, "Breech Plug Wrench", "Gunsmithing Tools");
            AddTag(context, "Rammer", "Gunsmithing Tools");
            AddTag(context, "Bullet Mould", "Gunsmithing Tools");
            AddTag(context, "Patch Cutter", "Gunsmithing Tools");
            AddTag(context, "Ball Puller", "Gunsmithing Tools");
            AddTag(context, "Lock Mortise Chisel", "Gunsmithing Tools");
            AddTag(context, "Barrel Mandrel", "Gunsmithing Tools");
            AddTag(context, "Rifling Bench", "Gunsmithing Tools");
            AddTag(context, "Vent Pick", "Gunsmithing Tools");
            AddTag(context, "Flint Knapping Hammer", "Gunsmithing Tools");
            AddTag(context, "Powder Measure", "Gunsmithing Tools");

            // Tanning
            AddTag(context, "Tanning Tools", "Tools");
            AddTag(context, "Hide Scraper", "Tanning Tools");
            AddTag(context, "Tanning Beam", "Tanning Tools");
            AddTag(context, "Tanning Paddle", "Tanning Tools");
            AddTag(context, "Tanning Rack", "Tanning Tools");
            AddTag(context, "Leather Dehairing Knife", "Tanning Tools");
            AddTag(context, "Brain Tanning Bucket", "Tanning Tools");

            // Armouring
            AddTag(context, "Armouring Tools", "Tools");
            AddTag(context, "Plate Snips", "Armouring Tools");
            AddTag(context, "Armourer's Stake", "Armouring Tools");
            AddTag(context, "Armourer's Anvil", "Armouring Tools");
            AddTag(context, "Dishing Form", "Armouring Tools");
            AddTag(context, "Raising Hammer", "Armouring Tools");
            AddTag(context, "Planishing Hammer", "Armouring Tools");
            AddTag(context, "Ball Stake", "Armouring Tools");
            AddTag(context, "T-Stake", "Armouring Tools");
            AddTag(context, "Armourer's Forming Bags", "Armouring Tools");
            AddTag(context, "Armourer's Pliers", "Armouring Tools");

            // Weaponsmithing
            AddTag(context, "Weaponsmithing Tools", "Tools");
            AddTag(context, "Swordsmith's Hammer", "Weaponsmithing Tools");
            AddTag(context, "Sword Anvil", "Weaponsmithing Tools");
            AddTag(context, "Fuller Tool", "Weaponsmithing Tools");
            AddTag(context, "Tang Punch", "Weaponsmithing Tools");
            AddTag(context, "Sword Vise", "Weaponsmithing Tools");
            AddTag(context, "Quenching Trough", "Weaponsmithing Tools");
            AddTag(context, "Pommel Tightening Jig", "Weaponsmithing Tools");
            AddTag(context, "Crossguard Fixture", "Weaponsmithing Tools");
            AddTag(context, "Forge Bellows", "Weaponsmithing Tools");
            AddTag(context, "Grindstone", "Weaponsmithing Tools");

            #region Butcher Tools
            AddTag(context, "Butcher Tools", "Tools");

            // Field Dressing
            AddTag(context, "Field Dressing Tools", "Butcher Tools");
            AddTag(context, "Skinning Knife", "Field Dressing Tools");
            AddTag(context, "Fleshing Knife", "Field Dressing Tools");
            AddTag(context, "Boning Knife", "Field Dressing Tools");
            AddTag(context, "Carcass Hook", "Field Dressing Tools");
            AddTag(context, "Gut Hook Knife", "Field Dressing Tools");
            AddTag(context, "Meat Saw", "Field Dressing Tools");
            AddTag(context, "Pelting Blade", "Field Dressing Tools");
            AddTag(context, "Splitting Saw", "Field Dressing Tools");

            // Cutting and Portioning
            AddTag(context, "Meat Cutting Tools", "Butcher Tools");
            AddTag(context, "Butcher's Knife", "Meat Cutting Tools");
            AddTag(context, "Cimeter Knife", "Meat Cutting Tools");
            AddTag(context, "Cleaver", "Meat Cutting Tools");
            AddTag(context, "Breaking Knife", "Meat Cutting Tools");
            AddTag(context, "Trimming Knife", "Meat Cutting Tools");
            AddTag(context, "Bone Saw", "Meat Cutting Tools");
            AddTag(context, "Portioning Blade", "Meat Cutting Tools");

            // Processing and Preparation
            AddTag(context, "Meat Processing Tools", "Butcher Tools");
            AddTag(context, "Meat Grinder", "Meat Processing Tools");
            AddTag(context, "Tenderizing Mallet", "Meat Processing Tools");
            AddTag(context, "Slicing Machine", "Meat Processing Tools");
            AddTag(context, "Stuffing Funnel", "Meat Processing Tools");
            AddTag(context, "Larding Needle", "Meat Processing Tools");

            // Cleaning and Maintenance
            AddTag(context, "Butcher Cleaning Tools", "Butcher Tools");
            AddTag(context, "Carcass Scraper", "Butcher Cleaning Tools");

            // Special Tools
            AddTag(context, "Special Butcher Tools", "Butcher Tools");
            AddTag(context, "Saw Guide", "Special Butcher Tools");
            AddTag(context, "Meat Injector", "Special Butcher Tools");
            AddTag(context, "Bone Dust Scraper", "Special Butcher Tools");
            AddTag(context, "Sinew Remover", "Special Butcher Tools");

            // Storage and Organization
            AddTag(context, "Meat Storage Tools", "Butcher Tools");
            AddTag(context, "Butcher Hook", "Meat Storage Tools");
            AddTag(context, "Hanging Rack", "Meat Storage Tools");
            AddTag(context, "Cooler Box", "Meat Storage Tools");
            AddTag(context, "Meat Bin", "Meat Storage Tools");
            #endregion

            // Pottery
            AddTag(context, "Pottery Tools", "Tools");
            AddTag(context, "Potter's Wheel", "Pottery Tools");
            AddTag(context, "Clay Knife", "Pottery Tools");
            AddTag(context, "Potter's Rib", "Pottery Tools");
            AddTag(context, "Loop Tool", "Pottery Tools");
            AddTag(context, "Needle Tool", "Pottery Tools");
            AddTag(context, "Wire Cutter", "Pottery Tools");
            AddTag(context, "Clay Stamp", "Pottery Tools");
            AddTag(context, "Pug Mill", "Pottery Tools");
            AddTag(context, "Slip Trailer", "Pottery Tools");
            AddTag(context, "Hump Mold", "Pottery Tools");
            AddTag(context, "Press Mold", "Pottery Tools");
            AddTag(context, "Extruder", "Pottery Tools");
            AddTag(context, "Slab Roller", "Pottery Tools");
            AddTag(context, "Kiln", "Pottery Tools");

            // Smelting
            AddTag(context, "Smelting Tools", "Tools");
            AddTag(context, "Smelting Furnace", "Smelting Tools");
            AddTag(context, "Crucible Tongs", "Smelting Tools");
            AddTag(context, "Slag Skimmer", "Smelting Tools");
            AddTag(context, "Furnace Bellows", "Smelting Tools");
            AddTag(context, "Ore Crusher", "Smelting Tools");
            AddTag(context, "Ore Roaster", "Smelting Tools");
            AddTag(context, "Slag Hammer", "Smelting Tools");
            AddTag(context, "Bloom Tongs", "Smelting Tools");
            AddTag(context, "Charging Bucket", "Smelting Tools");
            AddTag(context, "Tap Rod", "Smelting Tools");

            // Agriculture
            AddTag(context, "Agricultural Tools", "Tools");
            AddTag(context, "Sickle", "Agricultural Tools");
            AddTag(context, "Reaping Hook", "Agricultural Tools");
            AddTag(context, "Grain Flail", "Agricultural Tools");
            AddTag(context, "Winnowing Basket", "Agricultural Tools");
            AddTag(context, "Winnowing Fork", "Agricultural Tools");
            AddTag(context, "Winnowing Tray", "Agricultural Tools");
            AddTag(context, "Seed Dibber", "Agricultural Tools");
            AddTag(context, "Rake", "Agricultural Tools");
            AddTag(context, "Hay Rake", "Agricultural Tools");
            AddTag(context, "Pitchfork", "Agricultural Tools");
            AddTag(context, "Digging Fork", "Agricultural Tools");
            AddTag(context, "Ard", "Agricultural Tools");
            AddTag(context, "Plough", "Agricultural Tools");
            AddTag(context, "Ploughshare", "Agricultural Tools");
            AddTag(context, "Yoke", "Agricultural Tools");
            AddTag(context, "Drover's Goad", "Agricultural Tools");
            AddTag(context, "Pruning Hook", "Agricultural Tools");

            // Masonry
            AddTag(context, "Masonry Tools", "Construction Tools");
            AddTag(context, "Mason's Hammer", "Masonry Tools");
            AddTag(context, "Mason's Square", "Masonry Tools");
            AddTag(context, "Mason's Line", "Masonry Tools");
            AddTag(context, "Line Pin", "Masonry Tools");
            AddTag(context, "Point Chisel", "Masonry Tools");
            AddTag(context, "Tooth Chisel", "Masonry Tools");
            AddTag(context, "Dressing Axe", "Masonry Tools");
            AddTag(context, "Mason's Trowel", "Masonry Tools");
            AddTag(context, "Plastering Trowel", "Masonry Tools");
            AddTag(context, "Mortar Hoe", "Masonry Tools");
            AddTag(context, "Earth Rammer", "Masonry Tools");

            // Wheelwrighting
            AddTag(context, "Wheelwright Tools", "Woodcrafting Tools");
            AddTag(context, "Spokeshave", "Wheelwright Tools");
            AddTag(context, "Drawknife", "Wheelwright Tools");
            AddTag(context, "Travisher", "Wheelwright Tools");
            AddTag(context, "Wheelwright's Compass", "Wheelwright Tools");
            AddTag(context, "Hub Borer", "Wheelwright Tools");
            AddTag(context, "Tenon Cutter", "Wheelwright Tools");
            AddTag(context, "Felloe Shave", "Wheelwright Tools");
            AddTag(context, "Wheelwright's Clamp", "Wheelwright Tools");
            AddTag(context, "Wheelwright's Axe", "Wheelwright Tools");

            // Coopering
            AddTag(context, "Coopering Tools", "Woodcrafting Tools");
            AddTag(context, "Cooper's Adze", "Coopering Tools");
            AddTag(context, "Cooper's Axe", "Coopering Tools");
            AddTag(context, "Croze", "Coopering Tools");
            AddTag(context, "Howel", "Coopering Tools");
            AddTag(context, "Hoop Driver", "Coopering Tools");
            AddTag(context, "Bung Borer", "Coopering Tools");
            AddTag(context, "Cooper's Jointer", "Coopering Tools");
            AddTag(context, "Cooper's Mallet", "Coopering Tools");

            // Basketry
            AddTag(context, "Basketry Tools", "Textilecraft Tools");
            AddTag(context, "Basket Knife", "Basketry Tools");
            AddTag(context, "Packing Bone", "Basketry Tools");
            AddTag(context, "Weaving Bodkin", "Basketry Tools");
            AddTag(context, "Reed Splitter", "Basketry Tools");
            AddTag(context, "Basket Clamp", "Basketry Tools");
            AddTag(context, "Sleeking Bone", "Basketry Tools");
            AddTag(context, "Basket Gauge", "Basketry Tools");

            // Ropemaking
            AddTag(context, "Ropemaking Tools", "Textilecraft Tools");
            AddTag(context, "Ropewalk", "Ropemaking Tools");
            AddTag(context, "Rope Hook", "Ropemaking Tools");
            AddTag(context, "Ropemaking Top", "Ropemaking Tools");
            AddTag(context, "Serving Mallet", "Ropemaking Tools");
            AddTag(context, "Marlinespike", "Ropemaking Tools");
            AddTag(context, "Fid", "Ropemaking Tools");
            AddTag(context, "Twine Shuttle", "Ropemaking Tools");

            // Fishing
            AddTag(context, "Fishing Tools", "Tools");
            AddTag(context, "Fish Hook", "Fishing Tools");
            AddTag(context, "Gorge Hook", "Fishing Tools");
            AddTag(context, "Harpoon", "Fishing Tools");
            AddTag(context, "Fish Spear", "Fishing Tools");
            AddTag(context, "Leister", "Fishing Tools");
            AddTag(context, "Casting Net", "Fishing Tools");
            AddTag(context, "Drag Net", "Fishing Tools");
            AddTag(context, "Fish Trap", "Fishing Tools");
            AddTag(context, "Net Needle", "Fishing Tools");
            AddTag(context, "Net Gauge", "Fishing Tools");

            // Scribing
            AddTag(context, "Scribing Tools", "Tools");
            AddTag(context, "Stylus", "Scribing Tools");
            AddTag(context, "Reed Pen", "Scribing Tools");
            AddTag(context, "Quill Pen", "Scribing Tools");
            AddTag(context, "Ink Brush", "Scribing Tools");
            AddTag(context, "Pen Knife", "Scribing Tools");
            AddTag(context, "Scraper Knife", "Scribing Tools");
            AddTag(context, "Wax Spatula", "Scribing Tools");
            AddTag(context, "Inkwell", "Scribing Tools");
            AddTag(context, "Seal Stamp", "Scribing Tools");

            // Milling
            AddTag(context, "Milling Tools", "Tools");
            AddTag(context, "Saddle Quern", "Milling Tools");
            AddTag(context, "Rotary Quern", "Milling Tools");
            AddTag(context, "Hand Mill", "Milling Tools");
            AddTag(context, "Millstone", "Milling Tools");
            AddTag(context, "Grain Sieve", "Milling Tools");

            // Papyrus Making
            AddTag(context, "Papyrusmaking Tools", "Tools");
            AddTag(context, "Papyrus Strip Knife", "Papyrusmaking Tools");
            AddTag(context, "Papyrus Pith Needle", "Papyrusmaking Tools");
            AddTag(context, "Papyrus Soaking Trough", "Papyrusmaking Tools");
            AddTag(context, "Papyrus Pressing Board", "Papyrusmaking Tools");
            AddTag(context, "Papyrus Beating Mallet", "Papyrusmaking Tools");
            AddTag(context, "Papyrus Burnishing Shell", "Papyrusmaking Tools");
            AddTag(context, "Papyrus Drying Frame", "Papyrusmaking Tools");

            // Parchment Making
            AddTag(context, "Parchmentmaking Tools", "Tools");
            AddTag(context, "Parchment Fleshing Beam", "Parchmentmaking Tools");
            AddTag(context, "Parchment Scraping Knife", "Parchmentmaking Tools");
            AddTag(context, "Parchment Lunellum", "Parchmentmaking Tools");
            AddTag(context, "Parchment Stretching Frame", "Parchmentmaking Tools");
            AddTag(context, "Parchment Pegs", "Parchmentmaking Tools");
            AddTag(context, "Parchment Lacing Cord", "Parchmentmaking Tools");
            AddTag(context, "Pounce Bag", "Parchmentmaking Tools");
            AddTag(context, "Parchment Pumice", "Parchmentmaking Tools");
            AddTag(context, "Parchment Whitening Chalk", "Parchmentmaking Tools");

            // Bookbinding
            AddTag(context, "Bookbinding Tools", "Tools");
            AddTag(context, "Bookbinder's Sewing Frame", "Bookbinding Tools");
            AddTag(context, "Bookbinder's Needle", "Bookbinding Tools");
            AddTag(context, "Backing Hammer", "Bookbinding Tools");
            AddTag(context, "Lying Press", "Bookbinding Tools");
            AddTag(context, "Book Plough", "Bookbinding Tools");
            AddTag(context, "Book Press", "Bookbinding Tools");
            AddTag(context, "Endband Needle", "Bookbinding Tools");
            AddTag(context, "Sewing Support Cords", "Bookbinding Tools");
            AddTag(context, "Bookbinder's Punch", "Bookbinding Tools");
            AddTag(context, "Leather Paring Knife", "Bookbinding Tools");

            // Scrollmaking
            AddTag(context, "Scrollmaking Tools", "Tools");
            AddTag(context, "Scroll Roller Rod", "Scrollmaking Tools");
            AddTag(context, "Scroll End Knob", "Scrollmaking Tools");
            AddTag(context, "Scroll Tie Ribbon", "Scrollmaking Tools");
            AddTag(context, "Scroll Case", "Scrollmaking Tools");
            AddTag(context, "Scroll Label Tab", "Scrollmaking Tools");
            AddTag(context, "Scroll Seal Cord", "Scrollmaking Tools");
            AddTag(context, "Scroll Smoothing Stone", "Scrollmaking Tools");

            // Calligraphy
            AddTag(context, "Calligraphy Tools", "Tools");
            AddTag(context, "Qalam Cutter", "Calligraphy Tools");
            AddTag(context, "Quill Curing Sand", "Calligraphy Tools");
            AddTag(context, "Pen Rest", "Calligraphy Tools");
            AddTag(context, "Pen Wiper", "Calligraphy Tools");
            AddTag(context, "Calligrapher's Brush", "Calligraphy Tools");
            AddTag(context, "Ruling Board", "Calligraphy Tools");
            AddTag(context, "Manuscript Pricker", "Calligraphy Tools");
            AddTag(context, "Pen Rack", "Calligraphy Tools");

            // Illumination
            AddTag(context, "Illumination Tools", "Tools");
            AddTag(context, "Gesso Pot", "Illumination Tools");
            AddTag(context, "Gold Leaf Cushion", "Illumination Tools");
            AddTag(context, "Gilding Knife", "Illumination Tools");
            AddTag(context, "Gilding Tip", "Illumination Tools");
            AddTag(context, "Agate Burnisher", "Illumination Tools");
            AddTag(context, "Pigment Shell", "Illumination Tools");
            AddTag(context, "Pigment Muller", "Illumination Tools");
            AddTag(context, "Miniature Detail Brush", "Illumination Tools");
            AddTag(context, "Palette Slab", "Illumination Tools");

            // Woodblock Printing
            AddTag(context, "Woodblock Printing Tools", "Tools");
            AddTag(context, "Block Carving Knife", "Woodblock Printing Tools");
            AddTag(context, "Block Clearing Chisel", "Woodblock Printing Tools");
            AddTag(context, "Registration Pin", "Woodblock Printing Tools");
            AddTag(context, "Printing Baren", "Woodblock Printing Tools");
            AddTag(context, "Ink Dauber", "Woodblock Printing Tools");
            AddTag(context, "Paste Pot", "Woodblock Printing Tools");
            AddTag(context, "Impression Spoon", "Woodblock Printing Tools");
            AddTag(context, "Paper Dampening Brush", "Woodblock Printing Tools");

            // Movable Type Printing
            AddTag(context, "Movable Type Printing Tools", "Tools");
            AddTag(context, "Type Mould", "Movable Type Printing Tools");
            AddTag(context, "Composing Stick", "Movable Type Printing Tools");
            AddTag(context, "Type Galley", "Movable Type Printing Tools");
            AddTag(context, "Type Case Rack", "Movable Type Printing Tools");
            AddTag(context, "Type Tweezers", "Movable Type Printing Tools");
            AddTag(context, "Frisket Frame", "Movable Type Printing Tools");
            AddTag(context, "Tympan Frame", "Movable Type Printing Tools");
            AddTag(context, "Printer's Quoin", "Movable Type Printing Tools");

            // Silk Reeling
            AddTag(context, "Silk Reeling Tools", "Textilecraft Tools");
            AddTag(context, "Reeling Basin", "Silk Reeling Tools");
            AddTag(context, "Cocoon Brush", "Silk Reeling Tools");
            AddTag(context, "Filament Guide Ring", "Silk Reeling Tools");
            AddTag(context, "Reeling Wheel", "Silk Reeling Tools");
            AddTag(context, "Twisting Reel", "Silk Reeling Tools");
            AddTag(context, "Sericulture Tray", "Silk Reeling Tools");
            AddTag(context, "Silk Drying Rack", "Silk Reeling Tools");

            // Lacquerwork
            AddTag(context, "Lacquerwork Tools", "Tools");
            AddTag(context, "Lacquer Spatula", "Lacquerwork Tools");
            AddTag(context, "Lacquer Drying Cabinet", "Lacquerwork Tools");
            AddTag(context, "Lacquer Polishing Charcoal", "Lacquerwork Tools");
            AddTag(context, "Powder Sifter Tray", "Lacquerwork Tools");
            AddTag(context, "Gold Powder Tube", "Lacquerwork Tools");
            AddTag(context, "Lacquerer's Brush", "Lacquerwork Tools");
            AddTag(context, "Lacquer Polishing Stone", "Lacquerwork Tools");

            // Clockmaking
            AddTag(context, "Clockmaking Tools", "Metalworking Tools");
            AddTag(context, "Mainspring Winder", "Clockmaking Tools");
            AddTag(context, "Clockmaker's Turns", "Clockmaking Tools");
            AddTag(context, "Depthing Tool", "Clockmaking Tools");
            AddTag(context, "Pivot File", "Clockmaking Tools");
            AddTag(context, "Pinion Cutter", "Clockmaking Tools");
            AddTag(context, "Fusee Vise", "Clockmaking Tools");
            AddTag(context, "Escapement Gauge", "Clockmaking Tools");
            AddTag(context, "Clockmaker's Broach", "Clockmaking Tools");

            // Brewing
            AddTag(context, "Brewing Tools", "Tools");
            AddTag(context, "Mash Tun", "Brewing Tools");
            AddTag(context, "Lauter Tun", "Brewing Tools");
            AddTag(context, "Brew Copper", "Brewing Tools");
            AddTag(context, "Sparging Ladle", "Brewing Tools");
            AddTag(context, "Mashing Paddle", "Brewing Tools");
            AddTag(context, "Wort Grant", "Brewing Tools");
            AddTag(context, "Fermenting Gyle Tun", "Brewing Tools");
            AddTag(context, "Hop Back", "Brewing Tools");

            // Distilling
            AddTag(context, "Distilling Tools", "Tools");
            AddTag(context, "Alembic Still", "Distilling Tools");
            AddTag(context, "Cucurbit", "Distilling Tools");
            AddTag(context, "Distillation Head", "Distilling Tools");
            AddTag(context, "Worm Tub", "Distilling Tools");
            AddTag(context, "Condensing Coil", "Distilling Tools");
            AddTag(context, "Spirit Receiver", "Distilling Tools");
            AddTag(context, "Luting Clay", "Distilling Tools");
            AddTag(context, "Spirit Hydrometer", "Distilling Tools");

            // Dyeing
            AddTag(context, "Dyeing Tools", "Textilecraft Tools");
            AddTag(context, "Dye Vat", "Dyeing Tools");
            AddTag(context, "Mordant Cauldron", "Dyeing Tools");
            AddTag(context, "Dye Stirring Pole", "Dyeing Tools");
            AddTag(context, "Skein Rack", "Dyeing Tools");
            AddTag(context, "Indigo Beating Paddle", "Dyeing Tools");
            AddTag(context, "Dye Strainer", "Dyeing Tools");
            AddTag(context, "Madder Grinding Quern", "Dyeing Tools");
            AddTag(context, "Dye Drying Line", "Dyeing Tools");

            // Fulling
            AddTag(context, "Fulling Tools", "Textilecraft Tools");
            AddTag(context, "Fulling Stocks", "Fulling Tools");
            AddTag(context, "Fuller's Trough", "Fulling Tools");
            AddTag(context, "Fuller's Mallet", "Fulling Tools");
            AddTag(context, "Teasel Frame", "Fulling Tools");
            AddTag(context, "Napping Shears", "Fulling Tools");
            AddTag(context, "Cloth Tenter Frame", "Fulling Tools");

            // Lensmaking
            AddTag(context, "Lensmaking Tools", "Scientific Tools");
            AddTag(context, "Lens Grinding Dish", "Lensmaking Tools");
            AddTag(context, "Lens Polishing Lap", "Lensmaking Tools");
            AddTag(context, "Optician's Pitch Block", "Lensmaking Tools");
            AddTag(context, "Spectacle Riveting Pliers", "Lensmaking Tools");
            AddTag(context, "Lens Edging Template", "Lensmaking Tools");
            AddTag(context, "Lens Centering Gauge", "Lensmaking Tools");
            AddTag(context, "Grinding Abrasive Slurry", "Lensmaking Tools");
            AddTag(context, "Lens Blocking Cement", "Lensmaking Tools");

            // Engraving
            AddTag(context, "Engraving Tools", "Tools");
            AddTag(context, "Engraver's Burin", "Engraving Tools");
            AddTag(context, "Etcher's Needle", "Engraving Tools");
            AddTag(context, "Copperplate Scraper", "Engraving Tools");
            AddTag(context, "Copperplate Burnisher", "Engraving Tools");
            AddTag(context, "Etching Ground Dauber", "Engraving Tools");
            AddTag(context, "Aquatint Dust Box", "Engraving Tools");
            AddTag(context, "Plate Warming Brazier", "Engraving Tools");
            AddTag(context, "Copperplate Wiping Muslin", "Engraving Tools");

            // Medical Tools
            AddTag(context, "Medical Tools", "Tools");
            AddTag(context, "Stethoscope", "Medical Tools");
            AddTag(context, "Blood Pressure Monitor", "Medical Tools");
            AddTag(context, "Ophthalmoscope", "Medical Tools");
            AddTag(context, "Otascope", "Medical Tools");
            AddTag(context, "Dermatoscope", "Medical Tools");
            AddTag(context, "Electrocardiogram", "Medical Tools");
            AddTag(context, "Electroencephalogram", "Medical Tools");
            AddTag(context, "Glucometer", "Medical Tools");
            AddTag(context, "Spirometer", "Medical Tools");
            AddTag(context, "Mechanical Scale", "Medical Tools");
            AddTag(context, "Height Measuring Scale", "Medical Tools");
            AddTag(context, "Tendon Hammer", "Medical Tools");
            AddTag(context, "Human Blood Typing", "Medical Tools");

            // Surgical Tools
            AddTag(context, "Surgical Tools", "Tools");
            AddTag(context, "Scalpel", "Surgical Tools");
            AddTag(context, "Bonesaw", "Surgical Tools");
            AddTag(context, "Forceps", "Surgical Tools");
            AddTag(context, "Tissue Forceps", "Forceps");
            AddTag(context, "Dissecting Forceps", "Forceps");
            AddTag(context, "Kelly Forceps", "Forceps");
            AddTag(context, "DeBakey Forceps", "Forceps");
            AddTag(context, "Towel Clamp", "Surgical Tools");
            AddTag(context, "Intestinal Clamp", "Surgical Tools");
            AddTag(context, "Arterial Clamp", "Surgical Tools");
            AddTag(context, "Curette", "Surgical Tools");
            AddTag(context, "Surgical Retractor", "Surgical Tools");
            AddTag(context, "Speculum", "Surgical Tools");
            AddTag(context, "Surgical Suture Needle", "Surgical Tools");
            AddTag(context, "Ski Suture Needle", "Surgical Suture Needle");
            AddTag(context, "Canoe Suture Needle", "Surgical Suture Needle");
            AddTag(context, "Atraumatic Suture Needle", "Surgical Suture Needle");
            AddTag(context, "Trocar", "Surgical Tools");
            AddTag(context, "Absorbable Suture", "Surgical Suturing");
            AddTag(context, "Non-Absorbable Suture", "Surgical Suturing");
            AddTag(context, "Plain Gut Suture", "Absorbable Suture");
            AddTag(context, "Chromic Gut Suture", "Absorbable Suture");
            AddTag(context, "Fast Gut Suture", "Absorbable Suture");
            AddTag(context, "Synthetic Monofilament Suture", "Absorbable Suture");
            AddTag(context, "Synthetic Polyfilament Suture", "Absorbable Suture");
            AddTag(context, "Silk Suture", "Non-Absorbable Suture");
            AddTag(context, "Nylon Monofilament Suture", "Non-Absorbable Suture");
            AddTag(context, "Nylon Polyfilament Suture", "Non-Absorbable Suture");
            AddTag(context, "Surgical Steel Suture", "Non-Absorbable Suture");
            AddTag(context, "Surgical Stapler", "Surgical Tools");
            AddTag(context, "Surgical Staples", "Surgical Tools");

            // Science Tools
            AddTag(context, "Scientific Tools", "Tools");
            AddTag(context, "Plane Table", "Scientific Tools");

            // Measurement Tools
            AddTag(context, "Measurement Tools", "Scientific Tools");
            AddTag(context, "Micrometer", "Measurement Tools");
            AddTag(context, "Tribometer", "Measurement Tools");
            AddTag(context, "Absorption Wavemeter", "Measurement Tools");
            AddTag(context, "Accelerometer", "Measurement Tools");
            AddTag(context, "Pressure Gauge", "Measurement Tools");
            AddTag(context, "Voltmeter", "Measurement Tools");
            AddTag(context, "Flow Meter", "Measurement Tools");
            AddTag(context, "Capacitance Meter", "Measurement Tools");
            AddTag(context, "Chondrometer", "Measurement Tools");
            AddTag(context, "Deposit Gauge", "Measurement Tools");
            AddTag(context, "Diffusion Tube", "Measurement Tools");
            AddTag(context, "Dynameter", "Measurement Tools");
            AddTag(context, "Watch", "Measurement Tools");
            AddTag(context, "Stopwatch", "Watch");
            AddTag(context, "Wristwatch", "Watch");
            AddTag(context, "Chronograph", "Watch");
            AddTag(context, "Actinometer", "Measurement Tools");
            AddTag(context, "Aerometer", "Measurement Tools");
            AddTag(context, "Hydrometer", "Measurement Tools");
            AddTag(context, "Bolometer", "Measurement Tools");
            AddTag(context, "Butyrometer", "Measurement Tools");
            AddTag(context, "Calorimeter", "Measurement Tools");
            AddTag(context, "Anemometer", "Measurement Tools");
            AddTag(context, "Auxanometer", "Measurement Tools");
            AddTag(context, "Electrometer", "Measurement Tools");
            AddTag(context, "Electroscope", "Measurement Tools");
            AddTag(context, "Eudiometer", "Measurement Tools");
            AddTag(context, "Explosimeter", "Measurement Tools");
            AddTag(context, "Force Gauge", "Measurement Tools");
            AddTag(context, "Faraday Cup", "Measurement Tools");
            AddTag(context, "Geiger Counter", "Measurement Tools");
            AddTag(context, "Kofler Bench", "Measurement Tools");
            AddTag(context, "Load Cell", "Measurement Tools");
            AddTag(context, "Magnetometer", "Measurement Tools");
            AddTag(context, "Mass Spectrometer", "Measurement Tools");
            AddTag(context, "Multimeter", "Measurement Tools");
            AddTag(context, "Optometer", "Measurement Tools");
            AddTag(context, "Osmometer", "Measurement Tools");
            AddTag(context, "Pedometer", "Measurement Tools");
            AddTag(context, "Penetrometer", "Measurement Tools");
            AddTag(context, "PH Meter", "Measurement Tools");
            AddTag(context, "Phoropter", "Measurement Tools");
            AddTag(context, "Radar", "Measurement Tools");
            AddTag(context, "Radar Speed Gun", "Measurement Tools");
            AddTag(context, "Radiosonde", "Measurement Tools");
            AddTag(context, "Refractometer", "Measurement Tools");
            AddTag(context, "Seismometer", "Measurement Tools");
            AddTag(context, "Speedometer", "Measurement Tools");
            AddTag(context, "Stereoautograph", "Measurement Tools");
            AddTag(context, "Strain Gauge", "Measurement Tools");
            AddTag(context, "Tachometer", "Measurement Tools");
            AddTag(context, "Water Sensor", "Measurement Tools");
            AddTag(context, "Rain Gauge", "Measurement Tools");
            AddTag(context, "Thermometer", "Measurement Tools");
            AddTag(context, "Barometer", "Measurement Tools");

            // Survey Equipment
            AddTag(context, "Surveying Equipment", "Measurement Tools");
            AddTag(context, "Levelling Rod", "Surveying Equipment");
            AddTag(context, "Theodolyte", "Surveying Equipment");
            AddTag(context, "Plumb Bob", "Surveying Equipment");
            AddTag(context, "Surveyor's Chain", "Surveying Equipment");
            AddTag(context, "Circumferentor", "Surveying Equipment");
            AddTag(context, "Surveying Alidade", "Surveying Equipment");
            AddTag(context, "Jacob's Staff", "Surveying Equipment");
            AddTag(context, "Gunter's Scale", "Surveying Equipment");
            AddTag(context, "Surveyor's Cross", "Surveying Equipment");
            AddTag(context, "Surveyor's Field Book", "Surveying Equipment");
            AddTag(context, "Measuring Rod", "Surveying Equipment");

            // Navigational Tools
            AddTag(context, "Navigational Tools", "Scientific Tools");
            AddTag(context, "Airspeed Indicator", "Navigational Tools");
            AddTag(context, "Altimeter", "Navigational Tools");
            AddTag(context, "Backstaff", "Navigational Tools");
            AddTag(context, "Compass", "Navigational Tools");
            AddTag(context, "Pelorus", "Navigational Tools");
            AddTag(context, "Sextant", "Navigational Tools");
            AddTag(context, "Variometer", "Navigational Tools");
            AddTag(context, "Orrery", "Navigational Tools");
            AddTag(context, "Astrolabe", "Navigational Tools");
            AddTag(context, "Nocturlabe", "Navigational Tools");
            AddTag(context, "Sundial", "Navigational Tools");
            AddTag(context, "Telescope", "Navigational Tools");
            AddTag(context, "Periscope", "Navigational Tools");
            AddTag(context, "Mariner's Cross-Staff", "Navigational Tools");
            AddTag(context, "Octant", "Navigational Tools");
            AddTag(context, "Marine Chronometer", "Navigational Tools");
            AddTag(context, "Traverse Board", "Navigational Tools");
            AddTag(context, "Lead Line", "Navigational Tools");
            AddTag(context, "Chip Log Reel", "Navigational Tools");
            AddTag(context, "Sea Sandglass", "Navigational Tools");
            AddTag(context, "Azimuth Compass", "Navigational Tools");

            // Specialised Vessels
            AddTag(context, "Specialised Vessels", "Scientific Tools");
            AddTag(context, "Beaker", "Specialised Vessels");
            AddTag(context, "Volumetric Flask", "Specialised Vessels");
            AddTag(context, "Test Tube", "Specialised Vessels");
            AddTag(context, "Graduated Cylinder", "Specialised Vessels");
            AddTag(context, "Burette", "Specialised Vessels");
            AddTag(context, "Volumetric Pipette", "Specialised Vessels");
            AddTag(context, "Evaporating Dish", "Specialised Vessels");
            AddTag(context, "Petri Dish", "Specialised Vessels");
            AddTag(context, "Watch Glass", "Specialised Vessels");
            AddTag(context, "Titration Flask", "Specialised Vessels");
            AddTag(context, "Vacuum Flask", "Specialised Vessels");
            AddTag(context, "Retort", "Specialised Vessels");
            AddTag(context, "Round-Bottom Flask", "Specialised Vessels");
            AddTag(context, "Dropper", "Specialised Vessels");
            AddTag(context, "Laboratory Funnel", "Specialised Vessels");
            AddTag(context, "Vial", "Specialised Vessels");
            AddTag(context, "Reagent Bottle", "Specialised Vessels");

            // Laboratory Equipment
            AddTag(context, "Laboratory Equipment", "Scientific Tools");
            AddTag(context, "Laboratory Burner", "Laboratory Equipment");
            AddTag(context, "Bunsen Burner", "Laboratory Burner");
            AddTag(context, "Alcohol Burner", "Laboratory Burner");
            AddTag(context, "Meker Burner", "Laboratory Burner");
            AddTag(context, "Dessicator", "Laboratory Equipment");
            AddTag(context, "Heating Mantle", "Laboratory Equipment");
            AddTag(context, "Laboratory Hot Plate", "Laboratory Equipment");
            AddTag(context, "Laboratory Oven", "Laboratory Equipment");
            AddTag(context, "Laboratory Kiln", "Laboratory Equipment");
            AddTag(context, "Vacuum Dry Box", "Laboratory Equipment");
            AddTag(context, "Beaker Clamp", "Laboratory Equipment");
            AddTag(context, "Burette Clamp", "Laboratory Equipment");
            AddTag(context, "Flask Clamp", "Laboratory Equipment");
            AddTag(context, "Test Tube Rack", "Laboratory Equipment");
            AddTag(context, "Test Tube Holder", "Laboratory Equipment");
            AddTag(context, "Laboratory Tripod", "Laboratory Equipment");

            // Grooming
            AddTag(context, "Grooming", "Tools");
            AddTag(context, "Hairbrush", "Grooming");
            AddTag(context, "Comb", "Grooming");
            AddTag(context, "Hair Scissors", "Grooming");
            AddTag(context, "Shaving Razor", "Grooming");
            AddTag(context, "Tweezers", "Grooming");
            AddTag(context, "Hair Curlers", "Grooming");
            AddTag(context, "Hairdryer", "Grooming");
            AddTag(context, "Hair Straightener", "Grooming");
            AddTag(context, "Detangling Brush", "Grooming");
            AddTag(context, "Detangling Comb", "Grooming");
            AddTag(context, "Sectioning Clips", "Grooming");
            AddTag(context, "Edge Brush", "Grooming");
            AddTag(context, "Diffuser", "Grooming");

            // Market Items
            #region Market Items
            AddTag(context, "Market", "");

            AddTag(context, "Nourishment", "Market");
            AddTag(context, "Staple Food", "Nourishment");
            AddTag(context, "Standard Food", "Nourishment");
            AddTag(context, "Luxury Food", "Nourishment");
            AddTag(context, "Seasonings", "Nourishment");
            AddTag(context, "Salt", "Seasonings");
            AddTag(context, "Spices", "Seasonings");

            AddTag(context, "Clothing", "Market");
            AddTag(context, "Simple Clothing", "Clothing");
            AddTag(context, "Standard Clothing", "Clothing");
            AddTag(context, "Luxury Clothing", "Clothing");
            AddTag(context, "Winter Clothing", "Clothing");
            AddTag(context, "Military Uniforms", "Clothing");

            AddTag(context, "Domestic Heating", "Market");
            AddTag(context, "Combustion Heating", "Domestic Heating");
            AddTag(context, "Oil Heating", "Domestic Heating");
            AddTag(context, "Electric Heating", "Domestic Heating");

            AddTag(context, "Intoxicants", "Market");
            AddTag(context, "Wine", "Intoxicants");
            AddTag(context, "Beer", "Intoxicants");
            AddTag(context, "Mead", "Intoxicants");
            AddTag(context, "Spirits", "Intoxicants");

            AddTag(context, "Luxury Drinks", "Market");
            AddTag(context, "Tea", "Luxury Drinks");
            AddTag(context, "Coffee", "Luxury Drinks");

            AddTag(context, "Household Goods", "Market");
            AddTag(context, "Simple Furniture", "Household Goods");
            AddTag(context, "Standard Furniture", "Household Goods");
            AddTag(context, "Luxury Furniture", "Household Goods");
            AddTag(context, "Simple Decorations", "Household Goods");
            AddTag(context, "Standard Decorations", "Household Goods");
            AddTag(context, "Luxury Decorations", "Household Goods");
            AddTag(context, "Simple Wares", "Household Goods");
            AddTag(context, "Standard Wares", "Household Goods");
            AddTag(context, "Luxury Wares", "Household Goods");

            AddTag(context, "Military Goods", "Market");
            AddTag(context, "Weapons", "Military Goods");
            AddTag(context, "Spears", "Weapons");
            AddTag(context, "Swords", "Weapons");
            AddTag(context, "Clubs", "Weapons");
            AddTag(context, "Axes", "Weapons");
            AddTag(context, "Maces", "Weapons");
            AddTag(context, "Daggers", "Weapons");
            AddTag(context, "Crossbows", "Weapons");
            AddTag(context, "Bows", "Weapons");
            AddTag(context, "Guns", "Weapons");
            AddTag(context, "Hammers", "Weapons");
            AddTag(context, "Polearms", "Weapons");
            AddTag(context, "Other Weapons", "Weapons");
            AddTag(context, "Ammunition", "Military Goods");
            AddTag(context, "Arrows", "Ammunition");
            AddTag(context, "Bolts", "Ammunition");
            AddTag(context, "Bullets", "Ammunition");
            AddTag(context, "Blackpowder", "Ammunition");
            AddTag(context, "Armour", "Military Goods");
            AddTag(context, "Leather Armour", "Armour");
            AddTag(context, "Mail Armour", "Armour");
            AddTag(context, "Plate Armour", "Armour");
            AddTag(context, "Primitive Armour", "Armour");
            AddTag(context, "Shields", "Armour");

            AddTag(context, "Transportation", "Market");
            AddTag(context, "Cargo Transportation", "Transportation");
            AddTag(context, "Cart Haulage", "Cargo Transportation");
            AddTag(context, "Manual Haulage", "Cargo Transportation");
            AddTag(context, "Mule Haulage", "Cargo Transportation");
            AddTag(context, "Ship Haulage", "Cargo Transportation");
            AddTag(context, "Passenger Transportation", "Transportation");
            AddTag(context, "Cart Passage", "Passenger Transportation");
            AddTag(context, "Horse Passage", "Passenger Transportation");
            AddTag(context, "Wagon Passage", "Passenger Transportation");
            AddTag(context, "Ship Passage", "Passenger Transportation");

            AddTag(context, "Medicine", "Market");
            AddTag(context, "Simple Medicine", "Medicine");
            AddTag(context, "Standard Medicine", "Medicine");
            AddTag(context, "High-Quality Medicine", "Medicine");

            AddTag(context, "Writing Materials", "Market");
            AddTag(context, "Wax Tablets", "Writing Materials");
            AddTag(context, "Parchment", "Writing Materials");
            AddTag(context, "Paper", "Writing Materials");
            AddTag(context, "Ink", "Writing Materials");

            AddTag(context, "Warehousing", "Market");

            AddTag(context, "Professional Tools", "Market");
            AddTag(context, "Primitive Tools", "Professional Tools");
            AddTag(context, "Simple Tools", "Professional Tools");
            AddTag(context, "Standard Tools", "Professional Tools");
            AddTag(context, "High-Quality Tools", "Professional Tools");

            AddTag(context, "Raw Materials", "Market");
            AddTag(context, "Lumber", "Raw Materials");
            AddTag(context, "Straw", "Raw Materials");
            AddTag(context, "Cloth", "Raw Materials");
            AddTag(context, "Stone Blocks", "Raw Materials");
            AddTag(context, "Sand", "Raw Materials");
            AddTag(context, "Clay", "Raw Materials");
            AddTag(context, "Aggregate", "Raw Materials");
            AddTag(context, "Cement Mineral", "Raw Materials");
            AddTag(context, "Steel", "Raw Materials");
            AddTag(context, "Copper", "Raw Materials");
            AddTag(context, "Gold", "Raw Materials");
            AddTag(context, "Silver", "Raw Materials");
            AddTag(context, "Bronze", "Raw Materials");
            AddTag(context, "Brass", "Raw Materials");
            AddTag(context, "Lead", "Raw Materials");

            AddTag(context, "Lighting", "Market");
            AddTag(context, "Candles", "Lighting");
            AddTag(context, "Torches", "Lighting");
            AddTag(context, "Lamps", "Lighting");

            AddTag(context, "Hospitality", "Market");
            AddTag(context, "Simple Lodging", "Hospitality");
            AddTag(context, "Standard Lodging", "Hospitality");
            AddTag(context, "Luxury Lodging", "Hospitality");
            AddTag(context, "Prepared Meals", "Hospitality");
            AddTag(context, "Common Meals", "Prepared Meals");
            AddTag(context, "Fine Dining", "Prepared Meals");
            AddTag(context, "Bathhouse Services", "Hospitality");
            AddTag(context, "Stabling Services", "Hospitality");

            AddTag(context, "Entertainment", "Market");
            AddTag(context, "Cheap Entertainment", "Entertainment");
            AddTag(context, "Standard Entertainment", "Entertainment");
            AddTag(context, "Luxury Entertainment", "Entertainment");
            AddTag(context, "Music Performance", "Entertainment");
            AddTag(context, "Theatre Performance", "Entertainment");
            AddTag(context, "Festival Entertainment", "Entertainment");
            AddTag(context, "Sporting Entertainment", "Entertainment");

            AddTag(context, "Personal Services", "Market");
            AddTag(context, "Laundry Services", "Personal Services");
            AddTag(context, "Barbering", "Personal Services");
            AddTag(context, "Tailoring Services", "Personal Services");
            AddTag(context, "Domestic Services", "Personal Services");
            AddTag(context, "Bodyguard Services", "Personal Services");
            AddTag(context, "Bathing Services", "Personal Services");
            AddTag(context, "Grooming Supplies", "Personal Services");

            AddTag(context, "Communications", "Market");
            AddTag(context, "Messenger Services", "Communications");
            AddTag(context, "Courier Services", "Communications");
            AddTag(context, "Postal Services", "Communications");
            AddTag(context, "Printed News", "Communications");
            AddTag(context, "Telegraph Services", "Communications");
            AddTag(context, "Telephone Services", "Communications");

            AddTag(context, "Religious Goods", "Market");
            AddTag(context, "Ritual Supplies", "Religious Goods");
            AddTag(context, "Temple Offerings", "Religious Goods");
            AddTag(context, "Funerary Goods", "Religious Goods");
            AddTag(context, "Devotional Goods", "Religious Goods");

            AddTag(context, "Construction Materials", "Market");
            AddTag(context, "Brick", "Construction Materials");
            AddTag(context, "Mortar", "Construction Materials");
            AddTag(context, "Lime", "Construction Materials");
            AddTag(context, "Worked Timber", "Construction Materials");
            AddTag(context, "Worked Stone", "Construction Materials");
            AddTag(context, "Glass Panes", "Construction Materials");
            AddTag(context, "Roofing Materials", "Construction Materials");

            AddTag(context, "Household Consumables", "Market");
            AddTag(context, "Soap", "Household Consumables");
            AddTag(context, "Lamp Oil", "Household Consumables");
            AddTag(context, "Cleaning Supplies", "Household Consumables");
            AddTag(context, "Candlemaking Wax", "Household Consumables");
            AddTag(context, "Toiletries", "Household Consumables");

            #endregion

            #region Item Seeder Specific Tags
            AddTag(context, "Consumables", "");
            AddTag(context, "Padded Vest", "Consumables");
            AddTag(context, "Padded Gloves", "Consumables");
            AddTag(context, "Padded Trousers", "Consumables");
            AddTag(context, "Thick Leather", "Consumables");
            AddTag(context, "Armouring Rings", "Consumables");
            AddTag(context, "Armouring Studs", "Consumables");
            AddTag(context, "Armouring Scales", "Consumables");
            AddTag(context, "Wire", "Consumables");
            AddTag(context, "Sheet Metal", "Consumables");
            AddTag(context, "Deer Hindquarter", "Consumables");
            AddTag(context, "Deer Forequarter", "Consumables");
            AddTag(context, "Rump", "Consumables");
            AddTag(context, "Tenderloin", "Consumables");
            AddTag(context, "Pig Hindquarter", "Consumables");
            AddTag(context, "Pig Forequarter", "Consumables");
            AddTag(context, "Entrails", "Consumables");
            AddTag(context, "Suet", "Consumables");
            AddTag(context, "Plank", "Consumables");
            AddTag(context, "Log", "Consumables");
            AddTag(context, "Dirt", "Consumables");
            AddTag(context, "Nuts", "Consumables");
            AddTag(context, "Rabbit Roast", "Consumables");
            AddTag(context, "Deer Roast", "Consumables");
            AddTag(context, "Pig Roast", "Consumables");
            AddTag(context, "Grove Trees", "Consumables");
            AddTag(context, "Tree", "Consumables");
            AddTag(context, "Trunk", "Consumables");
            AddTag(context, "Open Grave", "Consumables");
            AddTag(context, "Thick Hide", "Consumables");
            AddTag(context, "Branch", "Consumables");
            AddTag(context, "Short Shaft", "Consumables");
            AddTag(context, "Knapped Stone", "Consumables");
            AddTag(context, "Reeds", "Consumables");
            AddTag(context, "Sword Blade", "Consumables");
            AddTag(context, "Knife Blade", "Consumables");
            AddTag(context, "Mace Head", "Consumables");
            AddTag(context, "Axe Head", "Consumables");
            AddTag(context, "Pole", "Consumables");
            AddTag(context, "Grass", "Consumables");
            AddTag(context, "Tusk", "Consumables");
            AddTag(context, "Spearhead", "Consumables");
            AddTag(context, "Mould Sheet Metal", "Consumables");
            AddTag(context, "Mould Sword Blade", "Consumables");
            AddTag(context, "Mould Knife Blade", "Consumables");
            AddTag(context, "Mould Mace Head", "Consumables");
            AddTag(context, "Mould Axe Head", "Consumables");
            AddTag(context, "Mould Spearhead", "Consumables");
            AddTag(context, "Construction Brick", "Consumables");
            AddTag(context, "Fletching", "Consumables");
            AddTag(context, "Sword Grip", "Consumables");
            AddTag(context, "Curved Leather Piece", "Consumables");
            AddTag(context, "Long Shaft", "Consumables");
            AddTag(context, "Drawplate", "Consumables");
            AddTag(context, "Mould Brick", "Consumables");
            #endregion

            context.SaveChanges();
        }
    }
}
