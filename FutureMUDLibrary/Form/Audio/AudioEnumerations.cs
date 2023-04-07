using System;

namespace MudSharp.Form.Audio {
    public static class AudioExtensions {
        public static string Describe(this AudioVolume volume) {
            switch (volume) {
                case AudioVolume.Silent:
                    return "Silent";
                case AudioVolume.Faint:
                    return "Faint";
                case AudioVolume.Quiet:
                    return "Quiet";
                case AudioVolume.Decent:
                    return "Decent";
                case AudioVolume.Loud:
                    return "Loud";
                case AudioVolume.VeryLoud:
                    return "Very Loud";
                case AudioVolume.ExtremelyLoud:
                    return "Extremely Loud";
                case AudioVolume.DangerouslyLoud:
                    return "Dangerously Loud";
                default:
                    throw new ArgumentOutOfRangeException(nameof(volume), volume, null);
            }
        }
    }

    public enum AudioVolume {
        Silent = 0,
        Faint = 1,
        Quiet = 2,
        Decent = 3,
        Loud = 4,
        VeryLoud = 5,
        ExtremelyLoud = 6,
        DangerouslyLoud = 7
    }

    public enum AudioQuality {
        Intermittent = 0,
        Garbled = 1,
        Mechanical = 2,
        Feedback = 3,
        Silent = 4,
        Tinny = 5,
        Decent = 6,
        Clear = 7,
        Crystal = 8
    }

    public enum AudioStyle {
        Whisper = 0,
        Say = 1,
        Tell = 2,
        ToldOther = 3,
        Shout = 4,
        Radio = 5,
        Phone = 6,
        RoomEcho = 7,
        CellEcho = 8,
        PlaneEcho = 9,
        Disembodied = 10,
        Atmospheric = 11,
        Direct = 12
    }
}