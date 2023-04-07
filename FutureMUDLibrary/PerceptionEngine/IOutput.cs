using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Character;
using MudSharp.Framework;

namespace MudSharp.PerceptionEngine
{
    [Flags]
    public enum OutputFlags {
        /// <summary>
        ///     No OutputFlags, handle as per default case
        /// </summary>
        Normal = 0,

        NoLanguage = 1 << 0,

        /// <summary>
        ///     Send only those those who have the AdminSight effect.
        /// </summary>
        WizOnly = 1 << 1,

        /// <summary>
        ///     Send only to those who CanSee the origin, as opposed to sending obscured tokens
        /// </summary>
        SuppressObscured = 1 << 2,

        /// <summary>
        ///     Do not send the message to the source
        /// </summary>
        SuppressSource = 1 << 3,
        ElectronicOnly = 1 << 4,

        /// <summary>
        ///     Target must pass a Notice Check against the origin to receive output
        /// </summary>
        NoticeCheckRequired = 1 << 5,

        /// <summary>
        ///     Output is purely audible information and so can only be noticed by those who can hear it
        /// </summary>
        PurelyAudible = 1 << 6,

        /// <summary>
        ///  NO LONGER IN USE - Default Behaviour - Wrap this message at the potentially shorter "inner wrap" length rather than the standard wrap
        /// </summary>
        InnerWrap = 1 << 7,

        /// <summary>
        /// This message is relatively insignificant, just for flavour, and should be ignored by those with combatbrief enabled.
        /// </summary>
        Insigificant = 1 << 8,

        /// <summary>
        /// This message is sent to a wide variety of locations, and that should be the way that it is disseminated to watching receivers rather than via spying effects
        /// </summary>
        IgnoreWatchers = 1 << 9,

        /// <summary>
        /// Wrap this message at the longer "full line width"
        /// </summary>
        WideWrap = 1 << 10,

        /// <summary>
        /// Output is purely visual and so should only be noticed by those who can see
        /// </summary>
        PurelyVisual = 1 << 11
    }

    public enum OutputRange {
        /// <summary>
        ///     Just this Perceiver.
        /// </summary>
        Personal,

        /// <summary>
        ///     Every Perceiver in the current Location.
        /// </summary>
        Local,

        /// <summary>
        ///     Every Perceiver in the current Location's Room
        /// </summary>
        Room,

        /// <summary>
        ///     Every Perceiver in a room adjacent to the origin
        /// </summary>
        Surrounds,

        /// <summary>
        ///     Every Perceiver in the current Location's Zone
        /// </summary>
        Zone,

        /// <summary>
        ///     Every Perceiver in the current Location's Shard.
        /// </summary>
        Shard,

        /// <summary>
        ///     Every perceiver in the Game.
        /// </summary>
        All,

        /// <summary>
        ///     Every Player (not every Perceiver) in the Game.
        /// </summary>
        Game
    }

    public enum OutputVisibility {
        Normal,
        OOC,
        Guest
    }

    public enum OutputStyle {
        Normal,
        Important,
        AdminImportant,
        Reminder,
        CombatMessage,
        Hint,
        NoNewLine,
        NoPage,
        Explosion,
        TextFragment, // Not designed to be fullstopped/capitaled etc
        IgnoreLiquidsAndFlags
    }

    public interface IOutput
    {
        public OutputVisibility Visibility { get; set; }

        public OutputStyle Style { get; set; }

        public OutputFlags Flags { get; set; }

        public string RawString { get; }

        public string ParseFor(IPerceiver perceiver);

        public bool ShouldSee(IPerceiver perceiver);
    }
}
