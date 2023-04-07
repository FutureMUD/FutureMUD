using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Framework
{
    [Flags]
    public enum PerceiveIgnoreFlags
    {
        None = 0,
        IgnoreSelf = 1,             //Describe object in 3rd person even if it is yourself
        IgnoreCorpse = 2,           //Return the description of the body, not the corpse
        IgnoreObscured = 4,         //Ignore that something is obscurred from view when determining visibility
        IgnoreConsciousness = 8,            //Ignore that the viewer is sleeping or unconscious
        IgnoreDark = 16,            //Ignore that the item is obscurred by darkness
        IgnoreSpotting = 32,        //Do not perform spotting checks vs hidden/sneaking if they passed their HIDE CHECK
        IgnoreHiding = 64,          // Do not include the hiding state in descriptions
        IgnoreCanSee = 128,
        IgnoreLayers = 256,         // Do not consider room layers in whether people can see, or descriptions
        IgnoreDisguises = 512,
        IgnoreNamesSetting = 1024,
        IgnoreLoadThings = 2048, // Being called at a time when you do not want to lazy load anything,
        IgnoreLiquidsAndFlags = 4096
    }
}