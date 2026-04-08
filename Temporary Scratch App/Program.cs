using MudSharp.Communication.Language;
using MudSharp.GameItems;
using MudSharp.Health;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Temporary_Work_App;

internal class Program
{
    public static int Main(string[] args)
    {
        // Feel free to comment out existing workloads and add your own scratch work
        return ScarSeederWork.DoScarSeederWork(args);
    }
}