using MudSharp.Communication.Language;
using MudSharp.GameItems;
using MudSharp.Health;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Temporary_Scratch_App;

public sealed record ShapeDamageRemap(
    bool Allow,
    double ChanceMultiplier = 1.0,
    int DistinctivenessAdjustment = 0,
    int SizeAdjustment = 0,
    MudSharp.Health.WoundSeverity? MinimumSeverityFloor = null,
    string? PatternFamilyOverride = null,
    string? ShortBaseOverride = null,
    string? FullBaseOverride = null,
    string? CausePhraseOverride = null);

public sealed class ScarTemplateExportFile
{
    public DateTime GeneratedUtc { get; init; }
    public int TemplateCount { get; init; }
    public List<ScarTemplateExport> Templates { get; init; } = new();
}

public sealed class ScarTemplateExport
{
    public string Name { get; init; } = "";
    public string ShortDescription { get; init; } = "";
    public string FullDescription { get; init; } = "";
    public int SizeSteps { get; init; }
    public int Distinctiveness { get; init; }
    public double DamageHealingScarChance { get; init; }
    public Dictionary<MudSharp.Health.DamageType, MudSharp.Health.WoundSeverity> DamageTypes { get; init; } = new();
    public List<string> BodypartShapeNames { get; init; } = new();

    public static ScarTemplateExport FromSeederDefinition(SeederScarTemplateDefinition source)
    {
        return new ScarTemplateExport
        {
            Name = source.Name,
            ShortDescription = source.ShortDescription,
            FullDescription = source.FullDescription,
            SizeSteps = source.SizeSteps,
            Distinctiveness = source.Distinctiveness,
            DamageHealingScarChance = source.DamageHealingScarChance,
            DamageTypes = source.DamageTypes.ToDictionary(x => x.Key, x => x.Value),
            BodypartShapeNames = source.BodypartShapeNames.ToList()
        };
    }
}

internal class Program
{
    public static int Main(string[] args)
    {
        try
        {
            var outputPath = args.Length > 0
                ? Path.GetFullPath(args[0])
                : Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "human_scar_templates.json"));

            Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);

            // Assumes all the code you already generated exists in this class.
            // If BuildHumanScarTemplates() / HumanScarTemplates live elsewhere,
            // change this line to call that type instead.
            var generatedTemplates = BuildHumanScarTemplates();

            var export = generatedTemplates
                .Select(ScarTemplateExport.FromSeederDefinition)
                .OrderBy(x => x.Name, StringComparer.Ordinal)
                .ToList();

            var payload = new ScarTemplateExportFile
            {
                GeneratedUtc = DateTime.UtcNow,
                TemplateCount = export.Count,
                Templates = export
            };

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                DefaultIgnoreCondition = JsonIgnoreCondition.Never
            };
            options.Converters.Add(new JsonStringEnumConverter());

            var json = JsonSerializer.Serialize(payload, options);
            File.WriteAllText(outputPath, json);

            Console.WriteLine($"Wrote {export.Count} scar templates to:");
            Console.WriteLine(outputPath);
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("Failed to generate scar templates.");
            Console.Error.WriteLine(ex);
            return 1;
        }
    }

    private sealed record ScarSeveritySpec(
    string Label,
    MudSharp.Health.WoundSeverity MinimumSeverity,
    string ShortModifier,
    string FullModifier,
    int SizeAdjustment,
    int BaseDistinctiveness,
    double BaseChance);

    private sealed record HighSeverityScarShapeSpec(
        string Shape,
        string CodeName,
        int SizeAdjustment,
        string Geometry,
        string OrientationPool);

    private sealed record HighSeverityScarDamageSpec(
        MudSharp.Health.DamageType DamageType,
        string CodeName,
        string ShortBase,
        string FullBase,
        string CausePhrase,
        string PatternFamily,
        int SizeAdjustment,
        int DistinctivenessAdjustment,
        double ChanceAdjustment);

    private sealed record HighSeverityScarOrientationSpec(
    string CodeName,
    string NameFragment,
    string LinePhrase,
    string TrackPhrase,
    string PatchPhrase,
    string ClusterPhrase,
    string WarpPhrase);

    private static readonly IReadOnlyList<HighSeverityScarOrientationSpec> Eye3Orientations = new[]
    {
    new HighSeverityScarOrientationSpec(
        "Crescent",
        "Crescent",
        "scores a crescent across {0}",
        "cuts a crescented wound-track through {0}",
        "leaves a crescent of opacity across {0}",
        "dots {0} in a crescent cluster of tiny pits",
        "draws {0} into a crescented distortion"),
    new HighSeverityScarOrientationSpec(
        "Radial",
        "Radial",
        "branches radially across {0}",
        "drives a radial wound-track out across {0}",
        "fans outward across {0} in a cloudy burst",
        "stars outward across {0} in a clustered burst",
        "pulls {0} into a starburst distortion"),
    new HighSeverityScarOrientationSpec(
        "Offset",
        "Offset",
        "rides one side of {0}",
        "marks one side of {0} with a narrow wound-track",
        "clouds one side of {0} in an uneven patch",
        "clusters over one side of {0}",
        "drags one side of {0} visibly out of true")
};

    private static readonly IReadOnlyList<HighSeverityScarOrientationSpec> Mouth3Orientations = new[]
    {
    new HighSeverityScarOrientationSpec(
        "Lipline",
        "Lipline",
        "runs along the line of {0}",
        "marks the line of {0} with a tight wound-track",
        "sits along the line of {0} in a drawn patch",
        "lies as a broken cluster along the line of {0}",
        "draws the line of {0} crooked and tight"),
    new HighSeverityScarOrientationSpec(
        "Vertical",
        "Vertical",
        "splits vertically through one lip of {0}",
        "drives a narrow vertical wound-track through one lip of {0}",
        "spreads vertically through one side of the lips of {0}",
        "clusters vertically through one side of the lips of {0}",
        "pulls one lip of {0} into a stiff vertical distortion"),
    new HighSeverityScarOrientationSpec(
        "Cornerwise",
        "Cornerwise",
        "hooks from one corner of {0} inward",
        "marks one corner of {0} with a hooked wound-track",
        "sprawls from one corner of {0} in a drawn patch",
        "clusters around one corner of {0}",
        "drags one corner of {0} into a tight, uneven hook")
};

    private static readonly IReadOnlyList<HighSeverityScarOrientationSpec> Nipple3Orientations = new[]
    {
    new HighSeverityScarOrientationSpec(
        "Across",
        "Across",
        "cuts directly across {0}",
        "marks {0} with a hard wound-track straight through it",
        "spreads across {0} in a tightened patch",
        "clusters across {0} in a broken line of pits",
        "pulls {0} sharply out of shape"),
    new HighSeverityScarOrientationSpec(
        "Encircling",
        "Encircling",
        "rings {0} in a tight seam",
        "marks a near-circular wound-track around {0}",
        "forms a tight ring of scar around {0}",
        "lies in a broken ring around {0}",
        "draws {0} inward in a tightened ring"),
    new HighSeverityScarOrientationSpec(
        "Offset",
        "Offset",
        "rides one side of {0}",
        "marks one side of {0} with a narrow wound-track",
        "sprawls over one side of {0} in a tightened patch",
        "clusters over one side of {0}",
        "pulls one side of {0} badly off-centre")
};

    private static readonly IReadOnlyList<HighSeverityScarOrientationSpec> Nose3Orientations = new[]
    {
    new HighSeverityScarOrientationSpec(
        "Bridgewise",
        "Bridgewise",
        "runs down the bridge of {0}",
        "marks the bridge of {0} with a narrow wound-track",
        "spreads down the bridge of {0} in a tightened patch",
        "lies in a broken cluster down the bridge of {0}",
        "pulls the bridge of {0} visibly out of line"),
    new HighSeverityScarOrientationSpec(
        "Crosswise",
        "Crosswise",
        "cuts crosswise over {0}",
        "marks a crosswise wound-track across {0}",
        "spreads crosswise over {0} in a glossy patch",
        "clusters crosswise over {0}",
        "buckles {0} crosswise"),
    new HighSeverityScarOrientationSpec(
        "Sidewall",
        "Sidewall",
        "rides one sidewall of {0}",
        "marks one sidewall of {0} with a deep wound-track",
        "sprawls over one sidewall of {0} in a drawn patch",
        "clusters over one sidewall of {0}",
        "pulls one sidewall of {0} inward and uneven")
};

    private static readonly IReadOnlyList<HighSeverityScarOrientationSpec> Ear3Orientations = new[]
    {
    new HighSeverityScarOrientationSpec(
        "Rimwise",
        "Rimwise",
        "runs along the rim of {0}",
        "marks the rim of {0} with a tight wound-track",
        "spreads along the rim of {0} in a tightened patch",
        "lies in a broken cluster along the rim of {0}",
        "crumples the rim of {0} into a warped fold"),
    new HighSeverityScarOrientationSpec(
        "Crosswise",
        "Crosswise",
        "cuts across {0}",
        "marks a wound-track across {0}",
        "spreads across {0} in a glossy patch",
        "clusters across {0}",
        "buckles {0} across its cartilage"),
    new HighSeverityScarOrientationSpec(
        "Lobeward",
        "Lobeward",
        "runs downward toward the lobe of {0}",
        "marks a downward wound-track toward the lobe of {0}",
        "spreads downward toward the lobe of {0}",
        "clusters downward toward the lobe of {0}",
        "draws the lower edge of {0} into a twisted droop")
};

    private static readonly IReadOnlyList<HighSeverityScarOrientationSpec> Tongue4Orientations = new[]
    {
    new HighSeverityScarOrientationSpec(
        "DorsalLengthwise",
        "DorsalLengthwise",
        "runs down the centre of {0}",
        "marks the centre of {0} with a deep wound-track",
        "spreads down the centre of {0} in a tightened patch",
        "lies in a broken cluster down the centre of {0}",
        "draws the centre of {0} into a ridged distortion"),
    new HighSeverityScarOrientationSpec(
        "Crosswise",
        "Crosswise",
        "cuts across the upper surface of {0}",
        "marks a crosswise wound-track across the upper surface of {0}",
        "spreads crosswise over the upper surface of {0}",
        "clusters crosswise over the upper surface of {0}",
        "buckles the upper surface of {0} into a stiff fold"),
    new HighSeverityScarOrientationSpec(
        "Diagonal",
        "Diagonal",
        "slashes diagonally across {0}",
        "marks a diagonal wound-track across {0}",
        "spreads diagonally across {0}",
        "clusters diagonally across {0}",
        "twists part of {0} into a diagonal pull"),
    new HighSeverityScarOrientationSpec(
        "Edgewise",
        "Edgewise",
        "rides one edge of {0}",
        "marks one edge of {0} with a narrow wound-track",
        "sprawls over one edge of {0} in a tightened patch",
        "clusters over one edge of {0}",
        "pulls one edge of {0} into a ragged, tightened line")
};

    private static readonly IReadOnlyList<HighSeverityScarOrientationSpec> Testicles3Orientations = new[]
    {
    new HighSeverityScarOrientationSpec(
        "HighSide",
        "HighSide",
        "runs down one upper side of {0}",
        "marks one upper side of {0} with a narrow wound-track",
        "spreads over one upper side of {0} in a tightened patch",
        "clusters over one upper side of {0}",
        "pulls one upper side of {0} into a puckered distortion"),
    new HighSeverityScarOrientationSpec(
        "LowCrosswise",
        "LowCrosswise",
        "cuts low across {0}",
        "marks a low wound-track across {0}",
        "spreads low across {0} in a glossy patch",
        "clusters low across {0}",
        "draws the lower curve of {0} into a tight, uneven fold"),
    new HighSeverityScarOrientationSpec(
        "Diagonal",
        "Diagonal",
        "slashes diagonally over {0}",
        "marks a diagonal wound-track over {0}",
        "spreads diagonally over {0}",
        "clusters diagonally over {0}",
        "twists {0} into a diagonal, tightened distortion")
};

    private static readonly IReadOnlyList<ScarSeveritySpec> HumanScarSeveritySpecs = new[]
    {
    new ScarSeveritySpec("VerySevere", MudSharp.Health.WoundSeverity.VerySevere, "heavy", "heavy", 0, 2, 0.018),
    new ScarSeveritySpec("Grievous", MudSharp.Health.WoundSeverity.Grievous, "deep", "deep", 0, 3, 0.014),
    new ScarSeveritySpec("Horrifying", MudSharp.Health.WoundSeverity.Horrifying, "catastrophic", "catastrophic", 1, 4, 0.011)
};

    private static readonly IReadOnlyList<HighSeverityScarOrientationSpec> Broad5Orientations = new[]
    {
    new HighSeverityScarOrientationSpec(
        "Lengthwise",
        "Lengthwise",
        "runs lengthwise along {0}",
        "marks a lengthwise wound-track along {0}",
        "stretches lengthwise along {0}",
        "lies in a lengthwise scatter along {0}",
        "warps the length of {0}"),
    new HighSeverityScarOrientationSpec(
        "Crosswise",
        "Crosswise",
        "cuts crosswise over {0}",
        "marks a crosswise wound-track over {0}",
        "spreads crosswise over {0}",
        "lies in a crosswise scatter over {0}",
        "buckles crosswise over {0}"),
    new HighSeverityScarOrientationSpec(
        "Diagonal",
        "Diagonal",
        "slashes diagonally across {0}",
        "marks a diagonal wound-track across {0}",
        "spreads diagonally across {0}",
        "lies in a diagonal scatter across {0}",
        "twists diagonally across {0}"),
    new HighSeverityScarOrientationSpec(
        "Curving",
        "Curving",
        "curves across {0}",
        "marks a curving wound-track over {0}",
        "fans in a curving spread over {0}",
        "lies in a curving scatter over {0}",
        "draws {0} into a curving distortion"),
    new HighSeverityScarOrientationSpec(
        "Radiating",
        "Radiating",
        "branches outward across {0}",
        "marks radiating wound-tracks across {0}",
        "fans outward across {0}",
        "lies in a radiating scatter across {0}",
        "radiates out from the centre of {0}")
};

    private static readonly IReadOnlyList<HighSeverityScarOrientationSpec> Long4Orientations = new[]
    {
    new HighSeverityScarOrientationSpec(
        "Lengthwise",
        "Lengthwise",
        "runs lengthwise along {0}",
        "marks a lengthwise wound-track along {0}",
        "stretches lengthwise along {0}",
        "lies in a lengthwise scatter along {0}",
        "warps the length of {0}"),
    new HighSeverityScarOrientationSpec(
        "Crosswise",
        "Crosswise",
        "cuts crosswise over {0}",
        "marks a crosswise wound-track over {0}",
        "spreads crosswise over {0}",
        "lies in a crosswise scatter over {0}",
        "buckles crosswise over {0}"),
    new HighSeverityScarOrientationSpec(
        "Diagonal",
        "Diagonal",
        "slashes diagonally across {0}",
        "marks a diagonal wound-track across {0}",
        "spreads diagonally across {0}",
        "lies in a diagonal scatter across {0}",
        "twists diagonally across {0}"),
    new HighSeverityScarOrientationSpec(
        "Wrapped",
        "Wrapped",
        "wraps partway around {0}",
        "marks a wrapping wound-track around {0}",
        "wraps partway around {0} in a tightened band",
        "lies in a wrapping scatter around {0}",
        "pulls part of {0} into a tightened wrap")
};

    private static readonly IReadOnlyList<HighSeverityScarOrientationSpec> Ring4Orientations = new[]
    {
    new HighSeverityScarOrientationSpec(
        "Encircling",
        "Encircling",
        "rings {0} in a tight band",
        "marks a near-circular wound-track around {0}",
        "rings {0} in a tightened band",
        "lies in a broken ring around {0}",
        "distorts {0} in a tightened ring"),
    new HighSeverityScarOrientationSpec(
        "Lengthwise",
        "Lengthwise",
        "runs lengthwise along {0}",
        "marks a lengthwise wound-track along {0}",
        "stretches lengthwise along {0}",
        "lies in a lengthwise scatter along {0}",
        "warps the length of {0}"),
    new HighSeverityScarOrientationSpec(
        "Oblique",
        "Oblique",
        "cuts obliquely across {0}",
        "marks an oblique wound-track across {0}",
        "spreads obliquely across {0}",
        "lies in an oblique scatter across {0}",
        "twists obliquely across {0}"),
    new HighSeverityScarOrientationSpec(
        "Crosswise",
        "Crosswise",
        "cuts crosswise over {0}",
        "marks a crosswise wound-track over {0}",
        "spreads crosswise over {0}",
        "lies in a crosswise scatter over {0}",
        "buckles crosswise over {0}")
};

    private static readonly IReadOnlyList<HighSeverityScarOrientationSpec> Round4Orientations = new[]
    {
    new HighSeverityScarOrientationSpec(
        "Central",
        "Central",
        "runs squarely over {0}",
        "marks a central wound-track over {0}",
        "sits squarely over {0}",
        "lies in a tight cluster over {0}",
        "draws the centre of {0} out of shape"),
    new HighSeverityScarOrientationSpec(
        "Diagonal",
        "Diagonal",
        "slashes diagonally across {0}",
        "marks a diagonal wound-track across {0}",
        "spreads diagonally across {0}",
        "lies in a diagonal scatter across {0}",
        "twists diagonally across {0}"),
    new HighSeverityScarOrientationSpec(
        "Curving",
        "Curving",
        "curves over {0}",
        "marks a curving wound-track over {0}",
        "fans in a curving spread over {0}",
        "lies in a curving scatter over {0}",
        "draws {0} into a curving distortion"),
    new HighSeverityScarOrientationSpec(
        "Offset",
        "Offset",
        "rides one side of {0}",
        "marks one side of {0} with a narrow wound-track",
        "sprawls over one side of {0}",
        "lies in an offset scatter over {0}",
        "pulls one side of {0} badly out of line")
};

    private static readonly IReadOnlyList<HighSeverityScarOrientationSpec> Small3Orientations = new[]
    {
    new HighSeverityScarOrientationSpec(
        "Lengthwise",
        "Lengthwise",
        "runs lengthwise along {0}",
        "marks a lengthwise wound-track along {0}",
        "stretches lengthwise along {0}",
        "lies in a lengthwise scatter along {0}",
        "warps the length of {0}"),
    new HighSeverityScarOrientationSpec(
        "Crosswise",
        "Crosswise",
        "cuts crosswise over {0}",
        "marks a crosswise wound-track over {0}",
        "spreads crosswise over {0}",
        "lies in a crosswise scatter over {0}",
        "buckles crosswise over {0}"),
    new HighSeverityScarOrientationSpec(
        "Diagonal",
        "Diagonal",
        "slashes diagonally across {0}",
        "marks a diagonal wound-track across {0}",
        "spreads diagonally across {0}",
        "lies in a diagonal scatter across {0}",
        "twists diagonally across {0}")
};

    private static readonly IReadOnlyList<HighSeverityScarShapeSpec> HumanScarShapeSpecs = new[]
{
    new HighSeverityScarShapeSpec("abdomen", "Abdomen", 0, "the broad wall of the abdomen from flank toward navel", "Broad5"),
    new HighSeverityScarShapeSpec("ankle", "Ankle", -1, "the narrow ring of the ankle above the heel", "Ring4"),
    new HighSeverityScarShapeSpec("belly", "Belly", 0, "the soft curve of the belly", "Broad5"),
    new HighSeverityScarShapeSpec("breast", "Breast", 0, "the rounded mound of the breast", "Broad5"),
    new HighSeverityScarShapeSpec("buttock", "Buttock", 0, "the rounded mass of the buttock", "Broad5"),
    new HighSeverityScarShapeSpec("calf", "Calf", 0, "the thick back curve of the calf", "Long4"),
    new HighSeverityScarShapeSpec("cheek", "Cheek", -1, "the rounded plane of the cheek beneath the eye", "Round4"),
    new HighSeverityScarShapeSpec("chin", "Chin", -1, "the projecting front of the chin along the jawline", "Small3"),
    new HighSeverityScarShapeSpec("ear", "Ear", -2, "the ear", "Ear3"),
    new HighSeverityScarShapeSpec("elbow", "Elbow", -1, "the point and bend of the elbow, bunching when the joint flexes", "Round4"),
    new HighSeverityScarShapeSpec("eye", "Eye", -2, "the exposed surface of the eye", "Eye3"),
    new HighSeverityScarShapeSpec("eye socket", "Eye Socket", -1, "the rimmed hollow of the eye socket", "Small3"),
    new HighSeverityScarShapeSpec("eyebrow", "Eyebrow", -2, "the narrow ridge of the eyebrow along the browbone", "Small3"),
    new HighSeverityScarShapeSpec("face", "Face", 0, "the central planes of the face across cheek, brow, and jaw", "Broad5"),
    new HighSeverityScarShapeSpec("finger", "Finger", -2, "the narrow length of the finger, wrapping slightly around it", "Small3"),
    new HighSeverityScarShapeSpec("foot", "Foot", 0, "the top and outer curve of the foot", "Long4"),
    new HighSeverityScarShapeSpec("forearm", "Forearm", 0, "the long outer plane of the forearm from elbow toward wrist", "Long4"),
    new HighSeverityScarShapeSpec("forehead", "Forehead", -1, "the broad curve of the forehead above the brow", "Broad5"),
    new HighSeverityScarShapeSpec("groin", "Groin", -1, "the crease and soft hollow of the groin", "Small3"),
    new HighSeverityScarShapeSpec("hand", "Hand", 0, "the back of the hand across knuckles and tendons", "Broad5"),
    new HighSeverityScarShapeSpec("head back", "Head Back", 0, "the rounded back of the head above the neck", "Round4"),
    new HighSeverityScarShapeSpec("heel", "Heel", -1, "the hard rounded back of the heel", "Small3"),
    new HighSeverityScarShapeSpec("hip", "Hip", -1, "the outward flare of the hip above the thigh", "Round4"),
    new HighSeverityScarShapeSpec("knee", "Knee", -1, "the front of the knee over the kneecap and its edges", "Round4"),
    new HighSeverityScarShapeSpec("knee back", "Knee Back", -1, "the crease at the back of the knee", "Round4"),
    new HighSeverityScarShapeSpec("lower back", "Lower Back", 0, "the broad span of the lower back above the hips", "Broad5"),
    new HighSeverityScarShapeSpec("mouth", "Mouth", -1, "the mouth", "Mouth3"),
    new HighSeverityScarShapeSpec("neck", "Neck", -1, "the narrow column of the neck from jaw to collar", "Ring4"),
    new HighSeverityScarShapeSpec("neck back", "Neck Back", -1, "the back line of the neck beneath the skull", "Ring4"),
    new HighSeverityScarShapeSpec("nipple", "Nipple", -2, "the nipple and areola", "Nipple3"),
    new HighSeverityScarShapeSpec("nose", "Nose", -2, "the nose", "Nose3"),
    new HighSeverityScarShapeSpec("penis", "Penis", -1, "the length of the penis along one side of the shaft", "Long4"),
    new HighSeverityScarShapeSpec("scalp", "Scalp", 0, "the curve of the scalp across the skull beneath the hair", "Broad5"),
    new HighSeverityScarShapeSpec("shin", "Shin", 0, "the hard front edge of the shinbone", "Long4"),
    new HighSeverityScarShapeSpec("shoulder", "Shoulder", 0, "the rounded cap of the shoulder over the joint", "Round4"),
    new HighSeverityScarShapeSpec("shoulder blade", "Shoulder Blade", 0, "the flat triangle of the shoulder blade", "Broad5"),
    new HighSeverityScarShapeSpec("temple", "Temple", -1, "the shallow side plane of the temple beside the eye", "Small3"),
    new HighSeverityScarShapeSpec("testicles", "Testicles", -1, "the scrotal sac", "Testicles3"),
    new HighSeverityScarShapeSpec("thigh", "Thigh", 0, "the broad front and outer mass of the thigh", "Broad5"),
    new HighSeverityScarShapeSpec("thigh back", "Thigh Back", 0, "the heavy back curve of the thigh beneath the buttock", "Broad5"),
    new HighSeverityScarShapeSpec("throat", "Throat", -1, "the front of the throat over the windpipe", "Ring4"),
    new HighSeverityScarShapeSpec("thumb", "Thumb", -2, "the thick short length of the thumb, wrapping around its outer side", "Small3"),
    new HighSeverityScarShapeSpec("toe", "Toe", -2, "the narrow length of the toe, wrapping slightly around it", "Small3"),
    new HighSeverityScarShapeSpec("tongue", "Tongue", -2, "the tongue", "Tongue4"),
    new HighSeverityScarShapeSpec("upper arm", "Upper Arm", 0, "the rounded outer mass of the upper arm", "Long4"),
    new HighSeverityScarShapeSpec("upper back", "Upper Back", 0, "the broad span of the upper back between shoulder and spine", "Broad5"),
    new HighSeverityScarShapeSpec("wrist", "Wrist", -1, "the narrow ring of the wrist above the hand", "Ring4")
};

    private static readonly IReadOnlyList<HighSeverityScarDamageSpec> HumanScarDamageSpecs = new[]
    {
    new HighSeverityScarDamageSpec(
        MudSharp.Health.DamageType.Slashing,
        "Slashing",
        "jagged slash scar",
        "jagged slash scar",
        "where a deep cut split the flesh and healed in an uneven seam",
        "Line",
        0,
        0,
        0.002),
    new HighSeverityScarDamageSpec(
        MudSharp.Health.DamageType.Chopping,
        "Chopping",
        "hacked chop scar",
        "hacked chop scar",
        "where a heavy chopping blow bit deep and healed with a stepped, hacked edge",
        "Line",
        0,
        1,
        0.003),
    new HighSeverityScarDamageSpec(
        MudSharp.Health.DamageType.Crushing,
        "Crushing",
        "puckered crush scar",
        "puckered crush scar",
        "where the tissue was pulped under impact and set into hard, warped ridges",
        "Warp",
        1,
        0,
        0.002),
    new HighSeverityScarDamageSpec(
        MudSharp.Health.DamageType.Piercing,
        "Piercing",
        "puncture scar",
        "puncture scar",
        "where a narrow puncture bored inward and healed as a sunken channel",
        "Track",
        -1,
        0,
        0.002),
    new HighSeverityScarDamageSpec(
        MudSharp.Health.DamageType.Ballistic,
        "Ballistic",
        "gunshot scar",
        "gunshot scar",
        "where a projectile punched through and left a puckered track of torn tissue",
        "Track",
        0,
        1,
        0.004),
    new HighSeverityScarDamageSpec(
        MudSharp.Health.DamageType.Burning,
        "Burning",
        "shiny burn scar",
        "shiny burn scar",
        "where intense heat cooked the surface and left it tight, glossy, and uneven",
        "Patch",
        1,
        1,
        0.004),
    new HighSeverityScarDamageSpec(
        MudSharp.Health.DamageType.Freezing,
        "Freezing",
        "frostbite scar",
        "frostbite scar",
        "where the tissue froze and died back before healing into pale, tightened scar",
        "Patch",
        0,
        0,
        0.002),
    new HighSeverityScarDamageSpec(
        MudSharp.Health.DamageType.Chemical,
        "Chemical",
        "caustic scar",
        "caustic scar",
        "where caustic damage ate into the surface and left it drawn and irregular",
        "Patch",
        1,
        1,
        0.003),
    new HighSeverityScarDamageSpec(
        MudSharp.Health.DamageType.Shockwave,
        "Shockwave",
        "blast scar",
        "blast scar",
        "where concussive force burst and bruised the tissue without a clean cut, leaving it warped",
        "Warp",
        1,
        0,
        0.001),
    new HighSeverityScarDamageSpec(
        MudSharp.Health.DamageType.Bite,
        "Bite",
        "bite scar",
        "bite scar",
        "where teeth tore in and left clustered puncture seams",
        "Cluster",
        0,
        1,
        0.003),
    new HighSeverityScarDamageSpec(
        MudSharp.Health.DamageType.Claw,
        "Claw",
        "claw-rake scar",
        "claw-rake scar",
        "where talons raked through in parallel lines that healed raised and uneven",
        "Line",
        0,
        1,
        0.003),
    new HighSeverityScarDamageSpec(
        MudSharp.Health.DamageType.Electrical,
        "Electrical",
        "branching electrical scar",
        "branching electrical scar",
        "where electrical trauma burned inward in branching, tightened seams",
        "Patch",
        0,
        1,
        0.003),
    new HighSeverityScarDamageSpec(
        MudSharp.Health.DamageType.Hypoxia,
        "Hypoxia",
        "pale hypoxic scar",
        "pale hypoxic scar",
        "where deprived tissue died shallowly and healed into a pale, shrunken patch",
        "Patch",
        0,
        0,
        0.001),
    new HighSeverityScarDamageSpec(
        MudSharp.Health.DamageType.Cellular,
        "Cellular",
        "sunken cellular-collapse scar",
        "sunken cellular-collapse scar",
        "where deep systemic tissue damage broke the skin down and healed in a sunken, unhealthy scar",
        "Patch",
        1,
        1,
        0.002),
    new HighSeverityScarDamageSpec(
        MudSharp.Health.DamageType.Sonic,
        "Sonic",
        "rupture scar",
        "rupture scar",
        "where violent vibration ruptured the tissue in a harsh, irregular band",
        "Line",
        0,
        0,
        0.001),
    new HighSeverityScarDamageSpec(
        MudSharp.Health.DamageType.Shearing,
        "Shearing",
        "split shear scar",
        "split shear scar",
        "where force tore layers of tissue past one another and healed in a split, dragged seam",
        "Line",
        0,
        1,
        0.002),
    new HighSeverityScarDamageSpec(
        MudSharp.Health.DamageType.BallisticArmourPiercing,
        "BallisticArmourPiercing",
        "armour-piercing gunshot scar",
        "armour-piercing gunshot scar",
        "where an armour-piercing shot drilled through in a hard, narrow wound-track",
        "Track",
        -1,
        1,
        0.005),
    new HighSeverityScarDamageSpec(
        MudSharp.Health.DamageType.Wrenching,
        "Wrenching",
        "twisted wrench scar",
        "twisted wrench scar",
        "where the flesh was twisted and torn under strain and healed badly out of line",
        "Warp",
        0,
        1,
        0.002),
    new HighSeverityScarDamageSpec(
        MudSharp.Health.DamageType.Shrapnel,
        "Shrapnel",
        "peppered shrapnel scar",
        "peppered shrapnel scar",
        "where fragments peppered and tore the flesh, leaving clustered pits and seams",
        "Cluster",
        1,
        1,
        0.004),
    new HighSeverityScarDamageSpec(
        MudSharp.Health.DamageType.Necrotic,
        "Necrotic",
        "sunken necrotic scar",
        "sunken necrotic scar",
        "where dead tissue sloughed away and healed as a harsh, sunken loss",
        "Patch",
        1,
        1,
        0.004),
    new HighSeverityScarDamageSpec(
        MudSharp.Health.DamageType.Falling,
        "Falling",
        "impact scar",
        "impact scar",
        "where a violent fall crushed and ripped the tissue into a broad, uneven knot of scar",
        "Warp",
        1,
        0,
        0.002),
    new HighSeverityScarDamageSpec(
        MudSharp.Health.DamageType.Eldritch,
        "Eldritch",
        "wrong-looking eldritch scar",
        "wrong-looking eldritch scar",
        "where an unnatural wound warped the flesh into a pattern that healed wrong",
        "Patch",
        1,
        1,
        0.005),
    new HighSeverityScarDamageSpec(
        MudSharp.Health.DamageType.Arcane,
        "Arcane",
        "arcane sear scar",
        "arcane sear scar",
        "where raw magical force seared the tissue into a strange, disciplined scar pattern",
        "Patch",
        1,
        1,
        0.005),
    new HighSeverityScarDamageSpec(
        MudSharp.Health.DamageType.ArmourPiercing,
        "ArmourPiercing",
        "armour-piercing puncture scar",
        "armour-piercing puncture scar",
        "where a narrow penetrating wound punched deep and healed as a hard channel",
        "Track",
        -1,
        1,
        0.004)
};

    private static readonly IReadOnlyList<SeederScarTemplateDefinition> HumanScarTemplates = BuildHumanScarTemplates();

    private static IReadOnlyList<SeederScarTemplateDefinition> BuildHumanScarTemplates()
    {
        var scars = new List<SeederScarTemplateDefinition>();

        foreach (var shape in HumanScarShapeSpecs)
        {
            var orientations = GetOrientationPool(shape.OrientationPool);

            foreach (var damage in HumanScarDamageSpecs)
            {
                foreach (var severity in HumanScarSeveritySpecs)
                {
                    foreach (var orientation in orientations)
                    {
                        var scar = BuildHumanDamageScar(shape, damage, severity, orientation);
                        if (scar is not null)
                        {
                            scars.Add(scar);
                        }
                    }
                }
            }
        }

        return scars;
    }

    private static SeederScarTemplateDefinition? BuildHumanDamageScar(
    HighSeverityScarShapeSpec shape,
    HighSeverityScarDamageSpec damage,
    ScarSeveritySpec severity,
    HighSeverityScarOrientationSpec orientation)
    {
        var remap = GetShapeDamageRemap(shape.Shape, damage.DamageType);
        if (remap?.Allow != true)
        {
            return null;
        }

        var effectiveMinimumSeverity = remap.MinimumSeverityFloor ?? severity.MinimumSeverity;
        if ((int)effectiveMinimumSeverity > (int)severity.MinimumSeverity)
        {
            return null;
        }

        if (!IsOrientationCompatible(shape, damage, orientation))
        {
            return null;
        }

        var sizeSteps = ClampSizeSteps(shape.SizeAdjustment + damage.SizeAdjustment + severity.SizeAdjustment + remap.SizeAdjustment);
        var distinctiveness = ClampDistinctiveness(severity.BaseDistinctiveness + damage.DistinctivenessAdjustment + remap.DistinctivenessAdjustment);
        var chance = ClampChance((severity.BaseChance + damage.ChanceAdjustment) * remap.ChanceMultiplier);

        var shortBase = remap.ShortBaseOverride ?? damage.ShortBase;
        var fullBase = remap.FullBaseOverride ?? damage.FullBase;
        var causePhrase = remap.CausePhraseOverride ?? damage.CausePhrase;
        var patternFamily = remap.PatternFamilyOverride ?? damage.PatternFamily;

        return new SeederScarTemplateDefinition(
            $"{severity.Label} {shape.CodeName} {damage.CodeName} {orientation.NameFragment} Scar",
            $"a {severity.ShortModifier} {shortBase}",
            $"A {severity.FullModifier} {fullBase} {GetOrientationPhrase(orientation, patternFamily, shape.Geometry)}; {causePhrase}.",
            SizeSteps: sizeSteps,
            Distinctiveness: distinctiveness,
            DamageHealingScarChance: chance,
            DamageTypes: new Dictionary<MudSharp.Health.DamageType, MudSharp.Health.WoundSeverity>
            {
                [damage.DamageType] = severity.MinimumSeverity
            },
            BodypartShapeNames: new[] { shape.Shape });
    }

    private static bool IsOrientationCompatible(
    HighSeverityScarShapeSpec shape,
    HighSeverityScarDamageSpec damage,
    HighSeverityScarOrientationSpec orientation)
    {
        return shape.OrientationPool switch
        {
            "Eye3" => IsEyeOrientationCompatible(damage.PatternFamily, orientation.CodeName),
            "Mouth3" => IsMouthOrientationCompatible(damage.PatternFamily, orientation.CodeName),
            "Nipple3" => IsNippleOrientationCompatible(damage.PatternFamily, orientation.CodeName),
            "Nose3" => IsNoseOrientationCompatible(damage.PatternFamily, orientation.CodeName),
            "Ear3" => IsEarOrientationCompatible(damage.PatternFamily, orientation.CodeName),
            "Tongue4" => IsTongueOrientationCompatible(damage.PatternFamily, orientation.CodeName),
            "Testicles3" => IsTesticlesOrientationCompatible(damage.PatternFamily, orientation.CodeName),
            _ => damage.PatternFamily switch
            {
                "Track" => IsTrackOrientationCompatible(orientation.CodeName),
                "Line" => IsLineOrientationCompatible(orientation.CodeName),
                "Cluster" => IsClusterOrientationCompatible(orientation.CodeName),
                "Warp" => IsWarpOrientationCompatible(orientation.CodeName),
                "Patch" => IsPatchOrientationCompatible(orientation.CodeName),
                _ => true
            }
        };
    }

    private static bool IsEyeOrientationCompatible(string patternFamily, string codeName)
    {
        return patternFamily switch
        {
            "Track" => codeName is "Crescent" or "Offset",
            "Line" => codeName is "Crescent" or "Offset",
            "Cluster" => codeName is "Crescent" or "Offset" or "Radial",
            "Warp" => codeName is "Crescent" or "Offset" or "Radial",
            "Patch" => true,
            _ => true
        };
    }

    private static bool IsMouthOrientationCompatible(string patternFamily, string codeName)
    {
        return patternFamily switch
        {
            "Track" => codeName is "Lipline" or "Vertical" or "Cornerwise",
            "Line" => codeName is "Lipline" or "Vertical" or "Cornerwise",
            "Cluster" => codeName is "Lipline" or "Vertical" or "Cornerwise",
            "Warp" => codeName is "Vertical" or "Cornerwise",
            "Patch" => true,
            _ => true
        };
    }

    private static bool IsNippleOrientationCompatible(string patternFamily, string codeName)
    {
        return patternFamily switch
        {
            "Track" => codeName is "Across" or "Offset",
            "Line" => codeName is "Across" or "Offset",
            "Cluster" => codeName is "Across" or "Offset",
            "Warp" => codeName is "Across" or "Offset",
            "Patch" => codeName is "Across" or "Offset" or "Encircling",
            _ => true
        };
    }

    private static bool IsNoseOrientationCompatible(string patternFamily, string codeName)
    {
        return patternFamily switch
        {
            "Track" => codeName is "Bridgewise" or "Sidewall" or "Crosswise",
            "Line" => codeName is "Bridgewise" or "Sidewall" or "Crosswise",
            "Cluster" => codeName is "Crosswise" or "Sidewall",
            "Warp" => codeName is "Bridgewise" or "Sidewall" or "Crosswise",
            "Patch" => true,
            _ => true
        };
    }

    private static bool IsEarOrientationCompatible(string patternFamily, string codeName)
    {
        return patternFamily switch
        {
            "Track" => codeName is "Rimwise" or "Crosswise" or "Lobeward",
            "Line" => codeName is "Rimwise" or "Crosswise" or "Lobeward",
            "Cluster" => codeName is "Crosswise" or "Lobeward",
            "Warp" => codeName is "Rimwise" or "Crosswise" or "Lobeward",
            "Patch" => true,
            _ => true
        };
    }

    private static bool IsTongueOrientationCompatible(string patternFamily, string codeName)
    {
        return patternFamily switch
        {
            "Track" => codeName is "DorsalLengthwise" or "Crosswise" or "Edgewise",
            "Line" => codeName is "DorsalLengthwise" or "Crosswise" or "Diagonal" or "Edgewise",
            "Cluster" => codeName is "Crosswise" or "Diagonal" or "Edgewise",
            "Warp" => codeName is "Crosswise" or "Diagonal" or "Edgewise",
            "Patch" => true,
            _ => true
        };
    }

    private static bool IsTesticlesOrientationCompatible(string patternFamily, string codeName)
    {
        return patternFamily switch
        {
            "Track" => codeName is "HighSide" or "Diagonal",
            "Line" => codeName is "HighSide" or "LowCrosswise" or "Diagonal",
            "Cluster" => codeName is "HighSide" or "LowCrosswise" or "Diagonal",
            "Warp" => codeName is "HighSide" or "LowCrosswise" or "Diagonal",
            "Patch" => true,
            _ => true
        };
    }

    private static bool IsTrackOrientationCompatible(string codeName)
    {
        return codeName switch
        {
            "Curving" => false,
            "Radiating" => false,
            "Wrapped" => false,
            "Encircling" => false,
            _ => true
        };
    }

    private static bool IsLineOrientationCompatible(string codeName)
    {
        return codeName switch
        {
            "Radiating" => false,
            "Wrapped" => false,
            "Encircling" => false,
            _ => true
        };
    }

    private static bool IsClusterOrientationCompatible(string codeName)
    {
        return codeName switch
        {
            "Radiating" => false,
            "Wrapped" => false,
            "Encircling" => false,
            _ => true
        };
    }

    private static bool IsWarpOrientationCompatible(string codeName)
    {
        return codeName switch
        {
            "Radiating" => false,
            "Wrapped" => false,
            "Encircling" => false,
            _ => true
        };
    }

    private static bool IsPatchOrientationCompatible(string codeName)
    {
        return true;
    }

    private static IReadOnlyList<HighSeverityScarOrientationSpec> GetOrientationPool(string poolName)
    {
        return poolName switch
        {
            "Broad5" => Broad5Orientations,
            "Long4" => Long4Orientations,
            "Ring4" => Ring4Orientations,
            "Round4" => Round4Orientations,
            "Eye3" => Eye3Orientations,
            "Mouth3" => Mouth3Orientations,
            "Nipple3" => Nipple3Orientations,
            "Nose3" => Nose3Orientations,
            "Ear3" => Ear3Orientations,
            "Tongue4" => Tongue4Orientations,
            "Testicles3" => Testicles3Orientations,
            _ => Small3Orientations
        };
    }

    private static string GetOrientationPhrase(
        HighSeverityScarOrientationSpec orientation,
        string patternFamily,
        string geometry)
    {
        var template = patternFamily switch
        {
            "Track" => orientation.TrackPhrase,
            "Patch" => orientation.PatchPhrase,
            "Cluster" => orientation.ClusterPhrase,
            "Warp" => orientation.WarpPhrase,
            _ => orientation.LinePhrase
        };

        return string.Format(template, geometry);
    }

    private static int ClampSizeSteps(int value)
    {
        return System.Math.Max(-3, System.Math.Min(0, value));
    }

    private static int ClampDistinctiveness(int value)
    {
        return System.Math.Max(1, System.Math.Min(4, value));
    }

    private static double ClampChance(double value)
    {
        return System.Math.Max(0.01, System.Math.Min(0.08, value));
    }

    private static readonly ShapeDamageRemap DefaultShapeDamageRemap = new(true);

    private static ShapeDamageRemap GetShapeDamageRemap(string shapeName, MudSharp.Health.DamageType damageType)
    {
        return shapeName switch
        {
            "eye" => GetEyeDamageRemap(damageType),
            "mouth" => GetMouthDamageRemap(damageType),
            "tongue" => GetTongueDamageRemap(damageType),
            "nipple" => GetNippleDamageRemap(damageType),
            "nose" => GetNoseDamageRemap(damageType),
            "ear" => GetEarDamageRemap(damageType),
            "testicles" => GetTesticlesDamageRemap(damageType),
            "penis" => GetPenisDamageRemap(damageType),
            _ => DefaultShapeDamageRemap
        };
    }

    private static ShapeDamageRemap GetEyeDamageRemap(MudSharp.Health.DamageType damageType)
    {
        return damageType switch
        {
            MudSharp.Health.DamageType.Burning => Prefer(
                2.2,
                MudSharp.Health.WoundSeverity.VerySevere,
                "Patch",
                shortBaseOverride: "blinded burn scar",
                fullBaseOverride: "blinded burn scar",
                causePhraseOverride: "where intense heat seared the eye and left it milky, glossy, and permanently scarred"),
            MudSharp.Health.DamageType.Chemical => Prefer(
                2.25,
                MudSharp.Health.WoundSeverity.VerySevere,
                "Patch",
                shortBaseOverride: "caustic eye scar",
                fullBaseOverride: "caustic eye scar",
                causePhraseOverride: "where caustic damage burned the eye and left it clouded and tightened"),
            MudSharp.Health.DamageType.Electrical => Prefer(
                1.8,
                MudSharp.Health.WoundSeverity.VerySevere,
                "Patch",
                shortBaseOverride: "electrical eye scar",
                fullBaseOverride: "electrical eye scar",
                causePhraseOverride: "where electrical trauma scorched the eye into a pale, cloudy scar"),
            MudSharp.Health.DamageType.Piercing => Prefer(
                2.0,
                MudSharp.Health.WoundSeverity.VerySevere,
                "Track",
                sizeAdjustment: -1,
                shortBaseOverride: "punctured eye scar",
                fullBaseOverride: "punctured eye scar",
                causePhraseOverride: "where a deep puncture drove into the eye and healed as a cloudy wound-track"),
            MudSharp.Health.DamageType.Ballistic => Prefer(
                2.25,
                MudSharp.Health.WoundSeverity.VerySevere,
                "Track",
                sizeAdjustment: -1,
                shortBaseOverride: "shot-through eye scar",
                fullBaseOverride: "shot-through eye scar",
                causePhraseOverride: "where a projectile tore through the eye and left a dense, ruined track of scar"),
            MudSharp.Health.DamageType.BallisticArmourPiercing => Prefer(
                2.35,
                MudSharp.Health.WoundSeverity.VerySevere,
                "Track",
                sizeAdjustment: -1,
                shortBaseOverride: "armour-piercing eye scar",
                fullBaseOverride: "armour-piercing eye scar",
                causePhraseOverride: "where an armour-piercing shot drilled through the eye and left a hard, catastrophic scar track"),
            MudSharp.Health.DamageType.ArmourPiercing => Prefer(
                2.0,
                MudSharp.Health.WoundSeverity.VerySevere,
                "Track",
                sizeAdjustment: -1,
                shortBaseOverride: "penetrating eye scar",
                fullBaseOverride: "penetrating eye scar",
                causePhraseOverride: "where a narrow penetrating wound drove deep into the eye and healed as a hard cloudy channel"),
            MudSharp.Health.DamageType.Claw => Prefer(
                1.85,
                MudSharp.Health.WoundSeverity.VerySevere,
                "Line",
                shortBaseOverride: "raked eye scar",
                fullBaseOverride: "raked eye scar",
                causePhraseOverride: "where claws raked across the eye and left it clouded and ridged"),
            MudSharp.Health.DamageType.Slashing => Prefer(
                1.75,
                MudSharp.Health.WoundSeverity.VerySevere,
                "Line",
                shortBaseOverride: "slashed eye scar",
                fullBaseOverride: "slashed eye scar",
                causePhraseOverride: "where a deep cut scored the eye and left a pale, cloudy seam"),
            MudSharp.Health.DamageType.Shrapnel => Prefer(
                2.0,
                MudSharp.Health.WoundSeverity.VerySevere,
                "Cluster",
                shortBaseOverride: "fragmented eye scar",
                fullBaseOverride: "fragmented eye scar",
                causePhraseOverride: "where fragments peppered the eye and left it pitted, clouded, and uneven"),
            MudSharp.Health.DamageType.Arcane => Prefer(
                1.8,
                MudSharp.Health.WoundSeverity.VerySevere,
                "Patch",
                shortBaseOverride: "arcane eye scar",
                fullBaseOverride: "arcane eye scar",
                causePhraseOverride: "where raw arcane force scarred the eye into a strange, clouded opacity"),
            MudSharp.Health.DamageType.Eldritch => Prefer(
                1.9,
                MudSharp.Health.WoundSeverity.VerySevere,
                "Patch",
                shortBaseOverride: "wrong-looking eye scar",
                fullBaseOverride: "wrong-looking eye scar",
                causePhraseOverride: "where an unnatural wound scarred the eye into a cloudy pattern that healed wrong"),
            MudSharp.Health.DamageType.Crushing => Discourage(
                0.45,
                MudSharp.Health.WoundSeverity.Grievous,
                "Warp",
                distinctivenessAdjustment: 1,
                shortBaseOverride: "ruptured eye scar",
                fullBaseOverride: "ruptured eye scar",
                causePhraseOverride: "where blunt force ruptured the eye and left it collapsed and cloudy"),
            MudSharp.Health.DamageType.Shockwave => Discourage(
                0.35,
                MudSharp.Health.WoundSeverity.Horrifying,
                "Patch",
                shortBaseOverride: "blast-blinded eye scar",
                fullBaseOverride: "blast-blinded eye scar",
                causePhraseOverride: "where concussive force ruined the eye and left it scarred opaque"),
            MudSharp.Health.DamageType.Falling => Discourage(
                0.30,
                MudSharp.Health.WoundSeverity.Horrifying,
                "Patch",
                shortBaseOverride: "impact-ruined eye scar",
                fullBaseOverride: "impact-ruined eye scar",
                causePhraseOverride: "where catastrophic impact damage left the eye scarred and opaque"),
            MudSharp.Health.DamageType.Sonic => Discourage(
                0.35,
                MudSharp.Health.WoundSeverity.Horrifying,
                "Patch",
                shortBaseOverride: "ruptured eye scar",
                fullBaseOverride: "ruptured eye scar",
                causePhraseOverride: "where violent vibration ruined the eye and left it scarred cloudy"),
            _ => DefaultShapeDamageRemap
        };
    }

    private static ShapeDamageRemap GetMouthDamageRemap(MudSharp.Health.DamageType damageType)
    {
        return damageType switch
        {
            MudSharp.Health.DamageType.Bite => Prefer(
                2.0,
                MudSharp.Health.WoundSeverity.VerySevere,
                "Cluster",
                shortBaseOverride: "torn mouth scar",
                fullBaseOverride: "torn mouth scar",
                causePhraseOverride: "where teeth tore into the lips and mouth and left clustered seams of scar"),
            MudSharp.Health.DamageType.Burning => Prefer(
                1.9,
                MudSharp.Health.WoundSeverity.VerySevere,
                "Patch",
                shortBaseOverride: "burned mouth scar",
                fullBaseOverride: "burned mouth scar",
                causePhraseOverride: "where severe heat burned the lips and mouth into a tight, glossy scar"),
            MudSharp.Health.DamageType.Chemical => Prefer(
                2.0,
                MudSharp.Health.WoundSeverity.VerySevere,
                "Patch",
                shortBaseOverride: "caustic mouth scar",
                fullBaseOverride: "caustic mouth scar",
                causePhraseOverride: "where caustic damage ate into the lips and mouth and left them drawn and uneven"),
            MudSharp.Health.DamageType.Slashing => Prefer(
                1.8,
                MudSharp.Health.WoundSeverity.VerySevere,
                "Line",
                shortBaseOverride: "split-lip scar",
                fullBaseOverride: "split-lip scar",
                causePhraseOverride: "where a deep cut split the lips or mouth and healed into a drawn seam"),
            MudSharp.Health.DamageType.Claw => Prefer(
                1.7,
                MudSharp.Health.WoundSeverity.VerySevere,
                "Line",
                shortBaseOverride: "raked mouth scar",
                fullBaseOverride: "raked mouth scar",
                causePhraseOverride: "where claws tore across the lips and mouth and healed unevenly"),
            MudSharp.Health.DamageType.Piercing => Prefer(
                1.45,
                MudSharp.Health.WoundSeverity.VerySevere,
                "Track",
                sizeAdjustment: -1),
            MudSharp.Health.DamageType.Chopping => Prefer(
                1.6,
                MudSharp.Health.WoundSeverity.VerySevere,
                "Line"),
            MudSharp.Health.DamageType.Crushing => Discourage(
                0.55,
                MudSharp.Health.WoundSeverity.Grievous,
                "Warp",
                shortBaseOverride: "crushed mouth scar",
                fullBaseOverride: "crushed mouth scar",
                causePhraseOverride: "where the lips and mouth were crushed and healed badly out of line"),
            MudSharp.Health.DamageType.Shockwave => Discourage(
                0.45,
                MudSharp.Health.WoundSeverity.Grievous,
                "Warp"),
            MudSharp.Health.DamageType.Falling => Discourage(
                0.55,
                MudSharp.Health.WoundSeverity.Grievous,
                "Warp"),
            _ => DefaultShapeDamageRemap
        };
    }

    private static ShapeDamageRemap GetTongueDamageRemap(MudSharp.Health.DamageType damageType)
    {
        return damageType switch
        {
            MudSharp.Health.DamageType.Bite => Prefer(
                2.3,
                MudSharp.Health.WoundSeverity.VerySevere,
                "Line",
                shortBaseOverride: "bitten tongue scar",
                fullBaseOverride: "bitten tongue scar",
                causePhraseOverride: "where the tongue was badly bitten through and healed in a ridged, uneven seam"),
            MudSharp.Health.DamageType.Slashing => Prefer(
                1.9,
                MudSharp.Health.WoundSeverity.VerySevere,
                "Line",
                shortBaseOverride: "split tongue scar",
                fullBaseOverride: "split tongue scar",
                causePhraseOverride: "where a deep cut split the tongue and left a stiffened seam of scar"),
            MudSharp.Health.DamageType.Chopping => Prefer(
                1.8,
                MudSharp.Health.WoundSeverity.VerySevere,
                "Line",
                shortBaseOverride: "hacked tongue scar",
                fullBaseOverride: "hacked tongue scar",
                causePhraseOverride: "where a chopping wound bit into the tongue and healed raggedly"),
            MudSharp.Health.DamageType.Burning => Prefer(
                1.9,
                MudSharp.Health.WoundSeverity.VerySevere,
                "Patch",
                shortBaseOverride: "burned tongue scar",
                fullBaseOverride: "burned tongue scar",
                causePhraseOverride: "where severe burning left the tongue tight, glossy, and uneven"),
            MudSharp.Health.DamageType.Chemical => Prefer(
                2.0,
                MudSharp.Health.WoundSeverity.VerySevere,
                "Patch",
                shortBaseOverride: "caustic tongue scar",
                fullBaseOverride: "caustic tongue scar",
                causePhraseOverride: "where caustic injury ate into the tongue and left it ridged and drawn"),
            MudSharp.Health.DamageType.Electrical => Prefer(
                1.8,
                MudSharp.Health.WoundSeverity.VerySevere,
                "Patch",
                shortBaseOverride: "electrical tongue scar",
                fullBaseOverride: "electrical tongue scar",
                causePhraseOverride: "where electrical trauma scorched the tongue into a pale, tightened scar"),
            MudSharp.Health.DamageType.Piercing => Prefer(
                1.55,
                MudSharp.Health.WoundSeverity.VerySevere,
                "Track",
                sizeAdjustment: -1),
            MudSharp.Health.DamageType.Crushing => Discourage(
                0.45,
                MudSharp.Health.WoundSeverity.Grievous,
                "Warp",
                shortBaseOverride: "crushed tongue scar",
                fullBaseOverride: "crushed tongue scar",
                causePhraseOverride: "where the tongue was pulped under force and healed thickened and stiff"),
            MudSharp.Health.DamageType.Shockwave => Discourage(
                0.35,
                MudSharp.Health.WoundSeverity.Horrifying,
                "Warp"),
            MudSharp.Health.DamageType.Falling => Discourage(
                0.40,
                MudSharp.Health.WoundSeverity.Horrifying,
                "Warp"),
            MudSharp.Health.DamageType.Ballistic => Discourage(
                0.70,
                MudSharp.Health.WoundSeverity.Grievous,
                "Track"),
            MudSharp.Health.DamageType.BallisticArmourPiercing => Discourage(
                0.65,
                MudSharp.Health.WoundSeverity.Grievous,
                "Track"),
            MudSharp.Health.DamageType.ArmourPiercing => Discourage(
                0.70,
                MudSharp.Health.WoundSeverity.Grievous,
                "Track"),
            _ => DefaultShapeDamageRemap
        };
    }

    private static ShapeDamageRemap GetNippleDamageRemap(MudSharp.Health.DamageType damageType)
    {
        return damageType switch
        {
            MudSharp.Health.DamageType.Bite => Prefer(
                1.9,
                MudSharp.Health.WoundSeverity.VerySevere,
                "Cluster",
                shortBaseOverride: "torn nipple scar",
                fullBaseOverride: "torn nipple scar",
                causePhraseOverride: "where teeth tore into the nipple and left clustered seams of scar"),
            MudSharp.Health.DamageType.Claw => Prefer(
                1.7,
                MudSharp.Health.WoundSeverity.VerySevere,
                "Line"),
            MudSharp.Health.DamageType.Burning => Prefer(
                1.85,
                MudSharp.Health.WoundSeverity.VerySevere,
                "Patch"),
            MudSharp.Health.DamageType.Chemical => Prefer(
                1.9,
                MudSharp.Health.WoundSeverity.VerySevere,
                "Patch"),
            MudSharp.Health.DamageType.Piercing => Prefer(
                1.7,
                MudSharp.Health.WoundSeverity.VerySevere,
                "Track",
                sizeAdjustment: -1),
            MudSharp.Health.DamageType.Slashing => Prefer(
                1.6,
                MudSharp.Health.WoundSeverity.VerySevere,
                "Line"),
            MudSharp.Health.DamageType.Crushing => Discourage(
                0.60,
                MudSharp.Health.WoundSeverity.Grievous,
                "Warp"),
            MudSharp.Health.DamageType.Shockwave => Discourage(
                0.45,
                MudSharp.Health.WoundSeverity.Horrifying,
                "Warp"),
            MudSharp.Health.DamageType.Falling => Discourage(
                0.50,
                MudSharp.Health.WoundSeverity.Horrifying,
                "Warp"),
            _ => DefaultShapeDamageRemap
        };
    }

    private static ShapeDamageRemap GetNoseDamageRemap(MudSharp.Health.DamageType damageType)
    {
        return damageType switch
        {
            MudSharp.Health.DamageType.Slashing => Prefer(
                1.7,
                MudSharp.Health.WoundSeverity.VerySevere,
                "Line",
                shortBaseOverride: "slashed nose scar",
                fullBaseOverride: "slashed nose scar",
                causePhraseOverride: "where a deep cut split the nose and healed in a tight, uneven seam"),
            MudSharp.Health.DamageType.Chopping => Prefer(
                1.75,
                MudSharp.Health.WoundSeverity.VerySevere,
                "Line",
                shortBaseOverride: "hacked nose scar",
                fullBaseOverride: "hacked nose scar",
                causePhraseOverride: "where a chopping wound bit hard into the nose and healed raggedly"),
            MudSharp.Health.DamageType.Crushing => Prefer(
                1.8,
                MudSharp.Health.WoundSeverity.VerySevere,
                "Warp",
                shortBaseOverride: "broken nose scar",
                fullBaseOverride: "broken nose scar",
                causePhraseOverride: "where blunt force broke and crushed the nose before it healed out of line"),
            MudSharp.Health.DamageType.Ballistic => Prefer(
                1.6,
                MudSharp.Health.WoundSeverity.VerySevere,
                "Track"),
            MudSharp.Health.DamageType.Shrapnel => Prefer(
                1.65,
                MudSharp.Health.WoundSeverity.VerySevere,
                "Cluster"),
            MudSharp.Health.DamageType.Burning => Prefer(
                1.75,
                MudSharp.Health.WoundSeverity.VerySevere,
                "Patch"),
            MudSharp.Health.DamageType.Chemical => Prefer(
                1.8,
                MudSharp.Health.WoundSeverity.VerySevere,
                "Patch"),
            MudSharp.Health.DamageType.Claw => Prefer(
                1.6,
                MudSharp.Health.WoundSeverity.VerySevere,
                "Line"),
            MudSharp.Health.DamageType.Wrenching => Prefer(
                1.4,
                MudSharp.Health.WoundSeverity.VerySevere,
                "Warp"),
            _ => DefaultShapeDamageRemap
        };
    }

    private static ShapeDamageRemap GetEarDamageRemap(MudSharp.Health.DamageType damageType)
    {
        return damageType switch
        {
            MudSharp.Health.DamageType.Slashing => Prefer(1.8, MudSharp.Health.WoundSeverity.VerySevere, "Line"),
            MudSharp.Health.DamageType.Chopping => Prefer(1.9, MudSharp.Health.WoundSeverity.VerySevere, "Line"),
            MudSharp.Health.DamageType.Claw => Prefer(1.7, MudSharp.Health.WoundSeverity.VerySevere, "Line"),
            MudSharp.Health.DamageType.Bite => Prefer(1.7, MudSharp.Health.WoundSeverity.VerySevere, "Cluster"),
            MudSharp.Health.DamageType.Burning => Prefer(1.65, MudSharp.Health.WoundSeverity.VerySevere, "Patch"),
            MudSharp.Health.DamageType.Chemical => Prefer(1.7, MudSharp.Health.WoundSeverity.VerySevere, "Patch"),
            MudSharp.Health.DamageType.Freezing => Prefer(1.8, MudSharp.Health.WoundSeverity.VerySevere, "Patch"),
            MudSharp.Health.DamageType.Wrenching => Prefer(
                1.95,
                MudSharp.Health.WoundSeverity.VerySevere,
                "Warp",
                shortBaseOverride: "torn ear scar",
                fullBaseOverride: "torn ear scar",
                causePhraseOverride: "where the ear was wrenched and partly torn, leaving it warped and folded"),
            MudSharp.Health.DamageType.Shrapnel => Prefer(1.6, MudSharp.Health.WoundSeverity.VerySevere, "Cluster"),
            MudSharp.Health.DamageType.Ballistic => Prefer(1.5, MudSharp.Health.WoundSeverity.VerySevere, "Track"),
            MudSharp.Health.DamageType.Crushing => Discourage(0.60, MudSharp.Health.WoundSeverity.Grievous, "Warp"),
            MudSharp.Health.DamageType.Falling => Discourage(0.55, MudSharp.Health.WoundSeverity.Grievous, "Warp"),
            _ => DefaultShapeDamageRemap
        };
    }

    private static ShapeDamageRemap GetTesticlesDamageRemap(MudSharp.Health.DamageType damageType)
    {
        return damageType switch
        {
            MudSharp.Health.DamageType.Crushing => Prefer(
                2.0,
                MudSharp.Health.WoundSeverity.VerySevere,
                "Warp",
                shortBaseOverride: "crushed testicular scar",
                fullBaseOverride: "crushed testicular scar",
                causePhraseOverride: "where severe crushing trauma pulped the tissue and left it puckered and distorted"),
            MudSharp.Health.DamageType.Wrenching => Prefer(
                1.9,
                MudSharp.Health.WoundSeverity.VerySevere,
                "Warp",
                shortBaseOverride: "twisted testicular scar",
                fullBaseOverride: "twisted testicular scar",
                causePhraseOverride: "where the tissue was violently twisted and healed badly out of line"),
            MudSharp.Health.DamageType.Bite => Prefer(1.8, MudSharp.Health.WoundSeverity.VerySevere, "Cluster"),
            MudSharp.Health.DamageType.Burning => Prefer(1.7, MudSharp.Health.WoundSeverity.VerySevere, "Patch"),
            MudSharp.Health.DamageType.Chemical => Prefer(1.75, MudSharp.Health.WoundSeverity.VerySevere, "Patch"),
            MudSharp.Health.DamageType.Necrotic => Prefer(1.7, MudSharp.Health.WoundSeverity.VerySevere, "Patch"),
            MudSharp.Health.DamageType.Piercing => Prefer(1.6, MudSharp.Health.WoundSeverity.VerySevere, "Track", sizeAdjustment: -1),
            MudSharp.Health.DamageType.Slashing => Prefer(1.6, MudSharp.Health.WoundSeverity.VerySevere, "Line"),
            MudSharp.Health.DamageType.Falling => Prefer(1.35, MudSharp.Health.WoundSeverity.Grievous, "Warp"),
            MudSharp.Health.DamageType.Shockwave => Prefer(1.2, MudSharp.Health.WoundSeverity.Grievous, "Warp"),
            _ => DefaultShapeDamageRemap
        };
    }

    private static ShapeDamageRemap GetPenisDamageRemap(MudSharp.Health.DamageType damageType)
    {
        return damageType switch
        {
            MudSharp.Health.DamageType.Slashing => Prefer(1.8, MudSharp.Health.WoundSeverity.VerySevere, "Line"),
            MudSharp.Health.DamageType.Piercing => Prefer(1.75, MudSharp.Health.WoundSeverity.VerySevere, "Track", sizeAdjustment: -1),
            MudSharp.Health.DamageType.Bite => Prefer(1.75, MudSharp.Health.WoundSeverity.VerySevere, "Cluster"),
            MudSharp.Health.DamageType.Burning => Prefer(1.8, MudSharp.Health.WoundSeverity.VerySevere, "Patch"),
            MudSharp.Health.DamageType.Chemical => Prefer(1.85, MudSharp.Health.WoundSeverity.VerySevere, "Patch"),
            MudSharp.Health.DamageType.Electrical => Prefer(1.6, MudSharp.Health.WoundSeverity.VerySevere, "Patch"),
            MudSharp.Health.DamageType.Wrenching => Prefer(1.5, MudSharp.Health.WoundSeverity.VerySevere, "Warp"),
            MudSharp.Health.DamageType.Crushing => Prefer(1.4, MudSharp.Health.WoundSeverity.VerySevere, "Warp"),
            MudSharp.Health.DamageType.Falling => Discourage(0.55, MudSharp.Health.WoundSeverity.Grievous, "Warp"),
            MudSharp.Health.DamageType.Shockwave => Discourage(0.50, MudSharp.Health.WoundSeverity.Grievous, "Warp"),
            _ => DefaultShapeDamageRemap
        };
    }

    private static ShapeDamageRemap Prefer(
        double chanceMultiplier,
        MudSharp.Health.WoundSeverity? minimumSeverityFloor = null,
        string? patternFamilyOverride = null,
        int distinctivenessAdjustment = 1,
        int sizeAdjustment = 0,
        string? shortBaseOverride = null,
        string? fullBaseOverride = null,
        string? causePhraseOverride = null)
    {
        return new ShapeDamageRemap(
            true,
            chanceMultiplier,
            distinctivenessAdjustment,
            sizeAdjustment,
            minimumSeverityFloor,
            patternFamilyOverride,
            shortBaseOverride,
            fullBaseOverride,
            causePhraseOverride);
    }

    private static ShapeDamageRemap Discourage(
        double chanceMultiplier,
        MudSharp.Health.WoundSeverity? minimumSeverityFloor = null,
        string? patternFamilyOverride = null,
        int distinctivenessAdjustment = 0,
        int sizeAdjustment = 0,
        string? shortBaseOverride = null,
        string? fullBaseOverride = null,
        string? causePhraseOverride = null)
    {
        return new ShapeDamageRemap(
            true,
            chanceMultiplier,
            distinctivenessAdjustment,
            sizeAdjustment,
            minimumSeverityFloor,
            patternFamilyOverride,
            shortBaseOverride,
            fullBaseOverride,
            causePhraseOverride);
    }

    private static MudSharp.Health.WoundSeverity MaxSeverity(
        MudSharp.Health.WoundSeverity a,
        MudSharp.Health.WoundSeverity b)
    {
        return (MudSharp.Health.WoundSeverity)System.Math.Max((int)a, (int)b);
    }
}

public abstract record SeederDisfigurementTemplateDefinition(
    string Name,
    string ShortDescription,
    string FullDescription,
    IReadOnlyList<string>? BodypartShapeNames = null,
    IReadOnlyList<string>? BodypartAliases = null,
    bool CanSelectInChargen = false,
    string? CanSelectInChargenProgName = null,
    IReadOnlyDictionary<string, int>? ChargenCosts = null,
    string? OverrideCharacteristicPlain = null,
    string? OverrideCharacteristicWith = null
);

public sealed record SeederTattooTextSlotDefinition(
    string Name,
    int MaximumLength,
    string? DefaultLanguageName = null,
    string? DefaultScriptName = null,
    string DefaultText = "",
    bool RequiredCustomText = false,
    WritingStyleDescriptors DefaultStyle = WritingStyleDescriptors.None,
    string? DefaultColourName = null,
    double DefaultMinimumSkill = 0.0,
    string DefaultAlternateText = ""
);

public sealed record SeederTattooTemplateDefinition(
    string Name,
    string ShortDescription,
    string FullDescription,
    SizeCategory MinimumBodypartSize = SizeCategory.Nanoscopic,
    string? RequiredKnowledgeName = null,
    double MinimumSkill = 0.0,
    IReadOnlyDictionary<string, double>? InkColours = null,
    IReadOnlyList<string>? BodypartShapeNames = null,
    IReadOnlyList<string>? BodypartAliases = null,
    bool CanSelectInChargen = false,
    string? CanSelectInChargenProgName = null,
    IReadOnlyDictionary<string, int>? ChargenCosts = null,
    string? OverrideCharacteristicPlain = null,
    string? OverrideCharacteristicWith = null,
    IReadOnlyList<SeederTattooTextSlotDefinition>? TextSlots = null
) : SeederDisfigurementTemplateDefinition(
    Name,
    ShortDescription,
    FullDescription,
    BodypartShapeNames,
    BodypartAliases,
    CanSelectInChargen,
    CanSelectInChargenProgName,
    ChargenCosts,
    OverrideCharacteristicPlain,
    OverrideCharacteristicWith
);

public sealed record SeederScarTemplateDefinition(
    string Name,
    string ShortDescription,
    string FullDescription,
    int SizeSteps = 0,
    int Distinctiveness = 1,
    bool Unique = false,
    double DamageHealingScarChance = 0.0,
    double SurgeryHealingScarChance = 0.0,
    IReadOnlyDictionary<DamageType, WoundSeverity>? DamageTypes = null,
    IReadOnlyList<SurgicalProcedureType>? SurgeryTypes = null,
    IReadOnlyList<string>? BodypartShapeNames = null,
    IReadOnlyList<string>? BodypartAliases = null,
    bool CanSelectInChargen = false,
    string? CanSelectInChargenProgName = null,
    IReadOnlyDictionary<string, int>? ChargenCosts = null,
    string? OverrideCharacteristicPlain = null,
    string? OverrideCharacteristicWith = null
) : SeederDisfigurementTemplateDefinition(
    Name,
    ShortDescription,
    FullDescription,
    BodypartShapeNames,
    BodypartAliases,
    CanSelectInChargen,
    CanSelectInChargenProgName,
    ChargenCosts,
    OverrideCharacteristicPlain,
    OverrideCharacteristicWith
);